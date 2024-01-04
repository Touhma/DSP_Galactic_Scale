using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.SetStarmapGalaxy))]
        public static bool SetStarmapGalaxy(ref UIGalaxySelect __instance)
        {
            GS3.Warn("B");
            if (NebulaCompat.IsMultiplayerActive && NebulaCompat.IsClient && !GSSettings.lobbyReceivedUpdateValues)
            {
                GS3.Warn("Running Nebula Code");
                NebulaCompat.SendPacket(new LobbyRequestUpdateSolarSystems());
                GS3.Warn("Nebula Requested Update");
                return false;
            }

            GSSettings.lobbyReceivedUpdateValues = false;

            // GS3.Log("Start");
            if (__instance.gameDesc != null && __instance.gameDesc.starCount <= 0) __instance.gameDesc.starCount = 1;

            GalaxyData galaxy; // = __instance.starmap.galaxyData;

            // if (GS3.Vanilla)
            //     galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            // else
                // GS3.Warn("Processing Galaxy");
                galaxy = GS3.ProcessGalaxy(__instance.gameDesc, true); //sp00ktober you probably want to alter this instead

            // GS3.Warn("Done");
            if (__instance.starmap == null) GS3.Warn("Starmap Null");
            if (__instance.starmap.galaxyData == null) GS3.Warn("starmapgalaxydata Null");
            // GS3.Warn("TESTING1");
            if (__instance.starmap.galaxyData != null)
            {
                // GS3.Warn("TESTING2");
                // GS3.Warn("Freeing GalaxyData!!!!!!!!!!!!!!!!!!!");
                __instance.starmap.galaxyData.Free();
            }

            if (galaxy == null) GS3.Warn("galaxy Null");
            // else GS3.Warn("Galaxy not null");
            __instance.starmap.galaxyData = galaxy;
            // GS3.Warn("TESTING3");
            // GameMain.data.galaxy = galaxy; // this line is important to let the client load into the correct galaxy lol

            // __instance.UpdateUIDisplay(__instance.starmap.galaxyData);
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            // GS3.Warn("TESTING4");
            if (GS3.ActiveGenerator.Config == null) GS3.Warn("GS3.generator.Config Null");
            // GS3.Warn("TESTING5");
            if (GS3.ActiveGenerator.Config.DisableStarCountSlider)
            {
                // GS3.Log("Disabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText == null) GS3.Warn("starcounttext null");
                if (starCountText == null) starCountText = Utils.CreateStarCountText(__instance.starCountSlider);
                //starCountText.GetComponent<Text>().text = __instance.starmap.galaxyData.starCount + "   (" + GS3.ActiveGenerator.Name + ")";
                starCountText.GetComponent<Text>().text = galaxy.starCount + "   (" + GS3.ActiveGenerator.Name + ")";
            }
            else
            {
                // GS3.Log("Enabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null) starCountText.SetActive(false);

                __instance.starCountSlider.gameObject.SetActive(true);
            }
            // GS3.Warn("TESTING6");
            if (GS3.ActiveGenerator.Config.DisableSeedInput)
            {
                // GS3.Log("Disabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/galaxy-seed/InputField");
                inputField.transform.parent.GetComponent<Text>().enabled = false;
                inputField.GetComponentInChildren<Text>().enabled = false;
                inputField.GetComponent<Image>().enabled = false;
            }
            else
            {
                // GS3.Log("Enabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/galaxy-seed/InputField");
                inputField.transform.parent.GetComponent<Text>().enabled = true;
                inputField.GetComponentInChildren<Text>().enabled = true;
                inputField.GetComponent<Image>().enabled = true;
            }

            // GS3.Log("End");
            return false;
        }
    }
}