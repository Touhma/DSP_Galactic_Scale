using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticScale
{
    public partial class PatchUIBuildingGrid
    {
        public static int refreshGridRadius = -1;
        //segment count to 512 lut
        public static Dictionary<int, int[]> LUT512 = new Dictionary<int, int[]>();
        [HarmonyPrefix, HarmonyPatch(typeof(UIBuildingGrid), "Update")]
        public static bool Update(UIBuildingGrid __instance, Material ___material, Material ___altMaterial)
        {

            if (!GS2.Vanilla)
                if (!DSPGame.IsMenuDemo)
                {
                    int segments;
                    if (refreshGridRadius != -1)
                    {
                        if (___material == null) return true;
                        segments = (int)(refreshGridRadius / 4f + 0.1f) * 4;
                        if (LUT512.ContainsKey(segments)) UpdateTextureToLUT(___material, segments);
                        else refreshGridRadius = -1;
          
                    }
                    return true;
                }
            return true;
        }

        public static void UpdateTextureToLUT(Material material, int segment)
        {
            Texture tex = material.GetTexture("_SegmentTable");
            if (tex.dimension == TextureDimension.Tex2D)
            {
                Texture2D tex2d = (Texture2D)tex;
                for (int i = 0; i < 512; i++)
                {
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