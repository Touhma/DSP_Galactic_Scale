
using BepInEx;
using FullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static List<VectorLF3> tmp_poses;
        public static List<VectorLF3> tmp_drunk;
        public static int[] tmp_state;
        public static GalaxyData galaxy;
        public static System.Random random;
        public static GameDesc gameDesc;
        public static string DataDir = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"),"config");
        public static bool Vanilla { get => generator.GUID == "space.customizing.generators.vanilla"; }
        public static Dictionary<string, GSTheme> planetThemes = new Dictionary<string, GSTheme>()
        {
            ["Mediterranian"] = new GSTheme()
            {
                name = "Mediterranian",
                type = EPlanetType.Ocean,
                theme = LDB.themes.Select(1),
            },
        };
        public delegate void GSOptionCallback(object o);

        public class GSOption
        {
            public string label;
            public string type;
            public object data;
            public GSOptionCallback callback;
            public string tip;
            public UnityEngine.RectTransform rectTransform;
            public UnityAction postfix;
            public GSOption(string _label, string _type, object _data, GSOptionCallback _callback, UnityAction postfix, string _tip = "")
            {
                this.label = _label;
                this.type = _type;
                this.data = _data;
                this.callback = _callback;
                if (postfix == null) this.postfix = new UnityAction(() => { });
                else this.postfix = postfix;
                this.tip = _tip;
            }
        }
       
        public static bool LoadSettingsFromJson(string path)
        {
            if (!CheckJsonFileExists(path)) return false;
            Log("Loading Settings from " + path);
            fsSerializer serializer = new fsSerializer();
            GSSettings.Stars.Clear();
            string json = File.ReadAllText(path);
            GSSettings result = GSSettings.Instance;
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<GSSettings>(data2, ref result).AssertSuccessWithoutWarnings();
            GSSettings.Instance = result;
            return true;

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
            Patch.Debug(s);
        }
        public static void Export(BinaryWriter w) // Export Settings to SaveGame
        {
            Log("()()()Exporting to Save");
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            int length = json.Length;
            w.Write(GSSettings.Instance.version);
            w.Write(json);
            Log("()()()Exported");
            GSSettings.Reset();

        }
        public static void Import(BinaryReader r) // Load Settings from SaveGame
        {
            Log("()()()Importing from Save");
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
            Log("()()()Imported");
        }
        public static void LoadSettings()
        {
            if (GSSettings.Instance.imported)
            {
                Log("Settings Loaded From Save File");
                return;
            }
            Log("Loading Data from Generator : " + generator.Name);
            generator.Generate(gameDesc.starCount);
            return;
        }
        public static void SavePreferences()
        {
            Log("Saving Preferences");
            GSPreferences preferences = new GSPreferences();
            preferences.GeneratorID = generator.GUID;
            Log("Set the GeneratorID, now trying to get the plugin prefs");
            foreach (iGenerator g in generators)
            {
                if (g is iConfigurableGenerator)
                {
                    iConfigurableGenerator gen = g as iConfigurableGenerator;
                    Log("trying to get prefs for " + gen.Name);
                    object prefs = gen.Export();
                    Log(gen.Name + " has exported its prefs");
                    preferences.PluginOptions[gen.GUID] = prefs;
                    Log("Finished for " + gen.Name);
                }
            }
            fsSerializer serializer = new fsSerializer();
            Log("Trying to serialize");
            serializer.TrySerialize(preferences, out fsData data);
            Log("serialized");
            string json = fsJsonPrinter.PrettyJson(data);
            Log(json);
            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
        }
        public class GSPreferences
        {
            public string GeneratorID = "space.customizing.vanilla";
            public Dictionary<string, object> PluginOptions = new Dictionary<string, object>();
        }
        public static void LoadPreferences()
        {
            string path = Path.Combine(DataDir, "Preferences.json");
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            if (!CheckJsonFileExists(path)) return;
            Log("Loading Preferences from " + path);
            fsSerializer serializer = new fsSerializer();
            string json = File.ReadAllText(path);
            GSPreferences result = new GSPreferences();
            Log("LoadPreferences Initial " + result.GeneratorID);
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<GSPreferences>(data2, ref result).AssertSuccessWithoutWarnings();
            Log("LoadPreferences Result " + result.GeneratorID);
            ParsePreferences(result);
        }
        public static void ParsePreferences(GSPreferences p)
        {
            Log("Parsing Preferences");
            generator = GetGeneratorByID(p.GeneratorID);
            if (p.PluginOptions != null)
            {
                foreach (KeyValuePair<string, object> pluginOptions in p.PluginOptions)
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
        }
        public static iGenerator GetGeneratorByID(string guid)
        {
            foreach (iGenerator g in generators)
            {
                if (g.GUID == guid) return g;
            }
            return new Generators.Dummy();
        }
        public static int GetCurrentGeneratorIndex()
        {
            for (var i = 0; i < generators.Count; i++)
            {
                if (generators[i] == generator) return i;
            }
            return -1;
        }
        public static void LoadPlugins()
        {
            Log("Loading Plugins...");
            foreach (iGenerator g in generators)
            {
                Log("Loading Generator: " + g.Name);
                g.Init();
            }
        }
    }
}

