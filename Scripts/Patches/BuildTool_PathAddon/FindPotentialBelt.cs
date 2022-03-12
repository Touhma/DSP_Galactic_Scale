using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static partial class PatchOnBuildTool_PathAddon
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_PathAddon), nameof(BuildTool_PathAddon.FindPotentialBelt))]
        private static bool FindPotentialBelt(BuildTool_PathAddon __instance)
        {
            Array.Clear(__instance.potentialBeltObjIdArray, 0, __instance.potentialBeltObjIdArray.Length);
            Array.Clear(__instance.addonAreaBeltObjIdArray, 0, __instance.addonAreaBeltObjIdArray.Length);
            __instance.potentialBeltCursor = 0;
            var addonAreaPoses = __instance.handbp.desc.addonAreaPoses;
            var addonAreaColPoses = __instance.handbp.desc.addonAreaColPoses;
            var addonAreaSize = __instance.handbp.desc.addonAreaSize;
            for (var i = 0; i < addonAreaColPoses.Length; i++)
            {
                var num = float.MaxValue;
                var num2 = 0;
                var b = __instance.handbp.lpos + __instance.handbp.lrot * addonAreaPoses[i].position;
                var center = __instance.handbp.lpos + __instance.handbp.lrot * addonAreaColPoses[i].position;
                var orientation = __instance.handbp.lrot * addonAreaColPoses[i].rotation;
                var halfExtents = addonAreaSize[i] * 2 * GameMain.localPlanet.radius / 200f; // scale by planet radius - starfish
                var mask = 428032;
                Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                var num3 = Physics.OverlapBoxNonAlloc(center, halfExtents, BuildTool._tmp_cols, orientation, mask, QueryTriggerInteraction.Collide);
                if (num3 > 0)
                {
                    var physics = __instance.player.planetData.physics;
                    for (var j = 0; j < num3; j++)
                    {
                        ColliderData colliderData2;
                        var colliderData = physics.GetColliderData(BuildTool._tmp_cols[j], out colliderData2);
                        var num4 = 0;
                        if (colliderData && colliderData2.isForBuild)
                        {
                            if (colliderData2.objType == EObjectType.Entity)
                                num4 = colliderData2.objId;
                            else if (colliderData2.objType == EObjectType.Prebuild) num4 = -colliderData2.objId;
                        }

                        var prefabDesc = __instance.GetPrefabDesc(num4);
                        if (prefabDesc != null && prefabDesc.isBelt)
                        {
                            if (__instance.potentialBeltCursor >= __instance.potentialBeltObjIdArray.Length)
                            {
                                var array = __instance.potentialBeltObjIdArray;
                                __instance.potentialBeltObjIdArray = new int[__instance.potentialBeltCursor * 2];
                                if (array != null) Array.Copy(array, __instance.potentialBeltObjIdArray, __instance.potentialBeltCursor);
                            }

                            __instance.potentialBeltObjIdArray[__instance.potentialBeltCursor] = ((Mathf.Abs(num4) << 4) + i) * (int)Mathf.Sign((float)num4);
                            __instance.potentialBeltCursor++;
                            var magnitude = (__instance.GetObjectPose(num4).position - b).magnitude;
                            if (magnitude < num && magnitude < 2f)
                            {
                                num = magnitude;
                                num2 = num4;
                            }
                        }
                    }
                }

                __instance.addonAreaBeltObjIdArray[i] = num2;
            }
            return false;
        }

    }
}