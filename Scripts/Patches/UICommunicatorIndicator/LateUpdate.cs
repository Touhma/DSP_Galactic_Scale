using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnUICommunicatorIndicator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UICommunicatorIndicator), "_OnLateUpdate")]
        public static bool _OnLateUpdate(UICommunicatorIndicator __instance)
        {
            if (__instance == null) return false;
            
            if (GameMain.gameScenario == null || GameMain.gameScenario.cosmicMessageManager == null) 
            {
                if (__instance.gameObject != null)
                {
                    __instance.gameObject.SetActive(false);
                }
                return false;
            }

            var messages = GameMain.gameScenario.cosmicMessageManager.messages;
            if (messages == null)
            {
                if (__instance.gameObject != null)
                {
                    __instance.gameObject.SetActive(false);
                }
                return false;
            }

            // Let the original method run if everything is properly initialized
            return true;
        }
    }
} 