using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetGrid))]
    public class PatchOnPlanetGrid {

        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int latitudeIndex, int segment, ref int __result) {
            Patch.Debug("PlanetGrid Vanilla DeterminLongitudeSegmentCount.", LogLevel.Debug, true);
            if (keyedLUTs.ContainsKey(segment)) {
                Patch.Debug("PlanetGrid Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, true);
                int index = Mathf.Abs(latitudeIndex) % (segment / 2);
                if (index >= segment / 4) {
                    index = segment / 4 - index;
                }

                Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount fetched " + keyedLUTs[segment][index] + " for segments " + segment + " at index " + latitudeIndex + "(" + index + ")", LogLevel.Debug, true);
                __result = keyedLUTs[segment][index];
            }
            else {
                var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (latitudeIndex / (double) (segment / 4f) * 3.14159274101257 * 0.5))) * segment);
                __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
            }
            // Patch.Debug("PlanetGrid - _latitudeIndex --> " + latitudeIndex, LogLevel.Debug, true);
            //int index = Mathf.RoundToInt(Mathf.Abs(Mathf.Cos((float) ((double) latitudeIndex / (double) ((float) segment / 4f) * Math.PI * 0.5))) * (float) segment);
            //CeilToInt --> RoundToInt
            //var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (latitudeIndex / (double) (segment / 4f) * 3.14159274101257 * 0.5))) * segment);
            //Patch.Debug("PlanetGrid - index --> " + index, LogLevel.Debug, true);
            //__result = index < 500 ? PlanetGrid.segmentTable[index] : (index + 49) / 100 * 100;
            // Patch.Debug("PlanetGrid - longitudeSegmentCount" + __result, LogLevel.Debug, true);

            return false;
        }

        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();


        /*
        public static bool SnapTo(ref PlanetGrid __instance, Vector3 pos, ref Vector3 __result) {
            pos.Normalize();
            var num1 = Mathf.Asin(pos.y);
            var num2 = Mathf.Atan2(pos.x, -pos.z);
            var f1 = num1 / 6.283185f * __instance.segment;
            //  float f1 = num1 / (2 * (float)Math.PI) * (float) __instance.segment;
            float longitudeSegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(Mathf.RoundToInt(Mathf.Max(0.0f, Mathf.Abs(f1) - 0.1f)), __instance.segment);
            var num3 = num2 / 6.283185f * longitudeSegmentCount;
            //  float num3 = num2 / (2 * (float)Math.PI)* longitudeSegmentCount;
            var num4 = Mathf.Round(f1 * 5f) / 5f;
            var num5 = Mathf.Round(num3 * 5f) / 5f;
            //float f2 = (float) ((double) num4 / (double) __instance.segment * 2 * (float)Math.PI);
            //float f3 = (float) ((double) num5 / (double) longitudeSegmentCount *2 * (float)Math.PI);
            var f2 = (float) (num4 / (double) __instance.segment * 6.283185f);
            var f3 = (float) (num5 / (double) longitudeSegmentCount * 6.283185f);
            var y = Mathf.Sin(f2);
            var num6 = Mathf.Cos(f2);
            var num7 = Mathf.Sin(f3);
            var num8 = Mathf.Cos(f3);
            __result = new Vector3(num6 * num7, y, num6 * -num8);
            return false;
        }
        */

        /*
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
            out Vector3 reformCenter,
            ref int __result) {

            pos.Normalize();

            var num1 = Mathf.Asin(pos.y);
            var num2 = Mathf.Atan2(pos.x, -pos.z);
            //float f1 = num1 / ( 2 * (float)Math.PI) * (float) __instance.segment;
            var f1 = num1 / 6.283185f * __instance.segment; //latitiude segment.partial
            int latitudeIndexSegment = Mathf.FloorToInt(Mathf.Abs(f1));
            int longitudeSegmentCount = 0;
            try
            {
                longitudeSegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndexSegment, __instance.segment);
            }
            catch
            {
                Patch.Debug("Exception during LongitudeSegmentCount.", LogLevel.Debug, true);
            }
            


            float num3 = longitudeSegmentCount;
            var f2 = num2 / 6.283185f * num3; //longitude segment.partial
            //6.28318548202514648
            //float f2 = num2 / ( 2 * (float)Math.PI) * num3;
            var f3 = Mathf.Round(f1 * 10f);
            var f4 = Mathf.Round(f2 * 10f);
            var num4 = Mathf.Abs(f3);
            var num5 = Mathf.Abs(f4);
            var num6 = reformSize % 2;
            if (num4 % 2.0 != num6) {
                num4 = Mathf.FloorToInt(Mathf.Abs(f1) * 10f);
                if (num4 % 2.0 != num6)
                    ++num4;
            }

            var num7 = (double) f3 < 0.0 ? -num4 : num4;
            if (num5 % 2.0 != num6) {
                num5 = Mathf.FloorToInt(Mathf.Abs(f2) * 10f);
                if (num5 % 2.0 != num6)
                    ++num5;
            }


            var num8 = (double) f4 < 0.0 ? -num5 : num5;
            // float f5 = (float) ((double) num7 / 10.0 / (double) __instance.segment * 2 * (float)Math.PI);
            // float f6 = (float) ((double) num8 / 10.0 / (double) num3 * 2 * (float)Math.PI);
            var f5 = (float) (num7 / 10.0 / __instance.segment * 6.28318548202515);
            var f6 = (float) (num8 / 10.0 / num3 * 6.28318548202515);
            var y1 = Mathf.Sin(f5);
            var num9 = Mathf.Cos(f5);
            var num10 = Mathf.Sin(f6);
            var num11 = Mathf.Cos(f6);
            reformCenter = new Vector3(num9 * num10, y1, num9 * -num11);
            Patch.Debug("--------------------------------------", LogLevel.Debug, true);
            //Patch.Debug("Mouse Loc: " + GridPos(__instance.segment, pos.x, pos.y, pos.z), LogLevel.Debug, true);
            //Patch.Debug("Center Loc: " + GridPos(__instance.segment, reformCenter.x, reformCenter.y, reformCenter.z), LogLevel.Debug, true);
            var num12 = 1 - reformSize;
            var num13 = 1 - reformSize;
            var index1 = 0;
            var num14 = 0;
            float num15 = platform.latitudeCount / 10;
            for (var index2 = 0; index2 < reformSize * reformSize; ++index2) {
                ++num14;
                var num16 = (float) ((num7 + (double) num12) / 10.0);
                var _longitudeSeg = (float) ((num8 + (double) num13) / 10.0);

                num13 += 2;
                if (num14 % reformSize == 0) {
                    num13 = 1 - reformSize;
                    num12 += 2;
                }

                if (num16 >= (double) num15 || num16 <= -(double) num15) {
                    //TODO debug numbers to see what they are and what they do
                    Patch.Debug("First Indexing, idx " + index2, LogLevel.Debug, true);
                    try
                    {
                        reformIndices[index2] = -1;
                    }
                    catch
                    {
                        Patch.Debug("Exception during first Indexing.", LogLevel.Debug, true);
                    }
                }
                else {
                    int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(num16));

                    if (longitudeSegmentCount != PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment)) {

                        Patch.Debug("Second Indexing, idx " + index2, LogLevel.Debug, true);
                        try
                        {
                            reformIndices[index2] = -1;
                        }
                        catch
                        {
                            Patch.Debug("Exception during second Indexing.", LogLevel.Debug, true);
                        }
                    }
                    else {
                        int reformIndexForSegment = 0;
                        try
                        {
                            reformIndexForSegment = platform.GetReformIndexForSegment(num16, _longitudeSeg);
                        }
                        catch
                        {
                            Patch.Debug("Exception during GetReformIndexForSegment.", LogLevel.Debug, true);
                        }

                        Patch.Debug("Third Indexing, idx " + index2, LogLevel.Debug, true);
                        try
                        {
                            reformIndices[index2] = reformIndexForSegment;
                        }
                        catch
                        {
                            Patch.Debug("Exception during third Indexing.", LogLevel.Debug, true);
                        }



                        int reformType1 = 0;
                        int reformColor1 = 0;

                        try
                        {
                            reformType1 = platform.GetReformType(reformIndexForSegment);
                            reformColor1 = platform.GetReformColor(reformIndexForSegment);
                        }
                        catch
                        {
                            Patch.Debug("Exception during third GetReformType or -Color.", LogLevel.Debug, true);
                        }

                        if (!platform.IsTerrainReformed(reformType1) && (reformType1 != reformType || reformColor1 != reformColor)) {
                            var f7 = (float) (num16 / (double) __instance.segment * 6.28318548202515);
                            // float f7 = (float) ((double) num16 / (double) __instance.segment *  ( 2 * (float)Math.PI));
                            var f8 = (float) (_longitudeSeg / (double) num3 * 6.28318548202515);
                            //float f8 = (float) ((double) _longitudeSeg / (double) num3 *  ( 2 * (float)Math.PI));
                            var y2 = Mathf.Sin(f7);
                            var num17 = Mathf.Cos(f7);
                            var num18 = Mathf.Sin(f8);
                            var num19 = Mathf.Cos(f8);
                            try
                            {
                                reformPoints[index1] = new Vector3(num17 * num18, y2, num17 * -num19);
                                //Patch.Debug("ReformPoint Loc: " + GridPos(__instance.segment, reformPoints[index1].x, reformPoints[index1].y, reformPoints[index1].z), LogLevel.Debug, true);
                            }
                            catch
                            {
                                Patch.Debug("Exception during third Indexing.", LogLevel.Debug, true);
                            }
                            ++index1;
                        }
                    }
                }
            }

            __result = index1;

            return false;
        }
        */

        private static float LatitudeSegIndex(int segmentCount, float yPos) {
            return Mathf.Abs(Mathf.Asin(yPos) / 6.283185f * segmentCount);
        }

        private static float LongitudeIndex(int segmentCount, float xPos, float zPos, int latitudeSegmentIndex) {
            var longitudeSegmentCount = PlanetGrid.DetermineLongitudeSegmentCount(latitudeSegmentIndex, segmentCount);
            return Mathf.Atan2(xPos, -zPos) / 6.283185f * (float) longitudeSegmentCount;
        }

        private static string GridPos(int segmentCount, float xPos, float yPos, float zPos) {
            float lat = LatitudeSegIndex(segmentCount, yPos);
            int latIndex = Mathf.FloorToInt(lat);
            float lon = LongitudeIndex(segmentCount, xPos, zPos, latIndex);
            int lonIndex = Mathf.FloorToInt(lon);

            int remLat = Mathf.FloorToInt((lat - latIndex) * 5);
            int remLon = Mathf.FloorToInt((lon - lonIndex) * 5);

            return string.Format("Segment Lat{0}, Sq.{1}; Segment Lon{2}, Sq.{3}", latIndex, remLat, lonIndex, remLon);
        }

    }
}