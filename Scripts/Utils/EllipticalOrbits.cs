using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static class EllipticalOrbits
    {
        private const double TwoPi = Math.PI * 2.0;
        private const double RuntimeTickSeconds = 1.0 / 60.0;

        public static bool TryGetElements(PlanetData planet, out GSPlanet gsPlanet)
        {
            gsPlanet = null;
            return planet != null && GS2.gsPlanets != null &&
                   GS2.gsPlanets.TryGetValue(planet.id, out gsPlanet) && gsPlanet != null &&
                   gsPlanet.OrbitEccentricity > 0.000001f;
        }

        public static void UpdateRuntimePose(PlanetData planet, double time, GSPlanet elements)
        {
            var eccentricity = Mathf.Clamp(elements.OrbitEccentricity, 0f, 0.999f);
            var periapsis = elements.OrbitArgumentOfPeriapsis;
            var anomaly = MeanAnomaly(time, planet.orbitalPeriod, planet.orbitPhase);
            var eccentricAnomaly = SolveEccentricAnomaly(anomaly, eccentricity);
            var localOffsetVector = OffsetAtEccentricAnomaly(planet.orbitRadius, eccentricity, periapsis, eccentricAnomaly);
            var localOffset = new VectorLF3(localOffsetVector.x, localOffsetVector.y, localOffsetVector.z);
            var relativePosition = Maths.QRotateLF(planet.runtimeOrbitRotation, localOffset);
            if (planet.orbitAroundPlanet != null)
            {
                relativePosition.x += planet.orbitAroundPlanet.runtimePosition.x;
                relativePosition.y += planet.orbitAroundPlanet.runtimePosition.y;
                relativePosition.z += planet.orbitAroundPlanet.runtimePosition.z;
            }

            planet.runtimeOrbitPhase = (float)NormalizeDegrees(TrueAnomaly(eccentricAnomaly, eccentricity) + periapsis);
            var rotationPhase = RotationPhase(time, planet.rotationPeriod, planet.rotationPhase);
            planet.runtimeRotationPhase = rotationPhase;
            planet.runtimePosition = relativePosition;
            planet.runtimeRotation = planet.runtimeSystemRotation * Quaternion.AngleAxis(rotationPhase, Vector3.down);
            planet.uPosition.x = planet.star.uPosition.x + relativePosition.x * 40000.0;
            planet.uPosition.y = planet.star.uPosition.y + relativePosition.y * 40000.0;
            planet.uPosition.z = planet.star.uPosition.z + relativePosition.z * 40000.0;
            planet.runtimeLocalSunDirection = Maths.QInvRotate(planet.runtimeRotation, -relativePosition);

            var nextTime = time + RuntimeTickSeconds;
            var nextAnomaly = MeanAnomaly(nextTime, planet.orbitalPeriod, planet.orbitPhase);
            var nextEccentricAnomaly = SolveEccentricAnomaly(nextAnomaly, eccentricity);
            var nextOffsetVector = OffsetAtEccentricAnomaly(planet.orbitRadius, eccentricity, periapsis, nextEccentricAnomaly);
            var nextOffset = new VectorLF3(nextOffsetVector.x, nextOffsetVector.y, nextOffsetVector.z);
            var nextRelativePosition = Maths.QRotateLF(planet.runtimeOrbitRotation, nextOffset);
            if (planet.orbitAroundPlanet != null)
            {
                nextRelativePosition.x += planet.orbitAroundPlanet.runtimePositionNext.x;
                nextRelativePosition.y += planet.orbitAroundPlanet.runtimePositionNext.y;
                nextRelativePosition.z += planet.orbitAroundPlanet.runtimePositionNext.z;
            }

            planet.runtimePositionNext = nextRelativePosition;
            planet.runtimeRotationNext = planet.runtimeSystemRotation * Quaternion.AngleAxis(
                RotationPhase(nextTime, planet.rotationPeriod, planet.rotationPhase), Vector3.down);
            planet.uPositionNext.x = planet.star.uPosition.x + nextRelativePosition.x * 40000.0;
            planet.uPositionNext.y = planet.star.uPosition.y + nextRelativePosition.y * 40000.0;
            planet.uPositionNext.z = planet.star.uPosition.z + nextRelativePosition.z * 40000.0;

            planet.galaxy.astrosData[planet.id].uPos = planet.uPosition;
            planet.galaxy.astrosData[planet.id].uRot = planet.runtimeRotation;
            planet.galaxy.astrosData[planet.id].uPosNext = planet.uPositionNext;
            planet.galaxy.astrosData[planet.id].uRotNext = planet.runtimeRotationNext;
            planet.galaxy.astrosFactory[planet.id] = planet.factory;
        }

        public static Pose PredictPose(PlanetData planet, double time, GSPlanet elements)
        {
            var eccentricity = Mathf.Clamp(elements.OrbitEccentricity, 0f, 0.999f);
            var eccentricAnomaly = SolveEccentricAnomaly(
                MeanAnomaly(time, planet.orbitalPeriod, planet.orbitPhase), eccentricity);
            var offsetVector = OffsetAtEccentricAnomaly(planet.orbitRadius, eccentricity,
                elements.OrbitArgumentOfPeriapsis, eccentricAnomaly);
            var offset = new VectorLF3(offsetVector.x, offsetVector.y, offsetVector.z);
            var position = Maths.QRotate(planet.runtimeOrbitRotation,
                new Vector3((float)offset.x, (float)offset.y, (float)offset.z));
            if (planet.orbitAroundPlanet != null)
            {
                var parentPose = planet.orbitAroundPlanet.PredictPose(time);
                position += parentPose.position;
            }

            var rotationPhase = RotationPhase(time, planet.rotationPeriod, planet.rotationPhase);
            var rotation = planet.runtimeSystemRotation * Quaternion.AngleAxis(rotationPhase, Vector3.down);
            return new Pose(position, rotation);
        }

        public static void PredictPose(PlanetData planet, double time, GSPlanet elements, out VectorLF3 position,
            out Quaternion rotation)
        {
            var eccentricity = Mathf.Clamp(elements.OrbitEccentricity, 0f, 0.999f);
            var eccentricAnomaly = SolveEccentricAnomaly(
                MeanAnomaly(time, planet.orbitalPeriod, planet.orbitPhase), eccentricity);
            var offset = OffsetAtEccentricAnomaly(planet.orbitRadius, eccentricity,
                elements.OrbitArgumentOfPeriapsis, eccentricAnomaly);
            position = Maths.QRotateLF(planet.runtimeOrbitRotation, offset);
            if (planet.orbitAroundPlanet != null)
            {
                planet.orbitAroundPlanet.PredictPose(time, out var parentPosition, out _);
                position.x += parentPosition.x;
                position.y += parentPosition.y;
                position.z += parentPosition.z;
            }

            var rotationPhase = RotationPhase(time, planet.rotationPeriod, planet.rotationPhase);
            rotation = planet.runtimeSystemRotation * Quaternion.AngleAxis(rotationPhase, Vector3.down);
        }

        public static Vector3 OffsetAtMeanAnomaly(float semiMajorAxis, float eccentricity,
            float argumentOfPeriapsis, double meanAnomalyRadians)
        {
            eccentricity = Mathf.Clamp(eccentricity, 0f, 0.999f);
            var eccentricAnomaly = SolveEccentricAnomaly(meanAnomalyRadians, eccentricity);
            var offset = OffsetAtEccentricAnomaly(semiMajorAxis, eccentricity, argumentOfPeriapsis, eccentricAnomaly);
            return new Vector3((float)offset.x, (float)offset.y, (float)offset.z);
        }

        public static Vector3 OffsetAtEccentricAnomaly(float semiMajorAxis, float eccentricity,
            float argumentOfPeriapsis, double eccentricAnomaly)
        {
            eccentricity = Mathf.Clamp(eccentricity, 0f, 0.999f);
            var x = semiMajorAxis * (Math.Cos(eccentricAnomaly) - eccentricity);
            var z = semiMajorAxis * Math.Sqrt(1.0 - eccentricity * eccentricity) * Math.Sin(eccentricAnomaly);
            var omega = argumentOfPeriapsis * Mathf.Deg2Rad;
            var cosOmega = Math.Cos(omega);
            var sinOmega = Math.Sin(omega);
            return new Vector3((float)(x * cosOmega - z * sinOmega), 0f,
                (float)(x * sinOmega + z * cosOmega));
        }

        public static void AssignGeneratedElements(GSStar star, float maximumEccentricity, float orbitSpacing,
            float innerBoundary)
        {
            maximumEccentricity = Mathf.Clamp(maximumEccentricity, 0f, 0.8f);
            if (star?.Planets == null || maximumEccentricity <= 0f) return;

            var random = new GS2.Random(GS2.Random.Mix(GSSettings.Seed, star.Seed, 0x656363));
            innerBoundary = Mathf.Max(star.RadiusAU, innerBoundary);
            AssignOrbitGroups(star.Planets, maximumEccentricity, orbitSpacing, innerBoundary, random);
            foreach (var planet in star.Planets) AssignMoonElements(planet, maximumEccentricity, random);
        }

        internal static void AssignGeneratedCometElements(GSStar star, GSPlanet comet, float maximumEccentricity,
            float orbitSpacing)
        {
            maximumEccentricity = Mathf.Clamp(maximumEccentricity, 0f, 0.8f);
            if (maximumEccentricity <= 0f) return;

            var random = new GS2.Random(GS2.Random.Mix(GSSettings.Seed, star.Seed, 0x636f6d));
            var eccentricity = random.NextFloat(0f, maximumEccentricity);
            var innerEdge = Mathf.Max(star.RadiusAU, star.OutermostOrbitRadius);
            var periapsis = Mathf.Max(comet.OrbitRadius, innerEdge + comet.RadiusAU + Mathf.Max(0f, orbitSpacing));
            comet.OrbitRadius = periapsis / (1f - eccentricity);
            comet.OrbitEccentricity = eccentricity;
            comet.OrbitArgumentOfPeriapsis = eccentricity > 0f ? random.NextFloat(0f, 360f) : 0f;
        }

        private static void AssignMoonElements(GSPlanet host, float maximumEccentricity, GS2.Random random)
        {
            if (host?.Moons == null || host.Moons.Count == 0) return;
            var spacing = Mathf.Max(0.00001f, host.RadiusAU * 0.1f);
            AssignOrbitGroups(host.Moons, maximumEccentricity, spacing, host.RadiusAU, random);
            foreach (var moon in host.Moons) AssignMoonElements(moon, maximumEccentricity, random);
        }

        private static void AssignOrbitGroups(IList<GSPlanet> planets, float maximumEccentricity, float spacing,
            float innerBoundary, GS2.Random random)
        {
            var sorted = new List<GSPlanet>(planets);
            sorted.Sort((left, right) => left.OrbitRadius.CompareTo(right.OrbitRadius));
            var groups = new List<OrbitGroup>();
            foreach (var planet in sorted)
            {
                if (groups.Count == 0 || Mathf.Abs(groups[groups.Count - 1].SemiMajorAxis - planet.OrbitRadius) > 0.00001f)
                    groups.Add(new OrbitGroup(planet.OrbitRadius));
                var group = groups[groups.Count - 1];
                group.Planets.Add(planet);
                group.SystemRadius = Mathf.Max(group.SystemRadius, planet.SystemRadius);
            }

            spacing = Mathf.Max(0f, spacing);
            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                if (group.SemiMajorAxis <= 0f)
                {
                    SetGroupElements(group, 0f, 0f);
                    continue;
                }

                var availableEccentricity = maximumEccentricity;
                if (i == 0)
                {
                    var periapsisClearance = group.SemiMajorAxis - group.SystemRadius - innerBoundary - spacing;
                    availableEccentricity = Mathf.Min(availableEccentricity,
                        Mathf.Max(0f, periapsisClearance / group.SemiMajorAxis));
                }
                else
                {
                    var previous = groups[i - 1];
                    var periapsisClearance = group.SemiMajorAxis - group.SystemRadius -
                        previous.SemiMajorAxis * (1f + previous.Eccentricity) - previous.SystemRadius - spacing;
                    availableEccentricity = Mathf.Min(availableEccentricity,
                        Mathf.Max(0f, periapsisClearance / group.SemiMajorAxis));
                }
                if (i + 1 < groups.Count)
                {
                    var next = groups[i + 1];
                    var apoapsisClearance = next.SemiMajorAxis - next.SystemRadius - group.SystemRadius - spacing -
                        group.SemiMajorAxis;
                    availableEccentricity = Mathf.Min(availableEccentricity,
                        Mathf.Max(0f, apoapsisClearance / group.SemiMajorAxis));
                }

                var eccentricity = availableEccentricity > 0f ? random.NextFloat(0f, availableEccentricity) : 0f;
                var periapsis = eccentricity > 0f ? random.NextFloat(0f, 360f) : 0f;
                SetGroupElements(group, eccentricity, periapsis);
            }
        }

        private static void SetGroupElements(OrbitGroup group, float eccentricity, float periapsis)
        {
            foreach (var planet in group.Planets)
            {
                planet.OrbitEccentricity = eccentricity;
                planet.OrbitArgumentOfPeriapsis = periapsis;
            }
            group.Eccentricity = eccentricity;
        }

        private static double MeanAnomaly(double time, double period, float phaseDegrees)
        {
            if (Math.Abs(period) < 1e-12) return phaseDegrees * Math.PI / 180.0;
            var cycles = time / period + phaseDegrees / 360.0;
            cycles -= Math.Floor(cycles);
            return NormalizeRadians(cycles * TwoPi);
        }

        private static float RotationPhase(double time, double period, float phaseDegrees)
        {
            if (Math.Abs(period) < 1e-12) return phaseDegrees;
            var cycles = time / period + phaseDegrees / 360.0;
            cycles -= Math.Floor(cycles);
            return (float)(cycles * 360.0);
        }

        private static double SolveEccentricAnomaly(double meanAnomaly, double eccentricity)
        {
            var mean = NormalizeRadians(meanAnomaly);
            if (eccentricity <= 0.000001) return mean;

            var low = -Math.PI;
            var high = Math.PI;
            var estimate = mean;
            for (var iteration = 0; iteration < 24; iteration++)
            {
                var residual = estimate - eccentricity * Math.Sin(estimate) - mean;
                if (Math.Abs(residual) < 1e-12) break;
                if (residual > 0.0) high = estimate;
                else low = estimate;

                var next = estimate - residual / (1.0 - eccentricity * Math.Cos(estimate));
                if (next <= low || next >= high) next = (low + high) * 0.5;
                estimate = next;
            }
            return estimate;
        }

        private static double TrueAnomaly(double eccentricAnomaly, double eccentricity)
        {
            var sin = Math.Sqrt(1.0 - eccentricity * eccentricity) * Math.Sin(eccentricAnomaly);
            var cos = Math.Cos(eccentricAnomaly) - eccentricity;
            return Math.Atan2(sin, cos) * 180.0 / Math.PI;
        }

        private static double NormalizeRadians(double radians)
        {
            radians %= TwoPi;
            if (radians > Math.PI) radians -= TwoPi;
            else if (radians < -Math.PI) radians += TwoPi;
            return radians;
        }

        private static double NormalizeDegrees(double degrees)
        {
            degrees %= 360.0;
            return degrees < 0.0 ? degrees + 360.0 : degrees;
        }

        private sealed class OrbitGroup
        {
            public readonly List<GSPlanet> Planets = new();
            public readonly float SemiMajorAxis;
            public float SystemRadius;
            public float Eccentricity;

            public OrbitGroup(float semiMajorAxis)
            {
                SemiMajorAxis = semiMajorAxis;
            }
        }
    }
}
