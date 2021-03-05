using System;
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
           // Patch.Debug("PlatformSystem - _latitudeIndex --> " + _latitudeIndex, LogLevel.Debug, true);
           // Patch.Debug("PlatformSystem - _segment --> " + _segment, LogLevel.Debug, true);
         //CeilToInt --> RoundToInt
           int index = Mathf.RoundToInt(Mathf.Abs(Mathf.Cos((float) ((double) _latitudeIndex / (double) ((float) _segment / 4f) *  3.14159274101257 * 0.5))) * (float) _segment);
           //int index = Mathf.RoundToInt(Mathf.Abs(Mathf.Cos((_latitudeIndex / (_segment / 4f) * (float)Math.PI * 0.5f))) * _segment);
           // Patch.Debug("PlatformSystem - index --> " + index, LogLevel.Debug, true);
            //Patch.Debug("PlatformSystem - segmentTable --> " + PlatformSystem.segmentTable[index]  , LogLevel.Debug, true);
            //Patch.Debug("PlatformSystem - (index + 49) / 100 * 100  --> " + (index + 49) / 100 * 100  , LogLevel.Debug, true);
            __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
            Patch.Debug("PlatformSystem - longitudeSegmentCount --> " + __result, LogLevel.Debug, true);

            return false;
        }

      
        public static bool GetReformIndexForPosition( ref PlatformSystem __instance , Vector3 pos, ref int __result)
        {
            pos.Normalize();
            float num1 = Mathf.Asin(pos.y);
            float num2 = Mathf.Atan2(pos.x, -pos.z);
            //float f1 = num1 / (2* (float)Math.PI) * (float) __instance.segment;
            float f1 = num1 / 6.283185f * (float) __instance.segment;
            float longitudeSegmentCount = (float) PlatformSystem.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(f1)), __instance.segment);
           // float f2 = num2 / (2* (float)Math.PI)  * longitudeSegmentCount;
            float f2 = num2 / 6.283185f * longitudeSegmentCount;
            float f3 = Mathf.Round(f1 * 10f);
            float f4 = Mathf.Round(f2 * 10f);
            float num3 = Mathf.Abs(f3);
            float num4 = Mathf.Abs(f4);
            if ((double) num3 % 2.0 != 1.0)
            {
                num3 = (float) Mathf.FloorToInt(Mathf.Abs(f1) * 10f);
                if ((double) num3 % 2.0 != 1.0)
                    ++num3;
            }
            float num5 = (double) f3 < 0.0 ? -num3 : num3;
            if ((double) num4 % 2.0 != 1.0)
            {
                num4 = (float) Mathf.FloorToInt(Mathf.Abs(f2) * 10f);
                if ((double) num4 % 2.0 != 1.0)
                    ++num4;
            }
            float num6 = (double) f4 < 0.0 ? -num4 : num4;
            float _latitudeSeg = num5 / 10f;
            float _longitudeSeg = num6 / 10f;

            float num7 = (float) (__instance.latitudeCount / 10);
            __result =  (double) _latitudeSeg >= (double) num7 || (double) _latitudeSeg <= -(double) num7 ? -1 : __instance.GetReformIndexForSegment(_latitudeSeg, _longitudeSeg);
            
            Patch.Debug("PlatformSystem - __result --> " + __result, LogLevel.Debug, true);

            return false;
        }
    }
}