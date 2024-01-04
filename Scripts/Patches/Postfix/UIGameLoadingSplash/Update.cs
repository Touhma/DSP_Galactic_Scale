﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGameLoadingSplash
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGameLoadingSplash), nameof(UIGameLoadingSplash.Update))]
        public static void Update(ref Text ___promptText)
        {
            var status = "WARNING - Galactic Scale savegames can be broken by updates.Read the FAQ @ http://customizing.space\r\n".Translate();

            if (GameMain.localStar != null && !GameMain.localStar.loaded && GameMain.localStar.planets != null && GameMain.localStar.planets.Length > 0) GetStatusText(ref status, GameMain.localStar);
            if (___promptText != null) ___promptText.text = status;
        }

        private static void GetStatusText(ref string status, StarData star)
        {
            // GS3.Warn("A");
            var SLS = GetStarLoadingStatus(star);
            var LPC = GetLoadedPlanetCount(star);
            status += "Loading Planets: ".Translate();
            status += SLS;
            if (GameMain.localStar != null && LPC >= star.planets?.Length) return;
            var loadingPlanet = GetLoadingPlanet(star);
            var planetSize = GetPlanetSize(loadingPlanet);
            var planetName = GetPlanetName(loadingPlanet);
            var planetTheme = GetPlanetGSThemeName(loadingPlanet);
            status += $"  {planetName}, {"a size".Translate()} {planetSize} {planetTheme} ";
            if (IsMoon(loadingPlanet)) status += "Moon ".Translate();
            else status += "Planet ".Translate();
        }

        private static bool IsMoon(PlanetData planet)
        {
            return planet?.orbitAroundPlanet != null;
        }

        private static int GetLoadedPlanetCount(StarData star)
        {
            return HandleLocalStarPlanets.GetLoadedPlanetCount(star);
        }

        private static string GetStarLoadingStatus(StarData star)
        {
            return HandleLocalStarPlanets.GetStarLoadingStatus(star);
        }

        private static string GetPlanetName(PlanetData planet)
        {
            var name = planet.displayName;
            if (name == string.Empty || name == null) name = "Error getting Planet Name";
            return name;
        }

        private static int GetPlanetSize(PlanetData planet)
        {
            var size = Mathf.RoundToInt(planet.realRadius);
            return size;
        }

        private static string GetPlanetGSThemeName(PlanetData planet)
        {
            var GSPlanetTheme = GetPlanetGSTheme(planet);
            return GSPlanetTheme.DisplayName;
        }

        private static PlanetData GetLoadingPlanet(StarData star)
        {
            var loadingPlanetCount = HandleLocalStarPlanets.GetLoadedPlanetCount(star);
            var planet = star.planets[loadingPlanetCount];
            return planet;
        }

        private static GSTheme GetPlanetGSTheme(PlanetData planet)
        {
            var theme = GS3.GetGSPlanet(planet)?.GsTheme;
            if (theme == null) return new GSTheme { DisplayName = "Error Loading Theme" };
            return theme;
        }
    }
}