using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [BepInPlugin("touhma.dsp.galactic-scale.planet-size", "Galactic Scale Plug-In - Planet Size", "1.0.0.0")]
    public class PatchForPlanetSize : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        public static bool DebugGeneral = false;
        public static bool DebugPlanetFactory = false;
        public static bool DebugPlanetFactoryDeep = false;
        public static bool DebugPlanetRawData = false;
        public static bool DebugGetModPlane = false;
        public static bool DebugPlanetData = false;
        public static bool DebugPlanetDataDeep = false;
        public static bool DebugPlanetModelingManager = false;
        public static bool DebugPlanetModelingManagerDeep = false;

        public static float VanillaGasGiantSize = 800f; // will be rescaled in the create planet
        public static float VanillaGasGiantScale = 10f; // will be rescaled in the create planet
        public static float VanillaTelluricSize = 200f;
        public static int VanillaTelluricPrecision = 200;
        public static float VanillaTelluricScale = 1f;

        public static bool DebugAtmoBlur = false;

        public static ConfigEntry<bool> EnableResizingFeature;

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


        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.planet-size");

            //Adding the Logger
            Logger = new ManualLogSource("PatchForPlanetSize");
            BepInEx.Logging.Logger.Sources.Add(Logger);

            EnableResizingFeature = Config.Bind("galactic-scale-planets-size",
                "EnableResizingFeature",
                true,
                "Decide if the resizing of the planets is enabled or not --> if true : EnableCustomStarAlgorithm=true from the generation is a dependency ");

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
                1400f,
                "Base Gas Giant Size  -- Not Advised to modify YET");

            BaseTelluricSizeVariationFactor = Config.Bind("galactic-scale-planets-size",
                "BaseTelluricSizeVariationFactor",
                200f,
                "Used to create variation on the planet size : help defining the min & max size for a Telluric planet-- Not Advised to modify YET");

            BaseGasGiantSizeVariationFactor = Config.Bind("galactic-scale-planets-size",
                "BaseGasGiantSizeVariationFactor",
                600f,
                "Used to create variation on the planet size : help defining the min & max size for a gas giant --  -- Not Advised to modify YET");

            if (EnableResizingFeature.Value) {
                //PatchForPlanetSize
                
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
               //Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
                //Harmony.CreateAndPatchAll(typeof(PatchOnPlayerNavigation));
                //Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
                //Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_Build));
            }
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
    }
}