using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public static class PatchOnPlanetFactory
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetFactory), "FlattenTerrain")]
        public static IEnumerable<CodeInstruction> FlattenTerrainTranspiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            var instructionList = new List<CodeInstruction>(instructions);

            var intNum4 = generator.DeclareLocal(typeof(int));
            intNum4.SetLocalSymInfo("intNum4");

            for (var instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
                if (instructionList[instructionCounter].opcode == OpCodes.Conv_I2 &&
                    instructionCounter + 1 < instructionList.Count &&
                    instructionList[instructionCounter + 1].opcode == OpCodes.Stloc_S &&
                    instructionList[instructionCounter + 1].operand is LocalBuilder lb_stloc &&
                    lb_stloc.LocalIndex == 18)
                {
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Conv_I4);
                    instructionList[instructionCounter + 1] = new CodeInstruction(OpCodes.Stloc_S, intNum4);

                    instructionCounter++;
                }
                else if (instructionList[instructionCounter].opcode == OpCodes.Ldloc_S &&
                         instructionList[instructionCounter].operand is LocalBuilder lb_ldloc &&
                         lb_ldloc.LocalIndex == 18)
                {
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Ldloc_S, intNum4);
                }

            return instructionList.AsEnumerable();
        }
    }
}