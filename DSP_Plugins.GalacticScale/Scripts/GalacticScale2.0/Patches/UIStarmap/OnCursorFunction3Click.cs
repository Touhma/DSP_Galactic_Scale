using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{

    public partial class PatchOnUIStarmap
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "OnCursorFunction3Click")]
        public static bool OnCursorFunction3Click(PlanetData ___viewPlanet)
        {
            var go = GameObject.Find("UI Root/Overlay Canvas/In Game/Starmap UIs/starmap-screen-ui/cursor-view/bg");
            if (___viewPlanet != null &&  (VFInput.control))
            {
                Bootstrap.TeleportPlanet = ___viewPlanet;
                Bootstrap.TeleportEnabled = true;
                return false;
            }
            else return true;
            
        }

    }
}