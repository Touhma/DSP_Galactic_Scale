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

        [HarmonyPrefix, HarmonyPatch(typeof(UIEscMenu), "OnButton5Click")]
        public static bool OnButton5Click(UIEscMenu __instance)
        {
            if (__instance.locked)
            {
                return false;
            }
            __instance.locked = true;
            UIRoot.instance.backToMainMenu = true;
            Modeler.Reset();
            Bootstrap.WaitUntil(()=>Modeler.Idle, DSPGame.EndGame);
            return false;
        }
        
    }
}