using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    [HarmonyPatch(typeof(UISpaceGuide))]
    public class PatchOnUISpaceGuide
    {

        //Strategy: Replace ldc.i4.s 10 instructions with a dynamic addition equal to the current system's planet count
        // Get the local system:
        /* 0x000E0746 02           */// IL_034A: ldarg.0
        /* 0x000E0747 7B0E190004   */// IL_034B: ldfld class GameData UISpaceGuide::gameData
        /* 0x000E074C 6F4C090006   */// IL_0350: callvirt instance class StarData GameData::get_localStar()
        // Get the planet count
        /* 0x000E0751 6F970A0006   */// IL_0355: ldfld instance int StarData::planetCount
        //
        //
        [HarmonyTranspiler]
        [HarmonyPatch("_OnLateUpdate")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].OperandIs(10))
                {
                    List<CodeInstruction> newInstructions = new List<CodeInstruction>();
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0)); // this.
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldfld, typeof(UISpaceGuide).GetField("gameData"))); // gameData.
                    newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, typeof(GameData).GetMethod("get_localStar"))); // localStar.
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldfld, typeof(StarData).GetField("planetCount"))); // planetCount
                    codes.RemoveAt(i); // remove the original instruction to load 10
                    codes.InsertRange(i, newInstructions); // in its place, instead load the combined lookup: this.gameData.localStar.planetCount
                }
            }
            return codes.AsEnumerable();
        }
    }
}