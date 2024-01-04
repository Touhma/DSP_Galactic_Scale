﻿using HarmonyLib;
using NebulaCompatibility;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.UpdateUIDisplay))]
        public static bool UpdateUIDisplay(ref UIGalaxySelect __instance, GalaxyData galaxy)
        {
            if (NebulaCompat.IsClient) return true;
            __instance.starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
            if (galaxy == null) return false;

            if (galaxy.stars == null) return false;

            __instance.seedInput.text = galaxy.seed.ToString("0000 0000");



                __instance.starCountSlider.value = GSSettings.PrimaryStarCount();
                __instance.starCountText.text = GSSettings.PrimaryStarCount().ToString();


            var M = 0;
            var K = 0;
            var G = 0;
            var F = 0;
            var A = 0;
            var B = 0;
            var O = 0;
            var N = 0;
            var W = 0;
            var H = 0;
            if (galaxy.stars == null) return false;

            foreach (var star in galaxy.stars)
                if (star.type == EStarType.MainSeqStar || star.type == EStarType.GiantStar)
                {
                    if (star.spectr == ESpectrType.M)
                        ++M;
                    else if (star.spectr == ESpectrType.K)
                        ++K;
                    else if (star.spectr == ESpectrType.G)
                        ++G;
                    else if (star.spectr == ESpectrType.F)
                        ++F;
                    else if (star.spectr == ESpectrType.A)
                        ++A;
                    else if (star.spectr == ESpectrType.B)
                        ++B;
                    else if (star.spectr == ESpectrType.O) ++O;
                }
                else if (star.type == EStarType.NeutronStar)
                {
                    ++N;
                }
                else if (star.type == EStarType.WhiteDwarf)
                {
                    ++W;
                }
                else if (star.type == EStarType.BlackHole)
                {
                    ++H;
                }

            __instance.mCountText.text = M.ToString();
            __instance.kCountText.text = K.ToString();
            __instance.gCountText.text = G.ToString();
            __instance.fCountText.text = F.ToString();
            __instance.aCountText.text = A.ToString();
            __instance.bCountText.text = B.ToString();
            __instance.oCountText.text = O.ToString();
            __instance.nCountText.text = N.ToString();
            __instance.wdCountText.text = W.ToString();
            __instance.bhCountText.text = H.ToString();
            __instance.sandboxToggle.isOn = __instance.gameDesc.isSandboxMode;
            return false;
        }
    }
}