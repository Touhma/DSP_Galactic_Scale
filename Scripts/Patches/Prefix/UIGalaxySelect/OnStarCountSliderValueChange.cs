using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static GalacticScale.Utils;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.OnStarCountSliderValueChange))]
        public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider, ref GameDesc ___gameDesc, float val)
        {
            if (delayer == null) delayer = ___starCountSlider.gameObject.AddComponent<Delayer>();
            delayer.Wait();
            var num = (int)(val + 0.1f);
            if (num == ___gameDesc.starCount) return false;
            __instance.starCountText.text = num.ToString();
            num = Mathf.Clamp(num, GS3.ActiveGenerator.Config.MinStarCount, GS3.ActiveGenerator.Config.MaxStarCount);
            ___gameDesc.starCount = num;
            GS3.gameDesc = ___gameDesc;
            SystemDisplay.AbortRender(__instance.starmap);
            return false;
        }

        
    }
}