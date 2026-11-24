using HarmonyLib;

using System.Runtime.CompilerServices;

using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmapPlanet
    {
        private static readonly ConditionalWeakTable<LineRenderer, OrbitDiskPose> OriginalOrbitDiskPoses = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnInit")]
        public static void InitOrbitLine(bool __result, PlanetData ___planet, LineRenderer ___orbitRenderer)
        {
            if (!__result || ___planet == null || ___orbitRenderer == null) return;

            var hasEllipticalOrbit = EllipticalOrbits.TryGetElements(___planet, out var elements);
            var eccentricity = hasEllipticalOrbit ? elements.OrbitEccentricity : 0f;
            var argumentOfPeriapsis = hasEllipticalOrbit ? elements.OrbitArgumentOfPeriapsis : 0f;
            var pointCount = ___orbitRenderer.positionCount;
            if (pointCount < 3) return;

            var semiMinorScale = Mathf.Sqrt(Mathf.Max(0f, 1f - eccentricity * eccentricity));
            var periapsisRadians = argumentOfPeriapsis * Mathf.Deg2Rad;
            var cosPeriapsis = Mathf.Cos(periapsisRadians);
            var sinPeriapsis = Mathf.Sin(periapsisRadians);
            for (var i = 0; i < pointCount; i++)
            {
                var angle = i * (2f * Mathf.PI / pointCount);
                var x = Mathf.Cos(angle) - eccentricity;
                var z = semiMinorScale * Mathf.Sin(angle);
                ___orbitRenderer.SetPosition(i, new Vector3(
                    x * cosPeriapsis - z * sinPeriapsis,
                    0f,
                    x * sinPeriapsis + z * cosPeriapsis));
            }

            UpdateOrbitDisk(___orbitRenderer, eccentricity, argumentOfPeriapsis, semiMinorScale);

            var orbitMaterial = ___orbitRenderer.sharedMaterial;
            if (orbitMaterial == null) return;
            if (hasEllipticalOrbit && orbitMaterial.shader != null && !orbitMaterial.HasProperty("_Eccentricity"))
                Utils.ReplaceShaderIfAvailable(orbitMaterial);
            if (orbitMaterial.HasProperty("_Eccentricity"))
                orbitMaterial.SetFloat("_Eccentricity", eccentricity);
            if (orbitMaterial.HasProperty("_ArgumentOfPeriapsis"))
                orbitMaterial.SetFloat("_ArgumentOfPeriapsis", argumentOfPeriapsis);
            if (orbitMaterial.HasProperty("_OrbitUvOffset"))
                orbitMaterial.SetFloat("_OrbitUvOffset", 0f);
        }

        private static void UpdateOrbitDisk(LineRenderer line, float eccentricity, float periapsis, float semiMinorScale)
        {
            var orbit = line.transform;
            var disk = orbit.Find("orbit-disk");
            if (disk == null)
            {
                foreach (var renderer in orbit.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (renderer.gameObject.name != "orbit-disk") continue;
                    disk = renderer.transform;
                    break;
                }
            }
            if (disk == null) return;

            if (eccentricity <= 0.000001f)
            {
                if (!OriginalOrbitDiskPoses.TryGetValue(line, out var original)) return;
                disk.localPosition = original.Position;
                disk.localRotation = original.Rotation;
                disk.localScale = original.Scale;
                OriginalOrbitDiskPoses.Remove(line);
                return;
            }

            if (!OriginalOrbitDiskPoses.TryGetValue(line, out var pose))
            {
                pose = new OrbitDiskPose(disk);
                OriginalOrbitDiskPoses.Add(line, pose);
            }

            var periapsisRotation = Quaternion.AngleAxis(-periapsis, Vector3.up);
            var centerOffset = periapsisRotation * (Vector3.left * eccentricity);
            var parent = disk.parent;
            disk.localPosition = pose.Position + parent.InverseTransformVector(orbit.TransformVector(centerOffset));
            disk.localRotation = Quaternion.AngleAxis(-periapsis, parent.InverseTransformDirection(orbit.up)) *
                                 pose.Rotation;
            disk.localScale = new Vector3(pose.Scale.x, pose.Scale.y * semiMinorScale, pose.Scale.z);
        }

        private sealed class OrbitDiskPose
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly Vector3 Scale;

            public OrbitDiskPose(Transform transform)
            {
                Position = transform.localPosition;
                Rotation = transform.localRotation;
                Scale = transform.localScale;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnLateUpdate")]
        public static void LateUpdateOrbitLine(PlanetData ___planet, LineRenderer ___orbitRenderer)
        {
            if (___planet == null || ___orbitRenderer == null) return;
            var hasEllipticalOrbit = EllipticalOrbits.TryGetElements(___planet, out var elements);
            var retrograde = ___planet.orbitalPeriod < 0 && (hasEllipticalOrbit || CometSystem.IsCometPlanet(___planet));
            if (!hasEllipticalOrbit && !retrograde) return;

            var planeRotation = ___planet.runtimeOrbitRotation;
            var phase = ___planet.runtimeOrbitPhase;
            var shaderRotation = planeRotation * Quaternion.Euler(0f, -phase, 0f);
            if (retrograde) shaderRotation *= new Quaternion(1f, 0f, 0f, 0f);
            if (hasEllipticalOrbit) ___orbitRenderer.transform.localRotation = planeRotation;
            var orbitMaterial = ___orbitRenderer.sharedMaterial;
            if (orbitMaterial == null) return;
            if (orbitMaterial.HasProperty("_Rotation"))
                orbitMaterial.SetVector("_Rotation", new Vector4(
                    shaderRotation.x, shaderRotation.y, shaderRotation.z, shaderRotation.w));
            if (!hasEllipticalOrbit) return;

            if (orbitMaterial.HasProperty("_Eccentricity"))
                orbitMaterial.SetFloat("_Eccentricity", elements.OrbitEccentricity);
            if (orbitMaterial.HasProperty("_ArgumentOfPeriapsis"))
                orbitMaterial.SetFloat("_ArgumentOfPeriapsis",
                    (elements.OrbitArgumentOfPeriapsis - phase) * (retrograde ? -1f : 1f));
            if (orbitMaterial.HasProperty("_OrbitUvOffset"))
                orbitMaterial.SetFloat("_OrbitUvOffset", GetOrbitUvOffset(
                    elements.OrbitEccentricity,
                    phase,
                    elements.OrbitArgumentOfPeriapsis));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnLateUpdate")]
        public static void LateUpdateCometTail(UIStarmapPlanet __instance)
        {
            CometSystem.UpdateStarmapTail(__instance);
        }

        private static float GetOrbitUvOffset(float eccentricity, float trueLongitudeDegrees,
            float argumentOfPeriapsisDegrees)
        {
            var e = Mathf.Clamp(eccentricity, 0f, 0.999f);
            var trueAnomaly = (trueLongitudeDegrees - argumentOfPeriapsisDegrees) * Mathf.Deg2Rad;
            var eccentricAnomaly = 2f * Mathf.Atan2(
                Mathf.Sqrt(1f - e) * Mathf.Sin(trueAnomaly * 0.5f),
                Mathf.Sqrt(1f + e) * Mathf.Cos(trueAnomaly * 0.5f));
            eccentricAnomaly = Mathf.Repeat(eccentricAnomaly, Mathf.PI * 2f);

            const int partialSteps = 64;
            const int circumferenceSteps = 256;
            var arc = EllipseArcLength(e, eccentricAnomaly, partialSteps);
            var circumference = EllipseArcLength(e, Mathf.PI * 2f, circumferenceSteps);
            return circumference > 0f ? arc / circumference : 0f;
        }

        private static float EllipseArcLength(float eccentricity, float endAnomaly, int steps)
        {
            if (endAnomaly <= 0f) return 0f;
            var step = endAnomaly / steps;
            var sum = EllipseSpeed(eccentricity, 0f) + EllipseSpeed(eccentricity, endAnomaly);
            for (var i = 1; i < steps; i++)
                sum += 2f * EllipseSpeed(eccentricity, i * step);
            return sum * step * 0.5f;
        }

        private static float EllipseSpeed(float eccentricity, float eccentricAnomaly)
        {
            return Mathf.Sqrt(Mathf.Max(0f,
                1f - eccentricity * eccentricity * Mathf.Cos(eccentricAnomaly) * Mathf.Cos(eccentricAnomaly)));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(ref UIStarmapPlanet ___focusPlanet, ref UIStarmapPlanet ___mouseHoverPlanet)
        {
            if (___mouseHoverPlanet != null)
            {
                var planet = ___mouseHoverPlanet.planet;
                var id = planet.id;
                if (planet.scale < 1 && planet.scale > 0) return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "OnPlanetClick")]
        public static bool OnPlanetClick(ref UIStarmapPlanet ___focusPlanet, UIStarmapPlanet planet)
        {
            var id = planet.planet.id;
            if (planet.planet.scale < 1 && planet.planet.scale > 0) return false;
            return true;
        }
    }
}
