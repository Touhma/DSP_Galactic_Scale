using HarmonyLib;
using UnityEngine;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using PatchSizeReworkPlanetGen = GalacticScale.Scripts.PatchPlanetSize.ReworkPlanetGen;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(GameData))]
    public class PatchOnGameData {
        [HarmonyPostfix]
        [HarmonyPatch("OnActivePlanetLoaded")]
        public static void OnActivePlanetLoaded(PlanetData planet) {
                if (PatchSize.EnableLimitedResizingFeature.Value)
                {
                    PatchSize.Debug("PlanetLoaded was called!", BepInEx.Logging.LogLevel.Debug, PatchSize.DebugNewPlanetGrid);
                int segments = (int) (planet.radius / 4f + 0.1f) * 4;
                if (!PatchUIBuildingGrid.LUT512.ContainsKey(segments)) {
                    PatchSizeReworkPlanetGen.SetLuts(segments, planet.radius);
                }
                PatchUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetOrCreateFactory")]
        public static bool GetOrCreateFactory(PlanetData planet, ref GameData __instance, ref PlanetFactory __result, ref int ___factoryCount, ref PlanetFactory[] ___factories, ref GameStatData ___statistics)
        {
            if (planet.factory != null)
            {
                GS2.Log("Planetfactory not null");
                __result = planet.factory;
                return false;
            }
            GS2.Log("Planetfactory  null");
            PlanetFactory planetFactory = new PlanetFactory();
            planetFactory.Init(__instance, planet, ___factoryCount);
            GS2.Log("Planetfactory 1 - " + ___factories.Length + " " + ___factoryCount);
            ___factories[___factoryCount] = planetFactory;
            GS2.Log("Planetfactory 2");
            planet.factory = planetFactory;
            planet.factoryIndex = ___factoryCount;
            GS2.Log("Planetfactory 3");
            ___statistics.production.CreateFactoryStat(___factoryCount);
            ++___factoryCount;
            __result = planetFactory;
            GS2.Log("Planetfactory 4");
            return false;
        }
    }
}