using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize  {
    [HarmonyPatch(typeof(PlanetFactory))]
    public class PatchOnPlanetFactory {
        [HarmonyPrefix]
        [HarmonyPatch("ComputeFlattenTerrainReform")]
        public static bool ComputeFlattenTerrainReform(
            Vector3[] points,
            Vector3 center,
            float radius,
            int pointsCount,
            float fade0 = 3f,
            float fade1 = 1f) {
            Patch.Debug("ComputeFlattenTerrainReform", LogLevel.Debug,
                Patch.DebugPlanetFactory);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("FlattenTerrain")]
        public static bool FlattenTerrain(
            ref PlanetFactory __instance,
            ref int __result,
            ref Dictionary<int, int> ___tmp_levelChanges,
            ref int[] ___tmp_ids,
            Vector3 pos,
            Quaternion rot,
            Bounds bound,
            float fade0 = 6f,
            float fade1 = 1f,
            bool removeVein = false,
            bool lift = false) {
            if (__instance.planet.name == "Luna") {
                Patch.Debug("FlattenTerrain -- Begining ", LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                if (___tmp_levelChanges == null) {
                    ___tmp_levelChanges = new Dictionary<int, int>();
                }

                if (___tmp_ids == null) {
                    ___tmp_ids = new int[1024];
                }

                ___tmp_levelChanges.Clear();
                Array.Clear((Array) ___tmp_ids, 0, ___tmp_ids.Length);
                bound.extents = new Vector3(bound.extents.x, bound.extents.y + 0.5f, bound.extents.z);
                Quaternion quaternion1 = rot;
                quaternion1.w = -quaternion1.w;
                Quaternion quaternion2 = Maths.SphericalRotation(pos, 22.5f);
                float realRadius = __instance.planet.realRadius;

                Patch.Debug("realRadius --  " + realRadius, LogLevel.Debug,
                    Patch.DebugPlanetFactory);

                float areaRadius = bound.extents.magnitude + fade0;

                Patch.Debug("areaRadius --  " + areaRadius, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                float areaRadiusSq = areaRadius * areaRadius;

                Patch.Debug("areaRadiusSq --  " + areaRadiusSq, LogLevel.Debug,
                    Patch.DebugPlanetFactory);

                // Def something related to that that mess up everything
                //areaRadiusSq *= scaleFactor;

                Patch.Debug("Patching areaRadiusSq --  " + areaRadiusSq, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                float circlePeriphery = (float) ((double) realRadius * 3.14159274101257 /
                                                 ((double) __instance.planet.precision * 2.0));

                Patch.Debug("circlePeriphery --  " + circlePeriphery, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                int levelAffected =
                    Mathf.CeilToInt(
                        (float) ((double) areaRadius * 1.41400003433228 / (double) circlePeriphery + 0.5));

                Patch.Debug("levelAffected --  " + levelAffected, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                Vector3[] vector3Array = new Vector3[9] {
                    pos,
                    pos + quaternion2 * (new Vector3(0.0f, 0.0f, 1.414f) * areaRadius),
                    pos + quaternion2 * (new Vector3(0.0f, 0.0f, -1.414f) * areaRadius),
                    pos + quaternion2 * (new Vector3(1.414f, 0.0f, 0.0f) * areaRadius),
                    pos + quaternion2 * (new Vector3(-1.414f, 0.0f, 0.0f) * areaRadius),
                    pos + quaternion2 * (new Vector3(1f, 0.0f, 1f) * areaRadius),
                    pos + quaternion2 * (new Vector3(-1f, 0.0f, -1f) * areaRadius),
                    pos + quaternion2 * (new Vector3(1f, 0.0f, -1f) * areaRadius),
                    pos + quaternion2 * (new Vector3(-1f, 0.0f, 1f) * areaRadius)
                };


                int stride = __instance.planet.data.stride;

                Patch.Debug("stride --  " + stride, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                int dataLength = __instance.planet.data.dataLength;

                Patch.Debug("dataLength --  " + dataLength, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                Vector3[] vertices = __instance.planet.data.vertices;

                Patch.Debug("vertices --  " + vertices, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                ushort[] heightData = __instance.planet.data.heightData;

                Patch.Debug("heightData --  " + heightData, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                short radiusHeight100 = (short) ((double) __instance.planet.realRadius * 100.0 + 20.0);

                Patch.Debug("radiusHeight100 --  " + radiusHeight100, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                int fRoundToInt = 0;
                foreach (Vector3 vpos in vector3Array) {
                    int posIndex = __instance.planet.data.QueryIndex(vpos);
                    for (int index1 = -levelAffected; index1 <= levelAffected; ++index1) {
                        int strideIndex = posIndex + index1 * stride;
                        if (strideIndex >= 0 && strideIndex < dataLength) {
                            for (int index2 = -levelAffected; index2 <= levelAffected; ++index2) {
                                int index3 = strideIndex + index2;
                                if ((long) (uint) index3 < (long) dataLength &&
                                    (lift || (int) heightData[index3] > (int) radiusHeight100)) {
                                    Vector3 vector3_1;
                                    vector3_1.x = vertices[index3].x * realRadius;
                                    vector3_1.y = vertices[index3].y * realRadius;
                                    vector3_1.z = vertices[index3].z * realRadius;
                                    Vector3 vector3_2;
                                    vector3_2.x = vector3_1.x - pos.x;
                                    vector3_2.y = vector3_1.y - pos.y;
                                    vector3_2.z = vector3_1.z - pos.z;
                                    if ((double) vector3_2.x * (double) vector3_2.x +
                                        (double) vector3_2.y * (double) vector3_2.y +
                                        (double) vector3_2.z * (double) vector3_2.z < (double) areaRadiusSq &&
                                        !___tmp_levelChanges.ContainsKey(index3)) {
                                        Patch.Debug("vector3_2 less than areaRadiusSq --  " + vector3_2,
                                            LogLevel.Debug,
                                            Patch.DebugPlanetFactoryDeep);
                                        vector3_1 = quaternion1 * (vector3_1 - pos);

                                        Patch.Debug("vector3_1 less than areaRadiusSq --  " + vector3_1,
                                            LogLevel.Debug,
                                            Patch.DebugPlanetFactoryDeep);

                                        if (bound.Contains(vector3_1)) {
                                            ___tmp_levelChanges[index3] = 3;
                                        }
                                        else {
                                            float boundDistance = Vector3.Distance(bound.ClosestPoint(vector3_1),
                                                vector3_1);
                                            int boundDistanceFade =
                                                (int) (((double) fade0 - (double) boundDistance) /
                                                    ((double) fade0 - (double) fade1) * 3.0 + 0.5);
                                            if (boundDistanceFade < 0) {
                                                boundDistanceFade = 0;
                                            }
                                            else if (boundDistanceFade > 3) {
                                                boundDistanceFade = 3;
                                            }

                                            int modLevel = __instance.planet.data.GetModLevel(index3);

                                            Patch.Debug("modLevel --  " + modLevel, LogLevel.Debug,
                                                Patch.DebugPlanetFactoryDeep);
                                            int boundDiff = boundDistanceFade - modLevel;

                                            Patch.Debug("boundDiff --  " + boundDiff, LogLevel.Debug,
                                                Patch.DebugPlanetFactoryDeep);
                                            if (boundDistanceFade >= modLevel && boundDiff != 0) {
                                                ___tmp_levelChanges[index3] = boundDistanceFade;
                                                float currentHeightData = (float) heightData[index3] * 0.01f;

                                                Patch.Debug("currentHeightData --  " + currentHeightData,
                                                    LogLevel.Debug,
                                                    Patch.DebugPlanetFactoryDeep);

                                                float currentHeightDataOnRadius =
                                                    realRadius + 0.2f - currentHeightData;

                                                Patch.Debug(
                                                    "currentHeightDataOnRadius --  " + currentHeightDataOnRadius,
                                                    LogLevel.Debug,
                                                    Patch.DebugPlanetFactoryDeep);
                                                //anomaly here
                                                //currentHeightDataOnRadius *= scaleFactor;

                                                Patch.Debug(
                                                    "Patching currentHeightDataOnRadius --  " +
                                                    currentHeightDataOnRadius, LogLevel.Debug,
                                                    Patch.DebugPlanetFactoryDeep);

                                                float f = (float) (100.0 * (double) boundDiff *
                                                                   (double) currentHeightDataOnRadius *
                                                                   0.333333313465118);

                                                Patch.Debug("f --  " + f, LogLevel.Debug,
                                                    Patch.DebugPlanetFactoryDeep);
                                                fRoundToInt += -Mathf.FloorToInt(f);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                bool levelized = __instance.planet.levelized;
                int plane = Mathf.RoundToInt(
                    (float) (((double) pos.magnitude - 0.200000002980232 - (double) __instance.planet.realRadius) /
                             1.33333325386047));


                Patch.Debug("plane --  " + plane, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                int num13 = plane * 133 + (int) radiusHeight100 - 60;

                Patch.Debug("num13 --  " + num13, LogLevel.Debug,
                    Patch.DebugPlanetFactory);
                foreach (KeyValuePair<int, int> tmpLevelChange in ___tmp_levelChanges) {
                    if (tmpLevelChange.Value > 0) {
                        if (levelized) {
                            if ((int) heightData[tmpLevelChange.Key] >= num13) {
                                if (__instance.planet.data.GetModLevel(tmpLevelChange.Key) < 3) {
                                    Patch.Debug("SetModPlane --  plane" + plane, LogLevel.Debug,
                                        Patch.DebugPlanetFactoryDeep);

                                    Patch.Debug(
                                        "SetModPlane --  heightData[tmpLevelChange.Key]" +
                                        heightData[tmpLevelChange.Key], LogLevel.Debug,
                                        Patch.DebugPlanetFactoryDeep);
                                    __instance.planet.data.SetModPlane(tmpLevelChange.Key, plane);
                                }


                                Patch.Debug(
                                    "If AddHeightMapModLevel --  tmpLevelChange.Value" + tmpLevelChange.Value,
                                    LogLevel.Debug,
                                    Patch.DebugPlanetFactoryDeep);

                                Patch.Debug(
                                    "If AddHeightMapModLevel --  heightData[tmpLevelChange.Key]" +
                                    heightData[tmpLevelChange.Key], LogLevel.Debug,
                                    Patch.DebugPlanetFactoryDeep);
                                __instance.planet.AddHeightMapModLevel(tmpLevelChange.Key, tmpLevelChange.Value);
                            }
                        }
                        else {
                            Patch.Debug(
                                "Else AddHeightMapModLevel --  tmpLevelChange.Value" + tmpLevelChange.Value,
                                LogLevel.Debug,
                                Patch.DebugPlanetFactoryDeep);

                            Patch.Debug(
                                "Else AddHeightMapModLevel --  heightData[tmpLevelChange.Key]" +
                                heightData[tmpLevelChange.Key], LogLevel.Debug,
                                Patch.DebugPlanetFactoryDeep);
                            __instance.planet.AddHeightMapModLevel(tmpLevelChange.Key, tmpLevelChange.Value);
                        }
                    }
                }

                bool flag = __instance.planet.UpdateDirtyMeshes();
                if (GameMain.isRunning && flag) {
                    __instance.RenderLocalPlanetHeightmap();
                }

                bound.extents += new Vector3(1.5f, 1.5f, 1.5f);
                NearColliderLogic nearColliderLogic = __instance.planet.physics.nearColliderLogic;
                int vegetablesInAreaNonAlloc =
                    nearColliderLogic.GetVegetablesInAreaNonAlloc(pos, areaRadius, ___tmp_ids);
                for (int index = 0; index < vegetablesInAreaNonAlloc; ++index) {
                    int tmpId = ___tmp_ids[index];
                    Vector3 pos1 = __instance.vegePool[tmpId].pos;
                    Vector3 point = quaternion1 * (pos1 - pos);
                    if (bound.Contains(point)) {
                        __instance.RemoveVegeWithComponents(tmpId);
                    }
                    else {
                        float num6 = __instance.planet.data.QueryModifiedHeight(__instance.vegePool[tmpId].pos) -
                                     0.03f;

                        Patch.Debug(
                            "num6 --  __instance.planet.data.QueryModifiedHeight(__instance.vegePool[tmpId].pos) - 0.03f + heightData[tmpLevelChange.Key]" +
                            num6, LogLevel.Debug,
                            Patch.DebugPlanetFactory);
                        __instance.vegePool[tmpId].pos = __instance.vegePool[tmpId].pos.normalized * num6;

                        Patch.Debug("__instance.vegePool[tmpId].pos --  _" + __instance.vegePool[tmpId].pos,
                            LogLevel.Debug,
                            Patch.DebugPlanetFactory);
                        GameMain.gpuiManager.AlterModel((int) __instance.vegePool[tmpId].modelIndex,
                            __instance.vegePool[tmpId].modelId, tmpId, __instance.vegePool[tmpId].pos,
                            __instance.vegePool[tmpId].rot,
                            false);
                    }
                }

                int veinsInAreaNonAlloc = nearColliderLogic.GetVeinsInAreaNonAlloc(pos, areaRadius, ___tmp_ids);
                for (int index = 0; index < veinsInAreaNonAlloc; ++index) {
                    int tmpId = ___tmp_ids[index];
                    Vector3 pos1 = __instance.veinPool[tmpId].pos;
                    if (removeVein && bound.Contains(pos1)) {
                        __instance.RemoveVeinWithComponents(tmpId);
                    }
                    else if ((double) pos1.magnitude > (double) __instance.planet.realRadius) {
                        float num6 = __instance.planet.data.QueryModifiedHeight(pos1) - 0.13f;
                        __instance.veinPool[tmpId].pos = pos1.normalized * num6;
                        GameMain.gpuiManager.AlterModel((int) __instance.veinPool[tmpId].modelIndex,
                            __instance.veinPool[tmpId].modelId, tmpId, __instance.veinPool[tmpId].pos, false);
                    }
                }

                ___tmp_levelChanges.Clear();
                Array.Clear((Array) ___tmp_ids, 0, ___tmp_ids.Length);
                GameMain.gpuiManager.SyncAllGPUBuffer();
                __result = fRoundToInt;


                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("FlattenTerrainReform")]
        public static bool FlattenTerrainReform(
            Vector3 center,
            float radius,
            int reformSize,
            bool veinBuried,
            float fade0 = 3f) {
            Patch.Debug("FlattenTerrainReform ", LogLevel.Debug,
                Patch.DebugPlanetFactoryDeep);
            return true;
        }
    }
}