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
	    [HarmonyTranspiler]
	    [HarmonyPatch(typeof(SpaceColliderLogic), nameof(SpaceColliderLogic.Init))]
	    public static IEnumerable<CodeInstruction> InitTranspiler(IEnumerable<CodeInstruction> instructions)
	    {
		    var matcher = new CodeMatcher(instructions)
			    .MatchForward(false,
				    new CodeMatch(i=>i.opcode == Ldc_I4 && Convert.ToInt32(i.operand) == 512)
			    );
		    if (matcher.IsValid)
		    {
			    matcher.Repeat(matcher =>
			    {
				    matcher.SetOperandAndAdvance(8192);
			    });
		    }
		    else
		    {
			    Error("Failed to patch SpaceColliderLogic.Init");
		    }
		    return instructions;
	    }
	    
	    
	    [HarmonyTranspiler]
	    [HarmonyPatch(typeof(LocalGeneralProjectile),  nameof(LocalGeneralProjectile.TickSkillLogic))] //225f 212f
	    public static IEnumerable<CodeInstruction> Fix39000(IEnumerable<CodeInstruction> instructions)
	    {
		    // var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromFactory));
            
		    var matcher = new CodeMatcher(instructions)
			    .MatchForward(
				    true,
				    new CodeMatch(i =>
				    {
					    return (i.opcode == Ldc_R4 ) &&
					           (
						           Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 39006.25) < 1 

					           );
				    })
			    );
		    if (matcher.IsInvalid) GS2.Error("Nope");
			   matcher.Repeat(matcher =>
			    {
				    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
				    // var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
				    // var mi = matcher.GetRadiusFromFactory();
				    // matcher.LogILPre();
				    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
				    matcher.InsertAndAdvance(new CodeInstruction(Utils.LoadField(typeof(LocalGeneralProjectile),
					    nameof(LocalGeneralProjectile.astroId))));
				    matcher.SetInstruction(new CodeInstruction(Call, matcher.GetSquareRadiusFromAstroFactoryId()));
				    // matcher.LogILPost(3);
			    });

				   instructions = matcher.InstructionEnumeration();

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
	    
	    [HarmonyPrefix, HarmonyPatch(typeof(UISpaceGuideEntry), "OnObjectChange")]
	    public static bool OnObjectChange(ref UISpaceGuideEntry __instance, ESpaceGuideType ___guideType,
		    float ___radius, int ___objId, GalaxyData ___galaxy, ref RectTransform ___rectTrans, ref Text ___nameText,
		    ref Image ___markIcon)
	    {

		    __instance.galaxy = __instance.parent.galaxy;
		    __instance.gameCamera = __instance.parent.gameCamera;
		    __instance.parentRectTrans = __instance.parent.rectTrans;
		    __instance.playerTrans = __instance.parent.player.transform;
		    if (__instance.guideType == ESpaceGuideType.Star)
		    {
			    StarData starData = __instance.galaxy.StarById(__instance.objId);
			    __instance.nameText.text = ((starData != null) ? starData.displayName : "Star");
			    float preferredWidth = __instance.nameText.preferredWidth;
			    __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth + 42f, 80f);
			    __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth * 0.4f + 40f, 0f);
			    __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth * 0.4f + 60f, 0f);
			    __instance.markIcon.sprite = __instance.starSprite;
			    __instance.markIcon.color = new Color(1f, 1f, 1f, 0.25f);
			    __instance.collImage.raycastTarget = true;
		    }
		    else if (__instance.guideType == ESpaceGuideType.Planet)
		    {
			    PlanetData planetData = __instance.galaxy.PlanetById(__instance.objId);
			    __instance.nameText.text = ((planetData != null)
				    ? ((__instance.radius > 0f)
					    ? planetData.displayName
					    : (planetData.displayName + " - " + planetData.star.displayName))
				    : "Planet");
			    float preferredWidth2 = __instance.nameText.preferredWidth;
			    __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth2 + 42f, 80f);
			    __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth2 * 0.4f + 40f, 0f);
			    __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth2 * 0.4f + 60f, 0f);
			    __instance.markIcon.sprite = __instance.circleSprite;
			    __instance.markIcon.color = new Color(1f, 1f, 1f, 0.5f);
			    __instance.collImage.raycastTarget = true;
		    }
		    else if (__instance.guideType == ESpaceGuideType.Ship)
		    {
			    ItemProto itemProto = LDB.items.Select(__instance.itemId);
			    __instance.nameText.text = ((itemProto != null) ? itemProto.name : "运输船".Translate());
			    __instance.markIcon.sprite = __instance.shipSprite;
			    __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
			    __instance.collImage.raycastTarget = false;
		    }
		    else if (__instance.guideType == ESpaceGuideType.DFHive)
		    {
			    GS2.DevLog(__instance.objId + "/" +GameMain.data.spaceSector.dfHivesByAstro.Length + " " + __instance.hivecodes.Length );
			    EnemyDFHiveSystem enemyDFHiveSystem = GameMain.data.spaceSector.dfHivesByAstro[__instance.objId - 1000000];
			    DevLog(GameMain.data.spaceSector.dfHivesByAstro.Length + " " + " " + enemyDFHiveSystem.hiveOrbitIndex.ToString() + __instance.hivecodes.Length );
			    __instance.nameText.text = " " + __instance.hivecodes[enemyDFHiveSystem.hiveOrbitIndex % __instance.hivecodes.Length] + " " + "巢穴简称".Translate();
			    float preferredWidth3 = __instance.nameText.preferredWidth;
			    __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth3 + 42f, 80f);
			    __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth3 * 0.4f + 40f, 0f);
			    __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth3 * 0.4f + 60f, 0f);
			    __instance.markIcon.sprite = __instance.hiveSprite;
			    __instance.markIcon.color = __instance.enemyColor;
			    __instance.collImage.raycastTarget = true;
		    }
		    else if (__instance.guideType == ESpaceGuideType.DFCarrier)
		    {
			    __instance.nameText.text = "黑雾运输船".Translate();
			    __instance.markIcon.sprite = __instance.shipSprite;
			    __instance.markIcon.color = __instance.enemyColor;
			    __instance.collImage.raycastTarget = true;
		    }
		    else if (__instance.guideType == ESpaceGuideType.Rocket)
		    {
			    __instance.nameText.text = "火箭".Translate();
			    __instance.markIcon.sprite = __instance.rocketSprite;
			    __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
			    __instance.collImage.raycastTarget = false;
		    }
		    else if (__instance.guideType == ESpaceGuideType.CosmicMessage ||
		             __instance.guideType == ESpaceGuideType.DFCommunicator)
		    {
			    __instance.nameText.text = ((__instance.guideType == ESpaceGuideType.CosmicMessage)
				    ? "宇宙讯息".Translate()
				    : "黑雾通讯器".Translate());
			    float preferredWidth4 = __instance.nameText.preferredWidth;
			    __instance.nameText.rectTransform.sizeDelta = new Vector2(preferredWidth4 + 42f, 80f);
			    __instance.pinRectTrans.anchoredPosition = new Vector2(preferredWidth4 * 0.4f + 40f, 0f);
			    __instance.indicatorRectTrans.anchoredPosition = new Vector2(preferredWidth4 * 0.4f + 60f, 0f);
			    __instance.markIcon.sprite = __instance.msgSpite;
			    __instance.markIcon.color = new Color(1f, 1f, 1f, 0.8f);
			    __instance.collImage.raycastTarget = true;
		    }

		    Color color =
			    ((__instance.guideType == ESpaceGuideType.DFHive || __instance.guideType == ESpaceGuideType.DFCarrier)
				    ? __instance.enemyTextColor
				    : __instance.normalTextColor);
		    __instance.nameText.color = color;
		    __instance.distText.color = color;
		    __instance.UpdatePinButtonRotation();
		    return false;
	    }

	    [HarmonyPostfix]
	    [HarmonyPatch(typeof(EnemyDFHiveSystem),nameof(EnemyDFHiveSystem.Init))]
	    [HarmonyPatch(typeof(EnemyDFHiveSystem),nameof(EnemyDFHiveSystem.Import))]
	    public static void EnemyDFHiveSystemInitImport(ref EnemyDFHiveSystem __instance)
	    {
		    if (__instance.idleRelayIds.Length < 2048)
		    {
			    var newArray = new int[2048];
			    Array.Copy(__instance.idleRelayIds,newArray, __instance.idleRelayIds.Length);
			    __instance.idleRelayIds = newArray;
		    }
	    }
	    
	    [HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Inspect),nameof(PlayerAction_Inspect.GetObjectSelectDistance))]
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