using HarmonyLib;
using UnityEngine.UI;
using System.Reflection;
using System.IO;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System;

namespace GalacticScale.Scripts.PatchUI {
    [BepInPlugin("dsp.galactic-scale.ui", "Galactic Scale Plug-In - UI", "1.4.0")]
    public class PatchUI : BaseUnityPlugin {
        public new static ManualLogSource Logger;
        public static string Version = "1.4";
        public static AssetBundle bundle;
        internal void Awake()
        {
            var harmony = new Harmony("dsp.galactic-scale.ui");
            Logger = new ManualLogSource("PatchUI");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Logger.LogMessage("Galactic Scale Version " + Version + " loading");
            try
            {
                harmony.PatchAll(typeof(PatchUI));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
            }
        }

        public static Sprite GetSpriteAsset(string name)
        {
            if (bundle == null) bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchUI)).Location), "galacticbundle"));
            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            return bundle.LoadAsset<Sprite>(name);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIEscMenu), "_OnOpen")]
        public static void _OnOpen(ref Text ___stateText)
        {
            ___stateText.text += "\r\nGalactic Scale v" + Version;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIPlanetGlobe), "_OnUpdate")]
        public static void PatchOnUpdate(ref Text ___geoInfoText)
        {
            if (GameMain.localPlanet != null && VFInput.alt) ___geoInfoText.text = "\r\nRadius " + GameMain.localPlanet.realRadius;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIGameLoadingSplash), "OnEnable")]
        public static void OnEnable(ref Text ___promptText)
        {
            ___promptText.text = "WARNING - Galactic Scale savegames can be broken by updates. Read the FAQ @ http://customizing.space";
            Image[] images = UIRoot.instance.overlayCanvas.GetComponentsInChildren<Image>();
            RawImage[] rimages = UIRoot.instance.overlayCanvas.GetComponentsInChildren<RawImage>();
            foreach (Image image in images)
            {
                if (image.name == "black-bg")
                {
                    Sprite splash = GetSpriteAsset("splash");
                    if (splash != null) image.sprite = splash;
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
            if (___textComp != null)
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
                    string userName = ___displayAccount.detail.userName;
                    if (string.IsNullOrEmpty(userName))
                    {
                        ___textComp.fontSize = 18;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\nGalactic Scale v" + Version;
                    }
                    else
                    {
                        ___textComp.fontSize = 24;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\r\n" + userName + " - Galactic Scale v" + Version;
                    }

                }
            }
            ___firstFrame = false;
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UIPlanetDetail), "OnPlanetDataSet")]
        public static void OnPlanetDataSet (ref UIPlanetDetail __instance, Text ___obliquityValueText)
        {
            // Add the planets radius to the Planet Detail UI
            if (___obliquityValueText.transform.parent.transform.parent.childCount == 6) 
            {

                GameObject radiusLabel;
                GameObject obliquityLabel = ___obliquityValueText.transform.parent.gameObject;
                radiusLabel = Instantiate(obliquityLabel, obliquityLabel.transform.parent.transform);
                radiusLabel.transform.localPosition += (Vector3.down * 20);
                Text radiusLabelText = radiusLabel.GetComponent<Text>();
                radiusLabelText.GetComponent<Localizer>().enabled = false;
                Image radiusIcon = radiusLabel.transform.GetChild(1).GetComponent<Image>();
                UIButton uiButton = radiusLabel.transform.GetChild(1).GetComponent<UIButton>();
                uiButton.tips.tipText = "How large the planet is. Also roughly the amount of larger gridlines at the equator.";
                uiButton.tips.tipTitle = "Planet Radius";
                radiusIcon.sprite = GetSpriteAsset("ruler");
                Text radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();        
                radiusLabelText.text = "Planetary Radius";
                radiusValueText.text = __instance.planet.realRadius.ToString();
            }
            if (___obliquityValueText.transform.parent.transform.parent.childCount == 7)
            {
                Transform p = ___obliquityValueText.transform.parent.parent;
                GameObject radiusLabel = p.GetChild(p.childCount - 1).gameObject;
                Text radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
                if (__instance.planet != null) radiusValueText.text = __instance.planet.realRadius.ToString();
            }
        }
        [HarmonyPrefix, HarmonyPatch(typeof(UIPlanetDetail), "SetResCount")]
        public static bool SetResCount(int count, ref RectTransform ___rectTrans, ref RectTransform ___paramGroup) // Adjust the height of the PlanetDetail UI to allow for Radius Text
        {
            ___rectTrans.sizeDelta = new Vector2(___rectTrans.sizeDelta.x, (float)(190 + count * 20) + 20f);
            ___paramGroup.anchoredPosition = new Vector2(___paramGroup.anchoredPosition.x, (float)(-90 - count * 20));
            return false;
        }

    }
}
