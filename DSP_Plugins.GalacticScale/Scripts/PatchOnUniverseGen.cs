using HarmonyLib;

namespace DSP_Plugin.GalacticScale {
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GalaxyData __instance, GameDesc gameDesc) {
            return true;
        }
    }
}