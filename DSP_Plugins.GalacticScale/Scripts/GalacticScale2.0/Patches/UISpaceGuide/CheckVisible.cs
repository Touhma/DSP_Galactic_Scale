using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace GalacticScale
{
    public partial class PatchOnUISpaceGuide
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
        [HarmonyTranspiler, HarmonyPatch(typeof(UISpaceGuide), "CheckVisible")]
        public static IEnumerable<CodeInstruction> VisibleTranspiler(IEnumerable<CodeInstruction> instructions) => ReplaceLd10(instructions);
        [HarmonyTranspiler, HarmonyPatch(typeof(UISpaceGuide), "CheckVisible")]
        public static IEnumerable<CodeInstruction> VisibleTranspiler2(IEnumerable<CodeInstruction> instructions) => ReplaceLd25(instructions);
        public static IEnumerable<CodeInstruction> ReplaceLd25(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].OperandIs(2.5f))
                {
                    codes[i] = new CodeInstruction(Transpilers.EmitDelegate<Del2>(
                    () =>
                    {
                        return 12.5f;
                    }));
                }
            }
            return codes.AsEnumerable();
        }

        private delegate float Del2();
        public static IEnumerable<CodeInstruction> ReplaceLd10(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].OperandIs(10))
                {
                    codes[i] = new CodeInstruction(Transpilers.EmitDelegate<Del>( // replace load10 with this delegate
                    () =>
                    {
                        // If localStar is defined, use its planetCount
                        if (GameMain.localStar != null)
                        {
                            return GameMain.localStar.planetCount;
                        }
                        // If localStar is not defined, stick with the default 10
                        return 10;
                    }));
                }
            }
            return codes.AsEnumerable();
        }

        private delegate int Del();
    }
}