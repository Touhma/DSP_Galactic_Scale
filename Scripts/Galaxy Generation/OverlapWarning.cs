using System.Collections.Generic;

namespace GalacticScale
{
    /// <summary>
    ///     Post-generation check for star systems whose planetary envelopes interpenetrate
    ///     (issue #294). The runtime locality handoff (2.78.8) makes overlap playable, so this
    ///     is a heads-up, not an error: the player is told which systems overlap and what
    ///     minimum star distance would separate them, while their configured orbits are left
    ///     completely untouched.
    /// </summary>
    public static class OverlapWarning
    {
        private static readonly HashSet<string> Warned = new();

        public static void Check()
        {
            if (GS2.IsMenuDemo) return;
            if (GSSettings.Instance == null || GSSettings.Stars == null) return;
            var stars = GSSettings.Stars;
            if (stars.Count < 2) return;
            var key = $"{GSSettings.Seed}:{stars.Count}";
            if (Warned.Contains(key)) return;

            var overlaps = 0;
            GSStar worstA = null, worstB = null;
            var worstGap = 0.0;
            var worstNeed = 0.0;
            var worstHave = 0.0;
            var suggestLy = 0.0;
            for (var i = 0; i < stars.Count; i++)
            {
                var a = stars[i];
                if (a == null || a.Decorative || a.PlanetCount == 0) continue;
                for (var j = i + 1; j < stars.Count; j++)
                {
                    var b = stars[j];
                    if (b == null || b.Decorative || b.PlanetCount == 0) continue;
                    var haveLy = (a.position - b.position).magnitude;
                    // Locality spheres are (SystemRadius + 2) AU each; 60 AU = 1 ly.
                    var needLy = (a.SystemRadius + 2f + b.SystemRadius + 2f) / 60.0;
                    if (haveLy >= needLy) continue;
                    overlaps++;
                    if (needLy > suggestLy) suggestLy = needLy;
                    if (needLy - haveLy > worstGap)
                    {
                        worstGap = needLy - haveLy;
                        worstA = a;
                        worstB = b;
                        worstNeed = needLy;
                        worstHave = haveLy;
                    }
                }
            }

            if (overlaps == 0 || worstA == null) return;
            Warned.Add(key);

            var detail =
                $"{overlaps} pair(s) of star systems overlap in this galaxy. Worst: {worstA.Name} and {worstB.Name} are {worstHave:F1} ly apart, but their planetary systems span {worstNeed:F1} ly combined.";
            GS2.Warn($"OverlapWarning: {detail} (suggested min star distance ~{suggestLy:F1} ly)");

            var message =
                detail +
                "\r\n\r\nThis is playable: planets in the overlap appear and load as you approach them. You may notice system borders handing off mid-flight, and Dark Fog territories can interleave." +
                $"\r\n\r\nTo generate this seed without overlap, raise the minimum distance between stars to about {suggestLy:F1} ly, or reduce orbit distances.";
            try
            {
                UIMessageBox.Show("Overlapping Star Systems".Translate(), message.Translate(), "Noted".Translate(), 0);
            }
            catch
            {
                // Headless or too-early UI: the log line above still records it.
            }
        }
    }
}
