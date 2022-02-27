using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlatformSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlatformSystem), "DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(PlatformSystem __instance, int _latitudeIndex, int _segment, ref int __result)
        {
            if (!DSPGame.IsMenuDemo)
                if (!GS2.Vanilla)
                {
                    if (!GS2.keyedLUTs.ContainsKey(_segment) && __instance.planet != null) GS2.SetLuts(_segment, __instance.planet.radius);
                    //GS2.Warn($"DetermineLongitudeSegmentCount(int _latitudeIndex:{_latitudeIndex}, int _segment:{_segment}, ref int __result:{__result})");
                    // GS2.Log($"{GS2.keyedLUTs.ContainsKey(_segment)}");
                    if (GS2.keyedLUTs.ContainsKey(_segment))
                    {
                        //GS2.Warn("GS2.keyedLUTs.ContainsKey(_segment) <-true");

                        var index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                        if (index >= _segment / 4) index = _segment / 4 - index;
                        // GS2.Log($"{index} {_segment} {_latitudeIndex} {__result}");
                        // GS2.WarnJson(PatchOnUIBuildingGrid.LUT512[_segment]);
                        __result = GS2.keyedLUTs[_segment][index];
                        //GS2.Warn($"Result:{__result}");
                    }
                    else
                    {
                        // GS2.Warn($"Using Original Algorithm");
                        //Original algorithm. Really shouldn't be used anymore... but just in case it's still here.
                        var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float)(_latitudeIndex / (double)(_segment / 4f) * 3.14159274101257 * 0.5))) * _segment);
                        __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
                    }

                    return false;
                }

            return true;
        }
    }
}