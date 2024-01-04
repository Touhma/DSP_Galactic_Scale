using System;
using System.Collections.Generic;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS3
    {
        private static GSPreferences Preferences = new();

        public static void SavePreferences()
        {
            // Warn("SavePreferences");
            Preferences.version = PreferencesVersion;
            Preferences.MainSettings = Config.Export();
            // foreach (var x in Preferences.MainSettings) GS3.Warn($"Key:{x.Key} Value:{x.Value}");
            foreach (var g in Plugins)
                if (g is iConfigurablePlugin)
                {
                    var plugin = g;
                    // Log("Trying to get plugin preferences for " + plugin.Name);
                    var prefs = plugin.Export();
                    Preferences.PluginPreferences[plugin.GUID] = prefs;
                    // Log("Finished updating preferences for " + plugin.Name);
                }
            foreach (var g in Generators) Preferences.Save(g as iConfigurableGenerator); 
            // if (ActiveGenerator is iConfigurableGenerator) Preferences.Save(ActiveGenerator as iConfigurableGenerator);
            // else GSPreferences.WriteToDisk(Preferences);
            GSPreferences.WriteToDisk(Preferences);
            Log("Preferences Saved");
        }

        public static void LoadPreferences(bool debug = false)
        {
            // Log("LoadPreferences");
            //var path = Path.Combine(DataDir, "Preferences.json");
            //if (!CheckJsonFileExists(path)) return;

            //Log("Loading Preferences from " + path);
            //var serializer = new fsSerializer();
            //var json = File.ReadAllText(path);
            //var preferences = new GSPreferences();
            //var data2 = fsJsonParser.Parse(json);
            //serializer.TryDeserialize(data2, ref preferences);
            //if (preferences.version != PreferencesVersion)
            //{
            //    Warn("Preferences.json Version Mismatch. Renaming to Preferences.Old");
            //    var newName = "Preferences.Old." + DateTime.Now.ToString("yyMMddHHmmss");
            //    if (File.Exists(Path.Combine(DataDir, newName))) File.Delete(Path.Combine(DataDir, newName));
            //    File.Move(Path.Combine(DataDir, "Preferences.json"), Path.Combine(DataDir, newName));
            //    updateMessage += "\r\nPreferences.json version is incompatible. It has been renamed to " + newName + "\r\nPlease reconfigure GS3\r\n";
            //    return;
            //}
            Preferences = GSPreferences.ReadFromDisk();
            if (!debug)
                ParsePreferences(Preferences);
            else
                Config.Import(Preferences.MainSettings);
            // debugOn = preferences.debug;
            Log("Preferences loaded");
            // Log("End");
        }

        private static void ParsePreferences(GSPreferences p)
        {
            // Log("Start");
            Config.Import(p.MainSettings);
            if (p.GeneratorPreferences != null)
                foreach (var generatorPreferences in p.GeneratorPreferences)
                {
                    // Log("Generator Preferences for " + generatorPreferences.Key + "found");
                    var gen = GetGeneratorByID(generatorPreferences.Key) as iConfigurableGenerator;
                    if (gen != null)
                    {
                        // Log(gen.Name + "'s preferences exported to generator");
                        gen.Import(generatorPreferences.Value);
                    }
                }

            if (p.PluginPreferences != null)
                foreach (var pluginPreferences in p.PluginPreferences)
                {
                    // Log("Plugin Preferences for " + pluginPreferences.Key + "found");
                    var plugin = GetPluginByID(pluginPreferences.Key);
                    if (plugin != null)
                    {
                        // Log(plugin.Name + "'s plugin preferences exported");
                        plugin.Import(pluginPreferences.Value);
                    }
                }

            // Log("End");
        }

        private class GSPreferences
        {
            public readonly Dictionary<string, GSGenPreferences> GeneratorPreferences = new();
            public readonly Dictionary<string, GSGenPreferences> PluginPreferences = new();

            public GSGenPreferences MainSettings = new();

            public int version;

            public static bool WriteToDisk(GSPreferences preferences)
            {
                // Warn("WriteToDisk");

                var serializer = new fsSerializer();
                var fsResult = serializer.TrySerialize(Preferences, out var data);
                if (fsResult.Failed)
                {
                    Error(fsResult.FormattedMessages);
                    return false;
                }

                var json = fsJsonPrinter.PrettyJson(data);
                if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
                if (!Directory.Exists(DataDir)) return false;
                try
                {
                    File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
                }
                catch (Exception e)
                {
                    Error(e.Message);
                    return false;
                }

                return true;
            }

            public static GSPreferences ReadFromDisk()
            {
                // Log("ReadFromDisk");
                var path = Path.Combine(DataDir, "Preferences.json");
                if (!CheckJsonFileExists(path))
                {
                    Warn("Cannot find Preferences.json. Creating");

                    var newPreferences = new GSPreferences();
                    WriteToDisk(newPreferences);
                    return newPreferences;
                }

                Log("Loading Preferences from " + path);
                var serializer = new fsSerializer();
                var json = File.ReadAllText(path);
                var preferences = new GSPreferences();
                var parsedJson = fsJsonParser.Parse(json);
                var fsResult = serializer.TryDeserialize(parsedJson, ref preferences);
                if (fsResult.Failed)
                {
                    Error("Failed to Deserialize Preferences.json");
                    Warn(fsResult.FormattedMessages);
                    return new GSPreferences();
                }

                // if (preferences.version != PreferencesVersion)
                // {
                //     Warn("Preferences.json Version Mismatch. Renaming to Preferences.Old");
                //     var newName = "Preferences.Old." + DateTime.Now.ToString("yyMMddHHmmss");
                //     if (File.Exists(Path.Combine(DataDir, newName))) File.Delete(Path.Combine(DataDir, newName));
                //     File.Move(Path.Combine(DataDir, "Preferences.json"), Path.Combine(DataDir, newName));
                //     updateMessage += "\r\nPreferences.json version is incompatible. It has been renamed to " + newName + "\r\nPlease reconfigure GS3\r\n";
                //     return new GSPreferences();
                // }

                return preferences;
            }

            public void Save(iConfigurableGenerator generator)
            {
                // Log($"Save (Generator) {generator.GUID}");
                var generatorPreferences = generator.Export();
                GeneratorPreferences[generator.GUID] = generatorPreferences;
                // return WriteToDisk(this);
            }

            public GSGenPreferences Load(iConfigurableGenerator generator, bool fromFile = false)
            {
                if (!fromFile)
                {
                    if (GeneratorPreferences.ContainsKey(generator.GUID)) return GeneratorPreferences[generator.GUID];
                    Warn("Generator Preferences do not exist, creating new");
                    return new GSGenPreferences();
                }

                var preferences = ReadFromDisk();
                if (preferences.GeneratorPreferences.ContainsKey(generator.GUID)) return preferences.GeneratorPreferences[generator.GUID];
                Warn("Generator Preferences do not exist, creating new");
                return new GSGenPreferences();
            }
        }
    }
}