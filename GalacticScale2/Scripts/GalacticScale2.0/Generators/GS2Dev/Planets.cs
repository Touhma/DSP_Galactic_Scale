using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void GeneratePlanets()
        {
            foreach (var star in GSSettings.Stars)
            {
                if (!star.Decorative) GeneratePlanetsForStar(star);
                if (!star.Decorative) NamePlanets(star);
            }
        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            star.Planets = new GSPlanets();
            // Warn($"Creating Planets for {star.Name}");
            var random = new GS2.Random(star.Seed);

            var starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            var moonChance = GetMoonChanceForStar(star);
            var gasChance = GetGasChanceForStar(star);
            var subMoonChance = 0.0;
            if (preferences.GetBool("secondarySatellites"))
                subMoonChance = preferences.GetFloat("chanceMoonMoon", 5f) / 100f;
            //moonChance = moonChance - subMoonChance;

            var gasCount = Mathf.RoundToInt(starBodyCount * (float)gasChance);

            var telluricCount = starBodyCount - gasCount;
            // GS2.Log($"GasCount:{gasCount} TelluricCount:{telluricCount}");
            if (telluricCount == 0 && star == birthStar)
            {
                telluricCount = 1;
                gasCount--;
            }

            var moonCount = Mathf.RoundToInt(telluricCount * (float)moonChance);
            // GS2.Log($"GasCount:{gasCount} TelluricCount:{telluricCount} MoonCount:{moonCount}");
            telluricCount = telluricCount - moonCount;
            // GS2.Log($"GasCount:{gasCount} TelluricCount:{telluricCount} MoonCount:{moonCount}");

            if (telluricCount == 0 && star == birthStar)
            {
                telluricCount = 1;
                moonCount--;
            }

            if (moonCount > 0 && gasCount == 0 && telluricCount == 0)
            {
                if (star == birthStar) telluricCount++;
                else if (gasChance > 0) gasCount++;
                else telluricCount++;
                if (moonCount > 0) moonCount--;
            }
            // GS2.Log($"GasCount:{gasCount} TelluricCount:{telluricCount} MoonCount:{moonCount}");

            var secondaryMoonCount = Mathf.RoundToInt((moonCount - 1) * (float)subMoonChance);
            moonCount -= secondaryMoonCount;
            // GS2.Log($"GasCount:{gasCount} TelluricCount:{telluricCount} MoonCount:{moonCount} SecondaryMoonCount:{secondaryMoonCount}");
            // GS2.Log($"GasChance:{gasChance} TelluricChance:{1-gasChance} MoonChance:{moonChance} SecondaryMoonChance:{subMoonChance}");


            var moons = new GSPlanets();
            if (star == birthStar)
            {
                birthPlanet = star.Planets.Add(new GSPlanet("BirthPlanet", "Mediterranean", preferences.GetInt("birthPlanetSize", 200), -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets()));
                GS2.Log($"Created Birth Planet in star {star.Name}: {birthPlanet}");
            }

            for (var i = star == birthStar ? 1 : 0; i < telluricCount; i++)
            {
                var radius = GetStarPlanetSize(star);
                var p = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                star.Planets.Add(p);
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
            }

            for (var i = 0; i < gasCount; i++)
            {
                var radius = GetStarPlanetSize(star);
                var p = new GSPlanet("planet_" + i, "Gas", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                if (!preferences.GetBool("hugeGasGiants", true)) p.Radius = 80;
                p.Scale = 10f;
                star.Planets.Add(p);
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
            }

            // GS2.Log($"StarPlanetCount:{star.Planets.Count} BirthStar?:{star == birthStar}");
            for (var i = 0; i < moonCount; i++)
            {
                GSPlanet randomPlanet;
                var gasPlanets = new GSPlanets();
                var telPlanets = new GSPlanets();
                foreach (var p in star.Planets)
                    if (p.Scale == 10f) gasPlanets.Add(p);
                    else telPlanets.Add(p);
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
                    GS2.Log($"{star.Planets.Count}...");
                    randomPlanet = random.Item(star.Planets);
                }

                var moon = new GSPlanet("Moon " + i, "Barren", GetStarMoonSize(star, randomPlanet.Radius, randomPlanet.Scale == 10f), -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                randomPlanet.Moons.Add(moon);
                moons.Add(moon);
                moon.genData.Add("hosttype", "planet");
                moon.genData.Add("hostname", randomPlanet.Name);
                // GS2.Log($"Added {moon} to {randomPlanet}");
            }

            for (var i = 0; i < secondaryMoonCount; i++)
            {
                var randomMoon = random.Item(moons);
                var mm = new GSPlanet("MoonMoon" + i, "Barren", GetStarMoonSize(star, randomMoon.Radius, false), -1, -1, -1, -1, -1, -1, -1, -1); // {radius = GetStarMoonSize(star, randomMoon.radius, false)});
                randomMoon.Moons.Add(mm);
                mm.genData.Add("hosttype", "moon");
                mm.genData.Add("hostname", randomMoon.Name);
                // GS2.Log($"Added {mm} {mm.Radius} to {randomMoon.Name}");
            }


            // GS2.WarnJson((from s in star.Planets select s.Name).ToList());
            // GS2.Warn("Now Assigning Moon Orbits");
            AssignMoonOrbits(star);
            // GS2.Warn("Now Assigning Planet Orbits");
            AssignPlanetOrbits(star);
            // GS2.Warn("Now Assigning Themes");
            SelectPlanetThemes(star);
            // GS2.Warn("Now assigning parameters");
            FudgeNumbersForPlanets(star);
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                if (GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                if (!GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 50f;
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                var starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination");
                var starLong = preferences.GetFloat($"{GetTypeLetterFromStar(star)}orbitLongitude", 0);
                if (starLong == -1)
                    body.OrbitLongitude = random.NextFloat() * 360f;
                else
                    body.OrbitLongitude = random.NextFloat() * starLong;
                // GS2.Log($"StarInc {starInc}");
                if (GS2.IsPlanetOfStar(star, body) && starInc > -1)
                {
                    // GS2.Warn($"SETTING starInc Orbit Inclination of {star.Name} to {starInc}");
                    if (starInc > 0) body.OrbitInclination = random.NextFloat(starInc);
                    else body.OrbitInclination = 0;
                }

                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f) * random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                if (GS2.IsPlanetOfStar(star, body) && body.OrbitRadius < preferences.GetFloat("innerPlanetDistance", 1f) && (random.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets")))
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (starInc == -1 && random.NextDouble() < 0.05) // Crazy Inclination
                    // GS2.Warn("Setting Crazy Inclination for " + star.Name);
                    body.OrbitInclination = random.NextFloat(20f, 85f);

                var rc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}rareChance");
                if (rc > 0f) body.rareChance = rc / 100f;
                else body.rareChance = rc;

                // Force inclinations for testing
                // body.OrbitInclination = 0f;
                // body.OrbitPhase = 0f;
                // body.OrbitalPeriod = 10000000f;
                if (body == birthPlanet)
                    if (preferences.GetBool("birthTidalLock"))
                        body.RotationPeriod = body.OrbitalPeriod;
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
                var heat = CalculateThemeHeat(star, planet.OrbitRadius);
                var type = EThemeType.Planet;
                if (planet != birthPlanet)
                {
                    if (planet.Scale == 10f) type = EThemeType.Gas;
                    planet.Theme = GSSettings.ThemeLibrary.Query(random, type, heat, planet.Radius);
                }
                else
                {
                    GS2.Warn("Setting Theme for BirthPlanet");
                    var habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Telluric, EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200), EThemeDistribute.Default, true);
                    if (preferences.GetBool("birthPlanetUnlock")) planet.Theme = habitableTheme;
                    else planet.Theme = "Mediterranean";
                    planet.Scale = 1f;
                }
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