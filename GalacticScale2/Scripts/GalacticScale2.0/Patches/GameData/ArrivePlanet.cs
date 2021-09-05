using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "ArrivePlanet")]
        public static bool ArrivePlanet(GameData __instance, PlanetData planet)
        {
            GS2.Log($"Arrived at {planet.name} Planet ID:{planet.id} Index:{planet.index}");
            if (planet == __instance.localPlanet)
            {
                GS2.Log($"{planet.name} is __instance.localPlanet");
                return false;
            }

            if (__instance.localPlanet != null)
            {
                GS2.Log("__instance.localPlanet not null, leaving");
                __instance.LeavePlanet();
            }

            if (planet != null)
            {
                GS2.Log($"{planet.name} is not null, but localplanet is");
                if (__instance.localStar != planet.star)
                {
                    GS2.Log("local star is not planet.star, arriveStar called");
                    __instance.ArriveStar(planet.star);
                }

                GS2.Log($"localplanet set to {planet.name}");
                __instance.localPlanet = planet;
                __instance.mainPlayer.planetId = planet.id;
                if (__instance.localPlanet.loaded)
                {
                    GS2.Log($"{planet.name} is loaded, calling OnActivePlanetLoaded");
                    GameMain.mainPlayer.transform.localScale = Vector3.one;
                    __instance.OnActivePlanetLoaded(__instance.localPlanet);
                    return false;
                }

                GS2.Log($"{planet.name} is not loaded, invoking OnActivePlanetLoaded...");
                //__instance.localPlanet.onLoaded += __instance.OnActivePlanetLoaded;
                //__instance.localPlanet.add_onLoaded(__instance.OnActivePlanetLoaded);
                //__instance.localPlanet.onLoaded.add(__instance.OnActivePlanetLoaded);
                AccessTools.Method(typeof(PlanetData), "add_onLoaded").Invoke(__instance.localPlanet, new object[] { (Action<PlanetData>)__instance.OnActivePlanetLoaded });
            }

            return false;
        }
        //[HarmonyPrefix, HarmonyPatch(typeof(UIPowerGizmo), "_OnUpdate")]
        //public static bool test()
        //{
        //    //GS2.Warn((GameMain.localPlanet == null).ToString());
        //    return true;
        //}
    }
}