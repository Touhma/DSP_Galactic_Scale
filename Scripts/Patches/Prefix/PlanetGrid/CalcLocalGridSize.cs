using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcLocalGridSize))]
        public static void CalcLocalGridSize(Vector3 posR, Vector3 dir, ref float __result, ref PlanetGrid __instance)
        {
            if (GS3.Config.FixCopyPaste && GameMain.localPlanet.radius == 480) __result -= 0.15f;
            if (GS3.Config.FixCopyPaste && GameMain.localPlanet.radius == 490) __result -= 0.15f;
            if (GS3.Config.FixCopyPaste && GameMain.localPlanet.radius == 500) __result -= 0.15f;
            if (GS3.Config.FixCopyPaste && GameMain.localPlanet.radius == 510) __result -= 0.19f;
        }
    }
}