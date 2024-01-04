using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix,HarmonyPatch(typeof(GameData), nameof(GameData.NewGame))]
        public static void NewGame(GameDesc _gameDesc, ref PlanetFactory[] ___factories)
        {
            if (DSPGame.IsMenuDemo) return;

            ___factories = new PlanetFactory[GSSettings.PlanetCount *2];
        }
    }
}