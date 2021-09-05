using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBuildTool_Click
    {
        [HarmonyPatch(typeof(BuildTool_Click), "CheckBuildConditions")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions).End().MatchBack(false, new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldflda, AccessTools.Field(typeof(BuildTool_Click), nameof(BuildTool_Click.cursorTarget)))).Advance(1);

            while (matcher.Opcode != OpCodes.Stloc_S) matcher.RemoveInstruction();

            matcher.InsertAndAdvance(Transpilers.EmitDelegate<Func<BuildTool_Click, Vector3>>(tool => { return tool.cursorTarget.normalized * Mathf.Min(tool.planet.realRadius * 0.025f, 20f); }));

            return matcher.InstructionEnumeration();
        }
    }
}