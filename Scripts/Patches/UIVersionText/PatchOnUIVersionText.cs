using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIVersionText
    {
        private static string loadingText = "";
        private static string oldLoadingText = "";
        private static string baseText = "";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static void RefreshPostfix(ref Text ___textComp, bool ___firstFrame)
        {
            var inSystemDisplay = SystemDisplay.inSystemDisplay;
            if (inSystemDisplay)
            {
                if (string.IsNullOrEmpty(baseText)) baseText = ___textComp.text;
                oldLoadingText = loadingText;
                var flag = SystemDisplay.viewStar != null;
                if (flag)
                {
                    var viewStar = SystemDisplay.viewStar;
                    var flag2 = !viewStar.scanned;
                    if (flag2)
                    {
                        loadingText = baseText + "\r\nStar Loading...\r\n";
                        var num = 0;
                        var gsstar = GS2.GetGSStar(viewStar);
                        foreach (var gsplanet in gsstar.Bodies)
                        {
                            var flag3 = !gsplanet.planetData.scanning && !gsplanet.planetData.scanned;
                            if (flag3) gsplanet.planetData.RunScanThread();
                            var flag4 = !gsplanet.planetData.scanned;
                            if (flag4) num++;
                        }

                        var bodyCount = gsstar.bodyCount;
                        loadingText += string.Format("Calculating planet {0}/{1}...\r\n".Translate(), bodyCount - num, bodyCount);
                    }
                    else
                    {
                        loadingText = "";
                    }
                }
                else
                {
                    loadingText = "";
                }

                var flag5 = loadingText != oldLoadingText;
                if (flag5) ___textComp.text = baseText + loadingText;
            }

            if (GS2.IsMenuDemo || !GameMain.isRunning)
            {
                if (!string.IsNullOrEmpty(baseText)) ___textComp.text = baseText;
                return;
            }
            if (string.IsNullOrEmpty(baseText)) baseText = ___textComp.text;
            if (___textComp != null && GameMain.localStar != null)
            {
                oldLoadingText = loadingText;
                if (GameMain.localStar != null && !GameMain.localStar.loaded)
                    loadingText = "\r\n" + "Loading Planets: ".Translate() + HandleLocalStarPlanets.GetStarLoadingStatus(GameMain.localStar);
                else loadingText = "";
                if (GameMain.localStar != null && GameMain.localStar.loaded) loadingText = "";
                if (GameMain.localStar == null) loadingText = "";
                if (loadingText != oldLoadingText) ___textComp.text = baseText + loadingText;
            }
        }
    }
}