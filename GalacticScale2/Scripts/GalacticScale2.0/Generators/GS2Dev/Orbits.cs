using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using UnityEngine;
using static GalacticScale.Generators.GS2Generator2;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void AssignPlanetOrbits(GSStar star)
        {
            GS2.Random r = new GS2.Random(star.Seed);
            var orbits = new List<Orbit>();
            ref var planets = ref star.Planets;
            planets.Sort(PlanetSortBySystemRadius);
            CalculateHabitableZone(star);
            var minimumOrbit = CalculateMinimumOrbit(star);
            var maximumOrbit = CalculateMaximumOrbit(star);
            var freeOrbitRanges = new List<(float inner, float outer)>();
         
            Warn("Orbit Count 0");
            if (star == birthStar) {
                var birthRadius = Mathf.Clamp(r.NextFloat(star.genData.Get("minHZ").Float(0f), star.genData.Get("maxHZ").Float(100f)), star.RadiusAU * 1.5f, 100f);
                Warn($"Selected Orbit {birthRadius} for planet {birthPlanet.Name}. Hz:{star.genData.Get("minHZ").Float(0f)}-{star.genData.Get("maxHZ").Float(100f)}");
                var orbit = new Orbit(birthRadius);
                orbit.planets.Add(birthPlanet);
                birthPlanet.OrbitRadius = birthRadius;
                orbits.Add(orbit);
                freeOrbitRanges.Clear();
                freeOrbitRanges.Add((minimumOrbit, birthRadius - birthPlanet.SystemRadius));
                freeOrbitRanges.Add((birthRadius + birthPlanet.SystemRadius, maximumOrbit));
            }
            else
            {
                freeOrbitRanges.Clear();
                freeOrbitRanges.Add((minimumOrbit, maximumOrbit));
            }
                for (var i = 0; i < planets.Count; i++)
                {
                    Orbit orbit;
                    var planet = planets[i];
                    Log(planet.SystemRadius.ToString());
                    planet.OrbitInclination = 0f;


                    Log($"Orbit Count > 1. Free orbit range count = {freeOrbitRanges.Count}");
                    var availableOrbits = new List<(float inner, float outer)>();
                    foreach (var range in freeOrbitRanges)
                    {
                        if (range.outer - range.inner > (2 * planet.SystemRadius)) availableOrbits.Add(range);
                    }

                    if (availableOrbits.Count == 0)
                    {
                        Warn("Free Orbit Ranges:");
                        LogJson(freeOrbitRanges);
                        Warn($"No Orbit Ranges found for planet {planet.Name} radius:{planet.SystemRadius}");
                        bool success = false;
                        foreach (var existingOrbit in orbits)
                        {
                            if (existingOrbit.hasRoom && existingOrbit.SystemRadius > planet.SystemRadius)
                            {
                                Warn($"Existing orbit {existingOrbit.radius} used for planet {planet.Name}");
                                existingOrbit.planets.Add(planet);
                                planet.OrbitRadius = existingOrbit.radius;
                                success = true;
                                break;
                            }
                        }

                        if (success) continue;
                        else
                        {
                            Warn("After all that, just couldn't find an orbit. Throwing planet into the sun.");
                            star.Planets.Remove(planet);
                            continue;
                        }
                    }
                    var selectedRange = r.Item(availableOrbits);
                    float radius = r.NextFloat(selectedRange.inner + planet.SystemRadius, selectedRange.outer - planet.SystemRadius);
                    freeOrbitRanges.Remove(selectedRange);
                    orbit = new Orbit(radius);
                    orbit.planets.Add(planet);
                    planet.OrbitRadius = radius;
                    Log($"selected orbit({radius}) for {planet.Name}({planet.SystemRadius}) SelectedRange:{selectedRange.inner}, {selectedRange.outer} New Ranges: {selectedRange.inner},{radius - planet.SystemRadius}({radius - planet.SystemRadius - selectedRange.inner}) | {radius + planet.SystemRadius}, {selectedRange.outer}({selectedRange.outer - radius - planet.SystemRadius})");
                    orbits.Add(orbit);
                    var minGap = 0.1f;
                    if (radius - planet.SystemRadius - selectedRange.inner > minGap) freeOrbitRanges.Add((selectedRange.inner, radius - planet.SystemRadius));
                    if (selectedRange.outer - radius - planet.SystemRadius > minGap) freeOrbitRanges.Add((radius + planet.SystemRadius, selectedRange.outer));

                }

            starOrbits[star] = orbits;
            star.Planets.Sort(PlanetSortByOrbit);
        }



        private int PlanetSortByOrbit(GSPlanet x, GSPlanet y)
        {
            if (x.OrbitRadius == y.OrbitRadius) return 0;
            if (x.OrbitRadius > y.OrbitRadius) return 1;
            return -1;
        }

        private int PlanetSortBySystemRadius(GSPlanet x, GSPlanet y)
        {
            if (x.SystemRadius == y.SystemRadius) return 0;
            if (x.SystemRadius < y.SystemRadius) return 1;
            return -1;
        }

        private void SetPlanetOrbitPhase()
        {
            Warn("Adjusting Orbits");
            var r = new GS2.Random(GSSettings.Seed);
            foreach (var star in GSSettings.Stars)
            {
                var orbits = starOrbits[star];
                foreach (var orbit in orbits)
                {
                    var planets = orbit.planets;
                    var pCount = planets.Count;
                    if (pCount == 0) continue;
                    var basePhase = r.NextFloat() * 360;
                    planets[0].OrbitPhase = basePhase;
                    for (var i = 1; i < pCount; i++)
                    {
                        planets[i].OrbitPhase = planets[i - 1].OrbitPhase + 360f / pCount;
                    }
                }
            }
        }

        public class Orbit
        {
            public Orbit next;
            public GSPlanets planets = new GSPlanets();
            public Orbit previous;
            public float radius;

            public Orbit(float radius)
            {
                this.radius = radius;
            }

            public bool hasRoom
            {
                get
                {
                    if (planets.Count < 6) return true;
                    return false;
                }
            }

            public float SystemRadius
            {
                get
                {
                    float sr = 0;
                    foreach (var p in planets) sr = Mathf.Max(p.SystemRadius, sr);
                    return sr;
                }
            }

            public override string ToString()
            {
                return radius.ToString();
            }

            public float AvailableSpace()
            {
                if (next == null) return radius - previous.radius - previous.SystemRadius;
                if (previous == null) return next.radius - next.SystemRadius - radius;
                return Mathf.Min(next.radius - next.SystemRadius - radius,
                    radius - previous.radius - previous.SystemRadius);
            }
        }
    }
}