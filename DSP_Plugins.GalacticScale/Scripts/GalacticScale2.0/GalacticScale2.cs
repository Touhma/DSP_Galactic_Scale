
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
        public static bool LoadSettingsFromJson(string path)
        {
            if (!CheckJsonFileExists(path)) return false;
            //Log("GalacticScale2|LoadSettingsFromJson|path=" + path);
            fsSerializer serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            string json = File.ReadAllText(path);
            GSSettings result = GSSettings.Instance;
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            fsData data2 = fsJsonParser.Parse(json);
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            serializer.TryDeserialize<GSSettings>(data2, ref result);
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            GSSettings.Instance = result;
            //Log("GalacticScale2|LoadSettingsFromJson|ThemeLibrary.Count=" + ThemeLibrary.Count);
            return true;

        }
        public static GSPlanet GetGSPlanet(PlanetData planet)
        {
            return gsPlanets[planet.id];
        }
        private static bool CheckJsonFileExists(string path)
        {
            //Log("Checking if Json File Exists");
            if (File.Exists(path)) return true;
            //Log("Json file does not exist at " + path);
            return false;
        }
        public static void SaveSettingsToJson(string path)
        {
            //Log("Saving Settings to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize<GSSettings>(GSSettings.Instance, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);

        }
        public static void LogJson(object o)
        {
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(o, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            Log(json);
        }
        public static void DumpObjectToJson(string path, object obj)
        {
            Log("Dumping Object to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(obj, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
        }
        public static void Log(string s)
        {
            
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            // Get calling method name
            Bootstrap.Debug(stackTrace.GetFrame(1).GetMethod().Name);
            Bootstrap.Debug(s);
        }
        public static void Export(BinaryWriter w) // Export Settings to SaveGame
        {
            //Log("()()()Exporting to Save");
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            int length = json.Length;
            w.Write(GSSettings.Instance.version);
            w.Write(json);
            //Log("()()()Exported");
            GSSettings.Reset(GSSettings.Seed);

        }
        public static void Import(BinaryReader r) // Load Settings from SaveGame
        {
            //Log("()()()Importing from Save");
            GSSettings.Stars.Clear();
            fsSerializer serializer = new fsSerializer();
            string version = r.ReadString();
            string json = r.ReadString();
            GSSettings result = GSSettings.Instance;
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<GSSettings>(data2, ref result);
            if (version != GSSettings.Instance.version)
            {
                Log("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
            }
            GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            //Log("()()()Imported");
        }
        public static void GenerateGalaxy()
        {
            if (GSSettings.Instance.imported)
            {
                //Log("Settings Loaded From Save File");
                return;
            }
            GSSettings.Reset(GSSettings.Seed);
            gsPlanets.Clear();
            //Log("Loading Data from Generator : " + generator.Name);
            generator.Generate(gameDesc.starCount);
            return;
        }
        public static void SavePreferences()
        {
            //Log("Saving Preferences");
            Preferences preferences = new Preferences();
            preferences.GeneratorID = generator.GUID;
            //Log("Set the GeneratorID, now trying to get the plugin prefs");
            foreach (iGenerator g in generators)
            {
                if (g is iConfigurableGenerator)
                {
                    iConfigurableGenerator gen = g as iConfigurableGenerator;
                    //Log("trying to get prefs for " + gen.Name);
                    GSGenPreferences prefs = gen.Export();
                    //Log(gen.Name + " has supplied preferences");
                    preferences.PluginOptions[gen.GUID] = prefs;
                    //Log("Finished adding preferences to GS preferences object for " + gen.Name);
                }
            }
            fsSerializer serializer = new fsSerializer();
            //Log("Trying to serialize preferences object");
            serializer.TrySerialize(preferences, out fsData data);
            //Log("serialized");
            string json = fsJsonPrinter.PrettyJson(data);
            //Log(json);
            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
        }


        private class Preferences
        {
            public string GeneratorID = "space.customizing.vanilla";
            public Dictionary<string, GSGenPreferences> PluginOptions = new Dictionary<string, GSGenPreferences>();
        }
        public static void Init()
        {
            //Log("GalacticScale2|Init");
            //Log("GalacticScale2|Init|ThemeLibrary.Count=" + ThemeLibrary.Count);
            List<GSTheme> themes = new List<GSTheme>();
            //Log("GalacticScale2|Creating List of Themes");
            foreach (KeyValuePair<string, GSTheme> t in ThemeLibrary) themes.Add(t.Value);
            //Log("GalacticScale2|Init|Processing Themes");
            for (var i = 0; i < themes.Count; i++)
            {
                themes[i].Process();
            }
            //Log("GalacticScale2|Init->End");
        }
          
        public static void LoadPreferences()
        {
            //Log("GalacticScale2|LoadPreferences");
            string path = Path.Combine(DataDir, "Preferences.json");
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            if (!CheckJsonFileExists(path)) return;
            //Log("Loading Preferences from " + path);
            fsSerializer serializer = new fsSerializer();
            string json = File.ReadAllText(path);
            Preferences result = new Preferences();
            //Log("LoadPreferences Initial " + result.GeneratorID);
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<Preferences>(data2, ref result);
            //Log("LoadPreferences Result " + result.GeneratorID);
            ParsePreferences(result);
        }
        private static void ParsePreferences(Preferences p)
        {
            //Log("GalacticScale2|ParsePreferences");
            generator = GetGeneratorByID(p.GeneratorID);
            if (p.PluginOptions != null)
            {
                foreach (KeyValuePair<string, GSGenPreferences> pluginOptions in p.PluginOptions)
                {
                    //Log("Plugin Options for " + pluginOptions.Key + "found");
                    iConfigurableGenerator gen = GetGeneratorByID(pluginOptions.Key) as iConfigurableGenerator;
                    if (gen != null)
                    {
                        //Log(gen.Name + " preferences exported");
                        gen.Import(pluginOptions.Value);
                    }
                }
            }
        }

        public static void LoadPlugins()
        {
            //Log("GalacticScale2|LoadPlugins");
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
                //Log("GalacticScale2|LoadPlugins|Loading Generator:" + g.Name);
                g.Init();
            }
        }
        public static void Error(string message)
        {
            Bootstrap.Debug(message, BepInEx.Logging.LogLevel.Error, true);
        }
        public static void Warn(string message)
        {
            Bootstrap.Debug(message, BepInEx.Logging.LogLevel.Warning, true);
        }
    }
}

