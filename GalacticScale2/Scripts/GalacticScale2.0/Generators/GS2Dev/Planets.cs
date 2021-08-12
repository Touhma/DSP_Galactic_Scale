using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GalacticScale.GS2;
using static GalacticScale.RomanNumbers;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void GeneratePlanets()
        {
            foreach (var star in GSSettings.Stars)
            {
                GeneratePlanetsForStar(star);
                NamePlanets(star);
            }
        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            GS2.Warn($"Creating Planets for {star.Name}");
            star.Planets = new GSPlanets();
            var starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            var moonChance = GetMoonChanceForStar(star);
            var moonMoonChance = preferences.GetFloat("chanceMoonMoon", 5f) / 100f;
            var moonCount = 0;
            var secondaryMoonCount = 0;
            var moons = new GSPlanets();
            if (star == birthStar)
            {
                birthPlanet = star.Planets.Add(new GSPlanet("BirthPlanet", "Mediterranean", preferences.GetInt("birthPlanetSize", 200), -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets()));
                GS2.Log($"Created Birth Planet: {birthPlanet}");
            }
            for (var i = (star == birthStar) ? 1 : 0; i < starBodyCount; i++)
            {
                if (random.NextPick(moonChance))
                {
                    moonCount++;
                }
                else
                {
                    var isGas = CalculateIsGas(star);
                    var radius = GetStarPlanetSize(star);
                    var p = new GSPlanet("planet_" + i, (isGas) ? "Gas" : "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                    if (isGas && !preferences.GetBool("hugeGasGiants", true)) p.Radius = 80;
                    if (isGas) p.Scale = 10f;
                    star.Planets.Add(p);
                    p.genData.Add("hosttype", "star");
                    p.genData.Add("hostname", star.Name);
                    GS2.Log($"Created Planet:{p}");
                }
            }

            for (var i = 0; i < moonCount; i++)
            {
                if (preferences.GetBool("secondarySatellites") && random.NextPick(moonMoonChance) && i != 0)
                {
                    secondaryMoonCount++;
                }
                else
                {
                    GSPlanet randomPlanet;
                    GSPlanets gasPlanets = new GSPlanets();
                    GSPlanets telPlanets = new GSPlanets();
                    foreach (var p in star.Planets)
                    {
                        if (p.Scale == 10f) gasPlanets.Add(p); else telPlanets.Add(p);
                    }
                    if (gasPlanets.Count > 0 && random.NextPick(preferences.GetFloat("MoonBias", 50f) / 100f))
                    {
                        randomPlanet = random.Item(gasPlanets);
                    }
                    else if (telPlanets.Count > 0)
                    {
                        randomPlanet = random.Item(telPlanets);
                    }
                    else
                    {
                        randomPlanet = random.Item(star.Planets);
                    }
                    var moon = new GSPlanet("Moon " + i, "Barren", GetStarMoonSize(star, randomPlanet.Radius, (randomPlanet.Scale == 10f)), -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                    randomPlanet.Moons.Add(moon);
                    moons.Add(moon);
                    moon.genData.Add("hosttype", "planet");
                    moon.genData.Add("hostname", randomPlanet.Name);
                    GS2.Log($"Added {moon} to {randomPlanet}");
                }
            }
            for (var i = 0; i < secondaryMoonCount; i++)
            {
                var randomMoon = random.Item(moons);
                var mm = new GSPlanet("MoonMoon" + i, "Barren", GetStarMoonSize(star, randomMoon.Radius, false), -1, -1, -1, -1, -1, -1, -1, -1, null);// {radius = GetStarMoonSize(star, randomMoon.radius, false)});
                randomMoon.Moons.Add(mm);
                mm.genData.Add("hosttype", "moon");
                mm.genData.Add("hostname", randomMoon.Name);
                GS2.Log($"Added {mm} to {randomMoon}");
            }


            GS2.WarnJson((from s in star.Planets select s.Name).ToList());
            GS2.Warn("Now Assigning Moon Orbits");
            AssignMoonOrbits(star);
            GS2.Warn("Now Assigning Planet Orbits");
            AssignPlanetOrbits(star);
            GS2.Warn("Now Assigning Themes");
            SelectPlanetThemes(star);
            GS2.Warn("Now assigning parameters");
            FudgeNumbersForPlanets(star);
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                if (IsPlanetOfStar(star, body))
                {

                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                }
                if (!IsPlanetOfStar(star, body))
                {
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 50f;
                }
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                var starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination", -1);
                // GS2.Log($"StarInc {starInc}");
                if (IsPlanetOfStar(star, body) && starInc > -1)
                {
                    // GS2.Warn($"SETTING starInc Orbit Inclination of {star.Name} to {starInc}");
                    body.OrbitInclination = random.NextFloat(starInc);
                }
                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f) * random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                if (IsPlanetOfStar(star, body) && body.OrbitRadius < preferences.GetFloat("innerPlanetDistance", 1f) && (random.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets", false)))
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (starInc == -1 && random.NextDouble() < 0.05) // Crazy Inclination
                {
                    // GS2.Warn("Setting Crazy Inclination for " + star.Name);
                    body.OrbitInclination = random.NextFloat(20f, 85f);
                }

                // Force inclinations for testing
                // body.OrbitInclination = 0f;
                // body.OrbitPhase = 0f;
                // body.OrbitalPeriod = 10000000f;
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int moonIndex)
        {
            var moons = planet.Moons;
            if (moonIndex == 0) return planet.RadiusAU + moons[moonIndex].SystemRadius;
            return moons[moonIndex - 1].SystemRadius + moons[moonIndex - 1].OrbitRadius + moons[moonIndex].SystemRadius;
        }

        private void AssignMoonOrbits(GSStar star)
        {
            // Now Work Backwards from secondary Satellites to Planets, creating orbits.
            for (var planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                var planet = star.Planets[planetIndex];
                //For each Planet
                for (var moonIndex = 0; moonIndex < planet.MoonCount; moonIndex++)
                {
                    var moon = planet.Moons[moonIndex];
                    //For Each Moon
                    for (var moon2Index = 0; moon2Index < moon.MoonCount; moon2Index++)
                    {
                        //for each subsatellite
                        var moon2 = moon.Moons[moon2Index];
                        moon2.OrbitRadius = GetMoonOrbit() / 2f + GetNextAvailableOrbit(moon, moon2Index);
                        moon2.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon2.OrbitRadius);
                    }

                    moon.OrbitRadius = GetMoonOrbit() + GetNextAvailableOrbit(planet, moonIndex);
                    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
                }
            }
            //Orbits should be set.
        }



        private float GetMoonOrbit()
        {
            return 0.01f + random.NextFloat(0f, 0.05f);
        }

        public void SelectPlanetThemes(GSStar star)
        {
            foreach (var planet in star.Planets)
            {
                if (planet == birthPlanet)
                {
                    var habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Telluric, EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200));
                    planet.Theme = habitableTheme;
                    planet.Scale = 1f;
                    continue;
                }
                var heat = CalculateThemeHeat(star, planet.OrbitRadius);
                var type = EThemeType.Planet;
                if (planet.Scale == 10f) type = EThemeType.Gas;
                planet.Theme = GSSettings.ThemeLibrary.Query(random, type, heat, planet.Radius);
                //Warn($"Planet Theme Selected. {planet.Name}:{planet.Theme} Radius:{planet.Radius * planet.Scale} {((planet.Scale == 10f) ? EThemeType.Gas : EThemeType.Planet)}");
                foreach (var body in planet.Bodies)
                    if (body != planet)
                        body.Theme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon, heat, body.Radius);
                //Warn($"Set Theme for {body.Name} to {body.Theme}");
            }
        }

        public bool CalculateIsGas(GSStar star)
        {
            var gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = (star.genData.Get("minHZ"), star.genData.Get("maxHZ"));
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < hz.max * 2) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }
    }
}

