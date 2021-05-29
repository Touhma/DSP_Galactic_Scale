using HarmonyLib;
namespace GalacticScale
{
    public static class PatchOnGameBegin
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void OnGameBegin()
        {
            GS2.Initialized = true;
        }
    }
}