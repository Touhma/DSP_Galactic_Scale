using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GalacticScale.Scripts.PatchPlanetSize  {
    [BepInPlugin("touhma.dsp.galactic-scale.planet-size", "Galactic Scale Plug-In", "1.0.0.0")]
    public class PatchForPlanetSize  : BaseUnityPlugin{
        public new static ManualLogSource Logger;
        
        public static float scaleFactor = 1;
        
        public static bool DebugPlanetAlgorithm1 = false;
        public static bool DebugPlanetFactory = false;
        public static bool DebugPlanetFactoryDeep = false;
        public static bool DebugPlanetRawData = false;
        public static bool DebugPlanetData = false;
        public static bool DebugPlanetModelingManager = false;
        public static bool DebugPlanetModelingManagerDeep = false;

        internal void Awake() {
            var harmony = new Harmony("touhma.dsp.galactic-scale.planet-size");
            
            //PatchForPlanetSize
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAlgorithm1));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
        }
        
        public static void Debug(object data, LogLevel logLevel , bool isActive) {
            if (isActive) {
                Logger.Log(logLevel, data);
            }
        }
    }
}