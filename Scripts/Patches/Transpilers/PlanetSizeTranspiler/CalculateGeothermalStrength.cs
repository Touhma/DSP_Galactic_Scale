using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public partial class PatchOnPowerSystem
    {
        public float FixRadius(PowerSystem instance)
        {
            return instance.planet.realRadius + 1;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PowerSystem),  nameof(PowerSystem.CalculateGeothermalStrength))]

    public static IEnumerable<CodeInstruction> Fix201f(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4) &&
                               (
                                   Convert.ToDouble(i.operand ?? 0.0) == 201.0
                            );
                    })
                )
                .Repeat(matcher =>
                {
                    matcher.Advance(1);
                    matcher.SetAndAdvance(Ldarg_0, null);
                    matcher.InsertAndAdvance(new CodeInstruction(Call, AccessTools.Method(typeof(PatchOnPowerSystem), nameof(FixRadius))));
                }).InstructionEnumeration();

            return instructions;
        }
    }
}