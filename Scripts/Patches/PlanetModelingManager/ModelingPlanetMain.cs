using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static bool ModelingPlanetMain(PlanetData planet)
        {
            planet.data.AddFactoredRadius(planet);
            return true;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static IEnumerable<CodeInstruction> ModelingPlanetMainTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = new List<CodeInstruction>(instructions);
            var lodPatched = 0;
            var modPlanePatched = 0;

            //Patch.Debug("ModelingPlanetMain Transpiler.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
            for (var instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
                if (instructionList[instructionCounter].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
                {
                    //Patch.Debug("Found realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                    if (instructionCounter + 4 < instructionList.Count && instructionList[instructionCounter + 1].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 1].OperandIs(0.2f) && instructionList[instructionCounter + 2].opcode == OpCodes.Add && instructionList[instructionCounter + 3].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 3].OperandIs(0.025f))
                    {
                        //Patch.Debug("Found THE CORRECT realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                        //+1 = ldc.r4 0.2
                        //+2 = add
                        //+3 = ldc.r4 0.025 <-- replace
                        instructionList.RemoveAt(instructionCounter + 3);
                        var toInsert = new List<CodeInstruction>
                        {
                            new(OpCodes.Ldarg_0),
                            new(instructionList[instructionCounter]),
                            new(OpCodes.Ldc_R4, 8000f),
                            new(OpCodes.Div)
                        };
                        instructionList.InsertRange(instructionCounter + 3, toInsert);
                        lodPatched++;
                    }
                }
                else if (instructionList[instructionCounter].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                {
                    //GS2.Log("Found GetModPlane callvirt. Replacing with GetModPlaneInt call.");
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                    modPlanePatched++;
                }

            if (lodPatched == 0)
                GS2.Error("PlanetModelingManager.ModelingPlanetMain transpiler: realRadius+0.2f/0.025f LOD pattern not found (game update changed the method?). Planet mesh LOD will assume a radius-200 planet.");
            if (modPlanePatched == 0)
                GS2.Error("PlanetModelingManager.ModelingPlanetMain transpiler: GetModPlane call not found (game update changed the method?). Height mods on planets larger than ~327 radius may overflow.");

            return instructionList.AsEnumerable();
        }
    }
}