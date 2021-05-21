using HarmonyLib;

namespace GalacticScale
{
    //[HarmonyPatch(typeof(PlanetAlgorithm))]
    //public class PatchOnPlanetAlgorithm
    //{
        //[HarmonyPrefix]
        //[HarmonyPatch("GenerateVeins")]
        //public static bool GenerateVeins(bool sketchOnly, ref PlanetData ___planet )
        //{
        //    if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
        //    GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
        //    if (gsPlanet == null)
        //    {
        //        return true;
        //    }
        //    VeinAlgorithms.GenerateVeins(gsPlanet, sketchOnly);
        //    return false;
        //}
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlanetAlgorithm1), "GenerateTerrain")]
        //public static bool GenerateTerrain1(ref PlanetData ___planet)
        //{
        //    if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
        //    GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
        //    if (gsPlanet == null)
        //    {
        //        return true;
        //    }
        //    if (GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings.terrainAlgorithm == "GS2")
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using GS2 algorithm");
        //        VeinAlgorithms.GenerateTerrain1(gsPlanet);
        //        return false;
        //    }
        //    else
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using vanilla algorithm");
        //        return true;
        //    }
        //}
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlanetAlgorithm3), "GenerateTerrain")]
        //public static bool GenerateTerrain3(ref PlanetData ___planet, double modX, double modY)
        //{
        //    if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
        //    GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
        //    if (gsPlanet == null)
        //    {
        //        return true;
        //    }
        //    if (GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings.terrainAlgorithm == "GS2")
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using GS2 algorithm");
        //        VeinAlgorithms.GenerateTerrain3(gsPlanet);
        //        return false;
        //    }
        //    else
        //    {
        //        GS2.Log("Generating Terrain3 for " + gsPlanet.Name + " using vanilla algorithm");
        //        return true;
        //    }
        //}
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlanetAlgorithm6), "GenerateTerrain")]
        //public static bool GenerateTerrain6(ref PlanetData ___planet)
        //{
        //    if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
        //    GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
        //    if (gsPlanet == null)
        //    {
        //        return true;
        //    }
        //    if (GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings.terrainAlgorithm == "GS2")
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using GS2 algorithm");
        //        VeinAlgorithms.GenerateTerrain6(gsPlanet);
        //        return false;
        //    }
        //    else
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using vanilla algorithm");
        //        return true;
        //    }
        //}
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlanetAlgorithm5), "GenerateTerrain")]
        //public static bool GenerateTerrain5(ref PlanetData ___planet, double modX, double modY)
        //{
        //    if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
        //    GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
        //    if (gsPlanet == null)
        //    {
        //        return true;
        //    }
        //    if (GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings.terrainAlgorithm == "GS2")
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using GS2 algorithm");
        //        VeinAlgorithms.GenerateTerrain5(gsPlanet, modX, modY);
        //        return false;
        //    }
        //    else
        //    {
        //        GS2.Log("Generating Terrain for " + gsPlanet.Name + " using vanilla algorithm");
        //        return true;
        //    }
        //}
    //}
}