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
        }
    }
}