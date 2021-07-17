using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPatch(typeof(GameData), "NewGame")]
        [HarmonyPostfix]
        public static void NewGame(GameDesc _gameDesc, ref PlanetFactory[] ___factories)
        {
            if (GS2.Vanilla || DSPGame.IsMenuDemo) return;

            ___factories = new PlanetFactory[GSSettings.PlanetCount];
        }
        //[HarmonyPatch(typeof(GameData), "SetForNewGame"), HarmonyPostfix]
        //public static void SetForNewGame_Postfix(GameData __instance)
        //{
        //    if (NebulaCompatibility.Initialized && !NebulaCompatibility.IsMasterClient)
        //    {
        //        __instance.mainPlayer.uPosition = GSSettings.BirthPlanet.planetData.uPosition;
        //        __instance.ArriveStar(GSSettings.BirthPlanet.planetData.star);
        //        __instance.ArrivePlanet(GSSettings.BirthPlanet.planetData);
        //    }
        //}
    }
}