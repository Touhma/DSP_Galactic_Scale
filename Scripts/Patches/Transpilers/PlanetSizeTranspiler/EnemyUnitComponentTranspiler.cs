using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

{
    public class EnemyUnitComponentTranspiler
    {

        public static T GetRadiusFromFactory<T>(T t, PlanetFactory factory)
        {
            var num = factory?.planet?.realRadius ?? 200f;
            float orig = Convert.ToSingle(t);
            var diff = orig - 200f;
            num += diff;
            if (VFInput.alt) GS3.Log($"GetRadiusFromFactory Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{orig} returning {num}");
            return (T)Convert.ChangeType(num, typeof(T));
        }
        public static T GetRadiusFromEnemyData<T>(T t, ref EnemyData enemyData)
        {
            float orig = Convert.ToSingle(t);
            var num = 200f;
            GS3.Log(enemyData.astroId.ToString());
            var planet = GameMain.galaxy.PlanetById(enemyData.astroId);
            if (planet != null)
            {
                var diff = orig - 200f;
                num += diff;
            }
            if (VFInput.alt) GS3.Log($"GetRadiusFromFactory Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{orig} returning {num}");
            
            
            
            return (T)Convert.ChangeType(num, typeof(T));
        }
        public static double GetRadiusFromAltitude(float alt, double radius)
        {
            var diff = alt - 200f;
            radius += diff;
            return radius + 25.0;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Defense_Ground))] //225
        public static IEnumerable<CodeInstruction> Fix225(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(GetRadiusFromAltitude));
            // Bootstrap.DumpInstructions(instructions, nameof(EnemyUnitComponent.RunBehavior_Defense_Ground),290, 20);
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 225.0) < 0.01f)
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    matcher.InsertAndAdvance(Utils.LoadArgument(5));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Call, methodInfo));
                    // Bootstrap.DumpMatcher(matcher, 3, 5, 5);
                }).InstructionEnumeration();

            return instructions;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRaider))] //200 206 202
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_GRanger))] //200 212 225
        public static IEnumerable<CodeInstruction> Fix200_225(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(EnemyUnitComponentTranspiler.GetRadiusFromFactory));
            
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
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_Engage_SHumpback))] //200 but need to find the planet...
        // [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(EnemyUnitComponentTranspiler.GetRadiusFromEnemyData));
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                )
                .Repeat(matcher =>
                {
                    Bootstrap.DumpMatcherPre(matcher, 1, 5, 5);
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_S, (sbyte)3));
                    matcher.Insert(new CodeInstruction(Call, mi));
                    Bootstrap.DumpMatcherPost(matcher, 3, 5, 5);
                }).InstructionEnumeration();

            return instructions;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUnitComponent),  nameof(EnemyUnitComponent.RunBehavior_OrbitTarget_SLancer))] //200 but need to find the planet...
        public static IEnumerable<CodeInstruction> Fix200Slancer(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(EnemyUnitComponentTranspiler), nameof(EnemyUnitComponentTranspiler.GetRadiusFromEnemyData));
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R8 && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                )
                .Repeat(matcher =>
                {
                    Bootstrap.DumpMatcherPre(matcher, 1, 5, 5);
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(Utils.LoadArgument(4));
                    matcher.Insert(new CodeInstruction(Call, mi));
                    Bootstrap.DumpMatcherPost(matcher, 3, 5, 5);
                }).InstructionEnumeration();

            return instructions;
        }
        
        
        
        // [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackLaser_Large))]//
        // [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackLaser_Ground))]
        // [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackPlasma_Ground))]
        // [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_DefenseShield_Ground))]
        // [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackPlasma_Small))]//
        // [HarmonyPatch(typeof(GrowthTool_Node_DFGround),  nameof(GrowthTool_Node_DFGround.CreateNode7))]
        // [HarmonyPatch(typeof(DFRelayComponent),  nameof(DFRelayComponent.RelaySailLogic))]
        // [HarmonyPatch(typeof(DFSTurretComponent),  nameof(DFSTurretComponent.Shoot_Plasma))]
        // [HarmonyPatch(typeof(DFGTurretComponent),  nameof(DFSTurretComponent.Aim))]
        // [HarmonyPatch(typeof(DFGTurretComponent),  nameof(DFSTurretComponent.Shoot_Plasma))]
        // [HarmonyPatch(typeof(DFTinderComponent),  nameof(DFTinderComponent.TinderSailLogic))]
        // [HarmonyPatch(typeof(FleetComponent),  nameof(FleetComponent.GetUnitOrbitingAstroPose))]
        //
        // [HarmonyPatch(typeof(LocalLaserOneShot),  nameof(LocalLaserOneShot.TickSkillLogic))]
        // [HarmonyPatch(typeof(LocalLaserContinuous),  nameof(LocalLaserContinuous.TickSkillLogic))]
        // [HarmonyPatch(typeof(SkillSystem),  nameof(SkillSystem.AddSpaceEnemyHatred), new[]
        // {
        //     typeof(EnemyDFHiveSystem), 
        //     typeof(EnemyData), 
        //     typeof(ETargetType), 
        //     typeof(int), 
        //     typeof(int)
        // }, new[]
        // {
        //     ArgumentType.Normal, 
        //     ArgumentType.Ref, 
        //     ArgumentType.Normal, 
        //     ArgumentType.Normal, 
        //     ArgumentType.Normal
        // })]
    }
}