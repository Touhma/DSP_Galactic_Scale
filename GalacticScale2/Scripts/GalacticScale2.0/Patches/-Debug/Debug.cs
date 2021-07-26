using HarmonyLib;
using System.Collections.Generic;
using System;
using UnityEngine;
using PowerNetworkStructures;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIBuildingGrid), "Start")]
        private static void Start(ref UIBuildingGrid __instance)
        {
            //__instance.material = UnityEngine.Object.Instantiate<Material>(__instance.gridRnd.sharedMaterial);
            //__instance.gridRnd.sharedMaterial = __instance.material;
            //__instance.altMaterial = UnityEngine.Object.Instantiate<Material>(__instance.altGridRnd.sharedMaterial);
            //__instance.altGridRnd.sharedMaterial = __instance.altMaterial;
            //__instance.blueprintMaterial = UnityEngine.Object.Instantiate<Material>(__instance.gridRnd.sharedMaterial);
            //__instance.blueprintGridRnd.sharedMaterial = __instance.blueprintMaterial;
           
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIBuildingGrid), "Update")]
        private static bool Update(ref UIBuildingGrid __instance)
        {
            PlanetData planetData = GameMain.localPlanet;
            Player mainPlayer = GameMain.mainPlayer;
            PlanetFactory planetFactory = (planetData == null) ? null : planetData.factory;
            if (planetFactory == null || !planetData.factoryLoaded)
            {
                planetData = null;
            }
            if (__instance.reformMapPlanet != planetData)
            {
                __instance.InitReformMap(planetData);
            }
            PlanetGrid planetGrid = null;
            if (mainPlayer != null && planetData != null && planetData.aux != null && planetData.aux.activeGridIndex < planetData.aux.customGrids.Count)
            {
                planetGrid = planetData.aux.customGrids[planetData.aux.activeGridIndex];
            }
            if (planetGrid != null)
            {
                float realRadius = planetData.realRadius;
                int altitude = mainPlayer.controller.actionBuild.pathTool.altitude;
                float num = realRadius * 2f;
                float num2 = (realRadius + 0.2f + (float)altitude * 1.3333333f) * 2f;
                CommandState cmd = mainPlayer.controller.cmd;
                if (cmd.state == 1 && planetData.type != EPlanetType.Gas)
                {
                    Vector3 test = cmd.test;
                    num = test.magnitude * 2f;
                    if (cmd.mode == 4)
                    {
                        float num3 = (realRadius + 0.2f) * 2f;
                        num = ((num > num3) ? num : num3);
                    }
                }
                if (Mathf.Abs(__instance.displayScale - num) > 10f)
                {
                    __instance.displayScale = num;
                }
                else
                {
                    __instance.displayScale = __instance.displayScale * 0.8f + num * 0.2f;
                }
                PlatformSystem platformSystem = planetFactory.platformSystem;
                int num4 = 0;
                if (__instance.reformCursorMap != null)
                {
                    num4 = __instance.reformCursorMap.Length;
                    Assert.False(__instance.reformCursorMap.Length != platformSystem.maxReformCount, "断言失败：cursorMap长度不相等");
                }
                PlayerAction_Build actionBuild = mainPlayer.controller.actionBuild;
                BuildTool_Reform reformTool = actionBuild.reformTool;
                ComputeBuffer reformOffsetsBuffer = platformSystem.reformOffsetsBuffer;
                ComputeBuffer reformDataBuffer = platformSystem.reformDataBuffer;
                if (cmd.type != ECommand.Build)
                {
                    __instance.gridRnd.enabled = false;
                    __instance.altGridRnd.enabled = false;
                    __instance.blueprintGridRnd.enabled = false;
                    return false;
                }
                __instance.gridRnd.enabled = (actionBuild.blueprintMode == EBlueprintMode.None);
                __instance.gridRnd.transform.localScale = new Vector3(__instance.displayScale, __instance.displayScale, __instance.displayScale);
                __instance.gridRnd.transform.rotation = planetGrid.rotation;
                __instance.altGridRnd.enabled = (altitude > 0);
                __instance.altGridRnd.transform.localScale = new Vector3(num2, num2, num2);
                __instance.blueprintGridRnd.transform.localScale = new Vector3(num2, num2, num2);
                __instance.material.SetFloat("_Segment", (float)planetGrid.segment);
                int[] cursorIndices = reformTool.cursorIndices;
                int num5 = reformTool.brushSize * reformTool.brushSize;
                int mode = cmd.mode;
                Vector4 value = Vector4.zero;
                if (mode == -1)
                {
                    __instance.material.SetColor("_TintColor", __instance.dismantleColor);
                    __instance.material.SetFloat("_ReformMode", 0f);
                    __instance.material.SetFloat("_ZMin", -0.5f);
                    num5 = 0;
                    if (actionBuild.dismantleTool.cursorType > 0 && actionBuild.dismantleTool.castGround)
                    {
                        value = planetGrid.GratboxByCenterSize(actionBuild.dismantleTool.castGroundPos, actionBuild.dismantleTool.cursorSize);
                    }
                }
                else if (mode == -2)
                {
                    __instance.material.SetColor("_TintColor", (actionBuild.upgradeTool.upgradeLevel >= 0) ? __instance.upgradeColor : __instance.downgradeColor);
                    __instance.material.SetFloat("_ReformMode", 0f);
                    __instance.material.SetFloat("_ZMin", -0.5f);
                    num5 = 0;
                    if (actionBuild.upgradeTool.cursorType > 0 && actionBuild.upgradeTool.castGround)
                    {
                        value = planetGrid.GratboxByCenterSize(actionBuild.upgradeTool.castGroundPos, actionBuild.upgradeTool.cursorSize);
                    }
                }
                else if (mode == 4)
                {
                    __instance.material.SetColor("_TintColor", __instance.reformColor);
                    __instance.material.SetFloat("_ReformMode", 1f);
                    __instance.material.SetFloat("_ZMin", -1.5f);
                }
                else
                {
                    __instance.material.SetColor("_TintColor", __instance.buildColor);
                    __instance.material.SetFloat("_ReformMode", 0f);
                    __instance.material.SetFloat("_ZMin", -0.5f);
                    num5 = 0;
                }
                __instance.material.SetVector("_CursorGratBox", value);
                if (!VFInput.onGUI)
                {
                    for (int i = 0; i < num5; i++)
                    {
                        int num6 = cursorIndices[i];
                        if (num6 >= 0 && num6 < num4)
                        {
                            __instance.reformCursorMap[num6] = 1;
                        }
                    }
                }
                if (platformSystem.reformData != null)
                {
                    reformDataBuffer.SetData(platformSystem.reformData);
                    __instance.material.SetBuffer("_DataBuffer", reformDataBuffer);
                }
                reformOffsetsBuffer.SetData(platformSystem.reformOffsets);
                __instance.reformCursorBuffer.SetData(__instance.reformCursorMap);
                __instance.material.SetBuffer("_OffsetsBuffer", reformOffsetsBuffer);
                __instance.material.SetBuffer("_CursorBuffer", __instance.reformCursorBuffer);
                if (!VFInput.onGUI)
                {
                    for (int j = 0; j < num5; j++)
                    {
                        int num7 = cursorIndices[j];
                        if (num7 >= 0 && num7 < num4)
                        {
                            __instance.reformCursorMap[num7] = 0;
                        }
                    }
                }
                if (actionBuild.blueprintMode == EBlueprintMode.None)
                {
                    __instance.blueprintGridRnd.enabled = false;
                    return false;
                }
                if (actionBuild.blueprintMode == EBlueprintMode.Copy)
                {
                    ///Added
                    __instance.blueprintMaterial.SetFloat("_Segment", GameMain.localPlanet.realRadius);
                    //__instance.blueprintMaterial.SetColor("_TintColor", Color.cyan);
                    __instance.blueprintMaterial.SetColor("_TintColor", __instance.blueprintColor);
                    for (int k = 0; k < 64; k++)
                    {
                        if (k < actionBuild.blueprintCopyTool.curActiveAreaGratBoxCursor)
                        {
                            __instance.blueprintMaterial.SetVector("_CursorGratBox", Vector4.zero);
                            __instance.blueprintMaterial.SetVector("_CursorGratBox" + k.ToString(), (Vector4)actionBuild.blueprintCopyTool.displayGratBoxArr[k]);
                            __instance.blueprintMaterial.SetFloat("_CursorGratBoxInfo" + k.ToString(), 0f);
                        }
                        else
                        {
                            __instance.blueprintMaterial.SetVector("_CursorGratBox", Vector4.zero);
                            __instance.blueprintMaterial.SetVector("_CursorGratBox" + k.ToString(), Vector4.zero);
                            __instance.blueprintMaterial.SetFloat("_CursorGratBoxInfo" + k.ToString(), 0f);
                        }
                    }
                    if (actionBuild.blueprintCopyTool.currSelectMethod == BuildTool_BlueprintCopy.SelectMethod.None)
                    {
                        __instance.blueprintMaterial.SetVector("_CursorGratBox", Vector4.zero);
                        __instance.blueprintMaterial.SetVector("_SelectColor", Vector4.one);
                    }
                    else if (actionBuild.blueprintCopyTool.currSelectMethod == BuildTool_BlueprintCopy.SelectMethod.Add)
                    {
                        __instance.blueprintMaterial.SetVector("_CursorGratBox", (Vector4)actionBuild.blueprintCopyTool.preSelectGratBox);
                        __instance.blueprintMaterial.SetVector("_SelectColor", Vector4.one * 0.85f);
                    }
                    else if (actionBuild.blueprintCopyTool.currSelectMethod == BuildTool_BlueprintCopy.SelectMethod.Sub)
                    {
                        __instance.blueprintMaterial.SetVector("_CursorGratBox", (Vector4)actionBuild.blueprintCopyTool.preSelectGratBox);
                        __instance.blueprintMaterial.SetVector("_SelectColor", Vector4.one * 0.15f);
                    }
                    __instance.blueprintMaterial.SetFloat("_DivideLine", actionBuild.blueprintCopyTool.divideLineRad*2);
                    //__instance.blueprintMaterial.SetFloat("_DivideLine", actionBuild.blueprintCopyTool.divideLineRad*2);
                    __instance.blueprintGridRnd.enabled = true;
                    GS2.Warn(__instance.blueprintMaterial.GetFloat("_Segment").ToString());
                    return false;
                }
                if (actionBuild.blueprintMode == EBlueprintMode.Paste)
                {
                    __instance.blueprintMaterial.SetVector("_CursorGratBox", Vector4.zero);
                    for (int l = 0; l < 64; l++)
                    {
                        if (l < actionBuild.blueprintPasteTool.gratBoxCursor)
                        {
                            IntVector4 intVector = actionBuild.blueprintPasteTool.bpGratBoxConditionArr[l];
                            __instance.blueprintMaterial.SetVector("_CursorGratBox" + l.ToString(), (Vector4)actionBuild.blueprintPasteTool.bpGratBoxArr[l]);
                            __instance.blueprintMaterial.SetFloat("_CursorGratBoxInfo" + l.ToString(), (float)(actionBuild.blueprintPasteTool.cannotBuild ? 1 : (intVector.x + intVector.y + intVector.z + intVector.w)));
                        }
                        else
                        {
                            __instance.blueprintMaterial.SetVector("_CursorGratBox" + l.ToString(), Vector4.zero);
                            __instance.blueprintMaterial.SetFloat("_CursorGratBoxInfo" + l.ToString(), 0f);
                        }
                    }
                    __instance.blueprintMaterial.SetColor("_TintColor", __instance.blueprintColor);
                    //__instance.blueprintMaterial.SetColor("_TintColor", Color.red);
                    __instance.blueprintGridRnd.enabled = true;
                    return false;
                }
            }
            else
            {
                __instance.gridRnd.enabled = false;
                __instance.altGridRnd.enabled = false;
                __instance.blueprintGridRnd.enabled = false;
            }
            return false;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(BuildTool_BlueprintPaste), "CheckBuildConditions")]
        public static bool CheckBuildConditions(ref BuildTool_BlueprintPaste __instance, ref bool __result)
        {
            GameHistoryData history = __instance.actionBuild.history;
            bool flag = false;
            for (int i = 0; i < __instance.bpCursor; i++)
            {
                BuildPreview buildPreview = __instance.bpPool[i];
                if (buildPreview.condition == EBuildCondition.Ok)
                {
                    Vector3 lpos = buildPreview.lpos;
                    Quaternion lrot = buildPreview.lrot;
                    Vector3 lpos2 = buildPreview.lpos2;
                    Quaternion lrot2 = buildPreview.lrot2;
                    Pose pose = new Pose(buildPreview.lpos, buildPreview.lrot);
                    Pose pose2 = new Pose(buildPreview.lpos2, buildPreview.lrot2);
                    Vector3 forward = pose.forward;
                    Vector3 forward2 = pose2.forward;
                    Vector3 up = pose.up;
                    Vector3 a = Vector3.Lerp(lpos, lpos2, 0.5f);
                    Vector3 forward3 = lpos2 - lpos;
                    if (forward3.sqrMagnitude < 0.0001f)
                    {
                        forward3 = Maths.SphericalRotation(lpos, 0f).Forward();
                    }
                    Quaternion quaternion = Quaternion.LookRotation(forward3, a.normalized);
                    bool flag2 = __instance.planet != null && __instance.planet.type == EPlanetType.Gas;
                    if (lpos.sqrMagnitude < 1f)
                    {
                        buildPreview.condition = EBuildCondition.Failure;
                        __instance.AddErrorMessage(buildPreview.condition);
                    }
                    else
                    {
                        bool flag3 = buildPreview.desc.minerType == EMinerType.None && !buildPreview.desc.isBelt && !buildPreview.desc.isSplitter && (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen || buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger) && !buildPreview.desc.isStation && !buildPreview.desc.isSilo && !buildPreview.desc.multiLevel;
                        if (buildPreview.desc.veinMiner)
                        {
                            Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                            Vector3 vector = lpos + forward * -1.2f;
                            Vector3 rhs = -forward;
                            Vector3 vector2 = up;
                            int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector, 12f, BuildTool._tmp_ids);
                            PrebuildData prebuildData = default(PrebuildData);
                            prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                            VeinData[] veinPool = __instance.factory.veinPool;
                            int paramCount = 0;
                            for (int j = 0; j < veinsInAreaNonAlloc; j++)
                            {
                                if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
                                {
                                    if (veinPool[BuildTool._tmp_ids[j]].type != EVeinType.Oil)
                                    {
                                        Vector3 vector3 = veinPool[BuildTool._tmp_ids[j]].pos - vector;
                                        float num = Vector3.Dot(vector2, vector3);
                                        vector3 -= vector2 * num;
                                        float sqrMagnitude = vector3.sqrMagnitude;
                                        float num2 = Vector3.Dot(vector3.normalized, rhs);
                                        if (sqrMagnitude <= 60.0625f && num2 >= 0.73f && Mathf.Abs(num) <= 2f)
                                        {
                                            prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[j];
                                        }
                                    }
                                }
                                else
                                {
                                    Assert.CannotBeReached();
                                }
                            }
                            prebuildData.paramCount = paramCount;
                            prebuildData.ArrageParametersArray();
                            buildPreview.parameters = prebuildData.parameters;
                            buildPreview.paramCount = prebuildData.paramCount;
                            Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                            if (prebuildData.paramCount == 0)
                            {
                                buildPreview.condition = EBuildCondition.NeedResource;
                                __instance.AddErrorMessage(buildPreview.condition);
                                goto IL_2278;
                            }
                        }
                        else if (buildPreview.desc.oilMiner)
                        {
                            Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                            Vector3 vector4 = lpos;
                            Vector3 vector5 = -up;
                            int veinsInAreaNonAlloc2 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector4, 10f, BuildTool._tmp_ids);
                            PrebuildData prebuildData2 = default(PrebuildData);
                            prebuildData2.InitParametersArray(veinsInAreaNonAlloc2);
                            VeinData[] veinPool2 = __instance.factory.veinPool;
                            int num3 = 0;
                            float num4 = 0.1f;
                            for (int k = 0; k < veinsInAreaNonAlloc2; k++)
                            {
                                if (BuildTool._tmp_ids[k] != 0 && veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k] && veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Oil)
                                {
                                    Vector3 pos = veinPool2[BuildTool._tmp_ids[k]].pos;
                                    Vector3 vector6 = pos - vector4;
                                    float d = Vector3.Dot(vector5, vector6);
                                    float sqrMagnitude2 = (vector6 - vector5 * d).sqrMagnitude;
                                    if (sqrMagnitude2 < num4)
                                    {
                                        num4 = sqrMagnitude2;
                                        num3 = BuildTool._tmp_ids[k];
                                    }
                                }
                            }
                            if (num3 == 0)
                            {
                                buildPreview.condition = EBuildCondition.NeedResource;
                                __instance.AddErrorMessage(buildPreview.condition);
                                goto IL_2278;
                            }
                            prebuildData2.parameters[0] = num3;
                            prebuildData2.paramCount = 1;
                            prebuildData2.ArrageParametersArray();
                            buildPreview.parameters = prebuildData2.parameters;
                            buildPreview.paramCount = prebuildData2.paramCount;
                            Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                        }
                        if (buildPreview.desc.isTank || buildPreview.desc.isStorage || buildPreview.desc.isLab || buildPreview.desc.isSplitter)
                        {
                            int num5 = buildPreview.desc.isLab ? history.labLevel : history.storageLevel;
                            int num6 = buildPreview.desc.isLab ? 15 : 8;
                            int num7 = 0;
                            float num8 = buildPreview.lpos.magnitude - (__instance.planet.realRadius + 0.2f);
                            if (buildPreview.desc.isTank)
                            {
                                num7 = Mathf.RoundToInt(num8 / 4f);
                            }
                            if (buildPreview.desc.isStorage)
                            {
                                if (buildPreview.desc.storageRow == 3)
                                {
                                    num7 = Mathf.RoundToInt(num8 / 2.666667f);
                                }
                                if (buildPreview.desc.storageRow == 6)
                                {
                                    num7 = Mathf.RoundToInt(num8 / 4f);
                                }
                            }
                            if (buildPreview.desc.isLab)
                            {
                                num7 = Mathf.RoundToInt(num8 / 4f);
                            }
                            if (buildPreview.desc.isSplitter)
                            {
                                num7 = Mathf.RoundToInt(num8 / 2.666667f);
                            }
                            if (num7 >= num5)
                            {
                                flag = (num5 >= num6);
                                buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
                                __instance.AddErrorMessage(buildPreview.condition);
                                goto IL_2278;
                            }
                        }
                        if (__instance.planet != null)
                        {
                            float num9 = history.buildMaxHeight + 0.5f + __instance.planet.realRadius * (flag2 ? 1.025f : 1f);
                            if (lpos.sqrMagnitude > num9 * num9)
                            {
                                buildPreview.condition = EBuildCondition.OutOfReach;
                                __instance.AddErrorMessage(buildPreview.condition);
                                goto IL_2278;
                            }
                        }
                        if (buildPreview.desc.isBelt && buildPreview.input == null)
                        {
                            if (buildPreview.output != null && buildPreview.output.desc.isBelt)
                            {
                                if ((buildPreview.output.lpos - buildPreview.lpos).sqrMagnitude > 5f)
                                {
                                    buildPreview.condition = EBuildCondition.TooFar;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                }
                            }
                            else if (buildPreview.outputObjId != 0)
                            {
                                PrefabDesc prefabDesc = __instance.GetPrefabDesc(buildPreview.outputObjId);
                                if (prefabDesc != null && prefabDesc.isBelt && (__instance.GetObjectPose(buildPreview.outputObjId).position - buildPreview.lpos).sqrMagnitude > 5f)
                                {
                                    buildPreview.condition = EBuildCondition.TooFar;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                }
                            }
                        }
                        if (buildPreview.desc.isInserter)
                        {
                            bool flag4 = buildPreview.input == null;
                            bool flag5 = buildPreview.output == null;
                            if (flag4)
                            {
                                buildPreview.inputObjId = 0;
                                __instance.MatchInserter(buildPreview);
                            }
                            if (flag5)
                            {
                                buildPreview.outputObjId = 0;
                                __instance.MatchInserter(buildPreview);
                            }
                        }
                        if (buildPreview.desc.hasBuildCollider)
                        {
                            ColliderData[] buildColliders = buildPreview.desc.buildColliders;
                            for (int l = 0; l < buildColliders.Length; l++)
                            {
                                ColliderData colliderData = buildPreview.desc.buildColliders[l];
                                if (buildPreview.desc.isInserter)
                                {
                                    colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y, Vector3.Distance(lpos2, lpos) * 0.5f + colliderData.ext.z - 0.5f);
                                    if (__instance.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt))
                                    {
                                        colliderData.pos.z = colliderData.pos.z - 0.35f;
                                        colliderData.ext.z = colliderData.ext.z + 0.35f;
                                    }
                                    else if (buildPreview.inputObjId == 0 && buildPreview.input == null)
                                    {
                                        colliderData.pos.z = colliderData.pos.z - 0.35f;
                                        colliderData.ext.z = colliderData.ext.z + 0.35f;
                                    }
                                    if (__instance.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt))
                                    {
                                        colliderData.pos.z = colliderData.pos.z + 0.35f;
                                        colliderData.ext.z = colliderData.ext.z + 0.35f;
                                    }
                                    else if (buildPreview.outputObjId == 0 && buildPreview.output == null)
                                    {
                                        colliderData.pos.z = colliderData.pos.z + 0.35f;
                                        colliderData.ext.z = colliderData.ext.z + 0.35f;
                                    }
                                    if (colliderData.ext.z < 0.1f)
                                    {
                                        colliderData.ext.z = 0.1f;
                                    }
                                    colliderData.pos = a + quaternion * colliderData.pos;
                                    colliderData.q = quaternion * colliderData.q;
                                    colliderData.DebugDraw();
                                }
                                else
                                {
                                    colliderData.pos = lpos + lrot * colliderData.pos;
                                    colliderData.q = lrot * colliderData.q;
                                }
                                int mask = 428032;
                                if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner)
                                {
                                    mask = 425984;
                                }
                                Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                                int num10 = Physics.OverlapBoxNonAlloc(colliderData.pos, colliderData.ext, BuildTool._tmp_cols, colliderData.q, mask, QueryTriggerInteraction.Collide);
                                if (num10 > 0)
                                {
                                    bool flag6 = false;
                                    PlanetPhysics physics = __instance.player.planetData.physics;
                                    int num11 = 0;
                                    while (num11 < num10 && buildPreview.coverObjId == 0)
                                    {
                                        ColliderData colliderData3;
                                        bool colliderData2 = physics.GetColliderData(BuildTool._tmp_cols[num11], out colliderData3);
                                        int num12 = 0;
                                        if (colliderData2 && colliderData3.isForBuild)
                                        {
                                            if (colliderData3.objType == EObjectType.Entity)
                                            {
                                                num12 = colliderData3.objId;
                                            }
                                            else if (colliderData3.objType == EObjectType.Prebuild)
                                            {
                                                num12 = -colliderData3.objId;
                                            }
                                        }
                                        Collider collider = BuildTool._tmp_cols[num11];
                                        if (collider.gameObject.layer == 18)
                                        {
                                            BuildPreviewModel component = collider.GetComponent<BuildPreviewModel>();
                                            if ((!(component != null) || component.index != buildPreview.previewIndex) && (!buildPreview.desc.isInserter || component.buildPreview.desc.isInserter))
                                            {
                                                if (buildPreview.desc.isInserter || !component.buildPreview.desc.isInserter)
                                                {
                                                    goto IL_C4E;
                                                }
                                            }
                                        }
                                        else if (!buildPreview.desc.isInserter || num12 == 0 || (num12 != buildPreview.inputObjId && num12 != buildPreview.outputObjId && (buildPreview.input == null || buildPreview.input.objId != num12) && (buildPreview.output == null || buildPreview.output.objId != num12) && (buildPreview.input == null || buildPreview.input.coverObjId != num12) && (buildPreview.output == null || buildPreview.output.coverObjId != num12)))
                                        {
                                            goto IL_C4E;
                                        }
                                        IL_E1A:
                                        num11++;
                                        continue;
                                        IL_C4E:
                                        flag6 = true;
                                        if (flag3 && num12 != 0)
                                        {
                                            ItemProto itemProto = __instance.GetItemProto(num12);
                                            if (buildPreview.item.IsSimilar(itemProto))
                                            {
                                                Pose objectPose = __instance.GetObjectPose(num12);
                                                Pose objectPose2 = __instance.GetObjectPose2(num12);
                                                if ((double)(objectPose.position - buildPreview.lpos).sqrMagnitude < 0.01 && (double)(objectPose2.position - buildPreview.lpos2).sqrMagnitude < 0.01 && ((double)(objectPose.forward - forward).sqrMagnitude < 1E-06 || buildPreview.desc.isInserter))
                                                {
                                                    if (buildPreview.item.ID == itemProto.ID)
                                                    {
                                                        buildPreview.coverObjId = num12;
                                                        buildPreview.willRemoveCover = false;
                                                        flag6 = false;
                                                        break;
                                                    }
                                                    buildPreview.coverObjId = num12;
                                                    buildPreview.willRemoveCover = true;
                                                    flag6 = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!buildPreview.desc.isBelt || num12 == 0 || (buildPreview.input != null && buildPreview.output != null) || !__instance.GetPrefabDesc(num12).isBelt)
                                        {
                                            goto IL_E1A;
                                        }
                                        bool flag7;
                                        int num13;
                                        int num14;
                                        if (buildPreview.input == null && buildPreview.output != null)
                                        {
                                            __instance.factory.ReadObjectConn(num12, 0, out flag7, out num13, out num14);
                                            if (num13 == 0)
                                            {
                                                buildPreview.coverObjId = num12;
                                                buildPreview.willRemoveCover = false;
                                                flag6 = false;
                                                break;
                                            }
                                        }
                                        if (buildPreview.input != null || buildPreview.output != null)
                                        {
                                            goto IL_E1A;
                                        }
                                        __instance.factory.ReadObjectConn(num12, 1, out flag7, out num13, out num14);
                                        if (num13 == 0)
                                        {
                                            buildPreview.coverObjId = num12;
                                            buildPreview.willRemoveCover = false;
                                            flag6 = false;
                                            break;
                                        }
                                        goto IL_E1A;
                                    }
                                    if (flag6)
                                    {
                                        buildPreview.condition = EBuildCondition.Collide;
                                        if (!buildPreview.desc.isBelt && !buildPreview.desc.isInserter)
                                        {
                                            __instance.AddErrorMessage(buildPreview.condition);
                                            break;
                                        }
                                        break;
                                    }
                                }
                                if (buildPreview.desc.veinMiner && Physics.CheckBox(colliderData.pos, colliderData.ext, colliderData.q, 2048, QueryTriggerInteraction.Collide))
                                {
                                    buildPreview.condition = EBuildCondition.Collide;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                    break;
                                }
                            }
                            if (buildPreview.condition != EBuildCondition.Ok)
                            {
                                goto IL_2278;
                            }
                        }
                        if (buildPreview.coverObjId == 0 || buildPreview.willRemoveCover)
                        {
                            int id = buildPreview.item.ID;
                            int num15 = 1;
                            if (__instance.tmpInhandId == id && __instance.tmpInhandCount > 0)
                            {
                                num15 = 1;
                                __instance.tmpInhandCount--;
                            }
                            else
                            {
                                __instance.tmpPackage.TakeTailItems(ref id, ref num15, false);
                            }
                            if (num15 == 0)
                            {
                                buildPreview.condition = EBuildCondition.NotEnoughItem;
                            }
                        }
                        if (buildPreview.coverObjId == 0)
                        {
                            if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
                            {
                                if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length != buildPreview.nearestPowerObjId.Length || buildPreview.nearestPowerObjId.Length < __instance.factory.powerSystem.netCursor)
                                {
                                    buildPreview.nearestPowerObjId = new int[__instance.factory.powerSystem.netCursor];
                                }
                                Array.Clear(buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
                                float num16 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
                                float x = lpos.x;
                                float y = lpos.y;
                                float z = lpos.z;
                                int netCursor = __instance.factory.powerSystem.netCursor;
                                PowerNetwork[] netPool = __instance.factory.powerSystem.netPool;
                                PowerNodeComponent[] nodePool = __instance.factory.powerSystem.nodePool;
                                PowerGeneratorComponent[] genPool = __instance.factory.powerSystem.genPool;
                                bool windForcedPower = buildPreview.desc.windForcedPower;
                                float num17;
                                for (int m = 1; m < netCursor; m++)
                                {
                                    if (netPool[m] != null && netPool[m].id != 0)
                                    {
                                        List<Node> nodes = netPool[m].nodes;
                                        int count = nodes.Count;
                                        num17 = 4900f;
                                        for (int n = 0; n < count; n++)
                                        {
                                            float num18 = x - nodes[n].x;
                                            float num19 = y - nodes[n].y;
                                            float num20 = z - nodes[n].z;
                                            float num21 = num18 * num18 + num19 * num19 + num20 * num20;
                                            if (num21 < num17 && (num21 < nodes[n].connDistance2 || num21 < num16))
                                            {
                                                buildPreview.nearestPowerObjId[m] = nodePool[nodes[n].id].entityId;
                                                num17 = num21;
                                            }
                                            if (windForcedPower && nodes[n].genId > 0 && genPool[nodes[n].genId].id == nodes[n].genId && genPool[nodes[n].genId].wind && num21 < 110.25f)
                                            {
                                                buildPreview.condition = EBuildCondition.WindTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                            else if (!buildPreview.desc.isPowerGen && nodes[n].genId == 0 && num21 < 12.25f)
                                            {
                                                buildPreview.condition = EBuildCondition.PowerTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                            else if (num21 < 12.25f)
                                            {
                                                buildPreview.condition = EBuildCondition.PowerTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                    }
                                }
                                PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
                                int prebuildCursor = __instance.factory.prebuildCursor;
                                num17 = 4900f;
                                for (int num22 = 1; num22 < prebuildCursor; num22++)
                                {
                                    if (prebuildPool[num22].id == num22 && prebuildPool[num22].protoId >= 2199 && prebuildPool[num22].protoId <= 2299)
                                    {
                                        float num23 = x - prebuildPool[num22].pos.x;
                                        float num19 = y - prebuildPool[num22].pos.y;
                                        float num20 = z - prebuildPool[num22].pos.z;
                                        float num21 = num23 * num23 + num19 * num19 + num20 * num20;
                                        if (num21 < num17)
                                        {
                                            ItemProto itemProto2 = LDB.items.Select((int)prebuildPool[num22].protoId);
                                            if (itemProto2 != null && itemProto2.prefabDesc.isPowerNode)
                                            {
                                                if (num21 < itemProto2.prefabDesc.powerConnectDistance * itemProto2.prefabDesc.powerConnectDistance || num21 < num16)
                                                {
                                                    buildPreview.nearestPowerObjId[0] = -num22;
                                                    num17 = num21;
                                                }
                                                if (windForcedPower && itemProto2.prefabDesc.windForcedPower && num21 < 110.25f)
                                                {
                                                    buildPreview.condition = EBuildCondition.WindTooClose;
                                                    __instance.AddErrorMessage(buildPreview.condition);
                                                }
                                                else if (!buildPreview.desc.isPowerGen && !itemProto2.prefabDesc.isPowerGen && num21 < 12.25f)
                                                {
                                                    buildPreview.condition = EBuildCondition.PowerTooClose;
                                                    __instance.AddErrorMessage(buildPreview.condition);
                                                }
                                                else if (num21 < 12.25f)
                                                {
                                                    buildPreview.condition = EBuildCondition.PowerTooClose;
                                                    __instance.AddErrorMessage(buildPreview.condition);
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int num24 = 0; num24 < __instance.bpCursor; num24++)
                                {
                                    if (num24 != i)
                                    {
                                        BuildPreview buildPreview2 = __instance.bpPool[num24];
                                        if (buildPreview2.item.ID >= 2199 && buildPreview2.item.ID <= 2299)
                                        {
                                            float num25 = x - buildPreview2.lpos.x;
                                            float num19 = y - buildPreview2.lpos.y;
                                            float num20 = z - buildPreview2.lpos.z;
                                            float num21 = num25 * num25 + num19 * num19 + num20 * num20;
                                            if (num21 < num17)
                                            {
                                                ItemProto item = buildPreview2.item;
                                                if (item != null && item.prefabDesc.isPowerNode)
                                                {
                                                    if (num21 < item.prefabDesc.powerConnectDistance * item.prefabDesc.powerConnectDistance || num21 < num16)
                                                    {
                                                        buildPreview.nearestPowerObjId[0] = -num24;
                                                        num17 = num21;
                                                    }
                                                    if (windForcedPower && item.prefabDesc.windForcedPower && num21 < 110.25f)
                                                    {
                                                        buildPreview.condition = EBuildCondition.WindTooClose;
                                                        __instance.AddErrorMessage(buildPreview.condition);
                                                    }
                                                    else if (!buildPreview.desc.isPowerGen && !item.prefabDesc.isPowerGen && num21 < 12.25f)
                                                    {
                                                        buildPreview.condition = EBuildCondition.PowerTooClose;
                                                        __instance.AddErrorMessage(buildPreview.condition);
                                                    }
                                                    else if (num21 < 12.25f)
                                                    {
                                                        buildPreview.condition = EBuildCondition.PowerTooClose;
                                                        __instance.AddErrorMessage(buildPreview.condition);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (buildPreview.desc.isCollectStation)
                            {
                                if (__instance.planet == null || __instance.planet.gasItems == null || __instance.planet.gasItems.Length == 0)
                                {
                                    buildPreview.condition = EBuildCondition.OutOfReach;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                    goto IL_2278;
                                }
                                for (int num26 = 0; num26 < __instance.planet.gasItems.Length; num26++)
                                {
                                    double num27 = 0.0;
                                    if ((double)buildPreview.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
                                    {
                                        num27 = 1.0 - (double)buildPreview.desc.workEnergyPerTick / ((double)buildPreview.desc.stationCollectSpeed * __instance.planet.gasTotalHeat * 0.016666666666666666);
                                    }
                                    if (num27 <= 0.0)
                                    {
                                        buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
                                    }
                                }
                                float y2 = __instance.cursorTarget.y;
                                if (y2 > 0.1f || y2 < -0.1f)
                                {
                                    buildPreview.condition = EBuildCondition.BuildInEquator;
                                    goto IL_2278;
                                }
                            }
                            if (buildPreview.desc.isStation)
                            {
                                StationComponent[] stationPool = __instance.factory.transport.stationPool;
                                int stationCursor = __instance.factory.transport.stationCursor;
                                PrebuildData[] prebuildPool2 = __instance.factory.prebuildPool;
                                int prebuildCursor2 = __instance.factory.prebuildCursor;
                                EntityData[] entityPool = __instance.factory.entityPool;
                                float num28 = 225f;
                                float num29 = 841f;
                                float num30 = 14297f;
                                num29 = (buildPreview.desc.isCollectStation ? num30 : num29);
                                for (int num31 = 1; num31 < stationCursor; num31++)
                                {
                                    if (stationPool[num31] != null && stationPool[num31].id == num31)
                                    {
                                        float num32 = (stationPool[num31].isStellar || buildPreview.desc.isStellarStation) ? num29 : num28;
                                        if ((entityPool[stationPool[num31].entityId].pos - lpos).sqrMagnitude < num32)
                                        {
                                            buildPreview.condition = EBuildCondition.TowerTooClose;
                                            __instance.AddErrorMessage(buildPreview.condition);
                                        }
                                    }
                                }
                                for (int num33 = 1; num33 < prebuildCursor2; num33++)
                                {
                                    if (prebuildPool2[num33].id == num33)
                                    {
                                        ItemProto itemProto3 = LDB.items.Select((int)prebuildPool2[num33].protoId);
                                        if (itemProto3 != null && itemProto3.prefabDesc.isStation)
                                        {
                                            float num34 = (itemProto3.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation) ? num29 : num28;
                                            float num35 = lpos.x - prebuildPool2[num33].pos.x;
                                            float num36 = lpos.y - prebuildPool2[num33].pos.y;
                                            float num37 = lpos.z - prebuildPool2[num33].pos.z;
                                            if (num35 * num35 + num36 * num36 + num37 * num37 < num34)
                                            {
                                                buildPreview.condition = EBuildCondition.TowerTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                    }
                                }
                                for (int num38 = 0; num38 < __instance.bpCursor; num38++)
                                {
                                    if (num38 != i)
                                    {
                                        BuildPreview buildPreview3 = __instance.bpPool[num38];
                                        if (buildPreview3.desc.isStation)
                                        {
                                            float num39 = (buildPreview3.desc.isStellarStation || buildPreview.desc.isStellarStation) ? num29 : num28;
                                            float num40 = lpos.x - buildPreview3.lpos.x;
                                            float num41 = lpos.y - buildPreview3.lpos.y;
                                            float num42 = lpos.z - buildPreview3.lpos.z;
                                            if (num40 * num40 + num41 * num41 + num42 * num42 < num39)
                                            {
                                                buildPreview.condition = EBuildCondition.TowerTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                    }
                                }
                            }
                            if (!buildPreview.desc.isInserter)
                            {
                                float num43 = lpos.magnitude - __instance.planet.realRadius + buildPreview.desc.cullingHeight;
                                Vector3 ext = buildPreview.desc.buildCollider.ext;
                                float num44 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
                                float num45 = 7.2f + num44;
                                if (num43 > 4.9f && !buildPreview.desc.isEjector)
                                {
                                    EjectorComponent[] ejectorPool = __instance.factory.factorySystem.ejectorPool;
                                    int ejectorCursor = __instance.factory.factorySystem.ejectorCursor;
                                    PrebuildData[] prebuildPool3 = __instance.factory.prebuildPool;
                                    int prebuildCursor3 = __instance.factory.prebuildCursor;
                                    EntityData[] entityPool2 = __instance.factory.entityPool;
                                    for (int num46 = 1; num46 < ejectorCursor; num46++)
                                    {
                                        if (ejectorPool[num46].id == num46 && (entityPool2[ejectorPool[num46].entityId].pos - lpos).sqrMagnitude < num45 * num45)
                                        {
                                            buildPreview.condition = EBuildCondition.EjectorTooClose;
                                            __instance.AddErrorMessage(buildPreview.condition);
                                        }
                                    }
                                    for (int num47 = 1; num47 < prebuildCursor3; num47++)
                                    {
                                        if (prebuildPool3[num47].id == num47)
                                        {
                                            ItemProto itemProto4 = LDB.items.Select((int)prebuildPool3[num47].protoId);
                                            if (itemProto4 != null && itemProto4.prefabDesc.isEjector)
                                            {
                                                float num48 = lpos.x - prebuildPool3[num47].pos.x;
                                                float num49 = lpos.y - prebuildPool3[num47].pos.y;
                                                float num50 = lpos.z - prebuildPool3[num47].pos.z;
                                                if (num48 * num48 + num49 * num49 + num50 * num50 < num45 * num45)
                                                {
                                                    buildPreview.condition = EBuildCondition.EjectorTooClose;
                                                    __instance.AddErrorMessage(buildPreview.condition);
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int num51 = 0; num51 < __instance.bpCursor; num51++)
                                {
                                    if (num51 != i && buildPreview.desc.isEjector)
                                    {
                                        BuildPreview buildPreview4 = __instance.bpPool[num51];
                                        if (buildPreview4.desc.isEjector)
                                        {
                                            float num52 = lpos.x - buildPreview4.lpos.x;
                                            float num53 = lpos.y - buildPreview4.lpos.y;
                                            float num54 = lpos.z - buildPreview4.lpos.z;
                                            if (num52 * num52 + num53 * num53 + num54 * num54 < num45 * num45)
                                            {
                                                buildPreview.condition = EBuildCondition.EjectorTooClose;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                    }
                                }
                            }
                            if (buildPreview.desc.isEjector)
                            {
                                __instance.GetOverlappedObjectsNonAlloc(lpos, 12f, 14.5f, false);
                                for (int num55 = 0; num55 < BuildTool._overlappedCount; num55++)
                                {
                                    PrefabDesc prefabDesc2 = __instance.GetPrefabDesc(BuildTool._overlappedIds[num55]);
                                    Vector3 position = __instance.GetObjectPose(BuildTool._overlappedIds[num55]).position;
                                    if (position.magnitude - __instance.planet.realRadius + prefabDesc2.cullingHeight > 4.9f)
                                    {
                                        float num56 = lpos.x - position.x;
                                        float num57 = lpos.y - position.y;
                                        float num58 = lpos.z - position.z;
                                        float num59 = num56 * num56 + num57 * num57 + num58 * num58;
                                        Vector3 ext2 = prefabDesc2.buildCollider.ext;
                                        float num60 = Mathf.Sqrt(ext2.x * ext2.x + ext2.z * ext2.z);
                                        float num61 = 7.2f + num60;
                                        if (prefabDesc2.isEjector)
                                        {
                                            num61 = 10.6f;
                                        }
                                        if (num59 < num61 * num61)
                                        {
                                            buildPreview.condition = EBuildCondition.BlockTooClose;
                                            __instance.AddErrorMessage(buildPreview.condition);
                                        }
                                    }
                                }
                            }
                            if ((!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0) && !buildPreview.desc.isInserter)
                            {
                                for (int num62 = 0; num62 < buildPreview.desc.landPoints.Length; num62++)
                                {
                                    Vector3 point = buildPreview.desc.landPoints[num62];
                                    point.y = 0f;
                                    Vector3 vector7 = lpos + lrot * point;
                                    Vector3 normalized = vector7.normalized;
                                    vector7 += normalized * 3f;
                                    Vector3 direction = -normalized;
                                    if (flag2)
                                    {
                                        Vector3 b = __instance.cursorTarget.normalized * __instance.planet.realRadius * 0.025f;
                                        vector7 -= b;
                                    }
                                    RaycastHit raycastHit;
                                    if (Physics.Raycast(new Ray(vector7, direction), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
                                    {
                                        float distance = raycastHit.distance;
                                        if (raycastHit.point.magnitude - __instance.factory.planet.realRadius < -0.3f)
                                        {
                                            buildPreview.condition = EBuildCondition.NeedGround;
                                            __instance.AddErrorMessage(buildPreview.condition);
                                        }
                                        else
                                        {
                                            float num63;
                                            if (Physics.Raycast(new Ray(vector7, direction), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
                                            {
                                                num63 = raycastHit.distance;
                                            }
                                            else
                                            {
                                                num63 = 1000f;
                                            }
                                            if (distance - num63 > 0.27f)
                                            {
                                                buildPreview.condition = EBuildCondition.NeedGround;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                    }
                                    else if (buildPreview.input == null)
                                    {
                                        buildPreview.condition = EBuildCondition.NeedGround;
                                        __instance.AddErrorMessage(buildPreview.condition);
                                    }
                                }
                                for (int num64 = 0; num64 < buildPreview.desc.waterPoints.Length; num64++)
                                {
                                    if (__instance.factory.planet.waterItemId <= 0)
                                    {
                                        buildPreview.condition = EBuildCondition.NeedWater;
                                        __instance.AddErrorMessage(buildPreview.condition);
                                    }
                                    else
                                    {
                                        Vector3 point2 = buildPreview.desc.waterPoints[num64];
                                        point2.y = 0f;
                                        Vector3 vector8 = lpos + lrot * point2;
                                        Vector3 normalized2 = vector8.normalized;
                                        vector8 += normalized2 * 3f;
                                        Vector3 direction2 = -normalized2;
                                        RaycastHit raycastHit;
                                        float num65;
                                        if (Physics.Raycast(new Ray(vector8, direction2), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
                                        {
                                            num65 = raycastHit.distance;
                                        }
                                        else
                                        {
                                            num65 = 1000f;
                                        }
                                        if (Physics.Raycast(new Ray(vector8, direction2), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
                                        {
                                            float distance2 = raycastHit.distance;
                                            if (num65 - distance2 <= 0.27f)
                                            {
                                                buildPreview.condition = EBuildCondition.NeedWater;
                                                __instance.AddErrorMessage(buildPreview.condition);
                                            }
                                        }
                                        else
                                        {
                                            buildPreview.condition = EBuildCondition.NeedWater;
                                            __instance.AddErrorMessage(buildPreview.condition);
                                        }
                                    }
                                }
                            }
                            if (buildPreview.desc.isInserter && buildPreview.condition == EBuildCondition.Ok)
                            {
                                bool flag8 = __instance.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt);
                                bool flag9 = __instance.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt);
                                Vector3 posR = Vector3.zero;
                                Vector3 vector9;
                                if (buildPreview.output != null)
                                {
                                    vector9 = buildPreview.output.lpos;
                                }
                                else
                                {
                                    vector9 = __instance.GetObjectPose(buildPreview.outputObjId).position;
                                }
                                Vector3 vector10;
                                if (buildPreview.input != null)
                                {
                                    vector10 = buildPreview.input.lpos;
                                }
                                else
                                {
                                    vector10 = __instance.GetObjectPose(buildPreview.inputObjId).position;
                                }
                                if (flag8 && !flag9)
                                {
                                    posR = vector9;
                                }
                                else if (!flag8 && flag9)
                                {
                                    posR = vector10;
                                }
                                else
                                {
                                    posR = (vector9 + vector10) * 0.5f;
                                }
                                float num66 = __instance.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(posR, buildPreview.lpos, buildPreview.lpos2);
                                float magnitude = forward3.magnitude;
                                float num67 = 5.5f;
                                float num68 = 0.6f;
                                if (flag8 && flag9)
                                {
                                    num68 = 0.4f;
                                    num67 = 5f;
                                }
                                else if (!flag8 && !flag9)
                                {
                                    num68 = 0.9f;
                                    num67 = 7.5f;
                                    num66 -= 0.3f;
                                }
                                if (magnitude > num67)
                                {
                                    buildPreview.condition = EBuildCondition.TooFar;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                }
                                else if (magnitude < num68)
                                {
                                    buildPreview.condition = EBuildCondition.TooClose;
                                    __instance.AddErrorMessage(buildPreview.condition);
                                }
                                else
                                {
                                    int oneParameter = Mathf.RoundToInt(Mathf.Clamp(num66, 1f, 3f));
                                    buildPreview.SetOneParameter(oneParameter);
                                }
                            }
                        }
                    }
                }
                IL_2278:;
            }
            for (int num69 = 0; num69 < __instance.bpCursor; num69++)
            {
                BuildPreview buildPreview5 = __instance.bpPool[num69];
                if (buildPreview5.desc.isInserter && buildPreview5.condition == EBuildCondition.Collide && ((buildPreview5.input != null && buildPreview5.input.coverObjId != 0) || (buildPreview5.output != null && buildPreview5.output.coverObjId != 0)))
                {
                    Vector3 lpos3 = buildPreview5.lpos;
                    Quaternion lrot3 = buildPreview5.lrot;
                    Vector3 lpos4 = buildPreview5.lpos2;
                    Quaternion lrot4 = buildPreview5.lrot2;
                    Pose pose3 = new Pose(buildPreview5.lpos, buildPreview5.lrot);
                    Pose pose4 = new Pose(buildPreview5.lpos2, buildPreview5.lrot2);
                    Vector3 forward4 = pose3.forward;
                    Vector3 forward5 = pose4.forward;
                    Vector3 up2 = pose3.up;
                    Vector3 a2 = Vector3.Lerp(lpos3, lpos4, 0.5f);
                    Vector3 forward6 = lpos4 - lpos3;
                    if (forward6.sqrMagnitude < 0.0001f)
                    {
                        forward6 = Maths.SphericalRotation(lpos3, 0f).Forward();
                    }
                    Quaternion quaternion2 = Quaternion.LookRotation(forward6, a2.normalized);
                    ColliderData[] buildColliders2 = buildPreview5.desc.buildColliders;
                    bool flag10 = false;
                    for (int num70 = 0; num70 < buildColliders2.Length; num70++)
                    {
                        ColliderData colliderData4 = buildPreview5.desc.buildColliders[num70];
                        colliderData4.ext = new Vector3(colliderData4.ext.x, colliderData4.ext.y, Vector3.Distance(lpos4, lpos3) * 0.5f + colliderData4.ext.z - 0.5f);
                        if (buildPreview5.desc.isInserter)
                        {
                            if (__instance.ObjectIsBelt(buildPreview5.inputObjId) || (buildPreview5.input != null && buildPreview5.input.desc.isBelt))
                            {
                                colliderData4.pos.z = colliderData4.pos.z - 0.35f;
                                colliderData4.ext.z = colliderData4.ext.z + 0.35f;
                            }
                            else if (buildPreview5.inputObjId == 0 && buildPreview5.input == null)
                            {
                                colliderData4.pos.z = colliderData4.pos.z - 0.35f;
                                colliderData4.ext.z = colliderData4.ext.z + 0.35f;
                            }
                            if (__instance.ObjectIsBelt(buildPreview5.outputObjId) || (buildPreview5.output != null && buildPreview5.output.desc.isBelt))
                            {
                                colliderData4.pos.z = colliderData4.pos.z + 0.35f;
                                colliderData4.ext.z = colliderData4.ext.z + 0.35f;
                            }
                            else if (buildPreview5.outputObjId == 0 && buildPreview5.output == null)
                            {
                                colliderData4.pos.z = colliderData4.pos.z + 0.35f;
                                colliderData4.ext.z = colliderData4.ext.z + 0.35f;
                            }
                            if (colliderData4.ext.z < 0.1f)
                            {
                                colliderData4.ext.z = 0.1f;
                            }
                        }
                        else
                        {
                            colliderData4.pos = lpos3 + lrot3 * colliderData4.pos;
                            colliderData4.q = lrot3 * colliderData4.q;
                        }
                        colliderData4.pos = a2 + quaternion2 * colliderData4.pos;
                        colliderData4.q = quaternion2 * colliderData4.q;
                        colliderData4.DebugDraw();
                        int mask2 = 428032;
                        if (buildPreview5.desc.veinMiner || buildPreview5.desc.oilMiner)
                        {
                            mask2 = 425984;
                        }
                        Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                        int num71 = Physics.OverlapBoxNonAlloc(colliderData4.pos, colliderData4.ext, BuildTool._tmp_cols, colliderData4.q, mask2, QueryTriggerInteraction.Collide);
                        if (num71 > 0)
                        {
                            PlanetPhysics physics2 = __instance.player.planetData.physics;
                            int num72 = 0;
                            while (num72 < num71 && buildPreview5.coverObjId == 0)
                            {
                                ColliderData colliderData6;
                                bool colliderData5 = physics2.GetColliderData(BuildTool._tmp_cols[num72], out colliderData6);
                                int num73 = 0;
                                if (colliderData5 && colliderData6.isForBuild)
                                {
                                    if (colliderData6.objType == EObjectType.Entity)
                                    {
                                        num73 = colliderData6.objId;
                                    }
                                    else if (colliderData6.objType == EObjectType.Prebuild)
                                    {
                                        num73 = -colliderData6.objId;
                                    }
                                }
                                Collider collider2 = BuildTool._tmp_cols[num72];
                                if (collider2.gameObject.layer == 18)
                                {
                                    BuildPreviewModel component2 = collider2.GetComponent<BuildPreviewModel>();
                                    if ((!(component2 != null) || component2.index != buildPreview5.previewIndex) && (!buildPreview5.desc.isInserter || component2.buildPreview.desc.isInserter))
                                    {
                                        if (buildPreview5.desc.isInserter || !component2.buildPreview.desc.isInserter)
                                        {
                                            goto IL_27CA;
                                        }
                                    }
                                }
                                else if (!buildPreview5.desc.isInserter || num73 == 0 || (num73 != buildPreview5.inputObjId && num73 != buildPreview5.outputObjId && (buildPreview5.input == null || buildPreview5.input.objId != num73) && (buildPreview5.output == null || buildPreview5.output.objId != num73) && (buildPreview5.input == null || buildPreview5.input.coverObjId != num73) && (buildPreview5.output == null || buildPreview5.output.coverObjId != num73)))
                                {
                                    goto IL_27CA;
                                }
                                IL_27CD:
                                num72++;
                                continue;
                                IL_27CA:
                                flag10 = true;
                                goto IL_27CD;
                            }
                        }
                    }
                    if (!flag10)
                    {
                        buildPreview5.condition = EBuildCondition.Ok;
                        bool flag11 = buildPreview5.input == null;
                        bool flag12 = buildPreview5.output == null;
                        if (flag11)
                        {
                            buildPreview5.inputObjId = 0;
                            __instance.MatchInserter(buildPreview5);
                        }
                        if (flag12)
                        {
                            buildPreview5.outputObjId = 0;
                            __instance.MatchInserter(buildPreview5);
                        }
                    }
                    __instance.AddErrorMessage(buildPreview5.condition);
                }
            }
            for (int num74 = 0; num74 < __instance.bpCursor; num74++)
            {
                BuildPreview buildPreview6 = __instance.bpPool[num74];
                if (buildPreview6.desc.isBelt)
                {
                    int num75 = -1;
                    BuildPreview buildPreview7 = null;
                    BuildPreview buildPreview8 = null;
                    BuildPreview buildPreview9 = null;
                    if (buildPreview6.input != null && !buildPreview6.input.desc.isBelt && buildPreview6.input.previewIndex >= 0)
                    {
                        num75 = buildPreview6.input.previewIndex;
                        buildPreview7 = buildPreview6.input;
                        buildPreview8 = buildPreview6;
                        buildPreview9 = buildPreview6.output;
                    }
                    if (buildPreview6.output != null && buildPreview6.output.desc.isBelt && buildPreview6.output.output != null && !buildPreview6.output.output.desc.isBelt && buildPreview6.output.output.previewIndex >= 0)
                    {
                        num75 = buildPreview6.output.output.previewIndex;
                        buildPreview7 = buildPreview6.output.output;
                        buildPreview8 = buildPreview6;
                        buildPreview9 = buildPreview6.output;
                    }
                    for (int num76 = 0; num76 < 2; num76++)
                    {
                        BuildPreview buildPreview10 = (num76 == 0) ? buildPreview8 : buildPreview9;
                        if (buildPreview10 != null && buildPreview10.desc.isBelt)
                        {
                            Vector3 lpos5 = buildPreview10.lpos;
                            Quaternion lrot5 = buildPreview10.lrot;
                            Vector3 lpos6 = buildPreview10.lpos2;
                            Quaternion lrot6 = buildPreview10.lrot2;
                            Pose pose5 = new Pose(buildPreview10.lpos, buildPreview10.lrot);
                            Pose pose6 = new Pose(buildPreview10.lpos2, buildPreview10.lrot2);
                            Vector3 forward7 = pose5.forward;
                            Vector3 forward8 = pose6.forward;
                            Vector3 up3 = pose5.up;
                            Vector3 a3 = Vector3.Lerp(lpos5, lpos6, 0.5f);
                            Vector3 forward9 = lpos6 - lpos5;
                            if (forward9.sqrMagnitude < 0.0001f)
                            {
                                forward9 = Maths.SphericalRotation(lpos5, 0f).Forward();
                            }
                            Quaternion quaternion3 = Quaternion.LookRotation(forward9, a3.normalized);
                            ColliderData[] buildColliders3 = buildPreview10.desc.buildColliders;
                            bool flag13 = false;
                            for (int num77 = 0; num77 < buildColliders3.Length; num77++)
                            {
                                ColliderData colliderData7 = buildPreview10.desc.buildColliders[num77];
                                colliderData7.ext = new Vector3(colliderData7.ext.x, colliderData7.ext.y, Vector3.Distance(lpos6, lpos5) * 0.5f + colliderData7.ext.z - 0.5f);
                                colliderData7.pos = lpos5 + lrot5 * colliderData7.pos;
                                colliderData7.q = lrot5 * colliderData7.q;
                                colliderData7.pos = a3 + quaternion3 * colliderData7.pos;
                                colliderData7.q = quaternion3 * colliderData7.q;
                                colliderData7.DebugDraw();
                                int mask3 = 428032;
                                Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                                int num78 = Physics.OverlapBoxNonAlloc(colliderData7.pos, colliderData7.ext, BuildTool._tmp_cols, colliderData7.q, mask3, QueryTriggerInteraction.Collide);
                                if (num78 > 0)
                                {
                                    PlanetPhysics physics3 = __instance.player.planetData.physics;
                                    int num79 = 0;
                                    while (num79 < num78)
                                    {
                                        ColliderData colliderData8;
                                        if (physics3.GetColliderData(BuildTool._tmp_cols[num79], out colliderData8) && colliderData8.isForBuild)
                                        {
                                            if (colliderData8.objType == EObjectType.Entity)
                                            {
                                                int objId = colliderData8.objId;
                                            }
                                            else if (colliderData8.objType == EObjectType.Prebuild)
                                            {
                                                int objId2 = colliderData8.objId;
                                            }
                                        }
                                        Collider collider3 = BuildTool._tmp_cols[num79];
                                        if (collider3.gameObject.layer != 18)
                                        {
                                            goto IL_2C61;
                                        }
                                        BuildPreviewModel component3 = collider3.GetComponent<BuildPreviewModel>();
                                        if ((!(component3 != null) || component3.index != buildPreview10.previewIndex) && (!buildPreview10.desc.isInserter || component3.buildPreview.desc.isInserter) && (buildPreview10.desc.isInserter || !component3.buildPreview.desc.isInserter) && (!(component3 != null) || component3.buildPreview.desc.isBelt || component3.buildPreview.previewIndex != num75))
                                        {
                                            goto IL_2C61;
                                        }
                                        IL_2C64:
                                        num79++;
                                        continue;
                                        IL_2C61:
                                        flag13 = true;
                                        goto IL_2C64;
                                    }
                                }
                            }
                            if (!flag13)
                            {
                                buildPreview10.condition = buildPreview7.condition;
                            }
                            __instance.AddErrorMessage(buildPreview10.condition);
                        }
                    }
                }
            }
            bool flag14 = true;
            int num80 = 0;
            while (num80 < __instance.bpCursor)
            {
                BuildPreview buildPreview11 = __instance.bpPool[num80];
                if (buildPreview11.bpgpuiModelId > 0 && buildPreview11.condition != EBuildCondition.Ok && buildPreview11.condition != EBuildCondition.NeedConn && buildPreview11.condition != EBuildCondition.NotEnoughItem)
                {
                    flag14 = false;
                    __instance.actionBuild.model.cursorState = -1;
                    if (buildPreview11.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
                    {
                        __instance.AddErrorMessage(EBuildCondition.OutOfVerticalConstructionHeight);
                        break;
                    }
                    break;
                }
                else
                {
                    num80++;
                }
            }
            if (flag14)
            {
                __instance.actionBuild.model.cursorState = 0;
            }
            if (!flag14 && !VFInput.onGUI)
            {
                UICursor.SetCursor(ECursor.Ban);
            }
            __result = flag14;
            return false;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(BuildTool_BlueprintCopy), "DeterminePreSelectGratBox")]
        public static bool DeterminePreSelectGratBox(ref BuildTool_BlueprintCopy __instance)
        {
            if (__instance.cursorValid)
            {
                float longitudeRad = BlueprintUtils.GetLongitudeRad(__instance.castGroundPosSnapped.normalized);
                float longitudeRad2 = BlueprintUtils.GetLongitudeRad(__instance.lastGroundPosSnapped.normalized);
                float latitudeRad = BlueprintUtils.GetLatitudeRad(__instance.castGroundPosSnapped.normalized);
                bool flag = latitudeRad >= 1.5707864f || latitudeRad <= -1.5707864f;
                float num = flag ? 0f : (longitudeRad - longitudeRad2);
                num = Mathf.Repeat(num + 3.1415927f, 6.2831855f) - 3.1415927f;
                __instance.preSelectArcBox.endLongitudeRad = __instance.preSelectArcBox.endLongitudeRad + num;
                __instance.preSelectArcBox.endLatitudeRad = latitudeRad;
                __instance.preSelectGratBox = __instance.preSelectArcBox;
                __instance.preSelectGratBox.x = ((__instance.preSelectArcBox.x < __instance.preSelectArcBox.z) ? __instance.preSelectArcBox.x : __instance.preSelectArcBox.z);
                __instance.preSelectGratBox.z = ((__instance.preSelectArcBox.x > __instance.preSelectArcBox.z) ? __instance.preSelectArcBox.x : __instance.preSelectArcBox.z);
                if (__instance.preSelectArcBox.x < __instance.preSelectArcBox.z)
                {
                    if (__instance.preSelectGratBox.z > __instance.preSelectGratBox.x + 6.2831855f - 1E-05f - 4E-06f)
                    {
                        __instance.preSelectGratBox.z = __instance.preSelectGratBox.x + 6.2831855f - 1E-05f - 4E-06f;
                    }
                    __instance.preSelectGratBox.z = Mathf.Repeat(__instance.preSelectGratBox.z + 3.1415927f, 6.2831855f) - 3.1415927f;
                }
                else
                {
                    if (__instance.preSelectGratBox.x < __instance.preSelectGratBox.z - 6.2831855f + 1E-05f + 4E-06f)
                    {
                        __instance.preSelectGratBox.x = __instance.preSelectGratBox.z - 6.2831855f + 1E-05f + 4E-06f;
                    }
                    __instance.preSelectGratBox.x = Mathf.Repeat(__instance.preSelectGratBox.x + 3.1415927f, 6.2831855f) - 3.1415927f;
                }
                __instance.preSelectGratBox.y = ((__instance.preSelectArcBox.y < __instance.preSelectArcBox.w) ? __instance.preSelectArcBox.y : __instance.preSelectArcBox.w);
                __instance.preSelectGratBox.w = ((__instance.preSelectArcBox.y > __instance.preSelectArcBox.w) ? __instance.preSelectArcBox.y : __instance.preSelectArcBox.w);
                float longitude = BlueprintUtils.GetLongitudeRadPerGrid((Mathf.Abs(__instance.castGroundPosSnapped.y) < Mathf.Abs(__instance.startGroundPosSnapped.y)) ? __instance.castGroundPosSnapped.normalized : __instance.startGroundPosSnapped.normalized, (int)__instance.planet.realRadius) * 0.33f;
                __instance.preSelectGratBox.Extend(longitude, 0.002f);
                if (!flag)
                {
                    __instance.lastGroundPosSnapped = __instance.castGroundPosSnapped;
                }
                
            }
            return false;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(BuildTool_BlueprintCopy), "get_segment")]
        public static bool get_segment(ref BuildTool_BlueprintCopy __instance, ref int __result)
        {
            if (__instance.planet.aux.activeGrid != null)
            {
                __result =  __instance.planet.aux.activeGrid.segment;
                return false;
            }
            GS2.Warn($"ActiveGrid not found for planet {__instance.planet.name}");
            __result =  200;
            return false;

        }
        [HarmonyPrefix, HarmonyPatch(typeof(BuildTool_BlueprintPaste), "get_segment")]
        public static bool get_segment(ref BuildTool_BlueprintPaste __instance, ref int __result)
        {
            if (__instance.planet.aux.activeGrid != null)
            {
                __result =  __instance.planet.aux.activeGrid.segment;
                return false;

            }
            GS2.Warn($"ActiveGrid not found for planet {__instance.planet.name}");
            __result =  200;
            return false;

        }
        // [HarmonyPrefix, HarmonyPatch(typeof(PlanetGrid), "CalcLocalGridSize")]
        // public static bool CalcLocalGridSize(Vector3 posR, Vector3 dir, ref float __result, ref PlanetGrid __instance)
        // {
        //  float f = Vector3.Dot(Vector3.Cross(posR, Vector3.up).normalized, dir);
        //  float magnitude = posR.magnitude;
        //  posR.Normalize();
        //  if ((double)Mathf.Abs(f) < 0.7)
        //  {
        //   __result = magnitude * 3.1415927f * 2f / (float)(__instance.segment * 5);
        //   // Log("1 "+__result);
        //   return false;
        //  }
        //  float num = Mathf.Asin(posR.y);
        //  float f2 = num / 6.2831855f * (float)__instance.segment;
        //  float num2 = (float)PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f2))), __instance.segment);
        //  
        //  // Warn($"num:{num} f2:{f2} seg:{__instance.segment} magnitude:{magnitude} num2:{num2} calc:{Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f)}");
        //  float num3 = Mathf.Max(0.0031415927f, Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f));
        //  __result = (magnitude * num3);
        //  if (GameMain.localPlanet.radius > 480) __result -= 0.3f;
        //  // Log("2 "+__result);
        //  return false;
        // } 
    }
}