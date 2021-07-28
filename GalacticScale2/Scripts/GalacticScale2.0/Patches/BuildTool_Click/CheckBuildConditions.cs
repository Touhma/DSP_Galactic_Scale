using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Click
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        public static bool CheckBuildConditions(ref bool __result, ref BuildTool_Click __instance,
            ref PlanetData ___planet, ref PlanetFactory ___factory)
        {
            if (__instance.buildPreviews.Count == 0)
            {
                __result = false;
                return false;
            }

            var history = __instance.actionBuild.history;
            var flag = false;
            var SingleOrPasteItems = 1;
            var templatePreviews = __instance.actionBuild.templatePreviews;
            if (templatePreviews.Count > 0) SingleOrPasteItems = templatePreviews.Count;
            for (var i = 0; i < __instance.buildPreviews.Count; i++)
            {
                var buildPreview = __instance.buildPreviews[i];
                var buildPreview2 = __instance.buildPreviews[i / SingleOrPasteItems * SingleOrPasteItems];
                if (buildPreview.condition != 0) continue;
                var vector = buildPreview.lpos;
                var quaternion = buildPreview.lrot;
                var lpos = buildPreview.lpos2;
                _ = buildPreview.lrot2;
                var pose = new Pose(buildPreview.lpos, buildPreview.lrot);
                var pose2 = new Pose(buildPreview.lpos2, buildPreview.lrot2);
                var forward = pose.forward;
                _ = pose2.forward;
                var up = pose.up;
                var vector2 = Vector3.Lerp(vector, lpos, 0.5f);
                var forward2 = lpos - vector;
                if (forward2.sqrMagnitude < 0.0001f) forward2 = Maths.SphericalRotation(vector, 0f).Forward();
                var quaternion2 = Quaternion.LookRotation(forward2, vector2.normalized);
                var isGas = ___planet != null && ___planet.type == EPlanetType.Gas;
                if (vector.sqrMagnitude < 1f)
                {
                    buildPreview.condition = EBuildCondition.Failure;
                    // Warn($"VecSqr{vector.sqrMagnitude}");
                    continue;
                }

                var flag3 = buildPreview.desc.minerType == EMinerType.None && !buildPreview.desc.isBelt &&
                            !buildPreview.desc.isSplitter &&
                            (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen ||
                             buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger) &&
                            !buildPreview.desc.isStation && !buildPreview.desc.isSilo && !buildPreview.desc.multiLevel;
                if (buildPreview.desc.veinMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    var vector3 = vector + forward * -1.2f;
                    var rhs = -forward;
                    var vector4 = up;
                    var veinsInAreaNonAlloc =
                        __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector3, 12f, BuildTool._tmp_ids);
                    var prebuildData = default(PrebuildData);
                    prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                    var veinPool = ___factory.veinPool;
                    var paramCount = 0;
                    for (var j = 0; j < veinsInAreaNonAlloc; j++)
                        if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
                        {
                            if (veinPool[BuildTool._tmp_ids[j]].type != EVeinType.Oil)
                            {
                                var rhs2 = veinPool[BuildTool._tmp_ids[j]].pos - vector3;
                                var num2 = Vector3.Dot(vector4, rhs2);
                                rhs2 -= vector4 * num2;
                                var sqrMagnitude = rhs2.sqrMagnitude;
                                var num3 = Vector3.Dot(rhs2.normalized, rhs);
                                if (!(sqrMagnitude > 60.0625f) && !(num3 < 0.73f) && !(Mathf.Abs(num2) > 2f))
                                    prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[j];
                            }
                        }
                        else
                        {
                            Assert.CannotBeReached();
                        }

                    prebuildData.paramCount = paramCount;
                    prebuildData.ArrageParametersArray();
                    buildPreview.parameters = prebuildData.parameters;
                    buildPreview.paramCount = prebuildData.paramCount;
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    if (prebuildData.paramCount == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }
                }
                else if (buildPreview.desc.oilMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    var vector5 = vector;
                    var vector6 = -up;
                    var veinsInAreaNonAlloc2 =
                        __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector5, 10f, BuildTool._tmp_ids);
                    var prebuildData2 = default(PrebuildData);
                    prebuildData2.InitParametersArray(veinsInAreaNonAlloc2);
                    var veinPool2 = ___factory.veinPool;
                    var num4 = 0;
                    var num5 = 100f;
                    var pos = vector5;
                    for (var k = 0; k < veinsInAreaNonAlloc2; k++)
                        if (BuildTool._tmp_ids[k] != 0 &&
                            veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k] &&
                            veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Oil)
                        {
                            var pos2 = veinPool2[BuildTool._tmp_ids[k]].pos;
                            var vector7 = pos2 - vector5;
                            var num6 = Vector3.Dot(vector6, vector7);
                            var sqrMagnitude2 = (vector7 - vector6 * num6).sqrMagnitude;
                            if (sqrMagnitude2 < num5)
                            {
                                num5 = sqrMagnitude2;
                                num4 = BuildTool._tmp_ids[k];
                                pos = pos2;
                            }
                        }

                    if (num4 == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;
                    }

                    prebuildData2.parameters[0] = num4;
                    prebuildData2.paramCount = 1;
                    prebuildData2.ArrageParametersArray();
                    buildPreview.parameters = prebuildData2.parameters;
                    buildPreview.paramCount = prebuildData2.paramCount;
                    var vector8 = ___factory.planet.aux.Snap(pos, true);
                    vector = pose.position = buildPreview.lpos2 = buildPreview.lpos = vector8;
                    quaternion = pose.rotation = buildPreview.lrot2 =
                        buildPreview.lrot = Maths.SphericalRotation(vector8, __instance.yaw);
                    forward = pose.forward;
                    up = pose.up;
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                }

                if (buildPreview.desc.isTank || buildPreview.desc.isStorage || buildPreview.desc.isLab ||
                    buildPreview.desc.isSplitter)
                {
                    var num7 = buildPreview.desc.isLab ? history.labLevel : history.storageLevel;
                    var num8 = buildPreview.desc.isLab ? 15 : 8;
                    var num9 = 0;
                    ___factory.ReadObjectConn(buildPreview.inputObjId, 14, out var isOutput, out var otherObjId,
                        out var otherSlot);
                    while (otherObjId != 0)
                    {
                        num9++;
                        ___factory.ReadObjectConn(otherObjId, 14, out isOutput, out otherObjId, out otherSlot);
                    }

                    if (num9 >= num7 - 1)
                    {
                        flag = num7 >= num8;
                        buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
                        continue;
                    }
                }

                var vector9 = __instance.player.position;
                var num10 = __instance.player.mecha.buildArea * __instance.player.mecha.buildArea;
                if (isGas) // gas
                {
                    vector9 = vector9.normalized;
                    vector9 *= ___planet.realRadius;
                    num10 *= 6f;
                }

                if ((vector - vector9).sqrMagnitude > num10)
                {
                    buildPreview.condition = EBuildCondition.OutOfReach;
                    continue;
                }

                if (___planet != null)
                {
                    var num11 = history.buildMaxHeight + 0.5f + ___planet.realRadius * (isGas ? 1.025f : 1f);
                    if (vector.sqrMagnitude > num11 * num11)
                    {
                        buildPreview.condition = EBuildCondition.OutOfReach;
                        continue;
                    }
                }

                if (buildPreview.desc.hasBuildCollider)
                {
                    // Log("HasBuildCollider");
                    var buildColliders = buildPreview.desc.buildColliders;
                    // GS2.Log(buildColliders.Length.ToString());
                    for (var l = 0; l < buildColliders.Length; l++)
                    {
                        var colliderData = buildPreview.desc.buildColliders[l];
                        if (buildPreview.desc.isInserter)
                        {
                            colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y,
                                Vector3.Distance(lpos, vector) * 0.5f + colliderData.ext.z - 0.5f);
                            if (__instance.ObjectIsBelt(buildPreview.inputObjId) ||
                                buildPreview.input != null && buildPreview.input.desc.isBelt)
                            {
                                colliderData.pos.z -= 0.35f;
                                colliderData.ext.z += 0.35f;
                            }
                            else if (buildPreview.inputObjId == 0 && buildPreview.input == null)
                            {
                                colliderData.pos.z -= 0.35f;
                                colliderData.ext.z += 0.35f;
                            }

                            if (__instance.ObjectIsBelt(buildPreview.outputObjId) ||
                                buildPreview.output != null && buildPreview.output.desc.isBelt)
                            {
                                // Log("IsBelt");
                                colliderData.pos.z += 0.35f;
                                colliderData.ext.z += 0.35f;
                            }
                            else if (buildPreview.outputObjId == 0 && buildPreview.output == null)
                            {
                                // Log("IsBeltNull");
                                colliderData.pos.z += 0.35f;
                                colliderData.ext.z += 0.35f;
                            }

                            if (colliderData.ext.z < 0.1f) colliderData.ext.z = 0.1f;
                            colliderData.pos = vector2 + quaternion2 * colliderData.pos;
                            colliderData.q = quaternion2 * colliderData.q;
                            colliderData.DebugDraw();
                        }
                        else
                        {
                            colliderData.pos = vector + quaternion * colliderData.pos;
                            colliderData.q = quaternion * colliderData.q;
                        }

                        // Log("l" + l + " " + colliderData.pos + " " + colliderData.ext);
                        var mask = 428032;
                        if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner) mask = 425984;
                        Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
                        var num12 = Physics.OverlapBoxNonAlloc(colliderData.pos, colliderData.ext, BuildTool._tmp_cols,
                            colliderData.q, mask, QueryTriggerInteraction.Collide);
                        if (num12 > 0)
                        {
                            var flag4 = false;
                            var physics = __instance.player.planetData.physics;
                            for (var m = 0; m < num12 && buildPreview.coverObjId == 0; m++)
                            {
                                ColliderData cd;
                                var colliderData2 = physics.GetColliderData(BuildTool._tmp_cols[m], out cd);
                                var num13 = 0;
                                if (colliderData2 && cd.isForBuild)
                                {
                                    if (cd.objType == EObjectType.Entity)
                                        num13 = cd.objId;
                                    else if (cd.objType == EObjectType.Prebuild) num13 = -cd.objId;
                                }

                                var collider = BuildTool._tmp_cols[m];
                                if (collider.gameObject.layer == 18)
                                {
                                    var component = collider.GetComponent<BuildPreviewModel>();
                                    if (component != null && component.index == buildPreview.previewIndex ||
                                        buildPreview.desc.isInserter && !component.buildPreview.desc.isInserter ||
                                        !buildPreview.desc.isInserter && component.buildPreview.desc.isInserter)
                                        continue;
                                }
                                else if (buildPreview.desc.isInserter && num13 != 0 &&
                                         (num13 == buildPreview.inputObjId || num13 == buildPreview.outputObjId ||
                                          num13 == buildPreview2.coverObjId))
                                {
                                    continue;
                                }

                                flag4 = true;
                                if (!flag3 || num13 == 0) continue;
                                var itemProto = __instance.GetItemProto(num13);
                                if (!buildPreview.item.IsSimilar(itemProto)) continue;
                                var objectPose = __instance.GetObjectPose(num13);
                                var objectPose2 = __instance.GetObjectPose2(num13);
                                if ((objectPose.position - buildPreview.lpos).sqrMagnitude < 0.01 &&
                                    (objectPose2.position - buildPreview.lpos2).sqrMagnitude < 0.01 &&
                                    ((objectPose.forward - forward).sqrMagnitude < 1E-06 ||
                                     buildPreview.desc.isInserter))
                                {
                                    if (buildPreview.item.ID == itemProto.ID)
                                    {
                                        buildPreview.coverObjId = num13;
                                        buildPreview.willRemoveCover = false;
                                        flag4 = false;
                                    }
                                    else
                                    {
                                        buildPreview.coverObjId = num13;
                                        buildPreview.willRemoveCover = true;
                                        flag4 = false;
                                    }

                                    break;
                                }
                            }

                            if (flag4)
                            {
                                buildPreview.condition = EBuildCondition.Collide;
                                break;
                            }
                        }

                        if (buildPreview.desc.veinMiner && Physics.CheckBox(colliderData.pos, colliderData.ext,
                            colliderData.q, 2048, QueryTriggerInteraction.Collide))
                        {
                            buildPreview.condition = EBuildCondition.Collide;
                            break;
                        }
                    }

                    if (buildPreview.condition != 0) continue;
                }

                if (buildPreview2.coverObjId != 0 && buildPreview.desc.isInserter)
                {
                    // Log("IsInserter");
                    if (buildPreview.output == buildPreview2)
                    {
                        buildPreview.outputObjId = buildPreview2.coverObjId;
                        buildPreview.output = null;
                    }

                    if (buildPreview.input == buildPreview2)
                    {
                        buildPreview.inputObjId = buildPreview2.coverObjId;
                        buildPreview.input = null;
                    }
                }

                if (buildPreview.coverObjId == 0 || buildPreview.willRemoveCover)
                {
                    // Log("WillRemoveCover");
                    var itemId = buildPreview.item.ID;
                    var count = 1;
                    if (__instance.tmpInhandId == itemId && __instance.tmpInhandCount > 0)
                    {
                        count = 1;
                        __instance.tmpInhandCount--;
                    }
                    else
                    {
                        __instance.tmpPackage.TakeTailItems(ref itemId, ref count);
                    }

                    if (count == 0)
                    {
                        buildPreview.condition = EBuildCondition.NotEnoughItem;
                        continue;
                    }
                }

                if (buildPreview.coverObjId != 0) continue;
                if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
                {
                    if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length !=
                        buildPreview.nearestPowerObjId.Length)
                        buildPreview.nearestPowerObjId = new int[__instance.factory.powerSystem.netCursor];
                    Array.Clear(buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
                    var num14 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
                    var x = vector.x;
                    var y = vector.y;
                    var z = vector.z;
                    var netCursor = __instance.factory.powerSystem.netCursor;
                    var netPool = __instance.factory.powerSystem.netPool;
                    var nodePool = __instance.factory.powerSystem.nodePool;
                    var genPool = __instance.factory.powerSystem.genPool;
                    var num15 = 0f;
                    var num16 = 0f;
                    var num17 = 0f;
                    var num18 = 4900f;
                    var windForcedPower = buildPreview.desc.windForcedPower;
                    for (var n = 1; n < netCursor; n++)
                    {
                        if (netPool[n] == null || netPool[n].id == 0) continue;
                        var nodes = netPool[n].nodes;
                        var count2 = nodes.Count;
                        num18 = 4900f;
                        for (var num19 = 0; num19 < count2; num19++)
                        {
                            var num20 = x - nodes[num19].x;
                            num15 = y - nodes[num19].y;
                            num16 = z - nodes[num19].z;
                            num17 = num20 * num20 + num15 * num15 + num16 * num16;
                            if (num17 < num18 && (num17 < nodes[num19].connDistance2 || num17 < num14))
                            {
                                buildPreview.nearestPowerObjId[n] = nodePool[nodes[num19].id].entityId;
                                num18 = num17;
                            }

                            if (windForcedPower && nodes[num19].genId > 0 &&
                                genPool[nodes[num19].genId].id == nodes[num19].genId &&
                                genPool[nodes[num19].genId].wind && num17 < 110.25f)
                                buildPreview.condition = EBuildCondition.WindTooClose;
                            else if (!buildPreview.desc.isPowerGen && nodes[num19].genId == 0 && num17 < 12.25f)
                                buildPreview.condition = EBuildCondition.PowerTooClose;
                            else if (num17 < 12.25f) buildPreview.condition = EBuildCondition.PowerTooClose;
                        }
                    }

                    var prebuildPool = __instance.factory.prebuildPool;
                    var prebuildCursor = __instance.factory.prebuildCursor;
                    num18 = 4900f;
                    for (var num21 = 1; num21 < prebuildCursor; num21++)
                    {
                        if (prebuildPool[num21].id != num21 || prebuildPool[num21].protoId < 2199 ||
                            prebuildPool[num21].protoId > 2299) continue;
                        var num22 = x - prebuildPool[num21].pos.x;
                        num15 = y - prebuildPool[num21].pos.y;
                        num16 = z - prebuildPool[num21].pos.z;
                        num17 = num22 * num22 + num15 * num15 + num16 * num16;
                        if (!(num17 < num18)) continue;
                        var itemProto2 = LDB.items.Select(prebuildPool[num21].protoId);
                        if (itemProto2 != null && itemProto2.prefabDesc.isPowerNode)
                        {
                            if (num17 < itemProto2.prefabDesc.powerConnectDistance *
                                itemProto2.prefabDesc.powerConnectDistance || num17 < num14)
                            {
                                buildPreview.nearestPowerObjId[0] = -num21;
                                num18 = num17;
                            }

                            if (windForcedPower && itemProto2.prefabDesc.windForcedPower && num17 < 110.25f)
                                buildPreview.condition = EBuildCondition.WindTooClose;
                            else if (!buildPreview.desc.isPowerGen && !itemProto2.prefabDesc.isPowerGen &&
                                     num17 < 12.25f)
                                buildPreview.condition = EBuildCondition.PowerTooClose;
                            else if (num17 < 12.25f) buildPreview.condition = EBuildCondition.PowerTooClose;
                        }
                    }
                }

                if (buildPreview.desc.isCollectStation)
                {
                    if (__instance.planet == null || __instance.planet.gasItems == null ||
                        __instance.planet.gasItems.Length == 0)
                    {
                        buildPreview.condition = EBuildCondition.OutOfReach;
                        continue;
                    }

                    for (var num23 = 0; num23 < __instance.planet.gasItems.Length; num23++)
                    {
                        var num24 = 0.0;
                        if (buildPreview.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
                            num24 = 1.0 - buildPreview.desc.workEnergyPerTick / (buildPreview.desc.stationCollectSpeed *
                                __instance.planet.gasTotalHeat * 0.016666666666666666);
                        if (num24 <= 0.0) buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
                    }

                    var y2 = __instance.cursorTarget.y;
                    if (y2 > 0.1f || y2 < -0.1f)
                    {
                        buildPreview.condition = EBuildCondition.BuildInEquator;
                        continue;
                    }
                }

                if (buildPreview.desc.isStation)
                {
                    var stationPool = __instance.factory.transport.stationPool;
                    var stationCursor = __instance.factory.transport.stationCursor;
                    var prebuildPool2 = __instance.factory.prebuildPool;
                    var prebuildCursor2 = __instance.factory.prebuildCursor;
                    var entityPool = __instance.factory.entityPool;
                    var num25 = 225f;
                    var num26 = 841f;
                    var num27 = 14297f;
                    num26 = buildPreview.desc.isCollectStation ? num27 : num26;
                    for (var num28 = 1; num28 < stationCursor; num28++)
                        if (stationPool[num28] != null && stationPool[num28].id == num28)
                        {
                            var num29 = stationPool[num28].isStellar || buildPreview.desc.isStellarStation
                                ? num26
                                : num25;
                            if ((entityPool[stationPool[num28].entityId].pos - vector).sqrMagnitude < num29)
                                buildPreview.condition = EBuildCondition.TowerTooClose;
                        }

                    for (var num30 = 1; num30 < prebuildCursor2; num30++)
                    {
                        if (prebuildPool2[num30].id != num30) continue;
                        var itemProto3 = LDB.items.Select(prebuildPool2[num30].protoId);
                        if (itemProto3 != null && itemProto3.prefabDesc.isStation)
                        {
                            var num31 = itemProto3.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation
                                ? num26
                                : num25;
                            var num32 = vector.x - prebuildPool2[num30].pos.x;
                            var num33 = vector.y - prebuildPool2[num30].pos.y;
                            var num34 = vector.z - prebuildPool2[num30].pos.z;
                            if (num32 * num32 + num33 * num33 + num34 * num34 < num31)
                                buildPreview.condition = EBuildCondition.TowerTooClose;
                        }
                    }
                }

                if (!buildPreview.desc.isInserter &&
                    vector.magnitude - __instance.planet.realRadius + buildPreview.desc.cullingHeight > 4.9f &&
                    !buildPreview.desc.isEjector)
                {
                    var ejectorPool = __instance.factory.factorySystem.ejectorPool;
                    var ejectorCursor = __instance.factory.factorySystem.ejectorCursor;
                    var prebuildPool3 = __instance.factory.prebuildPool;
                    var prebuildCursor3 = __instance.factory.prebuildCursor;
                    var entityPool2 = __instance.factory.entityPool;
                    var ext = buildPreview.desc.buildCollider.ext;
                    var num35 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
                    var num36 = 7.2f + num35;
                    for (var num37 = 1; num37 < ejectorCursor; num37++)
                        if (ejectorPool[num37].id == num37 &&
                            (entityPool2[ejectorPool[num37].entityId].pos - vector).sqrMagnitude < num36 * num36)
                            buildPreview.condition = EBuildCondition.EjectorTooClose;
                    for (var num38 = 1; num38 < prebuildCursor3; num38++)
                    {
                        if (prebuildPool3[num38].id != num38) continue;
                        var itemProto4 = LDB.items.Select(prebuildPool3[num38].protoId);
                        if (itemProto4 != null && itemProto4.prefabDesc.isEjector)
                        {
                            var num39 = vector.x - prebuildPool3[num38].pos.x;
                            var num40 = vector.y - prebuildPool3[num38].pos.y;
                            var num41 = vector.z - prebuildPool3[num38].pos.z;
                            if (num39 * num39 + num40 * num40 + num41 * num41 < num36 * num36)
                                buildPreview.condition = EBuildCondition.EjectorTooClose;
                        }
                    }
                }

                if (buildPreview.desc.isEjector)
                {
                    __instance.GetOverlappedObjectsNonAlloc(vector, 12f, 14.5f);
                    for (var num42 = 0; num42 < BuildTool._overlappedCount; num42++)
                    {
                        var prefabDesc = __instance.GetPrefabDesc(BuildTool._overlappedIds[num42]);
                        var position = __instance.GetObjectPose(BuildTool._overlappedIds[num42]).position;
                        if (position.magnitude - __instance.planet.realRadius + prefabDesc.cullingHeight > 4.9f)
                        {
                            var num43 = vector.x - position.x;
                            var num44 = vector.y - position.y;
                            var num45 = vector.z - position.z;
                            var num46 = num43 * num43 + num44 * num44 + num45 * num45;
                            var ext2 = prefabDesc.buildCollider.ext;
                            var num47 = Mathf.Sqrt(ext2.x * ext2.x + ext2.z * ext2.z);
                            var num48 = 7.2f + num47;
                            if (prefabDesc.isEjector) num48 = 10.6f;
                            if (num46 < num48 * num48) buildPreview.condition = EBuildCondition.BlockTooClose;
                        }
                    }
                }

                if ((!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0) && !buildPreview.desc.isInserter)
                {
                    // Log("Multilevel");
                    RaycastHit hitInfo;
                    for (var num49 = 0; num49 < buildPreview.desc.landPoints.Length; num49++)
                    {
                        var vector10 = buildPreview.desc.landPoints[num49];
                        vector10.y = 0f;
                        var origin = vector + quaternion * vector10;
                        var normalized = origin.normalized;
                        origin += normalized * 3f;
                        var direction = -normalized;
                        var num50 = 0f;
                        var num51 = 0f;
                        if (isGas)
                        {
                            var vector11 = __instance.cursorTarget.normalized *
                                           Mathf.Min(__instance.planet.realRadius * 0.025f, 20f);
                            origin -= vector11;
                        }

                        if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704,
                            QueryTriggerInteraction.Collide))
                        {
                            num50 = hitInfo.distance;
                            if (hitInfo.point.magnitude - __instance.factory.planet.realRadius < -0.3f)
                            {
                                // Warn("Failed 1");
                                buildPreview.condition = EBuildCondition.NeedGround;
                                continue;
                            }

                            num51 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16,
                                QueryTriggerInteraction.Collide)
                                ? 1000f
                                : hitInfo.distance;
                            if (num50 - num51 > 0.27f)
                                // Warn("Failed 2");

                                buildPreview.condition = EBuildCondition.NeedGround;
                        }
                        else
                        {
                            // Warn("Failed 3");

                            buildPreview.condition = EBuildCondition.NeedGround;
                        }
                    }

                    for (var num52 = 0; num52 < buildPreview.desc.waterPoints.Length; num52++)
                    {
                        if (__instance.factory.planet.waterItemId <= 0)
                        {
                            buildPreview.condition = EBuildCondition.NeedWater;
                            continue;
                        }

                        var vector12 = buildPreview.desc.waterPoints[num52];
                        vector12.y = 0f;
                        var origin2 = vector + quaternion * vector12;
                        var normalized2 = origin2.normalized;
                        origin2 += normalized2 * 3f;
                        var direction2 = -normalized2;
                        var num53 = 0f;
                        var num54 = 0f;
                        num53 = !Physics.Raycast(new Ray(origin2, direction2), out hitInfo, 5f, 8704,
                            QueryTriggerInteraction.Collide)
                            ? 1000f
                            : hitInfo.distance;
                        if (Physics.Raycast(new Ray(origin2, direction2), out hitInfo, 5f, 16,
                            QueryTriggerInteraction.Collide))
                        {
                            num54 = hitInfo.distance;
                            if (num53 - num54 <= 0.27f) buildPreview.condition = EBuildCondition.NeedWater;
                        }
                        else
                        {
                            buildPreview.condition = EBuildCondition.NeedWater;
                        }
                    }
                }

                if (buildPreview.desc.isInserter && buildPreview.condition == EBuildCondition.Ok)
                {
                    // Log("ok");
                    var flag5 = __instance.ObjectIsBelt(buildPreview.inputObjId) ||
                                buildPreview.input != null && buildPreview.input.desc.isBelt;
                    var flag6 = __instance.ObjectIsBelt(buildPreview.outputObjId) ||
                                buildPreview.output != null && buildPreview.output.desc.isBelt;
                    var zero = Vector3.zero;
                    var vector13 = buildPreview.output == null
                        ? __instance.GetObjectPose(buildPreview.outputObjId).position
                        : buildPreview.output.lpos;
                    var vector14 = buildPreview.input == null
                        ? __instance.GetObjectPose(buildPreview.inputObjId).position
                        : buildPreview.input.lpos;
                    zero = flag5 && !flag6 ? vector13 : !(!flag5 && flag6) ? (vector13 + vector14) * 0.5f : vector14;
                    var num55 = __instance.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(zero, buildPreview.lpos,
                        buildPreview.lpos2);
                    var num56 = num55;
                    var magnitude = forward2.magnitude;
                    var num57 = 5.5f;
                    var num58 = 0.6f;
                    var num59 = 3.499f;
                    var num60 = 0.88f;
                    if (flag5 && flag6)
                    {
                        num58 = 0.4f;
                        num57 = 5f;
                        num59 = 3.2f;
                        num60 = 0.8f;
                    }
                    else if (!flag5 && !flag6)
                    {
                        num58 = 0.9f;
                        num57 = 7.5f;
                        num59 = 3.799f;
                        num60 = 1.451f;
                        num56 -= 0.3f;
                    }

                    if (magnitude > num57)
                    {
                        buildPreview.condition = EBuildCondition.TooFar;
                        continue;
                    }

                    if (magnitude < num58)
                    {
                        buildPreview.condition = EBuildCondition.TooClose;
                        continue;
                    }

                    if (num55 > num59)
                    {
                        buildPreview.condition = EBuildCondition.TooFar;
                        continue;
                    }

                    if (num55 < num60)
                    {
                        buildPreview.condition = EBuildCondition.TooClose;
                        continue;
                    }

                    var oneParameter = Mathf.RoundToInt(Mathf.Clamp(num56, 1f, 3f));
                    buildPreview.SetOneParameter(oneParameter);
                }
            }

            var flag7 = true;
            for (var num61 = 0; num61 < __instance.buildPreviews.Count; num61++)
            {
                var buildPreview3 = __instance.buildPreviews[num61];
                if (buildPreview3.condition != 0 && buildPreview3.condition != EBuildCondition.NeedConn)
                {
                    flag7 = false;
                    __instance.actionBuild.model.cursorState = -1;
                    __instance.actionBuild.model.cursorText = buildPreview3.conditionText;
                    if (buildPreview3.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
                        __instance.actionBuild.model.cursorText += "垂直建造可升级".Translate();
                    break;
                }
            }

            if (flag7)
            {
                __instance.actionBuild.model.cursorState = 0;
                __instance.actionBuild.model.cursorText = "点击鼠标建造".Translate();
            }

            if (!flag7 && !VFInput.onGUI) UICursor.SetCursor(ECursor.Ban);
            __result = flag7;
            return false;
        }
    }
}