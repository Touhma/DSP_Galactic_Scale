using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static class PatchOnPlanetFactory
    {
        // [HarmonyTranspiler]
        // [HarmonyPatch(typeof(PlanetFactory), "FlattenTerrain")]
        // public static IEnumerable<CodeInstruction> FlattenTerrainTranspiler(IEnumerable<CodeInstruction> instructions,
        //     ILGenerator generator)
        // {
        //     var instructionList = new List<CodeInstruction>(instructions);
        //
        //     var intNum4 = generator.DeclareLocal(typeof(int));
        //     intNum4.SetLocalSymInfo("intNum4");
        //
        //     for (var instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
        //         if (instructionList[instructionCounter].opcode == OpCodes.Conv_I2 &&
        //             instructionCounter + 1 < instructionList.Count &&
        //             instructionList[instructionCounter + 1].opcode == OpCodes.Stloc_S &&
        //             instructionList[instructionCounter + 1].operand is LocalBuilder lb_stloc &&
        //             lb_stloc.LocalIndex == 18)
        //         {
        //             instructionList[instructionCounter] = new CodeInstruction(OpCodes.Conv_I4);
        //             instructionList[instructionCounter + 1] = new CodeInstruction(OpCodes.Stloc_S, intNum4);
        //
        //             instructionCounter++;
        //         }
        //         else if (instructionList[instructionCounter].opcode == OpCodes.Ldloc_S &&
        //                  instructionList[instructionCounter].operand is LocalBuilder lb_ldloc &&
        //                  lb_ldloc.LocalIndex == 18)
        //         {
        //             instructionList[instructionCounter] = new CodeInstruction(OpCodes.Ldloc_S, intNum4);
        //         }
        //
        //     return instructionList.AsEnumerable();
        // }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "FlattenTerrain")]
        public static bool FlattenTerrain(ref int __result, ref PlanetFactory __instance, Vector3 pos, Quaternion rot, Bounds bound, float fade0 = 6f, float fade1 = 1f, bool removeVein = false, bool lift = false)
        {
            if (__instance.tmp_levelChanges == null) __instance.tmp_levelChanges = new Dictionary<int, int>();
            if (__instance.tmp_ids == null) __instance.tmp_ids = new int[4096];
            __instance.tmp_levelChanges.Clear();
            Array.Clear(__instance.tmp_ids, 0, __instance.tmp_ids.Length);
            bound.extents = new Vector3(bound.extents.x, bound.extents.y + 0.5f, bound.extents.z);
            var quaternion = rot;
            quaternion.w = -quaternion.w;
            var rotation = Maths.SphericalRotation(pos, 22.5f);
            var realRadius = __instance.planet.realRadius;
            var num = bound.extents.magnitude + fade0;
            var num2 = num * num;
            var num3 = realRadius * 3.1415927f / (__instance.planet.precision * 2f);
            var num4 = Mathf.CeilToInt(num * 1.414f / num3 * 1.5f + 0.5f);
            Vector3[] array =
            {
                pos,
                pos + rotation * (new Vector3(0f, 0f, 1.414f) * num),
                pos + rotation * (new Vector3(0f, 0f, -1.414f) * num),
                pos + rotation * (new Vector3(1.414f, 0f, 0f) * num),
                pos + rotation * (new Vector3(-1.414f, 0f, 0f) * num),
                pos + rotation * (new Vector3(1f, 0f, 1f) * num),
                pos + rotation * (new Vector3(-1f, 0f, -1f) * num),
                pos + rotation * (new Vector3(1f, 0f, -1f) * num),
                pos + rotation * (new Vector3(-1f, 0f, 1f) * num)
            };
            var stride = __instance.planet.data.stride;
            var dataLength = __instance.planet.data.dataLength;
            var vertices = __instance.planet.data.vertices;
            var heightData = __instance.planet.data.heightData;
            var num5 = (int)(__instance.planet.realRadius * 100f + 20f); //changed short to int in both declaration and cast
            var num6 = 0;
            foreach (var vpos in array)
            {
                var num7 = __instance.planet.data.QueryIndex(vpos);
                for (var j = -num4; j <= num4; j++)
                {
                    var num8 = num7 + j * stride;
                    if (num8 >= 0 && num8 < dataLength)
                        for (var k = -num4; k <= num4; k++)
                        {
                            var num9 = num8 + k;
                            if ((ulong)num9 < (ulong)dataLength && (lift || heightData[num9] > num5)) //removed (short) cast from num5
                            {
                                Vector3 vector;
                                vector.x = vertices[num9].x * realRadius;
                                vector.y = vertices[num9].y * realRadius;
                                vector.z = vertices[num9].z * realRadius;
                                Vector3 vector2;
                                vector2.x = vector.x - pos.x;
                                vector2.y = vector.y - pos.y;
                                vector2.z = vector.z - pos.z;
                                if (vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z < num2 && !__instance.tmp_levelChanges.ContainsKey(num9))
                                {
                                    vector = quaternion * (vector - pos);
                                    if (bound.Contains(vector))
                                    {
                                        __instance.tmp_levelChanges[num9] = 3;
                                    }
                                    else
                                    {
                                        var num10 = Vector3.Distance(bound.ClosestPoint(vector), vector);
                                        var num11 = (int)((fade0 - num10) / (fade0 - fade1) * 3f + 0.5f);
                                        if (num11 < 0)
                                            num11 = 0;
                                        else if (num11 > 3) num11 = 3;
                                        var modLevel = __instance.planet.data.GetModLevel(num9);
                                        var num12 = num11 - modLevel;
                                        if (num11 >= modLevel && num12 != 0)
                                        {
                                            __instance.tmp_levelChanges[num9] = num11;
                                            var num13 = heightData[num9] * 0.01f;
                                            var num14 = realRadius + 0.2f - num13;
                                            var f = 100f * num12 * num14 * 0.3333333f;
                                            num6 += -Mathf.FloorToInt(f);
                                        }
                                    }
                                }
                            }
                        }
                }
            }

            var levelized = __instance.planet.levelized;
            var num15 = Mathf.RoundToInt((pos.magnitude - 0.2f - __instance.planet.realRadius) / 1.3333333f);
            var num16 = num15 * 133 + num5 - 60;
            foreach (var keyValuePair in __instance.tmp_levelChanges)
                if (keyValuePair.Value > 0)
                {
                    if (levelized)
                    {
                        if (heightData[keyValuePair.Key] >= num16)
                        {
                            if (__instance.planet.data.GetModLevel(keyValuePair.Key) < 3) __instance.planet.data.SetModPlane(keyValuePair.Key, num15);
                            __instance.planet.AddHeightMapModLevel(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    else
                    {
                        __instance.planet.AddHeightMapModLevel(keyValuePair.Key, keyValuePair.Value);
                    }
                }

            var flag = __instance.planet.UpdateDirtyMeshes();
            if (GameMain.isRunning && flag) __instance.RenderLocalPlanetHeightmap();
            var physics = __instance.planet.physics;
            bound.extents += new Vector3(1.5f, 1.5f, 1.5f);
            var nearColliderLogic = physics.nearColliderLogic;
            var num17 = nearColliderLogic.GetVegetablesInAreaNonAlloc(pos, num, __instance.tmp_ids);
            for (var l = 0; l < num17; l++)
            {
                var num18 = __instance.tmp_ids[l];
                var vector = __instance.vegePool[num18].pos;
                vector = quaternion * (vector - pos);
                if (bound.Contains(vector))
                {
                    __instance.RemoveVegeWithComponents(num18);
                }
                else
                {
                    var d = __instance.planet.data.QueryModifiedHeight(__instance.vegePool[num18].pos) - 0.03f;
                    __instance.vegePool[num18].pos = __instance.vegePool[num18].pos.normalized * d;
                    GameMain.gpuiManager.AlterModel(__instance.vegePool[num18].modelIndex, __instance.vegePool[num18].modelId, num18, __instance.vegePool[num18].pos, __instance.vegePool[num18].rot, false);
                }
            }

            num17 = nearColliderLogic.GetVeinsInAreaNonAlloc(pos, num, __instance.tmp_ids);
            for (var m = 0; m < num17; m++)
            {
                var num19 = __instance.tmp_ids[m];
                var vector = __instance.veinPool[num19].pos;
                if (removeVein && bound.Contains(vector))
                {
                    __instance.RemoveVeinWithComponents(num19);
                }
                else if (vector.magnitude > __instance.planet.realRadius)
                {
                    var d2 = __instance.planet.data.QueryModifiedHeight(vector) - 0.13f;
                    __instance.veinPool[num19].pos = vector.normalized * d2;
                    GameMain.gpuiManager.AlterModel(__instance.veinPool[num19].modelIndex, __instance.veinPool[num19].modelId, num19, __instance.veinPool[num19].pos, false);
                    var num20 = __instance.veinPool[num19].colliderId;
                    var num21 = num20 >> 20;
                    num20 &= 1048575;
                    physics.colChunks[num21].colliderPool[num20].pos = __instance.veinPool[num19].pos + __instance.veinPool[num19].pos.normalized * 0.4f;
                    physics.SetPlanetPhysicsColliderDirty();
                }
            }

            __instance.tmp_levelChanges.Clear();
            Array.Clear(__instance.tmp_ids, 0, __instance.tmp_ids.Length);
            GameMain.gpuiManager.SyncAllGPUBuffer();
            __result = num6;
            return false;
        }
    }
}