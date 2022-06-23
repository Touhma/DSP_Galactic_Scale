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

        [HarmonyTranspiler, HarmonyBefore("dsp.nebula - multiplayer")]
        [HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).MatchForward(true, new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "userName")).Advance(1).InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>(text =>
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
            bool inSystemDisplay = SystemDisplay.inSystemDisplay;
            if (inSystemDisplay)
            {
                if (string.IsNullOrEmpty(baseText)) baseText = ___textComp.text;
                PatchOnUIVersionText.oldLoadingText = PatchOnUIVersionText.loadingText;
                bool flag = SystemDisplay.viewStar != null;
                if (flag)
                {
                    StarData viewStar = SystemDisplay.viewStar;
                    bool flag2 = !viewStar.calculated;
                    if (flag2)
                    {
                        PatchOnUIVersionText.loadingText = baseText + "\r\nStar Loading...\r\n";
                        int num = 0;
                        GSStar gsstar = GS2.GetGSStar(viewStar);
                        foreach (GSPlanet gsplanet in gsstar.Bodies)
                        {
                            bool flag3 = !gsplanet.planetData.calculating && !gsplanet.planetData.calculated;
                            if (flag3)
                            {
                                gsplanet.planetData.RunCalculateThread();
                            }
                            bool flag4 = !gsplanet.planetData.calculated;
                            if (flag4)
                            {
                                num++;
                            }
                        }
                        int bodyCount = gsstar.bodyCount;
                        PatchOnUIVersionText.loadingText += string.Format("Calculating planet {0}/{1}...\r\n", bodyCount - num, bodyCount);
                    }
                    else
                    {
                        PatchOnUIVersionText.loadingText = "";
                    }
                }
                else
                {
                    PatchOnUIVersionText.loadingText = "";
                }
                bool flag5 = PatchOnUIVersionText.loadingText != PatchOnUIVersionText.oldLoadingText;
                if (flag5)
                {
                    ___textComp.text = PatchOnUIVersionText.baseText + PatchOnUIVersionText.loadingText;
                }
            }
            if (GS2.IsMenuDemo || !GameMain.isRunning) return;
            if (string.IsNullOrEmpty(baseText)) baseText = ___textComp.text;
            if (___textComp != null && GameMain.localStar != null)
            {
                oldLoadingText = loadingText;
                if (GameMain.localStar != null && !GameMain.localStar.loaded)
                    loadingText = "\r\nLoading Planets:" + HandleLocalStarPlanets.GetStarLoadingStatus(GameMain.localStar);
                else loadingText = "";
                if (GameMain.localStar != null && GameMain.localStar.loaded) loadingText = "";
                if (GameMain.localStar == null) loadingText = "";
                if (loadingText != oldLoadingText) ___textComp.text = baseText + loadingText;
            }
        }
    }
}