using System.IO;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void PatchMainMenu(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts)
        {
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;

            var contentGS = GameObject.Find("Option Window/details/content-gs");
            GS2.LoadExternalThemes(Path.Combine(GS2.DataDir, "CustomThemes"));

            if (contentGS == null)
            {
                __instance.applyButton.button.onClick.AddListener(GS2.SavePreferences);
                __instance.cancelButton.button.onClick.AddListener(() => { GS2.LoadPreferences(); });
                SettingsUI.CreateGalacticScaleSettingsPage(___tabButtons, ___tabTexts);
            }

            UIRoot.instance.optionWindow.SetTabIndex(SettingsUI.MainTabIndex, false);
            SettingsUI.GalacticScaleTabClick();
            if (!GS2.canvasOverlay)
            {
                overlayCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                GS2.canvasOverlay = true;
            }
        }
    }
}