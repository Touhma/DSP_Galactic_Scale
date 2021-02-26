using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize.Rework {
    public static class ReworkPlanetAtmoBlur {
        public static void ReworkUpdate(ref PlanetAtmoBlur instance) {
            Patch.Debug("ReworkPlanetAtmoBlur", LogLevel.Debug, Patch.DebugAtmoBlur);
            instance.cam.fieldOfView = Camera.main.fieldOfView;
            if (GameMain.localPlanet != null && GameMain.localPlanet.type != EPlanetType.Gas) {
                Patch.Debug("GameMain.localPlanet" + GameMain.localPlanet, LogLevel.Debug, Patch.DebugAtmoBlur);
                instance.blurMat.SetVector("_PlanetRadius",
                    new Vector4(200f * GameMain.localPlanet.GetScaleFactored(), 200f * GameMain.localPlanet.GetScaleFactored(), 240f * GameMain.localPlanet.GetScaleFactored(),
                        Mathf.Clamp01((float) ((1100.0 - GameMain.mainPlayer.position.magnitude) / 600.0))));

                Patch.Debug("instance.blurMat" + instance.blurMat.GetVector("_PlanetRadius"), LogLevel.Debug, Patch.DebugAtmoBlur);
            }
            else {
                instance.blurMat.SetVector("_PlanetRadius", new Vector4(200f, 200f, 200f, 0.0f));
                Patch.Debug("else" + instance.blurMat.GetVector("_PlanetRadius"), LogLevel.Debug, Patch.DebugAtmoBlur);
            }

            Shader.SetGlobalTexture("_Global_AtmoBlurTex", instance.planetAtmoBlurRTex);
        }
    }
}