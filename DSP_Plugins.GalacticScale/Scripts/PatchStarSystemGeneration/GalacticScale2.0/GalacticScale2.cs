
using BepInEx;
using FullSerializer;
using System.Collections.Generic;
using System.IO;
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
            public string category;
            public string label;
            public string type;
            public object data;
            public GSOptionCallback callback;
            public string tip;
            public GSOption(string _category, string _label, string _type, object _data, GSOptionCallback _callback, string _tip = "")
            {
                this.label = _label;
                this.type = _type;
                this.data = _data;
                this.callback = _callback;
                this.category = _category;
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
        public static void Export(BinaryWriter w)
        {
            Log("()()()Exporting to Save");
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            int length = json.Length;
            w.Write(GSSettings.Instance.version);
            w.Write(json);
            Log("()()()Exported");
            GSSettings.reset();

        }
        public static void Import(BinaryReader r)
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
            GSSettings generatedData = generator.Generate(gameDesc.starCount);
            GSSettings.Instance = generatedData;
            //string path = Path.Combine(Path.Combine(Paths.BepInExRootPath, "config"), "GSData.json");
            //if (!LoadSettingsFromJson(path))
            //{
            //    CreateDummySettings(12);
            //}
        }
        public static void SavePreferences()
        {
            GSPreferences preferences = new GSPreferences();
            preferences.GeneratorID = generator.GUID;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(preferences, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
        }
        public class GSPreferences
        {
            public string GeneratorID = "space.customizing.vanilla";
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
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<GSPreferences>(data2, ref result).AssertSuccessWithoutWarnings();
            ParsePreferences(result);
        }
        public static void ParsePreferences(GSPreferences p)
        {
            generator = GetGeneratorByID(p.GeneratorID);
        }
        public static iGenerator GetGeneratorByID(string guid)
        {
            foreach (iGenerator g in generators)
            {
                if (g.GUID == guid) return g;
            }
            return new Generators.Dummy();
        }
    }
}

