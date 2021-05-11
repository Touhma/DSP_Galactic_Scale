//using BepInEx;
//using BepInEx.Configuration;
//using BepInEx.Logging;
//using HarmonyLib;

//namespace GalacticScale.Scripts.PatchGalaxySizeSelection {
//    [BepInPlugin("dsp.galactic-scale.galaxy-size-select", "Galactic Scale Plug-In - Galaxy Size Select", "1.0.0.0")]
//    public class PatchForGalaxySizeSelection : BaseUnityPlugin {
//        public new static ManualLogSource Logger;

//        public static ConfigEntry<int> ConfigStarsMax;
//        public static ConfigEntry<int> ConfigStarsMin;

//        internal void Awake() {
//            ConfigStarsMax = Config.Bind("galactic-scale",
//                "MaxStars",
//                1024,
//                "The Maximum Number of stars desired");
//            ConfigStarsMin = Config.Bind("galactic-scale",
//                "MinStars",
//                32,
//                "The Minimum Number of stars desired");

//            var harmony = new Harmony("dsp.galactic-scale.galaxy-size-select");

//            //Adding the Logger
//            Logger = new ManualLogSource("PatchForGalaxySizeSelection");
//            BepInEx.Logging.Logger.Sources.Add(Logger);

//            // PatchsForGalaxySizeSelection
//            //Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
//            //Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
//        }
//    }
//}