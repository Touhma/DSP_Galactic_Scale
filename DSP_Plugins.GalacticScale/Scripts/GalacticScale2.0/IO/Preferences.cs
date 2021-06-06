using GSFullSerializer;
using System.Collections.Generic;
using System.IO;

namespace GalacticScale
{
    public static partial class GS2
    {
        private class GSPreferences
        {
            public bool debug = false;
            public bool skipPrologue = false;
            public bool noTutorials = false;
            public bool forceRare = false;
            public bool cheatMode = false;
            public string GeneratorID = "space.customizing.vanilla";
            public Dictionary<string, GSGenPreferences> PluginOptions = new Dictionary<string, GSGenPreferences>();
        }
        public static void SavePreferences()
        {
            Log("Start");
            GSPreferences preferences = new GSPreferences();
            preferences.GeneratorID = generator.GUID;
            preferences.debug = debugOn;
            preferences.forceRare = Force1RareChance;
            preferences.skipPrologue = SkipPrologue;
            preferences.noTutorials = tutorialsOff;
            preferences.cheatMode = CheatMode;
            Log("Retrieving preferences for plugins");
            foreach (iGenerator g in generators)
            {
                if (g is iConfigurableGenerator)
                {
                    iConfigurableGenerator gen = g as iConfigurableGenerator;
                    Log("Trying to get preferences for " + gen.Name);
                    GSGenPreferences prefs = gen.Export();

                    preferences.PluginOptions[gen.GUID] = prefs;
                    Log("Finished adding preferences for " + gen.Name);
                }
            }
            fsSerializer serializer = new fsSerializer();
            Log("Trying to serialize preferences object");
            serializer.TrySerialize(preferences, out fsData data);
            Log("Serialized");
            string json = fsJsonPrinter.PrettyJson(data);
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
            Log("End");
        }
        public static void LoadPreferences(bool debug = false)
        {
            Log("Start");
            string path = Path.Combine(DataDir, "Preferences.json");
            if (!CheckJsonFileExists(path)) return;
            Log("Loading Preferences from " + path);
            fsSerializer serializer = new fsSerializer();
            string json = File.ReadAllText(path);
            GSPreferences preferences = new GSPreferences();
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize(data2, ref preferences);
            if (!debug) ParsePreferences(preferences);
            else
            {
                debugOn = preferences.debug;
            }
            Log("Preferences loaded");
            Log("End");
        }
        private static void ParsePreferences(GSPreferences p)
        {
            Log("Start");
            debugOn = p.debug;
            Force1RareChance = p.forceRare;
            SkipPrologue = p.skipPrologue;
            tutorialsOff = p.noTutorials;
            CheatMode = p.cheatMode;
            generator = GetGeneratorByID(p.GeneratorID);
            if (p.PluginOptions != null)
            {
                foreach (KeyValuePair<string, GSGenPreferences> pluginOptions in p.PluginOptions)
                {
                    Log("Plugin Options for " + pluginOptions.Key + "found");
                    iConfigurableGenerator gen = GetGeneratorByID(pluginOptions.Key) as iConfigurableGenerator;
                    if (gen != null)
                    {
                        Log(gen.Name + "'s plugin options exported");
                        gen.Import(pluginOptions.Value);
                    }
                }
            }
            Log("End");
        }
    }
}