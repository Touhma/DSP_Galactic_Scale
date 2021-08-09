using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GalacticScale

{
    public class PatchOnWhatever
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIRoot), "ExitProgramSplash")]
        public static void ExitProgramSplash(UIRoot __instance)
        {
            var lg = GameObject.Find("UI Root/Overlay Canvas/Splash/");
            // while (lg.transform.childCount > 0)
            //     Object.DestroyImmediate(lg.transform.GetChild(0).gameObject);
            var images = lg.GetComponentsInChildren<Image>();
            var rimages = lg.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
                if (image.name == "black" || image.name == "black-bg")
                {
                    var splash = Utils.GetSplashSprite();
                    if (splash != null) image.sprite = splash;
                    GS2.splashImage = image;
                    image.color = Color.white;
                }
                else if (image.name == "bg" || image.name == "logo" || image.name == "dsp" || image.name =="dots" || image.name == "health-advice")
                {
                    image.enabled = false;
                }

            foreach (var rimage in rimages)
                if (rimage.name == "vignette" || rimage.name == "logo")
                    rimage.enabled = false;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Update")]
        public static void Update(VFPreload __instance)
        {
            __instance.splashes[0].gameObject.SetActive(true);
            __instance.splashes[1].gameObject.SetActive(false);
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Restart")]
        public static void Restart(VFPreload __instance)
        {
            GS2.splashImage.sprite = Utils.GetSplashSprite() ?? null;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Start")]
        public static void Start()
        {
            var lg = GameObject.Find("UI Root/Overlay Canvas/Splash/");
            // while (lg.transform.childCount > 0)
            //     Object.DestroyImmediate(lg.transform.GetChild(0).gameObject);
            var images = lg.GetComponentsInChildren<Image>();
            var rimages = lg.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
                if (image.name == "black" || image.name == "black-bg")
                {
                    var splash = Utils.GetSplashSprite();
                    if (splash != null) image.sprite = splash;
                    GS2.splashImage = image;
                    image.color = Color.white;
                }
                else if (image.name == "bg" || image.name == "logo" || image.name == "dsp" || image.name =="dots" || image.name == "health-advice")
                {
                    image.enabled = false;
                }

            foreach (var rimage in rimages)
                if (rimage.name == "vignette" || rimage.name == "logo")
                    rimage.enabled = false;
            
        }
    }
}