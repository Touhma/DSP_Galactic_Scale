using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale

{
	public class PatchOnUnspecified_Debug
	{
		[HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalculateVeinGroups")]
		public static bool CalculateVeinGroups(PlanetData __instance)
		{
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinCounts")]
		public static bool CalcVeinCounts(PlanetData __instance)
		{
			if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
			return true;
		}

		// [HarmonyPrefix, HarmonyPatch(typeof(MonitorComponent), "InternalUpdate")]
		// public static bool InternalUpdate(MonitorComponent __instance, CargoTraffic _traffic, bool sandbox,
		// 	EntityData[] _entityPool, SpeakerComponent[] _speakerPool, AnimData[] _animPool)
		// {
		// 	if (__instance.periodTickCount < 60)
		// 	{
		// 		__instance.SetPeriodTickCount(60);
		// 	}
		//
		// 	int num = 0;
		// 	__instance.cargoFlow -= (int)__instance.cargoBytesArray[0];
		// 	Array.Copy(__instance.cargoBytesArray, 1, __instance.cargoBytesArray, 0,
		// 		(int)(__instance.periodTickCount - 1));
		// 	if (__instance.periodCargoBytesArray == null ||
		// 	    __instance.periodCargoBytesArray.Length != __instance.cargoBytesArray.Length)
		// 	{
		// 		__instance.SetPeriodTickCount((int)__instance.periodTickCount);
		// 	}
		//
		// 	Array.Copy(__instance.periodCargoBytesArray, 1, __instance.periodCargoBytesArray, 0,
		// 		(int)(__instance.periodTickCount - 1));
		// 	__instance.cargoBytesArray[(int)(__instance.periodTickCount - 1)] = 0;
		// 	__instance.periodCargoBytesArray[(int)(__instance.periodTickCount - 1)] = __instance.cargoFlow;
		// 	int networkId = _traffic.factory.powerSystem.consumerPool[__instance.pcId].networkId;
		// 	PowerNetwork powerNetwork = _traffic.factory.powerSystem.netPool[networkId];
		// 	float num2 = (powerNetwork != null && networkId > 0) ? ((float)powerNetwork.consumerRatio) : 0f;
		// 	bool flag = num2 > 0.1f;
		// 	BeltComponent beltComponent = _traffic.beltPool[__instance.targetBeltId];
		// 	__instance.targetBeltId = beltComponent.id;
		// 	bool flag2 = __instance.targetBeltId > 0;
		// 	if (flag2)
		// 	{
		// 		int segPathId = beltComponent.segPathId;
		// 		CargoPath cargoPath = _traffic.GetCargoPath(segPathId);
		// 		int num3 = beltComponent.segIndex + beltComponent.segPivotOffset + (int)__instance.offset;
		// 		int num4 = num3 + 10;
		// 		int num5 = -1;
		// 		int num6 = -1;
		// 		int num7 = -1;
		// 		if (cargoPath != null)
		// 		{
		// 			if (__instance.prewarmSampleTick < __instance.periodTickCount * 2)
		// 			{
		// 				__instance.prewarmSampleTick += 1;
		// 			}
		//
		// 			Cargo cargo;
		// 			__instance.GetCargoAtIndexByFilter(__instance.cargoFilter, cargoPath, num3, out cargo, out num5,
		// 				out num6);
		// 			if (__instance.lastCargoId == -1 && num5 >= 0)
		// 			{
		// 				if (num5 == __instance.formerCargoId)
		// 				{
		// 					num -= (num6 + 1) * (int)cargo.stack;
		// 				}
		// 				else
		// 				{
		// 					num += (10 - num6 - 1) * (int)cargo.stack;
		// 				}
		// 			}
		// 			else if (__instance.lastCargoId >= 0 && num5 >= 0)
		// 			{
		// 				if (__instance.lastCargoId == num5)
		// 				{
		// 					num += (__instance.lastCargoOffset - num6) * (int)cargo.stack;
		// 				}
		// 				else if (__instance.formerCargoId == num5)
		// 				{
		// 					num += (__instance.lastCargoOffset + 1) * (int)__instance.lastCargoStack +
		// 					       (10 - num6 - 1) * (int)cargo.stack -
		// 					       (int)(10 * (__instance.lastCargoStack + cargo.stack));
		// 				}
		// 				else
		// 				{
		// 					num += (__instance.lastCargoOffset + 1) * (int)__instance.lastCargoStack +
		// 					       (10 - num6 - 1) * (int)cargo.stack;
		// 				}
		// 			}
		// 			else if (__instance.lastCargoId >= 0 && num5 == -1)
		// 			{
		// 				num += (__instance.lastCargoOffset + 1) * (int)__instance.lastCargoStack;
		// 			}
		//
		// 			if (num4 < cargoPath.pathLength)
		// 			{
		// 				Cargo cargo2;
		// 				__instance.GetCargoAtIndexByFilter(__instance.cargoFilter, cargoPath, num4, out cargo2,
		// 					out __instance.formerCargoId, out num7);
		// 			}
		// 			else
		// 			{
		// 				__instance.formerCargoId = -1;
		// 			}
		//
		// 			__instance.lastCargoId = num5;
		// 			__instance.lastCargoOffset = num6;
		// 			__instance.lastCargoStack = cargo.stack;
		// 			if (sandbox && __instance.spawnItemOperator > 0 && flag)
		// 			{
		// 				int num8;
		// 				if (beltComponent.speed == 1)
		// 				{
		// 					num8 = 10;
		// 				}
		// 				else if (beltComponent.speed == 2)
		// 				{
		// 					num8 = 5;
		// 				}
		// 				else
		// 				{
		// 					num8 = 2;
		// 				}
		//
		// 				double num9 = (double)__instance.targetCargoBytes / 10.0;
		// 				double num10 = (double)num2 * num9 / (double)__instance.periodTickCount;
		// 				__instance.spawnItemAccumulator += num10;
		// 				int num11 = (int)(num10 * (double)num8 + 0.99996);
		// 				if (num11 < 1)
		// 				{
		// 					num11 = 1;
		// 				}
		// 				else if (num11 > 4)
		// 				{
		// 					num11 = 4;
		// 				}
		//
		// 				if (__instance.spawnItemOperator == 2)
		// 				{
		// 					num11 = 4;
		// 				}
		//
		// 				if (__instance.spawnItemAccumulator >= (double)num11 + 0.99996)
		// 				{
		// 					__instance.spawnItemAccumulator = (double)num11 + 0.99996;
		// 				}
		//
		// 				int num12 = (int)(__instance.spawnItemAccumulator + 1E-08);
		// 				if (num12 >= num11)
		// 				{
		// 					num12 = num11;
		// 				}
		//
		// 				if (num12 > 0)
		// 				{
		// 					if (__instance.cargoFilter > 0 && __instance.spawnItemOperator == 1)
		// 					{
		// 						int num13 = 0;
		// 						int index = num3 + 10;
		// 						byte b;
		// 						byte b2;
		// 						if (cargoPath.QueryItemAtIndex(index, out b, out b2) == __instance.cargoFilter)
		// 						{
		// 							int cargoIdAtIndex = cargoPath.GetCargoIdAtIndex(index, 10);
		// 							int num14 = num11 - (int)b;
		// 							if (num14 > 0)
		// 							{
		// 								num13 = ((num12 >= num14) ? num14 : num12);
		// 								var cargoPool = cargoPath.cargoContainer.cargoPool;
		// 								byte b9 = (byte)num13;
		// 								cargoPool[cargoIdAtIndex].stack = (byte)(cargoPool[cargoIdAtIndex].stack + b9);
		// 							}
		// 						}
		// 						else if (cargoPath.TryInsertItem(index, __instance.cargoFilter, (byte)num12, 0))
		// 						{
		// 							num13 = num12;
		// 						}
		//
		// 						__instance.spawnItemAccumulator -= (double)num13;
		// 						num += num13 * 10;
		// 					}
		// 					else if (__instance.spawnItemOperator == 2)
		// 					{
		// 						int index2 = num3 - 10;
		// 						int cargoIdAtIndex2 = cargoPath.GetCargoIdAtIndex(index2, 10);
		// 						if (cargoIdAtIndex2 >= 0)
		// 						{
		// 							byte b3;
		// 							byte b4;
		// 							int num16 = cargoPath.QueryItemAtIndex(index2, out b3, out b4);
		// 							if (__instance.cargoFilter == 0 ||
		// 							    (__instance.cargoFilter > 0 && num16 == __instance.cargoFilter))
		// 							{
		// 								int num17;
		// 								if (num12 >= (int)b3)
		// 								{
		// 									cargoPath.RemoveCargoAtIndex(index2);
		// 									num17 = (int)b3;
		// 								}
		// 								else
		// 								{
		// 									Cargo[] cargoPool2 = cargoPath.cargoContainer.cargoPool;
		// 									int num18 = cargoIdAtIndex2;
		// 									cargoPool2[num18].stack = (byte)(cargoPool2[num18].stack - (byte)num12);
		// 									num17 = num12;
		// 								}
		//
		// 								__instance.spawnItemAccumulator -= (double)num17;
		// 								num += num17 * 10;
		// 							}
		// 						}
		// 					}
		// 				}
		// 			}
		// 		}
		// 		else
		// 		{
		// 			num = 0;
		// 		}
		//
		// 		__instance.cargoFlow += num;
		// 		__instance.totalCargoBytes += num;
		// 		__instance.cargoBytesArray[(int)(__instance.periodTickCount - 1)] = (sbyte)num;
		// 		__instance.periodCargoBytesArray[(int)(__instance.periodTickCount - 1)] = __instance.cargoFlow;
		// 		__instance.isSpeakerAlarming = (__instance.Alarming(_traffic, (int)__instance.alarmMode) && flag);
		// 		__instance.isSystemAlarming = __instance.Alarming(_traffic, __instance.systemWarningMode);
		// 		if (_speakerPool.Length > __instance.speakerId)
		// 		{
		// 			if (__instance.isSpeakerAlarming)
		// 			{
		// 				_speakerPool[__instance.speakerId].Play(ESpeakerPlaybackOrigin.Current, 0f);
		// 			}
		// 			else
		// 			{
		// 				_speakerPool[__instance.speakerId].Stop();
		// 			}
		// 		}
		// 	}
		// 	else
		// 	{
		// 		if (_speakerPool.Length > __instance.speakerId)
		// 		{
		// 			__instance.prewarmSampleTick = 0;
		// 			_speakerPool[__instance.speakerId].Stop();
		// 		}
		// 	}
		//
		// 	if (_speakerPool.Length > __instance.speakerId) _speakerPool[__instance.speakerId].SetPowerRatio(num2);
		// 	_animPool[__instance.entityId].prepare_length =
		// 		(float)((int)__instance.passColorId * 1000 + (int)__instance.failColorId);
		// 	_animPool[__instance.entityId].state = (uint)(__instance.totalCargoBytes / 10 & 33554431);
		// 	_animPool[__instance.entityId].working_length = (float)Mathf.FloorToInt((float)__instance.cargoFlow * 0.1f /
		// 		((float)__instance.periodTickCount / 3600f));
		// 	_animPool[__instance.entityId].power = num2;
		// 	if (flag2 && flag)
		// 	{
		// 		__instance.GetLogicState();
		// 		float num19 = Quaternion.Angle(_entityPool[__instance.entityId].rot,
		// 			_entityPool[beltComponent.entityId].rot);
		// 		_animPool[__instance.entityId].time = (float)(__instance.GetLogicState() +
		// 		                                              ((num19 < 0.001f) ? 20 : 10) + (flag2 ? 200 : 100) +
		// 		                                              ((__instance.prewarmSampleTick >=
		// 		                                                __instance.periodTickCount)
		// 			                                              ? 2000
		// 			                                              : 1000));
		// 		return false;
		// 	}
		//
		// 	_animPool[__instance.entityId].time = 100f;
		// 	return false;
		// }
		//
		//
		// [HarmonyPrefix, HarmonyPatch(typeof(PlanetFactory), "CreateEntityDisplayComponents", typeof(int))]
		// public static bool CreateEntityDisplayComponents(PlanetFactory __instance, int entityId)
		// {
		// 	int modelIndex = (int)__instance.entityPool[entityId].modelIndex;
		// 	ModelProto modelProto = LDB.models.Select(modelIndex);
		// 	if (modelProto == null)
		// 	{
		// 		return false;
		// 	}
		//
		// 	PrefabDesc prefabDesc = modelProto.prefabDesc;
		// 	if (prefabDesc == null)
		// 	{
		// 		return false;
		// 	}
		//
		// 	__instance.entityPool[entityId].modelId = GameMain.gpuiManager.AddModel(modelIndex, entityId,
		// 		__instance.entityPool[entityId].pos, __instance.entityPool[entityId].rot, true);
		// 	if (prefabDesc.minimapType > 0 && __instance.entityPool[entityId].mmblockId == 0)
		// 	{
		// 		if (__instance.entityPool[entityId].inserterId == 0)
		// 		{
		// 			__instance.entityPool[entityId].mmblockId = __instance.blockContainer.AddMiniBlock(entityId,
		// 				prefabDesc.minimapType, __instance.entityPool[entityId].pos,
		// 				__instance.entityPool[entityId].rot, prefabDesc.selectSize);
		// 		}
		// 		else
		// 		{
		// 			InserterComponent inserterComponent =
		// 				__instance.factorySystem.inserterPool[__instance.entityPool[entityId].inserterId];
		// 			Assert.Positive(inserterComponent.id);
		// 			Vector3 pos = Vector3.Lerp(__instance.entityPool[entityId].pos, inserterComponent.pos2, 0.5f);
		// 			Quaternion rot =
		// 				Quaternion.LookRotation(inserterComponent.pos2 - __instance.entityPool[entityId].pos,
		// 					pos.normalized);
		// 			Vector3 scl = new Vector3(0.7f, 0.7f,
		// 				Vector3.Distance(inserterComponent.pos2, __instance.entityPool[entityId].pos) * 0.5f + 0.2f);
		// 			__instance.entityPool[entityId].mmblockId =
		// 				__instance.blockContainer.AddMiniBlock(entityId, prefabDesc.minimapType, pos, rot, scl);
		// 		}
		// 	}
		//
		// 	if (prefabDesc.colliders != null && prefabDesc.colliders.Length != 0)
		// 	{
		// 		for (int i = 0; i < prefabDesc.colliders.Length; i++)
		// 		{
		// 			if (__instance.entityPool[entityId].inserterId != 0)
		// 			{
		// 				ColliderData colliderData = prefabDesc.colliders[i];
		// 				InserterComponent inserterComponent2 =
		// 					__instance.factorySystem.inserterPool[__instance.entityPool[entityId].inserterId];
		// 				Assert.Positive(inserterComponent2.id);
		// 				Vector3 wpos = Vector3.Lerp(__instance.entityPool[entityId].pos, inserterComponent2.pos2, 0.5f);
		// 				Quaternion wrot =
		// 					Quaternion.LookRotation(inserterComponent2.pos2 - __instance.entityPool[entityId].pos,
		// 						wpos.normalized);
		// 				colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y,
		// 					Mathf.Max(0.1f,
		// 						Vector3.Distance(inserterComponent2.pos2, __instance.entityPool[entityId].pos) * 0.5f +
		// 						colliderData.ext.z));
		// 				__instance.entityPool[entityId].colliderId = __instance.planet.physics.AddColliderData(
		// 					colliderData.BindToObject(entityId, __instance.entityPool[entityId].colliderId,
		// 						EObjectType.Entity, wpos, wrot));
		// 			}
		// 			else if (__instance.entityPool[entityId].beltId != 0)
		// 			{
		// 				if (__instance.entityPool[entityId].colliderId == 0)
		// 				{
		// 					__instance.entityPool[entityId].colliderId = __instance.planet.physics.AddColliderData(
		// 						prefabDesc.colliders[i].BindToObject(entityId, 0, EObjectType.Entity,
		// 							__instance.entityPool[entityId].pos, __instance.entityPool[entityId].rot));
		// 				}
		// 			}
		// 			else
		// 			{
		// 				__instance.entityPool[entityId].colliderId = __instance.planet.physics.AddColliderData(
		// 					prefabDesc.colliders[i].BindToObject(entityId, __instance.entityPool[entityId].colliderId,
		// 						EObjectType.Entity, __instance.entityPool[entityId].pos,
		// 						__instance.entityPool[entityId].rot));
		// 			}
		// 		}
		// 	}
		//
		// 	if (prefabDesc.hasAudio)
		// 	{
		// 		__instance.entityPool[entityId].audioId = __instance.planet.audio.AddAudioData(entityId,
		// 			EObjectType.Entity, __instance.entityPool[entityId].pos, prefabDesc);
		// 		if (__instance.entityPool[entityId].speakerId > 0)
		// 		{
		// 			if (__instance.digitalSystem.speakerPool.Length <= __instance.entityPool[entityId].speakerId)
		// 			{
		// 				return false;
		// 			}
		//
		// 			SpeakerComponent speakerComponent =
		// 				__instance.digitalSystem.speakerPool[__instance.entityPool[entityId].speakerId];
		// 			if (speakerComponent.id == __instance.entityPool[entityId].speakerId)
		// 			{
		// 				__instance.planet.audio.ChangeAudioSpeakerInfo(__instance.entityPool[entityId].audioId,
		// 					speakerComponent.audioId, speakerComponent.oneShotAudioId, speakerComponent.repeatTimes);
		// 				__instance.planet.audio.ChangeAudioDataFalloff(__instance.entityPool[entityId].audioId,
		// 					speakerComponent.falloffRadius0, speakerComponent.falloffRadius1);
		// 			}
		// 		}
		// 	}
		//
		// 	return false;
		// }
		[HarmonyPostfix, HarmonyPatch(typeof(GameData), "Import")]
		public static void GameData_Import(GameData __instance)
		{
			for (int i = 0; i < __instance.factoryCount; i++)
			{
				PlanetFactory factory = __instance.factories[i];
				for (int monitorId = 1; monitorId < factory.cargoTraffic.monitorCursor; monitorId++)
				{
					if (factory.cargoTraffic.monitorPool[monitorId].id == monitorId)
					{
						if (factory.cargoTraffic.monitorPool[monitorId].speakerId >= factory.digitalSystem.speakerCursor)
						{
							int speakerId = factory.cargoTraffic.monitorPool[monitorId].speakerId;
							int entityId = factory.cargoTraffic.monitorPool[monitorId].entityId;
							GS2.Warn($"{factory.planet.displayName}: Remove monitor {monitorId} for speakerId {speakerId} is out of bound");

							factory.entityPool[entityId].speakerId = 0;
							if (factory.entityPool[entityId].warningId >= __instance.warningSystem.warningCursor)
								factory.entityPool[entityId].warningId = 0;
							factory.RemoveEntityWithComponents(entityId);
						}
					}
				}
			}
		}

		[HarmonyPostfix, HarmonyPatch(typeof(WarningSystem), "Import")]
		public static void WarningSystem_Import(WarningSystem __instance)
		{
			if (__instance.warningCursor <= 0)
			{
				GS2.Warn("Reset WarningSystem");
				__instance.SetForNewGame();
			}
		}

		[HarmonyPostfix, HarmonyPatch(typeof(DigitalSystem), "Import")]
		public static void DigitalSystem_Import(DigitalSystem __instance)
		{
			if (__instance.speakerCursor <= 0)
			{
				GS2.Warn($"Reset DigitalSystem of {__instance.planet.displayName}");
				__instance.speakerCursor = 1;
				__instance.speakerRecycleCursor = 0;
				__instance.SetSpeakerCapacity(256);
			}
		}
	}
}