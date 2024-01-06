using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class PatchOnSpaceSector
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(SpaceSector), nameof(SpaceSector.SetForNewGame))]
        public static IEnumerable<CodeInstruction> TranspilerSpaceSectorSetForNewGameAstroCount(
            IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions)
                // Replace 1024 with the result of the CalcAstroCapacity method
                .MatchForward(true, new CodeMatch(i => i.opcode == Ldc_I4 && (int)i.operand == 1024))
                .SetAndAdvance(Call, AccessTools.Method("PatchOnSpaceSector:CalcAstroCapacity"))//;

                //Remove the following code:
                // ldloc.s 5 (System.Int32
                // ldc.i4.s 100
                // blt Label7
                // ldc.i4.0 NULL
                // stloc.s 6 (System.Int32)
                .MatchForward(
                    false,
                    new CodeMatch(i=> i.opcode == Ldloc_S),
                    new CodeMatch(i=> i.opcode ==Ldc_I4_S && Convert.ToInt32(i.operand) == 100),
                    new CodeMatch(i=> i.opcode ==Blt),
                    new CodeMatch(i=> i.opcode ==Ldc_I4_0),
                    new CodeMatch(i=> i.opcode ==Stloc_S));
                if (codeMatcher.IsInvalid)
                {
                    GS2.LogTranspilerError(instructions, "TranspileSpaceSectorSetForNewGameAstroCount failed.");
  
                    return instructions;
                }
                if (codeMatcher.IsValid)
                {
                    GS2.Log("TranspileSpaceSectorSetForNewGameAstroCount succeeded.");
                }


                codeMatcher.RemoveInstruction();
                codeMatcher.RemoveInstruction();
                codeMatcher.RemoveInstruction();
                codeMatcher.RemoveInstruction();
                codeMatcher.RemoveInstruction();
            return codeMatcher.InstructionEnumeration();
        }

        private static int CalcAstroCapacity()
        {
            var cap = 1024 * Mathf.CeilToInt(GameMain.spaceSector.galaxy.starCount / 64f);
            GS2.Log($"Set AstroCapacity to {cap} with {GameMain.spaceSector.galaxy.starCount} stars");
        return cap;
        }
    }
}