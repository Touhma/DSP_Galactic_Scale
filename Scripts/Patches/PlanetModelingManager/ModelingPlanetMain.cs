using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using BCE;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static bool ModelingPlanetMain(PlanetData planet)
        {
            // Only apply our custom logic to large planets in actual gameplay
            if (!DSPGame.IsMenuDemo && planet.radius > 200)
            {
                GS2.Log($"ModelingPlanetMain PREFIX: planet={planet.name}, radius={planet.radius}, " +
                       $"isMenuDemo={DSPGame.IsMenuDemo}, scale={planet.scale:F2}");
                planet.data.AddFactoredRadius(planet);
                GS2.Log($"  → Applied AddFactoredRadius for large planet");
                
                // Test our custom height data method directly
                if (planet.data?.heightData != null)
                {
                    GS2.Log($"  → Testing height data for {planet.name}:");
                    for (int i = 0; i < Math.Min(5, planet.data.heightData.Length); i++)
                    {
                        float height = planet.data.GetHeightDataFloatSafe(i);
                        GS2.Log($"  → Test heightData[{i}]: {height:F4}");
                    }
                    
                    // Check if all heights are the same (indicating flat terrain)
                    bool allSame = true;
                    float firstHeight = planet.data.GetHeightDataFloatSafe(0);
                    for (int i = 1; i < Math.Min(5, planet.data.heightData.Length); i++)
                    {
                        if (Math.Abs(planet.data.GetHeightDataFloatSafe(i) - firstHeight) > 0.001f)
                        {
                            allSame = false;
                            break;
                        }
                    }
                    
                    if (allSame)
                    {
                        GS2.Warn($"  → WARNING: All height values are identical ({firstHeight:F4}) - terrain generation may not be working!");
                    }
                    else
                    {
                        GS2.Log($"  → Height values are varied - terrain generation is working");
                    }
                }
            }
            return true;
        }
        
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetModelingManager), "ModelingPlanetMain")]
        public static IEnumerable<CodeInstruction> ModelingPlanetMainTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
                var instructionList = new List<CodeInstruction>(instructions);
                
                // Log original IL (this MUST execute to see what we're patching)
                instructions.Log("ModelingPlanetMain - ORIGINAL IL", 0, 100);
                
                GS2.Log("ModelingPlanetMainTranspiler: Starting IL analysis");
            
            // Patch heightmap texture size from hardcoded 512x512 to dynamic size based on planet radius
            bool heightmapSizePatched = false;
            int heightDataPatchCount = 0;
            
            for (var instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
            {
                // Find: ldc.i4 512 (for RenderTextureDescriptor constructor)
                // This appears twice in succession for width and height
                if (!heightmapSizePatched && 
                    instructionList[instructionCounter].opcode == OpCodes.Ldc_I4 && 
                    Convert.ToInt32(instructionList[instructionCounter].operand) == 512)
                {
                    // Check if the next instruction is also ldc.i4 512 (height parameter)
                    if (instructionCounter + 1 < instructionList.Count &&
                        instructionList[instructionCounter + 1].opcode == OpCodes.Ldc_I4 &&
                        Convert.ToInt32(instructionList[instructionCounter + 1].operand) == 512)
                    {
                        GS2.Log("Patching heightmap size from 512x512 to dynamic size");
                        
                        // Replace first 512 (width) with Utils.CalculateHeightmapSize(planet)
                        instructionList[instructionCounter] = new CodeInstruction(OpCodes.Ldarg_0); // Load planet parameter
                        instructionList.Insert(instructionCounter + 1, new CodeInstruction(OpCodes.Call, 
                            AccessTools.Method(typeof(Utils), nameof(Utils.CalculateHeightmapSize))));
                        
                        // Replace second 512 (height) with same call
                        // Now at +2 because we inserted one instruction
                        instructionList[instructionCounter + 2] = new CodeInstruction(OpCodes.Ldarg_0);
                        instructionList.Insert(instructionCounter + 3, new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Utils), nameof(Utils.CalculateHeightmapSize))));
                        
                        heightmapSizePatched = true;
                        GS2.Log("Successfully patched heightmap texture size");
                    }
                }
                
                // Patch ALL heightData reads to use GetHeightDataFloat for full precision
                // Pattern: ldfld heightData, ldloc index, ldelem.u2, conv.r4, ldc.r4 0.01, mul
                if (instructionList[instructionCounter].opcode == OpCodes.Ldfld && 
                    instructionList[instructionCounter].operand.ToString().Contains("heightData"))
                {
                    GS2.Log($"Found heightData ldfld at {instructionCounter}");
                    instructionList.Log("heightData pattern context", instructionCounter, 10);
                    
                    // Check if full pattern matches
                    if (instructionCounter + 5 < instructionList.Count &&
                        instructionList[instructionCounter + 1].opcode == OpCodes.Ldloc_S &&
                        instructionList[instructionCounter + 2].opcode == OpCodes.Ldelem_U2 &&
                        instructionList[instructionCounter + 3].opcode == OpCodes.Conv_R4 &&
                        instructionList[instructionCounter + 4].opcode == OpCodes.Ldc_R4 &&
                        instructionList[instructionCounter + 4].OperandIs(0.01f) &&
                        instructionList[instructionCounter + 5].opcode == OpCodes.Mul)
                    {
                        GS2.Log($"  → PATTERN MATCHED, applying patch");
                    // Replace: ldfld heightData, ldloc index, ldelem.u2, conv.r4, ldc.r4 0.01, mul
                    // With: nop, ldloc index, Call GetHeightDataFloat, nop, nop, nop
                    // The PlanetRawData is already on the stack before ldfld!
                    
                    // FIXED: Don't replace ldfld heightData with nop - we need the PlanetRawData on the stack
                    // Keep the original ldfld heightData instruction (it loads the PlanetRawData)
                    // Keep ldloc index (at instructionCounter + 1) 
                    // Replace ldelem.u2 with Call GetHeightDataFloatSafe (includes runtime guard)
                    instructionList[instructionCounter + 2] = new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(PlanetRawDataExtension), nameof(PlanetRawDataExtension.GetHeightDataFloatSafe)));
                    // Replace conv.r4, ldc.r4 0.01, mul with nops (preserve scale multiplication after)
                    instructionList[instructionCounter + 3] = new CodeInstruction(OpCodes.Nop);
                    instructionList[instructionCounter + 4] = new CodeInstruction(OpCodes.Nop);
                    instructionList[instructionCounter + 5] = new CodeInstruction(OpCodes.Nop);
                    
                        heightDataPatchCount++;
                        GS2.Log($"Applied heightData patch {heightDataPatchCount} at instruction {instructionCounter}");
                        instructionList.Log("heightData pattern after patch", instructionCounter, 6);
                        
                        // Skip ahead to avoid re-processing the patched instructions
                        instructionCounter += 5;
                    }
                    else
                    {
                        GS2.Warn($"  → Pattern incomplete, skipping");
                    }
                }
                else if (instructionList[instructionCounter].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
                {
                    //Patch.Debug("Found realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetManagerDeep);
                    if (instructionCounter + 4 < instructionList.Count && instructionList[instructionCounter + 1].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 1].OperandIs(0.2f) && instructionList[instructionCounter + 2].opcode == OpCodes.Add && instructionList[instructionCounter + 3].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 3].OperandIs(0.025f))
                    {
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
                    }
                }
                else if (instructionList[instructionCounter].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                {
                    //GS2.Log("Found GetModPlane callvirt. Replacing with GetModPlaneInt call.");
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                }
            }
            
            if (!heightmapSizePatched)
            {
                GS2.Warn("Failed to patch heightmap texture size - 512 limit may still apply!");
            }
            
            if (heightDataPatchCount == 0)
            {
                GS2.Warn("Failed to patch ANY heightData reads - terrain will not display correctly on large planets!");
            }
            else
            {
                GS2.Log($"Successfully patched {heightDataPatchCount} heightData read locations");
            }

            GS2.Log($"Transpiler done. HeightmapPatched={heightmapSizePatched}, HeightDataPatches={heightDataPatchCount}");
            
            // Log final IL to verify patches
            instructionList.Log("ModelingPlanetMain - FINAL IL", 0, 50);
            
            return instructionList.AsEnumerable();
        }
    }
}