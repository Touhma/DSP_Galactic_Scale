using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public class PlanetSizeTranspiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyData), "Formation",
            new Type[]
            {
                typeof(int), typeof(EnemyData), typeof(float), typeof(VectorLF3), typeof(Quaternion), typeof(Vector3)
            },
            new ArgumentType[]
            {
                ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref,
                ArgumentType.Ref
            })]
        [HarmonyPatch(typeof(TurretComponent), "CheckEnemyIsInAttackRange")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "ApproachToTargetPoint_SLancer")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "Attack_SLancer")]
        [HarmonyPatch(typeof(GrowthTool_Node_DFGround), "CreateNode7")]
        [HarmonyPatch(typeof(PlayerNavigation), "DetermineArrive")]
        [HarmonyPatch(typeof(DFRelayComponent), "RelaySailLogic")]
        [HarmonyPatch(typeof(PlayerAction_Navigate), "GameTick")]
        [HarmonyPatch(typeof(FleetComponent), "GetUnitOrbitingAstroPose")]
        [HarmonyPatch(typeof(PlayerNavigation), "Init")]
        [HarmonyPatch(typeof(PlanetEnvironment), "LateUpdate")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRanger")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_SHumpback")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_OrbitTarget_SLancer")]
        [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackLaser_Large")]
        [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_AttackLaser_Ground")]
        [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_AttackPlasma_Ground")]
        [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_DefenseShield_Ground")]
        [HarmonyPatch(typeof(UnitComponent), "RunBehavior_Engage_SAttackPlasma_Small")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Defense_Ground")]
        [HarmonyPatch(typeof(TurretComponent), "SetStateToAim_Default")]
        [HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Gauss_Space")]
        [HarmonyPatch(typeof(TurretComponent), "Shoot_Plasma")]
        [HarmonyPatch(typeof(PlayerAction_Combat), "Shoot_Plasma")]
        [HarmonyPatch(typeof(DFSTurretComponent), "Shoot_Plasma")]
        [HarmonyPatch(typeof(DFGTurretComponent), "Aim")]
        [HarmonyPatch(typeof(DFGTurretComponent), "Shoot_Plasma")]
        [HarmonyPatch(typeof(DFTinderComponent), "TinderSailLogic")]
        [HarmonyPatch(typeof(PlayerAction_Plant), "UpdateRaycast")]
        [HarmonyPatch(typeof(EnemyUnitComponent), "RunBehavior_Engage_GRaider")]
        [HarmonyPatch(typeof(LocalLaserOneShot), "TickSkillLogic")]
        [HarmonyPatch(typeof(LocalLaserContinuous), "TickSkillLogic")]
        [HarmonyPatch(typeof(PowerSystem), "CalculateGeothermalStrenth")]
        [HarmonyPatch(typeof(BuildTool_Reform), "UpdateRaycast")]
        [HarmonyPatch(typeof(BuildTool_Upgrade), "UpdateRaycast")]
        [HarmonyPatch(typeof(BuildTool_Path), "UpdateRaycast")]
        [HarmonyPatch(typeof(BuildTool_Path), "GetGridWidth")]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.GetReshapeData))]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.Reshape))]
        [HarmonyPatch(typeof(SpaceCapsule), nameof(SpaceCapsule.LateUpdate))]
        [HarmonyPatch(typeof(SkillSystem), "AddSpaceEnemyHatred", new[]
        {
            typeof(EnemyDFHiveSystem), typeof(EnemyData), typeof(ETargetType), typeof(int), typeof(int)
        }, new[]
        {
            ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal
        })]
        public static IEnumerable<CodeInstruction> Fix200f(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(Utils), nameof(Utils.FixRadius));
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                               (Convert.ToSingle(i.operand ?? 0f) == 196f ||
                                Convert.ToSingle(i.operand ?? 0f) == 200f ||
                                Convert.ToSingle(i.operand ?? 0f) == 200.0 ||
                                Convert.ToSingle(i.operand ?? 0f) == 212f ||
                                Convert.ToSingle(i.operand ?? 0f) == 225f ||
                                Convert.ToSingle(i.operand ?? 0f) == 255f ||
                                Convert.ToSingle(i.operand ?? 0f) == 228f ||
                                Convert.ToSingle(i.operand ?? 0f) == 225.0 ||
                                Convert.ToSingle(i.operand ?? 0f) == 200.5f ||
                                Convert.ToSingle(i.operand ?? 0f) == 202f ||
                                Convert.ToSingle(i.operand ?? 0f) == 206f ||
                                Convert.ToSingle(i.operand ?? 0.0) == 197.6f ||
                                Convert.ToSingle(i.operand ?? 0.0) == 197.5f ||
                                Convert.ToSingle(i.operand ?? 0.0) == 198.5f);
                    })
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at " + matcher.Pos + " type " +
                    //                          matcher.Operand?.GetType());
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    // if (matcher.Instruction.opcode != Call)
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
    }
}