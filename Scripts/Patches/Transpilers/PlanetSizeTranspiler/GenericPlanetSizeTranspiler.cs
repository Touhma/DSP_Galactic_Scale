using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches

{
    public class PlanetSizeTranspiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetNormalizedDir))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetNormalizedPos))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetExtendedGratBox),typeof(BPGratBox), typeof(float))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetExtendedGratBox),typeof(BPGratBox), typeof(float), typeof(float))]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), nameof(BuildTool_BlueprintPaste.CheckBuildConditions))]
        [HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.GetGridWidth))]
        [HarmonyPatch(typeof(PlayerNavigation),  nameof(PlayerNavigation.Init))]
        [HarmonyPatch(typeof(PlayerNavigation),  nameof(PlayerNavigation.DetermineArrive))]
        [HarmonyPatch(typeof(PlanetEnvironment),  nameof(PlanetEnvironment.LateUpdate))]
        [HarmonyPatch(typeof(PlayerAction_Combat),  nameof(PlayerAction_Combat.Shoot_Gauss_Space))]
        [HarmonyPatch(typeof(PlayerAction_Combat),  nameof(PlayerAction_Combat.Shoot_Plasma))]
        [HarmonyPatch(typeof(PlayerAction_Plant),  nameof(PlayerAction_Plant.UpdateRaycast))]
        [HarmonyPatch(typeof(PlayerAction_Navigate),  nameof(PlayerAction_Navigate.GameTick))]
        [HarmonyPatch(typeof(PowerSystem),  nameof(PowerSystem.CalculateGeothermalStrenth))]
        [HarmonyPatch(typeof(BuildTool_Reform),  nameof(BuildTool_Reform.UpdateRaycast))]
        [HarmonyPatch(typeof(BuildTool_Upgrade),  nameof(BuildTool_Upgrade.UpdateRaycast))]
        [HarmonyPatch(typeof(BuildTool_Path),  nameof(BuildTool_Path.UpdateRaycast))]
        [HarmonyPatch(typeof(BuildTool_Path),  nameof(BuildTool_Path.GetGridWidth))]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.GetReshapeData))]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.Reshape))]
        [HarmonyPatch(typeof(SpaceCapsule), nameof(SpaceCapsule.LateUpdate))]

        public static IEnumerable<CodeInstruction> Fix200f(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(Utils), nameof(Utils.FixRadius));
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                               (
                                   Convert.ToDouble(i.operand ?? 0.0) == 196.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 197.5 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 197.6 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 198.5 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 200.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 200.5 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 202.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 206.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 212.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 225.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 225.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 228.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 255.0 
                            );
                    })
                )
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at {matcher.Pos} type {matcher.Operand?.GetType()}");
                    var mi = methodInfo.MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }
    }
}