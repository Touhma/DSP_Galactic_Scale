using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    public static class PlanetaryRingSystem
    {
        private const string RingObjectName = "GS2_PlanetaryRing";
        private const int RingSegments = 128;
        private const int TextureWidth = 512;

        private enum RingPreset
        {
            None,
            Saturn,
            Uranus,
            ExoGolden,
            ExoIce,
            ExoAmber,
            ExoSilver
        }

        private struct RingSpec
        {
            public RingPreset Preset;
            public float InnerRatio;
            public float OuterRatio;
            public int Seed;
        }

        private sealed class RingShadowState : MonoBehaviour
        {
            public Mesh Mesh;
            public Vector3[] LocalVertices;
            public Color32[] Colors;
            public float PlanetRadius;
            public Vector3 LastSunDirLocal;
        }

        private static readonly Dictionary<int, Material> MaterialCache = new();
        private static Shader _cachedShader;

        private static bool TryGetRingSpec(PlanetData planet, out RingSpec spec)
        {
            spec = default;
            if (planet == null || planet.type != EPlanetType.Gas) return false;
            var ringMode = GS2.Config.PlanetaryRingsMode;
            if (string.Equals(ringMode, "Off", StringComparison.OrdinalIgnoreCase)) return false;

            var name = (planet.name ?? string.Empty).Trim();
            if (string.Equals(name, "Saturn", StringComparison.OrdinalIgnoreCase) || name == "土星")
            {
                spec = new RingSpec
                {
                    Preset = RingPreset.Saturn,
                    InnerRatio = 1.24f,
                    OuterRatio = ClampOuterToInnerMoons(planet, 2.24f),
                    Seed = 777
                };
                return spec.OuterRatio > spec.InnerRatio + 0.15f;
            }

            if (string.Equals(name, "Uranus", StringComparison.OrdinalIgnoreCase) || name == "天王星")
            {
                spec = new RingSpec
                {
                    Preset = RingPreset.Uranus,
                    InnerRatio = 1.44f,
                    OuterRatio = ClampOuterToInnerMoons(planet, 1.88f),
                    Seed = 999
                };
                return spec.OuterRatio > spec.InnerRatio + 0.12f;
            }

            if (string.Equals(ringMode, "Sol Only", StringComparison.OrdinalIgnoreCase))
                return false;

            // Skip other Solar System gas/ice giants (Jupiter / Neptune) so Sol & NiceSol stay true to life
            if (string.Equals(name, "Jupiter", StringComparison.OrdinalIgnoreCase) || name == "木星" ||
                string.Equals(name, "Neptune", StringComparison.OrdinalIgnoreCase) || name == "海王星")
                return false;

            // Deterministic ~22% chance for exoplanet gas/ice giants based on planet seed
            var hash = unchecked((uint)planet.seed * 2654435761u ^ (uint)planet.id * 2246822519u);
            hash ^= hash >> 13;
            hash *= 3266489917u;
            hash ^= hash >> 16;
            if ((hash % 100u) >= 22u) return false;

            var variant = (int)((hash >> 8) % 4u);
            var themeName = GS2.GetGSPlanet(planet)?.Theme ?? string.Empty;
            RingPreset preset;
            if (themeName.IndexOf("Ice", StringComparison.OrdinalIgnoreCase) >= 0)
                preset = (variant % 2 == 0) ? RingPreset.ExoIce : RingPreset.ExoSilver;
            else if (themeName.IndexOf("Hot", StringComparison.OrdinalIgnoreCase) >= 0)
                preset = RingPreset.ExoAmber;
            else
                preset = variant switch
                {
                    0 => RingPreset.ExoGolden,
                    1 => RingPreset.ExoAmber,
                    2 => RingPreset.ExoSilver,
                    _ => RingPreset.ExoIce
                };

            var inner = 1.24f + ((hash >> 12) % 24u) * 0.01f; // 1.24..1.47 R
            var width = 0.45f + ((hash >> 18) % 55u) * 0.01f; // 0.45..0.99 R
            var outer = ClampOuterToInnerMoons(planet, inner + width);
            if (outer <= inner + 0.15f) return false;

            spec = new RingSpec
            {
                Preset = preset,
                InnerRatio = inner,
                OuterRatio = outer,
                Seed = (int)(hash & 0x7FFFFFFFu)
            };
            return true;
        }

        private static float ClampOuterToInnerMoons(PlanetData planet, float desiredOuterRatio)
        {
            if (planet?.star?.planets == null || planet.realRadius <= 1f) return desiredOuterRatio;

            var maxSafeRatio = desiredOuterRatio;
            var planets = planet.star.planets;
            for (var i = 0; i < planets.Length; i++)
            {
                var other = planets[i];
                if (other == null || other.orbitAroundPlanet != planet) continue;
                var moonOrbitMeters = other.orbitRadius * 40000f;
                var safeMeters = moonOrbitMeters - other.realRadius * 1.6f - 400f;
                if (safeMeters > 0f)
                {
                    var ratio = safeMeters / planet.realRadius;
                    if (ratio < maxSafeRatio) maxSafeRatio = ratio;
                }
            }

            return maxSafeRatio;
        }

        public static void AttachToPlanetSimulator(PlanetSimulator simulator, PlanetData planet)
        {
            if (simulator == null || planet == null) return;
            try
            {
                RemoveExistingRing(simulator.transform);
                if (!TryGetRingSpec(planet, out var spec)) return;

                var mat = GetOrCreateMaterial(spec);
                if (mat == null) return;

                var r = Mathf.Max(100f, planet.realRadius);
                var go = new GameObject(RingObjectName)
                {
                    layer = simulator.gameObject.layer
                };
                go.transform.SetParent(simulator.transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var mesh = BuildDoubleSidedRingMesh(r * spec.InnerRatio, r * spec.OuterRatio, out var verts, out var colors);
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.lightProbeUsage = LightProbeUsage.Off;
                mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
                mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                var state = go.AddComponent<RingShadowState>();
                state.Mesh = mesh;
                state.LocalVertices = verts;
                state.Colors = colors;
                state.PlanetRadius = r;
                state.LastSunDirLocal = Vector3.zero;
            }
            catch (Exception ex)
            {
                GS2.Warn($"PlanetaryRingSystem.AttachToPlanetSimulator failed for {planet?.name}: {ex.Message}");
            }
        }

        public static void SyncLayer(PlanetSimulator simulator)
        {
            if (simulator == null) return;
            var ring = simulator.transform.Find(RingObjectName);
            if (ring != null) ring.gameObject.layer = 0;
        }

        public static void UpdateShadow(PlanetSimulator simulator)
        {
            if (simulator == null || simulator.planetData == null || simulator.planetData.star == null) return;
            var ringTrans = simulator.transform.Find(RingObjectName);
            if (ringTrans == null) return;

            var state = ringTrans.GetComponent<RingShadowState>();
            if (state == null || state.Mesh == null || state.LocalVertices == null) return;

            var planet = simulator.planetData;
            var sunDirWorld = (Vector3)(planet.star.uPosition - planet.uPosition).normalized;
            if (sunDirWorld.sqrMagnitude < 0.001f) return;

            var sunDirLocal = (Quaternion.Inverse(planet.runtimeRotation) * sunDirWorld).normalized;
            if ((sunDirLocal - state.LastSunDirLocal).sqrMagnitude < 0.0002f) return;
            state.LastSunDirLocal = sunDirLocal;

            var verts = state.LocalVertices;
            var colors = state.Colors;
            var radius = state.PlanetRadius;
            var innerShadowR = radius * 0.96f;
            var outerShadowR = radius * 1.04f;
            var invPenumbra = 1f / Mathf.Max(0.01f, outerShadowR - innerShadowR);

            for (var i = 0; i < verts.Length; i++)
            {
                var p = verts[i];
                var alongSun = Vector3.Dot(p, sunDirLocal);
                var shade = 1f;
                if (alongSun < 0f)
                {
                    var perp = p - sunDirLocal * alongSun;
                    var distFromAxis = perp.magnitude;
                    if (distFromAxis < outerShadowR)
                    {
                        var t = Mathf.Clamp01((distFromAxis - innerShadowR) * invPenumbra);
                        shade = Mathf.Lerp(0.14f, 1f, t);
                    }
                }

                var c = (byte)Mathf.RoundToInt(shade * 255f);
                colors[i] = new Color32(c, c, c, 255);
            }

            state.Mesh.colors32 = colors;
        }

        private sealed class StarmapRingTag : MonoBehaviour
        {
            public int PlanetId;
        }

        public static void SyncStarmapPlanet(UIStarmapPlanet starmapPlanet)
        {
            if (starmapPlanet == null || starmapPlanet.relatedObject == null || starmapPlanet.planet == null) return;
            AttachToStarmapPlanet(starmapPlanet.relatedObject, starmapPlanet.planet, starmapPlanet.planetRenderer);
        }

        public static void AttachToStarmapPlanet(GameObject relatedObject, PlanetData planet, Renderer planetRenderer = null)
        {
            if (relatedObject == null || planet == null) return;
            try
            {
                var host = planetRenderer != null ? planetRenderer.transform : relatedObject.transform;
                if (host != relatedObject.transform)
                    RemoveExistingRing(relatedObject.transform);

                var existing = host.Find(RingObjectName);
                if (existing != null)
                {
                    var tag = existing.GetComponent<StarmapRingTag>();
                    if (tag != null && tag.PlanetId == planet.id && TryGetRingSpec(planet, out _))
                        return;
                    UnityEngine.Object.Destroy(existing.gameObject);
                }

                if (!TryGetRingSpec(planet, out var spec)) return;

                var mat = GetOrCreateMaterial(spec);
                if (mat == null) return;

                var sphereRadius = 0.5f;
                var mfHost = host.GetComponent<MeshFilter>() ?? host.GetComponentInChildren<MeshFilter>();
                if (mfHost != null && mfHost.sharedMesh != null)
                {
                    var ext = mfHost.sharedMesh.bounds.extents.x;
                    if (ext > 0.01f) sphereRadius = ext;
                }

                var go = new GameObject(RingObjectName)
                {
                    layer = host.gameObject.layer
                };
                go.transform.SetParent(host, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.AddComponent<StarmapRingTag>().PlanetId = planet.id;

                var mesh = BuildDoubleSidedRingMesh(sphereRadius * spec.InnerRatio, sphereRadius * spec.OuterRatio, out _, out _);
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.lightProbeUsage = LightProbeUsage.Off;
                mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
                mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            }
            catch (Exception ex)
            {
                GS2.Warn($"PlanetaryRingSystem.AttachToStarmapPlanet failed for {planet?.name}: {ex.Message}");
            }
        }

        private static void RemoveExistingRing(Transform parent)
        {
            var existing = parent.Find(RingObjectName);
            if (existing != null) UnityEngine.Object.Destroy(existing.gameObject);
        }

        private static Mesh BuildDoubleSidedRingMesh(float innerRadius, float outerRadius, out Vector3[] vertices, out Color32[] colors)
        {
            var ringVertCount = (RingSegments + 1) * 2;
            var totalVerts = ringVertCount * 2; // top face + bottom face
            vertices = new Vector3[totalVerts];
            var normals = new Vector3[totalVerts];
            var uvs = new Vector2[totalVerts];
            colors = new Color32[totalVerts];
            var triangles = new int[RingSegments * 12];

            var white = new Color32(255, 255, 255, 255);
            for (var i = 0; i <= RingSegments; i++)
            {
                var angle = (i / (float)RingSegments) * Mathf.PI * 2f;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                var innerPos = new Vector3(cos * innerRadius, 0f, sin * innerRadius);
                var outerPos = new Vector3(cos * outerRadius, 0f, sin * outerRadius);

                var topIdx = i * 2;
                vertices[topIdx] = innerPos;
                vertices[topIdx + 1] = outerPos;
                normals[topIdx] = Vector3.up;
                normals[topIdx + 1] = Vector3.up;
                uvs[topIdx] = new Vector2(0f, 0.5f);
                uvs[topIdx + 1] = new Vector2(1f, 0.5f);
                colors[topIdx] = white;
                colors[topIdx + 1] = white;

                var botIdx = ringVertCount + i * 2;
                vertices[botIdx] = innerPos;
                vertices[botIdx + 1] = outerPos;
                normals[botIdx] = Vector3.down;
                normals[botIdx + 1] = Vector3.down;
                uvs[botIdx] = new Vector2(0f, 0.5f);
                uvs[botIdx + 1] = new Vector2(1f, 0.5f);
                colors[botIdx] = white;
                colors[botIdx + 1] = white;
            }

            var t = 0;
            for (var i = 0; i < RingSegments; i++)
            {
                var a = i * 2;
                var b = a + 1;
                var c = a + 2;
                var d = a + 3;

                // Top face (visible from +Y)
                triangles[t++] = a;
                triangles[t++] = c;
                triangles[t++] = b;
                triangles[t++] = b;
                triangles[t++] = c;
                triangles[t++] = d;

                // Bottom face (visible from -Y)
                var ba = ringVertCount + a;
                var bb = ringVertCount + b;
                var bc = ringVertCount + c;
                var bd = ringVertCount + d;
                triangles[t++] = ba;
                triangles[t++] = bb;
                triangles[t++] = bc;
                triangles[t++] = bb;
                triangles[t++] = bd;
                triangles[t++] = bc;
            }

            var mesh = new Mesh
            {
                name = "GS2_PlanetaryRingMesh",
                vertices = vertices,
                normals = normals,
                uv = uvs,
                colors32 = colors,
                triangles = triangles
            };
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Material GetOrCreateMaterial(RingSpec spec)
        {
            var cacheKey = ((int)spec.Preset << 16) ^ (spec.Preset == RingPreset.Saturn || spec.Preset == RingPreset.Uranus ? 0 : (spec.Seed & 7));
            if (MaterialCache.TryGetValue(cacheKey, out var cached) && cached != null) return cached;

            if (_cachedShader == null)
            {
                _cachedShader = Shader.Find("Sprites/Default")
                    ?? Shader.Find("UI/Default")
                    ?? Shader.Find("Unlit/Transparent");
            }
            if (_cachedShader == null) return null;

            var tex = BuildRingTexture(spec.Preset, cacheKey);
            var mat = new Material(_cachedShader)
            {
                name = $"GS2_RingMat_{spec.Preset}_{cacheKey}",
                mainTexture = tex,
                // Render strictly before local planet daytime atmosphere (2991) & clouds so distant rings are naturally veiled by thick skies (e.g., Venus)
                renderQueue = 2988
            };
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", Color.white);
            MaterialCache[cacheKey] = mat;
            return mat;
        }

        private static float SmoothStep01(float edge0, float edge1, float x)
        {
            var t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static Texture2D BuildRingTexture(RingPreset preset, int seed)
        {
            var tex = new Texture2D(TextureWidth, 2, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color[TextureWidth * 2];

            var phase1 = (seed * 1.37f) % 6.283f;
            var phase2 = (seed * 2.91f) % 6.283f;

            for (var x = 0; x < TextureWidth; x++)
            {
                var u = x / (float)(TextureWidth - 1); // 0 = inner edge, 1 = outer edge
                Color col;
                float alpha;

                if (preset == RingPreset.Saturn)
                {
                    // Realistic Saturn radial profile kept below DSP HDR Bloom threshold so bands stay crisp up close:
                    // C ring (0.00-0.23), B ring (0.23-0.66), Cassini Division (0.66-0.74), A ring (0.74-0.98) + Encke Gap
                    var fine = 0.88f + 0.08f * Mathf.Sin(u * 190f) + 0.04f * Mathf.Sin(u * 430f);
                    if (u < 0.23f)
                    {
                        var t = u / 0.23f;
                        col = Color.Lerp(new Color(0.34f, 0.30f, 0.26f), new Color(0.46f, 0.42f, 0.35f), t);
                        alpha = Mathf.Lerp(0f, 0.30f, SmoothStep01(0f, 1f, t)) * fine;
                    }
                    else if (u < 0.66f)
                    {
                        var t = (u - 0.23f) / 0.43f;
                        col = Color.Lerp(new Color(0.56f, 0.50f, 0.40f), new Color(0.65f, 0.60f, 0.50f), Mathf.Sin(t * Mathf.PI));
                        alpha = (0.76f + 0.14f * Mathf.Sin(u * 55f)) * fine;
                    }
                    else if (u < 0.74f)
                    {
                        // Cassini Division
                        col = new Color(0.22f, 0.20f, 0.19f);
                        var edge = Mathf.Abs((u - 0.70f) / 0.04f);
                        alpha = Mathf.Lerp(0.04f, 0.22f, edge * edge);
                    }
                    else
                    {
                        var t = (u - 0.74f) / 0.26f;
                        col = Color.Lerp(new Color(0.55f, 0.50f, 0.43f), new Color(0.46f, 0.43f, 0.38f), t);
                        alpha = 0.62f * fine * (1f - SmoothStep01(0.94f, 1f, u));
                        // Encke Gap
                        if (u > 0.905f && u < 0.922f) alpha *= 0.12f;
                    }
                }
                else if (preset == RingPreset.Uranus)
                {
                    // Narrow, delicate ice-blue epsilon/alpha/beta rings
                    col = Color.Lerp(new Color(0.44f, 0.58f, 0.70f), new Color(0.58f, 0.68f, 0.76f), u);
                    var r1 = Mathf.Exp(-Mathf.Pow((u - 0.18f) / 0.04f, 2f)) * 0.28f;
                    var r2 = Mathf.Exp(-Mathf.Pow((u - 0.48f) / 0.05f, 2f)) * 0.34f;
                    var r3 = Mathf.Exp(-Mathf.Pow((u - 0.84f) / 0.06f, 2f)) * 0.48f;
                    alpha = Mathf.Clamp01(r1 + r2 + r3);
                }
                else
                {
                    var (cInner, cMid, cOuter) = preset switch
                    {
                        RingPreset.ExoIce => (new Color(0.44f, 0.58f, 0.70f), new Color(0.58f, 0.66f, 0.74f), new Color(0.38f, 0.52f, 0.64f)),
                        RingPreset.ExoAmber => (new Color(0.54f, 0.36f, 0.26f), new Color(0.66f, 0.50f, 0.36f), new Color(0.46f, 0.32f, 0.24f)),
                        RingPreset.ExoSilver => (new Color(0.48f, 0.48f, 0.56f), new Color(0.62f, 0.60f, 0.68f), new Color(0.42f, 0.44f, 0.52f)),
                        _ => (new Color(0.54f, 0.48f, 0.38f), new Color(0.64f, 0.60f, 0.50f), new Color(0.48f, 0.44f, 0.36f))
                    };
                    col = u < 0.5f ? Color.Lerp(cInner, cMid, u * 2f) : Color.Lerp(cMid, cOuter, (u - 0.5f) * 2f);
                    var bands = 0.55f + 0.22f * Mathf.Sin(u * 42f + phase1) + 0.14f * Mathf.Sin(u * 135f + phase2);
                    var gapCenter = 0.58f + 0.14f * Mathf.Sin(phase1);
                    var gap = 1f - 0.88f * Mathf.Exp(-Mathf.Pow((u - gapCenter) / 0.035f, 2f));
                    var edgeFade = SmoothStep01(0f, 0.10f, u) * (1f - SmoothStep01(0.90f, 1f, u));
                    alpha = Mathf.Clamp01(bands * gap * edgeFade * 0.82f);
                }

                col.a = Mathf.Clamp01(alpha);
                pixels[x] = col;
                pixels[TextureWidth + x] = col;
            }

            tex.SetPixels(pixels);
            tex.Apply(false, true);
            return tex;
        }
    }
}
