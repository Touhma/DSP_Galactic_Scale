using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize
{

    public static class PatchOnPlanetFactory
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetFactory), "FlattenTerrain")]
        public static bool PatchFlattenTerrain(
        ref PlanetFactory __instance,
        ref int[] ___tmp_ids,
        ref Dictionary<int, int> ___tmp_levelChanges,
        ref int __result,
        Vector3 pos,
        Quaternion rot,
        Bounds bound,
        float fade0 = 6f,
        float fade1 = 1f,
        bool removeVein = false,
        bool lift = false)
        {
            if (___tmp_levelChanges == null)
                ___tmp_levelChanges = new Dictionary<int, int>();
            if (___tmp_ids == null)
                ___tmp_ids = new int[1024];
            ___tmp_levelChanges.Clear();
            Array.Clear((Array)___tmp_ids, 0, ___tmp_ids.Length);
            bound.extents = new Vector3(bound.extents.x, bound.extents.y + 0.5f, bound.extents.z);
            Quaternion quaternion1 = rot;
            quaternion1.w = -quaternion1.w;
            Quaternion quaternion2 = Maths.SphericalRotation(pos, 22.5f);
            float realRadius = __instance.planet.realRadius;
            float areaRadius = bound.extents.magnitude + fade0;
            float num1 = areaRadius * areaRadius;
            float num2 = (float)((double)realRadius * 3.14159274101257 / ((double)__instance.planet.precision * 2.0));
            int num3 = Mathf.CeilToInt((float)((double)areaRadius * 1.41400003433228 / (double)num2 + 0.5));
            Vector3[] vector3Array = new Vector3[9]
            {
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
            int dataLength = __instance.planet.data.dataLength;
            Vector3[] vertices = __instance.planet.data.vertices;
            ushort[] heightData = __instance.planet.data.heightData; 
            int num4 = (int)((double)__instance.planet.realRadius * 100.0 + 20.0);
            int num5 = 0;
            foreach (Vector3 vpos in vector3Array)
            {
                int num6 = __instance.planet.data.QueryIndex(vpos);
                for (int index1 = -num3; index1 <= num3; ++index1)
                {
                    int num7 = num6 + index1 * stride;
                    if (num7 >= 0 && num7 < dataLength)
                    {
                        for (int index2 = -num3; index2 <= num3; ++index2)
                        {
                            int index3 = num7 + index2;
                            if ((long)(uint)index3 < (long)dataLength && (lift || (int)heightData[index3] > (int)num4))
                            {
                                Vector3 vector3_1;
                                vector3_1.x = vertices[index3].x * realRadius;
                                vector3_1.y = vertices[index3].y * realRadius;
                                vector3_1.z = vertices[index3].z * realRadius;
                                Vector3 vector3_2;
                                vector3_2.x = vector3_1.x - pos.x;
                                vector3_2.y = vector3_1.y - pos.y;
                                vector3_2.z = vector3_1.z - pos.z;
                                if ((double)vector3_2.x * (double)vector3_2.x + (double)vector3_2.y * (double)vector3_2.y + (double)vector3_2.z * (double)vector3_2.z < (double)num1 && !___tmp_levelChanges.ContainsKey(index3))
                                {
                                    vector3_1 = quaternion1 * (vector3_1 - pos);
                                    if (bound.Contains(vector3_1))
                                    {
                                        ___tmp_levelChanges[index3] = 3;
                                    }
                                    else
                                    {
                                        float num8 = Vector3.Distance(bound.ClosestPoint(vector3_1), vector3_1);
                                        int num9 = (int)(((double)fade0 - (double)num8) / ((double)fade0 - (double)fade1) * 3.0 + 0.5);
                                        if (num9 < 0)
                                            num9 = 0;
                                        else if (num9 > 3)
                                            num9 = 3;
                                        int modLevel = __instance.planet.data.GetModLevel(index3);
                                        int num10 = num9 - modLevel;
                                        if (num9 >= modLevel && num10 != 0)
                                        {
                                            ___tmp_levelChanges[index3] = num9;
                                            float num11 = (float)heightData[index3] * 0.01f;
                                            float num12 = realRadius + 0.2f - num11;
                                            float f = (float)(100.0 * (double)num10 * (double)num12 * 0.333333313465118);
                                            num5 += -Mathf.FloorToInt(f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool levelized = __instance.planet.levelized;
            int plane = Mathf.RoundToInt((float)(((double)pos.magnitude - 0.200000002980232 - (double)__instance.planet.realRadius) / 1.33333325386047));
            int num13 = plane * 133 + (int)num4 - 60;
            foreach (KeyValuePair<int, int> tmpLevelChange in ___tmp_levelChanges)
            {
                if (tmpLevelChange.Value > 0)
                {
                    if (levelized)
                    {
                        if ((int)heightData[tmpLevelChange.Key] >= num13)
                        {
                            if (__instance.planet.data.GetModLevel(tmpLevelChange.Key) < 3)
                                __instance.planet.data.SetModPlane(tmpLevelChange.Key, plane);
                            __instance.planet.AddHeightMapModLevel(tmpLevelChange.Key, tmpLevelChange.Value);
                        }
                    }
                    else
                        __instance.planet.AddHeightMapModLevel(tmpLevelChange.Key, tmpLevelChange.Value);
                }
            }
            bool flag = __instance.planet.UpdateDirtyMeshes();
            if (GameMain.isRunning && flag)
                __instance.RenderLocalPlanetHeightmap();
            bound.extents += new Vector3(1.5f, 1.5f, 1.5f);
            NearColliderLogic nearColliderLogic = __instance.planet.physics.nearColliderLogic;
            int vegetablesInAreaNonAlloc = nearColliderLogic.GetVegetablesInAreaNonAlloc(pos, areaRadius, ___tmp_ids);
            for (int index = 0; index < vegetablesInAreaNonAlloc; ++index)
            {
                int tmpId = ___tmp_ids[index];
                Vector3 pos1 = __instance.vegePool[tmpId].pos;
                Vector3 point = quaternion1 * (pos1 - pos);
                if (bound.Contains(point))
                {
                    __instance.RemoveVegeWithComponents(tmpId);
                }
                else
                {
                    float num6 = __instance.planet.data.QueryModifiedHeight(__instance.vegePool[tmpId].pos) - 0.03f;
                    __instance.vegePool[tmpId].pos = __instance.vegePool[tmpId].pos.normalized * num6;
                    GameMain.gpuiManager.AlterModel((int)__instance.vegePool[tmpId].modelIndex, __instance.vegePool[tmpId].modelId, tmpId, __instance.vegePool[tmpId].pos, __instance.vegePool[tmpId].rot, false);
                }
            }
            int veinsInAreaNonAlloc = nearColliderLogic.GetVeinsInAreaNonAlloc(pos, areaRadius, ___tmp_ids);
            for (int index = 0; index < veinsInAreaNonAlloc; ++index)
            {
                int tmpId = ___tmp_ids[index];
                Vector3 pos1 = __instance.veinPool[tmpId].pos;
                if (removeVein && bound.Contains(pos1))
                    __instance.RemoveVeinWithComponents(tmpId);
                else if ((double)pos1.magnitude > (double)__instance.planet.realRadius)
                {
                    float num6 = __instance.planet.data.QueryModifiedHeight(pos1) - 0.13f;
                    __instance.veinPool[tmpId].pos = pos1.normalized * num6;
                    GameMain.gpuiManager.AlterModel((int)__instance.veinPool[tmpId].modelIndex, __instance.veinPool[tmpId].modelId, tmpId, __instance.veinPool[tmpId].pos, false);
                }
            }
            ___tmp_levelChanges.Clear();
            Array.Clear((Array)___tmp_ids, 0, ___tmp_ids.Length);
            GameMain.gpuiManager.SyncAllGPUBuffer();
            __result = num5;
            return false;
        }
    }
}