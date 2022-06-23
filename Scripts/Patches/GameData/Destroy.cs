using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "Destroy")]
        public static bool Destroy(ref GameData __instance)
        {
            PlanetModelingManager.End();
            return true;
        }
    }
}