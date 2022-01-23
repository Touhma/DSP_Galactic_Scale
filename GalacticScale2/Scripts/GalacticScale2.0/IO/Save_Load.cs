using System;
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

        public static bool Import(BinaryReader r, string Force = "") // Load Settings from Save Game
        {

            Log("Importing from Save");
            if (!GS2.SaveOrLoadWindowOpen) GSSettings.Reset(0);
            var serializer = new fsSerializer();
            var position = r.BaseStream.Position;
            var version = "2";
            var json = "";
            if (Force == "")
            {
                version = r.ReadString();
                json = r.ReadString();
            }
            else
            {
                LoadSettingsFromJson(Force);
            }

            if (GS2.SaveOrLoadWindowOpen) return true;
            if (Config.Dev)
            {
                File.WriteAllText(Path.Combine(GS2.DataDir, "SaveContentsRaw.txt"), json);
            }

            GSSettings result = new GSSettings(0);
            if (Force == "" && !SaveOrLoadWindowOpen) result = GSSettings.Instance;
            fsData data2;
            var parseResult = fsJsonParser.Parse(json, out data2);
            // if (Force == "")
            // {
                if (parseResult.Failed)
                {
                    Warn("Parse Failed");
                    if (Force == "") r.BaseStream.Position = position;
                    if (Force == "") ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                    if (Force == "") return false;
                }
            // }
            
            if (Config.Dev)
            {
                File.WriteAllText(Path.Combine(GS2.DataDir, "SaveContents.json"),json);
            }

            try
            {
                var deserialize = serializer.TryDeserialize(data2, ref result);
                // if (Force == "")
                // {
                    if (deserialize.Failed)
                    {
                        Warn("Deserialize Failed");
                        if (Force == "")r.BaseStream.Position = position;
                        if (Force == "")ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                        if (Force == "")return false;
                    }
                // }
                // else
                // {
                //     Warn("Loading Forced Json Settings");
                //     LoadSettingsFromJson(Force);
                // }
            }
            catch (Exception e)
            {
                GS2.Warn($"{e.Message}");
            }

            if (version != GSSettings.Instance.version)
            {
                Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
                if (Force == "") r.BaseStream.Position = position;
                if (Force == "" && !SaveOrLoadWindowOpen)ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                if (Force == "")return false;
            }

            if (Vanilla) ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs2dev");
            if (Force == "" && !SaveOrLoadWindowOpen) GSSettings.Instance = result;
            GSSettings.Instance.imported = true;
            return true;
        }
    }
}