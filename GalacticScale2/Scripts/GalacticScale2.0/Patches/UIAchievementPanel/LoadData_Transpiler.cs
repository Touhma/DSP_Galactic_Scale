using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GalacticScale
{
    /*
     * I really dont know why this is needed, but at least with Nebula this method tries to add stuff to a dictionary which already is added.
     * Thus this transpiler adds a check and if the key is already present in the dictionary it just skips to the next element.
     * 
     * problematic vanilla line was: this.uiEntries.Add(keyValuePair.Key, uiachievementEntry);
     */
    [HarmonyPatch(typeof(UIAchievementPanel))]
    public partial class PatchOnUIAchievementPanel
    {
        private delegate bool checkIfKeyExists(ref KeyValuePair<int, AchievementState> keyValuePair, UIAchievementPanel instance);

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(UIAchievementPanel.LoadData))]
        public static IEnumerable<CodeInstruction> LoadData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Br));

            object continueJmp = matcher.Operand;

            matcher.Advance(4);
            CodeInstruction loadKeyValuePair = matcher.Instruction;
            matcher.Advance(1);

            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
            matcher.InsertAndAdvance(HarmonyLib.Transpilers.EmitDelegate<checkIfKeyExists>((ref KeyValuePair<int, AchievementState> keyValuePair, UIAchievementPanel instance) =>
            {
                if (instance.uiEntries.ContainsKey(keyValuePair.Key))
                {
                    return false;
                }
                return true;
            }));
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse, continueJmp)); // call continue on foreach loop if key is already known.
            matcher.Insert(loadKeyValuePair);

            return matcher.InstructionEnumeration();
        }
    }
}
