using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.UpdateRuntimePose))]
        public static bool UpdateRuntimePose_EllipticalOrbit(PlanetData __instance, double time)
        {
            if (!EllipticalOrbits.TryGetElements(__instance, out var gsPlanet)) return true;
            EllipticalOrbits.UpdateRuntimePose(__instance, time, gsPlanet);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.PredictPose), new Type[] { typeof(double) })]
        public static bool PredictPose_EllipticalOrbit(PlanetData __instance, double time, ref Pose __result)
        {
            if (!EllipticalOrbits.TryGetElements(__instance, out var gsPlanet)) return true;
            __result = EllipticalOrbits.PredictPose(__instance, time, gsPlanet);
            return false;
        }
    }

    [HarmonyPatch]
    public static class PatchOnPlanetDataPredictPoseLF
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(PlanetData), nameof(PlanetData.PredictPose), new[]
            {
                typeof(double), typeof(VectorLF3).MakeByRefType(), typeof(Quaternion).MakeByRefType()
            });
        }

        [HarmonyPrefix]
        public static bool Prefix(PlanetData __instance, double time, ref VectorLF3 pos, ref Quaternion rot)
        {
            if (!EllipticalOrbits.TryGetElements(__instance, out var gsPlanet)) return true;
            EllipticalOrbits.PredictPose(__instance, time, gsPlanet, out pos, out rot);
            return false;
        }
    }
}
