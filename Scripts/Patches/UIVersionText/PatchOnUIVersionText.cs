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
        [HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static IEnumerable<CodeInstruction> Refresh_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).MatchForward(true, new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "userName")).Advance(1).InsertAndAdvance(Transpilers.EmitDelegate<Func<string, string>>(text =>
            {
                text = $"Galactic Scale v {GS2.Version}\r\n{text}";
                if (!GS2.IsMenuDemo && GS2.Config.DebugMode) text += $"\r\n{GameMain.data.factories.Length} Factories\r\n{GameMain.data.warningSystem.tmpEntityPools.Length} tmpEntityPools";
                return text;
            })).InstructionEnumeration();
            return instructions;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVersionText), "Refresh")]
        public static void RefreshPostfix(ref Text ___textComp, bool ___firstFrame)
        {
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