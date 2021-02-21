using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetData))]
    public class PatchOnPlanetData {
        [HarmonyPrefix]
        [HarmonyPatch("realRadius", MethodType.Getter)]
        public static bool RealRadiusGetter(ref PlanetData __instance, ref float __result) {
            if (__instance.type != EPlanetType.Gas) {
                __result = __instance.radius;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GenBirthPoints")]
        public static bool GenBirthPoints(PlanetRawData rawData, int _birthSeed) {
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateDirtyMesh")]
        public static bool UpdateDirtyMesh(ref PlanetData __instance, ref bool __result, int dirtyIdx) {
            Patch.Debug("UpdateDirtyMesh", LogLevel.Debug,
                Patch.DebugGeneral);

            Patch.Debug("UpdateDirtyMesh Start :", LogLevel.Debug,
                Patch.DebugPlanetData);
            Patch.Debug("scaleFactor updateDirtyMesh :" + __instance.GetScaleFactored(), LogLevel.Debug,
                Patch.DebugPlanetData);

            if (__instance.dirtyFlags[dirtyIdx]) {
                __instance.dirtyFlags[dirtyIdx] = false;
                int precisionOnSegment = __instance.precision / __instance.segment;

                Patch.Debug("precisionOnSegment :" + precisionOnSegment, LogLevel.Debug,
                    Patch.DebugPlanetData);
                int segmentSquared = __instance.segment * __instance.segment;

                Patch.Debug("segmentSquared :" + segmentSquared, LogLevel.Debug,
                    Patch.DebugPlanetData);
                int dirtyIOnSSqu = dirtyIdx / segmentSquared;
                int dirtyIOnSSquModulo = dirtyIOnSSqu % 2;
                int dirtyIOnSSquByTwo = dirtyIOnSSqu / 2;
                int dirtyIModuloSegmentSquared = dirtyIdx % segmentSquared;
                int DIMSQMod = dirtyIModuloSegmentSquared % __instance.segment * precisionOnSegment +
                               dirtyIOnSSquModulo * __instance.data.substride;

                Patch.Debug("DIMSQMod :" + DIMSQMod, LogLevel.Debug,
                    Patch.DebugPlanetData);
                int DIMSQBy = dirtyIModuloSegmentSquared / __instance.segment * precisionOnSegment +
                              dirtyIOnSSquByTwo * __instance.data.substride;

                Patch.Debug("DIMSQBy :" + DIMSQBy, LogLevel.Debug,
                    Patch.DebugPlanetData);
                int stride = __instance.data.stride;

                float radiusOffset = __instance.radius;

                Patch.Debug("radiusOffset :" + radiusOffset, LogLevel.Debug,
                    Patch.DebugPlanetData);
                Mesh mesh = __instance.meshes[dirtyIdx];
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                int IndexGeo = 0;
                for (int i = DIMSQBy; i <= DIMSQBy + precisionOnSegment; i++) {
                    for (int j = DIMSQMod; j <= DIMSQMod + precisionOnSegment; j++) {
                        int strideIndexLoop = j + i * stride;
                        float heightDataNormalized =
                            (float) __instance.data.heightData[strideIndexLoop] * 0.01f;

                        Patch.Debug("heightDataNormalized :" + heightDataNormalized, LogLevel.Debug,
                            Patch.DebugPlanetDataDeep);
                        float thirdOdModLevel =
                            (float) __instance.data.GetModLevel(strideIndexLoop) * 0.3333333f;

                        Patch.Debug("thirdOdModLevel :" + thirdOdModLevel, LogLevel.Debug,
                            Patch.DebugPlanetDataDeep);

                        float copyOfRadiusOffset = radiusOffset;
                        if (thirdOdModLevel > 0f) {
                            copyOfRadiusOffset = (float) __instance.data.GetModPlane(strideIndexLoop) *
                                                 __instance.GetScaleFactored() * 0.01f;
                        }

                        //patched copyOfRadiusOffset
                        // copyOfRadiusOffset *= scaleFactor;

                        Patch.Debug("copyOfRadiusOffset :" + copyOfRadiusOffset, LogLevel.Debug,
                            Patch.DebugPlanetDataDeep);
                        float ploup = heightDataNormalized * (1f - thirdOdModLevel) +
                                      copyOfRadiusOffset * thirdOdModLevel;


                        Patch.Debug("ploup :" + ploup, LogLevel.Debug,
                            Patch.DebugPlanetDataDeep);
                        if (ploup > __instance.radius) {
                            ploup -= (float) 0.2 * __instance.GetScaleFactored();
                            if (ploup < __instance.radius) {
                                ploup =  __instance.radius + 0.2f;
                            }
                        }
                        
                        

                        vertices[IndexGeo].x = __instance.data.vertices[strideIndexLoop].x * ploup;
                        vertices[IndexGeo].y = __instance.data.vertices[strideIndexLoop].y * ploup;
                        vertices[IndexGeo].z = __instance.data.vertices[strideIndexLoop].z * ploup;

                        normals[IndexGeo].x =
                            __instance.data.normals[strideIndexLoop].x * (1f - thirdOdModLevel) +
                            __instance.data.vertices[strideIndexLoop].x * thirdOdModLevel;
                        normals[IndexGeo].y =
                            __instance.data.normals[strideIndexLoop].y * (1f - thirdOdModLevel) +
                            __instance.data.vertices[strideIndexLoop].y * thirdOdModLevel;
                        normals[IndexGeo].z =
                            __instance.data.normals[strideIndexLoop].z * (1f - thirdOdModLevel) +
                            __instance.data.vertices[strideIndexLoop].z * thirdOdModLevel;
                        normals[IndexGeo].Normalize();
                        IndexGeo++;
                    }
                }

                mesh.vertices = vertices;
                mesh.normals = normals;
                __instance.meshColliders[dirtyIdx].sharedMesh = null;
                __instance.meshColliders[dirtyIdx].sharedMesh = mesh;

                __result = true;
                return false;
            }

            __result = false;
            return false;
        }
    }
}