using HarmonyLib;

namespace GalacticScale
{
    class PatchOnGameData
    {
		[HarmonyPatch(typeof(GameData), "NewGame"), HarmonyPostfix]
		public static void NewGame(GameDesc _gameDesc, ref PlanetFactory[] ___factories)
		{
			if (GS2.Vanilla || DSPGame.IsMenuDemo) return;
			___factories = new PlanetFactory[GSSettings.PlanetCount];
		}
		[HarmonyPatch(typeof(ProductionStatistics), "Init"), HarmonyPostfix]
		public static void Init(GameData _gameData, ref FactoryProductionStat[] ___factoryStatPool)
		{
			if (GS2.Vanilla || DSPGame.IsMenuDemo) return;
			GS2.Log("PRODUCTION " + _gameData.factories.Length);
			___factoryStatPool = new FactoryProductionStat[GSSettings.PlanetCount];
		}
	}
}