using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static partial class PatchOnBuildTool_PathAddon
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Addon), nameof(BuildTool_Addon.FindPotentialBeltStrict))]
        private static bool FindPotentialBeltStrict(BuildTool_Addon __instance)
        {
            if (__instance.handbp == null) return false;
            Array.Clear(__instance.potentialBeltObjIdArray, 0, __instance.potentialBeltObjIdArray.Length);
            __instance.potentialBeltCursor = 0;
            var addonAreaColPoses = __instance.handbp.desc.addonAreaColPoses;
            var addonAreaSize = __instance.handbp.desc.addonAreaSize;
            for (var i = 0; i < addonAreaColPoses.Length; i++)
            {
                var lineStart = __instance.handbp.lpos + __instance.handbp.lrot * (addonAreaColPoses[i].position + addonAreaColPoses[i].forward * addonAreaSize[i].z * 2.5f);
                var lineEnd = __instance.handbp.lpos + __instance.handbp.lrot * (addonAreaColPoses[i].position - addonAreaColPoses[i].forward * addonAreaSize[i].z * 2.5f);
                var center = __instance.handbp.lpos + __instance.handbp.lrot * addonAreaColPoses[i].position;
                var orientation = __instance.handbp.lrot * addonAreaColPoses[i].rotation;
                var halfExtents = addonAreaSize[i] * GameMain.localPlanet.radius / 200f; // scale by planet radius - starfish
                var mask = 428032;
                Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                var num = Physics.OverlapBoxNonAlloc(center, halfExtents, BuildTool._tmp_cols, orientation, mask, QueryTriggerInteraction.Collide);
                if (num > 0)
                {
                    var physics = __instance.player.planetData.physics;
                    for (var j = 0; j < num; j++)
                    {
                        ColliderData colliderData2;
                        var colliderData = physics.GetColliderData(BuildTool._tmp_cols[j], out colliderData2);
                        var num2 = 0;
                        if (colliderData && colliderData2.isForBuild)
                        {
                            if (colliderData2.objType == EObjectType.Entity)
                                num2 = colliderData2.objId;
                            else if (colliderData2.objType == EObjectType.Prebuild) num2 = -colliderData2.objId;
                        }

                        var prefabDesc = __instance.GetPrefabDesc(num2);
                        var objectPose = __instance.GetObjectPose(num2);
                        if (prefabDesc != null && prefabDesc.isBelt && Maths.DistancePointLine(objectPose.position, lineStart, lineEnd) <= 0.3f)
                        {
                            if (__instance.potentialBeltCursor >= __instance.potentialBeltObjIdArray.Length)
                            {
                                var array = __instance.potentialBeltObjIdArray;
                                __instance.potentialBeltObjIdArray = new int[__instance.potentialBeltCursor * 2];
                                if (array != null) Array.Copy(array, __instance.potentialBeltObjIdArray, __instance.potentialBeltCursor);
                            }

                            __instance.potentialBeltObjIdArray[__instance.potentialBeltCursor] = ((Mathf.Abs(num2) << 4) + i) * (int)Mathf.Sign(num2);
                            __instance.potentialBeltCursor++;
                        }
                    }
                }
            }

            return false;
        }
    }
}