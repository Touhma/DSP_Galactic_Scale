using HarmonyLib;

namespace GalacticScale
{
    [HarmonyPatch(typeof(PlanetAlgorithm))]
    public class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch("GenerateVeins")]
        public static bool GenerateVeins(PlanetAlgorithm __instance, bool sketchOnly, ref PlanetData ___planet )
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
    }
}