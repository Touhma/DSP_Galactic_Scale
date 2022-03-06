using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameAbnormalityData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameAbnormalityData), "NotifyAbnormality")]
        public static bool NotifyAbnormality()
        {
            return false;
        }
    }
}