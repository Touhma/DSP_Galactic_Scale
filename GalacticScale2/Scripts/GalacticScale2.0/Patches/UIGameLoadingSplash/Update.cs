using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGameLoadingSplash
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGameLoadingSplash), "Update")]
        public static void Update(ref Text ___promptText)
        {
            var status = "WARNING - Galactic Scale savegames can be broken by updates.Read the FAQ @ http://customizing.space\r\n".Translate();
            if (GameMain.localStar != null && !GameMain.localStar.loaded)
                status += "Loading Planets: ".Translate();
            // GS2.Warn("A");
            status += $"{HandleLocalStarPlanets.GetStarLoadingStatus(GameMain.localStar)}";
            // GS2.Warn("B");
            if (GameMain.localStar != null && HandleLocalStarPlanets.GetLoadedPlanetCount(GameMain.localStar) >= GameMain.localStar.planets.Length || GS2.Vanilla) return;
            // GS2.Warn("C");
            var loadingPlanet = GameMain.localStar?.planets[HandleLocalStarPlanets.GetLoadedPlanetCount(GameMain.localStar)];
            // GS2.Warn("D");
            status += $"  {loadingPlanet?.name}, a size {loadingPlanet?.realRadius} {GS2.GetGSPlanet(loadingPlanet)?.GsTheme?.DisplayName} ";
            // GS2.Warn("E");
            if (loadingPlanet?.orbitAroundPlanet != null) status += "Moon " ;
            else status += "Planet ";
            //status += $"with a {loadingPlanet.orbitRadius}AU Orbit";
            // GS2.Warn("F");
            if (___promptText != null) ___promptText.text = status;
        }
    }
}