using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public class PatchOnPlanetAuxData
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.ReformSnap))]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.ReformSnapCircle))]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.ReformSnapExceptDisableCircle))]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.Snap), new System.Type[] { typeof(Vector3), typeof(bool) })]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.Snap), new System.Type[] { typeof(Vector3), typeof(bool), typeof(int) })]
        [HarmonyPatch(typeof(PlanetAuxData), nameof(PlanetAuxData.SnapLineNonAlloc))]
        public static IEnumerable<CodeInstruction> PlanetAuxDataTranspiler(
            IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase __originalMethod)
        {
            var codeMatcher = new CodeMatcher(instructions, il).MatchForward(false,
                new CodeMatch(op => op.opcode == OpCodes.Ldfld && (FieldInfo)op.operand ==
                    AccessTools.Field(typeof(PlanetData),
                        nameof(PlanetData.radius)))); // Search for ldfld PlanetData::radius

            if (codeMatcher.IsInvalid)
            {
                GS2.Error("PlanetAuxDataTranspiler Transpiler Failed");
                return instructions;
            }

            var radiusAdjusted = codeMatcher.Repeat(
                    z =>
                    {
                        // Bootstrap.Logger.LogInfo($"Found value {z.Operand} at " + z.Pos );
                        z // Repeat for all occurences 
                            .SetInstructionAndAdvance(new CodeInstruction(Callvirt,
                                AccessTools.Property(typeof(PlanetData), nameof(PlanetData.realRadius)).GetMethod));
                    }) // Replace operand with PlanetData::realRadius
                .InstructionEnumeration();

            var code = new List<CodeInstruction>(radiusAdjusted);
            if (__originalMethod?.Name == nameof(PlanetAuxData.SnapLineNonAlloc))
            {
                var minimumStepIndex = code.FindIndex(instruction =>
                    instruction.opcode == Ldc_R4 && instruction.operand is float value && Math.Abs(value - 0.004f) < 0.000001f);
                if (minimumStepIndex >= 0)
                {
                    code.InsertRange(minimumStepIndex + 1, new[]
                    {
                        new CodeInstruction(Ldarg_0),
                        new CodeInstruction(Ldfld, AccessTools.Field(typeof(PlanetAuxData), nameof(PlanetAuxData.planet))),
                        new CodeInstruction(Call, AccessTools.Method(typeof(PatchOnPlanetAuxData), nameof(ScaleSnapGridMinimum)))
                    });
                }
                else
                {
                    GS2.Warn("PlanetAuxData.SnapLineNonAlloc: 0.004 angular minimum not found; using vanilla path step.");
                }
            }

            return code;
        }

        public static float ScaleSnapGridMinimum(float vanillaMinimum, PlanetData planet)
        {
            var radius = planet?.realRadius ?? 200f;
            return radius > 0f ? vanillaMinimum * (200f / radius) : vanillaMinimum;
        }
    }
}
