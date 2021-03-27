using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System.Reflection;


namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(UIGameLoadingSplash))]
    public class PatchUIGameLoadingSplash{
        private const string V = "WARNING - Galactic Scale savegames can be broken by updates. Read the FAQ @ http://customizing.space";

        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        public static void PatchOnEnable(ref Text ___promptText, ref RawImage ___noiseImage1) {
            ___promptText.text = V;
            //public RawImage rawImage;
            string dir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchUIGameLoadingSplash)).Location) + "\\splash.png";
            Patch.Log(dir);
            Texture2D tex = null;
            byte[] fileData;
            if (System.IO.File.Exists(dir))
            {
                fileData = File.ReadAllBytes(dir);
                tex = new Texture2D(2, 2);
                UnityEngine.ImageConversion.LoadImage(tex, fileData);
            }
            ___noiseImage1.texture = tex;
        }
    }
}