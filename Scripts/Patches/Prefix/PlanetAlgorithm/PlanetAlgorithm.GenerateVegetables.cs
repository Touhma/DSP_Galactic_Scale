using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm14), nameof(PlanetAlgorithm14.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm10), nameof(PlanetAlgorithm10.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm9), nameof(PlanetAlgorithm9.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm8), nameof(PlanetAlgorithm8.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm6), nameof(PlanetAlgorithm6.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm5), nameof(PlanetAlgorithm5.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm4), nameof(PlanetAlgorithm4.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm3), nameof(PlanetAlgorithm3.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm2), nameof(PlanetAlgorithm2.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm1), nameof(PlanetAlgorithm1.GenerateVegetables))]
        [HarmonyPatch(typeof(PlanetAlgorithm0), nameof(PlanetAlgorithm0.GenerateVegetables))]
        public static bool PlanetAlgorithm0_GenerateVegetables(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}