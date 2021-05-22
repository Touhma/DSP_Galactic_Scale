using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize {

    [BepInPlugin("dsp.galactic-scale.planet-size", "Galactic Scale Plug-In - Planet Size", "1.4.0")]
    public class PatchForPlanetSize : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        public static bool DebugGeneral = false;
        public static bool DebugPlanetRawData = false;
        public static bool DebugGetModPlane = false;
        public static bool DebugNewPlanetGrid = false;
        public static bool DebugPlanetModelingManager = false;
        public static bool DebugPlanetModelingManagerDeep = true;
        public static bool DebugAtmoBlur = false;
        public static bool DebugReformFix = false;

        public static float VanillaGasGiantSize = 800f;// will be rescaled in the create planet
        public static float VanillaGasGiantScale = 10f;// will be rescaled in the create planet
        public static float VanillaTelluricSize = 200f;
        public static int VanillaTelluricPrecision = 200;
        public static float VanillaTelluricScale = 1f;

        public static Dictionary<int, float> PlanetSizeParams = new Dictionary<int, float>();
        public static List<int> PlanetSizeList = new List<int>();
        public static Dictionary<int, float> MoonSizeParams = new Dictionary<int, float>();
        public static List<int> MoonSizeList = new List<int>();
        public static ConfigEntry<bool> MoonsHaveDifferentSizes;
        public static ConfigEntry<bool> EnableLimitedResizingFeature;
        public static ConfigEntry<int> StartingPlanetMinimumSize;
        public static ConfigEntry<bool> EnableMoonSizeFailSafe;

        //public static float MaxTelluricSize = 480f;
        public static ConfigEntry<float> BaseGasGiantSize;

        //public static float BaseTelluricSizeVariationFactor = 200f;
        // Min : 800, Max : 3200
        public static ConfigEntry<float> BaseGasGiantSizeVariationFactor;

        //public static float BaseGasGiantSizeVariationFactor = 1200f;
        public static ConfigEntry<string> LimitedResizingArray;
        public static ConfigEntry<string> LimitedResizingChances;
        public static ConfigEntry<string> LimitedResizingArrayMoons;
        public static ConfigEntry<string> LimitedResizingChancesMoons;
        internal void Awake() {
            var harmony = new Harmony("dsp.galactic-scale.planet-size");

            //Adding the Logger
            Logger = new ManualLogSource("PatchForPlanetSize");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            EnableLimitedResizingFeature = Config.Bind("galactic-scale-planets-size",
                "EnableLimitedResizingFeature",
                true,
                "limited version of the resizing feature : will be here the time we fix the other one --> if true : EnableCustomStarAlgorithm=true from the generation is a dependency --> should put EnableResizingFeature to false if activated");

            LimitedResizingArray = Config.Bind("galactic-scale-planets-size",
                "LimitedResizingArray",
                "50,100,200",
                "limited version of the resizing feature : will be here the time we fix the other one");

            LimitedResizingChances = Config.Bind("galactic-scale-planets-size",
                "LimitedResizingChances",
                "0.5,0.8,1",
                "chances for each size to appear --> 0 -> 0.5  = 1 , 0.5 -> 0.8 = 2 etc ...");
            MoonsHaveDifferentSizes = Config.Bind("galactic-scale-planets-size",
    "MoonsHaveDifferentSizes",
    false,
    "MoonsHaveDifferentSizes -> enable and moons will use a different list of sizes");
            LimitedResizingArrayMoons = Config.Bind("galactic-scale-planets-size",
    "LimitedResizingArrayMoons",
    "50,100,200",
    "Sizes moons can be");

            LimitedResizingChancesMoons = Config.Bind("galactic-scale-planets-size",
                "LimitedResizingChancesMoons",
                "0.5,0.8,1",
                "chances for each size moon to appear --> 0 -> 0.5  = 1 , 0.5 -> 0.8 = 2 etc ...");
            StartingPlanetMinimumSize = Config.Bind("galactic-scale-planets-size",
                "StartingPlanetMinimumSize",
                60,
                "StartingPlanetMinimumSize -> Sizes smaller than this may be missing resources required to progress in the game. It is advised to always check small starting planets for coal and oil. ~95% of seeds at size 50 are playable.");

            EnableMoonSizeFailSafe = Config.Bind("galactic-scale-planets-size",
                "EnableMoonSizeFailSafe",
                true,
                "EnableMoonSizeFailSafe -> enable and the moon will never be bigger than the host planet , true by default");

            BaseGasGiantSize = Config.Bind("galactic-scale-planets-size",
                "BaseGasGiantSize",
                2000f,
                "Base Gas Giant Size  -- Not Advised to modify YET");

            BaseGasGiantSizeVariationFactor = Config.Bind("galactic-scale-planets-size",
                "BaseGasGiantSizeVariationFactor",
                1200f,
                "Used to create variation on the planet size : help defining the min & max size for a gas giant --  -- Not Advised to modify YET");

            if (EnableLimitedResizingFeature.Value) { 
                ParseResizingSettings(LimitedResizingArray.Value, LimitedResizingChances.Value, LimitedResizingArrayMoons.Value, LimitedResizingChancesMoons.Value);

                Config.Save();

                //PatchForPlanetSize
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
                //Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGrid));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlatformSystem));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_Build));
                Harmony.CreateAndPatchAll(typeof(PatchUIBuildingGrid));
                Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
                Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
                Harmony.CreateAndPatchAll(typeof(PatchOnBuildingGizmo));
                Harmony.CreateAndPatchAll(typeof(PatchOnNearColliderLogic));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));
            }
        }


        public static void ParseResizingSettings(string configArray, string chanceArray, string configArrayMoon, string chanceArrayMoon) {
            var tempPlanetArray = Array.ConvertAll(configArray.Split(','), int.Parse);
            var tempChanceArray = Array.ConvertAll(chanceArray.Split(','), float.Parse);

            for (var i = 0; i < tempPlanetArray.Length; i++) {
                PlanetSizeParams.Add(tempPlanetArray[i], tempChanceArray[i]);
                PlanetSizeList.Add(tempPlanetArray[i]);
            }
            var tempMoonArray = Array.ConvertAll(configArrayMoon.Split(','), int.Parse);
            var tempMoonChanceArray = Array.ConvertAll(chanceArrayMoon.Split(','), float.Parse);
            for (var i = 0; i < tempMoonArray.Length; i++)
            {
                MoonSizeParams.Add(tempMoonArray[i], tempMoonChanceArray[i]);
                MoonSizeList.Add(tempMoonArray[i]);
            }
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
    }
}
