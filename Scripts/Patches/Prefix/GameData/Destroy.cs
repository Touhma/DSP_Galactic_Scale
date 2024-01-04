using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.Destroy))]
        public static bool Destroy(ref GameData __instance)
        {
            PlanetModelingManager.End();
            return true;
        }
    }
}