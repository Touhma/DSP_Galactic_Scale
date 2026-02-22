using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public static class EnemyUnitComponentTranspiler
    {
        // Change Log:
        // - 2026-02-22: Cap space pathing star radius reads to vanilla max for Dark Fog movement.

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Defense_Ground))] //225
        public static IEnumerable<CodeInstruction> Fix225(IEnumerable<CodeInstruction> instructions)
        {
            // Bootstrap.DumpInstructions(instructions, nameof(EnemyUnitComponent.RunBehavior_Defense_Ground),290, 20);
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 225.0) < 0.01f)
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    var mi = matcher.GetRadiusFromAltitude();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(Utils.LoadArgument(5));
                    matcher.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRaider))] //200 206 202
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRanger))] //200 212 225
        public static IEnumerable<CodeInstruction> Fix200_225(IEnumerable<CodeInstruction> instructions)
        {
            // var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromFactory));
            
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                               (
                                    Convert.ToDouble(i.operand ?? 0.0) == 200.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 202.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 206.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 212.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 225.0 

                            );
                    })
                )
                .Repeat(matcher =>
                {
                    var mi = matcher.GetRadiusFromFactory();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    matcher.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_SHumpback))] //200 but need to find the planet...
        // [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            // var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromEnemyData));
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                )
                .Repeat(matcher =>
                {
                    var mi = matcher.GetRadiusFromEnemyData();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_S, (sbyte)3));
                    matcher.Insert(new CodeInstruction(Call, mi));

                    // Bootstrap.DumpMatcherPost(matcher, 3, 5, 5);
                }).InstructionEnumeration();

            return instructions;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200Slancer(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                )
                .Repeat(matcher =>
                {
                    // var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    var mi =matcher.GetRadiusFromEnemyData();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(Utils.LoadArgument(4));
                    matcher.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.ApproachToTargetPoint_SLancer))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToHive_Space))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToHive_Space_FollowLeader))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToTargetPoint_Space))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToTargetPoint_Space_FollowLeader))]
        public static IEnumerable<CodeInstruction> CapStarRadiusToVanilla(IEnumerable<CodeInstruction> instructions)
        {
            var radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            var capMethod = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.LoadsField(radiusField))
                {
                    yield return new CodeInstruction(Call, capMethod);
                }
            }
        }
    }
}