using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetGrid
    {
 [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcLocalGridSize))]
        public static IEnumerable<CodeInstruction> CalcLocalGridSize_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions);
                matcher.MatchForward(
                    false,
                    new CodeMatch(i =>
                        i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 0.0031415927) < 0.01),
                    new CodeMatch(i => i.opcode == Ldloc_2),
                    new CodeMatch(i => i.opcode == Call),
                    new CodeMatch(i =>
                        i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 3.1415927) < 0.1),
                    new CodeMatch(i => i.opcode == Mul));
                if (!matcher.IsInvalid)
                {
                    matcher.SetInstruction(new CodeInstruction(Nop));
                }

                matcher.MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R4 && Convert.ToInt32(i.operand ?? 0.0) == 5),
                    new CodeMatch(i => i.opcode == Mul),
                    new CodeMatch(i => i.opcode == Div),
                    new CodeMatch(i => i.opcode == Call && i.operand.ToString().Contains("Max")));
                if (!matcher.IsInvalid)
                {
                   matcher.SetInstruction(new CodeInstruction(Nop));

                    instructions = matcher.InstructionEnumeration();
                    return instructions;
                }
                Bootstrap.Logger.LogError("PatchOnPlanetGrid.CalcLocalGridSize_Transpiler failed");
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("PatchOnPlanetGrid.CalcLocalGridSize_Transpiler failed (Caught)");
                return instructions;
            }
        }
    }
}