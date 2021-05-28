
using BepInEx;
using FullSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static ThemeLibrary ThemeLibrary = ThemeLibrary.Vanilla();
        public static TerrainAlgorithmLibrary TerrainAlgorithmLibrary = TerrainAlgorithmLibrary.Init();
        public static VeinAlgorithmLibrary VeinAlgorithmLibrary = VeinAlgorithmLibrary.Init();
        public static VegeAlgorithmLibrary VegeAlgorithmLibrary = VegeAlgorithmLibrary.Init();

        public static List<VectorLF3> tmp_poses;
        public static List<VectorLF3> tmp_drunk;
        public static int[] tmp_state;
        public static GalaxyData galaxy;
        public static Random random { get => new Random(GSSettings.Seed); }
        public static GameDesc gameDesc;
        public static string DataDir = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"),"config");
        public static bool Vanilla { get => generator.GUID == "space.customizing.generators.vanilla"; }
        public static Dictionary<int, GSPlanet> gsPlanets = new Dictionary<int, GSPlanet>();
        public static bool minifyJSON = false;



        public static void GenerateGalaxy()
        {
            Log("Start");
            if (GSSettings.Instance.imported)
            {
                Log("Settings Loaded From Save File");
                return;
            }
            GSSettings.Reset(GSSettings.Seed);
            gsPlanets.Clear();
            Log("Loading Data from Generator : " + generator.Name);
            generator.Generate(gameDesc.starCount);
            Log("End");
            return;
        }
        



        public static void Init()
        {
            LoadPreferences(true);
            Log("Start"+debugOn.ToString());
            List<GSTheme> themes = new List<GSTheme>();
            Log("GalacticScale2|Creating List of Themes");
            foreach (KeyValuePair<string, GSTheme> t in ThemeLibrary) themes.Add(t.Value);
            Log("GalacticScale2|Init|Processing Themes");
            for (var i = 0; i < themes.Count; i++)
            {
                themes[i].Process();
            }
            Log("End");
        }
          
        


        public static void LoadPlugins()
        {
            Log("Start");
            foreach (string filePath in Directory.GetFiles(Path.Combine(DataDir, "Generators")))
            {
                Log(filePath);
                foreach (Type type in Assembly.LoadFrom(filePath).GetTypes())
                    foreach (Type t in type.GetInterfaces())
                        if (t.Name == "iGenerator" && !type.IsAbstract && !type.IsInterface)
                            generators.Add((iGenerator)Activator.CreateInstance(type));
            }
            foreach (iGenerator g in generators)
            {
                Log("GalacticScale2|LoadPlugins|Loading Generator:" + g.Name);
                g.Init();
            }
            Log("End");
        }

    }
}

