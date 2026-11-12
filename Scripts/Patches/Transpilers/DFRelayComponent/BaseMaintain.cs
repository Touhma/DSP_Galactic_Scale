using System.Collections.Concurrent;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnDFRelayComponent
    {
        private static readonly ConcurrentDictionary<DFRelayComponent, byte> InvalidBaseRelayWarnings = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DFRelayComponent), nameof(DFRelayComponent.LogicTickBaseMaintain))]
        public static bool LogicTickBaseMaintain_Prefix(DFRelayComponent __instance)
        {
            if (__instance == null || __instance.baseState <= 0) return true;

            var hive = __instance.hive;
            var builders = hive?.builders;
            if (builders?.buffer == null || __instance.builderId <= 0 ||
                __instance.builderId >= builders.cursor || __instance.builderId >= builders.buffer.Length)
                return SkipInvalidBaseRelay(__instance, "hive builder is unavailable");

            if (__instance.baseState == 1) return true;
            if (__instance.baseState != 2)
                return SkipInvalidBaseRelay(__instance, $"unknown base state {__instance.baseState}");

            var factories = hive.galaxy?.astrosFactory;
            var astroId = __instance.targetAstroId;
            if (factories == null || astroId < 0 || astroId >= factories.Length)
                return SkipInvalidBaseRelay(__instance, $"target astro {astroId} is unavailable");

            var enemySystem = factories[astroId]?.enemySystem;
            var bases = enemySystem?.bases;
            if (bases?.buffer == null || __instance.baseId <= 0 ||
                __instance.baseId >= bases.cursor || __instance.baseId >= bases.buffer.Length)
                return SkipInvalidBaseRelay(__instance, $"ground base {__instance.baseId} is unavailable at astro {astroId}");

            var groundBase = bases.buffer[__instance.baseId];
            if (groundBase == null || groundBase.id != __instance.baseId)
                return SkipInvalidBaseRelay(__instance, $"ground base {__instance.baseId} is stale at astro {astroId}");

            var enemyBuilders = enemySystem.builders;
            if (enemyBuilders?.buffer == null || groundBase.builderId <= 0 ||
                groundBase.builderId >= enemyBuilders.cursor || groundBase.builderId >= enemyBuilders.buffer.Length)
                return SkipInvalidBaseRelay(__instance, $"ground base {__instance.baseId} has no builder at astro {astroId}");

            return true;
        }

        private static bool SkipInvalidBaseRelay(DFRelayComponent relay, string reason)
        {
            if (InvalidBaseRelayWarnings.TryAdd(relay, 0))
                GS2.Warn($"Skipping invalid Dark Fog relay base maintenance for relay {relay.id}: {reason}");
            return false;
        }
    }
}
