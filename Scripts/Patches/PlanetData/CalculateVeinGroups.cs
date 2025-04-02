using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), nameof(PlanetData.SummarizeVeinGroups))]
        public static bool CalculateVeinGroups(PlanetData __instance)
        {
            if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
            return false;
        }
    }
}