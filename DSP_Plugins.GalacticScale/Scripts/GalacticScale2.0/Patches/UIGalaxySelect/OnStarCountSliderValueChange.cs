using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale {
    public partial class PatchOnUIGalaxySelect {
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider,
            ref GameDesc ___gameDesc, float val) {
            var num = (int)(___starCountSlider.value + 0.100000001490116);
            if (num == ___gameDesc.starCount) {
                return false;
            }

            num = Mathf.Clamp(num, GS2.generator.Config.MinStarCount, GS2.generator.Config.MaxStarCount);
            ___gameDesc.starCount = num;
            GS2.gameDesc = ___gameDesc;
            __instance.SetStarmapGalaxy();
            return false;
        }
    }
}