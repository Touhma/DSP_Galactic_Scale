using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBlueprintUtils
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetNormalizedPos", typeof(Vector3), typeof(int))]
        public static bool GetNormalizedPos(ref Vector3 __result, Vector3 _npos, int _segmentCnt = 200)
        {
            var snappedLongitudeRad = BlueprintUtils.GetSnappedLongitudeRad(_npos, _segmentCnt);
            var latitudeRad = BlueprintUtils.GetSnappedLatitudeRad(_npos, _segmentCnt);
            var latitudeRad2 = BlueprintUtils.GetLatitudeRad(_npos);
            var num = BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt) * 0.5f;
            if (latitudeRad2 > 1.5707964f - num || latitudeRad2 < -1.5707964f + num)
            {
                if (latitudeRad2 > 1.5707964f - num) latitudeRad = 1.5702964f;
                if (latitudeRad2 < -1.5707964f + num) latitudeRad = -1.5702964f;
            }

            __result = BlueprintUtils.GetDir(snappedLongitudeRad, latitudeRad);
            return false;
        }
    }
}