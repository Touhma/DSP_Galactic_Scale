using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGame
    {
        private static Vector3 currentScale = new Vector3(200, 200, 200);
        private static PlanetData currentPlanet;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGame), "_OnUpdate")]
        public static void _OnUpdate(UIGame __instance)
        {
            if (GS2.Vanilla) return;
            if (GameMain.localPlanet == null)
            {
                currentScale = new Vector3(200, 200, 200);
                __instance.polarMark.transform.localScale = currentScale;
                return;
            }
            if (GameMain.localPlanet != currentPlanet)
            {
                //GS2.Warn("Polar Mark");
                //GS2.Warn(__instance.polarMark.name);
                //GS2.Warn(__instance.polarMark.transform.localScale.ToString());
                //GS2.Warn(__instance.polarMark.transform.position.ToString());
                currentPlanet = GameMain.localPlanet;
                var r = GameMain.localPlanet.realRadius;
                currentScale = new Vector3(r, r, r);
                __instance.polarMark.transform.localScale = currentScale;
            }

        }
    }
}