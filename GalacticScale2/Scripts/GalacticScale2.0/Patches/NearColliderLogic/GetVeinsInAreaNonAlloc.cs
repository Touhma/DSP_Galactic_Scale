using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    internal static class PatchOnNearColliderLogic
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NearColliderLogic), "GetVeinsInAreaNonAlloc")]
        public static bool GetVeinsInAreaNonAlloc(ref NearColliderLogic __instance, ref int __result, Vector3 center, float areaRadius, int[] veinIds, ref int ___activeColHashCount, ref int[] ___activeColHashes, ref ColliderContainer[] ___colChunks)
        {
            if (veinIds == null) return true;

            Array.Clear(veinIds, 0, veinIds.Length);
            ___activeColHashCount = 0;

            var normalized = center.normalized;
            float step = 2 + Mathf.RoundToInt(__instance.planet.realRadius / 150);

            var right = Vector3.Cross(normalized, Vector3.up).normalized;
            Vector3 forward;
            if (right.sqrMagnitude < 0.25)
            {
                right = Vector3.right;
                forward = Vector3.forward;
            }
            else
            {
                forward = Vector3.Cross(right, normalized).normalized;
            }

            var count = Mathf.RoundToInt(areaRadius / step);

            var centerMag = center.magnitude;

            for (var x = -count; x <= count; x++)
            for (var y = -count; y <= count; y++)
            {
                var xMag = x * step;
                var yMag = y * step;

                var position = center + right * xMag + forward * yMag;

                if (__instance.planet.realRadius < 100) position = position.normalized * centerMag;

                __instance.MarkActivePos(position);
            }

            var num = 0;

            if (___activeColHashCount > 0)
                for (var index1 = 0; index1 < ___activeColHashCount; ++index1)
                {
                    var activeColHash = ___activeColHashes[index1];
                    var colliderPool = ___colChunks[activeColHash].colliderPool;
                    for (var index2 = 1; index2 < ___colChunks[activeColHash].cursor; ++index2)
                        if (colliderPool[index2].idType != 0 && colliderPool[index2].usage != EColliderUsage.Build && colliderPool[index2].objType == EObjectType.Vein && (colliderPool[index2].pos - center).sqrMagnitude <= areaRadius * (double)areaRadius + colliderPool[index2].ext.sqrMagnitude)
                        {
                            var flag = false;
                            for (var index3 = 0; index3 < num; ++index3)
                                if (veinIds[index3] == colliderPool[index2].objId)
                                {
                                    flag = true;
                                    break;
                                }

                            if (!flag) veinIds[num++] = colliderPool[index2].objId;
                        }
                }

            __result = num;
            return false;
        }
    }
}