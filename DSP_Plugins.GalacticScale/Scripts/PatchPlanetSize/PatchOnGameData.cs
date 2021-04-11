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
           // if (PatchSize.EnableResizingFeature.Value || PatchSize.EnableLimitedResizingFeature.Value) {
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
    }
}