using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static string Version = "2.0.0a50";
        public static string DataDir = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "config");
        public static bool Failed = false;
        public static bool Initialized = false;
        public static bool CheatMode = false;
        public static bool ResearchUnlocked = false;
        public static bool IsMenuDemo
        {
            get
            {
                if (DSPGame.IsMenuDemo)
                {
                    return true;
                }

                if (!Initialized)
                {
                    return true;
                }

                return false;
            }
        }
        public static bool minifyJSON = false;
        public static ThemeLibrary ThemeLibrary = ThemeLibrary.Vanilla();
        public static TerrainAlgorithmLibrary TerrainAlgorithmLibrary = TerrainAlgorithmLibrary.Init();
        public static VeinAlgorithmLibrary VeinAlgorithmLibrary = VeinAlgorithmLibrary.Init();
        public static VegeAlgorithmLibrary VegeAlgorithmLibrary = VegeAlgorithmLibrary.Init();


        //public static int[] tmp_state;
        public static GalaxyData galaxy;
        //private static Random random;
        public static GameDesc gameDesc;

        public static bool Vanilla => generator.GUID == "space.customizing.generators.vanilla";
        public static Dictionary<int, GSPlanet> gsPlanets = new Dictionary<int, GSPlanet>();
        public static Dictionary<int, GSStar> gsStars = new Dictionary<int, GSStar>();
        private static UnityEngine.AssetBundle _bundle;
        public static UnityEngine.AssetBundle bundle
        {
            get
            {
                if (_bundle == null)
                {
                    _bundle = UnityEngine.AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location), "galacticbundle"));
                }

                if (_bundle == null)
                {
                    Error("Failed to load AssetBundle!");
                    UIMessageBox.Show("Error", "Asset Bundle not found. \r\nPlease ensure your directory structure is correct.\r\n Installation instructions can be found at http://customizing.space/release. \r\nAn error log has been generated in the plugin/ErrorLog Directory", "Return", 0);

                    return null;
                }
                //Warn("---Asset Bundle Contents---");
                //LogJson(_bundle.GetAllAssetNames());
                //Warn("---------------------------");
                return _bundle;
            }
        }

        public static void Init()
        {

            NebulaCompatibility.Init();
            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
            }

            LoadPreferences(true);
            Log("Start" + debugOn.ToString());
            List<GSTheme> themes = new List<GSTheme>();
            Log("GalacticScale2|Creating List of Themes");
            foreach (KeyValuePair<string, GSTheme> t in ThemeLibrary)
            {
                themes.Add(t.Value);
            }

            Log("GalacticScale2|Init|Processing Themes");
            for (var i = 0; i < themes.Count; i++)
            {
                themes[i].Process();
            }
            LoadPlugins();
            LoadPreferences();
            Log("End");
        }






    }
}

