using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class DarkFog
    {
        public static void GenerateMissingHiveOrbits(EnemyDFHiveSystem __instance)
        {
            if (__instance.starData.hiveAstroOrbits == null || __instance.starData.hiveAstroOrbits.Length == 0)
            {
                __instance.starData.hiveAstroOrbits = new AstroOrbitData[8];
            }

            if (__instance.starData.hiveAstroOrbits.Length - 1 < __instance.hiveOrbitIndex)
            {
                var temp = new AstroOrbitData[8];
                Array.Copy(__instance.starData.hiveAstroOrbits, temp, __instance.starData.hiveAstroOrbits.Length);
                __instance.starData.hiveAstroOrbits = temp;
            }

            if (__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex] == null)
            {
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex] = new AstroOrbitData();
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRadius =
                    10f * __instance.hiveOrbitIndex;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitInclination = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitLongitude = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitPhase = 0f;
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitalPeriod =
                    Utils.CalculateOrbitPeriod(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex]
                        .orbitRadius);
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRotation =
                    Quaternion.AngleAxis(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitLongitude,
                        Vector3.up) *
                    Quaternion.AngleAxis(
                        __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitInclination,
                        Vector3.forward);
                __instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitNormal =
                    Maths.QRotateLF(__instance.starData.hiveAstroOrbits[__instance.hiveOrbitIndex].orbitRotation,
                        new VectorLF3(0f, 1f, 0f)).normalized;
            }
        }
    }
}