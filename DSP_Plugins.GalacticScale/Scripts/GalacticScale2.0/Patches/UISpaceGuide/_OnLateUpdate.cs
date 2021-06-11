using System.Collections.Generic;
using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUISpaceGuide {

        //Strategy: Replace ldc.i4.s 10 instructions with a dynamic addition equal to the current system's planet count
        // Get the local system:
        /* 0x000E0746 02           */// IL_034A: ldarg.0
        /* 0x000E0747 7B0E190004   */// IL_034B: ldfld class GameData UISpaceGuide::gameData
        /* 0x000E074C 6F4C090006   */// IL_0350: callvirt instance class StarData GameData::get_localStar()
        // Get the planet count
        /* 0x000E0751 6F970A0006   */// IL_0355: ldfld instance int StarData::planetCount
        //
        //
        [HarmonyTranspiler, HarmonyPatch(typeof(UISpaceGuide),"_OnLateUpdate")]
        public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceLd10(instructions);
        }
    }
}