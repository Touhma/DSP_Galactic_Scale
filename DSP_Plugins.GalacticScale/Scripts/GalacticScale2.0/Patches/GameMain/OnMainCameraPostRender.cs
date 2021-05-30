using HarmonyLib;
using UnityEngine;

namespace GalacticScale {
    public partial class PatchOnGameMain
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameMain), "OnMainCameraPostRender")]
        static bool PatchOnMainCameraPostRender(Camera cam)
        {
            if (GameMain.data != null) GameMain.data.OnPostDraw();
            return false;
        }
    }
}
