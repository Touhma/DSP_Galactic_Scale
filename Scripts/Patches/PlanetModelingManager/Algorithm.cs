using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "Algorithm")]
        public static bool Algorithm(PlanetData planet, ref PlanetAlgorithm __result)
        {
            GS2.Log($"Start {SystemDisplay.inGalaxySelect}");
            if (DSPGame.IsMenuDemo && !SystemDisplay.inGalaxySelect)
            {
                GS2.Log("Menu");
                return true;
            }

            if (GS2.Vanilla)
            {
                GS2.Log("Vanilla");
                return true;
            }
            GS2.Log("CHOOSING ALGORITHM FOR " + planet.displayName + " rawdata?"+(planet.data != null));
            var gsPlanet = GS2.GetGSPlanet(planet);
            GS2.Log(gsPlanet?.Theme);
            var gsTheme = GSSettings.ThemeLibrary.Find(gsPlanet?.Theme);
            GS2.Log("Use Custom Generation? " + gsTheme.CustomGeneration);
            if (!gsTheme.CustomGeneration)
            {
                GS2.Log("CHOSE COMPLETELY VANILLA");
                if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings())
                {
                    GS2.Log("Returning control to base algorithm");
                    return true;
                }
            }

            GS2.Log("USING CUSTOM GENERATION FOR PLANET " + planet.displayName);
            __result = new GS2PlanetAlgorithm(gsPlanet); //new GS2PlanetAlgorithm(gsPlanet);
            __result.Reset(5, planet);
            GS2.Log("PatchOnPlanetModellingManager|Algorithm|" + __result.planet.name+"|End|"+__result.seed);
            return false;
        }
    }
}