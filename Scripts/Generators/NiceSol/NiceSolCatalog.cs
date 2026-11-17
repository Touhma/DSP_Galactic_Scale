using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GSSerializer;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class ConstellationStarEntry
    {
        public string nameEn;
        public string nameZh;
        public string con;
        public string starType;
        public string spectr;
        public double x;
        public double y;
        public double z;
        public double vx;
        public double vy;
        public double vz;
        public float mag;
        public float lum;
        public float temp;
        public float mass;
        public float radius;
    }

    public static class NiceSolCatalog
    {
        private const string ResourceName = "GalacticScale.Assets.constellations.json";
        private static List<ConstellationStarEntry> _cachedEntries;

        public static List<ConstellationStarEntry> LoadEntries()
        {
            if (_cachedEntries != null) return _cachedEntries;

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(ResourceName);
            if (stream == null)
            {
                GS2.Error($"Embedded resource '{ResourceName}' not found.");
                _cachedEntries = new List<ConstellationStarEntry>();
                return _cachedEntries;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var data = fsJsonParser.Parse(json);
            var serializer = new fsSerializer();
            var parsed = new List<ConstellationStarEntry>();
            var result = serializer.TryDeserialize(data, ref parsed);
            if (result.Failed)
            {
                GS2.Error($"Failed to deserialize {ResourceName}: {result.FormattedMessages}");
                _cachedEntries = new List<ConstellationStarEntry>();
                return _cachedEntries;
            }

            _cachedEntries = parsed ?? new List<ConstellationStarEntry>();
            return _cachedEntries;
        }

        public static EStarType ParseStarType(string starType)
        {
            switch (starType)
            {
                case "BlackHole": return EStarType.BlackHole;
                case "NeutronStar": return EStarType.NeutronStar;
                case "WhiteDwarf": return EStarType.WhiteDwarf;
                case "GiantStar": return EStarType.GiantStar;
                default: return EStarType.MainSeqStar;
            }
        }

        public static ESpectrType ParseSpectrType(string spectr)
        {
            if (string.IsNullOrEmpty(spectr)) return ESpectrType.G;
            switch (spectr[0])
            {
                case 'O': return ESpectrType.O;
                case 'B': return ESpectrType.B;
                case 'A': return ESpectrType.A;
                case 'F': return ESpectrType.F;
                case 'G': return ESpectrType.G;
                case 'K': return ESpectrType.K;
                case 'M': return ESpectrType.M;
                default: return ESpectrType.X;
            }
        }

        private static bool IsOMainSeq(ConstellationStarEntry entry)
        {
            return entry != null && entry.starType == "MainSeqStar" && entry.spectr == "O";
        }

        private static void ComputeQuotas(int limit, out int minBH, out int minNS, out int minWD, out int minO)
        {
            minBH = limit >= 64 ? 2 : (limit >= 3 ? 1 : 0);
            minNS = limit >= 64 ? 2 : (limit >= 4 ? 1 : 0);
            minWD = limit >= 64 ? 2 : (limit >= 6 ? 1 : 0);
            minO = limit >= 8 ? 1 : 0;
        }

        public static List<ConstellationStarEntry> SelectEntries(int maxCount)
        {
            return SelectEntries(LoadEntries(), maxCount);
        }

        /// <summary>
        /// Select up to maxCount stars in priority order while dynamically injecting Black Holes,
        /// Neutron Stars, White Dwarfs, and at least one O-type main-sequence star for smaller star counts.
        /// </summary>
        public static List<ConstellationStarEntry> SelectEntries(List<ConstellationStarEntry> entries, int maxCount)
        {
            if (entries == null || entries.Count == 0 || maxCount <= 0) return new List<ConstellationStarEntry>();

            var limit = Math.Min(Math.Max(1, maxCount), entries.Count);
            var selected = entries.GetRange(0, limit);
            if (limit == entries.Count) return selected;

            var selectedNames = new HashSet<string>(StringComparer.Ordinal);
            int bhCount = 0, nsCount = 0, wdCount = 0, oCount = 0;
            for (var i = 0; i < selected.Count; i++)
            {
                var e = selected[i];
                if (!string.IsNullOrEmpty(e.nameEn)) selectedNames.Add(e.nameEn);
                if (e.starType == "BlackHole") bhCount++;
                else if (e.starType == "NeutronStar") nsCount++;
                else if (e.starType == "WhiteDwarf") wdCount++;
                else if (IsOMainSeq(e)) oCount++;
            }

            ComputeQuotas(limit, out var minBH, out var minNS, out var minWD, out var minO);

            var toInject = new List<ConstellationStarEntry>();
            CollectMissing(entries, selectedNames, e => e.starType == "BlackHole", minBH - bhCount, toInject);
            CollectMissing(entries, selectedNames, e => e.starType == "NeutronStar", minNS - nsCount, toInject);
            CollectMissing(entries, selectedNames, e => e.starType == "WhiteDwarf", minWD - wdCount, toInject);
            CollectMissing(entries, selectedNames, IsOMainSeq, minO - oCount, toInject);

            if (toInject.Count > 0)
            {
                // Account for injected counts so protected slots stay exact while tail stars give way
                bhCount += CountMatching(toInject, e => e.starType == "BlackHole");
                nsCount += CountMatching(toInject, e => e.starType == "NeutronStar");
                wdCount += CountMatching(toInject, e => e.starType == "WhiteDwarf");
                oCount += CountMatching(toInject, IsOMainSeq);

                var removeIndices = new List<int>(toInject.Count);
                for (var idx = selected.Count - 1; idx >= 2 && removeIndices.Count < toInject.Count; idx--)
                {
                    var e = selected[idx];
                    if (e.starType == "BlackHole")
                    {
                        if (bhCount <= minBH) continue;
                        bhCount--;
                    }
                    else if (e.starType == "NeutronStar")
                    {
                        if (nsCount <= minNS) continue;
                        nsCount--;
                    }
                    else if (e.starType == "WhiteDwarf")
                    {
                        if (wdCount <= minWD) continue;
                        wdCount--;
                    }
                    else if (IsOMainSeq(e))
                    {
                        if (oCount <= minO) continue;
                        oCount--;
                    }

                    removeIndices.Add(idx);
                }

                // Remove ascending/sorted? We collected descending, so RemoveAt removes cleanly without shifting earlier indices
                for (var i = 0; i < removeIndices.Count; i++)
                    selected.RemoveAt(removeIndices[i]);

                for (var i = 0; i < toInject.Count && selected.Count < limit; i++)
                    selected.Add(toInject[i]);
            }

            return selected;
        }

        private static void CollectMissing(
            List<ConstellationStarEntry> entries,
            HashSet<string> selectedNames,
            Func<ConstellationStarEntry, bool> predicate,
            int needed,
            List<ConstellationStarEntry> toInject)
        {
            if (needed <= 0) return;
            for (var i = 0; i < entries.Count && needed > 0; i++)
            {
                var e = entries[i];
                if (!predicate(e)) continue;
                if (!string.IsNullOrEmpty(e.nameEn) && selectedNames.Contains(e.nameEn)) continue;
                selectedNames.Add(e.nameEn);
                toInject.Add(e);
                needed--;
            }
        }

        private static int CountMatching(List<ConstellationStarEntry> list, Func<ConstellationStarEntry, bool> predicate)
        {
            var count = 0;
            for (var i = 0; i < list.Count; i++)
                if (predicate(list[i])) count++;
            return count;
        }

        private static void EnforceInaccurateRemnantQuota(
            List<ConstellationStarEntry> selected,
            EStarType[] starTypes,
            ESpectrType[] spectrTypes,
            EStarType targetType,
            int targetMin,
            int minBH,
            int minNS,
            int minWD)
        {
            if (targetMin <= 0) return;

            int count = 0, bh = 0, ns = 0, wd = 0;
            for (var i = 0; i < starTypes.Length; i++)
            {
                if (starTypes[i] == targetType) count++;
                if (starTypes[i] == EStarType.BlackHole) bh++;
                else if (starTypes[i] == EStarType.NeutronStar) ns++;
                else if (starTypes[i] == EStarType.WhiteDwarf) wd++;
            }

            if (count >= targetMin) return;

            for (var pass = 0; pass < 2 && count < targetMin; pass++)
            {
                for (var idx = starTypes.Length - 1; idx >= 2 && count < targetMin; idx--)
                {
                    if (starTypes[idx] == targetType) continue;
                    // Pass 0 prefers slots whose underlying catalog entry actually is targetType
                    if (pass == 0 && ParseStarType(selected[idx].starType) != targetType) continue;

                    if (starTypes[idx] == EStarType.BlackHole && bh <= minBH) continue;
                    if (starTypes[idx] == EStarType.NeutronStar && ns <= minNS) continue;
                    if (starTypes[idx] == EStarType.WhiteDwarf && wd <= minWD) continue;

                    if (starTypes[idx] == EStarType.BlackHole) bh--;
                    else if (starTypes[idx] == EStarType.NeutronStar) ns--;
                    else if (starTypes[idx] == EStarType.WhiteDwarf) wd--;

                    starTypes[idx] = targetType;
                    spectrTypes[idx] = ESpectrType.X;
                    count++;
                    if (targetType == EStarType.BlackHole) bh++;
                    else if (targetType == EStarType.NeutronStar) ns++;
                    else if (targetType == EStarType.WhiteDwarf) wd++;
                }
            }
        }

        /// <summary>
        /// Build GSStar instances from the embedded 88-constellation catalog for a given epoch Julian Day,
        /// radial compression ceiling, language setting, and accuracy mode.
        /// </summary>
        public static GSStars BuildStars(double jd, double maxStarDistanceLy, bool useChinese, bool accurateStars, int baseSeed, int maxCount)
        {
            var selected = SelectEntries(LoadEntries(), maxCount);
            var limit = selected.Count;
            var result = new GSStars();
            if (limit == 0) return result;

            var placedList = new List<GSStar>(limit);
            var rng = new GS2.Random(baseSeed ^ 0x4E696365);

            var starTypes = new EStarType[limit];
            var spectrTypes = new ESpectrType[limit];

            for (var i = 0; i < limit; i++)
            {
                var entry = selected[i];
                var starType = ParseStarType(entry.starType);
                var spectr = ParseSpectrType(entry.spectr);

                if (i > 0 && !accurateStars)
                {
                    if (rng.NextPick(0.05))
                    {
                        var roll = rng.NextDouble();
                        if (roll < 0.2) starType = EStarType.NeutronStar;
                        else if (roll < 0.4) starType = EStarType.BlackHole;
                        else if (roll < 0.8) starType = EStarType.WhiteDwarf;
                        else starType = EStarType.GiantStar;
                        spectr = starType == EStarType.GiantStar ? ESpectrType.K : ESpectrType.X;
                    }
                    else
                    {
                        starType = EStarType.MainSeqStar;
                        var chance = rng.NextDouble();
                        if (chance < 0.06) spectr = ESpectrType.M;
                        else if (chance < 0.24) spectr = ESpectrType.K;
                        else if (chance < 0.44) spectr = ESpectrType.G;
                        else if (chance < 0.53) spectr = ESpectrType.F;
                        else if (chance < 0.59) spectr = ESpectrType.A;
                        else if (chance < 0.95) spectr = ESpectrType.B;
                        else spectr = ESpectrType.O;
                    }
                }

                starTypes[i] = starType;
                spectrTypes[i] = spectr;
            }

            if (!accurateStars && limit >= 3)
            {
                ComputeQuotas(limit, out var minBH, out var minNS, out var minWD, out _);
                EnforceInaccurateRemnantQuota(selected, starTypes, spectrTypes, EStarType.BlackHole, minBH, minBH, minNS, minWD);
                EnforceInaccurateRemnantQuota(selected, starTypes, spectrTypes, EStarType.NeutronStar, minNS, minBH, minNS, minWD);
                EnforceInaccurateRemnantQuota(selected, starTypes, spectrTypes, EStarType.WhiteDwarf, minWD, minBH, minNS, minWD);
            }

            for (var i = 0; i < limit; i++)
            {
                var entry = selected[i];
                var starSeed = GS2.Random.Mix(baseSeed, i + 1, 0x4E696365) & 0x7fffffff;
                if (starSeed == 0) starSeed = i + 1;

                var starType = starTypes[i];
                var spectr = spectrTypes[i];
                var starName = useChinese ? entry.nameZh : entry.nameEn;
                var star = new GSStar(starSeed, starName, spectr, starType, new GSPlanets());

                if (i == 0)
                {
                    star.position = VectorLF3.zero;
                    star.mass = 1.0f;
                    star.radius = 1.0f;
                    star.luminosity = 1.0f;
                    star.temperature = 5778f;
                }
                else
                {
                    var scaledPos = NiceSolEphemeris.ComputeStarDirectionAndDistance(
                        entry.x, entry.y, entry.z,
                        entry.vx, entry.vy, entry.vz,
                        jd, 3.8, maxStarDistanceLy,
                        out var unitDir, out _);

                    var resolvedPos = NiceSolEphemeris.ResolveRayCollision(unitDir, scaledPos.magnitude, placedList, 1.8);
                    star.position = resolvedPos;

                    // Let StarDefaults handle compact remnants (BlackHole, NeutronStar, WhiteDwarf)
                    // so accretion discs, dyson radii, temperatures, and masses match DSP visuals cleanly.
                    if (starType == EStarType.MainSeqStar || starType == EStarType.GiantStar)
                    {
                        if (accurateStars)
                        {
                            star.mass = Mathf.Clamp(entry.mass, 0.35f, 55f);
                            star.radius = starType == EStarType.GiantStar
                                ? Mathf.Clamp(entry.radius, 10f, 36f)
                                : Mathf.Clamp(entry.radius, 0.45f, 5.5f);
                            star.luminosity = Mathf.Clamp(Mathf.Pow(Mathf.Max(0.0005f, entry.lum), 0.24f), 0.12f, 18.0f);
                            if (entry.temp > 0f) star.temperature = entry.temp;
                        }
                    }
                }

                placedList.Add(star);
                result.Add(star);
            }

            return result;
        }
    }
}
