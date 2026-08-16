using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public partial class PatchOnPowerSystem
    {
        public static float GetGeothermalRadiusConstant(float vanilla, PowerSystem powerSystem)
        {
            var planet = powerSystem?.planet;
            if (planet == null)
            {
                GS2.Warn("GetGeothermalRadiusConstant: PowerSystem has no planet - using vanilla radius-200 value.");
                return vanilla;
            }
            return (float)Utils.ModifyRadius(vanilla, planet.realRadius);
        }

        // PowerSystem.CalculateGeothermalStrength compares sampled terrain height against a
        // radius-derived constant (196f = vanilla radius 200 - 4). It is patched here instead
        // of via PlanetSizeTranspiler because the game runs it for planets other than the
        // local one (it has a PlanetData.GetUnloadedCopy path, and rebuilds triggered while
        // the player is elsewhere call it too), so the radius must come from the owning
        // PowerSystem's planet, not GameMain.localPlanet.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PowerSystem), nameof(PowerSystem.CalculateGeothermalStrength))]
        public static IEnumerable<CodeInstruction> FixGeothermalRadius(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R4 && Convert.ToDouble(i.operand ?? 0.0) == 196.0)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("PowerSystem.CalculateGeothermalStrength transpiler: 196f constant not found (game update changed the method?). Returning original code - geothermal strength will use vanilla radius-200 behavior.");
                return instructions;
            }

            // Each match leaves the cursor ON the ldc.r4 196. The constant stays in place and
            // ldarg.0 + call are appended after it, so the pair consumes (vanilla, this) and
            // pushes the planet-scaled value - net stack effect identical to the bare constant.
            // Nothing is removed or replaced, so branch labels on surrounding code stay valid.
            return matcher
                .Repeat(m =>
                {
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    m.InsertAndAdvance(new CodeInstruction(Call, AccessTools.Method(typeof(PatchOnPowerSystem), nameof(GetGeothermalRadiusConstant))));
                }).InstructionEnumeration();
        }
    }
}
