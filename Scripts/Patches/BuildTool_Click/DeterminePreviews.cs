using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Click
    {
        //          IL_06e8: ldarg.0      // this
        //          IL_06e9: ldfld        class PlanetData BuildTool::planet
        //          IL_06ee: brfalse.s    IL_072c
        //
        //          IL_06f0: ldarg.0      // this
        //          IL_06f1: ldfld        class PlanetData BuildTool::planet
        //          IL_06f6: ldfld        valuetype EPlanetType PlanetData::'type'
        //          IL_06fb: ldc.i4.5
        //          IL_06fc: bne.un.s     IL_072c
        //
        //          // [359 15 - 359 83]
        //          IL_06fe: ldloca.s     pos
        //          IL_0700: call         instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
        //          IL_0705: ldarg.0      // this
        //          IL_0706: ldfld        class PlanetData BuildTool::planet
        //          IL_070b: callvirt     instance float32 PlanetData::get_realRadius()
        //          IL_0710: call         valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
        //          IL_0715: ldc.r4       0.025
        //          IL_071a: call         valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
        //          IL_071f: stloc.s      vector3_3

        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Click), "DeterminePreviews")]
        public static IEnumerable<CodeInstruction> BuildTool_Click_DeterminePreviews_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).MatchForward(true,
            new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_realRadius"),
            new CodeMatch(OpCodes.Call)) 
            .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(realRadius => { return Mathf.Min(realRadius * 0.025f, 20f)/0.025f; }))
            .InstructionEnumeration();
            return instructions;
        }
    }
    //     [HarmonyPrefix]
    //     [HarmonyPatch(typeof(BuildTool_Click), "DeterminePreviews")]
    //     public static bool DeterminePreviews(ref BuildTool_Click __instance)
    //     {
    //         __instance.waitForConfirm = false;
    //         if (__instance.cursorValid)
    //         {
    //             if (VFInput._switchModelStyle.onDown)
    //             {
    //                 if (__instance.handItem.ModelCount > 1)
    //                 {
    //                     __instance.modelOffset++;
    //                     BuildingParameters.template.SetEmpty();
    //                     __instance.actionBuild.NotifyTemplateChange();
    //                 }
    //                 else if (__instance.isDragging)
    //                 {
    //                     if (__instance.tabgapDir)
    //                     {
    //                         if (__instance.gap < 0.3f)
    //                             __instance.gap = 0.3333333f;
    //                         else if (__instance.gap < 0.4f)
    //                             __instance.gap = 0.5f;
    //                         else if (__instance.gap < 0.6f)
    //                             __instance.gap = 1f;
    //                         else if (__instance.gap < 3.5f)
    //                             __instance.gap += 1f;
    //                         else
    //                             __instance.tabgapDir = false;
    //                     }
    //                     else if (__instance.gap > 1.5f)
    //                     {
    //                         __instance.gap -= 1f;
    //                     }
    //                     else if (__instance.gap > 0.6f)
    //                     {
    //                         __instance.gap = 0.5f;
    //                     }
    //                     else if (__instance.gap > 0.4f)
    //                     {
    //                         __instance.gap = 0.3333333f;
    //                     }
    //                     else if (__instance.gap > 0.3f)
    //                     {
    //                         __instance.gap = 0f;
    //                     }
    //                     else
    //                     {
    //                         __instance.tabgapDir = true;
    //                     }
    //                 }
    //             }
    //
    //             var flag = !__instance.multiLevelCovering && __instance.handPrefabDesc.dragBuild;
    //             if (__instance.handPrefabDesc.geothermal && VFInput._ignoreGrid)
    //                 flag = false;
    //             if (VFInput._buildConfirm.onDown && __instance.controller.cmd.stage != 1)
    //             {
    //                 __instance.controller.cmd.stage = 1;
    //                 __instance.startGroundPosSnapped = __instance.castGroundPosSnapped;
    //                 if (flag) __instance.isDragging = true;
    //             }
    //
    //             if (__instance.controller.cmd.stage == 0)
    //             {
    //                 __instance.startGroundPosSnapped = __instance.castGroundPosSnapped;
    //                 __instance.isDragging = false;
    //             }
    //
    //             __instance.waitForConfirm = __instance.controller.cmd.stage == 1;
    //             if (__instance.isDragging)
    //             {
    //                 if (VFInput._cursorPlusKey.onDown)
    //                 {
    //                     if (__instance.gap < 0.3f)
    //                         __instance.gap = 0.3333333f;
    //                     else if (__instance.gap < 0.4f)
    //                         __instance.gap = 0.5f;
    //                     else if (__instance.gap < 0.6f)
    //                         __instance.gap = 1f;
    //                     else if (__instance.gap < 3.5f) __instance.gap += 1f;
    //                 }
    //
    //                 if (VFInput._cursorMinusKey.onDown)
    //                 {
    //                     if (__instance.gap > 1.5f)
    //                         __instance.gap -= 1f;
    //                     else if (__instance.gap > 0.6f)
    //                         __instance.gap = 0.5f;
    //                     else if (__instance.gap > 0.4f)
    //                         __instance.gap = 0.3333333f;
    //                     else if (__instance.gap > 0.3f) __instance.gap = 0f;
    //                 }
    //             }
    //
    //             if (VFInput._ignoreGrid && __instance.handPrefabDesc.minerType == EMinerType.Vein || __instance.handPrefabDesc.geothermal)
    //             {
    //                 if (VFInput._rotate)
    //                 {
    //                     __instance.yaw += 3f;
    //                     __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
    //                 }
    //
    //                 if (VFInput._counterRotate)
    //                 {
    //                     __instance.yaw -= 3f;
    //                     __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
    //                 }
    //             }
    //             else
    //             {
    //                 if (VFInput._rotate.onDown)
    //                 {
    //                     __instance.yaw += 90f;
    //                     __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
    //                     __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
    //                 }
    //
    //                 if (VFInput._counterRotate.onDown)
    //                 {
    //                     __instance.yaw -= 90f;
    //                     __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
    //                     __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
    //                 }
    //
    //                 if (__instance.handPrefabDesc.minerType != EMinerType.Vein)
    //                     __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
    //             }
    //
    //             Array.Clear(__instance.dotsSnapped, 0, __instance.dotsSnapped.Length);
    //             var num = 1;
    //             if (__instance.isDragging)
    //                 num = __instance.planet.aux.SnapDotsNonAlloc(__instance.startGroundPosSnapped, __instance.castGroundPosSnapped, __instance.handPrefabDesc.dragBuildDist, __instance.yaw, __instance.gap, __instance.dotsSnapped);
    //             else
    //                 __instance.dotsSnapped[0] = __instance.cursorTarget;
    //             var num2 = 1;
    //             var templatePreviews = __instance.actionBuild.templatePreviews;
    //             var flag2 = templatePreviews.Count > 0;
    //             if (flag2) num2 = templatePreviews.Count;
    //             var num3 = num * num2;
    //             while (__instance.buildPreviews.Count < num3) __instance.buildPreviews.Add(new BuildPreview());
    //             while (__instance.buildPreviews.Count > num3)
    //                 __instance.buildPreviews.RemoveAt(__instance.buildPreviews.Count - 1);
    //             for (var i = 0; i < num; i++)
    //             {
    //                 for (var j = 0; j < num2; j++)
    //                 {
    //                     var buildPreview = __instance.buildPreviews[i * num2 + j];
    //                     var buildPreview2 = flag2 ? templatePreviews[j] : null;
    //                     if (buildPreview2 == null)
    //                     {
    //                         buildPreview.ResetAll();
    //                         buildPreview.item = __instance.handItem;
    //                         buildPreview.desc = __instance.handPrefabDesc;
    //                         buildPreview.needModel = __instance.handPrefabDesc.lodCount > 0 && __instance.handPrefabDesc.lodMeshes[0] != null;
    //                     }
    //                     else
    //                     {
    //                         buildPreview.Clone(buildPreview2);
    //                     }
    //
    //                     if (j == 0)
    //                     {
    //                         var magnitude = buildPreview.desc.buildCollider.ext.magnitude;
    //                         buildPreview.genNearColliderArea2 = (magnitude + 4f) * (magnitude + 4f);
    //                     }
    //
    //                     var point = buildPreview2 == null ? Vector3.zero : buildPreview2.lpos;
    //                     var rhs = buildPreview2 == null ? Quaternion.identity : buildPreview2.lrot;
    //                     var point2 = buildPreview2 == null ? Vector3.zero : buildPreview2.lpos2;
    //                     var rhs2 = buildPreview2 == null ? Quaternion.identity : buildPreview2.lrot2;
    //                     Vector3 vector;
    //                     Quaternion quaternion;
    //                     if (__instance.multiLevelCovering)
    //                     {
    //                         if (j == 0)
    //                         {
    //                             buildPreview.input = null;
    //                             buildPreview.inputObjId = __instance.castObjectId;
    //                             buildPreview.inputFromSlot = 15;
    //                             buildPreview.inputToSlot = 14;
    //                             buildPreview.inputOffset = 0;
    //                         }
    //
    //                         var objectPose = __instance.GetObjectPose(__instance.castObjectId);
    //                         vector = objectPose.position + objectPose.rotation * __instance.handPrefabDesc.lapJoint;
    //                         quaternion = __instance.handPrefabDesc.multiLevelAllowRotate ? Maths.SphericalRotation(vector, __instance.yaw) : objectPose.rotation;
    //                     }
    //                     else
    //                     {
    //                         vector = __instance.dotsSnapped[i];
    //                         if (__instance.planet != null && __instance.planet.type == EPlanetType.Gas)
    //                         {
    //                             var b = vector.normalized * Mathf.Min(__instance.planet.realRadius * 0.025f, 20f);
    //                             vector += b;
    //                         }
    //
    //                         quaternion = Maths.SphericalRotation(vector, __instance.yaw);
    //                     }
    //
    //                     buildPreview.lpos = vector + quaternion * point;
    //                     buildPreview.lrot = quaternion * rhs;
    //                     buildPreview.lpos2 = vector + quaternion * point2;
    //                     buildPreview.lrot2 = quaternion * rhs2;
    //                     if (buildPreview.desc.isInserter)
    //                     {
    //                         var num4 = buildPreview.output != null ? buildPreview.outputToSlot : buildPreview.input != null ? buildPreview.inputFromSlot : -1;
    //                         if (num4 >= 0)
    //                         {
    //                             var buildPreview3 = __instance.buildPreviews[i * num2];
    //                             var vector2 = buildPreview.lpos2 - buildPreview.lpos;
    //                             var num5 = vector2.magnitude;
    //                             vector2.Normalize();
    //                             var num6 = __instance.actionBuild.planetAux.activeGrid.CalcLocalGridSize(buildPreview3.lpos, vector2);
    //                             var pose = buildPreview3.desc.slotPoses[num4];
    //                             var forward = pose.forward;
    //                             var num7 = Mathf.Abs(forward.x) > Mathf.Abs(forward.z) ? Mathf.Abs(pose.position.x) : Mathf.Abs(pose.position.z);
    //                             num5 = Mathf.Round((num7 + num6 * (num5 + 0.0001f)) / num6) * num6 - num7;
    //                             if (buildPreview.output != null)
    //                                 buildPreview.lpos = -vector2 * num5 + buildPreview.lpos2;
    //                             else
    //                                 buildPreview.lpos2 = vector2 * num5 + buildPreview.lpos;
    //                         }
    //                     }
    //                 }
    //
    //                 for (var k = 0; k < num2; k++)
    //                 {
    //                     var buildPreview4 = __instance.buildPreviews[i * num2 + k];
    //                     for (var l = 0; l < templatePreviews.Count; l++)
    //                     {
    //                         if (buildPreview4.output == templatePreviews[l])
    //                             buildPreview4.output = __instance.buildPreviews[i * num2 + l];
    //                         if (buildPreview4.input == templatePreviews[l])
    //                             buildPreview4.input = __instance.buildPreviews[i * num2 + l];
    //                     }
    //                 }
    //
    //                 for (var m = 0; m < num2; m++)
    //                 {
    //                     var buildPreview5 = __instance.buildPreviews[i * num2 + m];
    //                     if (buildPreview5.desc.isInserter) __instance.MatchInserter(buildPreview5);
    //                 }
    //             }
    //
    //             return false;
    //         }
    //
    //         __instance.buildPreviews.Clear();
    //         return false;
    //     }
    }
