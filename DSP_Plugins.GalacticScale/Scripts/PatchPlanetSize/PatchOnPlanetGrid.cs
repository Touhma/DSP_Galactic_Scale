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
                    var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (latitudeIndex / (double) (segment / 4f) * 3.14159274101257 * 0.5))) * segment);
                    __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
                }

                return false;
            }
            return true;
        }



    }
}