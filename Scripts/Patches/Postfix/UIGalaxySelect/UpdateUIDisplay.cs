using HarmonyLib;
using NebulaCompatibility;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.UpdateUIDisplay))]
        public static void UIPostfix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        {
            ___starCountSlider.onValueChanged.AddListener(__instance.OnStarCountSliderValueChange);
        }

       
    }
}