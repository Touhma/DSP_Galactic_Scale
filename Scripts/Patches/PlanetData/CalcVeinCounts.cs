using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinCounts")]
        public static bool CalcVeinCounts(PlanetData __instance)
        {
            if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
            return true;
        }
    }
}