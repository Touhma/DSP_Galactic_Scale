using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public static partial class PatchOnBuildTool_PathAddon
    {
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(BuildTool_Addon), nameof(BuildTool_Addon.FindPotentialBeltStrict))]
        // private static bool FindPotentialBeltStrict(BuildTool_Addon __instance, int bpIndex)
        // {
        //     BuildPreview buildPreview = __instance.buildPreviews[bpIndex];
        //     if (buildPreview == null)
        //     {
        //         return false;
        //     }
        //
        //     Array.Clear(__instance.potentialBeltObjIdArray[bpIndex], 0,
        //         __instance.potentialBeltObjIdArray[bpIndex].Length);
        //     Pose[] addonAreaColPoses = buildPreview.desc.addonAreaColPoses;
        //     Vector3[] addonAreaSize = buildPreview.desc.addonAreaSize;
        //     for (int i = 0; i < addonAreaColPoses.Length; i++)
        //     {
        //         Vector3 lineStart = buildPreview.lpos + buildPreview.lrot * (addonAreaColPoses[i].position +
        //                                                                      addonAreaColPoses[i].forward *
        //                                                                      addonAreaSize[i].z * 2.5f);
        //         Vector3 lineEnd = buildPreview.lpos + buildPreview.lrot * (addonAreaColPoses[i].position -
        //                                                                    addonAreaColPoses[i].forward *
        //                                                                    addonAreaSize[i].z * 2.5f);
        //         Vector3 center = buildPreview.lpos + buildPreview.lrot * addonAreaColPoses[i].position;
        //         Quaternion orientation = buildPreview.lrot * addonAreaColPoses[i].rotation;
        //         // Vector3 halfExtents = addonAreaSize[i];
        //         Vector3 halfExtents = addonAreaSize[i] * GameMain.localPlanet.radius / 200f; // scale by planet radius
        //         int mask = 395264;
        //         Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
        //         int num = Physics.OverlapBoxNonAlloc(center, halfExtents, BuildTool._tmp_cols, orientation, mask,
        //             QueryTriggerInteraction.Collide);
        //         if (num > 0)
        //         {
        //             PlanetPhysics physics = __instance.player.planetData.physics;
        //             for (int j = 0; j < num; j++)
        //             {
        //                 ColliderData colliderData2;
        //                 bool colliderData = physics.GetColliderData(BuildTool._tmp_cols[j], out colliderData2);
        //                 int num2 = 0;
        //                 if (colliderData && colliderData2.isForBuild)
        //                 {
        //                     if (colliderData2.objType == EObjectType.Entity)
        //                     {
        //                         num2 = colliderData2.objId;
        //                     }
        //                     else if (colliderData2.objType == EObjectType.Prebuild)
        //                     {
        //                         num2 = -colliderData2.objId;
        //                     }
        //                 }
        //
        //                 PrefabDesc prefabDesc = __instance.GetPrefabDesc(num2);
        //                 Pose objectPose = __instance.GetObjectPose(num2);
        //                 if (prefabDesc != null && prefabDesc.isBelt &&
        //                     Maths.DistancePointLine(objectPose.position, lineStart, lineEnd) <= 0.3f)
        //                 {
        //                     if (__instance.potentialBeltCursorArray[bpIndex] >=
        //                         __instance.potentialBeltObjIdArray[bpIndex].Length)
        //                     {
        //                         int[] array = __instance.potentialBeltObjIdArray[bpIndex];
        //                         __instance.potentialBeltObjIdArray[bpIndex] =
        //                             new int[__instance.potentialBeltCursorArray[bpIndex] * 2];
        //                         if (array != null)
        //                         {
        //                             Array.Copy(array, __instance.potentialBeltObjIdArray[bpIndex],
        //                                 __instance.potentialBeltCursorArray[bpIndex]);
        //                         }
        //                     }
        //
        //                     __instance.potentialBeltObjIdArray[bpIndex][__instance.potentialBeltCursorArray[bpIndex]] =
        //                         ((Mathf.Abs(num2) << 4) + i) * (int)Mathf.Sign((float)num2);
        //                     __instance.potentialBeltCursorArray[bpIndex]++;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return false;
        // }
    }
}