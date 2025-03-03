using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class PatchOnSectorModel
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(SectorModel), nameof(SectorModel.CreateGalaxyAstroBuffer))]
        public static IEnumerable<CodeInstruction> TranspilerSectorModelCreateGalaxyAstroBuffer(
            IEnumerable<CodeInstruction> instructions)
        {
            var calcMethod = AccessTools.Method(typeof(PatchOnSectorModel), nameof(CalcAstroBufferSize));
            instructions = new CodeMatcher(instructions)
                // Replace 25600 with the result of the CalcAstroBufferSize method
                .MatchForward(
                    true,
                    new CodeMatch(i => { return i.opcode == Ldc_I4 && Convert.ToInt32(i.operand ?? 0) == 25600; }
                    ))
                .Repeat(matcher => { matcher.SetInstructionAndAdvance(new CodeInstruction(Call, calcMethod)); }).InstructionEnumeration();

            return instructions;
        }

        public static int CalcAstroBufferSize()
        {
            return GameMain.spaceSector.galaxy.starCount * 400;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(SectorModel), nameof(SectorModel.OnCameraPostRender))]
        public static bool PatchOnCameraPostRender(ref SectorModel __instance, Camera cam)
        {
            if (GameMain.mainPlayer == null)
            {
                return false;
            }

            return true;
        }
    }
}