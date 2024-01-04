using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public class PatchOnUIGame
    {
        private static Vector3 currentScale = new(200, 200, 200);
        private static PlanetData currentPlanet;
        //Scale the polar markers
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGame), nameof(UIGame._OnUpdate))]
        public static void _OnUpdate(UIGame __instance)
        {
            if (GS3.IsMenuDemo) return;
            if (GameMain.localPlanet == null)
            {
                currentScale = new Vector3(200, 200, 200);
                __instance.polarMark.transform.localScale = currentScale;
                return;
            }

            if (GameMain.localPlanet != currentPlanet)
            {
                currentPlanet = GameMain.localPlanet;
                var r = GameMain.localPlanet.realRadius;
                currentScale = new Vector3(r, r, r);
                __instance.polarMark.transform.localScale = currentScale;
            }
        }
    }
}