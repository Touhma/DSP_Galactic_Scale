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
            ___promptText.text = status;
        }
    }
}