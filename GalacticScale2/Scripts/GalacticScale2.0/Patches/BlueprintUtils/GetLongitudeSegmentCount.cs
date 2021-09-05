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
            var num = BlueprintUtils.GetSnappedLatitudeGridIdx(BlueprintUtils.GetLatitudeRad(_npos), _segmentCnt);
            if (num < 0) num = -num;
            if (num > 0) num--;
            __result = PlanetGrid.DetermineLongitudeSegmentCount(num / 5, _segmentCnt);
            return false;
        }
    }
}