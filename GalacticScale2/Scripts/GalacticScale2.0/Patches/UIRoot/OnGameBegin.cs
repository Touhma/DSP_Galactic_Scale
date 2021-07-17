using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class PatchOnUIRoot
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void OnGameBegin()
        {
            GS2.Initialized = true;
        }
    }
}