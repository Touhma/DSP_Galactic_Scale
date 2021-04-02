using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using System;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(PlayerAction_Build))]
    public class PatchOnPlayerAction_BuildCheck // - innominata
    {
        [HarmonyPostfix]
        [HarmonyPatch("CheckBuildConditions")]
        static bool BuildConditionsCheck(bool __result,
            PlayerAction_Build __instance, ref string ___cursorText,
            ref bool ___cursorWarning, ref bool ___cursorValid,
            ref bool ___waitConfirm, ref int[] ____tmp_ids,
            ref NearColliderLogic ___nearcdLogic,
            ref PlanetFactory ___factory,
            Pose ___previewPose
            )
        {
            if (__instance.buildPreviews.Count > 1) // Check we are building
            {
                ItemProto itemProto = LDB.items.Select((int)___factory.entityPool[__instance.buildPreviews[0].inputObjId].protoId); // Grab the prototype of the first object in the chain
                if (itemProto != null && itemProto.prefabDesc.oilMiner) // Check that we are connected to an oil miner
                {
                    if (__instance.buildPreviews[0].condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
                    {
                        __instance.buildPreviews[0].condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                        for (int i = 0; i < __instance.buildPreviews.Count(); i++) // Check the rest of the belt for errors
                        {
                            if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift)) return (bool)false; //If there's some other problem with the belt, bail out.
                        }
                        ___cursorText = "Click to build";
                        ___cursorWarning = false; // Prevent red text
                        __result = true; // Override the build condition check
                        UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    }
                }
            }

            return __result;
        }
        /*
        // The following is unmodified from original, but I've left it here because it took a couple of hours to port over, and it will probably be needed in the future. 
        [HarmonyPrefix]
        [HarmonyPatch("CheckBuildConditions")]
        public static bool CheckBuildConditions(ref PlayerAction_Build __instance, ref bool __result,
        ref List<Renderer> ___previewRenderers,
        ref List<MeshFilter> ___previewMeshFilters,
        ref Vector3 ___reformChangedPoint,
        ref bool ___reformMouseOnDown,
        ref StorageComponent ___tmpPackage,
        ref int ___tmpInhandId,
        ref int ___tmpInhandCount,
        ref int ___beltMovePath,
        ref float ___beltMovePathTime,
        ref NearColliderLogic ___nearcdLogic,
        ref PlanetPhysics ___planetPhysics,
        ref PlanetFactory ___factory,
        ref FactoryModel ___factoryModel,
        ref ConnGizmoGraph ___connGraph,
        ref ConnGizmoRenderer ___connRenderer,
        ref PlanetAuxData ___planetAux,
        ref bool ___lastIsBuildMode,
        ref int[] ___tmp_conn,
        ref List<int> ___tmp_links,
        ref HashSet<int> ___once_upgrades,
        ref int[] ____nearObjectIds,
        ref int ____nearObjectCount,
        ref int[] ____overlappedIds,
        ref int ____overlappedCount,
        ref Collider[] ____tmp_cols,
        ref int[] ____tmp_ids,
        ref Pose[] ___belt_slots,
        ref Pose[] ___emptyPoseArr,
        ref int[] ___emptyRefArr
        )
        {
            Traverse ObjectIsBelt = Traverse.Create(__instance).Method("ObjectIsBelt", new[] { typeof(int) });
            Traverse GetObjectPose = Traverse.Create(__instance).Method("GetObjectPose", new[] { typeof(int) });
            Traverse GetPrefabDesc = Traverse.Create(__instance).Method("GetPrefabDesc", new[] { typeof(int) });
            Traverse GetLocalGates = Traverse.Create(__instance).Method("GetLocalGates", new[] { typeof(int) });
            GameHistoryData history = GameMain.history;
            bool flag1 = true;
            for (int index1 = 0; index1 < __instance.buildPreviews.Count; ++index1)
            {
                BuildPreview buildPreview = __instance.buildPreviews[index1];
                bool isBelt = buildPreview.desc.isBelt;
                bool isInserter = buildPreview.desc.isInserter;
                bool flag2 = false;
                if (buildPreview.condition == EBuildCondition.Ok)
                {
                    Vector3 vector3_1 = __instance.previewPose.position + __instance.previewPose.rotation * buildPreview.lpos;
                    Vector3 vector3_2 = __instance.previewPose.position + __instance.previewPose.rotation * buildPreview.lpos2;
                    if ((double)vector3_1.sqrMagnitude < 1.0)
                        buildPreview.condition = EBuildCondition.Failure;
                    else if (isInserter && (double)vector3_2.sqrMagnitude < 1.0)
                    {
                        buildPreview.condition = EBuildCondition.Failure;
                    }
                    else
                    {
                        if (buildPreview.coverObjId == 0 || buildPreview.willCover)
                        {
                            int id = buildPreview.item.ID;
                            int count = 1;
                            if (___tmpInhandId == id && ___tmpInhandCount > 0)
                            {
                                count = 1;
                                --___tmpInhandCount;
                            }
                            else
                                ___tmpPackage.TakeTailItems(ref id, ref count);
                            if (count == 0)
                            {
                                buildPreview.condition = EBuildCondition.NotEnoughItem;
                                goto label_281;
                            }
                        }
                        Pose pose;
                        pose.position = __instance.previewPose.position + __instance.previewPose.rotation * buildPreview.lpos;
                        pose.rotation = __instance.previewPose.rotation * buildPreview.lrot;
                        if (isInserter)
                        {
                            pose.position = Vector3.Lerp(buildPreview.lpos, buildPreview.lpos2, 0.5f);
                            Vector3 forward = buildPreview.lpos2 - buildPreview.lpos;
                            if ((double)forward.sqrMagnitude < 9.99999974737875E-05)
                                forward = Maths.SphericalRotation(buildPreview.lpos, 0.0f).Forward();
                            pose.rotation = Quaternion.LookRotation(forward, buildPreview.lpos.normalized);
                            Vector3 zero = Vector3.zero;
                            float num1 = ___planetAux.mainGrid.CalcSegmentsAcross(!(ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId)) || ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId) ? (ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId) || !ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId) ? (GetObjectPose.GetValue<Pose>(buildPreview.inputObjId).position + GetObjectPose.GetValue<Pose>(buildPreview.outputObjId).position) * 0.5f : GetObjectPose.GetValue<Pose>(buildPreview.inputObjId).position) : GetObjectPose.GetValue<Pose>(buildPreview.outputObjId).position, buildPreview.lpos, buildPreview.lpos2);
                            float num2 = num1;
                            float magnitude = forward.magnitude;
                            float num3 = 5.5f;
                            float num4 = 0.6f;
                            float num5 = 3.499f;
                            float num6 = 0.88f;
                            if (ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId) && ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId))
                            {
                                num4 = 0.4f;
                                num3 = 5f;
                                num5 = 3.2f;
                                num6 = 0.8f;
                            }
                            else if (!ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId) && !ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId))
                            {
                                num4 = 0.9f;
                                num3 = 7.5f;
                                num5 = 3.799f;
                                num6 = 1.451f;
                                num2 -= 0.3f;
                            }
                            if ((double)magnitude > (double)num3)
                            {
                                buildPreview.condition = EBuildCondition.TooFar;
                                goto label_281;
                            }
                            else if ((double)magnitude < (double)num4)
                            {
                                buildPreview.condition = EBuildCondition.TooClose;
                                goto label_281;
                            }
                            else if ((double)num1 > (double)num5)
                            {
                                buildPreview.condition = EBuildCondition.TooFar;
                                goto label_281;
                            }
                            else if ((double)num1 < (double)num6)
                            {
                                buildPreview.condition = EBuildCondition.TooClose;
                                goto label_281;
                            }
                            else
                            {
                                buildPreview.refCount = Mathf.RoundToInt(Mathf.Clamp(num2, 1f, 3f));
                                buildPreview.refArr = new int[buildPreview.refCount];
                                UIRoot.instance.uiGame.inserterBuildTip.gridLen = buildPreview.refCount;
                            }
                        }
                        if (isBelt && buildPreview.outputToSlot >= 4 && ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId))
                        {
                            if (buildPreview.output != null)
                                buildPreview.output.condition = EBuildCondition.InputFull;
                            else
                                buildPreview.condition = EBuildCondition.InputFull;
                        }
                        else
                        {
                            if (buildPreview.desc.veinMiner)
                            {
                                Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                                Vector3 center = pose.position + pose.forward * -1.2f;
                                Vector3 rhs1 = -pose.forward;
                                Vector3 up = pose.up;
                                int veinsInAreaNonAlloc = ___nearcdLogic.GetVeinsInAreaNonAlloc(center, 12f, ____tmp_ids);
                                PrebuildData prebuildData = new PrebuildData();
                                prebuildData.InitRefArray(veinsInAreaNonAlloc);
                                VeinData[] veinPool = ___factory.veinPool;
                                int num1 = 0;
                                for (int index2 = 0; index2 < veinsInAreaNonAlloc; ++index2)
                                {
                                    if (____tmp_ids[index2] != 0 && veinPool[____tmp_ids[index2]].id == ____tmp_ids[index2])
                                    {
                                        if (veinPool[____tmp_ids[index2]].type != EVeinType.Oil)
                                        {
                                            Vector3 rhs2 = veinPool[____tmp_ids[index2]].pos - center;
                                            float f = Vector3.Dot(up, rhs2);
                                            rhs2 -= up * f;
                                            float sqrMagnitude = rhs2.sqrMagnitude;
                                            float num2 = Vector3.Dot(rhs2.normalized, rhs1);
                                            if ((double)sqrMagnitude <= 961.0 / 16.0 && (double)num2 >= 0.730000019073486 && (double)Mathf.Abs(f) <= 2.0)
                                                prebuildData.refArr[num1++] = ____tmp_ids[index2];
                                        }
                                    }
                                    else
                                        Assert.CannotBeReached();
                                }
                                prebuildData.refCount = num1;
                                prebuildData.ArrageRefArray();
                                buildPreview.refArr = prebuildData.refArr;
                                buildPreview.refCount = prebuildData.refCount;
                                Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                                if (prebuildData.refCount == 0)
                                {
                                    buildPreview.condition = EBuildCondition.NeedResource;
                                    goto label_281;
                                }
                            }
                            else if (buildPreview.desc.oilMiner)
                            {
                                Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                                Vector3 position = pose.position;
                                Vector3 lhs = -pose.up;
                                int veinsInAreaNonAlloc = __instance.player.planetData.physics.nearColliderLogic.GetVeinsInAreaNonAlloc(position, 10f, ____tmp_ids);
                                PrebuildData prebuildData = new PrebuildData();
                                prebuildData.InitRefArray(veinsInAreaNonAlloc);
                                VeinData[] veinPool = ___factory.veinPool;
                                int num1 = 0;
                                float num2 = 100f;
                                Vector3 pos1 = position;
                                for (int index2 = 0; index2 < veinsInAreaNonAlloc; ++index2)
                                {
                                    if (____tmp_ids[index2] != 0 && veinPool[____tmp_ids[index2]].id == ____tmp_ids[index2] && veinPool[____tmp_ids[index2]].type == EVeinType.Oil)
                                    {
                                        Vector3 pos2 = veinPool[____tmp_ids[index2]].pos;
                                        Vector3 rhs = pos2 - position;
                                        float num3 = Vector3.Dot(lhs, rhs);
                                        float sqrMagnitude = (rhs - lhs * num3).sqrMagnitude;
                                        if ((double)sqrMagnitude < (double)num2)
                                        {
                                            num2 = sqrMagnitude;
                                            num1 = ____tmp_ids[index2];
                                            pos1 = pos2;
                                        }
                                    }
                                }
                                if (num1 != 0)
                                {
                                    prebuildData.refArr[0] = num1;
                                    prebuildData.refCount = 1;
                                    prebuildData.ArrageRefArray();
                                    buildPreview.refArr = prebuildData.refArr;
                                    buildPreview.refCount = prebuildData.refCount;
                                    Vector3 pos2 = ___factory.planet.aux.Snap(pos1, true, ___factory.planet.levelized);
                                    __instance.previewPose.position = pos2;
                                    __instance.previewPose.rotation = Maths.SphericalRotation(pos2, __instance.yaw);
                                    pose.position = __instance.previewPose.position + __instance.previewPose.rotation * buildPreview.lpos;
                                    pose.rotation = __instance.previewPose.rotation * buildPreview.lrot;
                                    Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                                }
                                else
                                {
                                    buildPreview.condition = EBuildCondition.NeedResource;
                                    goto label_281;
                                }
                            }
                            if (buildPreview.desc.isTank || buildPreview.desc.isStorage || (buildPreview.desc.isLab || buildPreview.desc.isSplitter))
                            {
                                int num1 = !buildPreview.desc.isLab ? history.storageLevel : history.labLevel;
                                int num2 = !buildPreview.desc.isLab ? 8 : 15;
                                int num3 = 0;
                                bool isOutput;
                                int otherObjId;
                                int otherSlot;
                                for (___factory.ReadObjectConn(buildPreview.inputObjId, 14, out isOutput, out otherObjId, out otherSlot); otherObjId != 0; ___factory.ReadObjectConn(otherObjId, 14, out isOutput, out otherObjId, out otherSlot))
                                    ++num3;
                                if (num3 >= num1 - 1)
                                {
                                    flag2 = num1 >= num2;
                                    buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
                                    goto label_281;
                                }
                            }
                            PlanetData planetData = __instance.player.planetData;
                            Vector3 vector3_3 = __instance.player.position;
                            if (planetData.type == EPlanetType.Gas)
                                vector3_3 = vector3_3.normalized * planetData.realRadius;
                            if ((double)(pose.position - vector3_3).sqrMagnitude > (double)__instance.player.mecha.buildArea * (double)__instance.player.mecha.buildArea)
                            {
                                buildPreview.condition = EBuildCondition.OutOfReach;
                            }
                            else
                            {
                                float num1 = history.buildMaxHeight + 0.5f + __instance.player.planetData.realRadius;
                                float num2 = planetData.type == EPlanetType.Gas ? num1 + planetData.realRadius * 0.025f : num1;
                                if ((double)pose.position.sqrMagnitude > (double)num2 * (double)num2)
                                {
                                    buildPreview.condition = EBuildCondition.OutOfReach;
                                }
                                else
                                {
                                    if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
                                    {
                                        if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length != buildPreview.nearestPowerObjId.Length)
                                            buildPreview.nearestPowerObjId = new int[___factory.powerSystem.netCursor];
                                        Array.Clear((Array)buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
                                        float num3 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
                                        float x = pose.position.x;
                                        float y = pose.position.y;
                                        float z = pose.position.z;
                                        int netCursor = ___factory.powerSystem.netCursor;
                                        PowerNetwork[] netPool = ___factory.powerSystem.netPool;
                                        PowerNodeComponent[] nodePool = ___factory.powerSystem.nodePool;
                                        PowerGeneratorComponent[] genPool = ___factory.powerSystem.genPool;
                                        bool windForcedPower = buildPreview.desc.windForcedPower;
                                        for (int index2 = 1; index2 < netCursor; ++index2)
                                        {
                                            if (netPool[index2] != null && netPool[index2].id != 0)
                                            {
                                                List<PowerNetworkStructures.Node> nodes = netPool[index2].nodes;
                                                int count = nodes.Count;
                                                float num4 = 4900f;
                                                for (int index3 = 0; index3 < count; ++index3)
                                                {
                                                    float num5 = x - nodes[index3].x;
                                                    float num6 = y - nodes[index3].y;
                                                    float num7 = z - nodes[index3].z;
                                                    float num8 = (float)((double)num5 * (double)num5 + (double)num6 * (double)num6 + (double)num7 * (double)num7);
                                                    if ((double)num8 < (double)num4 && ((double)num8 < (double)nodes[index3].connDistance2 || (double)num8 < (double)num3))
                                                    {
                                                        buildPreview.nearestPowerObjId[index2] = nodePool[nodes[index3].id].entityId;
                                                        num4 = num8;
                                                    }
                                                    if (windForcedPower && nodes[index3].genId > 0 && (genPool[nodes[index3].genId].id == nodes[index3].genId && genPool[nodes[index3].genId].wind) && (double)num8 < 110.25)
                                                    {
                                                        buildPreview.condition = EBuildCondition.WindTooClose;
                                                        goto label_281;
                                                    }
                                                    else if (!buildPreview.desc.isPowerGen && nodes[index3].genId == 0 && (double)num8 < 12.25)
                                                    {
                                                        buildPreview.condition = EBuildCondition.PowerTooClose;
                                                        goto label_281;
                                                    }
                                                    else if ((double)num8 < 12.25)
                                                    {
                                                        buildPreview.condition = EBuildCondition.PowerTooClose;
                                                        goto label_281;
                                                    }
                                                }
                                            }
                                        }
                                        PrebuildData[] prebuildPool = ___factory.prebuildPool;
                                        int prebuildCursor = ___factory.prebuildCursor;
                                        float num9 = 4900f;
                                        for (int index2 = 1; index2 < prebuildCursor; ++index2)
                                        {
                                            if (prebuildPool[index2].id == index2 && prebuildPool[index2].protoId >= (short)2199 && prebuildPool[index2].protoId <= (short)2299)
                                            {
                                                float num4 = x - prebuildPool[index2].pos.x;
                                                float num5 = y - prebuildPool[index2].pos.y;
                                                float num6 = z - prebuildPool[index2].pos.z;
                                                float num7 = (float)((double)num4 * (double)num4 + (double)num5 * (double)num5 + (double)num6 * (double)num6);
                                                if ((double)num7 < (double)num9)
                                                {
                                                    ItemProto itemProto = LDB.items.Select((int)prebuildPool[index2].protoId);
                                                    if (itemProto != null && itemProto.prefabDesc.isPowerNode)
                                                    {
                                                        if ((double)num7 < (double)itemProto.prefabDesc.powerConnectDistance * (double)itemProto.prefabDesc.powerConnectDistance || (double)num7 < (double)num3)
                                                        {
                                                            buildPreview.nearestPowerObjId[0] = -index2;
                                                            num9 = num7;
                                                        }
                                                        if (windForcedPower && itemProto.prefabDesc.windForcedPower && (double)num7 < 110.25)
                                                        {
                                                            buildPreview.condition = EBuildCondition.WindTooClose;
                                                            goto label_281;
                                                        }
                                                        else if (!buildPreview.desc.isPowerGen && !itemProto.prefabDesc.isPowerGen && (double)num7 < 12.25)
                                                        {
                                                            buildPreview.condition = EBuildCondition.PowerTooClose;
                                                            goto label_281;
                                                        }
                                                        else if ((double)num7 < 12.25)
                                                        {
                                                            buildPreview.condition = EBuildCondition.PowerTooClose;
                                                            goto label_281;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (buildPreview.desc.isCollectStation)
                                    {
                                        for (int index2 = 0; index2 < planetData.gasItems.Length; ++index2)
                                        {
                                            double num3 = 0.0;
                                            if ((double)buildPreview.desc.stationCollectSpeed * planetData.gasTotalHeat != 0.0)
                                                num3 = 1.0 - (double)buildPreview.desc.workEnergyPerTick / ((double)buildPreview.desc.stationCollectSpeed * planetData.gasTotalHeat * (1.0 / 60.0));
                                            if (num3 <= 0.0)
                                            {
                                                buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
                                                goto label_281;
                                            }
                                        }
                                        float y = __instance.cursorTarget.y;
                                        if ((double)y > 0.100000001490116 || (double)y < -0.100000001490116)
                                        {
                                            buildPreview.condition = EBuildCondition.BuildInEquator;
                                            goto label_281;
                                        }
                                    }
                                    if (buildPreview.desc.isStation)
                                    {
                                        StationComponent[] stationPool = ___factory.transport.stationPool;
                                        int stationCursor = ___factory.transport.stationCursor;
                                        PrebuildData[] prebuildPool = ___factory.prebuildPool;
                                        int prebuildCursor = ___factory.prebuildCursor;
                                        EntityData[] entityPool = ___factory.entityPool;
                                        float num3 = 225f;
                                        float num4 = 841f;
                                        float num5 = 14297f;
                                        float num6 = !buildPreview.desc.isCollectStation ? num4 : num5;
                                        for (int index2 = 1; index2 < stationCursor; ++index2)
                                        {
                                            if (stationPool[index2] != null && stationPool[index2].id == index2)
                                            {
                                                float num7 = stationPool[index2].isStellar || buildPreview.desc.isStellarStation ? num6 : num3;
                                                if ((double)(entityPool[stationPool[index2].entityId].pos - pose.position).sqrMagnitude < (double)num7)
                                                {
                                                    buildPreview.condition = EBuildCondition.TowerTooClose;
                                                    goto label_281;
                                                }
                                            }
                                        }
                                        for (int index2 = 1; index2 < prebuildCursor; ++index2)
                                        {
                                            if (prebuildPool[index2].id == index2)
                                            {
                                                ItemProto itemProto = LDB.items.Select((int)prebuildPool[index2].protoId);
                                                if (itemProto != null && itemProto.prefabDesc.isStation)
                                                {
                                                    float num7 = itemProto.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation ? num6 : num3;
                                                    float num8 = pose.position.x - prebuildPool[index2].pos.x;
                                                    float num9 = pose.position.y - prebuildPool[index2].pos.y;
                                                    float num10 = pose.position.z - prebuildPool[index2].pos.z;
                                                    if ((double)num8 * (double)num8 + (double)num9 * (double)num9 + (double)num10 * (double)num10 < (double)num7)
                                                    {
                                                        buildPreview.condition = EBuildCondition.TowerTooClose;
                                                        goto label_281;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if ((double)(pose.position.magnitude - planetData.realRadius + buildPreview.desc.cullingHeight) > 4.90000009536743 && !buildPreview.desc.isEjector)
                                    {
                                        EjectorComponent[] ejectorPool = ___factory.factorySystem.ejectorPool;
                                        int ejectorCursor = ___factory.factorySystem.ejectorCursor;
                                        PrebuildData[] prebuildPool = ___factory.prebuildPool;
                                        int prebuildCursor = ___factory.prebuildCursor;
                                        EntityData[] entityPool = ___factory.entityPool;
                                        Vector3 ext = buildPreview.desc.buildCollider.ext;
                                        float num3 = 7.2f + Mathf.Sqrt((float)((double)ext.x * (double)ext.x + (double)ext.z * (double)ext.z));
                                        for (int index2 = 1; index2 < ejectorCursor; ++index2)
                                        {
                                            if (ejectorPool[index2].id == index2 && (double)(entityPool[ejectorPool[index2].entityId].pos - pose.position).sqrMagnitude < (double)num3 * (double)num3)
                                            {
                                                buildPreview.condition = EBuildCondition.EjectorTooClose;
                                                goto label_281;
                                            }
                                        }
                                        for (int index2 = 1; index2 < prebuildCursor; ++index2)
                                        {
                                            if (prebuildPool[index2].id == index2)
                                            {
                                                ItemProto itemProto = LDB.items.Select((int)prebuildPool[index2].protoId);
                                                if (itemProto != null && itemProto.prefabDesc.isEjector)
                                                {
                                                    float num4 = pose.position.x - prebuildPool[index2].pos.x;
                                                    float num5 = pose.position.y - prebuildPool[index2].pos.y;
                                                    float num6 = pose.position.z - prebuildPool[index2].pos.z;
                                                    if ((double)num4 * (double)num4 + (double)num5 * (double)num5 + (double)num6 * (double)num6 < (double)num3 * (double)num3)
                                                    {
                                                        buildPreview.condition = EBuildCondition.EjectorTooClose;
                                                        goto label_281;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (buildPreview.desc.isEjector)
                                    {
                                        __instance.GetOverlappedObjectsNonAlloc(pose.position, 12f, 14.5f);
                                        for (int index2 = 0; index2 < ____overlappedCount; ++index2)
                                        {
                                            PrefabDesc prefabDesc = GetPrefabDesc.GetValue<PrefabDesc>(____overlappedIds[index2]);
                                            if ((double)(GetObjectPose.GetValue<Pose>(____overlappedIds[index2]).position.magnitude - planetData.realRadius + prefabDesc.cullingHeight) > 4.90000009536743)
                                            {
                                                Vector3 position = GetObjectPose.GetValue<Pose>(____overlappedIds[index2]).position;
                                                float num3 = pose.position.x - position.x;
                                                float num4 = pose.position.y - position.y;
                                                float num5 = pose.position.z - position.z;
                                                float num6 = (float)((double)num3 * (double)num3 + (double)num4 * (double)num4 + (double)num5 * (double)num5);
                                                Vector3 ext = prefabDesc.buildCollider.ext;
                                                float num7 = 7.2f + Mathf.Sqrt((float)((double)ext.x * (double)ext.x + (double)ext.z * (double)ext.z));
                                                if (prefabDesc.isEjector)
                                                    num7 = 10.6f;
                                                if ((double)num6 < (double)num7 * (double)num7)
                                                {
                                                    buildPreview.condition = EBuildCondition.BlockTooClose;
                                                    goto label_281;
                                                }
                                            }
                                        }
                                    }
                                    if (!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0)
                                    {
                                        RaycastHit hitInfo;
                                        for (int index2 = 0; index2 < buildPreview.desc.landPoints.Length; ++index2)
                                        {
                                            Vector3 landPoint = buildPreview.desc.landPoints[index2];
                                            landPoint.y = 0.0f;
                                            Vector3 vector3_4 = pose.position + pose.rotation * landPoint;
                                            Vector3 normalized = vector3_4.normalized;
                                            Vector3 origin = vector3_4 + normalized * 3f;
                                            Vector3 direction = -normalized;
                                            if (__instance.player.planetData.type == EPlanetType.Gas)
                                            {
                                                Vector3 vector3_5 = __instance.cursorTarget.normalized * __instance.player.planetData.realRadius * 0.025f;
                                                origin -= vector3_5;
                                            }
                                            if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide))
                                            {
                                                float distance = hitInfo.distance;
                                                if ((double)hitInfo.point.magnitude - (double)___factory.planet.realRadius < -0.300000011920929)
                                                {
                                                    buildPreview.condition = EBuildCondition.NeedGround;
                                                    goto label_281;
                                                }
                                                else
                                                {
                                                    float num3 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
                                                    if ((double)distance - (double)num3 > 0.270000010728836)
                                                    {
                                                        buildPreview.condition = EBuildCondition.NeedGround;
                                                        goto label_281;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                buildPreview.condition = EBuildCondition.NeedGround;
                                                goto label_281;
                                            }
                                        }
                                        for (int index2 = 0; index2 < buildPreview.desc.waterPoints.Length; ++index2)
                                        {
                                            if (___factory.planet.waterItemId <= 0)
                                            {
                                                buildPreview.condition = EBuildCondition.NeedWater;
                                                goto label_281;
                                            }
                                            else
                                            {
                                                Vector3 waterPoint = buildPreview.desc.waterPoints[index2];
                                                waterPoint.y = 0.0f;
                                                Vector3 vector3_4 = pose.position + pose.rotation * waterPoint;
                                                Vector3 normalized = vector3_4.normalized;
                                                Vector3 origin = vector3_4 + normalized * 3f;
                                                Vector3 direction = -normalized;
                                                float num3 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
                                                if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide))
                                                {
                                                    float distance = hitInfo.distance;
                                                    if ((double)num3 - (double)distance <= 0.270000010728836)
                                                    {
                                                        buildPreview.condition = EBuildCondition.NeedWater;
                                                        goto label_281;
                                                    }
                                                }
                                                else
                                                {
                                                    buildPreview.condition = EBuildCondition.NeedWater;
                                                    goto label_281;
                                                }
                                            }
                                        }
                                    }
                                    if (buildPreview.desc.hasBuildCollider)
                                    {
                                        ColliderData[] buildColliders = buildPreview.desc.buildColliders;
                                        for (int index2 = 0; index2 < buildColliders.Length; ++index2)
                                        {
                                            ColliderData buildCollider = buildPreview.desc.buildColliders[index2];
                                            buildCollider.pos = pose.position + pose.rotation * buildCollider.pos;
                                            buildCollider.q = pose.rotation * buildCollider.q;
                                            if (buildPreview.item.BuildMode == 1)
                                            {
                                                int layermask = 165888;
                                                if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner)
                                                    layermask = 163840;
                                                if (Physics.CheckBox(buildCollider.pos, buildCollider.ext, buildCollider.q, layermask, QueryTriggerInteraction.Collide))
                                                {
                                                    buildPreview.condition = EBuildCondition.Collide;
                                                    goto label_281;
                                                }
                                            }
                                            else if (isBelt)
                                            {
                                                Vector3 pos = buildPreview.lpos + buildPreview.lpos.normalized * 0.3f;
                                                if (!buildPreview.ignoreCollider)
                                                {
                                                    __instance.GetOverlappedObjectsNonAlloc(pos, 0.34f, 3f);
                                                    if (____overlappedCount > 0)
                                                    {
                                                        buildPreview.condition = EBuildCondition.Collide;
                                                        goto label_281;
                                                    }
                                                }
                                                if (__instance.buildPreviews.Count != 1 || buildPreview.inputObjId <= 0)
                                                {
                                                    __instance.GetOverlappedVeinsNonAlloc(pos, 0.6f, 3f);
                                                    if (____overlappedCount > 0)
                                                    {
                                                        buildPreview.condition = EBuildCondition.Collide;
                                                        goto label_281;
                                                    }
                                                }
                                                bool flag3 = false;
                                                Vector3 A = Vector3.zero;
                                                if (buildPreview.input != null)
                                                {
                                                    flag3 = true;
                                                    A = buildPreview.input.lpos;
                                                }
                                                if (buildPreview.inputObjId != 0)
                                                {
                                                    flag3 = true;
                                                    A = GetObjectPose.GetValue<Pose>(buildPreview.inputObjId).position;
                                                }
                                                bool flag4 = false;
                                                Vector3 B = Vector3.zero;
                                                if (buildPreview.output != null)
                                                {
                                                    flag4 = true;
                                                    B = buildPreview.output.lpos;
                                                }
                                                if (buildPreview.outputObjId != 0)
                                                {
                                                    flag4 = true;
                                                    B = GetObjectPose.GetValue<Pose>(buildPreview.outputObjId).position;
                                                }
                                                if (flag3 && flag4 && (double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, B) < 1.0)
                                                    buildPreview.condition = EBuildCondition.TooSkew;
                                            }
                                            else if (isInserter)
                                            {
                                                buildCollider = buildPreview.desc.buildColliders[index2];
                                                buildCollider.ext = new Vector3(buildCollider.ext.x, buildCollider.ext.y, (float)((double)Vector3.Distance(buildPreview.lpos2, buildPreview.lpos) * 0.5 + (double)buildCollider.ext.z - 0.5));
                                                if (ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId))
                                                {
                                                    buildCollider.pos.z -= 0.4f;
                                                    buildCollider.ext.z += 0.4f;
                                                }
                                                if (ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId))
                                                {
                                                    buildCollider.pos.z += 0.4f;
                                                    buildCollider.ext.z += 0.4f;
                                                }
                                                if ((double)buildCollider.ext.z < 0.100000001490116)
                                                    buildCollider.ext.z = 0.1f;
                                                buildCollider.pos = pose.position + pose.rotation * buildCollider.pos;
                                                buildCollider.q = pose.rotation * buildCollider.q;
                                                buildCollider.DebugDraw();
                                                int mask = 165888;
                                                int num3 = Physics.OverlapBoxNonAlloc(buildCollider.pos, buildCollider.ext, ____tmp_cols, buildCollider.q, mask, QueryTriggerInteraction.Collide);
                                                if (num3 > 0)
                                                {
                                                    PlanetPhysics physics = __instance.player.planetData.physics;
                                                    for (int index3 = 0; index3 < num3; ++index3)
                                                    {
                                                        ColliderData cd;
                                                        physics.GetColliderData(____tmp_cols[index3], out cd);
                                                        if (cd.objId != 0 && cd.usage == EColliderUsage.Build && (cd.objId != __instance.startObjId || cd.objType != EObjectType.Entity) && (cd.objId != __instance.castObjId || cd.objType != EObjectType.Entity))
                                                        {
                                                            cd.DebugDraw();
                                                            buildPreview.condition = EBuildCondition.Collide;
                                                            goto label_281;
                                                        }
                                                    }
                                                }
                                            }
                                            if (buildPreview.desc.veinMiner && Physics.CheckBox(buildCollider.pos, buildCollider.ext, buildCollider.q, 2048, QueryTriggerInteraction.Collide))
                                            {
                                                buildPreview.condition = EBuildCondition.Collide;
                                                goto label_281;
                                            }
                                        }
                                    }
                                    if (isBelt)
                                    {
                                        bool flag3 = false;
                                        Vector3 vector3_4 = Vector3.zero;
                                        if (buildPreview.input != null)
                                        {
                                            flag3 = true;
                                            vector3_4 = buildPreview.input.lpos;
                                        }
                                        if (buildPreview.inputObjId != 0)
                                        {
                                            flag3 = true;
                                            Pose objectPose = GetObjectPose.GetValue<Pose>(buildPreview.inputObjId);
                                            vector3_4 = objectPose.position;
                                            if (!ObjectIsBelt.GetValue<bool>(buildPreview.inputObjId))
                                            {
                                                Pose localGate = GetLocalGates.GetValue<Pose[]>(buildPreview.inputObjId)[buildPreview.inputFromSlot];
                                                vector3_4 = objectPose.position + objectPose.rotation * localGate.position;
                                            }
                                        }
                                        if (index1 > 0 && __instance.buildPreviews[index1 - 1].output == buildPreview)
                                        {
                                            flag3 = true;
                                            vector3_4 = __instance.buildPreviews[index1 - 1].lpos;
                                        }
                                        bool flag4 = false;
                                        Vector3 vector3_5 = Vector3.zero;
                                        if (buildPreview.output != null)
                                        {
                                            flag4 = true;
                                            vector3_5 = buildPreview.output.lpos;
                                        }
                                        if (buildPreview.outputObjId != 0)
                                        {
                                            flag4 = true;
                                            Pose objectPose = GetObjectPose.GetValue<Pose>(buildPreview.outputObjId);
                                            vector3_5 = objectPose.position;
                                            if (!ObjectIsBelt.GetValue<bool>(buildPreview.outputObjId))
                                            {
                                                Pose localGate = GetLocalGates.GetValue<Pose[]>(buildPreview.outputObjId)[buildPreview.outputToSlot];
                                                vector3_5 = objectPose.position + objectPose.rotation * localGate.position;
                                            }
                                        }
                                        float num3 = 3.141593f;
                                        if (flag3 && flag4)
                                        {
                                            num3 = Maths.SphericalAngleAOBInRAD(buildPreview.lpos, vector3_4, vector3_5);
                                            if ((double)num3 < 1.0)
                                            {
                                                buildPreview.condition = EBuildCondition.TooBend;
                                                goto label_281;
                                            }
                                        }
                                        float b = 0.0f;
                                        if (flag4)
                                        {
                                            b = Mathf.Abs(Maths.SphericalSlopeRatio(buildPreview.lpos, vector3_5));
                                            if ((double)b > 0.75)
                                            {
                                                buildPreview.condition = EBuildCondition.TooSteep;
                                                goto label_281;
                                            }
                                        }
                                        if (flag3)
                                        {
                                            b = Mathf.Max(Mathf.Abs(Maths.SphericalSlopeRatio(vector3_4, buildPreview.lpos)), b);
                                            if ((double)b > 0.75)
                                            {
                                                buildPreview.condition = EBuildCondition.TooSteep;
                                                goto label_281;
                                            }
                                        }
                                        if (flag4 && index1 < __instance.buildPreviews.Count - 1)
                                        {
                                            Vector3 lhs = vector3_5 - buildPreview.lpos;
                                            Vector3 normalized = buildPreview.lpos.normalized;
                                            Vector3 vector3_6 = lhs - Vector3.Dot(lhs, normalized) * normalized;
                                            if ((double)(buildPreview.lpos - vector3_5).magnitude < 0.400000005960464)
                                            {
                                                buildPreview.condition = EBuildCondition.TooClose;
                                                goto label_281;
                                            }
                                        }
                                        if ((double)num3 < 2.5 && (double)b > 0.100000001490116)
                                        {
                                            buildPreview.condition = EBuildCondition.TooBendToLift;
                                        }
                                        else
                                        {
                                            bool flag5 = ObjectIsBelt.GetValue<bool>(buildPreview.coverObjId);
                                            if ((double)b > 0.100000001490116)
                                            {
                                                bool isOutput;
                                                int otherObjId;
                                                int otherSlot;
                                                if (flag3 && index1 == __instance.buildPreviews.Count - 1)
                                                {
                                                    if (!flag5)
                                                    {
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                        goto label_281;
                                                    }
                                                    else
                                                    {
                                                        Vector3 zero = Vector3.zero;
                                                        Vector3 A = vector3_4;
                                                        ___factory.ReadObjectConn(buildPreview.coverObjId, 0, out isOutput, out otherObjId, out otherSlot);
                                                        if (otherObjId != 0)
                                                        {
                                                            Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                            if ((double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position) < 2.5)
                                                            {
                                                                buildPreview.condition = EBuildCondition.JointCannotLift;
                                                                goto label_281;
                                                            }
                                                            else
                                                            {
                                                                ___factory.ReadObjectConn(buildPreview.coverObjId, 1, out isOutput, out otherObjId, out otherSlot);
                                                                if (otherObjId != 0 && (double)Maths.SphericalSlopeRatio(GetObjectPose.GetValue<Pose>(otherObjId).position, buildPreview.lpos) >= 0.100000001490116)
                                                                {
                                                                    buildPreview.condition = EBuildCondition.JointCannotLift;
                                                                    goto label_281;
                                                                }
                                                                else
                                                                {
                                                                    ___factory.ReadObjectConn(buildPreview.coverObjId, 2, out isOutput, out otherObjId, out otherSlot);
                                                                    if (otherObjId != 0 && (double)Maths.SphericalSlopeRatio(GetObjectPose.GetValue<Pose>(otherObjId).position, buildPreview.lpos) >= 0.100000001490116)
                                                                    {
                                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                                        goto label_281;
                                                                    }
                                                                    else
                                                                    {
                                                                        ___factory.ReadObjectConn(buildPreview.coverObjId, 3, out isOutput, out otherObjId, out otherSlot);
                                                                        if (otherObjId != 0 && (double)Maths.SphericalSlopeRatio(GetObjectPose.GetValue<Pose>(otherObjId).position, buildPreview.lpos) >= 0.100000001490116)
                                                                        {
                                                                            buildPreview.condition = EBuildCondition.JointCannotLift;
                                                                            goto label_281;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            buildPreview.condition = EBuildCondition.JointCannotLift;
                                                            goto label_281;
                                                        }
                                                    }
                                                }
                                                if (flag4 && index1 == 0)
                                                {
                                                    if (!flag5)
                                                    {
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                        goto label_281;
                                                    }
                                                    else
                                                    {
                                                        Vector3 zero = Vector3.zero;
                                                        Vector3 A = vector3_5;
                                                        bool flag6 = false;
                                                        bool flag7 = false;
                                                        ___factory.ReadObjectConn(buildPreview.coverObjId, 1, out isOutput, out otherObjId, out otherSlot);
                                                        if (otherObjId != 0)
                                                        {
                                                            Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                            float num4 = Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position);
                                                            flag6 = true;
                                                            if ((double)num4 >= 2.5)
                                                                flag7 = true;
                                                        }
                                                        ___factory.ReadObjectConn(buildPreview.coverObjId, 2, out isOutput, out otherObjId, out otherSlot);
                                                        if (otherObjId != 0)
                                                        {
                                                            Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                            float num4 = Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position);
                                                            flag6 = true;
                                                            if ((double)num4 >= 2.5)
                                                                flag7 = true;
                                                        }
                                                        ___factory.ReadObjectConn(buildPreview.coverObjId, 3, out isOutput, out otherObjId, out otherSlot);
                                                        if (otherObjId != 0)
                                                        {
                                                            Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                            float num4 = Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position);
                                                            flag6 = true;
                                                            if ((double)num4 >= 2.5)
                                                                flag7 = true;
                                                        }
                                                        if (!flag7)
                                                        {
                                                            buildPreview.condition = !flag6 ? EBuildCondition.JointCannotLift : EBuildCondition.TooBendToLift;
                                                            goto label_281;
                                                        }
                                                    }
                                                }
                                            }
                                            if (index1 == __instance.buildPreviews.Count - 1 && flag3 && flag5)
                                            {
                                                Vector3 zero = Vector3.zero;
                                                Vector3 A = vector3_4;
                                                bool isOutput;
                                                int otherObjId;
                                                int otherSlot;
                                                ___factory.ReadObjectConn(buildPreview.coverObjId, 0, out isOutput, out otherObjId, out otherSlot);
                                                if (otherObjId != 0 && otherObjId != __instance.startObjId)
                                                {
                                                    Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                    if ((double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position) < 1.0)
                                                    {
                                                        buildPreview.condition = EBuildCondition.InputConflict;
                                                        goto label_281;
                                                    }
                                                    else if ((double)Mathf.Abs(Maths.SphericalSlopeRatio(position, buildPreview.lpos)) >= 0.100000001490116)
                                                    {
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                        goto label_281;
                                                    }
                                                }
                                                ___factory.ReadObjectConn(buildPreview.coverObjId, 1, out isOutput, out otherObjId, out otherSlot);
                                                if (otherObjId != 0 && otherObjId != __instance.startObjId)
                                                {
                                                    Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                    if ((double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position) < 1.0)
                                                    {
                                                        buildPreview.condition = EBuildCondition.InputConflict;
                                                        goto label_281;
                                                    }
                                                    else if ((double)Mathf.Abs(Maths.SphericalSlopeRatio(position, buildPreview.lpos)) >= 0.100000001490116)
                                                    {
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                        goto label_281;
                                                    }
                                                }
                                                ___factory.ReadObjectConn(buildPreview.coverObjId, 2, out isOutput, out otherObjId, out otherSlot);
                                                if (otherObjId != 0 && otherObjId != __instance.startObjId)
                                                {
                                                    Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                    if ((double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position) < 1.0)
                                                    {
                                                        buildPreview.condition = EBuildCondition.InputConflict;
                                                        goto label_281;
                                                    }
                                                    else if ((double)Mathf.Abs(Maths.SphericalSlopeRatio(position, buildPreview.lpos)) >= 0.100000001490116)
                                                    {
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                        goto label_281;
                                                    }
                                                }
                                                ___factory.ReadObjectConn(buildPreview.coverObjId, 3, out isOutput, out otherObjId, out otherSlot);
                                                if (otherObjId != 0 && otherObjId != __instance.startObjId)
                                                {
                                                    Vector3 position = GetObjectPose.GetValue<Pose>(otherObjId).position;
                                                    if ((double)Maths.SphericalAngleAOBInRAD(buildPreview.lpos, A, position) < 1.0)
                                                        buildPreview.condition = EBuildCondition.InputConflict;
                                                    else if ((double)Mathf.Abs(Maths.SphericalSlopeRatio(position, buildPreview.lpos)) >= 0.100000001490116)
                                                        buildPreview.condition = EBuildCondition.JointCannotLift;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            label_281:
                if (buildPreview.condition != EBuildCondition.Ok)
                {
                    flag1 = false;
                    if (!__instance.cursorWarning)
                    {
                        __instance.cursorWarning = true;
                        __instance.cursorText = buildPreview.conditionText;
                        if (buildPreview.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag2)
                            __instance.cursorText += "垂直建造可升级".Translate();
                    }
                }
            }
            if (flag1 && __instance.waitConfirm)
                __instance.cursorText = "点击鼠标建造".Translate();
            if (!flag1 && !VFInput.onGUI)
                UICursor.SetCursor(ECursor.Ban);
            __result = flag1;
            return false;
        }*/
    }
}