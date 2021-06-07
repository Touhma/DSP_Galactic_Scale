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
                //GSPlanet p = GS2.GetGSPlanet(planet);
                //if (p != null)
                //{
                    if (planet.scale < 1 && planet.scale > 0)
                    {
                        return false;
                    }
                //}
            }
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "OnPlanetClick")]
        public static bool OnPlanetClick(ref UIStarmapPlanet ___focusPlanet, UIStarmapPlanet planet)
        {
                int id = planet.planet.id;
                //GSPlanet p = GS2.GetGSPlanet(id);
                //if (p != null)
                //{
                    if (planet.planet.scale < 1 && planet.planet.scale > 0)
                    {
                        return false;
                    }
                //}
           return true;
        }


   //     [HarmonyPrefix]
   //     [HarmonyPatch(typeof(UIStarmapStar), "_OnLateUpdate")]
   //     public static bool UIStarmapStar_OnLateUpdate(UIStarmapStar __instance)
   //     {
   //         __instance.starObject._LateUpdate();
   //         Vector2 zero = Vector2.zero;
   //         bool flag = !__instance.starmap.isChangingToMilkyWay && __instance.starmap.WorldPointIntoScreen(__instance.starObject.vpos, out zero);
   //         RaycastHit raycastHit;
   //         Camera cam = (Camera)AccessTools.Field(typeof(UIStarmapStar), "cam").GetValue(__instance);
   //         GameHistoryData history = (GameHistoryData)AccessTools.Field(typeof(UIStarmapStar), "gameHistory").GetValue(__instance);
   //         /*
			//if (flag && Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(__instance.starObject.vpos)), out raycastHit, 200f, 1048576, QueryTriggerInteraction.Ignore) && raycastHit.collider.gameObject != __instance.starObject.gameObject)
			//{
			//	flag = false;
			//}
			//*/
   //         __instance.projected = flag;
   //         __instance.projectedCoord = new Vector2(Mathf.Round(zero.x), Mathf.Round(zero.y));
   //         if (flag)
   //         {
   //             if (history.GetStarPin(__instance.star.id) == EPin.Auto)
   //             {
   //                 flag = (__instance.starmap.mouseHoverStar == __instance || ((__instance.star == __instance.starmap.viewStar || __instance.star == __instance.starmap.viewStarSystem) && (double)cam.transform.localPosition.magnitude < (double)__instance.star.systemRadius * 40000.0 * 12.0 * 0.00025));
   //             }
   //             else
   //             {
   //                 flag = (history.GetStarPin(__instance.star.id) == EPin.Show || __instance.starmap.mouseHoverStar == __instance);
   //             }
   //         }
   //         if (flag)
   //         {
   //             float num = Mathf.Max(1f, __instance.starObject.vdist / __instance.starObject.vscale.x);
   //             zero.x += 8f + 600f / num;
   //             zero.y += 4f;
   //             __instance.nameText.rectTransform.anchoredPosition = zero;
   //         }
   //         __instance.nameText.gameObject.SetActive(flag && __instance.starmap.focusPlanet == null && __instance.starmap.focusStar == null);

   //         return false;
   //     }



    }



}
