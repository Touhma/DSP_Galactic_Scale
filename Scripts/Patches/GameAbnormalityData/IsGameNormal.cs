using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameAbnormalityData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameAbnormalityData), "IsGameNormal")]
        public static bool IsGameNormal(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}