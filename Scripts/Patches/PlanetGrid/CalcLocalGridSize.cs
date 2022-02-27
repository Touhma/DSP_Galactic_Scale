using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetGrid), "CalcLocalGridSize")]
        public static void CalcLocalGridSize(Vector3 posR, Vector3 dir, ref float __result, ref PlanetGrid __instance)
        {
            if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 480) __result -= 0.15f;
            if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 490) __result -= 0.15f;
            if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 500) __result -= 0.15f;
            if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 510) __result -= 0.19f;
            //if (GS2.Config.Test) __result += GS2.Config.TestNum;
        }
    }
}