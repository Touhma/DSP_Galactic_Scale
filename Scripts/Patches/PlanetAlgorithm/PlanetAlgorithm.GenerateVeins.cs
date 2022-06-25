using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetAlgorithm
    {


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm13), "GenerateVeins")]
        public static bool PlanetAlgorithm13_GenerateVeins(PlanetAlgorithm13 __instance)
        {
            GS2.Log("Running GenerateVeins on PA13");
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm12), "GenerateVeins")]
        public static bool PlanetAlgorithm12_GenerateVeins(PlanetAlgorithm12 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm11), "GenerateVeins")]
        public static bool PlanetAlgorithm11_GenerateVeins(PlanetAlgorithm11 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm7), "GenerateVeins")]
        public static bool PlanetAlgorithm7_GenerateVeins(PlanetAlgorithm7 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }






        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm), "GenerateVeins")]
        public static bool PlanetAlgorithm_GenerateVeins(PlanetAlgorithm __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAlgorithm0), "GenerateVeins")]
        public static bool PlanetAlgorithm0_GenerateVeins(PlanetAlgorithm0 __instance)
        {
            if (__instance.planet == null || __instance.planet.data == null) return false;
            return true;
        }
    }
}