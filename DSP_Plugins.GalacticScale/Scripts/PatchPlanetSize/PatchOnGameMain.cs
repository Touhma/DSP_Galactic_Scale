using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(GameMain))]
    public class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnMainCameraPostRender")]
        static bool PatchOnMainCameraPostRender(Camera cam) 
        {
            if (GameMain.data != null) GameMain.data.OnPostDraw();
            return false;
        }
    }
}