using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnGameData {
        /// <summary>
        /// Unnecessary, was using to debug.
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix, HarmonyPatch(typeof(GameData), "DetermineLocalPlanet")]
        public static bool DetermineLocalPlanet(ref bool __result, ref GameData __instance) {
            if (GS2.Vanilla || GS2.IsMenuDemo) {
                return true;
            }

            if (__instance.mainPlayer == null) {
                GS2.Error("MainPlayer Null");
                return false;
            }
            __result = HandleLocalStarPlanets.Update();
            return false;
            //StarData localStar = __instance.localStar;
            //PlanetData localPlanet = __instance.localPlanet;
            //StarData starData = localStar;
            //PlanetData planetData = localPlanet;
            //if (localStar != null && !localStar.loaded)
            //{
            //             GS2.Warn($"LocalStar != null && !localStar.loaded {localStar.name}");
            //             GS2.Warn($"localStar.planets == null?{(localStar.planets == null)} planetCount:{localStar.planetCount}");
            //             for (int i = 0; i < localStar.planetCount; i++)
            //             {
            //                 GS2.Warn($"i:{i} loaded = {localStar.planets[i].loaded} size:{localStar.planets[i].radius} scale:{localStar.planets[i].scale} ");
            //             }
            //             __result = false;
            //	return false;
            //}
            //if (localPlanet != null && (!localPlanet.loaded || !localPlanet.factoryLoaded || localPlanet.loading))
            //{
            //	GS2.Warn($"localPlanet != null && (!localPlanet.loaded || !localPlanet.factoryLoaded || localPlanet.loading) {localPlanet.name}");
            //	__result = false;
            //	return false;
            //}
            //__instance.GetNearestStarPlanet(ref starData, ref planetData);
            //if (starData != null && __instance.guideRunning && __instance.guideMission.forceLocalPlanet)
            //{
            //	planetData = __instance.guideMission.localPlanet;
            //}
            //bool result = false;
            //if (localStar != null)
            //{
            //	if (localPlanet != null)
            //	{
            //		if (planetData != localPlanet)
            //		{
            //			GS2.Log($"Leaving Planet {localPlanet.name}");
            //			__instance.LeavePlanet();
            //			result = true;
            //		}
            //	}
            //	else if (planetData != null)
            //	{
            //		GS2.Log($"Entering Planet {planetData.name}");
            //		__instance.ArrivePlanet(planetData);
            //		result = true;
            //	}
            //	if (starData != localStar)
            //	{
            //		GS2.Log($"Leaving Star {localStar.name}");
            //		__instance.LeaveStar();
            //		result = true;
            //	}
            //}
            //else if (starData != null)
            //{
            //	GS2.Log($"Entering Star {starData.name}");
            //	__instance.ArriveStar(starData);
            //	result = true;
            //}
            ////GS2.Log("End result:"+result.ToString());
            //__result = result;
            //return false;
        }



    }
}