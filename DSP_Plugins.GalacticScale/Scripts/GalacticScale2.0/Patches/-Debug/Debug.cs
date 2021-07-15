using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;
using NGPT;
using PowerNetworkStructures;
namespace GalacticScale
    
{
    public class PatchOnWhatever
    {
        [HarmonyPrefix, HarmonyPatch(typeof(BuildTool_Click), "DeterminePreviews")]
		public static bool DeterminePreviews(ref BuildTool_Click __instance)
		{
			__instance.waitForConfirm = false;
			if (__instance.cursorValid)
			{
				if (VFInput._switchModelStyle.onDown)
				{
					if (__instance.handItem.ModelCount > 1)
					{
						__instance.modelOffset++;
						BuildingParameters.template.SetEmpty();
						__instance.actionBuild.NotifyTemplateChange();
					}
					else if (__instance.isDragging)
					{
						if (__instance.tabgapDir)
						{
							if (__instance.gap < 0.3f)
							{
								__instance.gap = 0.3333333f;
							}
							else if (__instance.gap < 0.4f)
							{
								__instance.gap = 0.5f;
							}
							else if (__instance.gap < 0.6f)
							{
								__instance.gap = 1f;
							}
							else if (__instance.gap < 3.5f)
							{
								__instance.gap += 1f;
							}
							else
							{
								__instance.tabgapDir = false;
							}
						}
						else if (__instance.gap > 1.5f)
						{
							__instance.gap -= 1f;
						}
						else if (__instance.gap > 0.6f)
						{
							__instance.gap = 0.5f;
						}
						else if (__instance.gap > 0.4f)
						{
							__instance.gap = 0.3333333f;
						}
						else if (__instance.gap > 0.3f)
						{
							__instance.gap = 0f;
						}
						else
						{
							__instance.tabgapDir = true;
						}
					}
				}
				bool flag = !__instance.multiLevelCovering && __instance.handPrefabDesc.dragBuild;
				if (VFInput._buildConfirm.onDown && __instance.controller.cmd.stage != 1)
				{
					__instance.controller.cmd.stage = 1;
					__instance.startGroundPosSnapped = __instance.castGroundPosSnapped;
					if (flag)
					{
						__instance.isDragging = true;
					}
				}
				if (__instance.controller.cmd.stage == 0)
				{
					__instance.startGroundPosSnapped = __instance.castGroundPosSnapped;
					__instance.isDragging = false;
				}
				__instance.waitForConfirm = (__instance.controller.cmd.stage == 1);
				if (__instance.isDragging)
				{
					if (VFInput._cursorPlusKey.onDown)
					{
						if (__instance.gap < 0.3f)
						{
							__instance.gap = 0.3333333f;
						}
						else if (__instance.gap < 0.4f)
						{
							__instance.gap = 0.5f;
						}
						else if (__instance.gap < 0.6f)
						{
							__instance.gap = 1f;
						}
						else if (__instance.gap < 3.5f)
						{
							__instance.gap += 1f;
						}
					}
					if (VFInput._cursorMinusKey.onDown)
					{
						if (__instance.gap > 1.5f)
						{
							__instance.gap -= 1f;
						}
						else if (__instance.gap > 0.6f)
						{
							__instance.gap = 0.5f;
						}
						else if (__instance.gap > 0.4f)
						{
							__instance.gap = 0.3333333f;
						}
						else if (__instance.gap > 0.3f)
						{
							__instance.gap = 0f;
						}
					}
				}
				if (VFInput._ignoreGrid && __instance.handPrefabDesc.minerType == EMinerType.Vein)
				{
					if (VFInput._rotate)
					{
						__instance.yaw += 3f;
						__instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
					}
					if (VFInput._counterRotate)
					{
						__instance.yaw -= 3f;
						__instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
					}
				}
				else
				{
					if (VFInput._rotate.onDown)
					{
						__instance.yaw += 90f;
						__instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
						__instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
					}
					if (VFInput._counterRotate.onDown)
					{
						__instance.yaw -= 90f;
						__instance.yaw = Mathf.Repeat(__instance.yaw, 360f);
						__instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
					}
					if (__instance.handPrefabDesc.minerType != EMinerType.Vein)
					{
						__instance.yaw = Mathf.Round(__instance.yaw / 90f) * 90f;
					}
				}
				Array.Clear(__instance.dotsSnapped, 0, __instance.dotsSnapped.Length);
				int num = 1;
				if (__instance.isDragging)
				{
					num = __instance.planet.aux.SnapDotsNonAlloc(__instance.startGroundPosSnapped, __instance.castGroundPosSnapped, __instance.handPrefabDesc.dragBuildDist, __instance.yaw, __instance.gap, __instance.dotsSnapped);
				}
				else
				{
					__instance.dotsSnapped[0] = __instance.cursorTarget;
				}
				int num2 = 1;
				List<BuildPreview> templatePreviews = __instance.actionBuild.templatePreviews;
				bool flag2 = templatePreviews.Count > 0;
				if (flag2)
				{
					num2 = templatePreviews.Count;
				}
				int num3 = num * num2;
				while (__instance.buildPreviews.Count < num3)
				{
					__instance.buildPreviews.Add(new BuildPreview());
				}
				while (__instance.buildPreviews.Count > num3)
				{
					__instance.buildPreviews.RemoveAt(__instance.buildPreviews.Count - 1);
				}
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						BuildPreview buildPreview = __instance.buildPreviews[i * num2 + j];
						BuildPreview buildPreview2 = flag2 ? templatePreviews[j] : null;
						if (buildPreview2 == null)
						{
							buildPreview.ResetAll();
							buildPreview.item = __instance.handItem;
							buildPreview.desc = __instance.handPrefabDesc;
							buildPreview.needModel = (__instance.handPrefabDesc.lodCount > 0 && __instance.handPrefabDesc.lodMeshes[0] != null);
						}
						else
						{
							buildPreview.Clone(buildPreview2);
						}
						if (j == 0)
						{
							float magnitude = buildPreview.desc.buildCollider.ext.magnitude;
							buildPreview.genNearColliderArea2 = (magnitude + 4f) * (magnitude + 4f);
						}
						Vector3 point = (buildPreview2 == null) ? Vector3.zero : buildPreview2.lpos;
						Quaternion rhs = (buildPreview2 == null) ? Quaternion.identity : buildPreview2.lrot;
						Vector3 point2 = (buildPreview2 == null) ? Vector3.zero : buildPreview2.lpos2;
						Quaternion rhs2 = (buildPreview2 == null) ? Quaternion.identity : buildPreview2.lrot2;
						Vector3 vector;
						Quaternion quaternion;
						if (__instance.multiLevelCovering)
						{
							if (j == 0)
							{
								buildPreview.input = null;
								buildPreview.inputObjId = __instance.castObjectId;
								buildPreview.inputFromSlot = 15;
								buildPreview.inputToSlot = 14;
								buildPreview.inputOffset = 0;
							}
							Pose objectPose = __instance.GetObjectPose(__instance.castObjectId);
							vector = objectPose.position + objectPose.rotation * __instance.handPrefabDesc.lapJoint;
							quaternion = (__instance.handPrefabDesc.multiLevelAllowRotate ? Maths.SphericalRotation(vector, __instance.yaw) : objectPose.rotation);
						}
						else
						{
							vector = __instance.dotsSnapped[i];
							if (__instance.planet != null && __instance.planet.type == EPlanetType.Gas)
							{
								Vector3 b = vector.normalized * Mathf.Min(__instance.planet.realRadius * 0.025f, 20f);// __instance.planet.realRadius * 0.025f;
								//GS2.Log(b.ToString());
								//if (__instance.planet.realRadius < 1) b = vector.normalized * 0.001f;
								vector += b;
							}
							quaternion = Maths.SphericalRotation(vector, __instance.yaw);
						}
						buildPreview.lpos = vector + quaternion * point;
						buildPreview.lrot = quaternion * rhs;
						buildPreview.lpos2 = vector + quaternion * point2;
						buildPreview.lrot2 = quaternion * rhs2;
						if (buildPreview.desc.isInserter)
						{
							int num4 = (buildPreview.output != null) ? buildPreview.outputToSlot : ((buildPreview.input != null) ? buildPreview.inputFromSlot : -1);
							if (num4 >= 0)
							{
								BuildPreview buildPreview3 = __instance.buildPreviews[i * num2];
								Vector3 vector2 = buildPreview.lpos2 - buildPreview.lpos;
								float num5 = vector2.magnitude;
								vector2.Normalize();
								float num6 = __instance.actionBuild.planetAux.activeGrid.CalcLocalGridSize(buildPreview3.lpos, vector2);
								Pose pose = buildPreview3.desc.slotPoses[num4];
								Vector3 forward = pose.forward;
								float num7 = (Mathf.Abs(forward.x) > Mathf.Abs(forward.z)) ? Mathf.Abs(pose.position.x) : Mathf.Abs(pose.position.z);
								num5 = Mathf.Round((num7 + num6 * (num5 + 0.0001f)) / num6) * num6 - num7;
								if (buildPreview.output != null)
								{
									buildPreview.lpos = -vector2 * num5 + buildPreview.lpos2;
								}
								else
								{
									buildPreview.lpos2 = vector2 * num5 + buildPreview.lpos;
								}
							}
						}
					}
					for (int k = 0; k < num2; k++)
					{
						BuildPreview buildPreview4 = __instance.buildPreviews[i * num2 + k];
						for (int l = 0; l < templatePreviews.Count; l++)
						{
							if (buildPreview4.output == templatePreviews[l])
							{
								buildPreview4.output = __instance.buildPreviews[i * num2 + l];
							}
							if (buildPreview4.input == templatePreviews[l])
							{
								buildPreview4.input = __instance.buildPreviews[i * num2 + l];
							}
						}
					}
					for (int m = 0; m < num2; m++)
					{
						BuildPreview buildPreview5 = __instance.buildPreviews[i * num2 + m];
						if (buildPreview5.desc.isInserter)
						{
							__instance.MatchInserter(buildPreview5);
						}
					}
				}
				return false;
			}
			__instance.buildPreviews.Clear();
			return false;
		}
		[HarmonyPrefix, HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
		public static bool CheckBuildConditions(ref bool __result, ref BuildTool_Click __instance, ref PlanetData ___planet, ref PlanetFactory ___factory)
		{
			if (__instance.buildPreviews.Count == 0)
			{
				__result = false; return false;

			}
			GameHistoryData history = __instance.actionBuild.history;
			bool flag = false;
			int num = 1;
			List<BuildPreview> templatePreviews = __instance.actionBuild.templatePreviews;
			if (templatePreviews.Count > 0)
			{
				num = templatePreviews.Count;
			}
			for (int i = 0; i < __instance.buildPreviews.Count; i++)
			{
				BuildPreview buildPreview = __instance.buildPreviews[i];
				BuildPreview buildPreview2 = __instance.buildPreviews[i / num * num];
				if (buildPreview.condition != 0)
				{
					continue;
				}
				Vector3 vector = buildPreview.lpos;
				Quaternion quaternion = buildPreview.lrot;
				Vector3 lpos = buildPreview.lpos2;
				_ = buildPreview.lrot2;
				Pose pose = new Pose(buildPreview.lpos, buildPreview.lrot);
				Pose pose2 = new Pose(buildPreview.lpos2, buildPreview.lrot2);
				Vector3 forward = pose.forward;
				_ = pose2.forward;
				Vector3 up = pose.up;
				Vector3 vector2 = Vector3.Lerp(vector, lpos, 0.5f);
				Vector3 forward2 = lpos - vector;
				if (forward2.sqrMagnitude < 0.0001f)
				{
					forward2 = Maths.SphericalRotation(vector, 0f).Forward();
				}
				Quaternion quaternion2 = Quaternion.LookRotation(forward2, vector2.normalized);
				bool flag2 = ___planet != null && ___planet.type == EPlanetType.Gas;
				if (vector.sqrMagnitude < 1f)
				{
                    buildPreview.condition = EBuildCondition.Failure;
					GS2.Warn($"VecSqr{vector.sqrMagnitude}");
					continue;
				}
				bool flag3 = buildPreview.desc.minerType == EMinerType.None && !buildPreview.desc.isBelt && !buildPreview.desc.isSplitter && (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen || buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger) && !buildPreview.desc.isStation && !buildPreview.desc.isSilo && !buildPreview.desc.multiLevel;
				if (buildPreview.desc.veinMiner)
				{
					Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
					Vector3 vector3 = vector + forward * -1.2f;
					Vector3 rhs = -forward;
					Vector3 vector4 = up;
					int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector3, 12f, BuildTool._tmp_ids);
					PrebuildData prebuildData = default(PrebuildData);
					prebuildData.InitParametersArray(veinsInAreaNonAlloc);
					VeinData[] veinPool = ___factory.veinPool;
					int paramCount = 0;
					for (int j = 0; j < veinsInAreaNonAlloc; j++)
					{
						if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
						{
							if (veinPool[BuildTool._tmp_ids[j]].type != EVeinType.Oil)
							{
								Vector3 rhs2 = veinPool[BuildTool._tmp_ids[j]].pos - vector3;
								float num2 = Vector3.Dot(vector4, rhs2);
								rhs2 -= vector4 * num2;
								float sqrMagnitude = rhs2.sqrMagnitude;
								float num3 = Vector3.Dot(rhs2.normalized, rhs);
								if (!(sqrMagnitude > 60.0625f) && !(num3 < 0.73f) && !(Mathf.Abs(num2) > 2f))
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
						continue;
					}
				}
				else if (buildPreview.desc.oilMiner)
				{
					Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
					Vector3 vector5 = vector;
					Vector3 vector6 = -up;
					int veinsInAreaNonAlloc2 = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector5, 10f, BuildTool._tmp_ids);
					PrebuildData prebuildData2 = default(PrebuildData);
					prebuildData2.InitParametersArray(veinsInAreaNonAlloc2);
					VeinData[] veinPool2 = ___factory.veinPool;
					int num4 = 0;
					float num5 = 100f;
					Vector3 pos = vector5;
					for (int k = 0; k < veinsInAreaNonAlloc2; k++)
					{
						if (BuildTool._tmp_ids[k] != 0 && veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k] && veinPool2[BuildTool._tmp_ids[k]].type == EVeinType.Oil)
						{
							Vector3 pos2 = veinPool2[BuildTool._tmp_ids[k]].pos;
							Vector3 vector7 = pos2 - vector5;
							float num6 = Vector3.Dot(vector6, vector7);
							float sqrMagnitude2 = (vector7 - vector6 * num6).sqrMagnitude;
							if (sqrMagnitude2 < num5)
							{
								num5 = sqrMagnitude2;
								num4 = BuildTool._tmp_ids[k];
								pos = pos2;
							}
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
					Vector3 vector8 = ___factory.planet.aux.Snap(pos, onTerrain: true);
					vector = (pose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector8)));
					quaternion = (pose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector8, __instance.yaw))));
					forward = pose.forward;
					up = pose.up;
					Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
				}
				if (buildPreview.desc.isTank || buildPreview.desc.isStorage || buildPreview.desc.isLab || buildPreview.desc.isSplitter)
				{
					int num7 = (buildPreview.desc.isLab ? history.labLevel : history.storageLevel);
					int num8 = (buildPreview.desc.isLab ? 15 : 8);
					int num9 = 0;
					___factory.ReadObjectConn(buildPreview.inputObjId, 14, out var isOutput, out var otherObjId, out var otherSlot);
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
				Vector3 vector9 = __instance.player.position;
				float num10 = __instance.player.mecha.buildArea * __instance.player.mecha.buildArea;
				if (flag2) // gas
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
					float num11 = history.buildMaxHeight + 0.5f + ___planet.realRadius * (flag2 ? 1.025f : 1f);
					if (vector.sqrMagnitude > num11 * num11)
					{
						buildPreview.condition = EBuildCondition.OutOfReach;
						continue;
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
							colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y, Vector3.Distance(lpos, vector) * 0.5f + colliderData.ext.z - 0.5f);
							if (__instance.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt))
							{
								colliderData.pos.z -= 0.35f;
								colliderData.ext.z += 0.35f;
							}
							else if (buildPreview.inputObjId == 0 && buildPreview.input == null)
							{
								colliderData.pos.z -= 0.35f;
								colliderData.ext.z += 0.35f;
							}
							if (__instance.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt))
							{
								colliderData.pos.z += 0.35f;
								colliderData.ext.z += 0.35f;
							}
							else if (buildPreview.outputObjId == 0 && buildPreview.output == null)
							{
								colliderData.pos.z += 0.35f;
								colliderData.ext.z += 0.35f;
							}
							if (colliderData.ext.z < 0.1f)
							{
								colliderData.ext.z = 0.1f;
							}
							colliderData.pos = vector2 + quaternion2 * colliderData.pos;
							colliderData.q = quaternion2 * colliderData.q;
							colliderData.DebugDraw();
						}
						else
						{
							colliderData.pos = vector + quaternion * colliderData.pos;
							colliderData.q = quaternion * colliderData.q;
						}
						int mask = 428032;
						if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner)
						{
							mask = 425984;
						}
						Array.Clear(BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
						int num12 = Physics.OverlapBoxNonAlloc(colliderData.pos, colliderData.ext, BuildTool._tmp_cols, colliderData.q, mask, QueryTriggerInteraction.Collide);
						if (num12 > 0)
						{
							bool flag4 = false;
							PlanetPhysics physics = __instance.player.planetData.physics;
							for (int m = 0; m < num12 && buildPreview.coverObjId == 0; m++)
							{
								ColliderData cd;
								bool colliderData2 = physics.GetColliderData(BuildTool._tmp_cols[m], out cd);
								int num13 = 0;
								if (colliderData2 && cd.isForBuild)
								{
									if (cd.objType == EObjectType.Entity)
									{
										num13 = cd.objId;
									}
									else if (cd.objType == EObjectType.Prebuild)
									{
										num13 = -cd.objId;
									}
								}
								Collider collider = BuildTool._tmp_cols[m];
								if (collider.gameObject.layer == 18)
								{
									BuildPreviewModel component = collider.GetComponent<BuildPreviewModel>();
									if ((component != null && component.index == buildPreview.previewIndex) || (buildPreview.desc.isInserter && !component.buildPreview.desc.isInserter) || (!buildPreview.desc.isInserter && component.buildPreview.desc.isInserter))
									{
										continue;
									}
								}
								else if (buildPreview.desc.isInserter && num13 != 0 && (num13 == buildPreview.inputObjId || num13 == buildPreview.outputObjId || num13 == buildPreview2.coverObjId))
								{
									continue;
								}
								flag4 = true;
								if (!flag3 || num13 == 0)
								{
									continue;
								}
								ItemProto itemProto = __instance.GetItemProto(num13);
								if (!buildPreview.item.IsSimilar(itemProto))
								{
									continue;
								}
								Pose objectPose = __instance.GetObjectPose(num13);
								Pose objectPose2 = __instance.GetObjectPose2(num13);
								if ((double)(objectPose.position - buildPreview.lpos).sqrMagnitude < 0.01 && (double)(objectPose2.position - buildPreview.lpos2).sqrMagnitude < 0.01 && ((double)(objectPose.forward - forward).sqrMagnitude < 1E-06 || buildPreview.desc.isInserter))
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
						if (buildPreview.desc.veinMiner && Physics.CheckBox(colliderData.pos, colliderData.ext, colliderData.q, 2048, QueryTriggerInteraction.Collide))
						{
							buildPreview.condition = EBuildCondition.Collide;
							break;
						}
					}
					if (buildPreview.condition != 0)
					{
						continue;
					}
				}
				if (buildPreview2.coverObjId != 0 && buildPreview.desc.isInserter)
				{
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
					int itemId = buildPreview.item.ID;
					int count = 1;
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
				if (buildPreview.coverObjId != 0)
				{
					continue;
				}
				if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
				{
					if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length != buildPreview.nearestPowerObjId.Length)
					{
						buildPreview.nearestPowerObjId = new int[__instance.factory.powerSystem.netCursor];
					}
					Array.Clear(buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
					float num14 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
					float x = vector.x;
					float y = vector.y;
					float z = vector.z;
					int netCursor = __instance.factory.powerSystem.netCursor;
					PowerNetwork[] netPool = __instance.factory.powerSystem.netPool;
					PowerNodeComponent[] nodePool = __instance.factory.powerSystem.nodePool;
					PowerGeneratorComponent[] genPool = __instance.factory.powerSystem.genPool;
					float num15 = 0f;
					float num16 = 0f;
					float num17 = 0f;
					float num18 = 4900f;
					bool windForcedPower = buildPreview.desc.windForcedPower;
					for (int n = 1; n < netCursor; n++)
					{
						if (netPool[n] == null || netPool[n].id == 0)
						{
							continue;
						}
						List<Node> nodes = netPool[n].nodes;
						int count2 = nodes.Count;
						num18 = 4900f;
						for (int num19 = 0; num19 < count2; num19++)
						{
							float num20 = x - nodes[num19].x;
							num15 = y - nodes[num19].y;
							num16 = z - nodes[num19].z;
							num17 = num20 * num20 + num15 * num15 + num16 * num16;
							if (num17 < num18 && (num17 < nodes[num19].connDistance2 || num17 < num14))
							{
								buildPreview.nearestPowerObjId[n] = nodePool[nodes[num19].id].entityId;
								num18 = num17;
							}
							if (windForcedPower && nodes[num19].genId > 0 && genPool[nodes[num19].genId].id == nodes[num19].genId && genPool[nodes[num19].genId].wind && num17 < 110.25f)
							{
								buildPreview.condition = EBuildCondition.WindTooClose;
							}
							else if (!buildPreview.desc.isPowerGen && nodes[num19].genId == 0 && num17 < 12.25f)
							{
								buildPreview.condition = EBuildCondition.PowerTooClose;
							}
							else if (num17 < 12.25f)
							{
								buildPreview.condition = EBuildCondition.PowerTooClose;
							}
						}
					}
					PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
					int prebuildCursor = __instance.factory.prebuildCursor;
					num18 = 4900f;
					for (int num21 = 1; num21 < prebuildCursor; num21++)
					{
						if (prebuildPool[num21].id != num21 || prebuildPool[num21].protoId < 2199 || prebuildPool[num21].protoId > 2299)
						{
							continue;
						}
						float num22 = x - prebuildPool[num21].pos.x;
						num15 = y - prebuildPool[num21].pos.y;
						num16 = z - prebuildPool[num21].pos.z;
						num17 = num22 * num22 + num15 * num15 + num16 * num16;
						if (!(num17 < num18))
						{
							continue;
						}
						ItemProto itemProto2 = LDB.items.Select(prebuildPool[num21].protoId);
						if (itemProto2 != null && itemProto2.prefabDesc.isPowerNode)
						{
							if (num17 < itemProto2.prefabDesc.powerConnectDistance * itemProto2.prefabDesc.powerConnectDistance || num17 < num14)
							{
								buildPreview.nearestPowerObjId[0] = -num21;
								num18 = num17;
							}
							if (windForcedPower && itemProto2.prefabDesc.windForcedPower && num17 < 110.25f)
							{
								buildPreview.condition = EBuildCondition.WindTooClose;
							}
							else if (!buildPreview.desc.isPowerGen && !itemProto2.prefabDesc.isPowerGen && num17 < 12.25f)
							{
								buildPreview.condition = EBuildCondition.PowerTooClose;
							}
							else if (num17 < 12.25f)
							{
								buildPreview.condition = EBuildCondition.PowerTooClose;
							}
						}
					}
				}
				if (buildPreview.desc.isCollectStation)
				{
					if (__instance.planet == null || __instance.planet.gasItems == null || __instance.planet.gasItems.Length == 0)
					{
						buildPreview.condition = EBuildCondition.OutOfReach;
						continue;
					}
					for (int num23 = 0; num23 < __instance.planet.gasItems.Length; num23++)
					{
						double num24 = 0.0;
						if ((double)buildPreview.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
						{
							num24 = 1.0 - (double)buildPreview.desc.workEnergyPerTick / ((double)buildPreview.desc.stationCollectSpeed * __instance.planet.gasTotalHeat * 0.016666666666666666);
						}
						if (num24 <= 0.0)
						{
							buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
						}
					}
					float y2 = __instance.cursorTarget.y;
					if (y2 > 0.1f || y2 < -0.1f)
					{
						buildPreview.condition = EBuildCondition.BuildInEquator;
						continue;
					}
				}
				if (buildPreview.desc.isStation)
				{
					StationComponent[] stationPool = __instance.factory.transport.stationPool;
					int stationCursor = __instance.factory.transport.stationCursor;
					PrebuildData[] prebuildPool2 = __instance.factory.prebuildPool;
					int prebuildCursor2 = __instance.factory.prebuildCursor;
					EntityData[] entityPool = __instance.factory.entityPool;
					float num25 = 225f;
					float num26 = 841f;
					float num27 = 14297f;
					num26 = (buildPreview.desc.isCollectStation ? num27 : num26);
					for (int num28 = 1; num28 < stationCursor; num28++)
					{
						if (stationPool[num28] != null && stationPool[num28].id == num28)
						{
							float num29 = ((stationPool[num28].isStellar || buildPreview.desc.isStellarStation) ? num26 : num25);
							if ((entityPool[stationPool[num28].entityId].pos - vector).sqrMagnitude < num29)
							{
								buildPreview.condition = EBuildCondition.TowerTooClose;
							}
						}
					}
					for (int num30 = 1; num30 < prebuildCursor2; num30++)
					{
						if (prebuildPool2[num30].id != num30)
						{
							continue;
						}
						ItemProto itemProto3 = LDB.items.Select(prebuildPool2[num30].protoId);
						if (itemProto3 != null && itemProto3.prefabDesc.isStation)
						{
							float num31 = ((itemProto3.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation) ? num26 : num25);
							float num32 = vector.x - prebuildPool2[num30].pos.x;
							float num33 = vector.y - prebuildPool2[num30].pos.y;
							float num34 = vector.z - prebuildPool2[num30].pos.z;
							if (num32 * num32 + num33 * num33 + num34 * num34 < num31)
							{
								buildPreview.condition = EBuildCondition.TowerTooClose;
							}
						}
					}
				}
				if (!buildPreview.desc.isInserter && vector.magnitude - __instance.planet.realRadius + buildPreview.desc.cullingHeight > 4.9f && !buildPreview.desc.isEjector)
				{
					EjectorComponent[] ejectorPool = __instance.factory.factorySystem.ejectorPool;
					int ejectorCursor = __instance.factory.factorySystem.ejectorCursor;
					PrebuildData[] prebuildPool3 = __instance.factory.prebuildPool;
					int prebuildCursor3 = __instance.factory.prebuildCursor;
					EntityData[] entityPool2 = __instance.factory.entityPool;
					Vector3 ext = buildPreview.desc.buildCollider.ext;
					float num35 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
					float num36 = 7.2f + num35;
					for (int num37 = 1; num37 < ejectorCursor; num37++)
					{
						if (ejectorPool[num37].id == num37 && (entityPool2[ejectorPool[num37].entityId].pos - vector).sqrMagnitude < num36 * num36)
						{
							buildPreview.condition = EBuildCondition.EjectorTooClose;
						}
					}
					for (int num38 = 1; num38 < prebuildCursor3; num38++)
					{
						if (prebuildPool3[num38].id != num38)
						{
							continue;
						}
						ItemProto itemProto4 = LDB.items.Select(prebuildPool3[num38].protoId);
						if (itemProto4 != null && itemProto4.prefabDesc.isEjector)
						{
							float num39 = vector.x - prebuildPool3[num38].pos.x;
							float num40 = vector.y - prebuildPool3[num38].pos.y;
							float num41 = vector.z - prebuildPool3[num38].pos.z;
							if (num39 * num39 + num40 * num40 + num41 * num41 < num36 * num36)
							{
								buildPreview.condition = EBuildCondition.EjectorTooClose;
							}
						}
					}
				}
				if (buildPreview.desc.isEjector)
				{
					__instance.GetOverlappedObjectsNonAlloc(vector, 12f, 14.5f);
					for (int num42 = 0; num42 < BuildTool._overlappedCount; num42++)
					{
						PrefabDesc prefabDesc = __instance.GetPrefabDesc(BuildTool._overlappedIds[num42]);
						Vector3 position = __instance.GetObjectPose(BuildTool._overlappedIds[num42]).position;
						if (position.magnitude - __instance.planet.realRadius + prefabDesc.cullingHeight > 4.9f)
						{
							float num43 = vector.x - position.x;
							float num44 = vector.y - position.y;
							float num45 = vector.z - position.z;
							float num46 = num43 * num43 + num44 * num44 + num45 * num45;
							Vector3 ext2 = prefabDesc.buildCollider.ext;
							float num47 = Mathf.Sqrt(ext2.x * ext2.x + ext2.z * ext2.z);
							float num48 = 7.2f + num47;
							if (prefabDesc.isEjector)
							{
								num48 = 10.6f;
							}
							if (num46 < num48 * num48)
							{
								buildPreview.condition = EBuildCondition.BlockTooClose;
							}
						}
					}
				}
				if ((!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0) && !buildPreview.desc.isInserter)
				{
					RaycastHit hitInfo;
					for (int num49 = 0; num49 < buildPreview.desc.landPoints.Length; num49++)
					{
						Vector3 vector10 = buildPreview.desc.landPoints[num49];
						vector10.y = 0f;
						Vector3 origin = vector + quaternion * vector10;
						Vector3 normalized = origin.normalized;
						origin += normalized * 3f;
						Vector3 direction = -normalized;
						float num50 = 0f;
						float num51 = 0f;
						if (flag2)
						{
							Vector3 vector11 = __instance.cursorTarget.normalized * Mathf.Min(__instance.planet.realRadius * 0.025f, 20f);
							origin -= vector11;
						}
						if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide))
						{
							num50 = hitInfo.distance;
							if (hitInfo.point.magnitude - __instance.factory.planet.realRadius < -0.3f)
							{
								GS2.Warn("Failed 1");
								buildPreview.condition = EBuildCondition.NeedGround;
								continue;
							}
							num51 = ((!Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide)) ? 1000f : hitInfo.distance);
							if (num50 - num51 > 0.27f)
							{
								GS2.Warn("Failed 2");

								buildPreview.condition = EBuildCondition.NeedGround;
							}
						}
						else
						{
							GS2.Warn("Failed 3");

							buildPreview.condition = EBuildCondition.NeedGround;
						}
					}
					for (int num52 = 0; num52 < buildPreview.desc.waterPoints.Length; num52++)
					{
						if (__instance.factory.planet.waterItemId <= 0)
						{
							buildPreview.condition = EBuildCondition.NeedWater;
							continue;
						}
						Vector3 vector12 = buildPreview.desc.waterPoints[num52];
						vector12.y = 0f;
						Vector3 origin2 = vector + quaternion * vector12;
						Vector3 normalized2 = origin2.normalized;
						origin2 += normalized2 * 3f;
						Vector3 direction2 = -normalized2;
						float num53 = 0f;
						float num54 = 0f;
						num53 = ((!Physics.Raycast(new Ray(origin2, direction2), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide)) ? 1000f : hitInfo.distance);
						if (Physics.Raycast(new Ray(origin2, direction2), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide))
						{
							num54 = hitInfo.distance;
							if (num53 - num54 <= 0.27f)
							{
								buildPreview.condition = EBuildCondition.NeedWater;
							}
						}
						else
						{
							buildPreview.condition = EBuildCondition.NeedWater;
						}
					}
				}
				if (buildPreview.desc.isInserter && buildPreview.condition == EBuildCondition.Ok)
				{
					bool flag5 = __instance.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt);
					bool flag6 = __instance.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt);
					Vector3 zero = Vector3.zero;
					Vector3 vector13 = ((buildPreview.output == null) ? __instance.GetObjectPose(buildPreview.outputObjId).position : buildPreview.output.lpos);
					Vector3 vector14 = ((buildPreview.input == null) ? __instance.GetObjectPose(buildPreview.inputObjId).position : buildPreview.input.lpos);
					zero = ((flag5 && !flag6) ? vector13 : ((!(!flag5 && flag6)) ? ((vector13 + vector14) * 0.5f) : vector14));
					float num55 = __instance.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(zero, buildPreview.lpos, buildPreview.lpos2);
					float num56 = num55;
					float magnitude = forward2.magnitude;
					float num57 = 5.5f;
					float num58 = 0.6f;
					float num59 = 3.499f;
					float num60 = 0.88f;
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
					int oneParameter = Mathf.RoundToInt(Mathf.Clamp(num56, 1f, 3f));
					buildPreview.SetOneParameter(oneParameter);
				}
			}
			bool flag7 = true;
			for (int num61 = 0; num61 < __instance.buildPreviews.Count; num61++)
			{
				BuildPreview buildPreview3 = __instance.buildPreviews[num61];
				if (buildPreview3.condition != 0 && buildPreview3.condition != EBuildCondition.NeedConn)
				{
					flag7 = false;
					__instance.actionBuild.model.cursorState = -1;
					__instance.actionBuild.model.cursorText = buildPreview3.conditionText;
					if (buildPreview3.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
					{
						__instance.actionBuild.model.cursorText += "垂直建造可升级".Translate();
					}
					break;
				}
			}
			if (flag7)
			{
				__instance.actionBuild.model.cursorState = 0;
				__instance.actionBuild.model.cursorText = "点击鼠标建造".Translate();
			}
			if (!flag7 && !VFInput.onGUI)
			{
				UICursor.SetCursor(ECursor.Ban);
			}
			__result = flag7;
			return false;
		}
		//public static bool PlayFoorstepEffect(PlayerFootsteps __instance)
		//{
		//    GS2.Log($"{__instance.player.planetData.ambientDesc.biomoDustColor0} {Utils.AddressHelper.GetAddress(__instance.player.planetData.ambientDesc)} ");
		//    return true;
		//}
		//[HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "RequestLoadPlanet")]
		//public static bool RequestLoadPlanet(ref PlanetData planet)
		//{

		//    Queue<PlanetData> obj = PlanetModelingManager.genPlanetReqList;
		//    lock (obj)
		//    {
		//        Warn("Requested Load of " + planet.name);
		//        planet.wanted = true;
		//        if (!planet.loaded && !planet.loading)
		//        {
		//            planet.loading = true;
		//            Warn("Queueing " + planet.name);
		//            PlanetModelingManager.genPlanetReqList.Enqueue(planet);
		//        }
		//    }
		//    return false;
		//}
		//[HarmonyPrefix, HarmonyPatch(typeof(PlayerController), "GameTick")]
		//public static bool GameTick(ref PlayerController __instance, long time)
		//{
		//    if (NebulaCompatibility.IsMasterClient) return true;
		//    GS2.Warn("Start GameTick");
		//    __instance.UpdateEnvironment();
		//    GS2.Warn("Updated Environment");
		//    __instance.SetCommandStateHeader();
		//    GS2.Warn("SetCommandStateHeader");
		//    __instance.UpdateCommandState();
		//    GS2.Warn("UpdatedCommandState");

		//    __instance.GetInput();
		//    GS2.Warn("GotInput");

		//    __instance.Handle__instanceInput();
		//    GS2.Warn("Handled__instanceInput");

		//    __instance.ClearForce();
		//    GS2.Warn("Cleared Force");

		//    __instance.ApplyGravity();
		//    GS2.Warn("AppliedGravity");

		//    __instance.rigidbodySleep = false;
		//    __instance.movementStateInFrame = __instance.player.movementState;
		//    __instance.velocityOnLanding = Vector3.zero;
		//    GS2.Warn("Running PlayerAction GameTicks");

		//    foreach (PlayerAction playerAction in __instance.actions)
		//    {
		//        playerAction.GameTick(time);
		//    }
		//    GS2.Warn("Applying Local Force");

		//    __instance.ApplyLocalForce();
		//    GS2.Warn("Updating Rotation");

		//    __instance.UpdateRotation();
		//    GS2.Warn("RigidbodySafer");

		//    __instance.RigidbodySafer();
		//    GS2.Warn("Updating PhysicsDirect");

		//    __instance.UpdatePhysicsDirect();
		//    GS2.Warn("Updating Tracker");

		//    __instance.UpdateTracker();
		//    GS2.Warn("Setting MovementState");

		//    __instance.player.movementState = __instance.movementStateInFrame;
		//    if (__instance.velocityOnLanding.sqrMagnitude > 0f)
		//    {
		//        __instance.velocity = __instance.velocityOnLanding;
		//    }
		//    GS2.Warn("Done");

		//    if (DSPGame.IsMenuDemo) return true;

		//return false;
		//}

		//    [HarmonyPostfix, HarmonyPatch(typeof(PlayerController), "RigidbodySafer")]
		//    public static void Postfix()
		//    {
		//        GS2.Warn("RigidbodySafer Postfix");
		//    }




		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "ComputeMaxReformCount")]
		//		//public static bool ComputeMaxReformCount() {
		//		//    GS2.Warn("."); return true;
		//		//}
		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "DetermineLongitudeSegmentCount")]
		//		//public static bool DetermineLongitudeSegmentCount() {
		//		//    GS2.Warn("."); return true;
		//		//}
		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "FreeReformData")]
		//		//public static bool FreeReformData() {
		//		//    GS2.Warn("."); return true;
		//		//}
		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndex")]
		//		//public static bool GetReformIndex(ref int __result, ref PlatformSystem __instance, int x, int y) {
		//		//    __result = __instance.reformOffsets[y] + x;
		//		//    GS2.Warn($"{__result}: x:{x}, y:{y}");
		//		//    return false;
		//		//}

		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndexForPosition")]
		//		//public static bool GetReformIndexForPosition(ref int __result, ref PlatformSystem __instance, Vector3 pos) {
		//		//    GS2.Warn($"{pos}");
		//		//    pos.Normalize();
		//		//    float num = Mathf.Asin(pos.y);
		//		//    float num2 = Mathf.Atan2(pos.x, -pos.z);
		//		//    float num3 = num / 6.2831855f * (float)__instance.segment;
		//		//    float num4 = (float)PlatformSystem.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(num3)), __instance.segment);
		//		//    float num5 = num2 / 6.2831855f * num4;
		//		//    float num6 = Mathf.Round(num3 * 10f);
		//		//    float num7 = Mathf.Round(num5 * 10f);
		//		//    float num8 = Mathf.Abs(num6);
		//		//    float num9 = Mathf.Abs(num7);
		//		//    if (num8 % 2f != 1f) {
		//		//        num3 = Mathf.Abs(num3);
		//		//        num8 = (float)Mathf.FloorToInt(num3 * 10f);
		//		//        if (num8 % 2f != 1f) {
		//		//            num8 += 1f;
		//		//        }
		//		//    }
		//		//    num8 = ((num6 >= 0f) ? num8 : (-num8));
		//		//    if (num9 % 2f != 1f) {
		//		//        num5 = Mathf.Abs(num5);
		//		//        num9 = (float)Mathf.FloorToInt(num5 * 10f);
		//		//        if (num9 % 2f != 1f) {
		//		//            num9 += 1f;
		//		//        }
		//		//    }
		//		//    num9 = ((num7 >= 0f) ? num9 : (-num9));
		//		//    num8 /= 10f;
		//		//    num9 /= 10f;
		//		//    float num10 = (float)(__instance.latitudeCount / 10);
		//		//    if (num8 >= num10 || num8 <= -num10) {
		//		//        __result = -1; GS2.Warn(__result.ToString()); return false;
		//		//    }
		//		//    __result = __instance.GetReformIndexForSegment(num8, num9); GS2.Warn(__result.ToString()); return false;
		//		//}
		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformIndexForSegment")]
		//		//public static bool GetReformIndexForSegment(ref int __result, ref PlatformSystem __instance, float _latitudeSeg, float _longitudeSeg) {
		//		//    int LatitudeSegment = (_latitudeSeg > 0f) ? Mathf.CeilToInt(_latitudeSeg * 5f) : Mathf.FloorToInt(_latitudeSeg * 5f);
		//		//    int LongitudeSegment = (_longitudeSeg > 0f) ? Mathf.CeilToInt(_longitudeSeg * 5f) : Mathf.FloorToInt(_longitudeSeg * 5f);
		//		//    int HalfLatitudeCount = __instance.latitudeCount / 2;
		//		//    int y = (LatitudeSegment > 0) ? (LatitudeSegment - 1) : (HalfLatitudeCount - LatitudeSegment - 1);
		//		//    int LongSegmentCount = PlatformSystem.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Abs(_latitudeSeg)), __instance.segment);
		//		//    //GS2.Warn($"LongSegmentCount:{LongSegmentCount}, LongitudeSegment:{LongitudeSegment}, segment:{__instance.segment} {GS2.GetCaller(1)}");
		//		//    if (LongitudeSegment > LongSegmentCount * 5 / 2) {
		//		//        LongitudeSegment = LongitudeSegment - LongSegmentCount * 5 - 1;
		//		//    }
		//		//    if (LongitudeSegment < -LongSegmentCount * 5 / 2) {
		//		//        LongitudeSegment = LongSegmentCount * 5 + LongitudeSegment + 1;
		//		//    }
		//		//    int x = (LongitudeSegment > 0) ? (LongitudeSegment - 1) : (LongSegmentCount * 5 / 2 - LongitudeSegment - 1);
		//		//    __result = __instance.GetReformIndex(x, y);
		//		//    //GS2.Warn($"LongSegmentCount:{LongSegmentCount}, LongitudeSegment:{LongitudeSegment}, segment:{__instance.segment} _longitudeSeg:{_longitudeSeg} x:{x}");
		//		//    //GS2.Warn($"x:{x}, y:{y}, _latitudeSeg:{_latitudeSeg}, _longitudeSeg:{_longitudeSeg}. Result of DLSC:{LongSegmentCount}");
		//		//    return false;
		//		//}
		//		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "GetReformType")]
		//		////public static bool GetReformType() {
		//		////    GS2.Warn("."); return true;
		//		////}
		//		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "InitReformData")]
		//		////public static bool InitReformData() {
		//		////    GS2.Warn("."); return true;
		//		////}
		//		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "IsTerrainMapping")]
		//		////public static bool IsTerrainMapping() {
		//		////    GS2.Warn("."); return true;
		//		////}
		//		////[HarmonyPrefix, HarmonyPatch(typeof(PlatformSystem), "IsTerrainReformed")]
		//		////public static bool IsTerrainReformed() {
		//		////    GS2.Warn("."); return true;
		//		////}
		//		//static bool output = false;
		//		//[HarmonyPrefix, HarmonyPatch(typeof(PlanetGrid), "ReformSnapTo")]

		//		//public static bool ReformSnapTo(ref int __result, ref PlanetGrid __instance, Vector3 pos, int reformSize, int reformType, int reformColor, Vector3[] reformPoints, int[] reformIndices, PlatformSystem platform, out Vector3 reformCenter) {
		//		//    pos.Normalize();
		//		//    float AsinY = Mathf.Asin(pos.y);
		//		//    float AtanXZ = Mathf.Atan2(pos.x, -pos.z);
		//		//    float latitude = AsinY / 6.2831855f * (float)__instance.segment;
		//		//    int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(latitude));
		//		//    int LSC = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
		//		//    float fLSC = (float)LSC;
		//		//    float longitude = AtanXZ / 6.2831855f * fLSC;
		//		//    if (VFInput.control && !output) {
		//		//        GS2.Warn($"Latitude:{latitude}:{latitudeIndex}, Longitude:{longitude}, LSC:{LSC}, Segment:{__instance.segment}, AsinY:{AsinY}, AtanXZ:{AtanXZ} Pos:{pos}");
		//		//        output = true;
		//		//    }
		//		//    if (!VFInput.control) output = false;
		//		//        float LatitudeX10 = Mathf.Round(latitude * 10f);
		//		//    float LongitudeX10 = Mathf.Round(longitude * 10f);
		//		//    float num9 = Mathf.Abs(LatitudeX10);
		//		//    float absLongitudeX10 = Mathf.Abs(LongitudeX10);
		//		//    int reformSizeMod2 = reformSize % 2;
		//		//    if (num9 % 2f != (float)reformSizeMod2) {
		//		//        latitude = Mathf.Abs(latitude);
		//		//        num9 = (float)Mathf.FloorToInt(latitude * 10f);
		//		//        if (num9 % 2f != (float)reformSizeMod2) {
		//		//            num9 += 1f;
		//		//        }
		//		//    }
		//		//    num9 = (LatitudeX10 < 0f) ? (-num9) : num9;
		//		//    if (absLongitudeX10 % 2f != (float)reformSizeMod2) {
		//		//        longitude = Mathf.Abs(longitude);
		//		//        absLongitudeX10 = (float)Mathf.FloorToInt(longitude * 10f);
		//		//        if (absLongitudeX10 % 2f != (float)reformSizeMod2) {
		//		//            absLongitudeX10 += 1f;
		//		//        }
		//		//    }
		//		//    absLongitudeX10 = ((LongitudeX10 < 0f) ? (-absLongitudeX10) : absLongitudeX10);
		//		//    AsinY = num9 / 10f / (float)__instance.segment * 6.2831855f;
		//		//    AtanXZ = absLongitudeX10 / 10f / fLSC * 6.2831855f;
		//		//    float y = Mathf.Sin(AsinY);
		//		//    float num12 = Mathf.Cos(AsinY);
		//		//    float num13 = Mathf.Sin(AtanXZ);
		//		//    float num14 = Mathf.Cos(AtanXZ);
		//		//    reformCenter = new Vector3(num12 * num13, y, num12 * -num14);
		//		//    int num15 = 1 - reformSize;
		//		//    int num16 = 1 - reformSize;
		//		//    int num17 = 0;
		//		//    int num18 = 0;
		//		//    float num19 = (float)(platform.latitudeCount / 10);
		//		//    for (int i = 0; i < reformSize * reformSize; i++) {
		//		//        num18++;
		//		//        latitude = (num9 + (float)num15) / 10f;
		//		//        longitude = (absLongitudeX10 + (float)num16) / 10f;
		//		//        num16 += 2;
		//		//        if (num18 % reformSize == 0) {
		//		//            num16 = 1 - reformSize;
		//		//            num15 += 2;
		//		//        }
		//		//        if (latitude >= num19 || latitude <= -num19) {
		//		//            reformIndices[i] = -1;
		//		//        } else {
		//		//            latitudeIndex = Mathf.FloorToInt(Mathf.Abs(latitude));
		//		//            if (LSC != PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment)) {
		//		//                reformIndices[i] = -1;
		//		//            } else {
		//		//                int reformIndexForSegment = platform.GetReformIndexForSegment(latitude, longitude);
		//		//                reformIndices[i] = reformIndexForSegment;
		//		//                int reformType2 = platform.GetReformType(reformIndexForSegment);
		//		//                int reformColor2 = platform.GetReformColor(reformIndexForSegment);
		//		//                if (!platform.IsTerrainReformed(reformType2) && (reformType2 != reformType || reformColor2 != reformColor)) {
		//		//                    AsinY = latitude / (float)__instance.segment * 6.2831855f;
		//		//                    AtanXZ = longitude / fLSC * 6.2831855f;
		//		//                    y = Mathf.Sin(AsinY);
		//		//                    num12 = Mathf.Cos(AsinY);
		//		//                    num13 = Mathf.Sin(AtanXZ);
		//		//                    num14 = Mathf.Cos(AtanXZ);
		//		//                    reformPoints[num17] = new Vector3(num12 * num13, y, num12 * -num14);
		//		//                    num17++;
		//		//                }
		//		//            }
		//		//        }
		//		//    }
		//		//    __result = num17;
		//		//    return false;
		//		//}




	}
}