using HarmonyLib;

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
        }
    }
}