using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Patch = GalacticScale.Scripts.PatchUI.PatchForUI;


namespace GalacticScale.Scripts.PatchUI
{
    [HarmonyPatch(typeof(UIGameLoadingSplash))]
    public class PatchOnUIGameLoadingSplash
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnEnable")]
        public static void OnEnable(ref Text ___promptText)
        {
            ___promptText.text = "WARNING - Galactic Scale savegames can be broken by updates. Read the FAQ @ http://customizing.space";
            Image[] images = UIRoot.instance.overlayCanvas.GetComponentsInChildren<Image>();
            RawImage[] rimages = UIRoot.instance.overlayCanvas.GetComponentsInChildren<RawImage>();
            foreach (Image image in images)
            {
                if (image.name == "black-bg")
                {
                    Sprite splash = Patch.GetSpriteAsset("splash");
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
    }
}