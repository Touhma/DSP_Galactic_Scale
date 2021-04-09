using GalacticScale.Scripts.PatchPlanetSize.Rework;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetAtmoBlur))]
    public class PatchOnPlanetAtmoBlur {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Update(ref PlanetAtmoBlur __instance) {
            ReworkPlanetAtmoBlur.ReworkUpdate(ref __instance);
            return false;
        }
    }
}