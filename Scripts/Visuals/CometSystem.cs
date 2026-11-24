using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    /// <summary>Renders ion and dust tails for bodies using a comet theme.</summary>
    public static class CometSystem
    {
        private const string TailRootName = "GS2_CometTail";
        private const string IonTailName = "IonTail";
        private const string DustTailName = "DustTail";
        private const string ComaHaloName = "ComaHalo";
        private const string StreamPuffsName = "CometPuffs";
        private const int ConeRadialSegs = 24;
        private const int ConeLengthSegs = 12;
        private const int PuffCount = 48;

        private sealed class VolumetricConeState : MonoBehaviour
        {
            public Mesh Mesh;
            public Vector3[] BaseVerts;
            public float[] CosA;
            public float[] SinA;
            public float[] AlongFade;
            public float[] DeltaR;
            public Color32[] Colors;
            public Color32 BaseTint;

            private void OnDestroy() => UnityEngine.Object.Destroy(Mesh);
        }

        private sealed class StreamPuffsState : MonoBehaviour
        {
            public Mesh Mesh;
            public Vector3[] Verts;
            public Color32[] Colors;

            private void OnDestroy() => UnityEngine.Object.Destroy(Mesh);
        }

        private static Material _ionMat;
        private static Material _dustMat;
        private static Material _comaMat;
        private static Material _puffMat;
        private static Mesh _comaQuadMesh;

        public static bool IsCometPlanet(PlanetData planet)
        {
            return planet != null && planet.type != EPlanetType.Gas &&
                   GS2.gsPlanets.TryGetValue(planet.id, out var model) && model?.planetData == planet &&
                   model.Theme.StartsWith("Comet", StringComparison.OrdinalIgnoreCase);
        }

        private static void DestroyTail(Transform tail)
        {
            if (tail == null) return;
            tail.gameObject.SetActive(false);
            tail.SetParent(null, false);
            UnityEngine.Object.Destroy(tail.gameObject);
        }

        public static void AttachTailToPlanetSimulator(PlanetSimulator simulator, PlanetData planet)
        {
            if (simulator == null || planet == null) return;
            try
            {
                DestroyTail(simulator.transform.Find(TailRootName));
                if (!IsCometPlanet(planet)) return;

                // Comets inherit from IceGelisol which has a planetary cloud nephogram; disable it so comets never leave a giant grey cloud puff on takeoff
                DisableCometClouds(simulator);

                var root = CreateTailHierarchy(simulator.transform, simulator.gameObject.layer, true);
                if (root != null) UpdateTailOrientation(root, planet, true);
            }
            catch (Exception ex)
            {
                GS2.Warn($"CometSystem.AttachTailToPlanetSimulator failed for {planet?.name}: {ex.Message}");
            }
        }

        public static void SyncLayer(PlanetSimulator simulator)
        {
            if (simulator == null) return;
            var tail = simulator.transform.Find(TailRootName);
            if (tail != null) SetLayerRecursive(tail.gameObject, 0);
        }

        public static void UpdateSpaceTail(PlanetSimulator simulator)
        {
            if (simulator == null || simulator.planetData == null) return;
            var tail = simulator.transform.Find(TailRootName);
            if (tail == null) return;
            DisableCometClouds(simulator);
            UpdateTailOrientation(tail, simulator.planetData, true);
        }

        private static void DisableCometClouds(PlanetSimulator simulator)
        {
            var cloud = simulator?.cloudSimulator;
            if (cloud == null) return;
            if (cloud.gameObject.activeSelf) cloud.gameObject.SetActive(false);
            if (cloud.nephogram != null && cloud.nephogram.gameObject.activeSelf) cloud.nephogram.gameObject.SetActive(false);
        }

        private sealed class StarmapCometTag : MonoBehaviour
        {
            public PlanetData Planet;
        }

        private static bool IsStarmapOpen() =>
            UIRoot.instance != null && UIRoot.instance.uiGame != null &&
            UIRoot.instance.uiGame.starmap != null && UIRoot.instance.uiGame.starmap.active;

        public static void AttachStarmapTail(UIStarmapPlanet starmapPlanet)
        {
            if (starmapPlanet.planetRenderer == null) return;
            var planet = starmapPlanet.planet;
            var host = starmapPlanet.planetRenderer.transform;
            try
            {
                var existing = host.Find(TailRootName);
                var isComet = IsCometPlanet(planet);
                if (existing != null)
                {
                    if (isComet && existing.GetComponent<StarmapCometTag>()?.Planet == planet)
                    {
                        UpdateTailOrientation(existing, planet, false);
                        return;
                    }

                    DestroyTail(existing);
                }

                if (!isComet) return;

                var root = CreateTailHierarchy(host, host.gameObject.layer, false);
                if (root == null) return;
                root.gameObject.AddComponent<StarmapCometTag>().Planet = planet;
                UpdateTailOrientation(root, planet, false);
            }
            catch (Exception ex)
            {
                GS2.Warn($"CometSystem.AttachStarmapTail failed for {planet?.name}: {ex.Message}");
            }
        }

        public static void UpdateStarmapTail(UIStarmapPlanet starmapPlanet)
        {
            if (starmapPlanet.planetRenderer == null || !IsCometPlanet(starmapPlanet.planet)) return;
            var tail = starmapPlanet.planetRenderer.transform.Find(TailRootName);
            if (tail != null) UpdateTailOrientation(tail, starmapPlanet.planet, false);
        }

        private static Transform CreateTailHierarchy(Transform parent, int layer, bool includePuffs)
        {
            var comaMesh = GetOrCreateComaQuadMesh();
            var ionMat = GetOrCreateTailMaterial(ref _ionMat, new Color(0.32f, 0.84f, 1.00f, 0.44f), "GS2_CometIonMat");
            var dustMat = GetOrCreateTailMaterial(ref _dustMat, new Color(1.00f, 0.94f, 0.82f, 0.28f), "GS2_CometDustMat");
            var comaMat = GetOrCreateComaMaterial();
            if (ionMat == null || dustMat == null) return null;

            var root = new GameObject(TailRootName) { layer = layer };
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            AddVolumetricConeChild(root.transform, IonTailName, ionMat, new Color32(255, 255, 255, 255), 0.38f, 1.10f, layer);
            AddVolumetricConeChild(root.transform, DustTailName, dustMat, new Color32(255, 255, 255, 255), 0.44f, 1.35f, layer);
            if (comaMesh != null && comaMat != null)
                AddSimpleMeshChild(root.transform, ComaHaloName, comaMesh, comaMat, layer);
            if (includePuffs)
            {
                var puffMat = GetOrCreatePuffMaterial();
                if (puffMat != null)
                    AddStreamPuffsChild(root.transform, StreamPuffsName, puffMat, layer);
            }
            return root.transform;
        }

        private static void AddSimpleMeshChild(Transform root, string name, Mesh mesh, Material mat, int layer)
        {
            var go = new GameObject(name) { layer = layer };
            go.transform.SetParent(root, false);
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            ConfigureRenderer(go.AddComponent<MeshRenderer>(), mat);
        }

        private static void ConfigureRenderer(MeshRenderer mr, Material mat)
        {
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.lightProbeUsage = LightProbeUsage.Off;
            mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
            mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }

        private static void AddVolumetricConeChild(Transform root, string name, Material mat, Color32 tint, float r0, float r1, int layer)
        {
            var go = new GameObject(name) { layer = layer };
            go.transform.SetParent(root, false);

            var vertCount = (ConeLengthSegs + 1) * (ConeRadialSegs + 1);
            var verts = new Vector3[vertCount];
            var uvs = new Vector2[vertCount];
            var cols = new Color32[vertCount];
            var cosA = new float[vertCount];
            var sinA = new float[vertCount];
            var alongFade = new float[vertCount];
            var deltaR = new float[vertCount];
            var tris = new int[ConeLengthSegs * ConeRadialSegs * 6];

            var drTotal = r1 - r0;
            for (var l = 0; l <= ConeLengthSegs; l++)
            {
                var u = l / (float)ConeLengthSegs;
                var z = u;
                var r = Mathf.Lerp(r0, r1, Mathf.Pow(u, 0.78f));
                // Local radial slope dr/dz for accurate volumetric surface normal
                var slope = drTotal * 0.78f * Mathf.Pow(Mathf.Max(0.04f, u), -0.22f);
                var along = SmoothStep01(0f, 0.16f, u) * Mathf.Pow(1f - u, 2.0f);

                for (var s = 0; s <= ConeRadialSegs; s++)
                {
                    var v = s / (float)ConeRadialSegs;
                    var ang = v * Mathf.PI * 2f;
                    var c = Mathf.Cos(ang);
                    var sn = Mathf.Sin(ang);
                    var idx = l * (ConeRadialSegs + 1) + s;

                    verts[idx] = new Vector3(r * c, r * sn, z);
                    uvs[idx] = new Vector2(u, v);
                    cols[idx] = new Color32(tint.r, tint.g, tint.b, (byte)Mathf.RoundToInt(along * 255f));
                    cosA[idx] = c;
                    sinA[idx] = sn;
                    alongFade[idx] = along;
                    deltaR[idx] = slope;
                }
            }

            var t = 0;
            for (var l = 0; l < ConeLengthSegs; l++)
            {
                for (var s = 0; s < ConeRadialSegs; s++)
                {
                    var a = l * (ConeRadialSegs + 1) + s;
                    var b = a + 1;
                    var c = a + (ConeRadialSegs + 1);
                    var d = c + 1;
                    tris[t++] = a; tris[t++] = c; tris[t++] = b;
                    tris[t++] = b; tris[t++] = c; tris[t++] = d;
                }
            }

            var mesh = new Mesh
            {
                name = $"GS2_CometVolCone_{name}",
                vertices = verts,
                uv = uvs,
                colors32 = cols,
                triangles = tris
            };
            mesh.MarkDynamic();
            mesh.RecalculateBounds();

            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            ConfigureRenderer(go.AddComponent<MeshRenderer>(), mat);

            var state = go.AddComponent<VolumetricConeState>();
            state.Mesh = mesh;
            state.BaseVerts = verts;
            state.CosA = cosA;
            state.SinA = sinA;
            state.AlongFade = alongFade;
            state.DeltaR = deltaR;
            state.Colors = cols;
            state.BaseTint = tint;
        }

        private static void AddStreamPuffsChild(Transform root, string name, Material mat, int layer)
        {
            var go = new GameObject(name) { layer = layer };
            go.transform.SetParent(root, false);

            var verts = new Vector3[PuffCount * 4];
            var uvs = new Vector2[PuffCount * 4];
            var cols = new Color32[PuffCount * 4];
            var tris = new int[PuffCount * 6];

            for (var i = 0; i < PuffCount; i++)
            {
                var v = i * 4;
                uvs[v] = new Vector2(0f, 0f);
                uvs[v + 1] = new Vector2(1f, 0f);
                uvs[v + 2] = new Vector2(0f, 1f);
                uvs[v + 3] = new Vector2(1f, 1f);

                var ti = i * 6;
                tris[ti] = v; tris[ti + 1] = v + 2; tris[ti + 2] = v + 1;
                tris[ti + 3] = v + 1; tris[ti + 4] = v + 2; tris[ti + 5] = v + 3;
            }

            var mesh = new Mesh
            {
                name = "GS2_CometStreamPuffsMesh",
                vertices = verts,
                uv = uvs,
                colors32 = cols,
                triangles = tris
            };
            mesh.MarkDynamic();

            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            ConfigureRenderer(go.AddComponent<MeshRenderer>(), mat);

            var state = go.AddComponent<StreamPuffsState>();
            state.Mesh = mesh;
            state.Verts = verts;
            state.Colors = cols;
        }

        private static void UpdateVolumetricConeColors(Transform coneTrans, Camera cam, float width, float length, float baseScale, bool isSpaceView)
        {
            var state = coneTrans.GetComponent<VolumetricConeState>();
            if (state == null || state.Mesh == null || cam == null) return;

            // Camera position in coneTrans's rotated-but-scaled space (matches physical vertex coordinates regardless of parent/starmap scale)
            var camRelScaled = Vector3.Scale(coneTrans.InverseTransformPoint(cam.transform.position), new Vector3(width, width, length));
            var verts = state.BaseVerts;
            var cosA = state.CosA;
            var sinA = state.SinA;
            var along = state.AlongFade;
            var slope = state.DeltaR;
            var cols = state.Colors;
            var tint = state.BaseTint;
            var nearMin = baseScale * 0.45f;
            var nearMax = baseScale * 1.65f;

            for (var i = 0; i < verts.Length; i++)
            {
                var p = verts[i];
                var px = p.x * width;
                var py = p.y * width;
                var pz = p.z * length;

                var vx = camRelScaled.x - px;
                var vy = camRelScaled.y - py;
                var vz = camRelScaled.z - pz;
                var distSq = vx * vx + vy * vy + vz * vz;
                if (distSq < 1e-6f)
                {
                    cols[i] = new Color32(tint.r, tint.g, tint.b, 0);
                    continue;
                }
                var dist = Mathf.Sqrt(distSq);
                var invDist = 1f / dist;
                vx *= invDist; vy *= invDist; vz *= invDist;

                // True outward volumetric cone wall normal in scaled space
                var nx = cosA[i] * length;
                var ny = sinA[i] * length;
                var nz = -slope[i] * width;
                var nLen = Mathf.Sqrt(nx * nx + ny * ny + nz * nz);
                if (nLen > 1e-6f)
                {
                    var invN = 1f / nLen;
                    nx *= invN; ny *= invN; nz *= invN;
                }

                // Optical thickness: sharper Gaussian-like rim exponent (3.0) + 24 radial segments -> completely dissolves outer tube silhouette into space
                var facing = Mathf.Abs(nx * vx + ny * vy + nz * vz);
                var volProfile = Mathf.Pow(facing, 3.0f);
                var nearFade = isSpaceView ? SmoothStep01(nearMin, nearMax, dist) : 1f;
                var a = Mathf.Clamp01(along[i] * volProfile * nearFade);
                cols[i] = new Color32(tint.r, tint.g, tint.b, (byte)Mathf.RoundToInt(a * 255f));
            }

            state.Mesh.colors32 = cols;
        }

        private static void UpdateStreamPuffs(Transform puffsTrans, Transform tailRoot, Camera cam, Vector3 ionDir, Vector3 dustDir, float ionWidth, float ionLength, float dustWidth, float dustLength, float nucleusOffset, float baseScale, float t, float seedPhase)
        {
            var state = puffsTrans.GetComponent<StreamPuffsState>();
            if (state == null || state.Mesh == null || cam == null) return;

            var camRight = tailRoot.InverseTransformDirection(cam.transform.right).normalized;
            var camUp = tailRoot.InverseTransformDirection(cam.transform.up).normalized;
            var camLocalPos = tailRoot.InverseTransformPoint(cam.transform.position);

            var ionRef = Mathf.Abs(ionDir.y) < 0.9f ? Vector3.up : Vector3.right;
            var ionR = Vector3.Cross(ionRef, ionDir).normalized;
            var ionU = Vector3.Cross(ionDir, ionR).normalized;

            var dustRef = Mathf.Abs(dustDir.y) < 0.9f ? Vector3.up : Vector3.right;
            var dustR = Vector3.Cross(dustRef, dustDir).normalized;
            var dustU = Vector3.Cross(dustDir, dustR).normalized;

            var verts = state.Verts;
            var cols = state.Colors;
            var nearMin = baseScale * 0.40f;
            var nearMax = baseScale * 1.45f;

            for (var i = 0; i < PuffCount; i++)
            {
                var isIon = i < 30;
                var fi = (float)i;
                var speed = isIon ? (0.14f + (i % 5) * 0.022f) : (0.065f + (i % 4) * 0.014f);
                var phase = (fi * 0.6180339f + seedPhase * 0.13f) % 1f;
                var u = (t * speed + phase) % 1f;

                var dir = isIon ? ionDir : dustDir;
                var bR = isIon ? ionR : dustR;
                var bU = isIon ? ionU : dustU;
                var w = isIon ? ionWidth : dustWidth;
                var len = isIon ? ionLength : dustLength;

                // Golden-angle 3D volumetric distribution inside the cone with slow helical drift
                var theta = fi * 2.399963f + u * (isIon ? 1.8f : 1.1f) + seedPhase;
                var radialFrac = Mathf.Sqrt(((i * 37) % 100) / 100f) * 0.62f;
                var coneR = w * Mathf.Lerp(0.32f, 1.05f, Mathf.Pow(u, 0.78f)) * radialFrac;
                var zDist = (isIon ? nucleusOffset : nucleusOffset * 0.85f) + u * len * 0.92f;

                var center = dir * zDist + (bR * Mathf.Cos(theta) + bU * Mathf.Sin(theta)) * coneR;
                var camDist = (camLocalPos - center).magnitude;
                var nearFade = SmoothStep01(nearMin, nearMax, camDist);

                var sizeJitter = 0.78f + ((i * 19) % 10) * 0.048f;
                var halfSize = baseScale * (isIon ? 0.95f : 1.35f) * Mathf.Lerp(0.55f, 2.6f, Mathf.Pow(u, 0.75f)) * sizeJitter;

                var along = SmoothStep01(0f, 0.15f, u) * Mathf.Pow(1f - u, 1.85f);
                var alpha = Mathf.Clamp01(along * nearFade * (isIon ? 0.24f : 0.18f));
                var col = isIon
                    ? new Color32(115, 224, 255, (byte)Mathf.RoundToInt(alpha * 255f))
                    : new Color32(255, 232, 192, (byte)Mathf.RoundToInt(alpha * 255f));

                var rx = camRight * halfSize;
                var uy = camUp * halfSize;
                var vi = i * 4;
                verts[vi] = center - rx - uy;
                verts[vi + 1] = center + rx - uy;
                verts[vi + 2] = center - rx + uy;
                verts[vi + 3] = center + rx + uy;
                cols[vi] = col;
                cols[vi + 1] = col;
                cols[vi + 2] = col;
                cols[vi + 3] = col;
            }

            state.Mesh.vertices = verts;
            state.Mesh.colors32 = cols;
            state.Mesh.bounds = new Bounds(Vector3.zero, Vector3.one * (ionLength * 2.4f));
        }

        private static Quaternion ComputeStableTailRotation(Vector3 tailDirLocal)
        {
            var dir = tailDirLocal.normalized;
            var refUp = Mathf.Abs(dir.y) < 0.9f ? Vector3.up : Vector3.right;
            return Quaternion.LookRotation(dir, refUp);
        }

        private static void UpdateTailOrientation(Transform tailRoot, PlanetData planet, bool isSpaceView)
        {
            if (planet?.star == null) return;

            // Hide 3D space tail when Starmap UI is open (prevents 3D space tail leaking over Icarus in Starmap),
            // but KEEP it visible when standing on or launching from the comet so players can look up and see the tail in the sky!
            if (isSpaceView && IsStarmapOpen())
            {
                if (tailRoot.gameObject.activeSelf) tailRoot.gameObject.SetActive(false);
                return;
            }
            if (!tailRoot.gameObject.activeSelf) tailRoot.gameObject.SetActive(true);

            var onCometSurface = isSpaceView && GameMain.localPlanet == planet;

            var fromSun = planet.uPosition - planet.star.uPosition;
            var distMeters = fromSun.magnitude;
            if (distMeters < 10.0) return;

            var distAU = (float)(distMeters / 40000.0);
            var antiSunWorld = new Vector3((float)(fromSun.x / distMeters), (float)(fromSun.y / distMeters), (float)(fromSun.z / distMeters));

            // Dynamic solar-proximity flare: gracefully longer near perihelion (< 1.2 AU), fades past 6 AU
            var flare = Mathf.Clamp(Mathf.Pow(1.25f / Mathf.Max(0.45f, distAU), 1.05f), 0.25f, 2.4f);
            var baseScale = isSpaceView ? Mathf.Max(25f, planet.realRadius) : 0.65f;

            var ionLength = baseScale * (isSpaceView ? 38f : 7.5f) * flare;
            var ionWidth = baseScale * (isSpaceView ? 2.5f : 1.15f);
            var dustLength = ionLength * 0.78f;
            var dustWidth = ionWidth * 1.42f;

            var localAntiSun = isSpaceView
                ? (Quaternion.Inverse(planet.runtimeRotation) * antiSunWorld).normalized
                : (tailRoot.parent != null ? (Quaternion.Inverse(tailRoot.parent.rotation) * antiSunWorld).normalized : antiSunWorld);

            // Dust tail lags slightly behind anti-solar direction opposite to orbital motion (classic real twin-tail split)
            var vel = planet.runtimePositionNext - planet.runtimePosition;
            var dustDir = localAntiSun;
            if (vel.sqrMagnitude > 1e-12)
            {
                var velWorld = new Vector3((float)vel.x, (float)vel.y, (float)vel.z).normalized;
                var localVel = isSpaceView
                    ? (Quaternion.Inverse(planet.runtimeRotation) * velWorld).normalized
                    : (tailRoot.parent != null ? (Quaternion.Inverse(tailRoot.parent.rotation) * velWorld).normalized : velWorld);
                dustDir = (localAntiSun - localVel * 0.18f).normalized;
            }

            // Camera direction & distance in tailRoot local space (used only for ComaHalo orientation and close-range fade)
            var cam = Camera.main;
            var toCamLocal = Vector3.up;
            var camDist = float.MaxValue;
            if (cam != null)
            {
                var camWorld = cam.transform.position - tailRoot.position;
                camDist = camWorld.magnitude;
                if (camDist > 1e-3f)
                    toCamLocal = tailRoot.InverseTransformDirection(camWorld).normalized;
            }

            // Live solar-wind streaming animation (zero-GC UV scroll + gentle plasma breathing & subtle flutter)
            var t = Time.unscaledTime;
            if (_ionMat != null) _ionMat.mainTextureOffset = new Vector2(-(t * 0.28f) % 1f, 0f);
            if (_dustMat != null) _dustMat.mainTextureOffset = new Vector2(-(t * 0.09f) % 1f, 0f);

            var seedPhase = (planet.id % 37) * 0.73f;
            var ionPulse = 1f + 0.05f * Mathf.Sin(t * 0.52f + seedPhase) + 0.025f * Mathf.Cos(t * 0.91f + seedPhase * 1.7f);
            var dustPulse = 1f + 0.025f * Mathf.Sin(t * 0.28f + seedPhase * 0.6f);
            var swayAxis = Vector3.Cross(localAntiSun, Mathf.Abs(localAntiSun.y) < 0.9f ? Vector3.up : Vector3.right).normalized;
            var swayAngle = Mathf.Sin(t * 0.38f + seedPhase) * 1.5f + Mathf.Cos(t * 0.67f + seedPhase) * 0.7f;
            var ionSwayDir = (Quaternion.AngleAxis(swayAngle, swayAxis) * localAntiSun).normalized;

            // Start tails cleanly behind the comet's limb (slightly further out when standing on the comet surface)
            var nucleusOffset = isSpaceView ? baseScale * (onCometSurface ? 1.12f : 0.88f) : 0.12f;

            // True 3D volumetric soft-cone: per-vertex view-angle density drops smoothly to 0 at silhouette rim (ZERO flat planes, ZERO hard edges!)
            var ionW = ionWidth * (0.97f + 0.05f * ionPulse);
            var ionL = ionLength * ionPulse;
            var ion = tailRoot.Find(IonTailName);
            if (ion != null)
            {
                ion.localPosition = ionSwayDir * nucleusOffset;
                ion.localRotation = ComputeStableTailRotation(ionSwayDir);
                ion.localScale = new Vector3(ionW, ionW, ionL);
                UpdateVolumetricConeColors(ion, cam, ionW, ionL, baseScale, isSpaceView);
            }

            var dustW = dustWidth * dustPulse;
            var dustL = dustLength * dustPulse;
            var dust = tailRoot.Find(DustTailName);
            if (dust != null)
            {
                dust.localPosition = dustDir * (nucleusOffset * 0.85f);
                dust.localRotation = ComputeStableTailRotation(dustDir);
                dust.localScale = new Vector3(dustW, dustW, dustL);
                UpdateVolumetricConeColors(dust, cam, dustW, dustL, baseScale, isSpaceView);
            }

            // Live 3D volumetric streaming puff cloud (3D space & comet surface view)
            var puffs = tailRoot.Find(StreamPuffsName);
            if (puffs != null && isSpaceView)
            {
                UpdateStreamPuffs(puffs, tailRoot, cam, ionSwayDir, dustDir, ionW, ionL, dustW, dustL, nucleusOffset, baseScale, t, seedPhase);
            }

            // Soft radial Gaussian coma glow (smoothly shrinks/fades behind the nucleus as camera approaches landing so there is zero pop)
            var coma = tailRoot.Find(ComaHaloName);
            if (coma != null)
            {
                var comaFade = isSpaceView ? SmoothStep01(baseScale * 2.2f, baseScale * 5.2f, camDist) : 1f;
                var showComa = !onCometSurface && comaFade > 0.01f;
                if (coma.gameObject.activeSelf != showComa) coma.gameObject.SetActive(showComa);
                if (showComa)
                {
                    var comaScale = baseScale * (isSpaceView ? 1.85f : 1.00f) * (0.96f + 0.04f * dustPulse) * comaFade;
                    coma.localPosition = -toCamLocal * (isSpaceView ? baseScale * 0.55f : 0.05f) + localAntiSun * (isSpaceView ? baseScale * 0.30f : 0.08f);
                    coma.localRotation = Quaternion.LookRotation(toCamLocal, Vector3.up);
                    coma.localScale = new Vector3(comaScale, comaScale, 1f);
                }
            }
        }

        private static float SmoothStep01(float edge0, float edge1, float x)
        {
            var t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static Material GetOrCreateTailMaterial(ref Material cache, Color tint, string matName)
        {
            if (cache != null) return cache;
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("UI/Default") ?? Shader.Find("Unlit/Transparent");
            if (shader == null) return null;

            const int w = 128;
            const int h = 64;
            // Seamlessly periodic in BOTH U (streaming outward) and V (360 deg around volumetric cone) — ZERO UV seam, ZERO perpendicular stripes!
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapModeU = TextureWrapMode.Repeat, wrapModeV = TextureWrapMode.Repeat, filterMode = FilterMode.Bilinear };
            var px = new Color[w * h];
            for (var y = 0; y < h; y++)
            {
                var v = y / (float)h; // periodic [0, 1) around cone circumference
                var angV = v * Mathf.PI * 2f;
                var wisps = 0.78f + 0.14f * Mathf.Sin(angV * 5f + 0.4f) + 0.08f * Mathf.Cos(angV * 9f + 1.3f);
                for (var x = 0; x < w; x++)
                {
                    var u = x / (float)w; // periodic [0, 1) along tail axis
                    var angU = u * Mathf.PI * 2f;
                    // Elongated longitudinal plasma filaments wrapping seamlessly around the 3D cone
                    var flow = 0.74f
                        + 0.16f * Mathf.Sin(angU * 1f + angV * 3f)
                        + 0.10f * Mathf.Cos(angU * 2f - angV * 5f + 0.9f);
                    var lum = Mathf.Clamp01(wisps * flow);
                    var r = Mathf.Clamp01(tint.r + 0.18f * lum);
                    var g = Mathf.Clamp01(tint.g + 0.18f * lum);
                    var b = Mathf.Clamp01(tint.b + 0.18f * lum);
                    var a = Mathf.Clamp01(lum * tint.a);
                    px[y * w + x] = new Color(r, g, b, a);
                }
            }
            tex.SetPixels(px);
            tex.Apply(false, true);

            // Render before local planet daytime atmosphere (2991) & clouds so distant comet tails are naturally veiled by daytime skies
            cache = new Material(shader) { name = matName, mainTexture = tex, renderQueue = 2987 };
            if (cache.HasProperty("_Color")) cache.SetColor("_Color", Color.white);
            return cache;
        }

        private static Material GetOrCreatePuffMaterial()
        {
            if (_puffMat != null) return _puffMat;
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("UI/Default") ?? Shader.Find("Unlit/Transparent");
            if (shader == null) return null;

            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var px = new Color[size * size];
            for (var y = 0; y < size; y++)
            {
                var dy = (y / (float)(size - 1)) * 2f - 1f;
                for (var x = 0; x < size; x++)
                {
                    var dx = (x / (float)(size - 1)) * 2f - 1f;
                    var r = Mathf.Sqrt(dx * dx + dy * dy);
                    var gauss = Mathf.Exp(-r * r * 4.8f);
                    var edgeZero = 1f - SmoothStep01(0.62f, 1f, r);
                    var a = Mathf.Clamp01(gauss * edgeZero);
                    px[y * size + x] = new Color(1f, 1f, 1f, a);
                }
            }
            tex.SetPixels(px);
            tex.Apply(false, true);

            _puffMat = new Material(shader) { name = "GS2_CometStreamPuffMat", mainTexture = tex, renderQueue = 2987 };
            if (_puffMat.HasProperty("_Color")) _puffMat.SetColor("_Color", Color.white);
            return _puffMat;
        }

        private static Mesh GetOrCreateComaQuadMesh()
        {
            if (_comaQuadMesh != null) return _comaQuadMesh;
            _comaQuadMesh = new Mesh
            {
                name = "GS2_CometComaQuadMesh",
                vertices = new[]
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(1f, -1f, 0f),
                    new Vector3(-1f, 1f, 0f),
                    new Vector3(1f, 1f, 0f)
                },
                uv = new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f)
                },
                triangles = new[] { 0, 2, 1, 1, 2, 3, 0, 1, 2, 1, 3, 2 }
            };
            _comaQuadMesh.RecalculateBounds();
            return _comaQuadMesh;
        }

        private static Material GetOrCreateComaMaterial()
        {
            if (_comaMat != null) return _comaMat;
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("UI/Default") ?? Shader.Find("Unlit/Transparent");
            if (shader == null) return null;

            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var px = new Color[size * size];
            for (var y = 0; y < size; y++)
            {
                var dy = (y / (float)(size - 1)) * 2f - 1f;
                for (var x = 0; x < size; x++)
                {
                    var dx = (x / (float)(size - 1)) * 2f - 1f;
                    var r = Mathf.Sqrt(dx * dx + dy * dy);
                    var gauss = Mathf.Exp(-r * r * 6.5f);
                    var edgeZero = 1f - SmoothStep01(0.60f, 1f, r);
                    var a = Mathf.Clamp01(gauss * edgeZero * 0.24f);
                    px[y * size + x] = new Color(0.62f, 0.88f, 1.00f, a);
                }
            }
            tex.SetPixels(px);
            tex.Apply(false, true);

            _comaMat = new Material(shader) { name = "GS2_CometComaMat", mainTexture = tex, renderQueue = 2986 };
            if (_comaMat.HasProperty("_Color")) _comaMat.SetColor("_Color", Color.white);
            return _comaMat;
        }

        private static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            for (var i = 0; i < go.transform.childCount; i++)
                SetLayerRecursive(go.transform.GetChild(i).gameObject, layer);
        }
    }
}
