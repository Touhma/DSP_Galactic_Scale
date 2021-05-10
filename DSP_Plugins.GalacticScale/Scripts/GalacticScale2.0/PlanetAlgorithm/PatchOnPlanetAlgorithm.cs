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
            GS2.Log("Gen");
            if (GS2.Vanilla || DSPGame.IsMenuDemo) return true;
            GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
            if (gsPlanet == null)
            {
                GS2.Log("Gen2");
                return true;
            }
            GS2.Log("GenerateVeins " + gsPlanet.Name);
            //lock (___planet){
                GSPlanetAlgorithm.GenerateVeins(gsPlanet, sketchOnly);
            //}
            return false;
        }
    }
}