using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public class PatchOnPlanetAtmoBlur
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAtmoBlur), nameof(PlanetAtmoBlur.Update))]
        public static bool Update(ref PlanetAtmoBlur __instance)
        {
            //Last checked 0.10.28.21172
            __instance.cam.fieldOfView = Camera.main.fieldOfView;
            if (GameMain.localPlanet != null && GameMain.localPlanet.type != EPlanetType.Gas)
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(GameMain.localPlanet.realRadius, GameMain.localPlanet.realRadius, GameMain.localPlanet.realRadius * 1.2f, Mathf.Clamp01((float)((1100.0 - GameMain.mainPlayer.position.magnitude) / GameMain.localPlanet.realRadius*3f))));
            else
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f, 200f, 200f, 0.0f));

            Shader.SetGlobalTexture("_Global_AtmoBlurTex", __instance.planetAtmoBlurRTex);
            return false;
        }
    }
}