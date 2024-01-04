using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIEscMenu
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIEscMenu), nameof(UIEscMenu.OnButton5Click))]
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