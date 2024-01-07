using System.Collections.Generic;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale

{
    public class PatchOnDefenseSystem
    {

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DefenseSystem),  nameof(DefenseSystem.NewTurretComponent))]
        public static IEnumerable<CodeInstruction> NewTurretComponentTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
             matcher.MatchForward(
                    true,
                    new CodeMatch(i => i.opcode == Ldarg_0),
                    new CodeMatch(i => i.opcode == Ldfld && i.operand.ToString().Contains("TurretComponent")),
                    new CodeMatch(i => i.opcode == Callvirt),
                    new CodeMatch(i => i.opcode == Stloc_0)
                    );
                 if (!matcher.IsInvalid)
                 {
                     matcher.Advance(1)
                         .InsertAndAdvance(new CodeInstruction(Ldarg_0))
                         .InsertAndAdvance(new CodeInstruction(Ldloc_0))
                         .InsertAndAdvance(new CodeInstruction(Call,
                             AccessTools.Method(typeof(TurretComponentTranspiler),
                                 nameof(TurretComponentTranspiler.AddTurret)))
                         );
                     instructions = matcher.InstructionEnumeration();
                     return instructions;
                 }

                 Bootstrap.Logger.LogInfo("Transpiler failed!  ");
        
                 return instructions;
        }
      
    }
}