using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnPlanetFactory
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.InitVeinGroups), typeof(PlanetData))]
        public static bool InitVeinGroups(PlanetFactory __instance, PlanetData planet)
        {
            var veinGroupsLock = planet.veinGroupsLock;
            lock (veinGroupsLock)
            {
                if (planet.veinGroups == null) planet.veinGroups = new VeinGroup[1]; //<- this was added
                var num = planet.veinGroups.Length;
                var num2 = num >= 1 ? num : 1;
                __instance.veinGroups = new VeinGroup[num2];
                Array.Copy(planet.veinGroups, __instance.veinGroups, num);
                __instance.veinGroups[0].SetNull();
            }

            return false;
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
                GS3.Error("PlanetFactory_FlattenTerrain_Transpiler: fail to find codes look like (short)(A * 100f + 20f)");
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
                GS3.Error("PlanetFactory_FlattenTerrain_Transpiler: fail to find codes look like A = new Type[1024]");
                return instructions;
            }

            matcher.SetOperandAndAdvance(4096);

            return matcher.InstructionEnumeration();
        }
    }
}