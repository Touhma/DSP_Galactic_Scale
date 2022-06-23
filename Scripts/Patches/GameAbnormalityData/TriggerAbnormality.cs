using ABN;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameAbnormalityData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameAbnormalityData_0925), "TriggerAbnormality")]
        public static bool TriggerAbnormality()
        {
            return false;
        }
    }
}