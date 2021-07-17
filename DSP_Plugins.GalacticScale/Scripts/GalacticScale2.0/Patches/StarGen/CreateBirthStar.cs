using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnStarGen
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarGen), "CreateBirthStar")]
        public static bool CreateBirthStar(GalaxyData galaxy, int seed)
        {
            if (GS2.Vanilla)
            {
                var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
                galaxy.astroPoses = new AstroPose[gSize];
                return true;
            }

            return true;
        }
    }
}