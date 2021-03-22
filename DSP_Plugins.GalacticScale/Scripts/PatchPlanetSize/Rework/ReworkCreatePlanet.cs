using System;
using BepInEx.Logging;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    public static class ReworkPlanetGen {
        public static void SetLuts(int segments, float planetRadius) {
            if (!DSPGame.IsMenuDemo && (Patch.EnableResizingFeature.Value || Patch.EnableLimitedResizingFeature.Value)) { // Prevent special LUT's being created in main menu
                if (PatchOnPlanetGrid.keyedLUTs.ContainsKey(segments) && PatchOnPlatformSystem.keyedLUTs.ContainsKey(segments) && PatchUIBuildingGrid.LUT512.ContainsKey(segments)) {
                    return;
                }
                Patch.Debug("Setting Planet LUTs for size " + planetRadius, LogLevel.Debug, Patch.DebugNewPlanetGrid);
                int numSegments = segments / 4;
                int[] lut = new int[numSegments];
                float segmentAngle = (Mathf.PI / 2f) / numSegments;

                float lastMajorRadius = planetRadius;
                int lastMajorRadiusCount = numSegments * 4;

                int[] classicLUT = new int[512];
                classicLUT[0] = 1;

                for (int cnt = 0; cnt < numSegments; cnt++) {
                    float segmentXAngle = (Mathf.PI / 2f) - (cnt * segmentAngle);
                    float segmentLineHeight = Mathf.Cos(segmentXAngle);
                    float segmentCylinderHeight = segmentLineHeight * planetRadius * 2;

                    float ringradius = Mathf.Sqrt((planetRadius * planetRadius) - ((segmentCylinderHeight * segmentCylinderHeight) / 4.0f));
                    int classicIdx = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) ((cnt + 1) / (segments / 4f) * Math.PI * 0.5))) * (float) segments);

                    if (ringradius < (0.9 * lastMajorRadius)) {
                        lastMajorRadius = ringradius;
                        lastMajorRadiusCount = (int) (ringradius / 4.0) * 4;
                    }
                    lut[cnt] = lastMajorRadiusCount;
                    classicLUT[classicIdx] = lastMajorRadiusCount;
                }

                int last = 1;
                for (int oldlLutIdx = 1; oldlLutIdx < 512; oldlLutIdx++) {
                    if (classicLUT[oldlLutIdx] > last) {
                        //Offset of 1 is required to avoid mismatch of some longitude circles
                        int temp = classicLUT[oldlLutIdx];
                        classicLUT[oldlLutIdx] = last;
                        last = temp;
                    }
                    else {
                        classicLUT[oldlLutIdx] = last;
                    }
                }

                //Fill all Look Up Tables (Dictionaries really)
                if (!PatchOnPlanetGrid.keyedLUTs.ContainsKey(segments)) {
                    PatchOnPlanetGrid.keyedLUTs.Add(segments, lut);
                }
                if (!PatchOnPlatformSystem.keyedLUTs.ContainsKey(segments)) {
                    PatchOnPlatformSystem.keyedLUTs.Add(segments, lut);
                }
                if (!PatchUIBuildingGrid.LUT512.ContainsKey(segments)) {
                    PatchUIBuildingGrid.LUT512.Add(segments, classicLUT);
                }
            }
        }

        private static void DebugClassicLut(int[] classicLUT) {

            Patch.Debug("Classic LUT:", LogLevel.Debug, Patch.DebugNewPlanetGrid);
            for (int a = 0; a < 32; a++) {
                string str = "";
                for (int b = 0; b < 16; b++) {
                    str += classicLUT[a * 16 + b] + ", ";
                }
                Patch.Debug(str, LogLevel.Debug, true);
            }
        }
    }
}