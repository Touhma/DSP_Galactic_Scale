using System.Collections.Generic;
using System.Reflection;
using System.IO;
using GalacticScale.UI.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GalacticScale.Editor
{
    public class StarEditor : MonoBehaviour , iConfigurablePlugin
    {
        public static StarEditor instance;
        public static bool Initialized = false;
        public static Button resetButton;
        public static Button saveButton;
        public static RectTransform mainPanel;
        public static GSUIPanel settingsPanel;
        public static void OpenMainPanel()
        {
            
        }
        private static readonly Dictionary<string, GSUI> UI = new();
        public static GSOptions options = new GSOptions();
        public static GSGenPreferences preferences = new GSGenPreferences();
        public static void CreateMainPanel(GSStar star)
        {
            if (Initialized)
            {
                editStar = star;
                UpdateEditor();
                return;
            }

            editStar = star;
            var details = UIRoot.instance.galaxySelect.gameObject.GetComponent<RectTransform>();
            var gsuipath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location), "GSUI.dll");
            Assembly.LoadFrom(gsuipath);
            var gsp = GS2.Bundle.LoadAsset<GameObject>("assets/gssettingspanel.prefab");
            mainPanel = Object.Instantiate(gsp, details, false).GetComponent<RectTransform>();
            mainPanel.GetComponent<ScrollRect>().scrollSensitivity = 10;
            settingsPanel = mainPanel.GetComponentInChildren<GSUIPanel>();
            mainPanel.gameObject.SetActive(true);
            var bgimg = mainPanel.gameObject.AddComponent<Image>();
            bgimg.color = new Color(0, 0, 0, 0.75f);
            settingsPanel.gameObject.SetActive(true);
            mainPanel.pivot = new Vector2(0, 0.5f);
            mainPanel.anchorMax = new Vector2(0, 1);
            mainPanel.anchorMin = new Vector2(0, 0);
            mainPanel.sizeDelta = new Vector2(900f, -100f);
            mainPanel.anchoredPosition = new Vector2(50, 0);
            // mainPanel.anchoredPosition = new Vector2(50f, -120f);
            mainPanel.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            // Warn(mainPanel.sizeDelta.ToString());
            // mainPanel.sizeDelta = new Vector2(10f, 10f);
            // mainPanel.SetHeight(500f);
            // mainPanel.SetWidth(500f);
            // RectTransform obj = StarEditor.mainPanel;
            options.Add(GSUI.Header("Star Editor"));
            
            UI.Add("starName", options.Add(GSUI.Input("Name", star.Name, "starName", SetStarName)));
            for (var i = 0; i < options.Count; i++) // Main Settings
                // GS2.Warn($"Creating {options[i].Label}");
                SettingsUI.CreateUIElement(options[i], settingsPanel.contents);
            UpdateEditor();
            Initialized = true;
        }

        public static float width = 800;
        public static float height = 800;
        public static float y = 120;
        public static float x = 50;
        public static void ApplySizePosition()
        {
            mainPanel.SetLeftTopPosition(new Vector2(x,-1*y));
            mainPanel.SetWidth(width);
            mainPanel.SetHeight(height);
            
        }

        public string Name { get; } = "StarEditor";
        public string Author { get; } = "innominata";
        public string Description { get; } = "Editor for Stars";
        public string Version { get; } = "1";
        public string GUID { get; } = "StarEditor";
        public GSOptions Options { get; } = new();
        public bool Enabled { get; set; } = true;
        public static GSStar editStar;
        public void Awake()
        {
            GS2.Warn("Awake");
            instance = this;

        }

        public static void UpdateEditor()
        {
            UI["starName"]?.Set(editStar?.Name);
        }
        public static void SetStarName(Val o)
        {
            if (editStar != null)
            {
                editStar.Name = o;
                GSSettings.Instance.imported = true;
                GS2.ProcessGalaxy(GS2.gameDesc, true);
            }
        }

        public void Init()
        {
            GS2.Warn("StarEditor Init");
        }

        public void Import(GSGenPreferences preferences)
        {
            
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }

        public void OnUpdate(string key, Val val)
        {
            GS2.Warn($"Updating {key} {val}");
            preferences.Set(key, val);
        }
    }
}