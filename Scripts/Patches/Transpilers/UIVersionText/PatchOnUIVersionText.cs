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
        [HarmonyTranspiler]
        [HarmonyBefore("dsp.nebula - multiplayer")]
        [HarmonyPatch(typeof(UIVersionText), nameof(UIVersionText.Refresh))]
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
    }
}