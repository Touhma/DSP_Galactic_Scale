
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
        public static string DataDir = Path.Combine(Paths.BepInExRootPath, "output");

        public static Dictionary<string, GSTheme> planetThemes = new Dictionary<string, GSTheme>()
        {
            ["Mediterranian"] = new GSTheme()
            {
                name = "Mediterranian",
                type = EPlanetType.Ocean,
                theme = LDB.themes.Select(1),
            },
        };

        public static void CreateDummySettings(int starCount)
        {
            Log("Creating New Settings");
            GSSettings.Stars.Clear();
            List<GSplanet> p = new List<GSplanet>
                {
                    new GSplanet("Urf")
                };
            GSSettings.Stars.Add(new GSStar(1, "BeetleJuice", ESpectrType.O, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.X, EStarType.BlackHole, new List<GSplanet>()));
            }
            GSSettings.GalaxyParams = new galaxyParams();
            GSSettings.GalaxyParams.iterations = 4;
            GSSettings.GalaxyParams.flatten = 0.18;
            GSSettings.GalaxyParams.minDistance = 2;
            GSSettings.GalaxyParams.minStepLength = 2.3;
            GSSettings.GalaxyParams.maxStepLength = 3.5;
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
            string path = Path.Combine(Path.Combine(Paths.BepInExRootPath, "config"), "GSData.json");
            if (!LoadSettingsFromJson(path))
            {
                CreateDummySettings(12);
            }
        }
    }
}

