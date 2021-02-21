using System;
using System.Collections.Generic;
using HarmonyLib;
using NGPT;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlayerAction_Build))]
    public class PatchOnPlayerAction_Build {
        
        /*
        [HarmonyPrefix]
        [HarmonyPatch("PrepareBuild")]
        public static bool PrepareBuild(ref PlayerAction_Build __instance) {
            __instance.prepareCursorText = string.Empty;
            CommandState cmd = __instance.controller.cmd;
            bool destructing = __instance.destructing;
            bool upgrading = __instance.upgrading;
            __instance.destructing = cmd.mode == -1;
            __instance.upgrading = cmd.mode == -2;
            if (!destructing && !upgrading && (__instance.destructing || __instance.upgrading))
                __instance.handItemBeforeExtraMode = __instance.handItem == null ? 0 : __instance.handItem.ID;
            else if ((destructing || upgrading) && (!__instance.destructing && !__instance.upgrading) &&
                     (__instance.handItemBeforeExtraMode > 0 && __instance.player.inhandItemId == 0)) {
                __instance.player.SetHandItems(__instance.handItemBeforeExtraMode, 0);
                cmd.refId = __instance.handItemBeforeExtraMode;
            }

            if (!__instance.destructing && !__instance.upgrading)
                __instance.handItemBeforeExtraMode = 0;
            __instance.nearcdLogic = __instance.player.planetData.physics.nearColliderLogic;
            __instance.planetPhysics = __instance.player.planetData.physics;
            __instance.factory = __instance.player.factory;
            __instance.factoryModel = __instance.factory.planet.factoryModel;
            __instance.connRenderer = __instance.factoryModel.connGizmoRenderer;
            __instance.connGraph = __instance.connRenderer.cursorGizmoGraph;
            __instance.planetAux = __instance.player.planetData.aux;
            if (__instance.tmpPackage.size != __instance.player.package.size)
                __instance.tmpPackage.SetSize(__instance.player.package.size);
            Array.Copy((Array) __instance.player.package.grids, (Array) __instance.tmpPackage.grids,
                __instance.tmpPackage.size);
            __instance.tmpInhandId = __instance.player.inhandItemId;
            __instance.tmpInhandCount = __instance.player.inhandItemCount;
            __instance.currMouseRay = __instance.controller.mainCamera.ScreenPointToRay(Input.mousePosition);
            __instance.handItem = LDB.items.Select(cmd.refId);
            if (__instance.handItem != null && __instance.handItem.IsEntity) {
                int modelIndex = __instance.handItem.ModelIndex;
                int num = __instance.handItem.ModelCount;
                if (num < 1)
                    num = 1;
                ModelProto modelProto = LDB.models.Select(modelIndex + __instance.modelOffset % num);
                __instance.handPrefabDesc = modelProto == null ? __instance.handItem.prefabDesc : modelProto.prefabDesc;
                if (!__instance.handPrefabDesc.hasObject)
                    __instance.handPrefabDesc = (PrefabDesc) null;
            }
            else
                __instance.handPrefabDesc = (PrefabDesc) null;

            __instance.castGround = false;
            __instance.castTerrain = false;
            __instance.castGrid = false;
            __instance.castPlatform = false;
            __instance.groundTestPos = Vector3.zero;
            __instance.groundSnappedPos = Vector3.zero;
            __instance.castObject = false;
            __instance.castObjId = 0;
            __instance.castObjPos = Vector3.zero;
            __instance.castObjectUsed = false;
            __instance.cursorTarget = Vector3.zero;
            __instance.cursorValid = false;
            __instance.controller.cmd.test = __instance.controller.cmd.target = Vector3.zero;
            Array.Clear((Array) __instance.reformIndices, 0, __instance.reformIndices.Length);
            if (!VFInput.onGUI && VFInput.inScreen) {
                __instance.showingAltitude = cmd.mode == 1 || cmd.mode == 3 ? 0 : __instance.altitude;
                int layerMask = __instance.showingAltitude != 0 ? 24576 : 8720;
                RaycastHit hitInfo;
                __instance.castGround = Physics.Raycast(__instance.currMouseRay, out hitInfo, 400f, layerMask,
                    QueryTriggerInteraction.Collide);
                if (!__instance.castGround)
                    __instance.castGround =
                        Physics.Raycast(
                            new Ray(__instance.currMouseRay.GetPoint(200f), -__instance.currMouseRay.direction),
                            out hitInfo, 200f, layerMask, QueryTriggerInteraction.Collide);
                if (__instance.castGround) {
                    Layer layer = (Layer) hitInfo.collider.gameObject.layer;
                    __instance.castTerrain = layer == Layer.Terrain || layer == Layer.Water;
                    __instance.castGrid = layer == Layer.BuildGrid;
                    __instance.castPlatform = layer == Layer.Platform;
                    __instance.groundTestPos = __instance.controller.cmd.test =
                        __instance.controller.cmd.target = hitInfo.point;
                    if (cmd.mode == 4) {
                        if (__instance.factory.platformSystem.reformData == null)
                            __instance.factory.platformSystem.InitReformData();
                        __instance.reformPointsCount = __instance.planetAux.ReformSnap(__instance.groundTestPos,
                            __instance.reformSize,
                            __instance.reformType, __instance.reformColor, __instance.reformPoints,
                            __instance.reformIndices,
                            __instance.factory.platformSystem, out __instance.reformCenterPoint);
                    }

                    __instance.groundSnappedPos =
                        __instance.handPrefabDesc == null || !VFInput._ignoreGrid ||
                        __instance.handPrefabDesc.minerType != EMinerType.Vein
                            ? __instance.planetAux.Snap(__instance.groundTestPos, __instance.castTerrain,
                                __instance.castTerrain && __instance.factory.planet.levelized)
                            : __instance.groundTestPos.normalized * (__instance.factory.planet.realRadius );
                    __instance.controller.cmd.test = __instance.groundSnappedPos;
                    Vector3 normalized = __instance.groundSnappedPos.normalized;
                    if (Physics.Raycast(new Ray(__instance.groundSnappedPos + normalized * 5f, -normalized),
                        out hitInfo, 10f,
                        8720, QueryTriggerInteraction.Collide))
                        __instance.controller.cmd.test = hitInfo.point;
                    if ((double) __instance.groundTestPos.sqrMagnitude > 1.0) {
                        if ((double) __instance.groundTestPosLast.sqrMagnitude < 1.0)
                            __instance.groundTestPosLast = __instance.groundTestPos;
                        Vector3 vector3 = __instance.groundTestPos - __instance.groundTestPosLast;
                        if ((double) vector3.sqrMagnitude > 0.100000001490116) {
                            __instance.groundTestMovement = vector3;
                            __instance.groundTestPosLast = __instance.groundTestPos;
                        }
                    }
                    else {
                        __instance.groundTestPosLast = __instance.groundTestPos;
                        __instance.groundTestMovement = Vector3.zero;
                    }

                    __instance.cursorTarget = __instance.groundSnappedPos;
                    __instance.cursorValid = true;
                }

                if (cmd.mode != 2 || !VFInput._notSnapBuild) {
                    int castAllCount = cmd.raycast.castAllCount;
                    RaycastData[] castAll = cmd.raycast.castAll;
                    int objId1 = 0;
                    int objId2 = 0;
                    int objId3 = 0;
                    float num1 = float.MaxValue;
                    float num2 = float.MaxValue;
                    float num3 = float.MaxValue;
                    for (int index = 0; index < castAllCount; ++index) {
                        if (castAll[index].objType == EObjectType.Entity ||
                            castAll[index].objType == EObjectType.Prebuild) {
                            int objId4 = castAll[index].objType != EObjectType.Entity
                                ? -castAll[index].objId
                                : castAll[index].objId;
                            if (__instance.ObjectIsInserter(objId4)) {
                                objId1 = objId4;
                                num1 = castAll[index].rch.dist;
                                break;
                            }
                        }
                    }

                    for (int index = 0; index < castAllCount; ++index) {
                        if (castAll[index].objType == EObjectType.Entity ||
                            castAll[index].objType == EObjectType.Prebuild) {
                            int objId4 = castAll[index].objType != EObjectType.Entity
                                ? -castAll[index].objId
                                : castAll[index].objId;
                            if (__instance.ObjectIsBelt(objId4)) {
                                objId2 = objId4;
                                num2 = castAll[index].rch.dist;
                                break;
                            }
                        }
                    }

                    for (int index = 0; index < castAllCount; ++index) {
                        if (castAll[index].objType == EObjectType.Entity ||
                            castAll[index].objType == EObjectType.Prebuild) {
                            objId3 = castAll[index].objType != EObjectType.Entity
                                ? -castAll[index].objId
                                : castAll[index].objId;
                            num3 = castAll[index].rch.dist;
                            break;
                        }
                    }

                    if (cmd.mode <= 0) {
                        if (objId1 > 0)
                            num1 -= 2f;
                        if (objId2 > 0)
                            num2 -= 2f;
                    }

                    if (objId1 != 0 && (double) num1 < (double) num2 && (double) num1 < (double) num3) {
                        __instance.castObject = true;
                        __instance.castObjId = objId1;
                        __instance.castObjPos = __instance.GetObjectPose(objId1).position;
                    }
                    else if (objId2 != 0 && (double) num2 < (double) num1 && (double) num2 < (double) num3) {
                        __instance.castObject = true;
                        __instance.castObjId = objId2;
                        __instance.castObjPos = __instance.GetObjectPose(objId2).position;
                    }
                    else if (objId3 != 0) {
                        __instance.castObject = true;
                        __instance.castObjId = objId3;
                        __instance.castObjPos = __instance.GetObjectPose(objId3).position;
                    }
                }

                if (!__instance.upgrading && !__instance.destructing && !__instance.castObject &&
                    (cmd.mode == 2 && !VFInput._notSnapBuild || cmd.mode == 3)) {
                    __instance.GetOverlappedObjectsNonAlloc(__instance.cursorTarget, 0.3f, 3f);
                    if (__instance._overlappedCount > 0) {
                        __instance.castObject = true;
                        __instance.castObjId = __instance._overlappedIds[0];
                        __instance.castObjPos = __instance.GetObjectPose(__instance.castObjId).position;
                    }
                }

                if (!__instance.upgrading && !__instance.destructing && (__instance.castObject && cmd.mode == 2)) {
                    PrefabDesc prefabDesc = __instance.GetPrefabDesc(__instance.castObjId);
                    if (prefabDesc != null && (prefabDesc.slotPoses == null || prefabDesc.slotPoses.Length == 0) &&
                        !prefabDesc.isBelt) {
                        __instance.castObject = false;
                        __instance.castObjId = 0;
                        __instance.castObjPos = Vector3.zero;
                    }
                }

                if (!__instance.upgrading && !__instance.destructing && (__instance.castObject && cmd.mode == 3)) {
                    PrefabDesc prefabDesc = __instance.GetPrefabDesc(__instance.castObjId);
                    if (prefabDesc != null && (prefabDesc.insertPoses == null || prefabDesc.insertPoses.Length == 0) &&
                        !prefabDesc.isBelt) {
                        __instance.castObject = false;
                        __instance.castObjId = 0;
                        __instance.castObjPos = Vector3.zero;
                    }
                }

                if (__instance.castObject) {
                    __instance.castObjectUsed = true;
                    if (cmd.mode == 1 && !__instance.handPrefabDesc.multiLevel)
                        __instance.castObjectUsed = false;
                    if (__instance.castObjectUsed) {
                        __instance.cursorTarget = __instance.castObjPos;
                        __instance.controller.cmd.test = __instance.castObjPos;
                        __instance.cursorValid = true;
                    }
                }

                if (cmd.mode == 3)
                    __instance.cursorValid = __instance.castObjId > 0;
            }

            __instance.controller.cmd.state = !__instance.cursorValid ? 0 : 1;
            if (__instance.cursorValid)
                __instance.controller.cmd.target = __instance.cursorTarget;
            if (__instance.handItem == null && !__instance.destructing && (!__instance.upgrading && cmd.mode == 0) &&
                (__instance.castObjId != 0 && !VFInput.onGUI && VFInput.inScreen)) {
                ItemProto itemProto = __instance.GetItemProto(__instance.castObjId);
                if (itemProto != null) {
                    __instance.prepareCursorText = itemProto.name + "复制建筑".Translate();
                    if (VFInput._copyBuilding.onDown) {
                        __instance.copyRecipeId = 0;
                        __instance.copyFilterId = 0;
                        if (__instance.player.inhandItemId > 0)
                            __instance.player.SetHandItems(0, 0);
                        __instance.player.SetHandItems(itemProto.ID, 0);
                        __instance.SetCopyInfo(itemProto.ID, __instance.castObjId);
                    }

                    if (__instance.castObjId > 0 &&
                        (VFInput._openBuildingPanel.onDown && !VFInput.copyBuildingAndBuilingPanelConflict))
                        __instance.controller.actionInspect.SetInspectee(EObjectType.Entity, __instance.castObjId);
                }
            }

            bool flag = cmd.refId > 0 || __instance.buildPreviews.Count > 0;
            if (!VFInput._godModeMechMove &&
                ((VFInput.rtsCancel.onDown || VFInput.escKey.onDown) && (!VFInput.onGUI && VFInput.inScreen) ||
                 VFInput._buildKey.onDown)) {
                VFInput.UseBuildKey();
                VFInput.UseEscape();
                if (__instance.destructing) {
                    if (__instance.handItemBeforeExtraMode > 0) {
                        __instance.controller.cmd.type = ECommand.Build;
                        __instance.controller.cmd.mode = 0;
                    }
                    else {
                        __instance.player.SetHandItems(0, 0);
                        __instance.controller.cmd.type = ECommand.None;
                        __instance.controller.cmd.mode = 0;
                        __instance.controller.cmd.stage = 0;
                        __instance.controller.cmd.state = 0;
                        __instance.controller.cmd.refId = 0;
                    }
                }
                else if (__instance.upgrading) {
                    if (__instance.handItemBeforeExtraMode > 0) {
                        __instance.controller.cmd.type = ECommand.Build;
                        __instance.controller.cmd.mode = 0;
                    }
                    else {
                        __instance.player.SetHandItems(0, 0);
                        __instance.controller.cmd.type = ECommand.None;
                        __instance.controller.cmd.mode = 0;
                        __instance.controller.cmd.stage = 0;
                        __instance.controller.cmd.state = 0;
                        __instance.controller.cmd.refId = 0;
                    }
                }
                else if (cmd.stage == 0) {
                    if (flag) {
                        if (cmd.refId > 0) {
                            __instance.player.SetHandItems(0, 0);
                            __instance.controller.cmd.refId = 0;
                        }

                        if (__instance.buildPreviews.Count > 0)
                            __instance.ClearBuildPreviews();
                    }
                    else {
                        __instance.player.SetHandItems(0, 0);
                        __instance.controller.cmd.type = ECommand.None;
                        __instance.controller.cmd.mode = 0;
                        __instance.controller.cmd.stage = 0;
                        __instance.controller.cmd.state = 0;
                        __instance.controller.cmd.refId = 0;
                        VFInput.ResetAllAxes();
                    }
                }
                else
                    --__instance.controller.cmd.stage;
            }

            if (__instance.copyProtoId == 0)
                __instance.ResetCopyInfo();
            if (__instance.player.inhandItemId == 0)
                return false;
            if (__instance.copyProtoId != __instance.player.inhandItemId)
                __instance.ResetCopyInfo();
            ItemProto itemProto1 = LDB.items.Select(__instance.player.inhandItemId);
            if (itemProto1 != null && itemProto1.CanBuild)
                return false;
            __instance.ResetCopyInfo();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("DetermineBuildPreviews")]
        public static bool DetermineBuildPreviews(ref PlayerAction_Build __instance) {
            CommandState cmd = __instance.controller.cmd;
            __instance.waitConfirm = false;
            bool flag1 = false;
            bool flag2 = false;
            if (__instance.handPrefabDesc != null) {
                if (cmd.mode == 1) {
                    __instance.waitConfirm = __instance.cursorValid;
                    if (__instance.cursorValid) {
                        if (VFInput._switchSplitter.onDown)
                            ++__instance.modelOffset;
                        if (VFInput._ignoreGrid && __instance.handPrefabDesc.minerType == EMinerType.Vein) {
                            if ((bool) VFInput._rotate) {
                                __instance.yaw += 3f;
                                __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
                            }

                            if ((bool) VFInput._counterRotate) {
                                __instance.yaw -= 3f;
                                __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
                            }
                        }
                        else {
                            if (VFInput._rotate.onDown) {
                                __instance.yaw += 90f;
                                __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
                                __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
                            }

                            if (VFInput._counterRotate.onDown) {
                                __instance.yaw -= 90f;
                                __instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
                                __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
                            }

                            if (__instance.handPrefabDesc.minerType != EMinerType.Vein)
                                __instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
                        }

                        __instance.multiLevelCovering = false;
                        if (__instance.handPrefabDesc.multiLevel &&
                            __instance.GetObjectProtoId(__instance.castObjId) == __instance.handItem.ID)
                            __instance.multiLevelCovering = true;
                        if (__instance.multiLevelCovering) {
                            Pose objectPose = __instance.GetObjectPose(__instance.castObjId);
                            __instance.previewPose.position =
                                objectPose.position + objectPose.rotation * __instance.handPrefabDesc.lapJoint;
                            __instance.previewPose.rotation = !__instance.handPrefabDesc.multiLevelAllowRotate
                                ? objectPose.rotation
                                : Maths.SphericalRotation(__instance.cursorTarget, __instance.yaw);
                        }
                        else {
                            Vector3 zero = Vector3.zero;
                            Vector3 cursorTarget;
                            if (__instance.player.planetData.type == EPlanetType.Gas) {
                                cursorTarget = __instance.cursorTarget;
                                Vector3 vector3 = cursorTarget.normalized * __instance.player.planetData.realRadius *
                                                  0.025f;
                                cursorTarget += vector3;
                            }
                            else
                                cursorTarget = __instance.cursorTarget;

                            __instance.previewPose.position = cursorTarget;
                            __instance.previewPose.rotation = Maths.SphericalRotation(cursorTarget, __instance.yaw);
                        }

                        if (__instance.buildPreviews.Count > 1)
                            __instance.ClearBuildPreviews();
                        if (__instance.buildPreviews.Count == 0)
                            __instance.AddBuildPreview(BuildPreview.CreateSingle(__instance.handItem,
                                __instance.handPrefabDesc, true));
                        BuildPreview buildPreview = __instance.buildPreviews[0];
                        buildPreview.ResetInfos();
                        buildPreview.item = __instance.handItem;
                        buildPreview.desc = __instance.handPrefabDesc;
                        buildPreview.recipeId = __instance.copyRecipeId;
                        buildPreview.filterId = __instance.copyFilterId;
                        if (__instance.multiLevelCovering) {
                            buildPreview.input = (BuildPreview) null;
                            buildPreview.inputObjId = __instance.castObjId;
                            buildPreview.inputFromSlot = 15;
                            buildPreview.inputToSlot = 14;
                            buildPreview.inputOffset = 0;
                        }
                    }
                    else {
                        __instance.previewPose.position = Vector3.zero;
                        __instance.previewPose.rotation = Quaternion.identity;
                        __instance.ClearBuildPreviews();
                    }
                }
                else if (cmd.mode == 2) {
                    __instance.previewPose.position = Vector3.zero;
                    __instance.previewPose.rotation = Quaternion.identity;
                    if (VFInput._liftBeltsHeight.onDown)
                        ++__instance.altitude;
                    if (VFInput._reduceBeltsHeight.onDown)
                        --__instance.altitude;
                    if (VFInput._beltsZeroKey.onDown)
                        __instance.altitude = 0;
                    if (__instance.altitude > 60)
                        __instance.altitude = 60;
                    else if (__instance.altitude < 0)
                        __instance.altitude = 0;
                    if (cmd.stage == 0) {
                        if (__instance.cursorValid) {
                            __instance.connGraph.SetPointCount(0);
                            __instance.connGraph.AddPoint(cmd.target, 4U);
                            __instance.showConnGraph = true;
                            if (__instance.buildPreviews.Count > 1)
                                __instance.ClearBuildPreviews();
                            if (__instance.buildPreviews.Count == 0)
                                __instance.AddBuildPreview(BuildPreview.CreateSingle(__instance.handItem,
                                    __instance.handPrefabDesc,
                                    false));
                            BuildPreview buildPreview = __instance.buildPreviews[0];
                            buildPreview.ResetInfos();
                            buildPreview.isConnNode = true;
                            buildPreview.ignoreCollider = true;
                            buildPreview.inputObjId = __instance.castObjId;
                            __instance.buildPreviews[0].lpos = __instance.cursorTarget;
                            bool flag3 = __instance.ObjectIsBelt(__instance.castObjId);
                            if (__instance.castObjId != 0 && !flag3 &&
                                __instance.GetLocalGates(__instance.castObjId).Length == 0)
                                buildPreview.condition = EBuildCondition.BeltCannotConnectToBuilding;
                            if (VFInput._buildConfirm.onDown) {
                                VFInput.UseRtsConfirm();
                                __instance.startObjId = __instance.castObjId;
                                __instance.startTarget = __instance.cursorTarget;
                                __instance.snappedPointCount = 0;
                                __instance.keepSectionInfo = true;
                                __instance.controller.cmd.stage = 1;
                            }
                        }
                        else
                            __instance.ClearBuildPreviews();

                        __instance.cursorText = "选择起始位置".Translate();
                    }
                    else if (cmd.stage == 1) {
                        __instance.keepSectionInfo = true;
                        if (__instance.cursorValid) {
                            __instance.waitConfirm = true;
                            Pose[] localGates1 = __instance.GetLocalGates(__instance.startObjId);
                            Pose[] localGates2 = __instance.GetLocalGates(__instance.castObjId);
                            Vector3 begin1 = __instance.startTarget;
                            Vector3 end1 = __instance.cursorTarget;
                            Vector3 vector3_1 = __instance.cursorTarget - __instance.startTarget;
                            bool flag3 = __instance.ObjectIsBelt(__instance.startObjId);
                            int slot1 = -1;
                            Vector3 begin2 = Vector3.zero;
                            Vector3 vector3_2 = Vector3.zero;
                            bool flag4 = __instance.ObjectIsBelt(__instance.castObjId);
                            int slot2 = -1;
                            Vector3 begin3 = Vector3.zero;
                            Vector3 end2 = Vector3.zero;
                            if ((__instance.startObjId != 0 || __instance.castObjId != 0) &&
                                __instance.startObjId != __instance.castObjId) {
                                if (__instance.startObjId != 0 && localGates1.Length > 0) {
                                    PrefabDesc prefabDesc = __instance.GetPrefabDesc(__instance.startObjId);
                                    Pose objectPose = __instance.GetObjectPose(__instance.startObjId);
                                    bool flag5 = prefabDesc.minerType == EMinerType.Vein;
                                    bool isStation = prefabDesc.isStation;
                                    float num1 =
                                        (float) ((double) __instance.altitude * 1.33333325386047 +
                                                 (double) __instance.factory.planet.realRadius) -
                                        __instance.startTarget.magnitude;
                                    float num2 = -0.9f;
                                    for (int index = 0; index < localGates1.Length; ++index) {
                                        Vector3 vector3_3 = objectPose.position +
                                                            objectPose.rotation * localGates1[index].position;
                                        Vector3 vector3_4 = objectPose.rotation * localGates1[index].forward;
                                        Vector3 vector3_5 = vector3_3 + vector3_4 * (!flag5 ? 2f : 1.6f);
                                        Vector3 normalized = (vector3_5 - objectPose.position).normalized;
                                        float num3 = 0.25f -
                                                     Mathf.Abs((float) (((double) num1 -
                                                                         (double) localGates1[index].position.y) *
                                                                        0.25));
                                        float num4 = Vector3.Dot(vector3_1.normalized, normalized) + num3;
                                        if ((double) num4 > (double) num2) {
                                            num2 = num4;
                                            slot1 = index;
                                            begin2 = vector3_3;
                                            vector3_2 = vector3_5;
                                        }
                                    }

                                    if (slot1 >= 0) {
                                        begin1 = vector3_2;
                                        if (flag5) {
                                            begin1 = __instance.planetAux.Snap(vector3_2, true, false);
                                        }
                                        else {
                                            int path = 0;
                                            __instance.snappedPointCount = __instance.planetAux.SnapLineNonAlloc(
                                                begin2, vector3_2,
                                                ref path, __instance.snappedPoints);
                                            if (__instance.snappedPointCount >= 2) {
                                                for (int index = 1; index < __instance.snappedPointCount; ++index) {
                                                    if ((double) (__instance.snappedPoints[index] - begin2)
                                                        .magnitude >=
                                                        0.699999988079071) {
                                                        begin1 = __instance.snappedPoints[index];
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (__instance.startObjId > 0 && isStation) {
                                            flag2 = true;
                                            UIBeltBuildTip beltBuildTip = UIRoot.instance.uiGame.beltBuildTip;
                                            UIRoot.instance.uiGame.OpenBeltBuildTip();
                                            beltBuildTip.SetOutputEntity(__instance.startObjId, slot1);
                                            beltBuildTip.position = vector3_2;
                                            beltBuildTip.SetFilterToEntity();
                                        }
                                    }
                                }

                                if (__instance.castObjId != 0 && localGates2.Length > 0) {
                                    Pose objectPose = __instance.GetObjectPose(__instance.castObjId);
                                    float num1 =
                                        (float) ((double) __instance.altitude * 1.33333325386047 +
                                                 (double) __instance.factory.planet.realRadius) -
                                        __instance.cursorTarget.magnitude;
                                    float num2 = 0.0f;
                                    for (int index = 0; index < localGates2.Length; ++index) {
                                        Vector3 vector3_3 = objectPose.position +
                                                            objectPose.rotation * localGates2[index].position;
                                        Vector3 vector3_4 = objectPose.rotation * localGates2[index].forward;
                                        Vector3 vector3_5 = vector3_3 + vector3_4 * 2f;
                                        Vector3 normalized = (vector3_5 - objectPose.position).normalized;
                                        float num3 = 0.25f -
                                                     Mathf.Abs((float) (((double) num1 -
                                                                         (double) localGates2[index].position.y) *
                                                                        0.25));
                                        float num4 = Vector3.Dot(-vector3_1.normalized, normalized) + num3;
                                        if ((double) num4 > (double) num2) {
                                            num2 = num4;
                                            slot2 = index;
                                            begin3 = vector3_3;
                                            end2 = vector3_5;
                                        }
                                    }

                                    if (slot2 >= 0) {
                                        end1 = end2;
                                        int path = 0;
                                        __instance.snappedPointCount =
                                            __instance.planetAux.SnapLineNonAlloc(begin3, end2, ref path,
                                                __instance.snappedPoints);
                                        if (__instance.snappedPointCount >= 2) {
                                            for (int index = 1; index < __instance.snappedPointCount; ++index) {
                                                if ((double) (__instance.snappedPoints[index] - begin3).magnitude >=
                                                    0.699999988079071) {
                                                    end1 = __instance.snappedPoints[index];
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (VFInput._switchBeltsPath.onDown || VFInput._counterRotate.onDown)
                                __instance.beltMovePath = __instance.beltMovePath != 1
                                    ? (__instance.beltMovePath != 2 ? 1 : 1)
                                    : 2;
                            int beltMovePath = __instance.beltMovePath;
                            __instance.snappedPointCount =
                                __instance.planetAux.SnapLineNonAlloc(begin1, end1, ref beltMovePath,
                                    __instance.snappedPoints);
                            __instance.beltMovePathTime += 0.01f;
                            if ((double) __instance.beltMovePathTime > 0.600000023841858) {
                                if (beltMovePath != __instance.beltMovePath) {
                                    __instance.beltMovePath = beltMovePath;
                                    __instance.beltMovePathTime = 0.0f;
                                }
                            }
                            else
                                __instance.groundTestMovement = Vector3.zero;

                            bool flag6 = false;
                            bool flag7 = false;
                            if (slot1 >= 0) {
                                Array.Copy((Array) __instance.snappedPoints, 0, (Array) __instance.snappedPoints, 1,
                                    __instance.snappedPointCount);
                                __instance.snappedPoints[0] = begin2;
                                ++__instance.snappedPointCount;
                                int otherObjId;
                                __instance.factory.ReadObjectConn(__instance.startObjId, slot1, out bool _,
                                    out otherObjId,
                                    out int _);
                                if (otherObjId != 0)
                                    flag6 = true;
                            }

                            if (slot2 >= 0) {
                                __instance.snappedPoints[__instance.snappedPointCount] = begin3;
                                ++__instance.snappedPointCount;
                                int otherObjId;
                                __instance.factory.ReadObjectConn(__instance.castObjId, slot2, out bool _,
                                    out otherObjId,
                                    out int _);
                                if (otherObjId != 0)
                                    flag7 = true;
                            }

                            for (int index = 0; index < __instance.snappedPointCount; ++index) {
                                if (index < __instance.buildPreviews.Count) {
                                    __instance.buildPreviews[index].lpos = __instance.snappedPoints[index];
                                    __instance.buildPreviews[index].needModel = false;
                                    __instance.buildPreviews[index].isConnNode = true;
                                    __instance.buildPreviews[index].ResetInfos();
                                }
                                else {
                                    BuildPreview single =
                                        BuildPreview.CreateSingle(__instance.handItem, __instance.handPrefabDesc,
                                            false);
                                    single.lpos = __instance.snappedPoints[index];
                                    single.isConnNode = true;
                                    __instance.AddBuildPreview(single);
                                }
                            }

                            while (__instance.buildPreviews.Count > __instance.snappedPointCount)
                                __instance.RemoveBuildPreview(__instance.buildPreviews.Count - 1);
                            Assert.True(__instance.buildPreviews.Count == __instance.snappedPointCount);
                            for (int index = 0; index < __instance.buildPreviews.Count - 1; ++index) {
                                __instance.buildPreviews[index].output = __instance.buildPreviews[index + 1];
                                __instance.buildPreviews[index].outputObjId = 0;
                                __instance.buildPreviews[index].outputFromSlot = 0;
                                __instance.buildPreviews[index].outputToSlot = 1;
                                __instance.buildPreviews[index].outputOffset = 0;
                            }

                            if (__instance.buildPreviews.Count > 0) {
                                int index = __instance.buildPreviews.Count - 1;
                                if (__instance.startObjId != 0 && slot1 >= 0) {
                                    __instance.buildPreviews[0].input = (BuildPreview) null;
                                    __instance.buildPreviews[0].inputObjId = __instance.startObjId;
                                    __instance.buildPreviews[0].inputFromSlot = slot1;
                                    __instance.buildPreviews[0].inputToSlot = 1;
                                    __instance.buildPreviews[0].inputOffset = 0;
                                    __instance.buildPreviews[0].ignoreCollider = true;
                                    if (__instance.buildPreviews.Count < 2)
                                        __instance.buildPreviews[0].condition = EBuildCondition.TooShort;
                                    bool flag5 = __instance.ObjectIsSplitter(__instance.startObjId);
                                    if (__instance.buildPreviews.Count >= 2) {
                                        __instance.buildPreviews[1].ignoreCollider = true;
                                        Pose objectPose = __instance.GetObjectPose(__instance.startObjId);
                                        Vector3 from = __instance.buildPreviews[1].lpos -
                                                       __instance.buildPreviews[0].lpos;
                                        Vector3 to = objectPose.rotation * localGates1[slot1].forward;
                                        float num = !flag5 ? 45f : 10.8f;
                                        if ((double) Vector3.Angle(from, to) > (double) num)
                                            __instance.buildPreviews[1].condition = EBuildCondition.TooSkew;
                                    }

                                    if (__instance.buildPreviews.Count >= 3 &&
                                        (double) Vector3.Angle(
                                            __instance.buildPreviews[2].lpos - __instance.buildPreviews[1].lpos,
                                            __instance.GetObjectPose(__instance.startObjId).rotation *
                                            localGates1[slot1].forward) >
                                        80.0)
                                        __instance.buildPreviews[2].condition = EBuildCondition.TooSkew;
                                    if (flag6) {
                                        __instance.buildPreviews[0].condition = EBuildCondition.Occupied;
                                        if (__instance.buildPreviews.Count >= 2)
                                            __instance.buildPreviews[1].condition = EBuildCondition.Occupied;
                                    }
                                }

                                if (__instance.castObjId != 0 && slot2 >= 0) {
                                    __instance.buildPreviews[index].output = (BuildPreview) null;
                                    __instance.buildPreviews[index].outputObjId = __instance.castObjId;
                                    __instance.buildPreviews[index].outputFromSlot = 0;
                                    __instance.buildPreviews[index].outputToSlot = slot2;
                                    __instance.buildPreviews[index].outputOffset = 0;
                                    __instance.buildPreviews[index].ignoreCollider = true;
                                    if (__instance.buildPreviews.Count < 2)
                                        __instance.buildPreviews[index].condition = EBuildCondition.TooShort;
                                    if (__instance.buildPreviews.Count >= 2) {
                                        __instance.buildPreviews[index - 1].ignoreCollider = true;
                                        Pose objectPose = __instance.GetObjectPose(__instance.castObjId);
                                        if ((double) Vector3.Angle(
                                            __instance.buildPreviews[index - 1].lpos -
                                            __instance.buildPreviews[index].lpos,
                                            objectPose.rotation * localGates2[slot2].forward) > 10.8000001907349)
                                            __instance.buildPreviews[index - 1].condition = EBuildCondition.TooSkew;
                                    }

                                    if (__instance.buildPreviews.Count >= 3) {
                                        Pose objectPose = __instance.GetObjectPose(__instance.castObjId);
                                        if ((double) Vector3.Angle(
                                            __instance.buildPreviews[index - 2].lpos -
                                            __instance.buildPreviews[index - 1].lpos,
                                            objectPose.rotation * localGates2[slot2].forward) > 80.0)
                                            __instance.buildPreviews[index - 2].condition = EBuildCondition.TooSkew;
                                    }

                                    if (flag7) {
                                        __instance.buildPreviews[index].condition = EBuildCondition.Occupied;
                                        if (__instance.buildPreviews.Count >= 2)
                                            __instance.buildPreviews[index - 1].condition = EBuildCondition.Occupied;
                                    }
                                }

                                if (flag3) {
                                    int objectProtoId = __instance.GetObjectProtoId(__instance.startObjId);
                                    
                                    __instance.buildPreviews[0].coverObjId = __instance.startObjId;
                                    __instance.buildPreviews[0].ignoreCollider = true;
                                    __instance.buildPreviews[0].willCover =
                                        objectProtoId != __instance.buildPreviews[0].item.ID;
                                }

                                if (flag4) {
                                    int objectProtoId = __instance.GetObjectProtoId(__instance.castObjId);
                                    __instance.buildPreviews[index].coverObjId = __instance.castObjId;
                                    __instance.buildPreviews[index].ignoreCollider = true;
                                    __instance.buildPreviews[index].willCover =
                                        objectProtoId != __instance.buildPreviews[index].item.ID;
                                    if (__instance.buildPreviews.Count > 1) {
                                        if (__instance.castObjId > 0)
                                            Array.Copy((Array) __instance.factory.entityConnPool,
                                                __instance.castObjId * 16,
                                                (Array) __instance.tmp_conn, 0, 16);
                                        else
                                            Array.Copy((Array) __instance.factory.prebuildConnPool,
                                                -__instance.castObjId * 16,
                                                (Array) __instance.tmp_conn, 0, 16);
                                        __instance.buildPreviews[index - 1].outputToSlot = __instance.tmp_conn[1] != 0
                                            ? (__instance.tmp_conn[2] != 0
                                                ? (__instance.tmp_conn[3] != 0 ? 14 : 3)
                                                : 2)
                                            : 1;
                                    }
                                }

                                if (flag3 && flag4 && __instance.buildPreviews.Count <= 2) {
                                    __instance.buildPreviews[0].willCover = true;
                                    __instance.buildPreviews[index].willCover = true;
                                }

                                if (__instance.castObjId != 0 && !flag4 &&
                                    __instance.GetLocalGates(__instance.castObjId).Length == 0) {
                                    __instance.buildPreviews[index].condition =
                                        EBuildCondition.BeltCannotConnectToBuildingWithInserterTip;
                                    if (__instance.buildPreviews.Count > 1)
                                        __instance.buildPreviews[index - 1].condition =
                                            EBuildCondition.BeltCannotConnectToBuildingWithInserterTip;
                                    if (__instance.buildPreviews.Count > 2)
                                        __instance.buildPreviews[index - 2].condition =
                                            EBuildCondition.BeltCannotConnectToBuildingWithInserterTip;
                                }
                            }
                        }
                    }
                }
                else if (cmd.mode == 3) {
                    __instance.previewPose.position = Vector3.zero;
                    __instance.previewPose.rotation = Quaternion.identity;
                    if (cmd.stage == 0) {
                        if (!VFInput.onGUI)
                            UICursor.SetCursor(ECursor.TargetOut);
                        __instance.cursorText = "选择起始物体".Translate();
                        __instance.ClearBuildPreviews();
                        if (__instance.cursorValid) {
                            Pose[] poseArray = __instance.GetLocalInserts(__instance.castObjId);
                            bool flag3 = __instance.ObjectIsBelt(__instance.castObjId);
                            if (flag3)
                                poseArray = __instance.belt_slots;
                            if (poseArray.Length > 0) {
                                if (VFInput._buildConfirm.onDown) {
                                    VFInput.UseRtsConfirm();
                                    __instance.startObjId = __instance.castObjId;
                                    __instance.startTarget = __instance.cursorTarget;
                                    __instance.keepSectionInfo = true;
                                    __instance.controller.cmd.stage = 1;
                                    VFAudio.Create("build-start", (Transform) null, __instance.castObjPos, true);
                                }

                                __instance.cursorText = flag3 ? string.Empty : "选择起始物体".Translate();
                            }
                            else {
                                __instance.cursorText = "物体出入货物".Translate();
                                __instance.cursorWarning = true;
                            }
                        }
                        else if (__instance.castObjId < 0) {
                            __instance.cursorText = "已建成物体".Translate();
                            __instance.cursorWarning = true;
                        }
                    }
                    else if (cmd.stage == 1) {
                        if (!VFInput.onGUI)
                            UICursor.SetCursor(ECursor.TargetIn);
                        __instance.keepSectionInfo = true;
                        if (__instance.buildPreviews.Count > 1)
                            __instance.ClearBuildPreviews();
                        if (__instance.buildPreviews.Count == 0)
                            __instance.AddBuildPreview(BuildPreview.CreateSingle(__instance.handItem,
                                __instance.handPrefabDesc, true));
                        BuildPreview buildPreview = __instance.buildPreviews[0];
                        buildPreview.ResetInfos();
                        buildPreview.lpos = Vector3.zero;
                        buildPreview.lrot = Quaternion.identity;
                        buildPreview.lpos2 = Vector3.zero;
                        buildPreview.lrot2 = Quaternion.identity;
                        if (__instance.cursorValid && __instance.startObjId != __instance.castObjId &&
                            (__instance.startObjId > 0 && __instance.castObjId > 0)) {
                            CargoTraffic cargoTraffic = __instance.factory.cargoTraffic;
                            EntityData[] entityPool = __instance.factory.entityPool;
                            BeltComponent[] beltPool = cargoTraffic.beltPool;
                            __instance.posePairs.Clear();
                            __instance.startSlots.Clear();
                            __instance.endSlots.Clear();
                            Pose objectPose1 = __instance.GetObjectPose(__instance.startObjId);
                            
                            bool flag3 = __instance.ObjectIsBelt(__instance.startObjId);
                            Pose[] localInserts1 = __instance.GetLocalInserts(__instance.startObjId);
                            int castObjId = __instance.castObjId;
                            Pose objectPose2 = __instance.GetObjectPose(castObjId);
                            bool flag4 = __instance.ObjectIsBelt(castObjId);
                            Pose[] localInserts2 = __instance.GetLocalInserts(castObjId);
                            if (flag3) {
                                BeltComponent beltComponent = beltPool[entityPool[__instance.startObjId].beltId];
                                CargoPath cargoPath = cargoTraffic.GetCargoPath(beltComponent.segPathId);
                                int num1 = beltComponent.segIndex + beltComponent.segPivotOffset;
                                for (int index = 0; cargoPath != null && index < cargoPath.belts.Count; ++index) {
                                    int entityId = beltPool[cargoPath.belts[index]].entityId;
                                    if (entityId > 0) {
                                        int num2 = beltPool[cargoPath.belts[index]].segIndex +
                                            beltPool[cargoPath.belts[index]].segPivotOffset - num1;
                                        if (num2 < 0)
                                            num2 = -num2;
                                        if (num2 < 42) {
                                            foreach (Pose beltSlot in __instance.belt_slots) {
                                                PlayerAction_Build.SlotPoint slotPoint;
                                                slotPoint.objId = entityId;
                                                slotPoint.pose = beltSlot.GetTransformedBy(
                                                    new Pose(entityPool[entityId].pos, entityPool[entityId].rot));
                                                slotPoint.slotIdx = -1;
                                                __instance.startSlots.Add(slotPoint);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int index = 0; index < localInserts1.Length; ++index) {
                                PlayerAction_Build.SlotPoint slotPoint;
                                slotPoint.objId = __instance.startObjId;
                                slotPoint.pose = localInserts1[index].GetTransformedBy(objectPose1);
                                slotPoint.slotIdx = index;
                                __instance.startSlots.Add(slotPoint);
                            }

                            if (flag4) {
                                BeltComponent beltComponent = beltPool[entityPool[castObjId].beltId];
                                CargoPath cargoPath = cargoTraffic.GetCargoPath(beltComponent.segPathId);
                                int num1 = beltComponent.segIndex + beltComponent.segPivotOffset;
                                for (int index = 0; cargoPath != null && index < cargoPath.belts.Count; ++index) {
                                    int entityId = beltPool[cargoPath.belts[index]].entityId;
                                    if (entityId > 0) {
                                        int num2 = beltPool[cargoPath.belts[index]].segIndex +
                                            beltPool[cargoPath.belts[index]].segPivotOffset - num1;
                                        if (num2 < 0)
                                            num2 = -num2;
                                        if (num2 < 42) {
                                            foreach (Pose beltSlot in __instance.belt_slots) {
                                                PlayerAction_Build.SlotPoint slotPoint;
                                                slotPoint.objId = entityId;
                                                slotPoint.pose = beltSlot.GetTransformedBy(
                                                    new Pose(entityPool[entityId].pos, entityPool[entityId].rot));
                                                slotPoint.slotIdx = -1;
                                                __instance.endSlots.Add(slotPoint);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int index = 0; index < localInserts2.Length; ++index) {
                                PlayerAction_Build.SlotPoint slotPoint;
                                slotPoint.objId = castObjId;
                                slotPoint.pose = localInserts2[index].GetTransformedBy(objectPose2);
                                slotPoint.slotIdx = index;
                                __instance.endSlots.Add(slotPoint);
                            }

                            for (int index1 = 1; index1 < __instance.startSlots.Count; ++index1) {
                                bool flag5 = false;
                                for (int index2 = 0; index2 < index1; ++index2) {
                                    if (__instance.startSlots[index1].objId == __instance.startSlots[index2].objId &&
                                        __instance.startSlots[index1].slotIdx ==
                                        __instance.startSlots[index2].slotIdx) {
                                        flag5 = true;
                                        break;
                                    }
                                }

                                if (flag5) {
                                    __instance.startSlots.RemoveAt(index1);
                                    --index1;
                                }
                            }

                            for (int index1 = 1; index1 < __instance.endSlots.Count; ++index1) {
                                bool flag5 = false;
                                for (int index2 = 0; index2 < index1; ++index2) {
                                    if (__instance.endSlots[index1].objId == __instance.endSlots[index2].objId &&
                                        __instance.endSlots[index1].slotIdx == __instance.endSlots[index2].slotIdx) {
                                        flag5 = true;
                                        break;
                                    }
                                }

                                if (flag5) {
                                    __instance.endSlots.RemoveAt(index1);
                                    --index1;
                                }
                            }

                            bool flag6 = false;
                            PlayerAction_Build.PosePair posePair1 = new PlayerAction_Build.PosePair();
                            float num3 = 180f;
                            for (int index1 = 0; index1 < __instance.startSlots.Count; ++index1) {
                                for (int index2 = 0; index2 < __instance.endSlots.Count; ++index2) {
                                    PlayerAction_Build.PosePair posePair2 = new PlayerAction_Build.PosePair();
                                    posePair2.valid = true;
                                    posePair2.startObjId = __instance.startSlots[index1].objId;
                                    posePair2.startSlot = __instance.startSlots[index1].slotIdx;
                                    posePair2.startPose = __instance.startSlots[index1].pose;
                                    posePair2.startOffset = 0;
                                    posePair2.endObjId = __instance.endSlots[index2].objId;
                                    posePair2.endSlot = __instance.endSlots[index2].slotIdx;
                                    posePair2.endPose = __instance.endSlots[index2].pose;
                                    posePair2.endOffset = 0;
                                    posePair2.bias = 0.0f;
                                    bool flag5 = __instance.ObjectIsBelt(posePair2.startObjId);
                                    bool flag7 = __instance.ObjectIsBelt(posePair2.endObjId);
                                    Vector3 vector3_1 = posePair2.startPose.position - posePair2.endPose.position;
                                    Vector3 vector3_2 = -vector3_1;
                                    if (flag5) {
                                        Quaternion identity = Quaternion.identity;
                                        Vector3 zero = Vector3.zero;
                                        Quaternion rotation1 = posePair2.startPose.rotation *
                                                               Quaternion.Euler(0.0f, 90f, 0.0f);
                                        Vector3 to1 = rotation1.Forward();
                                        if ((double) Vector3.Angle(vector3_2, to1) < 40.0)
                                            posePair2.startPose.rotation = rotation1;
                                        Quaternion rotation2 = posePair2.startPose.rotation *
                                                               Quaternion.Euler(0.0f, 180f, 0.0f);
                                        Vector3 to2 = rotation2.Forward();
                                        if ((double) Vector3.Angle(vector3_2, to2) < 40.0)
                                            posePair2.startPose.rotation = rotation2;
                                        Quaternion rotation3 = posePair2.startPose.rotation *
                                                               Quaternion.Euler(0.0f, -90f, 0.0f);
                                        Vector3 to3 = rotation3.Forward();
                                        if ((double) Vector3.Angle(vector3_2, to3) < 40.0)
                                            posePair2.startPose.rotation = rotation3;
                                    }

                                    if (flag7) {
                                        Quaternion identity = Quaternion.identity;
                                        Vector3 zero = Vector3.zero;
                                        Quaternion rotation1 = posePair2.endPose.rotation *
                                                               Quaternion.Euler(0.0f, 90f, 0.0f);
                                        Vector3 to1 = rotation1.Forward();
                                        if ((double) Vector3.Angle(vector3_1, to1) < 40.0)
                                            posePair2.endPose.rotation = rotation1;
                                        Quaternion rotation2 = posePair2.endPose.rotation *
                                                               Quaternion.Euler(0.0f, 180f, 0.0f);
                                        Vector3 to2 = rotation2.Forward();
                                        if ((double) Vector3.Angle(vector3_1, to2) < 40.0)
                                            posePair2.endPose.rotation = rotation2;
                                        Quaternion rotation3 = posePair2.endPose.rotation *
                                                               Quaternion.Euler(0.0f, -90f, 0.0f);
                                        Vector3 to3 = rotation3.Forward();
                                        if ((double) Vector3.Angle(vector3_1, to3) < 40.0)
                                            posePair2.endPose.rotation = rotation3;
                                    }

                                    float num1 = Mathf.Max(Vector3.Angle(vector3_1, posePair2.endPose.forward),
                                        Vector3.Angle(vector3_2, posePair2.startPose.forward));
                                    float num2 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                        posePair2.endPose.forward);
                                    if (flag5 && !flag7) {
                                        Vector3 position = __instance.GetObjectPose(posePair2.endObjId).position;
                                        if ((double) Vector3.Angle(posePair2.endPose.forward,
                                            posePair2.startPose.position - position) > 80.0)
                                            continue;
                                    }
                                    else if (flag7 && !flag3) {
                                        Vector3 position = __instance.GetObjectPose(posePair2.startObjId).position;
                                        if ((double) Vector3.Angle(posePair2.startPose.forward,
                                            posePair2.endPose.position - position) > 80.0)
                                            continue;
                                    }

                                    if (flag5 || flag7) {
                                        num1 = Mathf.Abs(Mathf.Repeat(num1 + 45f, 90f) - 45f);
                                        num2 = Mathf.Abs(Mathf.Repeat(num2 + 45f, 90f) - 45f);
                                    }

                                    if ((double) num2 > (double) num1)
                                        num1 = num2;
                                    if ((double) num1 < (double) num3) {
                                        posePair2.bias = num1;
                                        num3 = num1;
                                        posePair1 = posePair2;
                                    }

                                    if ((double) num1 < 40.0) {
                                        if (flag5 && flag7) {
                                            bool flag8 = false;
                                            int beltId1 = __instance.factory.entityPool[posePair2.endObjId].beltId;
                                            Assert.Positive(beltId1);
                                            BeltComponent beltComponent1 = cargoTraffic.beltPool[beltId1];
                                            Assert.Positive(beltComponent1.segPathId);
                                            int segPathId1 = beltComponent1.segPathId;
                                            CargoPath cargoPath1 = cargoTraffic.GetCargoPath(segPathId1);
                                            Assert.NotNull((object) cargoPath1);
                                            int num4 = beltComponent1.segIndex - 5;
                                            int num5 = beltComponent1.segIndex + beltComponent1.segLength + 5;
                                            int num6 = beltComponent1.segIndex + beltComponent1.segPivotOffset;
                                            if (num4 < 4)
                                                num4 = 4;
                                            if (num4 > cargoPath1.pathLength - 5 - 1)
                                                num4 = cargoPath1.pathLength - 5 - 1;
                                            if (num5 < 4)
                                                num5 = 4;
                                            if (num5 > cargoPath1.pathLength - 5 - 1)
                                                num5 = cargoPath1.pathLength - 5 - 1;
                                            if (num6 < 4)
                                                num6 = 4;
                                            if (num6 > cargoPath1.pathLength - 5 - 1)
                                                num6 = cargoPath1.pathLength - 5 - 1;
                                            for (int index3 = num4; index3 < num5; ++index3) {
                                                float num7 = Vector3.Dot(vector3_2, posePair2.startPose.forward);
                                                Vector3 pointPo1 = cargoPath1.pointPos[index3];
                                                Vector3 pointPo2 = cargoPath1.pointPos[index3 + 1];
                                                Vector3 point = posePair2.startPose.position +
                                                                posePair2.startPose.forward * num7;
                                                float t = Kit.ClosestPoint2Straight(pointPo1, pointPo2, point);
                                                if ((double) t >= 0.0 && (double) t <= 1.0) {
                                                    posePair2.endPose.position = pointPo1 + (pointPo2 - pointPo1) * t;
                                                    posePair2.endPose.position -=
                                                        posePair2.endPose.position.normalized * 0.15f;
                                                    posePair2.endPose.rotation =
                                                        Quaternion.Slerp(cargoPath1.pointRot[index3],
                                                            cargoPath1.pointRot[index3 + 1], t);
                                                    Quaternion identity = Quaternion.identity;
                                                    Vector3 zero = Vector3.zero;
                                                    Quaternion rotation1 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, 90f, 0.0f);
                                                    Vector3 to1 = rotation1.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to1) < 40.0)
                                                        posePair2.endPose.rotation = rotation1;
                                                    Quaternion rotation2 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, 180f, 0.0f);
                                                    Vector3 to2 = rotation2.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to2) < 40.0)
                                                        posePair2.endPose.rotation = rotation2;
                                                    Quaternion rotation3 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, -90f, 0.0f);
                                                    Vector3 to3 = rotation3.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to3) < 40.0)
                                                        posePair2.endPose.rotation = rotation3;
                                                    vector3_1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                    vector3_2 = -vector3_1;
                                                    float num8 = Mathf.Max(
                                                        Vector3.Angle(vector3_1, posePair2.endPose.forward),
                                                        Vector3.Angle(vector3_2, posePair2.startPose.forward));
                                                    float num9 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                        posePair2.endPose.forward);
                                                    if ((double) num9 > (double) num8)
                                                        num8 = num9;
                                                    if ((double) num8 < (double) num3) {
                                                        posePair2.bias = num8;
                                                        num3 = num8;
                                                        posePair1 = posePair2;
                                                    }

                                                    if ((double) num8 < 11.0) {
                                                        posePair2.bias = num8;
                                                        posePair2.endOffset = index3 - num6;
                                                        __instance.posePairs.Add(posePair2);
                                                        flag8 = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            int beltId2 = __instance.factory.entityPool[posePair2.startObjId].beltId;
                                            Assert.Positive(beltId2);
                                            BeltComponent beltComponent2 = cargoTraffic.beltPool[beltId2];
                                            Assert.Positive(beltComponent2.segPathId);
                                            int segPathId2 = beltComponent2.segPathId;
                                            CargoPath cargoPath2 = cargoTraffic.GetCargoPath(segPathId2);
                                            Assert.NotNull((object) cargoPath2);
                                            int num10 = beltComponent2.segIndex - 5;
                                            int num11 = beltComponent2.segIndex + beltComponent2.segLength + 5;
                                            int num12 = beltComponent2.segIndex + beltComponent2.segPivotOffset;
                                            if (num10 < 4)
                                                num10 = 4;
                                            if (num10 > cargoPath2.pathLength - 5 - 1)
                                                num10 = cargoPath2.pathLength - 5 - 1;
                                            if (num11 < 4)
                                                num11 = 4;
                                            if (num11 > cargoPath2.pathLength - 5 - 1)
                                                num11 = cargoPath2.pathLength - 5 - 1;
                                            if (num12 < 4)
                                                num12 = 4;
                                            if (num12 > cargoPath2.pathLength - 5 - 1)
                                                num12 = cargoPath2.pathLength - 5 - 1;
                                            for (int index3 = num10; index3 < num11; ++index3) {
                                                float num7 = Vector3.Dot(vector3_1, posePair2.endPose.forward);
                                                Vector3 pointPo1 = cargoPath2.pointPos[index3];
                                                Vector3 pointPo2 = cargoPath2.pointPos[index3 + 1];
                                                Vector3 point = posePair2.endPose.position +
                                                                posePair2.endPose.forward * num7;
                                                float t = Kit.ClosestPoint2Straight(pointPo1, pointPo2, point);
                                                if ((double) t >= 0.0 && (double) t <= 1.0) {
                                                    posePair2.startPose.position = pointPo1 + (pointPo2 - pointPo1) * t;
                                                    posePair2.startPose.position -=
                                                        posePair2.startPose.position.normalized * 0.15f;
                                                    posePair2.startPose.rotation =
                                                        Quaternion.Slerp(cargoPath2.pointRot[index3],
                                                            cargoPath2.pointRot[index3 + 1], t);
                                                    Quaternion identity = Quaternion.identity;
                                                    Vector3 zero = Vector3.zero;
                                                    Quaternion rotation1 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, 90f, 0.0f);
                                                    Vector3 to1 = rotation1.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to1) < 40.0)
                                                        posePair2.startPose.rotation = rotation1;
                                                    Quaternion rotation2 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, 180f, 0.0f);
                                                    Vector3 to2 = rotation2.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to2) < 40.0)
                                                        posePair2.startPose.rotation = rotation2;
                                                    Quaternion rotation3 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, -90f, 0.0f);
                                                    Vector3 to3 = rotation3.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to3) < 40.0)
                                                        posePair2.startPose.rotation = rotation3;
                                                    vector3_1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                    vector3_2 = -vector3_1;
                                                    float num8 = Mathf.Max(
                                                        Vector3.Angle(vector3_1, posePair2.endPose.forward),
                                                        Vector3.Angle(vector3_2, posePair2.startPose.forward));
                                                    float num9 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                        posePair2.endPose.forward);
                                                    if ((double) num9 > (double) num8)
                                                        num8 = num9;
                                                    if ((double) num8 < (double) num3) {
                                                        posePair2.bias = num8;
                                                        num3 = num8;
                                                        posePair1 = posePair2;
                                                    }

                                                    if ((double) num8 < 11.0) {
                                                        posePair2.bias = num8;
                                                        posePair2.startOffset = index3 - num12;
                                                        __instance.posePairs.Add(posePair2);
                                                        flag8 = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!flag8) {
                                                Vector3 from1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                Vector3 from2 = -from1;
                                                float num7 = Mathf.Max(Vector3.Angle(from1, posePair2.endPose.forward),
                                                    Vector3.Angle(from2, posePair2.startPose.forward));
                                                float num8 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                    posePair2.endPose.forward);
                                                if ((double) num8 > (double) num7)
                                                    num7 = num8;
                                                if ((double) num7 < 11.0) {
                                                    posePair2.bias = num7;
                                                    __instance.posePairs.Add(posePair2);
                                                }
                                            }
                                        }
                                        else if (flag7) {
                                            int beltId = __instance.factory.entityPool[posePair2.endObjId].beltId;
                                            Assert.Positive(beltId);
                                            BeltComponent beltComponent = cargoTraffic.beltPool[beltId];
                                            Assert.Positive(beltComponent.segPathId);
                                            int segPathId = beltComponent.segPathId;
                                            CargoPath cargoPath = cargoTraffic.GetCargoPath(segPathId);
                                            Assert.NotNull((object) cargoPath);
                                            int num4 = beltComponent.segIndex - 5;
                                            int num5 = beltComponent.segIndex + beltComponent.segLength + 5;
                                            int num6 = beltComponent.segIndex + beltComponent.segPivotOffset;
                                            if (num4 < 4)
                                                num4 = 4;
                                            if (num4 > cargoPath.pathLength - 5 - 1)
                                                num4 = cargoPath.pathLength - 5 - 1;
                                            if (num5 < 4)
                                                num5 = 4;
                                            if (num5 > cargoPath.pathLength - 5 - 1)
                                                num5 = cargoPath.pathLength - 5 - 1;
                                            if (num6 < 4)
                                                num6 = 4;
                                            if (num6 > cargoPath.pathLength - 5 - 1)
                                                num6 = cargoPath.pathLength - 5 - 1;
                                            bool flag8 = false;
                                            for (int index3 = num4; index3 < num5; ++index3) {
                                                float num7 = Vector3.Dot(vector3_2, posePair2.startPose.forward);
                                                Vector3 pointPo1 = cargoPath.pointPos[index3];
                                                Vector3 pointPo2 = cargoPath.pointPos[index3 + 1];
                                                Vector3 point = posePair2.startPose.position +
                                                                posePair2.startPose.forward * num7;
                                                float t = Kit.ClosestPoint2Straight(pointPo1, pointPo2, point);
                                                if ((double) t >= 0.0 && (double) t <= 1.0) {
                                                    posePair2.endPose.position = pointPo1 + (pointPo2 - pointPo1) * t;
                                                    posePair2.endPose.position -=
                                                        posePair2.endPose.position.normalized * 0.15f;
                                                    posePair2.endPose.rotation =
                                                        Quaternion.Slerp(cargoPath.pointRot[index3],
                                                            cargoPath.pointRot[index3 + 1], t);
                                                    Quaternion identity = Quaternion.identity;
                                                    Vector3 zero = Vector3.zero;
                                                    Quaternion rotation1 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, 90f, 0.0f);
                                                    Vector3 to1 = rotation1.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to1) < 40.0)
                                                        posePair2.endPose.rotation = rotation1;
                                                    Quaternion rotation2 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, 180f, 0.0f);
                                                    Vector3 to2 = rotation2.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to2) < 40.0)
                                                        posePair2.endPose.rotation = rotation2;
                                                    Quaternion rotation3 = posePair2.endPose.rotation *
                                                                           Quaternion.Euler(0.0f, -90f, 0.0f);
                                                    Vector3 to3 = rotation3.Forward();
                                                    if ((double) Vector3.Angle(vector3_1, to3) < 40.0)
                                                        posePair2.endPose.rotation = rotation3;
                                                    vector3_1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                    vector3_2 = -vector3_1;
                                                    float num8 = Mathf.Max(
                                                        Vector3.Angle(vector3_1, posePair2.endPose.forward),
                                                        Vector3.Angle(vector3_2, posePair2.startPose.forward));
                                                    float num9 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                        posePair2.endPose.forward);
                                                    if ((double) num9 > (double) num8)
                                                        num8 = num9;
                                                    if ((double) num8 < (double) num3) {
                                                        posePair2.bias = num8;
                                                        num3 = num8;
                                                        posePair1 = posePair2;
                                                    }

                                                    if ((double) num8 < 11.0) {
                                                        posePair2.endOffset = index3 - num6;
                                                        posePair2.bias = num8;
                                                        __instance.posePairs.Add(posePair2);
                                                        flag8 = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!flag8) {
                                                Vector3 from1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                Vector3 from2 = -from1;
                                                float num7 = Mathf.Max(Vector3.Angle(from1, posePair2.endPose.forward),
                                                    Vector3.Angle(from2, posePair2.startPose.forward));
                                                float num8 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                    posePair2.endPose.forward);
                                                if ((double) num8 > (double) num7)
                                                    num7 = num8;
                                                if ((double) num7 < 11.0) {
                                                    posePair2.bias = num7;
                                                    __instance.posePairs.Add(posePair2);
                                                }
                                            }
                                        }
                                        else if (flag5) {
                                            int beltId = __instance.factory.entityPool[posePair2.startObjId].beltId;
                                            Assert.Positive(beltId);
                                            BeltComponent beltComponent = cargoTraffic.beltPool[beltId];
                                            Assert.Positive(beltComponent.segPathId);
                                            int segPathId = beltComponent.segPathId;
                                            CargoPath cargoPath = cargoTraffic.GetCargoPath(segPathId);
                                            Assert.NotNull((object) cargoPath);
                                            int num4 = beltComponent.segIndex - 5;
                                            int num5 = beltComponent.segIndex + beltComponent.segLength + 5;
                                            int num6 = beltComponent.segIndex + beltComponent.segPivotOffset;
                                            if (num4 < 4)
                                                num4 = 4;
                                            if (num4 > cargoPath.pathLength - 5 - 1)
                                                num4 = cargoPath.pathLength - 5 - 1;
                                            if (num5 < 4)
                                                num5 = 4;
                                            if (num5 > cargoPath.pathLength - 5 - 1)
                                                num5 = cargoPath.pathLength - 5 - 1;
                                            if (num6 < 4)
                                                num6 = 4;
                                            if (num6 > cargoPath.pathLength - 5 - 1)
                                                num6 = cargoPath.pathLength - 5 - 1;
                                            bool flag8 = false;
                                            for (int index3 = num4; index3 < num5; ++index3) {
                                                float num7 = Vector3.Dot(vector3_1, posePair2.endPose.forward);
                                                Vector3 pointPo1 = cargoPath.pointPos[index3];
                                                Vector3 pointPo2 = cargoPath.pointPos[index3 + 1];
                                                Vector3 point = posePair2.endPose.position +
                                                                posePair2.endPose.forward * num7;
                                                float t = Kit.ClosestPoint2Straight(pointPo1, pointPo2, point);
                                                if ((double) t >= 0.0 && (double) t <= 1.0) {
                                                    posePair2.startPose.position = pointPo1 + (pointPo2 - pointPo1) * t;
                                                    posePair2.startPose.position -=
                                                        posePair2.startPose.position.normalized * 0.15f;
                                                    posePair2.startPose.rotation =
                                                        Quaternion.Slerp(cargoPath.pointRot[index3],
                                                            cargoPath.pointRot[index3 + 1], t);
                                                    Quaternion identity = Quaternion.identity;
                                                    Vector3 zero = Vector3.zero;
                                                    Quaternion rotation1 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, 90f, 0.0f);
                                                    Vector3 to1 = rotation1.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to1) < 40.0)
                                                        posePair2.startPose.rotation = rotation1;
                                                    Quaternion rotation2 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, 180f, 0.0f);
                                                    Vector3 to2 = rotation2.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to2) < 40.0)
                                                        posePair2.startPose.rotation = rotation2;
                                                    Quaternion rotation3 = posePair2.startPose.rotation *
                                                                           Quaternion.Euler(0.0f, -90f, 0.0f);
                                                    Vector3 to3 = rotation3.Forward();
                                                    if ((double) Vector3.Angle(vector3_2, to3) < 40.0)
                                                        posePair2.startPose.rotation = rotation3;
                                                    vector3_1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                    vector3_2 = -vector3_1;
                                                    float num8 = Mathf.Max(
                                                        Vector3.Angle(vector3_1, posePair2.endPose.forward),
                                                        Vector3.Angle(vector3_2, posePair2.startPose.forward));
                                                    float num9 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                        posePair2.endPose.forward);
                                                    if ((double) num9 > (double) num8)
                                                        num8 = num9;
                                                    if ((double) num8 < (double) num3) {
                                                        posePair2.bias = num8;
                                                        num3 = num8;
                                                        posePair1 = posePair2;
                                                    }

                                                    if ((double) num8 < 11.0) {
                                                        posePair2.bias = num8;
                                                        posePair2.startOffset = index3 - num6;
                                                        __instance.posePairs.Add(posePair2);
                                                        flag8 = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!flag8) {
                                                Vector3 from1 = posePair2.startPose.position -
                                                                posePair2.endPose.position;
                                                Vector3 from2 = -from1;
                                                float num7 = Mathf.Max(Vector3.Angle(from1, posePair2.endPose.forward),
                                                    Vector3.Angle(from2, posePair2.startPose.forward));
                                                float num8 = 180f - Vector3.Angle(posePair2.startPose.forward,
                                                    posePair2.endPose.forward);
                                                if ((double) num8 > (double) num7)
                                                    num7 = num8;
                                                if ((double) num7 < 11.0) {
                                                    posePair2.bias = num7;
                                                    __instance.posePairs.Add(posePair2);
                                                }
                                            }
                                        }
                                        else if ((double) num1 < 14.0) {
                                            posePair2.bias = num1;
                                            __instance.posePairs.Add(posePair2);
                                        }
                                        else
                                            flag6 = true;
                                    }
                                    else
                                        flag6 = true;
                                }
                            }

                            if (__instance.posePairs.Count > 0) {
                                float num1 = 1000f;
                                float num2 =
                                    Vector3.Distance(__instance.currMouseRay.origin, __instance.cursorTarget) + 10f;
                                PlayerAction_Build.PosePair posePair2 = new PlayerAction_Build.PosePair();
                                bool flag5 = false;
                                for (int index = 0; index < __instance.posePairs.Count; ++index) {
                                    Vector3 origin = __instance.currMouseRay.origin;
                                    Vector3 endA = __instance.currMouseRay.origin +
                                                   __instance.currMouseRay.direction * num2;
                                    Vector3 vector3_1 = __instance.posePairs[index].startPose.position +
                                                        __instance.posePairs[index].startPose.up * 0.5f;
                                    Vector3 vector3_2 = __instance.posePairs[index].endPose.position +
                                                        __instance.posePairs[index].endPose.up * 0.5f;
                                    Vector3 vector3_3 = vector3_2 - vector3_1;
                                    Vector3 startB = vector3_1 - vector3_3 * 5f;
                                    Vector3 endB = vector3_2 + vector3_3 * 5f;
                                    Vector3 pointA;
                                    Vector3 pointB;
                                    Kit.ClosestPoint(origin, endA, startB, endB, out pointA, out pointB);
                                    float num4 = (float) ((double) Vector3.Distance(pointA, pointB) +
                                                          (double) Vector3.Distance(__instance.cursorTarget,
                                                              __instance.posePairs[index].endPose.position) * 0.5 +
                                                          (double) Vector3.Distance(__instance.startTarget,
                                                              __instance.posePairs[index].startPose.position) * 0.5 +
                                                          (double) Mathf.Abs(__instance.posePairs[index].bias) *
                                                          0.0599999986588955);
                                    if ((double) num4 < (double) num1) {
                                        num1 = num4;
                                        posePair2 = __instance.posePairs[index];
                                        flag5 = true;
                                    }
                                }

                                if (flag5) {
                                    __instance.waitConfirm = true;
                                    buildPreview.lpos = posePair2.startPose.position;
                                    buildPreview.lrot = posePair2.startPose.rotation;
                                    buildPreview.lpos2 = posePair2.endPose.position;
                                    buildPreview.lrot2 =
                                        posePair2.endPose.rotation * Quaternion.Euler(0.0f, 180f, 0.0f);
                                    buildPreview.ignoreCollider = false;
                                    buildPreview.outputObjId = posePair2.endObjId;
                                    buildPreview.outputFromSlot = 0;
                                    buildPreview.outputToSlot = posePair2.endSlot;
                                    buildPreview.outputOffset = posePair2.endOffset;
                                    buildPreview.inputObjId = posePair2.startObjId;
                                    buildPreview.inputToSlot = 1;
                                    buildPreview.inputFromSlot = posePair2.startSlot;
                                    buildPreview.inputOffset = posePair2.startOffset;
                                }
                            }
                            else if (flag6) {
                                __instance.waitConfirm = true;
                                buildPreview.condition = EBuildCondition.TooSkew;
                                buildPreview.lpos = posePair1.startPose.position;
                                buildPreview.lrot = posePair1.startPose.rotation;
                                buildPreview.lpos2 = posePair1.endPose.position;
                                buildPreview.lrot2 = posePair1.endPose.rotation * Quaternion.Euler(0.0f, 180f, 0.0f);
                                buildPreview.ignoreCollider = false;
                                buildPreview.outputObjId = posePair1.endObjId;
                                buildPreview.outputFromSlot = 0;
                                buildPreview.outputToSlot = posePair1.endSlot;
                                buildPreview.outputOffset = posePair1.endOffset;
                                buildPreview.inputObjId = posePair1.startObjId;
                                buildPreview.inputToSlot = 1;
                                buildPreview.inputFromSlot = posePair1.startSlot;
                                buildPreview.inputOffset = posePair1.startOffset;
                            }
                            else
                                buildPreview.condition = EBuildCondition.NeedConn;
                        }
                        else {
                            buildPreview.condition = EBuildCondition.NeedConn;
                            buildPreview.inputObjId = __instance.startObjId;
                        }

                        if (VFInput.readyToBuild) {
                            flag1 = true;
                            UIInserterBuildTip inserterBuildTip = UIRoot.instance.uiGame.inserterBuildTip;
                            inserterBuildTip.tryFindFilter = __instance.copyFilterId;
                            UIRoot.instance.uiGame.OpenInserterBuildTip();
                            inserterBuildTip.direction = (buildPreview.lpos2 - buildPreview.lpos).normalized;
                            inserterBuildTip.output = false;
                            inserterBuildTip.input = false;
                            inserterBuildTip.desc = buildPreview.desc;
                            if (buildPreview.outputObjId > 0) {
                                EntityData entityData = __instance.factory.entityPool[buildPreview.outputObjId];
                                if (entityData.beltId == 0 && entityData.inserterId == 0)
                                    inserterBuildTip.input = true;
                                if (entityData.beltId > 0)
                                    inserterBuildTip.inputBelt = true;
                            }

                            if (buildPreview.inputObjId > 0) {
                                EntityData entityData = __instance.factory.entityPool[buildPreview.inputObjId];
                                if (entityData.beltId == 0 && entityData.inserterId == 0)
                                    inserterBuildTip.output = true;
                                if (entityData.beltId > 0)
                                    inserterBuildTip.outputBelt = true;
                                inserterBuildTip.SetOutputEntity(buildPreview.inputObjId);
                            }
                            else
                                inserterBuildTip.SetOutputEntity(0);

                            if (buildPreview.outputObjId > 0) {
                                inserterBuildTip.position = (buildPreview.lpos + buildPreview.lpos2) * 0.5f;
                                inserterBuildTip.position1 = buildPreview.lpos;
                                inserterBuildTip.position2 = buildPreview.lpos2;
                            }
                            else {
                                inserterBuildTip.direction = Vector3.zero;
                                inserterBuildTip.position = __instance.GetObjectPose(__instance.startObjId).position;
                                inserterBuildTip.position1 = __instance.GetObjectPose(__instance.startObjId).position;
                                inserterBuildTip.position2 = __instance.GetObjectPose(__instance.startObjId).position;
                            }

                            buildPreview.filterId = inserterBuildTip.selectedFilter;
                        }
                    }
                }
                else {
                    __instance.previewPose.position = Vector3.zero;
                    __instance.previewPose.rotation = Quaternion.identity;
                    __instance.ClearBuildPreviews();
                }
            }
            else {
                if (cmd.mode == 4) {
                    int[] consumeRegister = GameMain.statistics.production.factoryStatPool[__instance.factory.index]
                        .consumeRegister;
                    if (__instance.reformSize < 1)
                        __instance.reformSize = 1;
                    else if (__instance.reformSize > 10)
                        __instance.reformSize = 10;
                    if ((double) (__instance.reformCenterPoint - __instance.player.position).sqrMagnitude >
                        (double) __instance.player.mecha.buildArea * (double) __instance.player.mecha.buildArea) {
                        if (!VFInput.onGUI) {
                            __instance.cursorText = "目标超出范围".Translate();
                            __instance.cursorWarning = true;
                            UICursor.SetCursor(ECursor.Ban);
                        }
                    }
                    else {
                        if (!VFInput.onGUI)
                            UICursor.SetCursor(ECursor.Reform);
                        bool flag3 = false;
                        if (VFInput._reformPlusKey.onDown) {
                            if (__instance.reformSize < 10) {
                                ++__instance.reformSize;
                                flag3 = true;
                                for (int index = 0; index < __instance.reformSize * __instance.reformSize; ++index)
                                    __instance.reformIndices[index] = -1;
                            }
                        }
                        else if (VFInput._reformMinusKey.onDown && __instance.reformSize > 1) {
                            --__instance.reformSize;
                            flag3 = true;
                        }

                        float radius = 0.9909459f * (float) __instance.reformSize;
                        int flattenTerrainReform = __instance.factory.ComputeFlattenTerrainReform(
                            __instance.reformPoints,
                            __instance.reformCenterPoint, radius, __instance.reformPointsCount);
                        if (__instance.cursorValid && !VFInput.onGUI) {
                            if (flattenTerrainReform > 0)
                                __instance.cursorText =
                                    "沙土消耗".Translate() + " " + flattenTerrainReform.ToString() + " " +
                                    "个沙土".Translate() + "\n" + "改造大小".Translate() +
                                    __instance.reformSize.ToString() + "x" + __instance.reformSize.ToString();
                            else if (flattenTerrainReform == 0) {
                                __instance.cursorText = "改造大小".Translate() + __instance.reformSize.ToString() + "x" +
                                                        __instance.reformSize.ToString();
                            }
                            else {
                                int num = -flattenTerrainReform;
                                __instance.cursorText =
                                    "沙土获得".Translate() + " " + num.ToString() + " " + "个沙土".Translate() +
                                    "\n" + "改造大小".Translate() + __instance.reformSize.ToString() + "x" +
                                    __instance.reformSize.ToString();
                            }

                            if (VFInput._buildConfirm.pressing) {
                                bool flag4 = false;
                                if (VFInput._buildConfirm.onDown) {
                                    flag4 = true;
                                    __instance.reformMouseOnDown = true;
                                }

                                if (__instance.reformMouseOnDown) {
                                    __instance.inReformOperation = true;
                                    if ((double) __instance.reformChangedPoint.x !=
                                        (double) __instance.reformCenterPoint.x ||
                                        (double) __instance.reformChangedPoint.y !=
                                        (double) __instance.reformCenterPoint.y ||
                                        ((double) __instance.reformChangedPoint.z !=
                                         (double) __instance.reformCenterPoint.z ||
                                         flag3)) {
                                        if (__instance.handItem.BuildMode == 4 &&
                                            __instance.player.package.GetItemCount(__instance.handItem.ID) +
                                            __instance.player.inhandItemCount >= __instance.reformPointsCount) {
                                            int newSandCount = __instance.player.sandCount - flattenTerrainReform;
                                            if (newSandCount >= 0) {
                                                __instance.factory.FlattenTerrainReform(__instance.reformCenterPoint,
                                                    radius,
                                                    __instance.reformSize, __instance.veinBuried);
                                                VFAudio.Create("reform-terrain", (Transform) null,
                                                    __instance.reformCenterPoint, true);
                                                __instance.player.SetSandCount(newSandCount);
                                                int num = __instance.reformSize * __instance.reformSize;
                                                for (int index = 0; index < num; ++index) {
                                                    int reformIndex = __instance.reformIndices[index];
                                                    PlatformSystem platformSystem = __instance.factory.platformSystem;
                                                    if (reformIndex >= 0) {
                                                        int reformType = platformSystem.GetReformType(reformIndex);
                                                        int reformColor = platformSystem.GetReformColor(reformIndex);
                                                        if (reformType != __instance.reformType ||
                                                            reformColor != __instance.reformColor) {
                                                            __instance.factory.platformSystem.SetReformType(
                                                                reformIndex,
                                                                __instance.reformType);
                                                            __instance.factory.platformSystem.SetReformColor(
                                                                reformIndex,
                                                                __instance.reformColor);
                                                        }
                                                    }
                                                }

                                                int reformPointsCount = __instance.reformPointsCount;
                                                if (__instance.player.inhandItemCount > 0) {
                                                    int cnt = __instance.reformPointsCount >=
                                                              __instance.player.inhandItemCount
                                                        ? __instance.player.inhandItemCount
                                                        : __instance.reformPointsCount;
                                                    __instance.player.UseHandItems(cnt);
                                                    reformPointsCount -= cnt;
                                                }

                                                int id = __instance.handItem.ID;
                                                consumeRegister[id] += __instance.reformPointsCount;
                                                __instance.player.package.TakeTailItems(ref id, ref reformPointsCount);
                                                GameMain.gameScenario.NotifyOnBuild(__instance.player.planetId,
                                                    __instance.handItem.ID, 0);
                                            }
                                            else if (flag4)
                                                UIRealtimeTip.Popup("沙土不足".Translate());
                                        }
                                        else if (flag4)
                                            UIRealtimeTip.Popup("物品不足".Translate(), tipId: 1);
                                    }
                                }
                                else
                                    __instance.inReformOperation = false;

                                __instance.reformChangedPoint = __instance.reformCenterPoint;
                            }
                            else {
                                __instance.inReformOperation = false;
                                __instance.reformChangedPoint = Vector3.zero;
                                __instance.reformMouseOnDown = false;
                            }
                        }
                    }
                }

                __instance.ClearBuildPreviews();
            }

            if (!flag1)
                UIRoot.instance.uiGame.CloseInserterBuildTip();
            if (flag2)
                return false;
            UIRoot.instance.uiGame.beltBuildTip.SetOutputEntity(0, -1);
            UIRoot.instance.uiGame.CloseBeltBuildTip();
            return false;
        }   */
    }
 
}