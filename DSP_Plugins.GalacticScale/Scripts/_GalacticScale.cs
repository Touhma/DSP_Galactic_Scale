using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace DSP_Plugin.GalacticScale {
    [BepInPlugin("touhma.dsp.plugins.galactic-scale", "Galactic Scale Plug-In", "1.0.0.0")]
    public class DSP_GalacticScale : BaseUnityPlugin {
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

            var harmony = new Harmony("touhma.dsp.plugins.galactic-scale");
            
            Harmony.CreateAndPatchAll(typeof(Patch));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
        }
    }
}