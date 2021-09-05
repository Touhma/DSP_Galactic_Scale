using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        public static Dictionary<int, Vector3[]> verDict = new Dictionary<int, Vector3[]>();
        public static Dictionary<int, int[]> inDict = new Dictionary<int, int[]>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "CalcVerts")]
        public static bool CalcVerts(ref PlanetRawData __instance, ref int[] ___indexMap80, ref int[] ___indexMap200, ref Vector3[] ___verts80, ref Vector3[] ___verts200)
        {
            if (verDict.ContainsKey(__instance.precision))
            {
                Array.Copy(verDict[__instance.precision], __instance.vertices, verDict[__instance.precision].Length);
                Array.Copy(inDict[__instance.precision], __instance.indexMap, inDict[__instance.precision].Length);
                return false;
            }

            for (var i = 0; i < __instance.indexMapDataLength; i++) //Set indexMap to [-1]
                __instance.indexMap[i] = -1;
            var stride = (__instance.precision + 1) * 2;
            var substride = __instance.precision + 1;
            for (var j = 0; j < __instance.dataLength; j++)
            {
                var num3 = j % stride;
                var num4 = j / stride;
                var num5 = num3 % substride;
                var num6 = num4 % substride;
                var num7 = ((num3 >= substride ? 1 : 0) + (num4 >= substride ? 1 : 0) * 2) * 2 + (num5 < num6 ? 1 : 0); //
                float num8 = num5 < num6 ? num5 : __instance.precision - num5;
                float num9 = num5 < num6 ? __instance.precision - num6 : num6;
                var num10 = __instance.precision - num9;
                num9 /= __instance.precision;
                num8 = !(num10 > 0f) ? 0f : num8 / num10;
                var num11 = 0;
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

                ref var reference = ref __instance.vertices[j];
                reference = Vector3.Slerp(Vector3.Slerp(a, b, num9), Vector3.Slerp(a2, b, num9), num8);
                var num12 = __instance.PositionHash(__instance.vertices[j], num11);
                if (__instance.indexMap[num12] == -1) __instance.indexMap[num12] = j;
            }

            var num13 = 0;
            for (var k = 1; k < __instance.indexMapDataLength; k++)
                if (__instance.indexMap[k] == -1)
                {
                    __instance.indexMap[k] = __instance.indexMap[k - 1];
                    num13++;
                }

            if (!verDict.ContainsKey(__instance.precision))
            {
                var precision = __instance.precision;
                var vertLength = __instance.vertices.Length;
                var indexMapLength = __instance.indexMap.Length;
                verDict[precision] = new Vector3[vertLength];
                inDict[precision] = new int[indexMapLength];
                Array.Copy(__instance.vertices, verDict[precision], vertLength);
                Array.Copy(__instance.indexMap, inDict[precision], indexMapLength);
                return false;
            }

            return false;
        }
    }
}