using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public partial class PatchOnDFRelayComponent
    {
        //Not even sure this is needed...

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent),  nameof(DFRelayComponent.RelaySailLogic))]
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {

            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                               (
                                   Convert.ToDouble(i.operand ?? 0.0) == 200.0

                               );
                    })
                )
                .Repeat(matcher =>
                {
                    // var methodInfo = AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(Utils.GetRadiusFromAstroId));
                    // var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(int));
                    var mi = matcher.GetRadiusFromAstroId();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    matcher.InsertAndAdvance(new CodeInstruction(Ldfld, AccessTools.Field(typeof(DFRelayComponent), nameof(DFRelayComponent.targetAstroId))));
                    matcher.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        
        
       
    }
}