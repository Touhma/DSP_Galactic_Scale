using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnUIRoot
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void OnGameBegin()
        {
            GS2.Initialized = true;
            GS2.OnMenuLoaded();
        }
    }
}