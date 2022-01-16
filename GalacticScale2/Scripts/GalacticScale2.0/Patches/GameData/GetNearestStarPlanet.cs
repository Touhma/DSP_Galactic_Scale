/*
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GalacticScale {

    [HarmonyPatch(typeof(GameData))]
    public partial class PatchOnGameData {

        delegate void printInfo(ref StarData nearestStar, int pIndex, double num7);

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(GameData.GetNearestStarPlanet))]
        public static IEnumerable<CodeInstruction> GetNearestStarPlanet_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_realRadius"),
                    new CodeMatch(OpCodes.Conv_R8),
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Stloc_S))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 7))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 8))
                .Insert(HarmonyLib.Transpilers.EmitDelegate<printInfo>((ref StarData nearestStar, int pIndex, double num7) =>
                {
                    GS2.Warn($"{nearestStar?.planets[pIndex].displayName} is {num7} away");
                }));
            return matcher.InstructionEnumeration();
        }

	}
}
*/
