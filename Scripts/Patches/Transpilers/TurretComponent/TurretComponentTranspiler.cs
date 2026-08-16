using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public static class TurretComponentTranspiler
    {
        // Both patched methods receive the owning PlanetFactory as their FIRST parameter
        // (arg1), so the radius comes straight from factory.planet - no lookup table.
        //
        // This replaces a Dictionary<TurretComponent, float> keyed on the MUTABLE STRUCT:
        // the stored key was a snapshot of the turret's state at build time, so every
        // later lookup missed (the patch always fell back to 200f and never worked) and
        // RemoveTurret could never find its entry either, leaking one stale snapshot per
        // turret ever built.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TurretComponent), nameof(TurretComponent.CheckEnemyIsInAttackRange))]
        [HarmonyPatch(typeof(TurretComponent), nameof(TurretComponent.Shoot_Plasma))]
        public static IEnumerable<CodeInstruction> FixTurretRadius(IEnumerable<CodeInstruction> instructions, System.Reflection.MethodBase __originalMethod)
        {
            var helper = AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromFactory));
            // Census (0.10.34): CheckEnemyIsInAttackRange has one ldc.r4 200; Shoot_Plasma
            // has two ldc.r8 200. Compare in the operand's own type.
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                        (i.opcode == Ldc_R4 && i.operand is float f && f == 200f) ||
                        (i.opcode == Ldc_R8 && i.operand is double d && d == 200.0))
                );
            if (matcher.IsInvalid)
            {
                GS2.Error($"TurretComponentTranspiler: no 200 constant found in {__originalMethod?.Name} (game update changed the method?). Returning original code - that method will use vanilla radius-200 behavior.");
                return instructions;
            }

            // Each match leaves the cursor ON the constant. It stays in place; ldarg.1
            // (the PlanetFactory parameter) and the factory-radius helper are appended, so
            // the pair consumes (vanilla, factory) and pushes ModifyRadius(200, realRadius)
            // in the constant's own type. Net stack effect identical to the bare constant.
            return matcher
                .Repeat(m =>
                {
                    var mi = helper.MakeGenericMethod(m.Operand?.GetType() ?? typeof(float));
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_1));
                    m.InsertAndAdvance(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();
        }
    }
}
