using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnUIRoot
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.OnGameBegin))]
        public static void OnGameBegin()
        {
            GS3.Initialized = true;
            GS3.OnMenuLoaded();
        }
    }
}