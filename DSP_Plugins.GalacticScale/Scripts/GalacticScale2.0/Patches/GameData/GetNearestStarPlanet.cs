using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnGameData {

		[HarmonyPrefix, HarmonyPatch(typeof(GameData), "GetNearestStarPlanet")]
		public static bool GetNearestStarPlanet(ref GameData __instance, ref StarData nearestStar, ref PlanetData nearestPlanet)
		{
			if (GS2.Vanilla) return true;
			if (GS2.IsMenuDemo) return true;
			if (__instance.mainPlayer == null)
			{
				nearestStar = null;
				nearestPlanet = null;
			}
			if (nearestStar != null && GS2.GetGSStar(nearestStar).Decorative)
			{
				//GS2.Warn($"Nullifying Decorative Star: {nearestStar.name}");
				nearestStar = null;
			}
			double num = 3239999.9141693115;
			double num2 = 900.0;
			if (nearestStar != null)
			{
				double magnitude = (__instance.mainPlayer.uPosition - nearestStar.uPosition).magnitude;
				num = 3600000.0;
				//GS2.Warn($"Checking System Radius:{nearestStar.systemRadius} magnitude:{magnitude}");
				num = (nearestStar.systemRadius + 2) * 40000; // Leave star 2AU from last planet.
				if (magnitude > num * 0.5)
				{
					//nearestStar = null;
					GameMain.data.LeaveStar();
				}
			}
			if (nearestPlanet != null)
			{
				double num3 = (__instance.mainPlayer.uPosition - nearestPlanet.uPosition).magnitude - (double)nearestPlanet.realRadius;
				num2 = 500.0; // Default is 1000.
				if (num3 > num2 * 0.4000000059604645)
				{
					nearestPlanet = null;
				}
			}
			if (nearestStar == null)
			{
                for (int i = 0; i < __instance.galaxy.starCount; i++) // Go through every star
				{
					if (__instance.galaxy.stars[i].planetCount == 0) continue;
					if (GS2.GetGSStar(__instance.galaxy.stars[i].id).Decorative) continue; //Ignore Decorative Stars
					double magnitude2 = (__instance.mainPlayer.uPosition - __instance.galaxy.stars[i].uPosition).magnitude; //Get the distance between you and the star
                    double num4 = (__instance.galaxy.stars[i].systemRadius + 2) * 40000;
                    if (magnitude2 < num4)
					{
						nearestStar = __instance.galaxy.stars[i];
						if (!nearestStar.loaded && GameMain.isRunning)
						{
							nearestStar.Load();
						}
					}
                }
			}
			if (__instance.mainPlayer.warping)
			{
				nearestPlanet = null;
				return false;
			}
			if (nearestPlanet == null)
			{
				double num5 = num2;
				int num6 = 0;
				while (nearestStar != null && num6 < nearestStar.planetCount)
				{
					double num7 = (__instance.mainPlayer.uPosition - nearestStar.planets[num6].uPosition).magnitude - (double)nearestStar.planets[num6].realRadius;
					if (num7 < num5)
					{
						nearestPlanet = nearestStar.planets[num6];
						num5 = num7;
					}
					num6++;
				}
			}
			return false;
		}



	}
}