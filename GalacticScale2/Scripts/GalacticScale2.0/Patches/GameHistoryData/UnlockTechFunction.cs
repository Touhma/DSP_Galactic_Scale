using HarmonyLib;

namespace GalacticScale
{
    public static class PatchOnGameHistoryData
    {
        // public static bool UnlockTechFunction(int func, double value, int level, ref GameHistoryData __instance)
        // {
        //     if (func == 16)
        //     {
        //         __instance.logisticShipSpeedScale += (float)value *;
        //         return false;
        //     }
        //     return true;
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), "get_logisticShipWarpSpeedModified")]
        public static void get_logisticShipWarpSpeedModified(ref float __result, ref GameHistoryData __instance)
        {
            __result = __instance.logisticShipWarpSpeed * __instance.logisticShipSpeedScale* GS2.Config.LogisticsShipMulti;
        }
    }
}