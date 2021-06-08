using HarmonyLib;
using UnityEngine;

namespace GalacticScale {
    public partial class PatchOnGameData {
        [HarmonyPostfix, HarmonyPatch(typeof(GameData),"OnActivePlanetLoaded")]
        public static void OnActivePlanetLoaded(PlanetData planet) {
                if (!GS2.Vanilla)
                {
                int segments = (int) (planet.radius / 4f + 0.1f) * 4;
                if (!PatchUIBuildingGrid.LUT512.ContainsKey(segments)) {
                    GS2.SetLuts(segments, planet.radius);
                }
                PatchUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            }
        }
	}
}