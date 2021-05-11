using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {

        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect),"SetStarmapGalaxy")]
        public static bool SetStarmapGalaxy(ref UIGalaxySelect __instance)
        {
            SettingsUI.TryCaptureSeedInput();
            
            GalaxyData galaxy;
            if (GS2.Vanilla) galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            else galaxy = GS2.CreateGalaxy(__instance.gameDesc, false);
            //GS2.Log("Galaxy starCount = " + __instance.gameDesc.starCount);
            if (__instance.starmap.galaxyData != null)
                __instance.starmap.galaxyData.Free();
            __instance.starmap.galaxyData = galaxy;
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            //GS2.LogJson(GS2.generator.Config);

                
                //__instance.starCountSlider.minValue = GS2.generator.Config.MinStarCount;
                //__instance.starCountSlider.maxValue = GS2.generator.Config.MaxStarCount;

            GS2.Log(__instance.starCountSlider.value + " " + GSSettings.starCount + "+"+ GS2.generator.Config.MinStarCount);
            if (GS2.generator.Config.DisableStarCountSlider)
            {
                //GS2.Log("Disabling StarCount Slider");
                var starCountText = GameObject.Find("GS Star Count");
                
                if (starCountText == null) starCountText = CreateStarCountText(__instance.starCountSlider);
                starCountText.GetComponent<Text>().text = galaxy.starCount.ToString() + "   (" + GS2.generator.Name + ")";


            } else
            {
                //GS2.Log("Enabling StarCount Slider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null) starCountText.SetActive(false);
                __instance.starCountSlider.gameObject.SetActive(true);
            }
            if (GS2.generator.Config.DisableSeedInput)
            {
                //GS2.Log("Disabling Seed Input");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                //if (inputField != null) inputField.GetComponent<InputField>().interactable = false;
                inputField.transform.parent.GetComponent<Text>().enabled = false;
                inputField.GetComponentInChildren<Text>().enabled = false;
                inputField.GetComponent<Image>().enabled = false;
            }
            else
            {
                //GS2.Log("Enabling Seed Input");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                //if (inputField != null) inputField.SetActive(true);
                //inputField.GetComponent<InputField>().interactable = true;
                inputField.transform.parent.GetComponent<Text>().enabled = true;
                inputField.GetComponentInChildren<Text>().enabled = true;
                inputField.GetComponent<Image>().enabled = true;
            }
            //GS2.Log("done");
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static bool UpdateUIDisplay(ref UIGalaxySelect __instance, GalaxyData galaxy)
        {
            __instance.starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
            //GS2.Log("UpdateUIDisplay");
            if (galaxy == null) return false;
            if (galaxy.stars == null) return false;
            //GS2.Log("UpdateUIDisplay stars");
            __instance.seedInput.text = galaxy.seed.ToString("0000 0000");
            //GS2.Log("3");
            //__instance.starCountSlider.minValue = 0;
            //GS2.Log("4");
            //__instance.starCountSlider.maxValue = 1024;
            GS2.Log("Setting StarCountSlider Value" +galaxy.starCount);
            __instance.starCountSlider.value = (float)galaxy.starCount;
            //GS2.Log("6");
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
            //GS2.Log("7");
            if (galaxy.stars == null)
            {
                //GS2.Log("galaxy.stars == null"); 
                return false;
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
            //GS2.Log("8");
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
            GS2.Log("INIT");
            ___starCountSlider.maxValue = GS2.generator.Config.MaxStarCount;
            ___starCountSlider.minValue = GS2.generator.Config.MinStarCount;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static bool OnStarCountSliderValueChange(UIGalaxySelect __instance, ref Slider ___starCountSlider,
            ref GameDesc ___gameDesc, float val)
        {
            //GS2.Log("OnStarCountSliderValueChange");
            var num = (int)(___starCountSlider.value + 0.100000001490116);
            //GS2.Log("OnStarCountSliderValueChange2"); 
            if (num == ___gameDesc.starCount) return false;
            //GS2.Log("OnStarCountSliderValueChange3");
            num = Mathf.Clamp(num, GS2.generator.Config.MinStarCount, GS2.generator.Config.MaxStarCount);
            ___gameDesc.starCount = num;
            //GS2.Log("OnStarCountSliderValueChange4");
            GS2.gameDesc = ___gameDesc;
            //GS2.Log("OnStarCountSliderValueChange5");
            __instance.SetStarmapGalaxy();
            //GS2.Log("OnStarCountSliderValueChange6");
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static void UIPrefix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        {
            ___starCountSlider.onValueChanged.RemoveListener(__instance.OnStarCountSliderValueChange);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static void UIPostfix(UIGalaxySelect __instance, ref Slider ___starCountSlider)
        {
            ___starCountSlider.onValueChanged.AddListener(__instance.OnStarCountSliderValueChange);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        public static bool _OnOpen(UIGalaxySelect __instance)
        {
            GS2.Log("checking if vanilla what");
            if (GS2.generator == null) return true;
            GS2.Log("wHAT "+ GS2.generator.Config.DefaultStarCount);
            __instance.random = new System.Random((int)(System.DateTime.Now.Ticks / 10000L));
            __instance.gameDesc = new GameDesc();
            __instance.gameDesc.SetForNewGame(UniverseGen.algoVersion, __instance.random.Next(100000000), GS2.generator.Config.DefaultStarCount, 1, 1f);
            GS2.gameDesc = __instance.gameDesc;
            __instance.starmapGroup.gameObject.SetActive(true);
            __instance.starmap._Open();
            //GS2.LogJson(__instance.gameDesc);
            __instance.SetStarmapGalaxy();
            return false;
        }
    }
}