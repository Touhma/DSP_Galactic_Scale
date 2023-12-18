using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale

{
	public class PatchOnUnspecified_Debug
	{
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
		// [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_OrbitTarget_SLancer")]
		[HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackLaser_Large")]
		[HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackPlasma_Small")]
		// [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToHive_Space_FollowLeader")]
		// [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToTargetPoint_Space_FollowLeader")]
		// [HarmonyPatch(typeof(FleetComponent), "SensorLogic_Ground")]
		[HarmonyPatch(typeof(TurretComponent), "SetStateToAim_Default")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Gauss_Space")]
		[HarmonyPatch(typeof(TurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFSTurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFTinderComponent), "TinderSailLogic")]
		[HarmonyPatch(typeof(PlayerAction_Plant), "UpdateRaycast")]
		
		public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
		{
			instructions = new CodeMatcher(instructions)
				.MatchForward(
					true, 
					new CodeMatch(OpCodes.Ldc_I4, 200)
					)
				.Repeat(matcher =>
					{
						matcher.SetInstruction(Transpilers.EmitDelegate<Func<float>>(() =>
								{
									var planet = GameMain.localPlanet;
									return planet?.realRadius ?? 200;
								}
							)
						);
				}).InstructionEnumeration();
		
			return instructions;
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
		// [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToHive_Space_FollowLeader")]
		// [HarmonyPatch(typeof(EnemyUnitComponent), "SeekToTargetPoint_Space_FollowLeader")]
		// [HarmonyPatch(typeof(FleetComponent), "SensorLogic_Ground")]
		[HarmonyPatch(typeof(TurretComponent), "SetStateToAim_Default")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Gauss_Space")]
		[HarmonyPatch(typeof(TurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFSTurretComponent), "Shoot_Plasma")]
		[HarmonyPatch(typeof(DFTinderComponent), "TinderSailLogic")]
		[HarmonyPatch(typeof(PlayerAction_Plant), "UpdateRaycast")]
		
		public static IEnumerable<CodeInstruction> Fix200f(IEnumerable<CodeInstruction> instructions)
		{
			instructions = new CodeMatcher(instructions)
				.MatchForward(
					true, 
					new CodeMatch(OpCodes.Ldc_R4, 202f)
					)
				.Repeat(matcher =>
					{
						matcher.SetInstruction(Transpilers.EmitDelegate<Func<float>>(() =>
								{
									var planet = GameMain.localPlanet;
									return planet?.realRadius + 2f ?? 202f;
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