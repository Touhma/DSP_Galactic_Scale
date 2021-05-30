using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmapPlanet
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIStarmapPlanet), "_OnLateUpdate")]
        public static void OnLateUpdate_Postfix (ref UIStarmap ___starmap, ref GameObject ___relatedObject, ref UIStarmapPlanet __instance, PlanetData ___planet, ref LineRenderer ___orbitRenderer)
        {
            //if (___planet.scale < 1)
            //{
            //    if (___starmap.mouseHoverPlanet == __instance)
            //    {
            //        //___starmap.mouseHoverPlanet = null;
            //        //___starmap.mouseOnGizmo = false;
            //        //___starmap.cursorViewDisplayObject = null;
            //        //___starmap.focusPlanet = null;
            //        //___starmap.cursorFunctionIcon1.gameObject.SetActive(false);
            //    }
            //}
            
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(ref UIStarmapPlanet ___focusPlanet, ref UIStarmapPlanet ___mouseHoverPlanet)
        {

            if (___mouseHoverPlanet != null) // && ___mouseHoverPlanet.id > 0)
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
            //if (___focusPlanet != null &&___focusPlanet.scale < 1) { ___focusPlanet = null; }
            //if (___mouseHoverPlanet != null && ___mouseHoverPlanet.id > 0 && GS2.GetGSPlanet(___mouseHoverPlanet).scale < 1) ___mouseHoverPlanet = null; 
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
       
            //if (___focusPlanet != null &&___focusPlanet.scale < 1) { ___focusPlanet = null; }
            //if (___mouseHoverPlanet != null && ___mouseHoverPlanet.id > 0 && GS2.GetGSPlanet(___mouseHoverPlanet).scale < 1) ___mouseHoverPlanet = null; 
            return true;
        }

    }
}