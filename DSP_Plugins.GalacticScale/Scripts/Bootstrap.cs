using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("dsp.galactic-scale.GSStar-system-generation", "Galactic Scale Plug-In - Star System Generation",
        "2.0.0.0")]
    public class Bootstrap : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        // Internal Variables
        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = false;
        public static bool DebugStarGenDeep = false;
        public static bool DebugStarNamingGen = false;

        internal void Awake() {
            var harmony = new Harmony("dsp.galactic-scale.star-system-generation");

            //Adding the Logger
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);

            //Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));
            //Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGenStarGraph));
            //Harmony.CreateAndPatchAll(typeof(PatchGuideMissionStandardMode));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDescImport));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDescExport));
            Harmony.CreateAndPatchAll(typeof(PatchOnOptionWindow));
            Harmony.CreateAndPatchAll(typeof(PatchOnSetTabIndex));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameOption));
            //Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
            Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnVanillaStarGen)); // Only used when using vanilla generator, to allow 1024 stars
            //Harmony.CreateAndPatchAll(typeof(PatchOnMainMenu));
            //Harmony.CreateAndPatchAll(typeof(PatchOnUIVirtualStarMap));
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