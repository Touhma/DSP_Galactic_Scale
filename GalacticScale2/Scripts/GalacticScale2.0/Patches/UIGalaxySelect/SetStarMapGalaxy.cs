using HarmonyLib;
using NebulaAPI;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "SetStarmapGalaxy")]
        public static bool SetStarmapGalaxy(ref UIGalaxySelect __instance)
        {
            if (NebulaModAPI.MultiplayerSession != null && NebulaModAPI.MultiplayerSession.LocalPlayer.IsClient && !GSSettings.lobbyReceivedUpdateValues)
            {
                NebulaModAPI.MultiplayerSession.Network.SendPacket(new LobbyRequestUpdateSolarSystems());
                GS2.Warn("Nebula Requested Update");
                return false;
            }
            GSSettings.lobbyReceivedUpdateValues = false;

            GS2.Log("Start");
            if (__instance.gameDesc == null) GS2.Warn("GameDesc Null 3");
            if (__instance.gameDesc.starCount <= 0) __instance.gameDesc.starCount = 1;

            GalaxyData galaxy;// = __instance.starmap.galaxyData;

            if (GS2.Vanilla)
                galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            else
                //GS2.Warn("Processing Galaxy");
                galaxy = GS2.ProcessGalaxy(__instance.gameDesc, true); //sp00ktober you probably want to alter this instead

            //GS2.Warn("Done");
            if (__instance.starmap == null) GS2.Warn("Starmap Null");
            if (__instance.starmap.galaxyData == null) GS2.Warn("starmapgalaxydata Null");
            if (__instance.starmap.galaxyData != null)
            {
                GS2.Warn("Freeing GalaxyData!!!!!!!!!!!!!!!!!!!");
                __instance.starmap.galaxyData.Free();
            }
            if (galaxy == null) GS2.Warn("galaxy Null");
            //else GS2.Warn("Galaxy not null");
            __instance.starmap.galaxyData = galaxy;
            // GameMain.data.galaxy = galaxy; // this line is important to let the client load into the correct galaxy lol

            // __instance.UpdateUIDisplay(__instance.starmap.galaxyData);
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            if (GS2.ActiveGenerator.Config == null) GS2.Warn("GS2.generator.Config Null");
            if (GS2.ActiveGenerator.Config.DisableStarCountSlider)
            {
                GS2.Log("Disabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText == null) GS2.Warn("starcounttext null");
                if (starCountText == null) starCountText = CreateStarCountText(__instance.starCountSlider);
                //starCountText.GetComponent<Text>().text = __instance.starmap.galaxyData.starCount + "   (" + GS2.ActiveGenerator.Name + ")";
                starCountText.GetComponent<Text>().text = galaxy.starCount + "   (" + GS2.ActiveGenerator.Name + ")";
            }
            else
            {
                GS2.Log("Enabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null) starCountText.SetActive(false);

                __instance.starCountSlider.gameObject.SetActive(true);
            }

            if (GS2.ActiveGenerator.Config.DisableSeedInput)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "SetStarmapGalaxy")]
        public static void SetStarmapGalaxy_Postfix(UIGalaxySelect __instance)
        {
            // this is needed at least in the nebula lobby, throws index out of bounds if not done.
            if(GS2.galaxy == null)
            {
                GS2.Log("Galaxy Null, Returning from Postfix");
                return;
            }
            if (NebulaModAPI.MultiplayerSession == null)
            {
                GS2.Log("Nebula Null, Returning from Postfix");

                return;
            }
            if(GameMain.universeSimulator == null)
            {
                GameMain.universeSimulator = UnityEngine.Object.Instantiate<UniverseSimulator>(Configs.builtin.universeSimulatorPrefab);
                GameMain.universeSimulator.gameObject.name = "Universe";
                GameMain.universeSimulator.galaxyData = GS2.galaxy;
            }
            GameMain.universeSimulator.starSimulators = new StarSimulator[GS2.galaxy.starCount];
            for (int i = 0; i < GS2.galaxy.starCount; i++)
            {
                StarData star = GS2.galaxy.stars[i];
                if(star == null)
                {
                    continue;
                }
                GameMain.universeSimulator.starSimulators[i] = UnityEngine.Object.Instantiate<StarSimulator>(GameMain.universeSimulator.starPrefab, GameMain.universeSimulator.transform);
                GameMain.universeSimulator.starSimulators[i].universeSimulator = GameMain.universeSimulator;
                GameMain.universeSimulator.starSimulators[i].SetStarData(star);
                GameMain.universeSimulator.starSimulators[i].gameObject.name = star.displayName;
                GameMain.universeSimulator.starSimulators[i].gameObject.layer = 24;
                GameMain.universeSimulator.starSimulators[i].gameObject.SetActive(true);
            }
        }
    }
}