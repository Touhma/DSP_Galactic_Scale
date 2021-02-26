using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale.Scripts.PatchGalaxySizeSelection {
    [HarmonyPatch(typeof(UIGalaxySelect))]
    public class PatchOnUIGalaxySelect {
        [HarmonyPostfix]
        [HarmonyPatch("_OnInit")]
        public static void Patch_OnInit(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
            ___starCountSlider.maxValue = PatchForGalaxySizeSelection.ConfigStarsMax.Value;
            ___starCountSlider.minValue = PatchForGalaxySizeSelection.ConfigStarsMin.Value;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnStarCountSliderValueChange")]
        public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider,
            ref GameDesc ___gameDesc, float val) {
            var num = (int) (___starCountSlider.value + 0.100000001490116);
            if (num == ___gameDesc.starCount) return false;

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