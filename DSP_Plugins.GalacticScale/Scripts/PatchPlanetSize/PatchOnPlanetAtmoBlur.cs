using GalacticScale.Scripts.PatchPlanetSize.Rework;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetAtmoBlur))]
    public class PatchOnPlanetAtmoBlur {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool CreatePlanet(ref PlanetAtmoBlur __instance) {
            ReworkPlanetAtmoBlur.ReworkUpdate(ref __instance);
            return false;
        }
        
    }
}