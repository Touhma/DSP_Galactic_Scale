using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlatformSystem))]
    public class PatchOnPlatformSystem {
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int _latitudeIndex, int _segment, ref int __result) {
            int index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) ((double) _latitudeIndex / (double) ((float) _segment / 4f) * 3.14159274101257 * 0.5))) * (float) _segment);
            __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
            Patch.Debug("PlatformSystem - longitudeSegmentCount" + __result, LogLevel.Debug, true);
            return false;
        }
    }
}