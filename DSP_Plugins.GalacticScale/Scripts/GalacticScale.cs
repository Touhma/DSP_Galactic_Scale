using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.UI;

namespace DSP_Plugin.GalacticScale {
    [BepInPlugin("touhma.dsp.plugins.galactic-scale", "Galactic Scale Plug-In", "1.0.0.0")]
    public class DSP_GalacticScale : BaseUnityPlugin {
        private static ConfigEntry<int> _configStarsMax;
        private static ConfigEntry<int> _configStarsMin;

        internal void Awake() {
            _configStarsMax = Config.Bind("galactic-scale",
                "MaxStars",
                1024,
                "The Maximum Number of stars desired");
            _configStarsMin = Config.Bind("galactic-scale",
                "MinStars",
                32,
                "The Minimum Number of stars desired");

            var harmony = new Harmony("touhma.dsp.plugins.galactic-scale");
            Harmony.CreateAndPatchAll(typeof(Patch));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchUIGalaxySlider));
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

        [HarmonyPatch(typeof(UIGalaxySelect))]
        private class PatchUIGalaxySlider {
            [HarmonyPrefix]
            [HarmonyPatch("UpdateUIDisplay")]
            public static void UIPrefix(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
                ___starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
            }

            [HarmonyPostfix]
            [HarmonyPatch("UpdateUIDisplay")]
            public static void UIPostfix(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
                ___starCountSlider.onValueChanged.AddListener(__instance.OnStarCountSliderValueChange);
            }
        }
    }
}