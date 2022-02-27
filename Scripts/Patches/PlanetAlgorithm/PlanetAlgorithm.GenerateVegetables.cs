using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm9), "GenerateVegetables")]
        public static bool PlanetAlgorithm9_GenerateVegetables(PlanetAlgorithm9 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm8), "GenerateVegetables")]
        public static bool PlanetAlgorithm8_GenerateVegetables(PlanetAlgorithm8 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm7), "GenerateVegetables")]
        public static bool PlanetAlgorithm7_GenerateVegetables(PlanetAlgorithm7 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm6), "GenerateVegetables")]
        public static bool PlanetAlgorithm6_GenerateVegetables(PlanetAlgorithm6 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm5), "GenerateVegetables")]
        public static bool PlanetAlgorithm5_GenerateVegetables(PlanetAlgorithm5 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm4), "GenerateVegetables")]
        public static bool PlanetAlgorithm4_GenerateVegetables(PlanetAlgorithm4 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm3), "GenerateVegetables")]
        public static bool PlanetAlgorithm3_GenerateVegetables(PlanetAlgorithm3 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm2), "GenerateVegetables")]
        public static bool PlanetAlgorithm2_GenerateVegetables(PlanetAlgorithm2 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm1), "GenerateVegetables")]
        public static bool PlanetAlgorithm1_GenerateVegetables(PlanetAlgorithm1 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm0), "GenerateVegetables")]
        public static bool PlanetAlgorithm0_GenerateVegetables(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}