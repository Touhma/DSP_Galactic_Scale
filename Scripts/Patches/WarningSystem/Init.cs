using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnWarningSystem
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarningSystem), "Init")]
        public static void Init(ref WarningSystem __instance)
        {
            var planetCount = GSSettings.PlanetCount * 2;
            if (__instance.gameData.factories.Length > planetCount) planetCount = __instance.gameData.factories.Length;
            __instance.tmpEntityPools = new EntityData[planetCount][];
            __instance.tmpPrebuildPools = new PrebuildData[planetCount][];
            __instance.tmpSignPools = new SignData[planetCount][];
            var l = GameMain.galaxy.starCount * 400;
            __instance.warningSignals = new int[65536];
            __instance.focusDetailSignals = new int[65536];
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WarningSystem), "SetWarningCapacity")]
        public static bool SetWarningCapacity(ref WarningSystem __instance, ref int newCapacity)
        {
            
            if (newCapacity == 0) {
            {
                GS2.Warn($"Setting Warning Capacity from 0 to 32");
                newCapacity = 32;
            }}
            return true;
        }

    }
}