using HarmonyLib;

namespace GalacticScale.Patches
{
    //Note: This is probably only used in vanilla. Doesn't seem to run during GS
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.GetNearestStarPlanet))]
        public static bool GetNearestStarPlanet(ref GameData __instance, ref StarData nearestStar,
            ref PlanetData nearestPlanet)
        {
            if (__instance.mainPlayer == null)
            {
                nearestStar = null;
                nearestPlanet = null;
            }

            // double num = 3239999.9141693115;
            var num2 = 900.0;
            if (nearestStar != null)
            {
                if (__instance.mainPlayer != null)
                {
                    var magnitude = (__instance.mainPlayer.uPosition - nearestStar.uPosition).magnitude;
                    // GS3.Log(
                        // $"Magnitude:{magnitude}, NearestStar TransitionRadius:{(nearestStar.systemRadius + 2) * 40000}");
                    // num = 3600000.0;
                    double num = (nearestStar.systemRadius + 2) * 40000;
                    if (magnitude > num * 0.5) nearestStar = null;
                }
            }

            if (nearestPlanet != null)
            {
                if (__instance.mainPlayer != null)
                {
                    var num3 = (__instance.mainPlayer.uPosition - nearestPlanet.uPosition).magnitude -
                               nearestPlanet.realRadius;
                    num2 = 1000.0;
                    if (num3 > num2 * 0.4000000059604645) nearestPlanet = null;
                }
            }

            if (nearestStar == null)
                // double num4 = num;
                for (var i = 0; i < __instance.galaxy.starCount; i++)
                {
                    if (__instance.mainPlayer != null)
                    {
                        var magnitude2 = (__instance.mainPlayer.uPosition - __instance.galaxy.stars[i].uPosition).magnitude;
                        // if (magnitude2 < num4)
                        if (magnitude2 < (__instance.galaxy.stars[i].systemRadius + 2) * 40000f)
                            nearestStar = __instance.galaxy.stars[i];
                    }

                    // num4 = magnitude2;
                }

            if (__instance.mainPlayer != null && __instance.mainPlayer.warping)
            {
                nearestPlanet = null;
                return false;
            }

            if (nearestPlanet == null)
            {
                var num5 = num2;
                var num6 = 0;
                while (nearestStar != null && num6 < nearestStar.planetCount)
                {
                    if (__instance.mainPlayer != null)
                    {
                        var num7 = (__instance.mainPlayer.uPosition - nearestStar.planets[num6].uPosition).magnitude -
                                   nearestStar.planets[num6].realRadius;
                        if (num7 < num5)
                        {
                            nearestPlanet = nearestStar.planets[num6];
                            num5 = num7;
                        }
                    }

                    num6++;
                }
            }


            return false;
        }
    }
}