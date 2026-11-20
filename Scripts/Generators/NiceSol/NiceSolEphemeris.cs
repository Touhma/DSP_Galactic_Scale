using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GalacticScale.Generators
{
    public static class NiceSolEphemeris
    {
        public const double ParsecsToLightYears = 3.26156378;
        private const double J2000JulianDay = 2451545.0;
        private const double DegToRad = Math.PI / 180.0;
        private const double RadToDeg = 180.0 / Math.PI;
        private const double ArcsecToRad = Math.PI / (180.0 * 3600.0);

        public struct OrbitalElements
        {
            public double a;
            public double e0, eDot;
            public double I0, IDot;
            public double L0, LDot;
            public double varpi0, varpiDot;
            public double omega0, omegaDot;
        }

        public struct PlanetPoseResult
        {
            public float orbitInclination;
            public float orbitLongitude;
            public float orbitPhase;
            public float obliquity;
        }

        public static readonly Dictionary<string, OrbitalElements> PlanetElements = new(StringComparer.Ordinal)
        {
            ["Mercury"] = new OrbitalElements
            {
                a = 0.38709927, e0 = 0.20563593, eDot = 0.00001906,
                I0 = 7.00497902, IDot = -0.00594749,
                L0 = 252.25032350, LDot = 149472.67411175,
                varpi0 = 77.45779628, varpiDot = 0.16047689,
                omega0 = 48.33076593, omegaDot = -0.12534081
            },
            ["Venus"] = new OrbitalElements
            {
                a = 0.72333566, e0 = 0.00677672, eDot = -0.00004107,
                I0 = 3.39467605, IDot = -0.00078890,
                L0 = 181.97909950, LDot = 58517.81538729,
                varpi0 = 131.60246718, varpiDot = 0.00268329,
                omega0 = 76.67984255, omegaDot = -0.27769418
            },
            ["Earth"] = new OrbitalElements
            {
                a = 1.00000261, e0 = 0.01671123, eDot = -0.00004392,
                I0 = 0.0, IDot = 0.0,
                L0 = 100.46457166, LDot = 35999.37244981,
                varpi0 = 102.93768193, varpiDot = 0.32327364,
                omega0 = 0.0, omegaDot = 0.0
            },
            ["Mars"] = new OrbitalElements
            {
                a = 1.52371034, e0 = 0.09339410, eDot = 0.00007882,
                I0 = 1.84969142, IDot = -0.00813131,
                L0 = -4.55343205, LDot = 19140.30268499,
                varpi0 = -23.94362959, varpiDot = 0.44441088,
                omega0 = 49.55953891, omegaDot = -0.29257343
            },
            ["Ceres"] = new OrbitalElements
            {
                a = 2.7675, e0 = 0.07582, eDot = 0.0,
                I0 = 10.593, IDot = 0.0,
                L0 = 249.891, LDot = 7820.36,
                varpi0 = 153.902, varpiDot = 0.15,
                omega0 = 80.305, omegaDot = -0.11
            },
            ["Jupiter"] = new OrbitalElements
            {
                a = 5.20288700, e0 = 0.04838624, eDot = -0.00013253,
                I0 = 1.30439695, IDot = -0.00183714,
                L0 = 34.39644051, LDot = 3034.74612775,
                varpi0 = 14.72847983, varpiDot = 0.21252668,
                omega0 = 100.47390909, omegaDot = 0.20469106
            },
            ["Saturn"] = new OrbitalElements
            {
                a = 9.53667594, e0 = 0.05386179, eDot = -0.00050991,
                I0 = 2.48599187, IDot = 0.00193609,
                L0 = 49.95424423, LDot = 1222.49362201,
                varpi0 = 92.59887831, varpiDot = -0.41897216,
                omega0 = 113.66242448, omegaDot = -0.28867794
            },
            ["Uranus"] = new OrbitalElements
            {
                a = 19.18916464, e0 = 0.04725744, eDot = -0.00004397,
                I0 = 0.77263783, IDot = -0.00242939,
                L0 = 313.23810451, LDot = 428.48202785,
                varpi0 = 170.95427630, varpiDot = 0.40805281,
                omega0 = 74.01692503, omegaDot = 0.04240589
            },
            ["Neptune"] = new OrbitalElements
            {
                a = 30.06992276, e0 = 0.00859048, eDot = 0.00005105,
                I0 = 1.77004347, IDot = 0.00035372,
                L0 = -55.12002969, LDot = 218.45945325,
                varpi0 = 44.96476227, varpiDot = -0.32241464,
                omega0 = 131.78422574, omegaDot = -0.00508664
            },
            ["Pluto"] = new OrbitalElements
            {
                a = 39.48211675, e0 = 0.24882730, eDot = 0.00005170,
                I0 = 17.14001206, IDot = 0.00004818,
                L0 = 238.92903833, LDot = 145.20780515,
                varpi0 = 224.06891629, varpiDot = -0.04062942,
                omega0 = 110.30393684, omegaDot = -0.01183482
            }
        };

        public static bool TryParseEpochJulianDay(string input, out double jd, out string normalizedDate)
        {
            var trimmed = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(trimmed) ||
                string.Equals(trimmed, "today", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "now", StringComparison.OrdinalIgnoreCase))
            {
                var utcToday = DateTime.UtcNow.Date;
                normalizedDate = utcToday.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                jd = DateTimeToJulianDay(utcToday);
                return true;
            }

            // Support optional time component separated by space or 'T'
            var parts = trimmed.Split(new[] { 'T', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var datePart = parts[0].Replace('/', '-');
            var negativeYear = datePart.StartsWith("-", StringComparison.Ordinal);
            var unsignedDate = negativeYear ? datePart.Substring(1) : datePart;
            var segs = unsignedDate.Split('-');
            if (segs.Length == 3 &&
                int.TryParse(segs[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var yearVal) &&
                int.TryParse(segs[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var month) &&
                int.TryParse(segs[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var day) &&
                month >= 1 && month <= 12 && day >= 1 && day <= 31)
            {
                var year = negativeYear ? -yearVal : yearVal;
                double hourFrac = 0.0;
                if (parts.Length >= 2 && TimeSpan.TryParse(parts[1], CultureInfo.InvariantCulture, out var ts))
                    hourFrac = ts.TotalHours / 24.0;

                jd = GregorianJulianToJD(year, month, day) + hourFrac;
                normalizedDate = parts.Length >= 2
                    ? $"{year:D4}-{month:D2}-{day:D2} {parts[1]}"
                    : $"{year:D4}-{month:D2}-{day:D2}";
                return true;
            }

            if (DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedDt))
            {
                normalizedDate = parsedDt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                jd = DateTimeToJulianDay(parsedDt);
                return true;
            }

            var fallback = DateTime.UtcNow.Date;
            normalizedDate = fallback.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            jd = DateTimeToJulianDay(fallback);
            return false;
        }

        public static double DateTimeToJulianDay(DateTime utcDateTime)
        {
            var utc = utcDateTime.Kind == DateTimeKind.Utc ? utcDateTime : utcDateTime.ToUniversalTime();
            var dayFraction = (utc.Hour + utc.Minute / 60.0 + utc.Second / 3600.0 + utc.Millisecond / 3600000.0) / 24.0;
            return GregorianJulianToJD(utc.Year, utc.Month, utc.Day) + dayFraction;
        }

        public static double GregorianJulianToJD(int year, int month, int day)
        {
            int y = year;
            int m = month;
            if (m <= 2)
            {
                y -= 1;
                m += 12;
            }

            int b = 0;
            bool isGregorian = year > 1582 || (year == 1582 && (month > 10 || (month == 10 && day >= 15)));
            if (isGregorian)
            {
                int a = (int)Math.Floor(y / 100.0);
                b = 2 - a + (int)Math.Floor(a / 4.0);
            }

            return Math.Floor(365.25 * (y + 4716)) + Math.Floor(30.6001 * (m + 1)) + day + b - 1524.5;
        }

        public static double JulianCenturiesFromJ2000(double jd)
        {
            return (jd - J2000JulianDay) / 36525.0;
        }

        public static float NormalizeDeg(double deg)
        {
            var r = deg % 360.0;
            if (r < 0.0) r += 360.0;
            return (float)r;
        }

        public static double SolveEccentricAnomaly(double M_rad, double e)
        {
            e = Math.Max(0.0, Math.Min(0.85, e));
            var M = M_rad % (2.0 * Math.PI);
            if (M > Math.PI) M -= 2.0 * Math.PI;
            if (M < -Math.PI) M += 2.0 * Math.PI;

            var E = M + e * Math.Sin(M) * (1.0 + e * Math.Cos(M));
            for (var i = 0; i < 6; i++)
            {
                var f = E - e * Math.Sin(E) - M;
                var fp = 1.0 - e * Math.Cos(E);
                E -= f / fp;
            }
            return E;
        }

        /// <summary>
        /// Compute true 3D DSP heliocentric/planetocentric unit direction and orbital elements for a body at epoch Julian Day.
        /// Right-handed ecliptic (X_ecl, Y_ecl, Z_ecl) -> Unity left-handed DSP (X_ecl, Z_ecl, Y_ecl).
        /// </summary>
        public static PlanetPoseResult ComputePlanetPose(string bodyKey, double jd, bool isEarth = false)
        {
            var el = PlanetElements[bodyKey];
            var T = JulianCenturiesFromJ2000(jd);

            var e = Math.Max(0.0, Math.Min(0.85, el.e0 + el.eDot * T));
            var I_deg = el.I0 + el.IDot * T;
            var L_deg = el.L0 + el.LDot * T;
            var varpi_deg = el.varpi0 + el.varpiDot * T;
            var omega_deg = el.omega0 + el.omegaDot * T;

            var M_deg = L_deg - varpi_deg;
            var argPeri_deg = varpi_deg - omega_deg;

            var E = SolveEccentricAnomaly(M_deg * DegToRad, e);
            var nu = 2.0 * Math.Atan2(Math.Sqrt(1.0 + e) * Math.Sin(E * 0.5), Math.Sqrt(1.0 - e) * Math.Cos(E * 0.5));
            var u = argPeri_deg * DegToRad + nu;

            var omega = omega_deg * DegToRad;
            var inc = I_deg * DegToRad;

            var cosO = Math.Cos(omega);
            var sinO = Math.Sin(omega);
            var cosU = Math.Cos(u);
            var sinU = Math.Sin(u);
            var cosI = Math.Cos(inc);
            var sinI = Math.Sin(inc);

            // Right-handed ecliptic coordinates of unit position vector
            var xEcl = cosO * cosU - sinO * sinU * cosI;
            var yEcl = sinO * cosU + cosO * sinU * cosI;
            var zEcl = sinU * sinI;

            // Convert to Unity left-handed DSP: (X_ecl, Z_ecl, Y_ecl)
            var invNorm = 1.0 / Math.Sqrt(xEcl * xEcl + yEcl * yEcl + zEcl * zEcl);
            var xDsp = xEcl * invNorm;
            var yDsp = zEcl * invNorm;
            var zDsp = yEcl * invNorm;

            float orbitInclination;
            float orbitLongitude;
            float obliquity = 0f;

            if (isEarth)
            {
                orbitInclination = 0f;
                // Aligns Unity axial tilt Quaternion.AngleAxis(90, up) * Quaternion.AngleAxis(eps, forward) * up
                // to (0, cos eps, +sin eps) in DSP, pointing directly to Polaris and keeping OrbitLongitude >= 0!
                orbitLongitude = 90f;
                obliquity = (float)Math.Max(21.5, Math.Min(25.0, 23.4392911 - 0.0130042 * T));
            }
            else
            {
                orbitInclination = (float)Math.Abs(I_deg);
                if (orbitInclination < 1e-4f)
                {
                    orbitInclination = 0f;
                    orbitLongitude = 0f;
                }
                else
                {
                    // Account for sign if secular IDot ever flipped I_deg negative
                    var effectiveOmega = I_deg >= 0 ? omega_deg : omega_deg + 180.0;
                    orbitLongitude = NormalizeDeg(270.0 - effectiveOmega);
                }
            }

            var phase = ComputeOrbitPhaseDeg(orbitLongitude, orbitInclination, xDsp, yDsp, zDsp);
            return new PlanetPoseResult
            {
                orbitInclination = orbitInclination,
                orbitLongitude = orbitLongitude,
                orbitPhase = phase,
                obliquity = obliquity
            };
        }

        /// <summary>
        /// Compute orbitLongitude and orbitPhase given mean ecliptic elements relative to ecliptic plane.
        /// Note: DSP rotates moons using runtimeOrbitRotation = AngleAxis(orbitLongitude, up) * AngleAxis(orbitInclination, forward)
        /// directly (CreatePlanet.cs:91 does NOT multiply by host planet's runtimeOrbitRotation).
        /// </summary>
        public static PlanetPoseResult ComputeBodyPoseFromAngles(double incDeg, double omegaDeg, double meanLonDeg)
        {
            var cosO = Math.Cos(omegaDeg * DegToRad);
            var sinO = Math.Sin(omegaDeg * DegToRad);
            var u = (meanLonDeg - omegaDeg) * DegToRad;
            var cosU = Math.Cos(u);
            var sinU = Math.Sin(u);
            var cosI = Math.Cos(incDeg * DegToRad);
            var sinI = Math.Sin(incDeg * DegToRad);

            var xEcl = cosO * cosU - sinO * sinU * cosI;
            var yEcl = sinO * cosU + cosO * sinU * cosI;
            var zEcl = sinU * sinI;

            var invNorm = 1.0 / Math.Sqrt(xEcl * xEcl + yEcl * yEcl + zEcl * zEcl);
            var xDsp = xEcl * invNorm;
            var yDsp = zEcl * invNorm;
            var zDsp = yEcl * invNorm;

            var orbitInclination = (float)Math.Abs(incDeg);
            var effectiveOmega = incDeg >= 0 ? omegaDeg : omegaDeg + 180.0;
            var orbitLongitude = orbitInclination < 1e-4f ? 0f : NormalizeDeg(270.0 - effectiveOmega);
            var phase = ComputeOrbitPhaseDeg(orbitLongitude, orbitInclination, xDsp, yDsp, zDsp);

            return new PlanetPoseResult
            {
                orbitInclination = orbitInclination,
                orbitLongitude = orbitLongitude,
                orbitPhase = phase,
                obliquity = 0f
            };
        }

        public static float ComputeOrbitPhaseDeg(float orbitLongitude, float orbitInclination, Vector3 targetDirDsp)
        {
            return ComputeOrbitPhaseDeg((double)orbitLongitude, (double)orbitInclination, targetDirDsp.x, targetDirDsp.y, targetDirDsp.z);
        }

        public static float ComputeOrbitPhaseDeg(double orbitLongitudeDeg, double orbitInclinationDeg, double xDsp, double yDsp, double zDsp)
        {
            var cosL = Math.Cos(orbitLongitudeDeg * DegToRad);
            var sinL = Math.Sin(orbitLongitudeDeg * DegToRad);
            var cosI = Math.Cos(orbitInclinationDeg * DegToRad);
            var sinI = Math.Sin(orbitInclinationDeg * DegToRad);

            var localX = xDsp * cosI * cosL + yDsp * sinI - zDsp * cosI * sinL;
            var localZ = xDsp * sinL + zDsp * cosL;
            return NormalizeDeg(Math.Atan2(localZ, localX) * RadToDeg);
        }

        public static PlanetPoseResult ComputeLunaPose(double jd)
        {
            var T = JulianCenturiesFromJ2000(jd);
            var omega = 125.04452 - 1934.136261 * T;
            var lMean = 218.3164591 + 481267.88134236 * T;
            var mMoon = 134.96298 + 477198.867398 * T;
            var lon = lMean + 6.289 * Math.Sin(mMoon * DegToRad);
            var pose = ComputeBodyPoseFromAngles(5.145, omega, lon);
            pose.obliquity = 6.68f;
            return pose;
        }

        /// <summary>
        /// Compute Earth's initial Greenwich / Solar rotation phase approximation at the UTC hour of jd.
        /// At midnight UTC (frac=0), Sun direction aligns with Greenwich midnight; keep deterministic continuous phase.
        /// </summary>
        public static float ComputeEarthRotationPhase(double jd, float earthOrbitPhase)
        {
            var utHours = ((jd - 0.5) % 1.0 + 1.0) % 1.0 * 360.0;
            return NormalizeDeg(utHours + earthOrbitPhase);
        }

        /// <summary>
        /// Compute DSP star position (light-years, Unity left-handed Ecliptic of Date frame)
        /// from J2000 Equatorial position (pc) and space velocity (pc/yr), applying:
        /// 1. 3D space motion to Epoch T
        /// 2. Lieske (1979) general equatorial precession from J2000 to Date T (bounded for trig stability)
        /// 3. Equatorial -> Right-Handed Ecliptic (mean obliquity eps(T)) -> Unity Left-Handed (X, Z, Y)
        /// 4. Smooth radial log-power compression into [minDist, maxDist] preserving exact unit direction
        /// </summary>
        public static VectorLF3 ComputeStarDirectionAndDistance(
            double xPc, double yPc, double zPc,
            double vxPcYr, double vyPcYr, double vzPcYr,
            double jd, double minDistLy, double maxDistLy,
            out VectorLF3 unitDirDsp, out double rawDistLy)
        {
            var T = JulianCenturiesFromJ2000(jd);
            var dtYears = T * 100.0;

            // Clamp space motion dt to +-50,000 years so linear velocity doesn't overshoot Solar neighborhood nonlinearly,
            // while supporting multi-millennia precession tests cleanly.
            var clampedYears = Math.Max(-50000.0, Math.Min(50000.0, dtYears));
            var px = xPc + vxPcYr * clampedYears;
            var py = yPc + vyPcYr * clampedYears;
            var pz = zPc + vzPcYr * clampedYears;

            rawDistLy = Math.Sqrt(px * px + py * py + pz * pz) * ParsecsToLightYears;
            if (rawDistLy < 1e-6)
            {
                unitDirDsp = VectorLF3.zero;
                return VectorLF3.zero;
            }

            // Lieske (1979) precession angles from J2000.0 to epoch T
            // Clamp polynomial T only for absurd out-of-range millennia (>500 centuries = 50,000 yr)
            var tPrec = Math.Max(-500.0, Math.Min(500.0, T));
            var t2 = tPrec * tPrec;
            var t3 = t2 * tPrec;

            var zeta = (2306.2181 * tPrec + 0.30188 * t2 + 0.017998 * t3) * ArcsecToRad;
            var zA = (2306.2181 * tPrec + 1.09468 * t2 + 0.018203 * t3) * ArcsecToRad;
            var theta = (2004.3109 * tPrec - 0.42665 * t2 - 0.041833 * t3) * ArcsecToRad;

            var cZeta = Math.Cos(zeta); var sZeta = Math.Sin(zeta);
            var cTheta = Math.Cos(theta); var sTheta = Math.Sin(theta);
            var cZa = Math.Cos(zA); var sZa = Math.Sin(zA);

            // Standard IAU precession matrix P = Rz(-zA) * Ry(+theta) * Rz(-zeta)
            var m00 = cZa * cTheta * cZeta - sZa * sZeta;
            var m01 = -cZa * cTheta * sZeta - sZa * cZeta;
            var m02 = -cZa * sTheta;

            var m10 = sZa * cTheta * cZeta + cZa * sZeta;
            var m11 = -sZa * cTheta * sZeta + cZa * cZeta;
            var m12 = -sZa * sTheta;

            var m20 = sTheta * cZeta;
            var m21 = -sTheta * sZeta;
            var m22 = cTheta;

            var xEq = m00 * px + m01 * py + m02 * pz;
            var yEq = m10 * px + m11 * py + m12 * pz;
            var zEq = m20 * px + m21 * py + m22 * pz;

            // Mean obliquity of the ecliptic
            var epsRad = (23.4392911 - 0.0130042 * Math.Max(-100.0, Math.Min(100.0, T))) * DegToRad;
            var cEps = Math.Cos(epsRad);
            var sEps = Math.Sin(epsRad);

            var xEcl = xEq;
            var yEcl = yEq * cEps + zEq * sEps;
            var zEcl = -yEq * sEps + zEq * cEps;

            // Map Right-Handed Ecliptic (X, Y, Z) -> Unity Left-Handed DSP (X_ecl, Z_ecl, Y_ecl)
            var norm = Math.Sqrt(xEcl * xEcl + yEcl * yEcl + zEcl * zEcl);
            unitDirDsp = new VectorLF3(xEcl / norm, zEcl / norm, yEcl / norm);

            var targetDist = CompressDistanceLy(rawDistLy, minDistLy, maxDistLy);
            return new VectorLF3(unitDirDsp.x * targetDist, unitDirDsp.y * targetDist, unitDirDsp.z * targetDist);
        }

        public static double CompressDistanceLy(double rawDistLy, double minDistLy, double maxDistLy)
        {
            if (rawDistLy <= 0.0) return 0.0;
            const double refKnee = 10.0;
            const double maxOpticalRef = 12000.0;
            var denom = Math.Log(1.0 + maxOpticalRef / refKnee);
            var t = Math.Log(1.0 + Math.Min(rawDistLy, maxOpticalRef) / refKnee) / denom;
            var scaled = minDistLy + (maxDistLy - minDistLy) * Math.Pow(t, 1.12);
            if (rawDistLy > maxOpticalRef)
                scaled += 4.0 * Math.Tanh((rawDistLy - maxOpticalRef) / 20000.0);
            return scaled;
        }

        /// <summary>
        /// Resolve any 3D overlap along the EXACT same sky ray unitDir so constellation angles from Sol remain 100% invariant.
        /// </summary>
        public static VectorLF3 ResolveRayCollision(VectorLF3 unitDir, double targetRadius, List<GSStar> existingStars, double minSepLy = 1.8)
        {
            var candidate = new VectorLF3(unitDir.x * targetRadius, unitDir.y * targetRadius, unitDir.z * targetRadius);
            if (!CollidesWithAny(candidate, existingStars, minSepLy)) return candidate;

            for (var step = 1; step <= 200; step++)
            {
                var delta = step * 0.45;
                var rOut = targetRadius + delta;
                var cOut = new VectorLF3(unitDir.x * rOut, unitDir.y * rOut, unitDir.z * rOut);
                if (!CollidesWithAny(cOut, existingStars, minSepLy)) return cOut;

                var rIn = targetRadius - delta;
                if (rIn >= 3.5)
                {
                    var cIn = new VectorLF3(unitDir.x * rIn, unitDir.y * rIn, unitDir.z * rIn);
                    if (!CollidesWithAny(cIn, existingStars, minSepLy)) return cIn;
                }
            }

            // Fallback past outer rim along exact ray
            var rFallback = targetRadius + 95.0;
            return new VectorLF3(unitDir.x * rFallback, unitDir.y * rFallback, unitDir.z * rFallback);
        }

        private static bool CollidesWithAny(VectorLF3 pos, List<GSStar> existingStars, double minSepLy)
        {
            var minSqr = minSepLy * minSepLy;
            for (var i = 0; i < existingStars.Count; i++)
            {
                var other = existingStars[i].position;
                var dx = pos.x - other.x;
                var dy = pos.y - other.y;
                var dz = pos.z - other.z;
                if (dx * dx + dy * dy + dz * dz < minSqr) return true;
            }
            return false;
        }
    }
}
