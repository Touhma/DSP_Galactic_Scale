using HarmonyLib;

namespace GalacticScale
{
    public static class PatchOnGameOption
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOption), "LoadGlobal")]
        public static void LoadGlobal()
        {
            GS2.Init();
        }
    }
}