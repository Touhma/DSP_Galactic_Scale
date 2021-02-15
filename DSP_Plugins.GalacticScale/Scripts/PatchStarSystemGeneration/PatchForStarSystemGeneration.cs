using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.Assertions.Comparers;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("touhma.dsp.galactic-scale.star-system-generation", "Galactic Scale Plug-In - Star System Generation",
        "1.0.0.0")]
    public class PatchForStarSystemGeneration : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        public static ConfigEntry<int> StartingSystemPlanetNb;
        //public static int StartingSystemPlanetNb = 30;

        // use custom orbit for planets
        public static ConfigEntry<bool> UseCustomOrbitRadiusArray;
        public static ConfigEntry<float[]> CustomOrbitRadiusArrayPlanets;
   
        // use custom orbit for moons
        public static ConfigEntry<bool> UseCustomOrbitRadiusArrayMoons;
        public static ConfigEntry<float[]> CustomOrbitRadiusArrayMoons;
        
        
        // Settings for the algorithms
        public static ConfigEntry<int> OrbitRadiusArrayMoonsNb;

        //public static int OrbitRadiusArrayMoonsNb = 20;
        public static ConfigEntry<int> OrbitRadiusArrayPlanetNb;
        //public static int OrbitRadiusArrayPlanetNb = 16;

        public static ConfigEntry<int> MaxOrbitInclination;

        //public static int MaxOrbitInclination = 35;
        // Increase the inclination if it's a moon
        public static ConfigEntry<float> MoonOrbitInclinationFactor;
        //public static float MoonOrbitInclinationFactor = 2.2f;

        // Increase the inclination if around a neutron star
        public static ConfigEntry<float> NeutronStarOrbitInclinationFactor;
        // public static float NeutronStarOrbitInclinationFactor = 1.3f;

        // Chance that the planet is "rolling" on it's orbit
        public static ConfigEntry<float> ChancePlanetLaySide;
        //public static float ChancePlanetLaySide = 0.04f;

        //ObliquityAngle for rolling : something between 70 -> 90  and -70 -> -90 by default
        public static ConfigEntry<float> LaySideBaseAngle;

        //public static float LaySideBaseAngle = 20f;
        public static ConfigEntry<float> LaySideAddingAngle;
        //public static float LaySideAddingAngle = 70f;

        //between 30 -> 70 and -30 -> -70
        public static ConfigEntry<float> ChanceBigObliquity;

        //public static float ChanceBigObliquity = 0.1f;
        public static ConfigEntry<float> BigObliquityBaseAngle;

        //public static float BigObliquityBaseAngle = 40f;
        public static ConfigEntry<float> BigObliquityAddingAngle;
        //public static float BigObliquityAddingAngle = 30f;

        // Between 30 -> -30
        public static ConfigEntry<float> StandardObliquityAngle;
        //public static float StandardObliquityAngle = 30f;


        // Minimum rotation period
        public static ConfigEntry<float> RotationPeriodBaseTime;

        //public static float RotationPeriodBaseTime = 400;
        // Can be between 0 -> 1000
        public static ConfigEntry<float> RotationPeriodVariabilityFactor;
        //public static float RotationPeriodVariabilityFactor = 1000;

        //Tidal Lock
        public static ConfigEntry<float> ChanceTidalLock;

        //public static float ChanceTidalLock = 0.10f;
        public static ConfigEntry<float> ChanceTidalLock1;

        //public static float ChanceTidalLock1 = 0.04f;
        public static ConfigEntry<float> ChanceTidalLock2;
        // public static float ChanceTidalLock2 = 0.07f;

        //Chance retrograde orbit
        public static ConfigEntry<float> ChanceRetrogradeOrbit;
        //public static float ChanceRetrogradeOrbit = 0.05f;


        //Habitability Related : 
        public static ConfigEntry<float> HabitabilityBaseConstant;
        //public static float HabitabilityBaseConstant = 1000f;

        // the habitability area : for 1 habitability radius of the star & a value of 0.2f : 0.8 -> 1.2 
        public static ConfigEntry<float> HabitableRadiusAreaBaseline;
        //public static float HabitableRadiusAreaBaseline = 0.2f;

        //Chance of being habitable if in the habitability zone of the star
        public static ConfigEntry<float> ChanceBeingHabitable;
        //public static float ChanceBeingHabitable = 0.4f;

        // if the ratio planet.distance & star.habitable radius is less than that : Volcano planet
        public static ConfigEntry<float> VolcanoPlanetDistanceRatio;

        //public static float VolcanoPlanetDistanceRatio = 0.3f;
        // if the ratio planet.distance & star.habitable radius is more than that : Ice planet
        public static ConfigEntry<float> IcePlanetDistanceRatio;
        //public static float IcePlanetDistanceRatio = 1.2f;

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


        //Generator Settings
        public static Dictionary<EStarType, StarSystemSetting> GeneratorSpecialsSystemConfig =
            new Dictionary<EStarType, StarSystemSetting>();

        public static Dictionary<ESpectrType, StarSystemSetting> GeneratorMainSystemConfig =
            new Dictionary<ESpectrType, StarSystemSetting>();


        // Internal Variables

        public static float[] OrbitRadiusArrayMoons;
        public static float[] OrbitRadiusPlanetArray;

        public static bool DebugPlanetGen = false;
        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = true;

        public static bool EnableCustomStarAlgorithm = false;


        // public static StarSystemSetting
        public PatchForStarSystemGeneration() {
            GeneratorSpecialsSystemConfig.Add(EStarType.BlackHole,
                new StarSystemSetting(1, 1, 4, 1, 0.5f, 0.5f, 0.5f, 0.5f));
            GeneratorSpecialsSystemConfig.Add(EStarType.NeutronStar,
                new StarSystemSetting(1, 1, 4, 2, 0.8f, 0.2f, 0.5f, 0.5f));
            GeneratorSpecialsSystemConfig.Add(EStarType.WhiteDwarf,
                new StarSystemSetting(2, 4, 3, 3, 0.6f, 0.6f, 0.7f, 0.2f));
            GeneratorSpecialsSystemConfig.Add(EStarType.GiantStar,
                new StarSystemSetting(6, 6, 5, 2, 0.8f, 0.8f, 0.8f, 0.2f));


            //Order of frequency Vanilla : B -> G -> K -> F -> M -> A -> O
            GeneratorMainSystemConfig.Add(ESpectrType.A, new StarSystemSetting(9, 10, 3, 3, 0.7f, 0.4f, 0.6f, 0.5f));
            GeneratorMainSystemConfig.Add(ESpectrType.B, new StarSystemSetting(6, 6, 5, 2, 0.8f, 0.3f, 0.8f, 0.5f));
            GeneratorMainSystemConfig.Add(ESpectrType.F, new StarSystemSetting(6, 6, 3, 2, 0.8f, 0.5f, 0.8f, 0.2f));
            GeneratorMainSystemConfig.Add(ESpectrType.G, new StarSystemSetting(3, 3, 2, 2, 0.8f, 0.6f, 0.6f, 0.3f));
            GeneratorMainSystemConfig.Add(ESpectrType.K, new StarSystemSetting(5, 4, 1, 2, 0.8f, 0.7f, 0.8f, 0.2f));
            GeneratorMainSystemConfig.Add(ESpectrType.M, new StarSystemSetting(4, 12, 1, 2, 0.7f, 0.8f, 0.6f, 0.3f));
            GeneratorMainSystemConfig.Add(ESpectrType.O, new StarSystemSetting(10, 14, 4, 2, 0.9f, 0.3f, 0.9f, 0.8f));

            // Don't seems called at all
            GeneratorMainSystemConfig.Add(ESpectrType.X, new StarSystemSetting(0, 0, 0, 0, 0f, 0f, 0f, 0f));
        }

        public class StarSystemSetting {
            public int MaxPlanetNb;
            public int MaxMoonNb;

            public int JumpOrbitPlanetIndex;
            public int JumpOrbitMoonIndex;

            // the higher the spacier the system will be 
            public int ChanceJumpOrbitPlanets;
            public int ChanceJumpOrbitMoons;

            public float ChancePlanet;
            public float ChanceGasGiant;

            // Moon
            public float ChanceMoon;
            public float ChanceMoonTelluric;

            public StarSystemSetting(
                int maxPlanetNb,
                int maxMoonNb,
                int jumpOrbitPlanetIndex,
                int jumpOrbitMoonIndex,
                float chancePlanet,
                float chanceGasGiant,
                float chanceMoon,
                float chanceMoonTelluric) {
                MaxPlanetNb = maxPlanetNb;
                MaxMoonNb = maxMoonNb;
                JumpOrbitPlanetIndex = jumpOrbitPlanetIndex;
                JumpOrbitMoonIndex = jumpOrbitMoonIndex;
                ChancePlanet = chancePlanet;
                ChanceGasGiant = chanceGasGiant;
                ChanceMoon = chanceMoon;
                ChanceMoonTelluric = chanceMoonTelluric;
            }
        }

        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.star-system-generation");

            //Adding the Logger
            Logger = new ManualLogSource("PatchForStarSystemGeneration");
            BepInEx.Logging.Logger.Sources.Add(Logger);

            List<float> _orbitRadiusArrayMoonList = new List<float>();
            List<float> _orbitRadiusArrayPlanetList = new List<float>();
            
            // Orbits for the moons planets
            _orbitRadiusArrayMoonList.Add(0.048125f);
            _orbitRadiusArrayMoonList.Add(0.06015625f);
            _orbitRadiusArrayMoonList.Add(0.0751953125f);
            _orbitRadiusArrayMoonList.Add(0.09399414063f);
            _orbitRadiusArrayMoonList.Add(0.1174926758f);
            _orbitRadiusArrayMoonList.Add(0.1468658447f);
            _orbitRadiusArrayMoonList.Add(0.1835823059f);
            _orbitRadiusArrayMoonList.Add(0.2294778824f);
            _orbitRadiusArrayMoonList.Add(0.286847353f);
            _orbitRadiusArrayMoonList.Add(0.3585591912f);
            _orbitRadiusArrayMoonList.Add(0.448198989f);
            _orbitRadiusArrayMoonList.Add(0.5602487363f);
            _orbitRadiusArrayMoonList.Add(0.7003109204f);
            _orbitRadiusArrayMoonList.Add(0.8753886505f);
            _orbitRadiusArrayMoonList.Add(1.094235813f);
            _orbitRadiusArrayMoonList.Add(1.367794766f);
            _orbitRadiusArrayMoonList.Add(1.709743458f);
            _orbitRadiusArrayMoonList.Add(2.137179322f);
            _orbitRadiusArrayMoonList.Add(2.671474153f);
            _orbitRadiusArrayMoonList.Add(3.339342691f);
            _orbitRadiusArrayMoonList.Add(4.174178364f);

            // Orbits for the planets
            _orbitRadiusArrayPlanetList.Add(0f); // star orbit
            _orbitRadiusArrayPlanetList.Add(0.3f);
            _orbitRadiusArrayPlanetList.Add(0.7f);
            _orbitRadiusArrayPlanetList.Add(1.1f);
            _orbitRadiusArrayPlanetList.Add(1.5f);
            _orbitRadiusArrayPlanetList.Add(1.9f);
            _orbitRadiusArrayPlanetList.Add(2.3f);
            _orbitRadiusArrayPlanetList.Add(3.5f);
            _orbitRadiusArrayPlanetList.Add(5.3f);
            _orbitRadiusArrayPlanetList.Add(7.7f);
            _orbitRadiusArrayPlanetList.Add(10.8f);
            _orbitRadiusArrayPlanetList.Add(14.7f);
            _orbitRadiusArrayPlanetList.Add(19.5f);
            _orbitRadiusArrayPlanetList.Add(25.3f);
            _orbitRadiusArrayPlanetList.Add(32.2f);
            _orbitRadiusArrayPlanetList.Add(40.3f);
            _orbitRadiusArrayPlanetList.Add(49.7f);
            _orbitRadiusArrayPlanetList.Add(60.5f);

            OrbitRadiusArrayMoonsNb  = Config.Bind("galactic-scale-systems",
                "OrbitRadiusArrayMoonsNb",
                20,
                "The size for the array of orbits for moons");

            OrbitRadiusArrayPlanetNb  = Config.Bind("galactic-scale-systems",
                "OrbitRadiusArrayPlanetNb",
                16,
                "The size for the array of orbits for planets");
            
            // use custom orbit for planets
            UseCustomOrbitRadiusArray= Config.Bind("galactic-scale-systems",
           "CustomOrbitRadiusArray",
           false,
           "turn it to true to use your own custom orbit array for planet --> don't forget to update the OrbitRadiusArrayPlanetNb value accordingly ( +1 will be added anyway, the first orbit is always the star's one ^^' ");

            // use custom orbit for moons
            UseCustomOrbitRadiusArrayMoons= Config.Bind("galactic-scale-systems",
             "CustomOrbitRadiusArray",
             false,
             "turn it to true to use your own custom orbit array for moons --> don't forget to update the OrbitRadiusArrayMoonsNb value accordingly");
            
            CustomOrbitRadiusArrayPlanets = Config.Bind("galactic-scale-systems",
                "CustomOrbitRadiusArrayPlanets", 
                new []{0f,1f,2.5f},
                "Custom Array for the value in UA for the orbits of the planets");
            
            CustomOrbitRadiusArrayMoons = Config.Bind("galactic-scale-systems",
                "CustomOrbitRadiusArrayMoon", 
                new []{0f,1f,2.5f},
                "Custom Array for the value in UA for the orbits of the moons");
            
            //nb of planet + star
            OrbitRadiusPlanetArray = new float[OrbitRadiusArrayPlanetNb.Value + 1];
            OrbitRadiusPlanetArray = !UseCustomOrbitRadiusArray.Value ? _orbitRadiusArrayPlanetList.ToArray() : CustomOrbitRadiusArrayPlanets.Value;
            OrbitRadiusArrayMoons = new float[OrbitRadiusArrayMoonsNb.Value];
            OrbitRadiusArrayMoons = !UseCustomOrbitRadiusArrayMoons.Value ? _orbitRadiusArrayMoonList.ToArray() : CustomOrbitRadiusArrayMoons.Value;

            StartingSystemPlanetNb = Config.Bind("galactic-scale-systems",
                "StartingSystemPlanetNb",
                30,
                "The Maximum Number of planet in the starting system -- not used yet");
            
            MaxOrbitInclination  = Config.Bind("galactic-scale-systems",
                "MaxOrbitInclination",
                35,
                "maximum absolute angle value for the Inclination of the orbits");
      
            MoonOrbitInclinationFactor  = Config.Bind("galactic-scale-systems",
                "MoonOrbitInclinationFactor",
                2.2f,
                "If it's a moon the inclination will be multiplied by that factor");

            NeutronStarOrbitInclinationFactor  = Config.Bind("galactic-scale-systems",
                "NeutronStarOrbitInclinationFactor",
                1.3f,
                "If in a neutron star system the inclination will be multiplied by that factor");

            ChancePlanetLaySide  = Config.Bind("galactic-scale-systems",
                "ChancePlanetLaySide",
                0.04f,
                "Chance of a planet to be on a rolling orbit --> laying on it's side");

            LaySideBaseAngle = Config.Bind("galactic-scale-systems",
                "LaySideBaseAngle",
                20f,
                "Base angle Value used for the LaySide");

            LaySideAddingAngle  = Config.Bind("galactic-scale-systems",
                "LaySideAddingAngle",
                70f,
                "Angle Value used to add some variation on the LaySide planets");

            ChanceBigObliquity  = Config.Bind("galactic-scale-systems",
                "ChanceBigObliquity",
                0.1f,
                "Chance for the planet to have an high obliquity value");

            BigObliquityBaseAngle = Config.Bind("galactic-scale-systems",
                "BigObliquityBaseAngle",
                40f,
                "Base Angle value to have an high obliquity value");

            BigObliquityAddingAngle = Config.Bind("galactic-scale-systems",
                "BigObliquityAddingAngle",
                30f,
                "Angle Value used to add some variation on the high Obliquity");

            StandardObliquityAngle  = Config.Bind("galactic-scale-systems",
                "StandardObliquityAngle",
                30f,
                "Base Angle value to use for the obliquity of the planets, it will be the most commonly used");

            RotationPeriodBaseTime  = Config.Bind("galactic-scale-systems",
                "RotationPeriodBaseTime",
                400f,
                "Base value to define the rotation period");

            RotationPeriodVariabilityFactor  = Config.Bind("galactic-scale-systems",
                "RotationPeriodVariabilityFactor",
                1000f,
                "Value used to add some variation ( by default : value between 0-1000 + base value = final value ) ");

            ChanceTidalLock = Config.Bind("galactic-scale-systems",
                "ChanceTidalLock",
                0.1f,
                "Chance for a planet to be tidally locked");

            ChanceTidalLock1 = Config.Bind("galactic-scale-systems",
                "ChanceTidalLock1",
                0.04f,
                "Chance for a planet to be tidally locked --> internal type : TidalLock1");

            ChanceTidalLock2 = Config.Bind("galactic-scale-systems",
                "ChanceTidalLock2",
                0.07f,
                "Chance for a planet to be tidally locked --> internal type : TidalLock2");

            ChanceRetrogradeOrbit = Config.Bind("galactic-scale-systems",
                "ChanceRetrogradeOrbit",
                0.05f,
                "Chance for a planet to have a retrograde orbit");

            HabitabilityBaseConstant  = Config.Bind("galactic-scale-systems",
                "HabitabilityBaseConstant",
                1000f,
                "Value used as a base internally --> Not usefull in that context ");
            
            HabitableRadiusAreaBaseline  = Config.Bind("galactic-scale-systems",
                "HabitableRadiusAreaBaseline",
                0.2f,
                "Value to define the width of the habitability area of a star : star.habitableRadius + / - HabitableRadiusAreaBaseline --> meaning for a value of 0.2AU the area is 0.4AU wide");
            
            ChanceBeingHabitable  = Config.Bind("galactic-scale-systems",
                "ChanceBeingHabitable",
                0.4f,
                "Chance for a planet in the habitability zone of a star to actually be habitable");

            VolcanoPlanetDistanceRatio  = Config.Bind("galactic-scale-systems",
                "VolcanoPlanetDistanceRatio",
                0.3f,
                "if planet.distance / star.habitableRadius is less than that --> the planet will be a volcano planet");

            IcePlanetDistanceRatio  = Config.Bind("galactic-scale-systems",
                "IcePlanetDistanceRatio",
                1.2f,
                "if planet.distance / star.habitableRadius is more than that --> the planet will be an ice planet");

            BaseTelluricSize  = Config.Bind("galactic-scale-systems",
                "BaseTelluricSize",
                280f,
                "Base Telluric planet Size  -- Not Advised to modify YET");

            MinTelluricSize  = Config.Bind("galactic-scale-systems",
                "MinTelluricSize",
                80f,
                "Min Value Telluric planet Size. Should be BaseTelluricSize - BaseTelluricSizeVariationFactor  -- Not Advised to modify YET");

            MaxTelluricSize  = Config.Bind("galactic-scale-systems",
                "MaxTelluricSize",
                480f,
                "Max Value Telluric planet Size --> more that than CAN break and is not supported YET. Should be BaseTelluricSize + BaseTelluricSizeVariationFactor  -- Not Advised to modify YET");

            BaseGasGiantSize  = Config.Bind("galactic-scale-systems",
                "BaseGasGiantSize",
                2000f,
                "Base Gas Giant Size  -- Not Advised to modify YET");

            BaseTelluricSizeVariationFactor  = Config.Bind("galactic-scale-systems",
                "BaseTelluricSizeVariationFactor",
                200f,
                "Used to create variation on the planet size : help defining the min & max size for a Telluric planet-- Not Advised to modify YET");

            BaseGasGiantSizeVariationFactor  = Config.Bind("galactic-scale-systems",
                "BaseGasGiantSizeVariationFactor",
                1200f,
                "Used to create variation on the planet size : help defining the min & max size for a gas giant --  -- Not Advised to modify YET");


            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGen));
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) {
                Logger.Log(logLevel, data);
            }
        }
    }
}