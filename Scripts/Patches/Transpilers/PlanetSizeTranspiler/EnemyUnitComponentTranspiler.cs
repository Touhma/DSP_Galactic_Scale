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
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 225.0) < 0.01f)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("EnemyUnitComponent.RunBehavior_Defense_Ground transpiler: 225 constant not found (game update changed the method?). Returning original code - DF ground defense range will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromAltitude();
                    m.Advance(1);
                    m.InsertAndAdvance(Utils.LoadArgument(5));
                    m.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRaider))] //200 206 202
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRanger))] //200 212 225
        public static IEnumerable<CodeInstruction> Fix200_225(IEnumerable<CodeInstruction> instructions)
        {
            // var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromFactory));
            
            var matcher = new CodeMatcher(instructions)
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
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("EnemyUnitComponent GRaider/GRanger transpiler: radius constants not found (game update changed the method?). Returning original code - DF ground raider ranges will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromFactory();
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    m.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_SHumpback))] //200 but need to find the planet...
        // [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            // var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(Utils.GetRadiusFromEnemyData));
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("EnemyUnitComponent.RunBehavior_Engage_SHumpback transpiler: 200.0 constant not found (game update changed the method?). Returning original code - Humpback engagement will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromEnemyData();
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_S, (sbyte)3));
                    m.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200Slancer(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer transpiler: 200.0 constant not found (game update changed the method?). Returning original code - Lancer orbit will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromEnemyData();
                    m.Advance(1);
                    m.InsertAndAdvance(Utils.LoadArgument(4));
                    m.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.ApproachToTargetPoint_SLancer))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToHive_Space))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToHive_Space_FollowLeader))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToTargetPoint_Space))]
        [HarmonyPatch(typeof(EnemyUnitComponent), nameof(EnemyUnitComponent.SeekToTargetPoint_Space_FollowLeader))]
        public static IEnumerable<CodeInstruction> CapStarRadiusToVanilla(IEnumerable<CodeInstruction> instructions, System.Reflection.MethodBase __originalMethod)
        {
            var radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            var capMethod = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));
            var patched = 0;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.LoadsField(radiusField))
                {
                    yield return new CodeInstruction(Call, capMethod);
                    patched++;
                }
            }

            // Warn, not Error: this transpiler blankets several seek methods and some
            // (currently the FollowLeader variants) legitimately contain no uRadius reads.
            if (patched == 0)
                GS2.Warn($"EnemyUnitComponent space-seek transpiler: no uRadius loads in {__originalMethod?.Name} (normal for some blanket-listed targets; if this appears for a method that previously patched, a game update changed it).");
        }
    }
}