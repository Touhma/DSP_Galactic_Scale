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

        public string Description => "Just like the other generators, but more so";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.galacticscale2";

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
        //private GSUI UI_galaxyDensity;
        private GSUI UI_maxPlanetCount;
        //private GSUI UI_secondarySatellites;
        //private GSUI UI_birthPlanetSiTi;
        //private GSUI UI_tidalLockInnerPlanets;
        private GSUI UI_moonsAreSmall;
        //private GSUI UI_hugeGasGiants;
        //private GSUI UI_regularBirthTheme;
        //private GSUI UI_systemDensity;
        private GS2.Random random = GS2.random;
        public void Init()
        {
            config.DefaultStarCount = 10;
            //UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            //UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            //UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            UI_maxPlanetCount = options.Add(GSUI.Slider("Max planets per system", 1, 10, 99, o => preferences.Set("maxPlanetCount", o)));
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
            //UI_birthPlanetSize = options.Add(GSUI.Slider("Birth planet size", 20, 50, 510, o =>
            //{
            //    preferences.Set("birthPlanetSize", Utils.ParsePlanetSize((float)o));
            //    UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize"));
            //}));
            //UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            //UI_birthPlanetSiTi = options.Add(GSUI.Checkbox("Birth planet Si/Ti", false, o => preferences.Set("birthPlanetSiTi", o)));
            //UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            UI_moonsAreSmall = options.Add(GSUI.Checkbox("Moons are small", true, o => preferences.Set("moonsAreSmall", o)));
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
        public EStarType getStarType(externalStarData s)
        {
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

        public void Generate(int starCount)
        {
            GS2.Warn("Start " + GS2.GetCaller());
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
                int bodyCount = random.Next(1, preferences.GetInt("MaxPlanetCount", 10));
                int planetCount = 1 + random.Next(Mathf.RoundToInt(bodyCount / 3), bodyCount - 1);
                int moonCount = bodyCount - planetCount;
                GS2.Log("Star " + star.Name + " planet Count = " + planetCount + " mooncount = " + moonCount);
                for (var j = 0; j < planetCount; j++)
                {
                    int planetMoonCount = 0;
                    for (var m = 0; m < 6; m++)
                    {
                        GS2.Log("MoonCount calculation : random < " + (j + 1 / (float)planetCount));
                        if (moonCount > 0 && random.NextFloat() < (j + 1 / (float)planetCount))
                        {
                            GS2.Log("Adding Moon");
                            planetMoonCount++;
                            moonCount--;
                        }
                    }
                    string planetName = star.Name + " - " + RomanNumbers.roman[j + 1];
                    star.Planets.Add(RandomPlanet(star, planetName, j, planetCount, planetMoonCount));
                }


            }

        }
        public float CalculateOrbitPeriod(float orbitRadius)
        {
            float d = Mathf.PI * orbitRadius * 2;
            float speed = 0.0005f; // AU/s
            return d / speed;
        }

        public GSPlanet RandomMoon(GSStar star, GSPlanet host, string name, int index, int orbitCount, string heat)
        {
            GS2.Log("Creating moon. Badly. Heat = " + heat + " name = " + name + " index = " + index + "/" + orbitCount);
            string theme = "Barren";
            List<string> themeNames = new List<string>();
            switch (heat)
            {
                case "Hot": themeNames = GSSettings.ThemeLibrary.Hot; break;
                case "Warm": themeNames = GSSettings.ThemeLibrary.Warm; break;
                case "Temperate": themeNames = GSSettings.ThemeLibrary.Temperate; break;
                case "Cold": themeNames = GSSettings.ThemeLibrary.Cold; break;
                default: themeNames = GSSettings.ThemeLibrary.Frozen; break;
            }
            theme = themeNames[random.Range(0, themeNames.Count - 1)];
            int radius = Utils.ParsePlanetSize(random.Range(30, 150));
            float previousOrbitRadius = (host.MoonCount > 0)? host.Moons[host.Moons.Count - 1].OrbitRadius:0.05f;
            float orbitRadius = previousOrbitRadius + (host.Radius * host.scale) / 40000.0f + random.NextFloat()/10;
            name += orbitRadius.ToString();
            float orbitalPeriod = CalculateOrbitPeriod(orbitRadius);
            float rotationPeriod = random.Range(60, 3600);
            if (index / orbitCount < random.NextFloat()) rotationPeriod = orbitalPeriod;
            float luminosity = -1;
            float orbitInclination = 0f;
            float orbitLongitude = 0f;
            float orbitPhase = random.NextFloat(360);
            float orbitObliquity = random.Range(0f, 90f);
            float rotationPhase = 0f;
            //GS2.Log("Got this far.");
            //GS2.Log("Host:" + host?.Name);
            GSPlanet moon = new GSPlanet(name, theme, radius, orbitRadius, orbitInclination, orbitLongitude, orbitalPeriod, orbitPhase, orbitObliquity, rotationPeriod, rotationPhase, luminosity);
            moon.scale = 1f;
            GS2.LogJson(moon);
            return moon;
        }
        public GSPlanet RandomPlanet(GSStar star, string name, int orbitIndex, int orbitCount, int moonCount)
        {
            GS2.Log("Creating Random Planet for " + star.Name + ". Named: " + name + " orbitIndex:" + orbitIndex + "/" + orbitCount + " moons:" + moonCount);
            
            float thisOrbitDistance;
            float radius;
            string themeName;
            string heat;
            int hotOrbitMax = Mathf.RoundToInt((float)orbitCount / 6.66f);
            int frozenOrbitMax = orbitCount - hotOrbitMax;
            int warmOrbitMax = hotOrbitMax * 2;
            int coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            int temperateOrbitMax = Mathf.RoundToInt((float)((coldOrbitMax - warmOrbitMax) / 2) + warmOrbitMax);
            float maxOrbitDistance = 5f * star.orbitScaler;
            //GS2.Log("maxOrbitDistance:" + maxOrbitDistance);

            float thisOrbitDistanceMax = (((float)orbitIndex + 1f) / (float)orbitCount) * maxOrbitDistance;
            //GS2.Log("thisOrbitDistanceMax = " + thisOrbitDistanceMax);
            float previousOrbitDistance;
            if (orbitIndex == 0) previousOrbitDistance = 0.1f;
            else previousOrbitDistance = star.Planets[orbitIndex - 1].OrbitRadius;
            //GS2.Log("previousOrbitDistance = " + previousOrbitDistance);
            thisOrbitDistance = random.Range(previousOrbitDistance + 0.1f, thisOrbitDistanceMax);
            //- (random.NextFloat() * ((maxOrbitDistance / orbitCount) / 2)
            //GS2.Log("thisOrbitDistance:" + thisOrbitDistance);

            List<string> themeNames;
            float chanceTiny = 0f;
            float chanceHuge = 0f;
            float chanceGas = 0f;
            if (orbitIndex < hotOrbitMax)
            {
                //GS2.Log("Hot");
                heat = "Hot";
                themeNames = GSSettings.ThemeLibrary.Hot;
                chanceTiny = 0.5f;
                chanceGas = 0.1f;
                chanceHuge = 0.1f;
            }
            else if (orbitIndex < warmOrbitMax)
            {
                //GS2.Log("Warm");
                heat = "Warm";
                themeNames = GSSettings.ThemeLibrary.Warm;
                chanceTiny = 0.3f;
                chanceGas = 0.05f;
                chanceHuge = 0.25f;
            }
            else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
            {
                //GS2.Log("Temperate");
                heat = "Temperate";
                themeNames = GSSettings.ThemeLibrary.Temperate;
                chanceTiny = 0.2f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }
            else if (orbitIndex < coldOrbitMax)
            {
                //GS2.Log("Cold");
                heat = "Cold";
                themeNames = GSSettings.ThemeLibrary.Cold;
                chanceTiny = 0.2f;
                chanceGas = 0.2f;
                chanceHuge = 0.3f;
            }
            else
            {
                //GS2.Log("Frozen");
                heat = "Frozen";
                themeNames = GSSettings.ThemeLibrary.Frozen;
                chanceTiny = 0.6f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }
            //GS2.Log("ChanceGas:" + chanceGas + " ChanceTiny:" + chanceTiny + " ChanceHuge:" + chanceHuge);
            //GS2.LogJson(themeNames);
            themeName = themeNames[random.Range(0, themeNames.Count - 1)];
            float scale;
            bool tiny = false;
            bool huge = false;
            bool gas = false;
            if (random.NextFloat() < chanceTiny) tiny = true;
            if (random.NextFloat() < chanceHuge) huge = true;
            if (random.NextFloat() < chanceGas) gas = true;
            //GS2.Log("GAS:" + gas);
            //GS2.Log("Tiny:" + tiny);
            //GS2.Log("Huge:" + huge);
            if (gas)
            {
                scale = 10f;
                if (!tiny && !huge) radius = random.Range(100, 200);
                else if (tiny && !huge) radius = random.Range(60, 200);
                else if (huge && !tiny) radius = random.Range(200, 1000);
                else radius = random.Range(60, 200);
                GS2.Log("Gas. Radius " + radius);
                if (orbitIndex < hotOrbitMax)
                {
                    GS2.Log("Hot Gas");
                    themeNames = GSSettings.ThemeLibrary.HotGasGiant;
                }
                else if (orbitIndex < warmOrbitMax)
                {
                    GS2.Log("Warm Gas");
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                }
                else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
                {
                    GS2.Log("Temperate Gas");
                    themeNames = GSSettings.ThemeLibrary.GasGiant;
                }
                else if (orbitIndex < coldOrbitMax)
                {
                    GS2.Log("Cold Gas");
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                }
                else
                {
                    GS2.Log("Frozen Gas");
                    themeNames = GSSettings.ThemeLibrary.IceGiant;
                    GS2.LogJson(themeNames);
                    
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
                //GS2.Log("Telluric. Radius " + radius);
            }



            float orbitalPeriod = CalculateOrbitPeriod(thisOrbitDistance);
            float rotationalPeriod = random.Range(60, 3600);
            if (thisOrbitDistance < 1f && random.NextFloat() < 0.5f) rotationalPeriod = orbitalPeriod;
            GSPlanet g = new GSPlanet(name, themeName, Utils.ParsePlanetSize(radius),
                thisOrbitDistance, (random.NextFloat() + random.NextFloat()), 0, orbitalPeriod,
                random.Next(359), random.NextFloat() * 20, rotationalPeriod, random.Next(359), -1);
            if (moonCount > 0)
            {

                //GS2.Warn("Adding Moons");
                g.Moons = new GSPlanets();
                for (var i = 0; i < moonCount; i++)
                {
                    //GS2.Log("Trying to add moon: ");
                    GSPlanet moon = RandomMoon(star, g, name + " - " + RomanNumbers.roman[i + 1], i, moonCount, heat);
                    //GS2.Log("Adding Moon");
                    g.Moons.Add(moon);
                }
            }
            
            g.Luminosity = -1;
            g.scale = scale;
            GS2.LogJson(g);
            return g;
        }

        public void GenerateSol(GSStar sol)
        {
            GS2.Log("Start" + sol.bodyCount);
            InitThemes();
            GSPlanet luna = new GSPlanet("The Moon", "BarrenSatellite", 110, 0.015f, 5.145f, 0.0f, 3278f, 0, 6.68f, 3278f, 0, 1.36f);
            ref GSPlanets planets = ref sol.Planets;
            planets.Add(new GSPlanet("Mercury", "Barren", 150, 0.39f, 7.005f, 252.25f, 10556.28f, 0, 0.034f, 7038f, 0, 9.0827f));
            GSPlanet oily = planets.Add(new GSPlanet(" ", "OilGiant", 5, 0.39f, 7f, 252f, 10556f, 355, 0.034f, 7038, 0, 9f));
            planets.Add(new GSPlanet("Venus", "AcidGreenhouse", 320, 0.72f, 3.39f, 182f, 26964f, 0, 177.4f, -1000, 0, 2.6f));
            planets.Add(new GSPlanet("Earth", "Mediterranean", 140, 1.0f, 0.0005f, 100f, 43830, 0, 23.44f, 119.67f, 0f, 1.36f, new GSPlanets() { luna }));
            planets.Add(new GSPlanet("Mars", "AridDesert", 210, 1.52f, 1.85f, 355.45f, 82437f, 0f, 25.19f, 123.11f, 0, 0.58f));
            planets.Add(new GSPlanet("Ceres", "DwarfPlanet", 30, 2.77f, 10.6f, 329.5f, 82437.6f, 120.6f, 0.034f, 45.5f, 31.7f, 0.2926f));
            GSPlanets jovianMoons = new GSPlanets()
            {
                new GSPlanet("Io", "IceGelisol", 110, 0.2f, 0.04f, 0, 216f, 0, 0, 212.5f, 0, 0.05f),
                new GSPlanet("Europa", "IceGelisol", 100, 0.25f, 0.47f, 0f, 432f, 0, 0, 426f, 0, 0.05f),
                new GSPlanet("Ganymede", "IceGelisol", 160, 0.3f, 0.18f, 0f, 864f, 0, 0, 858.5f, 0, 0.0526f),
                new GSPlanet("Callisto", "IceGelisol", 150, 0.35f, 0.19f, 0f, 2004f, 0, 0, 2002.5f, 0, 0.05f)
            };
            planets.Add(new GSPlanet("Jupiter", "GasGiant", 450, 5.2f, 1.3053f, 34.404f, 519670f, 0f, 3.13f, 49.63f, 0, 0.05026f, jovianMoons));
            planets.Add(new GSPlanet("Saturn", "GasGiant2", 380, 9.58f, 2.48446f, 49.94432f, 1291106f, 0f, 26.73f, 53.28f, 0, 0.01482f, new GSPlanets() {
                new GSPlanet("Titan", "AshenGelisol", 160, 0.2f, 0.33f, 0.0f, 1908f, 0f, 0f, 1913.5f, 0f, 0.01482f)}));
            planets.Add(new GSPlanet("Uranus", "IceGiant", 160, 19.2f, 0.8f, 313.2322f, 3682248f, 0f, 97.77f, 1000f, 0, 0.00369f));
            planets.Add(new GSPlanet("Neptune", "IceGiant2", 155, 30.05f, 1.769f, 78f, 72142680f, 0f, 28.3f, 80.55f, 0f, 0.001508f, new GSPlanets()
            {
                new GSPlanet("Triton", "AshenGelisol", 80, 0.2f, 157.3f, 0f, 708f, 0f, 0f, 1000f, 0f, 0.001508f)
            }));
            GSPlanet PlutoCharon = planets.Add(new GSPlanet(" ", "Center", 10, 39.48f, 17.16f, 238.9881f, 10867200.0f, 0, 122.53f, 1000f, 0f, 0.000873f));
            PlutoCharon.scale = 0.0001f;
            PlutoCharon.Moons = new GSPlanets() {
                new GSPlanet("Pluto", "AshenGelisol", 70, .015f, 17.16f, 238.9881f, 10867200.0f, 0, 122.53f, 1000f, 0f, 0.000873f),
                new GSPlanet("Charon", "BarrenSatellite", 40, .015f, 17.16f, 238.9881f, 10867200.0f, 180.03f, 122.53f, 1000f, 0f, 0.000873f)
            };
            //planets.Add(new GSPlanet("Obsidian", "Obsidian", 100, 0.72f, 3.39f, 182f, 26964f, 180, 177f, 1000, 0, 2.6f));
            //planets.Add(new GSPlanet("IceMalusol", "IceMalusol", 100, 0.72f, 3.39f, 182f, 26964f, 10, 177f, 1000, 0, 2.6f));
            oily.scale = 1f;
            GSSettings.Instance.birthPlanetName = "Earth";
        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
            //UI_ludicrousMode?.Set(preferences.GetBool("ludicrousMode"));
            //UI_birthPlanetSize?.Set(preferences.GetFloat("birthPlanetSize"));
            UI_minPlanetSize?.Set(preferences.GetFloat("minPlanetSize"));
            UI_maxPlanetSize?.Set(preferences.GetFloat("maxPlanetSize"));
            //UI_galaxyDensity?.Set(preferences.GetFloat("galaxyDensity"));
            UI_maxPlanetCount?.Set(preferences.GetFloat("maxPlanetCount"));
            //UI_secondarySatellites?.Set(preferences.GetBool("secondarySatellites"));
            //UI_birthPlanetSiTi?.Set(preferences.GetBool("birthPlanetSiTi"));
            //UI_tidalLockInnerPlanets?.Set(preferences.GetBool("tidalLockInnerPlanets"));
            UI_moonsAreSmall?.Set(preferences.GetBool("moonsAreSmall"));
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