using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm14), nameof(PlanetAlgorithm14.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm13), nameof(PlanetAlgorithm13.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm12), nameof(PlanetAlgorithm12.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm11), nameof(PlanetAlgorithm11.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm10), nameof(PlanetAlgorithm10.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm9), nameof(PlanetAlgorithm9.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm8), nameof(PlanetAlgorithm8.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm6), nameof(PlanetAlgorithm6.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm5), nameof(PlanetAlgorithm5.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm4), nameof(PlanetAlgorithm4.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm3), nameof(PlanetAlgorithm3.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm2), nameof(PlanetAlgorithm2.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm1), nameof(PlanetAlgorithm1.GenerateTerrain))]
        [HarmonyPatch(typeof(PlanetAlgorithm0), nameof(PlanetAlgorithm0.GenerateTerrain))]
        public static bool PlanetAlgorithm0_GenerateTerrain(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}