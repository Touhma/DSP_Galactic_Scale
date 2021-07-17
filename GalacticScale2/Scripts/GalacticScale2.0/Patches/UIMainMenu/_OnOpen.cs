using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class PatchOnUIMainMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenu), "_OnOpen")]
        public static void _OnOpen(ref UIMainMenu __instance)
        {
            var buttonRectTransform = Object.Instantiate(__instance.exitBtnRt);
            buttonRectTransform.GetComponent<Text>().text = "GS2 Help";
            buttonRectTransform.anchoredPosition += Vector2.down * 50;
        }
    }
}