using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    
    public partial class PlanetSizeTranspiler
    {
        // Radius-derived constants the game hardcodes for the vanilla 200m planet.
        private static readonly double[] RadiusConstants =
            { 196, 197.5, 197.6, 198.5, 200, 200.22, 200.5, 201, 202, 206, 212, 225, 228, 255 };

        private static bool IsRadiusConstant(CodeInstruction i)
        {
            // Compare in the operand's own type, and with EXACT equality on purpose: these
            // are compiler-emitted literal bit patterns, not measured geometry, so an
            // epsilon would only risk matching unrelated nearby constants. Fractional
            // constants like 200.22f widen to 200.2200012... as double, so the old
            // Convert.ToDouble comparison against the double literal could never match an
            // ldc.r4 operand - that silently left
            // BuildTool_BlueprintPaste.GenerateBlueprintGratBoxes (200.22f) unpatched.
            if (i.opcode == Ldc_R4 && i.operand is float f)
            {
                foreach (var c in RadiusConstants)
                    if (f == (float)c)
                        return true;
                return false;
            }
            if (i.opcode == Ldc_R8 && i.operand is double d)
            {
                foreach (var c in RadiusConstants)
                    if (d == c)
                        return true;
                return false;
            }
            if (i.opcode == Ldc_I4 && i.operand is int n)
            {
                foreach (var c in RadiusConstants)
                    if (n == c)
                        return true;
                return false;
            }
            return false;
        }

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
        // MinerComponent.IsTargetVeinInRange is deliberately NOT in this list: it is static
        // and also runs for remote planets (CreateEntityLogicComponents re-checks prebuild
        // vein IDs during BAB rebuild while the player is elsewhere), so localPlanet is the
        // wrong radius source there. It has a dedicated transpiler in IsTargetVeinInRange.cs.
        // PowerSystem.CalculateGeothermalStrength is deliberately NOT in this list: it runs
        // for remote planets too, so localPlanet is the wrong radius source there. It has a
        // dedicated transpiler in CalculateGeothermalStrength.cs.
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
                    new CodeMatch(IsRadiusConstant)
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