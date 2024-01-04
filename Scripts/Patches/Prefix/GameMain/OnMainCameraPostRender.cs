using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.OnMainCameraPostRender))]
        private static bool PatchOnMainCameraPostRender(Camera cam)
        {
            if (!GameMain.isPaused && GameMain.data != null) GameMain.data.OnPostDraw();

            return false;
        }
    }
}