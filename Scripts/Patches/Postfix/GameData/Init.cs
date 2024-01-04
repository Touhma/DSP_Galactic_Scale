using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProductionStatistics), nameof(ProductionStatistics.Init))]
        public static void Init(GameData _gameData, ref FactoryProductionStat[] ___factoryStatPool)
        {
            if (DSPGame.IsMenuDemo) return;
            ___factoryStatPool = new FactoryProductionStat[GSSettings.PlanetCount];
        }
    }
}