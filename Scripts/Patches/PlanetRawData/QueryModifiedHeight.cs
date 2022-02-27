using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance, ref float __result, Vector3 vpos)
        {
            //GS2.Warn((__instance == null).ToString());
            vpos.Normalize();
            var index1 = __instance.indexMap[__instance.PositionHash(vpos)];
            var radiusPrecision = 3.1415927f / (__instance.precision * 2) * 1.2f;
            var radiusPrecisionSq = radiusPrecision * radiusPrecision;
            var magnitudeOnPrecisionDummy = 0f;
            var HeightTimePrecision = 0f;
            var stride = __instance.stride;
            for (var index2 = -1; index2 <= 3; index2++)
            for (var index3 = -1; index3 <= 3; index3++)
            {
                var index4 = index1 + index2 + index3 * stride;
                if ((ulong)index4 < (ulong)__instance.dataLength)
                {
                    var sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;
                    if (sqrMagnitude <= radiusPrecisionSq)
                    {
                        var magnitudeOnPrecision = 1f - Mathf.Sqrt(sqrMagnitude) / radiusPrecision;
                        magnitudeOnPrecision *= __instance.GetFactoredScale();
                        var modLevel = __instance.GetModLevel(index4);
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
                                var num11 = modLevel * 0.3333333f;
                                HeightDataFinal = __instance.heightData[index4] * (1f - num11) + modPlane * num11;
                            }
                        }

                        magnitudeOnPrecisionDummy += magnitudeOnPrecision;
                        HeightTimePrecision += HeightDataFinal * magnitudeOnPrecision;
                    }
                }
            }

            if (magnitudeOnPrecisionDummy == 0f)
            {
                GS2.Log($"Bad Query. LocalPlanet:{GameMain.localPlanet.name} Vector:{vpos}");
                __result = __instance.heightData[0] * 0.01f;
                return false;
            }

            __result = HeightTimePrecision / magnitudeOnPrecisionDummy * 0.01f;
            return false;
        }
    }
}