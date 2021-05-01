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
    public static class SettingsUI
    {
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        private static UIButton[] tabButtons;
        private static Text[] tabTexts;

        private static RectTransform details;
        private static RectTransform templateOptionsCanvas;
        private static RectTransform templateUIComboBox;
        private static RectTransform templateCheckBox;
        private static RectTransform templateInputField;
        private static RectTransform templateSlider;
        private static RectTransform templateButton;

        private static List<RectTransform> optionRects = new List<RectTransform>();
        private static List<RectTransform> generatorCanvases = new List<RectTransform>();
        private static List<List<GSOption>> generatorPluginOptions = new List<List<GSOption>>();
        private static float anchorX;
        private static float anchorY;

        private static List<GSOption> options = new List<GSOption>();

        public static UnityEvent OptionsUIPostfix = new UnityEvent();
        public static void CreateGalacticScaleSettingsPage(UIButton[] _tabButtons, Text[] _tabTexts)
        {
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            tabButtons = _tabButtons;
            tabTexts = _tabTexts;

            //Add Tab Button
            Transform tabParent = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>().parent;
            RectTransform tabButtonTemplate = tabParent.GetChild(tabParent.childCount - 1).GetComponent<RectTransform>();
            galacticButton = Object.Instantiate(tabButtonTemplate, tabLine, false);
            galacticButton.name = "tab-button-gs";
            galacticButton.anchoredPosition = new Vector2(galacticButton.anchoredPosition.x + 160, galacticButton.anchoredPosition.y);
            Object.Destroy(galacticButton.GetComponentInChildren<Localizer>());
            galacticButton.GetComponent<Button>().onClick.RemoveAllListeners();
            galacticButton.GetComponentInChildren<Text>().text = "Galactic Scale";
            galacticButton.GetComponent<Button>().onClick.AddListener(new UnityAction(GalacticScaleTabClick));
            tabButtons.AddItem<UIButton>(galacticButton.GetComponent<UIButton>());
            tabTexts.AddItem<Text>(galacticButton.GetComponentInChildren<Text>());

            //Create the galactic scale settings panel
            RectTransform detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            details.gameObject.SetActive(false);
            details.gameObject.name = "content-gs";


            //Destroy surplus ui elements
            Transform tipLevel = details.Find("tiplevel");
            if (tipLevel != null) Object.Destroy(tipLevel.gameObject);

            
            //Copy original combobox as a template, then get rid of it
            RectTransform generatorPicker = details.Find("language").GetComponent<RectTransform>();
            anchorX = generatorPicker.anchoredPosition.x;
            anchorY = generatorPicker.anchoredPosition.y;
            //templateUIComboBox = Object.Instantiate<RectTransform>(details.Find("language").GetComponent<RectTransform>());
            //templateUIComboBox.gameObject.SetActive(false);
            GS2.Log("TEST3");
            templateUIComboBox = CreateTemplate(details.Find("language").GetComponent<RectTransform>());
            GS2.Log("TEST4");
            Object.Destroy(generatorPicker.gameObject);
            GS2.Log("TEST5");

            //Create a template of a button
            Transform revertButton = details.Find("revert-button");
            templateButton = CreateTemplate(templateUIComboBox);
            templateButton.GetComponentInChildren<UIComboBox>().gameObject.SetActive(false);
            //Object.Destroy(templateButton.GetComponentInChildren<UIComboBox>().gameObject);
            
            RectTransform templateButtonButton = Object.Instantiate(revertButton.GetComponent<RectTransform>(), templateButton, false);
            templateButtonButton.anchoredPosition = new Vector2(250, 0);
            templateButtonButton.sizeDelta = new Vector2(200, 30);
            templateButton.anchorMin = templateButton.anchorMax = new Vector2(0, 1);
            templateButton.anchoredPosition = new Vector2(anchorX, anchorY);
            if (revertButton != null) Object.Destroy(revertButton.gameObject);

            GS2.Log("TEST");
            templateOptionsCanvas = Object.Instantiate(details, details, false);
            templateOptionsCanvas.anchoredPosition = details.anchoredPosition + new Vector2(750f, 0);
            templateOptionsCanvas.gameObject.name = "templateCanvasPanel";
            GS2.Log("TEST2");
            while (templateOptionsCanvas.transform.childCount > 0)
            {
                Object.DestroyImmediate(templateOptionsCanvas.transform.GetChild(0).gameObject);
            }
            GS2.Log("TEST3");
            RectTransform checkBoxProto = GameObject.Find("Option Window/details/content-1/fullscreen").GetComponent<RectTransform>(); //need to remove localizer, has textcomponent, and child called Checkbox with a UIToggle and a unityengine.ui.toggle
            templateCheckBox = CreateTemplate(checkBoxProto);
            GS2.Log("TEST4"); 
            RectTransform sliderProto = GameObject.Find("Option Window/details/content-1/dofblur").GetComponent<RectTransform>(); // localizer,  a textcomponent, has child called slider which has a UI.Slider component ,
            templateSlider = CreateTemplate(sliderProto);
            GS2.Log("TEST5");
            RectTransform inputFieldProto = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed").GetComponentInChildren<InputField>().GetComponent<RectTransform>(); //localizer, has a ui.text comp, a child called inputfield which has a ui.inputfield, a uibutton and a eventsystems.eventtrigger
            GS2.Log("TEST6");
            RectTransform tempTransform = CreateTemplate(templateUIComboBox);
            GS2.Log("TEST7");
            UIComboBox tr = tempTransform.GetComponentInChildren<UIComboBox>();
            GS2.Log("TEST7.5"); 
            RectTransform tr2 = tr.GetComponent<RectTransform>();
            GS2.Log("TEST8");
            tempTransform.name = "TempTransform";
            inputFieldProto.name = "inputFieldProto";
            inputFieldProto.SetParent(tempTransform);
            inputFieldProto.anchorMin = tr2.anchorMin;
            inputFieldProto.anchorMax = tr2.anchorMax;
            inputFieldProto.offsetMin = tr2.offsetMin;
            inputFieldProto.offsetMax = tr2.offsetMax;
            inputFieldProto.sizeDelta = tr2.sizeDelta;
            inputFieldProto.anchoredPosition = new Vector2(250, -15);
            Object.DestroyImmediate(tempTransform.GetComponentInChildren<UIComboBox>().gameObject);
            templateInputField = CreateTemplate(tempTransform);
            templateInputField.name = "templateInputField*";

            
            GS2.Log("4");


            //Get a list of all loaded generators, and add a combobox to select between them.
            //List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            //options.Add(new GS2.GSOption("GS2", "Generator", "UIComboBox", generatorNames, new GS2.GSOptionCallback(GeneratorSelected)));
            CreateOwnOptions();
            ImportCustomGeneratorOptions();
            GS2.Log("Test9");
            CreateOptionsUI();

            GS2.Log("Test10");
            OptionsUIPostfix.Invoke();
            GS2.Log("Test11");
        }
        private static void ImportCustomGeneratorOptions()
        {
            for (var i = 0; i < GS2.generators.Count; i++)
            {
                List<GSOption> pluginOptions = new List<GSOption>();
                GS2.Log("IMPORT CUSTOM GENERATOR OPTIONS : " + GS2.generators[i].Name);
                if (GS2.generators[i] is iConfigurableGenerator gen) { GS2.Log(gen.Name + " is configurable"); foreach (GSOption o in gen.Options) pluginOptions.Add(o); }
                generatorPluginOptions.Add(pluginOptions);
            }
        }
        private static void CreateOwnOptions()
        {
            GS2.Log("CreateOwnOptions()");
            List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            options.Add(new GSOption("Generator", "UIComboBox", generatorNames, GeneratorSelected, CreateOwnOptionsPostFix));
        }
        private static void CreateOwnOptionsPostFix()
        {
            GS2.Log("CreateGeneratorOptionsPostFix");
            var generatorIndex = 0;
            List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            for (var i = 0; i < generatorNames.Count; i++) if (generatorNames[i] == GS2.generator.Name) { GS2.Log("index found!" + i); generatorIndex = i; }
            if (optionRects[0] != null)
            {
                GS2.Log("Setting combobox for generator index to " + generatorIndex);
                optionRects[0].GetComponentInChildren<UIComboBox>().itemIndex = generatorIndex;
            }
            else GS2.Log("optionRects[0] == null!@#");
        }
        private static RectTransform CreateTemplate(RectTransform original)
        {
            RectTransform template = Object.Instantiate(original, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            template.gameObject.SetActive(false);
            Localizer l = template.GetComponentInChildren<Localizer>();
            if (l != null) Object.Destroy(l);
            return template;
        }

        // Method that handles creation of the settings tab
        private static void CreateOptionsUI()
        {
            GS2.Log("CreateOptionsUI");
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].type)
                {
                    case "UIComboBox": CreateComboBox(options[i], details, i); break;
                    case "Input": CreateInputField(options[i], details, i); break;
                    case "Button": CreateButton(options[i], details, i); break;
                    default: break;
                }
            }
            int currentGenIndex = GS2.GetCurrentGeneratorIndex();
            GS2.Log("CreateGeneratorOptionsCanvases: currentGenIndex = " + currentGenIndex + " - " + GS2.generators[currentGenIndex].Name);
            for (var i = 0; i < generatorPluginOptions.Count; i++)
            { //for each canvas
                GS2.Log("Creating Canvas " + i);
                RectTransform canvas = Object.Instantiate(templateOptionsCanvas, details, false);
                canvas.name = "testCanvas" + i;
                generatorCanvases.Add(canvas);
                canvas.name = "generatorCanvas-" + GS2.generators[i].Name;
                if (currentGenIndex == i)
                {
                    GS2.Log("Setting canvas active");
                    canvas.gameObject.SetActive(true);
                }
                else canvas.gameObject.SetActive(false);
                AddGeneratorPluginUIElements(canvas, i);

            }
        }



        /// Iterate through all the plugins that have elements to add to the UI, add them,// then add their postfixes to the event listener
        private static void AddGeneratorPluginUIElements(RectTransform canvas, int genIndex)
        {
            GS2.Log("AddGeneratorPluginUIElements: " + GS2.generators[genIndex].Name);
            List<GSOption> options = generatorPluginOptions[genIndex];
            GS2.Log(GS2.generators[genIndex].Name + " option count = " + options.Count);
            for (int i = 0; i < options.Count; i++)
            {
                switch (options[i].type)
                {
                    case "ComboBox": CreateComboBox(options[i], canvas, i); break;
                    case "Button": CreateButton(options[i], canvas, i); break;
                    case "Input": CreateInputField(options[i], canvas, i); break;
                    default: break;
                }
                //if (options[i].postfix != null) OptionsUIPostfix.AddListener(options[i].postfix);
            }
        }

        // Create a combobox from a GSOption definition
        private static void CreateComboBox(GSOption o, RectTransform canvas, int index)
        {
            GS2.Log("CreateComboBox");
            RectTransform comboBoxRect = Object.Instantiate(templateUIComboBox, canvas);
            comboBoxRect.name = o.label;
            comboBoxRect.gameObject.SetActive(true);
            optionRects.Add(comboBoxRect);
            o.rectTransform = comboBoxRect;
            int offset = index * -40;
            comboBoxRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            UIComboBox comboBoxUI = comboBoxRect.GetComponentInChildren<UIComboBox>();
            comboBoxUI.name = o.label + "_comboBox";
            comboBoxUI.Items = o.data as List<string>;
            comboBoxUI.UpdateItems();
            comboBoxUI.itemIndex = 0;
            comboBoxUI.onItemIndexChange.RemoveAllListeners();
            if (o.callback != null) comboBoxUI.onItemIndexChange.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            comboBoxRect.GetComponentInChildren<Text>().text = o.label;
            RectTransform tipTransform = comboBoxRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + (index);
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.tip;
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            GS2.Log("Finished Creating ComboBox");
        }

        // Create an input field from a GSOption definition
        private static void CreateInputField(GSOption o, RectTransform canvas, int index)
        {
            GS2.Log("CreateInputField");
            RectTransform inputRect = Object.Instantiate(templateInputField, canvas);
            inputRect.name = o.label;
            inputRect.gameObject.SetActive(true);
            optionRects.Add(inputRect);
            o.rectTransform = inputRect;
            int offset = index * -40;
            inputRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            InputField inputUI = inputRect.GetComponentInChildren<InputField>();
            inputUI.name = o.label + "_inputField";
            inputUI.text = o.data as string;
            inputUI.onValueChanged.RemoveAllListeners();
            if (o.callback != null) inputUI.onValueChanged.AddListener(delegate { o.callback(inputUI.text); });
            inputRect.GetComponentInChildren<Text>().text = o.label;
            Object.Destroy(inputUI.GetComponentInChildren<UnityEngine.EventSystems.EventTrigger>());
            RectTransform tipTransform = inputRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + (index);
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.tip;
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            GS2.Log("Finished Creating InputField");
        }           // Create a button from a GSOption definition
        private static void CreateButton(GSOption o, RectTransform canvas, int index)
        {
            GS2.Log("CreateButton");
            RectTransform buttonRect = Object.Instantiate(templateButton, canvas);
            buttonRect.name = o.label;
            buttonRect.gameObject.SetActive(true);
            optionRects.Add(buttonRect);
            o.rectTransform = buttonRect;
            int offset = index * -40;
            buttonRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            UIButton uiButton = buttonRect.GetComponentInChildren<UIButton>();
            uiButton.name = o.label + "_button";
            uiButton.button.onClick.RemoveAllListeners();
            uiButton.GetComponentInChildren<Text>().text = o.data.ToString();
            var l = uiButton.GetComponentInChildren<Localizer>();
            Object.Destroy(l);
            if (o.callback != null) uiButton.button.onClick.AddListener(delegate { o.callback(null); });
            buttonRect.GetComponentInChildren<Text>().text = o.label;
            RectTransform tipTransform = buttonRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + (index);
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.tip;
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            GS2.Log("Finished Creating Button");
        }

        // Callback for own Generator ComboBox Selection Event
        private static void GeneratorSelected(object result)
        {
            GS2.Log("Result = " + result + GS2.generators[(int)result].Name);
            GS2.generator = GS2.generators[(int)result];
            GS2.Log("Set the generator, trying to disable every canvas");
            for (var i = 0; i < generatorCanvases.Count; i++) generatorCanvases[i].gameObject.SetActive(false);
            GS2.Log("tryuing to setactive");
            generatorCanvases[(int)result].gameObject.SetActive(true);
            GS2.Log("trying to save prefs");
            GS2.SavePreferences();
        }
        private static void GalacticScaleTabClick()
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