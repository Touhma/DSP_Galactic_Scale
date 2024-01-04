using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins))]
        [HarmonyPatch(typeof(PlanetAlgorithm0), nameof(PlanetAlgorithm0.GenerateVeins))]
        public static bool PlanetAlgorithm0_GenerateVeins(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}