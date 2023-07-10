using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.UI;

namespace GalacticScale
{
    public class PatchOnUIVersionText
    {
        private static string loadingText = "";
        private static string oldLoadingText = "";
        private static string baseText = "";

        [HarmonyTranspiler]
        [HarmonyBefore("dsp.nebula - multiplayer")]
        [HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions).MatchForward(true, new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "userName"));
            if (matcher.IsInvalid)
            {
                matcher.Start().MatchForward(true, new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_usernameAndSuffix"));
            }
            if (matcher.IsInvalid)
            {
                GS2.Warn("PatchOnUIVersionText.Refresh_Transpiler failed. GalacticScale mod version won't show up.");
                return instructions;
            }

            instructions = matcher.Advance(1).InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>(text =>
            {
                text = $"Galactic Scale v {GS2.Version}\r\n{text}";
                //if (!GS2.IsMenuDemo && GS2.Config.DebugMode) text += $"\r\n{GameMain.data.factories.Length} Factories\r\n{GameMain.data.warningSystem.tmpEntityPools.Length} tmpEntityPools";
                return text;
            })).InstructionEnumeration();
            return instructions;
        }

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
                    var flag2 = !viewStar.calculated;
                    if (flag2)
                    {
                        loadingText = baseText + "\r\nStar Loading...\r\n";
                        var num = 0;
                        var gsstar = GS2.GetGSStar(viewStar);
                        foreach (var gsplanet in gsstar.Bodies)
                        {
                            var flag3 = !gsplanet.planetData.calculating && !gsplanet.planetData.calculated;
                            if (flag3) gsplanet.planetData.RunCalculateThread();
                            var flag4 = !gsplanet.planetData.calculated;
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