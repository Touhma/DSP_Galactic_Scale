using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.OnActivePlanetLoaded))]
        public static void OnActivePlanetLoaded(PlanetData planet)
        {
            var segments = (int)(planet.radius / 4f + 0.1f) * 4;
            if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) GS3.SetLuts(segments, planet.radius);
            PatchOnUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
        }
    }
}