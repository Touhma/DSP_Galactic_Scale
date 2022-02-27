using HarmonyLib;
using NebulaAPI;
using NebulaCompatibility;
using NGPT;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIVirtualStarmap
    {
        // GSSettings.Instance.birthStar =
        //     __instance.starPool[GSSettings.BirthPlanet.planetData.star.index].starData;
        // GS2.ActiveGenerator.Generate(GSSettings.StarCount,__instance.starPool[GSSettings.BirthPlanet.planetData.star.index].starData );
        // __instance.galaxyData = GS2.ProcessGalaxy(GS2.gameDesc, true);
        // __instance.OnGalaxyDataReset();

        public static bool deBounce = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVirtualStarmap), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UIVirtualStarmap __instance)
        {
           SystemDisplay.OnUpdate(__instance);
           return false;
        }
    }
}