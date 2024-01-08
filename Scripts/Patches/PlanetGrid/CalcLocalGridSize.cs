using System;
using System.CodeDom;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;
namespace GalacticScale
{
    public partial class PatchOnPlanetGrid
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcLocalGridSize))]
        public static IEnumerable<CodeInstruction> CalcLocalGridSize_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var matcher = new CodeMatcher(instructions);
                matcher.MatchForward(
                    false,
                    new CodeMatch(i =>
                        i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 0.0031415927) < 0.01),
                    new CodeMatch(i => i.opcode == Ldloc_2),
                    new CodeMatch(i => i.opcode == Call),
                    new CodeMatch(i =>
                        i.opcode == Ldc_R4 && Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 3.1415927) < 0.1),
                    new CodeMatch(i => i.opcode == Mul));
                if (!matcher.IsInvalid)
                {
                    matcher.SetInstruction(new CodeInstruction(Nop));
                }

                matcher.MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldc_R4 && Convert.ToInt32(i.operand ?? 0.0) == 5),
                    new CodeMatch(i => i.opcode == Mul),
                    new CodeMatch(i => i.opcode == Div),
                    new CodeMatch(i => i.opcode == Call && i.operand.ToString().Contains("Max")));
                if (!matcher.IsInvalid)
                {
                   matcher.SetInstruction(new CodeInstruction(Nop));

                    instructions = matcher.InstructionEnumeration();
                    return instructions;
                }
                else
                {
                    Bootstrap.Logger.LogError("PatchOnPlanetGrid.CalcLocalGridSize_Transpiler failed 1");
                }
                return instructions;
            }
            catch
            {
                Bootstrap.Logger.LogInfo("PatchOnPlanetGrid.CalcLocalGridSize_Transpiler failed");
                return instructions;
            }
        }
        
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PlanetGrid), nameof(PlanetGrid.CalcLocalGridSize)) ]
        // public static bool CalcLocalGridSize(ref PlanetGrid __instance, ref float __result, Vector3 posR, Vector3 dir)
        // {
        //     float f = Vector3.Dot(Vector3.Cross(posR, Vector3.up).normalized, dir);
        //     float magnitude = posR.magnitude;
        //     posR.Normalize();
        //     if ((double)Mathf.Abs(f) < 0.7)
        //     {
        //         __result = magnitude * 3.1415927f * 2f / (float)(__instance.segment * 5);
        //         // if (VFInput.alt) Debug.Log($"1 f:{f} Dir:{dir} posR:{posR} Result:{__result} Segment:{__instance.segment} LatIdx:{Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(Mathf.Asin(posR.y) / 6.2831855f * (float)__instance.segment) - 0.1f))} Determine:{PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(Mathf.Asin(posR.y) / 6.2831855f * (float)__instance.segment) - 0.1f)), __instance.segment)}");
        //     
        //         return false;
        //     }
        //     float num = Mathf.Asin(posR.y);
        //     float f2 = num / 6.2831855f * (float)__instance.segment;
        //     float num2 = (float)PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f2) - 0.1f)), __instance.segment);
        //     // float num3 = Mathf.Max(0.0031415927f, Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f));
        //     float num3 =  Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f);
        //     __result = magnitude * num3;
        //     // if (VFInput.alt) Debug.Log($"2 num3:{num3} f:{f} Dir:{dir} posR:{posR} Result:{__result} Segment:{__instance.segment} LatIdx:{Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(Mathf.Asin(posR.y) / 6.2831855f * (float)__instance.segment) - 0.1f))} Determine:{PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(Mathf.Asin(posR.y) / 6.2831855f * (float)__instance.segment) - 0.1f)), __instance.segment)}");
        //
        //     return false;
        // }
        
        
        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(PlanetGrid), "CalcLocalGridSize")]
        // public static void CalcLocalGridSize(Vector3 posR, Vector3 dir, ref float __result, ref PlanetGrid __instance)
        // {
        //     if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 480) __result -= GS2.Config.FixCopyPasteSize;
        //     if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 490) __result -= GS2.Config.FixCopyPasteSize;
        //     if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 500) __result -= GS2.Config.FixCopyPasteSize;
        //     if (GS2.Config.FixCopyPaste && GameMain.localPlanet.radius == 510) __result -= GS2.Config.FixCopyPasteSize;
        // }
    }
}

// public float CalcLocalGridSize(Vector3 posR, Vector3 dir)
// {
//     float f = Vector3.Dot(Vector3.Cross(posR, Vector3.up).normalized, dir);
//     float magnitude = posR.magnitude;
//     posR.Normalize();
//     if ((double)Mathf.Abs(f) < 0.7)
//     {
//         return magnitude * 3.1415927f * 2f / (float)(this.segment * 5);
//     }
//     float num = Mathf.Asin(posR.y);
//     float f2 = num / 6.2831855f * (float)this.segment;
//     float num2 = (float)PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f2) - 0.1f)), this.segment);
//     float num3 = Mathf.Max(0.0031415927f, Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f));
//     return magnitude * num3;
// }

// /* 0x001755AA 222EE34D3B   */ IL_0096: ldc.r4    0.0031415927
// /* 0x001755AF 08           */ IL_009B: ldloc.2
// /* 0x001755B0 282B01000A   */ IL_009C: call      float32 [UnityEngine.CoreModule]UnityEngine.Mathf::Cos(float32)
// /* 0x001755B5 22DB0F4940   */ IL_00A1: ldc.r4    3.1415927
// /* 0x001755BA 5A           */ IL_00A6: mul
// /* 0x001755BB 2200000040   */ IL_00A7: ldc.r4    2
// /* 0x001755C0 5A           */ IL_00AC: mul
// /* 0x001755C1 1104         */ IL_00AD: ldloc.s   V_4
// /* 0x001755C3 220000A040   */ IL_00AF: ldc.r4    5
// /* 0x001755C8 5A           */ IL_00B4: mul
// /* 0x001755C9 5B           */ IL_00B5: div
// /* 0x001755CA 28D600000A   */ IL_00B6: call      float32 [UnityEngine.CoreModule]UnityEngine.Mathf::Max(float32, float32)
// /* 0x001755CF 1305         */ IL_00BB: stloc.s   V_5