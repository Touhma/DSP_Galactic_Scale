using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameAbnormalityData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameAbnormalityData), "IsAbnormalityChecked")]
        public static bool IsAbnormalityChecked(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}