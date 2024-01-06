using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Click
    {
        //          IL_06e8: ldarg.0      // this
        //          IL_06e9: ldfld        class PlanetData BuildTool::planet
        //          IL_06ee: brfalse.s    IL_072c
        //
        //          IL_06f0: ldarg.0      // this
        //          IL_06f1: ldfld        class PlanetData BuildTool::planet
        //          IL_06f6: ldfld        valuetype EPlanetType PlanetData::'type'
        //          IL_06fb: ldc.i4.5
        //          IL_06fc: bne.un.s     IL_072c
        //
        //          // [359 15 - 359 83]
        //          IL_06fe: ldloca.s     pos
        //          IL_0700: call         instance valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::get_normalized()
        //          IL_0705: ldarg.0      // this
        //          IL_0706: ldfld        class PlanetData BuildTool::planet
        //          IL_070b: callvirt     instance float32 PlanetData::get_realRadius()
        //          IL_0710: call         valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
        //          IL_0715: ldc.r4       0.025
        //          IL_071a: call         valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
        //          IL_071f: stloc.s      vector3_3


        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BuildTool_Click), nameof(BuildTool_Click.DeterminePreviews))]
        public static IEnumerable<CodeInstruction> BuildTool_Click_DeterminePreviews_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            instructions = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(i =>
                        i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_realRadius"),
                    new CodeMatch(OpCodes.Call))
                .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(realRadius =>
                {
                    return Mathf.Min(realRadius * 0.025f, 20f) / 0.025f;
                })).InstructionEnumeration();
            return instructions;
        }
    }

}