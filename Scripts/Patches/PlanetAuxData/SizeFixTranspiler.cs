using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
            IEnumerable<CodeInstruction> instructions, ILGenerator il)
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

            instructions = codeMatcher.Repeat(
                    z =>
                    {
                        // Bootstrap.Logger.LogInfo($"Found value {z.Operand} at " + z.Pos );
                        z // Repeat for all occurences 
                            .SetInstructionAndAdvance(new CodeInstruction(Callvirt,
                                AccessTools.Property(typeof(PlanetData), nameof(PlanetData.realRadius)).GetMethod));
                    }) // Replace operand with PlanetData::realRadius
                .InstructionEnumeration();
            return instructions;
        }
    }
}