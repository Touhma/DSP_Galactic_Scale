using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using UnityEngine;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void PruneOrbits(ref List<(float, float, List<GSPlanet>)> orbits, float minimumGap)
        {
            var i = 0;
            while (true)
                if (i + 1 < orbits.Count)
                    if (orbits[i + 1].Item1 - orbits[i].Item1 > minimumGap)
                        i = i + 1;
                    else
                        orbits.RemoveAt(i + 1);
                else
                    break;
        }

        private void AssignPlanetOrbits(GSStar star)
        {
            GS2.Random r = new GS2.Random(star.Seed);
            var orbits = new List<Orbit>();
            // var orbits = starOrbits[star];
            ref var planets = ref star.Planets;
            planets.Sort(PlanetSortBySystemRadius);
            // orbits.Sort(OrbitSortByAvailableSpace);
            CalculateHabitableZone(star);
            var minimumOrbit = CalculateMinimumOrbit(star);
            var maximumOrbit = CalculateMaximumOrbit(star);
            var freeOrbitRanges = new List<(float inner, float outer)>();
            for (var i = 0; i < planets.Count; i++)
            {
                
                Orbit orbit;
                var planet = planets[i];
                Log(planet.SystemRadius.ToString());
                planet.OrbitInclination = 0f;
                if (orbits.Count == 0)
                {
                    Warn("Orbit Count 0");
                    float radius;
                    if (star == birthStar) radius = Mathf.Clamp(r.NextFloat(star.genData.Get("minHZ").Float(0f), star.genData.Get("maxHZ").Float(100f)), star.RadiusAU * 1.5f, 100f);
                    else radius = r.NextFloat(minimumOrbit, maximumOrbit);
                    Warn($"Selected Orbit {radius} for planet {planet.Name}. Hz:{star.genData.Get("minHZ").Float(0f)}-{star.genData.Get("maxHZ").Float(100f)}");
                    orbit = new Orbit(radius);
                    orbit.planets.Add(planet);
                    planet.OrbitRadius = radius;
                    orbits.Add(orbit);
                    freeOrbitRanges.Clear();
                    freeOrbitRanges.Add((minimumOrbit, radius - planet.SystemRadius));
                    freeOrbitRanges.Add((radius + planet.SystemRadius, maximumOrbit));
                }
                else
                {
                    Log($"Orbit Count > 1. Free orbit range count = {freeOrbitRanges.Count}");
                    var availableOrbits = new List<(float inner, float outer)>();
                    foreach (var range in freeOrbitRanges)
                    {
                        if (range.outer - range.inner > (2*planet.SystemRadius)) availableOrbits.Add(range);
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
                    // Log($"Removing range {selectedRange.inner}, {selectedRange.outer}, total ranges available");
                    Log($"Before:{freeOrbitRanges.Count}");
                    freeOrbitRanges.Remove(selectedRange);
                    Log($"After:{freeOrbitRanges.Count}");

                    orbit = new Orbit(radius);
                    orbit.planets.Add(planet);
                    planet.OrbitRadius = radius;
                    Log($"selected orbit({radius}) for {planet.Name}({planet.SystemRadius}) SelectedRange:{selectedRange.inner}, {selectedRange.outer} New Ranges: {selectedRange.inner},{radius - planet.SystemRadius}({radius - planet.SystemRadius - selectedRange.inner}) | {radius + planet.SystemRadius}, {selectedRange.outer}({selectedRange.outer - radius - planet.SystemRadius})");
                    orbits.Add(orbit);
                    var minGap = 0.1f;
                    if (radius - planet.SystemRadius - selectedRange.inner > minGap) freeOrbitRanges.Add((selectedRange.inner, radius - planet.SystemRadius));
                    if (selectedRange.outer - radius - planet.SystemRadius > minGap) freeOrbitRanges.Add((radius + planet.SystemRadius, selectedRange.outer));
                }
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

        private int OrbitSortByAvailableSpace(Orbit x, Orbit y)
        {
            if (x.AvailableSpace() > y.AvailableSpace()) return 1;
            return -1;
        }

        private int PlanetSortBySystemRadius(GSPlanet x, GSPlanet y)
        {
            if (x.SystemRadius == y.SystemRadius) return 0;
            if (x.SystemRadius < y.SystemRadius) return 1;
            return -1;
        }

        // private Orbit SelectOrbit(GSStar star, GSPlanet planet)
        // {
        //     var orbits = starOrbits[star];
        //     Orbit orbit = null;
        //     var attempts = 0;
        //     while (orbit is null && attempts++ < 99)
        //     {
        //         Log($"Searching for orbit for {planet.Name} with sysRadius of {planet.SystemRadius}");
        //         var potentialOrbit = random.Item(orbits);
        //         Log($"Potential Orbit has {potentialOrbit.AvailableSpace()} space");
        //         if (potentialOrbit.hasRoom && potentialOrbit.AvailableSpace() > planet.SystemRadius)
        //             orbit = potentialOrbit;
        //     }
        //
        //     if (orbit is null)
        //     {
        //         Error("Could not find orbit");
        //         while (planet.MostDistantSatellite != planet)
        //         {
        //             var lastMoon = planet.Moons[planet.Moons.Count - 1];
        //             if (lastMoon.MoonCount > 0) lastMoon.Moons.Remove(lastMoon.MostDistantSatellite);
        //             else planet.Moons.Remove(lastMoon);
        //             var o = SelectOrbit(star, planet);
        //             if (o != null) return o;
        //         }
        //         // if (planet.Moons[planet.MoonCount-1].MoonCount > 0)
        //         // {
        //         //     planet.Moons[planet.MoonCount].Clear();
        //         //     var orbit2 = SelectOrbit(star, planet);
        //         //     if (orbit2 != null)
        //         //     {
        //         //         Warn($"Planet {planet.Name} had to have secondary moons removed to fit");
        //         //         return orbit2;
        //         //     }
        //         // }
        //         // if (planet.MoonCount > 0)
        //         // {
        //         //     planet.Moons.Clear();
        //         //     var orbit3 = SelectOrbit(star, planet);
        //         //     if (orbit3 is null)
        //         //     {
        //         //         Error("Couldn't find orbit after completely stripping planet");
        //         //         return new Orbit(-1);
        //         //     }
        //         //
        //         //     Warn($"Planet {planet.Name} had to have all moons removed");
        //         //     return orbit3;
        //         // }
        //
        //         return null;
        //     }
        //
        //     orbit.planets.Add(planet);
        //     Warn($"Selected orbit {orbit} for planet {planet.Name}");
        //     return orbit;
        // }

        // private void GenerateOrbits()
        // {
        //     // var minimumGap = 0.1f;
        //     foreach (var star in GSSettings.Stars)
        //     {
        //         // var r = new GS2.Random(star.Seed);
        //         var orbits = new List<Orbit>();
        //         CalculateHabitableZone(star);
        //         var minimumOrbit = CalculateMinimumOrbit(star);
        //         var maximumOrbit = CalculateMaximumOrbit(star);
        //         // var avgOrbit = minimumOrbit + (maximumOrbit - minimumOrbit) / 2f;
        //         // var sd = (maximumOrbit - minimumOrbit)/6f;
        //         for (var j = 0; j < 30; j++)
        //         {
        //             var radius = ClampedNormal(minimumOrbit, maximumOrbit, 50);
        //             orbits.Add(new Orbit(radius));
        //         }
        //
        //         orbits.Sort(OrbitComparison);
        //
        //         Log(orbits.Count.ToString());
        //         // PruneOrbits(ref orbits, minimumGap);
        //         // Log(orbits.Count.ToString());
        //         for (var i = 1; i < orbits.Count - 1; i++)
        //         {
        //             Log($"{i} {orbits[i].radius} {orbits.Count}");
        //             orbits[i].previous = orbits[i - 1];
        //             orbits[i].next = orbits[i + 1];
        //         }
        //
        //         Log("Setting 0 and max");
        //         orbits[0].next = orbits[1];
        //         orbits[orbits.Count - 1].previous = orbits[orbits.Count - 2];
        //         LogJson(orbits, true);
        //         // for (var i = 1; i < orbits.Count -1; i++)
        //         // {
        //         //     var diffPrev = orbits[i] - orbits[i - 1];
        //         //     if (diffPrev < minimumGap)
        //         //     {
        //         //         orbits[i - 1] += diffPrev / 2;
        //         //         orbits[i] = -1f;
        //         //         continue;
        //         //     } 
        //         //     var diffNext = orbits[i+1] - orbits[i];
        //         //     if (diffNext < minimumGap)
        //         //     {
        //         //         orbits[i + 1] -= diffNext / 2;
        //         //         orbits.RemoveAt(i);
        //         //     } 
        //         // }
        //         starOrbits.Add(star, orbits);
        //     }
        // }

        // private int OrbitComparison(Orbit orbit1, Orbit orbit2)
        // {
        //     if (orbit1.radius > orbit2.radius) return 1;
        //     return -1;
        // }

        private void AdjustPlanetOrbits()
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
                    switch (pCount)
                    {
                        case 5:
                            planets[1].OrbitPhase = 72 + basePhase;
                            // planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 144 + basePhase;
                            // planets[2].OrbitInclination = planets[0].OrbitInclination;

                            planets[3].OrbitPhase = 216 + basePhase;
                            // planets[3].OrbitInclination = planets[0].OrbitInclination;
                            planets[4].OrbitPhase = 288 + basePhase;
                            // planets[4].OrbitInclination = planets[0].OrbitInclination;
                            break;
                        case 6:
                            planets[1].OrbitPhase = 60 + basePhase;
                            // planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 120 + basePhase;
                            // planets[2].OrbitInclination = planets[0].OrbitInclination;

                            planets[3].OrbitPhase = 180 + basePhase;
                            // planets[3].OrbitInclination = planets[0].OrbitInclination;
                            planets[4].OrbitPhase = 240 + basePhase;
                            // planets[4].OrbitInclination = planets[0].OrbitInclination;
                            planets[5].OrbitPhase = 300 + basePhase;
                            // planets[5].OrbitInclination = planets[0].OrbitInclination;
                            break;
                        case 2:
                            // Log($"Binary planets {planets[0].Name} {planets[1].Name}");
                            planets[1].OrbitPhase = 180 + basePhase;
                            // planets[1].OrbitInclination = planets[0].OrbitInclination;
                            break;
                        case 4:
                            planets[1].OrbitPhase = 180 + basePhase;
                            // planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 90 + basePhase;
                            // planets[2].OrbitInclination = planets[0].OrbitInclination;

                            planets[3].OrbitPhase = 270 + basePhase;
                            // planets[3].OrbitInclination = planets[0].OrbitInclination;

                            break;
                        case 3:
                            planets[1].OrbitPhase = 120 + basePhase;
                            // planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 240 + basePhase;
                            // planets[2].OrbitInclination = planets[0].OrbitInclination;

                            break;
                    }
                }
            }
        }

        private void AssignOrbits()
        {
            foreach (var star in GSSettings.Stars) AssignPlanetOrbits(star);
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