using HarmonyLib;

namespace GalacticScale
{
    [HarmonyPatch(typeof(PlanetAlgorithm))]
    public class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateVeins")]
        public static bool GenerateVeins(bool sketchOnly, ref PlanetData ___planet )
        {
            if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
            GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
            if (gsPlanet == null)
            {
                return true;
            }
            GSPlanetAlgorithm.GenerateVeins(gsPlanet, sketchOnly);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm1),"GenerateTerrain")]
        public static bool GenerateTerrain(ref PlanetData ___planet)
        {
            if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
            GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
            if (gsPlanet == null)
            {
                return true;
            }
            if (GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings.terrainAlgorithm == "GS2")
            {
                GSPlanetAlgorithm.GenerateTerrain(gsPlanet);
                return false;
            }
            else return true;
        }
    }
}