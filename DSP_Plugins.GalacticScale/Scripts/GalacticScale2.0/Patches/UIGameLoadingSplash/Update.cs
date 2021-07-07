using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGameLoadingSplash
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIGameLoadingSplash), "Update")]
        public static void Update(ref Text ___promptText)
{
            string status = "WARNING - Galactic Scale savegames can be broken by updates.Read the FAQ @ http://customizing.space\r\n";
            if (GameMain.localStar != null && !GameMain.localStar.loaded) status += $"Loading Planets: {HandleLocalStarPlanets.GetStarLoadingStatus(GameMain.localStar)}";
            ___promptText.text= status;
        }
    }
}