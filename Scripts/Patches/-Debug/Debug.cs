using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using static GalacticScale.GS2;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;
using Logger = BepInEx.Logging.Logger;

namespace GalacticScale

{
    public class PatchOnUnspecified_Debug
    {
	    [HarmonyPrefix, HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.SetForNewCreate))]
	    public static bool SetForNewCreate(ref EnemyDFHiveSystem __instance)
	    {
		    Log($"{__instance.starData == null} {__instance.starData.hiveAstroOrbits == null}");
		    Log($"{__instance.hiveOrbitIndex.ToString()}/{__instance.starData.hiveAstroOrbits.Length}");
		    Log($"--{__instance.starData.index * 8 + __instance.hiveOrbitIndex + 1}/{__instance.sector.astros.Length}");
		    Log(":)");
		    return true;
	    }
	    
 //        [HarmonyPrefix, HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.CreateNativeRelays))]
 //        public static bool CreateNativeRelays(ref EnemyDFHiveSystem __instance)
	// {
	// 	int num = 2 - __instance.starData.initialHiveCount;//num is the number of relays to create?
	// 	if (num < 0)
	// 	{
	// 		num = (num - 2) / 2;
	// 	}
	// 	else
	// 	{
	// 		num *= 2;
	// 	}
	// 	if (num < -3)
	// 	{
	// 		num = -3;
	// 	}
	// 	float virtualHiveMatterDamandByTicks = EnemyDFConfigs.VirtualHiveMatterDamandByTicks(__instance.TicksToBuildChances(__instance.ticks));
	// 	float sixtyf = 60f;
	// 	float safetyFactor = ((1f - __instance.starData.safetyFactor) * 0.6f + 0.8f) * (1f + 0.125f * (__instance.history.combatSettings.maxDensity - 1f));
	// 	int totalRelays = Mathf.CeilToInt((virtualHiveMatterDamandByTicks + 20f) / sixtyf * safetyFactor) + __instance.ticks / 54000 + num;//num5 is the number of relays to create?
	// 	
	// 	int maxHiveCount = __instance.starData.maxHiveCount;
	// 	int relaysPerPlanet = ((maxHiveCount > 5) ? (12 - maxHiveCount) : 7); //num6 is the number of relays to create?
	// 	if (relaysPerPlanet < 1)
	// 	{
	// 		relaysPerPlanet = 1;
	// 	}
	// 	int num7 = (__instance.ticks - 300) / 600; //num7 is the number of relays to create?
	// 	int idleRelaysToCreate = num7 - totalRelays;
	// 	if (idleRelaysToCreate < 0)
	// 	{
	// 		idleRelaysToCreate = 0;
	// 		totalRelays = num7;
	// 	}
	// 	else if (idleRelaysToCreate > __instance.relayDocks.Length)
	// 	{
	// 		idleRelaysToCreate = __instance.relayDocks.Length;
	// 	}
	// 	
	// 	GS2.Log("Creating Relays: " + totalRelays + " idleRelaysToCreate:" + idleRelaysToCreate + " num7:" + num7 + " relaysPerPlanet:" + relaysPerPlanet + " num:" + num 
	// 	        + " safetyFactor:" + safetyFactor + " vhmdbt:" + virtualHiveMatterDamandByTicks + " ticks:" + __instance.ticks + __instance.starData.maxHiveCount + " " + __instance.starData.initialHiveCount 
	// 	        + " " + __instance.starData.id + " " + __instance.starData.name + " " + __instance.starData.planetCount + " " + __instance.starData.type + " " + __instance.starData.resourceCoef + " " + __instance.starData.orbitScaler + " " + __instance.starData.dysonRadius + " " + __instance.starData.acdiskRadius + " " + __instance.starData.habitableRadius + " " + __instance.starData.lightBalanceRadius + " " + __instance.starData.luminosity + " " + __instance.starData.temperature + " " + __instance.starData.mass + " " + __instance.starData.radius + " " + __instance.starData.age + " " + __instance.starData.lifetime + " " + __instance.starData.color + " " + __instance.starData.classFactor + " " + __instance.starData.spectr);
	// 	
	// 	var random = new GS2.Random(__instance.seed);
	// 	for (int i = 0; i < __instance.starData.planetCount; i++) //Do this for every planet
	// 	{
	// 		PlanetData planetData = __instance.starData.planets[i];
	// 		if (planetData.type != EPlanetType.Gas) //as long as its not a gas planet
	// 		{
	// 			bool lastPlanet = i == __instance.starData.planetCount - 1;
	// 			bool birthStar = __instance.galaxy.birthStarId == __instance.starData.id;
	// 			bool birthPlanet = __instance.galaxy.birthPlanetId == planetData.id;
	// 			int relaysToCreate = (birthPlanet ? 1 : (lastPlanet ? totalRelays : ((totalRelays + 1) / 2)));//num9 is the number of relays to create?
	// 			if (relaysToCreate > relaysPerPlanet)
	// 			{
	// 				relaysToCreate = relaysPerPlanet;
	// 			}
	// 			if (relaysToCreate > num7)
	// 			{
	// 				relaysToCreate = num7;
	// 			}
	// 			totalRelays -= relaysToCreate;
	// 			int num10 = 0;
	// 			VectorLF3 vectorLF = VectorLF3.zero;
	// 			for (int j = 0; j < relaysToCreate; j++)
	// 			{
	// 				double r1 = random.NextDouble();
	// 				double r2 = random.NextDouble();
	// 				int attempts = 0;//num13 is the number of times we've tried to find a valid position
	// 				double relayXpos = 0.0;//num14 is the x position of the relay
	// 				double relayYpos = 0.0;//num15 is the y position of the relay
	// 				double relayZpos = 0.0;//num16 is the z position of the relay
	// 				bool validRelayPositionFound = false;
	// 				do
	// 				{
	// 					attempts++;
	// 					double distToPlanetCenter = 0.0;//num17 is the distance from the center of the planet
	// 					while (distToPlanetCenter == 0.0 || distToPlanetCenter > 1.0)//while the distance is 0 or greater than 1
	// 					{
	// 						relayXpos = random.NextDouble() * 2.0 - 1.0;
	// 						relayYpos = random.NextDouble() * 2.0 - 1.0;
	// 						relayZpos = random.NextDouble() * 2.0 - 1.0;
	// 						distToPlanetCenter = relayXpos * relayXpos + relayYpos * relayYpos + relayZpos * relayZpos;
	// 					}
	// 					if ((double)planetData.veinBiasVector.sqrMagnitude < 0.1)
	// 					{
	// 						planetData.GenVeinBiasVector();
	// 					}
	// 					VectorLF3 vectorLF2 = (birthPlanet ? (planetData.veinBiasVector * 2f) : (planetData.veinBiasVector * 0.97f));
	// 					relayXpos -= vectorLF2.x;
	// 					relayYpos -= vectorLF2.y;
	// 					relayZpos -= vectorLF2.z;
	// 					distToPlanetCenter = relayXpos * relayXpos + relayYpos * relayYpos + relayZpos * relayZpos;
	// 					distToPlanetCenter = Math.Sqrt(distToPlanetCenter);
	// 					vectorLF = -vectorLF2.normalized;
	// 					double relayHeightCoef = (double)(planetData.realRadius + 70f) / distToPlanetCenter; 
	// 					relayXpos *= relayHeightCoef;
	// 					relayYpos *= relayHeightCoef;
	// 					relayZpos *= relayHeightCoef;
	// 					if (!__instance.CheckPositionCollideRelay(planetData.astroId, relayXpos, relayYpos, relayZpos))
	// 					{
	// 						validRelayPositionFound = true;
	// 					}
	// 					if (attempts >= 80)
	// 					{
	// 						Debug.LogWarning(string.Format("生成 Relay 经过多次随机仍无法找到坐标 planetId = {0}", planetData.id));
	// 					}
	// 				}
	// 				while (!validRelayPositionFound && attempts < 80);
	// 				if (validRelayPositionFound)
	// 				{
	// 					VectorLF3 vectorLF3 = new VectorLF3(relayXpos, relayYpos, relayZpos);
	// 					int enemyFinal = __instance.sector.CreateEnemyFinal(__instance, 8116, planetData.astroId, vectorLF3, Maths.SphericalRotation(vectorLF3, (float)r2 * 360f));
	// 					int dfRelayId = __instance.sector.enemyPool[enemyFinal].dfRelayId;
	// 					DFRelayComponent dfrelayComponent = __instance.relays.buffer[dfRelayId];
	// 					Assert.True(dfrelayComponent != null && dfRelayId > 0 && dfRelayId == dfrelayComponent.id);
	// 					if (dfrelayComponent != null)
	// 					{
	// 						dfrelayComponent.SetDockIndex(num10++);
	// 						dfrelayComponent.hiveAstroId = __instance.hiveAstroId;
	// 						dfrelayComponent.targetAstroId = planetData.astroId;
	// 						dfrelayComponent.targetLPos = vectorLF3;
	// 						dfrelayComponent.targetYaw = (float)r2 * 360f;
	// 						dfrelayComponent.baseState = 1;
	// 						dfrelayComponent.baseId = 0;
	// 						double num20 = VectorLF3.Dot(vectorLF3.normalized, vectorLF);
	// 						num20 = Maths.Clamp01((num20 + 1.0) * 0.5);
	// 						num20 = Math.Pow(num20, 0.5);
	// 						if (birthPlanet)
	// 						{
	// 							dfrelayComponent.baseTicks = (int)(3000f * (float)(r1 * 0.05 + 0.12) + 120.5f);
	// 						}
	// 						else if (birthStar)
	// 						{
	// 							dfrelayComponent.baseTicks = (int)(3000f * (float)(Math.Pow(r1 * 0.5 + 0.5, 1.5) * num20) + 150.5f);
	// 						}
	// 						else
	// 						{
	// 							dfrelayComponent.baseTicks = (int)(6400f * (float)(Math.Pow(r1, 2.0) * num20) + 200.5f);
	// 						}
	// 						dfrelayComponent.baseEvolve = __instance.evolve;
	// 						dfrelayComponent.baseEvolve.threat = 0;
	// 						dfrelayComponent.baseEvolve.waves = (birthPlanet ? 0 : 1);
	// 						dfrelayComponent.direction = 0;
	// 						dfrelayComponent.stage = 2;
	// 						int num21 = random.Next(180001) * 100;
	// 						int builderId = dfrelayComponent.builderId;
	// 						__instance.builders.buffer[builderId].energy = __instance.builders.buffer[builderId].maxEnergy + num21;
	// 						__instance.sector.enemyAnimPool[enemyFinal].time = 1f;
	// 						__instance.sector.enemyAnimPool[enemyFinal].state = 1U;
	// 						__instance.sector.enemyAnimPool[enemyFinal].power = -1f;
	// 					}
	// 				}
	// 			}
	// 		}
	// 	}
	// 	__instance.idleRelayCount = 0;
	// 	for (int k = 0; k < idleRelaysToCreate; k++)
	// 	{
	// 		ref DFDock ptr = ref __instance.relayDocks[k % __instance.relayDocks.Length];
	// 		int enemyId = __instance.sector.CreateEnemyFinal(__instance, 8116, __instance.hiveAstroId, ptr.pos, ptr.rot);
	// 		int dfRelayId2 = __instance.sector.enemyPool[enemyId].dfRelayId;
	// 		DFRelayComponent dfrelayComponent2 = __instance.relays.buffer[dfRelayId2];
	// 		Assert.True(dfrelayComponent2 != null && dfRelayId2 > 0 && dfRelayId2 == dfrelayComponent2.id);
	// 		if (dfrelayComponent2 != null)
	// 		{
	// 			dfrelayComponent2.SetDockIndex(k);
	// 			int[] array = __instance.idleRelayIds;
	// 			int oldIdleRelayCount = __instance.idleRelayCount;
	// 			__instance.idleRelayCount = oldIdleRelayCount + 1;
	// 			array[oldIdleRelayCount] = dfRelayId2;
	// 		}
	// 	}
 //
	// 	return false;
	// }
        
   //      [HarmonyPrefix, HarmonyPatch(typeof(KillStatistics), "RegisterFactoryKillStat")]
   //      public static bool  RegisterFactoryKillStat(ref KillStatistics __instance, int factoryIndex, int modelIndex)
   //      {
			// if (factoryIndex < __instance.factoryKillStatPool.Length - 1) return false;
   //          ref AstroKillStat ptr = ref __instance.factoryKillStatPool[factoryIndex];
   //          if (ptr == null)
   //          {
   //              ptr = new AstroKillStat();
   //              ptr.Init();
   //          }
   //          ptr.killRegister[modelIndex]++;
   //          return false;
   //      }
        
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


        

        //
        // [HarmonyPrefix, HarmonyPatch(typeof(DysonSphere), "Init")]
        // public static bool Init(DysonSphere __instance, GameData _gameData, StarData _starData)
        // {
        //     __instance.gameData = _gameData;
        //     __instance.starData = _starData;
        //     ProductionStatistics production = __instance.gameData.statistics.production;
        //     int[] firstCreateIds = production.firstCreateIds;
        //     int num = firstCreateIds.Length;
        //     int num2 = 0;
        //     for (int i = 0; i < num; i++)
        //     {
        //         int num3 = firstCreateIds[i] / 100;
        //         if (__instance.starData.id == num3)
        //         {
        //             num2 = __instance.gameData.galaxy.PlanetById(firstCreateIds[i]).factoryIndex;
        //             break;
        //         }
        //     }
        //
        //     FactoryProductionStat factoryProductionStat = production.factoryStatPool[num2];
        //     __instance.productRegister = factoryProductionStat.productRegister;
        //     __instance.consumeRegister = factoryProductionStat.consumeRegister;
        //     __instance.sunColor = Color.white;
        //     __instance.energyGenPerSail = Configs.freeMode.solarSailEnergyPerTick;
        //     __instance.energyGenPerNode = Configs.freeMode.dysonNodeEnergyPerTick;
        //     __instance.energyGenPerFrame = Configs.freeMode.dysonFrameEnergyPerTick;
        //     __instance.energyGenPerShell = Configs.freeMode.dysonShellEnergyPerTick;
        //     if (__instance.starData != null)
        //     {
        //         float num4 = 4f;
        //         __instance.gravity = (float)(86646732.73933044 * (double)__instance.starData.mass) * num4;
        //         double num5 = (double)__instance.starData.dysonLumino;
        //         __instance.energyGenPerSail = (long)((double)__instance.energyGenPerSail * num5);
        //         __instance.energyGenPerNode = (long)((double)__instance.energyGenPerNode * num5);
        //         __instance.energyGenPerFrame = (long)((double)__instance.energyGenPerFrame * num5);
        //         __instance.energyGenPerShell = (long)((double)__instance.energyGenPerShell * num5);
        //         __instance.sunColor = Configs.builtin.dysonSphereSunColors.Evaluate(__instance.starData.color);
        //         __instance.emissionColor =
        //             Configs.builtin.dysonSphereEmissionColors.Evaluate(__instance.starData.color);
        //         if (__instance.starData.type == EStarType.NeutronStar)
        //         {
        //             __instance.sunColor = Configs.builtin.dysonSphereNeutronSunColor;
        //             __instance.emissionColor = Configs.builtin.dysonSphereNeutronEmissionColor;
        //         }
        //
        //         __instance.defOrbitRadius = (float)((double)__instance.starData.dysonRadius * 40000.0);
        //         __instance.minOrbitRadius = __instance.starData.physicsRadius * 1.5f;
        //         if (__instance.minOrbitRadius < 4000f)
        //         {
        //             __instance.minOrbitRadius = 4000f;
        //         }
        //
        //         __instance.maxOrbitRadius = __instance.defOrbitRadius * 2f;
        //         if (__instance.starData.planets.Length != 0)
        //             __instance.avoidOrbitRadius = (float)((double)__instance.starData.planets[0].orbitRadius * 40000.0);
        //         else __instance.avoidOrbitRadius = 4000000;
        //         if (__instance.starData.type == EStarType.GiantStar)
        //         {
        //             __instance.minOrbitRadius *= 0.6f;
        //         }
        //
        //         __instance.defOrbitRadius = Mathf.Round(__instance.defOrbitRadius / 100f) * 100f;
        //         __instance.minOrbitRadius = Mathf.Ceil(__instance.minOrbitRadius / 100f) * 100f;
        //         __instance.maxOrbitRadius = Mathf.Round(__instance.maxOrbitRadius / 100f) * 100f;
        //         __instance.randSeed = __instance.starData.seed;
        //     }
        //
        //     __instance.swarm = new DysonSwarm(__instance);
        //     __instance.swarm.Init();
        //     __instance.layerCount = 0;
        //     __instance.layersSorted = new DysonSphereLayer[10];
        //     __instance.layersIdBased = new DysonSphereLayer[11];
        //     __instance.rocketCapacity = 0;
        //     __instance.rocketCursor = 1;
        //     __instance.rocketRecycleCursor = 0;
        //     __instance.autoNodes = new DysonNode[8];
        //     __instance.autoNodeCount = 0;
        //     __instance.nrdCapacity = 0;
        //     __instance.nrdCursor = 1;
        //     __instance.nrdRecycleCursor = 0;
        //     __instance.modelRenderer = new DysonSphereSegmentRenderer(__instance);
        //     __instance.modelRenderer.Init();
        //     __instance.rocketRenderer = new DysonRocketRenderer(__instance);
        //     __instance.inEditorRenderMaskL = -1;
        //     __instance.inEditorRenderMaskS = -1;
        //     __instance.inGameRenderMaskL = -1;
        //     __instance.inGameRenderMaskS = -1;
        //     return false;
        // }











    }
}