using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static string Version;
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        private static readonly string OldDataDir = Path.Combine(AssemblyPath, "config");
        public static readonly string DataDir = Path.Combine(Paths.ConfigPath, "GalacticScale2");
        public static bool Failed = false;
        public static string updateMessage = "";
        public static bool ModellingDone = true;
        public static Dictionary<string, ThemeLibrary> availableExternalThemes = new();
        public static bool canvasOverlay = false;
        public static Image splashImage;
        public static bool SaveOrLoadWindowOpen = false;
        public static bool Initialized = false;
        public static bool MenuHasLoaded;

        //Temp until GS2Cheats workaround
        public static bool ResearchUnlocked = false;

        public static TeleportComponent TP;
        public static InputComponent InputComponent;

        public static TerrainAlgorithmLibrary TerrainAlgorithmLibrary = TerrainAlgorithmLibrary.Init();
        public static VeinAlgorithmLibrary VeinAlgorithmLibrary = VeinAlgorithmLibrary.Init();
        public static VegeAlgorithmLibrary VegeAlgorithmLibrary = VegeAlgorithmLibrary.Init();
        public static GS2MainSettings Config = new();

        public static GalaxyData galaxy;

        public static GameDesc gameDesc;
        public static Dictionary<int, GSPlanet> gsPlanets = new();
        public static Dictionary<int, GSStar> gsStars = new();
        private static AssetBundle bundle;

        //Nebula 
        private static Button.ButtonClickedEvent origHost;
        private static Button.ButtonClickedEvent origLoad;

        public static bool IsMenuDemo => DSPGame.IsMenuDemo || !Initialized;
        public static bool Vanilla => ActiveGenerator.GUID == "space.customizing.generators.vanilla";
        public static AssetBundle Bundle
        {
            get
            {
                if (bundle == null)
                {
                    var path = Path.Combine(AssemblyPath, "galacticbundle");
                    var path2 = Path.Combine(AssemblyPath, "galactic.bundle");
                    if (File.Exists(path)) bundle = AssetBundle.LoadFromFile(path);
                    else bundle = AssetBundle.LoadFromFile(path2);
                    // foreach (var name in _bundle.GetAllAssetNames()) GS2.Warn("Bundle Contents:" + name);
                }
                if (bundle == null)
                {
                    Error("Failed to load AssetBundle!".Translate());
                    UIMessageBox.Show("Error", "Asset Bundle not found. \r\nPlease ensure your directory structure is correct.\r\n Installation instructions can be found at http://customizing.space/release. \r\nAn error log has been generated in the plugin/ErrorLog Directory".Translate(), "Return".Translate(), 0);
                    return null;
                }
                return bundle;
            }
        }

        public static void UpdateNebulaSettings()
        {
            var hostButton = GameObject.Find("UI Root/Overlay Canvas/Main Menu/multiplayer-menu/button-multiplayer/");
            var loadButton = GameObject.Find("UI Root/Overlay Canvas/Main Menu/multiplayer-menu/button-new/");
            if (hostButton != null)
            {
                if (ActiveGenerator.GUID == "space.customizing.generators.vanilla")
                {
                    var button = hostButton.GetComponent<Button>();
                    var button2 = loadButton.GetComponent<Button>();
                    // button.enabled = false;
                    origHost = button.onClick;
                    origLoad = button2.onClick;
                    button.onClick = new Button.ButtonClickedEvent();
                    button2.onClick = new Button.ButtonClickedEvent();
                    button.onClick.AddListener(() => ShowMessage("Cannot Host a GS2 Game using Vanilla Generator", "Warning", "OK".Translate()));
                    button2.onClick.AddListener(() => ShowMessage("Cannot Host a GS2 Game using Vanilla Generator", "Warning", "OK".Translate()));
                }
                else
                {
                    var button = hostButton.GetComponent<Button>();
                    var button2 = loadButton.GetComponent<Button>();
                    // button.enabled =  true;
                    if (origHost != null) button.onClick = origHost;
                    if (origLoad != null) button2.onClick = origLoad;
                }
            }
        }

        public static void Init()

        {
            Log($"Vanilla Theme Count: {LDB._themes.dataArray.Length.ToString()}");
            if (File.Exists(Path.Combine(AssemblyPath, "icon.png")))
            {
                if (ActiveGenerator != null && ActiveGenerator.GUID == "space.customizing.generators.vanilla") updateMessage += "Note: Settings for this mod are in the settings menu. Make sure to change the Generator to get the full Galactic Scale experience.\r\n";
                updateMessage += "Update Detected. Please do not save over existing saves \r\nuntil you are sure you can load saves saved with this version!\r\nPlease Click GS2 Help and click the link to join our community on discord for preview builds and to help shape the mod going forward".Translate();
                File.Delete(Path.Combine(AssemblyPath, "icon.png"));
                updateMessage += "The latest DSP update has added additional planet themes which are yet to be included in GS2. \r\nI'm working on getting them added to the GS2 themeset, as well as implementing their new subtheme system";
            }

            // Warn("Step2");
            if (Directory.Exists(OldDataDir) && !Directory.Exists(DataDir))
            {
                Warn($"Moving Configs from {OldDataDir} to {DataDir}");
                Directory.Move(OldDataDir, DataDir);
                updateMessage += "Galactic Scale config Directory has changed to \r\n ...\\BepInEx\\config\\GalacticScale \r\nThis is to prevent data being lost when updating using the mod manager.\r\n".Translate();
            }

            // Warn("Step3");
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            // Warn("Step4");
            CleanErrorLogs();

            Config.Init();
            // Warn("Step5");
            LoadPreferences(true);
            // Warn("Step6");
            var themes = GSSettings.ThemeLibrary.Select(t => t.Value).ToList();
            // Warn("Step7");
            foreach (var t in themes) t.Process();
            // Warn("Step8");
            LoadGenerators();
            // Warn("Step9");
            LoadPreferences();
            // Log("End");
        }

        public static void ShowMessage(string message, string title = "Galactic Scale", string button = "OK")
        {
            UIMessageBox.Show(title.Translate(), message.Translate(), button.Translate(), 0);
        }

        public static void OnMenuLoaded()
        {
            // Warn("Start");
            if (MenuHasLoaded) return;
            MenuHasLoaded = true;
            Log("Loading External Themes");
            LoadExternalThemes(Path.Combine(DataDir, "CustomThemes"));
            // Warn("Step1");
            ExternalThemeProcessor.LoadEnabledThemes();
            // Warn("Step2");
            Config.InitThemePanel();
            // Warn("Step3");
            if (Config.Dev) DumpObjectToJson(Path.Combine(DataDir, "ldbthemes.json"), LDB._themes.dataArray);
            // Warn("Step4");
            LDB._themes.Select(1);
            // Warn("Step5");
            if (Config.Dev) Utils.DumpProtosToCSharp();
            // Warn("Step6");
            if (Config.Dev)
            {
                var da = LDB._veges.dataArray;
                var vegeDict = new Dictionary<int, string>();
                foreach (var vegeProto in da)
                {
                    var name = vegeProto.Name;
                    var name2 = vegeProto.name;
                    var id = vegeProto.ID;
                    vegeDict.Add(id, name.Translate() + " " + name2.Translate());
                }

                DumpObjectToJson(Path.Combine(DataDir, "ldbvege.json"), vegeDict);
            }

            // Warn("Step7");
            if (updateMessage != "")
            {
                UIMessageBox.Show("Update Information", updateMessage, "Noted!", 0);
                updateMessage = "";
            }

            // Warn("Step8");
            UpdateNebulaSettings();
            // Warn("Step9");
        }
    }
}