using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static class PatchOnUIStarmapPlanet
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIStarmapPlanet), "_OnInit")]
        public static void UIStarmapPlanet_OnInit_Postfix (ref UIStarmapPlanet __instance, PlanetData ___planet, ref LineRenderer ___orbitRenderer)
        {
            GSPlanet gsPlanet = GS2.GetGSPlanet(___planet);
            if (___planet.scale < 1) ___orbitRenderer.enabled = false;
        }

    }
}