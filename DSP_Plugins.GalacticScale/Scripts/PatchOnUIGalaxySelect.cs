using HarmonyLib;
using UnityEngine.UI;

namespace DSP_Plugin.GalacticScale {
    [HarmonyPatch(typeof(UIGalaxySelect))]
    public class PatchOnUIGalaxySelect {

        [HarmonyPostfix]
        [HarmonyPatch("_OnInit")]
        public static void Patch_OnInit(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
            ___starCountSlider.maxValue = DSP_GalacticScale.ConfigStarsMax.Value;
            ___starCountSlider.minValue = DSP_GalacticScale.ConfigStarsMin.Value;
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