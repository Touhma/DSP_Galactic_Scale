using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

{
    public static class TurretComponentTranspiler
    {
        public static Dictionary<TurretComponent, float> Radii = new();

        public static float GetRadius(ref TurretComponent turret)
        {
            return Radii[turret];
        }

        public static void AddTurret(DefenseSystem defenseSystem, ref TurretComponent turret)
        {
            GS3.Log($"Added Turret {turret.id} from DefenseSystem {defenseSystem.planet.name}");
            Radii[turret] = defenseSystem.planet.realRadius + 1;
        }

        public static void RemoveTurret(ref TurretComponent turret)
        {
            GS3.Log($"Removed Turret {turret.id}");
            Radii.Remove(turret);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TurretComponent), nameof(TurretComponent.BeforeRemove))]
        public static IEnumerable<CodeInstruction> BeforeRemoveTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions).Advance(1)
                .InsertAndAdvance(new CodeInstruction(Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(Call,
                    AccessTools.Method(typeof(TurretComponentTranspiler), nameof(RemoveTurret))))
                .InstructionEnumeration();
            foreach (var i in instructions) Bootstrap.Logger.LogMessage($"{i.opcode}   {i.operand}");
            return instructions;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TurretComponent), nameof(TurretComponent.CheckEnemyIsInAttackRange))]
        [HarmonyPatch(typeof(TurretComponent), nameof(TurretComponent.Shoot_Plasma))]
        public static IEnumerable<CodeInstruction> Shoot_PlasmaTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(
                        true,
                        new CodeMatch(i => (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                                           Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 200.0) < 0.01f)
                    );
                if (!matcher.IsInvalid)
                {
                    matcher.Repeat(matcher =>
                    {
                        matcher.SetInstructionAndAdvance(new CodeInstruction(Ldarg_0));
                        matcher.InsertAndAdvance(new CodeInstruction(Call, AccessTools.Method(typeof(TurretComponentTranspiler), nameof(GetRadius))));
                    });
                    instructions = matcher.InstructionEnumeration();
                    return instructions;
                }
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("TurretComponentTranspiler.Shoot_PlasmaTranspiler failed");
                return instructions;
            }
        }


        // [HarmonyPatch(typeof(TurretComponent),  nameof(TurretComponent.SetStateToAim_Default))]
        // 
    }
}