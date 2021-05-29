using HarmonyLib;
using UnityEngine;

namespace GalacticScale {
    [HarmonyPatch(typeof(GameMain))]
    public class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnMainCameraPostRender")]
        static bool OnMainCameraPostRender(Camera cam)
        {
            if (GameMain.data != null) GameMain.data.OnPostDraw();
            return false;
        }
    }
}
