using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
	
	public class PatchOnUnspecified_Debug
	{
		// [HarmonyPrefix, HarmonyPatch(typeof(PlanetAlgorithm), "CalcLandPercent")]
		// public static bool CalcLandPercent(PlanetData _planet)
		// {
		// 	GS2.Log("Calculating Land Percent");
		// 	if (_planet == null)
		// 	{
		// 		return false;
		// 	}
		// 	PlanetRawData data = _planet.data;
		// 	if (data == null)
		// 	{
		// 		return false;
		// 	}
		// 	int stride = data.stride;
		// 	int num = stride / 2;
		// 	int dataLength = data.dataLength;
		// 	ushort[] heightData = data.heightData;
		// 	if (heightData == null)
		// 	{
		// 		return false;
		// 	}
		// 	float num2 = _planet.radius * 100f - 20f;
		// 	if (_planet.type == EPlanetType.Gas)
		// 	{
		// 		_planet.landPercent = 0f;
		// 		return false;
		// 	}
		// 	int num3 = 0;
		// 	int num4 = 0;
		// 	for (int i = 0; i < dataLength; i++)
		// 	{
		// 		int num5 = i % stride;
		// 		int num6 = i / stride;
		// 		if (num5 > num)
		// 		{
		// 			num5--;
		// 		}
		// 		if (num6 > num)
		// 		{
		// 			num6--;
		// 		}
		// 		if ((num5 & 1) == 1 && (num6 & 1) == 1)
		// 		{
		// 			if ((float)heightData[i] >= num2)
		// 			{
		// 				num4++;
		// 			}
		// 			else if (data.GetModLevel(i) == 3)
		// 			{
		// 				num4++;
		// 			}
		// 			num3++;
		// 		}
		// 	}
		// 	_planet.landPercent = ((num3 > 0) ? ((float)num4 / (float)num3) : 0f);
		// 	return false;
		// }
		
// 		[HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "UpdateRuntimePose")]
// 		public static bool UpdateRuntimePose(ref PlanetData __instance, double time)
// {
// 	double num = time / __instance.orbitalPeriod + (double)__instance.orbitPhase / 360.0;
// 	int num2 = (int)(num + 0.1);
// 	num -= (double)num2;
// 	__instance.runtimeOrbitPhase = (float)num * 360f;
// 	num *= 6.283185307179586;
// 	double num3 = time / __instance.rotationPeriod + (double)__instance.rotationPhase / 360.0;
// 	int num4 = (int)(num3 + 0.1);
// 	num3 = (num3 - (double)num4) * 360.0;
// 	__instance.runtimeRotationPhase = (float)num3;
// 	VectorLF3 vectorLF = Maths.QRotateLF(__instance.runtimeOrbitRotation, new VectorLF3((float)Math.Cos(num) * __instance.orbitRadius, 0f, (float)Math.Sin(num) * __instance.orbitRadius));
// 	if (__instance.orbitAroundPlanet != null)
// 	{
// 		vectorLF.x += __instance.orbitAroundPlanet.runtimePosition.x;
// 		vectorLF.y += __instance.orbitAroundPlanet.runtimePosition.y;
// 		vectorLF.z += __instance.orbitAroundPlanet.runtimePosition.z;
// 	}
// 	__instance.runtimePosition = vectorLF;
// 	__instance.runtimeRotation = __instance.runtimeSystemRotation * Quaternion.AngleAxis((float)num3, Vector3.down);
// 	__instance.uPosition.x = __instance.star.uPosition.x + vectorLF.x * 40000.0;
// 	__instance.uPosition.y = __instance.star.uPosition.y + vectorLF.y * 40000.0;
// 	__instance.uPosition.z = __instance.star.uPosition.z + vectorLF.z * 40000.0;
// 	__instance.runtimeLocalSunDirection = Maths.QInvRotate(__instance.runtimeRotation, -vectorLF);
// 	double num5 = time + 0.016666666666666666;
// 	double num6 = num5 / __instance.orbitalPeriod + (double)__instance.orbitPhase / 360.0;
// 	int num7 = (int)(num6 + 0.1);
// 	num6 -= (double)num7;
// 	num6 *= 6.283185307179586;
// 	double num8 = num5 / __instance.rotationPeriod + (double)__instance.rotationPhase / 360.0;
// 	int num9 = (int)(num8 + 0.1);
// 	num8 = (num8 - (double)num9) * 360.0;
// 	VectorLF3 vectorLF2 = Maths.QRotateLF(__instance.runtimeOrbitRotation, new VectorLF3((float)Math.Cos(num6) * __instance.orbitRadius, 0f, (float)Math.Sin(num6) * __instance.orbitRadius));
// 	if (__instance.orbitAroundPlanet != null)
// 	{
// 		vectorLF2.x += __instance.orbitAroundPlanet.runtimePositionNext.x;
// 		vectorLF2.y += __instance.orbitAroundPlanet.runtimePositionNext.y;
// 		vectorLF2.z += __instance.orbitAroundPlanet.runtimePositionNext.z;
// 	}
// 	__instance.runtimePositionNext = vectorLF2;
// 	__instance.runtimeRotationNext = __instance.runtimeSystemRotation * Quaternion.AngleAxis((float)num8, Vector3.down);
// 	__instance.uPositionNext.x = __instance.star.uPosition.x + vectorLF2.x * 40000.0;
// 	__instance.uPositionNext.y = __instance.star.uPosition.y + vectorLF2.y * 40000.0;
// 	__instance.uPositionNext.z = __instance.star.uPosition.z + vectorLF2.z * 40000.0;
// 	if (__instance.id >= __instance.galaxy.astrosFactory.Length) GS2.Log($"ID:{__instance.id}/{__instance.galaxy.astrosFactory.Length}");
// 	__instance.galaxy.astrosData[__instance.id].uPos = __instance.uPosition;
// 	__instance.galaxy.astrosData[__instance.id].uRot = __instance.runtimeRotation;
// 	__instance.galaxy.astrosData[__instance.id].uPosNext = __instance.uPositionNext;
// 	__instance.galaxy.astrosData[__instance.id].uRotNext = __instance.runtimeRotationNext;
// 	__instance.galaxy.astrosFactory[__instance.id] = __instance.factory;
// 	return false;
// }
		[HarmonyPrefix, HarmonyPatch(typeof(SectorModel), "CreateGalaxyAstroBuffer")]
		public static bool CreateGalaxyAstroBuffer(ref SectorModel __instance)
		{
			var size = 25700 * Mathf.CeilToInt(GameMain.galaxy.starCount / 64);
			size = Mathf.Max(size, 25700);
			GS2.Warn("New Size = "+size);
			__instance.galaxyAstroArr = new AstroPoseR[size];
			__instance.galaxyAstroBuffer = new ComputeBuffer(size, 32, ComputeBufferType.Default);
			for (int i = 0; i < __instance.galaxyAstroArr.Length; i++)
			{
				__instance.galaxyAstroArr[i].rpos.x = 0f;
				__instance.galaxyAstroArr[i].rpos.y = 0f;
				__instance.galaxyAstroArr[i].rpos.z = 0f;
				__instance.galaxyAstroArr[i].rrot.x = 0f;
				__instance.galaxyAstroArr[i].rrot.y = 0f;
				__instance.galaxyAstroArr[i].rrot.z = 0f;
				__instance.galaxyAstroArr[i].rrot.w = 1f;
				__instance.galaxyAstroArr[i].radius = 0f;
			}
			__instance.starmapGalaxyAstroArr = new AstroPoseR[size];
			__instance.starmapGalaxyAstroBuffer = new ComputeBuffer(size, 32, ComputeBufferType.Default);
			Array.Copy(__instance.galaxyAstroArr, __instance.starmapGalaxyAstroArr, __instance.galaxyAstroArr.Length);
			return false;
		}
		[HarmonyPrefix, HarmonyPatch(typeof(SpaceSector), "SetForNewGame")]
		public static bool SetForNewGame(ref SpaceSector __instance)
		{
			__instance.SetAstroCapacity(1024*Mathf.CeilToInt(__instance.galaxy.starCount/64f)); //0.10
			GS2.Log("NEW ASTRO CAPACITY:" + __instance.astros.Length);
			__instance.astroCursor = 1;
			for (int i = 0; i < __instance.maxHiveCount; i++)
			{
				__instance.NewAstroData(new AstroData
				{
					type = EAstroType.EnemyHive
				});
			}
			__instance.SetEnemyCapacity(__instance.isCombatMode ? 16384 : 32);
			__instance.enemyCursor = 1;
			__instance.enemyRecycleCursor = 0;
			__instance.SetCraftCapacity(64);
			__instance.craftCursor = 1;
			__instance.craftRecycleCursor = 0;
			__instance.spaceRuins.Reset();
			__instance.skillSystem.SetForNewGame();
			int starCount = __instance.galaxy.starCount;
			__instance.dfHives = new EnemyDFHiveSystem[starCount];
			if (__instance.isCombatMode)
			{
				HighStopwatch highStopwatch = new HighStopwatch();
				highStopwatch.Begin();
				for (int j = 0; j < starCount; j++)
				{
					__instance.dfHives[j] = null;
					int num = __instance.galaxy.stars[j].initialHiveCount;
					if (j >= 100)
					{
						num = 0;
					}
					EnemyDFHiveSystem enemyDFHiveSystem = null;
					for (int k = 0; k < num; k++)
					{
						EnemyDFHiveSystem enemyDFHiveSystem2 = new EnemyDFHiveSystem();
						enemyDFHiveSystem2.Init(__instance.gameData, __instance.galaxy.stars[j].id, k);
						if (enemyDFHiveSystem == null)
						{
							__instance.dfHives[j] = enemyDFHiveSystem2;
						}
						else
						{
							enemyDFHiveSystem.nextSibling = enemyDFHiveSystem2;
						}
						enemyDFHiveSystem2.prevSibling = enemyDFHiveSystem;
						enemyDFHiveSystem = enemyDFHiveSystem2;
						enemyDFHiveSystem2.SetForNewGame();
					}
				}
				double duration = highStopwatch.duration;
				GS2.Log(string.Format("Initialize and generate space enemy complete, time cost = {0:0.00} ms", duration * 1000.0));
			}
			__instance.combatSpaceSystem = new CombatSpaceSystem(__instance);
			return false;
		}
		
		
		[HarmonyPrefix, HarmonyPatch(typeof(DysonSphere), "Init")]
		
		public static bool Init(DysonSphere __instance, GameData _gameData, StarData _starData)
{
	__instance.gameData = _gameData;
	__instance.starData = _starData;
	ProductionStatistics production =__instance.gameData.statistics.production;
	int[] firstCreateIds = production.firstCreateIds;
	int num = firstCreateIds.Length;
	int num2 = 0;
	for (int i = 0; i < num; i++)
	{
		int num3 = firstCreateIds[i] / 100;
		if (__instance.starData.id == num3)
		{
			num2 =__instance.gameData.galaxy.PlanetById(firstCreateIds[i]).factoryIndex;
			break;
		}
	}
	FactoryProductionStat factoryProductionStat = production.factoryStatPool[num2];
	__instance.productRegister = factoryProductionStat.productRegister;
	__instance.consumeRegister = factoryProductionStat.consumeRegister;
	__instance.sunColor = Color.white;
	__instance.energyGenPerSail = Configs.freeMode.solarSailEnergyPerTick;
	__instance.energyGenPerNode = Configs.freeMode.dysonNodeEnergyPerTick;
	__instance.energyGenPerFrame = Configs.freeMode.dysonFrameEnergyPerTick;
	__instance.energyGenPerShell = Configs.freeMode.dysonShellEnergyPerTick;
	if (__instance.starData != null)
	{
		float num4 = 4f;
		__instance.gravity = (float)(86646732.73933044 * (double)__instance.starData.mass) * num4;
		double num5 = (double)__instance.starData.dysonLumino;
		__instance.energyGenPerSail = (long)((double)__instance.energyGenPerSail * num5);
		__instance.energyGenPerNode = (long)((double)__instance.energyGenPerNode * num5);
		__instance.energyGenPerFrame = (long)((double)__instance.energyGenPerFrame * num5);
		__instance.energyGenPerShell = (long)((double)__instance.energyGenPerShell * num5);
		__instance.sunColor = Configs.builtin.dysonSphereSunColors.Evaluate(__instance.starData.color);
		__instance.emissionColor = Configs.builtin.dysonSphereEmissionColors.Evaluate(__instance.starData.color);
		if (__instance.starData.type == EStarType.NeutronStar)
		{
			__instance.sunColor = Configs.builtin.dysonSphereNeutronSunColor;
			__instance.emissionColor = Configs.builtin.dysonSphereNeutronEmissionColor;
		}
		__instance.defOrbitRadius = (float)((double)__instance.starData.dysonRadius * 40000.0);
		__instance.minOrbitRadius =__instance.starData.physicsRadius * 1.5f;
		if (__instance.minOrbitRadius < 4000f)
		{
			__instance.minOrbitRadius = 4000f;
		}
		__instance.maxOrbitRadius =__instance.defOrbitRadius * 2f;
		if (__instance.starData.planets.Length != 0)
			__instance.avoidOrbitRadius = (float) ((double)__instance.starData.planets[0].orbitRadius * 40000.0);
		else __instance.avoidOrbitRadius = 4000000;
		if (__instance.starData.type == EStarType.GiantStar)
		{
			__instance.minOrbitRadius *= 0.6f;
		}
		__instance.defOrbitRadius = Mathf.Round(__instance.defOrbitRadius / 100f) * 100f;
		__instance.minOrbitRadius = Mathf.Ceil(__instance.minOrbitRadius / 100f) * 100f;
		__instance.maxOrbitRadius = Mathf.Round(__instance.maxOrbitRadius / 100f) * 100f;
		__instance.randSeed =__instance.starData.seed;
	}
	__instance.swarm = new DysonSwarm(__instance);
	__instance.swarm.Init();
	__instance.layerCount = 0;
	__instance.layersSorted = new DysonSphereLayer[10];
	__instance.layersIdBased = new DysonSphereLayer[11];
	__instance.rocketCapacity = 0;
	__instance.rocketCursor = 1;
	__instance.rocketRecycleCursor = 0;
	__instance.autoNodes = new DysonNode[8];
	__instance.autoNodeCount = 0;
	__instance.nrdCapacity = 0;
	__instance.nrdCursor = 1;
	__instance.nrdRecycleCursor = 0;
	__instance.modelRenderer = new DysonSphereSegmentRenderer(__instance);
	__instance.modelRenderer.Init();
	__instance.rocketRenderer = new DysonRocketRenderer(__instance);
	__instance.inEditorRenderMaskL = -1;
	__instance.inEditorRenderMaskS = -1;
	__instance.inGameRenderMaskL = -1;
	__instance.inGameRenderMaskS = -1;
	return false;
}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(EnemyData), "Formation", new Type[] {typeof(int), typeof(EnemyData), typeof(float), typeof(VectorLF3), typeof(Quaternion), typeof(Vector3)}, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Ref})]
		[HarmonyPatch(typeof(TurretComponent), "CheckEnemyIsInAttackRange")]
		[HarmonyPatch(typeof(DFGTurretComponent), "Aim")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "Attack_SLancer")]
		[HarmonyPatch(typeof(GrowthTool_Node_DFGround), "CreateNode7")]
		[HarmonyPatch(typeof(PlayerNavigation), "DetermineArrive")]
		[HarmonyPatch(typeof(DFRelayComponent), "RelaySailLogic")]
		[HarmonyPatch(typeof(PlayerAction_Navigate), "GameTick")]
		[HarmonyPatch(typeof(FleetComponent), "GetUnitOrbitingAstroPose")]
		[HarmonyPatch(typeof(PlayerNavigation), "Init")]
		[HarmonyPatch(typeof(PlanetEnvironment), "LateUpdate")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRanger")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_SHumpback")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_OrbitTarget_SLancer")]
		[HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackLaser_Large")]
		[HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackPlasma_Small")]
		[HarmonyPatch(typeof(TurretComponent), "SetStateToAim_Default")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Gauss_Space")]
		[HarmonyPatch(typeof(TurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFSTurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFTinderComponent), "TinderSailLogic")]
		[HarmonyPatch(typeof(PlayerAction_Plant), "UpdateRaycast")]
		[HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
		public static IEnumerable<CodeInstruction> Fix200f(IEnumerable<CodeInstruction> instructions)
		{
			instructions = new CodeMatcher(instructions)
				.MatchForward(
					true, 
					new CodeMatch(i =>
					{
						if (i.opcode != Ldc_I4 && i.opcode != Ldc_R4 && i.opcode != Ldc_R8) return false;

						if (!float.TryParse(i.operand.ToString(), out float num)) return false;
						return num > 199.99f && num < 260f;
					}))
				.Repeat(matcher =>
					{
						matcher.SetInstruction(Transpilers.EmitDelegate<Func<float>>(() =>
												{
													GS2.Log("Function is working idiot");
													var planet = GameMain.localPlanet;
													return planet?.realRadius ?? 200f;
												}
											)
										);
				}).InstructionEnumeration();
		
			return instructions;
		}


		[HarmonyPrefix, HarmonyPatch(typeof(TestCombatDetails), "Update")]
		public static bool TestCombatDetailsUpdate()
		{
			GS2.Log("TCDUpdate");
			return true;
		}
		[HarmonyPrefix, HarmonyPatch(typeof(TestEnemyGenerate), "Update")]
		public static bool TestEnemyGenerateUpdate()
		{
			GS2.Log("TEGUpdate");
			return true;
		}
		[HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalculateVeinGroups")]
		public static bool CalculateVeinGroups(PlanetData __instance)
		{
			if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
			return false;
		}

		[HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinCounts")]
		public static bool CalcVeinCounts(PlanetData __instance)
		{
			if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
			return true;
		}

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
							factory.RemoveEntityWithComponents(entityId, false);
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