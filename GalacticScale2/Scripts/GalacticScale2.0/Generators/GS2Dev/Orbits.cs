using System;
using System.Collections.Generic;
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
            var orbits = starOrbits[star];
            ref var planets = ref star.Planets;
            planets.Sort(PlanetSortBySystemRadius);
            orbits.Sort(OrbitSortByAvailableSpace);
            for (var i = 0; i < planets.Count; i++)
            {
                for (var j = 0; j < orbits.Count; j++)
                {
                    if (orbits[j].hasRoom && orbits[j].AvailableSpace() > planets[i].SystemRadius)
                    {
                        planets[i].OrbitRadius = orbits[j].radius;
                        orbits[j].planets.Add(planets[i]);
                        planets[i].OrbitalPeriod = Utils.CalculateOrbitPeriod(planets[i].OrbitRadius);
                    }
                }
            }

            starOrbits[star] = orbits;
            
        }

        private int OrbitSortByAvailableSpace(Orbit x, Orbit y)
        {
            if (x.AvailableSpace() > y.AvailableSpace()) return 1;
            return -1;
        }

        private int PlanetSortBySystemRadius(GSPlanet x, GSPlanet y)
        {
            if (x.SystemRadius > y.SystemRadius) return 1;
            return -1;
        }

        private Orbit SelectOrbit(GSStar star, GSPlanet planet)
        {
            var orbits = starOrbits[star];
            Orbit orbit = null;
            int attempts = 0;
            while (orbit is null && attempts++ < 99)
            {
                Log($"Searching for orbit for {planet.Name} with sysRadius of {planet.SystemRadius}");
                var potentialOrbit = random.Item(orbits); 
                Log($"Potential Orbit has {potentialOrbit.AvailableSpace()} space");
                if (potentialOrbit.hasRoom && potentialOrbit.AvailableSpace() > planet.SystemRadius) orbit = potentialOrbit;
            }

            if (orbit is null)
            {
                GS2.Error("Could not find orbit");
                while (planet.MostDistantSatellite != planet)
                {
                    var lastMoon = planet.Moons[planet.Moons.Count - 1];
                    if (lastMoon.MoonCount > 0) lastMoon.Moons.Remove(lastMoon.MostDistantSatellite);
                    else planet.Moons.Remove(lastMoon);
                    var o = SelectOrbit(star, planet);
                    if (o != null) return o;
                }
                // if (planet.Moons[planet.MoonCount-1].MoonCount > 0)
                // {
                //     planet.Moons[planet.MoonCount].Clear();
                //     var orbit2 = SelectOrbit(star, planet);
                //     if (orbit2 != null)
                //     {
                //         Warn($"Planet {planet.Name} had to have secondary moons removed to fit");
                //         return orbit2;
                //     }
                // }
                // if (planet.MoonCount > 0)
                // {
                //     planet.Moons.Clear();
                //     var orbit3 = SelectOrbit(star, planet);
                //     if (orbit3 is null)
                //     {
                //         Error("Couldn't find orbit after completely stripping planet");
                //         return new Orbit(-1);
                //     }
                //
                //     Warn($"Planet {planet.Name} had to have all moons removed");
                //     return orbit3;
                // }
                
                return null;
            }

            orbit.planets.Add(planet);
            Warn($"Selected orbit {orbit} for planet {planet.Name}");
            return orbit;
        }
        private void GenerateOrbits()
        {
            // var minimumGap = 0.1f;
            foreach (var star in GSSettings.Stars)
            {
                // var r = new GS2.Random(star.Seed);
                var orbits = new List<Orbit>();
                CalculateHabitableZone(star);
                var minimumOrbit = CalculateMinimumOrbit(star);
                var maximumOrbit = CalculateMaximumOrbit(star);
                // var avgOrbit = minimumOrbit + (maximumOrbit - minimumOrbit) / 2f;
                // var sd = (maximumOrbit - minimumOrbit)/6f;
                for (var j = 0; j < 30; j++)
                {
                    var radius = ClampedNormal(minimumOrbit, maximumOrbit, 50);
                    orbits.Add(new Orbit(radius));
                }

                orbits.Sort(OrbitComparison);

                Log(orbits.Count.ToString());
                // PruneOrbits(ref orbits, minimumGap);
                // Log(orbits.Count.ToString());
                for (var i = 1; i < orbits.Count -1; i++)
                {
                    Log($"{i} {orbits[i].radius} {orbits.Count}");
                    orbits[i].previous = orbits[i - 1];
                    orbits[i].next = orbits[i + 1];
                }
                Log("Setting 0 and max");
                orbits[0].next = orbits[1];
                orbits[orbits.Count - 1].previous = orbits[orbits.Count - 2];
                LogJson(orbits, true);
                // for (var i = 1; i < orbits.Count -1; i++)
                // {
                //     var diffPrev = orbits[i] - orbits[i - 1];
                //     if (diffPrev < minimumGap)
                //     {
                //         orbits[i - 1] += diffPrev / 2;
                //         orbits[i] = -1f;
                //         continue;
                //     } 
                //     var diffNext = orbits[i+1] - orbits[i];
                //     if (diffNext < minimumGap)
                //     {
                //         orbits[i + 1] -= diffNext / 2;
                //         orbits.RemoveAt(i);
                //     } 
                // }
                starOrbits.Add(star, orbits);
            }
        }

        private int OrbitComparison(Orbit orbit1, Orbit orbit2)
        {
            if (orbit1.radius > orbit2.radius) return 1;
            return -1;
        }

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
                    var basePhase = 0;//r.NextFloat() * 360;
                    planets[0].OrbitPhase = basePhase;
                    switch (pCount)
                    {
                        case 2:
                            Log($"Binary planets {planets[0].Name} {planets[1].Name}");
                            planets[1].OrbitPhase = 180 + basePhase;
                            planets[1].OrbitInclination = planets[0].OrbitInclination;
                            break;
                        case 4:
                            planets[1].OrbitPhase = 180 + basePhase;
                            planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 90 + basePhase;
                            planets[2].OrbitInclination = planets[0].OrbitInclination;

                            planets[3].OrbitPhase = 270 + basePhase;
                            planets[3].OrbitInclination = planets[0].OrbitInclination;

                            break;
                        case 3:
                            planets[1].OrbitPhase = 120 + basePhase;
                            planets[1].OrbitInclination = planets[0].OrbitInclination;

                            planets[2].OrbitPhase = 240 + basePhase;
                            planets[2].OrbitInclination = planets[0].OrbitInclination;

                            break;
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
                    if (planets.Count < 4) return true;
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
                if (next == null) return (radius - previous.radius - previous.SystemRadius);
                if (previous == null) return (next.radius - next.SystemRadius - radius);
                return Mathf.Min(next.radius - next.SystemRadius - radius,
                    radius - previous.radius - previous.SystemRadius);
            }
        }
    }
}