using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnStarGen
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarGen), "CreateBirthStar")]
        // This is never actually called by GS2, but it is called by vanilla (main menu)
        public static bool CreateBirthStar(GalaxyData galaxy, int seed)
        {
            if (GS2.Vanilla)
            {
                var gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
                galaxy.astrosData = new AstroData[gSize];
                return true;
            }

            return true;
        }
    }
}