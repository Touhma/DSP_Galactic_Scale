using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using ReworkPlanetGen = GalacticScale.Scripts.PatchPlanetSize.ReworkPlanetGen;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlatformSystem))]
    public class PatchOnPlatformSystem {
        
        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();
        [HarmonyPrefix]
        [HarmonyPatch("DetermineLongitudeSegmentCount")]
        public static bool DetermineLongitudeSegmentCount(int _latitudeIndex, int _segment, ref int __result) {
            if (!DSPGame.IsMenuDemo) 
                if (Patch.EnableLimitedResizingFeature.Value)
                { 
                    Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                    if (keyedLUTs.ContainsKey(_segment)) {
                        Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount Key Existed.", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        int index = Mathf.Abs(_latitudeIndex) % (_segment / 2);
                        if (index >= _segment / 4) {
                            index = _segment / 4 - index;
                        }
                        Patch.Debug("PlatformSystem Vanilla DeterminLongitudeSegmentCount fetched " + keyedLUTs[_segment][index] + " for segments " + _segment + " at index " + _latitudeIndex + "(" + index + ")", LogLevel.Debug, Patch.DebugNewPlanetGrid);
                        __result = keyedLUTs[_segment][index];
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
        [HarmonyPrefix]
        [HarmonyPatch("ComputeMaxReformCount")]
        public static bool ComputeMaxReformCount(ref PlatformSystem __instance, int _segment)
        {
            int num1 = __instance.latitudeCount / 2;
            int num2 = 0;
            for (int index = 0; index < __instance.latitudeCount + 1; ++index)
            {
                __instance.reformOffsets[index] = num2;
                if (index != __instance.latitudeCount)
                {
                    int _latitudeIndex = (index >= num1 ? index - num1 : index) / 5;
                    num2 += PlatformSystem.DetermineLongitudeSegmentCount(_latitudeIndex, _segment) * 5;
                }
                else
                    break;
            }
            Patch.Debug("Looking for LUT FOR " + _segment, LogLevel.Message, Patch.DebugReformFix);
            string s = "";
            Dictionary<int, int[]>.KeyCollection kc = keyedLUTs.Keys;
            foreach (int k in kc) s += k + " ";
            Patch.Debug("KEYEDLUTS: " + s, LogLevel.Message, Patch.DebugReformFix);
            if (!keyedLUTs.ContainsKey(_segment)) {
                ReworkPlanetGen.SetLuts(_segment, __instance.planet.radius);
            }
            if (keyedLUTs.ContainsKey(_segment))
            {
                int total = 0;
                foreach(int segCount in keyedLUTs[_segment])
                {
                    total += (segCount * 25 * 2);
                }
                __instance.maxReformCount = total;
            }
            else
            {
                __instance.maxReformCount = num2;
            }
            Patch.Debug("maxReformCount = " + __instance.maxReformCount, LogLevel.Message, Patch.DebugReformFix);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetReformType")]
        public static bool GetReformType (ref int __result, int index, ref PlatformSystem __instance) {
            int len = __instance.reformData.Length;
            if (index > len -1)
            {
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                byte[] tempArray = new byte[__instance.maxReformCount];
                Array.Copy(__instance.reformData, tempArray, len);
                __instance.reformData = tempArray;
            } else __result = (int)__instance.reformData[index] >> 5;
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("SetReformType")]
        public static bool SetReformType(int index, int type, ref PlatformSystem __instance)
        {
            int len = __instance.reformData.Length;
            if (index > len - 1)
            {
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                byte[] tempArray = new byte[__instance.maxReformCount];
                Array.Copy(__instance.reformData, tempArray, len);
                __instance.reformData = tempArray;
                __instance.reformData[index] = (byte)((type & 7) << 5 | (int)__instance.reformData[index] & 31);
            }
            else
                __instance.reformData[index] = (byte)((type & 7) << 5 | (int)__instance.reformData[index] & 31);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Export")]
        public static bool Export(BinaryWriter w, ref PlatformSystem __instance)
        {
            if (__instance.reformData != null)
            {
                int len = __instance.reformData.Length;
                __instance.ComputeMaxReformCount((int)(__instance.planet.radius / 4f + 0.01f) * 4);
                if (__instance.maxReformCount > len - 1)
                {
                    byte[] tempArray = new byte[__instance.maxReformCount];
                    Array.Copy(__instance.reformData, tempArray, len);
                    __instance.reformData = tempArray;
                }
            }
            return true;
        }
    }
}