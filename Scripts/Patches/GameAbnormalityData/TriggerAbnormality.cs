using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameAbnormalityData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(ABN.GameAbnormalityData_0925), "TriggerAbnormality")]
        public static bool TriggerAbnormality()
        {
            return false;
        }
    }
}