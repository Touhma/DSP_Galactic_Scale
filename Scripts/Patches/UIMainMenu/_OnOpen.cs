using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class PatchOnUIMainMenu
    {
        private static RectTransform mainMenuButtons;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenu), "_OnOpen")]
        public static void _OnOpen(ref UIMainMenu __instance)
        {
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null)
                // Log.Warn("'Overlay Canvas' not found!");
                return;
            if (overlayCanvas.transform.Find("Main Menu") == null)
                // Log.Warn("'Overlay Canvas/Main Menu' not found!");
                return;

            if (mainMenuButtons != null) return;
            GS2.Log("Attempting to get exit button");
            mainMenuButtons = GameObject.Find("Main Menu/button-group").GetComponent<RectTransform>();
            var buttonRectTransform = Object.Instantiate(GameObject.Find("Main Menu/button-group/button-exit").GetComponent<RectTransform>(), mainMenuButtons);
            Object.Destroy(buttonRectTransform.GetComponentInChildren<Localizer>());
            buttonRectTransform.GetComponentInChildren<Text>().text = "GS2 Help";
            var anchoredPosition = buttonRectTransform.anchoredPosition;
            buttonRectTransform.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y - buttonRectTransform.sizeDelta.y - 10);
            var button = buttonRectTransform.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { Application.OpenURL("http://customizing.space/"); });
        }
    }
}