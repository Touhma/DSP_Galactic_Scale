using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    public class PatchUI{
        [HarmonyPostfix, HarmonyPatch(typeof(UIPlanetGlobe), "_OnUpdate")]
        public static void PatchOnUpdate(ref Text ___geoInfoText)
        {
            if (GameMain.localPlanet != null && VFInput.alt) ___geoInfoText.text = "\r\nRadius " + GameMain.localPlanet.radius;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIGameLoadingSplash), "OnEnable")]
        public static void OnEnable(ref Text ___promptText, ref RawImage ___noiseImage1)
        {
            ___promptText.text = "WARNING - Galactic Scale savegames can be broken by updates. Read the FAQ @ http://customizing.space";
            string dir = Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchUIGameLoadingSplash)).Location) + "\\splash.jpg";
            Patch.Log(dir);
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(dir))
            {
                fileData = File.ReadAllBytes(dir);
                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, fileData);
            }
            Image[] images = UIRoot.instance.overlayCanvas.GetComponentsInChildren<Image>();
            RawImage[] rimages = UIRoot.instance.overlayCanvas.GetComponentsInChildren<RawImage>();
            foreach (Image image in images)
            {
                if (image.name == "black-bg")
                {
                    image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100f);
                    image.color = Color.white;
                }
                else if (image.name == "bg" || image.name == "dots" || image.name == "dsp") image.enabled = false;
            }
            foreach (RawImage rimage in rimages)
            {
                if (rimage.name == "vignette") rimage.enabled = false;
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static bool Refresh (ref Text ___textComp, string ___prefix, ref AccountData ___displayAccount, ref bool ___firstFrame)
        {
            if ((UnityEngine.Object)___textComp != (UnityEngine.Object)null)
            {
                bool flag = false;
                if (GameMain.data != null && !GameMain.instance.isMenuDemo && GameMain.isRunning)
                {
                    if (___displayAccount != GameMain.data.account)
                    {
                       ___displayAccount = GameMain.data.account;
                        flag = true;
                    }
                }
                else if (___displayAccount.userId != 0UL)
                {
                    ___displayAccount = AccountData.NULL;
                    flag = true;
                }
                if (___firstFrame || flag)
                {
                    string empty = string.Empty;
                    string userName = ___displayAccount.detail.userName;
                    if (string.IsNullOrEmpty(userName))
                    {
                        ___textComp.fontSize = 18;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\nGalactic Scale v" + Patch.Version;
                    }
                    else
                    {
                        ___textComp.fontSize = 24;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\r\n" + userName + " - Galactic Scale v" + Patch.Version;
                    }

                }
            }
            ___firstFrame = false;
            return false;
        }
    }
}