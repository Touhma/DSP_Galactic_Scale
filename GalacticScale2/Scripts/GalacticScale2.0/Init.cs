using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static string Version = "2.0.0a50";
        public static string AssemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location);
        public static string DataDir = Path.Combine(AssemblyPath, "config");
        public static bool Failed = false;
        public static bool Initialized = false;
        public static bool CheatMode = false;
        public static bool ResearchUnlocked = false;
        public static bool minifyJSON = false;
        public static ThemeLibrary ThemeLibrary = ThemeLibrary.Vanilla();
        public static TerrainAlgorithmLibrary TerrainAlgorithmLibrary = TerrainAlgorithmLibrary.Init();
        public static VeinAlgorithmLibrary VeinAlgorithmLibrary = VeinAlgorithmLibrary.Init();
        public static VegeAlgorithmLibrary VegeAlgorithmLibrary = VegeAlgorithmLibrary.Init();
        public static GS2MainSettings mainSettings = new GS2MainSettings();

        //public static int[] tmp_state;
        public static GalaxyData galaxy;

        //private static Random random;
        public static GameDesc gameDesc;
        public static Dictionary<int, GSPlanet> gsPlanets = new Dictionary<int, GSPlanet>();
        public static Dictionary<int, GSStar> gsStars = new Dictionary<int, GSStar>();
        private static AssetBundle _bundle;

        public static bool IsMenuDemo
        {
            get
            {
                if (DSPGame.IsMenuDemo) return true;

                if (!Initialized) return true;

                return false;
            }
        }

        public static bool Vanilla => generator.GUID == "space.customizing.generators.vanilla";

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
            NebulaCompatibility.Init();
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            mainSettings.Init();
            LoadPreferences(true);
            Log("Start" + debugOn);
            var themes = new List<GSTheme>();
            Log("GalacticScale2|Creating List of Themes");
            foreach (var t in ThemeLibrary) themes.Add(t.Value);

            Log("GalacticScale2|Init|Processing Themes");
            for (var i = 0; i < themes.Count; i++) themes[i].Process();
            LoadPlugins();
            LoadPreferences();
            Log("End");
        }
    }
}