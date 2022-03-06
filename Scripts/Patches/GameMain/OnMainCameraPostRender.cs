using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), "OnMainCameraPostRender")]
        private static bool PatchOnMainCameraPostRender(Camera cam)
        {
            if (!GameMain.isPaused && GameMain.data != null) GameMain.data.OnPostDraw();

            return false;
        }
    }
}