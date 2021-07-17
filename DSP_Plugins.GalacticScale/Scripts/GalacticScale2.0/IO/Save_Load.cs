using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void Export(BinaryWriter w) // Export Settings to Save Game
        {
            Log("Exporting to Save");
            var serializer = new fsSerializer();
            serializer.TrySerialize(GSSettings.Instance, out var data);
            var json = fsJsonPrinter.CompressedJson(data);
            var length = json.Length;
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
            var serializer = new fsSerializer();
            var version = r.ReadString();
            var json = r.ReadString();
            File.WriteAllText(DataDir + "\\save.json", json);
            var result = GSSettings.Instance;
            var data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize(data2, ref result);
            if (version != GSSettings.Instance.version)
                Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
            GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            Log("End");
        }
    }
}