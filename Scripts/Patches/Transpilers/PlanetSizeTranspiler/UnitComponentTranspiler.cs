using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public class UnitComponentTranspiler
    {

        public static T GetRadiusFromMecha<T>(T t, Mecha mecha)
        {
            var num = mecha?.player?.planetData.realRadius ?? 200f;
            float orig = Convert.ToSingle(t);
            var diff = orig - 200f;
            num += diff;
            // if (VFInput.alt) GS2.Log($"GetRadiusFromMecha Called By {GS2.GetCaller(0)} {GS2.GetCaller(1)} {GS2.GetCaller(2)} orig:{orig} returning {num}");
            return (T)Convert.ChangeType(num, typeof(T));
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackLaser_Ground))] //225f 212f
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackPlasma_Ground))]//225f 212f
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_DefenseShield_Ground))]
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
                                   Convert.ToDouble(i.operand ?? 0.0) == 212.0 ||
                                   Convert.ToDouble(i.operand ?? 0.0) == 225.0 

                               );
                    })
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    // var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    var mi = matcher.GetRadiusFromFactory();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        
        // Mecha
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackLaser_Large))]//
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackPlasma_Small))]//
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(UnitComponentTranspiler), nameof(UnitComponentTranspiler.GetRadiusFromMecha));
            
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_2));
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
        
        
        
        
        
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