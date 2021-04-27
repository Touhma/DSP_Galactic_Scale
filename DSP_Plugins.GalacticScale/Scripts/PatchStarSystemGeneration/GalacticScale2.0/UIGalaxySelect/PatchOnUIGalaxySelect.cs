using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect),"SetStarmapGalaxy")]
        public static bool SetStarmapGalaxy(ref UIGalaxySelect __instance)
        {
            GalaxyData galaxy;
            if (GS2.Vanilla) galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            else galaxy = GS2.CreateGalaxy(__instance.gameDesc, false);
            GS2.Log("Galaxy starCount = " + __instance.gameDesc.starCount);
            if (__instance.starmap.galaxyData != null)
                __instance.starmap.galaxyData.Free();
            __instance.starmap.galaxyData = galaxy;
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            if (GS2.generator.Config.DisableStarCountSlider)
            {
                GS2.Log("Disabling StarCount Slider");
                var starCountText = GameObject.Find("GS Star Count");
                
                if (starCountText == null) starCountText = CreateStarCountText(__instance.starCountSlider);
                starCountText.GetComponent<Text>().text = galaxy.starCount.ToString() + "   (" + GS2.generator.Name + ")";


            } else
            {
                GS2.Log("Enabling StarCount Slider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null) starCountText.SetActive(false);
                __instance.starCountSlider.gameObject.SetActive(true);
            }
            if (GS2.generator.Config.DisableSeedInput)
            {
                GS2.Log("Disabling Seed Input");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                if (inputField != null) inputField.SetActive(false);
            }
            else
            {
                GS2.Log("Enabling Seed Input");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                if (inputField != null) inputField.SetActive(true);
            }
            GS2.Log("done");
            return false;
        }
        public static GameObject CreateStarCountText(Slider slider)
        {
            RectTransform starCountSlider = slider.GetComponent<RectTransform>();
            slider.gameObject.SetActive(false);
            Text template = slider.GetComponentInParent<Text>();
            RectTransform starCountText = GameObject.Instantiate(template.GetComponent<RectTransform>(), template.GetComponentInParent<RectTransform>().parent, false);
            starCountText.anchoredPosition = new Vector2(starCountText.anchoredPosition.x + 140, starCountSlider.GetComponentInParent<RectTransform>().anchoredPosition.y);
            Object.DestroyImmediate(starCountText.GetComponent<Localizer>());
            starCountText.name = "GS Star Count";
            return starCountText.gameObject ;
        }
        //[HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        //public static bool UpdateUIDisplay(ref UIGalaxySelect __instance, GalaxyData galaxy)
        //{
        //    GS2.Log("1");
        //    if (galaxy == null) return false;
        //    GS2.Log("2");
        //    __instance.seedInput.text = galaxy.seed.ToString("0000 0000");
        //    GS2.Log("3"); 
        //    __instance.starCountSlider.minValue = 0;
        //    GS2.Log("4");
        //    __instance.starCountSlider.maxValue = 1024;
        //    GS2.Log("5");
        //    __instance.starCountSlider.value = (float)galaxy.starCount;
        //    GS2.Log("6");
        //    __instance.starCountText.text = galaxy.starCount.ToString();
        //    int num1 = 0;
        //    int num2 = 0;
        //    int num3 = 0;
        //    int num4 = 0;
        //    int num5 = 0;
        //    int num6 = 0;
        //    int num7 = 0;
        //    int num8 = 0;
        //    int num9 = 0;
        //    int num10 = 0;
        //    GS2.Log("7");
        //    if (galaxy.stars == null)
        //    {
        //        GS2.Log("galaxy.stars == null");   return false;
        //    }
        //    foreach (StarData star in galaxy.stars)
        //    {
        //        if (star.type == EStarType.MainSeqStar || star.type == EStarType.GiantStar)
        //        {
        //            if (star.spectr == ESpectrType.M)
        //                ++num1;
        //            else if (star.spectr == ESpectrType.K)
        //                ++num2;
        //            else if (star.spectr == ESpectrType.G)
        //                ++num3;
        //            else if (star.spectr == ESpectrType.F)
        //                ++num4;
        //            else if (star.spectr == ESpectrType.A)
        //                ++num5;
        //            else if (star.spectr == ESpectrType.B)
        //                ++num6;
        //            else if (star.spectr == ESpectrType.O)
        //                ++num7;
        //        }
        //        else if (star.type == EStarType.NeutronStar)
        //            ++num8;
        //        else if (star.type == EStarType.WhiteDwarf)
        //            ++num9;
        //        else if (star.type == EStarType.BlackHole)
        //            ++num10;
        //    }
        //    GS2.Log("8");
        //    __instance.mCountText.text = num1.ToString();
        //    __instance.kCountText.text = num2.ToString();
        //    __instance.gCountText.text = num3.ToString();
        //    __instance.fCountText.text = num4.ToString();
        //    __instance.aCountText.text = num5.ToString();
        //    __instance.bCountText.text = num6.ToString();
        //    __instance.oCountText.text = num7.ToString();
        //    __instance.nCountText.text = num8.ToString();
        //    __instance.wdCountText.text = num9.ToString();
        //    __instance.bhCountText.text = num10.ToString();
        //    return false;
        //}
    }
}