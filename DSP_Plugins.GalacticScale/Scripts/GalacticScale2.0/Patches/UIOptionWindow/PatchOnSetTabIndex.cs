using HarmonyLib;
using System.Reflection;

namespace GalacticScale
{
    public static class PatchOnSetTabIndex {
        [HarmonyPatch]
        static MethodBase TargetMethod() => AccessTools.Method(typeof(UIOptionWindow), "SetTabIndex");
      
        public static bool Prefix(int index, bool immediate, ref UIOptionWindow __instance)
        {
            if (index != SettingsUI.MainTabIndex) SettingsUI.DisableDetails(); return true;
        }
    }
}