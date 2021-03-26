using HarmonyLib;
using System;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(NearColliderLogic))]
    static class PatchNearColliderLogic
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetVeinsInAreaNonAlloc")]
        public static bool PatchGetVeinsInAreaNonAlloc(ref int __result, Vector3 center, float areaRadius, int[] veinIds, ref int ___activeColHashCount, ref int[] ___activeColHashes, ref ColliderContainer[] ___colChunks)
        {
            if (veinIds == null) return true;
            int num = 0;
            Array.Clear((Array)veinIds, 0, veinIds.Length);
            Vector3 normalized = center.normalized;
            Vector3 lhs = Vector3.Cross(normalized, Vector3.up).normalized;
            Vector3 vector3_1;
            Patch.Log(lhs.sqrMagnitude);
            if ((double)lhs.sqrMagnitude < 0.25)
            {
                lhs = Vector3.right;
                vector3_1 = Vector3.forward;
            }
            else
                vector3_1 = Vector3.Cross(lhs, normalized).normalized;
            lhs *= areaRadius - 5f; // Original had areaRadius + 3f;
            Vector3 vector3_2 = vector3_1 * (areaRadius - 5f); // Original had areaRadius + 3f;
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
            foreach (Vector3 pos in positions) //Grab the hash of the position, and add it to the array ___activeColHashes. activeColHashCount increases each time...but is always 9...
            {
                int n = PlanetPhysics.HashPhysBlock(pos);
                if (n == -1)
                    break;
                for (int index = 0; index < ___activeColHashCount; ++index)
                {
                    if (___activeColHashes[index] == n)
                        break;
                }
                ___activeColHashes[___activeColHashCount++] = n;
            }
            if (___activeColHashCount > 0) //If we have some hashes
            {
                for (int index1 = 0; index1 < ___activeColHashCount; ++index1) 
                {
                    int activeColHash = ___activeColHashes[index1]; //grab it's value (445 etc)
                    ColliderData[] colliderPool = ___colChunks[activeColHash].colliderPool; //and Grab the Collider Pool from the collider container ___colChunks[that hash]
                    for (int index2 = 1; index2 < ___colChunks[activeColHash].cursor; ++index2) //For values between 1 and some value called cursor from that collider container, which seems to be between 1 and 10...
                    {
                        if (colliderPool[index2].idType != 0 && //check array items 1-10 from the colliderPool and see if their idtype isnt 0
                            colliderPool[index2].usage != EColliderUsage.Build &&  //that they are not a building
                            colliderPool[index2].objType == EObjectType.Vein &&  //that their objType is a vein
                            (double)(colliderPool[index2].pos - center).sqrMagnitude <= (double)areaRadius * (double)areaRadius + (double)colliderPool[index2].ext.sqrMagnitude) //and their center is within the area we are scanning
                        {
                            bool flag = false; //if so, dont set the break flag
                            for (int index3 = 0; index3 < num; ++index3) //hang on a second, these are while loops :S
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
            __result = num;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetVegetablesInAreaNonAlloc")]
        public static bool GetVegetablesInAreaNonAlloc(ref int __result, Vector3 center, float areaRadius, int[] vegeIds, ref int ___activeColHashCount, ref int[] ___activeColHashes, ref ColliderContainer[] ___colChunks)
        {
            if (vegeIds == null)
                return true;
            int num = 0;
            Array.Clear((Array)vegeIds, 0, vegeIds.Length);
            Vector3 normalized = center.normalized;
            Vector3 lhs = Vector3.Cross(normalized, Vector3.up).normalized;
            Vector3 vector3_1;
            if ((double)lhs.sqrMagnitude < 0.25)
            {
                lhs = Vector3.right;
                vector3_1 = Vector3.forward;
            }
            else
                vector3_1 = Vector3.Cross(lhs, normalized).normalized;
            lhs *= areaRadius - 5f; // Original had areaRadius + 3f; This doesnt fix the foundation popthrough bug
            Vector3 vector3_2 = vector3_1 * (areaRadius - 5f); // Original had areaRadius + 3f; Neither does this.
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
            foreach (Vector3 pos in positions) 
            {
                int n = PlanetPhysics.HashPhysBlock(pos);
                if (n == -1)
                    break;
                for (int index = 0; index < ___activeColHashCount; ++index)
                {
                    if (___activeColHashes[index] == n)
                        break;
                }
                ___activeColHashes[___activeColHashCount++] = n;
            }
            if (___activeColHashCount > 0)
            {
                for (int index1 = 0; index1 < ___activeColHashCount; ++index1)
                {
                    int activeColHash = ___activeColHashes[index1];
                    ColliderData[] colliderPool = ___colChunks[activeColHash].colliderPool;
                    for (int index2 = 1; index2 < ___colChunks[activeColHash].cursor; ++index2)
                    {
                        if (colliderPool[index2].idType != 0 && colliderPool[index2].usage != EColliderUsage.Build &&
                            colliderPool[index2].objType == EObjectType.Vegetable &&
                            (double)(colliderPool[index2].pos - center).sqrMagnitude <= (double)areaRadius * (double)areaRadius + (double)colliderPool[index2].ext.sqrMagnitude)
                        {
                            bool flag = false;
                            for (int index3 = 0; index3 < num; ++index3)
                            {
                                if (vegeIds[index3] == colliderPool[index2].objId)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                                vegeIds[num++] = colliderPool[index2].objId;
                        }
                    }
                }
            }
            __result = num;
            return false;
        }
    }
}