using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIStarmap
    {
        // Append a landing-ETA line after vanilla UpdateCursorView. The existing
        // 最快秒 player-intercept seconds are left untouched.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), nameof(UIStarmap.UpdateCursorView))]
        public static void UpdateCursorView_LandingEta(UIStarmap __instance)
        {
            try
            {
                if (__instance?.cursorViewText == null)
                    return;

                var enemyId = __instance.focusEnemyId != 0
                    ? __instance.focusEnemyId
                    : __instance.mouseHoverEnemyId;
                if (!RelayLandingEta.TryFormat(__instance.spaceSector, enemyId, out var label))
                    return;

                __instance.cursorViewText.text = RelayLandingEta.ApplySuffix(
                    __instance.cursorViewText.text, "\r\n", label);

                if (__instance.cursorViewTrans != null)
                {
                    __instance.cursorViewTrans.sizeDelta = new Vector2(
                        __instance.cursorViewText.preferredWidth * 0.5f + 44f,
                        __instance.cursorViewText.preferredHeight * 0.5f + 14f);
                    if (__instance.cursorRightDeco != null)
                    {
                        __instance.cursorRightDeco.sizeDelta = new Vector2(
                            __instance.cursorViewTrans.sizeDelta.y - 12f, 5f);
                    }
                }
            }
            catch (Exception e)
            {
                GS2.Warn("UpdateCursorView landing-ETA postfix failed: " + e.Message);
            }
        }
    }
}
