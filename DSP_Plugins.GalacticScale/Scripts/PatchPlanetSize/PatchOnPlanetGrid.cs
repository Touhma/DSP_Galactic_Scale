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
        public static bool DetermineLongitudeSegmentCount(int latitudeIndex, int segment, ref int __result)
        {
            Patch.Debug("PlanetGrid Vanilla DeterminLongitudeSegmentCount.", LogLevel.Debug, true);
            if (keyedLUTs.ContainsKey(segment))
            {
                Patch.Debug("PlanetGrid Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, true);
                int index = Mathf.Abs(latitudeIndex) % (segment / 2);
                if (index >= segment / 4)
                {
                    index = segment / 4 - index;
                }

                Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount fetched " + keyedLUTs[segment][index] + " for segments " + segment + " at index " + latitudeIndex + "(" + index + ")", LogLevel.Debug, true);
                __result = keyedLUTs[segment][index];
            }
            else
            {
                //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float)(latitudeIndex / (double)(segment / 4f) * 3.14159274101257 * 0.5))) * segment);
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

    }
}