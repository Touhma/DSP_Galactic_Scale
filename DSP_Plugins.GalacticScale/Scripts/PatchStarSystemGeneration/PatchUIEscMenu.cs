using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(UIEscMenu))]
    public class PatchUIEscMenu {
        [HarmonyPostfix]
        [HarmonyPatch("_OnOpen")]
        public static void _OnOpen(ref UnityEngine.UI.Text ___stateText) {
            ___stateText.text += "\r\nGalactic Scale v" + Patch.version;
        }
    }
}