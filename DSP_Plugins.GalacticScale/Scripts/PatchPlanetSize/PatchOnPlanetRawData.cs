using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System;
using System.Collections.Generic;

namespace GalacticScale.Scripts.PatchPlanetSize {
    
    [HarmonyPatch(typeof(PlanetRawData))]
    public class PatchOnPlanetRawData {
        public static Dictionary<int, Vector3[]> verDict = new Dictionary<int, Vector3[]>();
        public static Dictionary<int, int[]> inDict = new Dictionary<int, int[]>();
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
            Patch.Debug(__instance.GetFactoredScale() + "InitModData " + (refModData == null) + " " + (__instance.dataLength), LogLevel.Debug, Patch.DebugGetModPlane);
            __instance.modData = refModData == null ? new byte[__instance.dataLength/2] : refModData; // changed from .dataLength/2, fixes issue where array can't fit all the data. Shad0wlife is going to take a look and see why it's trying to, but this works for now -innominata
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
        [HarmonyPrefix, HarmonyPatch("CalcVerts")]
        public static bool CalcVerts(ref PlanetRawData __instance, ref int[] ___indexMap80, ref int[] ___indexMap200, ref Vector3[] ___verts80, ref Vector3[] ___verts200)
        {
            if (verDict.ContainsKey(__instance.precision))
            {
                Patch.Debug("Loading Saved Data for precision " + __instance.precision, LogLevel.Debug,
               Patch.DebugPlanetRawData);
                Array.Copy(verDict[__instance.precision], __instance.vertices, verDict[__instance.precision].Length);
                Array.Copy(inDict[__instance.precision], __instance.indexMap, inDict[__instance.precision].Length);
                Patch.Debug("Loaded " + __instance.precision, LogLevel.Debug, Patch.DebugPlanetRawData);
                return false;
            }

            for (int i = 0; i < __instance.indexMapDataLength; i++) //Set indexMap to [-1]
            {
                __instance.indexMap[i] = -1;
            }
            int stride = (__instance.precision + 1) * 2;
            int substride = __instance.precision + 1;
            for (int j = 0; j < __instance.dataLength; j++)
            {
                int num3 = j % stride;
                int num4 = j / stride;
                int num5 = num3 % substride;
                int num6 = num4 % substride;
                int num7 = (((num3 >= substride) ? 1 : 0) + ((num4 >= substride) ? 1 : 0) * 2) * 2 + ((num5 < num6) ? 1 : 0); //
                float num8 = ((num5 < num6) ? num5 : (__instance.precision - num5));
                float num9 = ((num5 < num6) ? (__instance.precision - num6) : num6);
                float num10 = (float)__instance.precision - num9;
                num9 /= (float)__instance.precision;
                num8 = ((!(num10 > 0f)) ? 0f : (num8 / num10));
                int num11 = 0;
                Vector3 a;
                Vector3 a2;
                Vector3 b;
                switch (num7)
                {
                    case 0:
                        a = PlanetRawData.poles[2];
                        a2 = PlanetRawData.poles[0];
                        b = PlanetRawData.poles[4];
                        num11 = 7;
                        break;
                    case 1:
                        a = PlanetRawData.poles[3];
                        a2 = PlanetRawData.poles[4];
                        b = PlanetRawData.poles[0];
                        num11 = 5;
                        break;
                    case 2:
                        a = PlanetRawData.poles[2];
                        a2 = PlanetRawData.poles[4];
                        b = PlanetRawData.poles[1];
                        num11 = 6;
                        break;
                    case 3:
                        a = PlanetRawData.poles[3];
                        a2 = PlanetRawData.poles[1];
                        b = PlanetRawData.poles[4];
                        num11 = 4;
                        break;
                    case 4:
                        a = PlanetRawData.poles[2];
                        a2 = PlanetRawData.poles[1];
                        b = PlanetRawData.poles[5];
                        num11 = 2;
                        break;
                    case 5:
                        a = PlanetRawData.poles[3];
                        a2 = PlanetRawData.poles[5];
                        b = PlanetRawData.poles[1];
                        num11 = 0;
                        break;
                    case 6:
                        a = PlanetRawData.poles[2];
                        a2 = PlanetRawData.poles[5];
                        b = PlanetRawData.poles[0];
                        num11 = 3;
                        break;
                    case 7:
                        a = PlanetRawData.poles[3];
                        a2 = PlanetRawData.poles[0];
                        b = PlanetRawData.poles[5];
                        num11 = 1;
                        break;
                    default:
                        a = PlanetRawData.poles[2];
                        a2 = PlanetRawData.poles[0];
                        b = PlanetRawData.poles[4];
                        num11 = 7;
                        break;
                }
                ref Vector3 reference = ref __instance.vertices[j];
                reference = Vector3.Slerp(Vector3.Slerp(a, b, num9), Vector3.Slerp(a2, b, num9), num8);
                int num12 = __instance.PositionHash(__instance.vertices[j], num11);
                if (__instance.indexMap[num12] == -1)
                {
                    __instance.indexMap[num12] = j;
                }
            }
            int num13 = 0;
            for (int k = 1; k < __instance.indexMapDataLength; k++)
            {
                if (__instance.indexMap[k] == -1)
                {
                    __instance.indexMap[k] = __instance.indexMap[k - 1];
                    num13++;
                }
            }
            if (!verDict.ContainsKey(__instance.precision))
            {
                Patch.Debug("Saving VerDict "+__instance.precision, LogLevel.Debug, Patch.DebugPlanetRawData);
                int precision = __instance.precision;
                int vertLength = __instance.vertices.Length;
                int indexMapLength = __instance.indexMap.Length;
                verDict[precision] = new Vector3[vertLength];
                inDict[precision] = new int[indexMapLength];
                Array.Copy(__instance.vertices, verDict[precision], vertLength);
                Array.Copy(__instance.indexMap, inDict[precision], indexMapLength);
                Patch.Debug("Saved VerDict", LogLevel.Debug, Patch.DebugPlanetRawData);
                return false;
            }
            return false;
        }
    }
}
