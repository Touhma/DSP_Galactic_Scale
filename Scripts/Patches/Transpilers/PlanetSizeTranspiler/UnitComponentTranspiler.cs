using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public class UnitComponentTranspiler
    {


        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackLaser_Ground))] //225f 212f
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_AttackPlasma_Ground))]//225f 212f
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_DefenseShield_Ground))]
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
                                   Convert.ToDouble(i.operand ?? 0.0) == 212.0 ||
                                   Convert.ToDouble(i.operand ?? 0.0) == 225.0

                               );
                    })
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("UnitComponent ground-engage transpiler: 212/225 radius constants not found (game update changed the method?). Returning original code - ground unit engagement ranges will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromFactory();
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    m.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
        
        // Mecha
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackLaser_Large))]//
        [HarmonyPatch(typeof(UnitComponent),  nameof(UnitComponent.RunBehavior_Engage_SAttackPlasma_Small))]//
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            // var methodInfo = AccessTools.Method(typeof(UnitComponentTranspiler), nameof(UnitComponentTranspiler.GetRadiusFromMecha));
            
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) && Convert.ToDouble(i.operand ?? 0.0) == 200.0)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("UnitComponent mecha-engage transpiler: 200 radius constant not found (game update changed the method?). Returning original code - mecha engagement ranges will assume a radius-200 planet.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    var mi = m.GetRadiusFromMecha();
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_2));
                    m.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
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