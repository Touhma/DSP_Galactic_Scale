using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIPlanetDetail
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPlanetDetail), "SetResCount")]
        public static bool SetResCount(int count, ref RectTransform ___rectTrans, ref RectTransform ___paramGroup, ref int ___resourcesTabHeight, UIPlanetDetail __instance) // Adjust the height of the PlanetDetail UI to allow for Radius Text
        {
            var resourceHeight = System.Math.Max(count * 20, 46);
            ___resourcesTabHeight = 226 + resourceHeight + 20;
            var defaultWidth = AccessTools.Property(typeof(UIPlanetDetail), "defaultWidth")?.GetValue(__instance);
            var width = defaultWidth != null ? (int)defaultWidth : (int)___rectTrans.sizeDelta.x;
            ___rectTrans.sizeDelta = new Vector2(width, ___resourcesTabHeight);
            ___paramGroup.anchoredPosition = new Vector2(___paramGroup.anchoredPosition.x, -106 - resourceHeight);
            return false;
        }
    }
}
