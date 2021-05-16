using HarmonyLib;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NGPT;
using System.Reflection;
using System.Collections.Generic;

namespace GalacticScale
{
    public class PatchOnOptionWindow
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void PatchMainMenu(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts, ref Tweener[] ___tabTweeners, ref Image ___tabSlider)
        {
            GameObject overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;         
            var contentGS = GameObject.Find("Option Window/details/content-gs");
            if (contentGS == null)
            {
                __instance.applyButton.button.onClick.AddListener(new UnityAction(GS2.SavePreferences));
                __instance.cancelButton.button.onClick.AddListener(new UnityAction(GS2.LoadPreferences));
                SettingsUI.CreateGalacticScaleSettingsPage(___tabButtons, ___tabTexts);
            }
        }
    }
}