using HarmonyLib;
using UnityEngine.UI;
using Patch = GalacticScale.Scripts.PatchUI.PatchForUI;


namespace GalacticScale.Scripts.PatchUI
{
    public class PatchOnUIEscMenu
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIEscMenu), "_OnOpen")]
        public static void _OnOpen(ref Text ___stateText)
        {
            ___stateText.text += "\r\nGalactic Scale v" + Patch.Version;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenu),"_OnOpen")]
        public static void _OnOpenMM()
        {
            UIMessageBox.Show("Warning", "Galactic Scale 1.4 is no longer supported, please upgrade to Galactic Scale 2!\r\nAvailable on Thunderstore (http://dsp.thunderstore.io) or at http://customizing.space/\t\nThis version will continue to work until DSP is updated\r\nPlease don't start any new games using this version.", "Noted!", 0);
        }
    }
}