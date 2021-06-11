using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
  //      [HarmonyPrefix, HarmonyPatch(typeof(GameData), "ArrivePlanet")]
  //      public static bool ArrivePlanet(GameData __instance, PlanetData planet)
		//{
		//	if (planet == __instance.localPlanet)
		//	{
		//		return false;
		//	}
		//	if (__instance.localPlanet != null)
		//	{
		//		__instance.LeavePlanet();
		//	}
		//	if (planet != null)
		//	{
		//		if (__instance.localStar != planet.star)
		//		{
		//			__instance.ArriveStar(planet.star);
		//		}
		//		__instance.localPlanet = planet;
		//		__instance.mainPlayer.planetId = planet.id;
		//		if (__instance.localPlanet.loaded)
		//		{
		//			__instance.OnActivePlanetLoaded(__instance.localPlanet);
		//			return false;
		//		}
				
		//		__instance.localPlanet.onLoaded += __instance.OnActivePlanetLoaded;
				
		//	}
		//	return false;
		//}
	}
}