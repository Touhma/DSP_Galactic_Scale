using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnPlanetFactory
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.EndFlattenTerrain))]
        private static IEnumerable<CodeInstruction> EndFlattenTerrainRadiusTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            for (var i = 0; i + 4 < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_R4 && code[i].OperandIs(100f)
                    && code[i + 1].opcode == OpCodes.Mul
                    && code[i + 2].opcode == OpCodes.Ldc_R4 && code[i + 2].OperandIs(20f)
                    && code[i + 3].opcode == OpCodes.Add
                    && code[i + 4].opcode == OpCodes.Conv_I2)
                {
                    code[i + 4].opcode = OpCodes.Conv_I4;
                    return code;
                }
            }

            GS2.Warn("EndFlattenTerrain radius threshold pattern not found; batched builds on very large planets may use the vanilla short conversion.");
            return code;
        }

        // change
        // short num5 = (short)(planet.realRadius * 100f + 20f);
        // to
        // int newLocalVar = (int)(planet.realRadius * 100f + 20f)
        // assign newLocalVar to all variables which request num5
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.FlattenTerrain))]
        private static IEnumerable<CodeInstruction> PlanetFactory_FlattenTerrain_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var intNum5 = generator.DeclareLocal(typeof(int));

            var matcher = new CodeMatcher(instructions);

            // find codes look like (short)(A * 100f + 20f)
            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldc_R4, 100f), new CodeMatch(OpCodes.Mul), new CodeMatch(OpCodes.Ldc_R4, 20f), new CodeMatch(OpCodes.Add), new CodeMatch(OpCodes.Conv_I2), new CodeMatch(OpCodes.Stloc_S));

            if (matcher.IsInvalid)
            {
                GS2.Error("PlanetFactory_FlattenTerrain_Transpiler: fail to find codes look like (short)(A * 100f + 20f)");
                return instructions;
            }

            // index of the old varialbe
            var oldNum5Index = ((LocalBuilder)matcher.Operand).LocalIndex;

            // assign the result of the calculation to new local variable
            matcher.Advance(-1); // move back to conv.i2
            matcher.SetOpcodeAndAdvance(OpCodes.Conv_I4); // conv.i2 -> conv.i4
            matcher.SetAndAdvance(OpCodes.Stloc_S, intNum5.LocalIndex); // stloc.s 13 -> stloc.s intNum5

            // find all places reading old variable, replace them with new local variable
            while (matcher.IsValid)
            {
                matcher.MatchForward(true, new CodeMatch(instruction => instruction.opcode == OpCodes.Ldloc_S && ((LocalBuilder)instruction.operand).LocalIndex == oldNum5Index));
                if (matcher.IsValid) matcher.SetAndAdvance(OpCodes.Ldloc_S, intNum5.LocalIndex);
            }

            // find codes look like A = new Type[1024]
            matcher.Start();
            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4, 1024), new CodeMatch(OpCodes.Newarr), new CodeMatch(OpCodes.Stfld));

            if (matcher.IsInvalid)
            {
                GS2.Error("PlanetFactory_FlattenTerrain_Transpiler: fail to find codes look like A = new Type[1024]");
                return instructions;
            }

            matcher.SetOperandAndAdvance(4096);

            return matcher.InstructionEnumeration();
        }
    }
}
