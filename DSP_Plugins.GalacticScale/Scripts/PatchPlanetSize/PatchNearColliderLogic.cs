using HarmonyLib;
using System;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(NearColliderLogic))]
    static class PatchNearColliderLogic
    {
        public static int offset = 0;
        public static GameObject[] markers;

        public static void OnFixedUpdate()
        {
            int o = offset;
            if ((Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.KeypadPlus)) && VFInput.alt)
                ++offset;
            if ((Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) && VFInput.alt)
                --offset;
            if (o != offset) Patch.Debug(offset, BepInEx.Logging.LogLevel.Message, true);

        }
        [HarmonyPatch("GetVeinsInAreaNonAlloc")]
        public static bool GetVeinsInAreaNonAlloc(ref int __result, Vector3 center, float areaRadius, int[] veinIds, ref int ___activeColHashCount, ref int[] ___activeColHashes, ref ColliderContainer[] ___colChunks)
        {
            if (veinIds == null) return true;
            int num = 0;
            Array.Clear((Array)veinIds, 0, veinIds.Length);
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
            lhs *= areaRadius + offset; // Original had areaRadius + 3f; -innominata
            Vector3 vector3_2 = vector3_1 * (areaRadius + offset); // Original had areaRadius + 3f; -innominata
            Vector3[] positions = { //I've inlined the private function -innominata
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
            //if (markers.Length > 0)
            //{
            //    foreach (GameObject m in markers)
            //    {
            //        GameObject.Destroy(m);
            //    }
            //}
            if (markers == null)
            {
                markers = new GameObject[positions.Length];
                Patch.Debug("Created Markers", BepInEx.Logging.LogLevel.Message, true);
                for (int i = 0; i < positions.Length; i++)
                {
                    markers[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                }
            }
            for (int i = 0; i < positions.Length; i++)
            { 
                markers[i].transform.position = positions[i];
            }
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
            __result = num;
            return false;
        }
    }
}