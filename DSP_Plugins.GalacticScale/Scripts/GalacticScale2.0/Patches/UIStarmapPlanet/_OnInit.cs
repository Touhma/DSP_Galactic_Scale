using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmapPlanet
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIStarmapPlanet), "_OnInit")]
        public static void UIStarmapPlanet_OnInit_Postfix (ref GameObject ___relatedObject, ref UIStarmapPlanet __instance, PlanetData ___planet, ref LineRenderer ___orbitRenderer)
        {
            GS2.Warn("OnInit" + ___planet.scale);
            if (___planet.scale < 1)
            {
                //___orbitRenderer.enabled = false;
                ___relatedObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                
            }
        }

    }
}