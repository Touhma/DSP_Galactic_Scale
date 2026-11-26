using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmapPlanet
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnInit")]
        public static void UIStarmapPlanet_OnInit_Postfix(UIStarmapPlanet __instance, bool __result)
        {
            if (!__result || __instance.planet == null) return;
            if (__instance.planet.scale < 1 && __instance.planetRenderer != null)
                __instance.planetRenderer.enabled = false;
            CometSystem.AttachStarmapTail(__instance);
            PlanetaryRingSystem.SyncStarmapPlanet(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnOpen")]
        public static void UIStarmapPlanet_OnOpen_Postfix(UIStarmapPlanet __instance)
        {
            PlanetaryRingSystem.SyncStarmapPlanet(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapPlanet), "_OnLateUpdate")]
        public static void UIStarmapPlanet_OnLateUpdate_Postfix(UIStarmapPlanet __instance)
        {
            PlanetaryRingSystem.SyncStarmapPlanet(__instance);
            HideOverlappingMoonLabel(__instance);
        }

        private static void HideOverlappingMoonLabel(UIStarmapPlanet __instance)
        {
            if (__instance == null || __instance.planet == null || __instance.starmap == null) return;
            var host = __instance.planet.orbitAroundPlanet;
            if (host == null) return;
            if (__instance.nameText == null || !__instance.nameText.gameObject.activeSelf) return;

            var starmap = __instance.starmap;
            if (starmap.mouseHoverPlanet == __instance || starmap.focusPlanet == __instance) return;
            var planetUIs = starmap.planetUIs;
            if (planetUIs == null) return;

            var myPos = __instance.nameText.rectTransform.anchoredPosition;
            for (var i = 0; i < planetUIs.Length; i++)
            {
                var other = planetUIs[i];
                if (other == null || other == __instance || other.planet == null || other.nameText == null) continue;
                if (!other.nameText.gameObject.activeSelf) continue;

                // Hide moon label if it overlaps its host planet's label or an earlier visible sibling moon's label
                var isHost = other.planet == host;
                var isEarlierSibling = other.planet.orbitAroundPlanet == host && other.planet.id < __instance.planet.id;
                if (!isHost && !isEarlierSibling) continue;

                var otherPos = other.nameText.rectTransform.anchoredPosition;
                if (Mathf.Abs(myPos.x - otherPos.x) < 54f && Mathf.Abs(myPos.y - otherPos.y) < 22f)
                {
                    __instance.nameText.gameObject.SetActive(false);
                    return;
                }
            }
        }
    }
}