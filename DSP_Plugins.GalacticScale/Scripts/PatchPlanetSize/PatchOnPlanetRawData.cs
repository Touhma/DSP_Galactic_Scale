using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetRawData))]
    public class PatchOnPlanetRawData {
        [HarmonyPrefix]
        [HarmonyPatch("GetModPlane")]
        public static bool GetModPlane(int index, ref PlanetRawData __instance, ref short __result) {
            Patch.Debug("scaleFactor " + __instance.GetFactoredScale(), LogLevel.Debug,
                Patch.DebugGetModPlane);

            Patch.Debug("index " + index, LogLevel.Debug,
                Patch.DebugGetModPlane); 

            Patch.Debug("__instance.modData.Length " + __instance.modData.Length, LogLevel.Debug,
                Patch.DebugGetModPlane);

            Patch.Debug(
                "test " + ((__instance.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133, LogLevel.Debug,
                Patch.DebugGetModPlane);

            float baseHeight = 20;

            baseHeight += __instance.GetFactoredScale() * 200 * 100;

            Patch.Debug("baseHeight " + baseHeight, LogLevel.Debug,
                Patch.DebugGetModPlane);
            __result = (short) (((__instance.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 +
                                baseHeight);

            Patch.Debug("GetModPlane __result " + __result, LogLevel.Debug,
                Patch.DebugGetModPlane);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetModLevel")]
        public static bool GetModLevel(int index, ref PlanetRawData __instance, ref int __result)
        {
            try // try-catch block probably unnecessary, left in for debugging use in future
            {
                __result = __instance.modData[index >> 1] >> ((index & 1) << 2) & 3;
                return false;
            }
            catch (Exception e)
            {
                Patch.Debug("modData Index " + index + " doesn't exist: " + e, LogLevel.Error, true);
                return false;
            }

        }
        [HarmonyPrefix]
        [HarmonyPatch("InitModData")]
        public static bool InitModData(byte[] refModData, ref PlanetRawData __instance, ref byte[] __result)
        {
            Patch.Debug(__instance.GetFactoredScale() + "InitModData " + (refModData == null) + " " + (__instance.dataLength), LogLevel.Message, Patch.DebugGetModPlane);
            __instance.modData = refModData == null ? new byte[__instance.dataLength] : refModData; // changed from .dataLength/2, fixes issue where array can't fit all the data. Shad0wlife is going to take a look and see why it's trying to, but this works for now
            __result = __instance.modData;
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch("QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance,
            ref float __result, Vector3 vpos) {
            Patch.Debug("QueryModifiedHeight ", LogLevel.Debug,
                Patch.DebugPlanetRawData);
            vpos.Normalize();
            var index1 = __instance.indexMap[__instance.PositionHash(vpos)];
            var radiusPrecision =
                (float) (3.14159274101257 / (__instance.precision * 2) * 1.20000004768372);

            Patch.Debug("radiusPrecision " + radiusPrecision, LogLevel.Debug,
                Patch.DebugPlanetRawData);
            var radiusPrecisionSq = radiusPrecision * radiusPrecision;

            Patch.Debug("radiusPrecisionSq " + radiusPrecisionSq, LogLevel.Debug,
                Patch.DebugPlanetRawData);
            var magnetudeOnPrecisionDummy = 0.0f;
            var HeightTimePrecision = 0.0f;
            var stride = __instance.stride;
            //Stride is 62 for small planets, 322 for large planets
            for (var index2 = -1; index2 <= 3; ++index2)
            for (var index3 = -1; index3 <= 3; ++index3) {
                var index4 = index1 + index2 + index3 * stride;
                if ((uint) index4 < __instance.dataLength) {
                    var sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;

                    Patch.Debug("sqrMagnitude " + sqrMagnitude, LogLevel.Debug,
                        Patch.DebugPlanetRawData);
                    if (sqrMagnitude <= (double) radiusPrecisionSq) {
                        var magnetudeOnPrecision =
                            (float) (1.0 - Mathf.Sqrt(sqrMagnitude) / (double) radiusPrecision);

                        Patch.Debug("MagnetudeOnPrecision " + magnetudeOnPrecision, LogLevel.Debug,
                            Patch.DebugPlanetRawData);
                        magnetudeOnPrecision *= __instance.GetFactoredScale();

                        Patch.Debug("MagnetudeOnPrecision Patched " + magnetudeOnPrecision,
                            LogLevel.Debug,
                            Patch.DebugPlanetRawData);
                        var modLevel = __instance.GetModLevel(index4);
                        float heightDataFinal = __instance.heightData[index4];

                        Patch.Debug("heightDataFinal First " + heightDataFinal, LogLevel.Debug,
                            Patch.DebugPlanetRawData);
                        if (modLevel > 0) {
                            // try patching here
                            var modPlane = __instance.GetModPlaneInt(index4);

                            Patch.Debug("modPlane " + modPlane, LogLevel.Debug,
                                Patch.DebugPlanetRawData);
                            if (modLevel == 3) {
                                heightDataFinal = modPlane;

                                Patch.Debug("heightDataFinal Second " + heightDataFinal, LogLevel.Debug,
                                    Patch.DebugPlanetRawData);
                            }
                            else {
                                var num7 = modLevel * 0.3333333f;
                                heightDataFinal =
                                    (float) (__instance.heightData[index4] * (1.0 - num7) +
                                             modPlane * (double) num7);

                                Patch.Debug("heightDataFinal Third " + heightDataFinal, LogLevel.Debug,
                                    Patch.DebugPlanetRawData);
                            }
                        }

                        Patch.Debug("__result num6 " + heightDataFinal, LogLevel.Debug,
                            Patch.DebugPlanetRawData);

                        magnetudeOnPrecisionDummy += magnetudeOnPrecision;
                        HeightTimePrecision += heightDataFinal * magnetudeOnPrecision;

                        Patch.Debug("heightDataFinal Third " + heightDataFinal, LogLevel.Debug,
                            Patch.DebugPlanetRawData);
                    }
                }
            }

            if (magnetudeOnPrecisionDummy != 0.0) {
                __result = (float) (HeightTimePrecision / (double) magnetudeOnPrecisionDummy *
                                    0.00999999977648258);

                Patch.Debug("__result magnetudeOnPrecisionDummy" + __result, LogLevel.Debug,
                    Patch.DebugPlanetRawData);
                return false;
            }

            Debug.LogWarning("bad query");
            __result = __instance.heightData[0] * 0.01f;

            Patch.Debug("__result bad query" + __result, LogLevel.Debug,
                Patch.DebugPlanetRawData);

            return false;
        }
        //[HarmonyPrefix]
        //[HarmonyPatch("QueryHeight")]
        //public float QueryHeight(Vector3 vpos)
        //{
        //    vpos.Normalize();
        //    int index1 = this.indexMap[this.PositionHash(vpos)];
        //    float num1 = (float)(3.14159274101257 / (double)(this.precision * 2) * 1.20000004768372);
        //    float num2 = num1 * num1;
        //    float num3 = 0.0f;
        //    float num4 = 0.0f;
        //    int stride = this.stride;
        //    for (int index2 = -1; index2 <= 3; ++index2)
        //    {
        //        for (int index3 = -1; index3 <= 3; ++index3)
        //        {
        //            int index4 = index1 + index2 + index3 * stride;
        //            if ((long)(uint)index4 < (long)this.dataLength)
        //            {
        //                float sqrMagnitude = (this.vertices[index4] - vpos).sqrMagnitude;
        //                if ((double)sqrMagnitude <= (double)num2)
        //                {
        //                    float num5 = (float)(1.0 - (double)Mathf.Sqrt(sqrMagnitude) / (double)num1);
        //                    float num6 = (float)this.heightData[index4];
        //                    num3 += num5;
        //                    num4 += num6 * num5;
        //                }
        //            }
        //        }
        //    }
        //    if ((double)num3 != 0.0)
        //        return (float)((double)num4 / (double)num3 * 0.00999999977648258);
        //    Debug.LogWarning((object)"bad query");
        //    return (float)this.heightData[0] * 0.01f;
        //}
    }
}