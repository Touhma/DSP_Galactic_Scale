﻿using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUIStarmapPlanet {
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static bool UpdateCursorView(ref UIStarmapPlanet ___focusPlanet, ref UIStarmapPlanet ___mouseHoverPlanet) {
            if (___mouseHoverPlanet != null) {
                PlanetData planet = ___mouseHoverPlanet.planet;
                int id = planet.id;
                if (planet.scale < 1 && planet.scale > 0) {
                    return false;
                }
            }
            return true;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIStarmap), "OnPlanetClick")]
        public static bool OnPlanetClick(ref UIStarmapPlanet ___focusPlanet, UIStarmapPlanet planet) {
            int id = planet.planet.id;
            if (planet.planet.scale < 1 && planet.planet.scale > 0) {
                return false;
            }
            return true;
        }
    }
}
