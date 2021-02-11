using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [BepInPlugin("touhma.dsp.galactic-scale.star-system-generation", "Galactic Scale Plug-In", "1.0.0.0")]
    public class PatchForStarSystemGeneration : BaseUnityPlugin{
        public new static ManualLogSource Logger;
        
        public static int StartingSystemPlanetNb = 30;
        public static float OrbitMaxIncrement = 1f;
        public static float OrbitMinIncrement = 0.5f;
        public static float MINOrbitRadius = 0.4f;
        public static float[] OrbitRadiusArray;

        public static bool DebugPlanetGen = false;
        public static bool DebugStarGen = false;
        
        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.star-system-generation");
            
            //Adding the Logger
            Logger = new ManualLogSource("PatchForStarSystemGeneration");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            
             OrbitRadiusArray = new float[StartingSystemPlanetNb];
            OrbitRadiusArray = new float[StartingSystemPlanetNb];

            List<float> _orbitRadiusArrayList = new List<float>();
            // 1 unit environ 0.945 UA
            /*
            _orbitRadiusArrayList.Add(0.423f);
            _orbitRadiusArrayList.Add(0.7407f);
            _orbitRadiusArrayList.Add(1.058f); //--earth
            _orbitRadiusArrayList.Add(1.640f);
            _orbitRadiusArrayList.Add(5.5026f);
            _orbitRadiusArrayList.Add(10.0529f); // saturn
            _orbitRadiusArrayList.Add(20.3174f); // uranus
            _orbitRadiusArrayList.Add(31.7460f); // Neptune
            _orbitRadiusArrayList.Add(40f);
            _orbitRadiusArrayList.Add(41f);
            _orbitRadiusArrayList.Add(42f);
            _orbitRadiusArrayList.Add(43f);
            _orbitRadiusArrayList.Add(44f);
            */
            
            _orbitRadiusArrayList.Add(0f); // sun
            _orbitRadiusArrayList.Add(0.4f);
            _orbitRadiusArrayList.Add(0.7f);
            _orbitRadiusArrayList.Add(1f); //--earth -- 1.40
            _orbitRadiusArrayList.Add(1.5f); // mars
            _orbitRadiusArrayList.Add(5.2f); // jupiter
            _orbitRadiusArrayList.Add(9.5f); // saturn
            _orbitRadiusArrayList.Add(19.2f); // uranus
            _orbitRadiusArrayList.Add(30.1f); // Neptune
            _orbitRadiusArrayList.Add(39f); // Pluto
            _orbitRadiusArrayList.Add(43.13f); // Haumea 
            _orbitRadiusArrayList.Add(45.79f); // Makemake 
            _orbitRadiusArrayList.Add(68f); // Eris
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);
            _orbitRadiusArrayList.Add(50f);

            OrbitRadiusArray = _orbitRadiusArrayList.ToArray();

       
            
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

        public static void Debug(object data, LogLevel logLevel , bool isActive) {
            if (isActive) {
                Logger.Log(logLevel, data);
            }
        }
    }
}