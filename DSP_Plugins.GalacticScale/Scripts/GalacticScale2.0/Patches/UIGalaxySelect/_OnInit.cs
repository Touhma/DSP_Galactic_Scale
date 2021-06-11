using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIGalaxySelect),"_OnInit")]
        public static void Patch_OnInit(UIGalaxySelect __instance, ref Slider ___starCountSlider, ref InputField ___seedInput)
        {
            ___starCountSlider.maxValue = GS2.generator.Config.MaxStarCount;
            ___starCountSlider.minValue = GS2.generator.Config.MinStarCount;
            ___seedInput.onValueChanged.AddListener((string seed) =>
            {
                int s;
                int.TryParse(seed, out s);
                GSSettings.Seed = s;
            });
        }
    }
}