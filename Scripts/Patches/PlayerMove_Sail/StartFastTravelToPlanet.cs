using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    // ReSharper disable once InconsistentNaming
    public partial class PatchOnPlayerMove_Sail
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlayerMove_Sail), "StartFastTravelToPlanet")]
        public static bool StartFastTravelToPlanet(ref PlayerMove_Sail __instance, PlanetData destPlanet)
        {
            GS2.fastTravelTargetPlanet = destPlanet;
            destPlanet.Load();
            GameMain.data.localStar = destPlanet.star;
            return true;
        }
    }
}