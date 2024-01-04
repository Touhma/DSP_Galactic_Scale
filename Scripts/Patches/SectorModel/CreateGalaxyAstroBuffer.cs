using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public partial class PatchOnSectorModel
    {
        // [HarmonyPrefix, HarmonyPatch(typeof(SectorModel), "CreateGalaxyAstroBuffer")]
        // public static bool CreateGalaxyAstroBuffer(ref SectorModel __instance)
        // {
        //     var size = 400 * GameMain.galaxy.starCount ;
        //     GS2.Warn("Creating Galaxy AstroBuffer. Size:" + size);
        //     __instance.galaxyAstroArr = new AstroPoseR[size];
        //     __instance.galaxyAstroBuffer = new ComputeBuffer(size, 32, ComputeBufferType.Default);
        //     for (int i = 0; i < __instance.galaxyAstroArr.Length; i++)
        //     {
        //         __instance.galaxyAstroArr[i].rpos.x = 0f;
        //         __instance.galaxyAstroArr[i].rpos.y = 0f;
        //         __instance.galaxyAstroArr[i].rpos.z = 0f;
        //         __instance.galaxyAstroArr[i].rrot.x = 0f;
        //         __instance.galaxyAstroArr[i].rrot.y = 0f;
        //         __instance.galaxyAstroArr[i].rrot.z = 0f;
        //         __instance.galaxyAstroArr[i].rrot.w = 1f;
        //         __instance.galaxyAstroArr[i].radius = 0f;
        //     }
        //
        //     __instance.starmapGalaxyAstroArr = new AstroPoseR[size];
        //     __instance.starmapGalaxyAstroBuffer = new ComputeBuffer(size, 32, ComputeBufferType.Default);
        //     Array.Copy(__instance.galaxyAstroArr, __instance.starmapGalaxyAstroArr, __instance.galaxyAstroArr.Length);
        //     return false;
        // }
        
        
        [HarmonyTranspiler, HarmonyPatch(typeof(SectorModel), nameof(SectorModel.CreateGalaxyAstroBuffer))]
        public static IEnumerable<CodeInstruction> TranspilerSectorModelCreateGalaxyAstroBuffer(
            IEnumerable<CodeInstruction> instructions)
        {
            var calcMethod = AccessTools.Method(typeof(PatchOnSectorModel), nameof(CalcAstroBufferSize));
            instructions = new CodeMatcher(instructions)
                // Replace 25600 with the result of the CalcAstroBufferSize method
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return i.opcode == Ldc_I4 && Convert.ToInt32(i.operand ?? 0) == 25600;
                    }
                ))
                .Repeat(matcher =>
                {
                    // Bootstrap.Logger.LogInfo($"Found value {matcher.Operand} at " + matcher.Pos);
                    matcher.SetInstructionAndAdvance(new CodeInstruction(Call, calcMethod));
                }).InstructionEnumeration();

            return instructions;
        }

        public static int CalcAstroBufferSize()
        {
            return GameMain.spaceSector.galaxy.starCount * 400;
        }
    }
}