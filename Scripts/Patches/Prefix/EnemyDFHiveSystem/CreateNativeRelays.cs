using System;
using HarmonyLib;
using UnityEngine;


namespace GalacticScale.Patches
{
    public class PatchOnEnemyDFHiveSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.CreateNativeRelays))]
        public static bool CreateNativeRelays(ref EnemyDFHiveSystem __instance)
        {
            {
                int initialHiveCountFactor = 2 - __instance.starData.initialHiveCount;
                if (initialHiveCountFactor < 0)
                {
                    initialHiveCountFactor = (initialHiveCountFactor - 2) / 2;
                }
                else
                {
                    initialHiveCountFactor *= 2;
                }

                if (initialHiveCountFactor < -3)
                {
                    initialHiveCountFactor = -3;
                }

                float virtualHiveMatterDamandByTicks =
                    EnemyDFConfigs.VirtualHiveMatterDamandByTicks(__instance.TicksToBuildChances(__instance.ticks));
                float sixtyF = 60f;
                float dangerFactor = ((1f - __instance.starData.safetyFactor) * 0.6f + 0.8f) *
                                     (1f + 0.125f * (__instance.history.combatSettings.maxDensity - 1f));
                int desperationFactor =
                    Mathf.CeilToInt((virtualHiveMatterDamandByTicks + 20f) / sixtyF * dangerFactor) +
                    __instance.ticks / 54000 + initialHiveCountFactor;
                int maxHiveCount = __instance.starData.maxHiveCount;
                int hiveCountFactor = ((maxHiveCount > 5) ? (12 - maxHiveCount) : 7);
                if (hiveCountFactor < 1)
                {
                    hiveCountFactor = 1;
                }

                int timeFactor = (__instance.ticks - 300) / 600;
                int possibleRelays = timeFactor - desperationFactor;
                if (possibleRelays < 0)
                {
                    possibleRelays = 0;
                    desperationFactor = timeFactor;
                }
                else if (possibleRelays > __instance.relayDocks.Length)
                {
                    possibleRelays = __instance.relayDocks.Length;
                }

                GS3.Random random = new GS3.Random(__instance.seed);
                for (int i = 0; i < __instance.starData.planetCount; i++)
                {
                    PlanetData planetData = __instance.starData.planets[i];
                    if (planetData.type != EPlanetType.Gas)
                    {
                        bool lastPlanet = i == __instance.starData.planetCount - 1;
                        bool birthStar = __instance.galaxy.birthStarId == __instance.starData.id;
                        bool birthPlanet = __instance.galaxy.birthPlanetId == planetData.id;
                        int relayCount = (birthPlanet
                            ? 1
                            : (lastPlanet
                                ? desperationFactor
                                : ((desperationFactor + 1) /
                                   2))); //If Birthplanet 1. if last planet, someOtherFactor, else someOtherFactor+1/2
                        if (relayCount > hiveCountFactor)
                        {
                            relayCount = hiveCountFactor;
                        }

                        if (relayCount > timeFactor)
                        {
                            relayCount = timeFactor;
                        }

                        // desperationFactor -= relayCount;
                        relayCount += desperationFactor;
                        // relayCount = 25;
                        GS3.LogMagenta(
                            $"Creating Native Relays for Planet {planetData.name} with {relayCount} relays. Docks:{__instance.relayDocks} Possible Relays: {possibleRelays} birthPlanet:{birthPlanet} birthStar:{birthStar} hiveCountFactor:{hiveCountFactor} desperationFactor:{desperationFactor} timeFactor:{timeFactor} dangerFactor:{dangerFactor} virtualHiveMatterDamandByTicks:{virtualHiveMatterDamandByTicks} initialHiveCountFactor:{initialHiveCountFactor}");
                        int num10 = 0;
                        VectorLF3 vectorLF = VectorLF3.zero;
                        
                        for (int j = 0; j < relayCount; j++)
                        {
                            double num11 = random.NextDouble();
                            double num12 = random.NextDouble();
                            int iterations = 0;
                            double r1 = 0.0;
                            double r2 = 0.0;
                            double r3 = 0.0;
                            bool okToCreate = false;
                            do
                            {
                                iterations++;
                                double magnitude = 0.0;
                                while (magnitude == 0.0 || magnitude > 1.0)
                                {
                                    r1 = random.NextDouble() * 2.0 - 1.0;
                                    r2 = random.NextDouble() * 2.0 - 1.0;
                                    r3 = random.NextDouble() * 2.0 - 1.0;
                                    magnitude = r1 * r1 + r2 * r2 + r3 * r3;
                                }

                                if ((double)planetData.veinBiasVector.sqrMagnitude < 0.1)
                                {
                                    planetData.GenVeinBiasVector();
                                }

                                VectorLF3 veinBiasVector = (birthPlanet
                                    ? (planetData.veinBiasVector * 2f)
                                    : (planetData.veinBiasVector * 0.97f));
                                r1 -= veinBiasVector.x;
                                r2 -= veinBiasVector.y;
                                r3 -= veinBiasVector.z;
                                magnitude = r1 * r1 + r2 * r2 + r3 * r3;
                                magnitude = Math.Sqrt(magnitude);
                                vectorLF = -veinBiasVector.normalized;
                                double num18 = (double)(planetData.realRadius + 70f) / magnitude;
                                r1 *= num18;
                                r2 *= num18;
                                r3 *= num18;
                                GS3.Log($"{r1} {r2} {r3} {Utils.RandomDirection(random)}");
                                if (!__instance.CheckPositionCollideRelay(planetData.astroId, r1, r2, r3))
                                {
                                    okToCreate = true;
                                }

                                if (iterations >= 80)
                                {
                                    GS3.LogGreen("Couldn't find anywhere to put relay");
                                }
                            } while (!okToCreate && iterations < 80);

                            if (okToCreate)
                            {
                                VectorLF3 vectorLF3 = new VectorLF3(r1, r2, r3);
                                int num19 = __instance.sector.CreateEnemyFinal(__instance, 8116, planetData.astroId,
                                    vectorLF3, Maths.SphericalRotation(vectorLF3, (float)num12 * 360f));
                                int dfRelayId = __instance.sector.enemyPool[num19].dfRelayId;
                                DFRelayComponent dfrelayComponent = __instance.relays.buffer[dfRelayId];
                                Assert.True(dfrelayComponent != null && dfRelayId > 0 &&
                                            dfRelayId == dfrelayComponent.id);
                                if (dfrelayComponent != null)
                                {
                                    dfrelayComponent.SetDockIndex(num10++);
                                    dfrelayComponent.hiveAstroId = __instance.hiveAstroId;
                                    dfrelayComponent.targetAstroId = planetData.astroId;
                                    dfrelayComponent.targetLPos = vectorLF3;
                                    dfrelayComponent.targetYaw = (float)num12 * 360f;
                                    dfrelayComponent.baseState = 1;
                                    dfrelayComponent.baseId = 0;
                                    double num20 = VectorLF3.Dot(vectorLF3.normalized, vectorLF);
                                    num20 = Maths.Clamp01((num20 + 1.0) * 0.5);
                                    num20 = Math.Pow(num20, 0.5);
                                    if (birthPlanet)
                                    {
                                        dfrelayComponent.baseTicks =
                                            (int)(3000f * (float)(num11 * 0.05 + 0.12) + 120.5f);
                                    }
                                    else if (birthStar)
                                    {
                                        dfrelayComponent.baseTicks =
                                            (int)(3000f * (float)(Math.Pow(num11 * 0.5 + 0.5, 1.5) * num20) + 150.5f);
                                    }
                                    else
                                    {
                                        dfrelayComponent.baseTicks =
                                            (int)(6400f * (float)(Math.Pow(num11, 2.0) * num20) + 200.5f);
                                    }

                                    dfrelayComponent.baseEvolve = __instance.evolve;
                                    dfrelayComponent.baseEvolve.threat = 0;
                                    dfrelayComponent.baseEvolve.waves = (birthPlanet ? 0 : 1);
                                    dfrelayComponent.direction = 0;
                                    dfrelayComponent.stage = 2;
                                    int rHuge = random.Next(180001) * 100;
                                    int builderId = dfrelayComponent.builderId;
                                    __instance.builders.buffer[builderId].energy =
                                        __instance.builders.buffer[builderId].maxEnergy + rHuge;
                                    __instance.sector.enemyAnimPool[num19].time = 1f;
                                    __instance.sector.enemyAnimPool[num19].state = 1U;
                                    __instance.sector.enemyAnimPool[num19].power = -1f;
                                }
                            }
                        }
                    }
                }

                __instance.idleRelayCount = 0;
                for (int k = 0; k < possibleRelays; k++)
                {
                    ref DFDock ptr = ref __instance.relayDocks[k % __instance.relayDocks.Length];
                    int relayId =
                        __instance.sector.CreateEnemyFinal(__instance, 8116, __instance.hiveAstroId, ptr.pos, ptr.rot);
                    int dfRelayId2 = __instance.sector.enemyPool[relayId].dfRelayId;
                    DFRelayComponent dfrelayComponent2 = __instance.relays.buffer[dfRelayId2];
                    Assert.True(dfrelayComponent2 != null && dfRelayId2 > 0 && dfRelayId2 == dfrelayComponent2.id);
                    if (dfrelayComponent2 != null)
                    {
                        dfrelayComponent2.SetDockIndex(k);
                        int[] array = __instance.idleRelayIds;
                        __instance.idleRelayCount += 1;
                        array[__instance.idleRelayCount] = dfRelayId2;
                    }
                }
            }
            return false;
        }
    }
}