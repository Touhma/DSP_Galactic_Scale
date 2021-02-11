using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
namespace GalacticScale.Scripts.PatchPlanetSize  {
    [HarmonyPatch(typeof(PlanetRawData))]
    public class PatchOnPlanetRawData {
        [HarmonyPrefix]
        [HarmonyPatch("GetModPlane")]
        public static bool GetModPlane(int index, ref PlanetRawData __instance, ref short __result) {
            Patch.Debug("scaleFactor " + Patch.scaleFactor, LogLevel.Debug,
                Patch.DebugPlanetRawData);

            Patch.Debug("index " + index, LogLevel.Debug,
                Patch.DebugPlanetRawData);

            Patch.Debug("__instance.modData.Length " + __instance.modData.Length, LogLevel.Debug,
                Patch.DebugPlanetRawData);

            Patch.Debug(
                "test " + (((int) __instance.modData[index >> 1] >> ((index & 1) << 2) + 2 & 3) * 133), LogLevel.Debug,
                Patch.DebugPlanetRawData);

            if (Patch.scaleFactor > 1.0f) {
                float baseHeight = 20020;

                Patch.Debug("baseHeight " + baseHeight, LogLevel.Debug,
                    Patch.DebugPlanetRawData);
                __result = (short) (((int) __instance.modData[index >> 1] >> ((index & 1) << 2) + 2 & 3) * 133 +
                                    baseHeight);

                Patch.Debug("GetModPlane __result " + __result, LogLevel.Debug,
                    Patch.DebugPlanetRawData);
                return false;
            }
            else {
                return true;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch("QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance,
            ref float __result, Vector3 vpos) {
            Patch.Debug("QueryModifiedHeight ", LogLevel.Debug,
                Patch.DebugPlanetRawData);
            vpos.Normalize();
            int index1 = __instance.indexMap[__instance.PositionHash(vpos)];
            float radiusPrecision =
                (float) (3.14159274101257 / (double) (__instance.precision * 2) * 1.20000004768372);

            Patch.Debug("radiusPrecision " + radiusPrecision, LogLevel.Debug,
                Patch.DebugPlanetRawData);
            float radiusPrecisionSq = radiusPrecision * radiusPrecision;

            Patch.Debug("radiusPrecisionSq " + radiusPrecisionSq, LogLevel.Debug,
                Patch.DebugPlanetRawData);
            float magnetudeOnPrecisionDummy = 0.0f;
            float HeightTimePrecision = 0.0f;
            int stride = __instance.stride;
            for (int index2 = -1; index2 <= 3; ++index2) {
                for (int index3 = -1; index3 <= 3; ++index3) {
                    int index4 = index1 + index2 + index3 * stride;
                    if ((long) (uint) index4 < (long) __instance.dataLength) {
                        float sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;

                        Patch.Debug("sqrMagnitude " + sqrMagnitude, LogLevel.Debug,
                            Patch.DebugPlanetRawData);
                        if ((double) sqrMagnitude <= (double) radiusPrecisionSq) {
                            float magnetudeOnPrecision =
                                (float) (1.0 - (double) Mathf.Sqrt(sqrMagnitude) / (double) radiusPrecision);

                            Patch.Debug("MagnetudeOnPrecision " + magnetudeOnPrecision, LogLevel.Debug,
                                Patch.DebugPlanetRawData);
                            magnetudeOnPrecision *= Patch.scaleFactor;

                            Patch.Debug("MagnetudeOnPrecision Patched " + magnetudeOnPrecision,
                                LogLevel.Debug,
                                Patch.DebugPlanetRawData);
                            int modLevel = __instance.GetModLevel(index4);
                            float heightDataFinal = (float) __instance.heightData[index4];

                            Patch.Debug("heightDataFinal First " + heightDataFinal, LogLevel.Debug,
                                Patch.DebugPlanetRawData);
                            if (modLevel > 0) {
                                // try patching here
                                float modPlane = (float) __instance.GetModPlane(index4) * Patch.scaleFactor;

                                Patch.Debug("modPlane " + modPlane, LogLevel.Debug,
                                    Patch.DebugPlanetRawData);
                                if (modLevel == 3) {
                                    heightDataFinal = modPlane;

                                    Patch.Debug("heightDataFinal Second " + heightDataFinal, LogLevel.Debug,
                                        Patch.DebugPlanetRawData);
                                }
                                else {
                                    float num7 = (float) modLevel * 0.3333333f;
                                    heightDataFinal =
                                        (float) ((double) __instance.heightData[index4] * (1.0 - (double) num7) +
                                                 (double) modPlane * (double) num7);

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
            }

            if ((double) magnetudeOnPrecisionDummy != 0.0) {
                __result = (float) ((double) HeightTimePrecision / (double) magnetudeOnPrecisionDummy *
                                    0.00999999977648258);
                
                Patch.Debug("__result magnetudeOnPrecisionDummy" + __result, LogLevel.Debug,
                    Patch.DebugPlanetRawData);
                return false;
            }

            Debug.LogWarning((object) "bad query");
            __result = (float) __instance.heightData[0] * 0.01f;
            
            Patch.Debug("__result bad query" + __result, LogLevel.Debug,
                Patch.DebugPlanetRawData);

            return false;
        }
    }
}