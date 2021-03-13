using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
	[HarmonyPatch(typeof(UIBuildingGrid))]
	public class PatchUIBuildingGrid
    {
		[HarmonyPrefix]
		[HarmonyPatch("Update")]
		public static void PrefixUpdate(UIBuildingGrid __instance, Material ___material, Material ___altMaterial)
		{
            if (refreshGridRadius != -1)
			{
				if(___material == null)
                {
					Patch.Debug("Material was null!", BepInEx.Logging.LogLevel.Debug, true);
					return;
				}
				int segments = (int)(refreshGridRadius / 4f + 0.1f) * 4;
				if (LUT512.ContainsKey(segments))
				{
					Patch.Debug("Updating LUT for radius + " + refreshGridRadius + " and segments " + segments + "!", BepInEx.Logging.LogLevel.Debug, true);
					UpdateTextureToLUT(___material, segments);
				}
				else
				{
					//TODO
					Patch.Debug("LUT512 did not yet contain the texture for refreshing.", BepInEx.Logging.LogLevel.Debug, true);
					refreshGridRadius = -1;
				}
			}
		}

		public static void DumpTextures(params Material[] mats)
        {
			foreach(Material mat in mats)
			{
				Patch.Debug("Material name: " + mat.name, BepInEx.Logging.LogLevel.Debug, true);
				if (mat == null)
				{
					continue;
				}
				foreach (string str in mat.GetTexturePropertyNames())
				{
					Texture tex = mat.GetTexture(str);
					Patch.Debug("Texture: " + tex.name + "/ID: " + str + " with wrapMode " + tex.wrapMode.ToString() + ", width " + tex.width + ", height" + tex.height + ", dimension " + tex.dimension.ToString(), BepInEx.Logging.LogLevel.Debug, true);
					if(tex.name == "segment-table")
                    {
						Texture2D tex2d = tex as Texture2D;
						Color c = tex2d.GetPixel(10, 0);
						Patch.Debug("Color is level " + c.r * 255f, BepInEx.Logging.LogLevel.Debug, true);
                    }
				}
				Patch.Debug("IDs:", BepInEx.Logging.LogLevel.Debug, true);
			}
			texDumped = true;
        }

		public static void UpdateTextureToLUT(Material material, int segment)
        {
			Texture tex = material.GetTexture("_SegmentTable");
			if (tex.dimension == TextureDimension.Tex2D)
			{
				Texture2D tex2d = (Texture2D)tex;
				for(int i = 0; i < 512; i++)
				{
					float num = (LUT512[segment][i] / 4 + 0.05f)/255f;

					tex2d.SetPixel(i, 0, new Color(num, num, num, 1f));
					tex2d.SetPixel(i, 1, new Color(num, num, num, 1f));
					tex2d.SetPixel(i, 2, new Color(num, num, num, 1f));
					tex2d.SetPixel(i, 3, new Color(num, num, num, 1f));
				}
				tex2d.Apply();
			}

			texUpdated = true;
			refreshGridRadius = -1;
		}

		public static bool texDumped = false;
		public static bool texUpdated = false;
		public static int refreshGridRadius = -1;

		public static Dictionary<int, int[]> LUT512 = new Dictionary<int, int[]>(); //segment count to 512 lut
	}
}
