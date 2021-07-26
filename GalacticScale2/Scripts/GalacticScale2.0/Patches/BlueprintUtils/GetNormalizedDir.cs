using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBlueprintUtils
    {
        [HarmonyPrefix, HarmonyPatch(typeof(BlueprintUtils), "GetNormalizedDir", new Type[] {typeof(Vector3), typeof(int)})]
        public static bool GetNormalizedDir(ref Vector3 __result, Vector3 pos, int _segmentCnt = 200)
        {
            float snappedLongitudeRad = BlueprintUtils.GetSnappedLongitudeRad(pos, _segmentCnt);
            float latitudeRad = BlueprintUtils.GetSnappedLatitudeRad(pos, _segmentCnt);
            float latitudeRad2 = BlueprintUtils.GetLatitudeRad(pos);
            float num = BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt) * 0.5f;
            if (latitudeRad2 > 1.5707964f - num || latitudeRad2 < -1.5707964f + num)
            {
                if (latitudeRad2 > 1.5707964f - num)
                {
                    latitudeRad = 1.5702964f;
                }
                if (latitudeRad2 < -1.5707964f + num)
                {
                    latitudeRad = -1.5702964f;
                }
            }
            __result = BlueprintUtils.GetDir(snappedLongitudeRad, latitudeRad);
            return false;
        }
    }
}