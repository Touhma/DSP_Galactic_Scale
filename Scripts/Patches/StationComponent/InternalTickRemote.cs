using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnStationComponent
    {
        // [HarmonyDebug]
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static IEnumerable<CodeInstruction> BuildTool_Click_DeterminePreviews_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeMatcher = new CodeMatcher(instructions, il).MatchForward(false,
                new CodeMatch(op => op.opcode == OpCodes.Ldc_I4_S && op.OperandIs(10))); // Search for ldc.i4.s 10

            if (codeMatcher.IsInvalid)
            {
                GS2.Error("InternalTickRemote Transpiler Failed");
                return instructions;
            }
            instructions = codeMatcher.Repeat(z => z // Repeat for all occurences 
                   .Set(OpCodes.Ldc_I4_S, 99)) // Replace operand with 99
                .InstructionEnumeration();
            return instructions;
        }
    }
}