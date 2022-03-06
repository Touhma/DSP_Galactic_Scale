using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBlueprintUtils
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetLongitudeSegmentCount", typeof(float), typeof(int))]
        public static bool GetLongitudeSegmentCount(ref int __result, float _latitudeRad, int _segmentCnt = 200)
        {
            // GS2.Warn($"SegmentCount: {_segmentCnt}");
            var num = BlueprintUtils.GetSnappedLatitudeGridIdx(_latitudeRad, _segmentCnt);
            if (num < 0) num = -num;
            if (num > 0) num--;
            __result = PlanetGrid.DetermineLongitudeSegmentCount(num / 5, _segmentCnt);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetLongitudeSegmentCount", typeof(Vector3), typeof(int))]
        public static bool GetLongitudeSegmentCount(ref int __result, Vector3 _npos, int _segmentCnt = 200)
        {
            // GS2.Warn($"*SegmentCount: {_segmentCnt}");
            var num = BlueprintUtils.GetSnappedLatitudeGridIdx(BlueprintUtils.GetLatitudeRad(_npos), _segmentCnt);
            // GS2.Warn($"*Num: {num}");
            if (num < 0) num = -num;
            if (num > 0) num--;
            __result = PlanetGrid.DetermineLongitudeSegmentCount(num / 5, _segmentCnt);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetLongitudeSegmentCount", typeof(int), typeof(int))]
        public static bool GetLongitudeSegmentCount(ref int __result,int _latitudeGridIdx, int _segmentCnt = 200)
        {
            // GS2.Warn($"**SegmentCount: {_segmentCnt}");
            // GS2.Warn($"*_latitudeGridIdx: {_latitudeGridIdx}");

            if (_latitudeGridIdx < 0)
            {
                _latitudeGridIdx = -_latitudeGridIdx;
            }
            if (_latitudeGridIdx > 0)
            {
                _latitudeGridIdx--;
            }
            __result = PlanetGrid.DetermineLongitudeSegmentCount(_latitudeGridIdx / 5, _segmentCnt);
            return false;
        }
        
    }
}