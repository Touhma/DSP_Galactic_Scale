using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    /// <summary>
    /// Intermittent, night-side polar auroras (dual layered Auroral Ovals at ~65-75 deg N/S latitude).
    /// Features real-time flowing ribbon vertex motion, drifting vertical ray UVs, breathing curtain heights,
    /// and slow-evolving storm-cycle shape morphing so every aurora episode moves organically and looks unique.
    /// </summary>
    public static class PlanetaryAuroraSystem
    {
        private const string AuroraObjectName = "GS2_PlanetaryAurora";
        private const int Segments = 64;
        private const int LayerCount = 2; // Primary bright curtain + softer offset companion veil
        private const int TextureWidth = 256;
        private const int TextureHeight = 32;

        private sealed class AuroraState : MonoBehaviour
        {
            public Mesh Mesh;
            public Vector3[] Verts;
            public Vector2[] UVs;
            public Color32[] Colors;
            public float LowRadius;
            public float HighRadius;
            public Color BaseLowColor;
            public Color BaseHighColor;
            public Color StormTopColor;
            public float SeedPhase;
            public float WindBoost;
            public float LastUpdateTime;
        }

        private static Material _sharedMaterial;

        public static void AttachToPlanetSimulator(PlanetSimulator simulator, PlanetData planet)
        {
            if (simulator == null || planet == null) return;
            try
            {
                var existing = simulator.transform.Find(AuroraObjectName);
                if (existing != null) UnityEngine.Object.Destroy(existing.gameObject);

                if (!GS2.Config.PlanetaryAuroras) return;
                if (!ShouldHaveAurora(planet)) return;

                var mat = GetOrCreateMaterial();
                if (mat == null) return;

                var r = Mathf.Max(20f, planet.realRadius);
                var isGas = planet.type == EPlanetType.Gas;
                // Hug the upper atmospheric limb (realistic ISS proportion from space while staying above trees/clouds on surface)
                var lowAlt = isGas ? r * 1.022f : r + Mathf.Clamp(r * 0.036f, 12f, 22f);
                var highAlt = isGas ? r * 1.072f : r + Mathf.Clamp(r * 0.104f, 36f, 58f);

                var go = new GameObject(AuroraObjectName)
                {
                    layer = simulator.gameObject.layer
                };
                go.transform.SetParent(simulator.transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var mesh = BuildDualLayeredCurtainsMesh(lowAlt, highAlt, out var verts, out var uvs, out var colors);
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.lightProbeUsage = LightProbeUsage.Off;
                mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
                mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                var state = go.AddComponent<AuroraState>();
                state.Mesh = mesh;
                state.Verts = verts;
                state.UVs = uvs;
                state.Colors = colors;
                state.LowRadius = lowAlt;
                state.HighRadius = highAlt;
                state.SeedPhase = (Math.Abs(planet.seed) % 997) * 0.618f;
                state.WindBoost = ComputeSolarWindBoost(planet);
                state.LastUpdateTime = -999f;

                if (isGas)
                {
                    // Jupiter/Saturn-like ultraviolet/cyan-violet polar ovals
                    state.BaseLowColor = new Color(0.28f, 0.78f, 1.00f);
                    state.BaseHighColor = new Color(0.58f, 0.42f, 1.00f);
                    state.StormTopColor = new Color(0.86f, 0.36f, 0.96f);
                }
                else
                {
                    // Earth-like oxygen green lower curtain -> cyan mid -> rare violet/magenta storm fringe
                    state.BaseLowColor = new Color(0.16f, 0.96f, 0.56f);
                    state.BaseHighColor = new Color(0.18f, 0.80f, 0.92f);
                    state.StormTopColor = new Color(0.76f, 0.24f, 0.88f);
                }
            }
            catch (Exception ex)
            {
                GS2.Warn($"PlanetaryAuroraSystem.AttachToPlanetSimulator failed for {planet?.name}: {ex.Message}");
            }
        }

        private static float SmoothStep01(float edge0, float edge1, float x)
        {
            var t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        public static void SyncLayer(PlanetSimulator simulator)
        {
            if (simulator == null) return;
            var aurora = simulator.transform.Find(AuroraObjectName);
            if (aurora != null) aurora.gameObject.layer = 0;
        }

        public static void UpdateAurora(PlanetSimulator simulator)
        {
            if (simulator == null || simulator.planetData == null || simulator.planetData.star == null) return;
            var trans = simulator.transform.Find(AuroraObjectName);
            if (trans == null) return;

            var starmapOpen = UIRoot.instance != null && UIRoot.instance.uiGame != null && UIRoot.instance.uiGame.starmap != null && UIRoot.instance.uiGame.starmap.active;
            if (!GS2.Config.PlanetaryAuroras || starmapOpen)
            {
                if (trans.gameObject.activeSelf) trans.gameObject.SetActive(false);
                return;
            }
            if (!trans.gameObject.activeSelf) trans.gameObject.SetActive(true);
            if (trans.gameObject.layer != 0) trans.gameObject.layer = 0;

            var state = trans.GetComponent<AuroraState>();
            if (state == null || state.Mesh == null) return;

            var now = Time.unscaledTime;
            if (now - state.LastUpdateTime < 0.04f) return; // ~25 Hz butter-smooth ethereal morphing
            state.LastUpdateTime = now;

            var planet = simulator.planetData;
            var gameSec = GameMain.gameTick / 60.0;

            // Multi-period quasi-periodic geomagnetic storm envelope (not always on!).
            var phase = state.SeedPhase;
            var wave1 = Mathf.Sin((float)(gameSec * 0.0052) + phase);          // ~20 min period
            var wave2 = Mathf.Sin((float)(gameSec * 0.0087) + phase * 1.7f);   // ~12 min period
            var wave3 = Mathf.Sin((float)(gameSec * 0.0029) + phase * 2.3f);   // ~36 min period
            var rawActivity = 0.48f * wave1 + 0.34f * wave2 + 0.18f * wave3;   // [-1, 1]
            var threshold = Mathf.Clamp(0.32f - (state.WindBoost - 1f) * 0.18f, 0.05f, 0.45f);
            var stormStrength = Mathf.Clamp01((rawActivity - threshold) / (1f - threshold));

            if (planet.id == GSSettings.BirthPlanetId && gameSec < 210.0)
            {
                var introFade = 1f - SmoothStep01(150f, 210f, (float)gameSec);
                stormStrength = Mathf.Max(stormStrength, 0.72f * introFade);
            }

            var mr = trans.GetComponent<MeshRenderer>();
            if (stormStrength < 0.01f)
            {
                if (mr != null && mr.enabled) mr.enabled = false;
                return;
            }
            if (mr != null && !mr.enabled) mr.enabled = true;

            var sunDirWorld = (Vector3)(planet.star.uPosition - planet.uPosition).normalized;
            var sunDirLocal = (Quaternion.Inverse(planet.runtimeRotation) * sunDirWorld).normalized;

            var t = (float)gameSec;
            var strongStorm = Mathf.Clamp01((stormStrength - 0.48f) / 0.52f);
            var isGas = planet.type == EPlanetType.Gas;

            // Slow storm-cycle color mood harmonics so different aurora episodes naturally shift palette:
            // emerald-green -> cyan-teal -> rose/crimson-topped oxygen -> rare magenta-violet geomagnetic storm
            var cyanShift = 0.5f + 0.5f * Mathf.Sin(t * 0.0021f + phase * 1.9f);
            var crimsonMood = SmoothStep01(0.30f, 0.92f, 0.5f + 0.5f * Mathf.Sin(t * 0.0013f + phase * 2.7f));
            var topCrimsonWeight = Mathf.Clamp01(strongStorm * 0.85f + crimsonMood * 0.42f);
            var rarePinkBase = SmoothStep01(0.68f, 0.96f, 0.5f + 0.5f * Mathf.Cos(t * 0.0017f + phase * 0.9f)) * stormStrength;

            var altLowColor = isGas ? new Color(0.20f, 0.92f, 0.88f) : new Color(0.12f, 0.88f, 0.78f);
            var pinkFringeColor = isGas ? new Color(0.72f, 0.34f, 0.98f) : new Color(0.84f, 0.30f, 0.68f);
            var crimsonTopColor = isGas ? new Color(0.92f, 0.30f, 0.86f) : new Color(0.88f, 0.22f, 0.46f);

            // Slow incommensurate standing-wave amplitudes so curtains genuinely MORPH in place
            // (lobes swell, fold, and dissolve organically instead of rigidly sliding sideways)
            var amp2Sin = 2.8f * Mathf.Sin(t * 0.019f + phase * 1.1f);
            var amp2Cos = 2.4f * Mathf.Cos(t * 0.014f + phase * 1.7f);
            var amp3Sin = 2.1f * Mathf.Sin(t * 0.027f + phase * 2.3f);
            var amp3Cos = 1.9f * Mathf.Cos(t * 0.022f + phase * 0.7f);
            var amp5Sin = 1.15f * Mathf.Sin(t * 0.034f + phase * 3.1f);
            var equatorExpansion = strongStorm * 3.0f;

            var verts = state.Verts;
            var uvs = state.UVs;
            var colors = state.Colors;
            var lowBase = state.LowRadius;
            var heightSpan = state.HighRadius - state.LowRadius;
            var stripVerts = (Segments + 1) * 2;

            for (var pole = 0; pole < 2; pole++)
            {
                var sign = pole == 0 ? 1f : -1f;
                var polePhase = pole * 1.9f + phase;

                for (var layer = 0; layer < LayerCount; layer++)
                {
                    var isCompanion = layer == 1;
                    var baseLat = (isCompanion ? 72.2f : 68.4f) - equatorExpansion * (isCompanion ? 0.65f : 1.0f);
                    var layerAlphaMul = isCompanion ? 0.56f : 1.0f;
                    // Ultra-gentle ethereal ray drift (8x slower than before, no conveyor-belt sliding)
                    var uvDrift = isCompanion ? (-t * 0.0032f) : (t * 0.0045f);
                    var layerPhase = polePhase + layer * 2.4f;
                    var layerSign = isCompanion ? -0.85f : 1.0f;

                    var stripIndex = pole * LayerCount + layer;
                    var baseFront = stripIndex * 2 * stripVerts;
                    var baseBack = baseFront + stripVerts;

                    for (var i = 0; i <= Segments; i++)
                    {
                        var u = i / (float)Segments;
                        var ang = u * Mathf.PI * 2f;

                        // Standing-wave morphing: independent spatial modes wax and wane slowly
                        var a = ang + layerPhase;
                        var latOffset = layerSign * (
                            amp2Sin * Mathf.Sin(a * 2f) + amp2Cos * Mathf.Cos(a * 2f)
                            + amp3Sin * Mathf.Sin(a * 3f + 0.6f) + amp3Cos * Mathf.Cos(a * 3f - 0.4f)
                            + amp5Sin * Mathf.Sin(a * 5f + t * 0.012f));
                        var latDeg = Mathf.Clamp(baseLat + latOffset, 56f, 82f);

                        var latRad = latDeg * Mathf.Deg2Rad * sign;
                        var cosLat = Mathf.Cos(latRad);
                        var sinLat = Mathf.Sin(latRad);
                        var dir = new Vector3(cosLat * Mathf.Cos(ang), sinLat, cosLat * Mathf.Sin(ang));

                        // Slow vertical breathing as localized curtain patches brighten & stretch upward
                        var heightBreath = 0.80f
                            + 0.18f * Mathf.Sin(a * 3f) * Mathf.Sin(t * 0.045f + layer * 1.5f)
                            + 0.14f * Mathf.Cos(a * 5f) * Mathf.Cos(t * 0.062f + layerPhase);
                        var lowR = lowBase + (isCompanion ? heightSpan * 0.08f : 0f);
                        var highR = lowR + heightSpan * heightBreath * (1f + strongStorm * 0.22f);

                        var lowPos = dir * lowR;
                        var highPos = dir * highR;

                        // Night-side / deep twilight visibility mask
                        var sunDot = Vector3.Dot(dir, sunDirLocal);
                        var nightMask = 1f - SmoothStep01(-0.18f, 0.08f, sunDot);

                        // Localized slow blooming & fading brightness patches (standing waves, not fast scrolling)
                        var bloom1 = Mathf.Sin(a * 3f + 0.5f) * Mathf.Sin(t * 0.11f + layerPhase);
                        var bloom2 = Mathf.Cos(a * 7f - 0.8f) * Mathf.Cos(t * 0.17f - layerPhase * 0.6f);
                        var rippleLow = Mathf.Clamp01(0.56f + 0.26f * bloom1 + 0.18f * bloom2);
                        var rippleHigh = Mathf.Clamp01(0.54f + 0.24f * bloom1 + 0.20f * Mathf.Cos(a * 7f + 0.9f) * Mathf.Sin(t * 0.15f + layerPhase));

                        var alphaLow = stormStrength * nightMask * rippleLow * 0.90f * layerAlphaMul;
                        var alphaHigh = stormStrength * nightMask * rippleHigh * 0.74f * layerAlphaMul;

                        // Localized palette variation along the curtain (green/cyan base + occasional pink N2 edge + rose/violet O(630nm) crown)
                        var localPatch = 0.5f + 0.5f * Mathf.Sin(a * 2f + t * 0.015f + layerPhase);
                        var localCrimson = SmoothStep01(0.20f, 0.90f, 0.5f + 0.5f * Mathf.Cos(a * 3f - t * 0.011f + phase));

                        var colLow = Color.Lerp(state.BaseLowColor, altLowColor, cyanShift * 0.55f * localPatch);
                        if (rarePinkBase > 0.02f)
                            colLow = Color.Lerp(colLow, pinkFringeColor, rarePinkBase * 0.45f * localCrimson);

                        var colHigh = Color.Lerp(state.BaseHighColor, state.StormTopColor, topCrimsonWeight * (0.45f + 0.55f * localPatch));
                        if (topCrimsonWeight > 0.25f)
                            colHigh = Color.Lerp(colHigh, crimsonTopColor, (topCrimsonWeight - 0.25f) * 0.80f * localCrimson);
                        if (isCompanion && crimsonMood > 0.45f)
                            colHigh = Color.Lerp(colHigh, state.StormTopColor, (crimsonMood - 0.45f) * 0.70f);

                        var c32Low = new Color32(
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colLow.r * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colLow.g * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colLow.b * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(alphaLow * 255f), 0, 255));
                        var c32High = new Color32(
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colHigh.r * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colHigh.g * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(colHigh.b * 255f), 0, 255),
                            (byte)Mathf.Clamp(Mathf.RoundToInt(alphaHigh * 255f), 0, 255));

                        var uvU = u * 4f + uvDrift;
                        var uv0 = new Vector2(uvU, 0f);
                        var uv1 = new Vector2(uvU, 1f);

                        var f0 = baseFront + i * 2;
                        verts[f0] = lowPos; verts[f0 + 1] = highPos;
                        uvs[f0] = uv0; uvs[f0 + 1] = uv1;
                        colors[f0] = c32Low; colors[f0 + 1] = c32High;

                        var b0 = baseBack + i * 2;
                        verts[b0] = lowPos; verts[b0 + 1] = highPos;
                        uvs[b0] = uv0; uvs[b0 + 1] = uv1;
                        colors[b0] = c32Low; colors[b0 + 1] = c32High;
                    }
                }
            }

            state.Mesh.vertices = verts;
            state.Mesh.uv = uvs;
            state.Mesh.colors32 = colors;
        }

        private static bool ShouldHaveAurora(PlanetData planet)
        {
            if (planet == null) return false;
            if (planet.type == EPlanetType.Gas) return true;
            // Require atmosphere so airless moons, barren rocks, and comets don't spawn auroras
            if (planet.atmosMaterial == null) return false;
            // Venus and Mars lack a strong global dipole magnetic field -> no polar auroral ovals
            var name = (planet.name ?? string.Empty).Trim();
            if (string.Equals(name, "Venus", StringComparison.OrdinalIgnoreCase) || name == "金星" ||
                string.Equals(name, "Mars", StringComparison.OrdinalIgnoreCase) || name == "火星")
                return false;
            var theme = GS2.GetGSPlanet(planet)?.Theme ?? string.Empty;
            if (theme.StartsWith("Comet", StringComparison.OrdinalIgnoreCase) ||
                theme.IndexOf("Barren", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            return true;
        }

        private static float ComputeSolarWindBoost(PlanetData planet)
        {
            var boost = 1f;
            if (planet?.star != null)
            {
                if (planet.star.spectr == ESpectrType.M || planet.star.spectr == ESpectrType.O || planet.star.spectr == ESpectrType.B)
                    boost *= 1.35f;
                var orbit = Mathf.Max(0.3f, planet.orbitRadius);
                boost *= Mathf.Clamp(1.1f / Mathf.Sqrt(orbit), 0.6f, 1.5f);
            }
            return boost;
        }

        private static Mesh BuildDualLayeredCurtainsMesh(
            float lowRadius,
            float highRadius,
            out Vector3[] verts,
            out Vector2[] uvs,
            out Color32[] colors)
        {
            // 2 poles * LayerCount * 2 faces (front + back) * (Segments + 1) * 2 (bottom + top)
            var stripVerts = (Segments + 1) * 2;
            var totalStrips = 2 * LayerCount;
            var totalVerts = totalStrips * 2 * stripVerts;
            verts = new Vector3[totalVerts];
            uvs = new Vector2[totalVerts];
            colors = new Color32[totalVerts];
            var tris = new int[totalStrips * Segments * 12];

            var triIdx = 0;
            for (var s = 0; s < totalStrips; s++)
            {
                var pole = s / LayerCount;
                var sign = pole == 0 ? 1f : -1f;
                var baseFront = s * 2 * stripVerts;
                var baseBack = baseFront + stripVerts;

                for (var i = 0; i <= Segments; i++)
                {
                    var u = i / (float)Segments;
                    var ang = u * Mathf.PI * 2f;
                    var latRad = 69f * Mathf.Deg2Rad * sign;
                    var dir = new Vector3(Mathf.Cos(latRad) * Mathf.Cos(ang), Mathf.Sin(latRad), Mathf.Cos(latRad) * Mathf.Sin(ang));

                    var lowPos = dir * lowRadius;
                    var highPos = dir * highRadius;

                    var f0 = baseFront + i * 2;
                    verts[f0] = lowPos; verts[f0 + 1] = highPos;
                    uvs[f0] = new Vector2(u * 4f, 0f); uvs[f0 + 1] = new Vector2(u * 4f, 1f);

                    var b0 = baseBack + i * 2;
                    verts[b0] = lowPos; verts[b0 + 1] = highPos;
                    uvs[b0] = new Vector2(u * 4f, 0f); uvs[b0 + 1] = new Vector2(u * 4f, 1f);
                }

                for (var i = 0; i < Segments; i++)
                {
                    var fa = baseFront + i * 2;
                    var fb = fa + 1;
                    var fc = fa + 2;
                    var fd = fa + 3;
                    tris[triIdx++] = fa; tris[triIdx++] = fb; tris[triIdx++] = fc;
                    tris[triIdx++] = fb; tris[triIdx++] = fd; tris[triIdx++] = fc;

                    var ba = baseBack + i * 2;
                    var bb = ba + 1;
                    var bc = ba + 2;
                    var bd = ba + 3;
                    tris[triIdx++] = ba; tris[triIdx++] = bc; tris[triIdx++] = bb;
                    tris[triIdx++] = bb; tris[triIdx++] = bc; tris[triIdx++] = bd;
                }
            }

            var mesh = new Mesh
            {
                name = "GS2_PlanetaryAuroraMesh",
                vertices = verts,
                uv = uvs,
                colors32 = colors,
                triangles = tris
            };
            mesh.MarkDynamic();
            // Generous fixed bounds covering max equatorward expansion & height breathing so it is never frustum-culled
            var maxExtent = highRadius * 1.35f;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * (maxExtent * 2f));
            return mesh;
        }

        private static Material GetOrCreateMaterial()
        {
            if (_sharedMaterial != null) return _sharedMaterial;
            var shader = Shader.Find("Sprites/Default")
                ?? Shader.Find("UI/Default")
                ?? Shader.Find("Unlit/Transparent");
            if (shader == null) return null;

            var tex = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGBA32, false)
            {
                wrapModeU = TextureWrapMode.Repeat,
                wrapModeV = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color[TextureWidth * TextureHeight];
            for (var y = 0; y < TextureHeight; y++)
            {
                var v = y / (float)(TextureHeight - 1); // 0 = bottom sharp edge, 1 = top soft fade
                var verticalProfile = SmoothStep01(0f, 0.22f, v) * (1f - SmoothStep01(0.35f, 1f, v));
                for (var x = 0; x < TextureWidth; x++)
                {
                    var u = x / (float)TextureWidth * Mathf.PI * 2f;
                    var rays = 0.55f + 0.25f * Mathf.Sin(u * 13f) + 0.20f * Mathf.Sin(u * 29f + 1.1f);
                    var a = Mathf.Clamp01(verticalProfile * rays);
                    pixels[y * TextureWidth + x] = new Color(1f, 1f, 1f, a);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply(false, true);

            _sharedMaterial = new Material(shader)
            {
                name = "GS2_AuroraMat",
                mainTexture = tex,
                renderQueue = 3005
            };
            if (_sharedMaterial.HasProperty("_Color")) _sharedMaterial.SetColor("_Color", Color.white);
            return _sharedMaterial;
        }
    }
}
