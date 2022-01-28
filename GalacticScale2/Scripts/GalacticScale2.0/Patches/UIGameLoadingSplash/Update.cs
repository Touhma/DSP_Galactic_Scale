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
            status += $"{HandleLocalStarPlanets.GetStarLoadingStatus(GameMain.localStar)}";
            if (HandleLocalStarPlanets.GetLoadedPlanetCount(GameMain.localStar) >= GameMain.localStar.planets.Length || GS2.Vanilla) return;
            var loadingPlanet = GameMain.localStar.planets[HandleLocalStarPlanets.GetLoadedPlanetCount(GameMain.localStar)];
            status += $"  {loadingPlanet.name} a {loadingPlanet.realRadius} Radius {GS2.GetGSPlanet(loadingPlanet).GsTheme.Name} ";
            if (loadingPlanet.orbitAroundPlanet != null) status += "Moon " ;
            else status += "Planet ";
            //status += $"with a {loadingPlanet.orbitRadius}AU Orbit";
            ___promptText.text = status;
        }
    }
}