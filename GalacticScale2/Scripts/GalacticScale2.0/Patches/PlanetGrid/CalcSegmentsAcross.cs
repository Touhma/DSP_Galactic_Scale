using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetGrid), "CalcSegmentsAcross")]
        public static bool CalcSegmentsAcross(PlanetGrid __instance, Vector3 posR, Vector3 posA, Vector3 posB, ref float __result)
        {
            //No config check for Planet size change since this replicates vanilla in case of size 200
            posR.Normalize();
            posA.Normalize();
            posB.Normalize();
            var num = Mathf.Asin(posR.y);
            var f = num / ((float)Math.PI * 2f) * __instance.segment;
            var latitudeIndex = Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f) - 0.1f));
            float num2 = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
            //Replaced the fixed value 0.0048 with 1/segments * 0.96 [based on planet size 200: 1/200 = 0.005; 0.005 * 0.96 = 0.0048
            //since the value has to become smaller the larger the planet is, the inverse value (1/x) is used in the calculation
            var num3 = Mathf.Max(1.0f / __instance.segment * 0.96f, Mathf.Cos(num) * (float)Math.PI * 2f / (num2 * 5f));
            var num4 = (float)Math.PI * 2f / (__instance.segment * 5f);
            var num5 = Mathf.Asin(posA.y);
            var num6 = Mathf.Atan2(posA.x, 0f - posA.z);
            var num7 = Mathf.Asin(posB.y);
            var num8 = Mathf.Atan2(posB.x, 0f - posB.z);
            var num9 = Mathf.Abs(Mathf.DeltaAngle(num6 * 57.29578f, num8 * 57.29578f) * ((float)Math.PI / 180f));
            var num10 = Mathf.Abs(num5 - num7);
            var num11 = num10 + num9;
            var num12 = 0f;
            var num13 = 1f;
            if (num11 > 0f)
            {
                num12 = num9 / num11;
                num13 = num10 / num11;
            }

            var num14 = num3 * num12 + num4 * num13;
            __result = (posA - posB).magnitude / num14;
            return false;
        }
    }
}