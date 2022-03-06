using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetAlgorithm
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm9), "GenerateTerrain")]
        public static bool PlanetAlgorithm9_GenerateTerrain(PlanetAlgorithm9 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm8), "GenerateTerrain")]
        public static bool PlanetAlgorithm8_GenerateTerrain(PlanetAlgorithm8 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm7), "GenerateTerrain")]
        public static bool PlanetAlgorithm7_GenerateTerrain(PlanetAlgorithm7 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm6), "GenerateTerrain")]
        public static bool PlanetAlgorithm6_GenerateTerrain(PlanetAlgorithm6 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm5), "GenerateTerrain")]
        public static bool PlanetAlgorithm5_GenerateTerrain(PlanetAlgorithm5 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm4), "GenerateTerrain")]
        public static bool PlanetAlgorithm4_GenerateTerrain(PlanetAlgorithm4 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm3), "GenerateTerrain")]
        public static bool PlanetAlgorithm3_GenerateTerrain(PlanetAlgorithm3 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm2), "GenerateTerrain")]
        public static bool PlanetAlgorithm2_GenerateTerrain(PlanetAlgorithm2 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm1), "GenerateTerrain")]
        public static bool PlanetAlgorithm1_GenerateTerrain(PlanetAlgorithm1 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm0), "GenerateTerrain")]
        public static bool PlanetAlgorithm0_GenerateTerrain(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}