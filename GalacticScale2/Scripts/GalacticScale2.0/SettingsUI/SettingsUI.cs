using System.Collections.Generic;
using HarmonyLib;
using UITools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GalacticScale
{
    public struct GSSliderConfig
    {
        public float minValue;
        public float maxValue;
        public float defaultValue;

        public GSSliderConfig(float minValue, float value, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            defaultValue = value;
        }
    }

    public static class SettingsUI
    {
        public static int MainTabIndex = 5;
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        private static UIButton[] tabButtons;
        private static Text[] tabTexts;
        public static RectTransform seedInput;
        private static RectTransform details;
        private static RectTransform scrollview;
        private static RectTransform templateOptionsCanvas;
        private static RectTransform templateUIComboBox;
        private static RectTransform templateCheckBox;
        private static RectTransform templateInputField;
        private static RectTransform templateSlider;
        private static RectTransform templateButton;
        private static RectTransform templateScrollView;
        private static readonly List<RectTransform> optionRects = new List<RectTransform>();
        public static readonly List<RectTransform> GeneratorCanvases = new List<RectTransform>();
        public static readonly List<List<GSUI>> generatorPluginOptions = new List<List<GSUI>>();
        private static float anchorX;
        private static float anchorY;
        public static int GeneratorIndex;

        private static readonly GSOptions options = new GSOptions();

        public static UnityEvent OptionsUIPostfix = new UnityEvent();

        public static void CreateGalacticScaleSettingsPage(UIButton[] _tabButtons, Text[] _tabTexts)
        {
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();
            tabButtons = _tabButtons;
            tabTexts = _tabTexts;

            //Add Tab Button
            var tabParent = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>().parent;
            MainTabIndex = tabParent.childCount - 1;
            var tabButtonTemplate = tabParent.GetChild(tabParent.childCount - 1).GetComponent<RectTransform>();
            galacticButton = Object.Instantiate(tabButtonTemplate, tabLine, false);
            galacticButton.name = "tab-button-gs";
            galacticButton.anchoredPosition =
                new Vector2(galacticButton.anchoredPosition.x + 160, galacticButton.anchoredPosition.y);
            Object.Destroy(galacticButton.GetComponentInChildren<Localizer>());
            galacticButton.GetComponent<Button>().onClick.RemoveAllListeners();
            galacticButton.GetComponentInChildren<Text>().text = "Galactic Scale";
            galacticButton.GetComponent<Button>().onClick.AddListener(GalacticScaleTabClick);
            tabButtons.AddItem(galacticButton.GetComponent<UIButton>());
            tabTexts.AddItem(galacticButton.GetComponentInChildren<Text>());


            var scrollviewTemplate = GameObject
                .Find("UI Root/Overlay Canvas/Top Windows/Option Window/details/content-4/list")
                .GetComponent<RectTransform>();
            scrollview = Object.Instantiate(scrollviewTemplate,
                GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            scrollview.offsetMax = new Vector2(0, 0);
            scrollview.offsetMin = new Vector2(0, 0);
            scrollview.name = "GalacticScaleSettings";
            var svContent = scrollview.GetComponentInChildren<EnsureIntPosition>();
            while (svContent.transform.childCount > 0)
                Object.DestroyImmediate(svContent.transform.GetChild(0).gameObject);
            //   UI Root/Overlay Canvas/Top Windows/Option Window/details/content-4/sep-line-bottom-1
            //  UI Root/Overlay Canvas/Top Windows/Option Window/details/content-4/sep-line-top-1
            //Create the galactic scale settings panel
            var detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            details = Object.Instantiate(detailsTemplate,
                scrollview.GetComponentInChildren<EnsureIntPosition>().GetComponent<RectTransform>(), false);
            templateScrollView = Object.Instantiate(scrollview, details, false);
            templateScrollView.gameObject.SetActive(false);
            //details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            scrollview.gameObject.SetActive(false);
            details.gameObject.SetActive(true);
            details.gameObject.name = "content-gs";
            var advisorTips = details.Find("advisor-tips");
            if (advisorTips != null) Object.Destroy(advisorTips.gameObject);
            //Destroy surplus ui elements
            var tipLevel = details.Find("tiplevel");
            if (tipLevel != null) Object.Destroy(tipLevel.gameObject);


            //Copy original combobox as a template, then get rid of it
            var languageCombo = details.Find("language").GetComponent<RectTransform>();
            anchorX = languageCombo.anchoredPosition.x;
            anchorY = languageCombo.anchoredPosition.y;
            templateUIComboBox = CreateTemplate(details.Find("language").GetComponent<RectTransform>());
            Object.Destroy(languageCombo.gameObject);

            //Create a template of a button
            var revertButton = details.Find("revert-button");
            if (revertButton == null) GS2.Log("Couldn't find revert button");
            //GS2.Log("CreateGalacticScaleSettingsPage Test6");
            templateButton = CreateTemplate(templateUIComboBox);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.1");
            templateButton.GetComponentInChildren<UIComboBox>().gameObject.SetActive(false);
            //Object.Destroy(templateButton.GetComponentInChildren<UIComboBox>().gameObject);
            //GS2.Log("CreateGalacticScaleSettingsPage Test6.2");
            var templateButtonButton =
                Object.Instantiate(revertButton.GetComponent<RectTransform>(), templateButton, false);
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

            //GS2.Log("CreateGalacticScaleSettingsPage TEST");
            templateOptionsCanvas = Object.Instantiate(details, details, false);
            templateOptionsCanvas.anchoredPosition = details.anchoredPosition + new Vector2(750f, 0);
            templateOptionsCanvas.gameObject.name = "templateCanvasPanel";
            //GS2.Log("CreateGalacticScaleSettingsPage TEST2");
            while (templateOptionsCanvas.transform.childCount > 0)
                Object.DestroyImmediate(templateOptionsCanvas.transform.GetChild(0).gameObject);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST3");
            var checkBoxProto = GameObject.Find("Option Window/details/content-1/fullscreen")
                .GetComponent<RectTransform>(); //need to remove localizer, has textcomponent, and child called Checkbox with a UIToggle and a unityengine.ui.toggle
            templateCheckBox = CreateTemplate(checkBoxProto);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST4"); 
            var sliderProto = GameObject.Find("Option Window/details/content-1/dofblur")
                .GetComponent<RectTransform>(); // localizer,  a textcomponent, has child called slider which has a UI.Slider component ,
            templateSlider = CreateTemplate(sliderProto);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST5");

            //RectTransform inputFieldProto = seedInput;
            var inputFieldGO = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
            var inputFieldProto = Object.Instantiate(inputFieldGO.GetComponent<RectTransform>(), details, false);
            //GS2.Log("Hmm");
            //localizer, has a ui.text comp, a child called inputfield which has a ui.inputfield, a uibutton and a eventsystems.eventtrigger
            //inputFieldProto.GetComponent<InputField>().interactable = true;
            if (inputFieldProto.transform.parent.GetComponent<Text>() != null)
                inputFieldProto.transform.parent.GetComponent<Text>().enabled = true;
            //GS2.Log("Hmm2");
            inputFieldProto.GetComponentInChildren<Text>().enabled = true;
            //GS2.Log("Hmm3");
            if (inputFieldProto.GetComponent<Image>() != null) inputFieldProto.GetComponent<Image>().enabled = true;
            //GS2.Log("CreateGalacticScaleSettingsPage TEST6 - " + (seedInput != null));
            var tempTransform = CreateTemplate(templateUIComboBox);
            //GS2.Log("CreateGalacticScaleSettingsPage TEST7");
            var tr = tempTransform.GetComponentInChildren<UIComboBox>();
            //GS2.Log("CreateGalacticScaleSettingsPage TEST7.5"); 
            var tr2 = tr.GetComponent<RectTransform>();
            //GS2.Log("CreateGalacticScaleSettingsPage TEST8");
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
            Object.DestroyImmediate(tempTransform.GetComponentInChildren<UIComboBox>().gameObject);
            templateInputField = CreateTemplate(tempTransform);
            templateInputField.name = "templateInputField*";


            //GS2.Log("CreateGalacticScaleSettingsPage 4");
            //CreateCollapseBox();

            //Get a list of all loaded generators, and add a combobox to select between them.
            //List<string> generatorNames = GS2.generators.ConvertAll<string>((iGenerator iGen) => { return iGen.Name; });
            //options.Add(new GS2.GSOption("GS2", "Generator", "UIComboBox", generatorNames, new GS2.GSOptionCallback(GeneratorSelected)));
            // CreateOwnOptions();
            ImportCustomGeneratorOptions();
            //GS2.Log("CreateGalacticScaleSettingsPage Test9");
            CreateOptionsUI();
            CreateGeneratorTabs();
            GS2.LoadPreferences();
            //GS2.Log("CreateGalacticScaleSettingsPage Test10");
            OptionsUIPostfix.Invoke();
            //GS2.Log("CreateGalacticScaleSettingsPage Test11");
            //UI Root/Overlay Canvas/Top Windows/Option Window/details/content-4/list/scroll-view/viewport/content
            //UI Root/Overlay Canvas/Top Windows/Option Window/details/GalacticScaleSettings/scroll-view/viewport
            UpdateContentRect();
            //viewportRect.sizeDelta = this.keyScrollVbarRect.gameObject.activeSelf ? new Vector2(-10f, 0.0f) : Vector2.zero;
        }

        //private static RectTransform CreateCollapseBox()
        //{
        //    GameObject collapseBox = new GameObject("collapseBoxTemplate");
        //    collapseBox.transform.SetParent(details);
        //    collapseBox.transform.position = new Vector3(0, 0, 0);
        //    var rt = collapseBox.AddComponent<RectTransform>();
        //    rt.anchorMin = new Vector2(30, 30);
        //    rt.anchoredPosition = new Vector2(10, 10);
        //    rt.sizeDelta = new Vector2(100, 100);
        //    Image img = collapseBox.AddComponent<Image>();
        //    img.color = Color.red;
        //    return collapseBox.GetComponent<RectTransform>();
        //}
        public static void UpdateContentRect()
        {
            var optionlist = generatorPluginOptions[GeneratorIndex];
            var entries = optionlist.Count;
            //GS2.LogJson(optionlist, true);
            //GS2.Warn("Option List Count = " + entries);
            var contentRect = GameObject
                .Find(
                    "UI Root/Overlay Canvas/Top Windows/Option Window/details/GalacticScaleSettings/scroll-view/viewport/content")
                .GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 42 * entries + 8);
            contentRect.anchoredPosition = new Vector2(Mathf.Round(contentRect.anchoredPosition.x),
                Mathf.Round(contentRect.anchoredPosition.y));
        }

        private static void ImportCustomGeneratorOptions()
        {
            GS2.Config.Generate(0);
            for (var i = 0; i < GS2.Generators.Count; i++)
            {
                var pluginOptions = new List<GSUI>();
                //GS2.Log("IMPORT CUSTOM GENERATOR OPTIONS : " + GS2.generators[i].Name);
                if (GS2.Generators[i] is iConfigurableGenerator gen)
                    //GS2.Log(gen.Name + " is configurable"); 
                    for (var j = 0; j < gen.Options.Count; j++)
                        // (GSUI o in gen.Options) {
                        if (!(gen.Options[j] is GSUI))
                        {
                            GS2.Error($"Non UI Element Found in UI Element List for generator {gen.Name}");
                        }
                        else
                        {
                            var o = gen.Options[j];
                            //GS2.Warn(o.Label);
                            pluginOptions.Add(o);
                        }

                generatorPluginOptions.Add(pluginOptions);
            }
        }

        private static void CreateOwnOptions()
        {
            //GS2.Log("CreateOwnOptions()");
            // var generatorNames = GS2.generators.ConvertAll(iGen => { return iGen.Name; });
            // options.Add(GSUI.Combobox("Generator".Translate(), generatorNames, GeneratorSelected, CreateGeneratorTabs));
            // GS2.Force1RareChanceOption = options.Add(GSUI.Checkbox("Force Rare Spawn".Translate(), false,
            //     GS2.Force1RareOptionCallback, GS2.Force1RareOptionPostfix));
            // GS2.DebugLogOption = options.Add(GSUI.Checkbox("Debug Log".Translate(), false, GS2.DebugLogOptionCallback,
            //     GS2.DebugLogOptionPostfix));
            // GS2.SkipPrologueOption = options.Add(GSUI.Checkbox("Skip Prologue".Translate(), false, GS2.SkipPrologueOptionCallback,
            //     GS2.SkipPrologueOptionPostfix));
            // GS2.NoTutorialsOption = options.Add(GSUI.Checkbox("Skip Tutorials".Translate(), false, GS2.NoTutorialsOptionCallback,
            //     GS2.NoTutorialsOptionPostfix));
            // GS2.CheatModeOption = options.Add(GSUI.Checkbox("Cheat Mode".Translate(), false, GS2.CheatModeOptionCallback,
            //     GS2.CheatModeOptionPostfix));
        }

        public static void CreateGeneratorTabs()
        {
            //GS2.Log("CreateGeneratorOptionsPostFix");
            
            var generatorNames = GS2.Generators.ConvertAll(iGen => { return iGen.Name; });
            GS2.LogJson(generatorNames);
            for (var i = 0; i < generatorNames.Count; i++)
                if (generatorNames[i] == GS2.ActiveGenerator.Name)
                    /*GS2.Log("index found!" + i);*/
                    GeneratorIndex = i;

            if (optionRects[0] != null)
                //GS2.Log("Setting combobox for generator index to " + generatorIndex);
                optionRects[0].GetComponentInChildren<UIComboBox>().itemIndex = GeneratorIndex;
            //else GS2.Log("optionRects[0] == null!@#");
        }

        private static RectTransform CreateTemplate(RectTransform original)
        {
            var template = Object.Instantiate(original,
                GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);
            template.gameObject.SetActive(false);
            var l = template.GetComponentInChildren<Localizer>();
            if (l != null) Object.Destroy(l);

            return template;
        }

        // Method that handles creation of the settings tab
        private static void CreateOptionsUI()
        {

            GSOptions o = GS2.Config.Options;
            options.AddRange(o);
            for (var i = 0; i < options.Count; i++)
            {
                switch (options[i].Type)
                {
                    case "Combobox":
                        CreateComboBox(options[i], details, i);
                        break;
                    case "Input":
                        CreateInputField(options[i], details, i);
                        break;
                    case "Button":
                        CreateButton(options[i], details, i);
                        break;
                    case "Checkbox":
                        CreateCheckBox(options[i], details, i);
                        break;
                    case "Slider":
                        CreateSlider(options[i], details, i);
                        break;
                    default:
                        GS2.Warn($"Couldn't create option {options[i].Label}");
                        break;
                }
            }


            var currentGenIndex = GS2.GetCurrentGeneratorIndex();
            //GS2.Log("CreateGeneratorOptionsCanvases: currentGenIndex = " + currentGenIndex + " - " + GS2.generators[currentGenIndex].Name);
            for (var i = 0; i < generatorPluginOptions.Count; i++)
            {
                //for each canvas
                //GS2.Log("Creating Canvas " + i);
                var canvas = Object.Instantiate(templateOptionsCanvas, details, false);
                canvas.name = "testCanvas" + i;
                GeneratorCanvases.Add(canvas);
                canvas.name = "generatorCanvas-" + GS2.Generators[i].Name;
                if (currentGenIndex == i)
                {
                    //GS2.Log("Setting canvas active");
                    canvas.gameObject.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    canvas.gameObject.SetActive(false);
                }

                AddGeneratorPluginUIElements(canvas, i);
            }
        }


        /// Iterate through all the plugins that have elements to add to the UI, add them,// then add their postfixes to the event listener
        private static void AddGeneratorPluginUIElements(RectTransform canvas, int genIndex)
        {
            GS2.Log("AddGeneratorPluginUIElements: " + GS2.Generators[genIndex].Name);
            var generatorPluginOption = generatorPluginOptions[genIndex];
            GS2.Log(GS2.Generators[genIndex].Name + " option count = " + generatorPluginOption.Count);
            for (var i = 0; i < generatorPluginOption.Count; i++)
            {
                switch (generatorPluginOption[i].Type)
                {
                    case "Combobox":
                        CreateComboBox(generatorPluginOption[i], canvas, i);
                        break;
                    case "Button":
                        CreateButton(generatorPluginOption[i], canvas, i);
                        break;
                    case "Input":
                        CreateInputField(generatorPluginOption[i], canvas, i);
                        break;
                    case "Checkbox":
                        CreateCheckBox(generatorPluginOption[i], canvas, i);
                        break;
                    case "Slider":
                        CreateSlider(generatorPluginOption[i], canvas, i);
                        break;
                    default:
                        GS2.Warn($"Couldn't create option {generatorPluginOption[i].Label}");
                        break;
                }

                if (generatorPluginOption[i].Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(generatorPluginOption[i].Postfix));
            }
        }

        // Create a combobox from a GSOption definition
        private static void CreateComboBox(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateComboBox");
            var comboBoxRect = Object.Instantiate(templateUIComboBox, canvas);
            comboBoxRect.name = o.Label;
            comboBoxRect.gameObject.SetActive(true);
            optionRects.Add(comboBoxRect);
            o.RectTransform = comboBoxRect;
            var offset = index * -40;
            comboBoxRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            var comboBoxUI = comboBoxRect.GetComponentInChildren<UIComboBox>();
            comboBoxUI.name = o.Label + "_comboBox";
            comboBoxUI.Items = o.Data as List<string>;
            comboBoxUI.UpdateItems();
            comboBoxUI.itemIndex = 0;
            comboBoxUI.onItemIndexChange.RemoveAllListeners();
            if (o.callback != null) comboBoxUI.onSubmit.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            //if (o.callback != null) comboBoxUI.onItemIndexChange.AddListener(delegate { o.callback(comboBoxUI.itemIndex); });
            comboBoxRect.GetComponentInChildren<Text>().text = o.Label;
            var tipTransform = comboBoxRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + index;
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.Tip;
            if (o.Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.Postfix));
            //GS2.Log("Finished Creating ComboBox");
        }

        private static void CreateSlider(GSUI o, RectTransform canvas, int index)
        {
            var sliderRect = Object.Instantiate(templateSlider, canvas);
            sliderRect.name = o.Label;
            sliderRect.gameObject.SetActive(true);
            optionRects.Add(sliderRect);
            o.RectTransform = sliderRect;
            var offset = index * -40;
            sliderRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            var slider = sliderRect.GetComponentInChildren<Slider>();
            slider.name = o.Label + "_comboBox";
            var label = slider.GetComponentInChildren<Text>();

            var gssc = (GSSliderConfig) o.Data;
            slider.minValue = gssc.minValue;
            slider.maxValue = gssc.maxValue;
            slider.wholeNumbers = o.increment % 1 == 0;
            slider.value = gssc.defaultValue;
            label.text = slider.value.ToString();
            slider.onValueChanged.AddListener(v => label.text = v.ToString());
            if (o.callback != null) slider.onValueChanged.AddListener(delegate { o.callback(slider.value); });

            sliderRect.GetComponentInChildren<Text>().text = o.Label;
            var tipTransform = sliderRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + index;
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            //tipTransform.GetComponent<Text>().text = o.tip; GS2.Log("CreateSlider17");
            if (o.Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.Postfix));
        }

        private static void CreateCheckBox(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateComboBox");
            var checkBoxRect = Object.Instantiate(templateCheckBox, canvas);
            checkBoxRect.name = o.Label;
            checkBoxRect.gameObject.SetActive(true);
            optionRects.Add(checkBoxRect);
            o.RectTransform = checkBoxRect;
            var offset = index * -40;
            checkBoxRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            var toggle = checkBoxRect.GetComponentInChildren<Toggle>();
            toggle.name = o.Label + "_checkBox";
            toggle.isOn = (bool) o.Data;
            toggle.onValueChanged.RemoveAllListeners();
            if (o.callback != null) toggle.onValueChanged.AddListener(delegate { o.callback(toggle.isOn); });

            checkBoxRect.GetComponentInChildren<Text>().text = o.Label;
            //RectTransform tipTransform = checkBoxRect.GetChild(0).GetComponent<RectTransform>();
            //tipTransform.gameObject.name = "optionTip-" + (index);
            //Object.Destroy(tipTransform.GetComponent<Localizer>());
            //tipTransform.GetComponent<Text>().text = o.tip;
            if (o.Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.Postfix));
            //GS2.Log("Finished Creating ComboBox");
        }

        // Create an input field from a GSOption definition
        private static void CreateInputField(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateInputField");
            var inputRect = Object.Instantiate(templateInputField, canvas);
            //GS2.Log("-1");
            inputRect.name = o.Label;
            //GS2.Log("-2");
            inputRect.gameObject.SetActive(true);
            //GS2.Log("-3");
            optionRects.Add(inputRect);
            //GS2.Log("-4");
            o.RectTransform = inputRect;
            //GS2.Log("-5");
            var offset = index * -40;
            //GS2.Log("-6");
            inputRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            //GS2.Log("-7");
            var inputUI = inputRect.GetComponentInChildren<InputField>();
            //GS2.Log("-8");
            inputUI.name = o.Label + "_inputField";
            //GS2.Log("-9");
            inputUI.text = o.Data as string;
            //GS2.Log("-10");
            inputUI.onValueChanged.RemoveAllListeners();
            //GS2.Log("-11");
            if (o.callback != null)
                inputUI.onValueChanged.AddListener(delegate { o.callback(inputUI.text); }); //GS2.Log("-12");

            inputRect.GetComponentInChildren<Text>().text = o.Label; //GS2.Log("-13");
            Object.Destroy(inputUI.GetComponentInChildren<EventTrigger>()); //GS2.Log("-14");
            var tipTransform = inputRect.GetChild(0).GetComponent<RectTransform>(); //GS2.Log("-15");
            tipTransform.gameObject.name = "optionTip-" + index; //GS2.Log("-16");
            Object.Destroy(tipTransform.GetComponent<Localizer>()); //GS2.Log("-17");
            tipTransform.GetComponent<Text>().text = o.Tip; // GS2.Log("-18");
            if (o.Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.Postfix)); // GS2.Log("-19");
            //GS2.Log("Finished Creating InputField");
        } // Create a button from a GSOption definition

        private static void CreateButton(GSUI o, RectTransform canvas, int index)
        {
            //GS2.Log("CreateButton "+o.Label);
            var buttonRect = Object.Instantiate(templateButton, canvas);
            buttonRect.name = o.Label;
            buttonRect.gameObject.SetActive(true);
            optionRects.Add(buttonRect);
            o.RectTransform = buttonRect;
            var offset = index * -40;
            buttonRect.anchoredPosition = new Vector2(anchorX, anchorY + offset);
            var uiButton = buttonRect.GetComponentInChildren<UIButton>();
            uiButton.name = o.Label + "_button";
            uiButton.button.onClick.RemoveAllListeners();
            uiButton.GetComponentInChildren<Text>().text = o.Data.ToString();
            var l = uiButton.GetComponentInChildren<Localizer>();
            Object.Destroy(l);
            if (o.callback != null) uiButton.button.onClick.AddListener(delegate { o.callback(null); });

            buttonRect.GetComponentInChildren<Text>().text = o.Label;
            var tipTransform = buttonRect.GetChild(0).GetComponent<RectTransform>();
            tipTransform.gameObject.name = "optionTip-" + index;
            Object.Destroy(tipTransform.GetComponent<Localizer>());
            tipTransform.GetComponent<Text>().text = o.Tip;
            if (o.Postfix != null) OptionsUIPostfix.AddListener(new UnityAction(o.Postfix));
        }

        // Callback for own Generator ComboBox Selection Event
        private static void GeneratorSelected(Val result)
        {
            //GS2.Log("Result = " + result + GS2.generators[(int)result].Name);
            GS2.ActiveGenerator = GS2.Generators[result];
            //GS2.Log("Set the generator, trying to disable every canvas");
            for (var i = 0; i < GeneratorCanvases.Count; i++) GeneratorCanvases[i].gameObject.SetActive(false);
            //GS2.Log("trying to setactive");
            GeneratorCanvases[result].gameObject.SetActive(true);
            GeneratorIndex = result;
            UpdateContentRect();
            //GS2.Log("trying to save prefs");
            GS2.SavePreferences();
        }

        public static void GalacticScaleTabClick()
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