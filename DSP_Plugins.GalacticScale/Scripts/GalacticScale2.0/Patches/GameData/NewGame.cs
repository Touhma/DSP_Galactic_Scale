using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPatch(typeof(GameData), "NewGame"), HarmonyPostfix]
        public static void NewGame(GameDesc _gameDesc, ref PlanetFactory[] ___factories)
        {
            if (GS2.Vanilla || DSPGame.IsMenuDemo)
            {
                return;
            }

            ___factories = new PlanetFactory[GSSettings.PlanetCount];
        }
    }
}