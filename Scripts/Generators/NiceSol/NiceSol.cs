using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class NiceSol : iConfigurableGenerator
    {
        private int availMoons;
        private GSPlanet birthPlanet;
        private GSGenPreferences preferences = new();
        private GS2.Random random;
        private GSUI UI_epochDate;
        private GSUI UI_maxPlanetSize;
        private GSUI UI_minPlanetSize;

        public string Name => "NiceSol";
        public string Author => "Kimi-Arthur";
        public string Description => "88 Constellations & Realistic Dynamic Solar System";
        public string Version => "1.0";
        public string GUID => "space.customizing.generators.nicesol";

        public GSGeneratorConfig Config { get; } = new();
        public GSOptions Options { get; } = new();

        public void Init()
        {
            Config.MinStarCount = 32;
            Config.MaxStarCount = 1000;
            Config.DefaultStarCount = 500;

            var todayDefault = DateTime.UtcNow.ToString("yyyy-MM-dd");
            UI_epochDate = Options.Add(GSUI.Input("Epoch Date (YYYY-MM-DD)".Translate(), todayDefault, "epochDate", null,
                "UTC date for Solar System & star orbits".Translate()));
            Options.Add(GSUI.Button("Reset Epoch Date".Translate(), "Today (UTC)".Translate(), _ =>
            {
                var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
                preferences.Set("epochDate", today);
                UI_epochDate?.Set(today);
            }, null, "Set Epoch Date to today's UTC date".Translate()));

            Options.Add(GSUI.Selector("Name Language".Translate(), new List<string> { "Auto", "Chinese", "English" }, "Auto", "nameLanguage", null,
                "Language for star and planet names".Translate()));
            Options.Add(GSUI.Checkbox("Accurate Stars".Translate(), true, "accurateStars", null,
                "Use real star spectral & remnant types".Translate()));
            Options.Add(GSUI.Checkbox("Start in Sol".Translate(), true, "startInSol", null,
                "Start on Earth in the Solar System".Translate()));

            Options.Add(GSUI.Slider("Max Star Distance (ly)".Translate(), 40f, 95f, 200f, 5f, "maxStarDistance", null,
                "Outer radius of 88-constellation sphere".Translate()));
            Options.Add(GSUI.Slider("Sol Orbit Speed Multi".Translate(), 0.25f, 1f, 4f, 0.25f, "solOrbitSpeedMulti", null,
                "Sol orbital speed multi (1x = 4h year)".Translate()));
            Options.Add(GSUI.Slider("Sol System Day/Night Multi".Translate(), 0.5f, 1f, 10f, 0.5f, "IncreaseRotation", null,
                "Sol day/night multi (1x = 12m day)".Translate()));
            Options.Add(GSUI.Slider("Max planets per system".Translate(), 1, 10, 99, "maxPlanetCount"));

            UI_minPlanetSize = Options.Add(GSUI.PlanetSizeSlider("Min planet size".Translate(), 20, 30, 500, o =>
            {
                var maxSize = preferences.GetFloat("maxPlanetSize", 300);
                if (maxSize == -1f) maxSize = 500;
                if (maxSize < o.Int()) o = maxSize;
                preferences.Set("minPlanetSize", (float)o);
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize", 30));
            }, () => UI_minPlanetSize.Set(preferences.GetInt("minPlanetSize", 30))));

            UI_maxPlanetSize = Options.Add(GSUI.PlanetSizeSlider("Max planet size".Translate(), 50, 300, 500, o =>
            {
                var minSize = preferences.GetFloat("minPlanetSize", 30);
                if (minSize == -1f) minSize = 5;
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", (float)o);
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }, () => UI_maxPlanetSize.Set(preferences.GetInt("maxPlanetSize", 300))));

            Options.Add(GSUI.PlanetSizeSlider("Starting planet size".Translate(), 100, 400, 500, "birthPlanetSize"));
            Options.Add(GSUI.Checkbox("Birth planet Si/Ti".Translate(), false, "birthPlanetSiTi"));
            Options.Add(GSUI.Checkbox("Moons are small".Translate(), true, "moonsAreSmall"));
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            GSSettings.Reset(GSSettings.Seed);
            GSSettings.GalaxyParams.iterations = 1;
            GSSettings.GalaxyParams.graphDistance = 256;
            GSSettings.GalaxyParams.graphMaxStars = 1024;
            random = new GS2.Random(GSSettings.Seed);

            var useChinese = UseChineseNames();
            var accurateStars = preferences.GetBool("accurateStars", true);
            var maxStarDistance = Mathf.Clamp(preferences.GetFloat("maxStarDistance", 95f), 40f, 250f);
            var epochRaw = preferences.GetString("epochDate", DateTime.UtcNow.ToString("yyyy-MM-dd"));
            if (!NiceSolEphemeris.TryParseEpochJulianDay(epochRaw, out var epochJd, out var normalizedDate))
                GS2.Warn($"NiceSol: invalid epochDate '{epochRaw}', falling back to {normalizedDate}");

            var builtStars = NiceSolCatalog.BuildStars(epochJd, maxStarDistance, useChinese, accurateStars, GSSettings.Seed, starCount);
            if (builtStars.Count == 0)
            {
                GS2.Error("NiceSol: constellation catalog loaded 0 stars.");
                return;
            }

            for (var i = 0; i < builtStars.Count; i++) GSSettings.Stars.Add(builtStars[i]);

            GS2Generator2.SetupBaseThemes();
            GS2Generator2.InitThemes();

            var earth = GenerateSol(GSSettings.Stars[0], epochJd, useChinese);

            for (var i = 1; i < builtStars.Count; i++)
            {
                var star = GSSettings.Stars[i];
                if (star.Decorative || star.Planets.Count > 0) continue;
                GenerateExoplanets(star);
            }

            if (preferences.GetBool("startInSol", true))
            {
                birthPlanet = earth;
                GSSettings.BirthPlanetName = earth.Name;
            }
            else
            {
                PickNewBirthPlanet(earth);
                GSSettings.BirthPlanetName = birthPlanet.Name;
            }

            if (birthPlanet != null)
            {
                var desiredSize = Mathf.Clamp(preferences.GetInt("birthPlanetSize", 400), 100, 500);
                if (desiredSize != birthPlanet.Radius) birthPlanet.Radius = Utils.ParsePlanetSize(desiredSize);
            }

            if (preferences.GetBool("birthPlanetSiTi", false) && birthPlanet != null)
                ApplyBirthPlanetSiTi(birthPlanet, random);
        }

        private static void EnsureThemeVeinsPopulated(GSTheme theme)
        {
            if (theme != null && theme.VeinSettings.VeinTypes.Count == 0 && theme.VeinSpot != null && theme.VeinSpot.Length > 0)
                theme.PopulateVeinData();
        }

        public static void ApplyBirthPlanetSiTi(GSPlanet planet, GS2.Random rng = null)
        {
            if (planet == null) return;
            var theme = planet.GsTheme;
            if (theme == null) return;

            EnsureThemeVeinsPopulated(theme);
            var settings = planet.veinSettings ?? theme.VeinSettings.Clone();
            if (settings.Algorithm == "Vanilla") settings.Algorithm = theme.Algo == 7 ? "GS2W" : "GS2";

            var r = rng ?? new GS2.Random(planet.Seed);
            if (!settings.VeinTypes.ContainsVein(EVeinType.Silicium))
            {
                var vt = new GSVeinType(EVeinType.Silicium);
                var count = r.Next(3, 7);
                for (var i = 0; i < count; i++) vt.veins.Add(new GSVein(r.Next(8, 16), 0.6f));
                settings.VeinTypes.Add(vt);
            }

            if (!settings.VeinTypes.ContainsVein(EVeinType.Titanium))
            {
                var vt = new GSVeinType(EVeinType.Titanium);
                var count = r.Next(3, 7);
                for (var i = 0; i < count; i++) vt.veins.Add(new GSVein(r.Next(8, 16), 0.6f));
                settings.VeinTypes.Add(vt);
            }

            planet.veinSettings = settings;
        }

        public bool UseChineseNames()
        {
            var pref = preferences.GetString("nameLanguage", "Auto");
            if (string.Equals(pref, "Chinese", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(pref, "English", StringComparison.OrdinalIgnoreCase)) return false;
            return Localization.CurrentLanguageLCID == 2052;
        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences ?? new GSGenPreferences();
            if (this.preferences.ContainsKey("birthPlanetSize") && this.preferences.GetInt("birthPlanetSize", 400) < 100)
                this.preferences.Set("birthPlanetSize", 100);
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }

        public void OnUpdate(string key, Val val)
        {
            preferences.Set(key, val);
        }

        private static string SolBodyName(bool useChinese, string enName, string zhName)
        {
            return useChinese ? zhName : enName;
        }

        private static float CompressOrbitAU(double realAU)
        {
            return (float)Math.Pow(realAU, 0.6);
        }

        private static float ComputeSolPeriod(float compressedAU, float orbitSpeedMulti)
        {
            // 1.0 AU Earth = 14,400 s (4.0 hr), giving 20 sidereal / 19 solar days per Earth year at 12 min Earth day
            return 14400f * Mathf.Pow(compressedAU, 1.2f) / orbitSpeedMulti;
        }

        public GSPlanet GenerateSol(GSStar sol, double jd, bool useChinese)
        {
            var orbitSpeedMulti = Mathf.Clamp(preferences.GetFloat("solOrbitSpeedMulti", 1f), 0.1f, 10f);
            var dayNightMulti = Mathf.Clamp(preferences.GetFloat("IncreaseRotation", 1f), 0.1f, 50f);
            var dSinceJ2000 = jd - 2451545.0;

            ref var planets = ref sol.Planets;
            planets.Clear();

            // 1. Mercury
            var mercPose = NiceSolEphemeris.ComputePlanetPose("Mercury", jd);
            var mercR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Mercury"].a);
            var mercT = ComputeSolPeriod(mercR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Mercury", "水星"), "Barren", 150, mercR,
                mercPose.orbitInclination, mercT, mercPose.orbitPhase, 0.034f,
                mercT * (2f / 3f), 0f, 9.08f, null)
            { OrbitLongitude = mercPose.orbitLongitude, Scale = 1f });

            // 2. Venus (retrograde spin)
            var venusPose = NiceSolEphemeris.ComputePlanetPose("Venus", jd);
            var venusR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Venus"].a);
            var venusT = ComputeSolPeriod(venusR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Venus", "金星"), "AcidGreenhouse", 320, venusR,
                venusPose.orbitInclination, venusT, venusPose.orbitPhase, 177.36f,
                -3600f * dayNightMulti, 0f, 2.60f, null)
            { OrbitLongitude = venusPose.orbitLongitude, Scale = 1f });

            // 3. Earth + Luna
            var earthPose = NiceSolEphemeris.ComputePlanetPose("Earth", jd, isEarth: true);
            var lunaPose = NiceSolEphemeris.ComputeLunaPose(jd);
            var lunaT = 2400f / orbitSpeedMulti;
            var luna = new GSPlanet(
                SolBodyName(useChinese, "The Moon", "月球"), "BarrenSatellite", 110, 0.050f,
                lunaPose.orbitInclination, lunaT, lunaPose.orbitPhase, lunaPose.obliquity,
                lunaT, 0f, 1.36f)
            { OrbitLongitude = lunaPose.orbitLongitude, Scale = 1f };

            var earthT = 14400f / orbitSpeedMulti;
            var earthRotPhase = NiceSolEphemeris.ComputeEarthRotationPhase(jd, earthPose.orbitPhase);
            var earth = new GSPlanet(
                SolBodyName(useChinese, "Earth", "地球"), "Mediterranean", 400, 1.0f,
                earthPose.orbitInclination, earthT, earthPose.orbitPhase, earthPose.obliquity,
                720f * dayNightMulti, earthRotPhase, 1.36f, new GSPlanets { luna })
            { OrbitLongitude = earthPose.orbitLongitude, Scale = 1f };
            planets.Add(earth);

            // 4. Mars
            var marsPose = NiceSolEphemeris.ComputePlanetPose("Mars", jd);
            var marsR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Mars"].a);
            var marsT = ComputeSolPeriod(marsR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Mars", "火星"), "AridDesert", 210, marsR,
                marsPose.orbitInclination, marsT, marsPose.orbitPhase, 25.19f,
                739f * dayNightMulti, 0f, 0.58f, null)
            { OrbitLongitude = marsPose.orbitLongitude, Scale = 1f });

            // 5. Ceres
            var ceresPose = NiceSolEphemeris.ComputePlanetPose("Ceres", jd);
            var ceresR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Ceres"].a);
            var ceresT = ComputeSolPeriod(ceresR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Ceres", "谷神星"), "DwarfPlanet", 30, ceresR,
                ceresPose.orbitInclination, ceresT, ceresPose.orbitPhase, 4.0f,
                480f * dayNightMulti, 0f, 0.29f, null)
            { OrbitLongitude = ceresPose.orbitLongitude, Scale = 1f });

            // 6. Jupiter + 4 Galilean Moons
            var ioPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(3.13, 336.0, 106.08 + 203.4890 * dSinceJ2000);
            var europaPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(3.13, 336.0, 175.73 + 101.3747 * dSinceJ2000);
            var ganyPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(3.13, 336.0, 120.55 + 50.3176 * dSinceJ2000);
            var callPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(3.13, 336.0, 84.44 + 21.5711 * dSinceJ2000);

            var ioT = 900f / orbitSpeedMulti;
            var europaT = 1800f / orbitSpeedMulti;
            var ganyT = 3600f / orbitSpeedMulti;
            var callT = 8400f / orbitSpeedMulti;

            var jovianMoons = new GSPlanets
            {
                new(SolBodyName(useChinese, "Io", "木卫一"), "Lava", 110, 0.18f, ioPose.orbitInclination, ioT, ioPose.orbitPhase, 0f, ioT, 0f, 0.05f) { OrbitLongitude = ioPose.orbitLongitude, Scale = 1f },
                new(SolBodyName(useChinese, "Europa", "木卫二"), "IceGelisol", 100, 0.23f, europaPose.orbitInclination, europaT, europaPose.orbitPhase, 0f, europaT, 0f, 0.05f) { OrbitLongitude = europaPose.orbitLongitude, Scale = 1f },
                new(SolBodyName(useChinese, "Ganymede", "木卫三"), "IceGelisol", 160, 0.29f, ganyPose.orbitInclination, ganyT, ganyPose.orbitPhase, 0f, ganyT, 0f, 0.05f) { OrbitLongitude = ganyPose.orbitLongitude, Scale = 1f },
                new(SolBodyName(useChinese, "Callisto", "木卫四"), "AshenGelisol", 150, 0.36f, callPose.orbitInclination, callT, callPose.orbitPhase, 0f, callT, 0f, 0.05f) { OrbitLongitude = callPose.orbitLongitude, Scale = 1f }
            };

            var jupPose = NiceSolEphemeris.ComputePlanetPose("Jupiter", jd);
            var jupR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Jupiter"].a);
            var jupT = ComputeSolPeriod(jupR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Jupiter", "木星"), "GasGiant", 450, jupR,
                jupPose.orbitInclination, jupT, jupPose.orbitPhase, 3.13f,
                480f * dayNightMulti, 0f, 0.0503f, jovianMoons)
            { OrbitLongitude = jupPose.orbitLongitude, Scale = 10f });

            // 7. Saturn + Titan
            var titanPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(26.73, 169.5, 186.59 + 22.5770 * dSinceJ2000);
            var titanT = 3200f / orbitSpeedMulti;
            var titan = new GSPlanet(
                SolBodyName(useChinese, "Titan", "土卫六"), "AshenGelisol", 160, 0.22f,
                titanPose.orbitInclination, titanT, titanPose.orbitPhase, 0f,
                titanT, 0f, 0.015f)
            { OrbitLongitude = titanPose.orbitLongitude, Scale = 1f };

            var satPose = NiceSolEphemeris.ComputePlanetPose("Saturn", jd);
            var satR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Saturn"].a);
            var satT = ComputeSolPeriod(satR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Saturn", "土星"), "GasGiant2", 380, satR,
                satPose.orbitInclination, satT, satPose.orbitPhase, 26.73f,
                510f * dayNightMulti, 0f, 0.0148f, new GSPlanets { titan })
            { OrbitLongitude = satPose.orbitLongitude, Scale = 10f });

            // 8. Uranus (extreme axial tilt / retrograde)
            var uraPose = NiceSolEphemeris.ComputePlanetPose("Uranus", jd);
            var uraR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Uranus"].a);
            var uraT = ComputeSolPeriod(uraR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Uranus", "天王星"), "IceGiant", 160, uraR,
                uraPose.orbitInclination, uraT, uraPose.orbitPhase, 97.77f,
                -600f * dayNightMulti, 0f, 0.0037f, null)
            { OrbitLongitude = uraPose.orbitLongitude, Scale = 10f });

            // 9. Neptune + Triton (retrograde inclined moon)
            var tritonPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(156.87, 177.7, 264.76 + 61.2573 * dSinceJ2000);
            var tritonT = 2100f / orbitSpeedMulti;
            var triton = new GSPlanet(
                SolBodyName(useChinese, "Triton", "海卫一"), "IceGelisol", 80, 0.18f,
                tritonPose.orbitInclination, tritonT, tritonPose.orbitPhase, 0f,
                tritonT, 0f, 0.0015f)
            { OrbitLongitude = tritonPose.orbitLongitude, Scale = 1f };

            var nepPose = NiceSolEphemeris.ComputePlanetPose("Neptune", jd);
            var nepR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Neptune"].a);
            var nepT = ComputeSolPeriod(nepR, orbitSpeedMulti);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Neptune", "海王星"), "IceGiant2", 155, nepR,
                nepPose.orbitInclination, nepT, nepPose.orbitPhase, 28.32f,
                570f * dayNightMulti, 0f, 0.0015f, new GSPlanets { triton })
            { OrbitLongitude = nepPose.orbitLongitude, Scale = 10f });

            // 10. Pluto + Charon (clean direct orbit without invisible Center dummy body)
            var charonPose = NiceSolEphemeris.ComputeBodyPoseFromAngles(119.6, 223.0, 85.0 + 56.3625 * dSinceJ2000);
            var plutoT = ComputeSolPeriod(CompressOrbitAU(NiceSolEphemeris.PlanetElements["Pluto"].a), orbitSpeedMulti);
            var charonT = 1800f / orbitSpeedMulti;
            var charon = new GSPlanet(
                SolBodyName(useChinese, "Charon", "冥卫一"), "BarrenSatellite", 30, 0.020f,
                charonPose.orbitInclination, charonT, charonPose.orbitPhase, 122.53f,
                charonT, 0f, 0.0009f)
            { OrbitLongitude = charonPose.orbitLongitude, Scale = 1f };

            var plutoPose = NiceSolEphemeris.ComputePlanetPose("Pluto", jd);
            var plutoR = CompressOrbitAU(NiceSolEphemeris.PlanetElements["Pluto"].a);
            planets.Add(new GSPlanet(
                SolBodyName(useChinese, "Pluto", "冥王星"), "AshenGelisol", 70, plutoR,
                plutoPose.orbitInclination, plutoT, plutoPose.orbitPhase, 122.53f,
                charonT, 0f, 0.0009f, new GSPlanets { charon })
            { OrbitLongitude = plutoPose.orbitLongitude, Scale = 1f });

            return earth;
        }

        private void GenerateExoplanets(GSStar star)
        {
            var starRng = new GS2.Random(star.Seed);
            var maxPlanets = Mathf.Clamp(preferences.GetInt("maxPlanetCount", 10), 1, 99);
            var bodyCount = starRng.Next(1, maxPlanets);
            var planetCount = 1 + starRng.Next(Mathf.RoundToInt(bodyCount / 3f), Math.Max(1, bodyCount - 1));
            var moonCount = Math.Max(0, bodyCount - planetCount);
            availMoons = moonCount;

            var forceTelluricForRemnant = star.Type == EStarType.BlackHole || star.Type == EStarType.NeutronStar;

            for (var j = 0; j < planetCount; j++)
            {
                var planetMoonCount = 0;
                for (var m = 0; m < 6; m++)
                {
                    if (availMoons > 0 && starRng.NextFloat() < 2f * ((j + 1f) / planetCount))
                    {
                        planetMoonCount++;
                        availMoons--;
                    }
                }

                var roman = j + 1 < RomanNumbers.roman.Length ? RomanNumbers.roman[j + 1] : (j + 1).ToString();
                var planetName = $"{star.Name} - {roman}";
                var forceTelluric = forceTelluricForRemnant && j == 0;
                var p = CreateExoPlanet(star, starRng, planetName, j, planetCount, planetMoonCount, forceTelluric);
                if (p.OrbitRadius < star.MaxOrbit) star.Planets.Add(p);
                else break;
            }
        }

        private GSPlanet CreateExoPlanet(GSStar star, GS2.Random starRng, string name, int orbitIndex, int orbitCount, int moonCount, bool forceTelluric)
        {
            var hotOrbitMax = Mathf.RoundToInt(orbitCount / 6.66f);
            var frozenOrbitMax = orbitCount - hotOrbitMax;
            var warmOrbitMax = hotOrbitMax * 2;
            var coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            var temperateOrbitMax = Mathf.RoundToInt((coldOrbitMax - warmOrbitMax) / 2f + warmOrbitMax);

            EThemeHeat themeHeat;
            float chanceTiny, chanceHuge, chanceGas;
            if (orbitIndex < hotOrbitMax)
            {
                themeHeat = EThemeHeat.Hot;
                chanceTiny = 0.5f; chanceGas = 0.1f; chanceHuge = 0.1f;
            }
            else if (orbitIndex < warmOrbitMax)
            {
                themeHeat = EThemeHeat.Warm;
                chanceTiny = 0.3f; chanceGas = 0.05f; chanceHuge = 0.25f;
            }
            else if (orbitIndex < temperateOrbitMax)
            {
                themeHeat = EThemeHeat.Temperate;
                chanceTiny = 0.2f; chanceGas = 0.1f; chanceHuge = 0.3f;
            }
            else if (orbitIndex < coldOrbitMax)
            {
                themeHeat = EThemeHeat.Cold;
                chanceTiny = 0.2f; chanceGas = 0.2f; chanceHuge = 0.3f;
            }
            else
            {
                themeHeat = EThemeHeat.Frozen;
                chanceTiny = 0.6f; chanceGas = 0.1f; chanceHuge = 0.3f;
            }

            if (forceTelluric) chanceGas = 0f;

            var tiny = starRng.NextFloat() < chanceTiny;
            var huge = starRng.NextFloat() < chanceHuge;
            var gas = starRng.NextFloat() < chanceGas;

            int radius;
            float scale;
            string themeName;

            if (gas)
            {
                scale = 10f;
                radius = !tiny && !huge ? starRng.Next(100, 200)
                    : tiny && !huge ? starRng.Next(60, 200)
                    : huge && !tiny ? starRng.Next(200, 510)
                    : starRng.Next(60, 200);
                radius = Utils.ParsePlanetSize(radius);
                var gasThemes = GSSettings.ThemeLibrary.Query(EThemeType.Gas, themeHeat, radius);
                themeName = gasThemes.Count > 0 ? gasThemes[starRng.Next(gasThemes.Count)] : "GasGiant";
            }
            else
            {
                scale = 1f;
                radius = ChoosePlanetRadius(starRng, tiny, huge);
                var telThemes = GSSettings.ThemeLibrary.Query(EThemeType.Planet, themeHeat, radius);
                themeName = telThemes.Count > 0 ? telThemes[starRng.Next(telThemes.Count)] : "Barren";
            }

            var rotationPeriod = Mathf.Max(360f, starRng.Next(480, 2400));
            var planet = new GSPlanet(
                name, themeName, radius,
                -1f,
                starRng.NextFloat() * 4f + starRng.NextFloat() * 5f,
                -1f,
                starRng.Next(360),
                starRng.NextFloat() * 20f,
                rotationPeriod,
                starRng.Next(360),
                -1f)
            {
                Scale = scale
            };

            if (moonCount < 6 && gas && availMoons > 6) moonCount = starRng.Next(moonCount, 6);
            if (moonCount > 0)
            {
                planet.Moons = new GSPlanets();
                for (var i = 0; i < moonCount; i++)
                {
                    var mRoman = i + 1 < RomanNumbers.roman.Length ? RomanNumbers.roman[i + 1] : (i + 1).ToString();
                    planet.Moons.Add(CreateExoMoon(starRng, planet, $"{name} - {mRoman}", i, moonCount, themeHeat));
                }
            }

            planet.OrbitRadius = CalculateNextAvailableOrbit(star, planet);
            planet.OrbitalPeriod = Utils.CalculateOrbitPeriodFromStarMass(planet.OrbitRadius, Mathf.Max(0.2f, star.mass));

            if (starRng.NextDouble() < 0.02) planet.OrbitalPeriod = -planet.OrbitalPeriod;
            if (planet.OrbitRadius < 0.6f && starRng.NextFloat() < 0.5f)
                planet.RotationPeriod = planet.OrbitalPeriod;
            else if (planet.OrbitRadius < 1.5f && starRng.NextFloat() < 0.2f)
                planet.RotationPeriod = planet.OrbitalPeriod / 2f;

            if (starRng.NextDouble() < 0.05) planet.Obliquity = starRng.NextFloat(20f, 85f);
            if (starRng.NextDouble() < 0.05) planet.OrbitInclination = starRng.NextFloat(20f, 85f);

            return planet;
        }

        private GSPlanet CreateExoMoon(GS2.Random starRng, GSPlanet host, string name, int index, int orbitCount, EThemeHeat themeHeat)
        {
            var maxHostBound = Mathf.Clamp(host.Radius - 10, 30, 510);
            var radius = Utils.ParsePlanetSize(starRng.Next(30, Math.Max(31, maxHostBound)));
            if (preferences.GetBool("moonsAreSmall", true) && radius > 200) radius = Utils.ParsePlanetSize(starRng.Next(30, 190));

            var moonThemes = GSSettings.ThemeLibrary.Query(EThemeType.Moon, themeHeat, radius);
            var theme = moonThemes.Count > 0 ? moonThemes[starRng.Next(moonThemes.Count)] : "BarrenSatellite";

            var moon = new GSPlanet(
                name, theme, radius, -1f,
                starRng.NextFloat() * 12f, -1f,
                starRng.NextFloat(360f), starRng.NextFloat(0f, 45f),
                starRng.Next(480, 2400), 0f, -1f)
            {
                Scale = 1f
            };
            moon.OrbitRadius = CalculateNextAvailableMoonOrbit(starRng, host, moon);
            moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
            if (orbitCount > 0 && index / (float)orbitCount < starRng.NextFloat() && starRng.NextBool())
                moon.RotationPeriod = moon.OrbitalPeriod;
            return moon;
        }

        private int ChoosePlanetRadius(GS2.Random starRng, bool tiny, bool huge)
        {
            var maxPlanetSize = preferences.GetInt("maxPlanetSize", 300);
            var minPlanetSize = preferences.GetInt("minPlanetSize", 30);
            if (minPlanetSize > maxPlanetSize) minPlanetSize = maxPlanetSize;
            var avgPlanetSize = (maxPlanetSize - minPlanetSize) / 2 + minPlanetSize;

            int radius;
            if (!tiny && !huge)
                radius = starRng.Next(Mathf.Min(minPlanetSize + 100, avgPlanetSize), Mathf.Max(maxPlanetSize - 100, avgPlanetSize) + 1);
            else if (tiny && !huge)
                radius = starRng.Next(Mathf.Max(minPlanetSize, 30), Mathf.Max(Mathf.Max(minPlanetSize, 30) + 1, Mathf.Min(70, maxPlanetSize)));
            else if (huge && !tiny)
                radius = starRng.Next(Mathf.Min(avgPlanetSize, Mathf.Max(minPlanetSize, maxPlanetSize - 100)), Mathf.Min(maxPlanetSize, 510) + 1);
            else
                radius = starRng.Next(minPlanetSize, maxPlanetSize + 1);
            return Utils.ParsePlanetSize(radius);
        }

        private static float CalculateNextAvailableMoonOrbit(GS2.Random starRng, GSPlanet host, GSPlanet moon)
        {
            var jitter = starRng.NextFloat(0.008f, 0.018f);
            var hostSize = host.RadiusAU;
            var moonSize = moon.RadiusAU;
            var hostClearance = hostSize * 0.5f;
            if (moon.Radius > 200f) jitter = Mathf.Max(jitter, 0.025f);
            if (host.Moons == null || host.Moons.Count < 1) return hostSize + moonSize + hostClearance + jitter;
            var lastMoon = host.Moons[host.Moons.Count - 1];
            var lastOrbit = lastMoon.OrbitRadius + lastMoon.SystemRadius;
            return lastOrbit + moon.SystemRadius + hostClearance + jitter;
        }

        private static float CalculateNextAvailableOrbit(GSStar star, GSPlanet planet)
        {
            var starRng = new GS2.Random(star.Seed ^ (star.Planets.Count + 1));
            var gap = starRng.NextDouble() < 0.1 ? starRng.NextFloat(0.15f, 1.8f) : starRng.NextFloat(0.4f, 1.0f);
            var minInner = Mathf.Max(star.RadiusAU * 2.2f, star.acDiscRadius * 0.02f + 0.15f, 0.25f);
            var sysRad = planet.SystemRadius;

            if (star.Planets == null || star.Planets.Count < 1)
                return Mathf.Max(minInner + sysRad, gap + planet.RadiusAU);

            var lastPlanet = star.Planets[star.Planets.Count - 1];
            var lastOrbitEnd = lastPlanet.OrbitRadius + lastPlanet.SystemRadius;
            return Mathf.Max(minInner + sysRad, lastOrbitEnd + sysRad + gap);
        }

        private void PickNewBirthPlanet(GSPlanet earthFallback)
        {
            if (GSSettings.StarCount < 2)
            {
                birthPlanet = earthFallback;
                return;
            }

            var habitable = GSSettings.Stars.HabitablePlanets;
            if (habitable.Count <= 1)
            {
                var star = GSSettings.Stars[random.Next(1, GSSettings.StarCount)];
                if (star.PlanetCount == 0)
                {
                    birthPlanet = earthFallback;
                    return;
                }
                var index = star.PlanetCount > 1 ? Mathf.RoundToInt((star.PlanetCount - 1) / 2f) : 0;
                var planet = star.Planets[index];
                var themeNames = GSSettings.ThemeLibrary.Habitable;
                planet.Theme = themeNames[random.Next(themeNames.Count)];
                planet.Radius = Utils.ParsePlanetSize(Mathf.Clamp(preferences.GetInt("birthPlanetSize", 400), 100, 500));
                birthPlanet = planet;
                return;
            }

            var candidates = new List<GSPlanet>();
            foreach (var p in habitable)
                if (p != earthFallback) candidates.Add(p);

            birthPlanet = candidates.Count > 0 ? candidates[random.Next(candidates.Count)] : earthFallback;
        }
    }
}
