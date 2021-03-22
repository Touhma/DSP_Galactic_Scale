using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlatformSystem))]
    public class PatchOnPlatformSystem {
        
        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();
        
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int _latitudeIndex, int _segment, ref int __result) {
            if (!DSPGame.IsMenuDemo) 
                if (Patch.EnableResizingFeature.Value || Patch.EnableLimitedResizingFeature.Value) {
                    Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                    if (keyedLUTs.ContainsKey(_segment)) {
                        Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        int index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                        if (index >= _segment / 4) {
                            index = _segment / 4 - index;
                        }
                        Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount fetched " + keyedLUTs[_segment][index] + " for segments " + _segment + " at index " + _latitudeIndex + "(" + index + ")", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        __result = keyedLUTs[_segment][index];
                    }
                    else {
                        //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                        var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (_latitudeIndex / (double) (_segment / 4f) * 3.14159274101257 * 0.5))) * _segment);
                        __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
                    }
                    return false;
                }
            return true;
        }


    }
}