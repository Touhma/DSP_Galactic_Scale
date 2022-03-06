using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    public class PatchOnUIBuildingGrid
    {
        public static int refreshGridRadius = -1;

        //segment count to 512 lut
        public static Dictionary<int, int[]> LUT512 = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIBuildingGrid), "Update")]
        public static bool Update(UIBuildingGrid __instance, Material ___material, ref Material ___blueprintMaterial, Material ___altMaterial)
        {
            if (!GS2.Vanilla)
                if (!DSPGame.IsMenuDemo)
                {
                    int segments;
                    if (refreshGridRadius != -1)
                    {
                        if (___material == null) return true;

                        segments = (int)(refreshGridRadius / 4f + 0.1f) * 4;
                        __instance.blueprintMaterial.SetFloat("_Segment", segments);
                        if (LUT512.ContainsKey(segments)) UpdateTextureToLUT(___material, segments);
                        else
                            refreshGridRadius = -1;
                    }

                    return true;
                }

            return true;
        }

        public static void UpdateTextureToLUT(Material material, int segment)
        {
            // GS2.Warn("UpdateTextureToLUT:" + material.name);
            var tex = material.GetTexture("_SegmentTable");
            if (tex.dimension == TextureDimension.Tex2D)
            {
                var tex2d = (Texture2D)tex;
                for (var i = 0; i < 512; i++)
                {
                    var num = (LUT512[segment][i] / 4f + 0.05f) / 255f;

                    tex2d.SetPixel(i, 0, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 1, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 2, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 3, new Color(num, num, num, 1f));
                }

                tex2d.Apply();
            }
            else
            {
                GS2.Log("Nope");
            }

            refreshGridRadius = -1;
        }
    }
}