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
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4) &&
                               (
                                   Convert.ToDouble(i.operand ?? 0.0) == 201.0
                            );
                    })
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("PowerSystem.CalculateGeothermalStrength transpiler: 201f constant not found (game update changed the method?). Returning original code - geothermal strength will use vanilla radius.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    m.Advance(1);
                    m.SetAndAdvance(Ldarg_0, null);
                    m.InsertAndAdvance(new CodeInstruction(Call, AccessTools.Method(typeof(PatchOnPowerSystem), nameof(FixRadius))));
                }).InstructionEnumeration();
        }
    }
}