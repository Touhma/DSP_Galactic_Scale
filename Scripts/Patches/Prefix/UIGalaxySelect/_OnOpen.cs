using System;
using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        public static GameObject StartButton;

        public static UnityAction startAction;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect._OnOpen))]
        public static bool _OnOpen(UIGalaxySelect __instance, ref Slider ___starCountSlider, ref Slider ___resourceMultiplierSlider)
        {
            //GS3.Warn("Fix");

            if (GS3.canvasOverlay)
            {
                //GS3.Warn("FIXING WITH GALAXYSELECT!");
                UIRoot.instance.overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                GS3.canvasOverlay = false;
            }

            if (NebulaCompat.IsClient) return true;
            if (SystemDisplay.backButton == null)
            {
                // SystemDisplay.startButton = __instance.transform.GetChild(0).GetComponent<Button>();
                // SystemDisplay.randomButton = __instance.transform.GetChild(1).GetComponent<Button>();
                // SystemDisplay.backButton = __instance.transform.GetChild(2).GetComponent<Button>();
                SystemDisplay.startButton = __instance.transform.GetChild(1).GetComponent<Button>();
                SystemDisplay.randomButton = __instance.transform.GetChild(2).GetComponent<Button>();
                SystemDisplay.backButton = __instance.transform.GetChild(3).GetComponent<Button>();
                SystemDisplay.InitializeButtons(__instance);
            }

            SystemDisplay.inSystemDisplay = false;
            UIRoot.instance.galaxySelect.cameraPoser.distRatio = 1;
            SystemDisplay.ShowStarCount();
            if (GS3.ActiveGenerator == null) return true;
            // SystemDisplay.backButton = __instance.find
            ___starCountSlider.maxValue = GS3.ActiveGenerator.Config.MaxStarCount;
            ___starCountSlider.minValue = GS3.ActiveGenerator.Config.MinStarCount;
            if (__instance.gameDesc == null) GS3.Warn("GameDesc Null");
            if (StartButton == null) StartButton = __instance.GetComponentInChildren<UIButton>()?.gameObject;

            if (StartButton == null) GS3.Warn("StartButton Null");
            StartButton?.SetActive(true);

            GS3.Log(StartButton?.name);
            __instance.random =
                //new DotNet35Random(GSSettings.Seed);
                new DotNet35Random((int)(DateTime.Now.Ticks / 10000L));
            __instance.gameDesc = new GameDesc();
            __instance.gameDesc?.SetForNewGame(UniverseGen.algoVersion, __instance.random.Next(100000000), GS3.ActiveGenerator.Config.DefaultStarCount, 1, GS3.Config.ResourceMultiplier);
            GS3.gameDesc = __instance.gameDesc;
            GS3.gameDesc.isPeaceMode = false;//0.10
            int[] itemIds = PropertySystem.itemIds; //0.10
            for (int i = 0; i < itemIds.Length; i++)//0.10
            {//0.10
                int itemId = itemIds[i];//0.10
                __instance.propertyItems[i].SetPropertyIcon(itemId, 0, true);//0.10
            }//0.10
            if (__instance.starmapGroup == null) GS3.Warn("smg Null");
            __instance.starmapGroup?.gameObject?.SetActive(true);
            if (__instance.starmap == null) GS3.Warn("starmap Null");
            __instance.starmap?._Open();
            if (__instance.gameDesc == null) GS3.Warn("GameDesc Null 2");
            if (__instance.gameDesc?.starCount <= 0) __instance.gameDesc.starCount = 1;
            __instance.SetStarmapGalaxy();
            // PlanetModelingManager.PrepareWorks();
            __instance.darkFogToggle.isOn = __instance.gameDesc.isCombatMode;//0.10
            __instance.uiCombat.gameDesc = __instance.gameDesc;//0.10
            var grids = GameObject.Find("UI Root/Galaxy Select Starmap/grids");
            if (grids != null)
                for (var i = 0; i < grids.transform.childCount; i++)
                {
                    var grid = grids.transform.GetChild(i);
                    if (grid.name != "grid-0" && grid.name != "stars") grid.gameObject.SetActive(false);
                }
            if (DSPGame.CombatPlayedCount() == 0)//0.10
            {//0.10
                __instance.uiCombat.advisorEnabled = true;//0.10
            }//0.10
            SystemDisplay.InitHelpText(__instance);
            return false;
        }
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        //public static void _OnOpenPostfix(ref UIGalaxySelect __instance, ref Slider ___resourceMultiplierSlider)
        //{
        //        __instance.gameDesc.resourceMultiplier = GS3.Config.ResourceMultiplier;       
        //}
    }
}