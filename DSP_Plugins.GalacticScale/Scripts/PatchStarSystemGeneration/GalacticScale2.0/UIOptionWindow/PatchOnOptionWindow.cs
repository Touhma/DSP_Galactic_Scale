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
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        private static UIButton[] tabButtons;
        private static Text[] tabTexts;
        private static Tweener[] tabTweeners;
        //private static List<string> generators = new List<string>();
        //private static FastInvokeHandler SetTabIndexInfo;
        private static RectTransform details;
        private static List<GS2.GSOption> options = new List<GS2.GSOption>();
        private static RectTransform templateUIComboBox;
        private static List<RectTransform> optionRects = new List<RectTransform>();
        private static float anchorX;
        private static float anchorY;


        [HarmonyPostfix, HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
        public static void PatchMainMenu(ref UIButton[] ___tabButtons, ref Text[] ___tabTexts, ref Tweener[] ___tabTweeners, ref Image ___tabSlider)
        {
            GameObject overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;

            //Grab the tabgroup and store the relevant data in this class
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            tabButtons = ___tabButtons;
            tabTexts = ___tabTexts;
            tabTweeners = ___tabTweeners;
            //GS2.Log("TEST");
            //Get out of the patch, and start running our own code
            CreateGalacticScaleSettingsPage();
    }

        private static void CreateGalacticScaleSettingsPage()
        {
            GS2.Log("TEST2");
            //Add Tab Button
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

            //Create the galactic scale settings panel
            RectTransform detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            details.gameObject.SetActive(false);
            details.gameObject.name = "content-gs";

            //Destroy surplus ui elements
            Transform tl = details.Find("tiplevel");
            if (tl != null) Object.Destroy(tl.gameObject);

            //Copy original combobox as a template, then get rid of it
            RectTransform generatorPicker = details.Find("language").GetComponent<RectTransform>();
            anchorX = generatorPicker.anchoredPosition.x;
            anchorY = generatorPicker.anchoredPosition.y;
            templateUIComboBox = Object.Instantiate<RectTransform>(details.Find("language").GetComponent<RectTransform>());
            templateUIComboBox.gameObject.SetActive(false);
            Object.Destroy(generatorPicker.gameObject);

            //Get a list of all loaded generators, and add a combobox to select between them.
            List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            options.Add(new GS2.GSOption("GS2", "Generator", "UIComboBox", generatorNames, new GS2.GSOptionCallback(GeneratorSelected)));
            CreateOptionsUI();
            var generatorIndex = 0;
            for (var i = 0; i < generatorNames.Count; i++) if (generatorNames[i] == GS2.generator.Name) generatorIndex = i;
            if (optionRects[0] != null) optionRects[0].GetComponentInChildren<UIComboBox>().itemIndex = generatorIndex;
        }
        private static void CreateOptionsUI()
        {
            GS2.Log("CreateOptionsUI");
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].type)
                {
                    case "UIComboBox": CreateComboBox(options[i]); break;
                    default: break;
                }
            }
        }
        private static void CreateComboBox(GS2.GSOption o)
        {
            GS2.Log("CreateComboBox");
            RectTransform comboBoxRect = Object.Instantiate(templateUIComboBox, details);
            comboBoxRect.gameObject.SetActive(true);
            optionRects.Add(comboBoxRect);
            int offset = (optionRects.Count-1) * -40;
            comboBoxRect.anchoredPosition = new Vector2(anchorX , anchorY + offset);
            UIComboBox comboBoxUI = comboBoxRect.GetComponentInChildren<UIComboBox>();
            comboBoxUI.name = o.label;
            comboBoxUI.Items = o.data as List<string>;
            comboBoxUI.UpdateItems();
            comboBoxUI.itemIndex = 0;
            comboBoxUI.onItemIndexChange.RemoveAllListeners();
            comboBoxUI.onItemIndexChange.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            comboBoxRect.GetComponentInChildren<Text>().text = o.label;
            comboBoxRect.GetComponentInChildren<Text>().text = o.label;
            RectTransform tipTransform = comboBoxRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + (optionRects.Count -1);
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.tip;
            GS2.Log("Finished Creating ComboBox");
        }
        private static void GeneratorSelected(object result)
        {
            GS2.Log("Result = " + result + GS2.generators[(int)result].Name);
            GS2.generator = GS2.generators[(int)result];
            GS2.SavePreferences();
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