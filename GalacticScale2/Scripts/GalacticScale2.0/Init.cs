using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
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

        public static bool Initialized = false;

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
            NebulaCompatibility.Init();
            if (Directory.Exists(OldDataDir)) Directory.Move(OldDataDir, DataDir);
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            Config.Init();
            LoadPreferences(true);
            Log("GalacticScale2|Creating List of Themes");
            var themes = ThemeLibrary.Select(t => t.Value).ToList();
            Log("GalacticScale2|Init|Processing Themes");
            foreach (var t in themes) t.Process();
            LoadPlugins();
            LoadPreferences();
            Log("End");
        }
    }
}