using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmapPlanet
    {
// I think these dont need GSPlanet's, can be done with regular planet. I think the reason I couldn't get it to work before was because I was reading scale from a UIStarmapPlanet.scale not UIStarmapPlanet.planet.scale
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(ref UIStarmapPlanet ___focusPlanet, ref UIStarmapPlanet ___mouseHoverPlanet)
        {

            if (___mouseHoverPlanet != null)
            {
                PlanetData planet = ___mouseHoverPlanet.planet;
                int id = planet.id;
                GSPlanet p = GS2.GetGSPlanet(planet);
                if (p != null)
                {
                    if (p.scale < 1 && p.scale > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "OnPlanetClick")]
        public static bool OnPlanetClick(ref UIStarmapPlanet ___focusPlanet, UIStarmapPlanet planet)
        {
                int id = planet.planet.id;
                GSPlanet p = GS2.GetGSPlanet(id);
                if (p != null)
                {
                    if (p.scale < 1 && p.scale > 0)
                    {
                        return false;
                    }
                }
           return true;
        }

    }
}