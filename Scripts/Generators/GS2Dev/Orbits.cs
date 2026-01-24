using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator3 : iConfigurableGenerator
    {
        public void NamePlanets(GSStar star)
        {
            switch (preferences.GetString("planetNames", "default"))
            {
                case "Alpha":
                    NamePlanetsAlpha(star);
                    break;

                case "Random":
                    NamePlanetsRandom(star);
                    break;
                default:
                    NamePlanetsRoman(star);
                    break;
            }
        }

        public void NamePlanetsAlpha(GSStar star)
        {
            var i = 1;
            foreach (var planet in star.Planets)
            {
                planet.Name = $"{star.Name} {RomanNumbers.alpha[i]}";
                var j = 1;
                foreach (var moon in planet.Moons)
                {
                    moon.Name = $"{planet.Name} - {RomanNumbers.roman[j]}";
                    var h = 1;
                    foreach (var moon2 in moon.Moons)
                    {
                        moon2.Name = $"{moon.Name} - {RomanNumbers.roman[h]}";
                        h++;
                    }

                    j++;
                }

                i++;
            }
        }

        public void nameMoonsRoman(GSPlanet planet)
        {
            var i = 1;
            foreach (var moon in planet.Moons)
            {
                moon.Name = $"{planet.Name} - {RomanNumbers.roman[i]}";
                if (moon.MoonCount > 0) nameMoonsRoman(moon);
                i++;
            }
        }
        public void NamePlanetsRoman(GSStar star)
        {
            var i = 1;
            foreach (var planet in star.Planets)
            {
                planet.Name = $"{star.Name} - {RomanNumbers.roman[i]}";
                // var j = 1;
                if (planet.MoonCount > 0) nameMoonsRoman(planet);
                // foreach (var moon in planet.Moons)
                // {
                //     moon.Name = $"{planet.Name} - {RomanNumbers.roman[j]}";
                //     var h = 1;
                //     foreach (var moon2 in moon.Moons)
                //     {
                //         moon2.Name = $"{moon.Name} - {RomanNumbers.roman[h]}";
                //         h++;
                //     }
                //
                //     j++;
                // }

                i++;
            }
        }

        public void NamePlanetsRandom(GSStar star)
        {
            var availableNames = new List<string>();
            foreach (var name in PlanetNames) availableNames.Add(name);
            foreach (var planet in star.Bodies)
                if (availableNames.Count > 0)
                {
                    var name = random.Item(availableNames);
                    planet.Name = $"{star.Name} - {name}";
                    availableNames.Remove(name);
                }
        }

        private void AssignPlanetOrbits(GSStar star)
        {
            // GS2.Warn("-------------------------------------------------------------------------------");
            // GS2.Warn($"--{star.Name}-----------------------------------------------------------------------------");
            // GS2.Warn($"--{star.displayType}-----------------------------------------------------------------------------");
            // GS2.Warn($"Assigning Planet Orbits for {star.Name}:{star.Planets.Count} planets to assign");
            orbitSpacing = preferences.GetFloat("orbitSpacing", 0.05f);
            var r = new GS2.Random(star.Seed);
            var orbits = new List<Orbit>();
            ref var planets = ref star.Planets;
            var brokenPlanets = new GSPlanets();
            GSPlanet actualBirthPlanetOrHost = null;
            planets.Sort(PlanetSortBySystemRadius);
            // GS2.WarnJson((from s in planets select s.details).ToList());
            CalculateHabitableZone(star);
            var minimumOrbit = CalculateMinimumOrbit(star);
            var maximumOrbit = CalculateMaximumOrbit(star);
            // GS2.Warn($"Minimum Orbit:{minimumOrbit} Maximum Orbit:{maximumOrbit} Orbit Spacing:{orbitSpacing}");
            var freeOrbitRanges = new List<(float inner, float outer)>();
            
            foreach (var p in star.Planets)
            {
                if (p == birthPlanet)
                {
                    actualBirthPlanetOrHost = p;
                    break;
                }

                if (p.MoonCount > 0)
                {
                    foreach (var m in p.Bodies)
                    {
                        if (m == birthPlanet)
                        {
                            actualBirthPlanetOrHost = p;
                            break;
                        }
                    }
                }
                if (actualBirthPlanetOrHost != null) break;
            }

            if (star == birthStar)
            {
                if (actualBirthPlanetOrHost == null)
                {
                    GS2.Error("Failed to find Actual BirthPlanet or Host");
                }
                // else GS2.Log($"Found Actual BirthPlanet or Host = {actualBirthPlanetOrHost?.Name}");
            }
            //Warn("Orbit Count 0");
            // GS2.Log($"BirthStar = {birthStar.Name} {(birthPlanet != null ? birthPlanet.Name : "null")}");
            if (star == birthStar)
            {
                var birthRadius = Mathf.Clamp(r.NextFloat(star.genData.Get("minHZ").Float(0f), star.genData.Get("maxHZ").Float(100f)), star.RadiusAU * 1.5f, 100f);
                // GS2.Warn($"Selected Orbit {birthRadius} for planet {birthPlanet.Name}. Hz:{star.genData.Get("minHZ").Float(0f)}-{star.genData.Get("maxHZ").Float(100f)} StarRadiusAU:{star.RadiusAU}");
                var orbit = new Orbit(birthRadius);
                orbit.planets.Add(actualBirthPlanetOrHost);
                actualBirthPlanetOrHost.OrbitRadius = birthRadius;
                actualBirthPlanetOrHost.OrbitalPeriod = Utils.CalculateOrbitPeriod(actualBirthPlanetOrHost.OrbitRadius);
                orbits.Add(orbit);
                freeOrbitRanges.Clear();

                freeOrbitRanges.Add((minimumOrbit, birthRadius - actualBirthPlanetOrHost.SystemRadius * 2));
                freeOrbitRanges.Add((birthRadius + actualBirthPlanetOrHost.SystemRadius * 2, maximumOrbit));
                // GS2.LogJson(freeOrbitRanges);
            }
            else
            {
                freeOrbitRanges.Clear();
                freeOrbitRanges.Add((minimumOrbit, maximumOrbit));
            }

            // GS2.Warn($"Begin Loop:{star.Planets.Count} Planets to fit between {minimumOrbit} and {maximumOrbit}");
            for (var i = 0; i < planets.Count; i++)
            {
                Orbit orbit;
                var planet = planets[i];
                // GS2.Log($"Finding Orbit for planet index {i} - {planet.Name}");
                if (planet == actualBirthPlanetOrHost) continue;

                // Log(planet.SystemRadius.ToString());
                //planet.OrbitInclination = 0f;


                // GS2.Log($"Orbit Count > 1. Free orbit range count = {freeOrbitRanges.Count}");
                var availableOrbits = new List<(float inner, float outer)>();
                foreach (var range in freeOrbitRanges)
                {
                    // GS2.Log($"Free orbits:{range}. Checking SystemRadius:{planet.SystemRadius}. {0.05f + 2 * planet.SystemRadius}");


                    if (range.outer - range.inner >
                        2 * (planet.SystemRadius + preferences.GetFloat("orbitSpacing", 0.05f)))
                        //(1 + 1 * (GetSystemDensityBiasForStar(star) / 50)) * 2*planet.SystemRadius)
                        // GS2.Warn($"Adding {range} {preferences.GetFloat("orbitSpacing", 0.05f)} {range.outer - range.inner} {preferences.GetFloat("orbitSpacing", 0.05f) + 2 * planet.SystemRadius}");
                        availableOrbits.Add(range);
                }

                if (availableOrbits.Count == 0)
                {
                    // GS2.Warn("Free Orbit Ranges:");
                    // GS2.LogJson(freeOrbitRanges);
                    // GS2.Warn($"No Orbit Ranges found for planet {planet.Name} {planet.genData["hosttype"]} {planet.genData["hostname"]} radius:{planet.SystemRadius}");
                    var success = false;
                    foreach (var existingOrbit in orbits)
                        if (existingOrbit.hasRoom && existingOrbit.SystemRadius > planet.SystemRadius)
                        {
                            //GS2.Warn($"Existing orbit {existingOrbit.radius} used for planet {planet.Name}");
                            existingOrbit.planets.Add(planet);
                            planet.OrbitRadius = existingOrbit.radius;
                            planet.OrbitalPeriod = Utils.CalculateOrbitPeriod(planet.OrbitRadius);
                            success = true;
                            break;
                        }

                    //GS2.Log($"{planet.Name} orbit radius {planet.OrbitRadius}");
                    if (success) continue;

                    GS2.Warn($"Couldn't find an orbit for Planet {planet.Name} SysRadius:{planet.SystemRadius} HostType:{planet.genData["hosttype"]} HostName:{planet.genData["hostname"]} . Throwing planet into the sun.");

                    brokenPlanets.Add(planet);

                    // foreach (var b in brokenPlanets) b.DebugPlanetData();
                    continue;
                }
                
                if (availableOrbits.Count == 0)

                {
                    GS2.Log($"No Available Orbits Found for Planet {planet.Name}");
                    continue;
                }

                var selectedRange = r.Item(availableOrbits);
                if (preferences.GetBool("preferInnerPlanets")) selectedRange = availableOrbits[r.Next(0, Mathf.FloorToInt(availableOrbits.Count / 2f))];
                // GS2.Log($"radius = r.NextFloat({selectedRange.inner + planet.SystemRadius}, {selectedRange.outer - planet.SystemRadius})");
                var rangeMin = selectedRange.inner + preferences.GetFloat("orbitSpacing", 0.05f) + planet.SystemRadius;
                var rangeMax = selectedRange.outer - +preferences.GetFloat("orbitSpacing", 0.05f) - planet.SystemRadius;
                var radius = r.NextFloat(rangeMin, rangeMax);
                freeOrbitRanges.Remove(selectedRange);
                orbit = new Orbit(radius);
                orbit.planets.Add(planet);
                planet.OrbitRadius = radius;
                // GS2.Log($"-{planet.Name} orbit radius {planet.OrbitRadius} from {rangeMin}, {rangeMax}");

                planet.OrbitalPeriod = Utils.CalculateOrbitPeriod(planet.OrbitRadius);
                // GS2.Warn($"selected orbit({radius}) for {planet.Name}({planet.SystemRadius}) SelectedRange:{selectedRange.inner}, {selectedRange.outer} New Ranges: {selectedRange.inner},{radius - planet.SystemRadius}({radius - planet.SystemRadius - selectedRange.inner}) | {radius + planet.SystemRadius}, {selectedRange.outer}({selectedRange.outer - radius - planet.SystemRadius})");
                orbits.Add(orbit);
                var minGap = preferences.GetFloat("orbitSpacing", 0.05f);

                if (radius - planet.SystemRadius * 2 - selectedRange.inner > minGap)
                    freeOrbitRanges.Add((selectedRange.inner, radius - planet.SystemRadius * 2));
                if (selectedRange.outer - radius - planet.SystemRadius * 2 > minGap)
                    freeOrbitRanges.Add((radius + planet.SystemRadius * 2, selectedRange.outer));
            }

            foreach (var brokenPlanet in brokenPlanets)
            {
                GS2.Warn($"Removing Planet {brokenPlanet}");
                star.Planets.Remove(brokenPlanet);
            }
            //
            // // GS2.Warn($"***** Free orbit count = {freeOrbitRanges.Count}");
            // var possibleHiveOrbits = new List<float>();
            // var count = 0;
            // while (count < 20)
            // {
            //     foreach (var f in freeOrbitRanges)
            //     {
            //         // GS2.Log($"Free Orbit Range:{f.inner}:{f.outer}");
            //         possibleHiveOrbits.Add(random.ClampedNormal(Mathf.Max(star.RadiusAU+0.1f,f.inner), f.outer, 50));
            //     }
            //     count++;
            // }
            //
            // var possibleHiveOrbitsJson = Utils.Serialize(possibleHiveOrbits,false);
            // star.genData.Add("hiveOrbits", possibleHiveOrbitsJson);
            // // GS2.Log("Possible Hive Orbits:");
            // // GS2.LogJson(possibleHiveOrbits);
            //
            
            starOrbits[star] = orbits;
            star.Planets.Sort(PlanetSortByOrbit);
            orbits.Sort(OrbitSort);
            for (var i = 0; i < orbits.Count; i++)
            {
                var planets2 = orbits[i].planets;
                if (planets2.Count > 1)

                    for (var j = 0; j < planets2.Count; j++)

                    {
                        var planet = planets2[j];
                        planet.Name += $" {i}{(EAlphabet)j}";
                    }
                // else planets2[0].Name += $" {i}";
            }
            // GS2.Log($"Planets assigned {(birthPlanet != null ? birthPlanet.Name : "null")}");
        }

        private int OrbitSort(Orbit x, Orbit y)
        {
            if (x.radius == y.radius) return 0;
            if (x.radius > y.radius) return 1;
            return -1;
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
            // Log("Adjusting Orbits");
            var r = new GS2.Random(GSSettings.Seed);
            foreach (var star in GSSettings.Stars)
            {
                if (star.Decorative) continue;
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
                        planets[i].OrbitLongitude = planets[0].OrbitLongitude;
                        planets[i].OrbitInclination = planets[0].OrbitInclination;
                    }
                }
            }
        }

        public class Orbit
        {
            public Orbit next;
            public GSPlanets planets = new();
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
                    var largestRadius = 0f;
                    foreach (var planet in planets)
                        if (planet.SystemRadius > largestRadius)
                            largestRadius = planet.SystemRadius;
                    largestRadius += orbitSpacing;
                    var circumference = radius * 2 * Mathf.PI;
                    // GS2.Log($"HasRoom Circumference = {circumference} largestRadius = {largestRadius} Planet Count = {planets.Count}");
                    if (largestRadius * 2 * planets.Count < circumference) return true;
                    //if (planets.Count < 10) return true;
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
                return Mathf.Min(next.radius - next.SystemRadius - radius, radius - previous.radius - previous.SystemRadius);
            }
        }
    }

    

    
}