using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnBuildTool_Path
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(BuildTool_Path), nameof(BuildTool_Path.CheckBuildConditions))]
        public static IEnumerable<CodeInstruction> CheckBuildConditions_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                // replace : Physics.OverlapCapsuleNonAlloc(vector2, vector3, 0.28f, BuildTool._tmp_cols, 425984, QueryTriggerInteraction.Collide);
                // with    : Physics.OverlapCapsuleNonAlloc(vector2, vector3, 0.22f, BuildTool._tmp_cols, 425984, QueryTriggerInteraction.Collide);
                MethodInfo methodInfo = AccessTools.Method(typeof(Physics), "OverlapCapsuleNonAlloc", 
                    new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int), typeof(QueryTriggerInteraction) });
                int count = 0;
                var codeMatcher = new CodeMatcher(instructions)
                    .MatchForward(false, new CodeMatch(OpCodes.Call, methodInfo))
                    .Repeat(
                        matcher =>
                        {
                            if (matcher.InstructionAt(-4).opcode == OpCodes.Ldc_R4) {
                                matcher
                                    .Advance(-4)
                                    .SetOperandAndAdvance(0.22f);
                                count++;
                                matcher.Advance(4);
                            }
                        }
                    );

                if (count != 4)
                    GS3.Warn("BuildTool_Path.CheckBuildConditions transpiler doesn't work as expected: " + count);

                return codeMatcher.InstructionEnumeration();
            }
            catch (Exception e)
            {
                GS3.Warn("BuildTool_Path.CheckBuildConditions transpiler fail!");
                GS3.Warn(e.Message);
                return instructions;
            }
        }
    }
}