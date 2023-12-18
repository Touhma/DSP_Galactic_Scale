using HarmonyLib;
using UnityEngine;
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
            var r = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/combat-detail-btn").GetComponent<RectTransform>();
            if (r != null)
            {
                r.position = new Vector3(r.position.x + 2, r.position.y, r.position.z);
            }
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