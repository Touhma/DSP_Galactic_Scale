using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnWarningSystem
    {
        [HarmonyPostfix, HarmonyPatch(typeof(WarningSystem), "Import")]
        public static void WarningSystem_Import(WarningSystem __instance)
        {
            if (__instance.warningCursor <= 0)
            {
                __instance.SetForNewGame();
            }
        }
    }
}