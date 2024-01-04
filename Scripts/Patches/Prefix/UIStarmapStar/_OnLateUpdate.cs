using System;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    /// <summary>
    /// Each frame, for each star within screenscpace, a ray is cast from the point on the screen where a star might appear outward. If this ray hits anything but that star,
    /// projected is set to false. Because Physics.autoSyncTransforms = true and each ray is cast shortly after a transform is changed (in UIStarmapStarObject._LateUpdate()),
    /// this triggers an expensive Physics.SyncTransforms() in between each raycast.
    /// As far as I can tell, projected and projectedCoord are ONLY used to determine if your mouse is being blocked by a different object when hovering in
    /// UIStarmap.UpdateCursorView(), which had already does a raycast test for UIStarmapStar.uIStarmapStar, this can safely be removed.
    /// On a new game with 1024 stars, in the starmap with all stars in view, performace is 3-4x better.
    /// </summary>
    public class PatchOnUIStarmapStar
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmapStar), nameof(UIStarmapStar._OnLateUpdate))]
        public static bool _OnLateUpdate(ref UIStarmapStar __instance)
        {
            __instance.starObject._LateUpdate();
            var rectPoint = Vector2.zero;
            var flag = !UIStarmap.isChangingToMilkyWay && __instance.starmap.WorldPointIntoScreen(__instance.starObject.vpos, out rectPoint);

            //if (flag && Physics.Raycast(__instance.cam.ScreenPointToRay(__instance.cam.WorldToScreenPoint(__instance.starObject.vpos)), out var hitInfo, 200f, 1048576, QueryTriggerInteraction.Ignore) && hitInfo.collider.gameObject != __instance.starObject.gameObject)
            //{
            //    flag = false;
            //}
            //__instance.projected = flag;
            __instance.projected = true;
            __instance.projectedCoord = new Vector2(Mathf.Round(rectPoint.x), Mathf.Round(rectPoint.y));
            if (flag)
            {
                flag = ((__instance.gameHistory.GetStarPin(__instance.star.id) == EPin.Auto)
                    ? (__instance.starmap.mouseHoverStar == __instance ||
                       ((__instance.star == __instance.starmap.viewStar ||
                         __instance.star == __instance.starmap.viewStarSystem) &&
                         __instance.cam.transform.localPosition.magnitude <
                         __instance.star.systemRadius * 40000.0 * 12.0 * 0.00025))
                    : __instance.gameHistory.GetStarPin(__instance.star.id) == EPin.Show || __instance.starmap.mouseHoverStar == __instance);
            }
            if (flag)
            {
                var num = Mathf.Max(1f, __instance.starObject.vdist / __instance.starObject.vscale.x);
                rectPoint.x += 8f + 600f / num;
                rectPoint.y += 4f;
                __instance.nameText.rectTransform.anchoredPosition = rectPoint;
            }
            __instance.nameText.gameObject.SetActive(flag && __instance.starmap.focusPlanet == null && __instance.starmap.focusStar == null);

            return false;
        }
    }
}