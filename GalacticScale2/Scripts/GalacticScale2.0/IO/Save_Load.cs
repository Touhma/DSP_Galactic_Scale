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
            w.Write(GSSettings.Instance.version);
            w.Write(json);
        }

        public static bool Import(BinaryReader r) // Load Settings from Save Game
        {
            Log("Importing from Save");
            GSSettings.Reset(0);
            var serializer = new fsSerializer();
            var position = r.BaseStream.Position;
            var version = r.ReadString();
            var json = r.ReadString();
            var result = GSSettings.Instance;
            fsData data2;
                var parseResult = fsJsonParser.Parse(json, out data2);
            
            if (parseResult.Failed)
            {
                Warn("Parse Failed");
                r.BaseStream.Position = position;
                ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            var deserialize = serializer.TryDeserialize(data2, ref result);
            if (deserialize.Failed)
            {
                Warn("Deserialize Failed");
                r.BaseStream.Position = position;
                ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            if (version != GSSettings.Instance.version)
            {
                Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
                r.BaseStream.Position = position;
                ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            if (Vanilla) ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs2dev");
            GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            return true;
        }
    }
}