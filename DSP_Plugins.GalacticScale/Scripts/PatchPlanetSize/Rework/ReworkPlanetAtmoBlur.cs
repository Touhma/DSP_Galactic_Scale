//using UnityEngine;

//namespace GalacticScale {
//    public static class ReworkPlanetAtmoBlur {
//        public static void ReworkUpdate(ref PlanetAtmoBlur instance) {
//            instance.cam.fieldOfView = Camera.main.fieldOfView;
//            if (GameMain.localPlanet != null && GameMain.localPlanet.type != EPlanetType.Gas) {
//                instance.blurMat.SetVector("_PlanetRadius",
//                    new Vector4(200f * GameMain.localPlanet.GetScaleFactored(), 200f * GameMain.localPlanet.GetScaleFactored(), 240f * GameMain.localPlanet.GetScaleFactored(),
//                        Mathf.Clamp01((float) ((1100.0 - GameMain.mainPlayer.position.magnitude) / 600.0))));

//            }
//            else {
//                instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f, 200f, 200f, 0.0f));
//            }

//            Shader.SetGlobalTexture("_Global_AtmoBlurTex", instance.planetAtmoBlurRTex);
//        }
//    }
//}