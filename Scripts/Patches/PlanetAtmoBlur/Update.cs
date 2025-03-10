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
            if (__instance == null || __instance.cam == null || Camera.main == null) return false;

            __instance.cam.fieldOfView = Camera.main.fieldOfView;
            
            if (GameMain.localPlanet != null && GameMain.localPlanet.type != EPlanetType.Gas && GameMain.mainPlayer != null)
            {
                float playerDist = GameMain.mainPlayer.transform != null ? GameMain.mainPlayer.transform.localPosition.magnitude : 1100f;
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(
                    200f * GameMain.localPlanet.GetScaleFactored(), 
                    200f * GameMain.localPlanet.GetScaleFactored(), 
                    240f * GameMain.localPlanet.GetScaleFactored(), 
                    Mathf.Clamp01((1100f - playerDist) / 600f)
                ));
            }
            else
            {
                __instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f, 200f, 200f, 0f));
            }

            if (__instance.planetAtmoBlurRTex != null)
            {
                Shader.SetGlobalTexture("_Global_AtmoBlurTex", __instance.planetAtmoBlurRTex);
            }
            
            return false;
        }
    }
}