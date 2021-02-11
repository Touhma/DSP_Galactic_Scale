using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;


namespace GalacticScale.Scripts.PatchGalaxySizeSelection {
    [BepInPlugin("touhma.dsp.galactic-scale.galaxy-size-select", "Galactic Scale Plug-In", "1.0.0.0")]
    public class PatchForGalaxySizeSelection : BaseUnityPlugin {
        
        public static ConfigEntry<int> ConfigStarsMax;
        public static ConfigEntry<int> ConfigStarsMin;

        internal void Awake() {
            ConfigStarsMax = Config.Bind("galactic-scale",
                "MaxStars",
                1024,
                "The Maximum Number of stars desired");
            ConfigStarsMin = Config.Bind("galactic-scale",
                "MinStars",
                32,
                "The Minimum Number of stars desired");

            var harmony = new Harmony("touhma.dsp.galactic-scale.galaxy-size-select");

            // PatchsForGalaxySizeSelection
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
        }
        
    }
}