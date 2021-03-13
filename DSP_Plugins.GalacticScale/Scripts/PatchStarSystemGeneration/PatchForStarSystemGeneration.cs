using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("touhma.dsp.galactic-scale.star-system-generation", "Galactic Scale Plug-In - Star System Generation",
        "1.0.0.0")]
    public class PatchForStarSystemGeneration : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        public static ConfigEntry<int> StartingSystemMinPlanetTelluricNb;
        public static ConfigEntry<int> StartingSystemMinGasGiantNb;
        public static ConfigEntry<int> StartingSystemMinTelluricMoonNb;
        public static ConfigEntry<int> StartingSystemMinGasGiantMoonNb;

        //public static int StartingSystemPlanetNb = 30;

        // use custom orbit for planets
        public static ConfigEntry<bool> UseCustomOrbitRadiusArrayPlanets;
        public static ConfigEntry<string> CustomOrbitRadiusArrayPlanets;

        // use custom orbit for moons
        public static ConfigEntry<bool> UseCustomOrbitRadiusArrayMoons;
        public static ConfigEntry<string> CustomOrbitRadiusArrayMoons;

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


        public static ConfigEntry<string> CustomParamsForBlackHole;
        public static ConfigEntry<string> CustomParamsForNeutronStar;
        public static ConfigEntry<string> CustomParamsForWhiteDwarf;
        public static ConfigEntry<string> CustomParamsForGiantStar;
        public static ConfigEntry<string> CustomParamsForClassA;
        public static ConfigEntry<string> CustomParamsForClassB;
        public static ConfigEntry<string> CustomParamsForClassF;
        public static ConfigEntry<string> CustomParamsForClassG;
        public static ConfigEntry<string> CustomParamsForClassK;
        public static ConfigEntry<string> CustomParamsForClassM;
        public static ConfigEntry<string> CustomParamsForClassO;
        public static ConfigEntry<string> CustomParamsForClassX;
        public static ConfigEntry<bool> UseCustomParamsForBlackHole;
        public static ConfigEntry<bool> UseCustomParamsForNeutronStar;
        public static ConfigEntry<bool> UseCustomParamsForWhiteDwarf;
        public static ConfigEntry<bool> UseCustomParamsForGiantStar;
        public static ConfigEntry<bool> UseCustomParamsForClassA;
        public static ConfigEntry<bool> UseCustomParamsForClassB;
        public static ConfigEntry<bool> UseCustomParamsForClassF;
        public static ConfigEntry<bool> UseCustomParamsForClassG;
        public static ConfigEntry<bool> UseCustomParamsForClassK;
        public static ConfigEntry<bool> UseCustomParamsForClassM;
        public static ConfigEntry<bool> UseCustomParamsForClassO;
        public static ConfigEntry<bool> UseCustomParamsForClassX;


        //Generator Settings
        public static Dictionary<EStarType, StarSystemSetting> GeneratorSpecialsSystemConfig =
            new Dictionary<EStarType, StarSystemSetting>();

        public static Dictionary<ESpectrType, StarSystemSetting> GeneratorMainSystemConfig =
            new Dictionary<ESpectrType, StarSystemSetting>();


        // Internal Variables

        public static float[] OrbitRadiusArrayMoons;
        public static float[] OrbitRadiusPlanetArray;

        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = false;
        public static bool DebugStarGenDeep = false;
        public static bool DebugStarNamingGen = false;

        public static ConfigEntry<bool> EnableCustomStarAlgorithm;

        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.star-system-generation");

            //Adding the Logger
            Logger = new ManualLogSource("PatchForStarSystemGeneration");
            BepInEx.Logging.Logger.Sources.Add(Logger);

            var _orbitRadiusArrayMoonList = new List<float>();
            var _orbitRadiusArrayPlanetList = new List<float>();

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
            _orbitRadiusArrayPlanetList.Add(0f);// star orbit
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


            EnableCustomStarAlgorithm = Config.Bind("galactic-scale-systems",
                "EnableCustomStarAlgorithm",
                true,
                "Enable or not the custom algorithm");

            OrbitRadiusArrayMoonsNb = Config.Bind("galactic-scale-systems",
                "OrbitRadiusArrayMoonsNb",
                20,
                "The size for the array of orbits for moons");

            OrbitRadiusArrayPlanetNb = Config.Bind("galactic-scale-systems",
                "OrbitRadiusArrayPlanetNb",
                17,
                "The size for the array of orbits for planets");

            // use custom orbit for planets
            UseCustomOrbitRadiusArrayPlanets = Config.Bind("galactic-scale-systems",
                "UseCustomOrbitRadiusArrayPlanets",
                false,
                "turn it to true to use your own custom orbit array for planet --> don't forget to update the OrbitRadiusArrayPlanetNb value accordingly ( +1 will be added anyway, the first orbit is always the star's one ^^' ");

            // use custom orbit for moons
            UseCustomOrbitRadiusArrayMoons = Config.Bind("galactic-scale-systems",
                "UseCustomOrbitRadiusArrayMoons",
                false,
                "turn it to true to use your own custom orbit array for moons --> don't forget to update the OrbitRadiusArrayMoonsNb value accordingly");

            CustomOrbitRadiusArrayPlanets = Config.Bind("galactic-scale-systems",
                "CustomOrbitRadiusArrayPlanets",
                "0,0.3,0.7,1.1,1.5,1.9,2.3,3.5,5.3,7.7,10.8,14.7,19.5,25.3,32.2,40.3,49.7,60.5",
                "Custom Array for the value in UA for the orbits of the planets");

            CustomOrbitRadiusArrayMoons = Config.Bind("galactic-scale-systems",
                "CustomOrbitRadiusArrayMoon",
                "0.048125,0.06015625,0.0751953125,0.09399414063,0.1174926758,0.1468658447,0.1835823059,0.2294778824,0.286847353,0.3585591912,0.448198989,0.5602487363,0.7003109204,0.8753886505,1.094235813,1.367794766,1.709743458,2.137179322,2.671474153,3.339342691,4.174178364",
                "Custom Array for the value in UA for the orbits of the moons");

            //nb of planet + star
            OrbitRadiusPlanetArray = new float[OrbitRadiusArrayPlanetNb.Value + 1];
            OrbitRadiusPlanetArray = !UseCustomOrbitRadiusArrayPlanets.Value
                ? _orbitRadiusArrayPlanetList.ToArray()
                : Array.ConvertAll(CustomOrbitRadiusArrayPlanets.Value.Split(','), float.Parse);

            OrbitRadiusArrayMoons = new float[OrbitRadiusArrayMoonsNb.Value];
            OrbitRadiusArrayMoons = !UseCustomOrbitRadiusArrayMoons.Value
                ? _orbitRadiusArrayMoonList.ToArray()
                : Array.ConvertAll(CustomOrbitRadiusArrayMoons.Value.Split(','), float.Parse);

            UseCustomParamsForBlackHole = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForBlackHole",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            ;
            CustomParamsForBlackHole = Config.Bind("galactic-scale-systems",
                "CustomParamsForBlackHole",
                "1,0,6,0,4,0.9,2,0.9,0.5,0.5,0.5,0.5",
                "Custom Params for the specified system, \n " +
                "int maxTelluricNb : nb max of solid planets in the system in total,\n " +
                "int maxGasGiantNb : nb max of Gas Giants in the system in total,\n" +
                "int maxMoonTelluricNb : nb max of moons around solid planets in the system in total,\n " +
                "int maxMoonGasGiantNb : nb max of moons around gas giants in the system in total,\n" +
                "int jumpOrbitPlanetMax :the first planet of the host star will be on this orbit,\n" +
                "float chanceJumpOrbitPlanets : lower = denser systems, higher = further away from the star,\n" +
                "int jumpOrbitMoonMax: the first moon of the host planet will be on this orbit,\n" +
                "float chanceJumpOrbitMoons: lower = denser systems, higher = further away from the host planet,\n" +
                "float chanceTelluricPlanet: chance for a telluric planet to spawn,\n" +
                "float chanceGasGiant: chance for a gas giant to spawn,\n" +
                "float chanceGasGiantMoon : chance for a moon around a gasgiant to spawn,\n" +
                "float chanceMoonTelluric : chance for a moon around a telluric planet to spawn  \n "
            );
            if (!UseCustomParamsForBlackHole.Value)
                GeneratorSpecialsSystemConfig.Add(EStarType.BlackHole,
                    new StarSystemSetting(1, 0, 6, 0, 4, 0.9f, 2, 0.9f, 0.5f, 0.5f, 0.5f, 0.5f));
            else
                GeneratorSpecialsSystemConfig.Add(EStarType.BlackHole,
                    ParseCustomStarSystemSetting(CustomParamsForBlackHole.Value));

            UseCustomParamsForNeutronStar = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForNeutronStar",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForNeutronStar = Config.Bind("galactic-scale-systems",
                "CustomParamsForNeutronStar",
                "1,1,1,1,4,0.1,2,0.1,0.8,0.2,0.5,0.5",
                "Same for neutron star");
            if (!UseCustomParamsForNeutronStar.Value)
                GeneratorSpecialsSystemConfig.Add(EStarType.NeutronStar,
                    new StarSystemSetting(1, 1, 1, 1, 4, 0.1f, 2, 0.1f, 0.8f, 0.2f, 0.5f, 0.5f));
            else
                GeneratorSpecialsSystemConfig.Add(EStarType.NeutronStar,
                    ParseCustomStarSystemSetting(CustomParamsForNeutronStar.Value));

            UseCustomParamsForWhiteDwarf = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForWhiteDwarf",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForWhiteDwarf = Config.Bind("galactic-scale-systems",
                "CustomParamsForWhiteDwarf",
                "2,1,1,3,3,0.5,3,0.5,0.6,0.6,0.7,0.2",
                "Same for neutron star");
            if (!UseCustomParamsForWhiteDwarf.Value)
                GeneratorSpecialsSystemConfig.Add(EStarType.WhiteDwarf,
                    new StarSystemSetting(2, 1, 1, 3, 3, 0.5f, 3, 0.5f, 0.6f, 0.6f, 0.7f, 0.2f));
            else
                GeneratorSpecialsSystemConfig.Add(EStarType.WhiteDwarf,
                    ParseCustomStarSystemSetting(CustomParamsForWhiteDwarf.Value));

            UseCustomParamsForGiantStar = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForGiantStar",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForGiantStar = Config.Bind("galactic-scale-systems",
                "CustomParamsForGiantStar",
                "2,2,1,6,3,0.8,3,0.8,0.6,0.6,0.7,0.2",
                "Same for neutron star");
            if (!UseCustomParamsForGiantStar.Value)
                GeneratorSpecialsSystemConfig.Add(EStarType.GiantStar,
                    new StarSystemSetting(2, 2, 1, 6, 3, 0.8f, 3, 0.8f, 0.6f, 0.6f, 0.7f, 0.2f));
            else
                GeneratorSpecialsSystemConfig.Add(EStarType.GiantStar,
                    ParseCustomStarSystemSetting(CustomParamsForGiantStar.Value));

            UseCustomParamsForClassA = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassA",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassA = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassA",
                "5,4,4,8,3,0.3,3,0.4,0.7,0.4,0.6,0.5",
                "Same for Class A");
            if (!UseCustomParamsForClassA.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.A,
                    new StarSystemSetting(5, 4, 4, 8, 3, 0.3f, 3, 0.4f, 0.7f, 0.4f, 0.6f, 0.5f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.A,
                    ParseCustomStarSystemSetting(CustomParamsForClassA.Value));

            UseCustomParamsForClassB = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassB",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassB = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassB",
                "5,2,5,2,5,0.5,2,0.6,0.8,0.5,0.8,0.5",
                "Same for Class B");
            if (!UseCustomParamsForClassB.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.B,
                    new StarSystemSetting(5, 2, 5, 2, 5, 0.5f, 2, 0.6f, 0.8f, 0.5f, 0.8f, 0.5f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.B,
                    ParseCustomStarSystemSetting(CustomParamsForClassB.Value));

            UseCustomParamsForClassF = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassF",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassF = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassF",
                "4,3,2,4,3,0.6,2,0.4,0.8,0.7,0.8,0.2",
                "Same for Class F");
            if (!UseCustomParamsForClassF.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.F,
                    new StarSystemSetting(4, 3, 2, 4, 3, 0.6f, 2, 0.4f, 0.8f, 0.7f, 0.8f, 0.2f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.F,
                    ParseCustomStarSystemSetting(CustomParamsForClassF.Value));

            UseCustomParamsForClassG = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassG",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassG = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassG",
                "3,3,1,2,3,0.8,2,0.6,0.8,0.6,0.7,0.3",
                "Same for Class G");
            if (!UseCustomParamsForClassG.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.G,
                    new StarSystemSetting(3, 3, 1, 2, 3, 0.8f, 2, 0.6f, 0.8f, 0.7f, 0.6f, 0.3f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.G,
                    ParseCustomStarSystemSetting(CustomParamsForClassG.Value));

            UseCustomParamsForClassK = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassK",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassK = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassK",
                "4,3,1,4,2,0.5,2,0.5,0.8,0.8,0.8,0.2",
                "Same for Class K");
            if (!UseCustomParamsForClassK.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.K,
                    new StarSystemSetting(4, 3, 1, 4, 2, 0.5f, 2, 0.5f, 0.8f, 0.8f, 0.8f, 0.2f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.K,
                    ParseCustomStarSystemSetting(CustomParamsForClassK.Value));

            UseCustomParamsForClassM = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassM",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassM = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassM",
                "1,4,1,11,1,0.7,2,0.2,0.7,0.8,0.6,0.3",
                "Same for Class M");
            if (!UseCustomParamsForClassM.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.M,
                    new StarSystemSetting(1, 4, 1, 11, 1, 0.7f, 2, 0.2f, 0.7f, 0.8f, 0.6f, 0.3f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.M,
                    ParseCustomStarSystemSetting(CustomParamsForClassM.Value));

            UseCustomParamsForClassO = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassO",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassO = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassO",
                "7,3,6,8,4,0.2,2,0.2,0.9,0.5,0.9,0.8",
                "Same for Class O");
            if (!UseCustomParamsForClassO.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.O,
                    new StarSystemSetting(7, 3, 6, 8, 4, 0.2f, 2, 0.2f, 0.9f, 0.5f, 0.9f, 0.8f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.O,
                    ParseCustomStarSystemSetting(CustomParamsForClassO.Value));

            UseCustomParamsForClassX = Config.Bind("galactic-scale-systems",
                "UseCustomParamsForClassX",
                false,
                "Turn it to true to use your own custom config for the specified system Class");
            CustomParamsForClassX = Config.Bind("galactic-scale-systems",
                "CustomParamsForClassX",
                "0,0,0,0,0,0.0,0,0.0,0,0,0,0",
                "Same for Class X --> not used yet AFAIK");
            if (!UseCustomParamsForClassX.Value)
                GeneratorMainSystemConfig.Add(ESpectrType.X,
                    new StarSystemSetting(0, 0, 0, 0, 0, 0.0f, 0, 0.0f, 0f, 0f, 0f, 0f));
            else
                GeneratorMainSystemConfig.Add(ESpectrType.X,
                    ParseCustomStarSystemSetting(CustomParamsForClassX.Value));


            StartingSystemMinPlanetTelluricNb = Config.Bind("galactic-scale-systems",
                "StartingSystemMinPlanetTelluricNb",
                2,
                "The Minimum Number of Telluric planet in the starting system -- should not be less than 2");
            StartingSystemMinGasGiantNb = Config.Bind("galactic-scale-systems",
                "StartingSystemMinGasGiantNb",
                1,
                "The Minimum Number of GasGiants in the starting system -- should not be less than 1");
            StartingSystemMinTelluricMoonNb = Config.Bind("galactic-scale-systems",
                "StartingSystemMinTelluricMoonNb",
                1,
                "The Minimum Number of Moon of Telluric Planets in the starting system -- should not be less than 1");
            StartingSystemMinGasGiantMoonNb = Config.Bind("galactic-scale-systems",
                "StartingSystemMinGasGiantMoonNb",
                1,
                "The Minimum Number of Moons of GasGiant  in the starting system -- should not be less than 1");


            MaxOrbitInclination = Config.Bind("galactic-scale-systems",
                "MaxOrbitInclination",
                35,
                "maximum absolute angle value for the Inclination of the orbits");

            MoonOrbitInclinationFactor = Config.Bind("galactic-scale-systems",
                "MoonOrbitInclinationFactor",
                2.2f,
                "If it's a moon the inclination will be multiplied by that factor");

            NeutronStarOrbitInclinationFactor = Config.Bind("galactic-scale-systems",
                "NeutronStarOrbitInclinationFactor",
                1.3f,
                "If in a neutron star system the inclination will be multiplied by that factor");

            ChancePlanetLaySide = Config.Bind("galactic-scale-systems",
                "ChancePlanetLaySide",
                0.04f,
                "Chance of a planet to be on a rolling orbit --> laying on it's side");

            LaySideBaseAngle = Config.Bind("galactic-scale-systems",
                "LaySideBaseAngle",
                20f,
                "Base angle Value used for the LaySide");

            LaySideAddingAngle = Config.Bind("galactic-scale-systems",
                "LaySideAddingAngle",
                70f,
                "Angle Value used to add some variation on the LaySide planets");

            ChanceBigObliquity = Config.Bind("galactic-scale-systems",
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

            StandardObliquityAngle = Config.Bind("galactic-scale-systems",
                "StandardObliquityAngle",
                30f,
                "Base Angle value to use for the obliquity of the planets, it will be the most commonly used");

            RotationPeriodBaseTime = Config.Bind("galactic-scale-systems",
                "RotationPeriodBaseTime",
                400f,
                "Base value to define the rotation period");

            RotationPeriodVariabilityFactor = Config.Bind("galactic-scale-systems",
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

            HabitabilityBaseConstant = Config.Bind("galactic-scale-systems",
                "HabitabilityBaseConstant",
                1000f,
                "Value used as a base internally --> Not usefull in that context ");

            HabitableRadiusAreaBaseline = Config.Bind("galactic-scale-systems",
                "HabitableRadiusAreaBaseline",
                0.2f,
                "Value to define the width of the habitability area of a star : star.habitableRadius + / - HabitableRadiusAreaBaseline --> meaning for a value of 0.2AU the area is 0.4AU wide");

            ChanceBeingHabitable = Config.Bind("galactic-scale-systems",
                "ChanceBeingHabitable",
                0.4f,
                "Chance for a planet in the habitability zone of a star to actually be habitable");

            VolcanoPlanetDistanceRatio = Config.Bind("galactic-scale-systems",
                "VolcanoPlanetDistanceRatio",
                0.3f,
                "if planet.distance / star.habitableRadius is less than that --> the planet will be a volcano planet");

            IcePlanetDistanceRatio = Config.Bind("galactic-scale-systems",
                "IcePlanetDistanceRatio",
                1.2f,
                "if planet.distance / star.habitableRadius is more than that --> the planet will be an ice planet");

            //Forcing the Custom star Algo if resizing is enabled
            if (PatchSize.EnableResizingFeature.Value || PatchSize.EnableLimitedResizingFeature.Value) EnableCustomStarAlgorithm.Value = true;

            if (EnableCustomStarAlgorithm.Value) {
                Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
                Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGen));
                Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
                Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmapPlanet));
            }
        }

        public static StarSystemSetting ParseCustomStarSystemSetting(string config) {
            var configArray = Array.ConvertAll(config.Split(','), float.Parse);
            return new StarSystemSetting(
                (int) configArray[0],
                (int) configArray[1],
                (int) configArray[2],
                (int) configArray[3],
                (int) configArray[4],
                configArray[5],
                (int) configArray[6],
                configArray[7],
                configArray[8],
                configArray[9],
                configArray[10],
                configArray[11]);
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }

        public class StarSystemSetting {
            public float ChanceGasGiant;

            // Moon
            public float ChanceGasGiantMoon;

            public float ChanceJumpOrbitMoons;
            public float ChanceJumpOrbitPlanets;
            public float ChanceMoonTelluric;
            public float ChanceTelluricPlanet;

            public int JumpOrbitMoonMax;
            public int JumpOrbitPlanetMax;
            public int MaxGasGiantNb;
            public int MaxMoonGasGiantNb;

            public int MaxMoonTelluricNb;
            public int MaxTelluricNb;


            public StarSystemSetting(
                int maxTelluricNb,
                int maxGasGiantNb,
                int maxMoonTelluricNb,
                int maxMoonGasGiantNb,
                int jumpOrbitPlanetMax,
                float chanceJumpOrbitPlanets,
                int jumpOrbitMoonMax,
                float chanceJumpOrbitMoons,
                float chanceTelluricPlanet,
                float chanceGasGiant,
                float chanceGasGiantMoon,
                float chanceMoonTelluric) {
                MaxTelluricNb = maxTelluricNb;
                MaxGasGiantNb = maxGasGiantNb;
                MaxMoonTelluricNb = maxMoonTelluricNb;
                MaxMoonGasGiantNb = maxMoonGasGiantNb;
                JumpOrbitPlanetMax = jumpOrbitPlanetMax;
                ChanceJumpOrbitPlanets = chanceJumpOrbitPlanets;
                JumpOrbitMoonMax = jumpOrbitMoonMax;
                ChanceJumpOrbitMoons = chanceJumpOrbitMoons;
                ChanceTelluricPlanet = chanceTelluricPlanet;
                ChanceGasGiant = chanceGasGiant;
                ChanceGasGiantMoon = chanceGasGiantMoon;
                ChanceMoonTelluric = chanceMoonTelluric;
            }
        }
    }
}