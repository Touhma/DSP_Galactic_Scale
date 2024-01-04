using System.IO;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), nameof(UIOptionWindow._OnOpen))]
        public static void PatchMainMenu(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts)
        {
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;

            var contentGS = GameObject.Find("Option Window/details/content-gs");
            GS3.LoadExternalThemes(Path.Combine(GS3.DataDir, "CustomThemes"));

            if (contentGS == null)
            {
                __instance.applyButton.button.onClick.AddListener(GS3.SavePreferences);
                //__instance.cancelButton.button.onClick.AddListener(() => { GS3.LoadPreferences(); });
                SettingsUI.CreateGalacticScaleSettingsPage(__instance, ___tabButtons, ___tabTexts);
            }

            UIRoot.instance.optionWindow.SetTabIndex(SettingsUI.MainTabIndex, false);
            SettingsUI.GalacticScaleTabClick();
            if (!GS3.canvasOverlay)
            {
                overlayCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                GS3.canvasOverlay = true;
            }

            GS3.LoadPreferences();
        }
    }
}