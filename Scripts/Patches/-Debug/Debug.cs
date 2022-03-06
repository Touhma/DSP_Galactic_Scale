using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
	    [HarmonyPrefix]
	    [HarmonyPatch(typeof(GameHistoryData), "SetForNewGame")]
        public static bool SetForNewGame(GameHistoryData __instance)
{
	__instance.recipeUnlocked.Clear();
	__instance.tutorialUnlocked.Clear();
	__instance.featureKeys.Clear();
	__instance.featureValues.Clear();
	__instance.pinnedPlanets.Clear();
	__instance.journalSystem.SetForNewGame();
	int[] recipes = Configs.freeMode.recipes;
	if (recipes != null)
	{
		for (int i = 0; i < recipes.Length; i++)
		{
			__instance.recipeUnlocked.Add(recipes[i]);
		}
	}
	__instance.techStates.Clear();
	TechProto[] dataArray = LDB.techs.dataArray;
	for (int j = 0; j < dataArray.Length; j++)
	{
		int upoint = 0;
		if (dataArray[j].Items.Length != 0 && dataArray[j].Items[0] == 6006)
		{
			upoint = dataArray[j].ItemPoints[0];
		}
		TechState value = new TechState(false, dataArray[j].Level, dataArray[j].MaxLevel, 0L, dataArray[j].GetHashNeeded(dataArray[j].Level), upoint);
		// GS2.Warn($"{dataArray[j].Name.Translate()} {dataArray[j].ID} {dataArray[j].Desc.Translate()}");
		if (!__instance.techStates.ContainsKey(dataArray[j].ID)) __instance.techStates.Add(dataArray[j].ID, value);
		else
		{
			GS2.Warn($"Duplicate Tech ID {dataArray[j].Name.Translate()} {dataArray[j].ID} Ignored");
		}
	}
	int[] techs = Configs.freeMode.techs;
	if (techs != null)
	{
		for (int k = 0; k < techs.Length; k++)
		{
			__instance.UnlockTech(techs[k]);
		}
	}
	__instance.autoManageLabItems = true;
	__instance.currentTech = 0;
	__instance.techQueue = new int[8];
	__instance.universeObserveLevel = Configs.freeMode.universeObserveLevel;
	__instance.blueprintLimit = Configs.freeMode.blueprintLimit;
	__instance.solarSailLife = Configs.freeMode.solarSailLife;
	__instance.solarEnergyLossRate = Configs.freeMode.solarEnergyLossRate;
	__instance.useIonLayer = Configs.freeMode.useIonLayer;
	__instance.inserterStackCount = Configs.freeMode.inserterStackCount;
	__instance.logisticDroneSpeed = Configs.freeMode.logisticDroneSpeed;
	__instance.logisticDroneSpeedScale = 1f;
	__instance.logisticDroneCarries = Configs.freeMode.logisticDroneCarries;
	__instance.logisticShipSailSpeed = Configs.freeMode.logisticShipSailSpeed;
	__instance.logisticShipWarpSpeed = Configs.freeMode.logisticShipWarpSpeed;
	__instance.logisticShipSpeedScale = 1f;
	__instance.logisticShipWarpDrive = Configs.freeMode.logisticShipWarpDrive;
	__instance.logisticShipCarries = Configs.freeMode.logisticShipCarries;
	__instance.miningCostRate = Configs.freeMode.miningCostRate;
	__instance.miningSpeedScale = Configs.freeMode.miningSpeedScale;
	__instance.storageLevel = 2;
	__instance.labLevel = 3;
	__instance.techSpeed = Configs.freeMode.techSpeed;
	__instance.dysonNodeLatitude = 0f;
	__instance.universeMatrixPointUploaded = 0L;
	__instance.missionAccomplished = false;
	__instance.stationPilerLevel = 1;
	__instance.remoteStationExtraStorage = 0;
	__instance.localStationExtraStorage = 0;
	return false;
}
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(BuildTool_BlueprintPaste), "OperatingPrestage")]
        // public static bool OperatingPrestage(BuildTool_BlueprintPaste __instance)
        // {
	       //  GS2.Warn($"{__instance.castGroundPosSnapped}");
	       //  return true;
        // }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(BlueprintUtils), "GetSnappedLatitudeGridIdx", typeof(float), typeof(int))]
        // public static bool GetSnappedLatitudeGridIdx(ref int __result, float _latitudeRad, int _segmentCnt = 200)
        // {
	       //  GS2.Warn($"_latitudeRad:{_latitudeRad} radperGrid:{BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt)}");
	       //  __result = BlueprintUtils._round2int(_latitudeRad / BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt));
	       //  return false;
        // }
        
        
//         [HarmonyPrefix]
//         [HarmonyPatch(typeof(BlueprintUtils), "SnapTropic")]
//         public static bool SnapTropic(PlayerAction_Build _actionBuild, BlueprintData _blueprintData, Vector3[] _dots, int _dotsCursor, float _yaw, int _segmentCnt = 200)
// {
// 	if (_blueprintData.areas.Length <= 1)
// 	{
// 		return false;
// 	}
// 	int num = Mathf.FloorToInt(_yaw / 89.9f);
// 	int num2 = (num == 2 || num == 3) ? -1 : 1;
// 	BlueprintArea blueprintArea = _blueprintData.areas[_blueprintData.primaryAreaIdx];
// 	BlueprintArea blueprintArea2 = (_blueprintData.primaryAreaIdx == _blueprintData.areas.Length - 1) ? _blueprintData.areas[_blueprintData.primaryAreaIdx - 1] : _blueprintData.areas[_blueprintData.primaryAreaIdx + 1];
// 	Vector3 vector = _dots[0];
// 	Vector3 normalized = vector.normalized;
// 	float num3 = BlueprintUtils.GetLongitudeRad(normalized);
// 	float latitudeRad = BlueprintUtils.GetLatitudeRad(normalized);
// 	int snappedLatitudeGridIdx = BlueprintUtils.GetSnappedLatitudeGridIdx(latitudeRad, _segmentCnt);
// 	int num4 = (_blueprintData.primaryAreaIdx == _blueprintData.areas.Length - 1) ? (snappedLatitudeGridIdx - blueprintArea.height * num2) : (snappedLatitudeGridIdx + blueprintArea.height * num2);
// 	int num5 = 0;
// 	bool flag = false;
// 	int longitudeSegCnt = 0;
// 	int num6 = 0;
// 	for (int i = 0; i < _blueprintData.areas.Length; i++)
// 	{
// 		num6 += _blueprintData.areas[i].height;
// 	}
// 	int num7 = -Math.Max(num6 / 2, 6);
// 	int num8 = Math.Max(num6 / 2, 6);
// 	for (int j = num7; j <= num8; j++)
// 	{
// 		int num9 = (_blueprintData.primaryAreaIdx == _blueprintData.areas.Length - 1) ? (snappedLatitudeGridIdx + j) : (num4 + j);
// 		int latitudeGridIdx = (snappedLatitudeGridIdx > num4) ? (num9 + 1) : (num9 - 1);
// 		if (num9 > 50)
// 		{
// 			GS2.Warn($"_dots[0]:{_dots[0]} _dotscursor:{_dotsCursor}SnapTropic num9:{num9} lattitudeGridIdx:{latitudeGridIdx} snappedLatitudeGridIdx:{snappedLatitudeGridIdx} num4:{num4} primaryAreaIdx:{_blueprintData.primaryAreaIdx} _blueprintData.areas.Length:{_blueprintData.areas.Length}" + 
// 			         $"height:{blueprintArea.height} num2:{num2} num:{num} _yaw:{_yaw} _segmentCnt:{_segmentCnt}");
// 		}
// 		int longitudeSegmentCount = BlueprintUtils.GetLongitudeSegmentCount(num9, _segmentCnt);
// 		int longitudeSegmentCount2 = BlueprintUtils.GetLongitudeSegmentCount(latitudeGridIdx, _segmentCnt);
// 		if (longitudeSegmentCount != longitudeSegmentCount2)
// 		{
// 			if (_blueprintData.areas.Length == 2 && blueprintArea2.areaSegments > 4)
// 			{
// 				if (Mathf.Abs((float)blueprintArea.areaSegments / (float)blueprintArea2.areaSegments - (float)longitudeSegmentCount2 / (float)longitudeSegmentCount) > 0.0001f)
// 				{
// 					goto IL_1D2;
// 				}
// 			}
// 			else if (blueprintArea.areaSegments != longitudeSegmentCount2 || blueprintArea2.areaSegments != longitudeSegmentCount)
// 			{
// 				goto IL_1D2;
// 			}
// 			longitudeSegCnt = longitudeSegmentCount2;
// 			num5 = ((_blueprintData.primaryAreaIdx == _blueprintData.areas.Length - 1) ? (j + num2) : j);
// 			flag = true;
// 			break;
// 		}
// 		IL_1D2:;
// 	}
// 	if (flag)
// 	{
// 		float latitudeRadPerGrid = BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt);
// 		float magnitude = vector.magnitude;
// 		int num10 = 0;
// 		float longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(longitudeSegCnt, _segmentCnt);
// 		float longitudeRadPerGrid2 = BlueprintUtils.GetLongitudeRadPerGrid(latitudeRad + (float)num5 * latitudeRadPerGrid, _segmentCnt);
// 		num3 = (float)Mathf.RoundToInt(num3 / longitudeRadPerGrid2) * longitudeRadPerGrid2;
// 		BlueprintUtils.SnapLongitude(_blueprintData, BlueprintUtils.GetDir(num3, latitudeRad + (float)num5 * latitudeRadPerGrid), _yaw, ref num10, _segmentCnt);
// 		float num11 = num3;
// 		float num12 = latitudeRad;
// 		num11 += (float)num10 * longitudeRadPerGrid;
// 		num12 += (float)num5 * latitudeRadPerGrid;
// 		vector = _actionBuild.planetAux.Snap(BlueprintUtils.GetDir(num11, num12) * magnitude, true);
// 		Vector3 normalized2 = vector.normalized;
// 		float num13 = BlueprintUtils.GetLongitudeRad(normalized2) - num11;
// 		float num14 = BlueprintUtils.GetLatitudeRad(normalized2) - num12;
// 		_dots[0] = vector;
// 		for (int k = 1; k < _dotsCursor; k++)
// 		{
// 			float num15 = 0f;
// 			float num16 = 0f;
// 			BlueprintUtils.GetLongitudeLatitudeRad(_dots[k].normalized, ref num15, ref num16);
// 			num15 += num13;
// 			num16 += num14;
// 			_dots[k] = BlueprintUtils.GetDir(num11, num12) * magnitude;
// 		}
// 	}
//
// 	return false;
// }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(GameLoader), "CreateLoader")]
        // public static void CreateLoader()
        // {
        //     GS2.Warn("Added Gameloader component");
        //     GS2.Error(":)");
        // }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "typeString", MethodType.Getter)]
        public static bool typeString(ref string __result, PlanetData __instance)
        {
		        __result = "未知".Translate();
		        ThemeProto themeProto = LDB.themes.Select(__instance.theme);
		        if (themeProto != null)
		        {
			        __result = themeProto.displayName;
		        }
		        return false;
	        
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(UIStarmap __instance)
	{
		bool active = false;
		UIStarmapPlanet uistarmapPlanet = __instance.mouseHoverPlanet;
		UIStarmapStar uistarmapStar = __instance.mouseHoverStar;
		if (__instance.focusPlanet != null)
		{
			uistarmapPlanet = __instance.focusPlanet;
			uistarmapStar = null;
			__instance.cursorFunctionGroup.SetActive(true);
			EPin planetPin = GameMain.history.GetPlanetPin(__instance.focusPlanet.planet.id);
			__instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, (float)((planetPin == EPin.Show) ? -90 : ((planetPin == EPin.Hide) ? 90 : 0)));
			__instance.cursorFunctionText1.text = ((planetPin == EPin.Show) ? "天体显示标签" : ((planetPin == EPin.Hide) ? "天体隐藏标签" : "天体自动标签")).Translate();
		}
		else if (__instance.focusStar != null)
		{
			uistarmapPlanet = null;
			uistarmapStar = __instance.focusStar;
			__instance.cursorFunctionGroup.SetActive(true);
			EPin starPin = GameMain.history.GetStarPin(__instance.focusStar.star.id);
			__instance.cursorFunctionIcon1.localEulerAngles = new Vector3(0f, 0f, (float)((starPin == EPin.Show) ? -90 : ((starPin == EPin.Hide) ? 90 : 0)));
			__instance.cursorFunctionText1.text = ((starPin == EPin.Show) ? "天体显示标签" : ((starPin == EPin.Hide) ? "天体隐藏标签" : "天体自动标签")).Translate();
		}
		else
		{
			__instance.cursorFunctionGroup.SetActive(false);
		}
		if (uistarmapPlanet != null)
		{
			uistarmapStar = null;
		}
		if (uistarmapPlanet != null && uistarmapPlanet.projected)
		{
			active = true;
			__instance.cursorViewTrans.anchoredPosition = uistarmapPlanet.projectedCoord;
			if ((UnityEngine.Object)__instance.cursorViewDisplayObject != uistarmapPlanet || __instance.forceUpdateCursorView)
			{
				string str = "";
				PlanetData planet = uistarmapPlanet.planet;
				string arg = "";
				if (planet.singularity > EPlanetSingularity.None || planet.orbitAround > 0)
				{
					arg = "<color=\"#FD965EC0\">" + planet.singularityString + "</color>";
				}
				string text = string.Format("行星类型".Translate() + "\r\n", planet.typeString, arg);
				if (GS2.GetGSPlanet(planet).GsTheme.DisplayName == "Comet") text = "Comet\r\n";
				if (uistarmapPlanet == __instance.focusPlanet)
				{
					text = "<color=\"#FFFFFFB0\">" + __instance.focusPlanet.planet.displayName + "</color>\r\n" + text;
				}
				Player mainPlayer = GameMain.mainPlayer;
				double num = (planet.uPosition - mainPlayer.uPosition).magnitude - (double)planet.realRadius - 50.0;
				string str2;
				if (num < 50.0)
				{
					str2 = string.Format((planet.type != EPlanetType.Gas) ? "已登陆".Translate() : "已靠近".Translate(), Array.Empty<object>());
				}
				else if (num < 5000.0)
				{
					str2 = string.Format("距离米".Translate(), num);
				}
				else if (num < 2400000.0)
				{
					str2 = string.Format("距离日距".Translate(), num / 40000.0);
				}
				else
				{
					str2 = string.Format("距离光年".Translate(), num / 2400000.0);
				}
				double num2 = 0.0001;
				if (mainPlayer.mecha.thrusterLevel >= 2)
				{
					num2 = (double)mainPlayer.mecha.maxSailSpeed;
				}
				if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star == GameMain.localStar)
				{
					num2 += (double)mainPlayer.mecha.maxWarpSpeed * 0.03;
				}
				if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar)
				{
					num2 += (double)mainPlayer.mecha.maxWarpSpeed * 0.98;
				}
				float num3 = (float)(num / num2);
				if (mainPlayer.planetId != 0)
				{
					float num4 = mainPlayer.position.magnitude - mainPlayer.planetData.realRadius;
					num3 += Mathf.Clamp01((800f - num4) / 800f) * 10f;
				}
				if (mainPlayer.mecha.thrusterLevel >= 3 && uistarmapPlanet.planet.star != GameMain.localStar)
				{
					num3 += 24f;
				}
				int num5 = (int)num3 / 3600;
				int num6 = (int)num3 / 60 % 60;
				int num7 = (int)num3 % 60;
				if (num > 900.0)
				{
					if (num3 < 60f)
					{
						str = string.Format("最快秒".Translate(), num7);
					}
					else if (num3 < 600f)
					{
						str = string.Format("最快分秒".Translate(), num6, num7);
					}
					else if (num3 < 3600f)
					{
						str = string.Format("最快分钟".Translate(), num6);
					}
					else if (num3 < 720000f)
					{
						str = string.Format("最快小时".Translate(), num5, num6);
					}
					else if (num < 2400000.0)
					{
						str = string.Format("需要驱动引擎".Translate(), 2);
					}
					else
					{
						str = string.Format("需要驱动引擎".Translate(), 4);
					}
				}
				__instance.cursorViewText.text = text + str2 + str ;
				__instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
				__instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
				__instance.cursorViewDisplayObject = uistarmapPlanet;
			}
		}
		else if (uistarmapStar != null && uistarmapStar.projected)
		{
			active = true;
			__instance.cursorViewTrans.anchoredPosition = uistarmapStar.projectedCoord;
			if ((UnityEngine.Object)__instance.cursorViewDisplayObject != uistarmapStar || __instance.forceUpdateCursorView)
			{
				string str3 = "";
				StarData star = uistarmapStar.star;
				string text2 = star.typeString + "\r\n";
				if (uistarmapStar == __instance.focusStar)
				{
					text2 = "<color=\"#FFFFFFB0\">" + __instance.focusStar.star.displayName + "</color>\r\n" + text2;
				}
				Player mainPlayer2 = GameMain.mainPlayer;
				double num8 = (star.uPosition - mainPlayer2.uPosition).magnitude - (double)star.physicsRadius - 100.0;
				string str4;
				if (num8 < 50.0)
				{
					str4 = string.Format("已靠近".Translate(), Array.Empty<object>());
				}
				else if (num8 < 5000.0)
				{
					str4 = string.Format("距离米".Translate(), num8);
				}
				else if (num8 < 2400000.0)
				{
					str4 = string.Format("距离日距".Translate(), num8 / 40000.0);
				}
				else
				{
					str4 = string.Format("距离光年".Translate(), num8 / 2400000.0);
				}
				double num9 = 0.0001;
				if (mainPlayer2.mecha.thrusterLevel >= 2)
				{
					num9 = (double)mainPlayer2.mecha.maxSailSpeed;
				}
				if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star == GameMain.localStar)
				{
					num9 += (double)mainPlayer2.mecha.maxWarpSpeed * 0.03;
				}
				if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar)
				{
					num9 += (double)mainPlayer2.mecha.maxWarpSpeed * 0.98;
				}
				float num10 = (float)(num8 / num9);
				if (mainPlayer2.planetId != 0)
				{
					float num11 = mainPlayer2.position.magnitude - mainPlayer2.planetData.realRadius;
					num10 += Mathf.Clamp01((800f - num11) / 800f) * 9f;
				}
				if (mainPlayer2.mecha.thrusterLevel >= 3 && uistarmapStar.star != GameMain.localStar)
				{
					num10 += 20f;
				}
				int num12 = (int)num10 / 3600;
				int num13 = (int)num10 / 60 % 60;
				int num14 = (int)num10 % 60;
				if (num8 > 5000.0)
				{
					if (num10 < 60f)
					{
						str3 = string.Format("最快秒".Translate(), num14);
					}
					else if (num10 < 600f)
					{
						str3 = string.Format("最快分秒".Translate(), num13, num14);
					}
					else if (num10 < 3600f)
					{
						str3 = string.Format("最快分钟".Translate(), num13);
					}
					else if (num10 < 720000f)
					{
						str3 = string.Format("需要驱动引擎4".Translate(), num12, num13);
					}
					else if (num8 < 2400000.0)
					{
						str3 = string.Format("需要驱动引擎".Translate(), 2);
					}
					else
					{
						str3 = string.Format("需要驱动引擎".Translate(), 4);
					}
				}
				__instance.cursorViewText.text = text2 + str4 + str3;
				__instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
				__instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
				__instance.cursorViewDisplayObject = uistarmapStar;
			}
		}
		__instance.forceUpdateCursorView = false;
		__instance.cursorViewTrans.gameObject.SetActive(active);
		return false;
	}
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnUpdate")]
        public static void _OnUpdate_Postfix()
        {
            // as we need to load and generate planets for the detail view in the lobby, update the loading process here
            // PlanetModelingManager.ModelingPlanetCoroutine();
            // UIRoot.instance.uiGame.planetDetail._OnUpdate();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVirtualStarmap), "OnGalaxyDataReset")]
        public static bool OnGalaxyDataReset_Prefix(UIVirtualStarmap __instance)
        {
            __instance.clickText = ""; // reset to vanilla

            foreach (UIVirtualStarmap.ConnNode connNode in __instance.connPool)
            {
                connNode.lineRenderer.positionCount = 2;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), "AddHeightMapModLevel")]
        public static bool AddHeightMapModLevel(int index, int level, PlanetData __instance)
        {
            if (__instance.data.AddModLevel(index, level))
            {
                var num = __instance.precision / __instance.segment;
                var num2 = index % __instance.data.stride;
                var num3 = index / __instance.data.stride;
                var num4 = (num2 < __instance.data.substride ? 0 : 1) + (num3 < __instance.data.substride ? 0 : 2);
                var num5 = num2 % __instance.data.substride;
                var num6 = num3 % __instance.data.substride;
                var num7 = (num5 - 1) / num;
                var num8 = (num6 - 1) / num;
                var num9 = num5 / num;
                var num10 = num6 / num;
                if (num9 >= __instance.segment) num9 = __instance.segment - 1;
                if (num10 >= __instance.segment) num10 = __instance.segment - 1;
                var num11 = num4 * __instance.segment * __instance.segment;
                var num12 = num7 + num8 * __instance.segment + num11;
                var num13 = num9 + num8 * __instance.segment + num11;
                var num14 = num7 + num10 * __instance.segment + num11;
                var num15 = num9 + num10 * __instance.segment + num11;
                num12 = Mathf.Clamp(num12, 0, 99);
                num13 = Mathf.Clamp(num13, 0, 99);
                num14 = Mathf.Clamp(num14, 0, 99);
                num15 = Mathf.Clamp(num15, 0, 99);
                __instance.dirtyFlags[num12] = true;
                __instance.dirtyFlags[num13] = true;
                __instance.dirtyFlags[num14] = true;
                __instance.dirtyFlags[num15] = true;
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Inserter), "CheckBuildConditions")]
        public static void BuildToolInserter(BuildTool_Inserter __instance, ref bool __result)
        {
            if (__instance.buildPreviews.Count == 0) return;
            // if (__instance.buildPreviews == null) return;
            var preview = __instance.buildPreviews[0];
            // GS2.Warn(preview?.condition.ToString());

            if (__instance.planet.realRadius < 20)
                if (preview.condition == EBuildCondition.TooSkew)
                {
                    preview.condition = EBuildCondition.Ok;
                    // GS2.Warn("TooSkew");
                    __instance.cursorValid = true; // Prevent red text
                    __result = true; // Override the build condition check
                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    __instance.actionBuild.model.cursorText = "Click to build";
                    __instance.actionBuild.model.cursorState = 0;
                }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "_OnOpen")]
        public static bool UILoadGameWindow_OnOpen()
        {
            //GS2.Warn("Disabled Import");
            GS2.SaveOrLoadWindowOpen = true; // Prevents GSSettings getting overwritten when loading a save for purposes of displaying thumbnail
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "LoadSelectedGame")]
        public static bool UILoadGameWindow_LoadSelectedGame()
        {
            //GS2.Warn("Enabled Import");
            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "_OnClose")]
        public static bool UILoadGameWindow_OnClose()
        {
            //GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnOpen")]
        public static bool UISaveGameWindow_OnOpen()
        {
            //GS2.Warn("Disabled Import");

            GS2.SaveOrLoadWindowOpen = true;
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnClose")]
        public static bool UISaveGameWindow_OnClose()
        {
            //GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAchievementPanel), "LoadData")]
        public static bool LoadData(UIAchievementPanel __instance)
        {
            __instance.uiEntries.Clear(); //Is this necessary?
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarningSystem), "Init")]
        public static void Init(ref WarningSystem __instance)
        {
            //GS2.Warn("Warning System Initializing");
            //GS2.Warn($"Star Count: {GSSettings.StarCount}");
            var planetCount = GSSettings.PlanetCount;
            //GS2.Warn($"Planet Count: {planetCount}");
            //GS2.Warn($"Factory Length: {__instance.gameData.factories.Length}");
            if (__instance.gameData.factories.Length > planetCount) planetCount = __instance.gameData.factories.Length;
            __instance.tmpEntityPools = new EntityData[planetCount][];
            __instance.tmpPrebuildPools = new PrebuildData[planetCount][];
            __instance.tmpSignPools = new SignData[planetCount][];
            //__instance.warningCounts = new int[GameMain.galaxy.starCount * 1024];
            //__instance.warningSignals = new int[GameMain.galaxy.starCount * 32];
            //__instance.focusDetailCounts = new int[GameMain.galaxy.starCount * 1024];
            //__instance.focusDetailSignals = new int[GameMain.galaxy.starCount * 32];
            var l = GameMain.galaxy.starCount * 400;
            __instance.astroArr = new AstroPoseR[l];
            __instance.astroBuffer = new ComputeBuffer(l, 32, ComputeBufferType.Default);
            //GS2.Warn($"Pool Length: {__instance.tmpEntityPools.Length}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ThemeProto), "Preload")]
        public static bool Preload(ref ThemeProto __instance)
        {
            __instance.displayName = __instance.DisplayName.Translate();
            __instance.terrainMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "terrain", "{0}-{1}", true);
            __instance.oceanMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "ocean", "{0}-{1}", true);
            __instance.atmosMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "atmosphere", "{0}-{1}", true);
            __instance.lowMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "low", "{0}-{1}", true);
            __instance.thumbMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "thumb", "{0}-{1}", true);
            __instance.minimapMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "minimap", "{0}-{1}", true);
            __instance.ambientDesc = Utils.ResourcesLoadArray<AmbientDesc>(__instance.MaterialPath + "ambient", "{0}-{1}", true);
            __instance.ambientSfx = Utils.ResourcesLoadArray<AudioClip>(__instance.SFXPath, "{0}-{1}", true);
            if (__instance.RareSettings.Length != __instance.RareVeins.Length * 4) Debug.LogError("稀有矿物数组长度有误 " + __instance.displayName);
            return false;
        }
       
        [HarmonyPatch(typeof(UIReplicatorWindow), "OnPlusButtonClick")]
        [HarmonyPrefix]
        public static bool OnPlusButtonClick(ref UIReplicatorWindow __instance, int whatever)
        {
            // GS2.Log("Test");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.multipliers.ContainsKey(__instance.selectedRecipe.ID)) __instance.multipliers[__instance.selectedRecipe.ID] = 1;

                var num = __instance.multipliers[__instance.selectedRecipe.ID];
                if (VFInput.control) num += 10;
                else if (VFInput.shift) num += 100;
                else if (VFInput.alt) num = 999;
                else num++;
                if (num > 999) num = 999;

                __instance.multipliers[__instance.selectedRecipe.ID] = num;
                __instance.multiValueText.text = num + "x";
            }

            return false;
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnMinusButtonClick")]
        [HarmonyPrefix]
        public static bool OnMinusButtonClick(ref UIReplicatorWindow __instance, int whatever)
        {
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.multipliers.ContainsKey(__instance.selectedRecipe.ID)) __instance.multipliers[__instance.selectedRecipe.ID] = 1;
                var num = __instance.multipliers[__instance.selectedRecipe.ID];
                if (VFInput.control) num -= 10;
                else if (VFInput.shift) num -= 100;
                else if (VFInput.alt) num = 1;
                else num--;
                if (num < 1) num = 1;
                __instance.multipliers[__instance.selectedRecipe.ID] = num;
                __instance.multiValueText.text = num + "x";
            }


            return false;
        }

        [HarmonyPatch(typeof(UIReplicatorWindow), "OnOkButtonClick")]
        [HarmonyPrefix]
        public static bool OnOkButtonClick(ref UIReplicatorWindow __instance, int whatever, bool button_enable)
        {
            // GS2.Log("Test2");
            if (__instance.selectedRecipe != null)
            {
                if (!__instance.selectedRecipe.Handcraft)
                {
                    UIRealtimeTip.Popup("该配方".Translate() + __instance.selectedRecipe.madeFromString + "生产".Translate());
                    return false;
                }

                var id = __instance.selectedRecipe.ID;
                if (!GameMain.history.RecipeUnlocked(id))
                {
                    UIRealtimeTip.Popup("配方未解锁".Translate());
                    return false;
                }

                var num = 1;
                if (__instance.multipliers.ContainsKey(id)) num = __instance.multipliers[id];

                if (num < 1)
                    num = 1;
                else if (num > 999) num = 1000;

                var num2 = __instance.mechaForge.PredictTaskCount(__instance.selectedRecipe.ID, 999);
                // GS2.Log($"{num} - {num2}");
                if (num > num2) num = num2;

                if (num == 0)
                {
                    UIRealtimeTip.Popup("材料不足".Translate());
                    return false;
                }

                if (__instance.mechaForge.AddTask(id, num) == null)
                {
                    UIRealtimeTip.Popup("材料不足".Translate());
                    return false;
                }

                GameMain.history.RegFeatureKey(1000104);
            }

            return false;
        }

        private delegate void ShowSolarsystemDetails(UIVirtualStarmap starmap, int starIndex);

        private delegate bool IsBirthStar(UIVirtualStarmap starmap, int starIndex);

        private delegate bool IsBirthStar2(StarData starData, UIVirtualStarmap starmap);

        private delegate void TrackPlayerClick(UIVirtualStarmap starmap, int starIndex);
    }
}