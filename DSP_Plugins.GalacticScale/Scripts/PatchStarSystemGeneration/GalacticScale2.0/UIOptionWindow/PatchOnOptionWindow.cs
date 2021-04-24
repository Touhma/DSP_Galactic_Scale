using HarmonyLib;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NGPT;
using System.Reflection;

namespace GalacticScale
{
    public class PatchOnOptionWindow
    {
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        private static UIButton[] tabButtons;
        private static Text[] tabTexts;
        private static Tweener[] tabTweeners;
        //private static FastInvokeHandler SetTabIndexInfo;
        private static RectTransform details;

        [HarmonyPostfix, HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void PatchMainMenu(ref UIButton[] ___tabButtons, ref Text[] ___tabTexts, ref Tweener[] ___tabTweeners, ref Image ___tabSlider)
        {
            GameObject overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null)
            {
                GS2.Log("'Overlay Canvas' not found!");
                return;
            }

            if (overlayCanvas.transform.Find("Top Windows") == null)
            {
                GS2.Log("'Overlay Canvas/Top Windows' not found!");
                return;
            }

            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            
            tabButtons = ___tabButtons;
            tabTexts = ___tabTexts;
            tabTweeners = ___tabTweeners;
            //SetTabIndexInfo = MethodInvoker.GetHandler(AccessTools.Method(typeof(UIOptionWindow), "SetTabIndex", new System.Type[] { typeof(int), typeof(bool) }));
            AddGSTab();
        }

        private static void AddGSTab()
        {
            RectTransform tabButtonTemplate = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>();
            galacticButton = Object.Instantiate(tabButtonTemplate, tabLine, false);
            galacticButton.name = "tab-button-gs";
            galacticButton.anchoredPosition = new Vector2(galacticButton.anchoredPosition.x + 160, galacticButton.anchoredPosition.y);
            Object.Destroy(galacticButton.GetComponentInChildren<Localizer>());
            galacticButton.GetComponent<Button>().onClick.RemoveAllListeners();
            galacticButton.GetComponentInChildren<Text>().text = "Galactic Scale";
            galacticButton.GetComponent<Button>().onClick.AddListener(new UnityAction(OptionsButtonClick));
            tabButtons.AddItem<UIButton>(galacticButton.GetComponent<UIButton>());
            tabTexts.AddItem<Text>(galacticButton.GetComponentInChildren<Text>());
            tabTweeners.AddItem<Tweener>(galacticButton.GetComponent<Tweener>());
            RectTransform detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            details.gameObject.SetActive(false);
        }
        private static void OptionsButtonClick()
        {
            UIRoot.instance.optionWindow.SetTabIndex(5, false);
            details.gameObject.SetActive(true);
        }
        public static void DisableDetails()
        {
            if (details != null && details.gameObject != null) details.gameObject.SetActive(false);
        }
    }
}