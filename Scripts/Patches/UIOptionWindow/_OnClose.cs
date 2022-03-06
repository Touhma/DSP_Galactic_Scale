using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), "_OnClose")]
        public static void _OnClose(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts)
        {
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;
            if (GS2.canvasOverlay)
            {
                overlayCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                GS2.canvasOverlay = false;
            }
        }
    }
}