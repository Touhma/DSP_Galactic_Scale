using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("touhma.dsp.galactic-scale.star-system-generation", "Galactic Scale Plug-In - Star System Generation",
        "1.0.0.0")]
    public class PatchForStarSystemGeneration : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        public static int StartingSystemPlanetNb = 30;

        // Settings for the algorithms
        public static int OrbitRadiusArrayMoonsNb = 20;
        public static int OrbitRadiusArrayPlanetNb = 16;
        
        public static int MaxOrbitInclination = 35;
        // Increase the inclination if it's a moon
        public static float MoonOrbitInclinationFactor = 2.2f;
        
        // Increase the inclination if around a neutron star
        public static float NeutronStarOrbitInclinationFactor = 1.3f;
        
        // Chance that the planet is "rolling" on it's orbit
        public static float ChancePlanetLaySide = 0.04f;
        
        //ObliquityAngle for rolling : something between 70 -> 90  and -70 -> -90 by default
        public static float LaySideBaseAngle = 20f;
        public static float LaySideAddingAngle = 70f;
        
        //between 30 -> 70 and -30 -> -70
        public static float ChanceBigObliquity = 0.1f;
        public static float BigObliquityBaseAngle = 40f;
        public static float BigObliquityAddingAngle = 30f;
        
        // Between 30 -> -30
        public static float StandardObliquityAngle = 30f;
        
        
        // Minimum rotation period
        public static float RotationPeriodBaseTime = 400;
        // Can be between 0 -> 1000
        public static float RotationPeriodVariabilityFactor = 1000;
        
        //Tidal Lock
        public static float ChanceTidalLock = 0.10f;
        public static float ChanceTidalLock1 = 0.04f;
        public static float ChanceTidalLock2 = 0.07f;
        
        //Chance retrograde orbit
        public static float ChanceRetrogradeOrbit = 0.05f;
        
        
        //Habitability Related : 
        public static float HabitabilityBaseConstant = 1000f;
        
        // the habitability area : for 1 habitability radius of the star & a value of 0.2f : 0.8 -> 1.2 
        public static float HabitableRadiusAreaBaseline = 0.2f;
        
        //Chance of being habitable if in the habitability zone of the star
        public static float ChanceBeingHabitable = 0.4f;
        
        // if the ratio planet.distance & star.habitable radius is less than that : Volcano planet
        public static float VolcanoPlanetDistanceRatio = 0.3f;
        // if the ratio planet.distance & star.habitable radius is more than that : Ice planet
        public static float IcePlanetDistanceRatio = 1.2f;
        
        
        public static float BaseTelluricSize = 280f;
        public static float MinTelluricSize = 80f;
        public static float MaxTelluricSize = 480f;
        public static float BaseGasGiantSize = 2000f;
        // Min : 80, Max : 480
        public static float BaseTelluricSizeVariationFactor = 200f;
        // Min : 800, Max : 3200
        public static float BaseGasGiantSizeVariationFactor = 1200f;
        
        
        
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
            GeneratorSpecialsSystemConfig.Add(EStarType.BlackHole, new StarSystemSetting(1, 1, 4, 1, 0.5f, 0.5f, 0.5f, 0.5f));
            GeneratorSpecialsSystemConfig.Add(EStarType.NeutronStar, new StarSystemSetting(1, 1, 4, 2, 0.8f, 0.2f,0.5f, 0.5f));
            GeneratorSpecialsSystemConfig.Add(EStarType.WhiteDwarf, new StarSystemSetting(2, 4, 3, 3, 0.6f, 0.6f,0.7f,0.2f));
            GeneratorSpecialsSystemConfig.Add(EStarType.GiantStar, new StarSystemSetting(6, 6, 5, 2, 0.8f, 0.8f,0.8f,0.2f));
            
            
            //Order of frequency Vanilla : B -> G -> K -> F -> M -> A -> O
            GeneratorMainSystemConfig.Add(ESpectrType.A, new StarSystemSetting(9, 10, 3, 3, 0.7f, 0.4f,0.6f,0.5f));
            GeneratorMainSystemConfig.Add(ESpectrType.B, new StarSystemSetting(6, 6, 5, 2, 0.8f, 0.3f,0.8f,0.5f));
            GeneratorMainSystemConfig.Add(ESpectrType.F, new StarSystemSetting(6, 6, 3, 2, 0.8f, 0.5f,0.8f,0.2f));
            GeneratorMainSystemConfig.Add(ESpectrType.G, new StarSystemSetting(3, 3, 2, 2, 0.8f, 0.6f,0.6f,0.3f));
            GeneratorMainSystemConfig.Add(ESpectrType.K, new StarSystemSetting(5, 4, 1, 2, 0.8f, 0.7f,0.8f,0.2f));
            GeneratorMainSystemConfig.Add(ESpectrType.M, new StarSystemSetting(4, 12, 1, 2, 0.7f, 0.8f,0.6f,0.3f));
            GeneratorMainSystemConfig.Add(ESpectrType.O, new StarSystemSetting(10, 14, 4, 2, 0.9f, 0.3f,0.9f,0.8f));
            
            // Don't seems called at all
            GeneratorMainSystemConfig.Add(ESpectrType.X, new StarSystemSetting(0, 0, 0, 0, 0f, 0f,0f,0f));
             
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


            OrbitRadiusArrayMoons = new float[OrbitRadiusArrayMoonsNb];

            //nb of planet + star
            OrbitRadiusPlanetArray = new float[OrbitRadiusArrayPlanetNb + 1];

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

            OrbitRadiusArrayMoons = _orbitRadiusArrayMoonList.ToArray();

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

            OrbitRadiusPlanetArray = _orbitRadiusArrayPlanetList.ToArray();


            /*
            _orbitRadiusArrayPlanetList.Add(0f); // sun
            _orbitRadiusArrayPlanetList.Add(0.4f);
            _orbitRadiusArrayPlanetList.Add(0.7f);
            _orbitRadiusArrayPlanetList.Add(1f); //--earth -- 1.40
            _orbitRadiusArrayPlanetList.Add(1.5f); // mars
            _orbitRadiusArrayPlanetList.Add(5.2f); // jupiter
            _orbitRadiusArrayPlanetList.Add(9.5f); // saturn
            _orbitRadiusArrayPlanetList.Add(19.2f); // uranus
            _orbitRadiusArrayPlanetList.Add(30.1f); // Neptune
            _orbitRadiusArrayPlanetList.Add(39f); // Pluto
            _orbitRadiusArrayPlanetList.Add(43.13f); // Haumea 
            _orbitRadiusArrayPlanetList.Add(45.79f); // Makemake 
            _orbitRadiusArrayPlanetList.Add(68f); // Eris
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
            _orbitRadiusArrayPlanetList.Add(50f);
*/


            /*
                  Random randomOrbitRadius = new Random();
            _orbitRadiusArray[0] = _minOrbitRadius + (float) randomOrbitRadius.NextDouble() * _orbitMinIncrement;
            for (int indexOrbitRad = 1; indexOrbitRad < _orbitRadiusArray.Length; indexOrbitRad++) {
                _orbitRadiusArray[indexOrbitRad] = (float) (_orbitRadiusArray[(indexOrbitRad - 1)] +
                                                            (randomOrbitRadius.NextDouble() * _orbitMaxIncrement +
                                                             _orbitMinIncrement));
                //UnityEngine.Debug.Log("orbitRadius-1 " + indexOrbitRad + " :" + _orbitRadiusArray[(indexOrbitRad - 1)]);
                UnityEngine.Debug.Log("orbitRadius" + indexOrbitRad + " :" + _orbitRadiusArray[indexOrbitRad]);
            }
*/

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