using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnUIEnemyBriefInfo
    {
        // Extend the inbound-relay destination value ("Going to planet: <name>")
        // with landing ETA. relayState == 1 is the inbound flag SetBriefInfo sets.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEnemyBriefInfo), "_OnUpdate")]
        public static void _OnUpdate_LandingEta(UIEnemyBriefInfo __instance)
        {
            try
            {
                if (__instance?.enemyInfo == null || __instance.stateValueText == null)
                    return;
                if (__instance.enemyInfo.relayState != 1)
                    return;

                var sector = GameMain.data?.spaceSector ?? __instance.sector;
                if (!RelayLandingEta.TryFormat(sector, __instance.enemyInfo.enemyId, out var label))
                    return;

                __instance.stateValueText.text = RelayLandingEta.ApplySuffix(
                    __instance.stateValueText.text, "  ·  ", label);

                var size = __instance.contentSize;
                var need = __instance.stateValueText.preferredWidth + 16f;
                if (need > size.x)
                    __instance.contentSize = new Vector2(need, size.y);
            }
            catch (Exception e)
            {
                GS2.Warn("UIEnemyBriefInfo landing-ETA postfix failed: " + e.Message);
            }
        }
    }
}
