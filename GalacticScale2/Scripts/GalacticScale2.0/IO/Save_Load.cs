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

        public static bool Import(BinaryReader r) // Load Settings from Save Game
        {
            Log("Importing from Save");
            GSSettings.Reset(0);
            var serializer = new fsSerializer();
            GS2.Warn($"Peek:{r.PeekChar()}");
            GS2.Warn($"{r.BaseStream.Position} / {r.BaseStream.Length}");
            var position = r.BaseStream.Position;
            var version = r.ReadString();
            var json = r.ReadString();
            File.WriteAllText(DataDir + "\\save.json", json);
            var result = GSSettings.Instance;
            fsData data2;
                var parseResult = fsJsonParser.Parse(json, out data2);
            
            if (parseResult.Failed)
            {
                GS2.Warn("Parse Failed");
                r.BaseStream.Position = position;
                GS2.ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            var deserialize = serializer.TryDeserialize(data2, ref result);
            if (deserialize.Failed)
            {
                GS2.Warn("Deserialize Failed");
                r.BaseStream.Position = position;
                GS2.ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            GS2.WarnJson(version);
            if (version != GSSettings.Instance.version)
            {
                Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
                r.BaseStream.Position = position;
                GS2.ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                return false;
            }
            if (Vanilla) ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs2dev");
            GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            GS2.Warn($"{ActiveGenerator.GUID}");
            Log("End");
            return true;
        }
    }
}