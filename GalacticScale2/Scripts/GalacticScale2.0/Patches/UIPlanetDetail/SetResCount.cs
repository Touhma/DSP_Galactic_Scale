using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIPlanetDetail
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPlanetDetail), "SetResCount")]
        public static bool SetResCount(int count, ref RectTransform ___rectTrans, ref RectTransform ___paramGroup) // Adjust the height of the PlanetDetail UI to allow for Radius Text
        {
            ___rectTrans.sizeDelta = new Vector2(___rectTrans.sizeDelta.x, 210 + count * 20 + 20f);
            ___paramGroup.anchoredPosition = new Vector2(___paramGroup.anchoredPosition.x, -90 - count * 20);
            return false;
        }
    }
}