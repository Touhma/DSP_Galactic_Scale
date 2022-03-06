using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetData
    {
        //Strategy: 1) Remove checks for PlanetData.scale; 2) Convert GetModPlane to use our Int version
        // 1) find all calls to ldArg.0
        // if the following instruction is ldfld float32 PlanetData::scale
        //    change ldarg.0 to OpCodes.Nop
        //    change the following instruction to ldc.r4 1
        // 2) find all calls to GetModPlane
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetData), "UpdateDirtyMesh")]
        public static IEnumerable<CodeInstruction> UpdateDirtyMeshTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldarg_0 && i < codes.Count - 1)
                    // This condition removes references to this.scale. First we just check for the IL equivalent of "this."
                    // We stop checking this 1 early because we operate on both the current AND following line when we do anything
                {
                    // Check if the field we're reading from "this." is scale
                    if (codes[i + 1].LoadsField(typeof(PlanetData).GetField("scale")))
                    {
                        // Prevent "this." from being added to the stack (ordinarily, the field reference would remove it from the stack)
                        codes[i] = new CodeInstruction(OpCodes.Nop);
                        // Instead load the fixed value 1, as a float32
                        codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 1f);
                    }
                }
                else if (codes[i].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                    // This condition finds calls to PlanetRawData.GetModPlane (which returns a short)
                {
                    // We instead call PlanetRawDataExtension.GetModPlaneInt (which returns an int)
                    // All existing calls to GetModPlane cast the result to a float, anyway...
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                }

            return codes.AsEnumerable();
        }
    }
}