using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        public static GameObject StartButton;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        public static bool _OnOpen(UIGalaxySelect __instance , ref Slider ___starCountSlider)
        {
            if (GS2.ActiveGenerator == null) return true;
            ___starCountSlider.maxValue = GS2.ActiveGenerator.Config.MaxStarCount;
            ___starCountSlider.minValue = GS2.ActiveGenerator.Config.MinStarCount;
            
            if (__instance.gameDesc == null) GS2.Warn("GameDesc Null");
            if (StartButton == null) StartButton = __instance.GetComponentInChildren<UIButton>()?.gameObject;
            if (StartButton == null) GS2.Warn("StartButton Null");
            StartButton?.SetActive(true);
            GS2.Log(StartButton?.name);
            __instance.random =
                new DotNet35Random(GSSettings.Seed); //new DotNet35Random((int)(System.DateTime.Now.Ticks / 10000L));
            __instance.gameDesc = new GameDesc();
            __instance.gameDesc?.SetForNewGame(UniverseGen.algoVersion, __instance.random.Next(100000000),
                GS2.ActiveGenerator.Config.DefaultStarCount, 1, 1f);
            GS2.gameDesc = __instance.gameDesc;
            if (__instance.starmapGroup == null) GS2.Warn("smg Null");
            __instance.starmapGroup?.gameObject?.SetActive(true);
            if (__instance.starmap == null) GS2.Warn("starmap Null");
            __instance.starmap?._Open();
            if (__instance.gameDesc == null) GS2.Warn("GameDesc Null 2");
            if (__instance.gameDesc?.starCount <= 0) __instance.gameDesc.starCount = 1;
            __instance.SetStarmapGalaxy();
            return false;
        }
    }
}