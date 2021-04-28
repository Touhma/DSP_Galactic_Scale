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
        //private static RectTransform tabLine;
        //private static RectTransform galacticButton;
        //private static UIButton[] tabButtons;
        //private static Text[] tabTexts;
        //private static Tweener[] tabTweeners;

        //private static RectTransform details;
        //private static RectTransform templateOptionsCanvas;
        //private static RectTransform templateUIComboBox;
        //private static RectTransform templateCheckBox;
        //private static RectTransform templateInputField;
        //private static RectTransform templateSlider;
        //private static RectTransform templateButton;

        //private static List<RectTransform> optionRects = new List<RectTransform>();
        //private static List<RectTransform> generatorCanvases = new List<RectTransform>();
        //private static List<List<RectTransform>> generatorPluginOptionRects = new List<List<RectTransform>>();
        //private static List<List<GS2.GSOption>> generatorPluginOptions = new List<List<GS2.GSOption>>();
        //private static float anchorX;
        //private static float anchorY;

        //private static List<GS2.GSOption> options = new List<GS2.GSOption>();

        //public static UnityEvent OptionsUIPostfix = new UnityEvent();


        [HarmonyPostfix, HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void PatchMainMenu(ref UIButton[] ___tabButtons, ref Text[] ___tabTexts, ref Tweener[] ___tabTweeners, ref Image ___tabSlider)
        {
            GameObject overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;

            ////Grab the tabgroup and store the relevant data in this class
            //tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            ///tabButtons = ___tabButtons;
            //tabTexts = ___tabTexts;
            //tabTweeners = ___tabTweeners;
            ////GS2.Log("TEST");
            ////Get out of the patch, and start running our own code
            var contentGS = GameObject.Find("Option Window/details/content-gs");
            if (contentGS == null)
                SettingsUI.CreateGalacticScaleSettingsPage(___tabButtons, ___tabTexts);
            
    }

        

        
    }
}