using HarmonyLib;

namespace GalacticScale.Patches
{
    public static class PatchOnGameOption
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOption), nameof(GameOption.LoadGlobal))]
        public static void LoadGlobal()
        {
            GS3.Init();
        }
    }
}