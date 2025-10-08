using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameMain
    {
        // Broken 0.10.33.26934
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(GameMain), "OnMainCameraPostRender")]
        // private static bool PatchOnMainCameraPostRender(Camera cam)
        // {
        //     if (!GameMain.isPaused && GameMain.data != null) GameMain.data.();
        //
        //     return false;
        // }
    }
}