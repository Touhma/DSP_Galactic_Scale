﻿using HarmonyLib;
using NebulaCompatibility;
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
            GS2.Warn("B");
            if (NebulaCompat.IsMultiplayerActive && NebulaCompat.IsClient && !GSSettings.lobbyReceivedUpdateValues)
            {
                GS2.Warn("Running Nebula Code");
                NebulaCompat.SendPacket(new LobbyRequestUpdateSolarSystems());
                GS2.Warn("Nebula Requested Update");
                return false;
            }

            GSSettings.lobbyReceivedUpdateValues = false;

            // GS2.Log("Start");
            if (__instance.gameDesc != null && __instance.gameDesc.starCount <= 0) __instance.gameDesc.starCount = 1;

            GalaxyData galaxy; // = __instance.starmap.galaxyData;

            // if (GS2.Vanilla)
            //     galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
            // else
                // GS2.Warn("Processing Galaxy");
                galaxy = GS2.ProcessGalaxy(__instance.gameDesc, true); //sp00ktober you probably want to alter this instead

            // GS2.Warn("Done");
            if (__instance.starmap == null) GS2.Warn("Starmap Null");
            if (__instance.starmap.galaxyData == null) GS2.Warn("starmapgalaxydata Null");
            // GS2.Warn("TESTING1");
            if (__instance.starmap.galaxyData != null)
            {
                // GS2.Warn("TESTING2");
                // GS2.Warn("Freeing GalaxyData!!!!!!!!!!!!!!!!!!!");
                __instance.starmap.galaxyData.Free();
            }

            if (galaxy == null) GS2.Warn("galaxy Null");
            // else GS2.Warn("Galaxy not null");
            __instance.starmap.galaxyData = galaxy;
            // GS2.Warn("TESTING3");
            // GameMain.data.galaxy = galaxy; // this line is important to let the client load into the correct galaxy lol

            // __instance.UpdateUIDisplay(__instance.starmap.galaxyData);
            __instance.UpdateUIDisplay(galaxy);
            __instance.UpdateParametersUIDisplay();
            __instance.autoCameraYaw = true;
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            __instance.autoRotateSpeed = 0.0f;
            // GS2.Warn("TESTING4");
            if (GS2.ActiveGenerator.Config == null) GS2.Warn("GS2.generator.Config Null");
            // GS2.Warn("TESTING5");
            if (GS2.ActiveGenerator.Config.DisableStarCountSlider)
            {
                // GS2.Log("Disabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText == null) GS2.Warn("starcounttext null");
                if (starCountText == null) starCountText = CreateStarCountText(__instance.starCountSlider);
                //starCountText.GetComponent<Text>().text = __instance.starmap.galaxyData.starCount + "   (" + GS2.ActiveGenerator.Name + ")";
                starCountText.GetComponent<Text>().text = galaxy.starCount + "   (" + GS2.ActiveGenerator.Name + ")";
            }
            else
            {
                // GS2.Log("Enabling StarCountSlider");
                var starCountText = GameObject.Find("GS Star Count");
                if (starCountText != null) starCountText.SetActive(false);

                __instance.starCountSlider.gameObject.SetActive(true);
            }
            // GS2.Warn("TESTING6");
            if (GS2.ActiveGenerator.Config.DisableSeedInput)
            {
                // GS2.Log("Disabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/galaxy-seed/InputField");
                if (inputField != null)
                {
                    inputField.transform.parent.GetComponent<Text>().enabled = false;
                    inputField.GetComponentInChildren<Text>().enabled = false;
                    inputField.GetComponent<Image>().enabled = false;
                }
            }
            else
            {
                // GS2.Log("Enabling SeedInput");
                var inputField = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/galaxy-seed/InputField");
                if (inputField != null)
                {
                    inputField.transform.parent.GetComponent<Text>().enabled = true;
                    inputField.GetComponentInChildren<Text>().enabled = true;
                    inputField.GetComponent<Image>().enabled = true;
                }
            }

            // GS2.Log("End");
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "SetStarmapGalaxy")]
        public static void SetStarmapGalaxy_Postfix(UIGalaxySelect __instance)
        {
            // this is needed at least in the nebula lobby, throws index out of bounds if not done.
            if (GS2.galaxy == null)
            {
                GS2.Log("Galaxy Null, Returning from Postfix");
                return;
            }
            // GS2.Warn("A");
            // if (!NebulaCompat.IsMultiplayerActive)
            // {
            //     GS2.Log("Nebula Null, Returning from Postfix");
            //
            //     return;
            // }

            // GS2.Warn("B");

            // if (GameMain.universeSimulator == null)
            // {
            //     GS2.Warn("Instantiating UniverseSimulator");
            //     GameMain.universeSimulator = Object.Instantiate(Configs.builtin.universeSimulatorPrefab);
            //     GameMain.universeSimulator.gameObject.name = "Universe";
            //     GameMain.universeSimulator.galaxyData = GS2.galaxy;
            // }
            // else
            // {
            //     GameMain.instance.DestroyUniverseSimulator();
            //     GS2.Warn("Re-Instantiating UniverseSimulator");
            //     GameMain.universeSimulator = Object.Instantiate(Configs.builtin.universeSimulatorPrefab);
            //     GameMain.universeSimulator.gameObject.name = "Universe";
            //     GameMain.universeSimulator.galaxyData = GS2.galaxy;
            // }
            //
            // GameMain.universeSimulator.starSimulators = new StarSimulator[GS2.galaxy.starCount];
            // for (var i = 0; i < GS2.galaxy.starCount; i++)
            // {
            //     GS2.Warn($"Instantiating StarSimulator {i} / {GameMain.universeSimulator.starSimulators.Length}");
            //     var star = GS2.galaxy.stars[i];
            //     if (star == null) continue;
            //     GameMain.universeSimulator.starSimulators[i] = Object.Instantiate(GameMain.universeSimulator.starPrefab, GameMain.universeSimulator.transform);
            //     GameMain.universeSimulator.starSimulators[i].universeSimulator = GameMain.universeSimulator;
            //     GameMain.universeSimulator.starSimulators[i].SetStarData(star);
            //     GameMain.universeSimulator.starSimulators[i].gameObject.name = star.displayName;
            //     GameMain.universeSimulator.starSimulators[i].gameObject.layer = 24;
            //     GameMain.universeSimulator.starSimulators[i].gameObject.SetActive(true);
            // }

            var starmap = __instance.starmap;
            // Increase Pool Count to prevent Nebula from failing to initialize system view when starcount < planetcount
            while (starmap.starPool.Count <= 100)
            {
                var starNode2 = new UIVirtualStarmap.StarNode();
                starNode2.active = false;
                starNode2.starData = null;
                starNode2.pointRenderer = Object.Instantiate(starmap.starPointPrefab, starmap.starPointPrefab.transform.parent);
                starNode2.nameText = Object.Instantiate(starmap.nameTextPrefab, starmap.nameTextPrefab.transform.parent);
                starmap.starPool.Add(starNode2);
            }

            while (starmap.connPool.Count <= 100)
                starmap.connPool.Add(new UIVirtualStarmap.ConnNode
                {
                    active = false,
                    starA = null,
                    starB = null,
                    lineRenderer = Object.Instantiate(starmap.connLinePrefab, starmap.connLinePrefab.transform.parent)
                });
            // End 

            // GS2.Warn("End");
        }
    }
}