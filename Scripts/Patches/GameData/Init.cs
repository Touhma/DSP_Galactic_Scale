using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ProductionStatistics), "Init")]
        public static void Init(GameData _gameData, ref FactoryProductionStat[] ___factoryStatPool)
        {
            if (GS2.Vanilla || DSPGame.IsMenuDemo) return;

            ___factoryStatPool = new FactoryProductionStat[GSSettings.PlanetCount];
        }
    }
}