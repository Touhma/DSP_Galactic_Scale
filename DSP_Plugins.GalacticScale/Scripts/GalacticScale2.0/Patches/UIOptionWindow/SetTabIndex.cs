using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), "SetTabIndex")]
        public static void SetTabIndex(int index, bool immediate, ref UIOptionWindow __instance)
        {
            if (index != SettingsUI.MainTabIndex) SettingsUI.DisableDetails();
        }
    }
}