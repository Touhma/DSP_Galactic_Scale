using System;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS2
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
            fsData data2 = null;
            var parseResult = fsResult.Fail("DSV contains no legacy GalacticScale settings.");
            try
            {
                version = ReadLegacySaveString(r, 32);
                if (version == "2" || System.Version.TryParse(version, out _))
                {
                    json = ReadLegacySaveString(r, int.MaxValue);
                    parseResult = fsJsonParser.Parse(json, out data2);
                    if (parseResult.Succeeded && (!data2.IsDictionary ||
                        (!data2.AsDictionary.ContainsKey("Stars") && !data2.AsDictionary.ContainsKey("stars"))))
                        parseResult = fsResult.Fail("DSV contains no legacy GalacticScale settings.");
                }
            }
            catch (Exception e) when (e is EndOfStreamException || e is FormatException)
            {
                parseResult = fsResult.Fail("DSV contains no readable legacy GalacticScale settings.");
            }
            if (parseResult.Failed)
            {
                Warn("DSV Contained No GS2 Data.");
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

            if (Vanilla) ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs2dev");
            // if (Force == "") GSSettings.Instance = result;
            Warn($"StarCount3 : {GSSettings.StarCount}");
            GSSettings.Instance.imported = true;
            Warn($"Final Stream Position:{r.BaseStream.Position}");
            return true;
        }

        private static string ReadLegacySaveString(BinaryReader r, int maxBytes)
        {
            var position = r.BaseStream.Position;
            uint length = 0;
            for (var shift = 0; shift < 35; shift += 7)
            {
                var b = r.ReadByte();
                if (shift == 28 && (b & 0xf0) != 0) throw new FormatException("Invalid string length.");
                length |= (uint)(b & 0x7f) << shift;
                if ((b & 0x80) != 0) continue;
                if (length > maxBytes || length > r.BaseStream.Length - r.BaseStream.Position)
                    throw new EndOfStreamException("Legacy string exceeds the available save data.");
                r.BaseStream.Position = position;
                return r.ReadString();
            }
            throw new FormatException("Invalid string length.");
        }
    }
}
