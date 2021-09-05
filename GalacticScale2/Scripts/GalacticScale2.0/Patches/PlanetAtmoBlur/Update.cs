using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnPlanetAtmoBlur
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetAtmoBlur), "Update")]
        public static bool Update(ref PlanetAtmoBlur __instance)
        {
            __instance.cam.fieldOfView = Camera.main.fieldOfView;
            if (GameMain.localPlanet != null && GameMain.localPlanet.type != EPlanetType.Gas)
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f * GameMain.localPlanet.GetScaleFactored(), 200f * GameMain.localPlanet.GetScaleFactored(), 240f * GameMain.localPlanet.GetScaleFactored(), Mathf.Clamp01((float)((1100.0 - GameMain.mainPlayer.position.magnitude) / 600.0))));
            else
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f, 200f, 200f, 0.0f));

            Shader.SetGlobalTexture("_Global_AtmoBlurTex", __instance.planetAtmoBlurRTex);
            return false;
        }
    }
}