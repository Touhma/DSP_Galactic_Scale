using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    //The Patches in this class Add a PreLoad Splash

    public partial class PatchOnVFPreload
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), "Start")]
        public static void Start()
        {
            var lg = GameObject.Find("UI Root/Overlay Canvas/Splash/");
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
                else if (image.name == "bg" || image.name == "logo" || image.name == "dsp" || image.name == "dots" || image.name == "health-advice")
                {
                    image.enabled = false;
                }

            foreach (var rimage in rimages)
                if (rimage.name == "vignette" || rimage.name == "logo")
                    rimage.enabled = false;
        }
    }
}