using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), nameof(UIOptionWindow._OnClose))]
        public static void _OnClose(ref UIOptionWindow __instance, ref UIButton[] ___tabButtons, ref Text[] ___tabTexts)
        {
            var overlayCanvas = GameObject.Find("Overlay Canvas");
            if (overlayCanvas == null || overlayCanvas.transform.Find("Top Windows") == null) return;
            if (GS3.canvasOverlay)
            {
                overlayCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                GS3.canvasOverlay = false;
            }
        }
    }
}