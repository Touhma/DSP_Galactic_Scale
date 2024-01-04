using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnWarningSystem
    {
        [HarmonyPostfix, HarmonyPatch(typeof(WarningSystem), nameof(WarningSystem.Import))]
        public static void WarningSystem_Import(WarningSystem __instance)
        {
            if (__instance.warningCursor <= 0)
            {
                __instance.SetForNewGame();
            }
        }
    }
}