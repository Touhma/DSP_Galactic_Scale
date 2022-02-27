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
        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(GameLoader), "CreateLoader")]
        // public static void CreateLoader()
        // {
        //     GS2.Warn("Added Gameloader component");
        //     GS2.Error(":)");
        // }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnUpdate")]
        public static void _OnUpdate_Postfix()
        {
            // as we need to load and generate planets for the detail view in the lobby, update the loading process here
            PlanetModelingManager.ModelingPlanetCoroutine();
            UIRoot.instance.uiGame.planetDetail._OnUpdate();
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
            GS2.Warn("Disabled Import");
            GS2.SaveOrLoadWindowOpen = true;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "LoadSelectedGame")]
        public static bool UILoadGameWindow_LoadSelectedGame()
        {
            GS2.Warn("Enabled Import");
            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "_OnClose")]
        public static bool UILoadGameWindow_OnClose()
        {
            GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnOpen")]
        public static bool UISaveGameWindow_OnOpen()
        {
            GS2.Warn("Disabled Import");

            GS2.SaveOrLoadWindowOpen = true;
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISaveGameWindow), "_OnClose")]
        public static bool UISaveGameWindow_OnClose()
        {
            GS2.Warn("Enabled Import");

            GS2.SaveOrLoadWindowOpen = false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAchievementPanel), "LoadData")]
        public static bool LoadData(UIAchievementPanel __instance)
        {
            __instance.uiEntries.Clear();
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarningSystem), "Init")]
        public static void Init(ref WarningSystem __instance)
        {
            GS2.Warn("Warning System Initializing");
            GS2.Warn($"Star Count: {GSSettings.StarCount}");
            var planetCount = GSSettings.PlanetCount;
            GS2.Warn($"Planet Count: {planetCount}");
            GS2.Warn($"Factory Length: {__instance.gameData.factories.Length}");
            if (__instance.gameData.factories.Length > planetCount) planetCount = __instance.gameData.factories.Length;
            __instance.tmpEntityPools = new EntityData[planetCount][];
            __instance.tmpPrebuildPools = new PrebuildData[planetCount][];
            __instance.tmpSignPools = new SignData[planetCount][];
            __instance.warningCounts = new int[GameMain.galaxy.starCount * 1024];
            __instance.warningSignals = new int[GameMain.galaxy.starCount * 32];
            __instance.focusDetailCounts = new int[GameMain.galaxy.starCount * 1024];
            __instance.focusDetailSignals = new int[GameMain.galaxy.starCount * 32];
            var l = GameMain.galaxy.starCount * 400;
            __instance.astroArr = new AstroPoseR[l];
            __instance.astroBuffer = new ComputeBuffer(l, 32, ComputeBufferType.Default);
            GS2.Warn($"Pool Length: {__instance.tmpEntityPools.Length}");
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