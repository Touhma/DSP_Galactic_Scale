using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GalacticScale
{   public struct GSSliderConfig
    {
        public float minValue;
        public float maxValue;
        public float defaultValue;
        public bool wholeNumbers;
    }
    public static class SettingsUI
    {
        public static int MainTabIndex = 5;
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        private static UIButton[] tabButtons;
        private static Text[] tabTexts;
        public static RectTransform seedInput;
        private static RectTransform content_gs;
        private static RectTransform scrollview;
        private static RectTransform templateOptionsCanvas;
        private static RectTransform templateUIComboBox;
        private static RectTransform templateCheckBox;
        private static RectTransform templateInputField;
        private static RectTransform templateSlider;
        private static RectTransform templateButton;
        private static RectTransform templateScrollView;
        private static List<RectTransform> optionRects = new List<RectTransform>();
        private static List<RectTransform> generatorCanvases = new List<RectTransform>();
        private static List<List<GSUI>> generatorPluginOptions = new List<List<GSUI>>();
        private static float anchorX;
        private static float anchorY;

        private static GSOptions options = new GSOptions();

        public static UnityEvent OptionsUIPostfix = new UnityEvent();

        public static void CreateGalacticScaleSettingsPage(UIButton[] _tabButtons, Text[] _tabTexts)
        {
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            tabButtons = _tabButtons;
            tabTexts = _tabTexts;

            //Add Tab Button
            Transform tabParent = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>().parent;
            MainTabIndex = tabParent.childCount -1;
            RectTransform tabButtonTemplate = tabParent.GetChild(tabParent.childCount - 1).GetComponent<RectTransform>();
            galacticButton = Object.Instantiate(tabButtonTemplate, tabLine, false);
            galacticButton.name = "tab-button-gs";
            galacticButton.anchoredPosition = new Vector2(galacticButton.anchoredPosition.x + 160, galacticButton.anchoredPosition.y);
            Object.Destroy(galacticButton.GetComponentInChildren<Localizer>());
            galacticButton.GetComponent<Button>().onClick.RemoveAllListeners();
            galacticButton.GetComponentInChildren<Text>().text = "Galactic Scale";
            galacticButton.GetComponent<Button>().onClick.AddListener(new UnityAction(GalacticScaleTabClick));
            tabButtons.AddItem(galacticButton.GetComponent<UIButton>());
            tabTexts.AddItem(galacticButton.GetComponentInChildren<Text>());

            GS2.Warn("Creating Content_GS");
            RectTransform content_3 = GameObject.Find("Option Window/details/content-3").GetComponent<RectTransform>();
            content_gs = Object.Instantiate(content_3, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            content_gs.gameObject.SetActive(true);
            content_gs.gameObject.name = "content-gs";
            GS2.Warn("Getting Viewport Content");
            RectTransform viewportContent = content_gs.GetComponentInChildren<UITools.EnsureIntPosition>().GetComponent<RectTransform>();
            GS2.Warn("Destroying viewport content");
            while (viewportContent.transform.childCount > 0)
            {
                Object.DestroyImmediate(viewportContent.transform.GetChild(0).gameObject);
            }
            GS2.Warn("Destroyed viewport content");
            //templateScrollView = Object.Instantiate(scrollview, content_gs, false);
            //templateScrollView.gameObject.SetActive(false);
            //details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            //UI Root/Overlay Canvas/Top Windows/Option Window/details/content-3/list/scroll-view/viewport/content
            //scrollview.gameObject.SetActive(false);

            //Transform advisorTips = content_gs.Find("advisor-tips");
            //if (advisorTips != null) Object.Destroy(advisorTips.gameObject);
            //Destroy surplus ui elements
            //Transform tipLevel = content_gs.Find("tiplevel");
            //if (tipLevel != null) Object.Destroy(tipLevel.gameObject);


            //Copy original combobox as a template, then get rid of it
            //RectTransform generatorPicker = content_3.Find("language").GetComponent<RectTransform>();
            //anchorX = generatorPicker.anchoredPosition.x;
            //anchorY = generatorPicker.anchoredPosition.y;
            //templateUIComboBox = Object.Instantiate<RectTransform>(details.Find("language").GetComponent<RectTransform>());
            //templateUIComboBox.gameObject.SetActive(false);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST3");
            GS2.Warn("Finding building-view");
            for (var i = 0; i < content_3.childCount; i++)
                GS2.Warn(content_3.GetChild(i).name);
            templateUIComboBox = CreateTemplate(GameObject.Find("UI Root/Overlay Canvas/Top Windows/Option Window/details/content-3/list/scroll-view/viewport/content/building-view").GetComponent<RectTransform>());
            GS2.Warn("Found building-view");
            //GS2.Log("CreateGalacticScaleSettingsPage TEST4");
            //Object.Destroy(generatorPicker.gameObject);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST5");

            //Create a template of a button
            GS2.Warn("Finding revert "+ (content_3 == null));
            Transform revertButton = content_3.Find("revert-button").transform;
            if (revertButton == null) GS2.Log("Couldn't find revert button");
            //GS2.Log("CreateGalacticScaleSettingsPage Test6");
            GS2.Warn("creating templateButton");
            templateButton = CreateTemplate(templateUIComboBox); //Get the basic layout for the new Button Item from a Combobox Item
                                                                 //GS2.Log("CreateGalacticScaleSettingsPage Test6.1");
                                                                 //       templateButton.GetComponentInChildren<UIComboBox>().gameObject.SetActive(false);
            GS2.Warn("destroying its gameobject");
            Object.Destroy(templateButton.GetComponentInChildren<UIComboBox>().gameObject);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.2");
            GS2.Warn("making a button ");
            RectTransform templateButtonButton = Object.Instantiate(revertButton.GetComponent<RectTransform>(), templateButton, false);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.3"); 
            templateButtonButton.anchoredPosition = new Vector2(250, 0);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.4"); 
            templateButtonButton.sizeDelta = new Vector2(200, 30);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.5"); 
            templateButton.anchorMin = templateButton.anchorMax = new Vector2(0, 1);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.6"); 
            templateButton.anchoredPosition = new Vector2(anchorX, anchorY);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.7"); 
            if (revertButton != null) Object.Destroy(revertButton.gameObject);
            GS2.Warn("1");
            //GS2.Log("CreateGalacticScaleSettingsPage TEST");
            templateOptionsCanvas = Object.Instantiate(content_gs, content_gs, false);
            templateOptionsCanvas.anchoredPosition = content_gs.anchoredPosition + new Vector2(750f, 0);
            templateOptionsCanvas.gameObject.name = "templateCanvasPanel";
            //GS2.Log("CreateGalacticScaleSettingsPage TEST2");
            GS2.Warn("2");
            while (templateOptionsCanvas.transform.childCount > 0)
            {
                Object.DestroyImmediate(templateOptionsCanvas.transform.GetChild(0).gameObject);
            }
            //GS2.Log("CreateGalacticScaleSettingsPage TEST3");
            GS2.Warn("3");
            RectTransform checkBoxProto = GameObject.Find("Option Window/details/content-1/fullscreen").GetComponent<RectTransform>(); //need to remove localizer, has textcomponent, and child called Checkbox with a UIToggle and a unityengine.ui.toggle
            templateCheckBox = CreateTemplate(checkBoxProto);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST4"); 
            RectTransform sliderProto = GameObject.Find("Option Window/details/content-1/dofblur").GetComponent<RectTransform>(); // localizer,  a textcomponent, has child called slider which has a UI.Slider component ,
            templateSlider = CreateTemplate(sliderProto);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST5");
            GS2.Warn("4");
            //RectTransform inputFieldProto = seedInput;
            GameObject inputFieldGO = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
            RectTransform inputFieldProto = Object.Instantiate(inputFieldGO.GetComponent<RectTransform>(), content_gs, false);
            //GS2.Log("Hmm");
            GS2.Warn("5");
            //localizer, has a ui.text comp, a child called inputfield which has a ui.inputfield, a uibutton and a eventsystems.eventtrigger
            //inputFieldProto.GetComponent<InputField>().interactable = true;
            if (inputFieldProto.transform.parent.GetComponent<Text>() != null) inputFieldProto.transform.parent.GetComponent<Text>().enabled = true;
            //GS2.Log("Hmm2");
            inputFieldProto.GetComponentInChildren<Text>().enabled = true;
            //GS2.Log("Hmm3");
            GS2.Warn("6");
            if (inputFieldProto.GetComponent<Image>() != null) inputFieldProto.GetComponent<Image>().enabled = true;
            //GS2.Log("CreateGalacticScaleSettingsPage TEST6 - " + (seedInput != null));
            GS2.Warn("7");
            RectTransform tempTransform = CreateTemplate(templateUIComboBox);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST7");
            UIComboBox tr = tempTransform.GetComponentInChildren<UIComboBox>();
            //GS2.Log("CreateGalacticScaleSettingsPage TEST7.5"); 
            RectTransform tr2 = tr.GetComponent<RectTransform>();
            //GS2.Log("CreateGalacticScaleSettingsPage TEST8");
            GS2.Warn("8");
            tempTransform.name = "TempTransform";
            inputFieldProto.name = "inputFieldProto";
            inputFieldProto.GetComponent<InputField>().characterLimit = 0;
            inputFieldProto.SetParent(tempTransform);
            inputFieldProto.anchorMin = tr2.anchorMin;
            inputFieldProto.anchorMax = tr2.anchorMax;
            inputFieldProto.offsetMin = tr2.offsetMin;
            inputFieldProto.offsetMax = tr2.offsetMax;
            inputFieldProto.sizeDelta = tr2.sizeDelta;
            inputFieldProto.anchoredPosition = new Vector2(250, -15);
            GS2.Warn("9");
            Object.DestroyImmediate(tempTransform.GetComponentInChildren<UIComboBox>().gameObject);
            templateInputField = CreateTemplate(tempTransform);
            templateInputField.name = "templateInputField*";


            //GS2.Log("CreateGalacticScaleSettingsPage 4");

            GS2.Warn("10");
            //Get a list of all loaded generators, and add a combobox to select between them.
            //List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            //options.Add(new GS2.GSOption("GS2", "Generator", "UIComboBox", generatorNames, new GS2.GSOptionCallback(GeneratorSelected)));
            GS2.Warn("11"); CreateOwnOptions();
            GS2.Warn("12"); ImportCustomGeneratorOptions();
            //GS2.Log("CreateGalacticScaleSettingsPage Test9");
            GS2.Warn("13"); CreateOptionsUI();
            GS2.Warn("14"); GS2.LoadPreferences();
            //GS2.Log("CreateGalacticScaleSettingsPage Test10");
            GS2.Warn("15");
            OptionsUIPostfix.Invoke();
            GS2.Warn("End");
            //GS2.Log("CreateGalacticScaleSettingsPage Test11");
        }
        private static void ImportCustomGeneratorOptions()
        {
            for (var i = 0; i < GS2.generators.Count; i++)
            {
                List<GSUI> pluginOptions = new List<GSUI>();
                //GS2.Log("IMPORT CUSTOM GENERATOR OPTIONS : " + GS2.generators[i].Name);
                if (GS2.generators[i] is iConfigurableGenerator gen) { 
                    //GS2.Log(gen.Name + " is configurable"); 
                    foreach (GSUI o in gen.Options) pluginOptions.Add(o); }
                generatorPluginOptions.Add(pluginOptions);
            }
        }
        private static void CreateOwnOptions()
        {
            //GS2.Log("CreateOwnOptions()");
            List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            options.Add(new GSUI("Generator", "Combobox", generatorNames, GeneratorSelected, CreateOwnOptionsPostFix));
            GS2.GS2RareChanceCheckbox = options.Add(GSUI.Checkbox("Force Rare Spawn", false, GS2.Force1RareOptionCallback, GS2.Force1RareOptionPostfix));
            GS2.DebugLogOption = options.Add(GSUI.Checkbox("Debug Log", false, GS2.DebugLogOptionCallback, GS2.DebugLogOptionPostfix));
            GS2.SkipPrologueOption = options.Add(GSUI.Checkbox("Skip Prologue", false, GS2.SkipPrologueOptionCallback, GS2.SkipPrologueOptionPostfix));
            GS2.NoTutorialsOption = options.Add(GSUI.Checkbox("Skip Tutorials", false, GS2.NoTutorialsOptionCallback, GS2.NoTutorialsOptionPostfix));
            GS2.UnlockTechOption = options.Add(GSUI.Checkbox("Cheat Mode", false, GS2.CheatModeOptionCallback, null));
        }
        private static void CreateOwnOptionsPostFix()
        {
            //GS2.Log("CreateGeneratorOptionsPostFix");
            var generatorIndex = 0;
            List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            for (var i = 0; i < generatorNames.Count; i++) if (generatorNames[i] == GS2.generator.Name) { 
                    /*GS2.Log("index found!" + i);*/ 
                    generatorIndex = i; 
                }
            if (optionRects[0] != null)
            {
                //GS2.Log("Setting combobox for generator index to " + generatorIndex);
                optionRects[0].GetComponentInChildren<UIComboBox>().itemIndex = generatorIndex;
            }
            //else GS2.Log("optionRects[0] == null!@#");
        }
        private static RectTransform CreateTemplate(RectTransform original)
        {
            RectTransform template = Object.Instantiate(original, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            template.gameObject.SetActive(false);
            template.name += "_t";
            Localizer l = template.GetComponentInChildren<Localizer>();
            if (l != null) Object.Destroy(l);
            return template;
        }

        // Method that handles creation of the settings tab
        private static void CreateOptionsUI()
        {
            //GS2.Log("CreateOptionsUI");
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].type)
                {
                    case "Combobox": CreateComboBox(options[i], content_gs, i); break;
                    case "Input": CreateInputField(options[i], content_gs, i); break;
                    case "Button": CreateButton(options[i], content_gs, i); break;
                    case "Checkbox": CreateCheckBox(options[i], content_gs, i); break;
                    case "Slider": CreateSlider(options[i], content_gs, i); break;
                    default: break;
                }
            }
            int currentGenIndex = GS2.GetCurrentGeneratorIndex();
            //GS2.Log("CreateGeneratorOptionsCanvases: currentGenIndex = " + currentGenIndex + " - " + GS2.generators[currentGenIndex].Name);
            for (var i = 0; i < generatorPluginOptions.Count; i++)
            { //for each canvas
                //GS2.Log("Creating Canvas " + i);
                RectTransform canvas = Object.Instantiate(templateOptionsCanvas, content_gs, false);
                canvas.name = "testCanvas" + i;
                generatorCanvases.Add(canvas);
                canvas.name = "generatorCanvas-" + GS2.generators[i].Name;
                if (currentGenIndex == i)
                {
                    //GS2.Log("Setting canvas active");
                    canvas.gameObject.SetActive(true);
                }
                else canvas.gameObject.SetActive(false);
                AddGeneratorPluginUIElements(canvas, i);

            }
        }



        /// Iterate through all the plugins that have elements to add to the UI, add them,// then add their postfixes to the event listener
        private static void AddGeneratorPluginUIElements(RectTransform canvas, int genIndex)
        {
            //GS2.Log("AddGeneratorPluginUIElements: " + GS2.generators[genIndex].Name);
            List<GSUI> options = generatorPluginOptions[genIndex];
            //GS2.Log(GS2.generators[genIndex].Name + " option count = " + options.Count);
            for (int i = 0; i < options.Count; i++)
            {
                switch (options[i].type)
                {
                    case "Combobox": CreateComboBox(options[i], canvas, i); break;
                    case "Button": CreateButton(options[i], canvas, i); break;
                    case "Input": CreateInputField(options[i], canvas, i); break;
                    case "Checkbox": CreateCheckBox(options[i], canvas, i); break;
                    case "Slider": CreateSlider(options[i], canvas, i); break;
                    default: break;
                }
                if (options[i].postfix != null) OptionsUIPostfix.AddListener(new UnityAction(options[i].postfix));
            }
        }

        // Create a combobox from a GSOption definition
        private static void CreateComboBox(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateComboBox");
            GS2.Warn("0");
            RectTransform comboBoxRect = Object.Instantiate(templateUIComboBox, canvas);
            GS2.Warn("1");
            comboBoxRect.name = o.label;
            comboBoxRect.gameObject.SetActive(true);
            GS2.Warn("2");
            optionRects.Add(comboBoxRect);
            GS2.Warn("3");
            o.rectTransform = comboBoxRect;
            int offset = index * -40;
            GS2.Warn("4");
            comboBoxRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            GS2.Warn("5");
            UIComboBox comboBoxUI = comboBoxRect.GetComponentInChildren<UIComboBox>();
            GS2.Warn("6");
            comboBoxUI.name = o.label + "_comboBox";
            comboBoxUI.Items = o.data as List<string>;
            GS2.Warn("7");
            comboBoxUI.UpdateItems();
            comboBoxUI.itemIndex = 0;
            GS2.Warn("8");
            comboBoxUI.onItemIndexChange.RemoveAllListeners();
            GS2.Warn("9");
            if (o.callback != null) comboBoxUI.onSubmit.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            //if (o.callback != null) comboBoxUI.onItemIndexChange.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            GS2.Warn("10");
            comboBoxRect.GetComponentInChildren<Text>().text = o.label;
            GS2.Warn("11");
            RectTransform tipTransform = comboBoxRect.GetChild(0).GetComponent<RectTransform>();
            GS2.Warn("12");
            tipTransform.gameObject.name = "optionTip-" + (index);
            GS2.Warn("13");
            if (tipTransform.GetComponent<Localizer>() != null) Object.Destroy(tipTransform.GetComponent<Localizer>());
            GS2.Warn("14");
            //tipTransform.GetComponent<Text>().text = o.tip;
            GS2.Warn("15");
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            GS2.Warn("16");
            //GS2.Log("Finished Creating ComboBox");
        }
        private static void CreateSlider(GSUI o, RectTransform canvas, int index)
        {
            RectTransform sliderRect = Object.Instantiate(templateSlider, canvas);
            sliderRect.name = o.label;
            sliderRect.gameObject.SetActive(true); 
            optionRects.Add(sliderRect);
            o.rectTransform = sliderRect; 
            int offset = index * -40; 
            sliderRect.anchoredPosition = new Vector2(anchorX, anchorY + offset); 
            Slider slider = sliderRect.GetComponentInChildren<Slider>(); 
            slider.name = o.label + "_comboBox"; 
            Text label = slider.GetComponentInChildren<Text>();
            
            GSSliderConfig gssc = (GSSliderConfig)o.data; 
            slider.minValue = gssc.minValue;
            slider.maxValue = gssc.maxValue;
            slider.wholeNumbers = gssc.wholeNumbers;
            slider.value = gssc.defaultValue; 
            label.text = slider.value.ToString();
            slider.onValueChanged.AddListener((v) => label.text = v.ToString());
            if (o.callback != null) slider.onValueChanged.AddListener(delegate { o.callback(slider.value); }); 
            sliderRect.GetComponentInChildren<Text>().text = o.label; 
            RectTransform tipTransform = sliderRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + (index); 
            Object.Destroy(tipTransform.GetComponent<Localizer>()); 
            //tipTransform.GetComponent<Text>().text = o.tip; GS2.Log("CreateSlider17");
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix)); 
        }
        private static void CreateCheckBox(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateComboBox");
            RectTransform checkBoxRect = Object.Instantiate(templateCheckBox, canvas);
            checkBoxRect.name = o.label;
            checkBoxRect.gameObject.SetActive(true);
            optionRects.Add(checkBoxRect);
            o.rectTransform = checkBoxRect;
            int offset = index * -40;
            checkBoxRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            Toggle toggle = checkBoxRect.GetComponentInChildren<Toggle>();
            toggle.name = o.label + "_checkBox";
            toggle.isOn = (bool)o.data;
            toggle.onValueChanged.RemoveAllListeners();
            if (o.callback != null) toggle.onValueChanged.AddListener(delegate { o.callback(toggle.isOn); });
            checkBoxRect.GetComponentInChildren<Text>().text = o.label;
            //RectTransform tipTransform = checkBoxRect.GetChild(0).GetComponent<RectTransform>();
            //tipTransform.gameObject.name = "optionTip-" + (index);
            //Object.Destroy(tipTransform.GetComponent<Localizer>());
            //tipTransform.GetComponent<Text>().text = o.tip;
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            //GS2.Log("Finished Creating ComboBox");
        }

        // Create an input field from a GSOption definition
        private static void CreateInputField(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateInputField");
            RectTransform inputRect = Object.Instantiate(templateInputField, canvas);
            //GS2.Log("-1");
            inputRect.name = o.label;
            //GS2.Log("-2");
            inputRect.gameObject.SetActive(true);
            //GS2.Log("-3");
            optionRects.Add(inputRect);
            //GS2.Log("-4");
            o.rectTransform = inputRect;
            //GS2.Log("-5");
            int offset = index * -40;
            //GS2.Log("-6");
            inputRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            //GS2.Log("-7");
            InputField inputUI = inputRect.GetComponentInChildren<InputField>();
            //GS2.Log("-8");
            inputUI.name = o.label + "_inputField";
            //GS2.Log("-9");
            inputUI.text = o.data as string;
            //GS2.Log("-10");
            inputUI.onValueChanged.RemoveAllListeners();
            //GS2.Log("-11");
            if (o.callback != null) inputUI.onValueChanged.AddListener(delegate { o.callback(inputUI.text); }); //GS2.Log("-12");
            inputRect.GetComponentInChildren<Text>().text = o.label; //GS2.Log("-13");
            Object.Destroy(inputUI.GetComponentInChildren<UnityEngine.EventSystems.EventTrigger>()); //GS2.Log("-14");
            //RectTransform tipTransform = inputRect.GetChild(0).GetComponent<RectTransform>(); //GS2.Log("-15");
            //tipTransform.gameObject.name = "optionTip-" + (index); //GS2.Log("-16");
            //Object.Destroy(tipTransform.GetComponent<Localizer>()); //GS2.Log("-17");
            //tipTransform.GetComponent<Text>().text = o.tip;// GS2.Log("-18");
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));// GS2.Log("-19");
            //GS2.Log("Finished Creating InputField");
        }          
        // Create a button from a GSOption definition
        private static void CreateButton(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateButton "+o.label);
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
            //RectTransform tipTransform = buttonRect.GetChild(0).GetComponent<RectTransform>();
            //tipTransform.gameObject.name = "optionTip-" + (index);
            //Object.Destroy(tipTransform.GetComponent<Localizer>());
            //tipTransform.GetComponent<Text>().text = o.tip;
            if (o.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.postfix));
            //GS2.Log("Finished Creating Button");
        }

        // Callback for own Generator ComboBox Selection Event
        private static void GeneratorSelected(object result)
        {
            //GS2.Log("Result = " + result + GS2.generators[(int)result].Name);
            GS2.generator = GS2.generators[(int)result];
            //GS2.Log("Set the generator, trying to disable every canvas");
            for (var i = 0; i < generatorCanvases.Count; i++) generatorCanvases[i].gameObject.SetActive(false);
            //GS2.Log("trying to setactive");
            generatorCanvases[(int)result].gameObject.SetActive(true);
            //GS2.Log("trying to save prefs");
            GS2.SavePreferences();
        }
        private static void GalacticScaleTabClick()
        {
            UIRoot.instance.optionWindow.SetTabIndex(MainTabIndex, false);
            scrollview.gameObject.SetActive(true);
        }
        public static void DisableDetails()
        {
            if (scrollview != null && scrollview.gameObject != null) scrollview.gameObject.SetActive(false);
        }
    }
}