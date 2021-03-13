using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(GameData))]
    public class PatchOnGameData
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnActivePlanetLoaded")]
        public static void PostfixPlanetLoaded(PlanetData planet)
        {
            Patch.Debug("PlanetLoaded was called!", BepInEx.Logging.LogLevel.Debug, true);
            int segments = (int)(planet.radius / 4f + 0.1f) * 4;
            if (!PatchUIBuildingGrid.LUT512.ContainsKey(segments))
            {
                PatchStarSystemGeneration.ReworkPlanetGen.SetLuts(segments, planet.radius);
            }
            PatchUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
        }
    }
}
