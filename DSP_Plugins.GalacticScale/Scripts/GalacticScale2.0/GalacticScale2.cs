
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
        public static bool debugOn = false;
        public static GSUI DebugLogOption;
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
            Log("Start");
            if (!CheckJsonFileExists(path)) return false;
            Log("Path = " + path);
            fsSerializer serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            Log("Initializing ThemeLibrary");
            GSSettings.ThemeLibrary = ThemeLibrary.Vanilla();
            Log("Reading JSON");
            string json = File.ReadAllText(path);
            //Log(json);
            GSSettings result = GSSettings.Instance;
            Log("Parsing JSON");
            fsData data = fsJsonParser.Parse(json);
            //LogJson(data2);
            Log("Trying To Deserialize JSON");
            serializer.TryDeserialize(data, ref result);
            Log("Setting GSSettings.Instance");
            //GSSettings.Instance = result;
            Log("End");
            return true;

        }
        public static void DebugLogOptionCallback(object o)
        {
            debugOn = (bool)o;
        }
        public static void DebugLogOptionPostfix()
        {
            DebugLogOption.Set(debugOn);
        }
        public static GSPlanet GetGSPlanet(PlanetData planet)
        {
            return gsPlanets[planet.id];
        }
        private static bool CheckJsonFileExists(string path)
        {
            Log("Checking if Json File Exists");
            if (File.Exists(path)) return true;
            Log("Json file does not exist at " + path);
            return false;
        }
        public static void SaveSettingsToJson(string path)
        {
            Log("Saving Settings to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize<GSSettings>(GSSettings.Instance, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");

        }

        public static void DumpObjectToJson(string path, object obj)
        {
            Log("Dumping Object to " + path);
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(obj, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(path, json);
            Log("End");
        }
        public static void Log(string s)
        {
            if (debugOn) Bootstrap.Debug(GetCaller() +s);
        }
        public static void Error(string message)
        {
            Bootstrap.Debug(GetCaller()+message, BepInEx.Logging.LogLevel.Error, true);
        }
        public static void Warn(string message)
        {
            Bootstrap.Debug(GetCaller() + message, BepInEx.Logging.LogLevel.Warning, true);
        }
        public static void LogJson(object o)
        {
            if (!debugOn) return;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(o, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            Bootstrap.Debug(GetCaller() + json);
        }
        public static string GetCaller()
        {
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(2).GetMethod().Name;
            string[] classPath = stackTrace.GetFrame(2).GetMethod().ReflectedType.ToString().Split('.');
            string className = classPath[classPath.Length - 1];
            if (methodName == ".ctor") methodName = "<Constructor>";
            return className+"|"+methodName+"|";
        }
        public static void Export(BinaryWriter w) // Export Settings to SaveGame
        {
            Log("Exporting to Save");
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            int length = json.Length;
            w.Write(GSSettings.Instance.version);
            w.Write(json);
            Log("Exported. Resetting GSSettings");
            GSSettings.Reset(GSSettings.Seed);
            Log("End");
        }
        public static void Import(BinaryReader r) // Load Settings from SaveGame
        {
            Log("Importing from Save");
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
            Log("End");
        }
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
        public static void SavePreferences()
        {
            Log("Start");
            GSPreferences preferences = new GSPreferences();
            preferences.GeneratorID = generator.GUID;
            preferences.debug = debugOn;
            Log("Set the GeneratorID, now trying to get the plugin prefs");
            foreach (iGenerator g in generators)
            {
                if (g is iConfigurableGenerator)
                {
                    iConfigurableGenerator gen = g as iConfigurableGenerator;
                    Log("trying to get prefs for " + gen.Name);
                    GSGenPreferences prefs = gen.Export();
                    Log(gen.Name + " has supplied preferences");
                    preferences.PluginOptions[gen.GUID] = prefs;
                    Log("Finished adding preferences to GS preferences object for " + gen.Name);
                }
            }
            fsSerializer serializer = new fsSerializer();
            Log("Trying to serialize preferences object");
            serializer.TrySerialize(preferences, out fsData data);
            Log("Serialized");
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
            Log("End");
        }


        private class GSPreferences
        {
            public bool debug = false;
            public string GeneratorID = "space.customizing.vanilla";
            public Dictionary<string, GSGenPreferences> PluginOptions = new Dictionary<string, GSGenPreferences>();
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
          
        public static void LoadPreferences(bool debug = false)
        {
            Log("Start");
            string path = Path.Combine(DataDir, "Preferences.json");
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            if (!CheckJsonFileExists(path)) return;
            Log("Loading Preferences from " + path);
            fsSerializer serializer = new fsSerializer();
            string json = File.ReadAllText(path);
            GSPreferences preferences = new GSPreferences();
            Log("LoadPreferences Initial " + preferences.GeneratorID);
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<GSPreferences>(data2, ref preferences);
            Log("LoadPreferences Result " + preferences.GeneratorID);
            if (!debug) ParsePreferences(preferences);
            else
            {
                debugOn = preferences.debug;
            }
            Log("End");
        }
        private static void ParsePreferences(GSPreferences p)
        {
            Log("Start");
            debugOn = p.debug;
            generator = GetGeneratorByID(p.GeneratorID);
            if (p.PluginOptions != null)
            {
                foreach (KeyValuePair<string, GSGenPreferences> pluginOptions in p.PluginOptions)
                {
                    Log("Plugin Options for " + pluginOptions.Key + "found");
                    iConfigurableGenerator gen = GetGeneratorByID(pluginOptions.Key) as iConfigurableGenerator;
                    if (gen != null)
                    {
                        Log(gen.Name + " preferences exported");
                        gen.Import(pluginOptions.Value);
                    }
                }
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

