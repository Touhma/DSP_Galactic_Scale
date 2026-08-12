using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class PlanetSizeTranspiler
    {
        // Owning planet for IsTargetVeinInRange. The method is static (Vector3, Pose, PrefabDesc)
        // so there is no factory/planet argument; CreateEntityLogicComponents is the only 0.10.34
        // caller that is not local-planet-scoped (BAB rebuild via BuildFinally on a remote factory).
        // HarmonyX 2.5.5 (BepInEx 5.4.17) delivers Prefix `out T __state` into a void
        // Finalizer(T __state) on both normal return and thrown exception, including nested
        // same-thread pushes.
        [ThreadStatic] static PlanetData veinRangePlanet;

        public static float GetVeinRangeRadiusConstant(float vanilla)
        {
            var planet = veinRangePlanet ?? GameMain.localPlanet;
            if (planet == null)
                return vanilla;
            return (float)Utils.ModifyRadius(vanilla, planet.realRadius);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.CreateEntityLogicComponents))]
        public static void CreateEntityLogicComponents_PushPlanet(PlanetFactory __instance, out PlanetData __state)
        {
            __state = veinRangePlanet;
            veinRangePlanet = __instance.planet;
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.CreateEntityLogicComponents))]
        public static void CreateEntityLogicComponents_PopPlanet(PlanetData __state)
        {
            veinRangePlanet = __state;
        }

        // MinerComponent.IsTargetVeinInRange projects target and pose.position onto a radius-200
        // sphere, then applies fixed meter-space thresholds. It is patched here instead of via
        // PlanetSizeTranspiler.Fix200f because the game runs it for planets other than the local
        // one (CreateEntityLogicComponents re-validates prebuild vein IDs and drops any that fail,
        // including during off-world BAB rebuild), so the radius must come from the owning
        // factory's planet when that context has been pushed, not GameMain.localPlanet.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MinerComponent), nameof(MinerComponent.IsTargetVeinInRange))]
        public static IEnumerable<CodeInstruction> FixVeinRangeRadius(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R4 && i.operand is float f && f == 200f)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("MinerComponent.IsTargetVeinInRange transpiler: 200f constant not found (game update changed the method?). Returning original code - vein range will use vanilla radius-200 behavior.");
                return instructions;
            }

            // Each match leaves the cursor ON the ldc.r4 200. The constant stays in place and
            // the helper call is appended after it, so the pair consumes (vanilla) and pushes
            // the context-or-localPlanet scaled value - net stack effect identical to the bare
            // constant. The method's other floats (7.75 / 6.25 / 100 / 60.0625 / 0.73 / 2.0 /
            // -10 / -1.2) are meter-space thresholds and must not be wrapped. Nothing is
            // removed or replaced, so branch labels on surrounding code stay valid.
            return matcher
                .Repeat(m =>
                {
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Call, AccessTools.Method(typeof(PlanetSizeTranspiler), nameof(GetVeinRangeRadiusConstant))));
                }).InstructionEnumeration();
        }
    }
}
