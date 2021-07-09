using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "SetStarmapGalaxy")]
        public static bool SetStarmapGalaxy(ref UIGalaxySelect __instance)
        {
            GS2.Log("Start");
            GalaxyData galaxy;
            if (__instance.gameDesc == null) GS2.Warn("GameDesc Null 3");
            if ((__instance.gameDesc.starCount <= 0)) __instance.gameDesc.starCount = 1;
            if (GS2.Vanilla)
            {
                galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            }
            else
            {
                //GS2.Warn("Processing Galaxy");
                galaxy = GS2.ProcessGalaxy(__instance.gameDesc, true);
                //GS2.Warn("Done");
            }
            if (__instance.starmap == null) GS2.Warn("Starmap Null");
            if (__instance.starmap.galaxyData == null) GS2.Warn("starmapgalaxydata Null");
            if (__instance.starmap.galaxyData != null)
            {
                __instance.starmap.galaxyData.Free();
            }
            if (galaxy == null) GS2.Warn("galaxy Null");
            //else GS2.Warn("Galaxy not null");
            __instance.starmap.galaxyData = galaxy;
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            if (GS2.generator.Config == null) GS2.Warn("GS2.generator.Config Null");
            if (GS2.generator.Config.DisableStarCountSlider)
            {
                GS2.Log("Disabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText == null) GS2.Warn("starcounttext null");
                if (starCountText == null)
                {
                    starCountText = CreateStarCountText(__instance.starCountSlider);
                }
                starCountText.GetComponent<Text>().text = galaxy.starCount.ToString() + "   (" + GS2.generator.Name + ")";


            }
            else
            {
                GS2.Log("Enabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null)
                {
                    starCountText.SetActive(false);
                }

                __instance.starCountSlider.gameObject.SetActive(true);
            }
            if (GS2.generator.Config.DisableSeedInput)
            {
                GS2.Log("Disabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                inputField.transform.parent.GetComponent<Text>().enabled = false;
                inputField.GetComponentInChildren<Text>().enabled = false;
                inputField.GetComponent<Image>().enabled = false;
            }
            else
            {
                GS2.Log("Enabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/galaxy-seed/InputField");
                inputField.transform.parent.GetComponent<Text>().enabled = true;
                inputField.GetComponentInChildren<Text>().enabled = true;
                inputField.GetComponent<Image>().enabled = true;
            }
            GS2.Log("End");
            return false;
        }
    }
}