using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
   
    [HarmonyPatch(typeof(UIAchievementPanel))]
    public partial class PatchOnUIAchievementPanel
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAchievementPanel), "LoadData")]
        public static bool LoadData(UIAchievementPanel __instance)
        {
            __instance.uiEntries.Clear(); //Is this necessary?
            return true;
        }
    }
}