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
            __instance.starCountSlider.minValue = GS2.generator.Config.MinStarCount;
            __instance.starCountSlider.maxValue = GS2.generator.Config.MaxStarCount;
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
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static bool UpdateUIDisplay(ref UIGalaxySelect __instance, GalaxyData galaxy)
        {
            __instance.starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
            GS2.Log("UpdateUIDisplay");
            if (galaxy == null) return false;
            if (galaxy.stars == null) return false;
            GS2.Log("UpdateUIDisplay stars");
            __instance.seedInput.text = galaxy.seed.ToString("0000 0000");
            GS2.Log("3");
            //__instance.starCountSlider.minValue = 0;
            GS2.Log("4");
            //__instance.starCountSlider.maxValue = 1024;
            GS2.Log("5");
            //__instance.starCountSlider.value = (float)galaxy.starCount;
            GS2.Log("6");
            __instance.starCountText.text = galaxy.starCount.ToString();
            int M = 0;
            int K = 0;
            int G = 0;
            int F = 0;
            int A = 0;
            int B = 0;
            int O = 0;
            int N = 0;
            int W = 0;
            int H = 0;
            GS2.Log("7");
            if (galaxy.stars == null)
            {
                GS2.Log("galaxy.stars == null"); return false;
            }
            foreach (StarData star in galaxy.stars)
            {
                if (star.type == EStarType.MainSeqStar || star.type == EStarType.GiantStar)
                {
                    if (star.spectr == ESpectrType.M)
                        ++M;
                    else if (star.spectr == ESpectrType.K)
                        ++K;
                    else if (star.spectr == ESpectrType.G)
                        ++G;
                    else if (star.spectr == ESpectrType.F)
                        ++F;
                    else if (star.spectr == ESpectrType.A)
                        ++A;
                    else if (star.spectr == ESpectrType.B)
                        ++B;
                    else if (star.spectr == ESpectrType.O)
                        ++O;
                }
                else if (star.type == EStarType.NeutronStar)
                    ++N;
                else if (star.type == EStarType.WhiteDwarf)
                    ++W;
                else if (star.type == EStarType.BlackHole)
                    ++H;
            }
            GS2.Log("8");
            __instance.mCountText.text = M.ToString();
            __instance.kCountText.text = K.ToString();
            __instance.gCountText.text = G.ToString();
            __instance.fCountText.text = F.ToString();
            __instance.aCountText.text = A.ToString();
            __instance.bCountText.text = B.ToString();
            __instance.oCountText.text = O.ToString();
            __instance.nCountText.text = N.ToString();
            __instance.wdCountText.text = W.ToString();
            __instance.bhCountText.text = H.ToString();
            return false;
            //return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect),"_OnInit")]
        public static void Patch_OnInit(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        {
            ___starCountSlider.maxValue = GS2.generator.Config.MinStarCount;
            ___starCountSlider.minValue = GS2.generator.Config.MaxStarCount;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider,
            ref GameDesc ___gameDesc, float val)
        {
            var num = (int)(___starCountSlider.value + 0.100000001490116);
            if (num == ___gameDesc.starCount) return false;

            ___gameDesc.starCount = num;
            __instance.SetStarmapGalaxy();
            return false;
        }

        //[HarmonyPrefix]
        //[HarmonyPatch("UpdateUIDisplay")]
        //public static void UIPrefix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        //{
        //    ___starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static void UIPostfix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        {
            ___starCountSlider.onValueChanged.AddListener(__instance.OnStarCountSliderValueChange);
        }
    }
}