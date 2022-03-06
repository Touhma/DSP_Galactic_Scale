using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnUIEscMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEscMenu), "_OnOpen")]
        public static void _OnOpen(ref Text ___stateText)
        {
            ___stateText.text += "\r\nGalactic Scale v" + GS2.Version;
        }
    }
}