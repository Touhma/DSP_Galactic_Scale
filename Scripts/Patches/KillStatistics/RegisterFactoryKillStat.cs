using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnKillStatistics
    {
        [HarmonyPatch(typeof(KillStatistics), nameof(KillStatistics.RegisterFactoryKillStat))]
        [HarmonyPrefix]
        public static bool RegisterFactoryKillStat(ref KillStatistics __instance, int factoryIndex)
        {
            if (DSPGame.IsMenuDemo) return true;
            if (factoryIndex >= __instance.factoryKillStatPool.Length)
            {
                var newPool = new AstroKillStat[Mathf.Max(factoryIndex+1,GSSettings.PlanetCount +1)];
                Array.Copy(__instance.factoryKillStatPool, newPool, __instance.factoryKillStatPool.Length);
                __instance.factoryKillStatPool = newPool;
            }

            return true;
        }

    }
}