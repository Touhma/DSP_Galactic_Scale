using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnUIOptionWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIOptionWindow), nameof(UIOptionWindow.SetTabIndex))]
        public static void SetTabIndex(int index, bool immediate, ref UIOptionWindow __instance)
        {
            if (index != SettingsUI.MainTabIndex) SettingsUI.DisableDetails();
        }
    }
}