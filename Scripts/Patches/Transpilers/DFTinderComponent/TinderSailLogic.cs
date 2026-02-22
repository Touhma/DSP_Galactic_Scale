/*
 * Change Log:
 * - 2026-02-22: Cap Dark Fog tinder pathing star radius to vanilla max.
 */
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnDFTinderComponent
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFTinderComponent), nameof(DFTinderComponent.TinderSailLogic))]
        public static IEnumerable<CodeInstruction> CapStarRadiusToVanilla(IEnumerable<CodeInstruction> instructions)
        {
            var radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            var capMethod = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.LoadsField(radiusField))
                {
                    yield return new CodeInstruction(OpCodes.Call, capMethod);
                }
            }
        }
    }
}
