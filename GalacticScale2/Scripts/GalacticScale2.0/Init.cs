using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using rail;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static string Version = "2.0.0a50";
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        private static readonly string OldDataDir = Path.Combine(AssemblyPath, "config");
        public static readonly string DataDir = Path.Combine(Paths.ConfigPath, "GalacticScale2");
        public static bool Failed = false;
        public static string updateMessage = "";
        public static bool Initialized = false;
        public static Dictionary<string,ThemeLibrary> availableExternalThemes = new Dictionary<string, ThemeLibrary>();
        public static bool canvasOverlay = false;
        public static ExternalThemeSelector themeSelector;
        // public static bool CheatMode = false;
        public static bool ResearchUnlocked = false;

        // public static bool MinifyJson = false;
        public static ThemeLibrary ThemeLibrary = ThemeLibrary.Vanilla();
        public static TerrainAlgorithmLibrary TerrainAlgorithmLibrary = TerrainAlgorithmLibrary.Init();
        public static VeinAlgorithmLibrary VeinAlgorithmLibrary = VeinAlgorithmLibrary.Init();
        public static VegeAlgorithmLibrary VegeAlgorithmLibrary = VegeAlgorithmLibrary.Init();
        public static GS2MainSettings Config = new GS2MainSettings();

        //public static int[] tmp_state;
        public static GalaxyData galaxy;

        //private static Random random;
        public static GameDesc gameDesc;
        public static Dictionary<int, GSPlanet> gsPlanets = new Dictionary<int, GSPlanet>();
        public static Dictionary<int, GSStar> gsStars = new Dictionary<int, GSStar>();
        private static AssetBundle _bundle;
        // public static AssetBundle bundle2;

        public static bool IsMenuDemo
        {
            get
            {
                if (DSPGame.IsMenuDemo) return true;

                if (!Initialized) return true;

                return false;
            }
        }

        public static bool Vanilla => ActiveGenerator.GUID == "space.customizing.generators.vanilla";

        public static AssetBundle bundle
        {
            get
            {
                if (_bundle == null)
                {
                    var path = Path.Combine(AssemblyPath, "galacticbundle");
                    var path2 = Path.Combine(AssemblyPath, "galactic.bundle");
                    if (File.Exists(path)) _bundle = AssetBundle.LoadFromFile(path);
                    else _bundle = AssetBundle.LoadFromFile(path2);
                }

                if (_bundle == null)
                {
                    Error("Failed to load AssetBundle!");
                    UIMessageBox.Show("Error",
                        "Asset Bundle not found. \r\nPlease ensure your directory structure is correct.\r\n Installation instructions can be found at http://customizing.space/release. \r\nAn error log has been generated in the plugin/ErrorLog Directory",
                        "Return", 0);

                    return null;
                }

                return _bundle;
            }
        }

        public static void Init()

        {
            GS2.Warn("Start");
            // if (bundle2 == null)
            // {
            //     var path = Path.Combine(AssemblyPath, "themeselector");
            //     if (File.Exists(path)) bundle2 = AssetBundle.LoadFromFile(path);
            //     GS2.Warn(" ");
            //     
            //     GS2.Warn(" ");
            // }
            NebulaCompatibility.Init();
            if (Directory.Exists(OldDataDir) && !Directory.Exists(DataDir))
            {
                GS2.Warn($"Moving Configs from {OldDataDir} to {DataDir}");
                Directory.Move(OldDataDir, DataDir);
                updateMessage += "Galactic Scale config Directory has changed to \r\n ...\\BepInEx\\config\\GalacticScale \r\nThis is to prevent data being lost when updating using the mod manager.\r\n";

            }
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            Config.Init();
            
            LoadPreferences(true);
            // LoadExternalThemes(Path.Combine(DataDir, "CustomThemes"));
            // ExternalThemeProcessor.LoadEnabledThemes();
            //LogJson(availableExternalThemes);
            Log("GalacticScale2|Creating List of Themes");
            var themes = ThemeLibrary.Select(t => t.Value).ToList();
            Log("GalacticScale2|Init|Processing Themes");
            foreach (var t in themes) t.Process();
            LoadPlugins();
            LoadPreferences();
            Log("End");
        }

        public static bool MenuHasLoaded = false;
        public static void OnMenuLoaded()
        {
            if (MenuHasLoaded) return;
            MenuHasLoaded = true;
            GS2.Log("Loading External Themes");
            LoadExternalThemes(Path.Combine(DataDir, "CustomThemes"));
            ExternalThemeProcessor.LoadEnabledThemes();
            if (Config.Dev) DumpObjectToJson(Path.Combine(DataDir, "ldbthemes.json"), LDB._themes.dataArray);
            if (Config.Dev)
            {
                var da = LDB._veges.dataArray;
                Dictionary<int, string> vegeDict = new Dictionary<int, string>();
                foreach (var vegeProto in da)
                {
                    string name = vegeProto.Name;
                    string name2 = vegeProto.name;
                    int id = vegeProto.ID;
                    vegeDict.Add(id, name.Translate() + " " + name2.Translate());
                }
                DumpObjectToJson(Path.Combine(DataDir, "ldbvege.json"), vegeDict);
            }
            if (updateMessage != "")
            {
                UIMessageBox.Show("Update Information", GS2.updateMessage, "Noted!", 0);
                updateMessage = "";
            }
        }
    }
}