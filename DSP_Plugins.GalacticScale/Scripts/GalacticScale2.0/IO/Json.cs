using GSFullSerializer;
using System.IO;

namespace GalacticScale
{
    public static partial class GS2
    {
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
            GSSettings result = GSSettings.Instance;
            Log("Parsing JSON");
            fsData data = fsJsonParser.Parse(json);
            Log("Trying To Deserialize JSON");
            serializer.TryDeserialize(data, ref result);
            Log("End");
            return true;
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
        private static bool CheckJsonFileExists(string path)
        {
            Log("Checking if Json File Exists");
            if (File.Exists(path)) return true;
            Log("Json file does not exist at " + path);
            return false;
        }
    }
}