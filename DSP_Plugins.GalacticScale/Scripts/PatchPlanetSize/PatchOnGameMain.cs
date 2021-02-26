using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(GameMain))]
    public class PatchOnGameMain {
        [HarmonyPostfix]
        [HarmonyPatch("Begin")]
        public static void Begin(GameMain __instance) {
            Debug.Log("PatchOnGameMain Postfix ?");
            if (GameMain.isRunning)
                if (GameMain.localPlanet != null && GameMain.mainPlayer != null) {
                    GameMain.localPlanet.factory.FlattenTerrain(GameMain.mainPlayer.position, GameMain.mainPlayer.uRotation,
                        new Bounds(Vector3.zero, new Vector3(1f, 1f, 1f)), removeVein: false, lift: false);
                    GameMain.localPlanet.factory.RenderLocalPlanetHeightmap();
                }
        }
    }
}