using HarmonyLib;
using UnityEngine.UI;
using Patch = GalacticScale.Scripts.PatchUI.PatchForUI;


namespace GalacticScale.Scripts.PatchUI
{
    [HarmonyPatch(typeof(UIEscMenu))]
    public class PatchOnUIEscMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch("_OnOpen")]
        public static void _OnOpen(ref Text ___stateText)
        {
            ___stateText.text += "\r\nGalactic Scale v" + Patch.Version;
        }
    }
}