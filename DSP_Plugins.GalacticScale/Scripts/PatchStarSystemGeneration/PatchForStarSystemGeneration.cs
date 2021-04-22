using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FullSerializer;
using GalacticScale.Scripts.PatchStarSystemGeneration.Generator;
using HarmonyLib;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("dsp.galactic-scale.star-system-generation", "Galactic Scale Plug-In - Star System Generation",
        "1.0.0.0")]
    public class PatchForStarSystemGeneration : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        // Internal Variables
        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = false;
        public static bool DebugStarGenDeep = false;
        public static bool DebugStarNamingGen = false;

        public static GeneratorGlobalSettings gen = new GeneratorGlobalSettings();


        public class Test {
            public string test;
        }

        internal void Awake() {
            var harmony = new Harmony("dsp.galactic-scale.star-system-generation");

            //Adding the Logger
            Logger = new ManualLogSource("PatchForStarSystemGeneration");
            BepInEx.Logging.Logger.Sources.Add(Logger);

            //Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchGuideMissionStandardMode));
            //Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGen));
            //Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
            //Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
            //Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));


            var configPath = Path.Combine(Path.Combine(Paths.BepInExRootPath, "config"), "GalacticScale.json");
            Debug("---------------------------------- configPath : " + configPath, LogLevel.Debug, true);


            //*

            fsSerializer serializer = new fsSerializer();
            
            serializer.TrySerialize<GeneratorGlobalSettings>(gen, out fsData data).AssertSuccessWithoutWarnings();

            string json = fsJsonPrinter.PrettyJson(data);
            //Debug(json, LogLevel.Debug, true);
             System.IO.File.WriteAllText(configPath , json);
            var testConfig = System.IO.File.ReadAllText(configPath );
            fsData data2 = fsJsonParser.Parse(testConfig);
            GeneratorGlobalSettings result = new GeneratorGlobalSettings();
            serializer.TryDeserialize<GeneratorGlobalSettings>(data2, ref result).AssertSuccessWithoutWarnings();

            // JsonConvert.PopulateObject( testConfig , result);
            //Debug("---------------------------------- Test : " + result.GeneratorClusterSettings.Flatten, LogLevel.Debug, true);
    //*/
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
        public static void Debug(object data)
        {
            Logger.LogMessage(data);
        }

    }
}