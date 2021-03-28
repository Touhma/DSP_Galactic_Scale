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
            string dir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchUIGameLoadingSplash)).Location) + "\\splash.jpg";
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
            Image[] images = UIRoot.instance.overlayCanvas.GetComponentsInChildren<Image>();
            RawImage[] rimages = UIRoot.instance.overlayCanvas.GetComponentsInChildren<RawImage>();
            foreach (Image image in images)
            {
                Patch.Log(image.name + " " + image.color.ToString());
                if (image.name == "black-bg") {
                    
                    Component[] c = image.GetComponents<Component>();
                    Sprite sprite;
                    sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100f);
                    image.sprite = sprite;
                    Patch.Log(image.color.ToString());
                    image.color = Color.white;
                }
                if (image.name == "bg" || image.name == "dots" || image.name == "dsp")
                {
                    image.enabled = false;
                }
            }
            foreach (RawImage rimage in rimages)
            {
                if (rimage.name == "vignette") rimage.enabled = false;
            }
            
            //i.sprite = sprite;
        }
    }
}