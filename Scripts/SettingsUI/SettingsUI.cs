//using System.Linq;
//using UITools;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GalacticScale.GS2;

namespace GalacticScale
{
    public static class SettingsUI
    {
        public static int MainTabIndex = 5;
        private static RectTransform tabLine;
        private static RectTransform galacticButton;
        public static RectTransform details;
        private static RectTransform GSSettingsPanel;
        private static GSUIPanel SettingsPanel;
        public static RectTransform comboTemplate;

        //private static readonly List<RectTransform> optionRects = new List<RectTransform>();
        public static readonly List<RectTransform> GeneratorCanvases = new();

        public static readonly List<List<GSUI>> generatorPluginOptions = new();

        // public static GameObject themeselector;
        private static float anchorX;
        private static float anchorY;
        public static int GeneratorIndex;

        private static readonly GSOptions options = new();

        public static UnityEvent OptionsUIPostfix = new();

        public static void CreateGalacticScaleSettingsPage(UIButton[] _tabButtons, Text[] _tabTexts)
        {
            tabLine = GameObject.Find("Top Windows/Option Window/tab-line").GetComponent<RectTransform>();

            //Add Tab Button
            var tabParent = GameObject.Find("Option Window/tab-line/tab-button-5").GetComponent<RectTransform>().parent;
            MainTabIndex = tabParent.childCount - 1;
            var tabButtonTemplate = tabParent.GetChild(tabParent.childCount - 1).GetComponent<RectTransform>();
            galacticButton = Object.Instantiate(tabButtonTemplate, tabLine, false);
            galacticButton.name = "tab-button-gs";
            galacticButton.anchoredPosition = new Vector2(galacticButton.anchoredPosition.x + 160, galacticButton.anchoredPosition.y);
            Object.Destroy(galacticButton.GetComponentInChildren<Localizer>());
            galacticButton.GetComponent<Button>().onClick.RemoveAllListeners();
            galacticButton.GetComponentInChildren<Text>().text = "Galactic Scale";
            galacticButton.GetComponent<Button>().onClick.AddListener(GalacticScaleTabClick);
            _tabButtons.AddItem(galacticButton.GetComponent<UIButton>());
            _tabTexts.AddItem(galacticButton.GetComponentInChildren<Text>());

            var detailsTemplate = GameObject.Find("Option Window/details/content-5").GetComponent<RectTransform>();
            details = Object.Instantiate(detailsTemplate, GameObject.Find("Option Window/details").GetComponent<RectTransform>(), false);


            details.gameObject.SetActive(true);
            details.gameObject.name = "content-gs";

            var languageCombo = details.Find("language").GetComponent<RectTransform>();
            anchorX = languageCombo.anchoredPosition.x;
            anchorY = languageCombo.anchoredPosition.y;
            while (details.transform.childCount > 0)
                Object.DestroyImmediate(details.transform.GetChild(0).gameObject);

            ImportCustomGeneratorOptions();
            CreateOptionsUI();
            CreateGeneratorTabs();
            LoadPreferences();
            OptionsUIPostfix.Invoke();
        }

        private static void ImportCustomGeneratorOptions()
        {
            Config.Generate(0);
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
                            Error($"Non UI Element Found in UI Element List for generator {gen.Name}");
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

        public static void CreateGeneratorTabs()
        {
            // Log("CreateGeneratorOptionsPostFix");

            var generatorNames = GS2.Generators.ConvertAll(iGen => { return iGen.Name; });
            // LogJson(generatorNames);
            for (var i = 0; i < generatorNames.Count; i++)
                if (generatorNames[i] == ActiveGenerator.Name)
                    GeneratorIndex = i;
            // Log("Got this far");
        }

        private static void CreateOptionsUI()
        {
            var gsuipath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location), "GSUI.dll");
            if (!File.Exists(gsuipath))
            {
                ShowMessage("Missing GSUI.DLL".Translate(), "Error".Translate(), "Ok".Translate());
                return;
            }

            Assembly.LoadFrom(gsuipath);
            var gsp = Bundle.LoadAsset<GameObject>("assets/gssettingspanel.prefab");
            GSSettingsPanel = Object.Instantiate(gsp, details, false).GetComponent<RectTransform>();
            GSSettingsPanel.GetComponent<ScrollRect>().scrollSensitivity = 10;
            var sp = Bundle.LoadAsset<GameObject>("SettingsPanel");

            SettingsPanel = GSSettingsPanel.GetComponentInChildren<GSUIPanel>();
            options.AddRange(Config.Options);

            // Add UI Elements for Plugins
            for (var i = 0; i < Plugins.Count; i++)
            {
                Warn($"Loading Plugin #{i} {Plugins[i].Name}");
                var plugin = Plugins[i];
                var group = GSUI.Group(plugin.Name, plugin.Options, plugin.Description, true, true, o =>
                {
                    plugin.Enabled = o;
                    // var x = plugin.Export();
                    //Warn(plugin.Enabled.ToString());
                });
                UnityAction postfix = () =>
                {
                    //Warn($"Setting {plugin.Enabled}");
                    group.Set(plugin.Enabled);
                    plugin.Enabled = plugin.Enabled;
                };
                OptionsUIPostfix.AddListener(postfix);
                options.Add(GSUI.Spacer());
                options.Add(group);
                options.Add(GSUI.Spacer());
            }


            var currentGenIndex = GetCurrentGeneratorIndex();
            var scrollContentRect = SettingsPanel.transform.parent.GetComponent<RectTransform>();
            for (var i = 0; i < generatorPluginOptions.Count; i++)
            {
                //for each canvas
                var canvas = Object.Instantiate(sp, scrollContentRect, false).GetComponent<RectTransform>();

                canvas.anchoredPosition = new Vector2(anchorX + 750, anchorY);
                GeneratorCanvases.Add(canvas);
                canvas.name = "generatorCanvas-" + GS2.Generators[i].Name;
                if (currentGenIndex == i)
                {
                    // GS2.Log("Setting canvas active");
                    canvas.gameObject.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    canvas.gameObject.SetActive(false);
                }

                // GS2.Log($"Creating Generator Plugin UIElements for {i}");
                AddGeneratorPluginUIElements(canvas, i);
            }

            // Warn("Creating Main Settings");
            for (var i = 0; i < options.Count; i++) // Main Settings
                // GS2.Warn($"Creating {options[i].Label}");
                CreateUIElement(options[i], SettingsPanel.contents);
        }

        private static void ProcessListContents(GSUIList parentList, GSUI group)
        {
            var config = (GSUIGroupConfig)group.Data;
            foreach (var option in config.options) CreateUIElement(option, parentList);
        }


        /// Iterate through all the plugins that have elements to add to the UI, add them,// then add their postfixes to the event listener
        private static void AddGeneratorPluginUIElements(RectTransform canvas, int genIndex)
        {
            // GS2.Log("AddGeneratorPluginUIElements: " + GS2.Generators[genIndex].Name);
            var generatorPluginOption = generatorPluginOptions[genIndex];
            // GS2.Log(GS2.Generators[genIndex].Name + " option count = " + generatorPluginOption.Count);
            var list = canvas.GetComponentInChildren<GSUIPanel>().contents;
            for (var i = 0; i < generatorPluginOption.Count; i++)
            {
                var option = generatorPluginOption[i];
                CreateUIElement(option, list);
            }
        }

        private static void CreateUIElement(GSUI option, GSUIList list)
        {
            switch (option.Type)
            {
                case "Group":
                    if (option.callback == null)
                    {
                        // GS2.Log($"Adding normal list {option.Label}");
                        var newlist = list.AddList();
                        option.RectTransform = newlist.GetComponent<RectTransform>();
                        newlist.Initialize(option);
                        ProcessListContents(newlist, option);
                    }
                    else
                    {
                        // GS2.Log($"Adding toggle list {option.Label}");
                        var newlist = list.AddToggleList();
                        option.RectTransform = newlist.GetComponent<RectTransform>();
                        newlist.Initialize(option);
                        ProcessListContents(newlist, option);
                    }

                    break;
                case "Combobox":
                    var dropdown = list.AddDropdown();
                    option.RectTransform = dropdown.GetComponent<RectTransform>();
                    dropdown.initialize(option);
                    break;
                case "Selector":
                    var selector = list.AddSelector();
                    option.RectTransform = selector.GetComponent<RectTransform>();
                    selector.initialize(option);
                    break;
                case "Input":
                    var input = list.AddInput();
                    option.RectTransform = input.GetComponent<RectTransform>();
                    input.initialize(option);
                    break;
                case "Button":
                    var button = list.AddButton();
                    option.RectTransform = button.GetComponent<RectTransform>();
                    button.initialize(option);
                    break;
                case "Checkbox":
                    var toggle = list.AddToggle();
                    option.RectTransform = toggle.GetComponent<RectTransform>();
                    toggle.initialize(option);
                    break;
                case "Slider":
                    var slider = list.AddSlider();
                    option.RectTransform = slider.GetComponent<RectTransform>();
                    slider.initialize(option);
                    break;
                case "RangeSlider":
                    var rslider = list.AddRangeSlider();
                    option.RectTransform = rslider.GetComponent<RectTransform>();
                    rslider.initialize(option);
                    break;
                case "Header":
                    var header = list.AddHeader();
                    option.RectTransform = header.GetComponent<RectTransform>();
                    header.initialize(option);
                    break;
                case "Spacer":
                    var spacer = list.AddSpacer();
                    option.RectTransform = spacer;
                    break;
                case "Separator":
                    var separator = list.AddSeparator();
                    option.RectTransform = separator;
                    break;
                default:
                    Warn($"Couldn't create option {option.Label}");
                    break;
            }

            if (option.postfix != null) OptionsUIPostfix.AddListener(new UnityAction(option.postfix));
        }

        public static void GalacticScaleTabClick()
        {
            UIRoot.instance.optionWindow.SetTabIndex(MainTabIndex, false);
            details?.gameObject?.SetActive(true);
        }

        public static void DisableDetails()
        {
            details?.gameObject?.SetActive(false);
        }
    }
}