using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    internal static class PatchOnNearColliderLogic
    {
        public static int offset = -20;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NearColliderLogic), "GetVeinsInAreaNonAlloc")]
        public static bool GetVeinsInAreaNonAlloc(ref NearColliderLogic __instance, ref int __result, Vector3 center,
            float areaRadius, int[] veinIds, ref int ___activeColHashCount, ref int[] ___activeColHashes,
            ref ColliderContainer[] ___colChunks)
        {
            if (veinIds == null) return true;

            var num = 0;
            areaRadius += offset;
            var MarkActivePos = Traverse.Create(__instance).Method("MarkActivePos", new[] {typeof(Vector3)});
            Array.Clear(veinIds, 0, veinIds.Length);
            var normalized = center.normalized;
            var lhs = Vector3.Cross(normalized, Vector3.up).normalized;
            Vector3 vector3_1;
            if (lhs.sqrMagnitude < 0.25)
            {
                lhs = Vector3.right;
                vector3_1 = Vector3.forward;
            }
            else
            {
                vector3_1 = Vector3.Cross(lhs, normalized).normalized;
            }

            lhs *= (float) ((areaRadius + 3.0) * 0.5);
            var vector3_2 = vector3_1 * (float) ((areaRadius + 3.0) * 0.5);
            ___activeColHashCount = 0;
            MarkActivePos.GetValue(center);
            MarkActivePos.GetValue(center + lhs);
            MarkActivePos.GetValue(center - lhs);
            MarkActivePos.GetValue(center + vector3_2);
            MarkActivePos.GetValue(center - vector3_2);
            MarkActivePos.GetValue(center + lhs + vector3_2);
            MarkActivePos.GetValue(center - lhs + vector3_2);
            MarkActivePos.GetValue(center + lhs - vector3_2);
            MarkActivePos.GetValue(center - lhs - vector3_2);
            MarkActivePos.GetValue(center + lhs * 2f);
            MarkActivePos.GetValue(center - lhs * 2f);
            MarkActivePos.GetValue(center + vector3_2 * 2f);
            MarkActivePos.GetValue(center - vector3_2 * 2f);
            MarkActivePos.GetValue(center + lhs * 2f + vector3_2 * 2f);
            MarkActivePos.GetValue(center - lhs * 2f + vector3_2 * 2f);
            MarkActivePos.GetValue(center + lhs * 2f - vector3_2 * 2f);
            MarkActivePos.GetValue(center - lhs * 2f - vector3_2 * 2f);
            if (___activeColHashCount > 0)
                for (var index1 = 0; index1 < ___activeColHashCount; ++index1)
                {
                    var activeColHash = ___activeColHashes[index1];
                    var colliderPool = ___colChunks[activeColHash].colliderPool;
                    for (var index2 = 1; index2 < ___colChunks[activeColHash].cursor; ++index2)
                        if (colliderPool[index2].idType != 0 && colliderPool[index2].usage != EColliderUsage.Build &&
                            colliderPool[index2].objType == EObjectType.Vein &&
                            (colliderPool[index2].pos - center).sqrMagnitude <= areaRadius * (double) areaRadius +
                            colliderPool[index2].ext.sqrMagnitude)
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