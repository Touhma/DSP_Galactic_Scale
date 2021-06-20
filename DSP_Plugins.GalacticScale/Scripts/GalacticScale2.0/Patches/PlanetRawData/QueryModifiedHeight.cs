using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetRawData), "QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance,
        ref float __result, Vector3 vpos)
        {
            //GS2.Warn((__instance == null).ToString());
            vpos.Normalize();
            int index1 = __instance.indexMap[__instance.PositionHash(vpos, 0)];
            float radiusPrecision = 3.1415927f / (__instance.precision * 2) * 1.2f;
            float radiusPrecisionSq = radiusPrecision * radiusPrecision;
            float magnitudeOnPrecisionDummy = 0f;
            float HeightTimePrecision = 0f;
            int stride = __instance.stride;
            for (int index2 = -1; index2 <= 3; index2++)
            {
                for (int index3 = -1; index3 <= 3; index3++)
                {
                    int index4 = index1 + index2 + index3 * stride;
                    if ((ulong)index4 < (ulong)__instance.dataLength)
                    {
                        float sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;
                        if (sqrMagnitude <= radiusPrecisionSq)
                        {
                            float magnitudeOnPrecision = 1f - Mathf.Sqrt(sqrMagnitude) / radiusPrecision;
                            magnitudeOnPrecision *= __instance.GetFactoredScale();
                            int modLevel = __instance.GetModLevel(index4);
                            float HeightDataFinal = __instance.heightData[index4];
                            if (modLevel > 0)
                            {
                                float modPlane = __instance.GetModPlaneInt(index4);
                                if (modLevel == 3)
                                {
                                    HeightDataFinal = modPlane;
                                }
                                else
                                {
                                    float num11 = modLevel * 0.3333333f;
                                    HeightDataFinal = __instance.heightData[index4] * (1f - num11) + modPlane * num11;
                                }
                            }
                            magnitudeOnPrecisionDummy += magnitudeOnPrecision;
                            HeightTimePrecision += HeightDataFinal * magnitudeOnPrecision;
                        }
                    }
                }
            }

            if (magnitudeOnPrecisionDummy == 0f)
            {
                GS2.Warn("Bad Query");
                __result = __instance.heightData[0] * 0.01f;
                return false;
            }
            __result = HeightTimePrecision / magnitudeOnPrecisionDummy * 0.01f;
            return false;
        }
        //public static bool oldQueryModifiedHeight(ref PlanetRawData __instance,
        //    ref float __result, Vector3 vpos)
        //{
        //    GS2.Warn((__instance == null).ToString());
        //    vpos.Normalize();
        //    var index1 = __instance.indexMap[__instance.PositionHash(vpos)];
        //    //GS2.Warn($"InstancePrecision: {__instance.precision}");
        //    var radiusPrecision = (float)(3.14159274101257 / (__instance.precision * 2) * 1.20000004768372);
        //    var radiusPrecisionSq = radiusPrecision * radiusPrecision;
        //    var magnetudeOnPrecisionDummy = 0.0f;
        //    var HeightTimePrecision = 0.0f;
        //    var stride = __instance.stride;
        //    for (var index2 = -1; index2 <= 3; ++index2)
        //        for (var index3 = -1; index3 <= 3; ++index3)
        //        {
        //            var index4 = index1 + index2 + index3 * stride;
        //            if ((uint)index4 < __instance.dataLength)
        //            {
        //                var sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;
        //                if (sqrMagnitude <= (double)radiusPrecisionSq)
        //                {
        //                    var magnetudeOnPrecision =
        //                        (float)(1.0 - Mathf.Sqrt(sqrMagnitude) / (double)radiusPrecision);
        //                    magnetudeOnPrecision *= __instance.GetFactoredScale();
        //                    var modLevel = __instance.GetModLevel(index4);
        //                    float heightDataFinal = __instance.heightData[index4];
        //                    if (modLevel > 0)
        //                    {
        //                        // try patching here
        //                        var modPlane = __instance.GetModPlaneInt(index4);
        //                        if (modLevel == 3)
        //                        {
        //                            heightDataFinal = modPlane;
        //                        }
        //                        else
        //                        {
        //                            var num7 = modLevel * 0.3333333f;
        //                            heightDataFinal =
        //                                (float)(__instance.heightData[index4] * (1.0 - num7) +
        //                                         modPlane * (double)num7);
        //                        }
        //                    }
        //                    magnetudeOnPrecisionDummy += magnetudeOnPrecision;
        //                    HeightTimePrecision += heightDataFinal * magnetudeOnPrecision;
        //                }
        //            }
        //        }

        //    if (magnetudeOnPrecisionDummy != 0.0)
        //    {
        //        __result = (float)(HeightTimePrecision / (double)magnetudeOnPrecisionDummy *
        //                            0.00999999977648258);
        //        GS2.Warn("Completed");
        //        return false;
        //    }

        //    GS2.Error($"Bad Query {magnetudeOnPrecisionDummy}");
        //    __result = __instance.heightData[0] * 0.01f;

        //    return false;
        //}
    }
}