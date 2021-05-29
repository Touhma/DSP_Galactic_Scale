using HarmonyLib;
using UnityEngine;

namespace GalacticScale {
    public partial class PatchOnPlatformSystem {
        [HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int _latitudeIndex, int _segment, ref int __result) {
            if (!DSPGame.IsMenuDemo) 
                if (!GS2.Vanilla)
                { 
                    if (GS2.keyedLUTs.ContainsKey(_segment)) {
                        int index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                        if (index >= _segment / 4) {
                            index = _segment / 4 - index;
                        }
                        __result = GS2.keyedLUTs[_segment][index];
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