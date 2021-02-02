using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine.UI;

namespace DSP_Plugin {
    [BepInPlugin("org.bepinex.plugins.moreStars", "More Stars Plug-In", "1.0.0.0")]
    public class DSP_MoreStars : BaseUnityPlugin {
        internal void Awake() {
            var harmony = new Harmony("org.bepinex.plugins.moreStars");
          Harmony.CreateAndPatchAll(typeof(Patch));
        }

        [HarmonyPatch(typeof(UIGalaxySelect))]
        private class Patch {
            [HarmonyPostfix]
            [HarmonyPatch("_OnInit")]
            public static void Postfix(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
                ___starCountSlider.maxValue = 255;
            }
            
            [HarmonyPrefix]
            [HarmonyPatch("OnStarCountSliderValueChange")]
            public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider , ref GameDesc ___gameDesc, float val)
            {
                int num = (int) (___starCountSlider.value + 0.100000001490116);
                if (num == ___gameDesc.starCount) {
                    return false;
                }
                ___gameDesc.starCount = num;
                __instance.SetStarmapGalaxy();
                return false;
            }
        }
    }
}