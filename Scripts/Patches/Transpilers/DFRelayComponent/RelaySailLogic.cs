using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public partial class PatchOnDFRelayComponent
    {
        // Change Log:
        // - Fixed pathing issues with large stars by:
        //   1. Replacing hardcoded value 200.0 with planet's real radius
        //   2. Adding star radius adjustment for pathing calculations
        //   3. Patching star avoidance distance multiplier from 2.5f to 0.5f
        //   4. Adding adjustments to avoid safety distance calculations
        // - Updated pattern matching to be more robust and find the correct insertion points
        // - Fixed linter errors related to StarData field access
        // - 2026-02-22: Clamp all DF relay/carrier star radius pathing reads to vanilla max.

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent),  nameof(DFRelayComponent.RelaySailLogic))]
        public static IEnumerable<CodeInstruction> Fix200(IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4 || i.opcode == Ldc_R8 || i.opcode == Ldc_I4) &&
                               (
                                   Convert.ToDouble(i.operand ?? 0.0) == 200.0
                               );
                    })
                )
                .Repeat(matcher =>
                {
                    var mi = matcher.GetRadiusFromAstroId();
                    matcher.Advance(1);
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    matcher.InsertAndAdvance(new CodeInstruction(Ldfld, AccessTools.Field(typeof(DFRelayComponent), nameof(DFRelayComponent.targetAstroId))));
                    matcher.Insert(new CodeInstruction(Call, mi));
                }).InstructionEnumeration();

            return instructions;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.RelaySailLogic))]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.CarrierSailLogic))]
        public static IEnumerable<CodeInstruction> CapStarRadiusToVanilla(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            var capMethod = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].LoadsField(radiusField))
                {
                    yield return new CodeInstruction(OpCodes.Call, capMethod);
                }
            }
        }
        
        // Fix 2.5f star radius multiplier to 0.5f, same as we did for logistics ships
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.RelaySailLogic))]
        public static IEnumerable<CodeInstruction> FixStarMultiplier(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions).MatchForward(
                false, 
                new CodeMatch(op => op.opcode == Ldc_R4 && op.OperandIs(2.5f))
            );
            
            if (codeMatcher.IsInvalid)
            {
                GS2.Warn("RelaySailLogic 2.5f multiplier transpiler failed");
                return instructions;
            }
            
            instructions = codeMatcher.Repeat(z => z.Set(OpCodes.Ldc_R4, 0.5f))
                .InstructionEnumeration();
                
            return instructions;
        }
        
        // Fix star radius for pathing calculations - more robust approach
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.RelaySailLogic))]
        public static IEnumerable<CodeInstruction> FixStarRadiusForPathing(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            
            // Look for patterns that indicate radius loading in the obstacle detection code
            // Try different approaches to find the right spot
            
            // Look for patterns specific to "num18 = reference3.uRadius;" followed by star radius branching
            for (int i = 0; i < codes.Count - 5; i++)
            {
                // Log found patterns for debugging
                if (codes[i].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")))
                {
                    GS2.Log($"Found uRadius load at index {i}, next instruction: {codes[i+1].opcode}");
                    
                    if (i < codes.Count - 4 && (
                        // Match the pattern for star detection by modulo check
                        (codes[i+3].opcode == OpCodes.Rem || 
                         codes[i+4].opcode == OpCodes.Rem || 
                         (i < codes.Count - 5 && codes[i+5].opcode == OpCodes.Rem)) ||
                        // Or match the pattern for radius modification
                        (codes[i+2].opcode == OpCodes.Mul && 
                         codes[i+1].opcode == OpCodes.Stloc_S)))
                    {
                        // Found a radius load followed by star type check (modulo 100) or multiplication
                        codes.Insert(i+1, new CodeInstruction(OpCodes.Call, 
                            AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(AdjustRadiusForPathingDF))));
                        
                        patched = true;
                        i += 2; // Skip ahead
                        GS2.Log($"Inserted radius adjustment at index {i}");
                    }
                }
            }
            
            // If first approach failed, try looking at 5000f safety distance
            if (!patched)
            {
                GS2.Log("First approach failed, trying alternate method");
                
                // Find the safety distance near uRadius loads
                for (int i = 0; i < codes.Count - 10; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && 
                        (float)codes[i].operand >= 4900f && 
                        (float)codes[i].operand <= 5100f)
                    {
                        // Look for uRadius references nearby
                        for (int j = i-5; j < i+10 && j < codes.Count; j++)
                        {
                            if (j >= 0 && codes[j].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")))
                            {
                                // Insert our adjustment right after loading uRadius
                                codes.Insert(j+1, new CodeInstruction(OpCodes.Call, 
                                    AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(AdjustRadiusForPathingDF))));
                                
                                patched = true;
                                i = j + 2; // Skip ahead
                                GS2.Log($"Inserted radius adjustment at index {j+1} (second approach)");
                                break;
                            }
                        }
                    }
                }
            }
            
            if (patched)
            {
                GS2.Log("Successfully patched DFRelayComponent.RelaySailLogic for better pathing with large stars");
            }
            else
            {
                GS2.Warn("Failed to find insertion point for radius adjustment in DFRelayComponent.RelaySailLogic - will try alternative approach");
                
                // As a last resort, try to patch all uRadius loads
                int count = 0;
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    if (codes[i].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")))
                    {
                        // Insert our adjustment call after every uRadius load
                        codes.Insert(i+1, new CodeInstruction(OpCodes.Call, 
                            AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(AdjustRadiusForPathingDF))));
                        
                        i++; // Skip the instruction we just added
                        count++;
                    }
                }
                
                if (count > 0)
                {
                    GS2.Log($"Patched all {count} uRadius loads in DFRelayComponent.RelaySailLogic");
                    patched = true;
                }
            }
            
            if (!patched)
            {
                GS2.Error("All approaches failed to patch DFRelayComponent.RelaySailLogic");
            }
            
            return codes;
        }
        
        // Also patch the fixed safety distance value (5000f)
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.RelaySailLogic))]
        public static IEnumerable<CodeInstruction> FixSafetyDistance(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            
            // Find the safety distance calculation (typically around 5000f + speed)
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && 
                    (float)codes[i].operand >= 4900f && (float)codes[i].operand <= 5100f)
                {
                    // Add our adjustment method call
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(PatchOnDFRelayComponent), nameof(AdjustSafetyDistanceForPathCalculationDF))));
                    
                    patched = true;
                    i += 2; // Skip ahead since we modified the list
                    GS2.Log($"Patched safety distance at index {i}");
                }
            }
            
            if (patched)
            {
                GS2.Log("Patched DFRelayComponent.RelaySailLogic safety distances for large stars");
            }
            else
            {
                GS2.Warn("Failed to find safety distance in DFRelayComponent.RelaySailLogic");
            }
            
            return codes;
        }
        
        // Scaling function for avoidance calculations (similar to the one used for logistics ships)
        public static float AdjustRadiusForPathingDF(float originalRadius)
        {
            return DarkFogRadius.CapStarRadiusToVanillaMax(originalRadius);
        }
        
        // Adjust safety distance similar to the logistics ship patch
        public static float AdjustSafetyDistanceForPathCalculationDF(float originalDistance)
        {
            // Scale safety distance based on largest star radius in the galaxy
            float largestStarRadius = GetLargestStarRadiusDF();
            
            if (largestStarRadius <= 1000f)
                return originalDistance;
                
            // Scale the safety distance with diminishing returns
            float factor = 1f + Mathf.Log10(largestStarRadius / 1000f) * 0.75f;
            return originalDistance * factor;
        }
        
        // Helper to find the largest star radius in the galaxy (cached version)
        private static float largestCachedRadiusDF = 0f;
        private static int lastUpdateFrameDF = -1;
        
        private static float GetLargestStarRadiusDF()
        {
            // Cache this value as it's expensive to calculate
            if (Time.frameCount - lastUpdateFrameDF > 300 || largestCachedRadiusDF <= 0f)
            {
                lastUpdateFrameDF = Time.frameCount;
                largestCachedRadiusDF = 0f;
                
                if (GameMain.galaxy?.stars != null)
                {
                    foreach (var star in GameMain.galaxy.stars)
                    {
                        if (star != null)
                        {
                            // Use appropriate radius field based on what's available
                            float radius = 0f;
                            
                            // Try to access the radius using reflection to avoid compilation errors
                            if (star.GetType().GetField("physicsRadius") != null)
                            {
                                radius = (float)star.GetType().GetField("physicsRadius").GetValue(star);
                            }
                            else if (star.GetType().GetField("radius") != null)
                            {
                                radius = (float)star.GetType().GetField("radius").GetValue(star);
                            }
                            else
                            {
                                // Fallback if we can't find the right field
                                radius = 200f; // Default size
                            }
                            
                            if (radius > largestCachedRadiusDF)
                            {
                                largestCachedRadiusDF = radius;
                            }
                        }
                    }
                }
                
                // Fallback if we couldn't find a valid radius
                if (largestCachedRadiusDF <= 0f)
                    largestCachedRadiusDF = 1000f;
            }
            
            return largestCachedRadiusDF;
        }
    }
}

//Original IL:

// // Token: 0x06000777 RID: 1911 RVA: 0x0004857C File Offset: 0x0004677C
// .method public hidebysig 
// 	instance void RelaySailLogic (
// 		class SpaceSector sector,
// 		valuetype AstroData[] galaxyAstros,
// 		valuetype AstroData[] astros,
// 		valuetype EnemyData& enemyData,
// 		valuetype AnimData& animData,
// 		bool keyFrame
// 	) cil managed 
// {
// 	// Header Size: 12 bytes
// 	// Code Size: 5104 (0x13F0) bytes
// 	// LocalVarSig Token: 0x11000163 RID: 355
// 	.maxstack 6
// 	.locals init (
// 		[0] float32,
// 		[1] float32,
// 		[2] int32,
// 		[3] valuetype VectorLF3,
// 		[4] valuetype VectorLF3,
// 		[5] float32,
// 		[6] valuetype VectorLF3,
// 		[7] float64,
// 		[8] float32,
// 		[9] valuetype VectorLF3,
// 		[10] valuetype AstroData&,
// 		[11] valuetype VectorLF3,
// 		[12] valuetype VectorLF3,
// 		[13] valuetype VectorLF3,
// 		[14] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[15] valuetype VectorLF3,
// 		[16] bool,
// 		[17] valuetype VectorLF3,
// 		[18] float64,
// 		[19] bool,
// 		[20] float64,
// 		[21] float32,
// 		[22] float32,
// 		[23] int32,
// 		[24] float64,
// 		[25] float64,
// 		[26] int32,
// 		[27] float32,
// 		[28] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[29] valuetype VectorLF3,
// 		[30] float64,
// 		[31] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[32] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[33] float32,
// 		[34] class PlanetFactory,
// 		[35] valuetype VectorLF3,
// 		[36] valuetype AstroData&,
// 		[37] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[38] class PlanetFactory,
// 		[39] int32,
// 		[40] float32,
// 		[41] float32,
// 		[42] valuetype VectorLF3,
// 		[43] float64,
// 		[44] float64,
// 		[45] valuetype AstroData&,
// 		[46] float32,
// 		[47] float64,
// 		[48] float64,
// 		[49] float64,
// 		[50] float64,
// 		[51] float64,
// 		[52] float64,
// 		[53] valuetype VectorLF3,
// 		[54] float64,
// 		[55] valuetype VectorLF3,
// 		[56] float64,
// 		[57] valuetype VectorLF3,
// 		[58] valuetype VectorLF3,
// 		[59] valuetype AstroData&,
// 		[60] valuetype VectorLF3&,
// 		[61] valuetype VectorLF3,
// 		[62] float64,
// 		[63] float64,
// 		[64] int32,
// 		[65] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[66] float32,
// 		[67] valuetype VectorLF3,
// 		[68] valuetype VectorLF3,
// 		[69] float32,
// 		[70] float32,
// 		[71] valuetype VectorLF3,
// 		[72] float32,
// 		[73] float32,
// 		[74] valuetype VectorLF3,
// 		[75] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[76] float32,
// 		[77] float32
// 	)

// 	/* 0x00046788 220000FA43   */ IL_0000: ldc.r4    500
// 	/* 0x0004678D 0A           */ IL_0005: stloc.0
// 	/* 0x0004678E 220000C843   */ IL_0006: ldc.r4    400
// 	/* 0x00046793 0B           */ IL_000B: stloc.1
// 	/* 0x00046794 02           */ IL_000C: ldarg.0
// 	/* 0x00046795 7B9C060004   */ IL_000D: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x0004679A 7B990D0004   */ IL_0012: ldfld     class StarData EnemyDFHiveSystem::starData
// 	/* 0x0004679F 6F5D190006   */ IL_0017: callvirt  instance int32 StarData::get_astroId()
// 	/* 0x000467A4 0C           */ IL_001C: stloc.2
// 	/* 0x000467A5 0E05         */ IL_001D: ldarg.s   animData
// 	/* 0x000467A7 02           */ IL_001F: ldarg.0
// 	/* 0x000467A8 7BA7060004   */ IL_0020: ldfld     int32 DFRelayComponent::stage
// 	/* 0x000467AD 18           */ IL_0025: ldc.i4.2
// 	/* 0x000467AE 58           */ IL_0026: add
// 	/* 0x000467AF 7D34180004   */ IL_0027: stfld     uint32 AnimData::state
// 	/* 0x000467B4 02           */ IL_002C: ldarg.0
// 	/* 0x000467B5 7BA7060004   */ IL_002D: ldfld     int32 DFRelayComponent::stage
// 	/* 0x000467BA 1FFE         */ IL_0032: ldc.i4.s  -2
// 	/* 0x000467BC 3D02010000   */ IL_0034: bgt       IL_013B

// 	/* 0x000467C1 0E04         */ IL_0039: ldarg.s   enemyData
// 	/* 0x000467C3 02           */ IL_003B: ldarg.0
// 	/* 0x000467C4 7B9D060004   */ IL_003C: ldfld     int32 DFRelayComponent::hiveAstroId
// 	/* 0x000467C9 7D161A0004   */ IL_0041: stfld     int32 EnemyData::astroId
// 	/* 0x000467CE 02           */ IL_0046: ldarg.0
// 	/* 0x000467CF 2200000000   */ IL_0047: ldc.r4    0.0
// 	/* 0x000467D4 7DA8060004   */ IL_004C: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x000467D9 0E04         */ IL_0051: ldarg.s   enemyData
// 	/* 0x000467DB 02           */ IL_0053: ldarg.0
// 	/* 0x000467DC 7C9B060004   */ IL_0054: ldflda    valuetype DFDock DFRelayComponent::dock
// 	/* 0x000467E1 7BD8060004   */ IL_0059: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFDock::pos
// 	/* 0x000467E6 28BC020006   */ IL_005E: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x000467EB 7D1E1A0004   */ IL_0063: stfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x000467F0 0E04         */ IL_0068: ldarg.s   enemyData
// 	/* 0x000467F2 02           */ IL_006A: ldarg.0
// 	/* 0x000467F3 7C9B060004   */ IL_006B: ldflda    valuetype DFDock DFRelayComponent::dock
// 	/* 0x000467F8 7BD9060004   */ IL_0070: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion DFDock::rot
// 	/* 0x000467FD 7D1F1A0004   */ IL_0075: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00046802 0E04         */ IL_007A: ldarg.s   enemyData
// 	/* 0x00046804 7C201A0004   */ IL_007C: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046809 2200000000   */ IL_0081: ldc.r4    0.0
// 	/* 0x0004680E 7D4100000A   */ IL_0086: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x00046813 0E04         */ IL_008B: ldarg.s   enemyData
// 	/* 0x00046815 7C201A0004   */ IL_008D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x0004681A 2200000000   */ IL_0092: ldc.r4    0.0
// 	/* 0x0004681F 7D4200000A   */ IL_0097: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x00046824 0E04         */ IL_009C: ldarg.s   enemyData
// 	/* 0x00046826 7C201A0004   */ IL_009E: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x0004682B 2200000000   */ IL_00A3: ldc.r4    0.0
// 	/* 0x00046830 7D8000000A   */ IL_00A8: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x00046835 0E05         */ IL_00AD: ldarg.s   animData
// 	/* 0x00046837 2200000000   */ IL_00AF: ldc.r4    0.0
// 	/* 0x0004683C 7D35180004   */ IL_00B4: stfld     float32 AnimData::power
// 	/* 0x00046841 0E05         */ IL_00B9: ldarg.s   animData
// 	/* 0x00046843 2200000000   */ IL_00BB: ldc.r4    0.0
// 	/* 0x00046848 7D31180004   */ IL_00C0: stfld     float32 AnimData::time
// 	/* 0x0004684D 02           */ IL_00C5: ldarg.0
// 	/* 0x0004684E 7BA6060004   */ IL_00C6: ldfld     int32 DFRelayComponent::direction
// 	/* 0x00046853 16           */ IL_00CB: ldc.i4.0
// 	/* 0x00046854 3166         */ IL_00CC: ble.s     IL_0134

// 	/* 0x00046856 0E06         */ IL_00CE: ldarg.s   keyFrame
// 	/* 0x00046858 391A130000   */ IL_00D0: brfalse   IL_13EF

// 	/* 0x0004685D 02           */ IL_00D5: ldarg.0
// 	/* 0x0004685E 2876070006   */ IL_00D6: call      instance bool DFRelayComponent::SearchTargetPlaceProcess()
// 	/* 0x00046863 390F130000   */ IL_00DB: brfalse   IL_13EF

// 	/* 0x00046868 02           */ IL_00E0: ldarg.0
// 	/* 0x00046869 02           */ IL_00E1: ldarg.0
// 	/* 0x0004686A 7BB8060004   */ IL_00E2: ldfld     int32 DFRelayComponent::searchAstroId
// 	/* 0x0004686F 7D9E060004   */ IL_00E7: stfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046874 02           */ IL_00EC: ldarg.0
// 	/* 0x00046875 02           */ IL_00ED: ldarg.0
// 	/* 0x00046876 7BB9060004   */ IL_00EE: ldfld     int32 DFRelayComponent::searchBaseId
// 	/* 0x0004687B 7DA2060004   */ IL_00F3: stfld     int32 DFRelayComponent::baseId
// 	/* 0x00046880 02           */ IL_00F8: ldarg.0
// 	/* 0x00046881 02           */ IL_00F9: ldarg.0
// 	/* 0x00046882 7BBB060004   */ IL_00FA: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::searchLPos
// 	/* 0x00046887 7D9F060004   */ IL_00FF: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x0004688C 02           */ IL_0104: ldarg.0
// 	/* 0x0004688D 02           */ IL_0105: ldarg.0
// 	/* 0x0004688E 7B9C060004   */ IL_0106: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046893 7CA50D0004   */ IL_010B: ldflda    int32 EnemyDFHiveSystem::rtseed
// 	/* 0x00046898 2068010000   */ IL_0110: ldc.i4    360
// 	/* 0x0004689D 283D040006   */ IL_0115: call      int32 RandomTable::Integer(int32&, int32)
// 	/* 0x000468A2 6B           */ IL_011A: conv.r4
// 	/* 0x000468A3 7DA0060004   */ IL_011B: stfld     float32 DFRelayComponent::targetYaw
// 	/* 0x000468A8 02           */ IL_0120: ldarg.0
// 	/* 0x000468A9 16           */ IL_0121: ldc.i4.0
// 	/* 0x000468AA 7DA1060004   */ IL_0122: stfld     int32 DFRelayComponent::baseState
// 	/* 0x000468AF 02           */ IL_0127: ldarg.0
// 	/* 0x000468B0 2875070006   */ IL_0128: call      instance void DFRelayComponent::ResetSearchStates()
// 	/* 0x000468B5 02           */ IL_012D: ldarg.0
// 	/* 0x000468B6 2870070006   */ IL_012E: call      instance void DFRelayComponent::LeaveDock()
// 	/* 0x000468BB 2A           */ IL_0133: ret

// 	/* 0x000468BC 02           */ IL_0134: ldarg.0
// 	/* 0x000468BD 2871070006   */ IL_0135: call      instance void DFRelayComponent::ArriveDock()
// 	/* 0x000468C2 2A           */ IL_013A: ret

// 	/* 0x000468C3 02           */ IL_013B: ldarg.0
// 	/* 0x000468C4 7BA7060004   */ IL_013C: ldfld     int32 DFRelayComponent::stage
// 	/* 0x000468C9 15           */ IL_0141: ldc.i4.m1
// 	/* 0x000468CA 4063020000   */ IL_0142: bne.un    IL_03AA

// 	/* 0x000468CF 02           */ IL_0147: ldarg.0
// 	/* 0x000468D0 7C9B060004   */ IL_0148: ldflda    valuetype DFDock DFRelayComponent::dock
// 	/* 0x000468D5 7BD8060004   */ IL_014D: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFDock::pos
// 	/* 0x000468DA 28BC020006   */ IL_0152: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x000468DF 0D           */ IL_0157: stloc.3
// 	/* 0x000468E0 02           */ IL_0158: ldarg.0
// 	/* 0x000468E1 7BA6060004   */ IL_0159: ldfld     int32 DFRelayComponent::direction
// 	/* 0x000468E6 16           */ IL_015E: ldc.i4.0
// 	/* 0x000468E7 3E2A010000   */ IL_015F: ble       IL_028E

// 	/* 0x000468EC 0E04         */ IL_0164: ldarg.s   enemyData
// 	/* 0x000468EE 7B1E1A0004   */ IL_0166: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x000468F3 09           */ IL_016B: ldloc.3
// 	/* 0x000468F4 28BA020006   */ IL_016C: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000468F9 1304         */ IL_0171: stloc.s   V_4
// 	/* 0x000468FB 1104         */ IL_0173: ldloc.s   V_4
// 	/* 0x000468FD 7B41030004   */ IL_0175: ldfld     float64 VectorLF3::x
// 	/* 0x00046902 1104         */ IL_017A: ldloc.s   V_4
// 	/* 0x00046904 7B41030004   */ IL_017C: ldfld     float64 VectorLF3::x
// 	/* 0x00046909 5A           */ IL_0181: mul
// 	/* 0x0004690A 1104         */ IL_0182: ldloc.s   V_4
// 	/* 0x0004690C 7B42030004   */ IL_0184: ldfld     float64 VectorLF3::y
// 	/* 0x00046911 1104         */ IL_0189: ldloc.s   V_4
// 	/* 0x00046913 7B42030004   */ IL_018B: ldfld     float64 VectorLF3::y
// 	/* 0x00046918 5A           */ IL_0190: mul
// 	/* 0x00046919 58           */ IL_0191: add
// 	/* 0x0004691A 1104         */ IL_0192: ldloc.s   V_4
// 	/* 0x0004691C 7B43030004   */ IL_0194: ldfld     float64 VectorLF3::z
// 	/* 0x00046921 1104         */ IL_0199: ldloc.s   V_4
// 	/* 0x00046923 7B43030004   */ IL_019B: ldfld     float64 VectorLF3::z
// 	/* 0x00046928 5A           */ IL_01A0: mul
// 	/* 0x00046929 58           */ IL_01A1: add
// 	/* 0x0004692A 284B02000A   */ IL_01A2: call      float64 [netstandard]System.Math::Sqrt(float64)
// 	/* 0x0004692F 6B           */ IL_01A7: conv.r4
// 	/* 0x00046930 1305         */ IL_01A8: stloc.s   V_5
// 	/* 0x00046932 1105         */ IL_01AA: ldloc.s   V_5
// 	/* 0x00046934 2200004843   */ IL_01AC: ldc.r4    200
// 	/* 0x00046939 3611         */ IL_01B1: ble.un.s  IL_01C4

// 	/* 0x0004693B 02           */ IL_01B3: ldarg.0
// 	/* 0x0004693C 16           */ IL_01B4: ldc.i4.0
// 	/* 0x0004693D 7DA7060004   */ IL_01B5: stfld     int32 DFRelayComponent::stage
// 	/* 0x00046942 03           */ IL_01BA: ldarg.1
// 	/* 0x00046943 08           */ IL_01BB: ldloc.2
// 	/* 0x00046944 0E04         */ IL_01BC: ldarg.s   enemyData
// 	/* 0x00046946 6F41190006   */ IL_01BE: callvirt  instance void SpaceSector::AlterEnemyAstroId(int32, valuetype EnemyData&)
// 	/* 0x0004694B 2A           */ IL_01C3: ret

// 	/* 0x0004694C 02           */ IL_01C4: ldarg.0
// 	/* 0x0004694D 02           */ IL_01C5: ldarg.0
// 	/* 0x0004694E 7BA8060004   */ IL_01C6: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046953 22ABAAAA3D   */ IL_01CB: ldc.r4    0.083333336
// 	/* 0x00046958 58           */ IL_01D0: add
// 	/* 0x00046959 7DA8060004   */ IL_01D1: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x0004695E 0E04         */ IL_01D6: ldarg.s   enemyData
// 	/* 0x00046960 7C201A0004   */ IL_01D8: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046965 2200000000   */ IL_01DD: ldc.r4    0.0
// 	/* 0x0004696A 7D4100000A   */ IL_01E2: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x0004696F 0E04         */ IL_01E7: ldarg.s   enemyData
// 	/* 0x00046971 7C201A0004   */ IL_01E9: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046976 02           */ IL_01EE: ldarg.0
// 	/* 0x00046977 7BA8060004   */ IL_01EF: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x0004697C 65           */ IL_01F4: neg
// 	/* 0x0004697D 7D4200000A   */ IL_01F5: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x00046982 0E04         */ IL_01FA: ldarg.s   enemyData
// 	/* 0x00046984 7C201A0004   */ IL_01FC: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046989 2200000000   */ IL_0201: ldc.r4    0.0
// 	/* 0x0004698E 7D8000000A   */ IL_0206: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x00046993 0E04         */ IL_020B: ldarg.s   enemyData
// 	/* 0x00046995 7C1E1A0004   */ IL_020D: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x0004699A 7C41030004   */ IL_0212: ldflda    float64 VectorLF3::x
// 	/* 0x0004699F 25           */ IL_0217: dup
// 	/* 0x000469A0 4F           */ IL_0218: ldind.r8
// 	/* 0x000469A1 0E04         */ IL_0219: ldarg.s   enemyData
// 	/* 0x000469A3 7C201A0004   */ IL_021B: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000469A8 7B4100000A   */ IL_0220: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x000469AD 6C           */ IL_0225: conv.r8
// 	/* 0x000469AE 23111111111111913F */ IL_0226: ldc.r8    0.016666666666666666
// 	/* 0x000469B7 5A           */ IL_022F: mul
// 	/* 0x000469B8 58           */ IL_0230: add
// 	/* 0x000469B9 57           */ IL_0231: stind.r8
// 	/* 0x000469BA 0E04         */ IL_0232: ldarg.s   enemyData
// 	/* 0x000469BC 7C1E1A0004   */ IL_0234: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x000469C1 7C42030004   */ IL_0239: ldflda    float64 VectorLF3::y
// 	/* 0x000469C6 25           */ IL_023E: dup
// 	/* 0x000469C7 4F           */ IL_023F: ldind.r8
// 	/* 0x000469C8 0E04         */ IL_0240: ldarg.s   enemyData
// 	/* 0x000469CA 7C201A0004   */ IL_0242: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000469CF 7B4200000A   */ IL_0247: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x000469D4 6C           */ IL_024C: conv.r8
// 	/* 0x000469D5 23111111111111913F */ IL_024D: ldc.r8    0.016666666666666666
// 	/* 0x000469DE 5A           */ IL_0256: mul
// 	/* 0x000469DF 58           */ IL_0257: add
// 	/* 0x000469E0 57           */ IL_0258: stind.r8
// 	/* 0x000469E1 0E04         */ IL_0259: ldarg.s   enemyData
// 	/* 0x000469E3 7C1E1A0004   */ IL_025B: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x000469E8 7C43030004   */ IL_0260: ldflda    float64 VectorLF3::z
// 	/* 0x000469ED 25           */ IL_0265: dup
// 	/* 0x000469EE 4F           */ IL_0266: ldind.r8
// 	/* 0x000469EF 0E04         */ IL_0267: ldarg.s   enemyData
// 	/* 0x000469F1 7C201A0004   */ IL_0269: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000469F6 7B8000000A   */ IL_026E: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x000469FB 6C           */ IL_0273: conv.r8
// 	/* 0x000469FC 23111111111111913F */ IL_0274: ldc.r8    0.016666666666666666
// 	/* 0x00046A05 5A           */ IL_027D: mul
// 	/* 0x00046A06 58           */ IL_027E: add
// 	/* 0x00046A07 57           */ IL_027F: stind.r8
// 	/* 0x00046A08 0E05         */ IL_0280: ldarg.s   animData
// 	/* 0x00046A0A 1105         */ IL_0282: ldloc.s   V_5
// 	/* 0x00046A0C 7D35180004   */ IL_0284: stfld     float32 AnimData::power
// 	/* 0x00046A11 380F010000   */ IL_0289: br        IL_039D

// 	/* 0x00046A16 09           */ IL_028E: ldloc.3
// 	/* 0x00046A17 0E04         */ IL_028F: ldarg.s   enemyData
// 	/* 0x00046A19 7B1E1A0004   */ IL_0291: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046A1E 28BA020006   */ IL_0296: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x00046A23 1306         */ IL_029B: stloc.s   V_6
// 	/* 0x00046A25 1106         */ IL_029D: ldloc.s   V_6
// 	/* 0x00046A27 7B41030004   */ IL_029F: ldfld     float64 VectorLF3::x
// 	/* 0x00046A2C 1106         */ IL_02A4: ldloc.s   V_6
// 	/* 0x00046A2E 7B41030004   */ IL_02A6: ldfld     float64 VectorLF3::x
// 	/* 0x00046A33 5A           */ IL_02AB: mul
// 	/* 0x00046A34 1106         */ IL_02AC: ldloc.s   V_6
// 	/* 0x00046A36 7B42030004   */ IL_02AE: ldfld     float64 VectorLF3::y
// 	/* 0x00046A3B 1106         */ IL_02B3: ldloc.s   V_6
// 	/* 0x00046A3D 7B42030004   */ IL_02B5: ldfld     float64 VectorLF3::y
// 	/* 0x00046A42 5A           */ IL_02BA: mul
// 	/* 0x00046A43 58           */ IL_02BB: add
// 	/* 0x00046A44 1106         */ IL_02BC: ldloc.s   V_6
// 	/* 0x00046A46 7B43030004   */ IL_02BE: ldfld     float64 VectorLF3::z
// 	/* 0x00046A4B 1106         */ IL_02C3: ldloc.s   V_6
// 	/* 0x00046A4D 7B43030004   */ IL_02C5: ldfld     float64 VectorLF3::z
// 	/* 0x00046A52 5A           */ IL_02CA: mul
// 	/* 0x00046A53 58           */ IL_02CB: add
// 	/* 0x00046A54 284B02000A   */ IL_02CC: call      float64 [netstandard]System.Math::Sqrt(float64)
// 	/* 0x00046A59 1307         */ IL_02D1: stloc.s   V_7
// 	/* 0x00046A5B 1107         */ IL_02D3: ldloc.s   V_7
// 	/* 0x00046A5D 23000000000000E03F */ IL_02D5: ldc.r8    0.5
// 	/* 0x00046A66 3409         */ IL_02DE: bge.un.s  IL_02E9

// 	/* 0x00046A68 02           */ IL_02E0: ldarg.0
// 	/* 0x00046A69 1FFE         */ IL_02E1: ldc.i4.s  -2
// 	/* 0x00046A6B 7DA7060004   */ IL_02E3: stfld     int32 DFRelayComponent::stage
// 	/* 0x00046A70 2A           */ IL_02E8: ret

// 	/* 0x00046A71 1107         */ IL_02E9: ldloc.s   V_7
// 	/* 0x00046A73 239A9999999999E93F */ IL_02EB: ldc.r8    0.8
// 	/* 0x00046A7C 5A           */ IL_02F4: mul
// 	/* 0x00046A7D 23000000000000F03F */ IL_02F5: ldc.r8    1
// 	/* 0x00046A86 58           */ IL_02FE: add
// 	/* 0x00046A87 6B           */ IL_02FF: conv.r4
// 	/* 0x00046A88 1308         */ IL_0300: stloc.s   V_8
// 	/* 0x00046A8A 1108         */ IL_0302: ldloc.s   V_8
// 	/* 0x00046A8C 2200002041   */ IL_0304: ldc.r4    10
// 	/* 0x00046A91 3607         */ IL_0309: ble.un.s  IL_0312

// 	/* 0x00046A93 2200002041   */ IL_030B: ldc.r4    10
// 	/* 0x00046A98 1308         */ IL_0310: stloc.s   V_8

// 	/* 0x00046A9A 0E04         */ IL_0312: ldarg.s   enemyData
// 	/* 0x00046A9C 7B1E1A0004   */ IL_0314: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046AA1 1309         */ IL_0319: stloc.s   V_9
// 	/* 0x00046AA3 0E04         */ IL_031B: ldarg.s   enemyData
// 	/* 0x00046AA5 0E04         */ IL_031D: ldarg.s   enemyData
// 	/* 0x00046AA7 7B1E1A0004   */ IL_031F: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046AAC 28BD020006   */ IL_0324: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00046AB1 09           */ IL_0329: ldloc.3
// 	/* 0x00046AB2 28BD020006   */ IL_032A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00046AB7 1108         */ IL_032F: ldloc.s   V_8
// 	/* 0x00046AB9 228988883C   */ IL_0331: ldc.r4    0.016666668
// 	/* 0x00046ABE 5A           */ IL_0336: mul
// 	/* 0x00046ABF 285303000A   */ IL_0337: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::MoveTowards(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x00046AC4 28BC020006   */ IL_033C: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00046AC9 7D1E1A0004   */ IL_0341: stfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046ACE 0E04         */ IL_0346: ldarg.s   enemyData
// 	/* 0x00046AD0 0E04         */ IL_0348: ldarg.s   enemyData
// 	/* 0x00046AD2 7B1F1A0004   */ IL_034A: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00046AD7 02           */ IL_034F: ldarg.0
// 	/* 0x00046AD8 7C9B060004   */ IL_0350: ldflda    valuetype DFDock DFRelayComponent::dock
// 	/* 0x00046ADD 7BD9060004   */ IL_0355: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion DFDock::rot
// 	/* 0x00046AE2 220000003F   */ IL_035A: ldc.r4    0.5
// 	/* 0x00046AE7 285403000A   */ IL_035F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::RotateTowards(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 	/* 0x00046AEC 7D1F1A0004   */ IL_0364: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00046AF1 0E04         */ IL_0369: ldarg.s   enemyData
// 	/* 0x00046AF3 0E04         */ IL_036B: ldarg.s   enemyData
// 	/* 0x00046AF5 7B1E1A0004   */ IL_036D: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046AFA 1109         */ IL_0372: ldloc.s   V_9
// 	/* 0x00046AFC 28BA020006   */ IL_0374: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x00046B01 23111111111111913F */ IL_0379: ldc.r8    0.016666666666666666
// 	/* 0x00046B0A 28B8020006   */ IL_0382: call      valuetype VectorLF3 VectorLF3::op_Division(valuetype VectorLF3, float64)
// 	/* 0x00046B0F 28BD020006   */ IL_0387: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00046B14 7D201A0004   */ IL_038C: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046B19 0E05         */ IL_0391: ldarg.s   animData
// 	/* 0x00046B1B 2200000000   */ IL_0393: ldc.r4    0.0
// 	/* 0x00046B20 7D35180004   */ IL_0398: stfld     float32 AnimData::power

// 	/* 0x00046B25 0E05         */ IL_039D: ldarg.s   animData
// 	/* 0x00046B27 2200000000   */ IL_039F: ldc.r4    0.0
// 	/* 0x00046B2C 7D31180004   */ IL_03A4: stfld     float32 AnimData::time
// 	/* 0x00046B31 2A           */ IL_03A9: ret

// 	/* 0x00046B32 02           */ IL_03AA: ldarg.0
// 	/* 0x00046B33 7BA7060004   */ IL_03AB: ldfld     int32 DFRelayComponent::stage
// 	/* 0x00046B38 3A9C0B0000   */ IL_03B0: brtrue    IL_0F51

// 	/* 0x00046B3D 05           */ IL_03B5: ldarg.3
// 	/* 0x00046B3E 02           */ IL_03B6: ldarg.0
// 	/* 0x00046B3F 7B9D060004   */ IL_03B7: ldfld     int32 DFRelayComponent::hiveAstroId
// 	/* 0x00046B44 2040420F00   */ IL_03BC: ldc.i4    1000000
// 	/* 0x00046B49 59           */ IL_03C1: sub
// 	/* 0x00046B4A 8FE7010002   */ IL_03C2: ldelema   AstroData
// 	/* 0x00046B4F 130A         */ IL_03C7: stloc.s   V_10
// 	/* 0x00046B51 03           */ IL_03C9: ldarg.1
// 	/* 0x00046B52 08           */ IL_03CA: ldloc.2
// 	/* 0x00046B53 120B         */ IL_03CB: ldloca.s  V_11
// 	/* 0x00046B55 120E         */ IL_03CD: ldloca.s  V_14
// 	/* 0x00046B57 0E04         */ IL_03CF: ldarg.s   enemyData
// 	/* 0x00046B59 7B1E1A0004   */ IL_03D1: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00046B5E 0E04         */ IL_03D6: ldarg.s   enemyData
// 	/* 0x00046B60 7B1F1A0004   */ IL_03D8: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00046B65 6F26190006   */ IL_03DD: callvirt  instance void SpaceSector::TransformFromAstro(int32, valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 	/* 0x00046B6A 0E04         */ IL_03E2: ldarg.s   enemyData
// 	/* 0x00046B6C 7B201A0004   */ IL_03E4: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00046B71 28BC020006   */ IL_03E9: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00046B76 130C         */ IL_03EE: stloc.s   V_12
// 	/* 0x00046B78 120C         */ IL_03F0: ldloca.s  V_12
// 	/* 0x00046B7A 28C9020006   */ IL_03F2: call      instance valuetype VectorLF3 VectorLF3::get_normalized()
// 	/* 0x00046B7F 130F         */ IL_03F7: stloc.s   V_15
// 	/* 0x00046B81 16           */ IL_03F9: ldc.i4.0
// 	/* 0x00046B82 1310         */ IL_03FA: stloc.s   V_16
// 	/* 0x00046B84 0E06         */ IL_03FC: ldarg.s   keyFrame
// 	/* 0x00046B86 395D010000   */ IL_03FE: brfalse   IL_0560

// 	/* 0x00046B8B 02           */ IL_0403: ldarg.0
// 	/* 0x00046B8C 7B9C060004   */ IL_0404: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046B91 7B980D0004   */ IL_0409: ldfld     class GalaxyData EnemyDFHiveSystem::galaxy
// 	/* 0x00046B96 7BC61A0004   */ IL_040E: ldfld     class PlanetFactory[] GalaxyData::astrosFactory
// 	/* 0x00046B9B 02           */ IL_0413: ldarg.0
// 	/* 0x00046B9C 7B9E060004   */ IL_0414: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046BA1 9A           */ IL_0419: ldelem.ref
// 	/* 0x00046BA2 1322         */ IL_041A: stloc.s   V_34
// 	/* 0x00046BA4 02           */ IL_041C: ldarg.0
// 	/* 0x00046BA5 7BA2060004   */ IL_041D: ldfld     int32 DFRelayComponent::baseId
// 	/* 0x00046BAA 3ABE000000   */ IL_0422: brtrue    IL_04E5

// 	/* 0x00046BAF 1122         */ IL_0427: ldloc.s   V_34
// 	/* 0x00046BB1 3932010000   */ IL_0429: brfalse   IL_0560

// 	/* 0x00046BB6 02           */ IL_042E: ldarg.0
// 	/* 0x00046BB7 1122         */ IL_042F: ldloc.s   V_34
// 	/* 0x00046BB9 02           */ IL_0431: ldarg.0
// 	/* 0x00046BBA 7B9F060004   */ IL_0432: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00046BBF 2881070006   */ IL_0437: call      instance bool DFRelayComponent::CheckLandCondition(class PlanetFactory, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00046BC4 3A1F010000   */ IL_043C: brtrue    IL_0560

// 	/* 0x00046BC9 17           */ IL_0441: ldc.i4.1
// 	/* 0x00046BCA 1310         */ IL_0442: stloc.s   V_16
// 	/* 0x00046BCC 110B         */ IL_0444: ldloc.s   V_11
// 	/* 0x00046BCE 1122         */ IL_0446: ldloc.s   V_34
// 	/* 0x00046BD0 6F9E170006   */ IL_0448: callvirt  instance class PlanetData PlanetFactory::get_planet()
// 	/* 0x00046BD5 7BFB1C0004   */ IL_044D: ldfld     valuetype VectorLF3 PlanetData::uPosition
// 	/* 0x00046BDA 28BA020006   */ IL_0452: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x00046BDF 1323         */ IL_0457: stloc.s   V_35
// 	/* 0x00046BE1 1223         */ IL_0459: ldloca.s  V_35
// 	/* 0x00046BE3 28BE020006   */ IL_045B: call      instance float64 VectorLF3::get_sqrMagnitude()
// 	/* 0x00046BE8 230000000000F91541 */ IL_0460: ldc.r8    360000
// 	/* 0x00046BF1 345C         */ IL_0469: bge.un.s  IL_04C7

// 	/* 0x00046BF3 02           */ IL_046B: ldarg.0
// 	/* 0x00046BF4 16           */ IL_046C: ldc.i4.0
// 	/* 0x00046BF5 7D9E060004   */ IL_046D: stfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046BFA 02           */ IL_0472: ldarg.0
// 	/* 0x00046BFB 287800000A   */ IL_0473: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_zero()
// 	/* 0x00046C00 7D9F060004   */ IL_0478: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00046C05 02           */ IL_047D: ldarg.0
// 	/* 0x00046C06 2200000000   */ IL_047E: ldc.r4    0.0
// 	/* 0x00046C0B 7DA0060004   */ IL_0483: stfld     float32 DFRelayComponent::targetYaw
// 	/* 0x00046C10 02           */ IL_0488: ldarg.0
// 	/* 0x00046C11 16           */ IL_0489: ldc.i4.0
// 	/* 0x00046C12 7DA1060004   */ IL_048A: stfld     int32 DFRelayComponent::baseState
// 	/* 0x00046C17 02           */ IL_048F: ldarg.0
// 	/* 0x00046C18 16           */ IL_0490: ldc.i4.0
// 	/* 0x00046C19 7DA2060004   */ IL_0491: stfld     int32 DFRelayComponent::baseId
// 	/* 0x00046C1E 02           */ IL_0496: ldarg.0
// 	/* 0x00046C1F 16           */ IL_0497: ldc.i4.0
// 	/* 0x00046C20 7DA3060004   */ IL_0498: stfld     int32 DFRelayComponent::baseTicks
// 	/* 0x00046C25 02           */ IL_049D: ldarg.0
// 	/* 0x00046C26 7CA4060004   */ IL_049E: ldflda    valuetype EvolveData DFRelayComponent::baseEvolve
// 	/* 0x00046C2B FE1510020002 */ IL_04A3: initobj   EvolveData
// 	/* 0x00046C31 02           */ IL_04A9: ldarg.0
// 	/* 0x00046C32 16           */ IL_04AA: ldc.i4.0
// 	/* 0x00046C33 7DA5060004   */ IL_04AB: stfld     int32 DFRelayComponent::baseRespawnCD
// 	/* 0x00046C38 02           */ IL_04B0: ldarg.0
// 	/* 0x00046C39 15           */ IL_04B1: ldc.i4.m1
// 	/* 0x00046C3A 7DA6060004   */ IL_04B2: stfld     int32 DFRelayComponent::direction
// 	/* 0x00046C3F 02           */ IL_04B7: ldarg.0
// 	/* 0x00046C40 2200000000   */ IL_04B8: ldc.r4    0.0
// 	/* 0x00046C45 7DA9060004   */ IL_04BD: stfld     float32 DFRelayComponent::param0
// 	/* 0x00046C4A 3899000000   */ IL_04C2: br        IL_0560

// 	/* 0x00046C4F 02           */ IL_04C7: ldarg.0
// 	/* 0x00046C50 7B9C060004   */ IL_04C8: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046C55 02           */ IL_04CD: ldarg.0
// 	/* 0x00046C56 7B9E060004   */ IL_04CE: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046C5B 1F64         */ IL_04D3: ldc.i4.s  100
// 	/* 0x00046C5D 6F600C0006   */ IL_04D5: callvirt  instance void EnemyDFHiveSystem::AddPlanetHatred(int32, int32)
// 	/* 0x00046C62 02           */ IL_04DA: ldarg.0
// 	/* 0x00046C63 1122         */ IL_04DB: ldloc.s   V_34
// 	/* 0x00046C65 2882070006   */ IL_04DD: call      instance bool DFRelayComponent::SearchLandPlaceDuringSailing(class PlanetFactory)
// 	/* 0x00046C6A 26           */ IL_04E2: pop
// 	/* 0x00046C6B 2B7B         */ IL_04E3: br.s      IL_0560

// 	/* 0x00046C6D 1122         */ IL_04E5: ldloc.s   V_34
// 	/* 0x00046C6F 2C77         */ IL_04E7: brfalse.s IL_0560

// 	/* 0x00046C71 1122         */ IL_04E9: ldloc.s   V_34
// 	/* 0x00046C73 7B8B1D0004   */ IL_04EB: ldfld     class PlanetATField PlanetFactory::planetATField
// 	/* 0x00046C78 2C6E         */ IL_04F0: brfalse.s IL_0560

// 	/* 0x00046C7A 1122         */ IL_04F2: ldloc.s   V_34
// 	/* 0x00046C7C 7B8B1D0004   */ IL_04F4: ldfld     class PlanetATField PlanetFactory::planetATField
// 	/* 0x00046C81 02           */ IL_04F9: ldarg.0
// 	/* 0x00046C82 7B9F060004   */ IL_04FA: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00046C87 6F890D0006   */ IL_04FF: callvirt  instance bool PlanetATField::TestRelayCondition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00046C8C 2D5A         */ IL_0504: brtrue.s  IL_0560

// 	/* 0x00046C8E 17           */ IL_0506: ldc.i4.1
// 	/* 0x00046C8F 1310         */ IL_0507: stloc.s   V_16
// 	/* 0x00046C91 02           */ IL_0509: ldarg.0
// 	/* 0x00046C92 16           */ IL_050A: ldc.i4.0
// 	/* 0x00046C93 7D9E060004   */ IL_050B: stfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046C98 02           */ IL_0510: ldarg.0
// 	/* 0x00046C99 287800000A   */ IL_0511: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_zero()
// 	/* 0x00046C9E 7D9F060004   */ IL_0516: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00046CA3 02           */ IL_051B: ldarg.0
// 	/* 0x00046CA4 2200000000   */ IL_051C: ldc.r4    0.0
// 	/* 0x00046CA9 7DA0060004   */ IL_0521: stfld     float32 DFRelayComponent::targetYaw
// 	/* 0x00046CAE 02           */ IL_0526: ldarg.0
// 	/* 0x00046CAF 16           */ IL_0527: ldc.i4.0
// 	/* 0x00046CB0 7DA1060004   */ IL_0528: stfld     int32 DFRelayComponent::baseState
// 	/* 0x00046CB5 02           */ IL_052D: ldarg.0
// 	/* 0x00046CB6 16           */ IL_052E: ldc.i4.0
// 	/* 0x00046CB7 7DA2060004   */ IL_052F: stfld     int32 DFRelayComponent::baseId
// 	/* 0x00046CBC 02           */ IL_0534: ldarg.0
// 	/* 0x00046CBD 16           */ IL_0535: ldc.i4.0
// 	/* 0x00046CBE 7DA3060004   */ IL_0536: stfld     int32 DFRelayComponent::baseTicks
// 	/* 0x00046CC3 02           */ IL_053B: ldarg.0
// 	/* 0x00046CC4 7CA4060004   */ IL_053C: ldflda    valuetype EvolveData DFRelayComponent::baseEvolve
// 	/* 0x00046CC9 FE1510020002 */ IL_0541: initobj   EvolveData
// 	/* 0x00046CCF 02           */ IL_0547: ldarg.0
// 	/* 0x00046CD0 16           */ IL_0548: ldc.i4.0
// 	/* 0x00046CD1 7DA5060004   */ IL_0549: stfld     int32 DFRelayComponent::baseRespawnCD
// 	/* 0x00046CD6 02           */ IL_054E: ldarg.0
// 	/* 0x00046CD7 15           */ IL_054F: ldc.i4.m1
// 	/* 0x00046CD8 7DA6060004   */ IL_0550: stfld     int32 DFRelayComponent::direction
// 	/* 0x00046CDD 02           */ IL_0555: ldarg.0
// 	/* 0x00046CDE 2200000000   */ IL_0556: ldc.r4    0.0
// 	/* 0x00046CE3 7DA9060004   */ IL_055B: stfld     float32 DFRelayComponent::param0

// 	/* 0x00046CE8 02           */ IL_0560: ldarg.0
// 	/* 0x00046CE9 7BA6060004   */ IL_0561: ldfld     int32 DFRelayComponent::direction
// 	/* 0x00046CEE 16           */ IL_0566: ldc.i4.0
// 	/* 0x00046CEF 312B         */ IL_0567: ble.s     IL_0594

// 	/* 0x00046CF1 04           */ IL_0569: ldarg.2
// 	/* 0x00046CF2 02           */ IL_056A: ldarg.0
// 	/* 0x00046CF3 7B9E060004   */ IL_056B: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046CF8 8FE7010002   */ IL_0570: ldelema   AstroData
// 	/* 0x00046CFD 1324         */ IL_0575: stloc.s   V_36
// 	/* 0x00046CFF 1124         */ IL_0577: ldloc.s   V_36
// 	/* 0x00046D01 7C46180004   */ IL_0579: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x00046D06 1124         */ IL_057E: ldloc.s   V_36
// 	/* 0x00046D08 7C44180004   */ IL_0580: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 	/* 0x00046D0D 02           */ IL_0585: ldarg.0
// 	/* 0x00046D0E 7C9F060004   */ IL_0586: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00046D13 120D         */ IL_058B: ldloca.s  V_13
// 	/* 0x00046D15 2883070006   */ IL_058D: call      void DFRelayComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype VectorLF3&)
// 	/* 0x00046D1A 2B3D         */ IL_0592: br.s      IL_05D1

// 	/* 0x00046D1C 02           */ IL_0594: ldarg.0
// 	/* 0x00046D1D 7C9B060004   */ IL_0595: ldflda    valuetype DFDock DFRelayComponent::dock
// 	/* 0x00046D22 7BD8060004   */ IL_059A: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFDock::pos
// 	/* 0x00046D27 2200000000   */ IL_059F: ldc.r4    0.0
// 	/* 0x00046D2C 2200004843   */ IL_05A4: ldc.r4    200
// 	/* 0x00046D31 2200000000   */ IL_05A9: ldc.r4    0.0
// 	/* 0x00046D36 733B00000A   */ IL_05AE: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Vector3::.ctor(float32, float32, float32)
// 	/* 0x00046D3B 287900000A   */ IL_05B3: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Subtraction(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00046D40 1325         */ IL_05B8: stloc.s   V_37
// 	/* 0x00046D42 110A         */ IL_05BA: ldloc.s   V_10
// 	/* 0x00046D44 7C46180004   */ IL_05BC: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x00046D49 110A         */ IL_05C1: ldloc.s   V_10
// 	/* 0x00046D4B 7C44180004   */ IL_05C3: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 	/* 0x00046D50 1225         */ IL_05C8: ldloca.s  V_37
// 	/* 0x00046D52 120D         */ IL_05CA: ldloca.s  V_13
// 	/* 0x00046D54 2883070006   */ IL_05CC: call      void DFRelayComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype VectorLF3&)

// 	/* 0x00046D59 1211         */ IL_05D1: ldloca.s  V_17
// 	/* 0x00046D5B 110D         */ IL_05D3: ldloc.s   V_13
// 	/* 0x00046D5D 7B41030004   */ IL_05D5: ldfld     float64 VectorLF3::x
// 	/* 0x00046D62 110B         */ IL_05DA: ldloc.s   V_11
// 	/* 0x00046D64 7B41030004   */ IL_05DC: ldfld     float64 VectorLF3::x
// 	/* 0x00046D69 59           */ IL_05E1: sub
// 	/* 0x00046D6A 7D41030004   */ IL_05E2: stfld     float64 VectorLF3::x
// 	/* 0x00046D6F 1211         */ IL_05E7: ldloca.s  V_17
// 	/* 0x00046D71 110D         */ IL_05E9: ldloc.s   V_13
// 	/* 0x00046D73 7B42030004   */ IL_05EB: ldfld     float64 VectorLF3::y
// 	/* 0x00046D78 110B         */ IL_05F0: ldloc.s   V_11
// 	/* 0x00046D7A 7B42030004   */ IL_05F2: ldfld     float64 VectorLF3::y
// 	/* 0x00046D7F 59           */ IL_05F7: sub
// 	/* 0x00046D80 7D42030004   */ IL_05F8: stfld     float64 VectorLF3::y
// 	/* 0x00046D85 1211         */ IL_05FD: ldloca.s  V_17
// 	/* 0x00046D87 110D         */ IL_05FF: ldloc.s   V_13
// 	/* 0x00046D89 7B43030004   */ IL_0601: ldfld     float64 VectorLF3::z
// 	/* 0x00046D8E 110B         */ IL_0606: ldloc.s   V_11
// 	/* 0x00046D90 7B43030004   */ IL_0608: ldfld     float64 VectorLF3::z
// 	/* 0x00046D95 59           */ IL_060D: sub
// 	/* 0x00046D96 7D43030004   */ IL_060E: stfld     float64 VectorLF3::z
// 	/* 0x00046D9B 1111         */ IL_0613: ldloc.s   V_17
// 	/* 0x00046D9D 7B41030004   */ IL_0615: ldfld     float64 VectorLF3::x
// 	/* 0x00046DA2 1111         */ IL_061A: ldloc.s   V_17
// 	/* 0x00046DA4 7B41030004   */ IL_061C: ldfld     float64 VectorLF3::x
// 	/* 0x00046DA9 5A           */ IL_0621: mul
// 	/* 0x00046DAA 1111         */ IL_0622: ldloc.s   V_17
// 	/* 0x00046DAC 7B42030004   */ IL_0624: ldfld     float64 VectorLF3::y
// 	/* 0x00046DB1 1111         */ IL_0629: ldloc.s   V_17
// 	/* 0x00046DB3 7B42030004   */ IL_062B: ldfld     float64 VectorLF3::y
// 	/* 0x00046DB8 5A           */ IL_0630: mul
// 	/* 0x00046DB9 58           */ IL_0631: add
// 	/* 0x00046DBA 1111         */ IL_0632: ldloc.s   V_17
// 	/* 0x00046DBC 7B43030004   */ IL_0634: ldfld     float64 VectorLF3::z
// 	/* 0x00046DC1 1111         */ IL_0639: ldloc.s   V_17
// 	/* 0x00046DC3 7B43030004   */ IL_063B: ldfld     float64 VectorLF3::z
// 	/* 0x00046DC8 5A           */ IL_0640: mul
// 	/* 0x00046DC9 58           */ IL_0641: add
// 	/* 0x00046DCA 284B02000A   */ IL_0642: call      float64 [netstandard]System.Math::Sqrt(float64)
// 	/* 0x00046DCF 1312         */ IL_0647: stloc.s   V_18
// 	/* 0x00046DD1 0E06         */ IL_0649: ldarg.s   keyFrame
// 	/* 0x00046DD3 2C7C         */ IL_064B: brfalse.s IL_06C9

// 	/* 0x00046DD5 1110         */ IL_064D: ldloc.s   V_16
// 	/* 0x00046DD7 2D78         */ IL_064F: brtrue.s  IL_06C9

// 	/* 0x00046DD9 1112         */ IL_0651: ldloc.s   V_18
// 	/* 0x00046DDB 2300000000004CDD40 */ IL_0653: ldc.r8    30000
// 	/* 0x00046DE4 346B         */ IL_065C: bge.un.s  IL_06C9

// 	/* 0x00046DE6 02           */ IL_065E: ldarg.0
// 	/* 0x00046DE7 7BA6060004   */ IL_065F: ldfld     int32 DFRelayComponent::direction
// 	/* 0x00046DEC 17           */ IL_0664: ldc.i4.1
// 	/* 0x00046DED 3362         */ IL_0665: bne.un.s  IL_06C9

// 	/* 0x00046DEF 02           */ IL_0667: ldarg.0
// 	/* 0x00046DF0 7B9E060004   */ IL_0668: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046DF5 16           */ IL_066D: ldc.i4.0
// 	/* 0x00046DF6 3159         */ IL_066E: ble.s     IL_06C9

// 	/* 0x00046DF8 02           */ IL_0670: ldarg.0
// 	/* 0x00046DF9 7B9C060004   */ IL_0671: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046DFE 7B980D0004   */ IL_0676: ldfld     class GalaxyData EnemyDFHiveSystem::galaxy
// 	/* 0x00046E03 7BC61A0004   */ IL_067B: ldfld     class PlanetFactory[] GalaxyData::astrosFactory
// 	/* 0x00046E08 02           */ IL_0680: ldarg.0
// 	/* 0x00046E09 7B9E060004   */ IL_0681: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046E0E 9A           */ IL_0686: ldelem.ref
// 	/* 0x00046E0F 1326         */ IL_0687: stloc.s   V_38
// 	/* 0x00046E11 1126         */ IL_0689: ldloc.s   V_38
// 	/* 0x00046E13 2C3C         */ IL_068B: brfalse.s IL_06C9

// 	/* 0x00046E15 1126         */ IL_068D: ldloc.s   V_38
// 	/* 0x00046E17 6FA2170006   */ IL_068F: callvirt  instance int32 PlanetFactory::get_entityCount()
// 	/* 0x00046E1C 1126         */ IL_0694: ldloc.s   V_38
// 	/* 0x00046E1E 6FA3170006   */ IL_0696: callvirt  instance int32 PlanetFactory::get_prebuildCount()
// 	/* 0x00046E23 58           */ IL_069B: add
// 	/* 0x00046E24 16           */ IL_069C: ldc.i4.0
// 	/* 0x00046E25 312A         */ IL_069D: ble.s     IL_06C9

// 	/* 0x00046E27 02           */ IL_069F: ldarg.0
// 	/* 0x00046E28 7B9C060004   */ IL_06A0: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046E2D 7B950D0004   */ IL_06A5: ldfld     class GameData EnemyDFHiveSystem::gameData
// 	/* 0x00046E32 7BFB1A0004   */ IL_06AA: ldfld     class WarningSystem GameData::warningSystem
// 	/* 0x00046E37 1F0B         */ IL_06AF: ldc.i4.s  11
// 	/* 0x00046E39 1126         */ IL_06B1: ldloc.s   V_38
// 	/* 0x00046E3B 6F9A170006   */ IL_06B3: callvirt  instance int32 PlanetFactory::get_index()
// 	/* 0x00046E40 02           */ IL_06B8: ldarg.0
// 	/* 0x00046E41 7B9E060004   */ IL_06B9: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00046E46 02           */ IL_06BE: ldarg.0
// 	/* 0x00046E47 7B98060004   */ IL_06BF: ldfld     int32 DFRelayComponent::enemyId
// 	/* 0x00046E4C 6F8C0E0006   */ IL_06C4: callvirt  instance void WarningSystem::Broadcast(valuetype EBroadcastVocal, int32, int32, int32)

// 	/* 0x00046E51 16           */ IL_06C9: ldc.i4.0
// 	/* 0x00046E52 1313         */ IL_06CA: stloc.s   V_19
// 	/* 0x00046E54 1112         */ IL_06CC: ldloc.s   V_18
// 	/* 0x00046E56 230000000000001840 */ IL_06CE: ldc.r8    6
// 	/* 0x00046E5F 340F         */ IL_06D7: bge.un.s  IL_06E8

// 	/* 0x00046E61 02           */ IL_06D9: ldarg.0
// 	/* 0x00046E62 02           */ IL_06DA: ldarg.0
// 	/* 0x00046E63 7BA6060004   */ IL_06DB: ldfld     int32 DFRelayComponent::direction
// 	/* 0x00046E68 7DA7060004   */ IL_06E0: stfld     int32 DFRelayComponent::stage
// 	/* 0x00046E6D 17           */ IL_06E5: ldc.i4.1
// 	/* 0x00046E6E 1313         */ IL_06E6: stloc.s   V_19

// 	/* 0x00046E70 1112         */ IL_06E8: ldloc.s   V_18
// 	/* 0x00046E72 02           */ IL_06EA: ldarg.0
// 	/* 0x00046E73 7BA8060004   */ IL_06EB: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046E78 6C           */ IL_06F0: conv.r8
// 	/* 0x00046E79 239A9999999999B93F */ IL_06F1: ldc.r8    0.1
// 	/* 0x00046E82 58           */ IL_06FA: add
// 	/* 0x00046E83 5B           */ IL_06FB: div
// 	/* 0x00046E84 23A69BC420B072D83F */ IL_06FC: ldc.r8    0.382
// 	/* 0x00046E8D 5A           */ IL_0705: mul
// 	/* 0x00046E8E 1314         */ IL_0706: stloc.s   V_20
// 	/* 0x00046E90 2200000000   */ IL_0708: ldc.r4    0.0
// 	/* 0x00046E95 1315         */ IL_070D: stloc.s   V_21
// 	/* 0x00046E97 02           */ IL_070F: ldarg.0
// 	/* 0x00046E98 7BA8060004   */ IL_0710: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046E9D 6C           */ IL_0715: conv.r8
// 	/* 0x00046E9E 1114         */ IL_0716: ldloc.s   V_20
// 	/* 0x00046EA0 5A           */ IL_0718: mul
// 	/* 0x00046EA1 6B           */ IL_0719: conv.r4
// 	/* 0x00046EA2 220000F041   */ IL_071A: ldc.r4    30
// 	/* 0x00046EA7 58           */ IL_071F: add
// 	/* 0x00046EA8 1316         */ IL_0720: stloc.s   V_22
// 	/* 0x00046EAA 1116         */ IL_0722: ldloc.s   V_22
// 	/* 0x00046EAC 2200007A44   */ IL_0724: ldc.r4    1000
// 	/* 0x00046EB1 3607         */ IL_0729: ble.un.s  IL_0732

// 	/* 0x00046EB3 2200007A44   */ IL_072B: ldc.r4    1000
// 	/* 0x00046EB8 1316         */ IL_0730: stloc.s   V_22

// 	/* 0x00046EBA 228988883C   */ IL_0732: ldc.r4    0.016666668
// 	/* 0x00046EBF 06           */ IL_0737: ldloc.0
// 	/* 0x00046EC0 5A           */ IL_0738: mul
// 	/* 0x00046EC1 0A           */ IL_0739: stloc.0
// 	/* 0x00046EC2 02           */ IL_073A: ldarg.0
// 	/* 0x00046EC3 7BA8060004   */ IL_073B: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046EC8 1116         */ IL_0740: ldloc.s   V_22
// 	/* 0x00046ECA 06           */ IL_0742: ldloc.0
// 	/* 0x00046ECB 59           */ IL_0743: sub
// 	/* 0x00046ECC 3410         */ IL_0744: bge.un.s  IL_0756

// 	/* 0x00046ECE 02           */ IL_0746: ldarg.0
// 	/* 0x00046ECF 02           */ IL_0747: ldarg.0
// 	/* 0x00046ED0 7BA8060004   */ IL_0748: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046ED5 06           */ IL_074D: ldloc.0
// 	/* 0x00046ED6 58           */ IL_074E: add
// 	/* 0x00046ED7 7DA8060004   */ IL_074F: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046EDC 2B24         */ IL_0754: br.s      IL_077A

// 	/* 0x00046EDE 02           */ IL_0756: ldarg.0
// 	/* 0x00046EDF 7BA8060004   */ IL_0757: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046EE4 1116         */ IL_075C: ldloc.s   V_22
// 	/* 0x00046EE6 07           */ IL_075E: ldloc.1
// 	/* 0x00046EE7 58           */ IL_075F: add
// 	/* 0x00046EE8 3610         */ IL_0760: ble.un.s  IL_0772

// 	/* 0x00046EEA 02           */ IL_0762: ldarg.0
// 	/* 0x00046EEB 02           */ IL_0763: ldarg.0
// 	/* 0x00046EEC 7BA8060004   */ IL_0764: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046EF1 07           */ IL_0769: ldloc.1
// 	/* 0x00046EF2 59           */ IL_076A: sub
// 	/* 0x00046EF3 7DA8060004   */ IL_076B: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046EF8 2B08         */ IL_0770: br.s      IL_077A

// 	/* 0x00046EFA 02           */ IL_0772: ldarg.0
// 	/* 0x00046EFB 1116         */ IL_0773: ldloc.s   V_22
// 	/* 0x00046EFD 7DA8060004   */ IL_0775: stfld     float32 DFRelayComponent::uSpeed

// 	/* 0x00046F02 02           */ IL_077A: ldarg.0
// 	/* 0x00046F03 7BA8060004   */ IL_077B: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00046F08 1315         */ IL_0780: stloc.s   V_21
// 	/* 0x00046F0A 15           */ IL_0782: ldc.i4.m1
// 	/* 0x00046F0B 1317         */ IL_0783: stloc.s   V_23
// 	/* 0x00046F0D 230000000000000000 */ IL_0785: ldc.r8    0.0
// 	/* 0x00046F16 1318         */ IL_078E: stloc.s   V_24
// 	/* 0x00046F18 23A55CC3F129633D48 */ IL_0790: ldc.r8    1E+40
// 	/* 0x00046F21 1319         */ IL_0799: stloc.s   V_25
// 	/* 0x00046F23 02           */ IL_079B: ldarg.0
// 	/* 0x00046F24 7B9C060004   */ IL_079C: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x00046F29 7B990D0004   */ IL_07A1: ldfld     class StarData EnemyDFHiveSystem::starData
// 	/* 0x00046F2E 7B3D200004   */ IL_07A6: ldfld     int32 StarData::planetCount
// 	/* 0x00046F33 131A         */ IL_07AB: stloc.s   V_26
// 	/* 0x00046F35 2200409C45   */ IL_07AD: ldc.r4    5000
// 	/* 0x00046F3A 1115         */ IL_07B2: ldloc.s   V_21
// 	/* 0x00046F3C 58           */ IL_07B4: add
// 	/* 0x00046F3D 131B         */ IL_07B5: stloc.s   V_27
// 	/* 0x00046F3F 08           */ IL_07B7: ldloc.2
// 	/* 0x00046F40 1327         */ IL_07B8: stloc.s   V_39
// 	/* 0x00046F42 38F2000000   */ IL_07BA: br        IL_08B1
// 	// loop start (head: IL_08B1)
// 		/* 0x00046F47 04           */ IL_07BF: ldarg.2
// 		/* 0x00046F48 1127         */ IL_07C0: ldloc.s   V_39
// 		/* 0x00046F4A 8FE7010002   */ IL_07C2: ldelema   AstroData
// 		/* 0x00046F4F 7B43180004   */ IL_07C7: ldfld     float32 AstroData::uRadius
// 		/* 0x00046F54 1328         */ IL_07CC: stloc.s   V_40
// 		/* 0x00046F56 1128         */ IL_07CE: ldloc.s   V_40
// 		/* 0x00046F58 220000803F   */ IL_07D0: ldc.r4    1
// 		/* 0x00046F5D 3FD1000000   */ IL_07D5: blt       IL_08AB

// 		/* 0x00046F62 1128         */ IL_07DA: ldloc.s   V_40
// 		/* 0x00046F64 111B         */ IL_07DC: ldloc.s   V_27
// 		/* 0x00046F66 58           */ IL_07DE: add
// 		/* 0x00046F67 1329         */ IL_07DF: stloc.s   V_41
// 		/* 0x00046F69 110B         */ IL_07E1: ldloc.s   V_11
// 		/* 0x00046F6B 04           */ IL_07E3: ldarg.2
// 		/* 0x00046F6C 1127         */ IL_07E4: ldloc.s   V_39
// 		/* 0x00046F6E 8FE7010002   */ IL_07E6: ldelema   AstroData
// 		/* 0x00046F73 7B46180004   */ IL_07EB: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00046F78 28BA020006   */ IL_07F0: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00046F7D 132A         */ IL_07F5: stloc.s   V_42
// 		/* 0x00046F7F 112A         */ IL_07F7: ldloc.s   V_42
// 		/* 0x00046F81 7B41030004   */ IL_07F9: ldfld     float64 VectorLF3::x
// 		/* 0x00046F86 112A         */ IL_07FE: ldloc.s   V_42
// 		/* 0x00046F88 7B41030004   */ IL_0800: ldfld     float64 VectorLF3::x
// 		/* 0x00046F8D 5A           */ IL_0805: mul
// 		/* 0x00046F8E 112A         */ IL_0806: ldloc.s   V_42
// 		/* 0x00046F90 7B42030004   */ IL_0808: ldfld     float64 VectorLF3::y
// 		/* 0x00046F95 112A         */ IL_080D: ldloc.s   V_42
// 		/* 0x00046F97 7B42030004   */ IL_080F: ldfld     float64 VectorLF3::y
// 		/* 0x00046F9C 5A           */ IL_0814: mul
// 		/* 0x00046F9D 58           */ IL_0815: add
// 		/* 0x00046F9E 112A         */ IL_0816: ldloc.s   V_42
// 		/* 0x00046FA0 7B43030004   */ IL_0818: ldfld     float64 VectorLF3::z
// 		/* 0x00046FA5 112A         */ IL_081D: ldloc.s   V_42
// 		/* 0x00046FA7 7B43030004   */ IL_081F: ldfld     float64 VectorLF3::z
// 		/* 0x00046FAC 5A           */ IL_0824: mul
// 		/* 0x00046FAD 58           */ IL_0825: add
// 		/* 0x00046FAE 132B         */ IL_0826: stloc.s   V_43
// 		/* 0x00046FB0 110F         */ IL_0828: ldloc.s   V_15
// 		/* 0x00046FB2 7B41030004   */ IL_082A: ldfld     float64 VectorLF3::x
// 		/* 0x00046FB7 112A         */ IL_082F: ldloc.s   V_42
// 		/* 0x00046FB9 7B41030004   */ IL_0831: ldfld     float64 VectorLF3::x
// 		/* 0x00046FBE 5A           */ IL_0836: mul
// 		/* 0x00046FBF 110F         */ IL_0837: ldloc.s   V_15
// 		/* 0x00046FC1 7B42030004   */ IL_0839: ldfld     float64 VectorLF3::y
// 		/* 0x00046FC6 112A         */ IL_083E: ldloc.s   V_42
// 		/* 0x00046FC8 7B42030004   */ IL_0840: ldfld     float64 VectorLF3::y
// 		/* 0x00046FCD 5A           */ IL_0845: mul
// 		/* 0x00046FCE 58           */ IL_0846: add
// 		/* 0x00046FCF 110F         */ IL_0847: ldloc.s   V_15
// 		/* 0x00046FD1 7B43030004   */ IL_0849: ldfld     float64 VectorLF3::z
// 		/* 0x00046FD6 112A         */ IL_084E: ldloc.s   V_42
// 		/* 0x00046FD8 7B43030004   */ IL_0850: ldfld     float64 VectorLF3::z
// 		/* 0x00046FDD 5A           */ IL_0855: mul
// 		/* 0x00046FDE 58           */ IL_0856: add
// 		/* 0x00046FDF 65           */ IL_0857: neg
// 		/* 0x00046FE0 132C         */ IL_0858: stloc.s   V_44
// 		/* 0x00046FE2 112C         */ IL_085A: ldloc.s   V_44
// 		/* 0x00046FE4 230000000000000000 */ IL_085C: ldc.r8    0.0
// 		/* 0x00046FED 3010         */ IL_0865: bgt.s     IL_0877

// 		/* 0x00046FEF 112B         */ IL_0867: ldloc.s   V_43
// 		/* 0x00046FF1 1128         */ IL_0869: ldloc.s   V_40
// 		/* 0x00046FF3 1128         */ IL_086B: ldloc.s   V_40
// 		/* 0x00046FF5 5A           */ IL_086D: mul
// 		/* 0x00046FF6 220000E040   */ IL_086E: ldc.r4    7
// 		/* 0x00046FFB 5A           */ IL_0873: mul
// 		/* 0x00046FFC 6C           */ IL_0874: conv.r8
// 		/* 0x00046FFD 3434         */ IL_0875: bge.un.s  IL_08AB

// 		/* 0x00046FFF 112B         */ IL_0877: ldloc.s   V_43
// 		/* 0x00047001 1119         */ IL_0879: ldloc.s   V_25
// 		/* 0x00047003 342E         */ IL_087B: bge.un.s  IL_08AB

// 		/* 0x00047005 112B         */ IL_087D: ldloc.s   V_43
// 		/* 0x00047007 1129         */ IL_087F: ldloc.s   V_41
// 		/* 0x00047009 1129         */ IL_0881: ldloc.s   V_41
// 		/* 0x0004700B 5A           */ IL_0883: mul
// 		/* 0x0004700C 6C           */ IL_0884: conv.r8
// 		/* 0x0004700D 3424         */ IL_0885: bge.un.s  IL_08AB

// 		/* 0x0004700F 112C         */ IL_0887: ldloc.s   V_44
// 		/* 0x00047011 230000000000000000 */ IL_0889: ldc.r8    0.0
// 		/* 0x0004701A 3204         */ IL_0892: blt.s     IL_0898

// 		/* 0x0004701C 112C         */ IL_0894: ldloc.s   V_44
// 		/* 0x0004701E 2B09         */ IL_0896: br.s      IL_08A1

// 		/* 0x00047020 230000000000000000 */ IL_0898: ldc.r8    0.0

// 		/* 0x00047029 1318         */ IL_08A1: stloc.s   V_24
// 		/* 0x0004702B 1127         */ IL_08A3: ldloc.s   V_39
// 		/* 0x0004702D 1317         */ IL_08A5: stloc.s   V_23
// 		/* 0x0004702F 112B         */ IL_08A7: ldloc.s   V_43
// 		/* 0x00047031 1319         */ IL_08A9: stloc.s   V_25

// 		/* 0x00047033 1127         */ IL_08AB: ldloc.s   V_39
// 		/* 0x00047035 17           */ IL_08AD: ldc.i4.1
// 		/* 0x00047036 58           */ IL_08AE: add
// 		/* 0x00047037 1327         */ IL_08AF: stloc.s   V_39

// 		/* 0x00047039 1127         */ IL_08B1: ldloc.s   V_39
// 		/* 0x0004703B 08           */ IL_08B3: ldloc.2
// 		/* 0x0004703C 111A         */ IL_08B4: ldloc.s   V_26
// 		/* 0x0004703E 58           */ IL_08B6: add
// 		/* 0x0004703F 3E03FFFFFF   */ IL_08B7: ble       IL_07BF
// 	// end loop

// 	/* 0x00047044 2200000000   */ IL_08BC: ldc.r4    0.0
// 	/* 0x00047049 2200000000   */ IL_08C1: ldc.r4    0.0
// 	/* 0x0004704E 2200000000   */ IL_08C6: ldc.r4    0.0
// 	/* 0x00047053 73B3020006   */ IL_08CB: newobj    instance void VectorLF3::.ctor(float32, float32, float32)
// 	/* 0x00047058 28BD020006   */ IL_08D0: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x0004705D 131C         */ IL_08D5: stloc.s   V_28
// 	/* 0x0004705F 121D         */ IL_08D7: ldloca.s  V_29
// 	/* 0x00047061 2200000000   */ IL_08D9: ldc.r4    0.0
// 	/* 0x00047066 2200000000   */ IL_08DE: ldc.r4    0.0
// 	/* 0x0004706B 2200000000   */ IL_08E3: ldc.r4    0.0
// 	/* 0x00047070 28B3020006   */ IL_08E8: call      instance void VectorLF3::.ctor(float32, float32, float32)
// 	/* 0x00047075 230000000000000000 */ IL_08ED: ldc.r8    0.0
// 	/* 0x0004707E 131E         */ IL_08F6: stloc.s   V_30
// 	/* 0x00047080 1117         */ IL_08F8: ldloc.s   V_23
// 	/* 0x00047082 16           */ IL_08FA: ldc.i4.0
// 	/* 0x00047083 3E12030000   */ IL_08FB: ble       IL_0C12

// 	/* 0x00047088 04           */ IL_0900: ldarg.2
// 	/* 0x00047089 1117         */ IL_0901: ldloc.s   V_23
// 	/* 0x0004708B 8FE7010002   */ IL_0903: ldelema   AstroData
// 	/* 0x00047090 132D         */ IL_0908: stloc.s   V_45
// 	/* 0x00047092 112D         */ IL_090A: ldloc.s   V_45
// 	/* 0x00047094 7B43180004   */ IL_090C: ldfld     float32 AstroData::uRadius
// 	/* 0x00047099 132E         */ IL_0911: stloc.s   V_46
// 	/* 0x0004709B 1117         */ IL_0913: ldloc.s   V_23
// 	/* 0x0004709D 1F64         */ IL_0915: ldc.i4.s  100
// 	/* 0x0004709F 5D           */ IL_0917: rem
// 	/* 0x000470A0 2D0A         */ IL_0918: brtrue.s  IL_0924

// 	/* 0x000470A2 112E         */ IL_091A: ldloc.s   V_46
// 	/* 0x000470A4 2200002040   */ IL_091C: ldc.r4    2.5
// 	/* 0x000470A9 5A           */ IL_0921: mul
// 	/* 0x000470AA 132E         */ IL_0922: stloc.s   V_46

// 	/* 0x000470AC 23000000000000F03F */ IL_0924: ldc.r8    1
// 	/* 0x000470B5 112D         */ IL_092D: ldloc.s   V_45
// 	/* 0x000470B7 7B47180004   */ IL_092F: ldfld     valuetype VectorLF3 AstroData::uPosNext
// 	/* 0x000470BC 112D         */ IL_0934: ldloc.s   V_45
// 	/* 0x000470BE 7B46180004   */ IL_0936: ldfld     valuetype VectorLF3 AstroData::uPos
// 	/* 0x000470C3 28BA020006   */ IL_093B: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000470C8 1323         */ IL_0940: stloc.s   V_35
// 	/* 0x000470CA 1223         */ IL_0942: ldloca.s  V_35
// 	/* 0x000470CC 28BF020006   */ IL_0944: call      instance float64 VectorLF3::get_magnitude()
// 	/* 0x000470D1 23000000000000E03F */ IL_0949: ldc.r8    0.5
// 	/* 0x000470DA 59           */ IL_0952: sub
// 	/* 0x000470DB 23333333333333E33F */ IL_0953: ldc.r8    0.6
// 	/* 0x000470E4 5A           */ IL_095C: mul
// 	/* 0x000470E5 285503000A   */ IL_095D: call      float64 [netstandard]System.Math::Max(float64, float64)
// 	/* 0x000470EA 132F         */ IL_0962: stloc.s   V_47
// 	/* 0x000470EC 23000000000000F03F */ IL_0964: ldc.r8    1
// 	/* 0x000470F5 230000000000009940 */ IL_096D: ldc.r8    1600
// 	/* 0x000470FE 112E         */ IL_0976: ldloc.s   V_46
// 	/* 0x00047100 6C           */ IL_0978: conv.r8
// 	/* 0x00047101 5B           */ IL_0979: div
// 	/* 0x00047102 58           */ IL_097A: add
// 	/* 0x00047103 1330         */ IL_097B: stloc.s   V_48
// 	/* 0x00047105 23000000000000F03F */ IL_097D: ldc.r8    1
// 	/* 0x0004710E 230000000000406F40 */ IL_0986: ldc.r8    250
// 	/* 0x00047117 112E         */ IL_098F: ldloc.s   V_46
// 	/* 0x00047119 6C           */ IL_0991: conv.r8
// 	/* 0x0004711A 5B           */ IL_0992: div
// 	/* 0x0004711B 58           */ IL_0993: add
// 	/* 0x0004711C 1331         */ IL_0994: stloc.s   V_49
// 	/* 0x0004711E 1130         */ IL_0996: ldloc.s   V_48
// 	/* 0x00047120 112F         */ IL_0998: ldloc.s   V_47
// 	/* 0x00047122 112F         */ IL_099A: ldloc.s   V_47
// 	/* 0x00047124 5A           */ IL_099C: mul
// 	/* 0x00047125 5A           */ IL_099D: mul
// 	/* 0x00047126 1330         */ IL_099E: stloc.s   V_48
// 	/* 0x00047128 1117         */ IL_09A0: ldloc.s   V_23
// 	/* 0x0004712A 02           */ IL_09A2: ldarg.0
// 	/* 0x0004712B 7B9E060004   */ IL_09A3: ldfld     int32 DFRelayComponent::targetAstroId
// 	/* 0x00047130 2E07         */ IL_09A8: beq.s     IL_09B1

// 	/* 0x00047132 2200000040   */ IL_09AA: ldc.r4    2
// 	/* 0x00047137 2B05         */ IL_09AF: br.s      IL_09B6

// 	/* 0x00047139 229A99D93F   */ IL_09B1: ldc.r4    1.7

// 	/* 0x0004713E 6C           */ IL_09B6: conv.r8
// 	/* 0x0004713F 1332         */ IL_09B7: stloc.s   V_50
// 	/* 0x00047141 1119         */ IL_09B9: ldloc.s   V_25
// 	/* 0x00047143 284B02000A   */ IL_09BB: call      float64 [netstandard]System.Math::Sqrt(float64)
// 	/* 0x00047148 25           */ IL_09C0: dup
// 	/* 0x00047149 112E         */ IL_09C1: ldloc.s   V_46
// 	/* 0x0004714B 6C           */ IL_09C3: conv.r8
// 	/* 0x0004714C 233D0AD7A3703DEA3F */ IL_09C4: ldc.r8    0.82
// 	/* 0x00047155 5A           */ IL_09CD: mul
// 	/* 0x00047156 59           */ IL_09CE: sub
// 	/* 0x00047157 1333         */ IL_09CF: stloc.s   V_51
// 	/* 0x00047159 1133         */ IL_09D1: ldloc.s   V_51
// 	/* 0x0004715B 23000000000000F03F */ IL_09D3: ldc.r8    1
// 	/* 0x00047164 340B         */ IL_09DC: bge.un.s  IL_09E9

// 	/* 0x00047166 23000000000000F03F */ IL_09DE: ldc.r8    1
// 	/* 0x0004716F 1333         */ IL_09E7: stloc.s   V_51

// 	/* 0x00047171 1115         */ IL_09E9: ldloc.s   V_21
// 	/* 0x00047173 220000C040   */ IL_09EB: ldc.r4    6
// 	/* 0x00047178 59           */ IL_09F0: sub
// 	/* 0x00047179 6C           */ IL_09F1: conv.r8
// 	/* 0x0004717A 1133         */ IL_09F2: ldloc.s   V_51
// 	/* 0x0004717C 5B           */ IL_09F4: div
// 	/* 0x0004717D 23333333333333E33F */ IL_09F5: ldc.r8    0.6
// 	/* 0x00047186 5A           */ IL_09FE: mul
// 	/* 0x00047187 237B14AE47E17A843F */ IL_09FF: ldc.r8    0.01
// 	/* 0x00047190 59           */ IL_0A08: sub
// 	/* 0x00047191 1334         */ IL_0A09: stloc.s   V_52
// 	/* 0x00047193 1134         */ IL_0A0B: ldloc.s   V_52
// 	/* 0x00047195 23000000000000F83F */ IL_0A0D: ldc.r8    1.5
// 	/* 0x0004719E 360D         */ IL_0A16: ble.un.s  IL_0A25

// 	/* 0x000471A0 23000000000000F83F */ IL_0A18: ldc.r8    1.5
// 	/* 0x000471A9 1334         */ IL_0A21: stloc.s   V_52
// 	/* 0x000471AB 2B18         */ IL_0A23: br.s      IL_0A3D

// 	/* 0x000471AD 1134         */ IL_0A25: ldloc.s   V_52
// 	/* 0x000471AF 230000000000000000 */ IL_0A27: ldc.r8    0.0
// 	/* 0x000471B8 340B         */ IL_0A30: bge.un.s  IL_0A3D

// 	/* 0x000471BA 230000000000000000 */ IL_0A32: ldc.r8    0.0
// 	/* 0x000471C3 1334         */ IL_0A3B: stloc.s   V_52

// 	/* 0x000471C5 110B         */ IL_0A3D: ldloc.s   V_11
// 	/* 0x000471C7 110F         */ IL_0A3F: ldloc.s   V_15
// 	/* 0x000471C9 1118         */ IL_0A41: ldloc.s   V_24
// 	/* 0x000471CB 28B7020006   */ IL_0A43: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 	/* 0x000471D0 28BB020006   */ IL_0A48: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000471D5 112D         */ IL_0A4D: ldloc.s   V_45
// 	/* 0x000471D7 7B46180004   */ IL_0A4F: ldfld     valuetype VectorLF3 AstroData::uPos
// 	/* 0x000471DC 28BA020006   */ IL_0A54: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000471E1 1335         */ IL_0A59: stloc.s   V_53
// 	/* 0x000471E3 1235         */ IL_0A5B: ldloca.s  V_53
// 	/* 0x000471E5 28BF020006   */ IL_0A5D: call      instance float64 VectorLF3::get_magnitude()
// 	/* 0x000471EA 112E         */ IL_0A62: ldloc.s   V_46
// 	/* 0x000471EC 6C           */ IL_0A64: conv.r8
// 	/* 0x000471ED 5B           */ IL_0A65: div
// 	/* 0x000471EE 1336         */ IL_0A66: stloc.s   V_54
// 	/* 0x000471F0 1136         */ IL_0A68: ldloc.s   V_54
// 	/* 0x000471F2 1132         */ IL_0A6A: ldloc.s   V_50
// 	/* 0x000471F4 3469         */ IL_0A6C: bge.un.s  IL_0AD7

// 	/* 0x000471F6 1136         */ IL_0A6E: ldloc.s   V_54
// 	/* 0x000471F8 23000000000000F03F */ IL_0A70: ldc.r8    1
// 	/* 0x00047201 59           */ IL_0A79: sub
// 	/* 0x00047202 1132         */ IL_0A7A: ldloc.s   V_50
// 	/* 0x00047204 23000000000000F03F */ IL_0A7C: ldc.r8    1
// 	/* 0x0004720D 59           */ IL_0A85: sub
// 	/* 0x0004720E 5B           */ IL_0A86: div
// 	/* 0x0004720F 1338         */ IL_0A87: stloc.s   V_56
// 	/* 0x00047211 1138         */ IL_0A89: ldloc.s   V_56
// 	/* 0x00047213 230000000000000000 */ IL_0A8B: ldc.r8    0.0
// 	/* 0x0004721C 340B         */ IL_0A94: bge.un.s  IL_0AA1

// 	/* 0x0004721E 230000000000000000 */ IL_0A96: ldc.r8    0.0
// 	/* 0x00047227 1338         */ IL_0A9F: stloc.s   V_56

// 	/* 0x00047229 23000000000000F03F */ IL_0AA1: ldc.r8    1
// 	/* 0x00047232 1138         */ IL_0AAA: ldloc.s   V_56
// 	/* 0x00047234 1138         */ IL_0AAC: ldloc.s   V_56
// 	/* 0x00047236 5A           */ IL_0AAE: mul
// 	/* 0x00047237 59           */ IL_0AAF: sub
// 	/* 0x00047238 1338         */ IL_0AB0: stloc.s   V_56
// 	/* 0x0004723A 1235         */ IL_0AB2: ldloca.s  V_53
// 	/* 0x0004723C 28C9020006   */ IL_0AB4: call      instance valuetype VectorLF3 VectorLF3::get_normalized()
// 	/* 0x00047241 1134         */ IL_0AB9: ldloc.s   V_52
// 	/* 0x00047243 1134         */ IL_0ABB: ldloc.s   V_52
// 	/* 0x00047245 5A           */ IL_0ABD: mul
// 	/* 0x00047246 1138         */ IL_0ABE: ldloc.s   V_56
// 	/* 0x00047248 5A           */ IL_0AC0: mul
// 	/* 0x00047249 230000000000000040 */ IL_0AC1: ldc.r8    2
// 	/* 0x00047252 5A           */ IL_0ACA: mul
// 	/* 0x00047253 28B7020006   */ IL_0ACB: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 	/* 0x00047258 28BD020006   */ IL_0AD0: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x0004725D 131C         */ IL_0AD5: stloc.s   V_28

// 	/* 0x0004725F 1237         */ IL_0AD7: ldloca.s  V_55
// 	/* 0x00047261 110B         */ IL_0AD9: ldloc.s   V_11
// 	/* 0x00047263 7B41030004   */ IL_0ADB: ldfld     float64 VectorLF3::x
// 	/* 0x00047268 112D         */ IL_0AE0: ldloc.s   V_45
// 	/* 0x0004726A 7C46180004   */ IL_0AE2: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x0004726F 7B41030004   */ IL_0AE7: ldfld     float64 VectorLF3::x
// 	/* 0x00047274 59           */ IL_0AEC: sub
// 	/* 0x00047275 7D41030004   */ IL_0AED: stfld     float64 VectorLF3::x
// 	/* 0x0004727A 1237         */ IL_0AF2: ldloca.s  V_55
// 	/* 0x0004727C 110B         */ IL_0AF4: ldloc.s   V_11
// 	/* 0x0004727E 7B42030004   */ IL_0AF6: ldfld     float64 VectorLF3::y
// 	/* 0x00047283 112D         */ IL_0AFB: ldloc.s   V_45
// 	/* 0x00047285 7C46180004   */ IL_0AFD: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x0004728A 7B42030004   */ IL_0B02: ldfld     float64 VectorLF3::y
// 	/* 0x0004728F 59           */ IL_0B07: sub
// 	/* 0x00047290 7D42030004   */ IL_0B08: stfld     float64 VectorLF3::y
// 	/* 0x00047295 1237         */ IL_0B0D: ldloca.s  V_55
// 	/* 0x00047297 110B         */ IL_0B0F: ldloc.s   V_11
// 	/* 0x00047299 7B43030004   */ IL_0B11: ldfld     float64 VectorLF3::z
// 	/* 0x0004729E 112D         */ IL_0B16: ldloc.s   V_45
// 	/* 0x000472A0 7C46180004   */ IL_0B18: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x000472A5 7B43030004   */ IL_0B1D: ldfld     float64 VectorLF3::z
// 	/* 0x000472AA 59           */ IL_0B22: sub
// 	/* 0x000472AB 7D43030004   */ IL_0B23: stfld     float64 VectorLF3::z
// 	/* 0x000472B0 112E         */ IL_0B28: ldloc.s   V_46
// 	/* 0x000472B2 6C           */ IL_0B2A: conv.r8
// 	/* 0x000472B3 5B           */ IL_0B2B: div
// 	/* 0x000472B4 131E         */ IL_0B2C: stloc.s   V_30
// 	/* 0x000472B6 111E         */ IL_0B2E: ldloc.s   V_30
// 	/* 0x000472B8 111E         */ IL_0B30: ldloc.s   V_30
// 	/* 0x000472BA 5A           */ IL_0B32: mul
// 	/* 0x000472BB 131E         */ IL_0B33: stloc.s   V_30
// 	/* 0x000472BD 1130         */ IL_0B35: ldloc.s   V_48
// 	/* 0x000472BF 111E         */ IL_0B37: ldloc.s   V_30
// 	/* 0x000472C1 59           */ IL_0B39: sub
// 	/* 0x000472C2 1130         */ IL_0B3A: ldloc.s   V_48
// 	/* 0x000472C4 1131         */ IL_0B3C: ldloc.s   V_49
// 	/* 0x000472C6 59           */ IL_0B3E: sub
// 	/* 0x000472C7 5B           */ IL_0B3F: div
// 	/* 0x000472C8 131E         */ IL_0B40: stloc.s   V_30
// 	/* 0x000472CA 111E         */ IL_0B42: ldloc.s   V_30
// 	/* 0x000472CC 23000000000000F03F */ IL_0B44: ldc.r8    1
// 	/* 0x000472D5 360D         */ IL_0B4D: ble.un.s  IL_0B5C

// 	/* 0x000472D7 23000000000000F03F */ IL_0B4F: ldc.r8    1
// 	/* 0x000472E0 131E         */ IL_0B58: stloc.s   V_30
// 	/* 0x000472E2 2B18         */ IL_0B5A: br.s      IL_0B74

// 	/* 0x000472E4 111E         */ IL_0B5C: ldloc.s   V_30
// 	/* 0x000472E6 230000000000000000 */ IL_0B5E: ldc.r8    0.0
// 	/* 0x000472EF 340B         */ IL_0B67: bge.un.s  IL_0B74

// 	/* 0x000472F1 230000000000000000 */ IL_0B69: ldc.r8    0.0
// 	/* 0x000472FA 131E         */ IL_0B72: stloc.s   V_30

// 	/* 0x000472FC 111E         */ IL_0B74: ldloc.s   V_30
// 	/* 0x000472FE 230000000000000000 */ IL_0B76: ldc.r8    0.0
// 	/* 0x00047307 43E5010000   */ IL_0B7F: ble.un    IL_0D69

// 	/* 0x0004730C 112D         */ IL_0B84: ldloc.s   V_45
// 	/* 0x0004730E 7C44180004   */ IL_0B86: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 	/* 0x00047313 1237         */ IL_0B8B: ldloca.s  V_55
// 	/* 0x00047315 1239         */ IL_0B8D: ldloca.s  V_57
// 	/* 0x00047317 2813040006   */ IL_0B8F: call      void Maths::QInvRotateLF_refout(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype VectorLF3&)
// 	/* 0x0004731C 112D         */ IL_0B94: ldloc.s   V_45
// 	/* 0x0004731E 7C47180004   */ IL_0B96: ldflda    valuetype VectorLF3 AstroData::uPosNext
// 	/* 0x00047323 112D         */ IL_0B9B: ldloc.s   V_45
// 	/* 0x00047325 7C45180004   */ IL_0B9D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRotNext
// 	/* 0x0004732A 1239         */ IL_0BA2: ldloca.s  V_57
// 	/* 0x0004732C 123A         */ IL_0BA4: ldloca.s  V_58
// 	/* 0x0004732E 2884070006   */ IL_0BA6: call      void DFRelayComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype VectorLF3&)
// 	/* 0x00047333 230000000000000840 */ IL_0BAB: ldc.r8    3
// 	/* 0x0004733C 111E         */ IL_0BB4: ldloc.s   V_30
// 	/* 0x0004733E 59           */ IL_0BB6: sub
// 	/* 0x0004733F 111E         */ IL_0BB7: ldloc.s   V_30
// 	/* 0x00047341 59           */ IL_0BB9: sub
// 	/* 0x00047342 111E         */ IL_0BBA: ldloc.s   V_30
// 	/* 0x00047344 5A           */ IL_0BBC: mul
// 	/* 0x00047345 111E         */ IL_0BBD: ldloc.s   V_30
// 	/* 0x00047347 5A           */ IL_0BBF: mul
// 	/* 0x00047348 131E         */ IL_0BC0: stloc.s   V_30
// 	/* 0x0004734A 121D         */ IL_0BC2: ldloca.s  V_29
// 	/* 0x0004734C 113A         */ IL_0BC4: ldloc.s   V_58
// 	/* 0x0004734E 7B41030004   */ IL_0BC6: ldfld     float64 VectorLF3::x
// 	/* 0x00047353 110B         */ IL_0BCB: ldloc.s   V_11
// 	/* 0x00047355 7B41030004   */ IL_0BCD: ldfld     float64 VectorLF3::x
// 	/* 0x0004735A 59           */ IL_0BD2: sub
// 	/* 0x0004735B 111E         */ IL_0BD3: ldloc.s   V_30
// 	/* 0x0004735D 5A           */ IL_0BD5: mul
// 	/* 0x0004735E 7D41030004   */ IL_0BD6: stfld     float64 VectorLF3::x
// 	/* 0x00047363 121D         */ IL_0BDB: ldloca.s  V_29
// 	/* 0x00047365 113A         */ IL_0BDD: ldloc.s   V_58
// 	/* 0x00047367 7B42030004   */ IL_0BDF: ldfld     float64 VectorLF3::y
// 	/* 0x0004736C 110B         */ IL_0BE4: ldloc.s   V_11
// 	/* 0x0004736E 7B42030004   */ IL_0BE6: ldfld     float64 VectorLF3::y
// 	/* 0x00047373 59           */ IL_0BEB: sub
// 	/* 0x00047374 111E         */ IL_0BEC: ldloc.s   V_30
// 	/* 0x00047376 5A           */ IL_0BEE: mul
// 	/* 0x00047377 7D42030004   */ IL_0BEF: stfld     float64 VectorLF3::y
// 	/* 0x0004737C 121D         */ IL_0BF4: ldloca.s  V_29
// 	/* 0x0004737E 113A         */ IL_0BF6: ldloc.s   V_58
// 	/* 0x00047380 7B43030004   */ IL_0BF8: ldfld     float64 VectorLF3::z
// 	/* 0x00047385 110B         */ IL_0BFD: ldloc.s   V_11
// 	/* 0x00047387 7B43030004   */ IL_0BFF: ldfld     float64 VectorLF3::z
// 	/* 0x0004738C 59           */ IL_0C04: sub
// 	/* 0x0004738D 111E         */ IL_0C05: ldloc.s   V_30
// 	/* 0x0004738F 5A           */ IL_0C07: mul
// 	/* 0x00047390 7D43030004   */ IL_0C08: stfld     float64 VectorLF3::z
// 	/* 0x00047395 3857010000   */ IL_0C0D: br        IL_0D69

// 	/* 0x0004739A 04           */ IL_0C12: ldarg.2
// 	/* 0x0004739B 08           */ IL_0C13: ldloc.2
// 	/* 0x0004739C 8FE7010002   */ IL_0C14: ldelema   AstroData
// 	/* 0x000473A1 133B         */ IL_0C19: stloc.s   V_59
// 	/* 0x000473A3 02           */ IL_0C1B: ldarg.0
// 	/* 0x000473A4 7B9C060004   */ IL_0C1C: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x000473A9 7B9A0D0004   */ IL_0C21: ldfld     class AstroOrbitData EnemyDFHiveSystem::hiveAstroOrbit
// 	/* 0x000473AE 7C55180004   */ IL_0C26: ldflda    valuetype VectorLF3 AstroOrbitData::orbitNormal
// 	/* 0x000473B3 133C         */ IL_0C2B: stloc.s   V_60
// 	/* 0x000473B5 123D         */ IL_0C2D: ldloca.s  V_61
// 	/* 0x000473B7 110B         */ IL_0C2F: ldloc.s   V_11
// 	/* 0x000473B9 7B41030004   */ IL_0C31: ldfld     float64 VectorLF3::x
// 	/* 0x000473BE 113B         */ IL_0C36: ldloc.s   V_59
// 	/* 0x000473C0 7C46180004   */ IL_0C38: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x000473C5 7B41030004   */ IL_0C3D: ldfld     float64 VectorLF3::x
// 	/* 0x000473CA 59           */ IL_0C42: sub
// 	/* 0x000473CB 7D41030004   */ IL_0C43: stfld     float64 VectorLF3::x
// 	/* 0x000473D0 123D         */ IL_0C48: ldloca.s  V_61
// 	/* 0x000473D2 110B         */ IL_0C4A: ldloc.s   V_11
// 	/* 0x000473D4 7B42030004   */ IL_0C4C: ldfld     float64 VectorLF3::y
// 	/* 0x000473D9 113B         */ IL_0C51: ldloc.s   V_59
// 	/* 0x000473DB 7C46180004   */ IL_0C53: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x000473E0 7B42030004   */ IL_0C58: ldfld     float64 VectorLF3::y
// 	/* 0x000473E5 59           */ IL_0C5D: sub
// 	/* 0x000473E6 7D42030004   */ IL_0C5E: stfld     float64 VectorLF3::y
// 	/* 0x000473EB 123D         */ IL_0C63: ldloca.s  V_61
// 	/* 0x000473ED 110B         */ IL_0C65: ldloc.s   V_11
// 	/* 0x000473EF 7B43030004   */ IL_0C67: ldfld     float64 VectorLF3::z
// 	/* 0x000473F4 113B         */ IL_0C6C: ldloc.s   V_59
// 	/* 0x000473F6 7C46180004   */ IL_0C6E: ldflda    valuetype VectorLF3 AstroData::uPos
// 	/* 0x000473FB 7B43030004   */ IL_0C73: ldfld     float64 VectorLF3::z
// 	/* 0x00047400 59           */ IL_0C78: sub
// 	/* 0x00047401 7D43030004   */ IL_0C79: stfld     float64 VectorLF3::z
// 	/* 0x00047406 113D         */ IL_0C7E: ldloc.s   V_61
// 	/* 0x00047408 7B41030004   */ IL_0C80: ldfld     float64 VectorLF3::x
// 	/* 0x0004740D 113C         */ IL_0C85: ldloc.s   V_60
// 	/* 0x0004740F 7B41030004   */ IL_0C87: ldfld     float64 VectorLF3::x
// 	/* 0x00047414 5A           */ IL_0C8C: mul
// 	/* 0x00047415 113D         */ IL_0C8D: ldloc.s   V_61
// 	/* 0x00047417 7B42030004   */ IL_0C8F: ldfld     float64 VectorLF3::y
// 	/* 0x0004741C 113C         */ IL_0C94: ldloc.s   V_60
// 	/* 0x0004741E 7B42030004   */ IL_0C96: ldfld     float64 VectorLF3::y
// 	/* 0x00047423 5A           */ IL_0C9B: mul
// 	/* 0x00047424 58           */ IL_0C9C: add
// 	/* 0x00047425 113D         */ IL_0C9D: ldloc.s   V_61
// 	/* 0x00047427 7B43030004   */ IL_0C9F: ldfld     float64 VectorLF3::z
// 	/* 0x0004742C 113C         */ IL_0CA4: ldloc.s   V_60
// 	/* 0x0004742E 7B43030004   */ IL_0CA6: ldfld     float64 VectorLF3::z
// 	/* 0x00047433 5A           */ IL_0CAB: mul
// 	/* 0x00047434 58           */ IL_0CAC: add
// 	/* 0x00047435 283401000A   */ IL_0CAD: call      float64 [netstandard]System.Math::Abs(float64)
// 	/* 0x0004743A 133E         */ IL_0CB2: stloc.s   V_62
// 	/* 0x0004743C 230000000000409F40 */ IL_0CB4: ldc.r8    2000
// 	/* 0x00047445 113E         */ IL_0CBD: ldloc.s   V_62
// 	/* 0x00047447 59           */ IL_0CBF: sub
// 	/* 0x00047448 230000000000709740 */ IL_0CC0: ldc.r8    1500
// 	/* 0x00047451 5B           */ IL_0CC9: div
// 	/* 0x00047452 133F         */ IL_0CCA: stloc.s   V_63
// 	/* 0x00047454 113F         */ IL_0CCC: ldloc.s   V_63
// 	/* 0x00047456 23000000000000F03F */ IL_0CCE: ldc.r8    1
// 	/* 0x0004745F 111E         */ IL_0CD7: ldloc.s   V_30
// 	/* 0x00047461 59           */ IL_0CD9: sub
// 	/* 0x00047462 23000000000000F03F */ IL_0CDA: ldc.r8    1
// 	/* 0x0004746B 111E         */ IL_0CE3: ldloc.s   V_30
// 	/* 0x0004746D 59           */ IL_0CE5: sub
// 	/* 0x0004746E 5A           */ IL_0CE6: mul
// 	/* 0x0004746F 5A           */ IL_0CE7: mul
// 	/* 0x00047470 133F         */ IL_0CE8: stloc.s   V_63
// 	/* 0x00047472 113F         */ IL_0CEA: ldloc.s   V_63
// 	/* 0x00047474 23000000000000F03F */ IL_0CEC: ldc.r8    1
// 	/* 0x0004747D 360D         */ IL_0CF5: ble.un.s  IL_0D04

// 	/* 0x0004747F 23000000000000F03F */ IL_0CF7: ldc.r8    1
// 	/* 0x00047488 133F         */ IL_0D00: stloc.s   V_63
// 	/* 0x0004748A 2B18         */ IL_0D02: br.s      IL_0D1C

// 	/* 0x0004748C 113F         */ IL_0D04: ldloc.s   V_63
// 	/* 0x0004748E 230000000000000000 */ IL_0D06: ldc.r8    0.0
// 	/* 0x00047497 340B         */ IL_0D0F: bge.un.s  IL_0D1C

// 	/* 0x00047499 230000000000000000 */ IL_0D11: ldc.r8    0.0
// 	/* 0x000474A2 133F         */ IL_0D1A: stloc.s   V_63

// 	/* 0x000474A4 113F         */ IL_0D1C: ldloc.s   V_63
// 	/* 0x000474A6 230000000000000000 */ IL_0D1E: ldc.r8    0.0
// 	/* 0x000474AF 3640         */ IL_0D27: ble.un.s  IL_0D69

// 	/* 0x000474B1 230000000000000840 */ IL_0D29: ldc.r8    3
// 	/* 0x000474BA 113F         */ IL_0D32: ldloc.s   V_63
// 	/* 0x000474BC 59           */ IL_0D34: sub
// 	/* 0x000474BD 113F         */ IL_0D35: ldloc.s   V_63
// 	/* 0x000474BF 59           */ IL_0D37: sub
// 	/* 0x000474C0 113F         */ IL_0D38: ldloc.s   V_63
// 	/* 0x000474C2 5A           */ IL_0D3A: mul
// 	/* 0x000474C3 113F         */ IL_0D3B: ldloc.s   V_63
// 	/* 0x000474C5 5A           */ IL_0D3D: mul
// 	/* 0x000474C6 133F         */ IL_0D3E: stloc.s   V_63
// 	/* 0x000474C8 111D         */ IL_0D40: ldloc.s   V_29
// 	/* 0x000474CA 02           */ IL_0D42: ldarg.0
// 	/* 0x000474CB 7B9C060004   */ IL_0D43: ldfld     class EnemyDFHiveSystem DFRelayComponent::hive
// 	/* 0x000474D0 7B9A0D0004   */ IL_0D48: ldfld     class AstroOrbitData EnemyDFHiveSystem::hiveAstroOrbit
// 	/* 0x000474D5 113B         */ IL_0D4D: ldloc.s   V_59
// 	/* 0x000474D7 7B46180004   */ IL_0D4F: ldfld     valuetype VectorLF3 AstroData::uPos
// 	/* 0x000474DC 110B         */ IL_0D54: ldloc.s   V_11
// 	/* 0x000474DE 6F77140006   */ IL_0D56: callvirt  instance valuetype VectorLF3 AstroOrbitData::GetVelocityAtPoint(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000474E3 113F         */ IL_0D5B: ldloc.s   V_63
// 	/* 0x000474E5 28B7020006   */ IL_0D5D: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 	/* 0x000474EA 28BB020006   */ IL_0D62: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000474EF 131D         */ IL_0D67: stloc.s   V_29

// 	/* 0x000474F1 121F         */ IL_0D69: ldloca.s  V_31
// 	/* 0x000474F3 111C         */ IL_0D6B: ldloc.s   V_28
// 	/* 0x000474F5 7B4100000A   */ IL_0D6D: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x000474FA 7D4100000A   */ IL_0D72: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x000474FF 121F         */ IL_0D77: ldloca.s  V_31
// 	/* 0x00047501 111C         */ IL_0D79: ldloc.s   V_28
// 	/* 0x00047503 7B4200000A   */ IL_0D7B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x00047508 7D4200000A   */ IL_0D80: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x0004750D 121F         */ IL_0D85: ldloca.s  V_31
// 	/* 0x0004750F 111C         */ IL_0D87: ldloc.s   V_28
// 	/* 0x00047511 7B8000000A   */ IL_0D89: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x00047516 7D8000000A   */ IL_0D8E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x0004751B 1112         */ IL_0D93: ldloc.s   V_18
// 	/* 0x0004751D 230000000000000000 */ IL_0D95: ldc.r8    0.0
// 	/* 0x00047526 3642         */ IL_0D9E: ble.un.s  IL_0DE2

// 	/* 0x00047528 121F         */ IL_0DA0: ldloca.s  V_31
// 	/* 0x0004752A 7C4100000A   */ IL_0DA2: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x0004752F 25           */ IL_0DA7: dup
// 	/* 0x00047530 4E           */ IL_0DA8: ldind.r4
// 	/* 0x00047531 1111         */ IL_0DA9: ldloc.s   V_17
// 	/* 0x00047533 7B41030004   */ IL_0DAB: ldfld     float64 VectorLF3::x
// 	/* 0x00047538 1112         */ IL_0DB0: ldloc.s   V_18
// 	/* 0x0004753A 5B           */ IL_0DB2: div
// 	/* 0x0004753B 6B           */ IL_0DB3: conv.r4
// 	/* 0x0004753C 58           */ IL_0DB4: add
// 	/* 0x0004753D 56           */ IL_0DB5: stind.r4
// 	/* 0x0004753E 121F         */ IL_0DB6: ldloca.s  V_31
// 	/* 0x00047540 7C4200000A   */ IL_0DB8: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x00047545 25           */ IL_0DBD: dup
// 	/* 0x00047546 4E           */ IL_0DBE: ldind.r4
// 	/* 0x00047547 1111         */ IL_0DBF: ldloc.s   V_17
// 	/* 0x00047549 7B42030004   */ IL_0DC1: ldfld     float64 VectorLF3::y
// 	/* 0x0004754E 1112         */ IL_0DC6: ldloc.s   V_18
// 	/* 0x00047550 5B           */ IL_0DC8: div
// 	/* 0x00047551 6B           */ IL_0DC9: conv.r4
// 	/* 0x00047552 58           */ IL_0DCA: add
// 	/* 0x00047553 56           */ IL_0DCB: stind.r4
// 	/* 0x00047554 121F         */ IL_0DCC: ldloca.s  V_31
// 	/* 0x00047556 7C8000000A   */ IL_0DCE: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x0004755B 25           */ IL_0DD3: dup
// 	/* 0x0004755C 4E           */ IL_0DD4: ldind.r4
// 	/* 0x0004755D 1111         */ IL_0DD5: ldloc.s   V_17
// 	/* 0x0004755F 7B43030004   */ IL_0DD7: ldfld     float64 VectorLF3::z
// 	/* 0x00047564 1112         */ IL_0DDC: ldloc.s   V_18
// 	/* 0x00047566 5B           */ IL_0DDE: div
// 	/* 0x00047567 6B           */ IL_0DDF: conv.r4
// 	/* 0x00047568 58           */ IL_0DE0: add
// 	/* 0x00047569 56           */ IL_0DE1: stind.r4

// 	/* 0x0004756A 111F         */ IL_0DE2: ldloc.s   V_31
// 	/* 0x0004756C 283603000A   */ IL_0DE4: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::LookRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00047571 22F30435BF   */ IL_0DE9: ldc.r4    -0.70710677
// 	/* 0x00047576 2200000000   */ IL_0DEE: ldc.r4    0.0
// 	/* 0x0004757B 2200000000   */ IL_0DF3: ldc.r4    0.0
// 	/* 0x00047580 22F304353F   */ IL_0DF8: ldc.r4    0.70710677
// 	/* 0x00047585 735702000A   */ IL_0DFD: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 	/* 0x0004758A 281A01000A   */ IL_0E02: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 	/* 0x0004758F 1320         */ IL_0E07: stloc.s   V_32
// 	/* 0x00047591 1112         */ IL_0E09: ldloc.s   V_18
// 	/* 0x00047593 230000000000407F40 */ IL_0E0B: ldc.r8    500
// 	/* 0x0004759C 320E         */ IL_0E14: blt.s     IL_0E24

// 	/* 0x0004759E 02           */ IL_0E16: ldarg.0
// 	/* 0x0004759F 7BA8060004   */ IL_0E17: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x000475A4 2200007A44   */ IL_0E1C: ldc.r4    1000
// 	/* 0x000475A9 5B           */ IL_0E21: div
// 	/* 0x000475AA 2B08         */ IL_0E22: br.s      IL_0E2C

// 	/* 0x000475AC 02           */ IL_0E24: ldarg.0
// 	/* 0x000475AD 1112         */ IL_0E25: ldloc.s   V_18
// 	/* 0x000475AF 2878070006   */ IL_0E27: call      instance float32 DFRelayComponent::MapLenToToRotSens(float64)

// 	/* 0x000475B4 1321         */ IL_0E2C: stloc.s   V_33
// 	/* 0x000475B6 110E         */ IL_0E2E: ldloc.s   V_14
// 	/* 0x000475B8 1120         */ IL_0E30: ldloc.s   V_32
// 	/* 0x000475BA 228988883D   */ IL_0E32: ldc.r4    0.06666667
// 	/* 0x000475BF 28DE00000A   */ IL_0E37: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 	/* 0x000475C4 26           */ IL_0E3C: pop
// 	/* 0x000475C5 110E         */ IL_0E3D: ldloc.s   V_14
// 	/* 0x000475C7 1120         */ IL_0E3F: ldloc.s   V_32
// 	/* 0x000475C9 1121         */ IL_0E41: ldloc.s   V_33
// 	/* 0x000475CB 228988883C   */ IL_0E43: ldc.r4    0.016666668
// 	/* 0x000475D0 5A           */ IL_0E48: mul
// 	/* 0x000475D1 28DE00000A   */ IL_0E49: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 	/* 0x000475D6 130E         */ IL_0E4E: stloc.s   V_14
// 	/* 0x000475D8 0E04         */ IL_0E50: ldarg.s   enemyData
// 	/* 0x000475DA 110E         */ IL_0E52: ldloc.s   V_14
// 	/* 0x000475DC 28F2030006   */ IL_0E54: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 Maths::Up(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 	/* 0x000475E1 28D700000A   */ IL_0E59: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_UnaryNegation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x000475E6 02           */ IL_0E5E: ldarg.0
// 	/* 0x000475E7 7BA8060004   */ IL_0E5F: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x000475EC 289800000A   */ IL_0E64: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x000475F1 111D         */ IL_0E69: ldloc.s   V_29
// 	/* 0x000475F3 23111111111111913F */ IL_0E6B: ldc.r8    0.016666666666666666
// 	/* 0x000475FC 28B8020006   */ IL_0E74: call      valuetype VectorLF3 VectorLF3::op_Division(valuetype VectorLF3, float64)
// 	/* 0x00047601 28BD020006   */ IL_0E79: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00047606 289900000A   */ IL_0E7E: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x0004760B 7D201A0004   */ IL_0E83: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047610 120B         */ IL_0E88: ldloca.s  V_11
// 	/* 0x00047612 110B         */ IL_0E8A: ldloc.s   V_11
// 	/* 0x00047614 7B41030004   */ IL_0E8C: ldfld     float64 VectorLF3::x
// 	/* 0x00047619 0E04         */ IL_0E91: ldarg.s   enemyData
// 	/* 0x0004761B 7C201A0004   */ IL_0E93: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047620 7B4100000A   */ IL_0E98: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x00047625 6C           */ IL_0E9D: conv.r8
// 	/* 0x00047626 23111111111111913F */ IL_0E9E: ldc.r8    0.016666666666666666
// 	/* 0x0004762F 5A           */ IL_0EA7: mul
// 	/* 0x00047630 58           */ IL_0EA8: add
// 	/* 0x00047631 7D41030004   */ IL_0EA9: stfld     float64 VectorLF3::x
// 	/* 0x00047636 120B         */ IL_0EAE: ldloca.s  V_11
// 	/* 0x00047638 110B         */ IL_0EB0: ldloc.s   V_11
// 	/* 0x0004763A 7B42030004   */ IL_0EB2: ldfld     float64 VectorLF3::y
// 	/* 0x0004763F 0E04         */ IL_0EB7: ldarg.s   enemyData
// 	/* 0x00047641 7C201A0004   */ IL_0EB9: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047646 7B4200000A   */ IL_0EBE: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x0004764B 6C           */ IL_0EC3: conv.r8
// 	/* 0x0004764C 23111111111111913F */ IL_0EC4: ldc.r8    0.016666666666666666
// 	/* 0x00047655 5A           */ IL_0ECD: mul
// 	/* 0x00047656 58           */ IL_0ECE: add
// 	/* 0x00047657 7D42030004   */ IL_0ECF: stfld     float64 VectorLF3::y
// 	/* 0x0004765C 120B         */ IL_0ED4: ldloca.s  V_11
// 	/* 0x0004765E 110B         */ IL_0ED6: ldloc.s   V_11
// 	/* 0x00047660 7B43030004   */ IL_0ED8: ldfld     float64 VectorLF3::z
// 	/* 0x00047665 0E04         */ IL_0EDD: ldarg.s   enemyData
// 	/* 0x00047667 7C201A0004   */ IL_0EDF: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x0004766C 7B8000000A   */ IL_0EE4: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x00047671 6C           */ IL_0EE9: conv.r8
// 	/* 0x00047672 23111111111111913F */ IL_0EEA: ldc.r8    0.016666666666666666
// 	/* 0x0004767B 5A           */ IL_0EF3: mul
// 	/* 0x0004767C 58           */ IL_0EF4: add
// 	/* 0x0004767D 7D43030004   */ IL_0EF5: stfld     float64 VectorLF3::z
// 	/* 0x00047682 03           */ IL_0EFA: ldarg.1
// 	/* 0x00047683 08           */ IL_0EFB: ldloc.2
// 	/* 0x00047684 120B         */ IL_0EFC: ldloca.s  V_11
// 	/* 0x00047686 120E         */ IL_0EFE: ldloca.s  V_14
// 	/* 0x00047688 0E04         */ IL_0F00: ldarg.s   enemyData
// 	/* 0x0004768A 7C1E1A0004   */ IL_0F02: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x0004768F 0E04         */ IL_0F07: ldarg.s   enemyData
// 	/* 0x00047691 7C1F1A0004   */ IL_0F09: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00047696 6F21190006   */ IL_0F0E: callvirt  instance void SpaceSector::InverseTransformToAstro_ref(int32, valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&)
// 	/* 0x0004769B 1113         */ IL_0F13: ldloc.s   V_19
// 	/* 0x0004769D 2C23         */ IL_0F15: brfalse.s IL_0F3A

// 	/* 0x0004769F 02           */ IL_0F17: ldarg.0
// 	/* 0x000476A0 7BA6060004   */ IL_0F18: ldfld     int32 DFRelayComponent::direction
// 	/* 0x000476A5 16           */ IL_0F1D: ldc.i4.0
// 	/* 0x000476A6 3008         */ IL_0F1E: bgt.s     IL_0F28

// 	/* 0x000476A8 02           */ IL_0F20: ldarg.0
// 	/* 0x000476A9 7B9D060004   */ IL_0F21: ldfld     int32 DFRelayComponent::hiveAstroId
// 	/* 0x000476AE 2B06         */ IL_0F26: br.s      IL_0F2E

// 	/* 0x000476B0 02           */ IL_0F28: ldarg.0
// 	/* 0x000476B1 7B9E060004   */ IL_0F29: ldfld     int32 DFRelayComponent::targetAstroId

// 	/* 0x000476B6 1340         */ IL_0F2E: stloc.s   V_64
// 	/* 0x000476B8 03           */ IL_0F30: ldarg.1
// 	/* 0x000476B9 1140         */ IL_0F31: ldloc.s   V_64
// 	/* 0x000476BB 0E04         */ IL_0F33: ldarg.s   enemyData
// 	/* 0x000476BD 6F41190006   */ IL_0F35: callvirt  instance void SpaceSector::AlterEnemyAstroId(int32, valuetype EnemyData&)

// 	/* 0x000476C2 0E05         */ IL_0F3A: ldarg.s   animData
// 	/* 0x000476C4 1112         */ IL_0F3C: ldloc.s   V_18
// 	/* 0x000476C6 6B           */ IL_0F3E: conv.r4
// 	/* 0x000476C7 7D35180004   */ IL_0F3F: stfld     float32 AnimData::power
// 	/* 0x000476CC 0E05         */ IL_0F44: ldarg.s   animData
// 	/* 0x000476CE 2200000000   */ IL_0F46: ldc.r4    0.0
// 	/* 0x000476D3 7D31180004   */ IL_0F4B: stfld     float32 AnimData::time
// 	/* 0x000476D8 2A           */ IL_0F50: ret

// 	/* 0x000476D9 02           */ IL_0F51: ldarg.0
// 	/* 0x000476DA 7BA7060004   */ IL_0F52: ldfld     int32 DFRelayComponent::stage
// 	/* 0x000476DF 17           */ IL_0F57: ldc.i4.1
// 	/* 0x000476E0 4041030000   */ IL_0F58: bne.un    IL_129E

// 	/* 0x000476E5 02           */ IL_0F5D: ldarg.0
// 	/* 0x000476E6 7BA6060004   */ IL_0F5E: ldfld     int32 DFRelayComponent::direction
// 	/* 0x000476EB 16           */ IL_0F63: ldc.i4.0
// 	/* 0x000476EC 3E18010000   */ IL_0F64: ble       IL_1081

// 	/* 0x000476F1 02           */ IL_0F69: ldarg.0
// 	/* 0x000476F2 7B9F060004   */ IL_0F6A: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x000476F7 02           */ IL_0F6F: ldarg.0
// 	/* 0x000476F8 7BA0060004   */ IL_0F70: ldfld     float32 DFRelayComponent::targetYaw
// 	/* 0x000476FD 220000B443   */ IL_0F75: ldc.r4    360
// 	/* 0x00047702 5A           */ IL_0F7A: mul
// 	/* 0x00047703 281C040006   */ IL_0F7B: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion Maths::SphericalRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x00047708 1341         */ IL_0F80: stloc.s   V_65
// 	/* 0x0004770A 1141         */ IL_0F82: ldloc.s   V_65
// 	/* 0x0004770C 0E04         */ IL_0F84: ldarg.s   enemyData
// 	/* 0x0004770E 7B1F1A0004   */ IL_0F86: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00047713 285603000A   */ IL_0F8B: call      float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::Angle(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 	/* 0x00047718 1342         */ IL_0F90: stloc.s   V_66
// 	/* 0x0004771A 02           */ IL_0F92: ldarg.0
// 	/* 0x0004771B 7B9F060004   */ IL_0F93: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00047720 28BC020006   */ IL_0F98: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00047725 0E04         */ IL_0F9D: ldarg.s   enemyData
// 	/* 0x00047727 7B1E1A0004   */ IL_0F9F: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x0004772C 28BA020006   */ IL_0FA4: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x00047731 1343         */ IL_0FA9: stloc.s   V_67
// 	/* 0x00047733 1143         */ IL_0FAB: ldloc.s   V_67
// 	/* 0x00047735 7B41030004   */ IL_0FAD: ldfld     float64 VectorLF3::x
// 	/* 0x0004773A 1143         */ IL_0FB2: ldloc.s   V_67
// 	/* 0x0004773C 7B41030004   */ IL_0FB4: ldfld     float64 VectorLF3::x
// 	/* 0x00047741 5A           */ IL_0FB9: mul
// 	/* 0x00047742 1143         */ IL_0FBA: ldloc.s   V_67
// 	/* 0x00047744 7B42030004   */ IL_0FBC: ldfld     float64 VectorLF3::y
// 	/* 0x00047749 1143         */ IL_0FC1: ldloc.s   V_67
// 	/* 0x0004774B 7B42030004   */ IL_0FC3: ldfld     float64 VectorLF3::y
// 	/* 0x00047750 5A           */ IL_0FC8: mul
// 	/* 0x00047751 58           */ IL_0FC9: add
// 	/* 0x00047752 1143         */ IL_0FCA: ldloc.s   V_67
// 	/* 0x00047754 7B43030004   */ IL_0FCC: ldfld     float64 VectorLF3::z
// 	/* 0x00047759 1143         */ IL_0FD1: ldloc.s   V_67
// 	/* 0x0004775B 7B43030004   */ IL_0FD3: ldfld     float64 VectorLF3::z
// 	/* 0x00047760 5A           */ IL_0FD8: mul
// 	/* 0x00047761 58           */ IL_0FD9: add
// 	/* 0x00047762 23000000000000D03F */ IL_0FDA: ldc.r8    0.25
// 	/* 0x0004776B 341C         */ IL_0FE3: bge.un.s  IL_1001

// 	/* 0x0004776D 1142         */ IL_0FE5: ldloc.s   V_66
// 	/* 0x0004776F 220000003F   */ IL_0FE7: ldc.r4    0.5
// 	/* 0x00047774 3413         */ IL_0FEC: bge.un.s  IL_1001

// 	/* 0x00047776 02           */ IL_0FEE: ldarg.0
// 	/* 0x00047777 2200000000   */ IL_0FEF: ldc.r4    0.0
// 	/* 0x0004777C 7DA9060004   */ IL_0FF4: stfld     float32 DFRelayComponent::param0
// 	/* 0x00047781 02           */ IL_0FF9: ldarg.0
// 	/* 0x00047782 18           */ IL_0FFA: ldc.i4.2
// 	/* 0x00047783 7DA7060004   */ IL_0FFB: stfld     int32 DFRelayComponent::stage
// 	/* 0x00047788 2A           */ IL_1000: ret

// 	/* 0x00047789 0E04         */ IL_1001: ldarg.s   enemyData
// 	/* 0x0004778B 7B1E1A0004   */ IL_1003: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00047790 1344         */ IL_1008: stloc.s   V_68
// 	/* 0x00047792 0E04         */ IL_100A: ldarg.s   enemyData
// 	/* 0x00047794 0E04         */ IL_100C: ldarg.s   enemyData
// 	/* 0x00047796 7B1E1A0004   */ IL_100E: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x0004779B 28BD020006   */ IL_1013: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x000477A0 02           */ IL_1018: ldarg.0
// 	/* 0x000477A1 7B9F060004   */ IL_1019: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x000477A6 228988883C   */ IL_101E: ldc.r4    0.016666668
// 	/* 0x000477AB 285303000A   */ IL_1023: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::MoveTowards(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x000477B0 28BC020006   */ IL_1028: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x000477B5 7D1E1A0004   */ IL_102D: stfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x000477BA 0E04         */ IL_1032: ldarg.s   enemyData
// 	/* 0x000477BC 0E04         */ IL_1034: ldarg.s   enemyData
// 	/* 0x000477BE 7B1F1A0004   */ IL_1036: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x000477C3 1141         */ IL_103B: ldloc.s   V_65
// 	/* 0x000477C5 220000803E   */ IL_103D: ldc.r4    0.25
// 	/* 0x000477CA 285403000A   */ IL_1042: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::RotateTowards(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 	/* 0x000477CF 7D1F1A0004   */ IL_1047: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x000477D4 0E04         */ IL_104C: ldarg.s   enemyData
// 	/* 0x000477D6 0E04         */ IL_104E: ldarg.s   enemyData
// 	/* 0x000477D8 7B1E1A0004   */ IL_1050: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x000477DD 1144         */ IL_1055: ldloc.s   V_68
// 	/* 0x000477DF 28BA020006   */ IL_1057: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 	/* 0x000477E4 23111111111111913F */ IL_105C: ldc.r8    0.016666666666666666
// 	/* 0x000477ED 28B8020006   */ IL_1065: call      valuetype VectorLF3 VectorLF3::op_Division(valuetype VectorLF3, float64)
// 	/* 0x000477F2 28BD020006   */ IL_106A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x000477F7 7D201A0004   */ IL_106F: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000477FC 0E05         */ IL_1074: ldarg.s   animData
// 	/* 0x000477FE 2200000000   */ IL_1076: ldc.r4    0.0
// 	/* 0x00047803 7D35180004   */ IL_107B: stfld     float32 AnimData::power
// 	/* 0x00047808 2A           */ IL_1080: ret

// 	/* 0x00047809 0E05         */ IL_1081: ldarg.s   animData
// 	/* 0x0004780B 7B32180004   */ IL_1083: ldfld     float32 AnimData::prepare_length
// 	/* 0x00047810 1345         */ IL_1088: stloc.s   V_69
// 	/* 0x00047812 0E05         */ IL_108A: ldarg.s   animData
// 	/* 0x00047814 7B33180004   */ IL_108C: ldfld     float32 AnimData::working_length
// 	/* 0x00047819 1346         */ IL_1091: stloc.s   V_70
// 	/* 0x0004781B 0E04         */ IL_1093: ldarg.s   enemyData
// 	/* 0x0004781D 7B1E1A0004   */ IL_1095: ldfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00047822 1347         */ IL_109A: stloc.s   V_71
// 	/* 0x00047824 1147         */ IL_109C: ldloc.s   V_71
// 	/* 0x00047826 7B41030004   */ IL_109E: ldfld     float64 VectorLF3::x
// 	/* 0x0004782B 1147         */ IL_10A3: ldloc.s   V_71
// 	/* 0x0004782D 7B41030004   */ IL_10A5: ldfld     float64 VectorLF3::x
// 	/* 0x00047832 5A           */ IL_10AA: mul
// 	/* 0x00047833 1147         */ IL_10AB: ldloc.s   V_71
// 	/* 0x00047835 7B42030004   */ IL_10AD: ldfld     float64 VectorLF3::y
// 	/* 0x0004783A 1147         */ IL_10B2: ldloc.s   V_71
// 	/* 0x0004783C 7B42030004   */ IL_10B4: ldfld     float64 VectorLF3::y
// 	/* 0x00047841 5A           */ IL_10B9: mul
// 	/* 0x00047842 58           */ IL_10BA: add
// 	/* 0x00047843 1147         */ IL_10BB: ldloc.s   V_71
// 	/* 0x00047845 7B43030004   */ IL_10BD: ldfld     float64 VectorLF3::z
// 	/* 0x0004784A 1147         */ IL_10C2: ldloc.s   V_71
// 	/* 0x0004784C 7B43030004   */ IL_10C4: ldfld     float64 VectorLF3::z
// 	/* 0x00047851 5A           */ IL_10C9: mul
// 	/* 0x00047852 58           */ IL_10CA: add
// 	/* 0x00047853 284B02000A   */ IL_10CB: call      float64 [netstandard]System.Math::Sqrt(float64)
// 	/* 0x00047858 6B           */ IL_10D0: conv.r4
// 	/* 0x00047859 2200008743   */ IL_10D1: ldc.r4    270
// 	/* 0x0004785E 59           */ IL_10D6: sub
// 	/* 0x0004785F 1348         */ IL_10D7: stloc.s   V_72
// 	/* 0x00047861 1148         */ IL_10D9: ldloc.s   V_72
// 	/* 0x00047863 6B           */ IL_10DB: conv.r4
// 	/* 0x00047864 2200004843   */ IL_10DC: ldc.r4    200
// 	/* 0x00047869 5B           */ IL_10E1: div
// 	/* 0x0004786A 1349         */ IL_10E2: stloc.s   V_73
// 	/* 0x0004786C 1149         */ IL_10E4: ldloc.s   V_73
// 	/* 0x0004786E 2200000000   */ IL_10E6: ldc.r4    0.0
// 	/* 0x00047873 3409         */ IL_10EB: bge.un.s  IL_10F6

// 	/* 0x00047875 2200000000   */ IL_10ED: ldc.r4    0.0
// 	/* 0x0004787A 1349         */ IL_10F2: stloc.s   V_73
// 	/* 0x0004787C 2B10         */ IL_10F4: br.s      IL_1106

// 	/* 0x0004787E 1149         */ IL_10F6: ldloc.s   V_73
// 	/* 0x00047880 220000803F   */ IL_10F8: ldc.r4    1
// 	/* 0x00047885 3607         */ IL_10FD: ble.un.s  IL_1106

// 	/* 0x00047887 220000803F   */ IL_10FF: ldc.r4    1
// 	/* 0x0004788C 1349         */ IL_1104: stloc.s   V_73

// 	/* 0x0004788E 1149         */ IL_1106: ldloc.s   V_73
// 	/* 0x00047890 2272F97F3F   */ IL_1108: ldc.r4    0.9999
// 	/* 0x00047895 375B         */ IL_110D: blt.un.s  IL_116A

// 	/* 0x00047897 02           */ IL_110F: ldarg.0
// 	/* 0x00047898 16           */ IL_1110: ldc.i4.0
// 	/* 0x00047899 7DA7060004   */ IL_1111: stfld     int32 DFRelayComponent::stage
// 	/* 0x0004789E 02           */ IL_1116: ldarg.0
// 	/* 0x0004789F 2200000000   */ IL_1117: ldc.r4    0.0
// 	/* 0x000478A4 7DA8060004   */ IL_111C: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x000478A9 0E04         */ IL_1121: ldarg.s   enemyData
// 	/* 0x000478AB 7C201A0004   */ IL_1123: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000478B0 2200000000   */ IL_1128: ldc.r4    0.0
// 	/* 0x000478B5 7D4100000A   */ IL_112D: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x000478BA 0E04         */ IL_1132: ldarg.s   enemyData
// 	/* 0x000478BC 7C201A0004   */ IL_1134: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000478C1 2200000000   */ IL_1139: ldc.r4    0.0
// 	/* 0x000478C6 7D4200000A   */ IL_113E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x000478CB 0E04         */ IL_1143: ldarg.s   enemyData
// 	/* 0x000478CD 7C201A0004   */ IL_1145: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000478D2 2200000000   */ IL_114A: ldc.r4    0.0
// 	/* 0x000478D7 7D8000000A   */ IL_114F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x000478DC 0E05         */ IL_1154: ldarg.s   animData
// 	/* 0x000478DE 2200000000   */ IL_1156: ldc.r4    0.0
// 	/* 0x000478E3 7D31180004   */ IL_115B: stfld     float32 AnimData::time
// 	/* 0x000478E8 03           */ IL_1160: ldarg.1
// 	/* 0x000478E9 08           */ IL_1161: ldloc.2
// 	/* 0x000478EA 0E04         */ IL_1162: ldarg.s   enemyData
// 	/* 0x000478EC 6F41190006   */ IL_1164: callvirt  instance void SpaceSector::AlterEnemyAstroId(int32, valuetype EnemyData&)
// 	/* 0x000478F1 2A           */ IL_1169: ret

// 	/* 0x000478F2 1247         */ IL_116A: ldloca.s  V_71
// 	/* 0x000478F4 28C9020006   */ IL_116C: call      instance valuetype VectorLF3 VectorLF3::get_normalized()
// 	/* 0x000478F9 134A         */ IL_1171: stloc.s   V_74
// 	/* 0x000478FB 1147         */ IL_1173: ldloc.s   V_71
// 	/* 0x000478FD 28BD020006   */ IL_1175: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00047902 2200000000   */ IL_117A: ldc.r4    0.0
// 	/* 0x00047907 281C040006   */ IL_117F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion Maths::SphericalRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x0004790C 22F30435BF   */ IL_1184: ldc.r4    -0.70710677
// 	/* 0x00047911 2200000000   */ IL_1189: ldc.r4    0.0
// 	/* 0x00047916 2200000000   */ IL_118E: ldc.r4    0.0
// 	/* 0x0004791B 22F304353F   */ IL_1193: ldc.r4    0.70710677
// 	/* 0x00047920 735702000A   */ IL_1198: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 	/* 0x00047925 281A01000A   */ IL_119D: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 	/* 0x0004792A 134B         */ IL_11A2: stloc.s   V_75
// 	/* 0x0004792C 02           */ IL_11A4: ldarg.0
// 	/* 0x0004792D 220000C040   */ IL_11A5: ldc.r4    6
// 	/* 0x00047932 1149         */ IL_11AA: ldloc.s   V_73
// 	/* 0x00047934 1149         */ IL_11AC: ldloc.s   V_73
// 	/* 0x00047936 1149         */ IL_11AE: ldloc.s   V_73
// 	/* 0x00047938 5A           */ IL_11B0: mul
// 	/* 0x00047939 59           */ IL_11B1: sub
// 	/* 0x0004793A 5A           */ IL_11B2: mul
// 	/* 0x0004793B 220000F041   */ IL_11B3: ldc.r4    30
// 	/* 0x00047940 5A           */ IL_11B8: mul
// 	/* 0x00047941 220000C03F   */ IL_11B9: ldc.r4    1.5
// 	/* 0x00047946 5A           */ IL_11BE: mul
// 	/* 0x00047947 220000003F   */ IL_11BF: ldc.r4    0.5
// 	/* 0x0004794C 58           */ IL_11C4: add
// 	/* 0x0004794D 7DA8060004   */ IL_11C5: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00047952 0E04         */ IL_11CA: ldarg.s   enemyData
// 	/* 0x00047954 114A         */ IL_11CC: ldloc.s   V_74
// 	/* 0x00047956 02           */ IL_11CE: ldarg.0
// 	/* 0x00047957 7BA8060004   */ IL_11CF: ldfld     float32 DFRelayComponent::uSpeed
// 	/* 0x0004795C 6C           */ IL_11D4: conv.r8
// 	/* 0x0004795D 28B7020006   */ IL_11D5: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 	/* 0x00047962 28BD020006   */ IL_11DA: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 	/* 0x00047967 7D201A0004   */ IL_11DF: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x0004796C 0E04         */ IL_11E4: ldarg.s   enemyData
// 	/* 0x0004796E 7C1E1A0004   */ IL_11E6: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x00047973 7C41030004   */ IL_11EB: ldflda    float64 VectorLF3::x
// 	/* 0x00047978 25           */ IL_11F0: dup
// 	/* 0x00047979 4F           */ IL_11F1: ldind.r8
// 	/* 0x0004797A 0E04         */ IL_11F2: ldarg.s   enemyData
// 	/* 0x0004797C 7C201A0004   */ IL_11F4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047981 7B4100000A   */ IL_11F9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x00047986 6C           */ IL_11FE: conv.r8
// 	/* 0x00047987 23111111111111913F */ IL_11FF: ldc.r8    0.016666666666666666
// 	/* 0x00047990 5A           */ IL_1208: mul
// 	/* 0x00047991 58           */ IL_1209: add
// 	/* 0x00047992 57           */ IL_120A: stind.r8
// 	/* 0x00047993 0E04         */ IL_120B: ldarg.s   enemyData
// 	/* 0x00047995 7C1E1A0004   */ IL_120D: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x0004799A 7C42030004   */ IL_1212: ldflda    float64 VectorLF3::y
// 	/* 0x0004799F 25           */ IL_1217: dup
// 	/* 0x000479A0 4F           */ IL_1218: ldind.r8
// 	/* 0x000479A1 0E04         */ IL_1219: ldarg.s   enemyData
// 	/* 0x000479A3 7C201A0004   */ IL_121B: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000479A8 7B4200000A   */ IL_1220: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x000479AD 6C           */ IL_1225: conv.r8
// 	/* 0x000479AE 23111111111111913F */ IL_1226: ldc.r8    0.016666666666666666
// 	/* 0x000479B7 5A           */ IL_122F: mul
// 	/* 0x000479B8 58           */ IL_1230: add
// 	/* 0x000479B9 57           */ IL_1231: stind.r8
// 	/* 0x000479BA 0E04         */ IL_1232: ldarg.s   enemyData
// 	/* 0x000479BC 7C1E1A0004   */ IL_1234: ldflda    valuetype VectorLF3 EnemyData::pos
// 	/* 0x000479C1 7C43030004   */ IL_1239: ldflda    float64 VectorLF3::z
// 	/* 0x000479C6 25           */ IL_123E: dup
// 	/* 0x000479C7 4F           */ IL_123F: ldind.r8
// 	/* 0x000479C8 0E04         */ IL_1240: ldarg.s   enemyData
// 	/* 0x000479CA 7C201A0004   */ IL_1242: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x000479CF 7B8000000A   */ IL_1247: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x000479D4 6C           */ IL_124C: conv.r8
// 	/* 0x000479D5 23111111111111913F */ IL_124D: ldc.r8    0.016666666666666666
// 	/* 0x000479DE 5A           */ IL_1256: mul
// 	/* 0x000479DF 58           */ IL_1257: add
// 	/* 0x000479E0 57           */ IL_1258: stind.r8
// 	/* 0x000479E1 1149         */ IL_1259: ldloc.s   V_73
// 	/* 0x000479E3 229A99993E   */ IL_125B: ldc.r4    0.3
// 	/* 0x000479E8 361A         */ IL_1260: ble.un.s  IL_127C

// 	/* 0x000479EA 0E04         */ IL_1262: ldarg.s   enemyData
// 	/* 0x000479EC 0E04         */ IL_1264: ldarg.s   enemyData
// 	/* 0x000479EE 7B1F1A0004   */ IL_1266: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x000479F3 114B         */ IL_126B: ldloc.s   V_75
// 	/* 0x000479F5 220000803E   */ IL_126D: ldc.r4    0.25
// 	/* 0x000479FA 285403000A   */ IL_1272: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::RotateTowards(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 	/* 0x000479FF 7D1F1A0004   */ IL_1277: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot

// 	/* 0x00047A04 0E05         */ IL_127C: ldarg.s   animData
// 	/* 0x00047A06 1148         */ IL_127E: ldloc.s   V_72
// 	/* 0x00047A08 7D35180004   */ IL_1280: stfld     float32 AnimData::power
// 	/* 0x00047A0D 0E05         */ IL_1285: ldarg.s   animData
// 	/* 0x00047A0F 220000803F   */ IL_1287: ldc.r4    1
// 	/* 0x00047A14 1149         */ IL_128C: ldloc.s   V_73
// 	/* 0x00047A16 59           */ IL_128E: sub
// 	/* 0x00047A17 1145         */ IL_128F: ldloc.s   V_69
// 	/* 0x00047A19 1145         */ IL_1291: ldloc.s   V_69
// 	/* 0x00047A1B 1146         */ IL_1293: ldloc.s   V_70
// 	/* 0x00047A1D 58           */ IL_1295: add
// 	/* 0x00047A1E 5B           */ IL_1296: div
// 	/* 0x00047A1F 5A           */ IL_1297: mul
// 	/* 0x00047A20 7D31180004   */ IL_1298: stfld     float32 AnimData::time
// 	/* 0x00047A25 2A           */ IL_129D: ret

// 	/* 0x00047A26 02           */ IL_129E: ldarg.0
// 	/* 0x00047A27 7BA7060004   */ IL_129F: ldfld     int32 DFRelayComponent::stage
// 	/* 0x00047A2C 18           */ IL_12A4: ldc.i4.2
// 	/* 0x00047A2D 3F45010000   */ IL_12A5: blt       IL_13EF

// 	/* 0x00047A32 0E05         */ IL_12AA: ldarg.s   animData
// 	/* 0x00047A34 7B32180004   */ IL_12AC: ldfld     float32 AnimData::prepare_length
// 	/* 0x00047A39 134C         */ IL_12B1: stloc.s   V_76
// 	/* 0x00047A3B 0E05         */ IL_12B3: ldarg.s   animData
// 	/* 0x00047A3D 7B33180004   */ IL_12B5: ldfld     float32 AnimData::working_length
// 	/* 0x00047A42 134D         */ IL_12BA: stloc.s   V_77
// 	/* 0x00047A44 02           */ IL_12BC: ldarg.0
// 	/* 0x00047A45 7BA6060004   */ IL_12BD: ldfld     int32 DFRelayComponent::direction
// 	/* 0x00047A4A 6B           */ IL_12C2: conv.r4
// 	/* 0x00047A4B 2200000000   */ IL_12C3: ldc.r4    0.0
// 	/* 0x00047A50 4384000000   */ IL_12C8: ble.un    IL_1351

// 	/* 0x00047A55 02           */ IL_12CD: ldarg.0
// 	/* 0x00047A56 02           */ IL_12CE: ldarg.0
// 	/* 0x00047A57 7BA9060004   */ IL_12CF: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047A5C 226588083C   */ IL_12D4: ldc.r4    0.0083333
// 	/* 0x00047A61 58           */ IL_12D9: add
// 	/* 0x00047A62 7DA9060004   */ IL_12DA: stfld     float32 DFRelayComponent::param0
// 	/* 0x00047A67 0E05         */ IL_12DF: ldarg.s   animData
// 	/* 0x00047A69 02           */ IL_12E1: ldarg.0
// 	/* 0x00047A6A 7BA9060004   */ IL_12E2: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047A6F 114C         */ IL_12E7: ldloc.s   V_76
// 	/* 0x00047A71 114D         */ IL_12E9: ldloc.s   V_77
// 	/* 0x00047A73 58           */ IL_12EB: add
// 	/* 0x00047A74 5B           */ IL_12EC: div
// 	/* 0x00047A75 7D31180004   */ IL_12ED: stfld     float32 AnimData::time
// 	/* 0x00047A7A 02           */ IL_12F2: ldarg.0
// 	/* 0x00047A7B 7BA9060004   */ IL_12F3: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047A80 114C         */ IL_12F8: ldloc.s   V_76
// 	/* 0x00047A82 114D         */ IL_12FA: ldloc.s   V_77
// 	/* 0x00047A84 58           */ IL_12FC: add
// 	/* 0x00047A85 44A3000000   */ IL_12FD: blt.un    IL_13A5

// 	/* 0x00047A8A 02           */ IL_1302: ldarg.0
// 	/* 0x00047A8B 2200000000   */ IL_1303: ldc.r4    0.0
// 	/* 0x00047A90 7DA9060004   */ IL_1308: stfld     float32 DFRelayComponent::param0
// 	/* 0x00047A95 0E04         */ IL_130D: ldarg.s   enemyData
// 	/* 0x00047A97 02           */ IL_130F: ldarg.0
// 	/* 0x00047A98 7B9F060004   */ IL_1310: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00047A9D 28BC020006   */ IL_1315: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 	/* 0x00047AA2 7D1E1A0004   */ IL_131A: stfld     valuetype VectorLF3 EnemyData::pos
// 	/* 0x00047AA7 0E04         */ IL_131F: ldarg.s   enemyData
// 	/* 0x00047AA9 02           */ IL_1321: ldarg.0
// 	/* 0x00047AAA 7B9F060004   */ IL_1322: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 DFRelayComponent::targetLPos
// 	/* 0x00047AAF 02           */ IL_1327: ldarg.0
// 	/* 0x00047AB0 7BA0060004   */ IL_1328: ldfld     float32 DFRelayComponent::targetYaw
// 	/* 0x00047AB5 220000B443   */ IL_132D: ldc.r4    360
// 	/* 0x00047ABA 5A           */ IL_1332: mul
// 	/* 0x00047ABB 281C040006   */ IL_1333: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion Maths::SphericalRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 	/* 0x00047AC0 7D1F1A0004   */ IL_1338: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion EnemyData::rot
// 	/* 0x00047AC5 02           */ IL_133D: ldarg.0
// 	/* 0x00047AC6 2873070006   */ IL_133E: call      instance void DFRelayComponent::ArriveBase()
// 	/* 0x00047ACB 0E05         */ IL_1343: ldarg.s   animData
// 	/* 0x00047ACD 220000803F   */ IL_1345: ldc.r4    1
// 	/* 0x00047AD2 7D31180004   */ IL_134A: stfld     float32 AnimData::time
// 	/* 0x00047AD7 2B54         */ IL_134F: br.s      IL_13A5

// 	/* 0x00047AD9 02           */ IL_1351: ldarg.0
// 	/* 0x00047ADA 02           */ IL_1352: ldarg.0
// 	/* 0x00047ADB 7BA9060004   */ IL_1353: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047AE0 2270CE083D   */ IL_1358: ldc.r4    0.0334
// 	/* 0x00047AE5 58           */ IL_135D: add
// 	/* 0x00047AE6 7DA9060004   */ IL_135E: stfld     float32 DFRelayComponent::param0
// 	/* 0x00047AEB 0E05         */ IL_1363: ldarg.s   animData
// 	/* 0x00047AED 220000803F   */ IL_1365: ldc.r4    1
// 	/* 0x00047AF2 02           */ IL_136A: ldarg.0
// 	/* 0x00047AF3 7BA9060004   */ IL_136B: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047AF8 59           */ IL_1370: sub
// 	/* 0x00047AF9 114D         */ IL_1371: ldloc.s   V_77
// 	/* 0x00047AFB 5A           */ IL_1373: mul
// 	/* 0x00047AFC 114C         */ IL_1374: ldloc.s   V_76
// 	/* 0x00047AFE 58           */ IL_1376: add
// 	/* 0x00047AFF 114C         */ IL_1377: ldloc.s   V_76
// 	/* 0x00047B01 114D         */ IL_1379: ldloc.s   V_77
// 	/* 0x00047B03 58           */ IL_137B: add
// 	/* 0x00047B04 5B           */ IL_137C: div
// 	/* 0x00047B05 7D31180004   */ IL_137D: stfld     float32 AnimData::time
// 	/* 0x00047B0A 02           */ IL_1382: ldarg.0
// 	/* 0x00047B0B 7BA9060004   */ IL_1383: ldfld     float32 DFRelayComponent::param0
// 	/* 0x00047B10 220000803F   */ IL_1388: ldc.r4    1
// 	/* 0x00047B15 3616         */ IL_138D: ble.un.s  IL_13A5

// 	/* 0x00047B17 02           */ IL_138F: ldarg.0
// 	/* 0x00047B18 17           */ IL_1390: ldc.i4.1
// 	/* 0x00047B19 7DA7060004   */ IL_1391: stfld     int32 DFRelayComponent::stage
// 	/* 0x00047B1E 0E05         */ IL_1396: ldarg.s   animData
// 	/* 0x00047B20 114C         */ IL_1398: ldloc.s   V_76
// 	/* 0x00047B22 114C         */ IL_139A: ldloc.s   V_76
// 	/* 0x00047B24 114D         */ IL_139C: ldloc.s   V_77
// 	/* 0x00047B26 58           */ IL_139E: add
// 	/* 0x00047B27 5B           */ IL_139F: div
// 	/* 0x00047B28 7D31180004   */ IL_13A0: stfld     float32 AnimData::time

// 	/* 0x00047B2D 02           */ IL_13A5: ldarg.0
// 	/* 0x00047B2E 2200000000   */ IL_13A6: ldc.r4    0.0
// 	/* 0x00047B33 7DA8060004   */ IL_13AB: stfld     float32 DFRelayComponent::uSpeed
// 	/* 0x00047B38 0E04         */ IL_13B0: ldarg.s   enemyData
// 	/* 0x00047B3A 7C201A0004   */ IL_13B2: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047B3F 2200000000   */ IL_13B7: ldc.r4    0.0
// 	/* 0x00047B44 7D4100000A   */ IL_13BC: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 	/* 0x00047B49 0E04         */ IL_13C1: ldarg.s   enemyData
// 	/* 0x00047B4B 7C201A0004   */ IL_13C3: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047B50 2200000000   */ IL_13C8: ldc.r4    0.0
// 	/* 0x00047B55 7D4200000A   */ IL_13CD: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 	/* 0x00047B5A 0E04         */ IL_13D2: ldarg.s   enemyData
// 	/* 0x00047B5C 7C201A0004   */ IL_13D4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 EnemyData::vel
// 	/* 0x00047B61 2200000000   */ IL_13D9: ldc.r4    0.0
// 	/* 0x00047B66 7D8000000A   */ IL_13DE: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 	/* 0x00047B6B 0E05         */ IL_13E3: ldarg.s   animData
// 	/* 0x00047B6D 22000080BF   */ IL_13E5: ldc.r4    -1
// 	/* 0x00047B72 7D35180004   */ IL_13EA: stfld     float32 AnimData::power

// 	/* 0x00047B77 2A           */ IL_13EF: ret
// } // end of method DFRelayComponent::RelaySailLogic

