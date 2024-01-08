using System;
using System.Collections.Generic;
using BCE;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcSegmentsAcross))]
        public static IEnumerable<CodeInstruction> CalcSegmentsAcross_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions);
                matcher.MatchForward(
                    false,
                    new CodeMatch(i =>
                        i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 0.0048) < 0.0001),
                    new CodeMatch(i => i.opcode == Ldloc_0),
                    new CodeMatch(i => i.opcode == Call),
                    new CodeMatch(i => i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 3.1415927) < 0.1),
                    new CodeMatch(i => i.opcode == Mul));
                if (!matcher.IsInvalid)
                {
                    matcher.LogILPre();
                    matcher.SetInstructionAndAdvance(new CodeInstruction(Ldc_R4, 1f));
                    matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                    matcher.InsertAndAdvance(CodeInstruction.LoadField(typeof(PlanetGrid), nameof(PlanetGrid.segment)));
                    matcher.InsertAndAdvance(new CodeInstruction(Conv_R4));
                    matcher.InsertAndAdvance(new CodeInstruction(Div));
                    matcher.InsertAndAdvance(new CodeInstruction(Ldc_R4, 0.96f));
                    matcher.Insert(new CodeInstruction(Mul));
                    matcher.LogILPost(7);
                    instructions = matcher.InstructionEnumeration();
                } //1.0f / __instance.segment * 0.96f
                else
                {
                    Bootstrap.Logger.LogError("PatchOnPlanetGrid.CalcSegmentsAcross_Transpiler failed");
                }
                // 1f
                //  __instance.segment
                // mul
                //0.96f
                //mul
                
                
                
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("PatchOnPlanetGrid.CalcSegmentsAcross_Transpiler failed (Caught)");
                return instructions;
            }
        }
        
        
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcSegmentsAcross))]
        // public static bool CalcSegmentsAcross(PlanetGrid __instance, Vector3 posR, Vector3 posA, Vector3 posB, ref float __result)
        // {
        //     //No config check for Planet size change since this replicates vanilla in case of size 200
        //     posR.Normalize();
        //     posA.Normalize();
        //     posB.Normalize();
        //     var num = Mathf.Asin(posR.y);
        //     var f = num / ((float)Math.PI * 2f) * __instance.segment;
        //     var latitudeIndex = Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f) - 0.1f));
        //     float num2 = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
        //     //Replaced the fixed value 0.0048 with 1/segments * 0.96 [based on planet size 200: 1/200 = 0.005; 0.005 * 0.96 = 0.0048
        //     //since the value has to become smaller the larger the planet is, the inverse value (1/x) is used in the calculation
        //     var num3 = Mathf.Max(1.0f / __instance.segment * 0.96f, Mathf.Cos(num) * (float)Math.PI * 2f / (num2 * 5f));
        //     var num4 = (float)Math.PI * 2f / (__instance.segment * 5f);
        //     var num5 = Mathf.Asin(posA.y);
        //     var num6 = Mathf.Atan2(posA.x, 0f - posA.z);
        //     var num7 = Mathf.Asin(posB.y);
        //     var num8 = Mathf.Atan2(posB.x, 0f - posB.z);
        //     var num9 = Mathf.Abs(Mathf.DeltaAngle(num6 * 57.29578f, num8 * 57.29578f) * ((float)Math.PI / 180f));
        //     var num10 = Mathf.Abs(num5 - num7);
        //     var num11 = num10 + num9;
        //     var num12 = 0f;
        //     var num13 = 1f;
        //     if (num11 > 0f)
        //     {
        //         num12 = num9 / num11;
        //         num13 = num10 / num11;
        //     }
        //
        //     var num14 = num3 * num12 + num4 * num13;
        //     __result = (posA - posB).magnitude / num14;
        //     return false;
        // }
    }
}