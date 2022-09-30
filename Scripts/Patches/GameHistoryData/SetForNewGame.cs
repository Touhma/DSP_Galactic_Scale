using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameHistoryData
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
            var recipes = Configs.freeMode.recipes;
            if (recipes != null)
                for (var i = 0; i < recipes.Length; i++)
                    __instance.recipeUnlocked.Add(recipes[i]);
            __instance.techStates.Clear();
            var dataArray = LDB.techs.dataArray;
            for (var j = 0; j < dataArray.Length; j++)
            {
                var upoint = 0;
                if (dataArray[j].Items.Length != 0 && dataArray[j].Items[0] == 6006) upoint = dataArray[j].ItemPoints[0];
                var value = new TechState(false, dataArray[j].Level, dataArray[j].MaxLevel, 0L, dataArray[j].GetHashNeeded(dataArray[j].Level), upoint);
                // GS2.Warn($"{dataArray[j].Name.Translate()} {dataArray[j].ID} {dataArray[j].Desc.Translate()}");
                if (!__instance.techStates.ContainsKey(dataArray[j].ID)) __instance.techStates.Add(dataArray[j].ID, value);
                else
                    GS2.Warn($"Duplicate Tech ID {dataArray[j].Name.Translate()} {dataArray[j].ID} Ignored");
            }

            var techs = Configs.freeMode.techs;
            if (techs != null)
                for (var k = 0; k < techs.Length; k++)
                    __instance.UnlockTech(techs[k]);
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
            __instance.logisticCourierSpeed = Configs.freeMode.logisticCourierSpeed;//Added after 0.9.27 Update
            __instance.logisticCourierSpeedScale = 1f;//Added after 0.9.27 Update
            __instance.logisticCourierCarries = Configs.freeMode.logisticCourierCarries;//Added after 0.9.27 Update
            __instance.dispenserDeliveryMaxAngle = Configs.freeMode.dispenserDeliveryMaxAngle;//Added after 0.9.27 Update
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
            __instance.propertyData.Clear(); //Added after 0.9.25 update
            __instance.createWithSandboxMode = __instance.gameData.gameDesc.isSandboxMode; //Added after 0.9.27 Update
            return false;
        }
    }
}