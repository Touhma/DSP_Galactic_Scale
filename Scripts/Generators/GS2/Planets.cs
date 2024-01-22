using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void RemoveRaresFromStartingSystem()
        {
            foreach (GSPlanet p in birthStar.Bodies)
            {
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();

                GSVeinTypes newTypes = new GSVeinTypes();
                foreach (GSVeinType v in p.veinSettings.VeinTypes)
                    switch (v.type)
                    {
                        case EVeinType.Bamboo:
                        case EVeinType.Crysrub:
                        case EVeinType.Diamond:
                        case EVeinType.Fractal:
                        case EVeinType.Grat:
                        case EVeinType.Mag:
                            v.veins = new List<GSVein>();
                            break;
                        default:
                            newTypes.Add(v);
                            break;
                    }

                p.veinSettings.VeinTypes = newTypes;
            }
        }

        private void GeneratePlanets()
        {
            foreach (GSStar star in GSSettings.Stars)
            {
                if (!star.Decorative) GeneratePlanetsForStar(star);
                if (!star.Decorative) NamePlanets(star);
            }
        }

        private void CreateComet(GSStar star)
        {
            GSPlanet comet = new GSPlanet();
            comet.Name = random.Item(PlanetNames);

            comet.Radius = Utils.ParsePlanetSize(random.NextFloat(29, 41));
            comet.Theme = random.Item(cometThemes).Name;
            comet.OrbitInclination = 66f;
            if (star.Planets.Count > 0) comet.OrbitRadius = star.Planets[star.Planets.Count - 1].OrbitRadius + 0.5f;
            else return;
            comet.Luminosity = 0f;
            comet.OrbitalPeriod = Utils.CalculateOrbitPeriod(comet.OrbitRadius);
            star.Planets.Add(comet);
        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            star.Planets = new GSPlanets();
            GS2.Random random = new GS2.Random(star.Seed);

            int starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            double moonChance = GetMoonChanceForStar(star);
            if (starBodyCount == 1) moonChance = 0;
            double gasChance = GetGasChanceForStar(star);
            double subMoonChance = 0.0;
            if (preferences.GetBool("secondarySatellites"))
                subMoonChance = preferences.GetFloat("chanceMoonMoon", 5f) / 100f;
            float moonBias = preferences.GetFloat("MoonBias", 50f);

            bool isBirthStar = star == birthStar;

            bool startOnMoon = isBirthStar && preferences.GetBool("birthPlanetMoon");
            bool startIsMoonOfGas = isBirthStar && startOnMoon && preferences.GetBool("birthPlanetGasMoon");


            int birthPlanetSize = preferences.GetInt("birthPlanetSize", 200);

            int gasCount = Math.Max(startIsMoonOfGas ? 1 : 0, Mathf.RoundToInt(starBodyCount * (float)gasChance));
            int telluricCount = Math.Max(isBirthStar ? 1 : 0, starBodyCount - gasCount);
            int moonCount = Math.Max(startOnMoon ? 1 : 0, Mathf.RoundToInt(telluricCount * (float)moonChance));
            telluricCount -= moonCount;
            int secondaryMoonCount = moonCount > 1 ? Mathf.RoundToInt((moonCount - 1) * (float)subMoonChance) : 0;
            moonCount -= secondaryMoonCount;
            GSPlanets gasPlanets = new GSPlanets();
            GSPlanets telPlanets = new GSPlanets();
            GSPlanets moons = new GSPlanets();
            bool singlePlanet = starBodyCount == 1;
            if (singlePlanet)
            {
                GS2.Log(
                    $"Single Planet. Ignoring Settings. Original: starBodyCount:{starBodyCount} gasCount:{gasCount} telluricCount:{telluricCount} moonCount:{moonCount} startOnMoon:{startOnMoon}");
                if (isBirthStar)
                {
                    gasCount = 0;
                    telluricCount = 1;
                }

                moonCount = 0;
                secondaryMoonCount = 0;
                startOnMoon = false;
                startIsMoonOfGas = false;
            }

            for (int i = 0; i < telluricCount - (isBirthStar && !startOnMoon ? 1 : 0); i++)
            {
                int radius = GetStarPlanetSize(star);
                GSPlanet p = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
                telPlanets.Add(p);
            }

            for (int i = 0; i < gasCount; i++)
            {
                int radius = Mathf.RoundToInt(GetStarGasSize(star) / 10f);
                GSPlanet p = new GSPlanet("planet_" + i, "Gas", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                if (!preferences.GetBool("hugeGasGiants", true)) p.Radius = 80;
                p.Scale = 10f;
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
                gasPlanets.Add(p);
            }

            for (int i = 0; i < moonCount - (isBirthStar && startOnMoon ? 1 : 0); i++)
            {
                GSPlanet randomPlanet;
                bool hostGas = random.NextPick(moonBias / 100f);

                if (gasPlanets.Count > 0 && hostGas)
                {
                    randomPlanet = random.Item(gasPlanets);
                }
                else if (telPlanets.Count > 0)
                {
                    randomPlanet = random.Item(telPlanets);
                }
                else if (gasPlanets.Count > 0)
                {
                    randomPlanet = random.Item(gasPlanets);
                }
                else
                {
                    int radius = GetStarPlanetSize(star);
                    randomPlanet = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1,
                        new GSPlanets());
                    randomPlanet.genData.Add("hosttype", "star");
                    randomPlanet.genData.Add("hostname", star.Name);
                    telPlanets.Add(randomPlanet);
                }

                GSPlanet moon = new GSPlanet("Moon " + i, "Barren", GetStarMoonSize(star, randomPlanet.Radius, hostGas), -1,
                    -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                randomPlanet.Moons.Add(moon);
                moon.genData.Add("hosttype", "planet");
                moon.genData.Add("hostname", randomPlanet.Name);
                moons.Add(moon);
            }

            for (int i = 0; i < secondaryMoonCount; i++)
            {
                GSPlanet randomMoon = random.Item(moons);
                GSPlanet mm = new GSPlanet("MoonMoon" + i, "Barren", GetStarMoonSize(star, randomMoon.Radius, false), -1, -1,
                    -1, -1, -1, -1, -1, -1);
                mm.genData.Add("hosttype", "moon");
                mm.genData.Add("hostname", randomMoon.Name);
                randomMoon.Moons.Add(mm);
            }

            if (isBirthStar)
            {
                birthPlanet = new GSPlanet("BirthPlanet", "Mediterranean", birthPlanetSize, -1, -1, -1, -1, -1, -1, -1,
                    -1, new GSPlanets());

                if (startIsMoonOfGas)
                {
                    GS2.Log("BirthPlanet is moon of gas giant");
                    birthPlanet.genData.Set("IsMoon", true);
                    random.Item(gasPlanets).Moons.Add(birthPlanet);
                }
                else if (startOnMoon)
                {
                    GS2.Log("BirthPlanet is moon");
                    random.Item(telPlanets).Moons.Add(birthPlanet);
                    birthPlanet.genData.Set("IsMoon", true);
                }
                else
                {
                    GS2.Log("BirthPlanet is normal Planet");
                    telPlanets.Add(birthPlanet);
                    birthPlanet.genData.Set("IsMoon", false);
                }

                GS2.Log($"Created Birth Planet in star {star.Name}: {birthPlanet.Name}");
            }

            foreach (GSPlanet p in telPlanets)
                star.Planets.Add(p);

            foreach (GSPlanet p in gasPlanets)
                star.Planets.Add(p);


            GS2.LogGen($"Now Assigning Moon Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignMoonOrbits(star);
            GS2.LogGen($"Now Assigning Planet Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignPlanetOrbits(star);
            GS2.LogGen($"Now Assigning Themes {(birthPlanet != null ? birthPlanet.Name : "null")}");
            SelectPlanetThemes(star);
            FudgeNumbersForPlanets(star);
            AssignVeinSettings(star);
        }

        private void AssignVeinSettings(GSStar star)
        {
            foreach (GSPlanet p in star.Bodies)
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                    // GS2.Log($"Vein Settings missing for planet {p.Name} with theme {p.GsTheme.Name}. Cloning...");
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            foreach (GSPlanet body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                if (GS2.IsPlanetOfStar(star, body))
                    body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                if (!GS2.IsPlanetOfStar(star, body))
                    body.OrbitInclination = random.NextFloat() * 50f;
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                float starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination");
                float starLong = preferences.GetFloat($"{GetTypeLetterFromStar(star)}orbitLongitude", 0);
                if (starLong == -1)
                    body.OrbitLongitude = random.NextFloat() * 360f;
                else
                    body.OrbitLongitude = random.NextFloat() * starLong;
                if (GS2.IsPlanetOfStar(star, body) && starInc > -1)
                {
                    if (starInc > 0) body.OrbitInclination = random.NextFloat(starInc);
                    else body.OrbitInclination = 0;
                }

                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f) * random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                float innerPlanetDistanceForStar = GetInnerPlanetDistanceForStar(star);
                if (GS2.IsPlanetOfStar(star, body) && body.OrbitRadius < innerPlanetDistanceForStar &&
                    (random.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets")))
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f &&
                         random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f &&
                         random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (starInc == -1 && random.NextDouble() < 0.05) // Crazy Inclination
                    body.OrbitInclination = random.NextFloat(20f, 85f);

                float rc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}rareChance");
                if (rc > 0f) body.rareChance = rc / 100f;
                else body.rareChance = rc;

                if (body == birthPlanet)
                    if (preferences.GetBool("birthTidalLock"))
                        body.RotationPeriod = body.OrbitalPeriod;
                float oRadius = 1f;
                if (GS2.IsPlanetOfStar(star, body))
                    oRadius = body.OrbitRadius;
                else
                    foreach (GSPlanet p in star.Planets)
                        if (GS2.IsMoonOfPlanet(p, body, true))
                            oRadius = p.OrbitRadius;

                FloatPair solarRange = preferences.GetFloatFloat("solarRange", new FloatPair(1, 500));


                float minSolar = solarRange.low / 100f;
                float maxSolar = solarRange.high / 100f;

                float minHZ = star.genData.Get("minHZ", 1);
                float maxHZ = star.genData.Get("maxHZ", 100f);
                float hz = (maxHZ - minHZ) / 2 + minHZ;
                float oSquared = oRadius * oRadius;
                float hzSquared = hz * hz;
                float intensity = hzSquared / oSquared;

                float lumInverse = Mathf.Clamp(intensity, minSolar, maxSolar);
                float lumNone = Mathf.Clamp(star.luminosity, minSolar, maxSolar);
                float lumLinear = Mathf.Clamp(1 / (Mathf.Lerp(oRadius, hz, preferences.GetFloat("solarLerp", 0.5f)) / hz),
                    minSolar, maxSolar);
                switch (preferences.GetString("solarScheme", "Linear"))
                {
                    case "None":
                        body.Luminosity = lumNone;
                        break;
                    case "InverseSquare":
                        body.Luminosity = lumInverse;
                        break;
                    default:
                        body.Luminosity = lumLinear;
                        break;
                }
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int moonIndex)
        {
            GSPlanets moons = planet.Moons;
            if (moonIndex == 0) return planet.RadiusAU + moons[moonIndex].SystemRadius;
            return moons[moonIndex - 1].SystemRadius + moons[moonIndex - 1].OrbitRadius + moons[moonIndex].SystemRadius;
        }

        private void AssignMoonOrbits(GSStar star)
        {
            // Now Work Backwards from secondary Satellites to Planets, creating orbits.
            for (int planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                GSPlanet planet = star.Planets[planetIndex];
                //For each Planet
                for (int moonIndex = 0; moonIndex < planet.MoonCount; moonIndex++)
                {
                    GSPlanet moon = planet.Moons[moonIndex];
                    //For Each Moon
                    for (int moon2Index = 0; moon2Index < moon.MoonCount; moon2Index++)
                    {
                        //for each subsatellite
                        GSPlanet moon2 = moon.Moons[moon2Index];
                        moon2.OrbitRadius = GetMoonOrbit() / 2f + GetNextAvailableOrbit(moon, moon2Index);
                        moon2.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon2.OrbitRadius);
                    }

                    moon.OrbitRadius = GetMoonOrbit() + GetNextAvailableOrbit(planet, moonIndex);
                    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
                }
            }

            //Orbits should be set.
            GS2.Log($"Orbits Set {(birthPlanet != null ? birthPlanet.Name : "null")}");
            
        }


        private float GetMoonOrbit()
        {
            return 0.01f + random.NextFloat(0f, 0.05f);
        }

        public void SelectPlanetThemes(GSStar star)
        {
            foreach (GSPlanet planet in star.Planets)
            {
                EThemeHeat heat = CalculateThemeHeat(star, planet.OrbitRadius);
                EThemeType type = EThemeType.Planet;
                if (planet != birthPlanet)
                {
                    if (planet.Scale == 10f) type = EThemeType.Gas;
                    planet.Theme = GSSettings.ThemeLibrary.Query(random, type, heat, planet.Radius);
                }
                else
                {
                    GS2.Warn($"Setting Theme for BirthPlanet {birthPlanet.Name}");
                    string habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Telluric,
                        EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200), EThemeDistribute.Default,
                        true);
                    if (preferences.GetString("birthTheme", "") != "") planet.Theme = preferences.GetString("birthTheme");
                    else if (preferences.GetBool("birthPlanetUnlock")) planet.Theme = habitableTheme;
                    else planet.Theme = "Mediterranean";
                    GS2.DevLog("SELECTED " + planet.Theme);
                    planet.Scale = 1f;
                }

                foreach (GSPlanet body in planet.Bodies)
                    if (body != planet)
                    {
                        if (body != birthPlanet)
                        {
                            body.Theme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon, heat, body.Radius);
                        }
                        else
                        {
                            string habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon,
                                EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200),
                                EThemeDistribute.Default,
                                true);
                            if (preferences.GetString("birthTheme", "") != "") planet.Theme = preferences.GetString("birthTheme");
                            else if (preferences.GetBool("birthPlanetUnlock")) body.Theme = habitableTheme;
                            else body.Theme = "Mediterranean";
                            body.Scale = 1f;
                        }
                    }
            }
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = (star.genData.Get("minHZ"), star.genData.Get("maxHZ"));
            (float min, float max) orbit = (star.genData.Get("minOrbit"), star.genData.Get("maxOrbit"));
            float frozenOrbitStart = (orbit.max - hz.max) / 2 + hz.max;
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < frozenOrbitStart) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }
    }
}