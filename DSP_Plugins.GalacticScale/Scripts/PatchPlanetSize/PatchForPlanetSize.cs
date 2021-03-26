using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [BepInPlugin("dsp.galactic-scale.planet-size", "Galactic Scale Plug-In - Planet Size", "1.3.3.0")]
    public class PatchForPlanetSize : BaseUnityPlugin {
        public new static ManualLogSource Logger;
        public static string Version = "1.3.3";
        public static bool DebugGeneral = false;
        public static bool DebugPlanetRawData = false;
        public static bool DebugGetModPlane = false;
        public static bool DebugNewPlanetGrid = false;
        public static bool DebugPlanetModelingManager = false;
        public static bool DebugPlanetModelingManagerDeep = false;
        public static bool DebugAtmoBlur = false;


        public static float VanillaGasGiantSize = 800f;// will be rescaled in the create planet
        public static float VanillaGasGiantScale = 10f;// will be rescaled in the create planet
        public static float VanillaTelluricSize = 200f;
        public static int VanillaTelluricPrecision = 200;
        public static float VanillaTelluricScale = 1f;

        public static Dictionary<int, float> PlanetSizeParams = new Dictionary<int, float>();
        public static List<int> PlanetSizeList = new List<int>();


        public static ConfigEntry<bool> EnableResizingFeature;
        public static ConfigEntry<bool> EnableLimitedResizingFeature;
        public static ConfigEntry<bool> EnableMoonSizeFailSafe;

        public static ConfigEntry<float> BaseTelluricSize;

        //public static float BaseTelluricSize = 280f;
        public static ConfigEntry<float> MinTelluricSize;

        //public static float MinTelluricSize = 80f;
        public static ConfigEntry<float> MaxTelluricSize;

        //public static float MaxTelluricSize = 480f;
        public static ConfigEntry<float> BaseGasGiantSize;

        //public static float BaseGasGiantSize = 2000f;
        // Min : 80, Max : 480
        public static ConfigEntry<float> BaseTelluricSizeVariationFactor;

        //public static float BaseTelluricSizeVariationFactor = 200f;
        // Min : 800, Max : 3200
        public static ConfigEntry<float> BaseGasGiantSizeVariationFactor;

        //public static float BaseGasGiantSizeVariationFactor = 1200f;
        public static ConfigEntry<string> LimitedResizingArray;
        public static ConfigEntry<string> LimitedResizingChances;

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

            EnableResizingFeature = Config.Bind("galactic-scale-planets-size",
                "EnableResizingFeature",
                false,
                "Decide if the resizing of the planets is enabled or not --> if true : EnableCustomStarAlgorithm=true from the generation is a dependency --> should put EnableLimitedResizingFeature to false if activated");

            EnableMoonSizeFailSafe = Config.Bind("galactic-scale-planets-size",
                "EnableMoonSizeFailSafe",
                true,
                "EnableMoonSizeFailSafe -> enable and the moon will never be bigger than the host planet , true by default");

            BaseTelluricSize = Config.Bind("galactic-scale-planets-size",
                "BaseTelluricSize",
                280f,
                "Base Telluric planet Size  -- Not Advised to modify YET");

            MinTelluricSize = Config.Bind("galactic-scale-planets-size",
                "MinTelluricSize",
                80f,
                "Min Value Telluric planet Size. Should be BaseTelluricSize - BaseTelluricSizeVariationFactor  -- Not Advised to modify YET");

            MaxTelluricSize = Config.Bind("galactic-scale-planets-size",
                "MaxTelluricSize",
                480f,
                "Max Value Telluric planet Size --> more that than CAN break and is not supported YET. Should be BaseTelluricSize + BaseTelluricSizeVariationFactor  -- Not Advised to modify YET");

            BaseGasGiantSize = Config.Bind("galactic-scale-planets-size",
                "BaseGasGiantSize",
                2000f,
                "Base Gas Giant Size  -- Not Advised to modify YET");

            BaseTelluricSizeVariationFactor = Config.Bind("galactic-scale-planets-size",
                "BaseTelluricSizeVariationFactor",
                200f,
                "Used to create variation on the planet size : help defining the min & max size for a Telluric planet-- Not Advised to modify YET");

            BaseGasGiantSizeVariationFactor = Config.Bind("galactic-scale-planets-size",
                "BaseGasGiantSizeVariationFactor",
                1200f,
                "Used to create variation on the planet size : help defining the min & max size for a gas giant --  -- Not Advised to modify YET");

            if (EnableResizingFeature.Value || EnableLimitedResizingFeature.Value) {

                ParseResizinSettings(LimitedResizingArray.Value, LimitedResizingChances.Value);

                // check some configs dependencies 
                if (EnableLimitedResizingFeature.Value) EnableResizingFeature.Value = false;

                Config.Save();

                //PatchForPlanetSize
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGrid));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlatformSystem));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_BuildCheck));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_Build));
                Harmony.CreateAndPatchAll(typeof(PatchUIBuildingGrid));
                Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
                Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
<<<<<<< Updated upstream
                Harmony.CreateAndPatchAll(typeof(PatchBuildingGizmo));
                Harmony.CreateAndPatchAll(typeof(PatchNearColliderLogic));
                //Harmony.CreateAndPatchAll(typeof(PatchPlanetAlgorithm));
=======
                Harmony.CreateAndPatchAll(typeof(PatchUI));
>>>>>>> Stashed changes
            }
        }


        public static void ParseResizinSettings(string configArray, string chanceArray) {
            var tempPlanetArray = Array.ConvertAll(configArray.Split(','), int.Parse);
            var tempChanceArray = Array.ConvertAll(chanceArray.Split(','), float.Parse);

            for (var i = 0; i < tempPlanetArray.Length; i++) {
                PlanetSizeParams.Add(tempPlanetArray[i], tempChanceArray[i]);
                PlanetSizeList.Add(tempPlanetArray[i]);
            }
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
        public static void Log(object data) //Leaving here because the other one is a pain to type when you're rapidly debugging using the manual log method...
        {
            Debug(data, LogLevel.Message, true);
        }

        //This library is amazing for debugging.
        //private static readonly fsSerializer _serializer = new fsSerializer();

        //public static string Serialize(Type type, object value)
        //{
        //    // serialize the data
        //    fsData data;
        //    _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        //    // emit the data via JSON
        //    return fsJsonPrinter.CompressedJson(data);
        //}

        //public static object Deserialize(Type type, string serializedState)
        //{
        //    // step 1: parse the JSON data
        //    fsData data = fsJsonParser.Parse(serializedState);

        //    // step 2: deserialize the data
        //    object deserialized = null;
        //    _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        //    return deserialized;
        //}

    }
}