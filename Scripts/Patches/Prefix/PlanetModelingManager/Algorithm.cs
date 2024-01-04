using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), nameof(PlanetModelingManager.Algorithm))]
        public static bool Algorithm(PlanetData planet, ref PlanetAlgorithm __result)
        {
            // GS3.Log($"Starting. InGalaxySelect:{SystemDisplay.inGalaxySelect}. Called by {GS3.GetCaller(1)}");
            if (DSPGame.IsMenuDemo && !SystemDisplay.inGalaxySelect)
            {
                // GS3.Log("Menu");
                return true;
            }
            // GS3.Log("CHOOSING ALGORITHM FOR " + planet.displayName + " rawdata?"+(planet.data != null));
            var gsPlanet = GS3.GetGSPlanet(planet);
            if (gsPlanet == null)
            {
                __result = new NullAlgorithm();
                // GS3.Warn($"Tried creating planetAlgorithm for planet that cannot be found. {planet.name}");
                return false;
            }
            // GS3.Log(gsPlanet?.Theme);
            var gsTheme = GSSettings.ThemeLibrary.Find(gsPlanet?.Theme);
            // GS3.Log("Use Custom Generation? " + gsTheme.CustomGeneration);
            if (!gsTheme.CustomGeneration)
            {
                // GS3.Log("CHOSE COMPLETELY VANILLA");
                if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings())
                {
                    // GS3.Log("Returning control to base algorithm");
                    return true;
                }
            }
            // GS3.Log("USING CUSTOM GENERATION FOR PLANET " + planet.displayName);
            __result = new GS3PlanetAlgorithm(gsPlanet); //new GS3PlanetAlgorithm(gsPlanet);
            __result.Reset(5, planet);
            // GS3.Log("PatchOnPlanetModellingManager|Algorithm|" + __result.planet.name+"|End|"+__result.seed);
            return false;
        }
    }
}