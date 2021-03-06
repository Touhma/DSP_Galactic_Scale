using System.Collections.Generic;
using HarmonyLib;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc) {
            PatchSize.PlanetSizeParams = new Dictionary<int, float>();
            return true;
        }
    }
}