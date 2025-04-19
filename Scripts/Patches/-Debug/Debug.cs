using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using static GalacticScale.GS2;
using UnityEngine;
using UnityEngine.UI;
using static System.Reflection.Emit.OpCodes;
using Logger = BepInEx.Logging.Logger;

// using BCE;
namespace GalacticScale
{
    public static class PatchOnUnspecified_Debug
    {
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "ExportRuntime")]
        public static void ExportRuntime_Prefix(PlanetData __instance)
        {
            try
            {
                if (__instance == null) return;
                
                // Get the type of the instance
                var type = __instance.GetType();

                // Get all public instance properties
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                GS2.Log($"--- Exporting PlanetData: {__instance.displayName} ({__instance.id}) ---");

                // Iterate through properties and log their status
                foreach (var property in properties)
                {
                    try
                    {
                        // Skip indexers to avoid exceptions
                        if (property.GetIndexParameters().Length > 0) continue; 
                        
                        var value = property.GetValue(__instance, null);
                        string status;
                        if (value == null)
                        {
                            status = "null";
                        }
                        else
                        {
                            var propertyType = property.PropertyType;
                            // Check for primitive types, string, decimal, enums, and Vector types
                            if (propertyType.IsPrimitive || 
                                propertyType == typeof(string) || 
                                propertyType.IsEnum || 
                                propertyType == typeof(decimal) ||
                                propertyType == typeof(Vector2) ||
                                propertyType == typeof(Vector3) ||
                                propertyType == typeof(Vector4) ||
                                propertyType == typeof(VectorLF3) ||
                                propertyType == typeof(Quaternion)) 
                            {
                                status = value.ToString();
                            }
                            // Check for arrays or lists explicitly
                            else if (propertyType.IsArray || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                            {
                                System.Collections.IEnumerable collection = value as System.Collections.IEnumerable;
                                int count = 0;
                                if (collection != null)
                                {
                                     if (propertyType.IsArray) count = ((Array)collection).Length;
                                     else count = (int)propertyType.GetProperty("Count").GetValue(collection, null);
                                }
                                status = $"{propertyType.Name}[{count}]";
                            }
                            else
                            {
                                status = "Object"; // For other complex objects
                            }
                        }
                        GS2.Log($"Export:{property.Name}: {status}");
                    }
                    catch (Exception e)
                    {
                        // Handle potential exceptions when accessing properties 
                        GS2.Log($"Export:{property.Name}: Error accessing property - {e.GetType().Name}: {e.Message}");
                    }
                }
                GS2.Log($"--- Finished Exporting PlanetData: {__instance.displayName} ({__instance.id}) ---");

                if (__instance.modData != null) return;
                lock (PlanetModelingManager.planetProcessingLock)
                {
                    if (__instance.data == null) __instance.data = new PlanetRawData(__instance.precision);
                    __instance.data.InitModData(__instance.modData);
                }
            }
            catch
            {
                int precision = __instance.precision;
                int dataLength = (precision + 1) * (precision + 1) * 4;
                lock (PlanetModelingManager.planetProcessingLock)
                {
                    __instance.modData = new byte[dataLength / 2];
                }
            }
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.RequestScanPlanet))]
        public static void RequestScanPlanet(PlanetData planet)
        {
            DevLog($"{planet.displayName} scanning:{planet.scanning} scanned:{planet.scanned} queue:{PlanetModelingManager.scnPlanetReqList.Count}");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GalaxyData), nameof(GalaxyData.UpdateScanningProcedure))]
        public static bool UpdateScanningProcedure_Prefix(GalaxyData __instance, long gameTick)
        {
            return false;
        }



        [HarmonyPrefix, HarmonyPatch(typeof(DysonSphere), nameof(DysonSphere.RocketGameTick),new []{
            typeof(int), typeof(int), typeof(int)
        })]
        public static bool RocketGameTick(ref DysonSphere __instance, int _usedThreadCnt, int _curThreadIdx, int _minimumMissionCnt)
	{
		AstroData[] astrosData = __instance.starData.galaxy.astrosData;
		double timefactor = 0.016666666666666666;
		float orbitRadiusFactor = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
		float constantValue = 7.5f;
		float scaleOrbitRadius1 = 18f * orbitRadiusFactor;
		float scaleOrbitRadius2 = 280f * orbitRadiusFactor;
		long gameTick = GameMain.gameTick;
		int startindex;
		int endIndex;
		if (!WorkerThreadExecutor.CalculateMissionIndex(1, __instance.rocketCursor - 1, _usedThreadCnt, _curThreadIdx, _minimumMissionCnt, out startindex, out endIndex))
		{
			return false;
		}
		VectorLF3 starUPosition = __instance.starData.uPosition;
		for (int i = startindex; i < endIndex; i++)
		{
			if (__instance.rocketPool[i].id == i)
			{
				ref DysonRocket ptr = ref __instance.rocketPool[i];
				if (ptr.node == null)
				{
					__instance.RemoveDysonRocket(i);
				}
				else
				{
					DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[ptr.node.layerId];
					ref AstroData ptr2 = ref astrosData[ptr.planetId];
					VectorLF3 vectorLF;
					var radius = ptr2.uRadius;
					vectorLF.x = ptr.uPos.x - ptr2.uPos.x;
					vectorLF.y = ptr.uPos.y - ptr2.uPos.y;
					vectorLF.z = ptr.uPos.z - ptr2.uPos.z;
					double distanceFromPlanetSurface = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)ptr2.uRadius;
					if (ptr.t <= 0f)
					{
						if (distanceFromPlanetSurface < radius)
						{
							float num9 = (float)distanceFromPlanetSurface / radius;
							if (num9 < 0f)
							{
								num9 = 0f;
							}
							float num10 = num9 * num9 * 600f + 15f;
							ptr.uSpeed = ptr.uSpeed * 0.9f + num10 * 0.1f;
							ptr.t = (num9 - 1f) * 1.2f;
							if (ptr.t < -1f)
							{
								ptr.t = -1f;
							}
						}
						else
						{
							VectorLF3 vectorLF2;
							dysonSphereLayer.NodeEnterUPos(ptr.node, out vectorLF2);
							VectorLF3 vectorLF3;
							vectorLF3.x = vectorLF2.x - ptr.uPos.x;
							vectorLF3.y = vectorLF2.y - ptr.uPos.y;
							vectorLF3.z = vectorLF2.z - ptr.uPos.z;
							double num11 = Math.Sqrt(vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z);
							if (num11 < 50.0)
							{
								ptr.t = 0.0001f;
							}
							else
							{
								ptr.t = 0f;
							}
							double num12 = num11 / ((double)ptr.uSpeed + 0.1) * 0.382;
							double num13 = num11 / (double)scaleOrbitRadius2;
							float num14 = (float)((double)ptr.uSpeed * num12) + 150f;
							if (num14 > scaleOrbitRadius2)
							{
								num14 = scaleOrbitRadius2;
							}
							if (ptr.uSpeed < num14 - constantValue)
							{
								ptr.uSpeed += constantValue;
							}
							else if (ptr.uSpeed > num14 + scaleOrbitRadius1)
							{
								ptr.uSpeed -= scaleOrbitRadius1;
							}
							else
							{
								ptr.uSpeed = num14;
							}
							int num15 = -1;
							double rhs = 0.0;
							double num16 = 1E+40;
							int starID = ptr.planetId / 100 * 100;
							for (int j = starID; j < starID + 99; j++) // Added a 99 here
							{
								float uRadius = astrosData[j].uRadius;
								if (uRadius >= 1f && j != ptr.planetId)
								{
									float num18 = (j == starID) ? (dysonSphereLayer.orbitRadius + 8000f) : (uRadius + 6500f);
									VectorLF3 vectorLF4;
									vectorLF4.x = ptr.uPos.x - astrosData[j].uPos.x;
									vectorLF4.y = ptr.uPos.y - astrosData[j].uPos.y;
									vectorLF4.z = ptr.uPos.z - astrosData[j].uPos.z;
									double num19 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
									double num20 = -((double)ptr.uVel.x * vectorLF4.x + (double)ptr.uVel.y * vectorLF4.y + (double)ptr.uVel.z * vectorLF4.z);
									if ((num20 > 0.0 || num19 < (double)(uRadius * uRadius * 7f)) && num19 < num16 && num19 < (double)(num18 * num18))
									{
										rhs = ((num20 < 0.0) ? 0.0 : num20);
										num15 = j;
										num16 = num19;
									}
								}
							}
							VectorLF3 rhs2 = VectorLF3.zero;
							float num21 = 0f;
							if (num15 > 0)
							{
								float num22 = astrosData[num15].uRadius;
								bool flag = num15 % 100 == 0;
								if (flag)
								{
									num22 = dysonSphereLayer.orbitRadius - 400f;
								}
								double num23 = 1.25;
								VectorLF3 vectorLF5 = (VectorLF3)ptr.uPos + (VectorLF3)ptr.uVel * rhs - astrosData[num15].uPos;
								double num24 = vectorLF5.magnitude / (double)num22;
								if (num24 < num23)
								{
									double num25 = Math.Sqrt(num16) - (double)num22 * 0.82;
									if (num25 < 1.0)
									{
										num25 = 1.0;
									}
									double num26 = (num24 - 1.0) / (num23 - 1.0);
									if (num26 < 0.0)
									{
										num26 = 0.0;
									}
									num26 = 1.0 - num26 * num26;
									double num27 = (double)(ptr.uSpeed - 6f) / num25 * 2.5 - 0.01;
									if (num27 > 1.5)
									{
										num27 = 1.5;
									}
									else if (num27 < 0.0)
									{
										num27 = 0.0;
									}
									num27 = num27 * num27 * num26;
									num21 = (float)(flag ? 0.0 : (num27 * 0.5));
									rhs2 = vectorLF5.normalized * num27 * 2.0;
								}
							}
							int num28 = (num15 > 0 || distanceFromPlanetSurface < 2000.0 || num11 < 2000.0) ? 1 : 6;
							if (num28 == 1 || (gameTick + (long)i) % (long)num28 == 0L)
							{
								float num29 = 1f / (float)num13 - 0.05f;
								num29 += num21;
								float t = Mathf.Lerp(0.005f, 0.08f, num29) * (float)num28;
								ptr.uVel = Vector3.Slerp(ptr.uVel, vectorLF3.normalized + rhs2, t).normalized;
								Quaternion b;
								if (num11 < 350.0)
								{
									float t2 = ((float)num11 - 50f) / 300f;
									b = Quaternion.Slerp(dysonSphereLayer.NodeURot(ptr.node), Quaternion.LookRotation(ptr.uVel), t2);
								}
								else
								{
									b = Quaternion.LookRotation(ptr.uVel);
								}
								ptr.uRot = Quaternion.Slerp(ptr.uRot, b, 0.2f);
							}
						}
					}
					else
					{
						VectorLF3 vectorLF6;
						dysonSphereLayer.NodeSlotUPos(ptr.node, out vectorLF6);
						VectorLF3 vectorLF7;
						vectorLF7.x = vectorLF6.x - ptr.uPos.x;
						vectorLF7.y = vectorLF6.y - ptr.uPos.y;
						vectorLF7.z = vectorLF6.z - ptr.uPos.z;
						double num30 = Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z);
						if (num30 < 2.0)
						{
							__instance.ConstructSp(ptr.node);
							__instance.RemoveDysonRocket(i);
							goto IL_E26;
						}
						float num31 = (float)(num30 * 0.75 + 15.0);
						if (num31 > scaleOrbitRadius2)
						{
							num31 = scaleOrbitRadius2;
						}
						if (ptr.uSpeed < num31 - constantValue)
						{
							ptr.uSpeed += constantValue;
						}
						else if (ptr.uSpeed > num31 + scaleOrbitRadius1)
						{
							ptr.uSpeed -= scaleOrbitRadius1;
						}
						else
						{
							ptr.uSpeed = num31;
						}
						if ((gameTick + (long)i) % 2L == 0L)
						{
							ptr.uVel = Vector3.Slerp(ptr.uVel, vectorLF7.normalized, 0.15f);
							ptr.uRot = Quaternion.Slerp(ptr.uRot, dysonSphereLayer.NodeURot(ptr.node), 0.2f);
						}
						ptr.t = (350f - (float)num30) / 330f;
						if (ptr.t > 1f)
						{
							ptr.t = 1f;
						}
						else if (ptr.t < 0.0001f)
						{
							ptr.t = 0.0001f;
						}
					}
					VectorLF3 vectorLF8 = new VectorLF3(0f, 0f, 0f);
					bool flag2 = false;
					double num32 = (double)(2f - (float)distanceFromPlanetSurface / radius);
					if (num32 > 1.0)
					{
						num32 = 1.0;
					}
					else if (num32 < 0.0)
					{
						num32 = 0.0;
					}
					if (num32 > 0.0)
					{
						VectorLF3 v;
						Maths.QInvRotateLF_refout(ref ptr2.uRot, ref vectorLF, out v);
						VectorLF3 lhs = Maths.QRotateLF(ptr2.uRotNext, v) + ptr2.uPosNext;
						Quaternion quaternion;
						Maths.QInvMultiply_ref(ref ptr2.uRot, ref ptr.uRot, out quaternion);
						Quaternion quaternion2;
						Maths.QMultiply_ref(ref ptr2.uRotNext, ref quaternion, out quaternion2);
						num32 = (3.0 - num32 - num32) * num32 * num32;
						vectorLF8 = (lhs - ptr.uPos) * num32;
						ptr.uRot = ((num32 == 1.0) ? quaternion2 : Quaternion.Slerp(ptr.uRot, quaternion2, (float)num32));
						flag2 = true;
					}
					if (!flag2)
					{
						VectorLF3 vectorLF9;
						vectorLF9.x = ptr.uPos.x - starUPosition.x;
						vectorLF9.y = ptr.uPos.y - starUPosition.y;
						vectorLF9.z = ptr.uPos.z - starUPosition.z;
						double num33 = Math.Abs(Math.Sqrt(vectorLF9.x * vectorLF9.x + vectorLF9.y * vectorLF9.y + vectorLF9.z * vectorLF9.z) - (double)dysonSphereLayer.orbitRadius);
						double num34 = 1.5 - (double)((float)num33 / 1800f);
						if (num34 > 1.0)
						{
							num34 = 1.0;
						}
						else if (num34 < 0.0)
						{
							num34 = 0.0;
						}
						if (num34 > 0.0)
						{
							VectorLF3 v2;
							Maths.QInvRotateLF_refout(ref dysonSphereLayer.currentRotation, ref vectorLF9, out v2);
							VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v2) + starUPosition;
							Quaternion quaternion3;
							Maths.QInvMultiply_ref(ref dysonSphereLayer.currentRotation, ref ptr.uRot, out quaternion3);
							Quaternion quaternion4;
							Maths.QMultiply_ref(ref dysonSphereLayer.nextRotation, ref quaternion3, out quaternion4);
							num34 = (3.0 - num34 - num34) * num34 * num34;
							vectorLF8 = (lhs2 - ptr.uPos) * num34;
							ptr.uRot = ((num34 == 1.0) ? quaternion4 : Quaternion.Slerp(ptr.uRot, quaternion4, (float)num34));
						}
					}
					double num35 = (double)ptr.uSpeed * timefactor;
					ptr.uPos.x = ptr.uPos.x + (double)ptr.uVel.x * num35 + vectorLF8.x;
					ptr.uPos.y = ptr.uPos.y + (double)ptr.uVel.y * num35 + vectorLF8.y;
					ptr.uPos.z = ptr.uPos.z + (double)ptr.uVel.z * num35 + vectorLF8.z;
					if (distanceFromPlanetSurface < radius+50f)
					{
						vectorLF.x = ptr2.uPos.x - ptr.uPos.x;
						vectorLF.y = ptr2.uPos.y - ptr.uPos.y;
						vectorLF.z = ptr2.uPos.z - ptr.uPos.z;
						distanceFromPlanetSurface = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)ptr2.uRadius;
						if (distanceFromPlanetSurface < radius-20f)
						{
							ptr.uPos = ptr2.uPos + Maths.QRotateLF(ptr2.uRot, (VectorLF3)ptr.launch * ((double)ptr2.uRadius + distanceFromPlanetSurface));
							ptr.uRot = ptr2.uRot * Quaternion.LookRotation(ptr.launch);
						}
					}
				}
			}
			IL_E26:;
		}

		return false;
	}
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceSector), nameof(SpaceSector.Import))]
        public static bool Import(ref SpaceSector __instance, BinaryReader r)
        {
            if (GS2.IsMenuDemo) return true;
            int num = r.ReadInt32();
            int astroCapacity = r.ReadInt32();
            __instance.SetAstroCapacity(astroCapacity);
            __instance.astroCursor = r.ReadInt32();
            for (int i = 1; i < __instance.astroCursor; i++)
            {
                __instance.astros[i].id = r.ReadInt32();
                if (__instance.astros[i].id > 0)
                {
                    __instance.astros[i].type = (EAstroType)r.ReadInt32();
                    __instance.astros[i].parentId = r.ReadInt32();
                    __instance.astros[i].uPos.x = r.ReadDouble();
                    __instance.astros[i].uPos.y = r.ReadDouble();
                    __instance.astros[i].uPos.z = r.ReadDouble();
                    __instance.astros[i].uPosNext.x = r.ReadDouble();
                    __instance.astros[i].uPosNext.y = r.ReadDouble();
                    __instance.astros[i].uPosNext.z = r.ReadDouble();
                    __instance.astros[i].uRot.x = r.ReadSingle();
                    __instance.astros[i].uRot.y = r.ReadSingle();
                    __instance.astros[i].uRot.z = r.ReadSingle();
                    __instance.astros[i].uRot.w = r.ReadSingle();
                    __instance.astros[i].uRotNext.x = r.ReadSingle();
                    __instance.astros[i].uRotNext.y = r.ReadSingle();
                    __instance.astros[i].uRotNext.z = r.ReadSingle();
                    __instance.astros[i].uRotNext.w = r.ReadSingle();
                    __instance.astros[i].uRadius = r.ReadSingle();
                }
                else
                {
                    __instance.astros[i].SetEmpty();
                }
            }

            astroCapacity = r.ReadInt32();
            __instance.SetEnemyCapacity(astroCapacity);
            __instance.enemyCursor = r.ReadInt32();
            __instance.enemyRecycleCursor = r.ReadInt32();
            for (int j = 1; j < __instance.enemyCursor; j++)
            {
                __instance.enemyPool[j].Import(r);
            }

            for (int k = 0; k < __instance.enemyRecycleCursor; k++)
            {
                __instance.enemyRecycle[k] = r.ReadInt32();
            }

            for (int l = 1; l < __instance.enemyCursor; l++)
            {
                __instance.enemyAnimPool[l].time = r.ReadSingle();
                __instance.enemyAnimPool[l].prepare_length = r.ReadSingle();
                __instance.enemyAnimPool[l].working_length = r.ReadSingle();
                __instance.enemyAnimPool[l].state = r.ReadUInt32();
                __instance.enemyAnimPool[l].power = r.ReadSingle();
            }

            astroCapacity = r.ReadInt32();
            __instance.SetCraftCapacity(astroCapacity);
            __instance.craftCursor = r.ReadInt32();
            __instance.craftRecycleCursor = r.ReadInt32();
            for (int m = 1; m < __instance.craftCursor; m++)
            {
                __instance.craftPool[m].Import(r);
            }

            for (int n = 0; n < __instance.craftRecycleCursor; n++)
            {
                __instance.craftRecycle[n] = r.ReadInt32();
            }

            for (int num2 = 1; num2 < __instance.craftCursor; num2++)
            {
                __instance.craftAnimPool[num2].time = r.ReadSingle();
                __instance.craftAnimPool[num2].prepare_length = r.ReadSingle();
                __instance.craftAnimPool[num2].working_length = r.ReadSingle();
                __instance.craftAnimPool[num2].state = r.ReadUInt32();
                __instance.craftAnimPool[num2].power = r.ReadSingle();
            }

            if (num >= 1)
            {
                __instance.spaceRuins.Import(r);
            }

            __instance.skillSystem.Import(r);
            int num3 = r.ReadInt32();
            if (num3 > 0)
            {
                if (num3 > 65535)
                {
                    throw new Exception("invalid dfcnt!");
                }
                GS2.Random ra = new GS2.Random(num3);
                __instance.dfHives = new EnemyDFHiveSystem[num3];
                for (int num4 = 0; num4 < num3; num4++)
                {
                    __instance.dfHives[num4] = null;
                    var possibleOrbits = GS2.GeneratePossibleHiveOrbits(GetGSStar(__instance.galaxy.stars[num4]));
                    EnemyDFHiveSystem enemyDFHiveSystem = null;
                    while (r.ReadInt32() == 19884)
                    {
                        EnemyDFHiveSystem enemyDFHiveSystem2 = new EnemyDFHiveSystem();
                        enemyDFHiveSystem2.Init(__instance.gameData, __instance.galaxy.stars[num4].id, 0);
                        enemyDFHiveSystem2.Import(r);
                        if (enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius > __instance.galaxy.stars[num4].systemRadius)
                        {
                            Warn($"DFHive orbit radius is larger than {__instance.galaxy.stars[num4].name} system radius {enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius} > {__instance.galaxy.stars[num4].systemRadius}");
                            var orbit = ra.ItemAndRemove(possibleOrbits);
                            Warn($"New Orbit Radius = {orbit}");
                            enemyDFHiveSystem2.hiveAstroOrbit.orbitRadius = orbit;
                        }
                        

                            if (enemyDFHiveSystem == null)
                            {
                                __instance.dfHives[num4] = enemyDFHiveSystem2;
                            }
                            else
                            {
                                enemyDFHiveSystem.nextSibling = enemyDFHiveSystem2;
                            }

                            enemyDFHiveSystem2.prevSibling = enemyDFHiveSystem;
                            enemyDFHiveSystem = enemyDFHiveSystem2;
                        
                    }
                }
            }

            __instance.combatSpaceSystem = new CombatSpaceSystem(__instance);
            __instance.combatSpaceSystem.Import(r);
            return false;
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(SpaceSector), nameof(SpaceSector.GameTick))]
        // public static bool SSGameTick()
        // {
        // 	if (Config.DisableSpaceSector) return false;
        // 	return true;
        // }
        // static int u = 0;
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(CombatSpaceSystem), nameof(CombatSpaceSystem.GameTick))]
        // public static bool CSSGameTick(ref CombatSpaceSystem __instance,long tick)
        // {
        // 	int s = 0;
        // 	int t = 0;
        // 	int v = 0;
        // 	GameHistoryData history = __instance.gameData.history;
        // 	EnemyData[] enemyPool = __instance.spaceSector.enemyPool;
        // 	ref CombatSettings combatSettings = ref __instance.gameData.history.combatSettings;
        // 	bool flag = __instance.spaceSector.model == null || __instance.spaceSector.model.disableFleet;
        // 	SpaceObjectRenderer[] objectRenderers = __instance.spaceSector.model.gpuiManager.objectRenderers;
        // 	__instance.spaceSector.model.craftDirty = true;
        // 	int num = (int)(tick % 60L);
        // 	UnitComponent.gameTick = tick;
        // 	CombatUpgradeData combatUpgradeData = default(CombatUpgradeData);
        // 	history.GetCombatUpgradeData(ref combatUpgradeData);
        // 	ref VectorLF3 ptr = ref __instance.gameData.relativePos;
        // 	ref Quaternion ptr2 = ref __instance.gameData.relativeRot;
        // 	Vector3 vector = new Vector3(0f, 0f, 0f);
        // 	Quaternion identity = Quaternion.identity;
        // 	bool inStarmap = __instance.inStarmap;
        // 	FleetComponent[] buffer = __instance.fleets.buffer;
        // 	int cursor = __instance.fleets.cursor;
        // 	for (int i = 1; i < cursor; i++)
        // 	{
        // 		ref FleetComponent ptr3 = ref buffer[i];
        // 		if (ptr3.id == i)
        // 		{
        // 			v++;
        // 			ref CraftData ptr4 = ref __instance.spaceSector.craftPool[ptr3.craftId];
        // 			if ((GameMain.spaceSector.astros[ptr4.astroId].uPos - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
        // 			{
        // 				s++;
        // 				continue;
        // 			}
        // 			PrefabDesc pdesc = SpaceSector.PrefabDescByModelIndex[(int)ptr4.modelIndex];
        // 			if (i % 60 == num)
        // 			{
        // 				ptr3.SensorLogic_Space(ref ptr4, pdesc, __instance.spaceSector, tick);
        // 			}
        //
        // 			ptr3.InternalUpdate_Space(ref ptr4, ref __instance.spaceSector.craftAnimPool[ptr4.id], pdesc, __instance.spaceSector, __instance.mecha);
        // 			ptr3.AssembleUnits_Space(ref ptr4, ref combatUpgradeData, pdesc, __instance.mecha, __instance.spaceSector, tick);
        // 			ptr3.DetermineCraftAstroId(__instance.spaceSector, ref ptr4);
        // 			if (ptr3.DeterminActiveEnemyUnits(true, tick))
        // 			{
        // 				ptr3.ActiveEnemyUnits_Space(__instance.spaceSector, pdesc);
        // 			}
        //
        // 			if (flag)
        // 			{
        // 				__instance.spaceSector.craftAnimPool[ptr4.id].state = 0U;
        // 			}
        //
        // 			SpaceDynamicRenderer spaceDynamicRenderer = objectRenderers[(int)ptr4.modelIndex] as SpaceDynamicRenderer;
        // 			if (spaceDynamicRenderer != null)
        // 			{
        // 				ref SPACEOBJECT ptr5 = ref spaceDynamicRenderer.instPool[ptr4.modelId];
        // 				ptr5.astroId = (uint)ptr4.astroId;
        // 				if (ptr5.astroId == 0U)
        // 				{
        // 					if (inStarmap && UIGame.viewModeReady)
        // 					{
        // 						ref VectorLF3 ptr6 = ref __instance.gameData.starmapViewPos;
        // 						ptr5.posx = (float)((ptr4.pos.x - ptr6.x) * 0.00025);
        // 						ptr5.posy = (float)((ptr4.pos.y - ptr6.y) * 0.00025);
        // 						ptr5.posz = (float)((ptr4.pos.z - ptr6.z) * 0.00025);
        // 						ptr5.rotx = ptr4.rot.x;
        // 						ptr5.roty = ptr4.rot.y;
        // 						ptr5.rotz = ptr4.rot.z;
        // 						ptr5.rotw = ptr4.rot.w;
        // 					}
        // 					else
        // 					{
        // 						VectorLF3 vectorLF;
        // 						vectorLF.x = ptr4.pos.x - ptr.x;
        // 						vectorLF.y = ptr4.pos.y - ptr.y;
        // 						vectorLF.z = ptr4.pos.z - ptr.z;
        // 						Maths.QInvRotateLF_ref(ref ptr2, ref vectorLF, ref vector);
        // 						ptr5.posx = vector.x;
        // 						ptr5.posy = vector.y;
        // 						ptr5.posz = vector.z;
        // 						Maths.QInvMultiply_ref(ref ptr2, ref ptr4.rot, out identity);
        // 						ptr5.rotx = identity.x;
        // 						ptr5.roty = identity.y;
        // 						ptr5.rotz = identity.z;
        // 						ptr5.rotw = identity.w;
        // 					}
        // 				}
        // 				else
        // 				{
        // 					ptr5.posx = (float)ptr4.pos.x;
        // 					ptr5.posy = (float)ptr4.pos.y;
        // 					ptr5.posz = (float)ptr4.pos.z;
        // 					ptr5.rotx = ptr4.rot.x;
        // 					ptr5.roty = ptr4.rot.y;
        // 					ptr5.rotz = ptr4.rot.z;
        // 					ptr5.rotw = ptr4.rot.w;
        // 				}
        // 			}
        // 		}
        // 	}
        //
        // 	UnitComponent[] buffer2 = __instance.units.buffer;
        // 	int cursor2 = __instance.units.cursor;
        // 	for (int j = 1; j < cursor2; j++)
        // 	{
        // 		ref UnitComponent ptr7 = ref buffer2[j];
        //
        // 		if (ptr7.id == j)
        // 		{
        // 			v++;
        // 			ref CraftData ptr8 = ref __instance.spaceSector.craftPool[ptr7.craftId];
        // 			if ((GameMain.spaceSector.astros[ptr8.astroId].uPos - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
        // 			{
        // 				t++;
        // 				continue;
        // 			}
        // 			PrefabDesc pdesc2 = SpaceSector.PrefabDescByModelIndex[(int)ptr8.modelIndex];
        // 			if (j % 60 == num)
        // 			{
        // 				ptr7.hatred.Fade(0.75f, 5);
        // 				ptr7.SensorLogic_Space(ref ptr8, pdesc2, __instance.spaceSector);
        // 			}
        //
        // 			ptr7.AssistTeammates_Space(ref ptr8, __instance.spaceSector, __instance.mecha);
        // 			ptr7.UpdateFireCondition(ptr8.isSpace, pdesc2, ref combatUpgradeData);
        // 			int orbitAstroId = 0;
        // 			bool flag2 = false;
        // 			bool flag3 = false;
        // 			if (ptr8.owner > 0)
        // 			{
        // 				ref CraftData ptr9 = ref __instance.spaceSector.craftPool[(int)ptr8.owner];
        // 				if (ptr9.id != 0 && ptr9.fleetId > 0)
        // 				{
        // 					ref FleetComponent ptr10 = ref __instance.fleets.buffer[ptr9.fleetId];
        // 					orbitAstroId = FleetComponent.DetermineOrbitingAstro(__instance.spaceSector, ref ptr9);
        // 					ptr7.DetermineBehavior(ref ptr10.target, ref ptr10.targetPos, orbitAstroId, ptr10.dispatch);
        // 					flag2 = true;
        // 					if (ptr9.owner < 0 && __instance.mecha.player.isAlive)
        // 					{
        // 						flag3 = !ptr7.UpdateMechaEnergy(__instance.mecha, pdesc2, ptr8.isSpace);
        // 						if (flag3)
        // 						{
        // 							ptr7.behavior = EUnitBehavior.Recycled;
        // 						}
        // 					}
        // 				}
        // 			}
        //
        // 			if (flag2)
        // 			{
        // 				switch (ptr7.behavior)
        // 				{
        // 					case EUnitBehavior.None:
        // 						ptr7.RunBehavior_None();
        // 						break;
        // 					case EUnitBehavior.Initialize:
        // 						ptr7.RunBehavior_Initialize_Space(__instance.spaceSector, __instance.mecha, pdesc2, ref ptr8, ref combatUpgradeData, orbitAstroId, history.fighterInitializeSpeedScale);
        // 						break;
        // 					case EUnitBehavior.Recycled:
        // 						ptr7.RunBehavior_Recycled_Space(__instance.spaceSector, __instance.mecha, pdesc2, ref ptr8, ref combatSettings, ref combatUpgradeData, orbitAstroId, flag3);
        // 						break;
        // 					case EUnitBehavior.KeepForm:
        // 						ptr7.RunBehavior_KeepForm(ref ptr8);
        // 						break;
        // 					case EUnitBehavior.SeekForm:
        // 						ptr7.RunBehavior_SeekForm_Space(__instance.spaceSector, __instance.mecha, pdesc2, ref ptr8, ref combatUpgradeData);
        // 						break;
        // 					case EUnitBehavior.Engage:
        // 						ptr7.RunBehavior_Engage_Space(__instance.spaceSector, __instance.mecha, pdesc2, ref ptr8, ref combatSettings, ref combatUpgradeData);
        // 						break;
        // 					case EUnitBehavior.Orbiting:
        // 						ptr7.RunBehavior_Orbiting(__instance.spaceSector, __instance.mecha, pdesc2, ref ptr8, ref combatUpgradeData, orbitAstroId);
        // 						break;
        // 				}
        // 			}
        // 			else
        // 			{
        // 				__instance.spaceSector.RemoveCraftDeferred(ptr7.craftId);
        // 			}
        //
        // 			ptr7.DetermineCraftAstroId(__instance.spaceSector, ref ptr8);
        // 			SpaceDynamicRenderer spaceDynamicRenderer2 = objectRenderers[(int)ptr8.modelIndex] as SpaceDynamicRenderer;
        // 			if (spaceDynamicRenderer2 != null)
        // 			{
        // 				ref SPACEOBJECT ptr11 = ref spaceDynamicRenderer2.instPool[ptr8.modelId];
        // 				ptr11.astroId = (uint)ptr8.astroId;
        // 				if (ptr11.astroId == 0U)
        // 				{
        // 					if (inStarmap && UIGame.viewModeReady)
        // 					{
        // 						ref VectorLF3 ptr12 = ref __instance.gameData.starmapViewPos;
        // 						ptr11.posx = (float)((ptr8.pos.x - ptr12.x) * 0.00025);
        // 						ptr11.posy = (float)((ptr8.pos.y - ptr12.y) * 0.00025);
        // 						ptr11.posz = (float)((ptr8.pos.z - ptr12.z) * 0.00025);
        // 						ptr11.rotx = ptr8.rot.x;
        // 						ptr11.roty = ptr8.rot.y;
        // 						ptr11.rotz = ptr8.rot.z;
        // 						ptr11.rotw = ptr8.rot.w;
        // 					}
        // 					else
        // 					{
        // 						VectorLF3 vectorLF2;
        // 						vectorLF2.x = ptr8.pos.x - ptr.x;
        // 						vectorLF2.y = ptr8.pos.y - ptr.y;
        // 						vectorLF2.z = ptr8.pos.z - ptr.z;
        // 						Maths.QInvRotateLF_ref(ref ptr2, ref vectorLF2, ref vector);
        // 						ptr11.posx = vector.x;
        // 						ptr11.posy = vector.y;
        // 						ptr11.posz = vector.z;
        // 						Maths.QInvMultiply_ref(ref ptr2, ref ptr8.rot, out identity);
        // 						ptr11.rotx = identity.x;
        // 						ptr11.roty = identity.y;
        // 						ptr11.rotz = identity.z;
        // 						ptr11.rotw = identity.w;
        // 					}
        // 				}
        // 				else
        // 				{
        // 					ptr11.posx = (float)ptr8.pos.x;
        // 					ptr11.posy = (float)ptr8.pos.y;
        // 					ptr11.posz = (float)ptr8.pos.z;
        // 					ptr11.rotx = ptr8.rot.x;
        // 					ptr11.roty = ptr8.rot.y;
        // 					ptr11.rotz = ptr8.rot.z;
        // 					ptr11.rotw = ptr8.rot.w;
        // 					Vector4[] extraPool = spaceDynamicRenderer2.extraPool;
        // 					int modelId = ptr8.modelId;
        // 					extraPool[modelId].x = ptr7.anim;
        // 					extraPool[modelId].z = ptr7.steering;
        // 					extraPool[modelId].w = ptr7.speed;
        // 				}
        // 			}
        // 		}
        // 	}
        //
        // 	u++;
        // 	if (u > 1000)
        // 	{
        // 		Log($"Skipped {s} fleets and {t} units of {v} total");
        // 		u = 0;
        // 	}
        // 	return false;
        // }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.GameTickLogic))]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.KeyTickLogic))]
        public static bool EDFHGTL(ref EnemyDFHiveSystem __instance)
        {
            if (Config.SkipDFHiveLogic)
            {
                if ((__instance.starData.uPosition - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
                {
                    return false;
                }

                // return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.GameTickLogic))]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.KeyTickLogic))]
        public static bool EDFHGTL(ref EnemyDFGroundSystem __instance)
        {
            if (Config.SkipDFHiveLogic)
            {
                if ((__instance.planet.uPosition - GameMain.mainPlayer.uPosition).magnitude > Config.SkipDFHiveDistance * 2400000f)
                {
                    return false;
                }

                // return false;
            }

            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SpaceColliderLogic), nameof(SpaceColliderLogic.Init))]
        public static IEnumerable<CodeInstruction> InitTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(i => i.opcode == Ldc_I4 && Convert.ToInt32(i.operand) == 512)
                );
            if (matcher.IsValid)
            {
                matcher.Repeat(matcher => { matcher.SetOperandAndAdvance(8192); });
            }
            else
            {
                Error("Failed to patch SpaceColliderLogic.Init");
            }

            return instructions;
        }


        //  [HarmonyPrefix]
        //  [HarmonyPatch(typeof(EvolveData), nameof(EvolveData.AddExpPoint))]
        //  public static bool AddExpPoint(ref EvolveData __instance, int _addexpp)
        //  {
        //   if (__instance.level >= 100)
        //   {
        //    if (__instance.expf != 0 || __instance.expp != 0 || __instance.level != 100)
        //    {
        //     __instance.level = 100;
        //     __instance.expf = 0;
        //     __instance.expp = 0;
        //     __instance.expl = EvolveData.LevelCummulativeExp(100);
        //    }
        //    return false;
        //   }
        //   if (_addexpp > 0)
        //   {
        //    __instance.expp += _addexpp;
        //    if (__instance.expp >= 10000)
        //    {
        //     __instance.expf += __instance.expp / 10000;
        //     __instance.expp %= 10000;
        //     while (__instance.expf >= EvolveData.levelExps[__instance.level])
        //     {
        // 	    int num = EvolveData.levelExps.Length - 1;
        // 	    __instance.expf -= EvolveData.levelExps[__instance.level];
        // 	    __instance.expl += EvolveData.levelExps[__instance.level];
        // 	    __instance.level++;
        // 	    if (__instance.level >= num)
        // 	    {
        // 		    __instance.level = num;
        // 		    __instance.expf = 0;
        // 		    __instance.expp = 0;
        // 		    return false;
        // 	    }
        //     }
        //    }
        //   }
        //
        //   return false;
        //  }
        //  [HarmonyPrefix]
        //  [HarmonyPatch(typeof(EvolveData), nameof(EvolveData.AddExp))]
        //  public static bool AddExp(ref EvolveData __instance, int _addexp)
        //  {
        //   if (__instance.level >= 100)
        //   {
        //    if (__instance.expf != 0 || __instance.expp != 0 || __instance.level != 100)
        //    {
        //     __instance.level = 100;
        //     __instance.expf = 0;
        //     __instance.expp = 0;
        //     __instance.expl = EvolveData.LevelCummulativeExp(100);
        //    }
        //    return false;
        //   }
        //   __instance.expf += _addexp;
        //   while (__instance.expf >= EvolveData.levelExps[__instance.level])
        //   {
        //    int num = EvolveData.levelExps.Length - 1;
        //    __instance.expf -= EvolveData.levelExps[__instance.level];
        //    __instance.expl += EvolveData.levelExps[__instance.level];
        //    __instance.level++;
        //    if (__instance.level >= num)
        //    {
        //     __instance.level = num;
        //     __instance.expf = 0;
        //     return false;
        //    }
        //   }
        //
        //   return false;
        //  }
        //
        // [HarmonyPrefix, HarmonyPatch(typeof(UISpaceGuideEntry), "OnObjectChange")]
        // public static bool OnObjectChange(ref UISpaceGuideEntry __instance, ESpaceGuideType ___guideType,
        //     float ___radius, int ___objId, GalaxyData ___galaxy, ref RectTransform ___rectTrans, ref Text ___nameText,
        //     ref Image ___markIcon)
        // {
        //     __instance.galaxy = __instance.parent.galaxy;
        //     __instance.gameCamera = __instance.parent.gameCamera;
        //     __instance.parentRectTrans = __instance.parent.rectTrans;
        //     __instance.playerTrans = __instance.parent.player.transform;
        //     if (__instance.guideType == ESpaceGuideType.Star)
        //     {
        //         StarData starData = __instance.galaxy.StarById(__instance.objId);
        //         __instance.nameText.text = ((starData != null) ? starData.displayName : "Star");
        //         float preferredWidth = __instance.nameText.preferredWidth;
        //         __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth + 42f, 80f);
        //         __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth * 0.4f + 40f, 0f);
        //         __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth * 0.4f + 60f, 0f);
        //         __instance.markIcon.sprite = __instance.starSprite;
        //         __instance.markIcon.color = new Color(1f, 1f, 1f, 0.25f);
        //         __instance.collImage.raycastTarget = true;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.Planet)
        //     {
        //         PlanetData planetData = __instance.galaxy.PlanetById(__instance.objId);
        //         __instance.nameText.text = ((planetData != null)
        //             ? ((__instance.radius > 0f)
        //                 ? planetData.displayName
        //                 : (planetData.displayName + " - " + planetData.star.displayName))
        //             : "Planet");
        //         float preferredWidth2 = __instance.nameText.preferredWidth;
        //         __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth2 + 42f, 80f);
        //         __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth2 * 0.4f + 40f, 0f);
        //         __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth2 * 0.4f + 60f, 0f);
        //         __instance.markIcon.sprite = __instance.circleSprite;
        //         __instance.markIcon.color = new Color(1f, 1f, 1f, 0.5f);
        //         __instance.collImage.raycastTarget = true;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.Ship)
        //     {
        //         ItemProto itemProto = LDB.items.Select(__instance.itemId);
        //         __instance.nameText.text = ((itemProto != null) ? itemProto.name : "运输船".Translate());
        //         __instance.markIcon.sprite = __instance.shipSprite;
        //         __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
        //         __instance.collImage.raycastTarget = false;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.DFHive)
        //     {
        //         GS2.DevLog(__instance.objId + "/" + GameMain.data.spaceSector.dfHivesByAstro.Length + " " + __instance.hivecodes.Length);
        //         EnemyDFHiveSystem enemyDFHiveSystem = GameMain.data.spaceSector.dfHivesByAstro[__instance.objId - 1000000];
        //         DevLog(GameMain.data.spaceSector.dfHivesByAstro.Length + " " + " " + enemyDFHiveSystem.hiveOrbitIndex.ToString() + __instance.hivecodes.Length);
        //         __instance.nameText.text = " " + __instance.hivecodes[enemyDFHiveSystem.hiveOrbitIndex % __instance.hivecodes.Length] + " " + "巢穴简称".Translate();
        //         float preferredWidth3 = __instance.nameText.preferredWidth;
        //         __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth3 + 42f, 80f);
        //         __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth3 * 0.4f + 40f, 0f);
        //         __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth3 * 0.4f + 60f, 0f);
        //         __instance.markIcon.sprite = __instance.hiveSprite;
        //         __instance.markIcon.color = __instance.enemyColor;
        //         __instance.collImage.raycastTarget = true;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.DFCarrier)
        //     {
        //         __instance.nameText.text = "黑雾运输船".Translate();
        //         __instance.markIcon.sprite = __instance.shipSprite;
        //         __instance.markIcon.color = __instance.enemyColor;
        //         __instance.collImage.raycastTarget = true;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.Rocket)
        //     {
        //         __instance.nameText.text = "火箭".Translate();
        //         __instance.markIcon.sprite = __instance.rocketSprite;
        //         __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
        //         __instance.collImage.raycastTarget = false;
        //     }
        //     else if (__instance.guideType == ESpaceGuideType.CosmicMessage ||
        //              __instance.guideType == ESpaceGuideType.DFCommunicator)
        //     {
        //         __instance.nameText.text = ((__instance.guideType == ESpaceGuideType.CosmicMessage)
        //             ? "宇宙讯息".Translate()
        //             : "黑雾通讯器".Translate());
        //         float preferredWidth4 = __instance.nameText.preferredWidth;
        //         __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth4 + 42f, 80f);
        //         __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth4 * 0.4f + 40f, 0f);
        //         __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth4 * 0.4f + 60f, 0f);
        //         __instance.markIcon.sprite = __instance.msgSpite;
        //         __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
        //         __instance.collImage.raycastTarget = true;
        //     }
        //
        //     Color color =
        //         ((__instance.guideType == ESpaceGuideType.DFHive || __instance.guideType == ESpaceGuideType.DFCarrier)
        //             ? __instance.enemyTextColor
        //             : __instance.normalTextColor);
        //     __instance.nameText.color = color;
        //     __instance.distText.color = color;
        //     __instance.UpdatePinButtonRotation();
        //     return false;
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Init))]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.Import))]
        public static void EnemyDFHiveSystemInitImport(ref EnemyDFHiveSystem __instance)
        {
            if (__instance.idleRelayIds.Length < 2048)
            {
                var newArray = new int[2048];
                Array.Copy(__instance.idleRelayIds, newArray, __instance.idleRelayIds.Length);
                __instance.idleRelayIds = newArray;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Inspect), nameof(PlayerAction_Inspect.GetObjectSelectDistance))]
        public static void GetObjectSelectDistance(ref PlayerAction_Inspect __instance, ref float __result, EObjectType objType, int objid)
        {
            if (objid == 0)
            {
                return;
            }

            if (__instance.player.factory == null)
            {
                return;
            }

            if (objType != EObjectType.Entity) return;
            var id = __instance.player.factory.entityPool[objid].protoId;
            if (id == 2107 || id == 2103 || id == 2104) __result = 2000f;
            if (id == 2105) __result = 15000f;
            if (__result == 35f) __result = 50f;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefenseSystem), nameof(DefenseSystem.AfterTurretsImport))]
        private static void AfterTurretsImport(ref DefenseSystem __instance)
        {
            int cursor = __instance.turrets.cursor;
            TurretComponent[] buffer = __instance.turrets.buffer;
            for (int i = 1; i < cursor; i++)
            {
                ref TurretComponent ptr = ref buffer[i];
                if (ptr.id == 1) TurretComponentTranspiler.AddTurret(__instance, ref ptr);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.ApplyGravity))]
        private static bool ApplyGravity(ref PlayerController __instance)
        {
            if (Config.NewGravity) return true;
            VectorLF3 v = VectorLF3.zero;
            if (GameMain.localStar != null)
            {
                StarData localStar = GameMain.localStar;
                double num = 0.0;
                VectorLF3 lhs = VectorLF3.zero;
                for (int i = 0; i < localStar.planetCount; i++)
                {
                    PlanetData planetData = localStar.planets[i];
                    VectorLF3 lhs2 = planetData.uPosition - __instance.player.uPosition;
                    double magnitude = lhs2.magnitude;
                    if (magnitude > 1.0)
                    {
                        double y = (double)Math.Max((800f - (float)magnitude) / 150f, 0f);
                        double num2 = Math.Pow(10.0, y);
                        VectorLF3 lhs3 = lhs2 / magnitude;
                        double num3 = magnitude / (double)planetData.realRadius;
                        double num4 = num3 * 0.800000011920929;
                        double num5 = ((num3 < 1.0) ? num3 : (1.0 / (num4 * num4)));
                        if (num5 > 1.0)
                        {
                            num5 = 1.0;
                        }

                        double num6 = Math.Sqrt((double)planetData.realRadius) * 3.5;
                        lhs += lhs3 * (num6 * num5 * num2);
                        num += num2;
                    }
                }

                VectorLF3 lhs4 = localStar.uPosition - __instance.player.uPosition;
                double magnitude2 = lhs4.magnitude;
                if (magnitude2 > 1.0)
                {
                    double num7 = 1.0;
                    VectorLF3 lhs5 = lhs4 / magnitude2;
                    double num8 = magnitude2 / (double)(localStar.orbitScaler * 800f);
                    double num9 = num8 * 0.10000000149011612;
                    double num10 = ((num8 < 1.0) ? num8 : (1.0 / (num9 * num9)));
                    if (num10 > 1.0)
                    {
                        num10 = 1.0;
                    }

                    double num11 = 26.7;
                    lhs += lhs5 * (num11 * num10 * num7);
                    num += num7;
                }

                v = lhs / num;
            }

            if (v.sqrMagnitude > 1E-06)
            {
                __instance.universalGravity = v;
                __instance.localGravity = Maths.QInvRotateLF(__instance.gameData.relativeRot, v);
            }
            else
            {
                __instance.universalGravity = VectorLF3.zero;
                __instance.localGravity = Vector3.zero;
            }

            if (!__instance.player.sailing && !__instance.gameData.disableController && __instance.player.isAlive)
            {
                __instance.AddLocalForce(__instance.localGravity);
            }

            Debug.DrawRay(__instance.transform.localPosition, __instance.localGravity * 0.1f, Color.white);
            if (__instance.gameData.localPlanet != null)
            {
                Vector3 forward = __instance.transform.forward;
                Vector3 normalized = __instance.transform.localPosition.normalized;
                Vector3 normalized2 = Vector3.Cross(Vector3.Cross(normalized, forward).normalized, normalized).normalized;
                __instance.transform.localRotation = Quaternion.LookRotation(normalized2, normalized);
                return false;
            }

            __instance.transform.localRotation = Quaternion.identity;
            return false;
        }
        //     private static bool ApplyGravity(ref PlayerController __instance)
        //     {
        // 	    // return false;
        // 	VectorLF3 vectorLF = VectorLF3.zero;
        // 	if (GameMain.localStar != null)
        // 	{
        // 		StarData localStar = GameMain.localStar;
        // 		double num = 0.0;
        // 		VectorLF3 vectorLF2 = VectorLF3.zero;
        // 		// for (int i = 0; i < localStar.planetCount; i++)
        // 		// {
        // 		// 	PlanetData planetData = localStar.planets[i];
        // 		// 	VectorLF3 vectorLF3 = planetData.uPosition - __instance.player.uPosition;
        // 		// 	double magnitude = vectorLF3.magnitude;
        // 		// 	if (magnitude > 1.0)
        // 		// 	{
        // 		// 		double num2 = (double)Math.Max((5000f + planetData.realRadius - (float)magnitude) / 2500f, 0f);
        // 		// 		double num3 = Math.Pow(11.0, num2) - 1.0;
        // 		// 		VectorLF3 vectorLF4 = vectorLF3 / magnitude;
        // 		// 		double num4 = magnitude / (double)planetData.realRadius;
        // 		// 		double num5 = num4 * 0.800000011920929;
        // 		// 		double num6 = ((num4 < 1.0) ? num4 : (1.0 / (num5 * num5)));
        // 		// 		if (num6 > 1.0)
        // 		// 		{
        // 		// 			num6 = 1.0;
        // 		// 		}
        // 		// 		double num7 = Math.Sqrt((double)planetData.realRadius) * 3.5;
        // 		// 		vectorLF2 += vectorLF4 * (num7 * num6 * num3);
        // 		// 		num += num3;
        // 		// 	}
        // 		// }
        // 		VectorLF3 vectorLF5 = localStar.uPosition - __instance.player.uPosition;
        // 		double num8 = vectorLF5.magnitude;
        // 		if (num8 > 1.0)
        // 		{
        // 			double num9 = 1.0;
        // 			VectorLF3 vectorLF6 = vectorLF5 / num8;
        // 			double num10 = 64000000000000.0 * (double)localStar.mass * 1.3538551990520382E-06 * 4.0;
        // 			if (num8 < (double)localStar.physicsRadius)
        // 			{
        // 				num8 = (double)localStar.physicsRadius;
        // 			}
        // 			VectorLF3 vectorLF7 = vectorLF6 * (num10 / (num8 * num8));
        // 			vectorLF2 += vectorLF7;
        // 			num += num9;
        // 		}
        // 		vectorLF = vectorLF2 / num;
        // 	}
        // 	if (vectorLF.sqrMagnitude > 1E-06)
        // 	{
        // 		__instance.universalGravity = vectorLF;
        // 		__instance.localGravity = Maths.QInvRotateLF(__instance.gameData.relativeRot, vectorLF);
        // 	}
        // 	else
        // 	{
        // 		__instance.universalGravity = VectorLF3.zero;
        // 		__instance.localGravity = Vector3.zero;
        // 	}
        // 	if (!__instance.player.sailing && !__instance.gameData.disableController && __instance.player.isAlive)
        // 	{
        // 		__instance.universalGravity = VectorLF3.zero;
        // 		__instance.localGravity = Vector3.zero;
        // 		__instance.AddLocalForce(__instance.localGravity);
        // 	}
        // 	// Debug.DrawRay(base.transform.localPosition, __instance.localGravity * 0.1f, Color.white);
        // 	if (__instance.gameData.localPlanet != null)
        // 	{
        // 		Vector3 forward = __instance.transform.forward;
        // 		Vector3 normalized = __instance.transform.localPosition.normalized;
        // 		Vector3 normalized2 = Vector3.Cross(Vector3.Cross(normalized, forward).normalized, normalized).normalized;
        // 		__instance.transform.localRotation = Quaternion.LookRotation(normalized2, normalized);
        // 		return false;
        // 	}
        // 	__instance.transform.localRotation = Quaternion.identity;
        // 	return false;
        // 	
        // }
        // [HarmonyPostfix,
        //  HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.RunBehavior_Engage_GRaider))]
        // public static void RunBehavior_Engage_GRaider(ref EnemyUnitComponent __instance, PlanetFactory factory, ref EnemyData enemy)
        // {
        //  foreach (var e in factory.enemySystem.units.buffer)
        //  {
        //   var pos =  factory.enemyPool[e.enemyId].pos;
        //   var mag = pos.magnitude;
        //   if (mag < 300f && mag != 0) factory.enemyPool[e.enemyId].pos *= 1.01f;
        //  }
        // }
        //  [HarmonyPrefix, HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.SetForNewCreate))]
        //  public static bool SetForNewCreate(ref EnemyDFHiveSystem __instance)
        //  {
        //   Log($"{__instance.starData == null} {__instance.starData.hiveAstroOrbits == null}");
        //   Log($"{__instance.hiveOrbitIndex.ToString()}/{__instance.starData.hiveAstroOrbits.Length}");
        //   Log($"--{__instance.starData.index * 8 + __instance.hiveOrbitIndex + 1}/{__instance.sector.astros.Length}");
        //   Log(":)");
        //   return true;
        //  }

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