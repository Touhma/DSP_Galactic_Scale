using System;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS3
    {
        public static void Export(BinaryWriter w) // Export Settings to Save Game
        {
            // Log("Exporting to Save");
            // var serializer = new fsSerializer();
            // serializer.TrySerialize(GSSettings.Instance, out var data);
            // var json = fsJsonPrinter.CompressedJson(data);
            // w.Write(GSSettings.Instance.version);
            // w.Write(json);
        }

        public static bool Import(BinaryReader r, string LoadPath = "") // Load Settings from Save Game
        {
            // return true;
            Warn("Import");

            Log("Importing from Save.");
            if (!SaveOrLoadWindowOpen) GSSettings.Reset(0);
            var serializer = new fsSerializer();
            var position = r.BaseStream.Position;
            Warn($"Initial Stream Position:{position}");
            var version = "2";
            var json = "";
            // var isVanilla = true;
            version = r.ReadString();
            json = r.ReadString();
            fsData data2;
            var parseResult = fsJsonParser.Parse(json, out data2);
            if (parseResult.Failed)
            {
                Warn("DSV Contained No GS3 Data.");
                Warn(parseResult.FormattedMessages);
                r.BaseStream.Position = position;
                if (SaveOrLoadWindowOpen) return true;
                if (LoadPath == "" || !File.Exists(LoadPath)) // Must be a vanilla save?
                {
                    ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                    return false;
                }
            }

            Warn($"parseResult:{parseResult.Succeeded}");
            Warn($"Input file : {LoadPath}");
            Warn($"After Parse, Stream Position:{r.BaseStream.Position}");
            if (SaveOrLoadWindowOpen) return true;
            var result = new GSSettings(0);
            if (LoadPath != "")
            {
                Warn($"*** Loading Settings From {LoadPath}");
                LoadSettingsFromJson(LoadPath);
                Warn($"StarCount : {GSSettings.StarCount}");
            }
            else
            {
                try
                {
                    var deserialize = serializer.TryDeserialize(data2, ref result);
                    if (deserialize.Failed)
                    {
                        Warn("Deserialize Failed");
                        if (LoadPath == "") r.BaseStream.Position = position;
                        if (LoadPath == "") ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
                        Warn($"After Deserialize Stream Position:{r.BaseStream.Position}");
                        if (LoadPath == "") return false;
                    }
                    else
                    {
                        GSSettings.Instance = result;
                    }
                }
                catch (Exception e)
                {
                    Warn($"{e.Message}");
                }
            }

            Warn($"StarCount2 : {GSSettings.StarCount}");
            // if (Force == "") result = GSSettings.Instance;
            // Warn($"StarCount3 : {GSSettings.StarCount}");
            // if (float.Parse(version) != float.Parse( GSSettings.Instance.version))
            // {
            //     Warn("Version mismatch: " + GSSettings.Instance.version + " trying to load " + version + " savedata");
            //     if (Force == "") r.BaseStream.Position = position;
            //     if (Force == "" && !SaveOrLoadWindowOpen) ActiveGenerator = GetGeneratorByID("space.customizing.generators.vanilla");
            //     if (Force == "") return false;
            // }

            ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs3");
            // if (Force == "") GSSettings.Instance = result;
            Warn($"StarCount3 : {GSSettings.StarCount}");
            GSSettings.Instance.imported = true;
            Warn($"Final Stream Position:{r.BaseStream.Position}");
            return true;
        }
    }
}