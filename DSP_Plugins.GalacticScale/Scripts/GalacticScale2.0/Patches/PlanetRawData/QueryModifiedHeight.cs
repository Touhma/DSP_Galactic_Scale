using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetRawData), "QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance,
            ref float __result, Vector3 vpos) {
            vpos.Normalize();
            var index1 = __instance.indexMap[__instance.PositionHash(vpos)];
            var radiusPrecision =
                (float) (3.14159274101257 / (__instance.precision * 2) * 1.20000004768372);
            var radiusPrecisionSq = radiusPrecision * radiusPrecision;
            var magnetudeOnPrecisionDummy = 0.0f;
            var HeightTimePrecision = 0.0f;
            var stride = __instance.stride;
            for (var index2 = -1; index2 <= 3; ++index2)
            for (var index3 = -1; index3 <= 3; ++index3) {
                var index4 = index1 + index2 + index3 * stride;
                if ((uint) index4 < __instance.dataLength) {
                    var sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;
                    if (sqrMagnitude <= (double) radiusPrecisionSq) {
                        var magnetudeOnPrecision =
                            (float) (1.0 - Mathf.Sqrt(sqrMagnitude) / (double) radiusPrecision);
                        magnetudeOnPrecision *= __instance.GetFactoredScale();
                        var modLevel = __instance.GetModLevel(index4);
                        float heightDataFinal = __instance.heightData[index4];
                        if (modLevel > 0) {
                            // try patching here
                            var modPlane = __instance.GetModPlaneInt(index4);
                            if (modLevel == 3) {
                                heightDataFinal = modPlane;
                            }
                            else {
                                var num7 = modLevel * 0.3333333f;
                                heightDataFinal =
                                    (float) (__instance.heightData[index4] * (1.0 - num7) +
                                             modPlane * (double) num7);
                            }
                        }
                        magnetudeOnPrecisionDummy += magnetudeOnPrecision;
                        HeightTimePrecision += heightDataFinal * magnetudeOnPrecision;
                    }
                }
            }

            if (magnetudeOnPrecisionDummy != 0.0) {
                __result = (float) (HeightTimePrecision / (double) magnetudeOnPrecisionDummy *
                                    0.00999999977648258);
                return false;
            }

            GS2.Error("Bad Query");
            __result = __instance.heightData[0] * 0.01f;

            return false;
        }
    }
}