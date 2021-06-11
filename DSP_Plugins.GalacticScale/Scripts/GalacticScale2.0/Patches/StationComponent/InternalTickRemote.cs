using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;

namespace GalacticScale {
    public class PatchOnStationComponent {

        // Strategy: find where  replace all ldc.i4.s 10 instructions with dynamic references to the relevant star's planetCount
        //
        // GameMain.galaxy.PlanetById(int) returns null if not a planet, otherwise PlanetData
        /* 0x0002D43D 7E05130004   */// IL_000D: callVirt    class GalaxyData GameMain::get_galaxy
        // IL_xxxx: See below for how we find the right planet ID
        // IL_xxxx: callVirt class PlanetData GalaxyData::PlanetById(int)
        //
        // From there, PlanetData.star.planetCount gets what we need.
        //
        // Finding the planetID:
        // First, the C# code at this time:
        //   int num43 = shipData.planetA / 100 * 100; //this is later passed to astroPoses[i] and it basically represents a planet ID.
        //   int num44 = shipData.planetB / 100 * 100; //this is later passed to astroPoses[i] and it basically represents a planet ID.
        //	 for (int k = num43; k<num43 + 10; k++) {...}
        //   if (num44 != num43) {
        //     for (int l = num44; l<num44 + 10; l++) {...}}
        //
        // Note that "basically represents" is important - astroPoses is a zero-indexed array, but IDs start at 1.
        //
        // For loops are kind of backwards in CIL (compared to C# anyhow). Here's original IL for the planet IDs and prep...
        /* 0x0002EFFD 1221         */// IL_1BCD: ldloca.s V_33                  // shipData.
        /* 0x0002EFFF 7B28050004   */// IL_1BCF: ldfld int32 ShipData::planetA  // load planetA ID from prior
        /* 0x0002F004 1F64         */// IL_1BD4: ldc.i4.s  100                  // load 100
        /* 0x0002F006 5B           */// IL_1BD6: div                            // divide planetA ID by 100
        /* 0x0002F007 1F64         */// IL_1BD7: ldc.i4.s  100                  // load 100
        /* 0x0002F009 5A           */// IL_1BD9: mul                            // multiply result by 100
        /* 0x0002F00A 1340         */// IL_1BDA: stloc.s V_64                   // store into a variable (!)
        /* 0x0002F00C 1221         */// IL_1BDC: ldloca.s V_33                  // shipData. again
        /* 0x0002F00E 7B29050004   */// IL_1BDE: ldfld int32 ShipData::planetB  // load planetB ID from prior
        /* 0x0002F013 1F64         */// IL_1BE3: ldc.i4.s  100                  // load 100
        /* 0x0002F015 5B           */// IL_1BE5: div                            // divide planetB ID by 100
        /* 0x0002F016 1F64         */// IL_1BE6: ldc.i4.s  100                  // load 100
        /* 0x0002F018 5A           */// IL_1BE8: mul                            // multiply result by 100
        /* 0x0002F019 1341         */// IL_1BE9: stloc.s V_65                   // store into a different variable (!!)
        /* 0x0002F01B 1140         */// IL_1BEB: ldloc.s V_64                   // loop prep - load the planetA ID stored prior
        /* 0x0002F01D 1342         */// IL_1BED: stloc.s V_66                   // loop prep - save that ID into a new temp variable (i)
        /* 0x0002F01F 380E010000   */// IL_1BEF: br IL_1D02                     //skip to the for loop's limit checking line
                                     // ...                                     //skipping the code inside the loop
        /* 0x0002F132 1142         */// IL_1D02: ldloc.s   V_66                 // load in the loop variable
        /* 0x0002F134 1140         */// IL_1D04: ldloc.s   V_64                 // load planet A's ID
        /* 0x0002F136 1F0A         */// IL_1D06: ldc.i4.s  10                   // load the value 10
        /* 0x0002F138 58           */// IL_1D08: add                            // add the last two loads - planet A's ID + 10
        /* 0x0002F139 3FE6FEFFFF   */// IL_1D09: blt       IL_1BF4              // skip back to the actual loop code if the result is less than the loop variable loaded before it
        //
        // Unfortunately we can't guarantee that V_64 and V_65 are consistently going to be the variables we need, especially between patches and with other mods.
        // But, thankfully, we know that the line immediately before ldc.i4.s 10 is the variable we want to refer to.
        [HarmonyTranspiler, HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].OperandIs(10))
                {
                    List<CodeInstruction> newInstructions = new List<CodeInstruction>();
                    newInstructions.Add(new CodeInstruction(codes[i - 1])); //The line before adding 10 is the line which loads in the planet ID we care about, so copy it
                    newInstructions.Add(Transpilers.EmitDelegate<Del>(bodyID =>
                        // We add 1 to the body ID because it was originally an index in the astroPoses array but we need the actual ID of it.
                        // We add 1 to the planet count because the loop is <, not <=
                        GameMain.galaxy.PlanetById(bodyID + 1).star.planetCount + 1));
                    codes.RemoveAt(i); // Remove the original loading of 10
                    codes.InsertRange(i, newInstructions); //Instead, load the count of planets around the target star (plus one)
                }
            return codes.AsEnumerable();
            
        }
        delegate int Del(int bodyID);
    }


}