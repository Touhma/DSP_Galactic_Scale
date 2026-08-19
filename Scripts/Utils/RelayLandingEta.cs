using System;

namespace GalacticScale
{
    // Landing-ETA helper for inbound DF relays. Display-only; does not touch sail logic.
    public static class RelayLandingEta
    {
        // RelaySailLogic inbound cruise clamps target uSpeed with ldc.r4 1000
        // (IL_0724 / IL_072B). IL_0E1C divides uSpeed by the same 1000 for
        // turn-rate once remaining distance >= 500. There is no named field
        // for this cap: DFRelayComponent.kCarrier_Sail_Speed is 1800 and is
        // the carrier, not the relay.
        public const float CruiseSpeedCap = 1000f;

        // Invisible marker so a postfix can replace its own suffix if the
        // host method skipped a full text rebuild this frame.
        const string SuffixToken = "\u2060";

        public static bool TryFormat(SpaceSector sector, int enemyId, out string label)
        {
            label = null;
            if (sector?.enemyPool == null || enemyId <= 0 || enemyId >= sector.enemyPool.Length)
                return false;

            var enemy = sector.enemyPool[enemyId];
            if (enemy.id != enemyId || enemy.dfRelayId <= 0)
                return false;

            var hiveIndex = enemy.originAstroId - 1000000;
            if (sector.dfHivesByAstro == null || hiveIndex < 0 || hiveIndex >= sector.dfHivesByAstro.Length)
                return false;

            var hive = sector.dfHivesByAstro[hiveIndex];
            if (hive?.relays?.buffer == null)
                return false;

            var relayId = enemy.dfRelayId;
            if (relayId < 0 || relayId >= hive.relays.buffer.Length)
                return false;

            var relay = hive.relays.buffer[relayId];
            if (relay == null || relay.id != relayId)
                return false;
            if (relay.direction <= 0 || relay.targetAstroId <= 0)
                return false;

            var galaxy = GameMain.data?.galaxy ?? GameMain.galaxy;
            var planet = galaxy?.PlanetById(relay.targetAstroId);
            if (planet == null)
                return false;

            var lpos = enemy.pos;
            sector.TransformFromAstro_ref(enemy.astroId, out var relayUPos, ref lpos);
            var remain = (relayUPos - planet.uPosition).magnitude;
            if (double.IsNaN(remain) || double.IsInfinity(remain) || remain < 0)
                return false;

            label = FormatLabel(remain / CruiseSpeedCap);
            return !string.IsNullOrEmpty(label);
        }

        public static string ApplySuffix(string existing, string separator, string label)
        {
            if (string.IsNullOrEmpty(label))
                return existing ?? "";
            if (existing == null)
                existing = "";
            var cut = existing.IndexOf(SuffixToken, StringComparison.Ordinal);
            if (cut >= 0)
                existing = existing.Substring(0, cut).TrimEnd();
            return existing + separator + SuffixToken + label;
        }

        static string FormatLabel(double seconds)
        {
            return string.Format("Lands in {0}".Translate(), FormatDuration(seconds));
        }

        static string FormatDuration(double seconds)
        {
            if (seconds < 1.0)
                return "<1s";
            var total = (int)Math.Round(seconds);
            if (total < 60)
                return total + "s";
            var hours = total / 3600;
            var minutes = total / 60 % 60;
            var secs = total % 60;
            if (hours > 0)
                return hours + ":" + minutes.ToString("D2") + ":" + secs.ToString("D2");
            return minutes + ":" + secs.ToString("D2");
        }
    }
}
