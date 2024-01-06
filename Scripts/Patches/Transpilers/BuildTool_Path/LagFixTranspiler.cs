using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    // Thanks starfish :)
    public class LagFixTranspiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Click), nameof(BuildTool_Click.CreatePrebuilds))]
        [HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.CreatePrebuilds))]
        [HarmonyPatch(typeof(BuildTool_Addon), nameof(BuildTool_Path.CreatePrebuilds))] // Not compatible with 0.9.26
        [HarmonyPatch(typeof(BuildTool_Inserter), nameof(BuildTool_Inserter.CreatePrebuilds))]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), nameof(BuildTool_BlueprintPaste.CreatePrebuilds))]
        static IEnumerable<CodeInstruction> Real_Transpiler3(IEnumerable<CodeInstruction> instructions)
        {
            // Remove force GC.Collect()
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "Collect"));

            if (matcher.IsInvalid)
                return instructions;

            return matcher.SetOpcodeAndAdvance(OpCodes.Nop).InstructionEnumeration();
        }
    }
}