using HarmonyLib;

namespace GalacticScale {
    public class PatchOnPlanetAtmoBlur {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetAtmoBlur), "Update")]
        public static bool Update(ref PlanetAtmoBlur __instance) {
            ReworkPlanetAtmoBlur.ReworkUpdate(ref __instance);
            return false;
        }
    }
}