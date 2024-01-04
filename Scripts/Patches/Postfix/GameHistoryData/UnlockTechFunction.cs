using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnGameHistoryData
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.logisticShipWarpSpeedModified), MethodType.Getter)]
        public static void get_logisticShipWarpSpeedModified(ref float __result, ref GameHistoryData __instance)
        {
            __result = __instance.logisticShipWarpSpeed * __instance.logisticShipSpeedScale * GS3.Config.LogisticsShipMulti;
        }
    }
}