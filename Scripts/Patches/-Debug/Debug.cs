using HarmonyLib;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalculateVeinGroups")]
        public static bool CalculateVeinGroups(PlanetData __instance)
        {
            GS2.Warn("*CalculateVeinGroups");
            GSPlanet gsPlanet = GS2.GetGSPlanet(__instance);
            __instance.calculated = false;
            // PlanetModelingManager.RequestCalcPlanet(__instance);
            PlanetModelingManager.Algorithm(__instance).GenerateVeins();
            if (gsPlanet != null && gsPlanet.GsTheme.VeinSettings.Algorithm == "Vanilla") return true;
            return false;
        }      
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinCounts")]
        public static bool CalcVeinCounts(PlanetData __instance)
        {
            GS2.Warn("*CalcVeinCounts");
            if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
            GS2.LogJson(__instance.runtimeVeinGroups);
            GS2.LogJson(__instance.veinGroups);
            return true;
        }       
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinAmounts")]
        public static bool CalcVeinAmounts()
        {
            GS2.Warn("*CalcVeinAmounts");
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "RequestCalcStar")]
        public static bool RequestCalcStar()
        {
            GS2.Warn("*RequestCalcStar");
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "RequestCalcPlanet")]
        public static bool RequestCalcPlanet()
        {
            GS2.Warn("*RequestCalcPlanet");
            return true;
        }
    }
}