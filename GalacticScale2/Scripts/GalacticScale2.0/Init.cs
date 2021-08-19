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
        public static string Version = "2.0.0a50";
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        private static readonly string OldDataDir = Path.Combine(AssemblyPath, "config");
        public static readonly string DataDir = Path.Combine(Paths.ConfigPath, "GalacticScale2");
        public static bool Failed = false;
        public static string updateMessage = "";
        public static bool Initialized = false;
        public static Dictionary<string, ThemeLibrary> availableExternalThemes = new Dictionary<string, ThemeLibrary>();
        public static bool canvasOverlay = false;
        public static bool ResearchUnlocked = false;
        public static Image splashImage;

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
        private static AssetBundle bundle;

        public static bool MenuHasLoaded;
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
                    UIMessageBox.Show("Error",
                        "Asset Bundle not found. \r\nPlease ensure your directory structure is correct.\r\n Installation instructions can be found at http://customizing.space/release. \r\nAn error log has been generated in the plugin/ErrorLog Directory".Translate(),
                        "Return".Translate(), 0);

                    return null;
                }

                return bundle;
            }
        }

        public static void Init()

        {
            if (File.Exists(Path.Combine(AssemblyPath, "icon.png")))
            {
                updateMessage +=
                    "Update Detected. Please do not save over existing saves \r\nuntil you are sure you can load saves saved with this version!\r\nPlease note the settings panel is under construction, and missing options will reappear in future updates\r\nPlease Click GS2 Help and click the link to join our community on discord for preview builds and to help shape the mod going forward".Translate();
                File.Delete(Path.Combine(AssemblyPath, "icon.png"));
                updateMessage += "\r\nPLEASE NOTE: The 2.1 update will include changes to the planet grid system to bring 200 radius planets in line with vanilla. THIS WILL BREAK SAVES. Do not update if you have a current game.";
            }
            Warn("Start");
            NebulaCompatibility.Init();
            if (Directory.Exists(OldDataDir) && !Directory.Exists(DataDir))
            {
                Warn($"Moving Configs from {OldDataDir} to {DataDir}");
                Directory.Move(OldDataDir, DataDir);
                updateMessage +=
                    "Galactic Scale config Directory has changed to \r\n ...\\BepInEx\\config\\GalacticScale \r\nThis is to prevent data being lost when updating using the mod manager.\r\n".Translate();
            }

            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            Config.Init();

            LoadPreferences(true);
            var themes = ThemeLibrary.Select(t => t.Value).ToList();
            foreach (var t in themes) t.Process();
            LoadPlugins();
            LoadPreferences();
            Log("End");
        }

        public static void OnMenuLoaded()
        {
            if (MenuHasLoaded) return;
            MenuHasLoaded = true;
            Log("Loading External Themes");
            LoadExternalThemes(Path.Combine(DataDir, "CustomThemes"));
            ExternalThemeProcessor.LoadEnabledThemes();
            Config.InitThemePanel();
            if (Config.Dev) DumpObjectToJson(Path.Combine(DataDir, "ldbthemes.json"), LDB._themes.dataArray);
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

            if (updateMessage != "")
            {
                UIMessageBox.Show("Update Information", updateMessage, "Noted!", 0);
                updateMessage = "";
            }
        }
    }
}