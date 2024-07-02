using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;
using static System.Reflection.Emit.OpCodes;
using HarmonyLib;


namespace GalacticScale
{
    public partial class PatchOnBuildTool_Path
    {

// 	[HarmonyPrefix]
// 	[HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.CheckBuildConditions))]
// 	public static bool BuildTool_Path_CheckBuildConditions(ref BuildTool_Path __instance, ref bool __result)
// 	{
// 		if (__instance.buildPreviews.Count == 0)
// 		{
// 			__result = false;
// 			return false;
// 		}
// 		GameHistoryData history = __instance.actionBuild.history;
// 		bool flag = false;
// 		bool flag2 = false;
// 		Vector3 vector = Vector3.zero;
// 		if (__instance.planet.id == __instance.planet.galaxy.birthPlanetId && history.SpaceCapsuleExist())
// 		{
// 			vector = __instance.planet.birthPoint;
// 			flag2 = true;
// 		}
// 		bool commandStageGreaterThanZero = __instance.controller.cmd.stage > 0;
// 		bool commandStageZero = __instance.controller.cmd.stage == 0;
// 		int count = __instance.buildPreviews.Count;
// 		int lastIndex = count - 1;
// 		for (int i = 0; i < count; i++)
// 		{
// 			BuildPreview buildPreview = __instance.buildPreviews[i];
// 			bool isBelt = buildPreview.desc.isBelt;
// 			if (buildPreview.condition == EBuildCondition.Ok)
// 			{
// 				Vector3 vector2 = buildPreview.lpos;
// 				Quaternion lrot = buildPreview.lrot;
// 				Pose pose = new Pose(buildPreview.lpos, buildPreview.lrot);
// 				Vector3 forward = pose.forward;
// 				Vector3 up = pose.up;
// 				if (vector2.sqrMagnitude < 1f)
// 				{
// 					buildPreview.condition = EBuildCondition.Failure;
// 				}
// 				else
// 				{
// 					if (buildPreview.coverObjId == 0 || buildPreview.willRemoveCover)
// 					{
// 						int id = buildPreview.item.ID;
// 						int num2 = 1;
// 						if (__instance.tmpInhandId == id && __instance.tmpInhandCount > 0)
// 						{
// 							num2 = 1;
// 							__instance.tmpInhandCount--;
// 						}
// 						else
// 						{
// 							int num3;
// 							__instance.tmpPackage.TakeTailItems(ref id, ref num2, out num3, false);
// 						}
// 						if (num2 == 0)
// 						{
// 							buildPreview.condition = EBuildCondition.NotEnoughItem;
// 							goto IL_10A6;
// 						}
// 					}
// 					Vector3 position = __instance.player.position;
// 					float num4 = __instance.player.mecha.buildArea * __instance.player.mecha.buildArea;
// 					if ((vector2 - position).sqrMagnitude > num4)
// 					{
// 						buildPreview.condition = EBuildCondition.OutOfReach;
// 					}
// 					else
// 					{
// 						if (__instance.planet != null)
// 						{
// 							float num5 = 64f;
// 							float num6 = history.buildMaxHeight + 0.5f + __instance.planet.realRadius;
// 							if (vector2.sqrMagnitude > num6 * num6)
// 							{
// 								flag = history.buildMaxHeight + 0.5f > num5;
// 								buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
// 								goto IL_10A6;
// 							}
// 						}
// 						bool flag5 = i == 0;
// 						bool flag6 = i == 1;
// 						bool flag7 = i == 2;
// 						bool flag8 = flag5 || flag6;
// 						bool flag9 = flag5 || flag6 || flag7;
// 						bool flag10 = i == lastIndex;
// 						bool flag11 = i == lastIndex - 1;
// 						bool flag12 = i == lastIndex - 2;
// 						bool flag13 = flag10 || flag11;
// 						bool flag14 = flag10 || flag11 || flag12;
// 						int num7 = (commandStageZero ? __instance.castObjectId : __instance.startObjectId);
// 						int castObjectId = __instance.castObjectId;
// 						if (commandStageGreaterThanZero && count < 4 && num7 != 0 && !__instance.ObjectIsBelt(num7) && num7 == castObjectId)
// 						{
// 							buildPreview.condition = EBuildCondition.NeedExport;
// 						}
// 						else
// 						{
// 							bool flag15 = buildPreview.input != null && buildPreview.input.desc.isBelt;
// 							bool flag16 = buildPreview.output != null && buildPreview.output.desc.isBelt;
// 							Vector3 vector3 = buildPreview.lpos.normalized * 0.22f;
// 							Vector3 vector4 = buildPreview.lpos;
// 							Vector3 vector5 = (flag15 ? buildPreview.input.lpos : vector4);
// 							Vector3 vector6 = (flag16 ? buildPreview.output.lpos : vector4);
// 							vector5 = (vector5 - vector4) * 0.65f + vector4;
// 							vector6 = (vector6 - vector4) * 0.65f + vector4;
// 							Vector3 vector7 = vector6 - vector5;
// 							vector5 += (flag15 ? (vector7 * 0.45f) : Vector3.zero);
// 							vector6 -= (flag16 ? (vector7 * 0.45f) : Vector3.zero);
// 							vector4 += vector3;
// 							vector5 += vector3;
// 							vector6 += vector3;
// 							int num8;
// 							if (flag16 || flag15)
// 							{
// 								num8 = Physics.OverlapCapsuleNonAlloc(vector5, vector6, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 							}
// 							else
// 							{
// 								num8 = Physics.OverlapSphereNonAlloc(vector4, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 							}
// 							if (!flag8 && !flag13 && num8 > 0)
// 							{
// 								bool flag17 = false;
// 								for (int j = 0; j < num8; j++)
// 								{
// 									ColliderData colliderData;
// 									if (__instance.planet.physics.GetColliderData(BuildTool._tmp_cols[j], out colliderData))
// 									{
// 										int num9 = 0;
// 										if (colliderData.objType == EObjectType.Entity)
// 										{
// 											num9 = colliderData.objId;
// 										}
// 										else if (colliderData.objType == EObjectType.Prebuild)
// 										{
// 											num9 = -colliderData.objId;
// 										}
// 										if (num9 != 0 && __instance.GetPrefabDesc(num9).isBelt && colliderData.pos.magnitude < vector4.magnitude + 0.1f)
// 										{
// 											flag17 = true;
// 											break;
// 										}
// 									}
// 								}
// 								if (flag17)
// 								{
// 									buildPreview.lpos += buildPreview.lpos.normalized * 1.3333333f * 0.5f;
// 									buildPreview.lpos2 = buildPreview.lpos;
// 									vector2 = buildPreview.lpos;
// 									pose = new Pose(buildPreview.lpos, buildPreview.lrot);
// 									if (__instance.planet != null)
// 									{
// 										float num10 = 64f;
// 										float num11 = history.buildMaxHeight + 0.5f + __instance.planet.realRadius;
// 										if (vector2.sqrMagnitude > num11 * num11)
// 										{
// 											flag = history.buildMaxHeight + 0.5f > num10;
// 											buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
// 											goto IL_10A6;
// 										}
// 									}
// 									vector4 = buildPreview.lpos;
// 									vector5 = (flag15 ? buildPreview.input.lpos : vector4);
// 									vector6 = (flag16 ? buildPreview.output.lpos : vector4);
// 									vector5 = (vector5 - vector4) * 0.65f + vector4;
// 									vector6 = (vector6 - vector4) * 0.65f + vector4;
// 									vector7 = vector6 - vector5;
// 									vector5 += (flag15 ? (vector7 * 0.45f) : Vector3.zero);
// 									vector6 -= (flag16 ? (vector7 * 0.45f) : Vector3.zero);
// 									vector4 += vector3;
// 									vector5 += vector3;
// 									vector6 += vector3;
// 									if (flag16 || flag15)
// 									{
// 										num8 = Physics.OverlapCapsuleNonAlloc(vector5, vector6, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 									}
// 									else
// 									{
// 										num8 = Physics.OverlapSphereNonAlloc(vector4, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 									}
// 									if (num8 > 0 && buildPreview.lpos.magnitude > __instance.planet.realRadius + 0.2f + 1.3333333f - 0.01f)
// 									{
// 										bool flag18 = false;
// 										for (int k = 0; k < num8; k++)
// 										{
// 											ColliderData colliderData2;
// 											if (__instance.planet.physics.GetColliderData(BuildTool._tmp_cols[k], out colliderData2))
// 											{
// 												int num12 = 0;
// 												if (colliderData2.objType == EObjectType.Entity)
// 												{
// 													num12 = colliderData2.objId;
// 												}
// 												else if (colliderData2.objType == EObjectType.Prebuild)
// 												{
// 													num12 = -colliderData2.objId;
// 												}
// 												if (num12 != 0 && __instance.GetPrefabDesc(num12).isBelt && colliderData2.pos.magnitude > buildPreview.lpos.magnitude - 0.001f)
// 												{
// 													flag18 = true;
// 													break;
// 												}
// 											}
// 										}
// 										if (flag18)
// 										{
// 											buildPreview.lpos -= buildPreview.lpos.normalized * 1.3333333f;
// 											buildPreview.lpos2 = buildPreview.lpos;
// 											vector2 = buildPreview.lpos;
// 											pose = new Pose(buildPreview.lpos, buildPreview.lrot);
// 											vector4 = buildPreview.lpos;
// 											vector5 = (flag15 ? buildPreview.input.lpos : vector4);
// 											vector6 = (flag16 ? buildPreview.output.lpos : vector4);
// 											vector5 = (vector5 - vector4) * 0.65f + vector4;
// 											vector6 = (vector6 - vector4) * 0.65f + vector4;
// 											vector7 = vector6 - vector5;
// 											vector5 += (flag15 ? (vector7 * 0.45f) : Vector3.zero);
// 											vector6 -= (flag16 ? (vector7 * 0.45f) : Vector3.zero);
// 											vector4 += vector3;
// 											vector5 += vector3;
// 											vector6 += vector3;
// 											if (flag16 || flag15)
// 											{
// 												num8 = Physics.OverlapCapsuleNonAlloc(vector5, vector6, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 											}
// 											else
// 											{
// 												num8 = Physics.OverlapSphereNonAlloc(vector4, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 											}
// 											if (num8 > 0)
// 											{
// 												buildPreview.lpos += buildPreview.lpos.normalized * 1.3333333f * 0.5f;
// 												buildPreview.lpos2 = buildPreview.lpos;
// 												vector2 = buildPreview.lpos;
// 												pose = new Pose(buildPreview.lpos, buildPreview.lrot);
// 												vector4 = buildPreview.lpos;
// 												vector5 = (flag15 ? buildPreview.input.lpos : vector4);
// 												vector6 = (flag16 ? buildPreview.output.lpos : vector4);
// 												vector5 = (vector5 - vector4) * 0.65f + vector4;
// 												vector6 = (vector6 - vector4) * 0.65f + vector4;
// 												vector7 = vector6 - vector5;
// 												vector5 += (flag15 ? (vector7 * 0.45f) : Vector3.zero);
// 												vector6 -= (flag16 ? (vector7 * 0.45f) : Vector3.zero);
// 												vector4 += vector3;
// 												vector5 += vector3;
// 												vector6 += vector3;
// 												if (flag16 || flag15)
// 												{
// 													num8 = Physics.OverlapCapsuleNonAlloc(vector5, vector6, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 												}
// 												else
// 												{
// 													num8 = Physics.OverlapSphereNonAlloc(vector4, 0.22f, BuildTool._tmp_cols, 393216, QueryTriggerInteraction.Collide);
// 												}
// 											}
// 										}
// 									}
// 								}
// 							}
// 							if (num8 > 0)
// 							{
// 								bool flag19 = false;
// 								int l = 0;
// 								while (l < num8)
// 								{
// 									ColliderData colliderData3;
// 									if (__instance.planet.physics.GetColliderData(BuildTool._tmp_cols[l], out colliderData3))
// 									{
// 										int num13 = 0;
// 										if (colliderData3.objType == EObjectType.Entity)
// 										{
// 											num13 = colliderData3.objId;
// 										}
// 										else if (colliderData3.objType == EObjectType.Prebuild)
// 										{
// 											num13 = -colliderData3.objId;
// 										}
// 										if (num13 != 0)
// 										{
// 											PrefabDesc prefabDesc = __instance.GetPrefabDesc(num13);
// 											if ((!flag8 || num13 != num7) && (!flag13 || num13 != castObjectId) && (!prefabDesc.isStation || ((!flag9 || num13 != num7) && (!flag14 || num13 != castObjectId))))
// 											{
// 												if (prefabDesc.addonType == EAddonType.Belt)
// 												{
// 													Pose objectPose = __instance.GetObjectPose(num13);
// 													bool flag20 = false;
// 													bool flag21 = true;
// 													int num14 = 0;
// 													float num15 = (prefabDesc.isTurret ? 5f : 20.5f);
// 													Pose[] addonAreaColPoses = prefabDesc.addonAreaColPoses;
// 													Vector3[] addonAreaSize = prefabDesc.addonAreaSize;
// 													for (int m = 0; m < prefabDesc.addonAreaColPoses.Length; m++)
// 													{
// 														Vector3 vector8 = objectPose.position + objectPose.rotation * prefabDesc.addonAreaPoses[m].position;
// 														Quaternion quaternion = objectPose.rotation * prefabDesc.addonAreaPoses[m].rotation;
// 														Vector3 vector9 = objectPose.position + objectPose.rotation * (addonAreaColPoses[m].position + addonAreaColPoses[m].forward * addonAreaSize[m].z * 2.5f);
// 														Vector3 vector10 = objectPose.position + objectPose.rotation * (addonAreaColPoses[m].position - addonAreaColPoses[m].forward * addonAreaSize[m].z * 2.5f);
// 														float num16 = Maths.DistancePointLine(vector4, vector9, vector10);
// 														float num17 = (vector4 - vector8).sqrMagnitude - num16 * num16;
// 														float z = addonAreaSize[m].z;
// 														if (num17 < z * z)
// 														{
// 															flag21 = false;
// 														}
// 														if (__instance.CheckBeltAddonAreaCondition(i, num14, num15, new Pose(vector8, quaternion)))
// 														{
// 															flag20 = true;
// 															break;
// 														}
// 													}
// 													if (flag20 || flag21)
// 													{
// 														goto IL_F79;
// 													}
// 												}
// 												int num18 = 0;
// 												if (flag5)
// 												{
// 													num18 = num7;
// 												}
// 												if (num18 == 0 && flag10)
// 												{
// 													num18 = castObjectId;
// 												}
// 												if (num18 != 0 && __instance.ObjectIsBelt(num13) && !__instance.ObjectIsBelt(num18))
// 												{
// 													bool flag22 = false;
// 													for (int n = 0; n < 14; n++)
// 													{
// 														bool flag23;
// 														int num19;
// 														int num20;
// 														__instance.factory.ReadObjectConn(num13, n, out flag23, out num19, out num20);
// 														if (num19 == num18)
// 														{
// 															flag22 = true;
// 															break;
// 														}
// 													}
// 													if (flag22)
// 													{
// 														goto IL_F79;
// 													}
// 												}
// 												if (num18 != 0 && __instance.ObjectIsBelt(num18) && !__instance.ObjectIsBelt(num13) && __instance.GetLocalPorts(num13).Length != 0)
// 												{
// 													if (flag5)
// 													{
// 														bool flag24;
// 														int num21;
// 														int num22;
// 														__instance.factory.ReadObjectConn(num18, 1, out flag24, out num21, out num22);
// 														if (num21 != 0 && __instance.ObjectIsBelt(num21) && !flag24 && num22 == 0)
// 														{
// 															int num23;
// 															__instance.factory.ReadObjectConn(num21, 1, out flag24, out num23, out num22);
// 															if (num23 == num13 && !flag24)
// 															{
// 																goto IL_F79;
// 															}
// 														}
// 													}
// 													else if (flag10)
// 													{
// 														bool flag24;
// 														int num21;
// 														int num22;
// 														__instance.factory.ReadObjectConn(num18, 0, out flag24, out num21, out num22);
// 														if (num21 != 0 && __instance.ObjectIsBelt(num21) && flag24 && num22 == 1)
// 														{
// 															int num23;
// 															__instance.factory.ReadObjectConn(num21, 0, out flag24, out num23, out num22);
// 															if (num23 == num13 && flag24)
// 															{
// 																goto IL_F79;
// 															}
// 														}
// 													}
// 												}
// 												if (num18 == 0 || !__instance.ObjectIsBelt(num18) || !__instance.ObjectIsBelt(num13))
// 												{
// 													goto IL_ECC;
// 												}
// 												bool flag25;
// 												int num24;
// 												int num25;
// 												__instance.factory.ReadObjectConn(num18, 0, out flag25, out num24, out num25);
// 												if (num24 != num13)
// 												{
// 													__instance.factory.ReadObjectConn(num18, 1, out flag25, out num24, out num25);
// 													if (num24 != num13)
// 													{
// 														__instance.factory.ReadObjectConn(num18, 2, out flag25, out num24, out num25);
// 														if (num24 != num13)
// 														{
// 															__instance.factory.ReadObjectConn(num18, 3, out flag25, out num24, out num25);
// 															if (num24 != num13)
// 															{
// 																goto IL_ECC;
// 															}
// 														}
// 													}
// 												}
// 											}
// 											IL_F79:
// 											l++;
// 											continue;
// 										}
// 										IL_ECC:
// 										if (num13 != 0 && __instance.factory.entityCount < 400 && __instance.gameData.factoryCount == 1)
// 										{
// 											ItemProto itemProto = __instance.GetItemProto(num13);
// 											if (itemProto != null && (itemProto.prefabDesc.isAssembler || itemProto.prefabDesc.isStorage || itemProto.prefabDesc.isLab) && (itemProto.prefabDesc.portPoses == null || itemProto.prefabDesc.portPoses.Length == 0))
// 											{
// 												__instance.actionBuild.model.promptText = string.Format("传送带建造提示1".Translate(), itemProto.name);
// 											}
// 										}
// 									}
// 									flag19 = true;
// 									break;
// 								}
// 								if (flag19)
// 								{
// 									buildPreview.condition = EBuildCondition.Collide;
// 									goto IL_10A6;
// 								}
// 							}
// 							if (flag2 && vector2.magnitude < __instance.planet.realRadius + 3f)
// 							{
// 								Vector3 ext = buildPreview.desc.buildCollider.ext;
// 								float num26 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
// 								if ((vector2 - vector).magnitude - num26 < 3.7f)
// 								{
// 									buildPreview.condition = EBuildCondition.Collide;
// 									goto IL_10A6;
// 								}
// 							}
// 							float num27 = __instance.planet.data.QueryModifiedHeight(vector4) - (__instance.planet.realRadius + 0.2f);
// 							if (num27 < 0f)
// 							{
// 								num27 = 0f;
// 							}
// 							Vector3 vector11 = vector4 + vector4.normalized * (num27 + 0.4f);
// 							if ((!flag5 || num7 == 0) && (!flag10 || castObjectId == 0))
// 							{
// 								num8 = Physics.OverlapSphereNonAlloc(vector11, 0.5f, BuildTool._tmp_cols, 2048, QueryTriggerInteraction.Collide);
// 								if (num8 > 0)
// 								{
// 									buildPreview.condition = EBuildCondition.Collide;
// 								}
// 							}
// 						}
// 					}
// 				}
// 			}
// 			IL_10A6:;
// 		}
// 		for (int index = 0; index < count; index++)
// 		{
// 			BuildPreview buildPreview2 = __instance.buildPreviews[index];
// 			bool isBelt2 = buildPreview2.desc.isBelt;
// 			if (buildPreview2.condition == EBuildCondition.Ok && !commandStageZero)
// 			{
// 				bool indexIs0 = index == 0;
// 				bool indexIs1 = index == 1;
// 				bool indexIs2 = index == 2;
// 				bool indexIsLast = index == lastIndex;
// 				bool indexIsSecondToLast = index == lastIndex - 1;
// 				bool indexIsThirdToLast = index == lastIndex - 2;
// 				int num29 = (commandStageZero ? __instance.castObjectId : __instance.startObjectId);
// 				int castObjectId2 = __instance.castObjectId;
// 				if (isBelt2)
// 				{
// 					bool hasInputObj = false;
// 					bool outputNotNull = false;
// 					Vector3 vector12 = Vector3.zero;
// 					Vector3 BuildPreview2OrObjectPose3Position = Vector3.zero;
// 					bool input = buildPreview2.input != null;
// 					BuildPreview output = buildPreview2.output;
// 					if (input)
// 					{
// 						hasInputObj = true;
// 						vector12 = buildPreview2.input.lpos;
// 					}
// 					if (buildPreview2.inputObjId != 0)
// 					{
// 						hasInputObj = true;
// 						Pose objectPose2 = __instance.GetObjectPose(buildPreview2.inputObjId);
// 						if (__instance.ObjectIsBelt(buildPreview2.inputObjId))
// 						{
// 							vector12 = objectPose2.position;
// 						}
// 						else if (__instance.ObjectIsAddonBuilding(buildPreview2.addonObjId))
// 						{
// 							Pose pose2 = __instance.GetLocalAddonPose(buildPreview2.addonObjId)[buildPreview2.addonAreaIdx];
// 							vector12 = objectPose2.position + objectPose2.rotation * pose2.position;
// 						}
// 						else
// 						{
// 							Pose pose3 = __instance.GetLocalPorts(buildPreview2.inputObjId)[buildPreview2.inputFromSlot];
// 							vector12 = objectPose2.position + objectPose2.rotation * pose3.position;
// 						}
// 					}
// 					if (__instance.ObjectIsBelt(castObjectId2) && buildPreview2.outputToSlot >= 4)
// 					{
// 						if (buildPreview2.output != null)
// 						{
// 							buildPreview2.output.condition = EBuildCondition.InputFull;
// 						}
// 						else
// 						{
// 							buildPreview2.condition = EBuildCondition.InputFull;
// 						}
// 					}
// 					else
// 					{
// 						if (!indexIs0 && __instance.buildPreviews[index - 1].output == buildPreview2)
// 						{
// 							hasInputObj = true;
// 							vector12 = __instance.buildPreviews[index - 1].lpos;
// 							BuildPreview buildPreview3 = __instance.buildPreviews[index - 1];
// 						}
// 						if (output != null)
// 						{
// 							outputNotNull = true;
// 							BuildPreview2OrObjectPose3Position = buildPreview2.output.lpos;
// 						}
// 						if (buildPreview2.outputObjId != 0)
// 						{
// 							outputNotNull = true;
// 							Pose objectPose3 = __instance.GetObjectPose(buildPreview2.outputObjId);
// 							if (__instance.ObjectIsBelt(buildPreview2.outputObjId))
// 							{
// 								BuildPreview2OrObjectPose3Position = objectPose3.position;
// 							}
// 							else if (__instance.ObjectIsAddonBuilding(buildPreview2.addonObjId))
// 							{
// 								Pose pose4 = __instance.GetLocalAddonPose(buildPreview2.addonAreaIdx)[buildPreview2.outputToSlot];
// 								BuildPreview2OrObjectPose3Position = objectPose3.position + objectPose3.rotation * pose4.position;
// 							}
// 							else
// 							{
// 								Pose pose5 = __instance.GetLocalPorts(buildPreview2.outputObjId)[buildPreview2.outputToSlot];
// 								BuildPreview2OrObjectPose3Position = objectPose3.position + objectPose3.rotation * pose5.position;
// 							}
// 						}
// 						float sphericalAngleAobInRad = 3.1415927f;
// 						if (hasInputObj && outputNotNull)
// 						{
// 							sphericalAngleAobInRad = Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector12, BuildPreview2OrObjectPose3Position);
// 							if (sphericalAngleAobInRad < 0.87266463f)
// 							{
// 								buildPreview2.condition = EBuildCondition.TooBend;
// 								goto IL_1976;
// 							}
// 						}
// 						float sphericalSlopeRatio = 0f;
// 						if (outputNotNull)
// 						{
// 							sphericalSlopeRatio = Mathf.Abs(Maths.SphericalSlopeRatio(buildPreview2.lpos, BuildPreview2OrObjectPose3Position));
// 							if (sphericalSlopeRatio > 0.8f)
// 							{
// 								buildPreview2.condition = EBuildCondition.TooSteep;
// 								goto IL_1976;
// 							}
// 						}
// 						if (hasInputObj)
// 						{
// 							sphericalSlopeRatio = Mathf.Max(Mathf.Abs(Maths.SphericalSlopeRatio(vector12, buildPreview2.lpos)), sphericalSlopeRatio);
// 							if (sphericalSlopeRatio > 0.8f)
// 							{
// 								buildPreview2.condition = EBuildCondition.TooSteep;
// 								goto IL_1976;
// 							}
// 						}
// 						if (outputNotNull && !indexIsLast)
// 						{
// 							Vector3 vector14 = BuildPreview2OrObjectPose3Position - buildPreview2.lpos;
// 							Vector3 normalized = buildPreview2.lpos.normalized;
// 							// vector14 -= Vector3.Dot(vector14, normalized) * normalized;
// 							if ((buildPreview2.lpos - BuildPreview2OrObjectPose3Position).magnitude < 0.4f)
// 							{
// 								buildPreview2.condition = EBuildCondition.TooClose;
// 								goto IL_1976;
// 							}
// 						}
// 						GS2.DevLog($"SphericalAngleAOBInRad:{sphericalAngleAobInRad} SphericalSlopeRatio:{sphericalSlopeRatio}");
// 						if (sphericalAngleAobInRad < 2.5f && sphericalSlopeRatio > 0.1f)
// 						{
// 							buildPreview2.condition = EBuildCondition.TooBendToLift;
// 						}
// 						else
// 						{
// 							bool objectIsBelt = __instance.ObjectIsBelt(buildPreview2.coverObjId);
// 							if (sphericalSlopeRatio > 0.25f)//
// 							{
// 								if (hasInputObj && indexIsLast)
// 								{
// 									if (!objectIsBelt)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									Vector3 vector15 = Vector3.zero;
// 									Vector3 vector16 = vector12;
// 									bool flag35;
// 									int num32;
// 									int num33;
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 0, out flag35, out num32, out num33);
// 									if (num32 == 0)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									vector15 = __instance.GetObjectPose(num32).position;
// 									if (Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector16, vector15) < 2.5f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 1, out flag35, out num32, out num33);
// 									if (num32 != 0 && Maths.SphericalSlopeRatio(__instance.GetObjectPose(num32).position, buildPreview2.lpos) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 2, out flag35, out num32, out num33);
// 									if (num32 != 0 && Maths.SphericalSlopeRatio(__instance.GetObjectPose(num32).position, buildPreview2.lpos) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 3, out flag35, out num32, out num33);
// 									if (num32 != 0 && Maths.SphericalSlopeRatio(__instance.GetObjectPose(num32).position, buildPreview2.lpos) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 								}
// 								if (outputNotNull && indexIs0)
// 								{
// 									if (!objectIsBelt)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 									Vector3 vector17 = Vector3.zero;
// 									Vector3 vector18 = BuildPreview2OrObjectPose3Position;
// 									bool flag36 = false;
// 									bool flag37 = false;
// 									bool flag35;
// 									int num32;
// 									int num33;
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 1, out flag35, out num32, out num33);
// 									if (num32 != 0)
// 									{
// 										vector17 = __instance.GetObjectPose(num32).position;
// 										float num34 = Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector18, vector17);
// 										flag36 = true;
// 										if (num34 >= 2.5f)
// 										{
// 											flag37 = true;
// 										}
// 									}
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 2, out flag35, out num32, out num33);
// 									if (num32 != 0)
// 									{
// 										vector17 = __instance.GetObjectPose(num32).position;
// 										float num35 = Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector18, vector17);
// 										flag36 = true;
// 										if (num35 >= 2.5f)
// 										{
// 											flag37 = true;
// 										}
// 									}
// 									__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 3, out flag35, out num32, out num33);
// 									if (num32 != 0)
// 									{
// 										vector17 = __instance.GetObjectPose(num32).position;
// 										float num36 = Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector18, vector17);
// 										flag36 = true;
// 										if (num36 >= 2.5f)
// 										{
// 											flag37 = true;
// 										}
// 									}
// 									if (!flag37)
// 									{
// 										buildPreview2.condition = (flag36 ? EBuildCondition.TooBendToLift : EBuildCondition.JointCannotLift);
// 										goto IL_1976;
// 									}
// 								}
// 							}
// 							if (hasInputObj && indexIsLast && objectIsBelt)
// 							{
// 								Vector3 vector19 = Vector3.zero;
// 								Vector3 vector20 = vector12;
// 								bool flag38;
// 								int num37;
// 								int num38;
// 								__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 0, out flag38, out num37, out num38);
// 								if (num37 != 0 && num37 != num29)
// 								{
// 									vector19 = __instance.GetObjectPose(num37).position;
// 									if (Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector20, vector19) < 0.87266463f)
// 									{
// 										buildPreview2.condition = EBuildCondition.InputConflict;
// 										goto IL_1976;
// 									}
// 									if (Mathf.Abs(Maths.SphericalSlopeRatio(vector19, buildPreview2.lpos)) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 								}
// 								__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 1, out flag38, out num37, out num38);
// 								if (num37 != 0 && num37 != num29)
// 								{
// 									vector19 = __instance.GetObjectPose(num37).position;
// 									if (Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector20, vector19) < 0.87266463f)
// 									{
// 										buildPreview2.condition = EBuildCondition.InputConflict;
// 										goto IL_1976;
// 									}
// 									if (Mathf.Abs(Maths.SphericalSlopeRatio(vector19, buildPreview2.lpos)) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 								}
// 								__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 2, out flag38, out num37, out num38);
// 								if (num37 != 0 && num37 != num29)
// 								{
// 									vector19 = __instance.GetObjectPose(num37).position;
// 									if (Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector20, vector19) < 0.87266463f)
// 									{
// 										buildPreview2.condition = EBuildCondition.InputConflict;
// 										goto IL_1976;
// 									}
// 									if (Mathf.Abs(Maths.SphericalSlopeRatio(vector19, buildPreview2.lpos)) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 										goto IL_1976;
// 									}
// 								}
// 								__instance.factory.ReadObjectConn(buildPreview2.coverObjId, 3, out flag38, out num37, out num38);
// 								if (num37 != 0 && num37 != num29)
// 								{
// 									vector19 = __instance.GetObjectPose(num37).position;
// 									if (Maths.SphericalAngleAOBInRAD(buildPreview2.lpos, vector20, vector19) < 0.87266463f)
// 									{
// 										buildPreview2.condition = EBuildCondition.InputConflict;
// 									}
// 									else if (Mathf.Abs(Maths.SphericalSlopeRatio(vector19, buildPreview2.lpos)) >= 0.1f)
// 									{
// 										buildPreview2.condition = EBuildCondition.JointCannotLift;
// 									}
// 								}
// 							}
// 						}
// 					}
// 				}
// 			}
// 			IL_1976:;
// 			
// 		}
// 		bool flag39 = true;
// 		int num39 = 0;
// 		while (num39 < count)
// 		{
// 			BuildPreview buildPreview4 = __instance.buildPreviews[num39];
// 			if (buildPreview4.condition != EBuildCondition.Ok)
// 			{
// 				flag39 = false;
// 				__instance.actionBuild.model.cursorState = -1;
// 				__instance.actionBuild.model.cursorText = buildPreview4.conditionText;
// 				if (buildPreview4.condition == EBuildCondition.TooSkew)
// 				{
// 					BuildModel model = __instance.actionBuild.model;
// 					model.cursorText += "尝试变换路径".Translate();
// 				}
// 				if (buildPreview4.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
// 				{
// 					BuildModel model2 = __instance.actionBuild.model;
// 					model2.cursorText += "垂直建造可升级".Translate();
// 					break;
// 				}
// 				break;
// 			}
// 			else
// 			{
// 				num39++;
// 			}
// 		}
// 		if (flag39 && __instance.waitForConfirm)
// 		{
// 			__instance.actionBuild.model.cursorState = 0;
// 			__instance.actionBuild.model.cursorText = "点击鼠标建造".Translate() + "   (" + __instance.pathPointCount.ToString() + ")";
// 		}
// 		if (!flag39 && !VFInput.onGUI)
// 		{
// 			UICursor.SetCursor(ECursor.Ban);
// 		}
// 		__result = flag39;
// 		return false;
// 	}
// }

    
    
    
    
    
        [HarmonyTranspiler, HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.CheckBuildConditions))]
        public static IEnumerable<CodeInstruction> CheckBuildConditions_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                // replace : Physics.OverlapCapsuleNonAlloc(vector2, vector3, 0.28f, BuildTool._tmp_cols, 425984, QueryTriggerInteraction.Collide);
                // with    : Physics.OverlapCapsuleNonAlloc(vector2, vector3, 0.22f, BuildTool._tmp_cols, 425984, QueryTriggerInteraction.Collide);
                var codeMatcher = new CodeMatcher(instructions)
                    .MatchForward(false, 
                        new CodeMatch(i=> i.opcode == OpCodes.Ldc_R4 && Math.Abs(Convert.ToSingle(i.operand) - 0.28f) < 0.001f)
                        )
                    .Repeat(
                        matcher =>
                        {
                            matcher.SetOperandAndAdvance(0.22f);
                            
                        }
                    );
                instructions = codeMatcher.InstructionEnumeration();
                return instructions;
            }
            catch (Exception e)
            {
                GS2.Warn("BuildTool_Path.CheckBuildConditions transpiler fail!");
                GS2.Warn(e.Message);
                GS2.Warn(e.Data.ToString());
                GS2.Warn(e.StackTrace);
                GS2.Warn(e.Source);
                return instructions;
            }
        }

        [HarmonyTranspiler, HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.CheckBuildConditions))]
        public static IEnumerable<CodeInstruction> CheckBuildConditions_Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                // replace : bool flag34 = base.ObjectIsBelt(buildPreview2.coverObjId); if (num33 > 0.1f) { ... }
                // with    : bool flag34 = base.ObjectIsBelt(buildPreview2.coverObjId); if (num33 > 0.3f) { ... }

                var codeMatcher2 = new CodeMatcher(instructions);
                codeMatcher2.MatchForward(true,
                new CodeMatch(i=>i.opcode == Ldarg_0),
                new CodeMatch(i=>i.opcode == Ldloc_S && i.operand.ToString().Contains("BuildPreview")),
                new CodeMatch(i=>i.opcode == Ldfld && i.operand.ToString().Contains("coverObjId")),
                new CodeMatch(i=>i.opcode == Call && i.operand.ToString().Contains("ObjectIsBelt")),
                new CodeMatch(i=>i.opcode == Stloc_S),
                new CodeMatch(i=>i.opcode == Ldloc_S),
                new CodeMatch(i=>i.opcode == Ldc_R4 && Math.Abs(Convert.ToSingle(i.operand) - 0.1f) < 0.001f)
                );
                // GS2.Error((!codeMatcher2.IsInvalid).ToString());
                // codeMatcher2.LogILPre();
                codeMatcher2.SetInstruction(new CodeInstruction(Ldc_R4, 0.3f));
                // codeMatcher2.LogILPost();
                return codeMatcher2.InstructionEnumeration();
            }
            catch (Exception e)
            {
                GS2.Warn("BuildTool_Path.CheckBuildConditions transpiler fail!");
                GS2.Warn(e.Message);
                GS2.Warn(e.Data.ToString());
                GS2.Warn(e.StackTrace);
                GS2.Warn(e.Source);
                return instructions;
            }
        }

    }
}