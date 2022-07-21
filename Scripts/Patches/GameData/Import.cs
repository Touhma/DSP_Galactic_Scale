// using System.IO;
// using ABN;
// using HarmonyLib;
//
// namespace GalacticScale
// {
// 	public partial class PatchOnGameData
// 	{
// 		[HarmonyPrefix]
// 		[HarmonyPatch(typeof(GameData), "Import")]
// 		public static bool Import(GameData __instance, BinaryReader r)
// 		{
// 			GS2.Log("Importing GameData...");
// 			int num = r.ReadInt32();
// 			GS2.Log($"Version: {num}");
// 			__instance.patch = 0;
// 			if (num >= 4)
// 			{
// 				__instance.patch = r.ReadInt32();
// 			}
// 			GS2.Log($"Patch: {__instance.patch}");
// 			__instance.account = AccountData.NULL;
// 			if (num >= 3)
// 			{
// 				__instance.account.Import(r);
// 				GS2.Log("Imported Account Data");
// 			}
//
// 			if (__instance.account.isNull)
// 			{
// 				__instance.account = AccountData.me;
// 				GS2.Log("Set Missing Account Data to Default");
//
// 			}
//
// 			__instance.gameName = r.ReadString();
// 			GS2.Log($"Imported Game Name : {__instance.gameName}");
//
// 			__instance.gameDesc = new GameDesc();
// 			__instance.gameDesc.Import(r);
// 			GS2.Log($"Imported Game Desc : {__instance.gameDesc.clusterString}");
// 			DSPGame.GameDesc = __instance.gameDesc;
// 			DSPGame.LoadFile = "";
// 			GameMain.gameTick = r.ReadInt64();
// 			GS2.Log($"GameTick: {GameMain.gameTick}");
// 			__instance.gameAchievement = new GameAchievementData();
// 			__instance.gameAchievement.Init();
// 			if (num >= 7)
// 			{
// 				__instance.gameAchievement.Import(r);
// 				GS2.Log($"Imported Game Achievement Data");
// 			}
//
// 			__instance.galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
// 			GS2.Log("Created Galaxy");
// 			__instance.galaxy.UpdatePoses(GameMain.gameTime);
// 			GS2.Log("Updated Poses");
// 			__instance.factories = new PlanetFactory[__instance.gameDesc.starCount * 6];
// 			__instance.factoryCount = 0;
// 			PerformanceMonitor.EndData(ESaveDataEntry.Header);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Preference);
// 			GS2.Log("Performance Monitor Initialized");
// 			__instance.preferences = new GamePrefsData();
// 			__instance.preferences.Init(__instance);
// 			if (num >= 1)
// 			{
// 				__instance.preferences.Import(r);
// 				GS2.Log($"Imported Preferences");
// 			}
// 			else
// 			{
// 				__instance.preferences.SetForNewGame();
// 				GS2.Log("Initialized New Preferences");
// 			}
//
// 			__instance.preferences.EarlyRestore();
// 			__instance.galaxy.RegeneratePlanetNames();
// 			GS2.Log("Regenerated Planet Names");
// 			PerformanceMonitor.EndData(ESaveDataEntry.Preference);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.History);
// 			__instance.history = new GameHistoryData();
// 			__instance.history.Init(__instance);
// 			__instance.history.Import(r);
// 			GS2.Log($"Imported Game History Data ");
// 			PerformanceMonitor.EndData(ESaveDataEntry.History);
// 			if (num >= 2)
// 			{
// 				__instance.hidePlayerModel = r.ReadBoolean();
// 				__instance.disableController = r.ReadBoolean();
// 			}
// 			else
// 			{
// 				__instance.hidePlayerModel = false;
// 				__instance.disableController = false;
// 			}
//
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Statistics);
// 			__instance.statistics = new GameStatData();
// 			__instance.statistics.Init(__instance);
// 			__instance.statistics.Import(r);
// 			GS2.Log($"Imported Game Stats");
// 			PerformanceMonitor.EndData(ESaveDataEntry.Statistics);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Player);
// 			int planetId = r.ReadInt32();
// 			GS2.Log($"Planet ID : {planetId}");
// 			PlanetData planetData = __instance.galaxy.PlanetById(planetId);
// 			GS2.Log($"Planet:{planetData?.name}");
// 			__instance.mainPlayer = Player.Create(__instance, __instance.gameDesc.playerProto);
// 			__instance.mainPlayer.Import(r);
// 			GS2.Log($"Imported Main Player");
// 			PerformanceMonitor.EndData(ESaveDataEntry.Player);
// 			__instance.factoryCount = r.ReadInt32();
// 			__instance.galacticTransport = new GalacticTransport();
// 			__instance.galacticTransport.Init(__instance);
// 			__instance.galacticTransport.Import(r);
// 			GS2.Log($"Imported Galactic Transport System");
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Trash);
// 			__instance.trashSystem = new TrashSystem(__instance);
// 			if (num >= 5)
// 			{
// 				__instance.trashSystem.Import(r);
// 				GS2.Log($"Imported Trash System");
// 			}
// 			else
// 			{
// 				__instance.trashSystem.SetForNewGame();
// 				GS2.Log($"Created new Trash System");
// 			}
//
// 			PerformanceMonitor.EndData(ESaveDataEntry.Trash);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Factory);
// 			for (int i = 0; i < __instance.factoryCount; i++)
// 			{
// 				__instance.factories[i] = new PlanetFactory();
// 				__instance.factories[i].Import(i, __instance, r);
// 				GS2.Log($"Imported Factory: {i}");
// 				__instance.factories[i].planet.NotifyCalculated();
// 			}
// 			GS2.Log($"Imported Factories");
// 			PerformanceMonitor.EndData(ESaveDataEntry.Factory);
// 			__instance.galacticTransport.Arragement();
// 			__instance.galacticTransport.RefreshTraffic(0);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.DysonSphere);
// 			int num2 = r.ReadInt32();
// 			Assert.True(num2 == __instance.galaxy.starCount);
// 			if (num2 < __instance.galaxy.starCount)
// 			{
// 				num2 = __instance.galaxy.starCount;
// 			}
//
// 			__instance.dysonSpheres = new DysonSphere[num2];
// 			for (int j = 0; j < num2; j++)
// 			{
// 				int num3 = r.ReadInt32();
// 				if (num3 == 1)
// 				{
// 					__instance.dysonSpheres[j] = new DysonSphere();
// 					__instance.dysonSpheres[j].Init(__instance, __instance.galaxy.stars[j % __instance.galaxy.starCount]);
// 					__instance.dysonSpheres[j].Import(r);
// 					GS2.Log($"Imported Sphere:{j}");
// 				}
//
// 				Assert.True(num3 == 1 || num3 == 0);
// 			}
// 			GS2.Log($"Imported Spheres");
// 			PerformanceMonitor.EndData(ESaveDataEntry.DysonSphere);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.History);
// 			if (num >= 6)
// 			{
// 				int num4 = r.ReadInt32();
// 				if (num4 >= 2)
// 				{
// 					r.ReadInt32();
// 				}
//
// 				r.ReadInt32();
// 				if (num4 >= 1)
// 				{
// 					int num5 = r.ReadInt32();
// 					for (int k = 0; k < num5; k++)
// 					{
// 						r.ReadInt64();
// 					}
// 				}
// 			}
//
// 			__instance.abnormalData = new GameAbnormalityData_0925();
// 			__instance.abnormalData.Init(__instance);
// 			if (num >= 8)
// 			{
// 				__instance.abnormalData.Import(r);
// 				GS2.Log($"Imported Game Abnormality Data");
// 			}
//
// 			__instance.milestoneSystem = new MilestoneSystem();
// 			__instance.milestoneSystem.Init(__instance);
// 			if (num >= 7)
// 			{
// 				__instance.milestoneSystem.Import(r);
// 				GS2.Log($"Imported Milestone System");
// 			}
// 			else
// 			{
// 				__instance.milestoneSystem.SetForNewGame();
// 			}
//
// 			PerformanceMonitor.EndData(ESaveDataEntry.History);
// 			PerformanceMonitor.BeginData(ESaveDataEntry.Digital);
// 			__instance.warningSystem = new WarningSystem();
// 			__instance.warningSystem.Init(__instance);
// 			if (num >= 9)
// 			{
// 				__instance.warningSystem.Import(r);
// 				GS2.Log($"Imported WarningSystem");
// 			}
// 			else
// 			{
// 				__instance.warningSystem.SetForNewGame();
// 			}
//
// 			PerformanceMonitor.EndData(ESaveDataEntry.Digital);
// 			if (planetData != null)
// 			{
// 				__instance.ArrivePlanet(planetData);
// 			}
// 			else
// 			{
// 				__instance.DetermineLocalPlanet();
// 			}
//
// 			__instance.DetermineRelative();
// 			__instance.mainPlayer.SetAfterGameDataReady();
// 			__instance.history.SetAfterGameDataReady();
// 			GS2.Log($"Finished Importing GameData");
// 			return false;
// 		}
// 	}
// }