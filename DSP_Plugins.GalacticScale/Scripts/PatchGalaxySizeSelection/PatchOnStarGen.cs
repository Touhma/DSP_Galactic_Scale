using HarmonyLib;

namespace GalacticScale.Scripts.PatchGalaxySizeSelection {
    [HarmonyPatch(typeof(StarGen))]
    public class PatchOnStarGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateBirthStar")]
        public static bool CreateBirthStar(GalaxyData galaxy, int seed) {
            int gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
            galaxy.astroPoses = new AstroPose[gSize];
            return true;
        }
    }
}