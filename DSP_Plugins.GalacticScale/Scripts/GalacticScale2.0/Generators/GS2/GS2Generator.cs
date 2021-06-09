using GSFullSerializer;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace GalacticScale.Generators
{
    public partial class GS2Generator : iConfigurableGenerator
    {
        public string Name => "GalacticScale";

        public string Author => "innominata";

        public string Description => "The Galactic Scale 2 Generator";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.gs2";

        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        //private GSUI UI_ludicrousMode;
        //private GSUI UI_birthPlanetSize;
        private GSUI UI_minPlanetSize;
        private GSUI UI_maxPlanetSize;
        //private GSUI UI_accurateStars;
        //private GSUI UI_galaxyDensity;
        //private GSUI UI_maxPlanetCount;
        //private GSUI UI_secondarySatellites;
        //private GSUI UI_birthPlanetSiTi;
        //private GSUI UI_tidalLockInnerPlanets;
        //private GSUI UI_moonsAreSmall;
        //private GSUI UI_hugeGasGiants;
        //private GSUI UI_regularBirthTheme;
        //private GSUI UI_systemDensity;
        private GS2.Random random;
        private GSPlanet birthPlanet;
        public void enableLudicrousMode()
        {

        }
        public void Init()
        {
            config.DefaultStarCount = 10;
            options.Add(GSUI.Checkbox("Ludicrous Mode", false, "ludicrousMode", o => enableLudicrousMode()));
            options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9,"galaxyDensity"));
            options.Add(GSUI.Slider("Default StarCount", 1, 64, 1024, "defaultStarCount"));
            options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9, "galaxyDensity"));
            options.Add(GSUI.Slider("Starting Planet Size", 20, 50, 510, "birthPlanetSize"));
            options.Add(GSUI.Checkbox("Starting Planet Unlock", false, "birthPlanetUnlock"));
            options.Add(GSUI.Checkbox("Starting planet Si/Ti", false, "birthPlanetSiTi"));
            options.Add(GSUI.Checkbox("Moons are small", true, "moonsAreSmall"));
            options.Add(GSUI.Checkbox("Huge gas giants", true, "hugeGasGiants"));
            options.Add(GSUI.Checkbox("Tidal Lock Inner Planets", false, "tidalLockInnerPlanets"));
            options.Add(GSUI.Checkbox("Secondary satellites", false, "secondarySatellites"));

            options.Add(GSUI.Slider("Freq. Type K", 1, 40, 100, "freqK"));
            options.Add(GSUI.Slider("Freq. Type M", 1, 50, 100, "freqM"));
            options.Add(GSUI.Slider("Freq. Type G", 1, 30, 100, "freqG"));
            options.Add(GSUI.Slider("Freq. Type F", 1, 25, 100, "freqF"));
            options.Add(GSUI.Slider("Freq. Type A", 1, 10, 100, "freqA"));
            options.Add(GSUI.Slider("Freq. Type B", 1, 4, 100, "freqB"));
            options.Add(GSUI.Slider("Freq. Type O", 1, 2, 100, "freqO"));
            options.Add(GSUI.Slider("Freq. BlackHole", 1, 1, 100, "freqBH"));
            options.Add(GSUI.Slider("Freq. Neutron", 1, 1, 100, "freqN"));
            options.Add(GSUI.Slider("Freq. WhiteDwarf", 1, 2, 100, "freqW"));
            options.Add(GSUI.Slider("Freq. Red Giant", 1, 1, 100, "freqRG"));
            options.Add(GSUI.Slider("Freq. Yellow Giant", 1, 1, 100, "freqYG"));
            options.Add(GSUI.Slider("Freq. White Giant", 1, 1, 100, "freqWG"));
            options.Add(GSUI.Slider("Freq. Blue Giant", 1, 1, 100, "freqBG"));

            //options.Add(GSUI.Header("Default Settings", "Changing These Will Reset All Star Specific Options Below"));
            options.Add(GSUI.Slider("Min Planets/System", 1, 4, 50, "minPlanetCount"));
            options.Add(GSUI.Slider("Max Planets/System", 1, 10, 50, "maxPlanetCount"));
            UI_maxPlanetSize = options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o =>
            {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) minSize = 5;
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }));
            UI_minPlanetSize = options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o =>
            {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float)o) o = maxSize;
                preferences.Set("minPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize"));
            }));
            
            options.Add(GSUI.Slider("Planet Size Bias", 0, 50, 100, "sizeBias"));
            options.Add(GSUI.Slider("Chance Gas", 10, 20, 50, "chanceGas"));
            options.Add(GSUI.Slider("Chance Moon", 10, 20, 80, "chanceMoon"));
            options.Add(GSUI.Slider("System Density", 1, 3, 5, "systemDensity"));

            

            string[] typeDesc = {"Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O", "White Dwarf", "Red Giant", "Yellow Giant", "White Giant",
                "Blue Giant", "Neutron Star", "Black Hole" };
            string[] typeLetter = { "K", "M", "F", "G", "A", "B", "O", "WD", "RG", "YG", "WG", "BG", "NS", "BH" };

            for (var i=0; i<14; i++)
            {
                //options.Add(GSUI.Header("$Type K Star Override", "Settings for K type stars only"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Min Planets", 1, 4, 50, $"{typeLetter[i]}minPlanetCount"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Max Planets", 1, 10, 50, $"{typeLetter[i]}maxPlanetCount"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Min Size", 1, 4, 50, $"{typeLetter[i]}minPlanetSize"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Max Size", 1, 10, 50, $"{typeLetter[i]}maxPlanetSize"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Size Bias", 0, 50, 100, $"{typeLetter[i]}sizeBias"));
                options.Add(GSUI.Slider($"{typeDesc[i]} %Gas", 10, 20, 50, $"{typeLetter[i]}chanceGas"));
                options.Add(GSUI.Slider($"{typeDesc[i]} %Moon", 10, 20, 80, $"{typeLetter[i]}chanceMoon"));
                options.Add(GSUI.Slider($"{typeDesc[i]} Density", 1, 3, 5, $"{typeLetter[i]}systemDensity"));
            }

        }

        public class externalStarData
        {
            public string Name;
            public float x;
            public float y;
            public float z;
            public float mass;
            public string spect;
            public float radius;
            public float luminance;
            public float temp;
        }
        public ESpectrType getSpectrType(externalStarData s)
        {
            switch (s.spect[0])
            {
                case 'O': return ESpectrType.O;
                case 'F': return ESpectrType.F;
                case 'G': return ESpectrType.G;
                case 'B': return ESpectrType.B;
                case 'M': return ESpectrType.M;
                case 'A': return ESpectrType.A;
                case 'K': return ESpectrType.K;
                default: break;
            }
            return ESpectrType.X;
        }

        public EStarType RandomSpecialStarType()
        {
            double chance = random.NextDouble();
            if (chance < 0.2) return EStarType.NeutronStar;
            if (chance < 0.4) return EStarType.BlackHole;
            if (chance < 0.8) return EStarType.WhiteDwarf;
            return EStarType.GiantStar; 
        }
        public EStarType getStarType(externalStarData s)
        {
            bool AccurateStars = preferences.GetBool("accurateStars", true);
            //GS2.Warn($"AccurateStars:{AccurateStars}");
            //if (!AccurateStars && random.Bool(0.05)) return RandomSpecialStarType();
            switch (s.spect[0])
            {
                case 'O':
                case 'F':
                case 'G': return EStarType.MainSeqStar;
                case 'B': return EStarType.MainSeqStar;
                case 'M': return EStarType.MainSeqStar;
                case 'A': return EStarType.MainSeqStar;
                case 'K': return EStarType.MainSeqStar;
                default: break;
            }
            return EStarType.WhiteDwarf;
        }

        public void Generate(int starCount)
        {
            
            GS2.Warn($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
            GSSettings.GalaxyParams.graphDistance = 48;
            GSSettings.GalaxyParams.graphMaxStars = 512;
            random = new GS2.Random(GSSettings.Seed);
                      
            for (var i = 1; i < starCount; i++)
            {
                GSStar star = GSSettings.Stars.Add(StarDefaults.Random());
                star.Planets.Add(RandomPlanet(star, "Planet", 0, 1, 1, 0));

            }
            pickNewBirthPlanet();
            if (preferences.GetBool("birthPlanetSiTi", false)) AddSiTiToBirthPlanet();
            if (preferences.GetInt("birthPlanetSize", 400) != 400) birthPlanet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize", 400));
        }
        private void AddSiTiToBirthPlanet()
        {
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
        private void pickNewBirthPlanet()
        {
            GS2.LogJson(GSSettings.Stars.HabitablePlanets, true);

            GSPlanets HabitablePlanets = GSSettings.Stars.HabitablePlanets;
            if (HabitablePlanets.Count == 0)
            {
                GS2.Warn("Generating new habitable planet by overwriting an existing one");
                GSStar star = GSSettings.Stars.RandomStar;
                int index = 0;
                GS2.Warn("Getting index");
                if (star.planetCount > 1) index = Mathf.RoundToInt((star.planetCount - 1) / 2);
                GSPlanet planet = star.Planets[index];
                GS2.Warn("Getting themeNames");
                List<string> themeNames = GSSettings.ThemeLibrary.Habitable;
                GS2.Warn($"Count = {themeNames.Count}");
                string themeName = themeNames[random.Next(themeNames.Count)];
                GS2.Warn($"Setting Planet Theme to {themeName}");
                planet.Theme = themeName;
                planet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize",400));
                GS2.Warn("Setting birthPlanet");
                birthPlanet = planet;
                GS2.Warn($"Selected {birthPlanet.Name}");
                return;
            }
            else if (HabitablePlanets.Count > 1)
            {
                GS2.Warn("Selecting random habitable planet");
                birthPlanet = HabitablePlanets[random.Range(1, HabitablePlanets.Count - 1)];
                GS2.Warn($"Selected {birthPlanet.Name}");
                return;
            }
        }

        private float CalculateNextAvailableOrbit(GSPlanet planet, GSPlanet moon)
        {
            float randomvariance = random.Range(0.005f, 0.01f);
            float planetsize = planet.RadiusAU;
            float moonsize = moon.RadiusAU;
            if (planet.Moons?.Count < 1)
            {
                return planetsize + moonsize + randomvariance;
            }
            GSPlanet lastMoon = planet.Moons[planet.Moons.Count - 1];
            float lastOrbit = lastMoon.OrbitRadius + lastMoon.SystemRadius;
            float thisMoonSystemRadius = moon.SystemRadius;
            return lastOrbit + thisMoonSystemRadius + randomvariance;
        }
        private float CalculateNextAvailableOrbit(GSStar star, GSPlanet planet)
        {
            float randomvariance;
            if (random.NextDouble() < 0.1) randomvariance = random.Range(0.05f, 2f);
            else randomvariance = random.Range(0.4f, 1f);
            float planetsize = planet.RadiusAU;
            if (star.Planets?.Count < 1) return randomvariance + planetsize;
            GSPlanet lastPlanet = star.Planets[star.Planets.Count - 1];
            float lastPlanetOrbit = lastPlanet.OrbitRadius + lastPlanet.SystemRadius;
            float thisPlanetSystemRadius = planet.SystemRadius;
            return lastPlanetOrbit + thisPlanetSystemRadius + randomvariance;
        }


        public GSPlanet RandomMoon(GSStar star, GSPlanet host, string name, int index, int orbitCount, string heat)
        {
            GS2.Log($"Creating moon. Heat = {heat} name = {name} index = {index}/{orbitCount}");
            string theme;
            List<string> themeNames;
            switch (heat)
            {
                case "Hot": themeNames = GSSettings.ThemeLibrary.Hot; break;
                case "Warm": themeNames = GSSettings.ThemeLibrary.Warm; break;
                case "Temperate": themeNames = GSSettings.ThemeLibrary.Temperate; break;
                case "Cold": themeNames = GSSettings.ThemeLibrary.Cold; break;
                default: themeNames = GSSettings.ThemeLibrary.Frozen; break;
            }
            theme = themeNames[random.Range(0, themeNames.Count - 1)];
            int radius = Utils.ParsePlanetSize(random.Range(30, host.Radius - 10));
            if (preferences.GetBool("moonsAreSmall", true) && radius > 200) radius = random.Range(30, 190);
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
            if (index / orbitCount < random.NextFloat()) moon.RotationPeriod = moon.OrbitalPeriod;
            //GS2.LogJson(moon);
            return moon;
        }
        public GSPlanet RandomPlanet(GSStar star, string name, int orbitIndex, int orbitCount, int moonCount, int availMoons)
        {
            GS2.Log($"Creating Random Planet for {star.Name}. Named: {name}. orbitIndex:{orbitIndex}/{orbitCount} moons:{moonCount}");

            //float thisOrbitDistance;
            float radius;
            string themeName;
            string heat;
            int hotOrbitMax = Mathf.RoundToInt((float)orbitCount / 6.66f);
            int frozenOrbitMax = orbitCount - hotOrbitMax;
            int warmOrbitMax = hotOrbitMax * 2;
            int coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            int temperateOrbitMax = Mathf.RoundToInt((float)((coldOrbitMax - warmOrbitMax) / 2) + warmOrbitMax);

            List<string> themeNames;
            float chanceTiny;
            float chanceHuge;
            float chanceGas;
            if (orbitIndex < hotOrbitMax)
            {
                heat = "Hot";
                themeNames = GSSettings.ThemeLibrary.Hot;
                chanceTiny = 0.5f;
                chanceGas = 0.1f;
                chanceHuge = 0.1f;
            }
            else if (orbitIndex < warmOrbitMax)
            {
                heat = "Warm";
                themeNames = GSSettings.ThemeLibrary.Warm;
                chanceTiny = 0.3f;
                chanceGas = 0.05f;
                chanceHuge = 0.25f;
            }
            else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
            {
                heat = "Temperate";
                themeNames = GSSettings.ThemeLibrary.Temperate;
                chanceTiny = 0.2f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }
            else if (orbitIndex < coldOrbitMax)
            {
                heat = "Cold";
                themeNames = GSSettings.ThemeLibrary.Cold;
                chanceTiny = 0.2f;
                chanceGas = 0.2f;
                chanceHuge = 0.3f;
            }
            else
            {
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
            if (random.NextFloat() < chanceTiny) tiny = true;
            if (random.NextFloat() < chanceHuge) huge = true;
            if (random.NextFloat() < chanceGas) gas = true;
            if (gas)
            {
                scale = 10f;
                if (!tiny && !huge) radius = random.Range(100, 200);
                else if (tiny && !huge) radius = random.Range(60, 200);
                else if (huge && !tiny) radius = random.Range(200, 510);
                else radius = random.Range(60, 200);
                GS2.Log("Gas. Radius " + radius);
                if (orbitIndex < hotOrbitMax)
                {
                    themeNames = GSSettings.ThemeLibrary.HotGasGiant;
                }
                else if (orbitIndex < warmOrbitMax)
                {
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                }
                else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
                {
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                }
                else if (orbitIndex < coldOrbitMax)
                {
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                }
                else
                {
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                }
                themeName = themeNames[random.Range(0, themeNames.Count - 1)];
            }
            else
            {
                scale = 1f;
                if (!tiny && !huge) radius = random.Range(Mathf.Max(preferences.GetFloat("minPlanetSize"), 150), Mathf.Min(preferences.GetFloat("minPlanetSize"), 250));
                else if (tiny && !huge) radius = random.Range(Mathf.Max(preferences.GetFloat("minPlanetSize"), 30), 70);
                else if (huge && !tiny) radius = random.Range(350, 500); //needs more limits, but I got bored
                else radius = random.Range(100, 500);
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
            if (random.NextDouble() < 0.02)
            {
                g.OrbitalPeriod = -1 * g.OrbitalPeriod; // Clockwise Rotation
            }
            if (g.OrbitRadius < 1f && random.NextFloat() < 0.5f)
            {
                g.RotationPeriod = g.OrbitalPeriod; // Tidal Lock
            }
            else if (g.OrbitRadius < 1.5f && random.NextFloat() < 0.2f) g.RotationPeriod = g.OrbitalPeriod / 2; // 1:2 Resonance
            else if (g.OrbitRadius < 2f && random.NextFloat() < 0.1f) g.RotationPeriod = g.OrbitalPeriod / 4; // 1:4 Resonance
            if ((moonCount < 6 && gas && availMoons > 10))
            {
                moonCount = random.Range(moonCount, 7);
            }

            if (moonCount > 0 && availMoons > moonCount)
            {
                g.Moons = new GSPlanets();
                for (var i = 0; i < moonCount; i++)
                {
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
            return g;
        }


        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }
    }
}