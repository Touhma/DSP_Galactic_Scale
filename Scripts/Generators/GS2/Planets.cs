using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void RemoveRaresFromStartingSystem()
        {
            // GS2.Warn($"RemoveRares Running");
            foreach (var p in birthStar.Bodies)
            {
                // GS2.Warn($"RemoveRares {p.Name}");
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                    // GS2.Warn($"RemoveRares Initializing Veins for {p.Name}");
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();

                var newTypes = new GSVeinTypes();
                foreach (var v in p.veinSettings.VeinTypes)
                    switch (v.type)
                    {
                        case EVeinType.Bamboo:
                        case EVeinType.Crysrub:
                        case EVeinType.Diamond:
                        case EVeinType.Fractal:
                        case EVeinType.Grat:
                        case EVeinType.Mag:
                            GS2.Log($"RemoveRares Removing {v.type} from {p.Name}");
                            v.veins = new List<GSVein>();

                            // GS2.LogJson(v);

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
            foreach (var star in GSSettings.Stars)
            {
                if (!star.Decorative) GeneratePlanetsForStar(star);
                if (!star.Decorative) NamePlanets(star);
            }
        }

        private void CreateComet(GSStar star)
        {
            var comet = new GSPlanet();
            comet.Name = random.Item(NameGenerator.PlanetNames);

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
            // Warn($"Creating Planets for {star.Name}");
            GS2.Random random = new GS2.Random(star.Seed);

            bool isBirthStar = star == birthStar;
            bool startIsMoonOfGas = isBirthStar && preferences.GetBool("birthPlanetGasMoon");
            bool startOnMoon = isBirthStar && preferences.GetBool("birthPlanetMoon");

            int starBodyCount = GetStarPlanetCount(star);
            if (isBirthStar && startOnMoon && startIsMoonOfGas) starBodyCount = Math.Max(3, starBodyCount);
            else if (isBirthStar && (startOnMoon || startIsMoonOfGas)) starBodyCount = Math.Max(2, starBodyCount);
            if (starBodyCount == 0) return;
            double moonChance = GetMoonChanceForStar(star);
            if (starBodyCount == 1) moonChance = 0;
            double gasChance = GetGasChanceForStar(star);
            double subMoonChance = 0.0;
            if (preferences.GetBool("secondarySatellites")) subMoonChance = preferences.GetFloat("chanceMoonMoon", 5f) / 100f;
            float moonBias = preferences.GetFloat("moonBias", 50f);
            //moonChance = moonChance - subMoonChance;

            int birthPlanetSize = Mathf.Clamp(preferences.GetInt("birthPlanetSize", 200), 20, 500);

            int gasCount = Math.Max(startIsMoonOfGas ? 1 : 0, Mathf.RoundToInt(starBodyCount * (float)gasChance));
            if (isBirthStar && startOnMoon && !startIsMoonOfGas)
                starBodyCount = Math.Max(starBodyCount, gasCount + 2);
            int telluricCount = Math.Max(isBirthStar ? 1 : 0, starBodyCount - gasCount);
            int moonCount = Math.Max(startOnMoon ? 1 : 0, Mathf.RoundToInt(telluricCount * (float)moonChance));
            telluricCount -= moonCount;
            int secondaryMoonCount = moonCount > 1 ? Mathf.RoundToInt((moonCount - 1) * (float)subMoonChance) : 0;
            moonCount -= secondaryMoonCount;
            if (moonCount == 0 && secondaryMoonCount > 0 && startOnMoon)
            {
                moonCount += 1;
                secondaryMoonCount -= 1;
            }
            GSPlanets gasPlanets = new GSPlanets();
            GSPlanets telPlanets = new GSPlanets();
            GSPlanets moons = new GSPlanets();
            bool singlePlanet = starBodyCount == 1;
            if (singlePlanet)
            {
                // GS2.Log(
                //     $"Single Planet. Ignoring Settings. Original: starBodyCount:{starBodyCount} gasCount:{gasCount} telluricCount:{telluricCount} moonCount:{moonCount} startOnMoon:{startOnMoon}");
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
                int radius = GetStarPlanetSize(star, random);
                GSPlanet p = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
                telPlanets.Add(p);
            }

            for (int i = 0; i < gasCount; i++)
            {
                int radius = Mathf.RoundToInt(GetStarGasSize(star, random) / 10f);
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
                    // GS2.Log($"Picking Host Gas Planet {gasPlanets.Count}/{telPlanets.Count}");
                    randomPlanet = random.Item(gasPlanets);
                }
                else if (telPlanets.Count > 0)
                {
                    // GS2.Log("Picking Host Telluric Planet");
                    randomPlanet = random.Item(telPlanets);
                }
                else if (gasPlanets.Count > 0)
                {
                    // GS2.Log("Picking Host Gas Planet Due to no Telluric Planets");
                    randomPlanet = random.Item(gasPlanets);
                }
                else
                {
                    int radius = GetStarPlanetSize(star, random);
                    // GS2.Log("Picking No Host Planet");
                    randomPlanet = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                    randomPlanet.genData.Add("hosttype", "star");
                    randomPlanet.genData.Add("hostname", star.Name);
                    telPlanets.Add(randomPlanet);
                }

                GSPlanet moon = new GSPlanet("Moon " + i, "Barren", GetStarMoonSize(star, randomPlanet.Radius, hostGas, random), -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                randomPlanet.Moons.Add(moon);
                moon.genData.Add("hosttype", "planet");
                moon.genData.Add("hostname", randomPlanet.Name);
                moons.Add(moon);
                // GS2.Log($"Added {moon} to {randomPlanet}");
            }
            if (isBirthStar)
            {
                birthPlanet = new GSPlanet("BirthPlanet", "Mediterranean", birthPlanetSize, -1, -1, -1, -1, -1, -1, -1,
                    -1, new GSPlanets());
                if (startIsMoonOfGas)
                {
                    GSPlanet gasHost = random.Item(gasPlanets);
                    if (startOnMoon)
                    {
                        var rockyHost = new GSPlanet("BirthPlanetHost", "Barren",
                            GetStarMoonSize(star, gasHost.Radius, true, random), -1, -1, -1, -1, -1, -1, -1, -1,
                            new GSPlanets());
                        EnsureBirthMoonHostSize(rockyHost, birthPlanetSize);
                        rockyHost.genData.Add("hosttype", "planet");
                        rockyHost.genData.Add("hostname", gasHost.Name);
                        gasHost.Moons.Add(rockyHost);
                        rockyHost.Moons.Add(birthPlanet);
                        moons.Add(rockyHost);
                        moons.Add(birthPlanet);
                        GS2.Log($"Added BirthPlanet {birthPlanet.Name} as moon of rocky moon {rockyHost.Name} around gas host {gasHost.Name}");
                    }
                    else
                    {
                        EnsureBirthMoonHostSize(gasHost, birthPlanetSize);
                        gasHost.Moons.Add(birthPlanet);
                        moons.Add(birthPlanet);
                        birthPlanet.OrbitRadius = gasHost.Radius * 6;
                        GS2.Log($"Added BirthPlanet {birthPlanet.Name} to gas host {gasHost.Name}");
                    }
                }
                else if (startOnMoon)
                {
                    GSPlanet moonHost = random.Item(telPlanets);
                    EnsureBirthMoonHostSize(moonHost, birthPlanetSize);
                    moonHost.Moons.Add(birthPlanet);
                    moons.Add(birthPlanet);
                    GS2.Log($"Added Birthplanet {birthPlanet.Name} to moon host {moonHost.Name}");
                }
                else
                {
                    GS2.Log($"Added Birthplanet {birthPlanet.Name} to star host {star.Name}");
                    telPlanets.Add(birthPlanet);
                }
                birthPlanet.genData.Set("birthPlanet", true);
                GS2.Log($"Created Birth Planet in star {star.Name}: {birthPlanet}");
            }
            for (int i = 0; i < secondaryMoonCount; i++)
            {
                // GS2.Log($"Picking Moon to host Secondary {gasPlanets.Count}/{telPlanets.Count}/{moonCount}/{secondaryMoonCount}");
                GSPlanet randomMoon = random.Item(moons);
                GSPlanet mm = new GSPlanet("MoonMoon" + i, "Barren", GetStarMoonSize(star, randomMoon.Radius, false, random), -1, -1,
                    -1, -1, -1, -1, -1, -1);
                mm.genData.Add("hosttype", "moon");
                mm.genData.Add("hostname", randomMoon.Name);
                randomMoon.Moons.Add(mm);
                if (preferences.GetBool("moonCeption", false)) moons.Add(mm);
                
                // GS2.Log($"Added {mm} {mm.Radius} to {randomMoon.Name}");
            }



            foreach (GSPlanet p in telPlanets)
                star.Planets.Add(p);

            foreach (GSPlanet p in gasPlanets)
                star.Planets.Add(p);


            // GS2.WarnJson((from s in star.Planets select s.Name).ToList());
            // GS2.Warn($"Now Assigning Moon Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignMoonOrbits(star);
            // GS2.Warn($"Now Assigning Planet Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignPlanetOrbits(star);
            // GS2.Warn($"Now Assigning Themes {(birthPlanet != null ? birthPlanet.Name : "null")}");
            SelectPlanetThemes(star);
            // GS2.Warn($"Now assigning parameters {(birthPlanet != null ? birthPlanet.Name : "null")}");
            FudgeNumbersForPlanets(star);
            // GS2.Warn("Done");
            AssignVeinSettings(star);
            // GS2.Log($"Assigning Vein Settings for {star.Name}");
        }

        private static void EnsureBirthMoonHostSize(GSPlanet host, int birthPlanetSize)
        {
            var requiredSize = birthPlanetSize >= 500 ? 500 : birthPlanetSize + 10;
            var scale = host.Scale > 0f ? host.Scale : 1f;
            var requiredRadius = Mathf.CeilToInt(requiredSize / (scale * 10f)) * 10;
            if (host.Radius < requiredRadius) host.Radius = requiredRadius;
        }

        private void AssignVeinSettings(GSStar star)
        {
            foreach (var p in star.Bodies)
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                {
                    // GS2.Log($"Vein Settings missing for planet {p.Name} with theme {p.GsTheme.Name}. Cloning...");
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();
                }
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            // #290: same fix as themes - star-scoped derived stream instead of the
            // shared generator-instance rng
            var rng = new GS2.Random(GS2.Random.Mix(GSSettings.Seed, star.Seed, 0x66756467));
            // GS2.Warn($"{star.displayType} {star.radius} {star.RadiusAU} {star.luminosity}");
            // GS2.Warn("Star RadiusAU, Star Luminance, HZMin, HZMax, HZ, Orbit Radius, LumInv, LumLin , intensity ");
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = rng.Next(360);
                if (GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = rng.NextFloat() * 4 + rng.NextFloat() * 5;
                if (!GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = rng.NextFloat() * 50f;
                body.OrbitPhase = rng.Next(360);
                body.Obliquity = rng.NextFloat() * 20;
                var starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination");
                var starLong = preferences.GetFloat($"{GetTypeLetterFromStar(star)}orbitLongitude", 0);
                if (starLong == -1)
                    body.OrbitLongitude = rng.NextFloat() * 360f;
                else
                    body.OrbitLongitude = rng.NextFloat() * starLong;
                // GS2.Log($"StarInc {starInc}");
                if (GS2.IsPlanetOfStar(star, body) && starInc > -1)
                {
                    // GS2.Warn($"SETTING starInc Orbit Inclination of {star.Name} to {starInc}");
                    if (starInc > 0) body.OrbitInclination = rng.NextFloat(starInc);
                    else body.OrbitInclination = 0;
                }

                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f) * rng.Next(60, 3600);
                if (rng.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                var innerPlanetDistanceForStar = GetInnerPlanetDistanceForStar(star);
                // GS2.Warn($"{innerPlanetDistanceForStar} for star {star.Name} {star.displayType}");
                if (GS2.IsPlanetOfStar(star, body) && body.OrbitRadius < innerPlanetDistanceForStar &&
                    (rng.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets")))
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f &&
                         rng.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f &&
                         rng.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (rng.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = rng.NextFloat(20f, 85f);
                if (starInc == -1 && rng.NextDouble() < 0.05) // Crazy Inclination
                    // GS2.Warn("Setting Crazy Inclination for " + star.Name);
                    body.OrbitInclination = rng.NextFloat(20f, 85f);

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
                var oRadius = 1f;
                if (GS2.IsPlanetOfStar(star, body))
                    oRadius = body.OrbitRadius;
                else
                    foreach (var p in star.Planets)
                        if (GS2.IsMoonOfPlanet(p, body, true))
                            oRadius = p.OrbitRadius;

                // var starLum = Mathf.Pow((Mathf.Pow(star.luminosity, 0.33f)*preferences.GetFloat("luminosityBoost")),3);
                var solarRange = preferences.GetFloatFloat("solarRange", new FloatPair(1, 500));

                // var minSolar = solarRange.low / 100f;
                // var maxSolar = solarRange.high / 100f;
                // // oRadius += star.RadiusAU;
                // float minHZ = star.genData.Get("minHZ", 1);
                // float maxHZ = star.genData.Get("maxHZ", 100f);
                // var hz = (maxHZ - minHZ) / 2 + minHZ;
                // var oSquared = oRadius * oRadius;
                // var hzSquared = hz * hz;
                // var distance = hzSquared / oSquared;
                // var intensity = Mathf.Pow(distance,0.5f) * Mathf.Pow( starLum, 0.33f);
                //
                // //intensity1 x distance1squared = intensity2 x distance2squared
                var minSolar = solarRange.low / 100f;
                var maxSolar = solarRange.high / 100f;

                // oRadius += star.RadiusAU;
                float minHZ = star.genData.Get("minHZ", 1);
                float maxHZ = star.genData.Get("maxHZ", 100f);
                var hz = (maxHZ - minHZ) / 2 + minHZ;
                var oSquared = oRadius * oRadius;
                var hzSquared = hz * hz;
                var intensity = hzSquared / oSquared;
                // var intensity = Mathf.Pow(distance,0.5f) * Mathf.Pow( starLum, 0.33f);

                //1 x hzsquared = intensity2 x oRadiussquared
                //hzSquared / oRadsquared = intensity2;

                var lumInverse = Mathf.Clamp(intensity, minSolar, maxSolar);
                var lumNone = Mathf.Clamp(star.luminosity, minSolar, maxSolar);
                var lumLinear = Mathf.Clamp(1 / (Mathf.Lerp(oRadius, hz, preferences.GetFloat("solarLerp", 0.5f)) / hz),
                    minSolar, maxSolar);
                // GS2.Warn($"Lerping from {oRadius} to {hz} by {preferences.GetFloat("solarLerp",  0.5f)} = {Mathf.Lerp(oRadius, hz, preferences.GetFloat("solarLerp", 0.5f))} ");
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

                // GS2.Warn($"{star.RadiusAU}, {starLum}, {minHZ}, {maxHZ}, {hz}, {oRadius}, {lumInverse}, {lumLinear}, {intensity}  ");
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int moonIndex)
        {
            var moons = planet.Moons;
            if (moonIndex == 0) return planet.RadiusAU + moons[moonIndex].SystemRadius;
            return moons[moonIndex - 1].OrbitRadius + moons[moonIndex - 1].SystemRadius + moons[moonIndex].SystemRadius;
        }

        private void AssignMoonOrbits(GSStar star)
        {
            // #290: moon-orbit padding rolls used the shared generator-instance rng;
            // derive a star-scoped stream so they are a pure function of the seed
            var moonRng = new GS2.Random(GS2.Random.Mix(GSSettings.Seed, star.Seed, 0x6d6f6f6e));
            // Process all planets and recursively assign moon orbits from deepest level first
            for (var planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                var planet = star.Planets[planetIndex];
                AssignMoonOrbitsRecursive(planet, 0, moonRng);
            }
            //Orbits should be set.
            // GS2.Log($"Orbits Set {(birthPlanet != null ? birthPlanet.Name : "null")}");
        }

        private void AssignMoonOrbitsRecursive(GSPlanet host, int depth, GS2.Random rng)
        {
            if (host.Moons == null || host.MoonCount == 0) return;

            // First, recursively process all nested moons (deepest first)
            for (var moonIndex = 0; moonIndex < host.MoonCount; moonIndex++)
            {
                var moon = host.Moons[moonIndex];
                AssignMoonOrbitsRecursive(moon, depth + 1, rng);
            }

            // Now assign orbits for this level's moons (after all nested moons are processed)
            for (var moonIndex = 0; moonIndex < host.MoonCount; moonIndex++)
            {
                var moon = host.Moons[moonIndex];
                // Use smaller orbit spacing for deeper nesting levels
                var baseOrbit = depth == 0 ? GetMoonOrbit(rng) : GetMoonOrbit(rng) / 2f;
                if (moon.Radius > 200f) baseOrbit = Mathf.Max(baseOrbit, 0.025f);
                moon.OrbitRadius = baseOrbit + GetNextAvailableOrbit(host, moonIndex) + host.RadiusAU * 0.5f;
                moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
            }
        }


        private float GetMoonOrbit(GS2.Random rng)
        {
            return 0.01f + rng.NextFloat(0f, 0.05f);
        }

        public void SelectPlanetThemes(GSStar star)
        {
            // #290: theme selection consumed the leftover generator-instance stream,
            // whose position depended on call counts and boot-scoped iteration order -
            // the "same slot, different theme on one boot in five" effect. A star-scoped
            // derived stream makes themes a pure function of the galaxy seed.
            var rng = new GS2.Random(GS2.Random.Mix(GSSettings.Seed, star.Seed, 0x7468656d));
            foreach (var planet in star.Planets)
            {
                var heat = CalculateThemeHeat(star, planet.OrbitRadius);
                var type = EThemeType.Planet;
                if (planet != birthPlanet)
                {
                    if (planet.Scale == 10f) type = EThemeType.Gas;
                    var isBirthGasHost = star == birthStar && preferences.GetBool("birthPlanetGasMoon") &&
                                         planet.Bodies.Contains(birthPlanet);
                    var selectedGasTheme = preferences.GetString("birthGasGiantTheme", "GasGiant");
                    if (isBirthGasHost && GSSettings.ThemeLibrary.TryGetValue(selectedGasTheme, out var gasTheme) &&
                        gasTheme.PlanetType == EPlanetType.Gas)
                        planet.Theme = selectedGasTheme;
                    else
                        planet.Theme = GSSettings.ThemeLibrary.Query(rng, type, heat, planet.Radius);
                }
                else
                {
                    planet.Theme = GetSelectedBirthPlanetTheme(rng, EThemeType.Telluric);
                    planet.Scale = 1f;
                }

                //GS2.Warn($"Planet Theme Selected. {planet.Name}:{planet.Theme} Radius:{planet.Radius * planet.Scale} {((planet.Scale == 10f) ? EThemeType.Gas : EThemeType.Planet)}");
                foreach (var body in planet.Bodies)
                    if (body != planet)
                    {
                        if (body != birthPlanet)
                        {
                            body.Theme = GSSettings.ThemeLibrary.Query(rng, EThemeType.Moon, heat, body.Radius);
                        }
                        else
                        {
                            body.Theme = GetSelectedBirthPlanetTheme(rng, EThemeType.Moon);
                            body.Scale = 1f;
                        }
                    }
                //Warn($"Set Theme for {body.Name} to {body.Theme}");
            }
            // GS2.Log($"Themes Set {(birthPlanet != null ? birthPlanet.Name : "null")}");
        }

        private string GetSelectedBirthPlanetTheme(GS2.Random rng, EThemeType planetType)
        {
            if (!preferences.GetBool("birthPlanetUnlock")) return "Mediterranean";

            var selectedTheme = preferences.GetString("birthTheme", "Mediterranean");
            if (GSSettings.ThemeLibrary.TryGetValue(selectedTheme, out var theme) && theme.Habitable)
                return selectedTheme;

            return GSSettings.ThemeLibrary.Query(rng, planetType, EThemeHeat.Temperate,
                Mathf.Clamp(preferences.GetInt("birthPlanetSize", 200), 20, 500), EThemeDistribute.Default, true);
        }

        public bool CalculateIsGas(GSStar star)
        {
            var gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = (star.genData.Get("minHZ"), star.genData.Get("maxHZ"));
            (float min, float max) orbit = (star.genData.Get("minOrbit"), star.genData.Get("maxOrbit"));
            var frozenOrbitStart = (orbit.max - hz.max) / 2 + hz.max;
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < frozenOrbitStart) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }
    }
}
