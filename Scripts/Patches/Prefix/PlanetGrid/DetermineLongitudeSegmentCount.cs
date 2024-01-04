using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.DetermineLongitudeSegmentCount))]
        public static bool DetermineLongitudeSegmentCount(PlanetGrid __instance, int latitudeIndex, int segment, ref int __result)
        {
            if (DSPGame.IsMenuDemo) return true;
               
            if (!GS3.keyedLUTs.ContainsKey(segment)) GS3.SetLuts(segment, segment);
            if (GS3.keyedLUTs.ContainsKey(segment))
            {
                var index = Mathf.Abs(latitudeIndex) % (segment / 2);

                if (index >= segment / 4) index = segment / 4 - index;
                if (index < 0 || GS3.keyedLUTs[segment].Length == 0)
                {
                    __result = 4;
                    return false;
                }

                if (index > GS3.keyedLUTs[segment].Length - 1) __result = GS3.keyedLUTs[segment][GS3.keyedLUTs[segment].Length - 1];
                else __result = GS3.keyedLUTs[segment][index];
                //a = 1 * 1;
            }
            else
            {
                //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                // GS3.Warn("Using original algo");
                var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float)(latitudeIndex / (double)(segment / 4f) * 3.14159274101257 * 0.5))) * segment);
                __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
            }
            if (__result < 4) __result = 4;
            return false;
        }
    }
}