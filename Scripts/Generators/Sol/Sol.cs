using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GSSerializer;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class Sol : iConfigurableGenerator
    {
        private int availMoons;
        private GSPlanet birthPlanet;

        private GSGenPreferences preferences = new();

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
        public GSStars stars = new();

        private GSUI UI_maxPlanetSize;

        //private GSUI UI_ludicrousMode;
        //private GSUI UI_birthPlanetSize;
        private GSUI UI_minPlanetSize;

        public bool DisableStarCountSlider => false;
        public string Name => "Sol";

        public string Author => "innominata";

        public string Description => "Local Stars and Solar System";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.sol";

        public GSGeneratorConfig Config { get; } = new();

        public GSOptions Options { get; } = new();

        public void Init()
        {
            Config.DefaultStarCount = 10;
            //UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            //UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            //UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            //UI_accurateStars = options.Add(GSUI.Checkbox("Accurate Stars", true , 
            //    (o) => { preferences.Set("accurateStars", o); ReadStarData(); },
            //    ( ) => { UI_accurateStars.Set(preferences.GetBool("accurateStars", true)); }));
            Options.Add(GSUI.Checkbox("Accurate Stars".Translate(), true, "accurateStars")); //, (o) => { ReadStarData(); }));
            Options.Add(GSUI.Checkbox("Start in Sol".Translate(), true, "startInSol"));
            Options.Add(GSUI.Slider("Max planets per system".Translate(), 1, 10, 99, "maxPlanetCount"));
            UI_minPlanetSize = Options.Add(GSUI.PlanetSizeSlider("Min planet size".Translate(), 20, 30, 510, o =>
            {
                var maxSize = preferences.GetFloat("maxPlanetSize", 300);
                if (maxSize == -1f) maxSize = 500;

                if (maxSize < o.Int()) o = maxSize;

                preferences.Set("minPlanetSize", (float)o);
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize", 30));
            }, () => UI_minPlanetSize.Set(preferences.GetInt("minPlanetSize", 30))));


            UI_maxPlanetSize = Options.Add(GSUI.PlanetSizeSlider("Max planet size".Translate(), 50, 300, 510, o =>
            {
                var minSize = preferences.GetFloat("minPlanetSize", 30);
                if (minSize == -1f) minSize = 5;
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", (float)o);
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }, () => UI_maxPlanetSize.Set(preferences.GetInt("maxPlanetSize", 300))));
            //UI_secondarySatellites = options.Add(GSUI.Checkbox("Secondary satellites", false, o => preferences.Set("secondarySatellites", o)));
            Options.Add(GSUI.PlanetSizeSlider("Starting planet size".Translate(), 20, 400, 510, "birthPlanetSize"));
            //{
            //    preferences.Set("birthPlanetSize", Utils.ParsePlanetSize((float)o));
            //    UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize"));
            //}));
            //UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            Options.Add(GSUI.Checkbox("Birth planet Si/Ti".Translate(), false, "birthPlanetSiTi"));
            //UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            Options.Add(GSUI.Checkbox("Moons are small".Translate(), true, "moonsAreSmall"));
            //UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants".Translate(), true, o => preferences.Set("hugeGasGiants", o)));
            ReadStarData();
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            //GS2.Log($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
            GSSettings.GalaxyParams.graphDistance = 48;
            GSSettings.GalaxyParams.graphMaxStars = 512;
            random = new GS2.Random(GSSettings.Seed);
            if (starCount > stars.Count) starCount = stars.Count;

            for (var i = 0; i < starCount; i++)
            {
                var s = stars[i].Clone();
                if (!preferences.GetBool("accurateStars", true))
                {
                    if (random.NextPick(0.05))
                    {
                        s.Type = RandomSpecialStarType();
                    }
                    else
                    {
                        var chance = random.NextDouble();
                        if (chance < 0.06)
                            s.Spectr = ESpectrType.M;
                        else if (chance < 0.24)
                            s.Spectr = ESpectrType.K;
                        else if (chance < 0.44)
                            s.Spectr = ESpectrType.G;
                        else if (chance < 0.53)
                            s.Spectr = ESpectrType.F;
                        else if (chance < 0.59)
                            s.Spectr = ESpectrType.A;
                        else if (chance < 0.95)
                            s.Spectr = ESpectrType.B;
                        else
                            s.Spectr = ESpectrType.O;
                    }
                }

                GSSettings.Stars.Add(s);
            }

            GenerateSol(GSSettings.Stars[0]);
            //GS2.Warn($"Habitable Zone for Star with Luminosity {GSSettings.Stars[0].luminosity} is {hz.min} , {hz.max}");
            for (var i = 1; i < starCount; i++)
            {
                var star = GSSettings.Stars[i];
                if (star.Planets.Count > 0)
                {
                    GS2.Log($"{star.Name} already has generated planets. Returning.");
                    continue;
                }

                //if (star.Name.EndsWith(" B"))
                //{
                //    GS2.Warn($"Found a B Star:{star.Name}");
                //    star.Decorative = true;
                //    star.Planets = new GSPlanets();
                //    continue;
                //}
                if (star.Decorative) continue;

                var bodyCount = random.Next(1, preferences.GetInt("maxPlanetCount", 10));
                var planetCount = 1 + random.Next(Mathf.RoundToInt(bodyCount / 3), bodyCount - 1);
                var moonCount = bodyCount - planetCount;
                availMoons = moonCount;
                //GS2.LogSpace(3);
                //GS2.Log($"Creating Planets for Star {star.Name}. Planet Count = {planetCount}. Moon Count = {moonCount}");
                for (var j = 0; j < planetCount; j++)
                {
                    var planetMoonCount = 0;
                    for (var m = 0; m < 6; m++)
                        if (moonCount > 0 && random.NextFloat() < 2 * ((j + 1) / (float)planetCount))
                        {
                            planetMoonCount++;
                            moonCount--;
                        }

                    var planetName = star.Name + " - " + RomanNumbers.roman[j + 1];
                    var p = RandomPlanet(star, planetName, j, planetCount, planetMoonCount, moonCount);
                    availMoons -= p.MoonsCount;
                    //GS2.Log($"Adding Planet with {p.MoonCount} moons. Remaining moons for other planets = {availMoons}. Planet BodyCount = {p.Bodies.Count}");
                    if (p.OrbitRadius < star.MaxOrbit) star.Planets.Add(p);
                    //else GS2.Warn($"Skipping Planet '{p.Name}' as orbit exceeded max of {star.MaxOrbit}");
                }
            }

            //GS2.Warn($"---------{preferences.GetBool("startInSol", true)}");
            if (!preferences.GetBool("startInSol", true))
            {
                pickNewBirthPlanet();
                GSSettings.BirthPlanetName = birthPlanet.Name;
                //GS2.Log($"Set Birth Planet to {GSSettings.BirthPlanetName}");
                //GS2.LogJson(birthPlanet);
            }
            else
            {
                GSSettings.BirthPlanetName = "Earth";
                //GS2.Log("Set to earth");
            }

            if (preferences.GetBool("birthPlanetSiTi"))
            {
                //GS2.Warn("Setting SI/TI");
                birthPlanet.GsTheme.VeinSettings.Algorithm = "GS2";
                birthPlanet.GsTheme.CustomGeneration = true;
                birthPlanet.GsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.6f, 0.6f, 5, 10, false));
                birthPlanet.GsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(EVeinType.Titanium, 1, 10, 0.6f, 0.6f, 5, 10, false));
            }

            if (preferences.GetInt("birthPlanetSize", 400) != birthPlanet.Radius)
                birthPlanet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize", 400));
        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }

        public ESpectrType getSpectrType(ExternalStarData s)
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
            }

            return ESpectrType.X;
        }

        public EStarType RandomSpecialStarType()
        {
            var chance = random.NextDouble();
            if (chance < 0.2) return EStarType.NeutronStar;

            if (chance < 0.4) return EStarType.BlackHole;

            if (chance < 0.8) return EStarType.WhiteDwarf;

            return EStarType.GiantStar;
        }

        public EStarType GetStarType(ExternalStarData s)
        {
            var AccurateStars = preferences.GetBool("accurateStars", true);
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
            }

            return EStarType.WhiteDwarf;
        }

        public void ReadStarData()
        {
            stars.Clear();

            //string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "data"), "galaxy.json");
            var serializer = new fsSerializer();
            //string json = File.ReadAllText(path);
            var assembly = Assembly.GetExecutingAssembly();
            // var all = assembly.GetManifestResourceNames();
            // GS2.LogJson(all, true);

            var reader = new StreamReader(assembly.GetManifestResourceStream("GalacticScale2.Assets.galaxy.json"));
            var json = reader.ReadToEnd();
            var data2 = fsJsonParser.Parse(json);
            var localStars = new List<ExternalStarData>();
            serializer.TryDeserialize(data2, ref localStars);
            var stopwatch = new HighStopwatch();
            stopwatch.Begin();
            for (var i = 0; i < localStars.Count; i++)
            {
                stars.Add(new GSStar(1, localStars[i].Name, ESpectrType.G, EStarType.MainSeqStar, new GSPlanets()));
                stars[stars.Count - 1].position = new VectorLF3(localStars[i].x, localStars[i].y, localStars[i].z);
                stars[stars.Count - 1].mass = localStars[i].mass;
                stars[stars.Count - 1].radius = localStars[i].radius;
                stars[stars.Count - 1].Type = GetStarType(localStars[i]);
                stars[stars.Count - 1].Spectr = getSpectrType(localStars[i]);
                stars[stars.Count - 1].luminosity = localStars[i].luminance;
                stars[stars.Count - 1].temperature = localStars[i].temp;
                if (stars.Count > 2)
                {
                    var s1 = stars[stars.Count - 1];
                    var s2 = stars[stars.Count - 2];
                    var s3 = stars[stars.Count - 3];
                    //double s1mag = s1.position.magnitude;
                    //double s2mag = s2.position.magnitude;
                    //double s3mag = s3.position.magnitude;
                    //double absMag = System.Math.Abs(s1mag - s2mag);
                    //double absMag2 = System.Math.Abs(s1mag - s3mag);
                    var d2 = Utils.DistanceVLF3(s1.position, s2.position);
                    var d3 = Utils.DistanceVLF3(s1.position, s3.position);
                    //GS2.Warn($"Distance Between {s2.Name} ->{d2}<- {s1.Name} => {d3} <- {s3.Name}");
                    if (d2 < .01) //Inside each other
                        //GS2.Warn($"Distance:{d2} moving {s2.Name} 0.01 in x,y and z");
                        s2.position = new VectorLF3(s2.position.x + 0.01, s2.position.y + 0.01, s2.position.z + 0.01);
                    //GS2.Warn($"Distance:{d2} after move");
                    if (d3 < .01) //Inside each other
                        //GS2.Warn($"Distance:{d3} moving {s3.Name} 0.01 in x,y and z");
                        s3.position = new VectorLF3(s3.position.x + 0.01, s3.position.y + 0.01, s3.position.z + 0.01);
                    //GS2.Warn($"Distance:{d2} after move");
                    if (d2 < .2 && !s1.Decorative && !s2.Decorative)
                    {
                        // Right next to each other

                        if (s1.mass > s2.mass)
                            s2.Decorative = true;
                        else
                            s1.Decorative = true;
                        //GS2.Warn($"Distance:{d2} setting {(s1.Decorative?s1.Name:s2.Name)} decorative");
                    }

                    if (d3 < .2 && !s1.Decorative && !s3.Decorative)
                    {
                        // Right next to each other

                        if (s1.mass > s3.mass)
                            s3.Decorative = true;
                        else
                            s1.Decorative = true;
                        //GS2.Warn($"Distance:{d3} setting {(s1.Decorative ? s1.Name : s3.Name)} decorative");
                    }

                    if (d2 < 3 && !s1.Decorative && !s2.Decorative)
                    {
                        //GS2.Warn($"Proximity Alert: {s1.Name} vs {s2.Name} Making OrbitRadius Smaller:{d2 * 20f}");
                        s1.MaxOrbit = Mathf.Min(s1.MaxOrbit, (float)d2 * 20f);
                        s2.MaxOrbit = Mathf.Min(s2.MaxOrbit, (float)d2 * 20f);
                    }

                    if (d3 < 3 && !s1.Decorative && !s3.Decorative)
                    {
                        //GS2.Warn($"Proximity Alert: {s1.Name} vs {s3.Name} Making OrbitRadius Smaller:{d3 * 20f}");
                        s1.MaxOrbit = Mathf.Min(s1.MaxOrbit, (float)d3 * 20f);
                        s3.MaxOrbit = Mathf.Min(s3.MaxOrbit, (float)d3 * 20f);
                    }
                }
            }
            //GS2.Log("Star Parse Time: " + stopwatch.duration.ToString("0.000") + " s");
        }

        private void pickNewBirthPlanet()
        {
            //GS2.Log("Habitable Planets:");
            //GS2.LogJson(GSSettings.Stars.HabitablePlanets);
            if (GSSettings.StarCount < 2)
            {
                birthPlanet = GSSettings.Stars[0].Planets[3];
                //GS2.Log("There are no other stars to pick a birth planet from. Using Earth");
                return;
            }

            var HabitablePlanets = GSSettings.Stars.HabitablePlanets;
            if (HabitablePlanets.Count == 1)
            {
                //GS2.Log("There are no habitable planets other than Earth generated");
                //GS2.Log("Generating new habitable planet by overwriting an existing one");
                var star = GSSettings.Stars.RandomStar;
                var index = 0;
                //GS2.Log("Getting index");
                if (star.PlanetCount > 1) index = Mathf.RoundToInt((star.PlanetCount - 1) / 2);

                var planet = star.Planets[index];
                //GS2.Log("Getting themeNames");
                var themeNames = GSSettings.ThemeLibrary.Habitable;
                //GS2.Log($"Count = {themeNames.Count}");
                var themeName = themeNames[random.Next(themeNames.Count)];
                //GS2.Log($"Setting Planet Theme to {themeName}");
                planet.Theme = themeName;
                planet.Radius = Utils.ParsePlanetSize(preferences.GetInt("birthPlanetSize", 400));
                //GS2.Log("Setting birthPlanet");
                birthPlanet = planet;
                //GS2.Log($"Selected {birthPlanet.Name}");
                return;
            }

            if (HabitablePlanets.Count > 1)
            {
                //GS2.Log("Selecting random habitable planet");
                birthPlanet = HabitablePlanets[random.Next(1, HabitablePlanets.Count - 1)];
                //GS2.Log($"Selected {birthPlanet.Name}");
                return;
            }

            //int attempts = 1;
            //while (attempts < 200)
            //{
            //    int starIndex = random.Range(1, GSSettings.StarCount / 2);
            //    GSStar star = GSSettings.Stars[starIndex];
            //    foreach (GSPlanet body in star.Bodies) if (body.isHabitable) { birthPlanet = body; return; }
            //    attempts++;
            //}
            GS2.Warn("Could not find any other birth planets in the galaxy. Defaulting to Earth");
            birthPlanet = GSSettings.Stars[0].Planets[3];
        }

        public float getPlanetIndex(GSStar star, GSPlanet planet)
        {
            for (var i = 0; i < star.Planets.Count; i++)
                if (star.Planets[i] == planet)
                    return i;
            GS2.Error($"Planet {planet.Name} does not (yet) belong to star {star.Name}");
            return 0;
        }

        private float CalculateNextAvailableOrbit(GSPlanet planet, GSPlanet moon)
        {
            var randomvariance = random.NextFloat(0.005f, 0.01f);
            var planetsize = planet.RadiusAU;
            var moonsize = moon.RadiusAU;
            if (planet.Moons?.Count < 1) return planetsize + moonsize + randomvariance;
            var lastMoon = planet.Moons[planet.Moons.Count - 1];
            var lastOrbit = lastMoon.OrbitRadius + lastMoon.SystemRadius;
            var thisMoonSystemRadius = moon.SystemRadius;
            //GS2.Warn($"Calculating moon orbit. last orbit:{lastOrbit} thisMoonSystemRadius:{moon.SystemRadius} randomVariance:{randomvariance}");
            return lastOrbit + thisMoonSystemRadius + randomvariance;
        }

        private float CalculateNextAvailableOrbit(GSStar star, GSPlanet planet)
        {
            float randomvariance;
            if (random.NextDouble() < 0.1)
                randomvariance = random.NextFloat(0.05f, 2f);
            else
                randomvariance = random.NextFloat(0.4f, 1f);

            var planetsize = planet.RadiusAU;
            if (star.Planets?.Count < 1) return randomvariance + planetsize;

            var lastPlanet = star.Planets[star.Planets.Count - 1];
            var lastPlanetOrbit = lastPlanet.OrbitRadius + lastPlanet.SystemRadius;
            var thisPlanetSystemRadius = planet.SystemRadius;
            return lastPlanetOrbit + thisPlanetSystemRadius + randomvariance;
        }

        public float CalculateOrbitPeriod(float orbitRadius)
        {
            var d = Mathf.PI * orbitRadius * 2;
            var speed = 0.0005f; // AU/s
            return d / speed;
        }

        public GSPlanet RandomMoon(GSStar star, GSPlanet host, string name, int index, int orbitCount, string heat)
        {
            //GS2.Log($"Creating moon. Heat = {heat} name = {name} index = {index}/{orbitCount}");
            string theme;
            List<string> themeNames;

            var radius = random.Next(30, Mathf.Clamp(host.Radius - 10, 30, 510));
            if (preferences.GetBool("moonsAreSmall", true) && radius > 200) radius = random.Next(30, 190);
            //switch (heat)
            //{
            //    case "Hot": themeNames = GSSettings.ThemeLibrary.Hot; break;
            //    case "Warm": themeNames = GSSettings.ThemeLibrary.Warm; break;
            //    case "Temperate": themeNames = GSSettings.ThemeLibrary.Temperate; break;
            //    case "Cold": themeNames = GSSettings.ThemeLibrary.Cold; break;
            //    default: themeNames = GSSettings.ThemeLibrary.Frozen; break;
            //}
            var ThemeHeat = EThemeHeat.Frozen;
            switch (heat)
            {
                case "Hot":
                    ThemeHeat = EThemeHeat.Hot;
                    break;
                case "Warm":
                    ThemeHeat = EThemeHeat.Warm;
                    break;
                case "Temperate":
                    ThemeHeat = EThemeHeat.Temperate;
                    break;
                case "Cold":
                    ThemeHeat = EThemeHeat.Cold;
                    break;
            }

            themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Moon, ThemeHeat, radius);
            if (themeNames.Count == 0) GS2.Error("No Theme Names in List!");
            theme = themeNames[random.Next(0, themeNames.Count - 1)];
            float rotationPeriod = random.Next(60, 3600);

            float luminosity = -1;
            var orbitInclination = 0f;
            var orbitPhase = random.NextFloat(360);
            var orbitObliquity = random.NextFloat(0f, 90f);
            var rotationPhase = 0f;
            var moon = new GSPlanet(name, theme, radius, -1, orbitInclination, -1, orbitPhase, orbitObliquity, rotationPeriod, rotationPhase, luminosity);
            moon.Scale = 1f;
            moon.OrbitRadius = CalculateNextAvailableOrbit(host, moon);
            moon.OrbitalPeriod = CalculateOrbitPeriod(moon.OrbitRadius);
            if (index / orbitCount < random.NextFloat() && random.NextBool()) moon.RotationPeriod = moon.OrbitalPeriod;
            //GS2.LogJson(moon);
            return moon;
        }

        private int ChooseRadius(bool tiny, bool huge)
        {
            var radius = 200;
            var maxPlanetSize = preferences.GetInt("maxPlanetSize", 300);
            var minPlanetSize = preferences.GetInt("minPlanetSize", 30);
            var avgPlanetSize = (maxPlanetSize - minPlanetSize) / 2 + minPlanetSize;
            if (!tiny && !huge)
                //average
                radius = random.Next(Mathf.Min(minPlanetSize + 100, avgPlanetSize), Mathf.Max(maxPlanetSize - 100, avgPlanetSize));
            else if (tiny && !huge)
                //tiny
                radius = random.Next(Mathf.Max(minPlanetSize, 30), Mathf.Min(70, maxPlanetSize));
            else if (huge && !tiny)
                //huge
                radius = random.Next(Mathf.Min(avgPlanetSize, maxPlanetSize - 100), Mathf.Min(maxPlanetSize, 510)); //needs more limits, but I got bored
            else
                radius = random.Next(minPlanetSize, maxPlanetSize);
            return radius;
        }

        public GSPlanet RandomPlanet(GSStar star, string name, int orbitIndex, int orbitCount, int moonCount, int availMoons)
        {
            //GS2.Log($"Creating Random Planet for {star.Name}. Named: {name}. orbitIndex:{orbitIndex}/{orbitCount} moons:{moonCount}");
            var hz = Utils.CalculateHabitableZone(star.luminosity);
            //GS2.Warn($"Habitable Zone for Star with Luminosity {star.luminosity} is {hz.min} , {hz.max}");
            //float thisOrbitDistance;
            int radius;
            string themeName;
            string heat;
            var hotOrbitMax = Mathf.RoundToInt(orbitCount / 6.66f);
            var frozenOrbitMax = orbitCount - hotOrbitMax;
            var warmOrbitMax = hotOrbitMax * 2;
            var coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            var temperateOrbitMax = Mathf.RoundToInt((float)((coldOrbitMax - warmOrbitMax) / 2) + warmOrbitMax);
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
                //themeNames = GSSettings.ThemeLibrary.Hot;
                chanceTiny = 0.5f;
                chanceGas = 0.1f;
                chanceHuge = 0.1f;
            }
            else if (orbitIndex < warmOrbitMax)
            {
                heat = "Warm";
                //themeNames = GSSettings.ThemeLibrary.Warm;
                chanceTiny = 0.3f;
                chanceGas = 0.05f;
                chanceHuge = 0.25f;
            }
            else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
            {
                heat = "Temperate";
                //themeNames = GSSettings.ThemeLibrary.Temperate;
                chanceTiny = 0.2f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }
            else if (orbitIndex < coldOrbitMax)
            {
                heat = "Cold";
                //themeNames = GSSettings.ThemeLibrary.Cold;
                chanceTiny = 0.2f;
                chanceGas = 0.2f;
                chanceHuge = 0.3f;
            }
            else
            {
                heat = "Frozen";
                //themeNames = GSSettings.ThemeLibrary.Frozen;
                chanceTiny = 0.6f;
                chanceGas = 0.1f;
                chanceHuge = 0.3f;
            }

            var ThemeHeat = EThemeHeat.Frozen;
            switch (heat)
            {
                case "Hot":
                    ThemeHeat = EThemeHeat.Hot;
                    break;
                case "Warm":
                    ThemeHeat = EThemeHeat.Warm;
                    break;
                case "Temperate":
                    ThemeHeat = EThemeHeat.Temperate;
                    break;
                case "Cold":
                    ThemeHeat = EThemeHeat.Cold;
                    break;
            }

            //themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            float scale;
            var tiny = false;
            var huge = false;
            var gas = false;
            if (random.NextFloat() < chanceTiny) tiny = true;

            if (random.NextFloat() < chanceHuge) huge = true;

            if (random.NextFloat() < chanceGas) gas = true;

            if (gas)
            {
                scale = 10f;
                if (!tiny && !huge)
                    radius = random.Next(100, 200);
                else if (tiny && !huge)
                    radius = random.Next(60, 200);
                else if (huge && !tiny)
                    radius = random.Next(200, 510);
                else
                    radius = random.Next(60, 200);

                //GS2.Log("Gas. Radius " + radius);
                //if (orbitIndex < hotOrbitMax) {
                //    themeNames = GSSettings.ThemeLibrary.HotGasGiant;
                //} else if (orbitIndex < warmOrbitMax) {
                //    themeNames = GSSettings.ThemeLibrary.GasGiant;
                //} else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax) {
                //    themeNames = GSSettings.ThemeLibrary.GasGiant;
                //} else if (orbitIndex < coldOrbitMax) {
                //    themeNames = GSSettings.ThemeLibrary.IceGiant;
                //} else {
                //    themeNames = GSSettings.ThemeLibrary.IceGiant;
                //}
                themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Gas, ThemeHeat, radius);
                themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            }
            else
            {
                scale = 1f;
                radius = ChooseRadius(tiny, huge);
                themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Planet, ThemeHeat, radius);
                themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            }

            radius = Utils.ParsePlanetSize(radius);
            float rotationalPeriod = random.Next(60, 3600);

            var g = new GSPlanet(name, themeName, Utils.ParsePlanetSize(radius), // Radius
                -1, // Orbit Radius
                random.NextFloat() * 4 + random.NextFloat() * 5, // Orbit Inclination
                -1, // Orbit Period
                random.Next(359), // Phase
                random.NextFloat() * 20, // Obliquity
                rotationalPeriod, // Rotation Period
                random.Next(359), // Rotational Phase
                -1 // Luminosity
            );
            g.OrbitRadius = CalculateNextAvailableOrbit(star, g);
            g.OrbitalPeriod = Utils.CalculateOrbitPeriodFromStarMass(g.OrbitRadius, star.mass);
            //GS2.Warn($"Periods for {g.Name} : Current:{g.OrbitalPeriod} Kepler:{Utils.CalculateOrbitPeriodFromStarMass(g.OrbitRadius, star.mass)}");
            if (random.NextDouble() < 0.02) g.OrbitalPeriod = -1 * g.OrbitalPeriod; // Clockwise Rotation
            if (g.OrbitRadius < 1f && random.NextFloat() < 0.5f)
                //GS2.Warn($"Tidal Locking {g.Name}");
                g.RotationPeriod = g.OrbitalPeriod; // Tidal Lock
            else if (g.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                g.RotationPeriod = g.OrbitalPeriod / 2; // 1:2 Resonance
            else if (g.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                g.RotationPeriod = g.OrbitalPeriod / 4; // 1:4 Resonance

            if (moonCount < 6 && gas && availMoons > 10) moonCount = random.Next(moonCount, 7);

            if (moonCount > 0 && availMoons > moonCount)
            {
                g.Moons = new GSPlanets();
                for (var i = 0; i < moonCount; i++)
                {
                    var moon = RandomMoon(star, g, name + " - " + RomanNumbers.roman[i + 1], i, moonCount, heat);
                    g.Moons.Add(moon);
                }
            }

            if (random.NextDouble() < 0.05) // Crazy Obliquity
                g.Obliquity = random.NextFloat(20f, 85f);
            if (random.NextDouble() < 0.05) // Crazy Inclination
                g.OrbitInclination = random.NextFloat(20f, 85f);

            g.Luminosity = -1;
            g.Scale = scale;
            //GS2.Log($"Created Planet {g.Name} radius:{g.Radius} scale:{g.Scale}");
            return g;
        }

        public void GenerateSol(GSStar sol)
        {
            //GS2.Log("Generating the Sol System");
            GS2Generator2.SetupBaseThemes();
            GS2Generator2.InitThemes();

            var luna = new GSPlanet("The Moon", "BarrenSatellite", 110, 0.045f, 5.145f, 3278f, 0, 6.68f, 3278f, 0, 1.36f);
            ref var planets = ref sol.Planets;
            planets.Add(new GSPlanet("Mercury", "Barren", 150, 0.39f, 7.005f, 10556.28f, 0, 0.034f, 7038f, 0, 9.0827f));
            var oily = planets.Add(new GSPlanet(" ", "OilGiant", 5, 0.39f, 7f, 10556f, 355, 0.034f, 7038, 0, 9f));
            planets.Add(new GSPlanet("Venus", "AcidGreenhouse", 320, 0.72f, 3.39f, 26964f, 0, 177.4f, -1000, 0, 2.6f));
            var Earth = planets.Add(new GSPlanet("Earth", "Mediterranean", 400, 1.0f, 0.0005f, 43830f, 0, 23.44f, 119.67f, 0f, 1.36f, new GSPlanets { luna }));
            planets.Add(new GSPlanet("Mars", "AridDesert", 210, 1.52f, 1.85f, 82437f, 0f, 25.19f, 123.11f, 0, 0.58f));
            planets.Add(new GSPlanet("Ceres", "DwarfPlanet", 30, 2.77f, 10.6f, 82437.6f, 120.6f, 0.034f, 45.5f, 31.7f, 0.2926f));
            var jovianMoons = new GSPlanets
            {
                new("Io", "IceGelisol", 110, 0.2f, 0.04f, 216f, 0, 0, 212.5f, 0, 0.05f),
                new("Europa", "IceGelisol", 100, 0.25f, 0.47f, 432f, 0, 0, 426f, 0, 0.05f),
                new("Ganymede", "IceGelisol", 160, 0.3f, 0.18f, 864f, 0, 0, 858.5f, 0, 0.0526f),
                new("Callisto", "IceGelisol", 150, 0.35f, 0.19f, 2004f, 0, 0, 2002.5f, 0, 0.05f)
            };
            planets.Add(new GSPlanet("Jupiter", "GasGiant", 450, 5.2f, 1.3053f, 519670f, 0f, 3.13f, 49.63f, 0, 0.05026f, jovianMoons));
            var g = planets.Add(new GSPlanet("Saturn", "GasGiant2", 380, 9.58f, 2.48446f, 1291106f, 0f, 26.73f, 53.28f, 0, 0.01482f, new GSPlanets
            {
                new("Titan", "AshenGelisol", 160, 0.2f, 0.33f, 1908f, 0f, 0f, 1913.5f, 0f, 0.01482f)
            }));
            //GS2.Warn($"Periods for {g.Name} : Current:{g.OrbitalPeriod} Kepler:{Utils.CalculateOrbitPeriodFromStarMass(g.OrbitRadius, sol.mass)}");
            planets.Add(new GSPlanet("Uranus", "IceGiant", 160, 19.2f, 0.8f, 3682248f, 0f, 97.77f, 1000f, 0, 0.00369f));
            planets.Add(new GSPlanet("Neptune", "IceGiant2", 155, 30.05f, 1.769f, 72142680f, 0f, 28.3f, 80.55f, 0f, 0.001508f, new GSPlanets
            {
                new("Triton", "AshenGelisol", 80, 0.2f, 157.3f, 708f, 0f, 0f, 1000f, 0f, 0.001508f)
            }));
            var PlutoCharon = planets.Add(new GSPlanet(" ", "Center", 10, 39.48f, 17.16f, 10867200.0f, 0, 122.53f, 1000f, 0f, 0.000873f));
            PlutoCharon.Scale = 0.0001f;
            PlutoCharon.Moons = new GSPlanets
            {
                new("Pluto", "AshenGelisol", 70, .002f, 17.16f, 108.0f, 0, 122.53f, 1000f, 0f, 0.000873f),
                new("Charon", "BarrenSatellite", 30, .015f, 17.16f, 108.0f, 180.03f, 122.53f, 1000f, 0f, 0.000873f)
            };
            oily.Scale = 1f;
            if (preferences.GetBool("startInSol", true))
            {
                birthPlanet = Earth;
                GSSettings.BirthPlanetName = "Earth";
            }
        }

        public class ExternalStarData
        {
            public float luminance;
            public float mass;
            public string Name;
            public float radius;
            public string spect;
            public float temp;
            public float x;
            public float y;
            public float z;
        }
    }
}