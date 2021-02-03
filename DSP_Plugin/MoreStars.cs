using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.UI;

namespace DSP_Plugin {
    [BepInPlugin("touhma.dsp.plugins.moreStars", "More Stars Plug-In", "1.0.0.0")]
    public class DSP_MoreStars : BaseUnityPlugin {
        private static ConfigEntry<int> _configStarsMax;
        private static ConfigEntry<int> _configStarsMin;
        internal void Awake() {
            _configStarsMax = Config.Bind("MoreStars",   // The section under which the option is shown
                "MaxStars",  // The key of the configuration option in the configuration file
                1024, // The default value
                "The Maximum Number of stars desired");
            _configStarsMin = Config.Bind("MoreStars",   // The section under which the option is shown
                "MinStars",  // The key of the configuration option in the configuration file
                32, // The default value
                "The Minimum Number of stars desired");
            
            var harmony = new Harmony("touhma.dsp.plugins.moreStars");
            Harmony.CreateAndPatchAll(typeof(Patch));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
        }

        [HarmonyPatch(typeof(UIGalaxySelect))]
        private class Patch {
            [HarmonyPostfix]
            [HarmonyPatch("_OnInit")]
            public static void Postfix(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
                ___starCountSlider.maxValue = _configStarsMax.Value;
                ___starCountSlider.minValue = _configStarsMin.Value;
            }

            [HarmonyPrefix]
            [HarmonyPatch("OnStarCountSliderValueChange")]
            public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider,
                ref GameDesc ___gameDesc, float val) {
                int num = (int) (___starCountSlider.value + 0.100000001490116);
                if (num == ___gameDesc.starCount) {
                    return false;
                }

                ___gameDesc.starCount = num;
                __instance.SetStarmapGalaxy();
                return false;
            }
        }

        [HarmonyPatch(typeof(UniverseGen))]
        private class PatchOnUniverseGen {
            [HarmonyPrefix]
            [HarmonyPatch("CreateGalaxy")]
            public static bool CreateGalaxy(GalaxyData __instance, GameDesc gameDesc) {
                return true;
            }
        }
        
        [HarmonyPatch(typeof(StarGen))]
        private class PatchOnStarGen {
            [HarmonyPrefix]
            [HarmonyPatch("CreateBirthStar")]
            public static bool CreateBirthStar(GalaxyData galaxy, int seed) {
                int gSize = galaxy.starCount > 64 ? galaxy.starCount * 4 * 100 : 25600;
                galaxy.astroPoses = new AstroPose[gSize];
                return true;
            }
        }
    }
}