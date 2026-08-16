using System;
using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class PatchOnDFRelayComponent
    {
        // DFRelayComponent.SearchTargetPlaceProcess has two landing-site paths.
        // The random-site path already scales hover altitude as realRadius + 70.
        // The beacon path (dstMarkerId > 0) instead rescales dstMarkerLPos onto a
        // hardcoded 270f sphere (vanilla 200 + 70 m hover). On a resized planet that
        // parks the sail target at 270 m from the core — 70 m AGL on vanilla, well
        // above the surface on a small planet, and underground on a large one — so
        // the relay descends toward the beacon but never ArriveBase's (#270).
        // Patched here instead of via PlanetSizeTranspiler because 270 is not in
        // that list's radius-family set, and only this one site is wrong.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.SearchTargetPlaceProcess))]
        public static IEnumerable<CodeInstruction> FixBeaconHoverRadius(IEnumerable<CodeInstruction> instructions)
        {
            var methodInfo = AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromAstroId))
                .MakeGenericMethod(typeof(float));
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R4 && i.operand is float f && f == 270f)
                );
            if (matcher.IsInvalid)
            {
                GS2.Error("DFRelayComponent.SearchTargetPlaceProcess transpiler: 270f constant not found (game update changed the method?). Returning original code - beacon landings will target a vanilla-radius-200 hover shell.");
                return instructions;
            }

            // Each match leaves the cursor ON the ldc.r4 270. The constant stays in
            // place and ldarg.0 + ldfld dstMarkerAstroId + call are appended after it,
            // so GetRadiusFromAstroId<float>(270, dstMarkerAstroId) consumes
            // (vanilla, id) and pushes ModifyRadius(270, planet.realRadius) =
            // realRadius + 70. The following get_magnitude / div then scale the
            // beacon position onto that hover sphere. Net stack effect identical to
            // the bare constant. The method's other floats (70f on the already-correct
            // random path, 256f) are left untouched.
            return matcher
                .Repeat(m =>
                {
                    m.Advance(1);
                    m.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    // string lookup, not nameof: the checked-in Assembly-CSharp_public.dll
                    // reference assembly predates the dstMarker* fields; the live game has
                    // them (verified against 0.10.34.28529 IL).
                    m.InsertAndAdvance(new CodeInstruction(Ldfld, AccessTools.Field(typeof(DFRelayComponent), "dstMarkerAstroId")));
                    m.InsertAndAdvance(new CodeInstruction(Call, methodInfo));
                }).InstructionEnumeration();
        }
    }
}
