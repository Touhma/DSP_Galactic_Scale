using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;
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
                var precisionOnSegment = __instance.precision / __instance.segment;

                Patch.Debug("precisionOnSegment :" + precisionOnSegment, LogLevel.Debug,
                    Patch.DebugPlanetData);
                var segmentSquared = __instance.segment * __instance.segment;

                Patch.Debug("segmentSquared :" + segmentSquared, LogLevel.Debug,
                    Patch.DebugPlanetData);
                var dirtyIOnSSqu = dirtyIdx / segmentSquared;
                var dirtyIOnSSquModulo = dirtyIOnSSqu % 2;
                var dirtyIOnSSquByTwo = dirtyIOnSSqu / 2;
                var dirtyIModuloSegmentSquared = dirtyIdx % segmentSquared;
                var DIMSQMod = dirtyIModuloSegmentSquared % __instance.segment * precisionOnSegment +
                               dirtyIOnSSquModulo * __instance.data.substride;

                Patch.Debug("DIMSQMod :" + DIMSQMod, LogLevel.Debug,
                    Patch.DebugPlanetData);
                var DIMSQBy = dirtyIModuloSegmentSquared / __instance.segment * precisionOnSegment +
                              dirtyIOnSSquByTwo * __instance.data.substride;

                Patch.Debug("DIMSQBy :" + DIMSQBy, LogLevel.Debug,
                    Patch.DebugPlanetData);
                var stride = __instance.data.stride;

                var radiusOffset = __instance.radius;

                Patch.Debug("radiusOffset :" + radiusOffset, LogLevel.Debug,
                    Patch.DebugPlanetData);
                var mesh = __instance.meshes[dirtyIdx];
                var vertices = mesh.vertices;
                var normals = mesh.normals;
                var IndexGeo = 0;
                for (var i = DIMSQBy; i <= DIMSQBy + precisionOnSegment; i++)
                for (var j = DIMSQMod; j <= DIMSQMod + precisionOnSegment; j++) {
                    var strideIndexLoop = j + i * stride;
                    var heightDataNormalized =
                        __instance.data.heightData[strideIndexLoop] * 0.01f;

                    Patch.Debug("heightDataNormalized :" + heightDataNormalized, LogLevel.Debug,
                        Patch.DebugPlanetDataDeep);
                    var thirdOdModLevel =
                        __instance.data.GetModLevel(strideIndexLoop) * 0.3333333f;

                    Patch.Debug("thirdOdModLevel :" + thirdOdModLevel, LogLevel.Debug,
                        Patch.DebugPlanetDataDeep);

                    var copyOfRadiusOffset = radiusOffset;
                    if (thirdOdModLevel > 0f)
                        copyOfRadiusOffset = __instance.data.GetModPlane(strideIndexLoop) *
                                             __instance.GetScaleFactored() * 0.01f;

                    //patched copyOfRadiusOffset
                    // copyOfRadiusOffset *= scaleFactor;

                    Patch.Debug("copyOfRadiusOffset :" + copyOfRadiusOffset, LogLevel.Debug,
                        Patch.DebugPlanetDataDeep);
                    var ploup = heightDataNormalized * (1f - thirdOdModLevel) +
                                copyOfRadiusOffset * thirdOdModLevel;


                    Patch.Debug("ploup :" + ploup, LogLevel.Debug,
                        Patch.DebugPlanetDataDeep);
                    if (ploup > __instance.radius) {
                        ploup -= (float) 0.2 * __instance.GetScaleFactored();
                        if (ploup < __instance.radius) ploup = __instance.radius + 0.2f;
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