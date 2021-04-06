using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetGrid))]
    public class PatchOnPlanetGrid {
        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();

        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int latitudeIndex, int segment, ref int __result) {
            if (!DSPGame.IsMenuDemo) 
                if (Patch.EnableResizingFeature.Value || Patch.EnableLimitedResizingFeature.Value) {
                    Patch.Debug("PlanetGrid Vanilla DetermineLongitudeSegmentCount.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                    if (keyedLUTs.ContainsKey(segment)) {
                        Patch.Debug("PlanetGrid Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        int index = Mathf.Abs(latitudeIndex) % (segment / 2);
                        if (index >= segment / 4) {
                            index = segment / 4 - index;
                        }

                        Patch.Debug("PlatformSystem Vanilla DetermineLongitudeSegmentCount fetched " + keyedLUTs[segment][index] + " for segments " + segment + " at index " + latitudeIndex + "(" + index + ")", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        __result = keyedLUTs[segment][index];
                    }
                    else {
                        //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                        Patch.Debug("DetermineLongitudeSegmentCount Failed, Defaulting to original with segment count " + segment, LogLevel.Warning, true);
                        var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (latitudeIndex / (double) (segment / 4f) * 3.14159274101257 * 0.5))) * segment);
                        __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
                    }

                    return false;
                }
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch("CalcSegmentsAcross")]
        public static bool CalcSegmentsAcross(PlanetGrid __instance, Vector3 posR, Vector3 posA, Vector3 posB, ref float __result)
        {
            //No config check for Planet size change since this replicates vanilla in case of size 200
            posR.Normalize();
            posA.Normalize();
            posB.Normalize();
            float num = Mathf.Asin(posR.y);
            float f = num / ((float)Math.PI * 2f) * (float)__instance.segment;
            int latitudeIndex = Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f) - 0.1f));
            float num2 = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
            //Replaced the fixed value 0.0048 with 1/segments * 0.96 [based on planet size 200: 1/200 = 0.005; 0.005 * 0.96 = 0.0048
            //since the value has to become smaller the larger the planet is, the inverse value (1/x) is used in the calculation
            float num3 = Mathf.Max((1.0f / __instance.segment) * 0.96f, Mathf.Cos(num) * (float)Math.PI * 2f / (num2 * 5f));
            float num4 = (float)Math.PI * 2f / ((float)__instance.segment * 5f);
            float num5 = Mathf.Asin(posA.y);
            float num6 = Mathf.Atan2(posA.x, 0f - posA.z);
            float num7 = Mathf.Asin(posB.y);
            float num8 = Mathf.Atan2(posB.x, 0f - posB.z);
            float num9 = Mathf.Abs(Mathf.DeltaAngle(num6 * 57.29578f, num8 * 57.29578f) * ((float)Math.PI / 180f));
            float num10 = Mathf.Abs(num5 - num7);
            float num11 = num10 + num9;
            float num12 = 0f;
            float num13 = 1f;
            if (num11 > 0f)
            {
                num12 = num9 / num11;
                num13 = num10 / num11;
            }
            float num14 = num3 * num12 + num4 * num13;
            __result = (posA - posB).magnitude / num14;
            return false;
        }

    }
}