using HarmonyLib;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "runtimeVeinGroups", MethodType.Getter)]
        public static bool runtimeVeinGroupsGetter()
        {
            GS2.Log($"Getter accessed by {GS2.GetCaller(1)}");
            return true;
        }       
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetFactory), "InitVeinGroups", typeof(PlanetData))]
        public static bool initVeinGroups()
        {
            GS2.Log($"Init accessed by {GS2.GetCaller(1)}");
            return true;
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalculateVeinGroups")]
        public static bool CalculateVeinGroups(PlanetData __instance)
        {
            
            GS2.Warn($"*CalculateVeinGroups {__instance.name} accessed by {GS2.GetCaller(1)}");
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
            GS2.Warn($"*CalcVeinCounts calculated:{__instance.calculated} accessed by {GS2.GetCaller(1)}");
            if (__instance.runtimeVeinGroups == null) PlanetModelingManager.Algorithm(__instance).GenerateVeins();
            foreach (var x in __instance.runtimeVeinGroups)
            {
                if ((int)x.type > 7)
                {
                    GS2.Warn($"RuntimeGroups: {x.type}");
                }
            }
            // GS2.LogJson(__instance.veinGroups);
            return true;
        }       
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetData), "CalcVeinAmounts")]
        public static bool CalcVeinAmounts(PlanetData __instance)
        {
            GS2.Warn($"*CalcVeinAmounts calculated:{__instance.calculated} accessed by {GS2.GetCaller(1)}");
            foreach (var x in __instance.runtimeVeinGroups)
            {
                if ((int)x.type > 7)
                {
                    GS2.Warn($"RuntimeGroups: {x.type}");
                }
            }
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "RequestCalcStar")]
        public static bool RequestCalcStar(StarData star)
        {
            GS2.Warn($"*RequestCalcStar Stardata calculated:{star.calculated}");
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "RequestCalcPlanet")]
        public static bool RequestCalcPlanet(PlanetData planet)
        {
            GS2.Warn($"*RequestCalcPlanet  planet calculated:{planet.calculated}");
            return true;
        }
    }
}