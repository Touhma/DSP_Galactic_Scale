using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetGrid))]
    public class PatchOnPlanetGrid {
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int latitudeIndex, int segment, ref int __result) {
            int index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) ((double) latitudeIndex / (double) ((float) segment / 4f) * 3.14159274101257 * 0.5))) * (float) segment);
            __result = index < 500 ? PlanetGrid.segmentTable[index] : (index + 49) / 100 * 100;
            Patch.Debug("PlanetGrid - longitudeSegmentCount" + __result, LogLevel.Debug, true);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ReformSnapTo")]
        public static bool ReformSnapTo(
            ref PlanetGrid __instance,
            Vector3 pos,
            int reformSize,
            int reformType,
            int reformColor,
            Vector3[] reformPoints,
            int[] reformIndices,
            PlatformSystem platform,
            out Vector3 reformCenter, ref int __result) {
            pos.Normalize();


            float num1 = Mathf.Asin(pos.y);
            float num2 = Mathf.Atan2(pos.x, -pos.z);
            float f1 = num1 / 6.283185f * (float) __instance.segment;
            int longitudeSegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(f1)), __instance.segment);


            float num3 = (float) longitudeSegmentCount;
            float f2 = num2 / 6.283185f * num3;
            float f3 = Mathf.Round(f1 * 10f);
            float f4 = Mathf.Round(f2 * 10f);
            float num4 = Mathf.Abs(f3);
            float num5 = Mathf.Abs(f4);
            int num6 = reformSize % 2;
            if ((double) num4 % 2.0 != (double) num6) {
                num4 = (float) Mathf.FloorToInt(Mathf.Abs(f1) * 10f);
                if ((double) num4 % 2.0 != (double) num6)
                    ++num4;
            }

            float num7 = (double) f3 < 0.0 ? -num4 : num4;
            if ((double) num5 % 2.0 != (double) num6) {
                num5 = (float) Mathf.FloorToInt(Mathf.Abs(f2) * 10f);
                if ((double) num5 % 2.0 != (double) num6)
                    ++num5;
            }

            float num8 = (double) f4 < 0.0 ? -num5 : num5;
            float f5 = (float) ((double) num7 / 10.0 / (double) __instance.segment * 6.28318548202515);
            float f6 = (float) ((double) num8 / 10.0 / (double) num3 * 6.28318548202515);
            float y1 = Mathf.Sin(f5);
            float num9 = Mathf.Cos(f5);
            float num10 = Mathf.Sin(f6);
            float num11 = Mathf.Cos(f6);
            reformCenter = new Vector3(num9 * num10, y1, num9 * -num11);
            int num12 = 1 - reformSize;
            int num13 = 1 - reformSize;
            int index1 = 0;
            int num14 = 0;
            float num15 = (float) (platform.latitudeCount / 10);
            for (int index2 = 0; index2 < reformSize * reformSize; ++index2) {
                ++num14;
                float num16 = (float) (((double) num7 + (double) num12) / 10.0);
                float _longitudeSeg = (float) (((double) num8 + (double) num13) / 10.0);
                num13 += 2;
                if (num14 % reformSize == 0) {
                    num13 = 1 - reformSize;
                    num12 += 2;
                }

                if ((double) num16 >= (double) num15 || (double) num16 <= -(double) num15) {
                    reformIndices[index2] = -1;
                }
                else {
                    int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(num16));
                    if (longitudeSegmentCount != PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment)) {
                        reformIndices[index2] = -1;
                    }
                    else {
                        int reformIndexForSegment = platform.GetReformIndexForSegment(num16, _longitudeSeg);


                        reformIndices[index2] = reformIndexForSegment;
                        int reformType1 = platform.GetReformType(reformIndexForSegment);
                        int reformColor1 = platform.GetReformColor(reformIndexForSegment);
                        if (!platform.IsTerrainReformed(reformType1) && (reformType1 != reformType || reformColor1 != reformColor)) {
                            float f7 = (float) ((double) num16 / (double) __instance.segment * 6.28318548202515);
                            float f8 = (float) ((double) _longitudeSeg / (double) num3 * 6.28318548202515);
                            float y2 = Mathf.Sin(f7);
                            float num17 = Mathf.Cos(f7);
                            float num18 = Mathf.Sin(f8);
                            float num19 = Mathf.Cos(f8);
                            reformPoints[index1] = new Vector3(num17 * num18, y2, num17 * -num19);
                            ++index1;
                        }
                    }
                }
            }

            __result =  index1;
            return false;
        }
    }
}