using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "OnActivePlanetLoaded")]
        public static void OnActivePlanetLoaded(PlanetData planet)
        {
            //GS2.Warn($"{planet.name}");
            if (!GS2.Vanilla)
            {
                var segments = (int)(planet.radius / 4f + 0.1f) * 4;
                if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) GS2.SetLuts(segments, planet.radius);
                PatchOnUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            }
        }
    }
}