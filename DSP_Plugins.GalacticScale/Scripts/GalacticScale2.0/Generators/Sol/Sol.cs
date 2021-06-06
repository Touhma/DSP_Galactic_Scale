using BepInEx;
using GSFullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace GalacticScale.Generators
{
    public partial class Sol : iConfigurableGenerator
    {
        public string Name => "Sol";

        public string Author => "innominata";

        public string Description => "Local Stars and Solar System";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.sol";

        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        public List<GSStar> stars = new List<GSStar>();
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
        private GS2.Random random = GS2.random;
        private GSPlanet birthPlanet;
        public void Init()
        {
            config.DefaultStarCount = 10;
            //UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            //UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            //UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            //UI_accurateStars = options.Add(GSUI.Checkbox("Accurate Stars", true , 
            //    (o) => { preferences.Set("accurateStars", o); ReadStarData(); },
            //    ( ) => { UI_accurateStars.Set(preferences.GetBool("accurateStars", true)); }));
            options.Add(GSUI.Checkbox("Accurate Stars", true, "accurateStars"));
            options.Add(GSUI.Checkbox("Start in Sol", true, "startInSol"));
            options.Add(GSUI.Slider("Max planets per system", 1, 10, 99, "maxPlanetCount"));
            UI_minPlanetSize = options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o =>
            {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float)o) o = maxSize;
                preferences.Set("minPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize"));
            }));
            UI_maxPlanetSize = options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o =>
            {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) minSize = 5;
                //GS2.Log("min = " + minSize + " max = " + o.ToString());
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }));
            //UI_secondarySatellites = options.Add(GSUI.Checkbox("Secondary satellites", false, o => preferences.Set("secondarySatellites", o)));
            options.Add(GSUI.Slider("Starting planet size", 20, 50, 510, "birthPlanetSize"));
            //{
            //    preferences.Set("birthPlanetSize", Utils.ParsePlanetSize((float)o));
            //    UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize"));
            //}));
            //UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            options.Add(GSUI.Checkbox("Birth planet Si/Ti", false, "birthPlanetSiTi"));
            //UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            options.Add(GSUI.Checkbox("Moons are small", true, "moonsAreSmall"));
            //UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            ReadStarData();
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
            return EStarType.BlackHole; // Not Implemented
        }
        public EStarType getStarType(externalStarData s)
        {
            if (!preferences.GetBool("accurateStars",true) && random.NextDouble() < 0.1) return RandomSpecialStarType();
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
        public void ReadStarData()
        {
            stars.Clear();
            string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "data"), "galaxy.json");
            fsSerializer serializer = new fsSerializer();
            string json = File.ReadAllText(path);
            fsData data2 = fsJsonParser.Parse(json);
            List<externalStarData> localStars = new List<externalStarData>();
            serializer.TryDeserialize(data2, ref localStars);

            for (var i = 0; i < localStars.Count; i++)
            {
                stars.Add(new GSStar(1, localStars[i].Name, ESpectrType.G, EStarType.MainSeqStar, new GSPlanets()));
                stars[stars.Count - 1].position = new VectorLF3(localStars[i].x, localStars[i].y, localStars[i].z);
                stars[stars.Count - 1].mass = localStars[i].mass;
                stars[stars.Count - 1].radius = (localStars[i].radius);
                stars[stars.Count - 1].Type = getStarType(localStars[i]);
                stars[stars.Count - 1].Spectr = getSpectrType(localStars[i]);
                stars[stars.Count - 1].luminosity = localStars[i].luminance;
                stars[stars.Count - 1].temperature = localStars[i].temp;
            }

        }
        private int availMoons;
        public void Generate(int starCount)
        {
            GS2.Warn($"Start {GS2.GetCaller()}");
            GSSettings.Reset(GSSettings.Seed);
            if (starCount > stars.Count) starCount = stars.Count;
            for (var i = 0; i < starCount; i++)
            {
                GSStar s = stars[i].Clone();
                GSSettings.Stars.Add(s);
            }
            GenerateSol(GSSettings.Stars[0]);
            for (var i = 1; i < starCount; i++)
            {
                GSStar star = GSSettings.Stars[i];
                if (star.Planets.Count > 0)
                {
                    GS2.Log($"{star.Name} already has generated planets. Returning.");
                }
                int bodyCount = random.Next(1, preferences.GetInt("maxPlanetCount", 10));
                int planetCount = 1 + random.Next(Mathf.RoundToInt(bodyCount / 3), bodyCount - 1);
                int moonCount = bodyCount - planetCount;
                availMoons = moonCount;
                GS2.LogSpace(3);
                GS2.Log($"Creating Planets for Star {star.Name}. Planet Count = {planetCount}. Moon Count = {moonCount}");
                for (var j = 0; j < planetCount; j++)
                {
                    int planetMoonCount = 0;
                    for (var m = 0; m < 6; m++)
                    {
                        if (moonCount > 0 && random.NextFloat() < 2*((j + 1) / (float)planetCount))
                        {
                            planetMoonCount++;
                            moonCount--;
                        }
                    }
                    string planetName = star.Name + " - " + RomanNumbers.roman[j + 1];
                    GSPlanet p = RandomPlanet(star, planetName, j, planetCount, planetMoonCount, moonCount);
                    availMoons -= p.MoonCount;
                    GS2.Log($"Adding Planet with {p.MoonCount} moons. Remaining moons for other planets = {availMoons}. Planet BodyCount = {p.Bodies.Count}");
                    star.Planets.Add(p);
                }


            }
            if (!preferences.GetBool("startInSol", true))
            {
                pickNewBirthPlanet();
                GSSettings.BirthPlanetName = birthPlanet.Name;
            }
            if (preferences.GetBool("birthPlanetSiTi", false))
            {
                GS2.Warn("Setting SI/TI");
                birthPlanet.gsTheme.VeinSettings.Algorithm = "GS2";
                birthPlanet.gsTheme.CustomGeneration = true;
                birthPlanet.gsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(EVeinType.Silicium,
                    1, 10, 0.6f, 0.6f, 5, 10, false));
                birthPlanet.gsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(EVeinType.Titanium,
    1, 10, 0.6f, 0.6f, 5, 10, false));
            }
            if (preferences.GetInt("birthPlanetSize", 400) != 400) birthPlanet.Radius = preferences.GetInt("birthPlanetSize", 400);

        }
        private void pickNewBirthPlanet()
        {
            if (GSSettings.StarCount < 2) birthPlanet = GSSettings.Stars[0].Planets[3];
            int attempts = 1;
            while (attempts < 200)
            {
                int starIndex = random.Range(1, GSSettings.StarCount/2);
                GSStar star = GSSettings.Stars[starIndex];
                foreach (GSPlanet body in star.Bodies) if (body.isHabitable) { birthPlanet = body; return; }
                attempts++;
            }
            GS2.Warn("Could not find any other birth planets in the galaxy. Defaulting to Earth");
            birthPlanet = GSSettings.Stars[0].Planets[3];
        }
        public float getPlanetIndex(GSStar star, GSPlanet planet)
        {
            for (var i=0;i<star.Planets.Count;i++)
            {
                if (star.Planets[i] == planet) return i;
            }
            GS2.Error($"Planet {planet.Name} does not (yet) belong to star {star.Name}");
            return 0;
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
        public float CalculateOrbitPeriod(float orbitRadius)
        {
            float d = Mathf.PI * orbitRadius * 2;
            float speed = 0.0005f; // AU/s
            return d / speed;
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
            GSPlanet moon = new GSPlanet(name, theme, radius, -1, orbitInclination,  -1, orbitPhase, orbitObliquity, rotationPeriod, rotationPhase, luminosity);
            moon.Scale = 1f;
            moon.OrbitRadius = CalculateNextAvailableOrbit(host, moon);
            moon.OrbitalPeriod = CalculateOrbitPeriod(moon.OrbitRadius);
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
            //float maxOrbitDistance = 5f * star.orbitScaler;

            //float thisOrbitDistanceMax = (((float)orbitIndex + 1f) / (float)orbitCount) * maxOrbitDistance;
            //float previousOrbitDistance;
            //if (orbitIndex == 0) previousOrbitDistance = 0.1f;
            //else previousOrbitDistance = star.Planets[orbitIndex - 1].OrbitRadius;
            
            //thisOrbitDistance = random.Range(previousOrbitDistance + 0.1f, thisOrbitDistanceMax);

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
                (random.NextFloat()*4 + random.NextFloat()*5),  // Orbit Inclination
                -1,                                             // Orbit Period
                random.Next(359),                               // Phase
                random.NextFloat() * 20,                        // Obliquity
                rotationalPeriod,                               // Rotation Period
                random.Next(359),                               // Rotational Phase
                 -1                                             // Luminosity
              );
            g.OrbitRadius = CalculateNextAvailableOrbit(star, g);
            g.OrbitalPeriod = CalculateOrbitPeriod(g.OrbitRadius);
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

        public void GenerateSol(GSStar sol)
        {
            GS2.Log("Start");
            InitThemes();
            GSPlanet luna = new GSPlanet("The Moon", "BarrenSatellite", 110, 0.045f, 5.145f,  3278f, 0, 6.68f, 3278f, 0, 1.36f);
            ref GSPlanets planets = ref sol.Planets;
            planets.Add(new GSPlanet("Mercury", "Barren", 150, 0.39f, 7.005f,  10556.28f, 0, 0.034f, 7038f, 0, 9.0827f));
            GSPlanet oily = planets.Add(new GSPlanet(" ", "OilGiant", 5, 0.39f, 7f,  10556f, 355, 0.034f, 7038, 0, 9f));
            planets.Add(new GSPlanet("Venus", "AcidGreenhouse", 320, 0.72f, 3.39f,  26964f, 0, 177.4f, -1000, 0, 2.6f));
            GSPlanet Earth = planets.Add(new GSPlanet("Earth", "Mediterranean", 400, 1.0f, 0.0005f, 43830f, 0, 23.44f, 119.67f, 0f, 1.36f, new GSPlanets() { luna }));
            planets.Add(new GSPlanet("Mars", "AridDesert", 210, 1.52f, 1.85f,  82437f, 0f, 25.19f, 123.11f, 0, 0.58f));
            planets.Add(new GSPlanet("Ceres", "DwarfPlanet", 30, 2.77f, 10.6f,  82437.6f, 120.6f, 0.034f, 45.5f, 31.7f, 0.2926f));
            GSPlanets jovianMoons = new GSPlanets()
            {
                new GSPlanet("Io", "IceGelisol", 110, 0.2f, 0.04f,  216f, 0, 0, 212.5f, 0, 0.05f),
                new GSPlanet("Europa", "IceGelisol", 100, 0.25f, 0.47f,  432f, 0, 0, 426f, 0, 0.05f),
                new GSPlanet("Ganymede", "IceGelisol", 160, 0.3f, 0.18f, 864f, 0, 0, 858.5f, 0, 0.0526f),
                new GSPlanet("Callisto", "IceGelisol", 150, 0.35f, 0.19f,  2004f, 0, 0, 2002.5f, 0, 0.05f)
            };
            planets.Add(new GSPlanet("Jupiter", "GasGiant", 450, 5.2f, 1.3053f,  519670f, 0f, 3.13f, 49.63f, 0, 0.05026f, jovianMoons));
            planets.Add(new GSPlanet("Saturn", "GasGiant2", 380, 9.58f, 2.48446f, 1291106f, 0f, 26.73f, 53.28f, 0, 0.01482f, new GSPlanets() {
                new GSPlanet("Titan", "AshenGelisol", 160, 0.2f, 0.33f,  1908f, 0f, 0f, 1913.5f, 0f, 0.01482f)}));
            planets.Add(new GSPlanet("Uranus", "IceGiant", 160, 19.2f, 0.8f,  3682248f, 0f, 97.77f, 1000f, 0, 0.00369f));
            planets.Add(new GSPlanet("Neptune", "IceGiant2", 155, 30.05f, 1.769f,  72142680f, 0f, 28.3f, 80.55f, 0f, 0.001508f, new GSPlanets()
            {
                new GSPlanet("Triton", "AshenGelisol", 80, 0.2f, 157.3f,  708f, 0f, 0f, 1000f, 0f, 0.001508f)
            }));
            GSPlanet PlutoCharon = planets.Add(new GSPlanet(" ", "Center", 10, 39.48f, 17.16f, 10867200.0f, 0, 122.53f, 1000f, 0f, 0.000873f));
            PlutoCharon.Scale = 0.0001f;
            PlutoCharon.Moons = new GSPlanets() {
                new GSPlanet("Pluto", "AshenGelisol", 70, .015f, 17.16f,  10867200.0f, 0, 122.53f, 1000f, 0f, 0.000873f),
                new GSPlanet("Charon", "BarrenSatellite", 40, .015f, 17.16f,  10867200.0f, 180.03f, 122.53f, 1000f, 0f, 0.000873f)
            };
            oily.Scale = 1f;
            if (preferences.GetBool("startInSol", true))
            {
                birthPlanet = Earth;
                GSSettings.Instance.birthPlanetName = "Earth";
            }

        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
            //UI_ludicrousMode?.Set(preferences.GetBool("ludicrousMode"));
            //UI_birthPlanetSize?.Set(preferences.GetFloat("birthPlanetSize"));
            //UI_minPlanetSize?.Set(preferences.GetFloat("minPlanetSize"));
            //UI_maxPlanetSize?.Set(preferences.GetFloat("maxPlanetSize"));
            ////UI_galaxyDensity?.Set(preferences.GetFloat("galaxyDensity"));
            //UI_maxPlanetCount?.Set(preferences.GetFloat("maxPlanetCount"));
            ////UI_secondarySatellites?.Set(preferences.GetBool("secondarySatellites"));
            ////UI_birthPlanetSiTi?.Set(preferences.GetBool("birthPlanetSiTi"));
            ////UI_tidalLockInnerPlanets?.Set(preferences.GetBool("tidalLockInnerPlanets"));
            //UI_moonsAreSmall?.Set(preferences.GetBool("moonsAreSmall"));
            //UI_accurateStars?.Set(preferences.GetBool("accurateStars"));
            //UI_hugeGasGiants?.Set(preferences.GetBool("hugeGasGiants"));
            //UI_regularBirthTheme?.Set(preferences.GetBool("regularBirthTheme"));
            //UI_systemDensity?.Set(preferences.GetFloat("systemDensity"));
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }
    }
}