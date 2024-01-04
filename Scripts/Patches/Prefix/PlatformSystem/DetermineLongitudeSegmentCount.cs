using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), nameof(PlatformSystem.DetermineLongitudeSegmentCount))]
        public static bool DetermineLongitudeSegmentCount(PlatformSystem __instance, int _latitudeIndex, int _segment, ref int __result)
        {
            if (!DSPGame.IsMenuDemo)
            {

                if (!GS3.keyedLUTs.ContainsKey(_segment) && __instance.planet != null) GS3.SetLuts(_segment, __instance.planet.radius);
                //GS3.Warn($"DetermineLongitudeSegmentCount(int _latitudeIndex:{_latitudeIndex}, int _segment:{_segment}, ref int __result:{__result})");
                // GS3.Log($"{GS3.keyedLUTs.ContainsKey(_segment)}");
                if (GS3.keyedLUTs.ContainsKey(_segment))
                {
                    //GS3.Warn("GS3.keyedLUTs.ContainsKey(_segment) <-true");

                    var index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                    if (index >= _segment / 4) index = _segment / 4 - index;
                    // GS3.Log($"{index} {_segment} {_latitudeIndex} {__result}");
                    // GS3.WarnJson(PatchOnUIBuildingGrid.LUT512[_segment]);
                    __result = GS3.keyedLUTs[_segment][index];
                    //GS3.Warn($"Result:{__result}");
                }
                else
                {
                    // GS3.Warn($"Using Original Algorithm");
                    //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                    var index = Mathf.CeilToInt(
                        Mathf.Abs(Mathf.Cos((float)(_latitudeIndex / (double)(_segment / 4f) * 3.14159274101257 *
                                                    0.5))) * _segment);
                    __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
                }

                return false;
            }

            return true;
        }
    }
}