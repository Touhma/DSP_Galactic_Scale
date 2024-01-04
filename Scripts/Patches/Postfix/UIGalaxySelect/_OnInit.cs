using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect._OnInit))]
        public static void Patch_OnInit(UIGalaxySelect __instance, ref Slider ___starCountSlider, ref InputField ___seedInput)
        {
            ___starCountSlider.maxValue = GS3.ActiveGenerator.Config.MaxStarCount;
            ___starCountSlider.minValue = GS3.ActiveGenerator.Config.MinStarCount;
            ___seedInput.onValueChanged.AddListener(seed =>
            {
                int s;
                int.TryParse(seed, out s);
                GSSettings.Seed = s;
            });
            __instance.sandboxToggle.isOn = false;
            
        }
    }
}