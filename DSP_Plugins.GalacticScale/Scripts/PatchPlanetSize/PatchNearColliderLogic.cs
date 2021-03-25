using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FullSerializer;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(NearColliderLogic))]
    static class PatchNearColliderLogic
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value)
        {
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }

        [HarmonyPrefix]
        [HarmonyPatch("RefreshCollidersOnArrayChange")]
        public static bool PatchRefreshCollidersOnArrayChange(NearColliderLogic __instance)
        {
            Patch.Log("RefreshCollidersOnArrayChange");
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Free")]
        public static bool PatchFree()
        {
            Patch.Log("Free");
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Init")]
        public static bool PatchInit(PlanetData _planet)
        {
            Patch.Log("Init");
            //this.planet = _planet;
            //this.colChunks = this.planet.physics.colChunks;
            //this.activeColHashes = new int[600];
            //this.activeColHashCount = 0;
            //this.colliderObjs = new Dictionary<int, ColliderObject>();
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetVeinsInAreaNonAlloc")]
        public static bool PatchGetVeinsInAreaNonAlloc(NearColliderLogic __instance, ref int __result, Vector3 center, float areaRadius, int[] veinIds, ref int ___activeColHashCount, ref int[] ___activeColHashes, ref ColliderContainer[] ___colChunks)
        {
            //return true;
            if (veinIds == null)
                return true;
            int num = 0;
            Array.Clear((Array)veinIds, 0, veinIds.Length);
            Vector3 normalized = center.normalized;
            Vector3 lhs = Vector3.Cross(normalized, Vector3.up).normalized;
            Vector3 doon = Vector3.Cross(normalized, Vector3.left).normalized;
            Vector3 vector3_1;
            //Patch.Log(center);
            if ((double)lhs.sqrMagnitude < 0.25)
            {
                
                lhs = Vector3.right;
                vector3_1 = Vector3.forward;
            }
            else
                vector3_1 = Vector3.Cross(lhs, normalized).normalized;
            lhs *= areaRadius + 3f;
            Vector3 vector3_2 = vector3_1 * (areaRadius + 3f);
            ___activeColHashCount = 0;
            Traverse markActivePos = Traverse.Create(__instance).Method("MarkActivePos", new[] { typeof(Vector3) });
            //Vector3 lower = center + doon;
            
            markActivePos.GetValue(center);
            
            
            markActivePos.GetValue(center + lhs);
            markActivePos.GetValue(center - lhs);
            markActivePos.GetValue(center + vector3_2);
            markActivePos.GetValue(center - vector3_2);
            markActivePos.GetValue(center + lhs + vector3_2);
            markActivePos.GetValue(center - lhs + vector3_2);
            markActivePos.GetValue(center + lhs - vector3_2);
            markActivePos.GetValue(center - lhs - vector3_2);
            //markActivePos.GetValue(lower);
            //markActivePos.GetValue(lower + lhs);
            //markActivePos.GetValue(lower - lhs);
            //markActivePos.GetValue(lower + vector3_2);
            //markActivePos.GetValue(lower - vector3_2);
            //markActivePos.GetValue(lower + lhs + vector3_2);
            //markActivePos.GetValue(lower - lhs + vector3_2);
            //markActivePos.GetValue(lower + lhs - vector3_2);
            //markActivePos.GetValue(lower - lhs - vector3_2);
            //lower = Vector3.MoveTowards(center, Vector3.zero - center, 2);
            //markActivePos.GetValue(lower);
            //markActivePos.GetValue(lower + lhs);
            //markActivePos.GetValue(lower - lhs);
            //markActivePos.GetValue(lower + vector3_2);
            //markActivePos.GetValue(lower - vector3_2);
            //markActivePos.GetValue(lower + lhs + vector3_2);
            //markActivePos.GetValue(lower - lhs + vector3_2);
            //markActivePos.GetValue(lower + lhs - vector3_2);
            //markActivePos.GetValue(lower - lhs - vector3_2);
            Vector3[] positions = {
                center,
                center + lhs,
                center - lhs,
                center + vector3_2,
                center - vector3_2,
                center + lhs + vector3_2,
                center - lhs + vector3_2,
                center + lhs - vector3_2,
                center - lhs - vector3_2,
            };
            ___activeColHashCount = 0;
            //Patch.Log("****Start Foreach Loop");
            foreach (Vector3 pos in positions)
            {
                int n = PlanetPhysics.HashPhysBlock(pos);
                //Patch.Debug("n = " + n + " = PlanetPhysics.HashPhysBlock(" + pos + "); ___activeColHashCount = " + ___activeColHashCount, BepInEx.Logging.LogLevel.Message, true);
                if (n == -1)
                    break;
                for (int index = 0; index < ___activeColHashCount; ++index)
                {
                    if (___activeColHashes[index] == n)
                        break;
                }
                ___activeColHashes[___activeColHashCount++] = n;
            }

            //Patch.Log("****Left Foreach Loop");

            if (___activeColHashCount > 0)
            {
                for (int index1 = 0; index1 < ___activeColHashCount; ++index1)
                {
                    int activeColHash = ___activeColHashes[index1];
                    ColliderData[] colliderPool = ___colChunks[activeColHash].colliderPool;
                    Patch.Log("activeColHash = " + activeColHash + " colliderPool Length = " + colliderPool.Length + " ___colChunks length = " + ___colChunks.Length);
                    Patch.Log(Serialize(typeof(ColliderData[]), colliderPool));
                    for (int index2 = 1; index2 < ___colChunks[activeColHash].cursor; ++index2)
                    {
                        if (colliderPool[index2].idType != 0 && colliderPool[index2].usage != EColliderUsage.Build && colliderPool[index2].objType == EObjectType.Vein && (double)(colliderPool[index2].pos - center).sqrMagnitude <= (double)areaRadius * (double)areaRadius + (double)colliderPool[index2].ext.sqrMagnitude)
                        {
                            bool flag = false;
                            for (int index3 = 0; index3 < num; ++index3)
                            {
                                if (veinIds[index3] == colliderPool[index2].objId)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                                veinIds[num++] = colliderPool[index2].objId;
                        }
                    }
                }
            }
            Patch.Log("We found " + num + " veins");
            __result = num;
            return false;
        }
    }
}