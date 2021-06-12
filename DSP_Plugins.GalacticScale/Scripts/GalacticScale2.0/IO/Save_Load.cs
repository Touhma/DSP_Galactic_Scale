using GSFullSerializer;
using System.IO;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void Export(BinaryWriter w) // Export Settings to Save Game
        {
            Log("Exporting to Save");
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            int length = json.Length;
            w.Write(GSSettings.Instance.version);
            w.Write(json);
            Log("Exported. Resetting GSSettings");
            //GSSettings.Reset(GSSettings.Seed);
            Log("End");
        }
        public static void Import(BinaryReader r) // Load Settings from Save Game
        {
            Log("Importing from Save");
            GSSettings.Reset(0);
            fsSerializer serializer = new fsSerializer();
            string version = r.ReadString();
            string json = r.ReadString();
            File.WriteAllText(GS2.DataDir+ "\\save.json", json);
            GSSettings result = GSSettings.Instance;
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize(data2, ref result);
            if (version != GSSettings.Instance.version)
            {
                Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
            }
            GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            Log("End");
        }
    }
}