using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlatformSystem))]
    public class PatchOnPlatformSystem {
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int _latitudeIndex, int _segment, ref int __result) {
            Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount.", LogLevel.Debug, true);
            if (keyedLUTs.ContainsKey(_segment)) {
                Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, true);
                int index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                if (index >= _segment / 4) {
                    index = _segment / 4 - index;
                }
                Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount fetched " + keyedLUTs[_segment][index] + " for segments " + _segment + " at index " + _latitudeIndex + "(" + index + ")", LogLevel.Debug, true);
                __result = keyedLUTs[_segment][index];
            }
            else {
                var index = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float) (_latitudeIndex / (double) (_segment / 4f) * 3.14159274101257 * 0.5))) * _segment);
                __result = index < 500 ? PlatformSystem.segmentTable[index] : (index + 49) / 100 * 100;
            }
            // Patch.Debug("PlatformSystem - _latitudeIndex --> " + _latitudeIndex, LogLevel.Debug, true);
            // Patch.Debug("PlatformSystem - _segment --> " + _segment, LogLevel.Debug, true);
            //CeilToInt --> RoundToInt
            //int index = Mathf.RoundToInt(Mathf.Abs(Mathf.Cos((_latitudeIndex / (_segment / 4f) * (float)Math.PI * 0.5f))) * _segment);
            // Patch.Debug("PlatformSystem - index --> " + index, LogLevel.Debug, true);
            //Patch.Debug("PlatformSystem - segmentTable --> " + PlatformSystem.segmentTable[index]  , LogLevel.Debug, true);
            //Patch.Debug("PlatformSystem - (index + 49) / 100 * 100  --> " + (index + 49) / 100 * 100  , LogLevel.Debug, true);
            // Patch.Debug("PlatformSystem - longitudeSegmentCount --> " + __result, LogLevel.Debug, true);

            return false;
        }

        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();

        /*
        public static bool GetReformIndexForPosition(ref PlatformSystem __instance, Vector3 pos, ref int __result) {
            pos.Normalize();
            var num1 = Mathf.Asin(pos.y); //angle latitude
            var num2 = Mathf.Atan2(pos.x, -pos.z); //angle longitude
            //float f1 = num1 / (2* (float)Math.PI) * (float) __instance.segment;
            var f1 = num1 / 6.283185f * __instance.segment; //angle latitude specific
            int latitudeSegmentIndex = Mathf.FloorToInt(Mathf.Abs(f1)); //
            float longitudeSegmentCount = PlatformSystem.DetermineLongitudeSegmentCount(latitudeSegmentIndex, __instance.segment);
            // float f2 = num2 / (2* (float)Math.PI)  * longitudeSegmentCount;
            var f2 = num2 / 6.283185f * longitudeSegmentCount; //Longitude specific index
            var f3 = Mathf.Round(f1 * 10f); //specific latitude square
            var f4 = Mathf.Round(f2 * 10f); //specific longitude square
            var num3 = Mathf.Abs(f3);
            var num4 = Mathf.Abs(f4);
            if (num3 % 2.0 != 1.0) {
                num3 = Mathf.FloorToInt(Mathf.Abs(f1) * 10f);
                if (num3 % 2.0 != 1.0)
                    ++num3;
            }
            var num5 = (double) f3 < 0.0 ? -num3 : num3;
            if (num4 % 2.0 != 1.0) {
                num4 = Mathf.FloorToInt(Mathf.Abs(f2) * 10f);
                if (num4 % 2.0 != 1.0)
                    ++num4;
            }
            var num6 = (double) f4 < 0.0 ? -num4 : num4;
            var _latitudeSeg = num5 / 10f;
            var _longitudeSeg = num6 / 10f;

            float num7 = __instance.latitudeCount / 10;
            __result = (double) _latitudeSeg >= (double) num7 || (double) _latitudeSeg <= -(double) num7 ? -1 : __instance.GetReformIndexForSegment(_latitudeSeg, _longitudeSeg);

            Patch.Debug("PlatformSystem - __result --> " + __result, LogLevel.Debug, true);

            return false;
        }
        */
    }
}