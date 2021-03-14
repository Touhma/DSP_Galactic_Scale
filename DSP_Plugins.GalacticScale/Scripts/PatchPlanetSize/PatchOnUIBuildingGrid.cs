using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(UIBuildingGrid))]
    public class PatchUIBuildingGrid {

        public static int refreshGridRadius = -1;

        //segment count to 512 lut
        public static Dictionary<int, int[]> LUT512 = new Dictionary<int, int[]>();

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Update(UIBuildingGrid __instance, Material ___material, Material ___altMaterial) {
            if (Patch.EnableResizingFeature.Value || Patch.EnableLimitedResizingFeature.Value) {
                int segments;
                if (refreshGridRadius != -1) {
                    if (___material == null) {
                        Patch.Debug("Material was null!", BepInEx.Logging.LogLevel.Debug, true);
                        return false;
                    }
                    segments = (int) (refreshGridRadius / 4f + 0.1f) * 4;
                    if (LUT512.ContainsKey(segments)) {
                        Patch.Debug("Updating LUT for radius + " + refreshGridRadius + " and segments " + segments + "!", BepInEx.Logging.LogLevel.Debug, true);
                        UpdateTextureToLUT(___material, segments);
                    }
                    else {
                        //TODO
                        Patch.Debug("LUT512 did not yet contain the texture for refreshing.", BepInEx.Logging.LogLevel.Debug, true);
                        refreshGridRadius = -1;
                    }
                }
                return false;
            }
            return true;
        }

        public static void UpdateTextureToLUT(Material material, int segment) {
            Texture tex = material.GetTexture("_SegmentTable");
            if (tex.dimension == TextureDimension.Tex2D) {
                Texture2D tex2d = (Texture2D) tex;
                for (int i = 0; i < 512; i++) {
                    float num = (LUT512[segment][i] / 4f + 0.05f) / 255f;

                    tex2d.SetPixel(i, 0, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 1, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 2, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 3, new Color(num, num, num, 1f));
                }
                tex2d.Apply();
            }

            refreshGridRadius = -1;
        }
    }
}