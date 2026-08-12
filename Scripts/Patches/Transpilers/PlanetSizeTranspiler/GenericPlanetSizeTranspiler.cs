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
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetNormalizedDir))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetNormalizedPos))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetExtendedGratBox),typeof(BPGratBox), typeof(float))]
        [HarmonyPatch(typeof(BlueprintUtils), nameof(BlueprintUtils.GetExtendedGratBox),typeof(BPGratBox), typeof(float), typeof(float))]
        [HarmonyPatch(typeof(BuildTool_BlueprintPaste), nameof(BuildTool_BlueprintPaste.GenerateBlueprintGratBoxes))]
        [HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.GetGridWidth))]
        [HarmonyPatch(typeof(PlayerNavigation),  nameof(PlayerNavigation.Init))]
        [HarmonyPatch(typeof(PlayerNavigation),  nameof(PlayerNavigation.DetermineArrive))]
        [HarmonyPatch(typeof(PlanetEnvironment),  nameof(PlanetEnvironment.LateUpdate))]
        [HarmonyPatch(typeof(PlayerAction_Combat),  nameof(PlayerAction_Combat.Shoot_Gauss_Space))]
        [HarmonyPatch(typeof(PlayerAction_Combat),  nameof(PlayerAction_Combat.Shoot_Plasma))]
        [HarmonyPatch(typeof(PlayerAction_Plant),  nameof(PlayerAction_Plant.UpdateRaycast))]
        [HarmonyPatch(typeof(PlayerAction_Navigate),  nameof(PlayerAction_Navigate.GameTick))]
        // PowerSystem.CalculateGeothermalStrength is deliberately NOT in this list: it runs
        // for remote planets too, so localPlanet is the wrong radius source there. It has a
        // dedicated transpiler in CalculateGeothermalStrength.cs.
        [HarmonyPatch(typeof(MinerComponent),  nameof(MinerComponent.IsTargetVeinInRange))]
        [HarmonyPatch(typeof(BuildTool_Reform),  nameof(BuildTool_Reform.UpdateRaycastAndReform))]
        [HarmonyPatch(typeof(BuildTool_Upgrade),  nameof(BuildTool_Upgrade.UpdateRaycast))]
        [HarmonyPatch(typeof(BuildTool_Path),  nameof(BuildTool_Path.UpdateRaycast))]
        [HarmonyPatch(typeof(BuildTool_Path),  nameof(BuildTool_Path.GetGridWidth))]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.GetReshapeData))]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.Reshape))]
        [HarmonyPatch(typeof(SpaceCapsule), nameof(SpaceCapsule.LateUpdate))]
        

        public static IEnumerable<CodeInstruction> Fix200f(IEnumerable<CodeInstruction> instructions, System.Reflection.MethodBase __originalMethod)
        {
            var methodInfo = AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromLocalPlanet));
            var matcher = new CodeMatcher(instructions)
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
                                    Convert.ToDouble(i.operand ?? 0.0) == 200.22 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 200.5 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 202.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 206.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 212.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 225.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 228.0 ||
                                    Convert.ToDouble(i.operand ?? 0.0) == 255.0
                            );
                    })
                );
            // The transpiler runs once per target method; every target was chosen because it
            // contains at least one of the radius constants above. Zero matches therefore means
            // a game update removed/changed them in THIS method - report which one and leave it
            // vanilla instead of letting Repeat() throw (which would abort the rest of
            // Bootstrap's patch registration).
            if (matcher.IsInvalid)
            {
                GS2.Error($"PlanetSizeTranspiler: no known radius constant found in {__originalMethod?.DeclaringType?.Name}.{__originalMethod?.Name} (game update changed it?). That method will use vanilla radius-200 behavior.");
                return instructions;
            }

            return matcher
                .Repeat(m =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {m.Operand} at {m.Pos} type {m.Operand?.GetType()}");
                    var mi = methodInfo.MakeGenericMethod(m.Operand?.GetType() ?? typeof(float));
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
    }
}