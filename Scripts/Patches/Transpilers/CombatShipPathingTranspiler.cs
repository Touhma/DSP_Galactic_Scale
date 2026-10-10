using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public static class CombatShipPathingTranspiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UnitComponent), nameof(UnitComponent.GetAstroAvoidanceTargetPos))]
        public static IEnumerable<CodeInstruction> CapStarRadiusInAvoidance(IEnumerable<CodeInstruction> instructions)
        {
            return CapAstroRadii(instructions);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FleetComponent), nameof(FleetComponent.GetUnitOrbitingAstroPose))]
        public static IEnumerable<CodeInstruction> CapStarRadiusInOrbit(IEnumerable<CodeInstruction> instructions)
        {
            return CapAstroRadii(instructions);
        }

        private static IEnumerable<CodeInstruction> CapAstroRadii(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            MethodInfo capMethod = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));

            foreach (CodeInstruction instruction in instructions)
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
