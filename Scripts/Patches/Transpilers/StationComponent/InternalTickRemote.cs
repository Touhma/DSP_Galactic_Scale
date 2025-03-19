using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnStationComponent
    {
        // Three patches being made to StationComponent.InternalTickRemote:
        // 1. Allow logistics vessels to path in systems up to 100 astrobodies, up from 10.
        // 2. Allow logistics vessels to get much closer to stars, rather than staying 2.5x radius away.
        //    Makes planets near huge stars reachable by ship.
        // 3. Adjust star radius for pathing calculations to fix issues with large stars
        // - Updated pattern matching to be more robust and find the correct insertion points
        // [HarmonyDebug]
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.InternalTickRemote))]
        public static IEnumerable<CodeInstruction> InternalTickRemoteTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeMatcher = new CodeMatcher(instructions, il).MatchForward(false, new CodeMatch(op => op.opcode == OpCodes.Ldc_I4_S && op.OperandIs(10))); // Search for ldc.i4.s 10

            if (codeMatcher.IsInvalid)
            {
                GS2.Error("InternalTickRemote Transpiler Failed");
                return instructions;
            }

            instructions = codeMatcher.Repeat(z => z // Repeat for all occurences 
                    .Set(OpCodes.Ldc_I4_S, 100)) // Replace operand with 100
                .InstructionEnumeration();
            return instructions;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static IEnumerable<CodeInstruction> InternalTickRemoteTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeMatcher = new CodeMatcher(instructions, il).MatchForward(false, new CodeMatch(op => op.opcode == OpCodes.Ldc_R4 && op.OperandIs(2.5f))); // Search for ldc.r4 2.5f
            if (codeMatcher.IsInvalid)
            {
                GS2.Error("InternalTickRemote 2nd Transpiler Failed");
                return instructions;
            }

            instructions = codeMatcher.Repeat(z => z // Repeat for all occurences
                    .Set(OpCodes.Ldc_R4, 0.5f)) // Replace operand with 0.05f
                .InstructionEnumeration();

            return instructions;
        }
        
        // PATCH 3: Fix pathing issues with large stars by adjusting radius for pathing - more robust approach
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static IEnumerable<CodeInstruction> InternalTickRemoteRadiusAdjustTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            
            // From IL code, we know there's a 2.5f constant at offset 0x000902CF (IL_1393)
            // First approach: Look for the specific 2.5f constant
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && Math.Abs((float)codes[i].operand - 2.5f) < 0.01f)
                {
                    GS2.Log($"Found 2.5f constant at index {i}, replacing with 0.5f");
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, 0.5f);
                    patched = true;
                }
            }
            
            // Second approach: Look for the pattern of loading uRadius followed by specific operations
            if (!patched)
            {
                for (int i = 0; i < codes.Count - 5; i++)
                {
                    if (codes[i].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")))
                    {
                        // Look for patterns that indicate this is for pathing calculation
                        bool isPathingContext = false;
                        
                        // Check several common patterns
                        if (i < codes.Count - 3 && codes[i + 2].opcode == OpCodes.Add) isPathingContext = true;
                        else if (i < codes.Count - 3 && codes[i + 2].opcode == OpCodes.Mul) isPathingContext = true;
                        else if (i < codes.Count - 3 && codes[i + 1].IsStloc()) isPathingContext = true;
                        
                        if (isPathingContext)
                        {
                            codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                                AccessTools.Method(typeof(PatchOnStationComponent), nameof(AdjustRadiusForPathing))));
                            patched = true;
                            i++; // Skip the inserted instruction
                            GS2.Log($"Inserted radius adjustment at index {i}");
                        }
                    }
                }
            }
            
            // Last resort approach: Patch select uRadius accesses
            if (!patched)
            {
                GS2.Warn("Standard approaches failed, will patch all uRadius accesses");
                int count = 0;
                
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    if (codes[i].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")))
                    {
                        // Insert our adjustment after every uRadius load
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                            AccessTools.Method(typeof(PatchOnStationComponent), nameof(AdjustRadiusForPathing))));
                        
                        i++; // Skip our inserted instruction
                        count++;
                    }
                }
                
                if (count > 0)
                {
                    GS2.Log($"Inserted radius adjustment at all {count} uRadius loads (fallback approach)");
                    patched = true;
                }
            }
            
            if (patched)
            {
                GS2.Log("Successfully patched StationComponent.InternalTickRemote for better pathing with large stars");
            }
            else
            {
                GS2.Error("All approaches failed to patch star radius in StationComponent.InternalTickRemote");
            }
            
            return codes;
        }
        
        // Scaling function for avoidance calculations
        public static float AdjustRadiusForPathing(float originalRadius)
        {
            // For small stars (default game sizes), keep the original radius
            if (originalRadius < 1000f)
                return originalRadius;
                
            // For larger stars, apply a logarithmic scale to avoid excessive avoidance distances
            // This ensures ships still avoid the star but don't take extreme detours
            float adjustedRadius = 1000f + (float)(Math.Log10(originalRadius / 1000f + 1) * 1000f);
            
            // Ensure we never go below 60% of the original radius to avoid ships hitting stars
            return Math.Max(originalRadius * 0.6f, adjustedRadius);
        }
        
        // Patch for CalcRemoteSingleTripTime to adjust trip time calculations
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), "CalcRemoteSingleTripTime")]
        public static IEnumerable<CodeInstruction> CalcRemoteSingleTripTimeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            
            // Find hardcoded constants in path calculation
            for (int i = 0; i < codes.Count - 2; i++)
            {
                // Look for hardcoded distance values like 5000f
                if (codes[i].opcode == OpCodes.Ldc_R4 && 
                    (float)codes[i].operand >= 4900f && (float)codes[i].operand <= 5100f)
                {
                    // Insert our adjustment method call
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(PatchOnStationComponent), nameof(AdjustSafetyDistanceForPathCalculation))));
                    patched = true;
                    i += 2; // Skip ahead since we modified the list
                }
            }
            
            if (patched)
            {
                GS2.Log("Patched StationComponent.CalcRemoteSingleTripTime for better pathing with large stars");
            }
            else
            {
                GS2.Warn("Failed to patch StationComponent.CalcRemoteSingleTripTime for large stars");
            }
            
            return codes;
        }
        
        // Adjust safety distance for path calculation
        public static float AdjustSafetyDistanceForPathCalculation(float originalDistance)
        {
            // Scale safety distance based on largest star radius in the galaxy
            float largestStarRadius = GetLargestStarRadius();
            
            if (largestStarRadius <= 1000f)
                return originalDistance;
                
            // Scale the safety distance with diminishing returns
            float factor = 1f + Mathf.Log10(largestStarRadius / 1000f) * 0.75f;
            return originalDistance * factor;
        }
        
        // Helper to find the largest star radius in the galaxy
        private static float largestCachedRadius = 0f;
        private static int lastUpdateFrame = -1;
        
        private static float GetLargestStarRadius()
        {
            // Cache this value as it's expensive to calculate
            if (Time.frameCount - lastUpdateFrame > 300 || largestCachedRadius <= 0f)
            {
                lastUpdateFrame = Time.frameCount;
                largestCachedRadius = 0f;
                
                if (GameMain.galaxy?.stars != null)
                {
                    foreach (StarData star in GameMain.galaxy.stars)
                    {
                        if (star != null && star.physicsRadius > largestCachedRadius)
                        {
                            largestCachedRadius = star.physicsRadius;
                        }
                    }
                }
                
                // Fallback if we couldn't find a valid radius
                if (largestCachedRadius <= 0f)
                    largestCachedRadius = 1000f;
            }
            
            return largestCachedRadius;
        }
    }
}

// Original IL:

// // Token: 0x060009FD RID: 2557 RVA: 0x00090D30 File Offset: 0x0008EF30
// .method public hidebysig 
// 	instance void InternalTickRemote (
// 		class PlanetFactory factory,
// 		int32 timeGene,
// 		float32 shipSailSpeed,
// 		float32 shipWarpSpeed,
// 		int32 shipCarries,
// 		class StationComponent[] gStationPool,
// 		valuetype AstroData[] astroPoses,
// 		valuetype VectorLF3& relativePos,
// 		valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion& relativeRot,
// 		bool starmap,
// 		int32[] consumeRegister
// 	) cil managed 
// {
// 	// Header Size: 12 bytes
// 	// Code Size: 12553 (0x3109) bytes
// 	// LocalVarSig Token: 0x110001FB RID: 507
// 	.maxstack 8
// 	.locals init (
// 		[0] bool,
// 		[1] int32,
// 		[2] int32,
// 		[3] int32,
// 		[4] int32,
// 		[5] int32,
// 		[6] int32,
// 		[7] int32,
// 		[8] float32,
// 		[9] float32,
// 		[10] float32,
// 		[11] valuetype AstroData&,
// 		[12] float32,
// 		[13] float32,
// 		[14] float32,
// 		[15] float32,
// 		[16] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[17] valuetype VectorLF3,
// 		[18] float64,
// 		[19] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[20] valuetype StationStore[],
// 		[21] bool,
// 		[22] int32,
// 		[23] int32,
// 		[24] int32,
// 		[25] valuetype ShipData&,
// 		[26] valuetype ShipRenderingData&,
// 		[27] bool,
// 		[28] valuetype AstroData&,
// 		[29] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[30] float32,
// 		[31] class TrafficStatistics,
// 		[32] int32,
// 		[33] int32,
// 		[34] float32,
// 		[35] valuetype VectorLF3,
// 		[36] valuetype VectorLF3,
// 		[37] valuetype VectorLF3,
// 		[38] valuetype VectorLF3,
// 		[39] float64,
// 		[40] valuetype VectorLF3,
// 		[41] float64,
// 		[42] bool,
// 		[43] bool,
// 		[44] int32,
// 		[45] float32,
// 		[46] valuetype VectorLF3,
// 		[47] float64,
// 		[48] float32,
// 		[49] float32,
// 		[50] float64,
// 		[51] float64,
// 		[52] float64,
// 		[53] float64,
// 		[54] float64,
// 		[55] float64,
// 		[56] float32,
// 		[57] int32,
// 		[58] float64,
// 		[59] float64,
// 		[60] float32,
// 		[61] float32,
// 		[62] int32,
// 		[63] int32,
// 		[64] valuetype VectorLF3,
// 		[65] float32,
// 		[66] int32,
// 		[67] float32,
// 		[68] valuetype VectorLF3&,
// 		[69] float32,
// 		[70] float64,
// 		[71] float64,
// 		[72] int32,
// 		[73] float32,
// 		[74] valuetype VectorLF3&,
// 		[75] float32,
// 		[76] float64,
// 		[77] float64,
// 		[78] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[79] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[80] float32,
// 		[81] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[82] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[83] float32,
// 		[84] float32,
// 		[85] float32,
// 		[86] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[87] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[88] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[89] float32,
// 		[90] float32,
// 		[91] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[92] float32,
// 		[93] float32,
// 		[94] valuetype AstroData&,
// 		[95] float32,
// 		[96] float64,
// 		[97] float64,
// 		[98] float64,
// 		[99] float64,
// 		[100] float64,
// 		[101] float64,
// 		[102] float64,
// 		[103] float64,
// 		[104] valuetype VectorLF3,
// 		[105] float64,
// 		[106] valuetype VectorLF3,
// 		[107] float64,
// 		[108] float64,
// 		[109] valuetype VectorLF3,
// 		[110] float64,
// 		[111] valuetype VectorLF3,
// 		[112] valuetype VectorLF3,
// 		[113] float32,
// 		[114] float32,
// 		[115] float32,
// 		[116] float32,
// 		[117] valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion,
// 		[118] float32,
// 		[119] float32,
// 		[120] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[121] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[122] valuetype [UnityEngine.CoreModule]UnityEngine.Vector3,
// 		[123] float32,
// 		[124] float32,
// 		[125] float32,
// 		[126] valuetype VectorLF3,
// 		[127] valuetype VectorLF3,
// 		[128] valuetype VectorLF3,
// 		[129] class StationComponent,
// 		[130] valuetype StationStore[],
// 		[131] int32[],
// 		[132] class TrafficStatistics,
// 		[133] int32,
// 		[134] int32,
// 		[135] int32,
// 		[136] int32,
// 		[137] bool,
// 		[138] int32,
// 		[139] int32,
// 		[140] int32,
// 		[141] int32,
// 		[142] int32,
// 		[143] valuetype SupplyDemandPair,
// 		[144] int32,
// 		[145] int32,
// 		[146] int32,
// 		[147] int32,
// 		[148] int32,
// 		[149] int32,
// 		[150] int32,
// 		[151] int32,
// 		[152] class TrafficStatistics,
// 		[153] int32,
// 		[154] int32,
// 		[155] int32,
// 		[156] int32
// 	)

// 	/* 0x0008EF3C 0E04         */ IL_0000: ldarg.s   shipWarpSpeed
// 	/* 0x0008EF3E 05           */ IL_0002: ldarg.3
// 	/* 0x0008EF3F 220000803F   */ IL_0003: ldc.r4    1
// 	/* 0x0008EF44 58           */ IL_0008: add
// 	/* 0x0008EF45 FE02         */ IL_0009: cgt
// 	/* 0x0008EF47 0A           */ IL_000B: stloc.0
// 	/* 0x0008EF48 02           */ IL_000C: ldarg.0
// 	/* 0x0008EF49 7EC4330004   */ IL_000D: ldsfld    bool DSPGame::IsMenuDemo
// 	/* 0x0008EF4E 7D070C0004   */ IL_0012: stfld     bool StationComponent::warperFree
// 	/* 0x0008EF53 02           */ IL_0017: ldarg.0
// 	/* 0x0008EF54 7BD10B0004   */ IL_0018: ldfld     int32 StationComponent::warperCount
// 	/* 0x0008EF59 02           */ IL_001D: ldarg.0
// 	/* 0x0008EF5A 7BD20B0004   */ IL_001E: ldfld     int32 StationComponent::warperMaxCount
// 	/* 0x0008EF5F 3CD3000000   */ IL_0023: bge       IL_00FB

// 	/* 0x0008EF64 02           */ IL_0028: ldarg.0
// 	/* 0x0008EF65 7BE00B0004   */ IL_0029: ldfld     valuetype StationStore[] StationComponent::'storage'
// 	/* 0x0008EF6A 1314         */ IL_002E: stloc.s   V_20
// 	/* 0x0008EF6C 16           */ IL_0030: ldc.i4.0
// 	/* 0x0008EF6D 1315         */ IL_0031: stloc.s   V_21
// 	.try
// 	{
// 		/* 0x0008EF6F 1114         */ IL_0033: ldloc.s   V_20
// 		/* 0x0008EF71 1215         */ IL_0035: ldloca.s  V_21
// 		/* 0x0008EF73 287502000A   */ IL_0037: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 		/* 0x0008EF78 16           */ IL_003C: ldc.i4.0
// 		/* 0x0008EF79 1316         */ IL_003D: stloc.s   V_22
// 		/* 0x0008EF7B 389A000000   */ IL_003F: br        IL_00DE
// 		// loop start (head: IL_00DE)
// 			/* 0x0008EF80 02           */ IL_0044: ldarg.0
// 			/* 0x0008EF81 7BE00B0004   */ IL_0045: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008EF86 1116         */ IL_004A: ldloc.s   V_22
// 			/* 0x0008EF88 8F54010002   */ IL_004C: ldelema   StationStore
// 			/* 0x0008EF8D 7B1A0C0004   */ IL_0051: ldfld     int32 StationStore::itemId
// 			/* 0x0008EF92 20BA040000   */ IL_0056: ldc.i4    1210
// 			/* 0x0008EF97 337B         */ IL_005B: bne.un.s  IL_00D8

// 			/* 0x0008EF99 02           */ IL_005D: ldarg.0
// 			/* 0x0008EF9A 7BE00B0004   */ IL_005E: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008EF9F 1116         */ IL_0063: ldloc.s   V_22
// 			/* 0x0008EFA1 8F54010002   */ IL_0065: ldelema   StationStore
// 			/* 0x0008EFA6 7B1B0C0004   */ IL_006A: ldfld     int32 StationStore::count
// 			/* 0x0008EFAB 16           */ IL_006F: ldc.i4.0
// 			/* 0x0008EFAC 3166         */ IL_0070: ble.s     IL_00D8

// 			/* 0x0008EFAE 02           */ IL_0072: ldarg.0
// 			/* 0x0008EFAF 02           */ IL_0073: ldarg.0
// 			/* 0x0008EFB0 7BD10B0004   */ IL_0074: ldfld     int32 StationComponent::warperCount
// 			/* 0x0008EFB5 17           */ IL_0079: ldc.i4.1
// 			/* 0x0008EFB6 58           */ IL_007A: add
// 			/* 0x0008EFB7 7DD10B0004   */ IL_007B: stfld     int32 StationComponent::warperCount
// 			/* 0x0008EFBC 02           */ IL_0080: ldarg.0
// 			/* 0x0008EFBD 7BE00B0004   */ IL_0081: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008EFC2 1116         */ IL_0086: ldloc.s   V_22
// 			/* 0x0008EFC4 8F54010002   */ IL_0088: ldelema   StationStore
// 			/* 0x0008EFC9 7B1C0C0004   */ IL_008D: ldfld     int32 StationStore::inc
// 			/* 0x0008EFCE 02           */ IL_0092: ldarg.0
// 			/* 0x0008EFCF 7BE00B0004   */ IL_0093: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008EFD4 1116         */ IL_0098: ldloc.s   V_22
// 			/* 0x0008EFD6 8F54010002   */ IL_009A: ldelema   StationStore
// 			/* 0x0008EFDB 7B1B0C0004   */ IL_009F: ldfld     int32 StationStore::count
// 			/* 0x0008EFE0 5B           */ IL_00A4: div
// 			/* 0x0008EFE1 1317         */ IL_00A5: stloc.s   V_23
// 			/* 0x0008EFE3 02           */ IL_00A7: ldarg.0
// 			/* 0x0008EFE4 7BE00B0004   */ IL_00A8: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008EFE9 1116         */ IL_00AD: ldloc.s   V_22
// 			/* 0x0008EFEB 8F54010002   */ IL_00AF: ldelema   StationStore
// 			/* 0x0008EFF0 7C1B0C0004   */ IL_00B4: ldflda    int32 StationStore::count
// 			/* 0x0008EFF5 25           */ IL_00B9: dup
// 			/* 0x0008EFF6 4A           */ IL_00BA: ldind.i4
// 			/* 0x0008EFF7 17           */ IL_00BB: ldc.i4.1
// 			/* 0x0008EFF8 59           */ IL_00BC: sub
// 			/* 0x0008EFF9 54           */ IL_00BD: stind.i4
// 			/* 0x0008EFFA 02           */ IL_00BE: ldarg.0
// 			/* 0x0008EFFB 7BE00B0004   */ IL_00BF: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008F000 1116         */ IL_00C4: ldloc.s   V_22
// 			/* 0x0008F002 8F54010002   */ IL_00C6: ldelema   StationStore
// 			/* 0x0008F007 7C1C0C0004   */ IL_00CB: ldflda    int32 StationStore::inc
// 			/* 0x0008F00C 25           */ IL_00D0: dup
// 			/* 0x0008F00D 4A           */ IL_00D1: ldind.i4
// 			/* 0x0008F00E 1117         */ IL_00D2: ldloc.s   V_23
// 			/* 0x0008F010 59           */ IL_00D4: sub
// 			/* 0x0008F011 54           */ IL_00D5: stind.i4
// 			/* 0x0008F012 DE23         */ IL_00D6: leave.s   IL_00FB

// 			/* 0x0008F014 1116         */ IL_00D8: ldloc.s   V_22
// 			/* 0x0008F016 17           */ IL_00DA: ldc.i4.1
// 			/* 0x0008F017 58           */ IL_00DB: add
// 			/* 0x0008F018 1316         */ IL_00DC: stloc.s   V_22

// 			/* 0x0008F01A 1116         */ IL_00DE: ldloc.s   V_22
// 			/* 0x0008F01C 02           */ IL_00E0: ldarg.0
// 			/* 0x0008F01D 7BE00B0004   */ IL_00E1: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008F022 8E           */ IL_00E6: ldlen
// 			/* 0x0008F023 69           */ IL_00E7: conv.i4
// 			/* 0x0008F024 3F57FFFFFF   */ IL_00E8: blt       IL_0044
// 		// end loop

// 		/* 0x0008F029 DE0C         */ IL_00ED: leave.s   IL_00FB
// 	} // end .try
// 	finally
// 	{
// 		/* 0x0008F02B 1115         */ IL_00EF: ldloc.s   V_21
// 		/* 0x0008F02D 2C07         */ IL_00F1: brfalse.s IL_00FA

// 		/* 0x0008F02F 1114         */ IL_00F3: ldloc.s   V_20
// 		/* 0x0008F031 287602000A   */ IL_00F5: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 		/* 0x0008F036 DC           */ IL_00FA: endfinally
// 	} // end handler

// 	/* 0x0008F037 16           */ IL_00FB: ldc.i4.0
// 	/* 0x0008F038 0B           */ IL_00FC: stloc.1
// 	/* 0x0008F039 16           */ IL_00FD: ldc.i4.0
// 	/* 0x0008F03A 0C           */ IL_00FE: stloc.2
// 	/* 0x0008F03B 16           */ IL_00FF: ldc.i4.0
// 	/* 0x0008F03C 0D           */ IL_0100: stloc.3
// 	/* 0x0008F03D 16           */ IL_0101: ldc.i4.0
// 	/* 0x0008F03E 1304         */ IL_0102: stloc.s   V_4
// 	/* 0x0008F040 16           */ IL_0104: ldc.i4.0
// 	/* 0x0008F041 1305         */ IL_0105: stloc.s   V_5
// 	/* 0x0008F043 16           */ IL_0107: ldc.i4.0
// 	/* 0x0008F044 1306         */ IL_0108: stloc.s   V_6
// 	/* 0x0008F046 16           */ IL_010A: ldc.i4.0
// 	/* 0x0008F047 1307         */ IL_010B: stloc.s   V_7
// 	/* 0x0008F049 05           */ IL_010D: ldarg.3
// 	/* 0x0008F04A 2200001644   */ IL_010E: ldc.r4    600
// 	/* 0x0008F04F 5B           */ IL_0113: div
// 	/* 0x0008F050 1308         */ IL_0114: stloc.s   V_8
// 	/* 0x0008F052 1108         */ IL_0116: ldloc.s   V_8
// 	/* 0x0008F054 22CDCCCC3E   */ IL_0118: ldc.r4    0.4
// 	/* 0x0008F059 28A300000A   */ IL_011D: call      float32 [UnityEngine.CoreModule]UnityEngine.Mathf::Pow(float32, float32)
// 	/* 0x0008F05E 1309         */ IL_0122: stloc.s   V_9
// 	/* 0x0008F060 1109         */ IL_0124: ldloc.s   V_9
// 	/* 0x0008F062 130A         */ IL_0126: stloc.s   V_10
// 	/* 0x0008F064 110A         */ IL_0128: ldloc.s   V_10
// 	/* 0x0008F066 220000803F   */ IL_012A: ldc.r4    1
// 	/* 0x0008F06B 360F         */ IL_012F: ble.un.s  IL_0140

// 	/* 0x0008F06D 110A         */ IL_0131: ldloc.s   V_10
// 	/* 0x0008F06F 289303000A   */ IL_0133: call      float32 [UnityEngine.CoreModule]UnityEngine.Mathf::Log(float32)
// 	/* 0x0008F074 220000803F   */ IL_0138: ldc.r4    1
// 	/* 0x0008F079 58           */ IL_013D: add
// 	/* 0x0008F07A 130A         */ IL_013E: stloc.s   V_10

// 	/* 0x0008F07C 1108         */ IL_0140: ldloc.s   V_8
// 	/* 0x0008F07E 220000FA43   */ IL_0142: ldc.r4    500
// 	/* 0x0008F083 3607         */ IL_0147: ble.un.s  IL_0150

// 	/* 0x0008F085 220000FA43   */ IL_0149: ldc.r4    500
// 	/* 0x0008F08A 1308         */ IL_014E: stloc.s   V_8

// 	/* 0x0008F08C 0E07         */ IL_0150: ldarg.s   astroPoses
// 	/* 0x0008F08E 02           */ IL_0152: ldarg.0
// 	/* 0x0008F08F 7BC40B0004   */ IL_0153: ldfld     int32 StationComponent::planetId
// 	/* 0x0008F094 8FE7010002   */ IL_0158: ldelema   AstroData
// 	/* 0x0008F099 130B         */ IL_015D: stloc.s   V_11
// 	/* 0x0008F09B 05           */ IL_015F: ldarg.3
// 	/* 0x0008F09C 228FC2F53C   */ IL_0160: ldc.r4    0.03
// 	/* 0x0008F0A1 5A           */ IL_0165: mul
// 	/* 0x0008F0A2 130C         */ IL_0166: stloc.s   V_12
// 	/* 0x0008F0A4 05           */ IL_0168: ldarg.3
// 	/* 0x0008F0A5 228FC2F53D   */ IL_0169: ldc.r4    0.12
// 	/* 0x0008F0AA 5A           */ IL_016E: mul
// 	/* 0x0008F0AB 110A         */ IL_016F: ldloc.s   V_10
// 	/* 0x0008F0AD 5A           */ IL_0171: mul
// 	/* 0x0008F0AE 130D         */ IL_0172: stloc.s   V_13
// 	/* 0x0008F0B0 05           */ IL_0174: ldarg.3
// 	/* 0x0008F0B1 22CDCCCC3E   */ IL_0175: ldc.r4    0.4
// 	/* 0x0008F0B6 5A           */ IL_017A: mul
// 	/* 0x0008F0B7 1108         */ IL_017B: ldloc.s   V_8
// 	/* 0x0008F0B9 5A           */ IL_017D: mul
// 	/* 0x0008F0BA 130E         */ IL_017E: stloc.s   V_14
// 	/* 0x0008F0BC 1109         */ IL_0180: ldloc.s   V_9
// 	/* 0x0008F0BE 22A69BC43B   */ IL_0182: ldc.r4    0.006
// 	/* 0x0008F0C3 5A           */ IL_0187: mul
// 	/* 0x0008F0C4 22ACC52737   */ IL_0188: ldc.r4    1E-05
// 	/* 0x0008F0C9 58           */ IL_018D: add
// 	/* 0x0008F0CA 130F         */ IL_018E: stloc.s   V_15
// 	/* 0x0008F0CC 1210         */ IL_0190: ldloca.s  V_16
// 	/* 0x0008F0CE 2200000000   */ IL_0192: ldc.r4    0.0
// 	/* 0x0008F0D3 2200000000   */ IL_0197: ldc.r4    0.0
// 	/* 0x0008F0D8 2200000000   */ IL_019C: ldc.r4    0.0
// 	/* 0x0008F0DD 283B00000A   */ IL_01A1: call      instance void [UnityEngine.CoreModule]UnityEngine.Vector3::.ctor(float32, float32, float32)
// 	/* 0x0008F0E2 1211         */ IL_01A6: ldloca.s  V_17
// 	/* 0x0008F0E4 2200000000   */ IL_01A8: ldc.r4    0.0
// 	/* 0x0008F0E9 2200000000   */ IL_01AD: ldc.r4    0.0
// 	/* 0x0008F0EE 2200000000   */ IL_01B2: ldc.r4    0.0
// 	/* 0x0008F0F3 28B3020006   */ IL_01B7: call      instance void VectorLF3::.ctor(float32, float32, float32)
// 	/* 0x0008F0F8 230000000000000000 */ IL_01BC: ldc.r8    0.0
// 	/* 0x0008F101 1312         */ IL_01C5: stloc.s   V_18
// 	/* 0x0008F103 1213         */ IL_01C7: ldloca.s  V_19
// 	/* 0x0008F105 2200000000   */ IL_01C9: ldc.r4    0.0
// 	/* 0x0008F10A 2200000000   */ IL_01CE: ldc.r4    0.0
// 	/* 0x0008F10F 2200000000   */ IL_01D3: ldc.r4    0.0
// 	/* 0x0008F114 220000803F   */ IL_01D8: ldc.r4    1
// 	/* 0x0008F119 285702000A   */ IL_01DD: call      instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 	/* 0x0008F11E 16           */ IL_01E2: ldc.i4.0
// 	/* 0x0008F11F 1318         */ IL_01E3: stloc.s   V_24
// 	/* 0x0008F121 38842E0000   */ IL_01E5: br        IL_306E
// 	// loop start (head: IL_306E)
// 		/* 0x0008F126 02           */ IL_01EA: ldarg.0
// 		/* 0x0008F127 7BDB0B0004   */ IL_01EB: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F12C 1118         */ IL_01F0: ldloc.s   V_24
// 		/* 0x0008F12E 8F57010002   */ IL_01F2: ldelema   ShipData
// 		/* 0x0008F133 1319         */ IL_01F7: stloc.s   V_25
// 		/* 0x0008F135 02           */ IL_01F9: ldarg.0
// 		/* 0x0008F136 7BDE0B0004   */ IL_01FA: ldfld     valuetype ShipRenderingData[] StationComponent::shipRenderers
// 		/* 0x0008F13B 1119         */ IL_01FF: ldloc.s   V_25
// 		/* 0x0008F13D 7B440C0004   */ IL_0201: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F142 8F58010002   */ IL_0206: ldelema   ShipRenderingData
// 		/* 0x0008F147 131A         */ IL_020B: stloc.s   V_26
// 		/* 0x0008F149 16           */ IL_020D: ldc.i4.0
// 		/* 0x0008F14A 131B         */ IL_020E: stloc.s   V_27
// 		/* 0x0008F14C 1213         */ IL_0210: ldloca.s  V_19
// 		/* 0x0008F14E 1213         */ IL_0212: ldloca.s  V_19
// 		/* 0x0008F150 1213         */ IL_0214: ldloca.s  V_19
// 		/* 0x0008F152 2200000000   */ IL_0216: ldc.r4    0.0
// 		/* 0x0008F157 25           */ IL_021B: dup
// 		/* 0x0008F158 131E         */ IL_021C: stloc.s   V_30
// 		/* 0x0008F15A 7D5502000A   */ IL_021E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::z
// 		/* 0x0008F15F 111E         */ IL_0223: ldloc.s   V_30
// 		/* 0x0008F161 25           */ IL_0225: dup
// 		/* 0x0008F162 131E         */ IL_0226: stloc.s   V_30
// 		/* 0x0008F164 7D5402000A   */ IL_0228: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::y
// 		/* 0x0008F169 111E         */ IL_022D: ldloc.s   V_30
// 		/* 0x0008F16B 7D5302000A   */ IL_022F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::x
// 		/* 0x0008F170 1213         */ IL_0234: ldloca.s  V_19
// 		/* 0x0008F172 220000803F   */ IL_0236: ldc.r4    1
// 		/* 0x0008F177 7D5602000A   */ IL_023B: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::w
// 		/* 0x0008F17C 0E07         */ IL_0240: ldarg.s   astroPoses
// 		/* 0x0008F17E 1119         */ IL_0242: ldloc.s   V_25
// 		/* 0x0008F180 7B330C0004   */ IL_0244: ldfld     int32 ShipData::planetB
// 		/* 0x0008F185 8FE7010002   */ IL_0249: ldelema   AstroData
// 		/* 0x0008F18A 131C         */ IL_024E: stloc.s   V_28
// 		/* 0x0008F18C 1211         */ IL_0250: ldloca.s  V_17
// 		/* 0x0008F18E 110B         */ IL_0252: ldloc.s   V_11
// 		/* 0x0008F190 7C46180004   */ IL_0254: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F195 7B41030004   */ IL_0259: ldfld     float64 VectorLF3::x
// 		/* 0x0008F19A 111C         */ IL_025E: ldloc.s   V_28
// 		/* 0x0008F19C 7C46180004   */ IL_0260: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F1A1 7B41030004   */ IL_0265: ldfld     float64 VectorLF3::x
// 		/* 0x0008F1A6 59           */ IL_026A: sub
// 		/* 0x0008F1A7 7D41030004   */ IL_026B: stfld     float64 VectorLF3::x
// 		/* 0x0008F1AC 1211         */ IL_0270: ldloca.s  V_17
// 		/* 0x0008F1AE 110B         */ IL_0272: ldloc.s   V_11
// 		/* 0x0008F1B0 7C46180004   */ IL_0274: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F1B5 7B42030004   */ IL_0279: ldfld     float64 VectorLF3::y
// 		/* 0x0008F1BA 111C         */ IL_027E: ldloc.s   V_28
// 		/* 0x0008F1BC 7C46180004   */ IL_0280: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F1C1 7B42030004   */ IL_0285: ldfld     float64 VectorLF3::y
// 		/* 0x0008F1C6 59           */ IL_028A: sub
// 		/* 0x0008F1C7 7D42030004   */ IL_028B: stfld     float64 VectorLF3::y
// 		/* 0x0008F1CC 1211         */ IL_0290: ldloca.s  V_17
// 		/* 0x0008F1CE 110B         */ IL_0292: ldloc.s   V_11
// 		/* 0x0008F1D0 7C46180004   */ IL_0294: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F1D5 7B43030004   */ IL_0299: ldfld     float64 VectorLF3::z
// 		/* 0x0008F1DA 111C         */ IL_029E: ldloc.s   V_28
// 		/* 0x0008F1DC 7C46180004   */ IL_02A0: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F1E1 7B43030004   */ IL_02A5: ldfld     float64 VectorLF3::z
// 		/* 0x0008F1E6 59           */ IL_02AA: sub
// 		/* 0x0008F1E7 7D43030004   */ IL_02AB: stfld     float64 VectorLF3::z
// 		/* 0x0008F1EC 1111         */ IL_02B0: ldloc.s   V_17
// 		/* 0x0008F1EE 7B41030004   */ IL_02B2: ldfld     float64 VectorLF3::x
// 		/* 0x0008F1F3 1111         */ IL_02B7: ldloc.s   V_17
// 		/* 0x0008F1F5 7B41030004   */ IL_02B9: ldfld     float64 VectorLF3::x
// 		/* 0x0008F1FA 5A           */ IL_02BE: mul
// 		/* 0x0008F1FB 1111         */ IL_02BF: ldloc.s   V_17
// 		/* 0x0008F1FD 7B42030004   */ IL_02C1: ldfld     float64 VectorLF3::y
// 		/* 0x0008F202 1111         */ IL_02C6: ldloc.s   V_17
// 		/* 0x0008F204 7B42030004   */ IL_02C8: ldfld     float64 VectorLF3::y
// 		/* 0x0008F209 5A           */ IL_02CD: mul
// 		/* 0x0008F20A 58           */ IL_02CE: add
// 		/* 0x0008F20B 1111         */ IL_02CF: ldloc.s   V_17
// 		/* 0x0008F20D 7B43030004   */ IL_02D1: ldfld     float64 VectorLF3::z
// 		/* 0x0008F212 1111         */ IL_02D6: ldloc.s   V_17
// 		/* 0x0008F214 7B43030004   */ IL_02D8: ldfld     float64 VectorLF3::z
// 		/* 0x0008F219 5A           */ IL_02DD: mul
// 		/* 0x0008F21A 58           */ IL_02DE: add
// 		/* 0x0008F21B 284B02000A   */ IL_02DF: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x0008F220 1312         */ IL_02E4: stloc.s   V_18
// 		/* 0x0008F222 1119         */ IL_02E6: ldloc.s   V_25
// 		/* 0x0008F224 7B3D0C0004   */ IL_02E8: ldfld     int32 ShipData::otherGId
// 		/* 0x0008F229 16           */ IL_02ED: ldc.i4.0
// 		/* 0x0008F22A 301A         */ IL_02EE: bgt.s     IL_030A

// 		/* 0x0008F22C 1119         */ IL_02F0: ldloc.s   V_25
// 		/* 0x0008F22E 15           */ IL_02F2: ldc.i4.m1
// 		/* 0x0008F22F 7D3E0C0004   */ IL_02F3: stfld     int32 ShipData::direction
// 		/* 0x0008F234 1119         */ IL_02F8: ldloc.s   V_25
// 		/* 0x0008F236 7B310C0004   */ IL_02FA: ldfld     int32 ShipData::stage
// 		/* 0x0008F23B 16           */ IL_02FF: ldc.i4.0
// 		/* 0x0008F23C 3108         */ IL_0300: ble.s     IL_030A

// 		/* 0x0008F23E 1119         */ IL_0302: ldloc.s   V_25
// 		/* 0x0008F240 16           */ IL_0304: ldc.i4.0
// 		/* 0x0008F241 7D310C0004   */ IL_0305: stfld     int32 ShipData::stage

// 		/* 0x0008F246 1119         */ IL_030A: ldloc.s   V_25
// 		/* 0x0008F248 7B310C0004   */ IL_030C: ldfld     int32 ShipData::stage
// 		/* 0x0008F24D 15           */ IL_0311: ldc.i4.m1
// 		/* 0x0008F24E 3CD4030000   */ IL_0312: bge       IL_06EB

// 		/* 0x0008F253 1119         */ IL_0317: ldloc.s   V_25
// 		/* 0x0008F255 7B3E0C0004   */ IL_0319: ldfld     int32 ShipData::direction
// 		/* 0x0008F25A 16           */ IL_031E: ldc.i4.0
// 		/* 0x0008F25B 313A         */ IL_031F: ble.s     IL_035B

// 		/* 0x0008F25D 1119         */ IL_0321: ldloc.s   V_25
// 		/* 0x0008F25F 7C3F0C0004   */ IL_0323: ldflda    float32 ShipData::t
// 		/* 0x0008F264 25           */ IL_0328: dup
// 		/* 0x0008F265 4E           */ IL_0329: ldind.r4
// 		/* 0x0008F266 22029A083D   */ IL_032A: ldc.r4    0.03335
// 		/* 0x0008F26B 58           */ IL_032F: add
// 		/* 0x0008F26C 56           */ IL_0330: stind.r4
// 		/* 0x0008F26D 1119         */ IL_0331: ldloc.s   V_25
// 		/* 0x0008F26F 7B3F0C0004   */ IL_0333: ldfld     float32 ShipData::t
// 		/* 0x0008F274 220000803F   */ IL_0338: ldc.r4    1
// 		/* 0x0008F279 437F020000   */ IL_033D: ble.un    IL_05C1

// 		/* 0x0008F27E 1119         */ IL_0342: ldloc.s   V_25
// 		/* 0x0008F280 2200000000   */ IL_0344: ldc.r4    0.0
// 		/* 0x0008F285 7D3F0C0004   */ IL_0349: stfld     float32 ShipData::t
// 		/* 0x0008F28A 1119         */ IL_034E: ldloc.s   V_25
// 		/* 0x0008F28C 15           */ IL_0350: ldc.i4.m1
// 		/* 0x0008F28D 7D310C0004   */ IL_0351: stfld     int32 ShipData::stage
// 		/* 0x0008F292 3866020000   */ IL_0356: br        IL_05C1

// 		/* 0x0008F297 1119         */ IL_035B: ldloc.s   V_25
// 		/* 0x0008F299 7C3F0C0004   */ IL_035D: ldflda    float32 ShipData::t
// 		/* 0x0008F29E 25           */ IL_0362: dup
// 		/* 0x0008F29F 4E           */ IL_0363: ldind.r4
// 		/* 0x0008F2A0 22029A083D   */ IL_0364: ldc.r4    0.03335
// 		/* 0x0008F2A5 59           */ IL_0369: sub
// 		/* 0x0008F2A6 56           */ IL_036A: stind.r4
// 		/* 0x0008F2A7 1119         */ IL_036B: ldloc.s   V_25
// 		/* 0x0008F2A9 7B3F0C0004   */ IL_036D: ldfld     float32 ShipData::t
// 		/* 0x0008F2AE 2200000000   */ IL_0372: ldc.r4    0.0
// 		/* 0x0008F2B3 4145020000   */ IL_0377: bge.un    IL_05C1

// 		/* 0x0008F2B8 1119         */ IL_037C: ldloc.s   V_25
// 		/* 0x0008F2BA 2200000000   */ IL_037E: ldc.r4    0.0
// 		/* 0x0008F2BF 7D3F0C0004   */ IL_0383: stfld     float32 ShipData::t
// 		/* 0x0008F2C4 02           */ IL_0388: ldarg.0
// 		/* 0x0008F2C5 1119         */ IL_0389: ldloc.s   V_25
// 		/* 0x0008F2C7 7B400C0004   */ IL_038B: ldfld     int32 ShipData::itemId
// 		/* 0x0008F2CC 1119         */ IL_0390: ldloc.s   V_25
// 		/* 0x0008F2CE 7B410C0004   */ IL_0392: ldfld     int32 ShipData::itemCount
// 		/* 0x0008F2D3 1119         */ IL_0397: ldloc.s   V_25
// 		/* 0x0008F2D5 7B420C0004   */ IL_0399: ldfld     int32 ShipData::inc
// 		/* 0x0008F2DA 280D0A0006   */ IL_039E: call      instance int32 StationComponent::AddItem(int32, int32, int32)
// 		/* 0x0008F2DF 26           */ IL_03A3: pop
// 		/* 0x0008F2E0 03           */ IL_03A4: ldarg.1
// 		/* 0x0008F2E1 6F9C170006   */ IL_03A5: callvirt  instance class GameData PlanetFactory::get_gameData()
// 		/* 0x0008F2E6 7BE51A0004   */ IL_03AA: ldfld     class GameStatData GameData::statistics
// 		/* 0x0008F2EB 7B891C0004   */ IL_03AF: ldfld     class TrafficStatistics GameStatData::traffic
// 		/* 0x0008F2F0 131F         */ IL_03B4: stloc.s   V_31
// 		/* 0x0008F2F2 111F         */ IL_03B6: ldloc.s   V_31
// 		/* 0x0008F2F4 02           */ IL_03B8: ldarg.0
// 		/* 0x0008F2F5 7BC40B0004   */ IL_03B9: ldfld     int32 StationComponent::planetId
// 		/* 0x0008F2FA 1119         */ IL_03BE: ldloc.s   V_25
// 		/* 0x0008F2FC 7B400C0004   */ IL_03C0: ldfld     int32 ShipData::itemId
// 		/* 0x0008F301 1119         */ IL_03C5: ldloc.s   V_25
// 		/* 0x0008F303 7B410C0004   */ IL_03C7: ldfld     int32 ShipData::itemCount
// 		/* 0x0008F308 6FB41A0006   */ IL_03CC: callvirt  instance void TrafficStatistics::RegisterPlanetInputStat(int32, int32, int32)
// 		/* 0x0008F30D 1119         */ IL_03D1: ldloc.s   V_25
// 		/* 0x0008F30F 7B330C0004   */ IL_03D3: ldfld     int32 ShipData::planetB
// 		/* 0x0008F314 1F64         */ IL_03D8: ldc.i4.s  100
// 		/* 0x0008F316 5B           */ IL_03DA: div
// 		/* 0x0008F317 02           */ IL_03DB: ldarg.0
// 		/* 0x0008F318 7BC40B0004   */ IL_03DC: ldfld     int32 StationComponent::planetId
// 		/* 0x0008F31D 1F64         */ IL_03E1: ldc.i4.s  100
// 		/* 0x0008F31F 5B           */ IL_03E3: div
// 		/* 0x0008F320 1320         */ IL_03E4: stloc.s   V_32
// 		/* 0x0008F322 1120         */ IL_03E6: ldloc.s   V_32
// 		/* 0x0008F324 2E19         */ IL_03E8: beq.s     IL_0403

// 		/* 0x0008F326 111F         */ IL_03EA: ldloc.s   V_31
// 		/* 0x0008F328 1120         */ IL_03EC: ldloc.s   V_32
// 		/* 0x0008F32A 1119         */ IL_03EE: ldloc.s   V_25
// 		/* 0x0008F32C 7B400C0004   */ IL_03F0: ldfld     int32 ShipData::itemId
// 		/* 0x0008F331 1119         */ IL_03F5: ldloc.s   V_25
// 		/* 0x0008F333 7B410C0004   */ IL_03F7: ldfld     int32 ShipData::itemCount
// 		/* 0x0008F338 6FB11A0006   */ IL_03FC: callvirt  instance void TrafficStatistics::RegisterStarInputStat(int32, int32, int32)
// 		/* 0x0008F33D 2B17         */ IL_0401: br.s      IL_041A

// 		/* 0x0008F33F 111F         */ IL_0403: ldloc.s   V_31
// 		/* 0x0008F341 1120         */ IL_0405: ldloc.s   V_32
// 		/* 0x0008F343 1119         */ IL_0407: ldloc.s   V_25
// 		/* 0x0008F345 7B400C0004   */ IL_0409: ldfld     int32 ShipData::itemId
// 		/* 0x0008F34A 1119         */ IL_040E: ldloc.s   V_25
// 		/* 0x0008F34C 7B410C0004   */ IL_0410: ldfld     int32 ShipData::itemCount
// 		/* 0x0008F351 6FB31A0006   */ IL_0415: callvirt  instance void TrafficStatistics::RegisterStarInternalStat(int32, int32, int32)

// 		/* 0x0008F356 03           */ IL_041A: ldarg.1
// 		/* 0x0008F357 1119         */ IL_041B: ldloc.s   V_25
// 		/* 0x0008F359 7B330C0004   */ IL_041D: ldfld     int32 ShipData::planetB
// 		/* 0x0008F35E 0E06         */ IL_0422: ldarg.s   gStationPool
// 		/* 0x0008F360 1119         */ IL_0424: ldloc.s   V_25
// 		/* 0x0008F362 7B3D0C0004   */ IL_0426: ldfld     int32 ShipData::otherGId
// 		/* 0x0008F367 9A           */ IL_042B: ldelem.ref
// 		/* 0x0008F368 1119         */ IL_042C: ldloc.s   V_25
// 		/* 0x0008F36A 7B320C0004   */ IL_042E: ldfld     int32 ShipData::planetA
// 		/* 0x0008F36F 02           */ IL_0433: ldarg.0
// 		/* 0x0008F370 1119         */ IL_0434: ldloc.s   V_25
// 		/* 0x0008F372 7B400C0004   */ IL_0436: ldfld     int32 ShipData::itemId
// 		/* 0x0008F377 1119         */ IL_043B: ldloc.s   V_25
// 		/* 0x0008F379 7B410C0004   */ IL_043D: ldfld     int32 ShipData::itemCount
// 		/* 0x0008F37E 6FF9170006   */ IL_0442: callvirt  instance void PlanetFactory::NotifyShipDelivery(int32, class StationComponent, int32, class StationComponent, int32, int32)
// 		/* 0x0008F383 02           */ IL_0447: ldarg.0
// 		/* 0x0008F384 7BDC0B0004   */ IL_0448: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F389 1118         */ IL_044D: ldloc.s   V_24
// 		/* 0x0008F38B 8F5B010002   */ IL_044F: ldelema   RemoteLogisticOrder
// 		/* 0x0008F390 7B5C0C0004   */ IL_0454: ldfld     int32 RemoteLogisticOrder::itemId
// 		/* 0x0008F395 16           */ IL_0459: ldc.i4.0
// 		/* 0x0008F396 3EA2000000   */ IL_045A: ble       IL_0501

// 		/* 0x0008F39B 02           */ IL_045F: ldarg.0
// 		/* 0x0008F39C 7BE00B0004   */ IL_0460: ldfld     valuetype StationStore[] StationComponent::'storage'
// 		/* 0x0008F3A1 1314         */ IL_0465: stloc.s   V_20
// 		/* 0x0008F3A3 16           */ IL_0467: ldc.i4.0
// 		/* 0x0008F3A4 1315         */ IL_0468: stloc.s   V_21
// 		.try
// 		{
// 			/* 0x0008F3A6 1114         */ IL_046A: ldloc.s   V_20
// 			/* 0x0008F3A8 1215         */ IL_046C: ldloca.s  V_21
// 			/* 0x0008F3AA 287502000A   */ IL_046E: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 			/* 0x0008F3AF 02           */ IL_0473: ldarg.0
// 			/* 0x0008F3B0 7BE00B0004   */ IL_0474: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008F3B5 02           */ IL_0479: ldarg.0
// 			/* 0x0008F3B6 7BDC0B0004   */ IL_047A: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0008F3BB 1118         */ IL_047F: ldloc.s   V_24
// 			/* 0x0008F3BD 8F5B010002   */ IL_0481: ldelema   RemoteLogisticOrder
// 			/* 0x0008F3C2 7B5D0C0004   */ IL_0486: ldfld     int32 RemoteLogisticOrder::thisIndex
// 			/* 0x0008F3C7 8F54010002   */ IL_048B: ldelema   StationStore
// 			/* 0x0008F3CC 7B1A0C0004   */ IL_0490: ldfld     int32 StationStore::itemId
// 			/* 0x0008F3D1 02           */ IL_0495: ldarg.0
// 			/* 0x0008F3D2 7BDC0B0004   */ IL_0496: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0008F3D7 1118         */ IL_049B: ldloc.s   V_24
// 			/* 0x0008F3D9 8F5B010002   */ IL_049D: ldelema   RemoteLogisticOrder
// 			/* 0x0008F3DE 7B5C0C0004   */ IL_04A2: ldfld     int32 RemoteLogisticOrder::itemId
// 			/* 0x0008F3E3 3338         */ IL_04A7: bne.un.s  IL_04E1

// 			/* 0x0008F3E5 02           */ IL_04A9: ldarg.0
// 			/* 0x0008F3E6 7BE00B0004   */ IL_04AA: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x0008F3EB 02           */ IL_04AF: ldarg.0
// 			/* 0x0008F3EC 7BDC0B0004   */ IL_04B0: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0008F3F1 1118         */ IL_04B5: ldloc.s   V_24
// 			/* 0x0008F3F3 8F5B010002   */ IL_04B7: ldelema   RemoteLogisticOrder
// 			/* 0x0008F3F8 7B5D0C0004   */ IL_04BC: ldfld     int32 RemoteLogisticOrder::thisIndex
// 			/* 0x0008F3FD 8F54010002   */ IL_04C1: ldelema   StationStore
// 			/* 0x0008F402 7C1E0C0004   */ IL_04C6: ldflda    int32 StationStore::remoteOrder
// 			/* 0x0008F407 25           */ IL_04CB: dup
// 			/* 0x0008F408 4A           */ IL_04CC: ldind.i4
// 			/* 0x0008F409 02           */ IL_04CD: ldarg.0
// 			/* 0x0008F40A 7BDC0B0004   */ IL_04CE: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0008F40F 1118         */ IL_04D3: ldloc.s   V_24
// 			/* 0x0008F411 8F5B010002   */ IL_04D5: ldelema   RemoteLogisticOrder
// 			/* 0x0008F416 7B5E0C0004   */ IL_04DA: ldfld     int32 RemoteLogisticOrder::thisOrdered
// 			/* 0x0008F41B 59           */ IL_04DF: sub
// 			/* 0x0008F41C 54           */ IL_04E0: stind.i4

// 			/* 0x0008F41D DE0C         */ IL_04E1: leave.s   IL_04EF
// 		} // end .try
// 		finally
// 		{
// 			/* 0x0008F41F 1115         */ IL_04E3: ldloc.s   V_21
// 			/* 0x0008F421 2C07         */ IL_04E5: brfalse.s IL_04EE

// 			/* 0x0008F423 1114         */ IL_04E7: ldloc.s   V_20
// 			/* 0x0008F425 287602000A   */ IL_04E9: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 			/* 0x0008F42A DC           */ IL_04EE: endfinally
// 		} // end handler

// 		/* 0x0008F42B 02           */ IL_04EF: ldarg.0
// 		/* 0x0008F42C 7BDC0B0004   */ IL_04F0: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F431 1118         */ IL_04F5: ldloc.s   V_24
// 		/* 0x0008F433 8F5B010002   */ IL_04F7: ldelema   RemoteLogisticOrder
// 		/* 0x0008F438 28310A0006   */ IL_04FC: call      instance void RemoteLogisticOrder::ClearThis()

// 		/* 0x0008F43D 1119         */ IL_0501: ldloc.s   V_25
// 		/* 0x0008F43F 7B440C0004   */ IL_0503: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F444 1321         */ IL_0508: stloc.s   V_33
// 		/* 0x0008F446 02           */ IL_050A: ldarg.0
// 		/* 0x0008F447 7BDB0B0004   */ IL_050B: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F44C 1118         */ IL_0510: ldloc.s   V_24
// 		/* 0x0008F44E 17           */ IL_0512: ldc.i4.1
// 		/* 0x0008F44F 58           */ IL_0513: add
// 		/* 0x0008F450 02           */ IL_0514: ldarg.0
// 		/* 0x0008F451 7BDB0B0004   */ IL_0515: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F456 1118         */ IL_051A: ldloc.s   V_24
// 		/* 0x0008F458 02           */ IL_051C: ldarg.0
// 		/* 0x0008F459 7BDB0B0004   */ IL_051D: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F45E 8E           */ IL_0522: ldlen
// 		/* 0x0008F45F 69           */ IL_0523: conv.i4
// 		/* 0x0008F460 1118         */ IL_0524: ldloc.s   V_24
// 		/* 0x0008F462 59           */ IL_0526: sub
// 		/* 0x0008F463 17           */ IL_0527: ldc.i4.1
// 		/* 0x0008F464 59           */ IL_0528: sub
// 		/* 0x0008F465 28A001000A   */ IL_0529: call      void [netstandard]System.Array::Copy(class [netstandard]System.Array, int32, class [netstandard]System.Array, int32, int32)
// 		/* 0x0008F46A 02           */ IL_052E: ldarg.0
// 		/* 0x0008F46B 7BDC0B0004   */ IL_052F: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F470 1118         */ IL_0534: ldloc.s   V_24
// 		/* 0x0008F472 17           */ IL_0536: ldc.i4.1
// 		/* 0x0008F473 58           */ IL_0537: add
// 		/* 0x0008F474 02           */ IL_0538: ldarg.0
// 		/* 0x0008F475 7BDC0B0004   */ IL_0539: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F47A 1118         */ IL_053E: ldloc.s   V_24
// 		/* 0x0008F47C 02           */ IL_0540: ldarg.0
// 		/* 0x0008F47D 7BDC0B0004   */ IL_0541: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F482 8E           */ IL_0546: ldlen
// 		/* 0x0008F483 69           */ IL_0547: conv.i4
// 		/* 0x0008F484 1118         */ IL_0548: ldloc.s   V_24
// 		/* 0x0008F486 59           */ IL_054A: sub
// 		/* 0x0008F487 17           */ IL_054B: ldc.i4.1
// 		/* 0x0008F488 59           */ IL_054C: sub
// 		/* 0x0008F489 28A001000A   */ IL_054D: call      void [netstandard]System.Array::Copy(class [netstandard]System.Array, int32, class [netstandard]System.Array, int32, int32)
// 		/* 0x0008F48E 02           */ IL_0552: ldarg.0
// 		/* 0x0008F48F 02           */ IL_0553: ldarg.0
// 		/* 0x0008F490 7BD80B0004   */ IL_0554: ldfld     int32 StationComponent::workShipCount
// 		/* 0x0008F495 17           */ IL_0559: ldc.i4.1
// 		/* 0x0008F496 59           */ IL_055A: sub
// 		/* 0x0008F497 7DD80B0004   */ IL_055B: stfld     int32 StationComponent::workShipCount
// 		/* 0x0008F49C 02           */ IL_0560: ldarg.0
// 		/* 0x0008F49D 02           */ IL_0561: ldarg.0
// 		/* 0x0008F49E 7BD70B0004   */ IL_0562: ldfld     int32 StationComponent::idleShipCount
// 		/* 0x0008F4A3 17           */ IL_0567: ldc.i4.1
// 		/* 0x0008F4A4 58           */ IL_0568: add
// 		/* 0x0008F4A5 7DD70B0004   */ IL_0569: stfld     int32 StationComponent::idleShipCount
// 		/* 0x0008F4AA 02           */ IL_056E: ldarg.0
// 		/* 0x0008F4AB 1121         */ IL_056F: ldloc.s   V_33
// 		/* 0x0008F4AD 28F4090006   */ IL_0571: call      instance void StationComponent::WorkShipBackToIdle(int32)
// 		/* 0x0008F4B2 02           */ IL_0576: ldarg.0
// 		/* 0x0008F4B3 7BDB0B0004   */ IL_0577: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F4B8 02           */ IL_057C: ldarg.0
// 		/* 0x0008F4B9 7BD80B0004   */ IL_057D: ldfld     int32 StationComponent::workShipCount
// 		/* 0x0008F4BE 02           */ IL_0582: ldarg.0
// 		/* 0x0008F4BF 7BDB0B0004   */ IL_0583: ldfld     valuetype ShipData[] StationComponent::workShipDatas
// 		/* 0x0008F4C4 8E           */ IL_0588: ldlen
// 		/* 0x0008F4C5 69           */ IL_0589: conv.i4
// 		/* 0x0008F4C6 02           */ IL_058A: ldarg.0
// 		/* 0x0008F4C7 7BD80B0004   */ IL_058B: ldfld     int32 StationComponent::workShipCount
// 		/* 0x0008F4CC 59           */ IL_0590: sub
// 		/* 0x0008F4CD 289C00000A   */ IL_0591: call      void [netstandard]System.Array::Clear(class [netstandard]System.Array, int32, int32)
// 		/* 0x0008F4D2 02           */ IL_0596: ldarg.0
// 		/* 0x0008F4D3 7BDC0B0004   */ IL_0597: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F4D8 02           */ IL_059C: ldarg.0
// 		/* 0x0008F4D9 7BD80B0004   */ IL_059D: ldfld     int32 StationComponent::workShipCount
// 		/* 0x0008F4DE 02           */ IL_05A2: ldarg.0
// 		/* 0x0008F4DF 7BDC0B0004   */ IL_05A3: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0008F4E4 8E           */ IL_05A8: ldlen
// 		/* 0x0008F4E5 69           */ IL_05A9: conv.i4
// 		/* 0x0008F4E6 02           */ IL_05AA: ldarg.0
// 		/* 0x0008F4E7 7BD80B0004   */ IL_05AB: ldfld     int32 StationComponent::workShipCount
// 		/* 0x0008F4EC 59           */ IL_05B0: sub
// 		/* 0x0008F4ED 289C00000A   */ IL_05B1: call      void [netstandard]System.Array::Clear(class [netstandard]System.Array, int32, int32)
// 		/* 0x0008F4F2 1118         */ IL_05B6: ldloc.s   V_24
// 		/* 0x0008F4F4 17           */ IL_05B8: ldc.i4.1
// 		/* 0x0008F4F5 59           */ IL_05B9: sub
// 		/* 0x0008F4F6 1318         */ IL_05BA: stloc.s   V_24
// 		/* 0x0008F4F8 38A72A0000   */ IL_05BC: br        IL_3068

// 		/* 0x0008F4FD 1119         */ IL_05C1: ldloc.s   V_25
// 		/* 0x0008F4FF 110B         */ IL_05C3: ldloc.s   V_11
// 		/* 0x0008F501 7B46180004   */ IL_05C5: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F506 110B         */ IL_05CA: ldloc.s   V_11
// 		/* 0x0008F508 7B44180004   */ IL_05CC: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F50D 02           */ IL_05D1: ldarg.0
// 		/* 0x0008F50E 7BFD0B0004   */ IL_05D2: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3[] StationComponent::shipDiskPos
// 		/* 0x0008F513 1119         */ IL_05D7: ldloc.s   V_25
// 		/* 0x0008F515 7B440C0004   */ IL_05D9: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F51A A310000001   */ IL_05DE: ldelem    [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x0008F51F 28BC020006   */ IL_05E3: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0008F524 280D040006   */ IL_05E8: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x0008F529 28BB020006   */ IL_05ED: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0008F52E 7D340C0004   */ IL_05F2: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008F533 1119         */ IL_05F7: ldloc.s   V_25
// 		/* 0x0008F535 7C350C0004   */ IL_05F9: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F53A 2200000000   */ IL_05FE: ldc.r4    0.0
// 		/* 0x0008F53F 7D4100000A   */ IL_0603: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F544 1119         */ IL_0608: ldloc.s   V_25
// 		/* 0x0008F546 7C350C0004   */ IL_060A: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F54B 2200000000   */ IL_060F: ldc.r4    0.0
// 		/* 0x0008F550 7D4200000A   */ IL_0614: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F555 1119         */ IL_0619: ldloc.s   V_25
// 		/* 0x0008F557 7C350C0004   */ IL_061B: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F55C 2200000000   */ IL_0620: ldc.r4    0.0
// 		/* 0x0008F561 7D8000000A   */ IL_0625: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F566 1119         */ IL_062A: ldloc.s   V_25
// 		/* 0x0008F568 2200000000   */ IL_062C: ldc.r4    0.0
// 		/* 0x0008F56D 7D360C0004   */ IL_0631: stfld     float32 ShipData::uSpeed
// 		/* 0x0008F572 1119         */ IL_0636: ldloc.s   V_25
// 		/* 0x0008F574 110B         */ IL_0638: ldloc.s   V_11
// 		/* 0x0008F576 7B44180004   */ IL_063A: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F57B 02           */ IL_063F: ldarg.0
// 		/* 0x0008F57C 7BFE0B0004   */ IL_0640: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion[] StationComponent::shipDiskRot
// 		/* 0x0008F581 1119         */ IL_0645: ldloc.s   V_25
// 		/* 0x0008F583 7B440C0004   */ IL_0647: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F588 A327000001   */ IL_064C: ldelem    [UnityEngine.CoreModule]UnityEngine.Quaternion
// 		/* 0x0008F58D 281A01000A   */ IL_0651: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x0008F592 7D380C0004   */ IL_0656: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x0008F597 1119         */ IL_065B: ldloc.s   V_25
// 		/* 0x0008F599 7C390C0004   */ IL_065D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F59E 2200000000   */ IL_0662: ldc.r4    0.0
// 		/* 0x0008F5A3 7D4100000A   */ IL_0667: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F5A8 1119         */ IL_066C: ldloc.s   V_25
// 		/* 0x0008F5AA 7C390C0004   */ IL_066E: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F5AF 2200000000   */ IL_0673: ldc.r4    0.0
// 		/* 0x0008F5B4 7D4200000A   */ IL_0678: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F5B9 1119         */ IL_067D: ldloc.s   V_25
// 		/* 0x0008F5BB 7C390C0004   */ IL_067F: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F5C0 2200000000   */ IL_0684: ldc.r4    0.0
// 		/* 0x0008F5C5 7D8000000A   */ IL_0689: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F5CA 1119         */ IL_068E: ldloc.s   V_25
// 		/* 0x0008F5CC 2200000000   */ IL_0690: ldc.r4    0.0
// 		/* 0x0008F5D1 7D3A0C0004   */ IL_0695: stfld     float32 ShipData::uAngularSpeed
// 		/* 0x0008F5D6 1119         */ IL_069A: ldloc.s   V_25
// 		/* 0x0008F5D8 2200000000   */ IL_069C: ldc.r4    0.0
// 		/* 0x0008F5DD 2200000000   */ IL_06A1: ldc.r4    0.0
// 		/* 0x0008F5E2 2200000000   */ IL_06A6: ldc.r4    0.0
// 		/* 0x0008F5E7 73B3020006   */ IL_06AB: newobj    instance void VectorLF3::.ctor(float32, float32, float32)
// 		/* 0x0008F5EC 7D3B0C0004   */ IL_06B0: stfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x0008F5F1 1119         */ IL_06B5: ldloc.s   V_25
// 		/* 0x0008F5F3 2200000000   */ IL_06B7: ldc.r4    0.0
// 		/* 0x0008F5F8 2200000000   */ IL_06BC: ldc.r4    0.0
// 		/* 0x0008F5FD 2200000000   */ IL_06C1: ldc.r4    0.0
// 		/* 0x0008F602 220000803F   */ IL_06C6: ldc.r4    1
// 		/* 0x0008F607 735702000A   */ IL_06CB: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x0008F60C 7D3C0C0004   */ IL_06D0: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp
// 		/* 0x0008F611 111A         */ IL_06D5: ldloc.s   V_26
// 		/* 0x0008F613 7C4A0C0004   */ IL_06D7: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x0008F618 2200000000   */ IL_06DC: ldc.r4    0.0
// 		/* 0x0008F61D 7D7E01000A   */ IL_06E1: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x0008F622 381D280000   */ IL_06E6: br        IL_2F08

// 		/* 0x0008F627 1119         */ IL_06EB: ldloc.s   V_25
// 		/* 0x0008F629 7B310C0004   */ IL_06ED: ldfld     int32 ShipData::stage
// 		/* 0x0008F62E 15           */ IL_06F2: ldc.i4.m1
// 		/* 0x0008F62F 409E020000   */ IL_06F3: bne.un    IL_0996

// 		/* 0x0008F634 2200000000   */ IL_06F8: ldc.r4    0.0
// 		/* 0x0008F639 1322         */ IL_06FD: stloc.s   V_34
// 		/* 0x0008F63B 1119         */ IL_06FF: ldloc.s   V_25
// 		/* 0x0008F63D 7B3E0C0004   */ IL_0701: ldfld     int32 ShipData::direction
// 		/* 0x0008F642 16           */ IL_0706: ldc.i4.0
// 		/* 0x0008F643 3EE9000000   */ IL_0707: ble       IL_07F5

// 		/* 0x0008F648 1119         */ IL_070C: ldloc.s   V_25
// 		/* 0x0008F64A 7C3F0C0004   */ IL_070E: ldflda    float32 ShipData::t
// 		/* 0x0008F64F 25           */ IL_0713: dup
// 		/* 0x0008F650 4E           */ IL_0714: ldind.r4
// 		/* 0x0008F651 110F         */ IL_0715: ldloc.s   V_15
// 		/* 0x0008F653 58           */ IL_0717: add
// 		/* 0x0008F654 56           */ IL_0718: stind.r4
// 		/* 0x0008F655 1119         */ IL_0719: ldloc.s   V_25
// 		/* 0x0008F657 7B3F0C0004   */ IL_071B: ldfld     float32 ShipData::t
// 		/* 0x0008F65C 1322         */ IL_0720: stloc.s   V_34
// 		/* 0x0008F65E 1119         */ IL_0722: ldloc.s   V_25
// 		/* 0x0008F660 7B3F0C0004   */ IL_0724: ldfld     float32 ShipData::t
// 		/* 0x0008F665 220000803F   */ IL_0729: ldc.r4    1
// 		/* 0x0008F66A 361B         */ IL_072E: ble.un.s  IL_074B

// 		/* 0x0008F66C 1119         */ IL_0730: ldloc.s   V_25
// 		/* 0x0008F66E 220000803F   */ IL_0732: ldc.r4    1
// 		/* 0x0008F673 7D3F0C0004   */ IL_0737: stfld     float32 ShipData::t
// 		/* 0x0008F678 220000803F   */ IL_073C: ldc.r4    1
// 		/* 0x0008F67D 1322         */ IL_0741: stloc.s   V_34
// 		/* 0x0008F67F 1119         */ IL_0743: ldloc.s   V_25
// 		/* 0x0008F681 16           */ IL_0745: ldc.i4.0
// 		/* 0x0008F682 7D310C0004   */ IL_0746: stfld     int32 ShipData::stage

// 		/* 0x0008F687 111A         */ IL_074B: ldloc.s   V_26
// 		/* 0x0008F689 7C4A0C0004   */ IL_074D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x0008F68E 1122         */ IL_0752: ldloc.s   V_34
// 		/* 0x0008F690 7D7E01000A   */ IL_0754: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x0008F695 2200004040   */ IL_0759: ldc.r4    3
// 		/* 0x0008F69A 1122         */ IL_075E: ldloc.s   V_34
// 		/* 0x0008F69C 59           */ IL_0760: sub
// 		/* 0x0008F69D 1122         */ IL_0761: ldloc.s   V_34
// 		/* 0x0008F69F 59           */ IL_0763: sub
// 		/* 0x0008F6A0 1122         */ IL_0764: ldloc.s   V_34
// 		/* 0x0008F6A2 5A           */ IL_0766: mul
// 		/* 0x0008F6A3 1122         */ IL_0767: ldloc.s   V_34
// 		/* 0x0008F6A5 5A           */ IL_0769: mul
// 		/* 0x0008F6A6 1322         */ IL_076A: stloc.s   V_34
// 		/* 0x0008F6A8 1119         */ IL_076C: ldloc.s   V_25
// 		/* 0x0008F6AA 110B         */ IL_076E: ldloc.s   V_11
// 		/* 0x0008F6AC 7B46180004   */ IL_0770: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F6B1 110B         */ IL_0775: ldloc.s   V_11
// 		/* 0x0008F6B3 7B44180004   */ IL_0777: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F6B8 02           */ IL_077C: ldarg.0
// 		/* 0x0008F6B9 7BFD0B0004   */ IL_077D: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3[] StationComponent::shipDiskPos
// 		/* 0x0008F6BE 1119         */ IL_0782: ldloc.s   V_25
// 		/* 0x0008F6C0 7B440C0004   */ IL_0784: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F6C5 A310000001   */ IL_0789: ldelem    [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x0008F6CA 02           */ IL_078E: ldarg.0
// 		/* 0x0008F6CB 7BFD0B0004   */ IL_078F: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3[] StationComponent::shipDiskPos
// 		/* 0x0008F6D0 1119         */ IL_0794: ldloc.s   V_25
// 		/* 0x0008F6D2 7B440C0004   */ IL_0796: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F6D7 8F10000001   */ IL_079B: ldelema   [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x0008F6DC 289200000A   */ IL_07A0: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x0008F6E1 220000C841   */ IL_07A5: ldc.r4    25
// 		/* 0x0008F6E6 1122         */ IL_07AA: ldloc.s   V_34
// 		/* 0x0008F6E8 5A           */ IL_07AC: mul
// 		/* 0x0008F6E9 289800000A   */ IL_07AD: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x0008F6EE 289900000A   */ IL_07B2: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0008F6F3 28BC020006   */ IL_07B7: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0008F6F8 280D040006   */ IL_07BC: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x0008F6FD 28BB020006   */ IL_07C1: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0008F702 7D340C0004   */ IL_07C6: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008F707 1119         */ IL_07CB: ldloc.s   V_25
// 		/* 0x0008F709 110B         */ IL_07CD: ldloc.s   V_11
// 		/* 0x0008F70B 7B44180004   */ IL_07CF: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F710 02           */ IL_07D4: ldarg.0
// 		/* 0x0008F711 7BFE0B0004   */ IL_07D5: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion[] StationComponent::shipDiskRot
// 		/* 0x0008F716 1119         */ IL_07DA: ldloc.s   V_25
// 		/* 0x0008F718 7B440C0004   */ IL_07DC: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F71D A327000001   */ IL_07E1: ldelem    [UnityEngine.CoreModule]UnityEngine.Quaternion
// 		/* 0x0008F722 281A01000A   */ IL_07E6: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x0008F727 7D380C0004   */ IL_07EB: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x0008F72C 381E010000   */ IL_07F0: br        IL_0913

// 		/* 0x0008F731 1119         */ IL_07F5: ldloc.s   V_25
// 		/* 0x0008F733 7C3F0C0004   */ IL_07F7: ldflda    float32 ShipData::t
// 		/* 0x0008F738 25           */ IL_07FC: dup
// 		/* 0x0008F739 4E           */ IL_07FD: ldind.r4
// 		/* 0x0008F73A 110F         */ IL_07FE: ldloc.s   V_15
// 		/* 0x0008F73C 22ABAA2A3F   */ IL_0800: ldc.r4    0.6666667
// 		/* 0x0008F741 5A           */ IL_0805: mul
// 		/* 0x0008F742 59           */ IL_0806: sub
// 		/* 0x0008F743 56           */ IL_0807: stind.r4
// 		/* 0x0008F744 1119         */ IL_0808: ldloc.s   V_25
// 		/* 0x0008F746 7B3F0C0004   */ IL_080A: ldfld     float32 ShipData::t
// 		/* 0x0008F74B 1322         */ IL_080F: stloc.s   V_34
// 		/* 0x0008F74D 1119         */ IL_0811: ldloc.s   V_25
// 		/* 0x0008F74F 7B3F0C0004   */ IL_0813: ldfld     float32 ShipData::t
// 		/* 0x0008F754 2200000000   */ IL_0818: ldc.r4    0.0
// 		/* 0x0008F759 341C         */ IL_081D: bge.un.s  IL_083B

// 		/* 0x0008F75B 1119         */ IL_081F: ldloc.s   V_25
// 		/* 0x0008F75D 220000803F   */ IL_0821: ldc.r4    1
// 		/* 0x0008F762 7D3F0C0004   */ IL_0826: stfld     float32 ShipData::t
// 		/* 0x0008F767 2200000000   */ IL_082B: ldc.r4    0.0
// 		/* 0x0008F76C 1322         */ IL_0830: stloc.s   V_34
// 		/* 0x0008F76E 1119         */ IL_0832: ldloc.s   V_25
// 		/* 0x0008F770 1FFE         */ IL_0834: ldc.i4.s  -2
// 		/* 0x0008F772 7D310C0004   */ IL_0836: stfld     int32 ShipData::stage

// 		/* 0x0008F777 111A         */ IL_083B: ldloc.s   V_26
// 		/* 0x0008F779 7C4A0C0004   */ IL_083D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x0008F77E 1122         */ IL_0842: ldloc.s   V_34
// 		/* 0x0008F780 7D7E01000A   */ IL_0844: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x0008F785 2200004040   */ IL_0849: ldc.r4    3
// 		/* 0x0008F78A 1122         */ IL_084E: ldloc.s   V_34
// 		/* 0x0008F78C 59           */ IL_0850: sub
// 		/* 0x0008F78D 1122         */ IL_0851: ldloc.s   V_34
// 		/* 0x0008F78F 59           */ IL_0853: sub
// 		/* 0x0008F790 1122         */ IL_0854: ldloc.s   V_34
// 		/* 0x0008F792 5A           */ IL_0856: mul
// 		/* 0x0008F793 1122         */ IL_0857: ldloc.s   V_34
// 		/* 0x0008F795 5A           */ IL_0859: mul
// 		/* 0x0008F796 1322         */ IL_085A: stloc.s   V_34
// 		/* 0x0008F798 110B         */ IL_085C: ldloc.s   V_11
// 		/* 0x0008F79A 7B46180004   */ IL_085E: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F79F 110B         */ IL_0863: ldloc.s   V_11
// 		/* 0x0008F7A1 7B44180004   */ IL_0865: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F7A6 02           */ IL_086A: ldarg.0
// 		/* 0x0008F7A7 7BFD0B0004   */ IL_086B: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3[] StationComponent::shipDiskPos
// 		/* 0x0008F7AC 1119         */ IL_0870: ldloc.s   V_25
// 		/* 0x0008F7AE 7B440C0004   */ IL_0872: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F7B3 A310000001   */ IL_0877: ldelem    [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x0008F7B8 28BC020006   */ IL_087C: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0008F7BD 280D040006   */ IL_0881: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x0008F7C2 28BB020006   */ IL_0886: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0008F7C7 1323         */ IL_088B: stloc.s   V_35
// 		/* 0x0008F7C9 110B         */ IL_088D: ldloc.s   V_11
// 		/* 0x0008F7CB 7B46180004   */ IL_088F: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F7D0 110B         */ IL_0894: ldloc.s   V_11
// 		/* 0x0008F7D2 7B44180004   */ IL_0896: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F7D7 1119         */ IL_089B: ldloc.s   V_25
// 		/* 0x0008F7D9 7B3B0C0004   */ IL_089D: ldfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x0008F7DE 280D040006   */ IL_08A2: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x0008F7E3 28BB020006   */ IL_08A7: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0008F7E8 1324         */ IL_08AC: stloc.s   V_36
// 		/* 0x0008F7EA 1119         */ IL_08AE: ldloc.s   V_25
// 		/* 0x0008F7EC 1123         */ IL_08B0: ldloc.s   V_35
// 		/* 0x0008F7EE 220000803F   */ IL_08B2: ldc.r4    1
// 		/* 0x0008F7F3 1122         */ IL_08B7: ldloc.s   V_34
// 		/* 0x0008F7F5 59           */ IL_08B9: sub
// 		/* 0x0008F7F6 6C           */ IL_08BA: conv.r8
// 		/* 0x0008F7F7 28B7020006   */ IL_08BB: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x0008F7FC 1124         */ IL_08C0: ldloc.s   V_36
// 		/* 0x0008F7FE 1122         */ IL_08C2: ldloc.s   V_34
// 		/* 0x0008F800 6C           */ IL_08C4: conv.r8
// 		/* 0x0008F801 28B7020006   */ IL_08C5: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x0008F806 28BB020006   */ IL_08CA: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0008F80B 7D340C0004   */ IL_08CF: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008F810 1119         */ IL_08D4: ldloc.s   V_25
// 		/* 0x0008F812 110B         */ IL_08D6: ldloc.s   V_11
// 		/* 0x0008F814 7B44180004   */ IL_08D8: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F819 02           */ IL_08DD: ldarg.0
// 		/* 0x0008F81A 7BFE0B0004   */ IL_08DE: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion[] StationComponent::shipDiskRot
// 		/* 0x0008F81F 1119         */ IL_08E3: ldloc.s   V_25
// 		/* 0x0008F821 7B440C0004   */ IL_08E5: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F826 A327000001   */ IL_08EA: ldelem    [UnityEngine.CoreModule]UnityEngine.Quaternion
// 		/* 0x0008F82B 1119         */ IL_08EF: ldloc.s   V_25
// 		/* 0x0008F82D 7B3C0C0004   */ IL_08F1: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp
// 		/* 0x0008F832 1122         */ IL_08F6: ldloc.s   V_34
// 		/* 0x0008F834 2200000040   */ IL_08F8: ldc.r4    2
// 		/* 0x0008F839 5A           */ IL_08FD: mul
// 		/* 0x0008F83A 220000803F   */ IL_08FE: ldc.r4    1
// 		/* 0x0008F83F 59           */ IL_0903: sub
// 		/* 0x0008F840 28DE00000A   */ IL_0904: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 		/* 0x0008F845 281A01000A   */ IL_0909: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x0008F84A 7D380C0004   */ IL_090E: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot

// 		/* 0x0008F84F 1119         */ IL_0913: ldloc.s   V_25
// 		/* 0x0008F851 7C350C0004   */ IL_0915: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F856 2200000000   */ IL_091A: ldc.r4    0.0
// 		/* 0x0008F85B 7D4100000A   */ IL_091F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F860 1119         */ IL_0924: ldloc.s   V_25
// 		/* 0x0008F862 7C350C0004   */ IL_0926: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F867 2200000000   */ IL_092B: ldc.r4    0.0
// 		/* 0x0008F86C 7D4200000A   */ IL_0930: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F871 1119         */ IL_0935: ldloc.s   V_25
// 		/* 0x0008F873 7C350C0004   */ IL_0937: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0008F878 2200000000   */ IL_093C: ldc.r4    0.0
// 		/* 0x0008F87D 7D8000000A   */ IL_0941: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F882 1119         */ IL_0946: ldloc.s   V_25
// 		/* 0x0008F884 2200000000   */ IL_0948: ldc.r4    0.0
// 		/* 0x0008F889 7D360C0004   */ IL_094D: stfld     float32 ShipData::uSpeed
// 		/* 0x0008F88E 1119         */ IL_0952: ldloc.s   V_25
// 		/* 0x0008F890 7C390C0004   */ IL_0954: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F895 2200000000   */ IL_0959: ldc.r4    0.0
// 		/* 0x0008F89A 7D4100000A   */ IL_095E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F89F 1119         */ IL_0963: ldloc.s   V_25
// 		/* 0x0008F8A1 7C390C0004   */ IL_0965: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F8A6 2200000000   */ IL_096A: ldc.r4    0.0
// 		/* 0x0008F8AB 7D4200000A   */ IL_096F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F8B0 1119         */ IL_0974: ldloc.s   V_25
// 		/* 0x0008F8B2 7C390C0004   */ IL_0976: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x0008F8B7 2200000000   */ IL_097B: ldc.r4    0.0
// 		/* 0x0008F8BC 7D8000000A   */ IL_0980: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F8C1 1119         */ IL_0985: ldloc.s   V_25
// 		/* 0x0008F8C3 2200000000   */ IL_0987: ldc.r4    0.0
// 		/* 0x0008F8C8 7D3A0C0004   */ IL_098C: stfld     float32 ShipData::uAngularSpeed
// 		/* 0x0008F8CD 3872250000   */ IL_0991: br        IL_2F08

// 		/* 0x0008F8D2 1119         */ IL_0996: ldloc.s   V_25
// 		/* 0x0008F8D4 7B310C0004   */ IL_0998: ldfld     int32 ShipData::stage
// 		/* 0x0008F8D9 3AAC160000   */ IL_099D: brtrue    IL_204E

// 		/* 0x0008F8DE 1119         */ IL_09A2: ldloc.s   V_25
// 		/* 0x0008F8E0 7B3E0C0004   */ IL_09A4: ldfld     int32 ShipData::direction
// 		/* 0x0008F8E5 16           */ IL_09A9: ldc.i4.0
// 		/* 0x0008F8E6 3E9C000000   */ IL_09AA: ble       IL_0A4B

// 		/* 0x0008F8EB 0E06         */ IL_09AF: ldarg.s   gStationPool
// 		/* 0x0008F8ED 1119         */ IL_09B1: ldloc.s   V_25
// 		/* 0x0008F8EF 7B3D0C0004   */ IL_09B3: ldfld     int32 ShipData::otherGId
// 		/* 0x0008F8F4 9A           */ IL_09B8: ldelem.ref
// 		/* 0x0008F8F5 7BC90B0004   */ IL_09B9: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0008F8FA 1310         */ IL_09BE: stloc.s   V_16
// 		/* 0x0008F8FC 1110         */ IL_09C0: ldloc.s   V_16
// 		/* 0x0008F8FE 7B4100000A   */ IL_09C2: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F903 1110         */ IL_09C7: ldloc.s   V_16
// 		/* 0x0008F905 7B4100000A   */ IL_09C9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F90A 5A           */ IL_09CE: mul
// 		/* 0x0008F90B 1110         */ IL_09CF: ldloc.s   V_16
// 		/* 0x0008F90D 7B4200000A   */ IL_09D1: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F912 1110         */ IL_09D6: ldloc.s   V_16
// 		/* 0x0008F914 7B4200000A   */ IL_09D8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F919 5A           */ IL_09DD: mul
// 		/* 0x0008F91A 58           */ IL_09DE: add
// 		/* 0x0008F91B 1110         */ IL_09DF: ldloc.s   V_16
// 		/* 0x0008F91D 7B8000000A   */ IL_09E1: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F922 1110         */ IL_09E6: ldloc.s   V_16
// 		/* 0x0008F924 7B8000000A   */ IL_09E8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F929 5A           */ IL_09ED: mul
// 		/* 0x0008F92A 58           */ IL_09EE: add
// 		/* 0x0008F92B 6C           */ IL_09EF: conv.r8
// 		/* 0x0008F92C 284B02000A   */ IL_09F0: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x0008F931 6B           */ IL_09F5: conv.r4
// 		/* 0x0008F932 1330         */ IL_09F6: stloc.s   V_48
// 		/* 0x0008F934 220000803F   */ IL_09F8: ldc.r4    1
// 		/* 0x0008F939 220000C841   */ IL_09FD: ldc.r4    25
// 		/* 0x0008F93E 1130         */ IL_0A02: ldloc.s   V_48
// 		/* 0x0008F940 5B           */ IL_0A04: div
// 		/* 0x0008F941 58           */ IL_0A05: add
// 		/* 0x0008F942 1330         */ IL_0A06: stloc.s   V_48
// 		/* 0x0008F944 1210         */ IL_0A08: ldloca.s  V_16
// 		/* 0x0008F946 7C4100000A   */ IL_0A0A: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F94B 25           */ IL_0A0F: dup
// 		/* 0x0008F94C 4E           */ IL_0A10: ldind.r4
// 		/* 0x0008F94D 1130         */ IL_0A11: ldloc.s   V_48
// 		/* 0x0008F94F 5A           */ IL_0A13: mul
// 		/* 0x0008F950 56           */ IL_0A14: stind.r4
// 		/* 0x0008F951 1210         */ IL_0A15: ldloca.s  V_16
// 		/* 0x0008F953 7C4200000A   */ IL_0A17: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F958 25           */ IL_0A1C: dup
// 		/* 0x0008F959 4E           */ IL_0A1D: ldind.r4
// 		/* 0x0008F95A 1130         */ IL_0A1E: ldloc.s   V_48
// 		/* 0x0008F95C 5A           */ IL_0A20: mul
// 		/* 0x0008F95D 56           */ IL_0A21: stind.r4
// 		/* 0x0008F95E 1210         */ IL_0A22: ldloca.s  V_16
// 		/* 0x0008F960 7C8000000A   */ IL_0A24: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F965 25           */ IL_0A29: dup
// 		/* 0x0008F966 4E           */ IL_0A2A: ldind.r4
// 		/* 0x0008F967 1130         */ IL_0A2B: ldloc.s   V_48
// 		/* 0x0008F969 5A           */ IL_0A2D: mul
// 		/* 0x0008F96A 56           */ IL_0A2E: stind.r4
// 		/* 0x0008F96B 111C         */ IL_0A2F: ldloc.s   V_28
// 		/* 0x0008F96D 7C46180004   */ IL_0A31: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008F972 111C         */ IL_0A36: ldloc.s   V_28
// 		/* 0x0008F974 7C44180004   */ IL_0A38: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008F979 1210         */ IL_0A3D: ldloca.s  V_16
// 		/* 0x0008F97B 1225         */ IL_0A3F: ldloca.s  V_37
// 		/* 0x0008F97D 28140A0006   */ IL_0A41: call      void StationComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype VectorLF3&)
// 		/* 0x0008F982 389A000000   */ IL_0A46: br        IL_0AE5

// 		/* 0x0008F987 02           */ IL_0A4B: ldarg.0
// 		/* 0x0008F988 7BFD0B0004   */ IL_0A4C: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3[] StationComponent::shipDiskPos
// 		/* 0x0008F98D 1119         */ IL_0A51: ldloc.s   V_25
// 		/* 0x0008F98F 7B440C0004   */ IL_0A53: ldfld     int32 ShipData::shipIndex
// 		/* 0x0008F994 A310000001   */ IL_0A58: ldelem    [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x0008F999 1310         */ IL_0A5D: stloc.s   V_16
// 		/* 0x0008F99B 1110         */ IL_0A5F: ldloc.s   V_16
// 		/* 0x0008F99D 7B4100000A   */ IL_0A61: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F9A2 1110         */ IL_0A66: ldloc.s   V_16
// 		/* 0x0008F9A4 7B4100000A   */ IL_0A68: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F9A9 5A           */ IL_0A6D: mul
// 		/* 0x0008F9AA 1110         */ IL_0A6E: ldloc.s   V_16
// 		/* 0x0008F9AC 7B4200000A   */ IL_0A70: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F9B1 1110         */ IL_0A75: ldloc.s   V_16
// 		/* 0x0008F9B3 7B4200000A   */ IL_0A77: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F9B8 5A           */ IL_0A7C: mul
// 		/* 0x0008F9B9 58           */ IL_0A7D: add
// 		/* 0x0008F9BA 1110         */ IL_0A7E: ldloc.s   V_16
// 		/* 0x0008F9BC 7B8000000A   */ IL_0A80: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F9C1 1110         */ IL_0A85: ldloc.s   V_16
// 		/* 0x0008F9C3 7B8000000A   */ IL_0A87: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008F9C8 5A           */ IL_0A8C: mul
// 		/* 0x0008F9C9 58           */ IL_0A8D: add
// 		/* 0x0008F9CA 6C           */ IL_0A8E: conv.r8
// 		/* 0x0008F9CB 284B02000A   */ IL_0A8F: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x0008F9D0 6B           */ IL_0A94: conv.r4
// 		/* 0x0008F9D1 1331         */ IL_0A95: stloc.s   V_49
// 		/* 0x0008F9D3 220000803F   */ IL_0A97: ldc.r4    1
// 		/* 0x0008F9D8 220000C841   */ IL_0A9C: ldc.r4    25
// 		/* 0x0008F9DD 1131         */ IL_0AA1: ldloc.s   V_49
// 		/* 0x0008F9DF 5B           */ IL_0AA3: div
// 		/* 0x0008F9E0 58           */ IL_0AA4: add
// 		/* 0x0008F9E1 1331         */ IL_0AA5: stloc.s   V_49
// 		/* 0x0008F9E3 1210         */ IL_0AA7: ldloca.s  V_16
// 		/* 0x0008F9E5 7C4100000A   */ IL_0AA9: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0008F9EA 25           */ IL_0AAE: dup
// 		/* 0x0008F9EB 4E           */ IL_0AAF: ldind.r4
// 		/* 0x0008F9EC 1131         */ IL_0AB0: ldloc.s   V_49
// 		/* 0x0008F9EE 5A           */ IL_0AB2: mul
// 		/* 0x0008F9EF 56           */ IL_0AB3: stind.r4
// 		/* 0x0008F9F0 1210         */ IL_0AB4: ldloca.s  V_16
// 		/* 0x0008F9F2 7C4200000A   */ IL_0AB6: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0008F9F7 25           */ IL_0ABB: dup
// 		/* 0x0008F9F8 4E           */ IL_0ABC: ldind.r4
// 		/* 0x0008F9F9 1131         */ IL_0ABD: ldloc.s   V_49
// 		/* 0x0008F9FB 5A           */ IL_0ABF: mul
// 		/* 0x0008F9FC 56           */ IL_0AC0: stind.r4
// 		/* 0x0008F9FD 1210         */ IL_0AC1: ldloca.s  V_16
// 		/* 0x0008F9FF 7C8000000A   */ IL_0AC3: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0008FA04 25           */ IL_0AC8: dup
// 		/* 0x0008FA05 4E           */ IL_0AC9: ldind.r4
// 		/* 0x0008FA06 1131         */ IL_0ACA: ldloc.s   V_49
// 		/* 0x0008FA08 5A           */ IL_0ACC: mul
// 		/* 0x0008FA09 56           */ IL_0ACD: stind.r4
// 		/* 0x0008FA0A 110B         */ IL_0ACE: ldloc.s   V_11
// 		/* 0x0008FA0C 7C46180004   */ IL_0AD0: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FA11 110B         */ IL_0AD5: ldloc.s   V_11
// 		/* 0x0008FA13 7C44180004   */ IL_0AD7: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0008FA18 1210         */ IL_0ADC: ldloca.s  V_16
// 		/* 0x0008FA1A 1225         */ IL_0ADE: ldloca.s  V_37
// 		/* 0x0008FA1C 28140A0006   */ IL_0AE0: call      void StationComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype VectorLF3&)

// 		/* 0x0008FA21 1226         */ IL_0AE5: ldloca.s  V_38
// 		/* 0x0008FA23 1125         */ IL_0AE7: ldloc.s   V_37
// 		/* 0x0008FA25 7B41030004   */ IL_0AE9: ldfld     float64 VectorLF3::x
// 		/* 0x0008FA2A 1119         */ IL_0AEE: ldloc.s   V_25
// 		/* 0x0008FA2C 7C340C0004   */ IL_0AF0: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FA31 7B41030004   */ IL_0AF5: ldfld     float64 VectorLF3::x
// 		/* 0x0008FA36 59           */ IL_0AFA: sub
// 		/* 0x0008FA37 7D41030004   */ IL_0AFB: stfld     float64 VectorLF3::x
// 		/* 0x0008FA3C 1226         */ IL_0B00: ldloca.s  V_38
// 		/* 0x0008FA3E 1125         */ IL_0B02: ldloc.s   V_37
// 		/* 0x0008FA40 7B42030004   */ IL_0B04: ldfld     float64 VectorLF3::y
// 		/* 0x0008FA45 1119         */ IL_0B09: ldloc.s   V_25
// 		/* 0x0008FA47 7C340C0004   */ IL_0B0B: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FA4C 7B42030004   */ IL_0B10: ldfld     float64 VectorLF3::y
// 		/* 0x0008FA51 59           */ IL_0B15: sub
// 		/* 0x0008FA52 7D42030004   */ IL_0B16: stfld     float64 VectorLF3::y
// 		/* 0x0008FA57 1226         */ IL_0B1B: ldloca.s  V_38
// 		/* 0x0008FA59 1125         */ IL_0B1D: ldloc.s   V_37
// 		/* 0x0008FA5B 7B43030004   */ IL_0B1F: ldfld     float64 VectorLF3::z
// 		/* 0x0008FA60 1119         */ IL_0B24: ldloc.s   V_25
// 		/* 0x0008FA62 7C340C0004   */ IL_0B26: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FA67 7B43030004   */ IL_0B2B: ldfld     float64 VectorLF3::z
// 		/* 0x0008FA6C 59           */ IL_0B30: sub
// 		/* 0x0008FA6D 7D43030004   */ IL_0B31: stfld     float64 VectorLF3::z
// 		/* 0x0008FA72 1126         */ IL_0B36: ldloc.s   V_38
// 		/* 0x0008FA74 7B41030004   */ IL_0B38: ldfld     float64 VectorLF3::x
// 		/* 0x0008FA79 1126         */ IL_0B3D: ldloc.s   V_38
// 		/* 0x0008FA7B 7B41030004   */ IL_0B3F: ldfld     float64 VectorLF3::x
// 		/* 0x0008FA80 5A           */ IL_0B44: mul
// 		/* 0x0008FA81 1126         */ IL_0B45: ldloc.s   V_38
// 		/* 0x0008FA83 7B42030004   */ IL_0B47: ldfld     float64 VectorLF3::y
// 		/* 0x0008FA88 1126         */ IL_0B4C: ldloc.s   V_38
// 		/* 0x0008FA8A 7B42030004   */ IL_0B4E: ldfld     float64 VectorLF3::y
// 		/* 0x0008FA8F 5A           */ IL_0B53: mul
// 		/* 0x0008FA90 58           */ IL_0B54: add
// 		/* 0x0008FA91 1126         */ IL_0B55: ldloc.s   V_38
// 		/* 0x0008FA93 7B43030004   */ IL_0B57: ldfld     float64 VectorLF3::z
// 		/* 0x0008FA98 1126         */ IL_0B5C: ldloc.s   V_38
// 		/* 0x0008FA9A 7B43030004   */ IL_0B5E: ldfld     float64 VectorLF3::z
// 		/* 0x0008FA9F 5A           */ IL_0B63: mul
// 		/* 0x0008FAA0 58           */ IL_0B64: add
// 		/* 0x0008FAA1 284B02000A   */ IL_0B65: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x0008FAA6 1327         */ IL_0B6A: stloc.s   V_39
// 		/* 0x0008FAA8 1119         */ IL_0B6C: ldloc.s   V_25
// 		/* 0x0008FAAA 7B3E0C0004   */ IL_0B6E: ldfld     int32 ShipData::direction
// 		/* 0x0008FAAF 16           */ IL_0B73: ldc.i4.0
// 		/* 0x0008FAB0 3162         */ IL_0B74: ble.s     IL_0BD8

// 		/* 0x0008FAB2 1228         */ IL_0B76: ldloca.s  V_40
// 		/* 0x0008FAB4 110B         */ IL_0B78: ldloc.s   V_11
// 		/* 0x0008FAB6 7C46180004   */ IL_0B7A: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FABB 7B41030004   */ IL_0B7F: ldfld     float64 VectorLF3::x
// 		/* 0x0008FAC0 1119         */ IL_0B84: ldloc.s   V_25
// 		/* 0x0008FAC2 7C340C0004   */ IL_0B86: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FAC7 7B41030004   */ IL_0B8B: ldfld     float64 VectorLF3::x
// 		/* 0x0008FACC 59           */ IL_0B90: sub
// 		/* 0x0008FACD 7D41030004   */ IL_0B91: stfld     float64 VectorLF3::x
// 		/* 0x0008FAD2 1228         */ IL_0B96: ldloca.s  V_40
// 		/* 0x0008FAD4 110B         */ IL_0B98: ldloc.s   V_11
// 		/* 0x0008FAD6 7C46180004   */ IL_0B9A: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FADB 7B42030004   */ IL_0B9F: ldfld     float64 VectorLF3::y
// 		/* 0x0008FAE0 1119         */ IL_0BA4: ldloc.s   V_25
// 		/* 0x0008FAE2 7C340C0004   */ IL_0BA6: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FAE7 7B42030004   */ IL_0BAB: ldfld     float64 VectorLF3::y
// 		/* 0x0008FAEC 59           */ IL_0BB0: sub
// 		/* 0x0008FAED 7D42030004   */ IL_0BB1: stfld     float64 VectorLF3::y
// 		/* 0x0008FAF2 1228         */ IL_0BB6: ldloca.s  V_40
// 		/* 0x0008FAF4 110B         */ IL_0BB8: ldloc.s   V_11
// 		/* 0x0008FAF6 7C46180004   */ IL_0BBA: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FAFB 7B43030004   */ IL_0BBF: ldfld     float64 VectorLF3::z
// 		/* 0x0008FB00 1119         */ IL_0BC4: ldloc.s   V_25
// 		/* 0x0008FB02 7C340C0004   */ IL_0BC6: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FB07 7B43030004   */ IL_0BCB: ldfld     float64 VectorLF3::z
// 		/* 0x0008FB0C 59           */ IL_0BD0: sub
// 		/* 0x0008FB0D 7D43030004   */ IL_0BD1: stfld     float64 VectorLF3::z
// 		/* 0x0008FB12 2B60         */ IL_0BD6: br.s      IL_0C38

// 		/* 0x0008FB14 1228         */ IL_0BD8: ldloca.s  V_40
// 		/* 0x0008FB16 111C         */ IL_0BDA: ldloc.s   V_28
// 		/* 0x0008FB18 7C46180004   */ IL_0BDC: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FB1D 7B41030004   */ IL_0BE1: ldfld     float64 VectorLF3::x
// 		/* 0x0008FB22 1119         */ IL_0BE6: ldloc.s   V_25
// 		/* 0x0008FB24 7C340C0004   */ IL_0BE8: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FB29 7B41030004   */ IL_0BED: ldfld     float64 VectorLF3::x
// 		/* 0x0008FB2E 59           */ IL_0BF2: sub
// 		/* 0x0008FB2F 7D41030004   */ IL_0BF3: stfld     float64 VectorLF3::x
// 		/* 0x0008FB34 1228         */ IL_0BF8: ldloca.s  V_40
// 		/* 0x0008FB36 111C         */ IL_0BFA: ldloc.s   V_28
// 		/* 0x0008FB38 7C46180004   */ IL_0BFC: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FB3D 7B42030004   */ IL_0C01: ldfld     float64 VectorLF3::y
// 		/* 0x0008FB42 1119         */ IL_0C06: ldloc.s   V_25
// 		/* 0x0008FB44 7C340C0004   */ IL_0C08: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FB49 7B42030004   */ IL_0C0D: ldfld     float64 VectorLF3::y
// 		/* 0x0008FB4E 59           */ IL_0C12: sub
// 		/* 0x0008FB4F 7D42030004   */ IL_0C13: stfld     float64 VectorLF3::y
// 		/* 0x0008FB54 1228         */ IL_0C18: ldloca.s  V_40
// 		/* 0x0008FB56 111C         */ IL_0C1A: ldloc.s   V_28
// 		/* 0x0008FB58 7C46180004   */ IL_0C1C: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0008FB5D 7B43030004   */ IL_0C21: ldfld     float64 VectorLF3::z
// 		/* 0x0008FB62 1119         */ IL_0C26: ldloc.s   V_25
// 		/* 0x0008FB64 7C340C0004   */ IL_0C28: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x0008FB69 7B43030004   */ IL_0C2D: ldfld     float64 VectorLF3::z
// 		/* 0x0008FB6E 59           */ IL_0C32: sub
// 		/* 0x0008FB6F 7D43030004   */ IL_0C33: stfld     float64 VectorLF3::z

// 		/* 0x0008FB74 1128         */ IL_0C38: ldloc.s   V_40
// 		/* 0x0008FB76 7B41030004   */ IL_0C3A: ldfld     float64 VectorLF3::x
// 		/* 0x0008FB7B 1128         */ IL_0C3F: ldloc.s   V_40
// 		/* 0x0008FB7D 7B41030004   */ IL_0C41: ldfld     float64 VectorLF3::x
// 		/* 0x0008FB82 5A           */ IL_0C46: mul
// 		/* 0x0008FB83 1128         */ IL_0C47: ldloc.s   V_40
// 		/* 0x0008FB85 7B42030004   */ IL_0C49: ldfld     float64 VectorLF3::y
// 		/* 0x0008FB8A 1128         */ IL_0C4E: ldloc.s   V_40
// 		/* 0x0008FB8C 7B42030004   */ IL_0C50: ldfld     float64 VectorLF3::y
// 		/* 0x0008FB91 5A           */ IL_0C55: mul
// 		/* 0x0008FB92 58           */ IL_0C56: add
// 		/* 0x0008FB93 1128         */ IL_0C57: ldloc.s   V_40
// 		/* 0x0008FB95 7B43030004   */ IL_0C59: ldfld     float64 VectorLF3::z
// 		/* 0x0008FB9A 1128         */ IL_0C5E: ldloc.s   V_40
// 		/* 0x0008FB9C 7B43030004   */ IL_0C60: ldfld     float64 VectorLF3::z
// 		/* 0x0008FBA1 5A           */ IL_0C65: mul
// 		/* 0x0008FBA2 58           */ IL_0C66: add
// 		/* 0x0008FBA3 1329         */ IL_0C67: stloc.s   V_41
// 		/* 0x0008FBA5 1129         */ IL_0C69: ldloc.s   V_41
// 		/* 0x0008FBA7 110B         */ IL_0C6B: ldloc.s   V_11
// 		/* 0x0008FBA9 7B43180004   */ IL_0C6D: ldfld     float32 AstroData::uRadius
// 		/* 0x0008FBAE 110B         */ IL_0C72: ldloc.s   V_11
// 		/* 0x0008FBB0 7B43180004   */ IL_0C74: ldfld     float32 AstroData::uRadius
// 		/* 0x0008FBB5 5A           */ IL_0C79: mul
// 		/* 0x0008FBB6 6C           */ IL_0C7A: conv.r8
// 		/* 0x0008FBB7 230000000000000240 */ IL_0C7B: ldc.r8    2.25
// 		/* 0x0008FBC0 5A           */ IL_0C84: mul
// 		/* 0x0008FBC1 FE03         */ IL_0C85: cgt.un
// 		/* 0x0008FBC3 16           */ IL_0C87: ldc.i4.0
// 		/* 0x0008FBC4 FE01         */ IL_0C88: ceq
// 		/* 0x0008FBC6 132A         */ IL_0C8A: stloc.s   V_42
// 		/* 0x0008FBC8 16           */ IL_0C8C: ldc.i4.0
// 		/* 0x0008FBC9 132B         */ IL_0C8D: stloc.s   V_43
// 		/* 0x0008FBCB 1127         */ IL_0C8F: ldloc.s   V_39
// 		/* 0x0008FBCD 220000C040   */ IL_0C91: ldc.r4    6
// 		/* 0x0008FBD2 110A         */ IL_0C96: ldloc.s   V_10
// 		/* 0x0008FBD4 5A           */ IL_0C98: mul
// 		/* 0x0008FBD5 6C           */ IL_0C99: conv.r8
// 		/* 0x0008FBD6 341D         */ IL_0C9A: bge.un.s  IL_0CB9

// 		/* 0x0008FBD8 1119         */ IL_0C9C: ldloc.s   V_25
// 		/* 0x0008FBDA 220000803F   */ IL_0C9E: ldc.r4    1
// 		/* 0x0008FBDF 7D3F0C0004   */ IL_0CA3: stfld     float32 ShipData::t
// 		/* 0x0008FBE4 1119         */ IL_0CA8: ldloc.s   V_25
// 		/* 0x0008FBE6 1119         */ IL_0CAA: ldloc.s   V_25
// 		/* 0x0008FBE8 7B3E0C0004   */ IL_0CAC: ldfld     int32 ShipData::direction
// 		/* 0x0008FBED 7D310C0004   */ IL_0CB1: stfld     int32 ShipData::stage
// 		/* 0x0008FBF2 17           */ IL_0CB6: ldc.i4.1
// 		/* 0x0008FBF3 132B         */ IL_0CB7: stloc.s   V_43

// 		/* 0x0008FBF5 17           */ IL_0CB9: ldc.i4.1
// 		/* 0x0008FBF6 132C         */ IL_0CBA: stloc.s   V_44
// 		/* 0x0008FBF8 2200000000   */ IL_0CBC: ldc.r4    0.0
// 		/* 0x0008FBFD 132D         */ IL_0CC1: stloc.s   V_45
// 		/* 0x0008FBFF 06           */ IL_0CC3: ldloc.0
// 		/* 0x0008FC00 39E1010000   */ IL_0CC4: brfalse   IL_0EAA

// 		/* 0x0008FC05 1112         */ IL_0CC9: ldloc.s   V_18
// 		/* 0x0008FC07 230000000000000040 */ IL_0CCB: ldc.r8    2
// 		/* 0x0008FC10 5A           */ IL_0CD4: mul
// 		/* 0x0008FC11 1332         */ IL_0CD5: stloc.s   V_50
// 		/* 0x0008FC13 0E04         */ IL_0CD7: ldarg.s   shipWarpSpeed
// 		/* 0x0008FC15 6C           */ IL_0CD9: conv.r8
// 		/* 0x0008FC16 1132         */ IL_0CDA: ldloc.s   V_50
// 		/* 0x0008FC18 3204         */ IL_0CDC: blt.s     IL_0CE2

// 		/* 0x0008FC1A 1132         */ IL_0CDE: ldloc.s   V_50
// 		/* 0x0008FC1C 2B03         */ IL_0CE0: br.s      IL_0CE5

// 		/* 0x0008FC1E 0E04         */ IL_0CE2: ldarg.s   shipWarpSpeed
// 		/* 0x0008FC20 6C           */ IL_0CE4: conv.r8

// 		/* 0x0008FC21 1333         */ IL_0CE5: stloc.s   V_51
// 		/* 0x0008FC23 02           */ IL_0CE7: ldarg.0
// 		/* 0x0008FC24 7BEF0B0004   */ IL_0CE8: ldfld     float64 StationComponent::warpEnableDist
// 		/* 0x0008FC29 23000000000000E03F */ IL_0CED: ldc.r8    0.5
// 		/* 0x0008FC32 5A           */ IL_0CF6: mul
// 		/* 0x0008FC33 1334         */ IL_0CF7: stloc.s   V_52
// 		/* 0x0008FC35 1119         */ IL_0CF9: ldloc.s   V_25
// 		/* 0x0008FC37 7B370C0004   */ IL_0CFB: ldfld     float32 ShipData::warpState
// 		/* 0x0008FC3C 2200000000   */ IL_0D00: ldc.r4    0.0
// 		/* 0x0008FC41 3568         */ IL_0D05: bgt.un.s  IL_0D6F

// 		/* 0x0008FC43 1119         */ IL_0D07: ldloc.s   V_25
// 		/* 0x0008FC45 2200000000   */ IL_0D09: ldc.r4    0.0
// 		/* 0x0008FC4A 7D370C0004   */ IL_0D0E: stfld     float32 ShipData::warpState
// 		/* 0x0008FC4F 1129         */ IL_0D13: ldloc.s   V_41
// 		/* 0x0008FC51 230000000084D77741 */ IL_0D15: ldc.r8    25000000
// 		/* 0x0008FC5A 4387010000   */ IL_0D1E: ble.un    IL_0EAA

// 		/* 0x0008FC5F 1127         */ IL_0D23: ldloc.s   V_39
// 		/* 0x0008FC61 1134         */ IL_0D25: ldloc.s   V_52
// 		/* 0x0008FC63 437E010000   */ IL_0D27: ble.un    IL_0EAA

// 		/* 0x0008FC68 1119         */ IL_0D2C: ldloc.s   V_25
// 		/* 0x0008FC6A 7B360C0004   */ IL_0D2E: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FC6F 05           */ IL_0D33: ldarg.3
// 		/* 0x0008FC70 4471010000   */ IL_0D34: blt.un    IL_0EAA

// 		/* 0x0008FC75 1119         */ IL_0D39: ldloc.s   V_25
// 		/* 0x0008FC77 7B450C0004   */ IL_0D3B: ldfld     int32 ShipData::warperCnt
// 		/* 0x0008FC7C 16           */ IL_0D40: ldc.i4.0
// 		/* 0x0008FC7D 300B         */ IL_0D41: bgt.s     IL_0D4E

// 		/* 0x0008FC7F 02           */ IL_0D43: ldarg.0
// 		/* 0x0008FC80 7B070C0004   */ IL_0D44: ldfld     bool StationComponent::warperFree
// 		/* 0x0008FC85 395C010000   */ IL_0D49: brfalse   IL_0EAA

// 		/* 0x0008FC8A 1119         */ IL_0D4E: ldloc.s   V_25
// 		/* 0x0008FC8C 7C450C0004   */ IL_0D50: ldflda    int32 ShipData::warperCnt
// 		/* 0x0008FC91 25           */ IL_0D55: dup
// 		/* 0x0008FC92 4A           */ IL_0D56: ldind.i4
// 		/* 0x0008FC93 17           */ IL_0D57: ldc.i4.1
// 		/* 0x0008FC94 59           */ IL_0D58: sub
// 		/* 0x0008FC95 54           */ IL_0D59: stind.i4
// 		/* 0x0008FC96 1119         */ IL_0D5A: ldloc.s   V_25
// 		/* 0x0008FC98 7C370C0004   */ IL_0D5C: ldflda    float32 ShipData::warpState
// 		/* 0x0008FC9D 25           */ IL_0D61: dup
// 		/* 0x0008FC9E 4E           */ IL_0D62: ldind.r4
// 		/* 0x0008FC9F 228988883C   */ IL_0D63: ldc.r4    0.016666668
// 		/* 0x0008FCA4 58           */ IL_0D68: add
// 		/* 0x0008FCA5 56           */ IL_0D69: stind.r4
// 		/* 0x0008FCA6 383B010000   */ IL_0D6A: br        IL_0EAA

// 		/* 0x0008FCAB 1133         */ IL_0D6F: ldloc.s   V_51
// 		/* 0x0008FCAD 230000000000488F40 */ IL_0D71: ldc.r8    1001
// 		/* 0x0008FCB6 1119         */ IL_0D7A: ldloc.s   V_25
// 		/* 0x0008FCB8 7B370C0004   */ IL_0D7C: ldfld     float32 ShipData::warpState
// 		/* 0x0008FCBD 6C           */ IL_0D81: conv.r8
// 		/* 0x0008FCBE 282F01000A   */ IL_0D82: call      float64 [netstandard]System.Math::Pow(float64, float64)
// 		/* 0x0008FCC3 23000000000000F03F */ IL_0D87: ldc.r8    1
// 		/* 0x0008FCCC 59           */ IL_0D90: sub
// 		/* 0x0008FCCD 230000000000408F40 */ IL_0D91: ldc.r8    1000
// 		/* 0x0008FCD6 5B           */ IL_0D9A: div
// 		/* 0x0008FCD7 5A           */ IL_0D9B: mul
// 		/* 0x0008FCD8 6B           */ IL_0D9C: conv.r4
// 		/* 0x0008FCD9 132D         */ IL_0D9D: stloc.s   V_45
// 		/* 0x0008FCDB 112D         */ IL_0D9F: ldloc.s   V_45
// 		/* 0x0008FCDD 6C           */ IL_0DA1: conv.r8
// 		/* 0x0008FCDE 23E9482EFF21FDA63F */ IL_0DA2: ldc.r8    0.0449
// 		/* 0x0008FCE7 5A           */ IL_0DAB: mul
// 		/* 0x0008FCE8 23000000000088B340 */ IL_0DAC: ldc.r8    5000
// 		/* 0x0008FCF1 58           */ IL_0DB5: add
// 		/* 0x0008FCF2 05           */ IL_0DB6: ldarg.3
// 		/* 0x0008FCF3 6C           */ IL_0DB7: conv.r8
// 		/* 0x0008FCF4 23000000000000D03F */ IL_0DB8: ldc.r8    0.25
// 		/* 0x0008FCFD 5A           */ IL_0DC1: mul
// 		/* 0x0008FCFE 58           */ IL_0DC2: add
// 		/* 0x0008FCFF 1335         */ IL_0DC3: stloc.s   V_53
// 		/* 0x0008FD01 1127         */ IL_0DC5: ldloc.s   V_39
// 		/* 0x0008FD03 1135         */ IL_0DC7: ldloc.s   V_53
// 		/* 0x0008FD05 59           */ IL_0DC9: sub
// 		/* 0x0008FD06 1336         */ IL_0DCA: stloc.s   V_54
// 		/* 0x0008FD08 1136         */ IL_0DCC: ldloc.s   V_54
// 		/* 0x0008FD0A 230000000000000000 */ IL_0DCE: ldc.r8    0.0
// 		/* 0x0008FD13 340B         */ IL_0DD7: bge.un.s  IL_0DE4

// 		/* 0x0008FD15 230000000000000000 */ IL_0DD9: ldc.r8    0.0
// 		/* 0x0008FD1E 1336         */ IL_0DE2: stloc.s   V_54

// 		/* 0x0008FD20 1127         */ IL_0DE4: ldloc.s   V_39
// 		/* 0x0008FD22 1135         */ IL_0DE6: ldloc.s   V_53
// 		/* 0x0008FD24 3412         */ IL_0DE8: bge.un.s  IL_0DFC

// 		/* 0x0008FD26 1119         */ IL_0DEA: ldloc.s   V_25
// 		/* 0x0008FD28 7C370C0004   */ IL_0DEC: ldflda    float32 ShipData::warpState
// 		/* 0x0008FD2D 25           */ IL_0DF1: dup
// 		/* 0x0008FD2E 4E           */ IL_0DF2: ldind.r4
// 		/* 0x0008FD2F 228988883D   */ IL_0DF3: ldc.r4    0.06666667
// 		/* 0x0008FD34 59           */ IL_0DF8: sub
// 		/* 0x0008FD35 56           */ IL_0DF9: stind.r4
// 		/* 0x0008FD36 2B10         */ IL_0DFA: br.s      IL_0E0C

// 		/* 0x0008FD38 1119         */ IL_0DFC: ldloc.s   V_25
// 		/* 0x0008FD3A 7C370C0004   */ IL_0DFE: ldflda    float32 ShipData::warpState
// 		/* 0x0008FD3F 25           */ IL_0E03: dup
// 		/* 0x0008FD40 4E           */ IL_0E04: ldind.r4
// 		/* 0x0008FD41 228988883C   */ IL_0E05: ldc.r4    0.016666668
// 		/* 0x0008FD46 58           */ IL_0E0A: add
// 		/* 0x0008FD47 56           */ IL_0E0B: stind.r4

// 		/* 0x0008FD48 1119         */ IL_0E0C: ldloc.s   V_25
// 		/* 0x0008FD4A 7B370C0004   */ IL_0E0E: ldfld     float32 ShipData::warpState
// 		/* 0x0008FD4F 2200000000   */ IL_0E13: ldc.r4    0.0
// 		/* 0x0008FD54 340E         */ IL_0E18: bge.un.s  IL_0E28

// 		/* 0x0008FD56 1119         */ IL_0E1A: ldloc.s   V_25
// 		/* 0x0008FD58 2200000000   */ IL_0E1C: ldc.r4    0.0
// 		/* 0x0008FD5D 7D370C0004   */ IL_0E21: stfld     float32 ShipData::warpState
// 		/* 0x0008FD62 2B1A         */ IL_0E26: br.s      IL_0E42

// 		/* 0x0008FD64 1119         */ IL_0E28: ldloc.s   V_25
// 		/* 0x0008FD66 7B370C0004   */ IL_0E2A: ldfld     float32 ShipData::warpState
// 		/* 0x0008FD6B 220000803F   */ IL_0E2F: ldc.r4    1
// 		/* 0x0008FD70 360C         */ IL_0E34: ble.un.s  IL_0E42

// 		/* 0x0008FD72 1119         */ IL_0E36: ldloc.s   V_25
// 		/* 0x0008FD74 220000803F   */ IL_0E38: ldc.r4    1
// 		/* 0x0008FD79 7D370C0004   */ IL_0E3D: stfld     float32 ShipData::warpState

// 		/* 0x0008FD7E 1119         */ IL_0E42: ldloc.s   V_25
// 		/* 0x0008FD80 7B370C0004   */ IL_0E44: ldfld     float32 ShipData::warpState
// 		/* 0x0008FD85 2200000000   */ IL_0E49: ldc.r4    0.0
// 		/* 0x0008FD8A 365A         */ IL_0E4E: ble.un.s  IL_0EAA

// 		/* 0x0008FD8C 1133         */ IL_0E50: ldloc.s   V_51
// 		/* 0x0008FD8E 230000000000488F40 */ IL_0E52: ldc.r8    1001
// 		/* 0x0008FD97 1119         */ IL_0E5B: ldloc.s   V_25
// 		/* 0x0008FD99 7B370C0004   */ IL_0E5D: ldfld     float32 ShipData::warpState
// 		/* 0x0008FD9E 6C           */ IL_0E62: conv.r8
// 		/* 0x0008FD9F 282F01000A   */ IL_0E63: call      float64 [netstandard]System.Math::Pow(float64, float64)
// 		/* 0x0008FDA4 23000000000000F03F */ IL_0E68: ldc.r8    1
// 		/* 0x0008FDAD 59           */ IL_0E71: sub
// 		/* 0x0008FDAE 230000000000408F40 */ IL_0E72: ldc.r8    1000
// 		/* 0x0008FDB7 5B           */ IL_0E7B: div
// 		/* 0x0008FDB8 5A           */ IL_0E7C: mul
// 		/* 0x0008FDB9 6B           */ IL_0E7D: conv.r4
// 		/* 0x0008FDBA 132D         */ IL_0E7E: stloc.s   V_45
// 		/* 0x0008FDBC 112D         */ IL_0E80: ldloc.s   V_45
// 		/* 0x0008FDBE 6C           */ IL_0E82: conv.r8
// 		/* 0x0008FDBF 23111111111111913F */ IL_0E83: ldc.r8    0.016666666666666666
// 		/* 0x0008FDC8 5A           */ IL_0E8C: mul
// 		/* 0x0008FDC9 1136         */ IL_0E8D: ldloc.s   V_54
// 		/* 0x0008FDCB 3619         */ IL_0E8F: ble.un.s  IL_0EAA

// 		/* 0x0008FDCD 1136         */ IL_0E91: ldloc.s   V_54
// 		/* 0x0008FDCF 23111111111111913F */ IL_0E93: ldc.r8    0.016666666666666666
// 		/* 0x0008FDD8 5B           */ IL_0E9C: div
// 		/* 0x0008FDD9 23295C8FC2F528F03F */ IL_0E9D: ldc.r8    1.01
// 		/* 0x0008FDE2 5A           */ IL_0EA6: mul
// 		/* 0x0008FDE3 6B           */ IL_0EA7: conv.r4
// 		/* 0x0008FDE4 132D         */ IL_0EA8: stloc.s   V_45

// 		/* 0x0008FDE6 1129         */ IL_0EAA: ldloc.s   V_41
// 		/* 0x0008FDE8 23000000A2941A6D42 */ IL_0EAC: ldc.r8    1000000000000
// 		/* 0x0008FDF1 364E         */ IL_0EB5: ble.un.s  IL_0F05

// 		/* 0x0008FDF3 1129         */ IL_0EB7: ldloc.s   V_41
// 		/* 0x0008FDF5 05           */ IL_0EB9: ldarg.3
// 		/* 0x0008FDF6 05           */ IL_0EBA: ldarg.3
// 		/* 0x0008FDF7 5A           */ IL_0EBB: mul
// 		/* 0x0008FDF8 2200209945   */ IL_0EBC: ldc.r4    4900
// 		/* 0x0008FDFD 5A           */ IL_0EC1: mul
// 		/* 0x0008FDFE 6C           */ IL_0EC2: conv.r8
// 		/* 0x0008FDFF 3640         */ IL_0EC3: ble.un.s  IL_0F05

// 		/* 0x0008FE01 1127         */ IL_0EC5: ldloc.s   V_39
// 		/* 0x0008FE03 230000000080842E41 */ IL_0EC7: ldc.r8    1000000
// 		/* 0x0008FE0C 112D         */ IL_0ED0: ldloc.s   V_45
// 		/* 0x0008FE0E 6C           */ IL_0ED2: conv.r8
// 		/* 0x0008FE0F 239A9999999999E13F */ IL_0ED3: ldc.r8    0.55
// 		/* 0x0008FE18 5A           */ IL_0EDC: mul
// 		/* 0x0008FE19 58           */ IL_0EDD: add
// 		/* 0x0008FE1A 3606         */ IL_0EDE: ble.un.s  IL_0EE6

// 		/* 0x0008FE1C 1F1E         */ IL_0EE0: ldc.i4.s  30
// 		/* 0x0008FE1E 132C         */ IL_0EE2: stloc.s   V_44
// 		/* 0x0008FE20 2B1F         */ IL_0EE4: br.s      IL_0F05

// 		/* 0x0008FE22 1127         */ IL_0EE6: ldloc.s   V_39
// 		/* 0x0008FE24 230000000080842E41 */ IL_0EE8: ldc.r8    1000000
// 		/* 0x0008FE2D 112D         */ IL_0EF1: ldloc.s   V_45
// 		/* 0x0008FE2F 6C           */ IL_0EF3: conv.r8
// 		/* 0x0008FE30 239A9999999999C93F */ IL_0EF4: ldc.r8    0.2
// 		/* 0x0008FE39 5A           */ IL_0EFD: mul
// 		/* 0x0008FE3A 58           */ IL_0EFE: add
// 		/* 0x0008FE3B 3604         */ IL_0EFF: ble.un.s  IL_0F05

// 		/* 0x0008FE3D 1F0A         */ IL_0F01: ldc.i4.s  10
// 		/* 0x0008FE3F 132C         */ IL_0F03: stloc.s   V_44

// 		/* 0x0008FE41 122E         */ IL_0F05: ldloca.s  V_46
// 		/* 0x0008FE43 2200000000   */ IL_0F07: ldc.r4    0.0
// 		/* 0x0008FE48 2200000000   */ IL_0F0C: ldc.r4    0.0
// 		/* 0x0008FE4D 2200000000   */ IL_0F11: ldc.r4    0.0
// 		/* 0x0008FE52 28B3020006   */ IL_0F16: call      instance void VectorLF3::.ctor(float32, float32, float32)
// 		/* 0x0008FE57 112C         */ IL_0F1B: ldloc.s   V_44
// 		/* 0x0008FE59 17           */ IL_0F1D: ldc.i4.1
// 		/* 0x0008FE5A 2E13         */ IL_0F1E: beq.s     IL_0F33

// 		/* 0x0008FE5C 02           */ IL_0F20: ldarg.0
// 		/* 0x0008FE5D 7BC70B0004   */ IL_0F21: ldfld     int32 StationComponent::gene
// 		/* 0x0008FE62 1118         */ IL_0F26: ldloc.s   V_24
// 		/* 0x0008FE64 58           */ IL_0F28: add
// 		/* 0x0008FE65 04           */ IL_0F29: ldarg.2
// 		/* 0x0008FE66 58           */ IL_0F2A: add
// 		/* 0x0008FE67 112C         */ IL_0F2B: ldloc.s   V_44
// 		/* 0x0008FE69 5D           */ IL_0F2D: rem
// 		/* 0x0008FE6A 3A480F0000   */ IL_0F2E: brtrue    IL_1E7B

// 		/* 0x0008FE6F 1127         */ IL_0F33: ldloc.s   V_39
// 		/* 0x0008FE71 1119         */ IL_0F35: ldloc.s   V_25
// 		/* 0x0008FE73 7B360C0004   */ IL_0F37: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FE78 6C           */ IL_0F3C: conv.r8
// 		/* 0x0008FE79 239A9999999999B93F */ IL_0F3D: ldc.r8    0.1
// 		/* 0x0008FE82 58           */ IL_0F46: add
// 		/* 0x0008FE83 5B           */ IL_0F47: div
// 		/* 0x0008FE84 23A69BC420B072D83F */ IL_0F48: ldc.r8    0.382
// 		/* 0x0008FE8D 5A           */ IL_0F51: mul
// 		/* 0x0008FE8E 1337         */ IL_0F52: stloc.s   V_55
// 		/* 0x0008FE90 2200000000   */ IL_0F54: ldc.r4    0.0
// 		/* 0x0008FE95 1338         */ IL_0F59: stloc.s   V_56
// 		/* 0x0008FE97 1119         */ IL_0F5B: ldloc.s   V_25
// 		/* 0x0008FE99 7B370C0004   */ IL_0F5D: ldfld     float32 ShipData::warpState
// 		/* 0x0008FE9E 2200000000   */ IL_0F62: ldc.r4    0.0
// 		/* 0x0008FEA3 3622         */ IL_0F67: ble.un.s  IL_0F8B

// 		/* 0x0008FEA5 1119         */ IL_0F69: ldloc.s   V_25
// 		/* 0x0008FEA7 05           */ IL_0F6B: ldarg.3
// 		/* 0x0008FEA8 112D         */ IL_0F6C: ldloc.s   V_45
// 		/* 0x0008FEAA 58           */ IL_0F6E: add
// 		/* 0x0008FEAB 25           */ IL_0F6F: dup
// 		/* 0x0008FEAC 131E         */ IL_0F70: stloc.s   V_30
// 		/* 0x0008FEAE 7D360C0004   */ IL_0F72: stfld     float32 ShipData::uSpeed
// 		/* 0x0008FEB3 111E         */ IL_0F77: ldloc.s   V_30
// 		/* 0x0008FEB5 1338         */ IL_0F79: stloc.s   V_56
// 		/* 0x0008FEB7 1138         */ IL_0F7B: ldloc.s   V_56
// 		/* 0x0008FEB9 05           */ IL_0F7D: ldarg.3
// 		/* 0x0008FEBA 4392000000   */ IL_0F7E: ble.un    IL_1015

// 		/* 0x0008FEBF 05           */ IL_0F83: ldarg.3
// 		/* 0x0008FEC0 1338         */ IL_0F84: stloc.s   V_56
// 		/* 0x0008FEC2 388A000000   */ IL_0F86: br        IL_1015

// 		/* 0x0008FEC7 1119         */ IL_0F8B: ldloc.s   V_25
// 		/* 0x0008FEC9 7B360C0004   */ IL_0F8D: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FECE 6C           */ IL_0F92: conv.r8
// 		/* 0x0008FECF 1137         */ IL_0F93: ldloc.s   V_55
// 		/* 0x0008FED1 5A           */ IL_0F95: mul
// 		/* 0x0008FED2 110A         */ IL_0F96: ldloc.s   V_10
// 		/* 0x0008FED4 6C           */ IL_0F98: conv.r8
// 		/* 0x0008FED5 5A           */ IL_0F99: mul
// 		/* 0x0008FED6 6B           */ IL_0F9A: conv.r4
// 		/* 0x0008FED7 220000C040   */ IL_0F9B: ldc.r4    6
// 		/* 0x0008FEDC 1109         */ IL_0FA0: ldloc.s   V_9
// 		/* 0x0008FEDE 5A           */ IL_0FA2: mul
// 		/* 0x0008FEDF 58           */ IL_0FA3: add
// 		/* 0x0008FEE0 229A99193E   */ IL_0FA4: ldc.r4    0.15
// 		/* 0x0008FEE5 1108         */ IL_0FA9: ldloc.s   V_8
// 		/* 0x0008FEE7 5A           */ IL_0FAB: mul
// 		/* 0x0008FEE8 58           */ IL_0FAC: add
// 		/* 0x0008FEE9 133C         */ IL_0FAD: stloc.s   V_60
// 		/* 0x0008FEEB 113C         */ IL_0FAF: ldloc.s   V_60
// 		/* 0x0008FEED 05           */ IL_0FB1: ldarg.3
// 		/* 0x0008FEEE 3603         */ IL_0FB2: ble.un.s  IL_0FB7

// 		/* 0x0008FEF0 05           */ IL_0FB4: ldarg.3
// 		/* 0x0008FEF1 133C         */ IL_0FB5: stloc.s   V_60

// 		/* 0x0008FEF3 228988883C   */ IL_0FB7: ldc.r4    0.016666668
// 		/* 0x0008FEF8 112A         */ IL_0FBC: ldloc.s   V_42
// 		/* 0x0008FEFA 2D04         */ IL_0FBE: brtrue.s  IL_0FC4

// 		/* 0x0008FEFC 110D         */ IL_0FC0: ldloc.s   V_13
// 		/* 0x0008FEFE 2B02         */ IL_0FC2: br.s      IL_0FC6

// 		/* 0x0008FF00 110C         */ IL_0FC4: ldloc.s   V_12

// 		/* 0x0008FF02 5A           */ IL_0FC6: mul
// 		/* 0x0008FF03 133D         */ IL_0FC7: stloc.s   V_61
// 		/* 0x0008FF05 1119         */ IL_0FC9: ldloc.s   V_25
// 		/* 0x0008FF07 7B360C0004   */ IL_0FCB: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FF0C 113C         */ IL_0FD0: ldloc.s   V_60
// 		/* 0x0008FF0E 113D         */ IL_0FD2: ldloc.s   V_61
// 		/* 0x0008FF10 59           */ IL_0FD4: sub
// 		/* 0x0008FF11 340F         */ IL_0FD5: bge.un.s  IL_0FE6

// 		/* 0x0008FF13 1119         */ IL_0FD7: ldloc.s   V_25
// 		/* 0x0008FF15 7C360C0004   */ IL_0FD9: ldflda    float32 ShipData::uSpeed
// 		/* 0x0008FF1A 25           */ IL_0FDE: dup
// 		/* 0x0008FF1B 4E           */ IL_0FDF: ldind.r4
// 		/* 0x0008FF1C 113D         */ IL_0FE0: ldloc.s   V_61
// 		/* 0x0008FF1E 58           */ IL_0FE2: add
// 		/* 0x0008FF1F 56           */ IL_0FE3: stind.r4
// 		/* 0x0008FF20 2B26         */ IL_0FE4: br.s      IL_100C

// 		/* 0x0008FF22 1119         */ IL_0FE6: ldloc.s   V_25
// 		/* 0x0008FF24 7B360C0004   */ IL_0FE8: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FF29 113C         */ IL_0FED: ldloc.s   V_60
// 		/* 0x0008FF2B 110E         */ IL_0FEF: ldloc.s   V_14
// 		/* 0x0008FF2D 58           */ IL_0FF1: add
// 		/* 0x0008FF2E 360F         */ IL_0FF2: ble.un.s  IL_1003

// 		/* 0x0008FF30 1119         */ IL_0FF4: ldloc.s   V_25
// 		/* 0x0008FF32 7C360C0004   */ IL_0FF6: ldflda    float32 ShipData::uSpeed
// 		/* 0x0008FF37 25           */ IL_0FFB: dup
// 		/* 0x0008FF38 4E           */ IL_0FFC: ldind.r4
// 		/* 0x0008FF39 110E         */ IL_0FFD: ldloc.s   V_14
// 		/* 0x0008FF3B 59           */ IL_0FFF: sub
// 		/* 0x0008FF3C 56           */ IL_1000: stind.r4
// 		/* 0x0008FF3D 2B09         */ IL_1001: br.s      IL_100C

// 		/* 0x0008FF3F 1119         */ IL_1003: ldloc.s   V_25
// 		/* 0x0008FF41 113C         */ IL_1005: ldloc.s   V_60
// 		/* 0x0008FF43 7D360C0004   */ IL_1007: stfld     float32 ShipData::uSpeed

// 		/* 0x0008FF48 1119         */ IL_100C: ldloc.s   V_25
// 		/* 0x0008FF4A 7B360C0004   */ IL_100E: ldfld     float32 ShipData::uSpeed
// 		/* 0x0008FF4F 1338         */ IL_1013: stloc.s   V_56

// 		/* 0x0008FF51 15           */ IL_1015: ldc.i4.m1
// 		/* 0x0008FF52 1339         */ IL_1016: stloc.s   V_57
// 		/* 0x0008FF54 230000000000000000 */ IL_1018: ldc.r8    0.0
// 		/* 0x0008FF5D 133A         */ IL_1021: stloc.s   V_58
// 		/* 0x0008FF5F 23000000205FA02242 */ IL_1023: ldc.r8    40000000000
// 		/* 0x0008FF68 133B         */ IL_102C: stloc.s   V_59
// 		/* 0x0008FF6A 112C         */ IL_102E: ldloc.s   V_44
// 		/* 0x0008FF6C 17           */ IL_1030: ldc.i4.1
// 		/* 0x0008FF6D 40FB020000   */ IL_1031: bne.un    IL_1331

// 		/* 0x0008FF72 1119         */ IL_1036: ldloc.s   V_25
// 		/* 0x0008FF74 7B320C0004   */ IL_1038: ldfld     int32 ShipData::planetA
// 		/* 0x0008FF79 1F64         */ IL_103D: ldc.i4.s  100
// 		/* 0x0008FF7B 5B           */ IL_103F: div
// 		/* 0x0008FF7C 1F64         */ IL_1040: ldc.i4.s  100
// 		/* 0x0008FF7E 5A           */ IL_1042: mul
// 		/* 0x0008FF7F 133E         */ IL_1043: stloc.s   V_62
// 		/* 0x0008FF81 1119         */ IL_1045: ldloc.s   V_25
// 		/* 0x0008FF83 7B330C0004   */ IL_1047: ldfld     int32 ShipData::planetB
// 		/* 0x0008FF88 1F64         */ IL_104C: ldc.i4.s  100
// 		/* 0x0008FF8A 5B           */ IL_104E: div
// 		/* 0x0008FF8B 1F64         */ IL_104F: ldc.i4.s  100
// 		/* 0x0008FF8D 5A           */ IL_1051: mul
// 		/* 0x0008FF8E 133F         */ IL_1052: stloc.s   V_63
// 		/* 0x0008FF90 2200409C45   */ IL_1054: ldc.r4    5000
// 		/* 0x0008FF95 1138         */ IL_1059: ldloc.s   V_56
// 		/* 0x0008FF97 58           */ IL_105B: add
// 		/* 0x0008FF98 1341         */ IL_105C: stloc.s   V_65
// 		/* 0x0008FF9A 113E         */ IL_105E: ldloc.s   V_62
// 		/* 0x0008FF9C 1342         */ IL_1060: stloc.s   V_66
// 		/* 0x0008FF9E 3850010000   */ IL_1062: br        IL_11B7
// 		// loop start (head: IL_11B7)
// 			/* 0x0008FFA3 0E07         */ IL_1067: ldarg.s   astroPoses
// 			/* 0x0008FFA5 1142         */ IL_1069: ldloc.s   V_66
// 			/* 0x0008FFA7 8FE7010002   */ IL_106B: ldelema   AstroData
// 			/* 0x0008FFAC 7B43180004   */ IL_1070: ldfld     float32 AstroData::uRadius
// 			/* 0x0008FFB1 1343         */ IL_1075: stloc.s   V_67
// 			/* 0x0008FFB3 1143         */ IL_1077: ldloc.s   V_67
// 			/* 0x0008FFB5 220000803F   */ IL_1079: ldc.r4    1
// 			/* 0x0008FFBA 3F2E010000   */ IL_107E: blt       IL_11B1

// 			/* 0x0008FFBF 0E07         */ IL_1083: ldarg.s   astroPoses
// 			/* 0x0008FFC1 1142         */ IL_1085: ldloc.s   V_66
// 			/* 0x0008FFC3 8FE7010002   */ IL_1087: ldelema   AstroData
// 			/* 0x0008FFC8 7C46180004   */ IL_108C: ldflda    valuetype VectorLF3 AstroData::uPos
// 			/* 0x0008FFCD 1344         */ IL_1091: stloc.s   V_68
// 			/* 0x0008FFCF 1143         */ IL_1093: ldloc.s   V_67
// 			/* 0x0008FFD1 1141         */ IL_1095: ldloc.s   V_65
// 			/* 0x0008FFD3 58           */ IL_1097: add
// 			/* 0x0008FFD4 1345         */ IL_1098: stloc.s   V_69
// 			/* 0x0008FFD6 1240         */ IL_109A: ldloca.s  V_64
// 			/* 0x0008FFD8 1119         */ IL_109C: ldloc.s   V_25
// 			/* 0x0008FFDA 7C340C0004   */ IL_109E: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x0008FFDF 7B41030004   */ IL_10A3: ldfld     float64 VectorLF3::x
// 			/* 0x0008FFE4 1144         */ IL_10A8: ldloc.s   V_68
// 			/* 0x0008FFE6 7B41030004   */ IL_10AA: ldfld     float64 VectorLF3::x
// 			/* 0x0008FFEB 59           */ IL_10AF: sub
// 			/* 0x0008FFEC 7D41030004   */ IL_10B0: stfld     float64 VectorLF3::x
// 			/* 0x0008FFF1 1240         */ IL_10B5: ldloca.s  V_64
// 			/* 0x0008FFF3 1119         */ IL_10B7: ldloc.s   V_25
// 			/* 0x0008FFF5 7C340C0004   */ IL_10B9: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x0008FFFA 7B42030004   */ IL_10BE: ldfld     float64 VectorLF3::y
// 			/* 0x0008FFFF 1144         */ IL_10C3: ldloc.s   V_68
// 			/* 0x00090001 7B42030004   */ IL_10C5: ldfld     float64 VectorLF3::y
// 			/* 0x00090006 59           */ IL_10CA: sub
// 			/* 0x00090007 7D42030004   */ IL_10CB: stfld     float64 VectorLF3::y
// 			/* 0x0009000C 1240         */ IL_10D0: ldloca.s  V_64
// 			/* 0x0009000E 1119         */ IL_10D2: ldloc.s   V_25
// 			/* 0x00090010 7C340C0004   */ IL_10D4: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x00090015 7B43030004   */ IL_10D9: ldfld     float64 VectorLF3::z
// 			/* 0x0009001A 1144         */ IL_10DE: ldloc.s   V_68
// 			/* 0x0009001C 7B43030004   */ IL_10E0: ldfld     float64 VectorLF3::z
// 			/* 0x00090021 59           */ IL_10E5: sub
// 			/* 0x00090022 7D43030004   */ IL_10E6: stfld     float64 VectorLF3::z
// 			/* 0x00090027 1140         */ IL_10EB: ldloc.s   V_64
// 			/* 0x00090029 7B41030004   */ IL_10ED: ldfld     float64 VectorLF3::x
// 			/* 0x0009002E 1140         */ IL_10F2: ldloc.s   V_64
// 			/* 0x00090030 7B41030004   */ IL_10F4: ldfld     float64 VectorLF3::x
// 			/* 0x00090035 5A           */ IL_10F9: mul
// 			/* 0x00090036 1140         */ IL_10FA: ldloc.s   V_64
// 			/* 0x00090038 7B42030004   */ IL_10FC: ldfld     float64 VectorLF3::y
// 			/* 0x0009003D 1140         */ IL_1101: ldloc.s   V_64
// 			/* 0x0009003F 7B42030004   */ IL_1103: ldfld     float64 VectorLF3::y
// 			/* 0x00090044 5A           */ IL_1108: mul
// 			/* 0x00090045 58           */ IL_1109: add
// 			/* 0x00090046 1140         */ IL_110A: ldloc.s   V_64
// 			/* 0x00090048 7B43030004   */ IL_110C: ldfld     float64 VectorLF3::z
// 			/* 0x0009004D 1140         */ IL_1111: ldloc.s   V_64
// 			/* 0x0009004F 7B43030004   */ IL_1113: ldfld     float64 VectorLF3::z
// 			/* 0x00090054 5A           */ IL_1118: mul
// 			/* 0x00090055 58           */ IL_1119: add
// 			/* 0x00090056 1346         */ IL_111A: stloc.s   V_70
// 			/* 0x00090058 1119         */ IL_111C: ldloc.s   V_25
// 			/* 0x0009005A 7C350C0004   */ IL_111E: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x0009005F 7B4100000A   */ IL_1123: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 			/* 0x00090064 6C           */ IL_1128: conv.r8
// 			/* 0x00090065 1140         */ IL_1129: ldloc.s   V_64
// 			/* 0x00090067 7B41030004   */ IL_112B: ldfld     float64 VectorLF3::x
// 			/* 0x0009006C 5A           */ IL_1130: mul
// 			/* 0x0009006D 1119         */ IL_1131: ldloc.s   V_25
// 			/* 0x0009006F 7C350C0004   */ IL_1133: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x00090074 7B4200000A   */ IL_1138: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 			/* 0x00090079 6C           */ IL_113D: conv.r8
// 			/* 0x0009007A 1140         */ IL_113E: ldloc.s   V_64
// 			/* 0x0009007C 7B42030004   */ IL_1140: ldfld     float64 VectorLF3::y
// 			/* 0x00090081 5A           */ IL_1145: mul
// 			/* 0x00090082 58           */ IL_1146: add
// 			/* 0x00090083 1119         */ IL_1147: ldloc.s   V_25
// 			/* 0x00090085 7C350C0004   */ IL_1149: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x0009008A 7B8000000A   */ IL_114E: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 			/* 0x0009008F 6C           */ IL_1153: conv.r8
// 			/* 0x00090090 1140         */ IL_1154: ldloc.s   V_64
// 			/* 0x00090092 7B43030004   */ IL_1156: ldfld     float64 VectorLF3::z
// 			/* 0x00090097 5A           */ IL_115B: mul
// 			/* 0x00090098 58           */ IL_115C: add
// 			/* 0x00090099 65           */ IL_115D: neg
// 			/* 0x0009009A 1347         */ IL_115E: stloc.s   V_71
// 			/* 0x0009009C 1147         */ IL_1160: ldloc.s   V_71
// 			/* 0x0009009E 230000000000000000 */ IL_1162: ldc.r8    0.0
// 			/* 0x000900A7 3010         */ IL_116B: bgt.s     IL_117D

// 			/* 0x000900A9 1146         */ IL_116D: ldloc.s   V_70
// 			/* 0x000900AB 1143         */ IL_116F: ldloc.s   V_67
// 			/* 0x000900AD 1143         */ IL_1171: ldloc.s   V_67
// 			/* 0x000900AF 5A           */ IL_1173: mul
// 			/* 0x000900B0 220000E040   */ IL_1174: ldc.r4    7
// 			/* 0x000900B5 5A           */ IL_1179: mul
// 			/* 0x000900B6 6C           */ IL_117A: conv.r8
// 			/* 0x000900B7 3434         */ IL_117B: bge.un.s  IL_11B1

// 			/* 0x000900B9 1146         */ IL_117D: ldloc.s   V_70
// 			/* 0x000900BB 113B         */ IL_117F: ldloc.s   V_59
// 			/* 0x000900BD 342E         */ IL_1181: bge.un.s  IL_11B1

// 			/* 0x000900BF 1146         */ IL_1183: ldloc.s   V_70
// 			/* 0x000900C1 1145         */ IL_1185: ldloc.s   V_69
// 			/* 0x000900C3 1145         */ IL_1187: ldloc.s   V_69
// 			/* 0x000900C5 5A           */ IL_1189: mul
// 			/* 0x000900C6 6C           */ IL_118A: conv.r8
// 			/* 0x000900C7 3424         */ IL_118B: bge.un.s  IL_11B1

// 			/* 0x000900C9 1147         */ IL_118D: ldloc.s   V_71
// 			/* 0x000900CB 230000000000000000 */ IL_118F: ldc.r8    0.0
// 			/* 0x000900D4 3204         */ IL_1198: blt.s     IL_119E

// 			/* 0x000900D6 1147         */ IL_119A: ldloc.s   V_71
// 			/* 0x000900D8 2B09         */ IL_119C: br.s      IL_11A7

// 			/* 0x000900DA 230000000000000000 */ IL_119E: ldc.r8    0.0

// 			/* 0x000900E3 133A         */ IL_11A7: stloc.s   V_58
// 			/* 0x000900E5 1142         */ IL_11A9: ldloc.s   V_66
// 			/* 0x000900E7 1339         */ IL_11AB: stloc.s   V_57
// 			/* 0x000900E9 1146         */ IL_11AD: ldloc.s   V_70
// 			/* 0x000900EB 133B         */ IL_11AF: stloc.s   V_59

// 			/* 0x000900ED 1142         */ IL_11B1: ldloc.s   V_66
// 			/* 0x000900EF 17           */ IL_11B3: ldc.i4.1
// 			/* 0x000900F0 58           */ IL_11B4: add
// 			/* 0x000900F1 1342         */ IL_11B5: stloc.s   V_66

// 			/* 0x000900F3 1142         */ IL_11B7: ldloc.s   V_66
// 			/* 0x000900F5 113E         */ IL_11B9: ldloc.s   V_62
// 			/* 0x000900F7 1F0A         */ IL_11BB: ldc.i4.s  10
// 			/* 0x000900F9 58           */ IL_11BD: add
// 			/* 0x000900FA 3FA4FEFFFF   */ IL_11BE: blt       IL_1067
// 		// end loop

// 		/* 0x000900FF 113F         */ IL_11C3: ldloc.s   V_63
// 		/* 0x00090101 113E         */ IL_11C5: ldloc.s   V_62
// 		/* 0x00090103 3B65010000   */ IL_11C7: beq       IL_1331

// 		/* 0x00090108 113F         */ IL_11CC: ldloc.s   V_63
// 		/* 0x0009010A 1348         */ IL_11CE: stloc.s   V_72
// 		/* 0x0009010C 3850010000   */ IL_11D0: br        IL_1325
// 		// loop start (head: IL_1325)
// 			/* 0x00090111 0E07         */ IL_11D5: ldarg.s   astroPoses
// 			/* 0x00090113 1148         */ IL_11D7: ldloc.s   V_72
// 			/* 0x00090115 8FE7010002   */ IL_11D9: ldelema   AstroData
// 			/* 0x0009011A 7B43180004   */ IL_11DE: ldfld     float32 AstroData::uRadius
// 			/* 0x0009011F 1349         */ IL_11E3: stloc.s   V_73
// 			/* 0x00090121 1149         */ IL_11E5: ldloc.s   V_73
// 			/* 0x00090123 220000803F   */ IL_11E7: ldc.r4    1
// 			/* 0x00090128 3F2E010000   */ IL_11EC: blt       IL_131F

// 			/* 0x0009012D 0E07         */ IL_11F1: ldarg.s   astroPoses
// 			/* 0x0009012F 1148         */ IL_11F3: ldloc.s   V_72
// 			/* 0x00090131 8FE7010002   */ IL_11F5: ldelema   AstroData
// 			/* 0x00090136 7C46180004   */ IL_11FA: ldflda    valuetype VectorLF3 AstroData::uPos
// 			/* 0x0009013B 134A         */ IL_11FF: stloc.s   V_74
// 			/* 0x0009013D 1149         */ IL_1201: ldloc.s   V_73
// 			/* 0x0009013F 1141         */ IL_1203: ldloc.s   V_65
// 			/* 0x00090141 58           */ IL_1205: add
// 			/* 0x00090142 134B         */ IL_1206: stloc.s   V_75
// 			/* 0x00090144 1240         */ IL_1208: ldloca.s  V_64
// 			/* 0x00090146 1119         */ IL_120A: ldloc.s   V_25
// 			/* 0x00090148 7C340C0004   */ IL_120C: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x0009014D 7B41030004   */ IL_1211: ldfld     float64 VectorLF3::x
// 			/* 0x00090152 114A         */ IL_1216: ldloc.s   V_74
// 			/* 0x00090154 7B41030004   */ IL_1218: ldfld     float64 VectorLF3::x
// 			/* 0x00090159 59           */ IL_121D: sub
// 			/* 0x0009015A 7D41030004   */ IL_121E: stfld     float64 VectorLF3::x
// 			/* 0x0009015F 1240         */ IL_1223: ldloca.s  V_64
// 			/* 0x00090161 1119         */ IL_1225: ldloc.s   V_25
// 			/* 0x00090163 7C340C0004   */ IL_1227: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x00090168 7B42030004   */ IL_122C: ldfld     float64 VectorLF3::y
// 			/* 0x0009016D 114A         */ IL_1231: ldloc.s   V_74
// 			/* 0x0009016F 7B42030004   */ IL_1233: ldfld     float64 VectorLF3::y
// 			/* 0x00090174 59           */ IL_1238: sub
// 			/* 0x00090175 7D42030004   */ IL_1239: stfld     float64 VectorLF3::y
// 			/* 0x0009017A 1240         */ IL_123E: ldloca.s  V_64
// 			/* 0x0009017C 1119         */ IL_1240: ldloc.s   V_25
// 			/* 0x0009017E 7C340C0004   */ IL_1242: ldflda    valuetype VectorLF3 ShipData::uPos
// 			/* 0x00090183 7B43030004   */ IL_1247: ldfld     float64 VectorLF3::z
// 			/* 0x00090188 114A         */ IL_124C: ldloc.s   V_74
// 			/* 0x0009018A 7B43030004   */ IL_124E: ldfld     float64 VectorLF3::z
// 			/* 0x0009018F 59           */ IL_1253: sub
// 			/* 0x00090190 7D43030004   */ IL_1254: stfld     float64 VectorLF3::z
// 			/* 0x00090195 1140         */ IL_1259: ldloc.s   V_64
// 			/* 0x00090197 7B41030004   */ IL_125B: ldfld     float64 VectorLF3::x
// 			/* 0x0009019C 1140         */ IL_1260: ldloc.s   V_64
// 			/* 0x0009019E 7B41030004   */ IL_1262: ldfld     float64 VectorLF3::x
// 			/* 0x000901A3 5A           */ IL_1267: mul
// 			/* 0x000901A4 1140         */ IL_1268: ldloc.s   V_64
// 			/* 0x000901A6 7B42030004   */ IL_126A: ldfld     float64 VectorLF3::y
// 			/* 0x000901AB 1140         */ IL_126F: ldloc.s   V_64
// 			/* 0x000901AD 7B42030004   */ IL_1271: ldfld     float64 VectorLF3::y
// 			/* 0x000901B2 5A           */ IL_1276: mul
// 			/* 0x000901B3 58           */ IL_1277: add
// 			/* 0x000901B4 1140         */ IL_1278: ldloc.s   V_64
// 			/* 0x000901B6 7B43030004   */ IL_127A: ldfld     float64 VectorLF3::z
// 			/* 0x000901BB 1140         */ IL_127F: ldloc.s   V_64
// 			/* 0x000901BD 7B43030004   */ IL_1281: ldfld     float64 VectorLF3::z
// 			/* 0x000901C2 5A           */ IL_1286: mul
// 			/* 0x000901C3 58           */ IL_1287: add
// 			/* 0x000901C4 134C         */ IL_1288: stloc.s   V_76
// 			/* 0x000901C6 1119         */ IL_128A: ldloc.s   V_25
// 			/* 0x000901C8 7C350C0004   */ IL_128C: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x000901CD 7B4100000A   */ IL_1291: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 			/* 0x000901D2 6C           */ IL_1296: conv.r8
// 			/* 0x000901D3 1140         */ IL_1297: ldloc.s   V_64
// 			/* 0x000901D5 7B41030004   */ IL_1299: ldfld     float64 VectorLF3::x
// 			/* 0x000901DA 5A           */ IL_129E: mul
// 			/* 0x000901DB 1119         */ IL_129F: ldloc.s   V_25
// 			/* 0x000901DD 7C350C0004   */ IL_12A1: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x000901E2 7B4200000A   */ IL_12A6: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 			/* 0x000901E7 6C           */ IL_12AB: conv.r8
// 			/* 0x000901E8 1140         */ IL_12AC: ldloc.s   V_64
// 			/* 0x000901EA 7B42030004   */ IL_12AE: ldfld     float64 VectorLF3::y
// 			/* 0x000901EF 5A           */ IL_12B3: mul
// 			/* 0x000901F0 58           */ IL_12B4: add
// 			/* 0x000901F1 1119         */ IL_12B5: ldloc.s   V_25
// 			/* 0x000901F3 7C350C0004   */ IL_12B7: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 			/* 0x000901F8 7B8000000A   */ IL_12BC: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 			/* 0x000901FD 6C           */ IL_12C1: conv.r8
// 			/* 0x000901FE 1140         */ IL_12C2: ldloc.s   V_64
// 			/* 0x00090200 7B43030004   */ IL_12C4: ldfld     float64 VectorLF3::z
// 			/* 0x00090205 5A           */ IL_12C9: mul
// 			/* 0x00090206 58           */ IL_12CA: add
// 			/* 0x00090207 65           */ IL_12CB: neg
// 			/* 0x00090208 134D         */ IL_12CC: stloc.s   V_77
// 			/* 0x0009020A 114D         */ IL_12CE: ldloc.s   V_77
// 			/* 0x0009020C 230000000000000000 */ IL_12D0: ldc.r8    0.0
// 			/* 0x00090215 3010         */ IL_12D9: bgt.s     IL_12EB

// 			/* 0x00090217 114C         */ IL_12DB: ldloc.s   V_76
// 			/* 0x00090219 1149         */ IL_12DD: ldloc.s   V_73
// 			/* 0x0009021B 1149         */ IL_12DF: ldloc.s   V_73
// 			/* 0x0009021D 5A           */ IL_12E1: mul
// 			/* 0x0009021E 220000E040   */ IL_12E2: ldc.r4    7
// 			/* 0x00090223 5A           */ IL_12E7: mul
// 			/* 0x00090224 6C           */ IL_12E8: conv.r8
// 			/* 0x00090225 3434         */ IL_12E9: bge.un.s  IL_131F

// 			/* 0x00090227 114C         */ IL_12EB: ldloc.s   V_76
// 			/* 0x00090229 113B         */ IL_12ED: ldloc.s   V_59
// 			/* 0x0009022B 342E         */ IL_12EF: bge.un.s  IL_131F

// 			/* 0x0009022D 114C         */ IL_12F1: ldloc.s   V_76
// 			/* 0x0009022F 114B         */ IL_12F3: ldloc.s   V_75
// 			/* 0x00090231 114B         */ IL_12F5: ldloc.s   V_75
// 			/* 0x00090233 5A           */ IL_12F7: mul
// 			/* 0x00090234 6C           */ IL_12F8: conv.r8
// 			/* 0x00090235 3424         */ IL_12F9: bge.un.s  IL_131F

// 			/* 0x00090237 114D         */ IL_12FB: ldloc.s   V_77
// 			/* 0x00090239 230000000000000000 */ IL_12FD: ldc.r8    0.0
// 			/* 0x00090242 3204         */ IL_1306: blt.s     IL_130C

// 			/* 0x00090244 114D         */ IL_1308: ldloc.s   V_77
// 			/* 0x00090246 2B09         */ IL_130A: br.s      IL_1315

// 			/* 0x00090248 230000000000000000 */ IL_130C: ldc.r8    0.0

// 			/* 0x00090251 133A         */ IL_1315: stloc.s   V_58
// 			/* 0x00090253 1148         */ IL_1317: ldloc.s   V_72
// 			/* 0x00090255 1339         */ IL_1319: stloc.s   V_57
// 			/* 0x00090257 114C         */ IL_131B: ldloc.s   V_76
// 			/* 0x00090259 133B         */ IL_131D: stloc.s   V_59

// 			/* 0x0009025B 1148         */ IL_131F: ldloc.s   V_72
// 			/* 0x0009025D 17           */ IL_1321: ldc.i4.1
// 			/* 0x0009025E 58           */ IL_1322: add
// 			/* 0x0009025F 1348         */ IL_1323: stloc.s   V_72

// 			/* 0x00090261 1148         */ IL_1325: ldloc.s   V_72
// 			/* 0x00090263 113F         */ IL_1327: ldloc.s   V_63
// 			/* 0x00090265 1F0A         */ IL_1329: ldc.i4.s  10
// 			/* 0x00090267 58           */ IL_132B: add
// 			/* 0x00090268 3FA4FEFFFF   */ IL_132C: blt       IL_11D5
// 		// end loop

// 		/* 0x0009026D 2200000000   */ IL_1331: ldc.r4    0.0
// 		/* 0x00090272 2200000000   */ IL_1336: ldc.r4    0.0
// 		/* 0x00090277 2200000000   */ IL_133B: ldc.r4    0.0
// 		/* 0x0009027C 73B3020006   */ IL_1340: newobj    instance void VectorLF3::.ctor(float32, float32, float32)
// 		/* 0x00090281 28BD020006   */ IL_1345: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 		/* 0x00090286 134E         */ IL_134A: stloc.s   V_78
// 		/* 0x00090288 2200000000   */ IL_134C: ldc.r4    0.0
// 		/* 0x0009028D 2200000000   */ IL_1351: ldc.r4    0.0
// 		/* 0x00090292 2200000000   */ IL_1356: ldc.r4    0.0
// 		/* 0x00090297 73B3020006   */ IL_135B: newobj    instance void VectorLF3::.ctor(float32, float32, float32)
// 		/* 0x0009029C 28BD020006   */ IL_1360: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 		/* 0x000902A1 134F         */ IL_1365: stloc.s   V_79
// 		/* 0x000902A3 2200000000   */ IL_1367: ldc.r4    0.0
// 		/* 0x000902A8 1350         */ IL_136C: stloc.s   V_80
// 		/* 0x000902AA 1139         */ IL_136E: ldloc.s   V_57
// 		/* 0x000902AC 16           */ IL_1370: ldc.i4.0
// 		/* 0x000902AD 3EEF030000   */ IL_1371: ble       IL_1765

// 		/* 0x000902B2 0E07         */ IL_1376: ldarg.s   astroPoses
// 		/* 0x000902B4 1139         */ IL_1378: ldloc.s   V_57
// 		/* 0x000902B6 8FE7010002   */ IL_137A: ldelema   AstroData
// 		/* 0x000902BB 135E         */ IL_137F: stloc.s   V_94
// 		/* 0x000902BD 115E         */ IL_1381: ldloc.s   V_94
// 		/* 0x000902BF 7B43180004   */ IL_1383: ldfld     float32 AstroData::uRadius
// 		/* 0x000902C4 135F         */ IL_1388: stloc.s   V_95
// 		/* 0x000902C6 1139         */ IL_138A: ldloc.s   V_57
// 		/* 0x000902C8 1F64         */ IL_138C: ldc.i4.s  100
// 		/* 0x000902CA 5D           */ IL_138E: rem
// 		/* 0x000902CB 2D0A         */ IL_138F: brtrue.s  IL_139B

// 		/* 0x000902CD 115F         */ IL_1391: ldloc.s   V_95
// 		/* 0x000902CF 2200002040   */ IL_1393: ldc.r4    2.5
// 		/* 0x000902D4 5A           */ IL_1398: mul
// 		/* 0x000902D5 135F         */ IL_1399: stloc.s   V_95

// 		/* 0x000902D7 23000000000000F03F */ IL_139B: ldc.r8    1
// 		/* 0x000902E0 115E         */ IL_13A4: ldloc.s   V_94
// 		/* 0x000902E2 7B47180004   */ IL_13A6: ldfld     valuetype VectorLF3 AstroData::uPosNext
// 		/* 0x000902E7 115E         */ IL_13AB: ldloc.s   V_94
// 		/* 0x000902E9 7B46180004   */ IL_13AD: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x000902EE 28BA020006   */ IL_13B2: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x000902F3 136D         */ IL_13B7: stloc.s   V_109
// 		/* 0x000902F5 126D         */ IL_13B9: ldloca.s  V_109
// 		/* 0x000902F7 28BF020006   */ IL_13BB: call      instance float64 VectorLF3::get_magnitude()
// 		/* 0x000902FC 23000000000000E03F */ IL_13C0: ldc.r8    0.5
// 		/* 0x00090305 59           */ IL_13C9: sub
// 		/* 0x00090306 23333333333333E33F */ IL_13CA: ldc.r8    0.6
// 		/* 0x0009030F 5A           */ IL_13D3: mul
// 		/* 0x00090310 285503000A   */ IL_13D4: call      float64 [netstandard]System.Math::Max(float64, float64)
// 		/* 0x00090315 1360         */ IL_13D9: stloc.s   V_96
// 		/* 0x00090317 23000000000000F03F */ IL_13DB: ldc.r8    1
// 		/* 0x00090320 230000000000009940 */ IL_13E4: ldc.r8    1600
// 		/* 0x00090329 115F         */ IL_13ED: ldloc.s   V_95
// 		/* 0x0009032B 6C           */ IL_13EF: conv.r8
// 		/* 0x0009032C 5B           */ IL_13F0: div
// 		/* 0x0009032D 58           */ IL_13F1: add
// 		/* 0x0009032E 1361         */ IL_13F2: stloc.s   V_97
// 		/* 0x00090330 23000000000000F03F */ IL_13F4: ldc.r8    1
// 		/* 0x00090339 230000000000406F40 */ IL_13FD: ldc.r8    250
// 		/* 0x00090342 115F         */ IL_1406: ldloc.s   V_95
// 		/* 0x00090344 6C           */ IL_1408: conv.r8
// 		/* 0x00090345 5B           */ IL_1409: div
// 		/* 0x00090346 58           */ IL_140A: add
// 		/* 0x00090347 1362         */ IL_140B: stloc.s   V_98
// 		/* 0x00090349 1161         */ IL_140D: ldloc.s   V_97
// 		/* 0x0009034B 1160         */ IL_140F: ldloc.s   V_96
// 		/* 0x0009034D 1160         */ IL_1411: ldloc.s   V_96
// 		/* 0x0009034F 5A           */ IL_1413: mul
// 		/* 0x00090350 5A           */ IL_1414: mul
// 		/* 0x00090351 1361         */ IL_1415: stloc.s   V_97
// 		/* 0x00090353 1139         */ IL_1417: ldloc.s   V_57
// 		/* 0x00090355 1119         */ IL_1419: ldloc.s   V_25
// 		/* 0x00090357 7B320C0004   */ IL_141B: ldfld     int32 ShipData::planetA
// 		/* 0x0009035C 2E12         */ IL_1420: beq.s     IL_1434

// 		/* 0x0009035E 1139         */ IL_1422: ldloc.s   V_57
// 		/* 0x00090360 1119         */ IL_1424: ldloc.s   V_25
// 		/* 0x00090362 7B330C0004   */ IL_1426: ldfld     int32 ShipData::planetB
// 		/* 0x00090367 2E07         */ IL_142B: beq.s     IL_1434

// 		/* 0x00090369 220000C03F   */ IL_142D: ldc.r4    1.5
// 		/* 0x0009036E 2B05         */ IL_1432: br.s      IL_1439

// 		/* 0x00090370 220000A03F   */ IL_1434: ldc.r4    1.25

// 		/* 0x00090375 6C           */ IL_1439: conv.r8
// 		/* 0x00090376 1363         */ IL_143A: stloc.s   V_99
// 		/* 0x00090378 113B         */ IL_143C: ldloc.s   V_59
// 		/* 0x0009037A 284B02000A   */ IL_143E: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x0009037F 1364         */ IL_1443: stloc.s   V_100
// 		/* 0x00090381 115F         */ IL_1445: ldloc.s   V_95
// 		/* 0x00090383 6C           */ IL_1447: conv.r8
// 		/* 0x00090384 1164         */ IL_1448: ldloc.s   V_100
// 		/* 0x00090386 5B           */ IL_144A: div
// 		/* 0x00090387 239A9999999999F93F */ IL_144B: ldc.r8    1.6
// 		/* 0x00090390 5A           */ IL_1454: mul
// 		/* 0x00090391 239A9999999999B93F */ IL_1455: ldc.r8    0.1
// 		/* 0x0009039A 59           */ IL_145E: sub
// 		/* 0x0009039B 1365         */ IL_145F: stloc.s   V_101
// 		/* 0x0009039D 1165         */ IL_1461: ldloc.s   V_101
// 		/* 0x0009039F 23000000000000F03F */ IL_1463: ldc.r8    1
// 		/* 0x000903A8 360D         */ IL_146C: ble.un.s  IL_147B

// 		/* 0x000903AA 23000000000000F03F */ IL_146E: ldc.r8    1
// 		/* 0x000903B3 1365         */ IL_1477: stloc.s   V_101
// 		/* 0x000903B5 2B18         */ IL_1479: br.s      IL_1493

// 		/* 0x000903B7 1165         */ IL_147B: ldloc.s   V_101
// 		/* 0x000903B9 230000000000000000 */ IL_147D: ldc.r8    0.0
// 		/* 0x000903C2 340B         */ IL_1486: bge.un.s  IL_1493

// 		/* 0x000903C4 230000000000000000 */ IL_1488: ldc.r8    0.0
// 		/* 0x000903CD 1365         */ IL_1491: stloc.s   V_101

// 		/* 0x000903CF 1164         */ IL_1493: ldloc.s   V_100
// 		/* 0x000903D1 115F         */ IL_1495: ldloc.s   V_95
// 		/* 0x000903D3 6C           */ IL_1497: conv.r8
// 		/* 0x000903D4 233D0AD7A3703DEA3F */ IL_1498: ldc.r8    0.82
// 		/* 0x000903DD 5A           */ IL_14A1: mul
// 		/* 0x000903DE 59           */ IL_14A2: sub
// 		/* 0x000903DF 1366         */ IL_14A3: stloc.s   V_102
// 		/* 0x000903E1 1166         */ IL_14A5: ldloc.s   V_102
// 		/* 0x000903E3 23000000000000F03F */ IL_14A7: ldc.r8    1
// 		/* 0x000903EC 340B         */ IL_14B0: bge.un.s  IL_14BD

// 		/* 0x000903EE 23000000000000F03F */ IL_14B2: ldc.r8    1
// 		/* 0x000903F7 1366         */ IL_14BB: stloc.s   V_102

// 		/* 0x000903F9 1138         */ IL_14BD: ldloc.s   V_56
// 		/* 0x000903FB 220000C040   */ IL_14BF: ldc.r4    6
// 		/* 0x00090400 59           */ IL_14C4: sub
// 		/* 0x00090401 6C           */ IL_14C5: conv.r8
// 		/* 0x00090402 1166         */ IL_14C6: ldloc.s   V_102
// 		/* 0x00090404 110A         */ IL_14C8: ldloc.s   V_10
// 		/* 0x00090406 6C           */ IL_14CA: conv.r8
// 		/* 0x00090407 5A           */ IL_14CB: mul
// 		/* 0x00090408 5B           */ IL_14CC: div
// 		/* 0x00090409 23333333333333E33F */ IL_14CD: ldc.r8    0.6
// 		/* 0x00090412 5A           */ IL_14D6: mul
// 		/* 0x00090413 237B14AE47E17A843F */ IL_14D7: ldc.r8    0.01
// 		/* 0x0009041C 59           */ IL_14E0: sub
// 		/* 0x0009041D 1367         */ IL_14E1: stloc.s   V_103
// 		/* 0x0009041F 1167         */ IL_14E3: ldloc.s   V_103
// 		/* 0x00090421 23000000000000F83F */ IL_14E5: ldc.r8    1.5
// 		/* 0x0009042A 360D         */ IL_14EE: ble.un.s  IL_14FD

// 		/* 0x0009042C 23000000000000F83F */ IL_14F0: ldc.r8    1.5
// 		/* 0x00090435 1367         */ IL_14F9: stloc.s   V_103
// 		/* 0x00090437 2B18         */ IL_14FB: br.s      IL_1515

// 		/* 0x00090439 1167         */ IL_14FD: ldloc.s   V_103
// 		/* 0x0009043B 230000000000000000 */ IL_14FF: ldc.r8    0.0
// 		/* 0x00090444 340B         */ IL_1508: bge.un.s  IL_1515

// 		/* 0x00090446 230000000000000000 */ IL_150A: ldc.r8    0.0
// 		/* 0x0009044F 1367         */ IL_1513: stloc.s   V_103

// 		/* 0x00090451 1119         */ IL_1515: ldloc.s   V_25
// 		/* 0x00090453 7B340C0004   */ IL_1517: ldfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090458 1119         */ IL_151C: ldloc.s   V_25
// 		/* 0x0009045A 7B350C0004   */ IL_151E: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0009045F 28BC020006   */ IL_1523: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090464 113A         */ IL_1528: ldloc.s   V_58
// 		/* 0x00090466 28B7020006   */ IL_152A: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x0009046B 28BB020006   */ IL_152F: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00090470 115E         */ IL_1534: ldloc.s   V_94
// 		/* 0x00090472 7B46180004   */ IL_1536: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00090477 28BA020006   */ IL_153B: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0009047C 1368         */ IL_1540: stloc.s   V_104
// 		/* 0x0009047E 1268         */ IL_1542: ldloca.s  V_104
// 		/* 0x00090480 28BF020006   */ IL_1544: call      instance float64 VectorLF3::get_magnitude()
// 		/* 0x00090485 115F         */ IL_1549: ldloc.s   V_95
// 		/* 0x00090487 6C           */ IL_154B: conv.r8
// 		/* 0x00090488 5B           */ IL_154C: div
// 		/* 0x00090489 1369         */ IL_154D: stloc.s   V_105
// 		/* 0x0009048B 1169         */ IL_154F: ldloc.s   V_105
// 		/* 0x0009048D 1163         */ IL_1551: ldloc.s   V_99
// 		/* 0x0009048F 3478         */ IL_1553: bge.un.s  IL_15CD

// 		/* 0x00090491 1169         */ IL_1555: ldloc.s   V_105
// 		/* 0x00090493 23000000000000F03F */ IL_1557: ldc.r8    1
// 		/* 0x0009049C 59           */ IL_1560: sub
// 		/* 0x0009049D 1163         */ IL_1561: ldloc.s   V_99
// 		/* 0x0009049F 23000000000000F03F */ IL_1563: ldc.r8    1
// 		/* 0x000904A8 59           */ IL_156C: sub
// 		/* 0x000904A9 5B           */ IL_156D: div
// 		/* 0x000904AA 136E         */ IL_156E: stloc.s   V_110
// 		/* 0x000904AC 116E         */ IL_1570: ldloc.s   V_110
// 		/* 0x000904AE 230000000000000000 */ IL_1572: ldc.r8    0.0
// 		/* 0x000904B7 340B         */ IL_157B: bge.un.s  IL_1588

// 		/* 0x000904B9 230000000000000000 */ IL_157D: ldc.r8    0.0
// 		/* 0x000904C2 136E         */ IL_1586: stloc.s   V_110

// 		/* 0x000904C4 23000000000000F03F */ IL_1588: ldc.r8    1
// 		/* 0x000904CD 116E         */ IL_1591: ldloc.s   V_110
// 		/* 0x000904CF 116E         */ IL_1593: ldloc.s   V_110
// 		/* 0x000904D1 5A           */ IL_1595: mul
// 		/* 0x000904D2 59           */ IL_1596: sub
// 		/* 0x000904D3 136E         */ IL_1597: stloc.s   V_110
// 		/* 0x000904D5 1268         */ IL_1599: ldloca.s  V_104
// 		/* 0x000904D7 28C9020006   */ IL_159B: call      instance valuetype VectorLF3 VectorLF3::get_normalized()
// 		/* 0x000904DC 1167         */ IL_15A0: ldloc.s   V_103
// 		/* 0x000904DE 1167         */ IL_15A2: ldloc.s   V_103
// 		/* 0x000904E0 5A           */ IL_15A4: mul
// 		/* 0x000904E1 116E         */ IL_15A5: ldloc.s   V_110
// 		/* 0x000904E3 5A           */ IL_15A7: mul
// 		/* 0x000904E4 230000000000000040 */ IL_15A8: ldc.r8    2
// 		/* 0x000904ED 5A           */ IL_15B1: mul
// 		/* 0x000904EE 220000803F   */ IL_15B2: ldc.r4    1
// 		/* 0x000904F3 1119         */ IL_15B7: ldloc.s   V_25
// 		/* 0x000904F5 7B370C0004   */ IL_15B9: ldfld     float32 ShipData::warpState
// 		/* 0x000904FA 59           */ IL_15BE: sub
// 		/* 0x000904FB 6C           */ IL_15BF: conv.r8
// 		/* 0x000904FC 5A           */ IL_15C0: mul
// 		/* 0x000904FD 28B7020006   */ IL_15C1: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x00090502 28BD020006   */ IL_15C6: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 		/* 0x00090507 134F         */ IL_15CB: stloc.s   V_79

// 		/* 0x00090509 126A         */ IL_15CD: ldloca.s  V_106
// 		/* 0x0009050B 1119         */ IL_15CF: ldloc.s   V_25
// 		/* 0x0009050D 7C340C0004   */ IL_15D1: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090512 7B41030004   */ IL_15D6: ldfld     float64 VectorLF3::x
// 		/* 0x00090517 115E         */ IL_15DB: ldloc.s   V_94
// 		/* 0x00090519 7C46180004   */ IL_15DD: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0009051E 7B41030004   */ IL_15E2: ldfld     float64 VectorLF3::x
// 		/* 0x00090523 59           */ IL_15E7: sub
// 		/* 0x00090524 7D41030004   */ IL_15E8: stfld     float64 VectorLF3::x
// 		/* 0x00090529 126A         */ IL_15ED: ldloca.s  V_106
// 		/* 0x0009052B 1119         */ IL_15EF: ldloc.s   V_25
// 		/* 0x0009052D 7C340C0004   */ IL_15F1: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090532 7B42030004   */ IL_15F6: ldfld     float64 VectorLF3::y
// 		/* 0x00090537 115E         */ IL_15FB: ldloc.s   V_94
// 		/* 0x00090539 7C46180004   */ IL_15FD: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0009053E 7B42030004   */ IL_1602: ldfld     float64 VectorLF3::y
// 		/* 0x00090543 59           */ IL_1607: sub
// 		/* 0x00090544 7D42030004   */ IL_1608: stfld     float64 VectorLF3::y
// 		/* 0x00090549 126A         */ IL_160D: ldloca.s  V_106
// 		/* 0x0009054B 1119         */ IL_160F: ldloc.s   V_25
// 		/* 0x0009054D 7C340C0004   */ IL_1611: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090552 7B43030004   */ IL_1616: ldfld     float64 VectorLF3::z
// 		/* 0x00090557 115E         */ IL_161B: ldloc.s   V_94
// 		/* 0x00090559 7C46180004   */ IL_161D: ldflda    valuetype VectorLF3 AstroData::uPos
// 		/* 0x0009055E 7B43030004   */ IL_1622: ldfld     float64 VectorLF3::z
// 		/* 0x00090563 59           */ IL_1627: sub
// 		/* 0x00090564 7D43030004   */ IL_1628: stfld     float64 VectorLF3::z
// 		/* 0x00090569 1165         */ IL_162D: ldloc.s   V_101
// 		/* 0x0009056B 1164         */ IL_162F: ldloc.s   V_100
// 		/* 0x0009056D 5B           */ IL_1631: div
// 		/* 0x0009056E 136B         */ IL_1632: stloc.s   V_107
// 		/* 0x00090570 124E         */ IL_1634: ldloca.s  V_78
// 		/* 0x00090572 116A         */ IL_1636: ldloc.s   V_106
// 		/* 0x00090574 7B41030004   */ IL_1638: ldfld     float64 VectorLF3::x
// 		/* 0x00090579 116B         */ IL_163D: ldloc.s   V_107
// 		/* 0x0009057B 5A           */ IL_163F: mul
// 		/* 0x0009057C 6B           */ IL_1640: conv.r4
// 		/* 0x0009057D 7D4100000A   */ IL_1641: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090582 124E         */ IL_1646: ldloca.s  V_78
// 		/* 0x00090584 116A         */ IL_1648: ldloc.s   V_106
// 		/* 0x00090586 7B42030004   */ IL_164A: ldfld     float64 VectorLF3::y
// 		/* 0x0009058B 116B         */ IL_164F: ldloc.s   V_107
// 		/* 0x0009058D 5A           */ IL_1651: mul
// 		/* 0x0009058E 6B           */ IL_1652: conv.r4
// 		/* 0x0009058F 7D4200000A   */ IL_1653: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090594 124E         */ IL_1658: ldloca.s  V_78
// 		/* 0x00090596 116A         */ IL_165A: ldloc.s   V_106
// 		/* 0x00090598 7B43030004   */ IL_165C: ldfld     float64 VectorLF3::z
// 		/* 0x0009059D 116B         */ IL_1661: ldloc.s   V_107
// 		/* 0x0009059F 5A           */ IL_1663: mul
// 		/* 0x000905A0 6B           */ IL_1664: conv.r4
// 		/* 0x000905A1 7D8000000A   */ IL_1665: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000905A6 1165         */ IL_166A: ldloc.s   V_101
// 		/* 0x000905A8 6B           */ IL_166C: conv.r4
// 		/* 0x000905A9 1350         */ IL_166D: stloc.s   V_80
// 		/* 0x000905AB 1164         */ IL_166F: ldloc.s   V_100
// 		/* 0x000905AD 115F         */ IL_1671: ldloc.s   V_95
// 		/* 0x000905AF 6C           */ IL_1673: conv.r8
// 		/* 0x000905B0 5B           */ IL_1674: div
// 		/* 0x000905B1 136C         */ IL_1675: stloc.s   V_108
// 		/* 0x000905B3 116C         */ IL_1677: ldloc.s   V_108
// 		/* 0x000905B5 116C         */ IL_1679: ldloc.s   V_108
// 		/* 0x000905B7 5A           */ IL_167B: mul
// 		/* 0x000905B8 136C         */ IL_167C: stloc.s   V_108
// 		/* 0x000905BA 1161         */ IL_167E: ldloc.s   V_97
// 		/* 0x000905BC 116C         */ IL_1680: ldloc.s   V_108
// 		/* 0x000905BE 59           */ IL_1682: sub
// 		/* 0x000905BF 1161         */ IL_1683: ldloc.s   V_97
// 		/* 0x000905C1 1162         */ IL_1685: ldloc.s   V_98
// 		/* 0x000905C3 59           */ IL_1687: sub
// 		/* 0x000905C4 5B           */ IL_1688: div
// 		/* 0x000905C5 136C         */ IL_1689: stloc.s   V_108
// 		/* 0x000905C7 116C         */ IL_168B: ldloc.s   V_108
// 		/* 0x000905C9 23000000000000F03F */ IL_168D: ldc.r8    1
// 		/* 0x000905D2 360D         */ IL_1696: ble.un.s  IL_16A5

// 		/* 0x000905D4 23000000000000F03F */ IL_1698: ldc.r8    1
// 		/* 0x000905DD 136C         */ IL_16A1: stloc.s   V_108
// 		/* 0x000905DF 2B18         */ IL_16A3: br.s      IL_16BD

// 		/* 0x000905E1 116C         */ IL_16A5: ldloc.s   V_108
// 		/* 0x000905E3 230000000000000000 */ IL_16A7: ldc.r8    0.0
// 		/* 0x000905EC 340B         */ IL_16B0: bge.un.s  IL_16BD

// 		/* 0x000905EE 230000000000000000 */ IL_16B2: ldc.r8    0.0
// 		/* 0x000905F7 136C         */ IL_16BB: stloc.s   V_108

// 		/* 0x000905F9 116C         */ IL_16BD: ldloc.s   V_108
// 		/* 0x000905FB 230000000000000000 */ IL_16BF: ldc.r8    0.0
// 		/* 0x00090604 4398000000   */ IL_16C8: ble.un    IL_1765

// 		/* 0x00090609 115E         */ IL_16CD: ldloc.s   V_94
// 		/* 0x0009060B 7C44180004   */ IL_16CF: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090610 126A         */ IL_16D4: ldloca.s  V_106
// 		/* 0x00090612 126F         */ IL_16D6: ldloca.s  V_111
// 		/* 0x00090614 2813040006   */ IL_16D8: call      void Maths::QInvRotateLF_refout(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype VectorLF3&)
// 		/* 0x00090619 115E         */ IL_16DD: ldloc.s   V_94
// 		/* 0x0009061B 7C47180004   */ IL_16DF: ldflda    valuetype VectorLF3 AstroData::uPosNext
// 		/* 0x00090620 115E         */ IL_16E4: ldloc.s   V_94
// 		/* 0x00090622 7C45180004   */ IL_16E6: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRotNext
// 		/* 0x00090627 126F         */ IL_16EB: ldloca.s  V_111
// 		/* 0x00090629 1270         */ IL_16ED: ldloca.s  V_112
// 		/* 0x0009062B 28150A0006   */ IL_16EF: call      void StationComponent::lpos2upos_out(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype VectorLF3&)
// 		/* 0x00090630 230000000000000840 */ IL_16F4: ldc.r8    3
// 		/* 0x00090639 116C         */ IL_16FD: ldloc.s   V_108
// 		/* 0x0009063B 59           */ IL_16FF: sub
// 		/* 0x0009063C 116C         */ IL_1700: ldloc.s   V_108
// 		/* 0x0009063E 59           */ IL_1702: sub
// 		/* 0x0009063F 116C         */ IL_1703: ldloc.s   V_108
// 		/* 0x00090641 5A           */ IL_1705: mul
// 		/* 0x00090642 116C         */ IL_1706: ldloc.s   V_108
// 		/* 0x00090644 5A           */ IL_1708: mul
// 		/* 0x00090645 136C         */ IL_1709: stloc.s   V_108
// 		/* 0x00090647 122E         */ IL_170B: ldloca.s  V_46
// 		/* 0x00090649 1170         */ IL_170D: ldloc.s   V_112
// 		/* 0x0009064B 7B41030004   */ IL_170F: ldfld     float64 VectorLF3::x
// 		/* 0x00090650 1119         */ IL_1714: ldloc.s   V_25
// 		/* 0x00090652 7C340C0004   */ IL_1716: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090657 7B41030004   */ IL_171B: ldfld     float64 VectorLF3::x
// 		/* 0x0009065C 59           */ IL_1720: sub
// 		/* 0x0009065D 116C         */ IL_1721: ldloc.s   V_108
// 		/* 0x0009065F 5A           */ IL_1723: mul
// 		/* 0x00090660 7D41030004   */ IL_1724: stfld     float64 VectorLF3::x
// 		/* 0x00090665 122E         */ IL_1729: ldloca.s  V_46
// 		/* 0x00090667 1170         */ IL_172B: ldloc.s   V_112
// 		/* 0x00090669 7B42030004   */ IL_172D: ldfld     float64 VectorLF3::y
// 		/* 0x0009066E 1119         */ IL_1732: ldloc.s   V_25
// 		/* 0x00090670 7C340C0004   */ IL_1734: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090675 7B42030004   */ IL_1739: ldfld     float64 VectorLF3::y
// 		/* 0x0009067A 59           */ IL_173E: sub
// 		/* 0x0009067B 116C         */ IL_173F: ldloc.s   V_108
// 		/* 0x0009067D 5A           */ IL_1741: mul
// 		/* 0x0009067E 7D42030004   */ IL_1742: stfld     float64 VectorLF3::y
// 		/* 0x00090683 122E         */ IL_1747: ldloca.s  V_46
// 		/* 0x00090685 1170         */ IL_1749: ldloc.s   V_112
// 		/* 0x00090687 7B43030004   */ IL_174B: ldfld     float64 VectorLF3::z
// 		/* 0x0009068C 1119         */ IL_1750: ldloc.s   V_25
// 		/* 0x0009068E 7C340C0004   */ IL_1752: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090693 7B43030004   */ IL_1757: ldfld     float64 VectorLF3::z
// 		/* 0x00090698 59           */ IL_175C: sub
// 		/* 0x00090699 116C         */ IL_175D: ldloc.s   V_108
// 		/* 0x0009069B 5A           */ IL_175F: mul
// 		/* 0x0009069C 7D43030004   */ IL_1760: stfld     float64 VectorLF3::z

// 		/* 0x000906A1 1119         */ IL_1765: ldloc.s   V_25
// 		/* 0x000906A3 7B380C0004   */ IL_1767: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x000906A8 1119         */ IL_176C: ldloc.s   V_25
// 		/* 0x000906AA 7C350C0004   */ IL_176E: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000906AF 1251         */ IL_1773: ldloca.s  V_81
// 		/* 0x000906B1 28F3030006   */ IL_1775: call      void Maths::ForwardUp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&)
// 		/* 0x000906B6 220000803F   */ IL_177A: ldc.r4    1
// 		/* 0x000906BB 1150         */ IL_177F: ldloc.s   V_80
// 		/* 0x000906BD 59           */ IL_1781: sub
// 		/* 0x000906BE 1353         */ IL_1782: stloc.s   V_83
// 		/* 0x000906C0 1252         */ IL_1784: ldloca.s  V_82
// 		/* 0x000906C2 1151         */ IL_1786: ldloc.s   V_81
// 		/* 0x000906C4 7B4100000A   */ IL_1788: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000906C9 1153         */ IL_178D: ldloc.s   V_83
// 		/* 0x000906CB 5A           */ IL_178F: mul
// 		/* 0x000906CC 114E         */ IL_1790: ldloc.s   V_78
// 		/* 0x000906CE 7B4100000A   */ IL_1792: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000906D3 1150         */ IL_1797: ldloc.s   V_80
// 		/* 0x000906D5 5A           */ IL_1799: mul
// 		/* 0x000906D6 58           */ IL_179A: add
// 		/* 0x000906D7 7D4100000A   */ IL_179B: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000906DC 1252         */ IL_17A0: ldloca.s  V_82
// 		/* 0x000906DE 1151         */ IL_17A2: ldloc.s   V_81
// 		/* 0x000906E0 7B4200000A   */ IL_17A4: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000906E5 1153         */ IL_17A9: ldloc.s   V_83
// 		/* 0x000906E7 5A           */ IL_17AB: mul
// 		/* 0x000906E8 114E         */ IL_17AC: ldloc.s   V_78
// 		/* 0x000906EA 7B4200000A   */ IL_17AE: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000906EF 1150         */ IL_17B3: ldloc.s   V_80
// 		/* 0x000906F1 5A           */ IL_17B5: mul
// 		/* 0x000906F2 58           */ IL_17B6: add
// 		/* 0x000906F3 7D4200000A   */ IL_17B7: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000906F8 1252         */ IL_17BC: ldloca.s  V_82
// 		/* 0x000906FA 1151         */ IL_17BE: ldloc.s   V_81
// 		/* 0x000906FC 7B8000000A   */ IL_17C0: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090701 1153         */ IL_17C5: ldloc.s   V_83
// 		/* 0x00090703 5A           */ IL_17C7: mul
// 		/* 0x00090704 114E         */ IL_17C8: ldloc.s   V_78
// 		/* 0x00090706 7B8000000A   */ IL_17CA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009070B 1150         */ IL_17CF: ldloc.s   V_80
// 		/* 0x0009070D 5A           */ IL_17D1: mul
// 		/* 0x0009070E 58           */ IL_17D2: add
// 		/* 0x0009070F 7D8000000A   */ IL_17D3: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090714 1152         */ IL_17D8: ldloc.s   V_82
// 		/* 0x00090716 7B4100000A   */ IL_17DA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009071B 1119         */ IL_17DF: ldloc.s   V_25
// 		/* 0x0009071D 7C350C0004   */ IL_17E1: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090722 7B4100000A   */ IL_17E6: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090727 5A           */ IL_17EB: mul
// 		/* 0x00090728 1152         */ IL_17EC: ldloc.s   V_82
// 		/* 0x0009072A 7B4200000A   */ IL_17EE: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009072F 1119         */ IL_17F3: ldloc.s   V_25
// 		/* 0x00090731 7C350C0004   */ IL_17F5: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090736 7B4200000A   */ IL_17FA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009073B 5A           */ IL_17FF: mul
// 		/* 0x0009073C 58           */ IL_1800: add
// 		/* 0x0009073D 1152         */ IL_1801: ldloc.s   V_82
// 		/* 0x0009073F 7B8000000A   */ IL_1803: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090744 1119         */ IL_1808: ldloc.s   V_25
// 		/* 0x00090746 7C350C0004   */ IL_180A: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x0009074B 7B8000000A   */ IL_180F: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090750 5A           */ IL_1814: mul
// 		/* 0x00090751 58           */ IL_1815: add
// 		/* 0x00090752 1354         */ IL_1816: stloc.s   V_84
// 		/* 0x00090754 1252         */ IL_1818: ldloca.s  V_82
// 		/* 0x00090756 1152         */ IL_181A: ldloc.s   V_82
// 		/* 0x00090758 7B4100000A   */ IL_181C: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009075D 1154         */ IL_1821: ldloc.s   V_84
// 		/* 0x0009075F 1119         */ IL_1823: ldloc.s   V_25
// 		/* 0x00090761 7C350C0004   */ IL_1825: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090766 7B4100000A   */ IL_182A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009076B 5A           */ IL_182F: mul
// 		/* 0x0009076C 59           */ IL_1830: sub
// 		/* 0x0009076D 7D4100000A   */ IL_1831: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090772 1252         */ IL_1836: ldloca.s  V_82
// 		/* 0x00090774 1152         */ IL_1838: ldloc.s   V_82
// 		/* 0x00090776 7B4200000A   */ IL_183A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009077B 1154         */ IL_183F: ldloc.s   V_84
// 		/* 0x0009077D 1119         */ IL_1841: ldloc.s   V_25
// 		/* 0x0009077F 7C350C0004   */ IL_1843: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090784 7B4200000A   */ IL_1848: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090789 5A           */ IL_184D: mul
// 		/* 0x0009078A 59           */ IL_184E: sub
// 		/* 0x0009078B 7D4200000A   */ IL_184F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090790 1252         */ IL_1854: ldloca.s  V_82
// 		/* 0x00090792 1152         */ IL_1856: ldloc.s   V_82
// 		/* 0x00090794 7B8000000A   */ IL_1858: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090799 1154         */ IL_185D: ldloc.s   V_84
// 		/* 0x0009079B 1119         */ IL_185F: ldloc.s   V_25
// 		/* 0x0009079D 7C350C0004   */ IL_1861: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000907A2 7B8000000A   */ IL_1866: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000907A7 5A           */ IL_186B: mul
// 		/* 0x000907A8 59           */ IL_186C: sub
// 		/* 0x000907A9 7D8000000A   */ IL_186D: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000907AE 1152         */ IL_1872: ldloc.s   V_82
// 		/* 0x000907B0 7B4100000A   */ IL_1874: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000907B5 1152         */ IL_1879: ldloc.s   V_82
// 		/* 0x000907B7 7B4100000A   */ IL_187B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000907BC 5A           */ IL_1880: mul
// 		/* 0x000907BD 1152         */ IL_1881: ldloc.s   V_82
// 		/* 0x000907BF 7B4200000A   */ IL_1883: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000907C4 1152         */ IL_1888: ldloc.s   V_82
// 		/* 0x000907C6 7B4200000A   */ IL_188A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000907CB 5A           */ IL_188F: mul
// 		/* 0x000907CC 58           */ IL_1890: add
// 		/* 0x000907CD 1152         */ IL_1891: ldloc.s   V_82
// 		/* 0x000907CF 7B8000000A   */ IL_1893: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000907D4 1152         */ IL_1898: ldloc.s   V_82
// 		/* 0x000907D6 7B8000000A   */ IL_189A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000907DB 5A           */ IL_189F: mul
// 		/* 0x000907DC 58           */ IL_18A0: add
// 		/* 0x000907DD 6C           */ IL_18A1: conv.r8
// 		/* 0x000907DE 284B02000A   */ IL_18A2: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x000907E3 6B           */ IL_18A7: conv.r4
// 		/* 0x000907E4 1355         */ IL_18A8: stloc.s   V_85
// 		/* 0x000907E6 1155         */ IL_18AA: ldloc.s   V_85
// 		/* 0x000907E8 2200000000   */ IL_18AC: ldc.r4    0.0
// 		/* 0x000907ED 3633         */ IL_18B1: ble.un.s  IL_18E6

// 		/* 0x000907EF 1252         */ IL_18B3: ldloca.s  V_82
// 		/* 0x000907F1 1152         */ IL_18B5: ldloc.s   V_82
// 		/* 0x000907F3 7B4100000A   */ IL_18B7: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000907F8 1155         */ IL_18BC: ldloc.s   V_85
// 		/* 0x000907FA 5B           */ IL_18BE: div
// 		/* 0x000907FB 7D4100000A   */ IL_18BF: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090800 1252         */ IL_18C4: ldloca.s  V_82
// 		/* 0x00090802 1152         */ IL_18C6: ldloc.s   V_82
// 		/* 0x00090804 7B4200000A   */ IL_18C8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090809 1155         */ IL_18CD: ldloc.s   V_85
// 		/* 0x0009080B 5B           */ IL_18CF: div
// 		/* 0x0009080C 7D4200000A   */ IL_18D0: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090811 1252         */ IL_18D5: ldloca.s  V_82
// 		/* 0x00090813 1152         */ IL_18D7: ldloc.s   V_82
// 		/* 0x00090815 7B8000000A   */ IL_18D9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009081A 1155         */ IL_18DE: ldloc.s   V_85
// 		/* 0x0009081C 5B           */ IL_18E0: div
// 		/* 0x0009081D 7D8000000A   */ IL_18E1: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z

// 		/* 0x00090822 1256         */ IL_18E6: ldloca.s  V_86
// 		/* 0x00090824 114F         */ IL_18E8: ldloc.s   V_79
// 		/* 0x00090826 7B4100000A   */ IL_18EA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009082B 7D4100000A   */ IL_18EF: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090830 1256         */ IL_18F4: ldloca.s  V_86
// 		/* 0x00090832 114F         */ IL_18F6: ldloc.s   V_79
// 		/* 0x00090834 7B4200000A   */ IL_18F8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090839 7D4200000A   */ IL_18FD: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009083E 1256         */ IL_1902: ldloca.s  V_86
// 		/* 0x00090840 114F         */ IL_1904: ldloc.s   V_79
// 		/* 0x00090842 7B8000000A   */ IL_1906: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090847 7D8000000A   */ IL_190B: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009084C 1127         */ IL_1910: ldloc.s   V_39
// 		/* 0x0009084E 230000000000000000 */ IL_1912: ldc.r8    0.0
// 		/* 0x00090857 3642         */ IL_191B: ble.un.s  IL_195F

// 		/* 0x00090859 1256         */ IL_191D: ldloca.s  V_86
// 		/* 0x0009085B 7C4100000A   */ IL_191F: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090860 25           */ IL_1924: dup
// 		/* 0x00090861 4E           */ IL_1925: ldind.r4
// 		/* 0x00090862 1126         */ IL_1926: ldloc.s   V_38
// 		/* 0x00090864 7B41030004   */ IL_1928: ldfld     float64 VectorLF3::x
// 		/* 0x00090869 1127         */ IL_192D: ldloc.s   V_39
// 		/* 0x0009086B 5B           */ IL_192F: div
// 		/* 0x0009086C 6B           */ IL_1930: conv.r4
// 		/* 0x0009086D 58           */ IL_1931: add
// 		/* 0x0009086E 56           */ IL_1932: stind.r4
// 		/* 0x0009086F 1256         */ IL_1933: ldloca.s  V_86
// 		/* 0x00090871 7C4200000A   */ IL_1935: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090876 25           */ IL_193A: dup
// 		/* 0x00090877 4E           */ IL_193B: ldind.r4
// 		/* 0x00090878 1126         */ IL_193C: ldloc.s   V_38
// 		/* 0x0009087A 7B42030004   */ IL_193E: ldfld     float64 VectorLF3::y
// 		/* 0x0009087F 1127         */ IL_1943: ldloc.s   V_39
// 		/* 0x00090881 5B           */ IL_1945: div
// 		/* 0x00090882 6B           */ IL_1946: conv.r4
// 		/* 0x00090883 58           */ IL_1947: add
// 		/* 0x00090884 56           */ IL_1948: stind.r4
// 		/* 0x00090885 1256         */ IL_1949: ldloca.s  V_86
// 		/* 0x00090887 7C8000000A   */ IL_194B: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009088C 25           */ IL_1950: dup
// 		/* 0x0009088D 4E           */ IL_1951: ldind.r4
// 		/* 0x0009088E 1126         */ IL_1952: ldloc.s   V_38
// 		/* 0x00090890 7B43030004   */ IL_1954: ldfld     float64 VectorLF3::z
// 		/* 0x00090895 1127         */ IL_1959: ldloc.s   V_39
// 		/* 0x00090897 5B           */ IL_195B: div
// 		/* 0x00090898 6B           */ IL_195C: conv.r4
// 		/* 0x00090899 58           */ IL_195D: add
// 		/* 0x0009089A 56           */ IL_195E: stind.r4

// 		/* 0x0009089B 1119         */ IL_195F: ldloc.s   V_25
// 		/* 0x0009089D 7C350C0004   */ IL_1961: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000908A2 1256         */ IL_1966: ldloca.s  V_86
// 		/* 0x000908A4 1257         */ IL_1968: ldloca.s  V_87
// 		/* 0x000908A6 28160A0006   */ IL_196A: call      void StationComponent::Vector3Cross_ref(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&)
// 		/* 0x000908AB 1119         */ IL_196F: ldloc.s   V_25
// 		/* 0x000908AD 7C350C0004   */ IL_1971: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000908B2 7B4100000A   */ IL_1976: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000908B7 1156         */ IL_197B: ldloc.s   V_86
// 		/* 0x000908B9 7B4100000A   */ IL_197D: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000908BE 5A           */ IL_1982: mul
// 		/* 0x000908BF 1119         */ IL_1983: ldloc.s   V_25
// 		/* 0x000908C1 7C350C0004   */ IL_1985: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000908C6 7B4200000A   */ IL_198A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000908CB 1156         */ IL_198F: ldloc.s   V_86
// 		/* 0x000908CD 7B4200000A   */ IL_1991: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000908D2 5A           */ IL_1996: mul
// 		/* 0x000908D3 58           */ IL_1997: add
// 		/* 0x000908D4 1119         */ IL_1998: ldloc.s   V_25
// 		/* 0x000908D6 7C350C0004   */ IL_199A: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000908DB 7B8000000A   */ IL_199F: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000908E0 1156         */ IL_19A4: ldloc.s   V_86
// 		/* 0x000908E2 7B8000000A   */ IL_19A6: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000908E7 5A           */ IL_19AB: mul
// 		/* 0x000908E8 58           */ IL_19AC: add
// 		/* 0x000908E9 1251         */ IL_19AD: ldloca.s  V_81
// 		/* 0x000908EB 1252         */ IL_19AF: ldloca.s  V_82
// 		/* 0x000908ED 1258         */ IL_19B1: ldloca.s  V_88
// 		/* 0x000908EF 28160A0006   */ IL_19B3: call      void StationComponent::Vector3Cross_ref(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&)
// 		/* 0x000908F4 1151         */ IL_19B8: ldloc.s   V_81
// 		/* 0x000908F6 7B4100000A   */ IL_19BA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000908FB 1152         */ IL_19BF: ldloc.s   V_82
// 		/* 0x000908FD 7B4100000A   */ IL_19C1: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090902 5A           */ IL_19C6: mul
// 		/* 0x00090903 1151         */ IL_19C7: ldloc.s   V_81
// 		/* 0x00090905 7B4200000A   */ IL_19C9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009090A 1152         */ IL_19CE: ldloc.s   V_82
// 		/* 0x0009090C 7B4200000A   */ IL_19D0: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090911 5A           */ IL_19D5: mul
// 		/* 0x00090912 58           */ IL_19D6: add
// 		/* 0x00090913 1151         */ IL_19D7: ldloc.s   V_81
// 		/* 0x00090915 7B8000000A   */ IL_19D9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009091A 1152         */ IL_19DE: ldloc.s   V_82
// 		/* 0x0009091C 7B8000000A   */ IL_19E0: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090921 5A           */ IL_19E5: mul
// 		/* 0x00090922 58           */ IL_19E6: add
// 		/* 0x00090923 1359         */ IL_19E7: stloc.s   V_89
// 		/* 0x00090925 2200000000   */ IL_19E9: ldc.r4    0.0
// 		/* 0x0009092A 3474         */ IL_19EE: bge.un.s  IL_1A64

// 		/* 0x0009092C 1157         */ IL_19F0: ldloc.s   V_87
// 		/* 0x0009092E 7B4100000A   */ IL_19F2: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090933 1157         */ IL_19F7: ldloc.s   V_87
// 		/* 0x00090935 7B4100000A   */ IL_19F9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009093A 5A           */ IL_19FE: mul
// 		/* 0x0009093B 1157         */ IL_19FF: ldloc.s   V_87
// 		/* 0x0009093D 7B4200000A   */ IL_1A01: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090942 1157         */ IL_1A06: ldloc.s   V_87
// 		/* 0x00090944 7B4200000A   */ IL_1A08: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090949 5A           */ IL_1A0D: mul
// 		/* 0x0009094A 58           */ IL_1A0E: add
// 		/* 0x0009094B 1157         */ IL_1A0F: ldloc.s   V_87
// 		/* 0x0009094D 7B8000000A   */ IL_1A11: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090952 1157         */ IL_1A16: ldloc.s   V_87
// 		/* 0x00090954 7B8000000A   */ IL_1A18: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090959 5A           */ IL_1A1D: mul
// 		/* 0x0009095A 58           */ IL_1A1E: add
// 		/* 0x0009095B 6C           */ IL_1A1F: conv.r8
// 		/* 0x0009095C 284B02000A   */ IL_1A20: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x00090961 6B           */ IL_1A25: conv.r4
// 		/* 0x00090962 1371         */ IL_1A26: stloc.s   V_113
// 		/* 0x00090964 1171         */ IL_1A28: ldloc.s   V_113
// 		/* 0x00090966 2200000000   */ IL_1A2A: ldc.r4    0.0
// 		/* 0x0009096B 3633         */ IL_1A2F: ble.un.s  IL_1A64

// 		/* 0x0009096D 1257         */ IL_1A31: ldloca.s  V_87
// 		/* 0x0009096F 1157         */ IL_1A33: ldloc.s   V_87
// 		/* 0x00090971 7B4100000A   */ IL_1A35: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090976 1171         */ IL_1A3A: ldloc.s   V_113
// 		/* 0x00090978 5B           */ IL_1A3C: div
// 		/* 0x00090979 7D4100000A   */ IL_1A3D: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009097E 1257         */ IL_1A42: ldloca.s  V_87
// 		/* 0x00090980 1157         */ IL_1A44: ldloc.s   V_87
// 		/* 0x00090982 7B4200000A   */ IL_1A46: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090987 1171         */ IL_1A4B: ldloc.s   V_113
// 		/* 0x00090989 5B           */ IL_1A4D: div
// 		/* 0x0009098A 7D4200000A   */ IL_1A4E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009098F 1257         */ IL_1A53: ldloca.s  V_87
// 		/* 0x00090991 1157         */ IL_1A55: ldloc.s   V_87
// 		/* 0x00090993 7B8000000A   */ IL_1A57: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090998 1171         */ IL_1A5C: ldloc.s   V_113
// 		/* 0x0009099A 5B           */ IL_1A5E: div
// 		/* 0x0009099B 7D8000000A   */ IL_1A5F: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z

// 		/* 0x000909A0 1159         */ IL_1A64: ldloc.s   V_89
// 		/* 0x000909A2 2200000000   */ IL_1A66: ldc.r4    0.0
// 		/* 0x000909A7 3474         */ IL_1A6B: bge.un.s  IL_1AE1

// 		/* 0x000909A9 1158         */ IL_1A6D: ldloc.s   V_88
// 		/* 0x000909AB 7B4100000A   */ IL_1A6F: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000909B0 1158         */ IL_1A74: ldloc.s   V_88
// 		/* 0x000909B2 7B4100000A   */ IL_1A76: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000909B7 5A           */ IL_1A7B: mul
// 		/* 0x000909B8 1158         */ IL_1A7C: ldloc.s   V_88
// 		/* 0x000909BA 7B4200000A   */ IL_1A7E: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000909BF 1158         */ IL_1A83: ldloc.s   V_88
// 		/* 0x000909C1 7B4200000A   */ IL_1A85: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000909C6 5A           */ IL_1A8A: mul
// 		/* 0x000909C7 58           */ IL_1A8B: add
// 		/* 0x000909C8 1158         */ IL_1A8C: ldloc.s   V_88
// 		/* 0x000909CA 7B8000000A   */ IL_1A8E: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000909CF 1158         */ IL_1A93: ldloc.s   V_88
// 		/* 0x000909D1 7B8000000A   */ IL_1A95: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x000909D6 5A           */ IL_1A9A: mul
// 		/* 0x000909D7 58           */ IL_1A9B: add
// 		/* 0x000909D8 6C           */ IL_1A9C: conv.r8
// 		/* 0x000909D9 284B02000A   */ IL_1A9D: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x000909DE 6B           */ IL_1AA2: conv.r4
// 		/* 0x000909DF 1372         */ IL_1AA3: stloc.s   V_114
// 		/* 0x000909E1 1172         */ IL_1AA5: ldloc.s   V_114
// 		/* 0x000909E3 2200000000   */ IL_1AA7: ldc.r4    0.0
// 		/* 0x000909E8 3633         */ IL_1AAC: ble.un.s  IL_1AE1

// 		/* 0x000909EA 1258         */ IL_1AAE: ldloca.s  V_88
// 		/* 0x000909EC 1158         */ IL_1AB0: ldloc.s   V_88
// 		/* 0x000909EE 7B4100000A   */ IL_1AB2: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000909F3 1172         */ IL_1AB7: ldloc.s   V_114
// 		/* 0x000909F5 5B           */ IL_1AB9: div
// 		/* 0x000909F6 7D4100000A   */ IL_1ABA: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000909FB 1258         */ IL_1ABF: ldloca.s  V_88
// 		/* 0x000909FD 1158         */ IL_1AC1: ldloc.s   V_88
// 		/* 0x000909FF 7B4200000A   */ IL_1AC3: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090A04 1172         */ IL_1AC8: ldloc.s   V_114
// 		/* 0x00090A06 5B           */ IL_1ACA: div
// 		/* 0x00090A07 7D4200000A   */ IL_1ACB: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090A0C 1258         */ IL_1AD0: ldloca.s  V_88
// 		/* 0x00090A0E 1158         */ IL_1AD2: ldloc.s   V_88
// 		/* 0x00090A10 7B8000000A   */ IL_1AD4: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090A15 1172         */ IL_1AD9: ldloc.s   V_114
// 		/* 0x00090A17 5B           */ IL_1ADB: div
// 		/* 0x00090A18 7D8000000A   */ IL_1ADC: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z

// 		/* 0x00090A1D 1137         */ IL_1AE1: ldloc.s   V_55
// 		/* 0x00090A1F 230000000000000840 */ IL_1AE3: ldc.r8    3
// 		/* 0x00090A28 3217         */ IL_1AEC: blt.s     IL_1B05

// 		/* 0x00090A2A 1138         */ IL_1AEE: ldloc.s   V_56
// 		/* 0x00090A2C 05           */ IL_1AF0: ldarg.3
// 		/* 0x00090A2D 5B           */ IL_1AF1: div
// 		/* 0x00090A2E 112A         */ IL_1AF2: ldloc.s   V_42
// 		/* 0x00090A30 2D07         */ IL_1AF4: brtrue.s  IL_1AFD

// 		/* 0x00090A32 220000803F   */ IL_1AF6: ldc.r4    1
// 		/* 0x00090A37 2B05         */ IL_1AFB: br.s      IL_1B02

// 		/* 0x00090A39 22CDCC4C3E   */ IL_1AFD: ldc.r4    0.2

// 		/* 0x00090A3E 5A           */ IL_1B02: mul
// 		/* 0x00090A3F 2B0F         */ IL_1B03: br.s      IL_1B14

// 		/* 0x00090A41 2200005040   */ IL_1B05: ldc.r4    3.25
// 		/* 0x00090A46 1137         */ IL_1B0A: ldloc.s   V_55
// 		/* 0x00090A48 6B           */ IL_1B0C: conv.r4
// 		/* 0x00090A49 59           */ IL_1B0D: sub
// 		/* 0x00090A4A 2200008040   */ IL_1B0E: ldc.r4    4
// 		/* 0x00090A4F 5A           */ IL_1B13: mul

// 		/* 0x00090A50 135A         */ IL_1B14: stloc.s   V_90
// 		/* 0x00090A52 1157         */ IL_1B16: ldloc.s   V_87
// 		/* 0x00090A54 115A         */ IL_1B18: ldloc.s   V_90
// 		/* 0x00090A56 289800000A   */ IL_1B1A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00090A5B 1158         */ IL_1B1F: ldloc.s   V_88
// 		/* 0x00090A5D 2200000040   */ IL_1B21: ldc.r4    2
// 		/* 0x00090A62 289800000A   */ IL_1B26: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00090A67 289900000A   */ IL_1B2B: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090A6C 1357         */ IL_1B30: stloc.s   V_87
// 		/* 0x00090A6E 125B         */ IL_1B32: ldloca.s  V_91
// 		/* 0x00090A70 1157         */ IL_1B34: ldloc.s   V_87
// 		/* 0x00090A72 7B4100000A   */ IL_1B36: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090A77 1119         */ IL_1B3B: ldloc.s   V_25
// 		/* 0x00090A79 7C390C0004   */ IL_1B3D: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090A7E 7B4100000A   */ IL_1B42: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090A83 59           */ IL_1B47: sub
// 		/* 0x00090A84 7D4100000A   */ IL_1B48: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090A89 125B         */ IL_1B4D: ldloca.s  V_91
// 		/* 0x00090A8B 1157         */ IL_1B4F: ldloc.s   V_87
// 		/* 0x00090A8D 7B4200000A   */ IL_1B51: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090A92 1119         */ IL_1B56: ldloc.s   V_25
// 		/* 0x00090A94 7C390C0004   */ IL_1B58: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090A99 7B4200000A   */ IL_1B5D: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090A9E 59           */ IL_1B62: sub
// 		/* 0x00090A9F 7D4200000A   */ IL_1B63: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090AA4 125B         */ IL_1B68: ldloca.s  V_91
// 		/* 0x00090AA6 1157         */ IL_1B6A: ldloc.s   V_87
// 		/* 0x00090AA8 7B8000000A   */ IL_1B6C: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090AAD 1119         */ IL_1B71: ldloc.s   V_25
// 		/* 0x00090AAF 7C390C0004   */ IL_1B73: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090AB4 7B8000000A   */ IL_1B78: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090AB9 59           */ IL_1B7D: sub
// 		/* 0x00090ABA 7D8000000A   */ IL_1B7E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090ABF 115B         */ IL_1B83: ldloc.s   V_91
// 		/* 0x00090AC1 7B4100000A   */ IL_1B85: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090AC6 115B         */ IL_1B8A: ldloc.s   V_91
// 		/* 0x00090AC8 7B4100000A   */ IL_1B8C: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090ACD 5A           */ IL_1B91: mul
// 		/* 0x00090ACE 115B         */ IL_1B92: ldloc.s   V_91
// 		/* 0x00090AD0 7B4200000A   */ IL_1B94: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090AD5 115B         */ IL_1B99: ldloc.s   V_91
// 		/* 0x00090AD7 7B4200000A   */ IL_1B9B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090ADC 5A           */ IL_1BA0: mul
// 		/* 0x00090ADD 58           */ IL_1BA1: add
// 		/* 0x00090ADE 115B         */ IL_1BA2: ldloc.s   V_91
// 		/* 0x00090AE0 7B8000000A   */ IL_1BA4: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090AE5 115B         */ IL_1BA9: ldloc.s   V_91
// 		/* 0x00090AE7 7B8000000A   */ IL_1BAB: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090AEC 5A           */ IL_1BB0: mul
// 		/* 0x00090AED 58           */ IL_1BB1: add
// 		/* 0x00090AEE 22CDCCCC3D   */ IL_1BB2: ldc.r4    0.1
// 		/* 0x00090AF3 320A         */ IL_1BB7: blt.s     IL_1BC3

// 		/* 0x00090AF5 22CDCC4C3D   */ IL_1BB9: ldc.r4    0.05
// 		/* 0x00090AFA 110A         */ IL_1BBE: ldloc.s   V_10
// 		/* 0x00090AFC 5A           */ IL_1BC0: mul
// 		/* 0x00090AFD 2B05         */ IL_1BC1: br.s      IL_1BC8

// 		/* 0x00090AFF 220000803F   */ IL_1BC3: ldc.r4    1

// 		/* 0x00090B04 135C         */ IL_1BC8: stloc.s   V_92
// 		/* 0x00090B06 115C         */ IL_1BCA: ldloc.s   V_92
// 		/* 0x00090B08 220000803F   */ IL_1BCC: ldc.r4    1
// 		/* 0x00090B0D 3607         */ IL_1BD1: ble.un.s  IL_1BDA

// 		/* 0x00090B0F 220000803F   */ IL_1BD3: ldc.r4    1
// 		/* 0x00090B14 135C         */ IL_1BD8: stloc.s   V_92

// 		/* 0x00090B16 1119         */ IL_1BDA: ldloc.s   V_25
// 		/* 0x00090B18 7C390C0004   */ IL_1BDC: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B1D 1119         */ IL_1BE1: ldloc.s   V_25
// 		/* 0x00090B1F 7C390C0004   */ IL_1BE3: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B24 7B4100000A   */ IL_1BE8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090B29 115B         */ IL_1BED: ldloc.s   V_91
// 		/* 0x00090B2B 7B4100000A   */ IL_1BEF: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090B30 115C         */ IL_1BF4: ldloc.s   V_92
// 		/* 0x00090B32 5A           */ IL_1BF6: mul
// 		/* 0x00090B33 58           */ IL_1BF7: add
// 		/* 0x00090B34 7D4100000A   */ IL_1BF8: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090B39 1119         */ IL_1BFD: ldloc.s   V_25
// 		/* 0x00090B3B 7C390C0004   */ IL_1BFF: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B40 1119         */ IL_1C04: ldloc.s   V_25
// 		/* 0x00090B42 7C390C0004   */ IL_1C06: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B47 7B4200000A   */ IL_1C0B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090B4C 115B         */ IL_1C10: ldloc.s   V_91
// 		/* 0x00090B4E 7B4200000A   */ IL_1C12: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090B53 115C         */ IL_1C17: ldloc.s   V_92
// 		/* 0x00090B55 5A           */ IL_1C19: mul
// 		/* 0x00090B56 58           */ IL_1C1A: add
// 		/* 0x00090B57 7D4200000A   */ IL_1C1B: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090B5C 1119         */ IL_1C20: ldloc.s   V_25
// 		/* 0x00090B5E 7C390C0004   */ IL_1C22: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B63 1119         */ IL_1C27: ldloc.s   V_25
// 		/* 0x00090B65 7C390C0004   */ IL_1C29: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B6A 7B8000000A   */ IL_1C2E: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090B6F 115B         */ IL_1C33: ldloc.s   V_91
// 		/* 0x00090B71 7B8000000A   */ IL_1C35: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090B76 115C         */ IL_1C3A: ldloc.s   V_92
// 		/* 0x00090B78 5A           */ IL_1C3C: mul
// 		/* 0x00090B79 58           */ IL_1C3D: add
// 		/* 0x00090B7A 7D8000000A   */ IL_1C3E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090B7F 1119         */ IL_1C43: ldloc.s   V_25
// 		/* 0x00090B81 7C390C0004   */ IL_1C45: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B86 7B4100000A   */ IL_1C4A: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090B8B 1119         */ IL_1C4F: ldloc.s   V_25
// 		/* 0x00090B8D 7C390C0004   */ IL_1C51: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B92 7B4100000A   */ IL_1C56: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090B97 5A           */ IL_1C5B: mul
// 		/* 0x00090B98 1119         */ IL_1C5C: ldloc.s   V_25
// 		/* 0x00090B9A 7C390C0004   */ IL_1C5E: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090B9F 7B4200000A   */ IL_1C63: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090BA4 1119         */ IL_1C68: ldloc.s   V_25
// 		/* 0x00090BA6 7C390C0004   */ IL_1C6A: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090BAB 7B4200000A   */ IL_1C6F: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090BB0 5A           */ IL_1C74: mul
// 		/* 0x00090BB1 58           */ IL_1C75: add
// 		/* 0x00090BB2 1119         */ IL_1C76: ldloc.s   V_25
// 		/* 0x00090BB4 7C390C0004   */ IL_1C78: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090BB9 7B8000000A   */ IL_1C7D: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090BBE 1119         */ IL_1C82: ldloc.s   V_25
// 		/* 0x00090BC0 7C390C0004   */ IL_1C84: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090BC5 7B8000000A   */ IL_1C89: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090BCA 5A           */ IL_1C8E: mul
// 		/* 0x00090BCB 58           */ IL_1C8F: add
// 		/* 0x00090BCC 6C           */ IL_1C90: conv.r8
// 		/* 0x00090BCD 284B02000A   */ IL_1C91: call      float64 [netstandard]System.Math::Sqrt(float64)
// 		/* 0x00090BD2 6B           */ IL_1C96: conv.r4
// 		/* 0x00090BD3 135D         */ IL_1C97: stloc.s   V_93
// 		/* 0x00090BD5 115D         */ IL_1C99: ldloc.s   V_93
// 		/* 0x00090BD7 2200000000   */ IL_1C9B: ldc.r4    0.0
// 		/* 0x00090BDC 367A         */ IL_1CA0: ble.un.s  IL_1D1C

// 		/* 0x00090BDE 115D         */ IL_1CA2: ldloc.s   V_93
// 		/* 0x00090BE0 6C           */ IL_1CA4: conv.r8
// 		/* 0x00090BE1 23111111111111913F */ IL_1CA5: ldc.r8    0.016666666666666666
// 		/* 0x00090BEA 5A           */ IL_1CAE: mul
// 		/* 0x00090BEB 112C         */ IL_1CAF: ldloc.s   V_44
// 		/* 0x00090BED 6C           */ IL_1CB1: conv.r8
// 		/* 0x00090BEE 5A           */ IL_1CB2: mul
// 		/* 0x00090BEF 23000000000000E03F */ IL_1CB3: ldc.r8    0.5
// 		/* 0x00090BF8 5A           */ IL_1CBC: mul
// 		/* 0x00090BF9 25           */ IL_1CBD: dup
// 		/* 0x00090BFA 286B02000A   */ IL_1CBE: call      float64 [netstandard]System.Math::Cos(float64)
// 		/* 0x00090BFF 6B           */ IL_1CC3: conv.r4
// 		/* 0x00090C00 1373         */ IL_1CC4: stloc.s   V_115
// 		/* 0x00090C02 286A02000A   */ IL_1CC6: call      float64 [netstandard]System.Math::Sin(float64)
// 		/* 0x00090C07 6B           */ IL_1CCB: conv.r4
// 		/* 0x00090C08 115D         */ IL_1CCC: ldloc.s   V_93
// 		/* 0x00090C0A 5B           */ IL_1CCE: div
// 		/* 0x00090C0B 1374         */ IL_1CCF: stloc.s   V_116
// 		/* 0x00090C0D 1275         */ IL_1CD1: ldloca.s  V_117
// 		/* 0x00090C0F 1119         */ IL_1CD3: ldloc.s   V_25
// 		/* 0x00090C11 7C390C0004   */ IL_1CD5: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090C16 7B4100000A   */ IL_1CDA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090C1B 1174         */ IL_1CDF: ldloc.s   V_116
// 		/* 0x00090C1D 5A           */ IL_1CE1: mul
// 		/* 0x00090C1E 1119         */ IL_1CE2: ldloc.s   V_25
// 		/* 0x00090C20 7C390C0004   */ IL_1CE4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090C25 7B4200000A   */ IL_1CE9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090C2A 1174         */ IL_1CEE: ldloc.s   V_116
// 		/* 0x00090C2C 5A           */ IL_1CF0: mul
// 		/* 0x00090C2D 1119         */ IL_1CF1: ldloc.s   V_25
// 		/* 0x00090C2F 7C390C0004   */ IL_1CF3: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090C34 7B8000000A   */ IL_1CF8: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090C39 1174         */ IL_1CFD: ldloc.s   V_116
// 		/* 0x00090C3B 5A           */ IL_1CFF: mul
// 		/* 0x00090C3C 1173         */ IL_1D00: ldloc.s   V_115
// 		/* 0x00090C3E 285702000A   */ IL_1D02: call      instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x00090C43 1119         */ IL_1D07: ldloc.s   V_25
// 		/* 0x00090C45 1175         */ IL_1D09: ldloc.s   V_117
// 		/* 0x00090C47 1119         */ IL_1D0B: ldloc.s   V_25
// 		/* 0x00090C49 7B380C0004   */ IL_1D0D: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090C4E 281A01000A   */ IL_1D12: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090C53 7D380C0004   */ IL_1D17: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot

// 		/* 0x00090C58 1119         */ IL_1D1C: ldloc.s   V_25
// 		/* 0x00090C5A 7B370C0004   */ IL_1D1E: ldfld     float32 ShipData::warpState
// 		/* 0x00090C5F 2200000000   */ IL_1D23: ldc.r4    0.0
// 		/* 0x00090C64 3656         */ IL_1D28: ble.un.s  IL_1D80

// 		/* 0x00090C66 1119         */ IL_1D2A: ldloc.s   V_25
// 		/* 0x00090C68 7B370C0004   */ IL_1D2C: ldfld     float32 ShipData::warpState
// 		/* 0x00090C6D 1119         */ IL_1D31: ldloc.s   V_25
// 		/* 0x00090C6F 7B370C0004   */ IL_1D33: ldfld     float32 ShipData::warpState
// 		/* 0x00090C74 5A           */ IL_1D38: mul
// 		/* 0x00090C75 1119         */ IL_1D39: ldloc.s   V_25
// 		/* 0x00090C77 7B370C0004   */ IL_1D3B: ldfld     float32 ShipData::warpState
// 		/* 0x00090C7C 5A           */ IL_1D40: mul
// 		/* 0x00090C7D 1376         */ IL_1D41: stloc.s   V_118
// 		/* 0x00090C7F 1119         */ IL_1D43: ldloc.s   V_25
// 		/* 0x00090C81 1119         */ IL_1D45: ldloc.s   V_25
// 		/* 0x00090C83 7B380C0004   */ IL_1D47: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090C88 1156         */ IL_1D4C: ldloc.s   V_86
// 		/* 0x00090C8A 1152         */ IL_1D4E: ldloc.s   V_82
// 		/* 0x00090C8C 283A01000A   */ IL_1D50: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::LookRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090C91 1176         */ IL_1D55: ldloc.s   V_118
// 		/* 0x00090C93 28DE00000A   */ IL_1D57: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 		/* 0x00090C98 7D380C0004   */ IL_1D5C: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090C9D 1119         */ IL_1D61: ldloc.s   V_25
// 		/* 0x00090C9F 7C390C0004   */ IL_1D63: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00090CA4 25           */ IL_1D68: dup
// 		/* 0x00090CA5 7110000001   */ IL_1D69: ldobj     [UnityEngine.CoreModule]UnityEngine.Vector3
// 		/* 0x00090CAA 220000803F   */ IL_1D6E: ldc.r4    1
// 		/* 0x00090CAF 1176         */ IL_1D73: ldloc.s   V_118
// 		/* 0x00090CB1 59           */ IL_1D75: sub
// 		/* 0x00090CB2 289800000A   */ IL_1D76: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00090CB7 8110000001   */ IL_1D7B: stobj     [UnityEngine.CoreModule]UnityEngine.Vector3

// 		/* 0x00090CBC 1127         */ IL_1D80: ldloc.s   V_39
// 		/* 0x00090CBE 230000000000005940 */ IL_1D82: ldc.r8    100
// 		/* 0x00090CC7 41EB000000   */ IL_1D8B: bge.un    IL_1E7B

// 		/* 0x00090CCC 220000803F   */ IL_1D90: ldc.r4    1
// 		/* 0x00090CD1 1127         */ IL_1D95: ldloc.s   V_39
// 		/* 0x00090CD3 6B           */ IL_1D97: conv.r4
// 		/* 0x00090CD4 220000C842   */ IL_1D98: ldc.r4    100
// 		/* 0x00090CD9 5B           */ IL_1D9D: div
// 		/* 0x00090CDA 59           */ IL_1D9E: sub
// 		/* 0x00090CDB 1377         */ IL_1D9F: stloc.s   V_119
// 		/* 0x00090CDD 2200004040   */ IL_1DA1: ldc.r4    3
// 		/* 0x00090CE2 1177         */ IL_1DA6: ldloc.s   V_119
// 		/* 0x00090CE4 59           */ IL_1DA8: sub
// 		/* 0x00090CE5 1177         */ IL_1DA9: ldloc.s   V_119
// 		/* 0x00090CE7 59           */ IL_1DAB: sub
// 		/* 0x00090CE8 1177         */ IL_1DAC: ldloc.s   V_119
// 		/* 0x00090CEA 5A           */ IL_1DAE: mul
// 		/* 0x00090CEB 1177         */ IL_1DAF: ldloc.s   V_119
// 		/* 0x00090CED 5A           */ IL_1DB1: mul
// 		/* 0x00090CEE 1377         */ IL_1DB2: stloc.s   V_119
// 		/* 0x00090CF0 1177         */ IL_1DB4: ldloc.s   V_119
// 		/* 0x00090CF2 1177         */ IL_1DB6: ldloc.s   V_119
// 		/* 0x00090CF4 5A           */ IL_1DB8: mul
// 		/* 0x00090CF5 1377         */ IL_1DB9: stloc.s   V_119
// 		/* 0x00090CF7 1119         */ IL_1DBB: ldloc.s   V_25
// 		/* 0x00090CF9 7B3E0C0004   */ IL_1DBD: ldfld     int32 ShipData::direction
// 		/* 0x00090CFE 16           */ IL_1DC2: ldc.i4.0
// 		/* 0x00090CFF 314B         */ IL_1DC3: ble.s     IL_1E10

// 		/* 0x00090D01 1119         */ IL_1DC5: ldloc.s   V_25
// 		/* 0x00090D03 7B380C0004   */ IL_1DC7: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090D08 111C         */ IL_1DCC: ldloc.s   V_28
// 		/* 0x00090D0A 7B44180004   */ IL_1DCE: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090D0F 0E06         */ IL_1DD3: ldarg.s   gStationPool
// 		/* 0x00090D11 1119         */ IL_1DD5: ldloc.s   V_25
// 		/* 0x00090D13 7B3D0C0004   */ IL_1DD7: ldfld     int32 ShipData::otherGId
// 		/* 0x00090D18 9A           */ IL_1DDC: ldelem.ref
// 		/* 0x00090D19 7BCA0B0004   */ IL_1DDD: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion StationComponent::shipDockRot
// 		/* 0x00090D1E 22F304353F   */ IL_1DE2: ldc.r4    0.70710677
// 		/* 0x00090D23 2200000000   */ IL_1DE7: ldc.r4    0.0
// 		/* 0x00090D28 2200000000   */ IL_1DEC: ldc.r4    0.0
// 		/* 0x00090D2D 22F30435BF   */ IL_1DF1: ldc.r4    -0.70710677
// 		/* 0x00090D32 735702000A   */ IL_1DF6: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x00090D37 281A01000A   */ IL_1DFB: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090D3C 281A01000A   */ IL_1E00: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090D41 1177         */ IL_1E05: ldloc.s   V_119
// 		/* 0x00090D43 28DE00000A   */ IL_1E07: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 		/* 0x00090D48 1313         */ IL_1E0C: stloc.s   V_19
// 		/* 0x00090D4A 2B68         */ IL_1E0E: br.s      IL_1E78

// 		/* 0x00090D4C 1119         */ IL_1E10: ldloc.s   V_25
// 		/* 0x00090D4E 7B340C0004   */ IL_1E12: ldfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090D53 110B         */ IL_1E17: ldloc.s   V_11
// 		/* 0x00090D55 7B46180004   */ IL_1E19: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00090D5A 28BA020006   */ IL_1E1E: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00090D5F 136D         */ IL_1E23: stloc.s   V_109
// 		/* 0x00090D61 126D         */ IL_1E25: ldloca.s  V_109
// 		/* 0x00090D63 28C9020006   */ IL_1E27: call      instance valuetype VectorLF3 VectorLF3::get_normalized()
// 		/* 0x00090D68 28BD020006   */ IL_1E2C: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 VectorLF3::op_Implicit(valuetype VectorLF3)
// 		/* 0x00090D6D 1378         */ IL_1E31: stloc.s   V_120
// 		/* 0x00090D6F 1119         */ IL_1E33: ldloc.s   V_25
// 		/* 0x00090D71 7B350C0004   */ IL_1E35: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090D76 1119         */ IL_1E3A: ldloc.s   V_25
// 		/* 0x00090D78 7B350C0004   */ IL_1E3C: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090D7D 1178         */ IL_1E41: ldloc.s   V_120
// 		/* 0x00090D7F 283D01000A   */ IL_1E43: call      float32 [UnityEngine.CoreModule]UnityEngine.Vector3::Dot(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090D84 1178         */ IL_1E48: ldloc.s   V_120
// 		/* 0x00090D86 285001000A   */ IL_1E4A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(float32, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090D8B 287900000A   */ IL_1E4F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Subtraction(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090D90 137A         */ IL_1E54: stloc.s   V_122
// 		/* 0x00090D92 127A         */ IL_1E56: ldloca.s  V_122
// 		/* 0x00090D94 289200000A   */ IL_1E58: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x00090D99 1379         */ IL_1E5D: stloc.s   V_121
// 		/* 0x00090D9B 1119         */ IL_1E5F: ldloc.s   V_25
// 		/* 0x00090D9D 7B380C0004   */ IL_1E61: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090DA2 1179         */ IL_1E66: ldloc.s   V_121
// 		/* 0x00090DA4 1178         */ IL_1E68: ldloc.s   V_120
// 		/* 0x00090DA6 283A01000A   */ IL_1E6A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::LookRotation(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00090DAB 1177         */ IL_1E6F: ldloc.s   V_119
// 		/* 0x00090DAD 28DE00000A   */ IL_1E71: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 		/* 0x00090DB2 1313         */ IL_1E76: stloc.s   V_19

// 		/* 0x00090DB4 17           */ IL_1E78: ldc.i4.1
// 		/* 0x00090DB5 131B         */ IL_1E79: stloc.s   V_27

// 		/* 0x00090DB7 1119         */ IL_1E7B: ldloc.s   V_25
// 		/* 0x00090DB9 7B360C0004   */ IL_1E7D: ldfld     float32 ShipData::uSpeed
// 		/* 0x00090DBE 6C           */ IL_1E82: conv.r8
// 		/* 0x00090DBF 23111111111111913F */ IL_1E83: ldc.r8    0.016666666666666666
// 		/* 0x00090DC8 5A           */ IL_1E8C: mul
// 		/* 0x00090DC9 132F         */ IL_1E8D: stloc.s   V_47
// 		/* 0x00090DCB 1119         */ IL_1E8F: ldloc.s   V_25
// 		/* 0x00090DCD 7C340C0004   */ IL_1E91: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090DD2 1119         */ IL_1E96: ldloc.s   V_25
// 		/* 0x00090DD4 7C340C0004   */ IL_1E98: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090DD9 7B41030004   */ IL_1E9D: ldfld     float64 VectorLF3::x
// 		/* 0x00090DDE 1119         */ IL_1EA2: ldloc.s   V_25
// 		/* 0x00090DE0 7C350C0004   */ IL_1EA4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090DE5 7B4100000A   */ IL_1EA9: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00090DEA 6C           */ IL_1EAE: conv.r8
// 		/* 0x00090DEB 112F         */ IL_1EAF: ldloc.s   V_47
// 		/* 0x00090DED 5A           */ IL_1EB1: mul
// 		/* 0x00090DEE 58           */ IL_1EB2: add
// 		/* 0x00090DEF 112E         */ IL_1EB3: ldloc.s   V_46
// 		/* 0x00090DF1 7B41030004   */ IL_1EB5: ldfld     float64 VectorLF3::x
// 		/* 0x00090DF6 58           */ IL_1EBA: add
// 		/* 0x00090DF7 7D41030004   */ IL_1EBB: stfld     float64 VectorLF3::x
// 		/* 0x00090DFC 1119         */ IL_1EC0: ldloc.s   V_25
// 		/* 0x00090DFE 7C340C0004   */ IL_1EC2: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090E03 1119         */ IL_1EC7: ldloc.s   V_25
// 		/* 0x00090E05 7C340C0004   */ IL_1EC9: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090E0A 7B42030004   */ IL_1ECE: ldfld     float64 VectorLF3::y
// 		/* 0x00090E0F 1119         */ IL_1ED3: ldloc.s   V_25
// 		/* 0x00090E11 7C350C0004   */ IL_1ED5: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090E16 7B4200000A   */ IL_1EDA: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00090E1B 6C           */ IL_1EDF: conv.r8
// 		/* 0x00090E1C 112F         */ IL_1EE0: ldloc.s   V_47
// 		/* 0x00090E1E 5A           */ IL_1EE2: mul
// 		/* 0x00090E1F 58           */ IL_1EE3: add
// 		/* 0x00090E20 112E         */ IL_1EE4: ldloc.s   V_46
// 		/* 0x00090E22 7B42030004   */ IL_1EE6: ldfld     float64 VectorLF3::y
// 		/* 0x00090E27 58           */ IL_1EEB: add
// 		/* 0x00090E28 7D42030004   */ IL_1EEC: stfld     float64 VectorLF3::y
// 		/* 0x00090E2D 1119         */ IL_1EF1: ldloc.s   V_25
// 		/* 0x00090E2F 7C340C0004   */ IL_1EF3: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090E34 1119         */ IL_1EF8: ldloc.s   V_25
// 		/* 0x00090E36 7C340C0004   */ IL_1EFA: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090E3B 7B43030004   */ IL_1EFF: ldfld     float64 VectorLF3::z
// 		/* 0x00090E40 1119         */ IL_1F04: ldloc.s   V_25
// 		/* 0x00090E42 7C350C0004   */ IL_1F06: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00090E47 7B8000000A   */ IL_1F0B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00090E4C 6C           */ IL_1F10: conv.r8
// 		/* 0x00090E4D 112F         */ IL_1F11: ldloc.s   V_47
// 		/* 0x00090E4F 5A           */ IL_1F13: mul
// 		/* 0x00090E50 58           */ IL_1F14: add
// 		/* 0x00090E51 112E         */ IL_1F15: ldloc.s   V_46
// 		/* 0x00090E53 7B43030004   */ IL_1F17: ldfld     float64 VectorLF3::z
// 		/* 0x00090E58 58           */ IL_1F1C: add
// 		/* 0x00090E59 7D43030004   */ IL_1F1D: stfld     float64 VectorLF3::z
// 		/* 0x00090E5E 112B         */ IL_1F22: ldloc.s   V_43
// 		/* 0x00090E60 39D2000000   */ IL_1F24: brfalse   IL_1FFB

// 		/* 0x00090E65 1119         */ IL_1F29: ldloc.s   V_25
// 		/* 0x00090E67 1113         */ IL_1F2B: ldloc.s   V_19
// 		/* 0x00090E69 7D380C0004   */ IL_1F2D: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090E6E 1119         */ IL_1F32: ldloc.s   V_25
// 		/* 0x00090E70 7B3E0C0004   */ IL_1F34: ldfld     int32 ShipData::direction
// 		/* 0x00090E75 16           */ IL_1F39: ldc.i4.0
// 		/* 0x00090E76 3147         */ IL_1F3A: ble.s     IL_1F83

// 		/* 0x00090E78 1119         */ IL_1F3C: ldloc.s   V_25
// 		/* 0x00090E7A 111C         */ IL_1F3E: ldloc.s   V_28
// 		/* 0x00090E7C 7B44180004   */ IL_1F40: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090E81 1119         */ IL_1F45: ldloc.s   V_25
// 		/* 0x00090E83 7B340C0004   */ IL_1F47: ldfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090E88 111C         */ IL_1F4C: ldloc.s   V_28
// 		/* 0x00090E8A 7B46180004   */ IL_1F4E: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00090E8F 28BA020006   */ IL_1F53: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00090E94 2811040006   */ IL_1F58: call      valuetype VectorLF3 Maths::QInvRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00090E99 7D3B0C0004   */ IL_1F5D: stfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x00090E9E 1119         */ IL_1F62: ldloc.s   V_25
// 		/* 0x00090EA0 111C         */ IL_1F64: ldloc.s   V_28
// 		/* 0x00090EA2 7B44180004   */ IL_1F66: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090EA7 281901000A   */ IL_1F6B: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Inverse(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090EAC 1119         */ IL_1F70: ldloc.s   V_25
// 		/* 0x00090EAE 7B380C0004   */ IL_1F72: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090EB3 281A01000A   */ IL_1F77: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090EB8 7D3C0C0004   */ IL_1F7C: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp
// 		/* 0x00090EBD 2B45         */ IL_1F81: br.s      IL_1FC8

// 		/* 0x00090EBF 1119         */ IL_1F83: ldloc.s   V_25
// 		/* 0x00090EC1 110B         */ IL_1F85: ldloc.s   V_11
// 		/* 0x00090EC3 7B44180004   */ IL_1F87: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090EC8 1119         */ IL_1F8C: ldloc.s   V_25
// 		/* 0x00090ECA 7B340C0004   */ IL_1F8E: ldfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00090ECF 110B         */ IL_1F93: ldloc.s   V_11
// 		/* 0x00090ED1 7B46180004   */ IL_1F95: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00090ED6 28BA020006   */ IL_1F9A: call      valuetype VectorLF3 VectorLF3::op_Subtraction(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00090EDB 2811040006   */ IL_1F9F: call      valuetype VectorLF3 Maths::QInvRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00090EE0 7D3B0C0004   */ IL_1FA4: stfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x00090EE5 1119         */ IL_1FA9: ldloc.s   V_25
// 		/* 0x00090EE7 110B         */ IL_1FAB: ldloc.s   V_11
// 		/* 0x00090EE9 7B44180004   */ IL_1FAD: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00090EEE 281901000A   */ IL_1FB2: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Inverse(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090EF3 1119         */ IL_1FB7: ldloc.s   V_25
// 		/* 0x00090EF5 7B380C0004   */ IL_1FB9: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00090EFA 281A01000A   */ IL_1FBE: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00090EFF 7D3C0C0004   */ IL_1FC3: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp

// 		/* 0x00090F04 1213         */ IL_1FC8: ldloca.s  V_19
// 		/* 0x00090F06 1213         */ IL_1FCA: ldloca.s  V_19
// 		/* 0x00090F08 1213         */ IL_1FCC: ldloca.s  V_19
// 		/* 0x00090F0A 2200000000   */ IL_1FCE: ldc.r4    0.0
// 		/* 0x00090F0F 25           */ IL_1FD3: dup
// 		/* 0x00090F10 131E         */ IL_1FD4: stloc.s   V_30
// 		/* 0x00090F12 7D5502000A   */ IL_1FD6: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::z
// 		/* 0x00090F17 111E         */ IL_1FDB: ldloc.s   V_30
// 		/* 0x00090F19 25           */ IL_1FDD: dup
// 		/* 0x00090F1A 131E         */ IL_1FDE: stloc.s   V_30
// 		/* 0x00090F1C 7D5402000A   */ IL_1FE0: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::y
// 		/* 0x00090F21 111E         */ IL_1FE5: ldloc.s   V_30
// 		/* 0x00090F23 7D5302000A   */ IL_1FE7: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::x
// 		/* 0x00090F28 1213         */ IL_1FEC: ldloca.s  V_19
// 		/* 0x00090F2A 220000803F   */ IL_1FEE: ldc.r4    1
// 		/* 0x00090F2F 7D5602000A   */ IL_1FF3: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Quaternion::w
// 		/* 0x00090F34 16           */ IL_1FF8: ldc.i4.0
// 		/* 0x00090F35 131B         */ IL_1FF9: stloc.s   V_27

// 		/* 0x00090F37 111A         */ IL_1FFB: ldloc.s   V_26
// 		/* 0x00090F39 7C4A0C0004   */ IL_1FFD: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00090F3E 7B7E01000A   */ IL_2002: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x00090F43 220000803F   */ IL_2007: ldc.r4    1
// 		/* 0x00090F48 3617         */ IL_200C: ble.un.s  IL_2025

// 		/* 0x00090F4A 111A         */ IL_200E: ldloc.s   V_26
// 		/* 0x00090F4C 7C4A0C0004   */ IL_2010: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00090F51 7C7E01000A   */ IL_2015: ldflda    float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x00090F56 25           */ IL_201A: dup
// 		/* 0x00090F57 4E           */ IL_201B: ldind.r4
// 		/* 0x00090F58 220BD7A33B   */ IL_201C: ldc.r4    0.0050000004
// 		/* 0x00090F5D 59           */ IL_2021: sub
// 		/* 0x00090F5E 56           */ IL_2022: stind.r4
// 		/* 0x00090F5F 2B11         */ IL_2023: br.s      IL_2036

// 		/* 0x00090F61 111A         */ IL_2025: ldloc.s   V_26
// 		/* 0x00090F63 7C4A0C0004   */ IL_2027: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00090F68 220000803F   */ IL_202C: ldc.r4    1
// 		/* 0x00090F6D 7D7E01000A   */ IL_2031: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z

// 		/* 0x00090F72 111A         */ IL_2036: ldloc.s   V_26
// 		/* 0x00090F74 7C4A0C0004   */ IL_2038: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00090F79 1119         */ IL_203D: ldloc.s   V_25
// 		/* 0x00090F7B 7B370C0004   */ IL_203F: ldfld     float32 ShipData::warpState
// 		/* 0x00090F80 7D7F01000A   */ IL_2044: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::w
// 		/* 0x00090F85 38BA0E0000   */ IL_2049: br        IL_2F08

// 		/* 0x00090F8A 1119         */ IL_204E: ldloc.s   V_25
// 		/* 0x00090F8C 7B310C0004   */ IL_2050: ldfld     int32 ShipData::stage
// 		/* 0x00090F91 17           */ IL_2055: ldc.i4.1
// 		/* 0x00090F92 40D3030000   */ IL_2056: bne.un    IL_242E

// 		/* 0x00090F97 2200000000   */ IL_205B: ldc.r4    0.0
// 		/* 0x00090F9C 137B         */ IL_2060: stloc.s   V_123
// 		/* 0x00090F9E 1119         */ IL_2062: ldloc.s   V_25
// 		/* 0x00090FA0 7B3E0C0004   */ IL_2064: ldfld     int32 ShipData::direction
// 		/* 0x00090FA5 16           */ IL_2069: ldc.i4.0
// 		/* 0x00090FA6 3E31020000   */ IL_206A: ble       IL_22A0

// 		/* 0x00090FAB 1119         */ IL_206F: ldloc.s   V_25
// 		/* 0x00090FAD 7C3F0C0004   */ IL_2071: ldflda    float32 ShipData::t
// 		/* 0x00090FB2 25           */ IL_2076: dup
// 		/* 0x00090FB3 4E           */ IL_2077: ldind.r4
// 		/* 0x00090FB4 110F         */ IL_2078: ldloc.s   V_15
// 		/* 0x00090FB6 22ABAA2A3F   */ IL_207A: ldc.r4    0.6666667
// 		/* 0x00090FBB 5A           */ IL_207F: mul
// 		/* 0x00090FBC 59           */ IL_2080: sub
// 		/* 0x00090FBD 56           */ IL_2081: stind.r4
// 		/* 0x00090FBE 1119         */ IL_2082: ldloc.s   V_25
// 		/* 0x00090FC0 7B3F0C0004   */ IL_2084: ldfld     float32 ShipData::t
// 		/* 0x00090FC5 137B         */ IL_2089: stloc.s   V_123
// 		/* 0x00090FC7 1119         */ IL_208B: ldloc.s   V_25
// 		/* 0x00090FC9 7B3F0C0004   */ IL_208D: ldfld     float32 ShipData::t
// 		/* 0x00090FCE 2200000000   */ IL_2092: ldc.r4    0.0
// 		/* 0x00090FD3 341B         */ IL_2097: bge.un.s  IL_20B4

// 		/* 0x00090FD5 1119         */ IL_2099: ldloc.s   V_25
// 		/* 0x00090FD7 220000803F   */ IL_209B: ldc.r4    1
// 		/* 0x00090FDC 7D3F0C0004   */ IL_20A0: stfld     float32 ShipData::t
// 		/* 0x00090FE1 2200000000   */ IL_20A5: ldc.r4    0.0
// 		/* 0x00090FE6 137B         */ IL_20AA: stloc.s   V_123
// 		/* 0x00090FE8 1119         */ IL_20AC: ldloc.s   V_25
// 		/* 0x00090FEA 18           */ IL_20AE: ldc.i4.2
// 		/* 0x00090FEB 7D310C0004   */ IL_20AF: stfld     int32 ShipData::stage

// 		/* 0x00090FF0 2200004040   */ IL_20B4: ldc.r4    3
// 		/* 0x00090FF5 117B         */ IL_20B9: ldloc.s   V_123
// 		/* 0x00090FF7 59           */ IL_20BB: sub
// 		/* 0x00090FF8 117B         */ IL_20BC: ldloc.s   V_123
// 		/* 0x00090FFA 59           */ IL_20BE: sub
// 		/* 0x00090FFB 117B         */ IL_20BF: ldloc.s   V_123
// 		/* 0x00090FFD 5A           */ IL_20C1: mul
// 		/* 0x00090FFE 117B         */ IL_20C2: ldloc.s   V_123
// 		/* 0x00091000 5A           */ IL_20C4: mul
// 		/* 0x00091001 137B         */ IL_20C5: stloc.s   V_123
// 		/* 0x00091003 117B         */ IL_20C7: ldloc.s   V_123
// 		/* 0x00091005 2200000040   */ IL_20C9: ldc.r4    2
// 		/* 0x0009100A 5A           */ IL_20CE: mul
// 		/* 0x0009100B 137C         */ IL_20CF: stloc.s   V_124
// 		/* 0x0009100D 117B         */ IL_20D1: ldloc.s   V_123
// 		/* 0x0009100F 2200000040   */ IL_20D3: ldc.r4    2
// 		/* 0x00091014 5A           */ IL_20D8: mul
// 		/* 0x00091015 220000803F   */ IL_20D9: ldc.r4    1
// 		/* 0x0009101A 59           */ IL_20DE: sub
// 		/* 0x0009101B 137D         */ IL_20DF: stloc.s   V_125
// 		/* 0x0009101D 111C         */ IL_20E1: ldloc.s   V_28
// 		/* 0x0009101F 7B46180004   */ IL_20E3: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00091024 111C         */ IL_20E8: ldloc.s   V_28
// 		/* 0x00091026 7B44180004   */ IL_20EA: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0009102B 0E06         */ IL_20EF: ldarg.s   gStationPool
// 		/* 0x0009102D 1119         */ IL_20F1: ldloc.s   V_25
// 		/* 0x0009102F 7B3D0C0004   */ IL_20F3: ldfld     int32 ShipData::otherGId
// 		/* 0x00091034 9A           */ IL_20F8: ldelem.ref
// 		/* 0x00091035 7BC90B0004   */ IL_20F9: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0009103A 0E06         */ IL_20FE: ldarg.s   gStationPool
// 		/* 0x0009103C 1119         */ IL_2100: ldloc.s   V_25
// 		/* 0x0009103E 7B3D0C0004   */ IL_2102: ldfld     int32 ShipData::otherGId
// 		/* 0x00091043 9A           */ IL_2107: ldelem.ref
// 		/* 0x00091044 7CC90B0004   */ IL_2108: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x00091049 289200000A   */ IL_210D: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x0009104E 22D8A3E840   */ IL_2112: ldc.r4    7.2700005
// 		/* 0x00091053 289800000A   */ IL_2117: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00091058 289900000A   */ IL_211C: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0009105D 28BC020006   */ IL_2121: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091062 280D040006   */ IL_2126: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00091067 28BB020006   */ IL_212B: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0009106C 137E         */ IL_2130: stloc.s   V_126
// 		/* 0x0009106E 117B         */ IL_2132: ldloc.s   V_123
// 		/* 0x00091070 220000003F   */ IL_2134: ldc.r4    0.5
// 		/* 0x00091075 43A6000000   */ IL_2139: ble.un    IL_21E4

// 		/* 0x0009107A 111C         */ IL_213E: ldloc.s   V_28
// 		/* 0x0009107C 7B46180004   */ IL_2140: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00091081 111C         */ IL_2145: ldloc.s   V_28
// 		/* 0x00091083 7B44180004   */ IL_2147: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00091088 1119         */ IL_214C: ldloc.s   V_25
// 		/* 0x0009108A 7B3B0C0004   */ IL_214E: ldfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x0009108F 280D040006   */ IL_2153: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00091094 28BB020006   */ IL_2158: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00091099 137F         */ IL_215D: stloc.s   V_127
// 		/* 0x0009109B 1119         */ IL_215F: ldloc.s   V_25
// 		/* 0x0009109D 117E         */ IL_2161: ldloc.s   V_126
// 		/* 0x0009109F 220000803F   */ IL_2163: ldc.r4    1
// 		/* 0x000910A4 117D         */ IL_2168: ldloc.s   V_125
// 		/* 0x000910A6 59           */ IL_216A: sub
// 		/* 0x000910A7 6C           */ IL_216B: conv.r8
// 		/* 0x000910A8 28B7020006   */ IL_216C: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x000910AD 117F         */ IL_2171: ldloc.s   V_127
// 		/* 0x000910AF 117D         */ IL_2173: ldloc.s   V_125
// 		/* 0x000910B1 6C           */ IL_2175: conv.r8
// 		/* 0x000910B2 28B7020006   */ IL_2176: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x000910B7 28BB020006   */ IL_217B: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x000910BC 7D340C0004   */ IL_2180: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x000910C1 1119         */ IL_2185: ldloc.s   V_25
// 		/* 0x000910C3 111C         */ IL_2187: ldloc.s   V_28
// 		/* 0x000910C5 7B44180004   */ IL_2189: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x000910CA 0E06         */ IL_218E: ldarg.s   gStationPool
// 		/* 0x000910CC 1119         */ IL_2190: ldloc.s   V_25
// 		/* 0x000910CE 7B3D0C0004   */ IL_2192: ldfld     int32 ShipData::otherGId
// 		/* 0x000910D3 9A           */ IL_2197: ldelem.ref
// 		/* 0x000910D4 7BCA0B0004   */ IL_2198: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion StationComponent::shipDockRot
// 		/* 0x000910D9 22F304353F   */ IL_219D: ldc.r4    0.70710677
// 		/* 0x000910DE 2200000000   */ IL_21A2: ldc.r4    0.0
// 		/* 0x000910E3 2200000000   */ IL_21A7: ldc.r4    0.0
// 		/* 0x000910E8 22F30435BF   */ IL_21AC: ldc.r4    -0.70710677
// 		/* 0x000910ED 735702000A   */ IL_21B1: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x000910F2 281A01000A   */ IL_21B6: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x000910F7 1119         */ IL_21BB: ldloc.s   V_25
// 		/* 0x000910F9 7B3C0C0004   */ IL_21BD: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp
// 		/* 0x000910FE 117D         */ IL_21C2: ldloc.s   V_125
// 		/* 0x00091100 220000C03F   */ IL_21C4: ldc.r4    1.5
// 		/* 0x00091105 5A           */ IL_21C9: mul
// 		/* 0x00091106 220000003F   */ IL_21CA: ldc.r4    0.5
// 		/* 0x0009110B 59           */ IL_21CF: sub
// 		/* 0x0009110C 28DE00000A   */ IL_21D0: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::Slerp(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, float32)
// 		/* 0x00091111 281A01000A   */ IL_21D5: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00091116 7D380C0004   */ IL_21DA: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x0009111B 38AD010000   */ IL_21DF: br        IL_2391

// 		/* 0x00091120 111C         */ IL_21E4: ldloc.s   V_28
// 		/* 0x00091122 7B46180004   */ IL_21E6: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00091127 111C         */ IL_21EB: ldloc.s   V_28
// 		/* 0x00091129 7B44180004   */ IL_21ED: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0009112E 0E06         */ IL_21F2: ldarg.s   gStationPool
// 		/* 0x00091130 1119         */ IL_21F4: ldloc.s   V_25
// 		/* 0x00091132 7B3D0C0004   */ IL_21F6: ldfld     int32 ShipData::otherGId
// 		/* 0x00091137 9A           */ IL_21FB: ldelem.ref
// 		/* 0x00091138 7BC90B0004   */ IL_21FC: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0009113D 0E06         */ IL_2201: ldarg.s   gStationPool
// 		/* 0x0009113F 1119         */ IL_2203: ldloc.s   V_25
// 		/* 0x00091141 7B3D0C0004   */ IL_2205: ldfld     int32 ShipData::otherGId
// 		/* 0x00091146 9A           */ IL_220A: ldelem.ref
// 		/* 0x00091147 7CC90B0004   */ IL_220B: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0009114C 289200000A   */ IL_2210: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x00091151 22666666C1   */ IL_2215: ldc.r4    -14.4
// 		/* 0x00091156 289800000A   */ IL_221A: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x0009115B 289900000A   */ IL_221F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091160 28BC020006   */ IL_2224: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091165 280D040006   */ IL_2229: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x0009116A 28BB020006   */ IL_222E: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x0009116F 1380         */ IL_2233: stloc.s   V_128
// 		/* 0x00091171 1119         */ IL_2235: ldloc.s   V_25
// 		/* 0x00091173 1180         */ IL_2237: ldloc.s   V_128
// 		/* 0x00091175 220000803F   */ IL_2239: ldc.r4    1
// 		/* 0x0009117A 117C         */ IL_223E: ldloc.s   V_124
// 		/* 0x0009117C 59           */ IL_2240: sub
// 		/* 0x0009117D 6C           */ IL_2241: conv.r8
// 		/* 0x0009117E 28B7020006   */ IL_2242: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x00091183 117E         */ IL_2247: ldloc.s   V_126
// 		/* 0x00091185 117C         */ IL_2249: ldloc.s   V_124
// 		/* 0x00091187 6C           */ IL_224B: conv.r8
// 		/* 0x00091188 28B7020006   */ IL_224C: call      valuetype VectorLF3 VectorLF3::op_Multiply(valuetype VectorLF3, float64)
// 		/* 0x0009118D 28BB020006   */ IL_2251: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00091192 7D340C0004   */ IL_2256: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091197 1119         */ IL_225B: ldloc.s   V_25
// 		/* 0x00091199 111C         */ IL_225D: ldloc.s   V_28
// 		/* 0x0009119B 7B44180004   */ IL_225F: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x000911A0 0E06         */ IL_2264: ldarg.s   gStationPool
// 		/* 0x000911A2 1119         */ IL_2266: ldloc.s   V_25
// 		/* 0x000911A4 7B3D0C0004   */ IL_2268: ldfld     int32 ShipData::otherGId
// 		/* 0x000911A9 9A           */ IL_226D: ldelem.ref
// 		/* 0x000911AA 7BCA0B0004   */ IL_226E: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion StationComponent::shipDockRot
// 		/* 0x000911AF 22F304353F   */ IL_2273: ldc.r4    0.70710677
// 		/* 0x000911B4 2200000000   */ IL_2278: ldc.r4    0.0
// 		/* 0x000911B9 2200000000   */ IL_227D: ldc.r4    0.0
// 		/* 0x000911BE 22F30435BF   */ IL_2282: ldc.r4    -0.70710677
// 		/* 0x000911C3 735702000A   */ IL_2287: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x000911C8 281A01000A   */ IL_228C: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x000911CD 281A01000A   */ IL_2291: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x000911D2 7D380C0004   */ IL_2296: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x000911D7 38F1000000   */ IL_229B: br        IL_2391

// 		/* 0x000911DC 1119         */ IL_22A0: ldloc.s   V_25
// 		/* 0x000911DE 7C3F0C0004   */ IL_22A2: ldflda    float32 ShipData::t
// 		/* 0x000911E3 25           */ IL_22A7: dup
// 		/* 0x000911E4 4E           */ IL_22A8: ldind.r4
// 		/* 0x000911E5 110F         */ IL_22A9: ldloc.s   V_15
// 		/* 0x000911E7 58           */ IL_22AB: add
// 		/* 0x000911E8 56           */ IL_22AC: stind.r4
// 		/* 0x000911E9 1119         */ IL_22AD: ldloc.s   V_25
// 		/* 0x000911EB 7B3F0C0004   */ IL_22AF: ldfld     float32 ShipData::t
// 		/* 0x000911F0 137B         */ IL_22B4: stloc.s   V_123
// 		/* 0x000911F2 1119         */ IL_22B6: ldloc.s   V_25
// 		/* 0x000911F4 7B3F0C0004   */ IL_22B8: ldfld     float32 ShipData::t
// 		/* 0x000911F9 220000803F   */ IL_22BD: ldc.r4    1
// 		/* 0x000911FE 361B         */ IL_22C2: ble.un.s  IL_22DF

// 		/* 0x00091200 1119         */ IL_22C4: ldloc.s   V_25
// 		/* 0x00091202 220000803F   */ IL_22C6: ldc.r4    1
// 		/* 0x00091207 7D3F0C0004   */ IL_22CB: stfld     float32 ShipData::t
// 		/* 0x0009120C 220000803F   */ IL_22D0: ldc.r4    1
// 		/* 0x00091211 137B         */ IL_22D5: stloc.s   V_123
// 		/* 0x00091213 1119         */ IL_22D7: ldloc.s   V_25
// 		/* 0x00091215 16           */ IL_22D9: ldc.i4.0
// 		/* 0x00091216 7D310C0004   */ IL_22DA: stfld     int32 ShipData::stage

// 		/* 0x0009121B 2200004040   */ IL_22DF: ldc.r4    3
// 		/* 0x00091220 117B         */ IL_22E4: ldloc.s   V_123
// 		/* 0x00091222 59           */ IL_22E6: sub
// 		/* 0x00091223 117B         */ IL_22E7: ldloc.s   V_123
// 		/* 0x00091225 59           */ IL_22E9: sub
// 		/* 0x00091226 117B         */ IL_22EA: ldloc.s   V_123
// 		/* 0x00091228 5A           */ IL_22EC: mul
// 		/* 0x00091229 117B         */ IL_22ED: ldloc.s   V_123
// 		/* 0x0009122B 5A           */ IL_22EF: mul
// 		/* 0x0009122C 137B         */ IL_22F0: stloc.s   V_123
// 		/* 0x0009122E 1119         */ IL_22F2: ldloc.s   V_25
// 		/* 0x00091230 111C         */ IL_22F4: ldloc.s   V_28
// 		/* 0x00091232 7B46180004   */ IL_22F6: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00091237 111C         */ IL_22FB: ldloc.s   V_28
// 		/* 0x00091239 7B44180004   */ IL_22FD: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x0009123E 0E06         */ IL_2302: ldarg.s   gStationPool
// 		/* 0x00091240 1119         */ IL_2304: ldloc.s   V_25
// 		/* 0x00091242 7B3D0C0004   */ IL_2306: ldfld     int32 ShipData::otherGId
// 		/* 0x00091247 9A           */ IL_230B: ldelem.ref
// 		/* 0x00091248 7BC90B0004   */ IL_230C: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0009124D 0E06         */ IL_2311: ldarg.s   gStationPool
// 		/* 0x0009124F 1119         */ IL_2313: ldloc.s   V_25
// 		/* 0x00091251 7B3D0C0004   */ IL_2315: ldfld     int32 ShipData::otherGId
// 		/* 0x00091256 9A           */ IL_231A: ldelem.ref
// 		/* 0x00091257 7CC90B0004   */ IL_231B: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x0009125C 289200000A   */ IL_2320: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x00091261 22666666C1   */ IL_2325: ldc.r4    -14.4
// 		/* 0x00091266 229A991D42   */ IL_232A: ldc.r4    39.4
// 		/* 0x0009126B 117B         */ IL_232F: ldloc.s   V_123
// 		/* 0x0009126D 5A           */ IL_2331: mul
// 		/* 0x0009126E 58           */ IL_2332: add
// 		/* 0x0009126F 289800000A   */ IL_2333: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00091274 289900000A   */ IL_2338: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091279 28BC020006   */ IL_233D: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x0009127E 280D040006   */ IL_2342: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00091283 28BB020006   */ IL_2347: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00091288 7D340C0004   */ IL_234C: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x0009128D 1119         */ IL_2351: ldloc.s   V_25
// 		/* 0x0009128F 111C         */ IL_2353: ldloc.s   V_28
// 		/* 0x00091291 7B44180004   */ IL_2355: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00091296 0E06         */ IL_235A: ldarg.s   gStationPool
// 		/* 0x00091298 1119         */ IL_235C: ldloc.s   V_25
// 		/* 0x0009129A 7B3D0C0004   */ IL_235E: ldfld     int32 ShipData::otherGId
// 		/* 0x0009129F 9A           */ IL_2363: ldelem.ref
// 		/* 0x000912A0 7BCA0B0004   */ IL_2364: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion StationComponent::shipDockRot
// 		/* 0x000912A5 22F304353F   */ IL_2369: ldc.r4    0.70710677
// 		/* 0x000912AA 2200000000   */ IL_236E: ldc.r4    0.0
// 		/* 0x000912AF 2200000000   */ IL_2373: ldc.r4    0.0
// 		/* 0x000912B4 22F30435BF   */ IL_2378: ldc.r4    -0.70710677
// 		/* 0x000912B9 735702000A   */ IL_237D: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x000912BE 281A01000A   */ IL_2382: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x000912C3 281A01000A   */ IL_2387: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x000912C8 7D380C0004   */ IL_238C: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot

// 		/* 0x000912CD 1119         */ IL_2391: ldloc.s   V_25
// 		/* 0x000912CF 7C350C0004   */ IL_2393: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000912D4 2200000000   */ IL_2398: ldc.r4    0.0
// 		/* 0x000912D9 7D4100000A   */ IL_239D: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x000912DE 1119         */ IL_23A2: ldloc.s   V_25
// 		/* 0x000912E0 7C350C0004   */ IL_23A4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000912E5 2200000000   */ IL_23A9: ldc.r4    0.0
// 		/* 0x000912EA 7D4200000A   */ IL_23AE: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x000912EF 1119         */ IL_23B3: ldloc.s   V_25
// 		/* 0x000912F1 7C350C0004   */ IL_23B5: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x000912F6 2200000000   */ IL_23BA: ldc.r4    0.0
// 		/* 0x000912FB 7D8000000A   */ IL_23BF: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00091300 1119         */ IL_23C4: ldloc.s   V_25
// 		/* 0x00091302 2200000000   */ IL_23C6: ldc.r4    0.0
// 		/* 0x00091307 7D360C0004   */ IL_23CB: stfld     float32 ShipData::uSpeed
// 		/* 0x0009130C 1119         */ IL_23D0: ldloc.s   V_25
// 		/* 0x0009130E 7C390C0004   */ IL_23D2: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091313 2200000000   */ IL_23D7: ldc.r4    0.0
// 		/* 0x00091318 7D4100000A   */ IL_23DC: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x0009131D 1119         */ IL_23E1: ldloc.s   V_25
// 		/* 0x0009131F 7C390C0004   */ IL_23E3: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091324 2200000000   */ IL_23E8: ldc.r4    0.0
// 		/* 0x00091329 7D4200000A   */ IL_23ED: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x0009132E 1119         */ IL_23F2: ldloc.s   V_25
// 		/* 0x00091330 7C390C0004   */ IL_23F4: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091335 2200000000   */ IL_23F9: ldc.r4    0.0
// 		/* 0x0009133A 7D8000000A   */ IL_23FE: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x0009133F 1119         */ IL_2403: ldloc.s   V_25
// 		/* 0x00091341 2200000000   */ IL_2405: ldc.r4    0.0
// 		/* 0x00091346 7D3A0C0004   */ IL_240A: stfld     float32 ShipData::uAngularSpeed
// 		/* 0x0009134B 111A         */ IL_240F: ldloc.s   V_26
// 		/* 0x0009134D 7C4A0C0004   */ IL_2411: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00091352 117B         */ IL_2416: ldloc.s   V_123
// 		/* 0x00091354 229A99D93F   */ IL_2418: ldc.r4    1.7
// 		/* 0x00091359 5A           */ IL_241D: mul
// 		/* 0x0009135A 223333333F   */ IL_241E: ldc.r4    0.7
// 		/* 0x0009135F 59           */ IL_2423: sub
// 		/* 0x00091360 7D7E01000A   */ IL_2424: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x00091365 38DA0A0000   */ IL_2429: br        IL_2F08

// 		/* 0x0009136A 1119         */ IL_242E: ldloc.s   V_25
// 		/* 0x0009136C 7B3E0C0004   */ IL_2430: ldfld     int32 ShipData::direction
// 		/* 0x00091371 16           */ IL_2435: ldc.i4.0
// 		/* 0x00091372 3E3B090000   */ IL_2436: ble       IL_2D76

// 		/* 0x00091377 1119         */ IL_243B: ldloc.s   V_25
// 		/* 0x00091379 7C3F0C0004   */ IL_243D: ldflda    float32 ShipData::t
// 		/* 0x0009137E 25           */ IL_2442: dup
// 		/* 0x0009137F 4E           */ IL_2443: ldind.r4
// 		/* 0x00091380 2270CE083D   */ IL_2444: ldc.r4    0.0334
// 		/* 0x00091385 59           */ IL_2449: sub
// 		/* 0x00091386 56           */ IL_244A: stind.r4
// 		/* 0x00091387 1119         */ IL_244B: ldloc.s   V_25
// 		/* 0x00091389 7B3F0C0004   */ IL_244D: ldfld     float32 ShipData::t
// 		/* 0x0009138E 2200000000   */ IL_2452: ldc.r4    0.0
// 		/* 0x00091393 414C090000   */ IL_2457: bge.un    IL_2DA8

// 		/* 0x00091398 1119         */ IL_245C: ldloc.s   V_25
// 		/* 0x0009139A 2200000000   */ IL_245E: ldc.r4    0.0
// 		/* 0x0009139F 7D3F0C0004   */ IL_2463: stfld     float32 ShipData::t
// 		/* 0x000913A4 0E06         */ IL_2468: ldarg.s   gStationPool
// 		/* 0x000913A6 1119         */ IL_246A: ldloc.s   V_25
// 		/* 0x000913A8 7B3D0C0004   */ IL_246C: ldfld     int32 ShipData::otherGId
// 		/* 0x000913AD 9A           */ IL_2471: ldelem.ref
// 		/* 0x000913AE 1381         */ IL_2472: stloc.s   V_129
// 		/* 0x000913B0 1181         */ IL_2474: ldloc.s   V_129
// 		/* 0x000913B2 7BE00B0004   */ IL_2476: ldfld     valuetype StationStore[] StationComponent::'storage'
// 		/* 0x000913B7 1382         */ IL_247B: stloc.s   V_130
// 		/* 0x000913B9 1112         */ IL_247D: ldloc.s   V_18
// 		/* 0x000913BB 02           */ IL_247F: ldarg.0
// 		/* 0x000913BC 7BEF0B0004   */ IL_2480: ldfld     float64 StationComponent::warpEnableDist
// 		/* 0x000913C1 365D         */ IL_2485: ble.un.s  IL_24E4

// 		/* 0x000913C3 1119         */ IL_2487: ldloc.s   V_25
// 		/* 0x000913C5 7B450C0004   */ IL_2489: ldfld     int32 ShipData::warperCnt
// 		/* 0x000913CA 2D54         */ IL_248E: brtrue.s  IL_24E4

// 		/* 0x000913CC 1181         */ IL_2490: ldloc.s   V_129
// 		/* 0x000913CE 7BD10B0004   */ IL_2492: ldfld     int32 StationComponent::warperCount
// 		/* 0x000913D3 16           */ IL_2497: ldc.i4.0
// 		/* 0x000913D4 314A         */ IL_2498: ble.s     IL_24E4

// 		/* 0x000913D6 0E0B         */ IL_249A: ldarg.s   consumeRegister
// 		/* 0x000913D8 1383         */ IL_249C: stloc.s   V_131
// 		/* 0x000913DA 16           */ IL_249E: ldc.i4.0
// 		/* 0x000913DB 1315         */ IL_249F: stloc.s   V_21
// 		.try
// 		{
// 			/* 0x000913DD 1183         */ IL_24A1: ldloc.s   V_131
// 			/* 0x000913DF 1215         */ IL_24A3: ldloca.s  V_21
// 			/* 0x000913E1 287502000A   */ IL_24A5: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 			/* 0x000913E6 1119         */ IL_24AA: ldloc.s   V_25
// 			/* 0x000913E8 7C450C0004   */ IL_24AC: ldflda    int32 ShipData::warperCnt
// 			/* 0x000913ED 25           */ IL_24B1: dup
// 			/* 0x000913EE 4A           */ IL_24B2: ldind.i4
// 			/* 0x000913EF 17           */ IL_24B3: ldc.i4.1
// 			/* 0x000913F0 58           */ IL_24B4: add
// 			/* 0x000913F1 54           */ IL_24B5: stind.i4
// 			/* 0x000913F2 1181         */ IL_24B6: ldloc.s   V_129
// 			/* 0x000913F4 25           */ IL_24B8: dup
// 			/* 0x000913F5 7BD10B0004   */ IL_24B9: ldfld     int32 StationComponent::warperCount
// 			/* 0x000913FA 17           */ IL_24BE: ldc.i4.1
// 			/* 0x000913FB 59           */ IL_24BF: sub
// 			/* 0x000913FC 7DD10B0004   */ IL_24C0: stfld     int32 StationComponent::warperCount
// 			/* 0x00091401 0E0B         */ IL_24C5: ldarg.s   consumeRegister
// 			/* 0x00091403 20BA040000   */ IL_24C7: ldc.i4    1210
// 			/* 0x00091408 8F2A010001   */ IL_24CC: ldelema   [netstandard]System.Int32
// 			/* 0x0009140D 25           */ IL_24D1: dup
// 			/* 0x0009140E 4A           */ IL_24D2: ldind.i4
// 			/* 0x0009140F 17           */ IL_24D3: ldc.i4.1
// 			/* 0x00091410 58           */ IL_24D4: add
// 			/* 0x00091411 54           */ IL_24D5: stind.i4
// 			/* 0x00091412 DE0C         */ IL_24D6: leave.s   IL_24E4
// 		} // end .try
// 		finally
// 		{
// 			/* 0x00091414 1115         */ IL_24D8: ldloc.s   V_21
// 			/* 0x00091416 2C07         */ IL_24DA: brfalse.s IL_24E3

// 			/* 0x00091418 1183         */ IL_24DC: ldloc.s   V_131
// 			/* 0x0009141A 287602000A   */ IL_24DE: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 			/* 0x0009141F DC           */ IL_24E3: endfinally
// 		} // end handler

// 		/* 0x00091420 1119         */ IL_24E4: ldloc.s   V_25
// 		/* 0x00091422 7B410C0004   */ IL_24E6: ldfld     int32 ShipData::itemCount
// 		/* 0x00091427 16           */ IL_24EB: ldc.i4.0
// 		/* 0x00091428 3E7F060000   */ IL_24EC: ble       IL_2B70

// 		/* 0x0009142D 1181         */ IL_24F1: ldloc.s   V_129
// 		/* 0x0009142F 1119         */ IL_24F3: ldloc.s   V_25
// 		/* 0x00091431 7B400C0004   */ IL_24F5: ldfld     int32 ShipData::itemId
// 		/* 0x00091436 1119         */ IL_24FA: ldloc.s   V_25
// 		/* 0x00091438 7B410C0004   */ IL_24FC: ldfld     int32 ShipData::itemCount
// 		/* 0x0009143D 1119         */ IL_2501: ldloc.s   V_25
// 		/* 0x0009143F 7B420C0004   */ IL_2503: ldfld     int32 ShipData::inc
// 		/* 0x00091444 6F0D0A0006   */ IL_2508: callvirt  instance int32 StationComponent::AddItem(int32, int32, int32)
// 		/* 0x00091449 26           */ IL_250D: pop
// 		/* 0x0009144A 03           */ IL_250E: ldarg.1
// 		/* 0x0009144B 6F9C170006   */ IL_250F: callvirt  instance class GameData PlanetFactory::get_gameData()
// 		/* 0x00091450 7BE51A0004   */ IL_2514: ldfld     class GameStatData GameData::statistics
// 		/* 0x00091455 7B891C0004   */ IL_2519: ldfld     class TrafficStatistics GameStatData::traffic
// 		/* 0x0009145A 1384         */ IL_251E: stloc.s   V_132
// 		/* 0x0009145C 1184         */ IL_2520: ldloc.s   V_132
// 		/* 0x0009145E 1181         */ IL_2522: ldloc.s   V_129
// 		/* 0x00091460 7BC40B0004   */ IL_2524: ldfld     int32 StationComponent::planetId
// 		/* 0x00091465 1119         */ IL_2529: ldloc.s   V_25
// 		/* 0x00091467 7B400C0004   */ IL_252B: ldfld     int32 ShipData::itemId
// 		/* 0x0009146C 1119         */ IL_2530: ldloc.s   V_25
// 		/* 0x0009146E 7B410C0004   */ IL_2532: ldfld     int32 ShipData::itemCount
// 		/* 0x00091473 6FB41A0006   */ IL_2537: callvirt  instance void TrafficStatistics::RegisterPlanetInputStat(int32, int32, int32)
// 		/* 0x00091478 1181         */ IL_253C: ldloc.s   V_129
// 		/* 0x0009147A 7BC40B0004   */ IL_253E: ldfld     int32 StationComponent::planetId
// 		/* 0x0009147F 1F64         */ IL_2543: ldc.i4.s  100
// 		/* 0x00091481 5B           */ IL_2545: div
// 		/* 0x00091482 1385         */ IL_2546: stloc.s   V_133
// 		/* 0x00091484 02           */ IL_2548: ldarg.0
// 		/* 0x00091485 7BC40B0004   */ IL_2549: ldfld     int32 StationComponent::planetId
// 		/* 0x0009148A 1F64         */ IL_254E: ldc.i4.s  100
// 		/* 0x0009148C 5B           */ IL_2550: div
// 		/* 0x0009148D 1386         */ IL_2551: stloc.s   V_134
// 		/* 0x0009148F 1185         */ IL_2553: ldloc.s   V_133
// 		/* 0x00091491 1186         */ IL_2555: ldloc.s   V_134
// 		/* 0x00091493 2E19         */ IL_2557: beq.s     IL_2572

// 		/* 0x00091495 1184         */ IL_2559: ldloc.s   V_132
// 		/* 0x00091497 1185         */ IL_255B: ldloc.s   V_133
// 		/* 0x00091499 1119         */ IL_255D: ldloc.s   V_25
// 		/* 0x0009149B 7B400C0004   */ IL_255F: ldfld     int32 ShipData::itemId
// 		/* 0x000914A0 1119         */ IL_2564: ldloc.s   V_25
// 		/* 0x000914A2 7B410C0004   */ IL_2566: ldfld     int32 ShipData::itemCount
// 		/* 0x000914A7 6FB11A0006   */ IL_256B: callvirt  instance void TrafficStatistics::RegisterStarInputStat(int32, int32, int32)
// 		/* 0x000914AC 2B17         */ IL_2570: br.s      IL_2589

// 		/* 0x000914AE 1184         */ IL_2572: ldloc.s   V_132
// 		/* 0x000914B0 1185         */ IL_2574: ldloc.s   V_133
// 		/* 0x000914B2 1119         */ IL_2576: ldloc.s   V_25
// 		/* 0x000914B4 7B400C0004   */ IL_2578: ldfld     int32 ShipData::itemId
// 		/* 0x000914B9 1119         */ IL_257D: ldloc.s   V_25
// 		/* 0x000914BB 7B410C0004   */ IL_257F: ldfld     int32 ShipData::itemCount
// 		/* 0x000914C0 6FB31A0006   */ IL_2584: callvirt  instance void TrafficStatistics::RegisterStarInternalStat(int32, int32, int32)

// 		/* 0x000914C5 03           */ IL_2589: ldarg.1
// 		/* 0x000914C6 1119         */ IL_258A: ldloc.s   V_25
// 		/* 0x000914C8 7B320C0004   */ IL_258C: ldfld     int32 ShipData::planetA
// 		/* 0x000914CD 02           */ IL_2591: ldarg.0
// 		/* 0x000914CE 1119         */ IL_2592: ldloc.s   V_25
// 		/* 0x000914D0 7B330C0004   */ IL_2594: ldfld     int32 ShipData::planetB
// 		/* 0x000914D5 1181         */ IL_2599: ldloc.s   V_129
// 		/* 0x000914D7 1119         */ IL_259B: ldloc.s   V_25
// 		/* 0x000914D9 7B400C0004   */ IL_259D: ldfld     int32 ShipData::itemId
// 		/* 0x000914DE 1119         */ IL_25A2: ldloc.s   V_25
// 		/* 0x000914E0 7B410C0004   */ IL_25A4: ldfld     int32 ShipData::itemCount
// 		/* 0x000914E5 6FF9170006   */ IL_25A9: callvirt  instance void PlanetFactory::NotifyShipDelivery(int32, class StationComponent, int32, class StationComponent, int32, int32)
// 		/* 0x000914EA 1119         */ IL_25AE: ldloc.s   V_25
// 		/* 0x000914EC 16           */ IL_25B0: ldc.i4.0
// 		/* 0x000914ED 7D410C0004   */ IL_25B1: stfld     int32 ShipData::itemCount
// 		/* 0x000914F2 1119         */ IL_25B6: ldloc.s   V_25
// 		/* 0x000914F4 16           */ IL_25B8: ldc.i4.0
// 		/* 0x000914F5 7D420C0004   */ IL_25B9: stfld     int32 ShipData::inc
// 		/* 0x000914FA 02           */ IL_25BE: ldarg.0
// 		/* 0x000914FB 7BDC0B0004   */ IL_25BF: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x00091500 1118         */ IL_25C4: ldloc.s   V_24
// 		/* 0x00091502 8F5B010002   */ IL_25C6: ldelema   RemoteLogisticOrder
// 		/* 0x00091507 7B5F0C0004   */ IL_25CB: ldfld     int32 RemoteLogisticOrder::otherStationGId
// 		/* 0x0009150C 16           */ IL_25D0: ldc.i4.0
// 		/* 0x0009150D 3E96000000   */ IL_25D1: ble       IL_266C

// 		/* 0x00091512 1182         */ IL_25D6: ldloc.s   V_130
// 		/* 0x00091514 1314         */ IL_25D8: stloc.s   V_20
// 		/* 0x00091516 16           */ IL_25DA: ldc.i4.0
// 		/* 0x00091517 1315         */ IL_25DB: stloc.s   V_21
// 		.try
// 		{
// 			/* 0x00091519 1114         */ IL_25DD: ldloc.s   V_20
// 			/* 0x0009151B 1215         */ IL_25DF: ldloca.s  V_21
// 			/* 0x0009151D 287502000A   */ IL_25E1: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 			/* 0x00091522 1182         */ IL_25E6: ldloc.s   V_130
// 			/* 0x00091524 02           */ IL_25E8: ldarg.0
// 			/* 0x00091525 7BDC0B0004   */ IL_25E9: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0009152A 1118         */ IL_25EE: ldloc.s   V_24
// 			/* 0x0009152C 8F5B010002   */ IL_25F0: ldelema   RemoteLogisticOrder
// 			/* 0x00091531 7B600C0004   */ IL_25F5: ldfld     int32 RemoteLogisticOrder::otherIndex
// 			/* 0x00091536 8F54010002   */ IL_25FA: ldelema   StationStore
// 			/* 0x0009153B 7B1A0C0004   */ IL_25FF: ldfld     int32 StationStore::itemId
// 			/* 0x00091540 02           */ IL_2604: ldarg.0
// 			/* 0x00091541 7BDC0B0004   */ IL_2605: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091546 1118         */ IL_260A: ldloc.s   V_24
// 			/* 0x00091548 8F5B010002   */ IL_260C: ldelema   RemoteLogisticOrder
// 			/* 0x0009154D 7B5C0C0004   */ IL_2611: ldfld     int32 RemoteLogisticOrder::itemId
// 			/* 0x00091552 3334         */ IL_2616: bne.un.s  IL_264C

// 			/* 0x00091554 1182         */ IL_2618: ldloc.s   V_130
// 			/* 0x00091556 02           */ IL_261A: ldarg.0
// 			/* 0x00091557 7BDC0B0004   */ IL_261B: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0009155C 1118         */ IL_2620: ldloc.s   V_24
// 			/* 0x0009155E 8F5B010002   */ IL_2622: ldelema   RemoteLogisticOrder
// 			/* 0x00091563 7B600C0004   */ IL_2627: ldfld     int32 RemoteLogisticOrder::otherIndex
// 			/* 0x00091568 8F54010002   */ IL_262C: ldelema   StationStore
// 			/* 0x0009156D 7C1E0C0004   */ IL_2631: ldflda    int32 StationStore::remoteOrder
// 			/* 0x00091572 25           */ IL_2636: dup
// 			/* 0x00091573 4A           */ IL_2637: ldind.i4
// 			/* 0x00091574 02           */ IL_2638: ldarg.0
// 			/* 0x00091575 7BDC0B0004   */ IL_2639: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x0009157A 1118         */ IL_263E: ldloc.s   V_24
// 			/* 0x0009157C 8F5B010002   */ IL_2640: ldelema   RemoteLogisticOrder
// 			/* 0x00091581 7B610C0004   */ IL_2645: ldfld     int32 RemoteLogisticOrder::otherOrdered
// 			/* 0x00091586 59           */ IL_264A: sub
// 			/* 0x00091587 54           */ IL_264B: stind.i4

// 			/* 0x00091588 DE0C         */ IL_264C: leave.s   IL_265A
// 		} // end .try
// 		finally
// 		{
// 			/* 0x0009158A 1115         */ IL_264E: ldloc.s   V_21
// 			/* 0x0009158C 2C07         */ IL_2650: brfalse.s IL_2659

// 			/* 0x0009158E 1114         */ IL_2652: ldloc.s   V_20
// 			/* 0x00091590 287602000A   */ IL_2654: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 			/* 0x00091595 DC           */ IL_2659: endfinally
// 		} // end handler

// 		/* 0x00091596 02           */ IL_265A: ldarg.0
// 		/* 0x00091597 7BDC0B0004   */ IL_265B: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x0009159C 1118         */ IL_2660: ldloc.s   V_24
// 		/* 0x0009159E 8F5B010002   */ IL_2662: ldelema   RemoteLogisticOrder
// 		/* 0x000915A3 28300A0006   */ IL_2667: call      instance void RemoteLogisticOrder::ClearOther()

// 		/* 0x000915A8 02           */ IL_266C: ldarg.0
// 		/* 0x000915A9 7BFC0B0004   */ IL_266D: ldfld     int32[] StationComponent::remotePairOffsets
// 		/* 0x000915AE 39F5060000   */ IL_2672: brfalse   IL_2D6C

// 		/* 0x000915B3 02           */ IL_2677: ldarg.0
// 		/* 0x000915B4 7BFC0B0004   */ IL_2678: ldfld     int32[] StationComponent::remotePairOffsets
// 		/* 0x000915B9 1C           */ IL_267D: ldc.i4.6
// 		/* 0x000915BA 94           */ IL_267E: ldelem.i4
// 		/* 0x000915BB 16           */ IL_267F: ldc.i4.0
// 		/* 0x000915BC 3EE7060000   */ IL_2680: ble       IL_2D6C

// 		/* 0x000915C1 02           */ IL_2685: ldarg.0
// 		/* 0x000915C2 7BF70B0004   */ IL_2686: ldfld     valuetype ERemoteRoutePriority StationComponent::routePriority
// 		/* 0x000915C7 18           */ IL_268B: ldc.i4.2
// 		/* 0x000915C8 3308         */ IL_268C: bne.un.s  IL_2696

// 		/* 0x000915CA 17           */ IL_268E: ldc.i4.1
// 		/* 0x000915CB 1387         */ IL_268F: stloc.s   V_135
// 		/* 0x000915CD 1B           */ IL_2691: ldc.i4.5
// 		/* 0x000915CE 1388         */ IL_2692: stloc.s   V_136
// 		/* 0x000915D0 2B20         */ IL_2694: br.s      IL_26B6

// 		/* 0x000915D2 02           */ IL_2696: ldarg.0
// 		/* 0x000915D3 7BF70B0004   */ IL_2697: ldfld     valuetype ERemoteRoutePriority StationComponent::routePriority
// 		/* 0x000915D8 19           */ IL_269C: ldc.i4.3
// 		/* 0x000915D9 2E09         */ IL_269D: beq.s     IL_26A8

// 		/* 0x000915DB 02           */ IL_269F: ldarg.0
// 		/* 0x000915DC 7BF70B0004   */ IL_26A0: ldfld     valuetype ERemoteRoutePriority StationComponent::routePriority
// 		/* 0x000915E1 1A           */ IL_26A5: ldc.i4.4
// 		/* 0x000915E2 3308         */ IL_26A6: bne.un.s  IL_26B0

// 		/* 0x000915E4 17           */ IL_26A8: ldc.i4.1
// 		/* 0x000915E5 1387         */ IL_26A9: stloc.s   V_135
// 		/* 0x000915E7 1A           */ IL_26AB: ldc.i4.4
// 		/* 0x000915E8 1388         */ IL_26AC: stloc.s   V_136
// 		/* 0x000915EA 2B06         */ IL_26AE: br.s      IL_26B6

// 		/* 0x000915EC 16           */ IL_26B0: ldc.i4.0
// 		/* 0x000915ED 1387         */ IL_26B1: stloc.s   V_135
// 		/* 0x000915EF 16           */ IL_26B3: ldc.i4.0
// 		/* 0x000915F0 1388         */ IL_26B4: stloc.s   V_136

// 		/* 0x000915F2 17           */ IL_26B6: ldc.i4.1
// 		/* 0x000915F3 1389         */ IL_26B7: stloc.s   V_137
// 		/* 0x000915F5 1187         */ IL_26B9: ldloc.s   V_135
// 		/* 0x000915F7 138A         */ IL_26BB: stloc.s   V_138
// 		/* 0x000915F9 38A0040000   */ IL_26BD: br        IL_2B62
// 		// loop start (head: IL_2B62)
// 			/* 0x000915FE 02           */ IL_26C2: ldarg.0
// 			/* 0x000915FF 7BFC0B0004   */ IL_26C3: ldfld     int32[] StationComponent::remotePairOffsets
// 			/* 0x00091604 118A         */ IL_26C8: ldloc.s   V_138
// 			/* 0x00091606 17           */ IL_26CA: ldc.i4.1
// 			/* 0x00091607 58           */ IL_26CB: add
// 			/* 0x00091608 94           */ IL_26CC: ldelem.i4
// 			/* 0x00091609 02           */ IL_26CD: ldarg.0
// 			/* 0x0009160A 7BFC0B0004   */ IL_26CE: ldfld     int32[] StationComponent::remotePairOffsets
// 			/* 0x0009160F 118A         */ IL_26D3: ldloc.s   V_138
// 			/* 0x00091611 94           */ IL_26D5: ldelem.i4
// 			/* 0x00091612 59           */ IL_26D6: sub
// 			/* 0x00091613 138B         */ IL_26D7: stloc.s   V_139
// 			/* 0x00091615 118B         */ IL_26D9: ldloc.s   V_139
// 			/* 0x00091617 16           */ IL_26DB: ldc.i4.0
// 			/* 0x00091618 3E7B040000   */ IL_26DC: ble       IL_2B5C

// 			/* 0x0009161D 02           */ IL_26E1: ldarg.0
// 			/* 0x0009161E 7BFC0B0004   */ IL_26E2: ldfld     int32[] StationComponent::remotePairOffsets
// 			/* 0x00091623 118A         */ IL_26E7: ldloc.s   V_138
// 			/* 0x00091625 94           */ IL_26E9: ldelem.i4
// 			/* 0x00091626 138C         */ IL_26EA: stloc.s   V_140
// 			/* 0x00091628 02           */ IL_26EC: ldarg.0
// 			/* 0x00091629 7BE40B0004   */ IL_26ED: ldfld     int32[] StationComponent::remotePairProcesses
// 			/* 0x0009162E 118A         */ IL_26F2: ldloc.s   V_138
// 			/* 0x00091630 02           */ IL_26F4: ldarg.0
// 			/* 0x00091631 7BE40B0004   */ IL_26F5: ldfld     int32[] StationComponent::remotePairProcesses
// 			/* 0x00091636 118A         */ IL_26FA: ldloc.s   V_138
// 			/* 0x00091638 94           */ IL_26FC: ldelem.i4
// 			/* 0x00091639 118B         */ IL_26FD: ldloc.s   V_139
// 			/* 0x0009163B 5D           */ IL_26FF: rem
// 			/* 0x0009163C 9E           */ IL_2700: stelem.i4
// 			/* 0x0009163D 02           */ IL_2701: ldarg.0
// 			/* 0x0009163E 7BE40B0004   */ IL_2702: ldfld     int32[] StationComponent::remotePairProcesses
// 			/* 0x00091643 118A         */ IL_2707: ldloc.s   V_138
// 			/* 0x00091645 94           */ IL_2709: ldelem.i4
// 			/* 0x00091646 138D         */ IL_270A: stloc.s   V_141
// 			/* 0x00091648 02           */ IL_270C: ldarg.0
// 			/* 0x00091649 7BE40B0004   */ IL_270D: ldfld     int32[] StationComponent::remotePairProcesses
// 			/* 0x0009164E 118A         */ IL_2712: ldloc.s   V_138
// 			/* 0x00091650 94           */ IL_2714: ldelem.i4
// 			/* 0x00091651 138E         */ IL_2715: stloc.s   V_142
// 			// loop start (head: IL_2717)
// 				/* 0x00091653 02           */ IL_2717: ldarg.0
// 				/* 0x00091654 7BFB0B0004   */ IL_2718: ldfld     valuetype SupplyDemandPair[] StationComponent::remotePairs
// 				/* 0x00091659 118E         */ IL_271D: ldloc.s   V_142
// 				/* 0x0009165B 118C         */ IL_271F: ldloc.s   V_140
// 				/* 0x0009165D 58           */ IL_2721: add
// 				/* 0x0009165E A383010002   */ IL_2722: ldelem    SupplyDemandPair
// 				/* 0x00091663 138F         */ IL_2727: stloc.s   V_143
// 				/* 0x00091665 118F         */ IL_2729: ldloc.s   V_143
// 				/* 0x00091667 7B270F0004   */ IL_272B: ldfld     int32 SupplyDemandPair::demandId
// 				/* 0x0009166C 02           */ IL_2730: ldarg.0
// 				/* 0x0009166D 7BC20B0004   */ IL_2731: ldfld     int32 StationComponent::gid
// 				/* 0x00091672 400D010000   */ IL_2736: bne.un    IL_2848

// 				/* 0x00091677 118F         */ IL_273B: ldloc.s   V_143
// 				/* 0x00091679 7B250F0004   */ IL_273D: ldfld     int32 SupplyDemandPair::supplyId
// 				/* 0x0009167E 1181         */ IL_2742: ldloc.s   V_129
// 				/* 0x00091680 7BC20B0004   */ IL_2744: ldfld     int32 StationComponent::gid
// 				/* 0x00091685 40FA000000   */ IL_2749: bne.un    IL_2848

// 				/* 0x0009168A 02           */ IL_274E: ldarg.0
// 				/* 0x0009168B 7BE10B0004   */ IL_274F: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 				/* 0x00091690 118F         */ IL_2754: ldloc.s   V_143
// 				/* 0x00091692 7B280F0004   */ IL_2756: ldfld     int32 SupplyDemandPair::demandIndex
// 				/* 0x00091697 8F55010002   */ IL_275B: ldelema   StationPriorityLock
// 				/* 0x0009169C 7B240C0004   */ IL_2760: ldfld     uint8 StationPriorityLock::priorityIndex
// 				/* 0x000916A1 118A         */ IL_2765: ldloc.s   V_138
// 				/* 0x000916A3 2F2C         */ IL_2767: bge.s     IL_2795

// 				/* 0x000916A5 02           */ IL_2769: ldarg.0
// 				/* 0x000916A6 7BE10B0004   */ IL_276A: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 				/* 0x000916AB 118F         */ IL_276F: ldloc.s   V_143
// 				/* 0x000916AD 7B280F0004   */ IL_2771: ldfld     int32 SupplyDemandPair::demandIndex
// 				/* 0x000916B2 8F55010002   */ IL_2776: ldelema   StationPriorityLock
// 				/* 0x000916B7 7B250C0004   */ IL_277B: ldfld     uint8 StationPriorityLock::lockTick
// 				/* 0x000916BC 16           */ IL_2780: ldc.i4.0
// 				/* 0x000916BD 3112         */ IL_2781: ble.s     IL_2795

// 				/* 0x000916BF 118E         */ IL_2783: ldloc.s   V_142
// 				/* 0x000916C1 17           */ IL_2785: ldc.i4.1
// 				/* 0x000916C2 58           */ IL_2786: add
// 				/* 0x000916C3 138E         */ IL_2787: stloc.s   V_142
// 				/* 0x000916C5 118E         */ IL_2789: ldloc.s   V_142
// 				/* 0x000916C7 118B         */ IL_278B: ldloc.s   V_139
// 				/* 0x000916C9 5D           */ IL_278D: rem
// 				/* 0x000916CA 138E         */ IL_278E: stloc.s   V_142
// 				/* 0x000916CC 38B7030000   */ IL_2790: br        IL_2B4C

// 				/* 0x000916D1 1181         */ IL_2795: ldloc.s   V_129
// 				/* 0x000916D3 7BE10B0004   */ IL_2797: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 				/* 0x000916D8 118F         */ IL_279C: ldloc.s   V_143
// 				/* 0x000916DA 7B260F0004   */ IL_279E: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x000916DF 8F55010002   */ IL_27A3: ldelema   StationPriorityLock
// 				/* 0x000916E4 7B240C0004   */ IL_27A8: ldfld     uint8 StationPriorityLock::priorityIndex
// 				/* 0x000916E9 118A         */ IL_27AD: ldloc.s   V_138
// 				/* 0x000916EB 2F2D         */ IL_27AF: bge.s     IL_27DE

// 				/* 0x000916ED 1181         */ IL_27B1: ldloc.s   V_129
// 				/* 0x000916EF 7BE10B0004   */ IL_27B3: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 				/* 0x000916F4 118F         */ IL_27B8: ldloc.s   V_143
// 				/* 0x000916F6 7B260F0004   */ IL_27BA: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x000916FB 8F55010002   */ IL_27BF: ldelema   StationPriorityLock
// 				/* 0x00091700 7B250C0004   */ IL_27C4: ldfld     uint8 StationPriorityLock::lockTick
// 				/* 0x00091705 16           */ IL_27C9: ldc.i4.0
// 				/* 0x00091706 3112         */ IL_27CA: ble.s     IL_27DE

// 				/* 0x00091708 118E         */ IL_27CC: ldloc.s   V_142
// 				/* 0x0009170A 17           */ IL_27CE: ldc.i4.1
// 				/* 0x0009170B 58           */ IL_27CF: add
// 				/* 0x0009170C 138E         */ IL_27D0: stloc.s   V_142
// 				/* 0x0009170E 118E         */ IL_27D2: ldloc.s   V_142
// 				/* 0x00091710 118B         */ IL_27D4: ldloc.s   V_139
// 				/* 0x00091712 5D           */ IL_27D6: rem
// 				/* 0x00091713 138E         */ IL_27D7: stloc.s   V_142
// 				/* 0x00091715 386E030000   */ IL_27D9: br        IL_2B4C

// 				/* 0x0009171A 02           */ IL_27DE: ldarg.0
// 				/* 0x0009171B 7BE00B0004   */ IL_27DF: ldfld     valuetype StationStore[] StationComponent::'storage'
// 				/* 0x00091720 1314         */ IL_27E4: stloc.s   V_20
// 				/* 0x00091722 16           */ IL_27E6: ldc.i4.0
// 				/* 0x00091723 1315         */ IL_27E7: stloc.s   V_21
// 				.try
// 				{
// 					/* 0x00091725 1114         */ IL_27E9: ldloc.s   V_20
// 					/* 0x00091727 1215         */ IL_27EB: ldloca.s  V_21
// 					/* 0x00091729 287502000A   */ IL_27ED: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 					/* 0x0009172E 02           */ IL_27F2: ldarg.0
// 					/* 0x0009172F 7BE00B0004   */ IL_27F3: ldfld     valuetype StationStore[] StationComponent::'storage'
// 					/* 0x00091734 118F         */ IL_27F8: ldloc.s   V_143
// 					/* 0x00091736 7B280F0004   */ IL_27FA: ldfld     int32 SupplyDemandPair::demandIndex
// 					/* 0x0009173B 8F54010002   */ IL_27FF: ldelema   StationStore
// 					/* 0x00091740 281B0A0006   */ IL_2804: call      instance int32 StationStore::get_remoteDemandCount()
// 					/* 0x00091745 0B           */ IL_2809: stloc.1
// 					/* 0x00091746 02           */ IL_280A: ldarg.0
// 					/* 0x00091747 7BE00B0004   */ IL_280B: ldfld     valuetype StationStore[] StationComponent::'storage'
// 					/* 0x0009174C 118F         */ IL_2810: ldloc.s   V_143
// 					/* 0x0009174E 7B280F0004   */ IL_2812: ldfld     int32 SupplyDemandPair::demandIndex
// 					/* 0x00091753 8F54010002   */ IL_2817: ldelema   StationStore
// 					/* 0x00091758 281D0A0006   */ IL_281C: call      instance int32 StationStore::get_totalDemandCount()
// 					/* 0x0009175D 0C           */ IL_2821: stloc.2
// 					/* 0x0009175E 02           */ IL_2822: ldarg.0
// 					/* 0x0009175F 7BE00B0004   */ IL_2823: ldfld     valuetype StationStore[] StationComponent::'storage'
// 					/* 0x00091764 118F         */ IL_2828: ldloc.s   V_143
// 					/* 0x00091766 7B280F0004   */ IL_282A: ldfld     int32 SupplyDemandPair::demandIndex
// 					/* 0x0009176B 8F54010002   */ IL_282F: ldelema   StationStore
// 					/* 0x00091770 7B1A0C0004   */ IL_2834: ldfld     int32 StationStore::itemId
// 					/* 0x00091775 0D           */ IL_2839: stloc.3
// 					/* 0x00091776 DE0C         */ IL_283A: leave.s   IL_2848
// 				} // end .try
// 				finally
// 				{
// 					/* 0x00091778 1115         */ IL_283C: ldloc.s   V_21
// 					/* 0x0009177A 2C07         */ IL_283E: brfalse.s IL_2847

// 					/* 0x0009177C 1114         */ IL_2840: ldloc.s   V_20
// 					/* 0x0009177E 287602000A   */ IL_2842: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 					/* 0x00091783 DC           */ IL_2847: endfinally
// 				} // end handler

// 				/* 0x00091784 118F         */ IL_2848: ldloc.s   V_143
// 				/* 0x00091786 7B270F0004   */ IL_284A: ldfld     int32 SupplyDemandPair::demandId
// 				/* 0x0009178B 02           */ IL_284F: ldarg.0
// 				/* 0x0009178C 7BC20B0004   */ IL_2850: ldfld     int32 StationComponent::gid
// 				/* 0x00091791 4082000000   */ IL_2855: bne.un    IL_28DC

// 				/* 0x00091796 118F         */ IL_285A: ldloc.s   V_143
// 				/* 0x00091798 7B250F0004   */ IL_285C: ldfld     int32 SupplyDemandPair::supplyId
// 				/* 0x0009179D 1181         */ IL_2861: ldloc.s   V_129
// 				/* 0x0009179F 7BC20B0004   */ IL_2863: ldfld     int32 StationComponent::gid
// 				/* 0x000917A4 3372         */ IL_2868: bne.un.s  IL_28DC

// 				/* 0x000917A6 1182         */ IL_286A: ldloc.s   V_130
// 				/* 0x000917A8 1314         */ IL_286C: stloc.s   V_20
// 				/* 0x000917AA 16           */ IL_286E: ldc.i4.0
// 				/* 0x000917AB 1315         */ IL_286F: stloc.s   V_21
// 				.try
// 				{
// 					/* 0x000917AD 1114         */ IL_2871: ldloc.s   V_20
// 					/* 0x000917AF 1215         */ IL_2873: ldloca.s  V_21
// 					/* 0x000917B1 287502000A   */ IL_2875: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 					/* 0x000917B6 1182         */ IL_287A: ldloc.s   V_130
// 					/* 0x000917B8 118F         */ IL_287C: ldloc.s   V_143
// 					/* 0x000917BA 7B260F0004   */ IL_287E: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000917BF 8F54010002   */ IL_2883: ldelema   StationStore
// 					/* 0x000917C4 7B1B0C0004   */ IL_2888: ldfld     int32 StationStore::count
// 					/* 0x000917C9 1304         */ IL_288D: stloc.s   V_4
// 					/* 0x000917CB 1182         */ IL_288F: ldloc.s   V_130
// 					/* 0x000917CD 118F         */ IL_2891: ldloc.s   V_143
// 					/* 0x000917CF 7B260F0004   */ IL_2893: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000917D4 8F54010002   */ IL_2898: ldelema   StationStore
// 					/* 0x000917D9 7B1C0C0004   */ IL_289D: ldfld     int32 StationStore::inc
// 					/* 0x000917DE 1305         */ IL_28A2: stloc.s   V_5
// 					/* 0x000917E0 1182         */ IL_28A4: ldloc.s   V_130
// 					/* 0x000917E2 118F         */ IL_28A6: ldloc.s   V_143
// 					/* 0x000917E4 7B260F0004   */ IL_28A8: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000917E9 8F54010002   */ IL_28AD: ldelema   StationStore
// 					/* 0x000917EE 281A0A0006   */ IL_28B2: call      instance int32 StationStore::get_remoteSupplyCount()
// 					/* 0x000917F3 1306         */ IL_28B7: stloc.s   V_6
// 					/* 0x000917F5 1182         */ IL_28B9: ldloc.s   V_130
// 					/* 0x000917F7 118F         */ IL_28BB: ldloc.s   V_143
// 					/* 0x000917F9 7B260F0004   */ IL_28BD: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000917FE 8F54010002   */ IL_28C2: ldelema   StationStore
// 					/* 0x00091803 281C0A0006   */ IL_28C7: call      instance int32 StationStore::get_totalSupplyCount()
// 					/* 0x00091808 1307         */ IL_28CC: stloc.s   V_7
// 					/* 0x0009180A DE0C         */ IL_28CE: leave.s   IL_28DC
// 				} // end .try
// 				finally
// 				{
// 					/* 0x0009180C 1115         */ IL_28D0: ldloc.s   V_21
// 					/* 0x0009180E 2C07         */ IL_28D2: brfalse.s IL_28DB

// 					/* 0x00091810 1114         */ IL_28D4: ldloc.s   V_20
// 					/* 0x00091812 287602000A   */ IL_28D6: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 					/* 0x00091817 DC           */ IL_28DB: endfinally
// 				} // end handler

// 				/* 0x00091818 118F         */ IL_28DC: ldloc.s   V_143
// 				/* 0x0009181A 7B270F0004   */ IL_28DE: ldfld     int32 SupplyDemandPair::demandId
// 				/* 0x0009181F 02           */ IL_28E3: ldarg.0
// 				/* 0x00091820 7BC20B0004   */ IL_28E4: ldfld     int32 StationComponent::gid
// 				/* 0x00091825 4051020000   */ IL_28E9: bne.un    IL_2B3F

// 				/* 0x0009182A 118F         */ IL_28EE: ldloc.s   V_143
// 				/* 0x0009182C 7B250F0004   */ IL_28F0: ldfld     int32 SupplyDemandPair::supplyId
// 				/* 0x00091831 1181         */ IL_28F5: ldloc.s   V_129
// 				/* 0x00091833 7BC20B0004   */ IL_28F7: ldfld     int32 StationComponent::gid
// 				/* 0x00091838 403E020000   */ IL_28FC: bne.un    IL_2B3F

// 				/* 0x0009183D 07           */ IL_2901: ldloc.1
// 				/* 0x0009183E 16           */ IL_2902: ldc.i4.0
// 				/* 0x0009183F 3E15020000   */ IL_2903: ble       IL_2B1D

// 				/* 0x00091844 08           */ IL_2908: ldloc.2
// 				/* 0x00091845 16           */ IL_2909: ldc.i4.0
// 				/* 0x00091846 3E0E020000   */ IL_290A: ble       IL_2B1D

// 				/* 0x0009184B 1104         */ IL_290F: ldloc.s   V_4
// 				/* 0x0009184D 0E05         */ IL_2911: ldarg.s   shipCarries
// 				/* 0x0009184F 3FF3010000   */ IL_2913: blt       IL_2B0B

// 				/* 0x00091854 1106         */ IL_2918: ldloc.s   V_6
// 				/* 0x00091856 0E05         */ IL_291A: ldarg.s   shipCarries
// 				/* 0x00091858 3FEA010000   */ IL_291C: blt       IL_2B0B

// 				/* 0x0009185D 1107         */ IL_2921: ldloc.s   V_7
// 				/* 0x0009185F 0E05         */ IL_2923: ldarg.s   shipCarries
// 				/* 0x00091861 3FE1010000   */ IL_2925: blt       IL_2B0B

// 				/* 0x00091866 0E05         */ IL_292A: ldarg.s   shipCarries
// 				/* 0x00091868 1104         */ IL_292C: ldloc.s   V_4
// 				/* 0x0009186A 3204         */ IL_292E: blt.s     IL_2934

// 				/* 0x0009186C 1104         */ IL_2930: ldloc.s   V_4
// 				/* 0x0009186E 2B02         */ IL_2932: br.s      IL_2936

// 				/* 0x00091870 0E05         */ IL_2934: ldarg.s   shipCarries

// 				/* 0x00091872 1390         */ IL_2936: stloc.s   V_144
// 				/* 0x00091874 1104         */ IL_2938: ldloc.s   V_4
// 				/* 0x00091876 1391         */ IL_293A: stloc.s   V_145
// 				/* 0x00091878 1105         */ IL_293C: ldloc.s   V_5
// 				/* 0x0009187A 1392         */ IL_293E: stloc.s   V_146
// 				/* 0x0009187C 02           */ IL_2940: ldarg.0
// 				/* 0x0009187D 1291         */ IL_2941: ldloca.s  V_145
// 				/* 0x0009187F 1292         */ IL_2943: ldloca.s  V_146
// 				/* 0x00091881 1190         */ IL_2945: ldloc.s   V_144
// 				/* 0x00091883 28110A0006   */ IL_2947: call      instance int32 StationComponent::split_inc(int32&, int32&, int32)
// 				/* 0x00091888 1393         */ IL_294C: stloc.s   V_147
// 				/* 0x0009188A 1119         */ IL_294E: ldloc.s   V_25
// 				/* 0x0009188C 02           */ IL_2950: ldarg.0
// 				/* 0x0009188D 7BDC0B0004   */ IL_2951: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x00091892 1118         */ IL_2956: ldloc.s   V_24
// 				/* 0x00091894 8F5B010002   */ IL_2958: ldelema   RemoteLogisticOrder
// 				/* 0x00091899 09           */ IL_295D: ldloc.3
// 				/* 0x0009189A 25           */ IL_295E: dup
// 				/* 0x0009189B 1394         */ IL_295F: stloc.s   V_148
// 				/* 0x0009189D 7D5C0C0004   */ IL_2961: stfld     int32 RemoteLogisticOrder::itemId
// 				/* 0x000918A2 1194         */ IL_2966: ldloc.s   V_148
// 				/* 0x000918A4 7D400C0004   */ IL_2968: stfld     int32 ShipData::itemId
// 				/* 0x000918A9 1119         */ IL_296D: ldloc.s   V_25
// 				/* 0x000918AB 1190         */ IL_296F: ldloc.s   V_144
// 				/* 0x000918AD 7D410C0004   */ IL_2971: stfld     int32 ShipData::itemCount
// 				/* 0x000918B2 1119         */ IL_2976: ldloc.s   V_25
// 				/* 0x000918B4 1193         */ IL_2978: ldloc.s   V_147
// 				/* 0x000918B6 7D420C0004   */ IL_297A: stfld     int32 ShipData::inc
// 				/* 0x000918BB 1182         */ IL_297F: ldloc.s   V_130
// 				/* 0x000918BD 1314         */ IL_2981: stloc.s   V_20
// 				/* 0x000918BF 16           */ IL_2983: ldc.i4.0
// 				/* 0x000918C0 1315         */ IL_2984: stloc.s   V_21
// 				.try
// 				{
// 					/* 0x000918C2 1114         */ IL_2986: ldloc.s   V_20
// 					/* 0x000918C4 1215         */ IL_2988: ldloca.s  V_21
// 					/* 0x000918C6 287502000A   */ IL_298A: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 					/* 0x000918CB 1182         */ IL_298F: ldloc.s   V_130
// 					/* 0x000918CD 118F         */ IL_2991: ldloc.s   V_143
// 					/* 0x000918CF 7B260F0004   */ IL_2993: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000918D4 8F54010002   */ IL_2998: ldelema   StationStore
// 					/* 0x000918D9 7C1B0C0004   */ IL_299D: ldflda    int32 StationStore::count
// 					/* 0x000918DE 25           */ IL_29A2: dup
// 					/* 0x000918DF 4A           */ IL_29A3: ldind.i4
// 					/* 0x000918E0 1190         */ IL_29A4: ldloc.s   V_144
// 					/* 0x000918E2 59           */ IL_29A6: sub
// 					/* 0x000918E3 54           */ IL_29A7: stind.i4
// 					/* 0x000918E4 1182         */ IL_29A8: ldloc.s   V_130
// 					/* 0x000918E6 118F         */ IL_29AA: ldloc.s   V_143
// 					/* 0x000918E8 7B260F0004   */ IL_29AC: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x000918ED 8F54010002   */ IL_29B1: ldelema   StationStore
// 					/* 0x000918F2 7C1C0C0004   */ IL_29B6: ldflda    int32 StationStore::inc
// 					/* 0x000918F7 25           */ IL_29BB: dup
// 					/* 0x000918F8 4A           */ IL_29BC: ldind.i4
// 					/* 0x000918F9 1193         */ IL_29BD: ldloc.s   V_147
// 					/* 0x000918FB 59           */ IL_29BF: sub
// 					/* 0x000918FC 54           */ IL_29C0: stind.i4
// 					/* 0x000918FD 1184         */ IL_29C1: ldloc.s   V_132
// 					/* 0x000918FF 1181         */ IL_29C3: ldloc.s   V_129
// 					/* 0x00091901 7BC40B0004   */ IL_29C5: ldfld     int32 StationComponent::planetId
// 					/* 0x00091906 1182         */ IL_29CA: ldloc.s   V_130
// 					/* 0x00091908 118F         */ IL_29CC: ldloc.s   V_143
// 					/* 0x0009190A 7B260F0004   */ IL_29CE: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x0009190F 8F54010002   */ IL_29D3: ldelema   StationStore
// 					/* 0x00091914 7B1A0C0004   */ IL_29D8: ldfld     int32 StationStore::itemId
// 					/* 0x00091919 1190         */ IL_29DD: ldloc.s   V_144
// 					/* 0x0009191B 6FB51A0006   */ IL_29DF: callvirt  instance void TrafficStatistics::RegisterPlanetOutputStat(int32, int32, int32)
// 					/* 0x00091920 1185         */ IL_29E4: ldloc.s   V_133
// 					/* 0x00091922 1186         */ IL_29E6: ldloc.s   V_134
// 					/* 0x00091924 2E20         */ IL_29E8: beq.s     IL_2A0A

// 					/* 0x00091926 1184         */ IL_29EA: ldloc.s   V_132
// 					/* 0x00091928 1185         */ IL_29EC: ldloc.s   V_133
// 					/* 0x0009192A 1182         */ IL_29EE: ldloc.s   V_130
// 					/* 0x0009192C 118F         */ IL_29F0: ldloc.s   V_143
// 					/* 0x0009192E 7B260F0004   */ IL_29F2: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x00091933 8F54010002   */ IL_29F7: ldelema   StationStore
// 					/* 0x00091938 7B1A0C0004   */ IL_29FC: ldfld     int32 StationStore::itemId
// 					/* 0x0009193D 1190         */ IL_2A01: ldloc.s   V_144
// 					/* 0x0009193F 6FB21A0006   */ IL_2A03: callvirt  instance void TrafficStatistics::RegisterStarOutputStat(int32, int32, int32)
// 					/* 0x00091944 DE2C         */ IL_2A08: leave.s   IL_2A36

// 					/* 0x00091946 1184         */ IL_2A0A: ldloc.s   V_132
// 					/* 0x00091948 1185         */ IL_2A0C: ldloc.s   V_133
// 					/* 0x0009194A 1182         */ IL_2A0E: ldloc.s   V_130
// 					/* 0x0009194C 118F         */ IL_2A10: ldloc.s   V_143
// 					/* 0x0009194E 7B260F0004   */ IL_2A12: ldfld     int32 SupplyDemandPair::supplyIndex
// 					/* 0x00091953 8F54010002   */ IL_2A17: ldelema   StationStore
// 					/* 0x00091958 7B1A0C0004   */ IL_2A1C: ldfld     int32 StationStore::itemId
// 					/* 0x0009195D 1190         */ IL_2A21: ldloc.s   V_144
// 					/* 0x0009195F 6FB31A0006   */ IL_2A23: callvirt  instance void TrafficStatistics::RegisterStarInternalStat(int32, int32, int32)
// 					/* 0x00091964 DE0C         */ IL_2A28: leave.s   IL_2A36
// 				} // end .try
// 				finally
// 				{
// 					/* 0x00091966 1115         */ IL_2A2A: ldloc.s   V_21
// 					/* 0x00091968 2C07         */ IL_2A2C: brfalse.s IL_2A35

// 					/* 0x0009196A 1114         */ IL_2A2E: ldloc.s   V_20
// 					/* 0x0009196C 287602000A   */ IL_2A30: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 					/* 0x00091971 DC           */ IL_2A35: endfinally
// 				} // end handler

// 				/* 0x00091972 02           */ IL_2A36: ldarg.0
// 				/* 0x00091973 7BDC0B0004   */ IL_2A37: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x00091978 1118         */ IL_2A3C: ldloc.s   V_24
// 				/* 0x0009197A 8F5B010002   */ IL_2A3E: ldelema   RemoteLogisticOrder
// 				/* 0x0009197F 1181         */ IL_2A43: ldloc.s   V_129
// 				/* 0x00091981 7BC20B0004   */ IL_2A45: ldfld     int32 StationComponent::gid
// 				/* 0x00091986 7D5F0C0004   */ IL_2A4A: stfld     int32 RemoteLogisticOrder::otherStationGId
// 				/* 0x0009198B 02           */ IL_2A4F: ldarg.0
// 				/* 0x0009198C 7BDC0B0004   */ IL_2A50: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x00091991 1118         */ IL_2A55: ldloc.s   V_24
// 				/* 0x00091993 8F5B010002   */ IL_2A57: ldelema   RemoteLogisticOrder
// 				/* 0x00091998 118F         */ IL_2A5C: ldloc.s   V_143
// 				/* 0x0009199A 7B280F0004   */ IL_2A5E: ldfld     int32 SupplyDemandPair::demandIndex
// 				/* 0x0009199F 7D5D0C0004   */ IL_2A63: stfld     int32 RemoteLogisticOrder::thisIndex
// 				/* 0x000919A4 02           */ IL_2A68: ldarg.0
// 				/* 0x000919A5 7BDC0B0004   */ IL_2A69: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x000919AA 1118         */ IL_2A6E: ldloc.s   V_24
// 				/* 0x000919AC 8F5B010002   */ IL_2A70: ldelema   RemoteLogisticOrder
// 				/* 0x000919B1 118F         */ IL_2A75: ldloc.s   V_143
// 				/* 0x000919B3 7B260F0004   */ IL_2A77: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x000919B8 7D600C0004   */ IL_2A7C: stfld     int32 RemoteLogisticOrder::otherIndex
// 				/* 0x000919BD 02           */ IL_2A81: ldarg.0
// 				/* 0x000919BE 7BDC0B0004   */ IL_2A82: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x000919C3 1118         */ IL_2A87: ldloc.s   V_24
// 				/* 0x000919C5 8F5B010002   */ IL_2A89: ldelema   RemoteLogisticOrder
// 				/* 0x000919CA 1190         */ IL_2A8E: ldloc.s   V_144
// 				/* 0x000919CC 7D5E0C0004   */ IL_2A90: stfld     int32 RemoteLogisticOrder::thisOrdered
// 				/* 0x000919D1 02           */ IL_2A95: ldarg.0
// 				/* 0x000919D2 7BDC0B0004   */ IL_2A96: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 				/* 0x000919D7 1118         */ IL_2A9B: ldloc.s   V_24
// 				/* 0x000919D9 8F5B010002   */ IL_2A9D: ldelema   RemoteLogisticOrder
// 				/* 0x000919DE 16           */ IL_2AA2: ldc.i4.0
// 				/* 0x000919DF 7D610C0004   */ IL_2AA3: stfld     int32 RemoteLogisticOrder::otherOrdered
// 				/* 0x000919E4 02           */ IL_2AA8: ldarg.0
// 				/* 0x000919E5 7BE00B0004   */ IL_2AA9: ldfld     valuetype StationStore[] StationComponent::'storage'
// 				/* 0x000919EA 1314         */ IL_2AAE: stloc.s   V_20
// 				/* 0x000919EC 16           */ IL_2AB0: ldc.i4.0
// 				/* 0x000919ED 1315         */ IL_2AB1: stloc.s   V_21
// 				.try
// 				{
// 					/* 0x000919EF 1114         */ IL_2AB3: ldloc.s   V_20
// 					/* 0x000919F1 1215         */ IL_2AB5: ldloca.s  V_21
// 					/* 0x000919F3 287502000A   */ IL_2AB7: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 					/* 0x000919F8 02           */ IL_2ABC: ldarg.0
// 					/* 0x000919F9 7BE00B0004   */ IL_2ABD: ldfld     valuetype StationStore[] StationComponent::'storage'
// 					/* 0x000919FE 118F         */ IL_2AC2: ldloc.s   V_143
// 					/* 0x00091A00 7B280F0004   */ IL_2AC4: ldfld     int32 SupplyDemandPair::demandIndex
// 					/* 0x00091A05 8F54010002   */ IL_2AC9: ldelema   StationStore
// 					/* 0x00091A0A 7C1E0C0004   */ IL_2ACE: ldflda    int32 StationStore::remoteOrder
// 					/* 0x00091A0F 25           */ IL_2AD3: dup
// 					/* 0x00091A10 4A           */ IL_2AD4: ldind.i4
// 					/* 0x00091A11 1190         */ IL_2AD5: ldloc.s   V_144
// 					/* 0x00091A13 58           */ IL_2AD7: add
// 					/* 0x00091A14 54           */ IL_2AD8: stind.i4
// 					/* 0x00091A15 DE0C         */ IL_2AD9: leave.s   IL_2AE7
// 				} // end .try
// 				finally
// 				{
// 					/* 0x00091A17 1115         */ IL_2ADB: ldloc.s   V_21
// 					/* 0x00091A19 2C07         */ IL_2ADD: brfalse.s IL_2AE6

// 					/* 0x00091A1B 1114         */ IL_2ADF: ldloc.s   V_20
// 					/* 0x00091A1D 287602000A   */ IL_2AE1: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 					/* 0x00091A22 DC           */ IL_2AE6: endfinally
// 				} // end handler

// 				/* 0x00091A23 02           */ IL_2AE7: ldarg.0
// 				/* 0x00091A24 118F         */ IL_2AE8: ldloc.s   V_143
// 				/* 0x00091A26 7B280F0004   */ IL_2AEA: ldfld     int32 SupplyDemandPair::demandIndex
// 				/* 0x00091A2B 118A         */ IL_2AEF: ldloc.s   V_138
// 				/* 0x00091A2D 28030A0006   */ IL_2AF1: call      instance void StationComponent::SetPriorityLock(int32, int32)
// 				/* 0x00091A32 1181         */ IL_2AF6: ldloc.s   V_129
// 				/* 0x00091A34 118F         */ IL_2AF8: ldloc.s   V_143
// 				/* 0x00091A36 7B260F0004   */ IL_2AFA: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x00091A3B 118A         */ IL_2AFF: ldloc.s   V_138
// 				/* 0x00091A3D 6F030A0006   */ IL_2B01: callvirt  instance void StationComponent::SetPriorityLock(int32, int32)
// 				/* 0x00091A42 16           */ IL_2B06: ldc.i4.0
// 				/* 0x00091A43 1389         */ IL_2B07: stloc.s   V_137
// 				/* 0x00091A45 2B4A         */ IL_2B09: br.s      IL_2B55

// 				/* 0x00091A47 1181         */ IL_2B0B: ldloc.s   V_129
// 				/* 0x00091A49 118F         */ IL_2B0D: ldloc.s   V_143
// 				/* 0x00091A4B 7B260F0004   */ IL_2B0F: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x00091A50 118A         */ IL_2B14: ldloc.s   V_138
// 				/* 0x00091A52 6F030A0006   */ IL_2B16: callvirt  instance void StationComponent::SetPriorityLock(int32, int32)
// 				/* 0x00091A57 2B22         */ IL_2B1B: br.s      IL_2B3F

// 				/* 0x00091A59 1104         */ IL_2B1D: ldloc.s   V_4
// 				/* 0x00091A5B 0E05         */ IL_2B1F: ldarg.s   shipCarries
// 				/* 0x00091A5D 310C         */ IL_2B21: ble.s     IL_2B2F

// 				/* 0x00091A5F 1106         */ IL_2B23: ldloc.s   V_6
// 				/* 0x00091A61 0E05         */ IL_2B25: ldarg.s   shipCarries
// 				/* 0x00091A63 3106         */ IL_2B27: ble.s     IL_2B2F

// 				/* 0x00091A65 1107         */ IL_2B29: ldloc.s   V_7
// 				/* 0x00091A67 0E05         */ IL_2B2B: ldarg.s   shipCarries
// 				/* 0x00091A69 3010         */ IL_2B2D: bgt.s     IL_2B3F

// 				/* 0x00091A6B 1181         */ IL_2B2F: ldloc.s   V_129
// 				/* 0x00091A6D 118F         */ IL_2B31: ldloc.s   V_143
// 				/* 0x00091A6F 7B260F0004   */ IL_2B33: ldfld     int32 SupplyDemandPair::supplyIndex
// 				/* 0x00091A74 118A         */ IL_2B38: ldloc.s   V_138
// 				/* 0x00091A76 6F030A0006   */ IL_2B3A: callvirt  instance void StationComponent::SetPriorityLock(int32, int32)

// 				/* 0x00091A7B 118E         */ IL_2B3F: ldloc.s   V_142
// 				/* 0x00091A7D 17           */ IL_2B41: ldc.i4.1
// 				/* 0x00091A7E 58           */ IL_2B42: add
// 				/* 0x00091A7F 138E         */ IL_2B43: stloc.s   V_142
// 				/* 0x00091A81 118E         */ IL_2B45: ldloc.s   V_142
// 				/* 0x00091A83 118B         */ IL_2B47: ldloc.s   V_139
// 				/* 0x00091A85 5D           */ IL_2B49: rem
// 				/* 0x00091A86 138E         */ IL_2B4A: stloc.s   V_142

// 				/* 0x00091A88 118D         */ IL_2B4C: ldloc.s   V_141
// 				/* 0x00091A8A 118E         */ IL_2B4E: ldloc.s   V_142
// 				/* 0x00091A8C 40C2FBFFFF   */ IL_2B50: bne.un    IL_2717
// 			// end loop

// 			/* 0x00091A91 1189         */ IL_2B55: ldloc.s   V_137
// 			/* 0x00091A93 3910020000   */ IL_2B57: brfalse   IL_2D6C

// 			/* 0x00091A98 118A         */ IL_2B5C: ldloc.s   V_138
// 			/* 0x00091A9A 17           */ IL_2B5E: ldc.i4.1
// 			/* 0x00091A9B 58           */ IL_2B5F: add
// 			/* 0x00091A9C 138A         */ IL_2B60: stloc.s   V_138

// 			/* 0x00091A9E 118A         */ IL_2B62: ldloc.s   V_138
// 			/* 0x00091AA0 1188         */ IL_2B64: ldloc.s   V_136
// 			/* 0x00091AA2 3E57FBFFFF   */ IL_2B66: ble       IL_26C2
// 		// end loop

// 		/* 0x00091AA7 38FC010000   */ IL_2B6B: br        IL_2D6C

// 		/* 0x00091AAC 1119         */ IL_2B70: ldloc.s   V_25
// 		/* 0x00091AAE 7B400C0004   */ IL_2B72: ldfld     int32 ShipData::itemId
// 		/* 0x00091AB3 1395         */ IL_2B77: stloc.s   V_149
// 		/* 0x00091AB5 0E05         */ IL_2B79: ldarg.s   shipCarries
// 		/* 0x00091AB7 1396         */ IL_2B7B: stloc.s   V_150
// 		/* 0x00091AB9 1181         */ IL_2B7D: ldloc.s   V_129
// 		/* 0x00091ABB 1295         */ IL_2B7F: ldloca.s  V_149
// 		/* 0x00091ABD 1296         */ IL_2B81: ldloca.s  V_150
// 		/* 0x00091ABF 1297         */ IL_2B83: ldloca.s  V_151
// 		/* 0x00091AC1 6F0F0A0006   */ IL_2B85: callvirt  instance void StationComponent::TakeItem(int32&, int32&, int32&)
// 		/* 0x00091AC6 03           */ IL_2B8A: ldarg.1
// 		/* 0x00091AC7 6F9C170006   */ IL_2B8B: callvirt  instance class GameData PlanetFactory::get_gameData()
// 		/* 0x00091ACC 7BE51A0004   */ IL_2B90: ldfld     class GameStatData GameData::statistics
// 		/* 0x00091AD1 7B891C0004   */ IL_2B95: ldfld     class TrafficStatistics GameStatData::traffic
// 		/* 0x00091AD6 1398         */ IL_2B9A: stloc.s   V_152
// 		/* 0x00091AD8 1198         */ IL_2B9C: ldloc.s   V_152
// 		/* 0x00091ADA 1181         */ IL_2B9E: ldloc.s   V_129
// 		/* 0x00091ADC 7BC40B0004   */ IL_2BA0: ldfld     int32 StationComponent::planetId
// 		/* 0x00091AE1 1195         */ IL_2BA5: ldloc.s   V_149
// 		/* 0x00091AE3 1196         */ IL_2BA7: ldloc.s   V_150
// 		/* 0x00091AE5 6FB51A0006   */ IL_2BA9: callvirt  instance void TrafficStatistics::RegisterPlanetOutputStat(int32, int32, int32)
// 		/* 0x00091AEA 1181         */ IL_2BAE: ldloc.s   V_129
// 		/* 0x00091AEC 7BC40B0004   */ IL_2BB0: ldfld     int32 StationComponent::planetId
// 		/* 0x00091AF1 1F64         */ IL_2BB5: ldc.i4.s  100
// 		/* 0x00091AF3 5B           */ IL_2BB7: div
// 		/* 0x00091AF4 1399         */ IL_2BB8: stloc.s   V_153
// 		/* 0x00091AF6 02           */ IL_2BBA: ldarg.0
// 		/* 0x00091AF7 7BC40B0004   */ IL_2BBB: ldfld     int32 StationComponent::planetId
// 		/* 0x00091AFC 1F64         */ IL_2BC0: ldc.i4.s  100
// 		/* 0x00091AFE 5B           */ IL_2BC2: div
// 		/* 0x00091AFF 139A         */ IL_2BC3: stloc.s   V_154
// 		/* 0x00091B01 1199         */ IL_2BC5: ldloc.s   V_153
// 		/* 0x00091B03 119A         */ IL_2BC7: ldloc.s   V_154
// 		/* 0x00091B05 2E0F         */ IL_2BC9: beq.s     IL_2BDA

// 		/* 0x00091B07 1198         */ IL_2BCB: ldloc.s   V_152
// 		/* 0x00091B09 1199         */ IL_2BCD: ldloc.s   V_153
// 		/* 0x00091B0B 1195         */ IL_2BCF: ldloc.s   V_149
// 		/* 0x00091B0D 1196         */ IL_2BD1: ldloc.s   V_150
// 		/* 0x00091B0F 6FB21A0006   */ IL_2BD3: callvirt  instance void TrafficStatistics::RegisterStarOutputStat(int32, int32, int32)
// 		/* 0x00091B14 2B0D         */ IL_2BD8: br.s      IL_2BE7

// 		/* 0x00091B16 1198         */ IL_2BDA: ldloc.s   V_152
// 		/* 0x00091B18 1199         */ IL_2BDC: ldloc.s   V_153
// 		/* 0x00091B1A 1195         */ IL_2BDE: ldloc.s   V_149
// 		/* 0x00091B1C 1196         */ IL_2BE0: ldloc.s   V_150
// 		/* 0x00091B1E 6FB31A0006   */ IL_2BE2: callvirt  instance void TrafficStatistics::RegisterStarInternalStat(int32, int32, int32)

// 		/* 0x00091B23 1119         */ IL_2BE7: ldloc.s   V_25
// 		/* 0x00091B25 1196         */ IL_2BE9: ldloc.s   V_150
// 		/* 0x00091B27 7D410C0004   */ IL_2BEB: stfld     int32 ShipData::itemCount
// 		/* 0x00091B2C 1119         */ IL_2BF0: ldloc.s   V_25
// 		/* 0x00091B2E 1197         */ IL_2BF2: ldloc.s   V_151
// 		/* 0x00091B30 7D420C0004   */ IL_2BF4: stfld     int32 ShipData::inc
// 		/* 0x00091B35 02           */ IL_2BF9: ldarg.0
// 		/* 0x00091B36 7BDC0B0004   */ IL_2BFA: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x00091B3B 1118         */ IL_2BFF: ldloc.s   V_24
// 		/* 0x00091B3D 8F5B010002   */ IL_2C01: ldelema   RemoteLogisticOrder
// 		/* 0x00091B42 7B5F0C0004   */ IL_2C06: ldfld     int32 RemoteLogisticOrder::otherStationGId
// 		/* 0x00091B47 16           */ IL_2C0B: ldc.i4.0
// 		/* 0x00091B48 3E96000000   */ IL_2C0C: ble       IL_2CA7

// 		/* 0x00091B4D 1182         */ IL_2C11: ldloc.s   V_130
// 		/* 0x00091B4F 1314         */ IL_2C13: stloc.s   V_20
// 		/* 0x00091B51 16           */ IL_2C15: ldc.i4.0
// 		/* 0x00091B52 1315         */ IL_2C16: stloc.s   V_21
// 		.try
// 		{
// 			/* 0x00091B54 1114         */ IL_2C18: ldloc.s   V_20
// 			/* 0x00091B56 1215         */ IL_2C1A: ldloca.s  V_21
// 			/* 0x00091B58 287502000A   */ IL_2C1C: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 			/* 0x00091B5D 1182         */ IL_2C21: ldloc.s   V_130
// 			/* 0x00091B5F 02           */ IL_2C23: ldarg.0
// 			/* 0x00091B60 7BDC0B0004   */ IL_2C24: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091B65 1118         */ IL_2C29: ldloc.s   V_24
// 			/* 0x00091B67 8F5B010002   */ IL_2C2B: ldelema   RemoteLogisticOrder
// 			/* 0x00091B6C 7B600C0004   */ IL_2C30: ldfld     int32 RemoteLogisticOrder::otherIndex
// 			/* 0x00091B71 8F54010002   */ IL_2C35: ldelema   StationStore
// 			/* 0x00091B76 7B1A0C0004   */ IL_2C3A: ldfld     int32 StationStore::itemId
// 			/* 0x00091B7B 02           */ IL_2C3F: ldarg.0
// 			/* 0x00091B7C 7BDC0B0004   */ IL_2C40: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091B81 1118         */ IL_2C45: ldloc.s   V_24
// 			/* 0x00091B83 8F5B010002   */ IL_2C47: ldelema   RemoteLogisticOrder
// 			/* 0x00091B88 7B5C0C0004   */ IL_2C4C: ldfld     int32 RemoteLogisticOrder::itemId
// 			/* 0x00091B8D 3334         */ IL_2C51: bne.un.s  IL_2C87

// 			/* 0x00091B8F 1182         */ IL_2C53: ldloc.s   V_130
// 			/* 0x00091B91 02           */ IL_2C55: ldarg.0
// 			/* 0x00091B92 7BDC0B0004   */ IL_2C56: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091B97 1118         */ IL_2C5B: ldloc.s   V_24
// 			/* 0x00091B99 8F5B010002   */ IL_2C5D: ldelema   RemoteLogisticOrder
// 			/* 0x00091B9E 7B600C0004   */ IL_2C62: ldfld     int32 RemoteLogisticOrder::otherIndex
// 			/* 0x00091BA3 8F54010002   */ IL_2C67: ldelema   StationStore
// 			/* 0x00091BA8 7C1E0C0004   */ IL_2C6C: ldflda    int32 StationStore::remoteOrder
// 			/* 0x00091BAD 25           */ IL_2C71: dup
// 			/* 0x00091BAE 4A           */ IL_2C72: ldind.i4
// 			/* 0x00091BAF 02           */ IL_2C73: ldarg.0
// 			/* 0x00091BB0 7BDC0B0004   */ IL_2C74: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091BB5 1118         */ IL_2C79: ldloc.s   V_24
// 			/* 0x00091BB7 8F5B010002   */ IL_2C7B: ldelema   RemoteLogisticOrder
// 			/* 0x00091BBC 7B610C0004   */ IL_2C80: ldfld     int32 RemoteLogisticOrder::otherOrdered
// 			/* 0x00091BC1 59           */ IL_2C85: sub
// 			/* 0x00091BC2 54           */ IL_2C86: stind.i4

// 			/* 0x00091BC3 DE0C         */ IL_2C87: leave.s   IL_2C95
// 		} // end .try
// 		finally
// 		{
// 			/* 0x00091BC5 1115         */ IL_2C89: ldloc.s   V_21
// 			/* 0x00091BC7 2C07         */ IL_2C8B: brfalse.s IL_2C94

// 			/* 0x00091BC9 1114         */ IL_2C8D: ldloc.s   V_20
// 			/* 0x00091BCB 287602000A   */ IL_2C8F: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 			/* 0x00091BD0 DC           */ IL_2C94: endfinally
// 		} // end handler

// 		/* 0x00091BD1 02           */ IL_2C95: ldarg.0
// 		/* 0x00091BD2 7BDC0B0004   */ IL_2C96: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 		/* 0x00091BD7 1118         */ IL_2C9B: ldloc.s   V_24
// 		/* 0x00091BD9 8F5B010002   */ IL_2C9D: ldelema   RemoteLogisticOrder
// 		/* 0x00091BDE 28300A0006   */ IL_2CA2: call      instance void RemoteLogisticOrder::ClearOther()

// 		/* 0x00091BE3 02           */ IL_2CA7: ldarg.0
// 		/* 0x00091BE4 7BE00B0004   */ IL_2CA8: ldfld     valuetype StationStore[] StationComponent::'storage'
// 		/* 0x00091BE9 1314         */ IL_2CAD: stloc.s   V_20
// 		/* 0x00091BEB 16           */ IL_2CAF: ldc.i4.0
// 		/* 0x00091BEC 1315         */ IL_2CB0: stloc.s   V_21
// 		.try
// 		{
// 			/* 0x00091BEE 1114         */ IL_2CB2: ldloc.s   V_20
// 			/* 0x00091BF0 1215         */ IL_2CB4: ldloca.s  V_21
// 			/* 0x00091BF2 287502000A   */ IL_2CB6: call      void [netstandard]System.Threading.Monitor::Enter(object, bool&)
// 			/* 0x00091BF7 02           */ IL_2CBB: ldarg.0
// 			/* 0x00091BF8 7BE00B0004   */ IL_2CBC: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x00091BFD 02           */ IL_2CC1: ldarg.0
// 			/* 0x00091BFE 7BDC0B0004   */ IL_2CC2: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C03 1118         */ IL_2CC7: ldloc.s   V_24
// 			/* 0x00091C05 8F5B010002   */ IL_2CC9: ldelema   RemoteLogisticOrder
// 			/* 0x00091C0A 7B5D0C0004   */ IL_2CCE: ldfld     int32 RemoteLogisticOrder::thisIndex
// 			/* 0x00091C0F 8F54010002   */ IL_2CD3: ldelema   StationStore
// 			/* 0x00091C14 7B1A0C0004   */ IL_2CD8: ldfld     int32 StationStore::itemId
// 			/* 0x00091C19 02           */ IL_2CDD: ldarg.0
// 			/* 0x00091C1A 7BDC0B0004   */ IL_2CDE: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C1F 1118         */ IL_2CE3: ldloc.s   V_24
// 			/* 0x00091C21 8F5B010002   */ IL_2CE5: ldelema   RemoteLogisticOrder
// 			/* 0x00091C26 7B5C0C0004   */ IL_2CEA: ldfld     int32 RemoteLogisticOrder::itemId
// 			/* 0x00091C2B 336D         */ IL_2CEF: bne.un.s  IL_2D5E

// 			/* 0x00091C2D 02           */ IL_2CF1: ldarg.0
// 			/* 0x00091C2E 7BDC0B0004   */ IL_2CF2: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C33 1118         */ IL_2CF7: ldloc.s   V_24
// 			/* 0x00091C35 8F5B010002   */ IL_2CF9: ldelema   RemoteLogisticOrder
// 			/* 0x00091C3A 7B5E0C0004   */ IL_2CFE: ldfld     int32 RemoteLogisticOrder::thisOrdered
// 			/* 0x00091C3F 1196         */ IL_2D03: ldloc.s   V_150
// 			/* 0x00091C41 2E57         */ IL_2D05: beq.s     IL_2D5E

// 			/* 0x00091C43 1196         */ IL_2D07: ldloc.s   V_150
// 			/* 0x00091C45 02           */ IL_2D09: ldarg.0
// 			/* 0x00091C46 7BDC0B0004   */ IL_2D0A: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C4B 1118         */ IL_2D0F: ldloc.s   V_24
// 			/* 0x00091C4D 8F5B010002   */ IL_2D11: ldelema   RemoteLogisticOrder
// 			/* 0x00091C52 7B5E0C0004   */ IL_2D16: ldfld     int32 RemoteLogisticOrder::thisOrdered
// 			/* 0x00091C57 59           */ IL_2D1B: sub
// 			/* 0x00091C58 139B         */ IL_2D1C: stloc.s   V_155
// 			/* 0x00091C5A 02           */ IL_2D1E: ldarg.0
// 			/* 0x00091C5B 7BE00B0004   */ IL_2D1F: ldfld     valuetype StationStore[] StationComponent::'storage'
// 			/* 0x00091C60 02           */ IL_2D24: ldarg.0
// 			/* 0x00091C61 7BDC0B0004   */ IL_2D25: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C66 1118         */ IL_2D2A: ldloc.s   V_24
// 			/* 0x00091C68 8F5B010002   */ IL_2D2C: ldelema   RemoteLogisticOrder
// 			/* 0x00091C6D 7B5D0C0004   */ IL_2D31: ldfld     int32 RemoteLogisticOrder::thisIndex
// 			/* 0x00091C72 8F54010002   */ IL_2D36: ldelema   StationStore
// 			/* 0x00091C77 7C1E0C0004   */ IL_2D3B: ldflda    int32 StationStore::remoteOrder
// 			/* 0x00091C7C 25           */ IL_2D40: dup
// 			/* 0x00091C7D 4A           */ IL_2D41: ldind.i4
// 			/* 0x00091C7E 119B         */ IL_2D42: ldloc.s   V_155
// 			/* 0x00091C80 58           */ IL_2D44: add
// 			/* 0x00091C81 54           */ IL_2D45: stind.i4
// 			/* 0x00091C82 02           */ IL_2D46: ldarg.0
// 			/* 0x00091C83 7BDC0B0004   */ IL_2D47: ldfld     valuetype RemoteLogisticOrder[] StationComponent::workShipOrders
// 			/* 0x00091C88 1118         */ IL_2D4C: ldloc.s   V_24
// 			/* 0x00091C8A 8F5B010002   */ IL_2D4E: ldelema   RemoteLogisticOrder
// 			/* 0x00091C8F 7C5E0C0004   */ IL_2D53: ldflda    int32 RemoteLogisticOrder::thisOrdered
// 			/* 0x00091C94 25           */ IL_2D58: dup
// 			/* 0x00091C95 4A           */ IL_2D59: ldind.i4
// 			/* 0x00091C96 119B         */ IL_2D5A: ldloc.s   V_155
// 			/* 0x00091C98 58           */ IL_2D5C: add
// 			/* 0x00091C99 54           */ IL_2D5D: stind.i4

// 			/* 0x00091C9A DE0C         */ IL_2D5E: leave.s   IL_2D6C
// 		} // end .try
// 		finally
// 		{
// 			/* 0x00091C9C 1115         */ IL_2D60: ldloc.s   V_21
// 			/* 0x00091C9E 2C07         */ IL_2D62: brfalse.s IL_2D6B

// 			/* 0x00091CA0 1114         */ IL_2D64: ldloc.s   V_20
// 			/* 0x00091CA2 287602000A   */ IL_2D66: call      void [netstandard]System.Threading.Monitor::Exit(object)

// 			/* 0x00091CA7 DC           */ IL_2D6B: endfinally
// 		} // end handler

// 		/* 0x00091CA8 1119         */ IL_2D6C: ldloc.s   V_25
// 		/* 0x00091CAA 15           */ IL_2D6E: ldc.i4.m1
// 		/* 0x00091CAB 7D3E0C0004   */ IL_2D6F: stfld     int32 ShipData::direction
// 		/* 0x00091CB0 2B32         */ IL_2D74: br.s      IL_2DA8

// 		/* 0x00091CB2 1119         */ IL_2D76: ldloc.s   V_25
// 		/* 0x00091CB4 7C3F0C0004   */ IL_2D78: ldflda    float32 ShipData::t
// 		/* 0x00091CB9 25           */ IL_2D7D: dup
// 		/* 0x00091CBA 4E           */ IL_2D7E: ldind.r4
// 		/* 0x00091CBB 2270CE083D   */ IL_2D7F: ldc.r4    0.0334
// 		/* 0x00091CC0 58           */ IL_2D84: add
// 		/* 0x00091CC1 56           */ IL_2D85: stind.r4
// 		/* 0x00091CC2 1119         */ IL_2D86: ldloc.s   V_25
// 		/* 0x00091CC4 7B3F0C0004   */ IL_2D88: ldfld     float32 ShipData::t
// 		/* 0x00091CC9 220000803F   */ IL_2D8D: ldc.r4    1
// 		/* 0x00091CCE 3614         */ IL_2D92: ble.un.s  IL_2DA8

// 		/* 0x00091CD0 1119         */ IL_2D94: ldloc.s   V_25
// 		/* 0x00091CD2 2200000000   */ IL_2D96: ldc.r4    0.0
// 		/* 0x00091CD7 7D3F0C0004   */ IL_2D9B: stfld     float32 ShipData::t
// 		/* 0x00091CDC 1119         */ IL_2DA0: ldloc.s   V_25
// 		/* 0x00091CDE 17           */ IL_2DA2: ldc.i4.1
// 		/* 0x00091CDF 7D310C0004   */ IL_2DA3: stfld     int32 ShipData::stage

// 		/* 0x00091CE4 1119         */ IL_2DA8: ldloc.s   V_25
// 		/* 0x00091CE6 111C         */ IL_2DAA: ldloc.s   V_28
// 		/* 0x00091CE8 7B46180004   */ IL_2DAC: ldfld     valuetype VectorLF3 AstroData::uPos
// 		/* 0x00091CED 111C         */ IL_2DB1: ldloc.s   V_28
// 		/* 0x00091CEF 7B44180004   */ IL_2DB3: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00091CF4 0E06         */ IL_2DB8: ldarg.s   gStationPool
// 		/* 0x00091CF6 1119         */ IL_2DBA: ldloc.s   V_25
// 		/* 0x00091CF8 7B3D0C0004   */ IL_2DBC: ldfld     int32 ShipData::otherGId
// 		/* 0x00091CFD 9A           */ IL_2DC1: ldelem.ref
// 		/* 0x00091CFE 7BC90B0004   */ IL_2DC2: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x00091D03 0E06         */ IL_2DC7: ldarg.s   gStationPool
// 		/* 0x00091D05 1119         */ IL_2DC9: ldloc.s   V_25
// 		/* 0x00091D07 7B3D0C0004   */ IL_2DCB: ldfld     int32 ShipData::otherGId
// 		/* 0x00091D0C 9A           */ IL_2DD0: ldelem.ref
// 		/* 0x00091D0D 7CC90B0004   */ IL_2DD1: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 StationComponent::shipDockPos
// 		/* 0x00091D12 289200000A   */ IL_2DD6: call      instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
// 		/* 0x00091D17 22666666C1   */ IL_2DDB: ldc.r4    -14.4
// 		/* 0x00091D1C 289800000A   */ IL_2DE0: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
// 		/* 0x00091D21 289900000A   */ IL_2DE5: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091D26 28BC020006   */ IL_2DEA: call      valuetype VectorLF3 VectorLF3::op_Implicit(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3)
// 		/* 0x00091D2B 280D040006   */ IL_2DEF: call      valuetype VectorLF3 Maths::QRotateLF(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype VectorLF3)
// 		/* 0x00091D30 28BB020006   */ IL_2DF4: call      valuetype VectorLF3 VectorLF3::op_Addition(valuetype VectorLF3, valuetype VectorLF3)
// 		/* 0x00091D35 7D340C0004   */ IL_2DF9: stfld     valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091D3A 1119         */ IL_2DFE: ldloc.s   V_25
// 		/* 0x00091D3C 7C350C0004   */ IL_2E00: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091D41 2200000000   */ IL_2E05: ldc.r4    0.0
// 		/* 0x00091D46 7D4100000A   */ IL_2E0A: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00091D4B 1119         */ IL_2E0F: ldloc.s   V_25
// 		/* 0x00091D4D 7C350C0004   */ IL_2E11: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091D52 2200000000   */ IL_2E16: ldc.r4    0.0
// 		/* 0x00091D57 7D4200000A   */ IL_2E1B: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00091D5C 1119         */ IL_2E20: ldloc.s   V_25
// 		/* 0x00091D5E 7C350C0004   */ IL_2E22: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091D63 2200000000   */ IL_2E27: ldc.r4    0.0
// 		/* 0x00091D68 7D8000000A   */ IL_2E2C: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00091D6D 1119         */ IL_2E31: ldloc.s   V_25
// 		/* 0x00091D6F 2200000000   */ IL_2E33: ldc.r4    0.0
// 		/* 0x00091D74 7D360C0004   */ IL_2E38: stfld     float32 ShipData::uSpeed
// 		/* 0x00091D79 1119         */ IL_2E3D: ldloc.s   V_25
// 		/* 0x00091D7B 111C         */ IL_2E3F: ldloc.s   V_28
// 		/* 0x00091D7D 7B44180004   */ IL_2E41: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion AstroData::uRot
// 		/* 0x00091D82 0E06         */ IL_2E46: ldarg.s   gStationPool
// 		/* 0x00091D84 1119         */ IL_2E48: ldloc.s   V_25
// 		/* 0x00091D86 7B3D0C0004   */ IL_2E4A: ldfld     int32 ShipData::otherGId
// 		/* 0x00091D8B 9A           */ IL_2E4F: ldelem.ref
// 		/* 0x00091D8C 7BCA0B0004   */ IL_2E50: ldfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion StationComponent::shipDockRot
// 		/* 0x00091D91 22F304353F   */ IL_2E55: ldc.r4    0.70710677
// 		/* 0x00091D96 2200000000   */ IL_2E5A: ldc.r4    0.0
// 		/* 0x00091D9B 2200000000   */ IL_2E5F: ldc.r4    0.0
// 		/* 0x00091DA0 22F30435BF   */ IL_2E64: ldc.r4    -0.70710677
// 		/* 0x00091DA5 735702000A   */ IL_2E69: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x00091DAA 281A01000A   */ IL_2E6E: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00091DAF 281A01000A   */ IL_2E73: call      valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion [UnityEngine.CoreModule]UnityEngine.Quaternion::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion)
// 		/* 0x00091DB4 7D380C0004   */ IL_2E78: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00091DB9 1119         */ IL_2E7D: ldloc.s   V_25
// 		/* 0x00091DBB 7C390C0004   */ IL_2E7F: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091DC0 2200000000   */ IL_2E84: ldc.r4    0.0
// 		/* 0x00091DC5 7D4100000A   */ IL_2E89: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00091DCA 1119         */ IL_2E8E: ldloc.s   V_25
// 		/* 0x00091DCC 7C390C0004   */ IL_2E90: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091DD1 2200000000   */ IL_2E95: ldc.r4    0.0
// 		/* 0x00091DD6 7D4200000A   */ IL_2E9A: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00091DDB 1119         */ IL_2E9F: ldloc.s   V_25
// 		/* 0x00091DDD 7C390C0004   */ IL_2EA1: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uAngularVel
// 		/* 0x00091DE2 2200000000   */ IL_2EA6: ldc.r4    0.0
// 		/* 0x00091DE7 7D8000000A   */ IL_2EAB: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00091DEC 1119         */ IL_2EB0: ldloc.s   V_25
// 		/* 0x00091DEE 2200000000   */ IL_2EB2: ldc.r4    0.0
// 		/* 0x00091DF3 7D3A0C0004   */ IL_2EB7: stfld     float32 ShipData::uAngularSpeed
// 		/* 0x00091DF8 1119         */ IL_2EBC: ldloc.s   V_25
// 		/* 0x00091DFA 2200000000   */ IL_2EBE: ldc.r4    0.0
// 		/* 0x00091DFF 2200000000   */ IL_2EC3: ldc.r4    0.0
// 		/* 0x00091E04 2200000000   */ IL_2EC8: ldc.r4    0.0
// 		/* 0x00091E09 73B3020006   */ IL_2ECD: newobj    instance void VectorLF3::.ctor(float32, float32, float32)
// 		/* 0x00091E0E 7D3B0C0004   */ IL_2ED2: stfld     valuetype VectorLF3 ShipData::pPosTemp
// 		/* 0x00091E13 1119         */ IL_2ED7: ldloc.s   V_25
// 		/* 0x00091E15 2200000000   */ IL_2ED9: ldc.r4    0.0
// 		/* 0x00091E1A 2200000000   */ IL_2EDE: ldc.r4    0.0
// 		/* 0x00091E1F 2200000000   */ IL_2EE3: ldc.r4    0.0
// 		/* 0x00091E24 220000803F   */ IL_2EE8: ldc.r4    1
// 		/* 0x00091E29 735702000A   */ IL_2EED: newobj    instance void [UnityEngine.CoreModule]UnityEngine.Quaternion::.ctor(float32, float32, float32, float32)
// 		/* 0x00091E2E 7D3C0C0004   */ IL_2EF2: stfld     valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::pRotTemp
// 		/* 0x00091E33 111A         */ IL_2EF7: ldloc.s   V_26
// 		/* 0x00091E35 7C4A0C0004   */ IL_2EF9: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00091E3A 2200000000   */ IL_2EFE: ldc.r4    0.0
// 		/* 0x00091E3F 7D7E01000A   */ IL_2F03: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z

// 		/* 0x00091E44 121D         */ IL_2F08: ldloca.s  V_29
// 		/* 0x00091E46 1119         */ IL_2F0A: ldloc.s   V_25
// 		/* 0x00091E48 7C350C0004   */ IL_2F0C: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091E4D 7B4100000A   */ IL_2F11: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00091E52 1119         */ IL_2F16: ldloc.s   V_25
// 		/* 0x00091E54 7B360C0004   */ IL_2F18: ldfld     float32 ShipData::uSpeed
// 		/* 0x00091E59 5A           */ IL_2F1D: mul
// 		/* 0x00091E5A 7D4100000A   */ IL_2F1E: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::x
// 		/* 0x00091E5F 121D         */ IL_2F23: ldloca.s  V_29
// 		/* 0x00091E61 1119         */ IL_2F25: ldloc.s   V_25
// 		/* 0x00091E63 7C350C0004   */ IL_2F27: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091E68 7B4200000A   */ IL_2F2C: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00091E6D 1119         */ IL_2F31: ldloc.s   V_25
// 		/* 0x00091E6F 7B360C0004   */ IL_2F33: ldfld     float32 ShipData::uSpeed
// 		/* 0x00091E74 5A           */ IL_2F38: mul
// 		/* 0x00091E75 7D4200000A   */ IL_2F39: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::y
// 		/* 0x00091E7A 121D         */ IL_2F3E: ldloca.s  V_29
// 		/* 0x00091E7C 1119         */ IL_2F40: ldloc.s   V_25
// 		/* 0x00091E7E 7C350C0004   */ IL_2F42: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 ShipData::uVel
// 		/* 0x00091E83 7B8000000A   */ IL_2F47: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00091E88 1119         */ IL_2F4C: ldloc.s   V_25
// 		/* 0x00091E8A 7B360C0004   */ IL_2F4E: ldfld     float32 ShipData::uSpeed
// 		/* 0x00091E8F 5A           */ IL_2F53: mul
// 		/* 0x00091E90 7D8000000A   */ IL_2F54: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector3::z
// 		/* 0x00091E95 111B         */ IL_2F59: ldloc.s   V_27
// 		/* 0x00091E97 2C71         */ IL_2F5B: brfalse.s IL_2FCE

// 		/* 0x00091E99 111A         */ IL_2F5D: ldloc.s   V_26
// 		/* 0x00091E9B 1119         */ IL_2F5F: ldloc.s   V_25
// 		/* 0x00091E9D 7C340C0004   */ IL_2F61: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091EA2 1213         */ IL_2F66: ldloca.s  V_19
// 		/* 0x00091EA4 0E08         */ IL_2F68: ldarg.s   relativePos
// 		/* 0x00091EA6 0E09         */ IL_2F6A: ldarg.s   relativeRot
// 		/* 0x00091EA8 121D         */ IL_2F6C: ldloca.s  V_29
// 		/* 0x00091EAA 1119         */ IL_2F6E: ldloc.s   V_25
// 		/* 0x00091EAC 7B410C0004   */ IL_2F70: ldfld     int32 ShipData::itemCount
// 		/* 0x00091EB1 16           */ IL_2F75: ldc.i4.0
// 		/* 0x00091EB2 3003         */ IL_2F76: bgt.s     IL_2F7B

// 		/* 0x00091EB4 16           */ IL_2F78: ldc.i4.0
// 		/* 0x00091EB5 2B07         */ IL_2F79: br.s      IL_2F82

// 		/* 0x00091EB7 1119         */ IL_2F7B: ldloc.s   V_25
// 		/* 0x00091EB9 7B400C0004   */ IL_2F7D: ldfld     int32 ShipData::itemId

// 		/* 0x00091EBE 28290A0006   */ IL_2F82: call      instance void ShipRenderingData::SetPose(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, int32)
// 		/* 0x00091EC3 0E0A         */ IL_2F87: ldarg.s   starmap
// 		/* 0x00091EC5 39B6000000   */ IL_2F89: brfalse   IL_3044

// 		/* 0x00091ECA 02           */ IL_2F8E: ldarg.0
// 		/* 0x00091ECB 7BDF0B0004   */ IL_2F8F: ldfld     valuetype ShipUIRenderingData[] StationComponent::shipUIRenderers
// 		/* 0x00091ED0 1119         */ IL_2F94: ldloc.s   V_25
// 		/* 0x00091ED2 7B440C0004   */ IL_2F96: ldfld     int32 ShipData::shipIndex
// 		/* 0x00091ED7 8F59010002   */ IL_2F9B: ldelema   ShipUIRenderingData
// 		/* 0x00091EDC 1119         */ IL_2FA0: ldloc.s   V_25
// 		/* 0x00091EDE 7C340C0004   */ IL_2FA2: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091EE3 1213         */ IL_2FA7: ldloca.s  V_19
// 		/* 0x00091EE5 1112         */ IL_2FA9: ldloc.s   V_18
// 		/* 0x00091EE7 6B           */ IL_2FAB: conv.r4
// 		/* 0x00091EE8 1119         */ IL_2FAC: ldloc.s   V_25
// 		/* 0x00091EEA 7B360C0004   */ IL_2FAE: ldfld     float32 ShipData::uSpeed
// 		/* 0x00091EEF 1119         */ IL_2FB3: ldloc.s   V_25
// 		/* 0x00091EF1 7B410C0004   */ IL_2FB5: ldfld     int32 ShipData::itemCount
// 		/* 0x00091EF6 16           */ IL_2FBA: ldc.i4.0
// 		/* 0x00091EF7 3003         */ IL_2FBB: bgt.s     IL_2FC0

// 		/* 0x00091EF9 16           */ IL_2FBD: ldc.i4.0
// 		/* 0x00091EFA 2B07         */ IL_2FBE: br.s      IL_2FC7

// 		/* 0x00091EFC 1119         */ IL_2FC0: ldloc.s   V_25
// 		/* 0x00091EFE 7B400C0004   */ IL_2FC2: ldfld     int32 ShipData::itemId

// 		/* 0x00091F03 282B0A0006   */ IL_2FC7: call      instance void ShipUIRenderingData::SetPose(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, float32, float32, int32)
// 		/* 0x00091F08 2B76         */ IL_2FCC: br.s      IL_3044

// 		/* 0x00091F0A 111A         */ IL_2FCE: ldloc.s   V_26
// 		/* 0x00091F0C 1119         */ IL_2FD0: ldloc.s   V_25
// 		/* 0x00091F0E 7C340C0004   */ IL_2FD2: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091F13 1119         */ IL_2FD7: ldloc.s   V_25
// 		/* 0x00091F15 7C380C0004   */ IL_2FD9: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00091F1A 0E08         */ IL_2FDE: ldarg.s   relativePos
// 		/* 0x00091F1C 0E09         */ IL_2FE0: ldarg.s   relativeRot
// 		/* 0x00091F1E 121D         */ IL_2FE2: ldloca.s  V_29
// 		/* 0x00091F20 1119         */ IL_2FE4: ldloc.s   V_25
// 		/* 0x00091F22 7B410C0004   */ IL_2FE6: ldfld     int32 ShipData::itemCount
// 		/* 0x00091F27 16           */ IL_2FEB: ldc.i4.0
// 		/* 0x00091F28 3003         */ IL_2FEC: bgt.s     IL_2FF1

// 		/* 0x00091F2A 16           */ IL_2FEE: ldc.i4.0
// 		/* 0x00091F2B 2B07         */ IL_2FEF: br.s      IL_2FF8

// 		/* 0x00091F2D 1119         */ IL_2FF1: ldloc.s   V_25
// 		/* 0x00091F2F 7B400C0004   */ IL_2FF3: ldfld     int32 ShipData::itemId

// 		/* 0x00091F34 28290A0006   */ IL_2FF8: call      instance void ShipRenderingData::SetPose(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3&, int32)
// 		/* 0x00091F39 0E0A         */ IL_2FFD: ldarg.s   starmap
// 		/* 0x00091F3B 2C43         */ IL_2FFF: brfalse.s IL_3044

// 		/* 0x00091F3D 02           */ IL_3001: ldarg.0
// 		/* 0x00091F3E 7BDF0B0004   */ IL_3002: ldfld     valuetype ShipUIRenderingData[] StationComponent::shipUIRenderers
// 		/* 0x00091F43 1119         */ IL_3007: ldloc.s   V_25
// 		/* 0x00091F45 7B440C0004   */ IL_3009: ldfld     int32 ShipData::shipIndex
// 		/* 0x00091F4A 8F59010002   */ IL_300E: ldelema   ShipUIRenderingData
// 		/* 0x00091F4F 1119         */ IL_3013: ldloc.s   V_25
// 		/* 0x00091F51 7C340C0004   */ IL_3015: ldflda    valuetype VectorLF3 ShipData::uPos
// 		/* 0x00091F56 1119         */ IL_301A: ldloc.s   V_25
// 		/* 0x00091F58 7C380C0004   */ IL_301C: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion ShipData::uRot
// 		/* 0x00091F5D 1112         */ IL_3021: ldloc.s   V_18
// 		/* 0x00091F5F 6B           */ IL_3023: conv.r4
// 		/* 0x00091F60 1119         */ IL_3024: ldloc.s   V_25
// 		/* 0x00091F62 7B360C0004   */ IL_3026: ldfld     float32 ShipData::uSpeed
// 		/* 0x00091F67 1119         */ IL_302B: ldloc.s   V_25
// 		/* 0x00091F69 7B410C0004   */ IL_302D: ldfld     int32 ShipData::itemCount
// 		/* 0x00091F6E 16           */ IL_3032: ldc.i4.0
// 		/* 0x00091F6F 3003         */ IL_3033: bgt.s     IL_3038

// 		/* 0x00091F71 16           */ IL_3035: ldc.i4.0
// 		/* 0x00091F72 2B07         */ IL_3036: br.s      IL_303F

// 		/* 0x00091F74 1119         */ IL_3038: ldloc.s   V_25
// 		/* 0x00091F76 7B400C0004   */ IL_303A: ldfld     int32 ShipData::itemId

// 		/* 0x00091F7B 282B0A0006   */ IL_303F: call      instance void ShipUIRenderingData::SetPose(valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&, float32, float32, int32)

// 		/* 0x00091F80 111A         */ IL_3044: ldloc.s   V_26
// 		/* 0x00091F82 7C4A0C0004   */ IL_3046: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00091F87 7B7E01000A   */ IL_304B: ldfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z
// 		/* 0x00091F8C 2200000000   */ IL_3050: ldc.r4    0.0
// 		/* 0x00091F91 3411         */ IL_3055: bge.un.s  IL_3068

// 		/* 0x00091F93 111A         */ IL_3057: ldloc.s   V_26
// 		/* 0x00091F95 7C4A0C0004   */ IL_3059: ldflda    valuetype [UnityEngine.CoreModule]UnityEngine.Vector4 ShipRenderingData::anim
// 		/* 0x00091F9A 2200000000   */ IL_305E: ldc.r4    0.0
// 		/* 0x00091F9F 7D7E01000A   */ IL_3063: stfld     float32 [UnityEngine.CoreModule]UnityEngine.Vector4::z

// 		/* 0x00091FA4 1118         */ IL_3068: ldloc.s   V_24
// 		/* 0x00091FA6 17           */ IL_306A: ldc.i4.1
// 		/* 0x00091FA7 58           */ IL_306B: add
// 		/* 0x00091FA8 1318         */ IL_306C: stloc.s   V_24

// 		/* 0x00091FAA 1118         */ IL_306E: ldloc.s   V_24
// 		/* 0x00091FAC 02           */ IL_3070: ldarg.0
// 		/* 0x00091FAD 7BD80B0004   */ IL_3071: ldfld     int32 StationComponent::workShipCount
// 		/* 0x00091FB2 3F6FD1FFFF   */ IL_3076: blt       IL_01EA
// 	// end loop

// 	/* 0x00091FB7 02           */ IL_307B: ldarg.0
// 	/* 0x00091FB8 0E07         */ IL_307C: ldarg.s   astroPoses
// 	/* 0x00091FBA 0E08         */ IL_307E: ldarg.s   relativePos
// 	/* 0x00091FBC 0E09         */ IL_3080: ldarg.s   relativeRot
// 	/* 0x00091FBE 28FB090006   */ IL_3082: call      instance void StationComponent::ShipRenderersOnTick(valuetype AstroData[], valuetype VectorLF3&, valuetype [UnityEngine.CoreModule]UnityEngine.Quaternion&)
// 	/* 0x00091FC3 16           */ IL_3087: ldc.i4.0
// 	/* 0x00091FC4 139C         */ IL_3088: stloc.s   V_156
// 	/* 0x00091FC6 2B70         */ IL_308A: br.s      IL_30FC
// 	// loop start (head: IL_30FC)
// 		/* 0x00091FC8 02           */ IL_308C: ldarg.0
// 		/* 0x00091FC9 7BE10B0004   */ IL_308D: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00091FCE 119C         */ IL_3092: ldloc.s   V_156
// 		/* 0x00091FD0 8F55010002   */ IL_3094: ldelema   StationPriorityLock
// 		/* 0x00091FD5 7B240C0004   */ IL_3099: ldfld     uint8 StationPriorityLock::priorityIndex
// 		/* 0x00091FDA 16           */ IL_309E: ldc.i4.0
// 		/* 0x00091FDB 3255         */ IL_309F: blt.s     IL_30F6

// 		/* 0x00091FDD 02           */ IL_30A1: ldarg.0
// 		/* 0x00091FDE 7BE10B0004   */ IL_30A2: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00091FE3 119C         */ IL_30A7: ldloc.s   V_156
// 		/* 0x00091FE5 8F55010002   */ IL_30A9: ldelema   StationPriorityLock
// 		/* 0x00091FEA 7B250C0004   */ IL_30AE: ldfld     uint8 StationPriorityLock::lockTick
// 		/* 0x00091FEF 16           */ IL_30B3: ldc.i4.0
// 		/* 0x00091FF0 311A         */ IL_30B4: ble.s     IL_30D0

// 		/* 0x00091FF2 02           */ IL_30B6: ldarg.0
// 		/* 0x00091FF3 7BE10B0004   */ IL_30B7: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00091FF8 119C         */ IL_30BC: ldloc.s   V_156
// 		/* 0x00091FFA 8F55010002   */ IL_30BE: ldelema   StationPriorityLock
// 		/* 0x00091FFF 7C250C0004   */ IL_30C3: ldflda    uint8 StationPriorityLock::lockTick
// 		/* 0x00092004 25           */ IL_30C8: dup
// 		/* 0x00092005 47           */ IL_30C9: ldind.u1
// 		/* 0x00092006 17           */ IL_30CA: ldc.i4.1
// 		/* 0x00092007 59           */ IL_30CB: sub
// 		/* 0x00092008 D2           */ IL_30CC: conv.u1
// 		/* 0x00092009 52           */ IL_30CD: stind.i1
// 		/* 0x0009200A 2B26         */ IL_30CE: br.s      IL_30F6

// 		/* 0x0009200C 02           */ IL_30D0: ldarg.0
// 		/* 0x0009200D 7BE10B0004   */ IL_30D1: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00092012 119C         */ IL_30D6: ldloc.s   V_156
// 		/* 0x00092014 8F55010002   */ IL_30D8: ldelema   StationPriorityLock
// 		/* 0x00092019 16           */ IL_30DD: ldc.i4.0
// 		/* 0x0009201A 7D250C0004   */ IL_30DE: stfld     uint8 StationPriorityLock::lockTick
// 		/* 0x0009201F 02           */ IL_30E3: ldarg.0
// 		/* 0x00092020 7BE10B0004   */ IL_30E4: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00092025 119C         */ IL_30E9: ldloc.s   V_156
// 		/* 0x00092027 8F55010002   */ IL_30EB: ldelema   StationPriorityLock
// 		/* 0x0009202C 16           */ IL_30F0: ldc.i4.0
// 		/* 0x0009202D 7D240C0004   */ IL_30F1: stfld     uint8 StationPriorityLock::priorityIndex

// 		/* 0x00092032 119C         */ IL_30F6: ldloc.s   V_156
// 		/* 0x00092034 17           */ IL_30F8: ldc.i4.1
// 		/* 0x00092035 58           */ IL_30F9: add
// 		/* 0x00092036 139C         */ IL_30FA: stloc.s   V_156

// 		/* 0x00092038 119C         */ IL_30FC: ldloc.s   V_156
// 		/* 0x0009203A 02           */ IL_30FE: ldarg.0
// 		/* 0x0009203B 7BE10B0004   */ IL_30FF: ldfld     valuetype StationPriorityLock[] StationComponent::priorityLocks
// 		/* 0x00092040 8E           */ IL_3104: ldlen
// 		/* 0x00092041 69           */ IL_3105: conv.i4
// 		/* 0x00092042 3284         */ IL_3106: blt.s     IL_308C
// 	// end loop

// 	/* 0x00092044 2A           */ IL_3108: ret
// } // end of method StationComponent::InternalTickRemote
