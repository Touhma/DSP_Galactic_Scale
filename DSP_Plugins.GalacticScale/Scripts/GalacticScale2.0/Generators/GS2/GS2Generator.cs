using System.Collections.Generic;
using UnityEngine;
namespace GalacticScale.Generators {
    public partial class GS2Generator : iConfigurableGenerator {
        public string Name => "GalacticScale";

        public string Author => "innominata";

        public string Description => "The Galactic Scale 2 Generator";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.gs2";

        private GS2.Random random;
        private GSPlanet birthPlanet;


        public void Generate(int starCount) {

            GS2.Warn($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
            GSSettings.GalaxyParams.graphDistance = 48;
            GSSettings.GalaxyParams.graphMaxStars = 512;
            random = new GS2.Random(GSSettings.Seed);

            for (var i = 1; i < starCount; i++) {
                GSStar star = GSSettings.Stars.Add(StarDefaults.Random());
                star.Name = SystemNames.GetName(i);
                GeneratePlanetsForStar(star);

            }
            pickNewBirthPlanet();
            if (preferences.GetBool("birthPlanetSiTi", false)) {
                AddSiTiToBirthPlanet();
            }

            if (preferences.GetInt("birthPlanetSize", 400) != 400) {
                birthPlanet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize", 400));
            }
        }
        private void GeneratePlanetsForStar(GSStar star) {
            for (var i = 0; i < 20; i++) {
                star.Planets.Add(RandomPlanet(star, star.Name + "-Planet-" + i, 0, 1, 1, 0));
            }
        }
        private void AddSiTiToBirthPlanet() {
            GS2.Warn("Setting SI/TI");
            birthPlanet.gsTheme.VeinSettings.Algorithm = "GS2";
            birthPlanet.gsTheme.CustomGeneration = true;
            birthPlanet.gsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(
                EVeinType.Silicium,
                1, 10, 0.6f, 0.6f, 5, 10, false));
            birthPlanet.gsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(
                EVeinType.Titanium,
                1, 10, 0.6f, 0.6f, 5, 10, false));
        }
        private void pickNewBirthPlanet() {
            GS2.LogJson(GSSettings.Stars.HabitablePlanets, true);

            GSPlanets HabitablePlanets = GSSettings.Stars.HabitablePlanets;
            if (HabitablePlanets.Count == 0) {
                GS2.Warn("Generating new habitable planet by overwriting an existing one");
                GSStar star = GSSettings.Stars.RandomStar;
                int index = 0;
                GS2.Warn("Getting index");
                if (star.planetCount > 1) {
                    index = Mathf.RoundToInt((star.planetCount - 1) / 2);
                }

                GSPlanet planet = star.Planets[index];
                GS2.Warn("Getting themeNames");
                List<string> themeNames = GSSettings.ThemeLibrary.Habitable;
                GS2.Warn($"Count = {themeNames.Count}");
                string themeName = themeNames[random.Next(themeNames.Count)];
                GS2.Warn($"Setting Planet Theme to {themeName}");
                planet.Theme = themeName;
                planet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize", 400));
                GS2.Warn("Setting birthPlanet");
                birthPlanet = planet;
                GS2.Warn($"Selected {birthPlanet.Name}");
                return;
            } else if (HabitablePlanets.Count > 1) {
                GS2.Warn("Selecting random habitable planet");
                birthPlanet = HabitablePlanets[random.Range(1, HabitablePlanets.Count - 1)];
                GS2.Warn($"Selected {birthPlanet.Name}");
                return;
            }
        }

        private float CalculateNextAvailableOrbit(GSPlanet planet, GSPlanet moon) {
            float randomvariance = random.Range(0.005f, 0.01f);
            float planetsize = planet.RadiusAU;
            float moonsize = moon.RadiusAU;
            if (planet.Moons?.Count < 1) {
                return planetsize + moonsize + randomvariance;
            }
            GSPlanet lastMoon = planet.Moons[planet.Moons.Count - 1];
            float lastOrbit = lastMoon.OrbitRadius + lastMoon.SystemRadius;
            float thisMoonSystemRadius = moon.SystemRadius;
            return lastOrbit + thisMoonSystemRadius + randomvariance;
        }
        private float CalculateNextAvailableOrbit(GSStar star, GSPlanet planet) {
            float randomvariance;
            if (random.NextDouble() < 0.1) {
                randomvariance = random.Range(0.05f, 2f);
            } else {
                randomvariance = random.Range(0.4f, 1f);
            }

            float planetsize = planet.RadiusAU;
            if (star.Planets?.Count < 1) {
                return randomvariance + planetsize;
            }

            GSPlanet lastPlanet = star.Planets[star.Planets.Count - 1];
            float lastPlanetOrbit = lastPlanet.OrbitRadius + lastPlanet.SystemRadius;
            float thisPlanetSystemRadius = planet.SystemRadius;
            return lastPlanetOrbit + thisPlanetSystemRadius + randomvariance;
        }


        public GSPlanet RandomMoon(GSStar star, GSPlanet host, string name, int index, int orbitCount, string heat) {
            GS2.Log($"Creating moon. Heat = {heat} name = {name} index = {index}/{orbitCount}");
            string theme;
            List<string> themeNames;
            switch (heat) {
                case "Hot": themeNames = GSSettings.ThemeLibrary.Hot; break;
                case "Warm": themeNames = GSSettings.ThemeLibrary.Warm; break;
                case "Temperate": themeNames = GSSettings.ThemeLibrary.Temperate; break;
                case "Cold": themeNames = GSSettings.ThemeLibrary.Cold; break;
                default: themeNames = GSSettings.ThemeLibrary.Frozen; break;
            }
            theme = themeNames[random.Range(0, themeNames.Count - 1)];
            int radius = Utils.ParsePlanetSize(random.Range(30, host.Radius - 10));
            if (preferences.GetBool("moonsAreSmall", true) && radius > 200) {
                radius = random.Range(30, 190);
            }

            float rotationPeriod = random.Range(60, 3600);

            float luminosity = -1;
            float orbitInclination = 0f;
            float orbitPhase = random.NextFloat(360);
            float orbitObliquity = random.Range(0f, 90f);
            float rotationPhase = 0f;
            GSPlanet moon = new GSPlanet(name, theme, radius, -1, orbitInclination, -1, orbitPhase, orbitObliquity, rotationPeriod, rotationPhase, luminosity);
            moon.Scale = 1f;
            moon.OrbitRadius = CalculateNextAvailableOrbit(host, moon);
            moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
            if (index / orbitCount < random.NextFloat()) {
                moon.RotationPeriod = moon.OrbitalPeriod;
            }
            //GS2.LogJson(moon);
            return moon;
        }
        public GSPlanet RandomPlanet(GSStar star, string name, int orbitIndex, int orbitCount, int moonCount, int availMoons) {
            GS2.Log($"Creating Random Planet for {star.Name}. Named: {name}. orbitIndex:{orbitIndex}/{orbitCount} moons:{moonCount}");

            //float thisOrbitDistance;
            float radius;
            string themeName;
            string heat;
            int hotOrbitMax = Mathf.RoundToInt(orbitCount / 6.66f);
            int frozenOrbitMax = orbitCount - hotOrbitMax;
            int warmOrbitMax = hotOrbitMax * 2;
            int coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            int temperateOrbitMax = Mathf.RoundToInt((float)((coldOrbitMax - warmOrbitMax) / 2) + warmOrbitMax);

            List<string> themeNames;
            float chanceTiny;
            float chanceHuge;
            float chanceGas;
            if (orbitIndex < hotOrbitMax) {
                heat = "Hot";
                themeNames = GSSettings.ThemeLibrary.Hot;
                chanceTiny = 0.5f;
                chanceGas = 0.1f;
                chanceHuge = 0.1f;
            } else if (orbitIndex < warmOrbitMax) {
                heat = "Warm";
                themeNames = GSSettings.ThemeLibrary.Warm;
                chanceTiny = 0.3f;
                chanceGas = 0.05f;
                chanceHuge = 0.25f;
            } else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax) {
                heat = "Temperate";
                themeNames = GSSettings.ThemeLibrary.Temperate;
                chanceTiny = 0.2f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            } else if (orbitIndex < coldOrbitMax) {
                heat = "Cold";
                themeNames = GSSettings.ThemeLibrary.Cold;
                chanceTiny = 0.2f;
                chanceGas = 0.2f;
                chanceHuge = 0.3f;
            } else {
                heat = "Frozen";
                themeNames = GSSettings.ThemeLibrary.Frozen;
                chanceTiny = 0.6f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }

            themeName = themeNames[random.Range(0, themeNames.Count - 1)];
            float scale;
            bool tiny = false;
            bool huge = false;
            bool gas = false;
            if (random.NextFloat() < chanceTiny) {
                tiny = true;
            }

            if (random.NextFloat() < chanceHuge) {
                huge = true;
            }

            if (random.NextFloat() < chanceGas) {
                gas = true;
            }

            if (gas) {
                scale = 10f;
                if (!tiny && !huge) {
                    radius = random.Range(100, 200);
                } else if (tiny && !huge) {
                    radius = random.Range(60, 200);
                } else if (huge && !tiny) {
                    radius = random.Range(200, 510);
                } else {
                    radius = random.Range(60, 200);
                }

                GS2.Log("Gas. Radius " + radius);
                if (orbitIndex < hotOrbitMax) {
                    themeNames = GSSettings.ThemeLibrary.HotGasGiant;
                } else if (orbitIndex < warmOrbitMax) {
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                } else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax) {
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                } else if (orbitIndex < coldOrbitMax) {
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                } else {
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                }
                themeName = themeNames[random.Range(0, themeNames.Count - 1)];
            } else {
                scale = 1f;
                if (!tiny && !huge) {
                    radius = random.Range(Mathf.Max(preferences.GetFloat("minPlanetSize"), 150), Mathf.Min(preferences.GetFloat("minPlanetSize"), 250));
                } else if (tiny && !huge) {
                    radius = random.Range(Mathf.Max(preferences.GetFloat("minPlanetSize"), 30), 70);
                } else if (huge && !tiny) {
                    radius = random.Range(350, 500); //needs more limits, but I got bored
                } else {
                    radius = random.Range(100, 500);
                }
            }

            float rotationalPeriod = random.Range(60, 3600);

            GSPlanet g = new GSPlanet(
                name,
                themeName,
                Utils.ParsePlanetSize(radius),                  // Radius
                -1,                                             // Orbit Radius
                (random.NextFloat() * 4 + random.NextFloat() * 5),  // Orbit Inclination
                -1,                                             // Orbit Period
                random.Next(359),                               // Phase
                random.NextFloat() * 20,                        // Obliquity
                rotationalPeriod,                               // Rotation Period
                random.Next(359),                               // Rotational Phase
                 -1                                             // Luminosity
              );
            g.OrbitRadius = CalculateNextAvailableOrbit(star, g);
            g.OrbitalPeriod = Utils.CalculateOrbitPeriod(g.OrbitRadius);
            if (random.NextDouble() < 0.02) {
                g.OrbitalPeriod = -1 * g.OrbitalPeriod; // Clockwise Rotation
            }
            if (g.OrbitRadius < 1f && random.NextFloat() < 0.5f) {
                g.RotationPeriod = g.OrbitalPeriod; // Tidal Lock
            } else if (g.OrbitRadius < 1.5f && random.NextFloat() < 0.2f) {
                g.RotationPeriod = g.OrbitalPeriod / 2; // 1:2 Resonance
            } else if (g.OrbitRadius < 2f && random.NextFloat() < 0.1f) {
                g.RotationPeriod = g.OrbitalPeriod / 4; // 1:4 Resonance
            }

            if ((moonCount < 6 && gas && availMoons > 10)) {
                moonCount = random.Range(moonCount, 7);
            }

            if (moonCount > 0 && availMoons > moonCount) {
                g.Moons = new GSPlanets();
                for (var i = 0; i < moonCount; i++) {
                    GSPlanet moon = RandomMoon(star, g, name + " - " + RomanNumbers.roman[i + 1], i, moonCount, heat);
                    g.Moons.Add(moon);
                }
            }
            if (random.NextDouble() < 0.05) // Crazy Obliquity
            {
                g.Obliquity = random.Range(20f, 85f);
            }
            if (random.NextDouble() < 0.05) // Crazy Inclination
            {
                g.OrbitInclination = random.Range(20f, 85f);
            }

            g.Luminosity = -1;
            g.Scale = scale;
            GS2.Warn($"Planet {g.Name} scale:{g.Scale}");
            return g;
        }
    }
}