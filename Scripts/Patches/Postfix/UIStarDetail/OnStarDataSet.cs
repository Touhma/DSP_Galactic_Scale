using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIStarDetail
    {
        private static int actualLevel = 5;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarDetail), nameof(UIStarDetail.OnStarDataSet))]
        private static void OnStarDataSetPost(StarData ____star, InputField ___nameInput, Text ___typeText, RectTransform ___paramGroup, Text ___massValueText, Text ___spectrValueText, Text ___radiusValueText, Text ___luminoValueText, Text ___temperatureValueText, Text ___ageValueText, Sprite ___unknownResIcon, GameObject ___trslBg, GameObject ___imgBg, UIResAmountEntry ___tipEntry, UIResAmountEntry ___entryPrafab, ref UIStarDetail __instance)
        {
            if (!SystemDisplay.inGalaxySelect) return;
            GameMain.history.universeObserveLevel = actualLevel;
        }
        
    }
}