using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIStarDetail
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarDetail), "RefreshDynamicProperties")]
        public static bool RefreshDynamicProperties(ref UIStarDetail __instance)
        {
            if (__instance.star != null)
            {
                var magnitude = (__instance.star.uPosition - GameMain.mainPlayer.uPosition).magnitude;
                var num = __instance.star == GameMain.localStar ? 2 : magnitude < 14400000.0 ? 3 : 4;
                var flag = GameMain.history.universeObserveLevel >= num;
                foreach (var uiresAmountEntry in __instance.entries)
                    if (uiresAmountEntry.refId > 0)
                    {
                        if (flag)
                        {
                            var num2 = __instance.star.loaded ? __instance.star.GetResourceAmount(uiresAmountEntry.refId) : __instance.star.GetResourceSpots(uiresAmountEntry.refId);
                            if (uiresAmountEntry.refId == 7)
                            {
                                var num3 = num2 * (double)VeinData.oilSpeedMultiplier;
                                if (__instance.star.loaded)
                                {
                                    StringBuilderUtility.WritePositiveFloat(uiresAmountEntry.sb, 0, 8, (float)num3);
                                    uiresAmountEntry.DisplayStringBuilder();
                                }
                                else
                                {
                                    uiresAmountEntry.valueString = (num2 > 0L ? "探测到信号" : "无").Translate();
                                }
                            }
                            else if (__instance.star.loaded)
                            {
                                if (num2 < 1000000000L)
                                    StringBuilderUtility.WriteCommaULong(uiresAmountEntry.sb, 0, 16, (ulong)num2);
                                else
                                    StringBuilderUtility.WriteKMG(uiresAmountEntry.sb, 15, num2);
                                uiresAmountEntry.DisplayStringBuilder();
                            }
                            else
                            {
                                if (uiresAmountEntry.refId < 7)
                                    uiresAmountEntry.valueString = (num2 > 0L ? "Probable Signal" : "无").Translate();
                                else uiresAmountEntry.valueString = (num2 > 0L ? "Possible Signal" : "无").Translate();
                            }

                            uiresAmountEntry.SetObserved(true);
                        }
                        else
                        {
                            uiresAmountEntry.valueString = "未知".Translate();
                            if (uiresAmountEntry.refId > 7) uiresAmountEntry.overrideLabel = "未知珍奇信号".Translate();
                            if (uiresAmountEntry.refId > 7)
                                uiresAmountEntry.SetObserved(false);
                            else
                                uiresAmountEntry.SetObserved(true);
                        }
                    }

                if (__instance.tipEntry != null)
                {
                    if (!flag)
                        __instance.tipEntry.valueString = "宇宙探索等级".Translate() + num;
                    else
                        __instance.tipEntry.valueString = "";
                    __instance.SetResCount(flag ? __instance.entries.Count - 1 : __instance.entries.Count);
                }
            }

            return false;
        }
    }
}