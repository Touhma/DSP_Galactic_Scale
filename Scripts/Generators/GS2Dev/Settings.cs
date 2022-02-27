using System.Collections.Generic;
using System.Linq;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2
    {
        private static bool loaded;

        private readonly string[] typeDesc =
        {
            "Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O", "White Dwarf", "Red Giant",
            "Yellow Giant", "White Giant",
            "Blue Giant", "Neutron Star", "Black Hole"
        };

        private readonly string[] typeLetter = { "K", "M", "F", "G", "A", "B", "O", "WD", "RG", "YG", "WG", "BG", "NS", "BH" };

        // ReSharper disable once InconsistentNaming
        private readonly Dictionary<string, GSUI> UI = new();

        private List<string> _forcedStars = new();

        private Dictionary<string, FloatPair> hzDefs = new();
        private GSGenPreferences preferences = new();
        private Dictionary<string, double> starFreq = new();
        public GSGeneratorConfig Config { get; } = new();

        public GSOptions Options { get; } = new();

        public void Init()
        {
            Config.enableStarSelector = true;
            AddUIElements();
            InitPreferences();
        }

        public void Import(GSGenPreferences importedPreferences)
        {
            // Warn("Importing Preferences");
            for (var i = 0; i < importedPreferences.Count; i++)
            {
                var key = importedPreferences.Keys.ElementAt(i);
                // Warn($"Setting:{key} to {importedPreferences[key]}");
                preferences.Set(key, importedPreferences[key]);
                if (loaded && UI.ContainsKey(key))
                {
                    // Warn("UI Loaded, setting UI Element to match");
                    UI[key].Set(importedPreferences[key]);
                }
            }

            if (loaded) loaded = false;
            Config.DefaultStarCount = importedPreferences.GetInt("defaultStarCount");
            //if (!importedPreferences.GetBool("ludicrousMode")) Config.MaxStarCount = 1024;
        }

        public GSGenPreferences Export()
        {
            // Warn("Exporting Preferences");
            return preferences;
        }

        private string GetTypeLetterFromStar(GSStar star)
        {
            switch (star.Type)
            {
                case EStarType.BlackHole: return "BH";
                case EStarType.NeutronStar: return "NS";
                case EStarType.WhiteDwarf: return "WD";
                case EStarType.GiantStar: return GetGiantStarTypeLetter(star.Spectr);
                case EStarType.MainSeqStar: return GetMainSeqStarTypeLetter(star.Spectr);
                default:
                    Error($"Failed to get star type letter from star of type {star.Type} spectr {star.Spectr}");
                    return "WD";
            }
        }

        private static string GetMainSeqStarTypeLetter(ESpectrType spectr)
        {
            switch (spectr)
            {
                case ESpectrType.A: return "A";
                case ESpectrType.O: return "O";
                case ESpectrType.B: return "B";
                case ESpectrType.G: return "G";
                case ESpectrType.F: return "F";
                case ESpectrType.M: return "M";
                case ESpectrType.K: return "K";
            }

            Error($"Failed to get star type letter from main sequence star of spectr {spectr}");
            return "WD";
        }

        private string GetGiantStarTypeLetter(ESpectrType spectr)
        {
            switch (spectr)
            {
                case ESpectrType.A: return "WG";
                case ESpectrType.O:
                case ESpectrType.B: return "BG";
                case ESpectrType.G:
                case ESpectrType.F: return "YG";
                case ESpectrType.M:
                case ESpectrType.K: return "RG";
            }

            Error($"Failed to get star type letter from giant star of spectr {spectr}");
            return "WD";
        }

        private void DefaultStarCountCallback(Val o)
        {
            Config.DefaultStarCount = preferences.GetInt("defaultStarCount", 64);
        }

        private void Reset(Val o)
        {
            preferences = new GSGenPreferences();
            InitPreferences();
            foreach (var ui in UI)
            {
                Log($"Resetting {ui.Key}");
                if (GSUI.Settables.Contains(ui.Value.Type)) ui.Value.Set(preferences.Get(ui.Key));
            }
        }

        private void GetHz()
        {
            hzDefs = new Dictionary<string, FloatPair>
            {
                { "K", Utils.CalculateHabitableZone(1.2f) },
                { "M", Utils.CalculateHabitableZone(0.6f) },
                { "A", Utils.CalculateHabitableZone(2.1f) },
                { "B", Utils.CalculateHabitableZone(5f) },
                { "F", Utils.CalculateHabitableZone(1.3f) },
                { "G", Utils.CalculateHabitableZone(1f) },
                { "O", Utils.CalculateHabitableZone(12f) },
                { "RG", Utils.CalculateHabitableZone(.85f) },
                { "YG", Utils.CalculateHabitableZone(1f) },
                { "WG", Utils.CalculateHabitableZone(6f) },
                { "BG", Utils.CalculateHabitableZone(10f) },
                { "WD", Utils.CalculateHabitableZone(0.05f) },
                { "NS", Utils.CalculateHabitableZone(0.35f) },
                { "BH", Utils.CalculateHabitableZone(0.006f) }
            };
        }

        private void InitPreferences()
        {
            Log("InitPreferences");
            GetHz();
            foreach (var hz in hzDefs) preferences.Set($"{hz.Key}hz", hz.Value);
            preferences.Set("birthPlanetStar", 14);
            preferences.Set("rotationMulti", 1f);
            preferences.Set("innerPlanetDistance", 1f);
            preferences.Set("allowResonances", true);
            preferences.Set("galaxyDensity", 5);
            preferences.Set("defaultStarCount", 64);
            preferences.Set("starSizeMulti", 10);
            preferences.Set("binaryChance", 0);
            preferences.Set("birthPlanetSize", 200);
            preferences.Set("birthPlanetUnlock", false);
            preferences.Set("birthPlanetSiTi", false);
            preferences.Set("birthTidalLock", false);
            preferences.Set("birthRareDisable", true);
            preferences.Set("limitPlanetSize20", false);
            preferences.Set("limitPlanetSize50", false);
            preferences.Set("limitPlanetSize100", false);
            preferences.Set("limitPlanetSize200", false);
            preferences.Set("limitPlanetSize300", false);
            preferences.Set("limitPlanetSize400", false);
            preferences.Set("limitPlanetSize500", false);
            preferences.Set("moonsAreSmall", true);
            preferences.Set("moonBias", 50);
            preferences.Set("hugeGasGiants", true);
            preferences.Set("tidalLockInnerPlanets", false);
            preferences.Set("secondarySatellites", false);
            preferences.Set("moonBias", 50);
            preferences.Set("planetCount", new FloatPair(1, 10));
            preferences.Set("planetSize", new FloatPair(50, 500));
            preferences.Set("sizeBias", 50);
            preferences.Set("countBias", 50);
            preferences.Set("freqK", 40);
            preferences.Set("freqM", 50);
            preferences.Set("freqG", 30);
            preferences.Set("freqF", 25);
            preferences.Set("freqA", 10);
            preferences.Set("freqB", 4);
            preferences.Set("freqO", 2);
            preferences.Set("freqBH", 1);
            preferences.Set("freqNS", 1);
            preferences.Set("freqWD", 2);
            preferences.Set("freqRG", 1);
            preferences.Set("freqYG", 1);
            preferences.Set("freqWG", 1);
            preferences.Set("freqBG", 1);
            preferences.Set("chanceGas", 20);
            preferences.Set("chanceMoon", 20);
            preferences.Set("chanceMoonMoon", 5);
            preferences.Set("orbitOverride", false);
            preferences.Set("hzOverride", false);
            preferences.Set("hz", new FloatPair(0.9f, 2));
            preferences.Set("orbits", new FloatPair(0.1f, 10));
            preferences.Set("inclination", -1);
            preferences.Set("orbitLongitude", -1);
            preferences.Set("rareChance", -1f);
            preferences.Set("luminosityBoost", 1);
            for (var i = 0; i < 14; i++)
            {
                preferences.Set($"{typeLetter[i]}minStars", 0);
                preferences.Set($"{typeLetter[i]}planetCount", new FloatPair(1, 10));
                preferences.Set($"{typeLetter[i]}planetSize", new FloatPair(50, 500));
                preferences.Set($"{typeLetter[i]}sizeBias", 50);
                preferences.Set($"{typeLetter[i]}countBias", 50);
                preferences.Set($"{typeLetter[i]}chanceGas", 20);
                preferences.Set($"{typeLetter[i]}chanceMoon", 20);
                preferences.Set($"{typeLetter[i]}orbits", new FloatPair(0.9f, 2));
                preferences.Set($"{typeLetter[i]}orbits", new FloatPair(0.1f, 10));
                preferences.Set($"{typeLetter[i]}orbitOverride", false);
                preferences.Set($"{typeLetter[i]}inclination", -1);
                preferences.Set($"{typeLetter[i]}orbitLongitude", 0);
                preferences.Set($"{typeLetter[i]}hzOverride", false);
                preferences.Set($"{typeLetter[i]}rareChance", -1f);
                preferences.Set($"{typeLetter[i]}luminosityBoost", 0);
                preferences.Set($"{typeLetter[i]}binaryEnabled", false);
            }

            preferences.Set("KbinaryEnabled", true);
        }

        private void AddUIElements()
        {
            Options.Add(GSUI.Group("Galaxy Settings".Translate(), CreateGalaxyOptions(), "Settings that control Galaxy formation".Translate()));
            AddSpacer();
            Options.Add(GSUI.Group("Birth Planet Settings".Translate(), CreateBirthOptions(), "Settings that only affect the starting planet".Translate()));
            AddSpacer();
            Options.Add(GSUI.Group("System Settings".Translate(), CreateSystemOptions(), "Settings that control how systems are generated".Translate()));
            AddSpacer();
            Options.Add(GSUI.Group("Planet Settings".Translate(), CreatePlanetOptions(), "Settings that control how planets are generated".Translate()));
            AddSpacer();
            Options.Add(GSUI.Group("Star Specific Overrides".Translate(), CreateStarOverrideOptions(), "Settings that are star specific".Translate()));
            AddSpacer();
            Options.Add(GSUI.Button("Reset".Translate(), "Now".Translate(), Reset));
            loaded = true;
        }

        private GSOptions CreateSystemOptions()
        {
            var sOptions = new GSOptions();
            AddSpacer(sOptions);
            UI.Add("tidalLockInnerPlanets", sOptions.Add(GSUI.Checkbox("Tidal Lock Inner Planets".Translate(), false, "tidalLockInnerPlanets", null, "Force planets below the orbit threshold to be tidally locked".Translate())));
            UI.Add("innerPlanetDistance", sOptions.Add(GSUI.Slider("Inner Planet Distance (AU)".Translate(), 0, 1, 100, 0.1f, "innerPlanetDistance", null, "Distance forced tidal locking stops acting".Translate())));
            UI.Add("allowResonances", sOptions.Add(GSUI.Checkbox("Allow Orbital Harmonics".Translate(), true, "allowResonances", null, "Allow Orbital Resonance 1:2 and 1:4".Translate())));
            UI.Add("moonsAreSmall", sOptions.Add(GSUI.Checkbox("Moons Are Small".Translate(), true, "moonsAreSmall", null, "Try to ensure moons are 1/2 their planets size or less".Translate())));
            UI.Add("moonBias", sOptions.Add(GSUI.Slider("Gas Giants Moon Bias".Translate(), 0, 50, 100, "moonBias", null, "Lower prefers telluric planets, higher gas giants".Translate())));
            UI.Add("secondarySatellites", sOptions.Add(GSUI.Checkbox("Secondary satellites".Translate(), false, "secondarySatellites", null, "Allow moons to have moons".Translate())));
            UI.Add("chanceMoonMoon", sOptions.Add(GSUI.Slider("Secondary Satellite Chance".Translate(), 0, 5, 99, "chanceMoonMoon", null, "% Chance for a moon to have a moon".Translate())));
            AddSpacer(sOptions);
            sOptions.Add(GSUI.Header("Default Settings".Translate(), "Changing these will reset all star specific overrides".Translate()));
            UI.Add("planetCount", sOptions.Add(GSUI.RangeSlider("Planet Count".Translate(), 1, 2, 10, 99, 1f, "planetCount", null, PlanetCountLow, PlanetCountHigh, "The amount of planets per star".Translate())));
            UI.Add("countBias", sOptions.Add(GSUI.Slider("Planet Count Bias".Translate(), 0, 50, 100, "sizeBias", CountBiasCallback, "Prefer Less (lower) or More (higher) Planets".Translate())));
            UI.Add("hugeGasGiants", sOptions.Add(GSUI.Checkbox("Huge Gas Giants".Translate(), true, "hugeGasGiants", null, "Allow gas giants larger than 800 radius".Translate())));
            UI.Add("chanceGas", sOptions.Add(GSUI.Slider("Chance Gas".Translate(), 0, 20, 99, "chanceGas", GasChanceCallback, "% Chance of a planet being a Gas Giant".Translate())));
            UI.Add("chanceMoon", sOptions.Add(GSUI.Slider("Chance Moon".Translate(), 0, 20, 99, "chanceMoon", MoonChanceCallback, "% Chance of a rocky planet being a moon".Translate())));
            UI.Add("hzOverride", sOptions.Add(GSUI.Checkbox("Override Habitable Zone".Translate(), false, "hzOverride", HzOverrideCallback, "Enable the slider below".Translate())));
            UI.Add("hz", sOptions.Add(GSUI.RangeSlider("Habitable Zone".Translate(), 0, preferences.GetFloatFloat("hz", new FloatPair(0, 1)).low, preferences.GetFloatFloat("hz", new FloatPair(0, 3)).high, 100, 0.01f, "hz", null, HzLowCallback, HzHighCallback, "Force habitable zone between these distances".Translate())));
            UI.Add("orbitOverride", sOptions.Add(GSUI.Checkbox("Override Orbits".Translate(), false, "orbitOverride", OrbitOverrideCallback, "Enable the slider below".Translate())));
            UI.Add("orbits", sOptions.Add(GSUI.RangeSlider("Orbit Range".Translate(), 0.02f, 0.1f, 30, 99, 0.01f, "orbits", null, OrbitLowCallback, OrbitHighCallback, "Force the distances planets can spawn between".Translate())));
            AddSpacer(sOptions);
            return sOptions;
        }

        private GSOptions CreatePlanetOptions()
        {
            var sOptions = new GSOptions();
            AddSpacer(sOptions);
            UI.Add("rotationMulti", sOptions.Add(GSUI.Slider("Day/Night Multiplier".Translate(), 0.5f, 1, 10, 0.5f, "rotationMulti", null, "Increase the duration of night/day".Translate())));
            UI.Add("planetSize", sOptions.Add(GSUI.PlanetSizeRangeSlider("Telluric Planet Size".Translate(), 5, 50, 400, 510, "planetSize", null, PlanetSizeLow, PlanetSizeHigh, "Min/Max Size of Rocky Planets".Translate())));
            sOptions.Add(GSUI.Group("Limit Planet Sizes".Translate(), CreateLimitPlanetSizeOptions(), "Force Planets to these sizes".Translate()));
            UI.Add("sizeBias", sOptions.Add(GSUI.Slider("Planet Size Bias".Translate(), 0, 50, 100, "sizeBias", SizeBiasCallback, "Prefer Smaller (lower) or Larger (higher) Sizes".Translate())));
            UI.Add("inclination", sOptions.Add(GSUI.Slider("Max Inclination".Translate(), -1, -1, 180, 1f, "inclination", InclinationCallback, "Maximum angle of orbit".Translate(), "Random".Translate())));
            UI.Add("orbitLongitude", sOptions.Add(GSUI.Slider("Max Orbit Longitude".Translate(), -1, -1, 360, 1f, "orbitLongitude", LongitudeCallback, "Maximum longitude of the ascending node".Translate(), "Random".Translate())));
            UI.Add("rareChance", sOptions.Add(GSUI.Slider("Rare Vein Chance % Override".Translate(), -1, -1, 100, 1f, "rareChance", RareChanceCallback, "Override the chance of planets spawning rare veins".Translate(), "Default".Translate())));
            AddSpacer(sOptions);
            return sOptions;
        }

        private GSOptions CreateBirthOptions()
        {
            var bOptions = new GSOptions();
            var starTypes = new List<string>
            {
                "BlackHole",
                "WhiteDwarf",
                "NeutronStar",
                "O", "B", "A", "F", "G", "M", "K",
                "BlueGiant",
                "WhiteGiant",
                "YellowGiant",
                "RedGiant",
                "Random"
            };
            AddSpacer(bOptions);
            UI.Add("birthPlanetSize", bOptions.Add(GSUI.PlanetSizeSlider("Starting Planet Size".Translate(), 20, 200, 510, "birthPlanetSize", null, "How big the starting planet is. Default is 200".Translate())));
            UI.Add("birthPlanetUnlock", bOptions.Add(GSUI.Checkbox("Starting Planet Unlock".Translate(), false, "birthPlanetUnlock", null, "Allow other habitable themes for starting planet".Translate())));
            UI.Add("birthPlanetSiTi", bOptions.Add(GSUI.Checkbox("Starting planet Si/Ti".Translate(), false, "birthPlanetSiTi", null, "Force Silicon and Titanium on the starting planet".Translate())));
            UI.Add("birthPlanetStar", bOptions.Add(GSUI.Combobox("Starting Planet Star".Translate(), starTypes, 7, "birthStar", null, "Type of Star to Start at".Translate())));
            UI.Add("birthTidalLock", bOptions.Add(GSUI.Checkbox("Tidal Lock Starting Planet".Translate(), false, "birthTidalLock", null, "Force the starting planet to be tidally locked".Translate())));
            UI.Add("birthRareDisable", bOptions.Add(GSUI.Checkbox("Starting Planet No Rares".Translate(), true, "birthRareDisable", null, "Disable to allow rare veins on starting planet".Translate())));
            AddSpacer(bOptions);
            return bOptions;
        }

        private GSOptions CreateBinaryStarOptions()
        {
            var bOptions = new GSOptions();
            UI.Add("binaryChance", bOptions.Add(GSUI.Slider("Binary Star Chance %".Translate(), 0, 0, 100, 1f, "binaryChance", null, "% Chance of a star having a binary companion".Translate())));
            for (var i = 0; i < 14; i++)
            {
                if (i >= 8 && i <= 11) continue;
                UI.Add($"{typeLetter[i]}binaryEnabled", bOptions.Add(GSUI.Checkbox($"{typeDesc[i]}", i == 0, $"{typeLetter[i]}binaryEnabled", BinaryCallback, $"Allow {typeDesc[i]} to spawn as binary companions".Translate())));
            }

            AddSpacer(bOptions);
            return bOptions;
        }

        private GSOptions CreateGalaxyOptions()
        {
            var gOptions = new GSOptions();
            AddSpacer(gOptions);
            UI.Add("galaxyDensity", gOptions.Add(GSUI.Slider("Galaxy Spread".Translate(), 1, 5, 20, "galaxyDensity", null, "Lower = Stars are closer to each other. Default is 5".Translate())));
            UI.Add("defaultStarCount", gOptions.Add(GSUI.Slider("Default StarCount".Translate(), 1, 64, 1024, "defaultStarCount", DefaultStarCountCallback, "How many stars should the slider default to".Translate())));
            UI.Add("starSizeMulti", gOptions.Add(GSUI.Slider("Star Size Multiplier".Translate(), 0.5f, 5f, 20, 0.5f, "starSizeMulti", null, "GS2 uses 10x as standard. They just look cooler.".Translate())));
            UI.Add("luminosityBoost", gOptions.Add(GSUI.Slider("Luminosity Multiplier".Translate(), 0, 0, 10, .25f, "luminosityBoost", LuminosityBoostCallback, "Increase the luminosity of all stars by this multiplier".Translate(), "Default".Translate())));

            AddSpacer(gOptions);
            gOptions.Add(GSUI.Group("Binary Star Settings".Translate(), CreateBinaryStarOptions(), "Settings that control Binary Star formation".Translate()));
            AddSpacer(gOptions);
            gOptions.Add(GSUI.Group("Star Relative Frequencies".Translate(), CreateFreqOptions(), "How often to select a star type".Translate()));
            AddSpacer(gOptions);
            return gOptions;
        }

        private GSOptions CreateLimitPlanetSizeOptions()
        {
            var gOptions = new GSOptions();
            AddSpacer(gOptions);
            UI.Add("limitPlanetSize20", gOptions.Add(GSUI.Checkbox("20".Translate(), false, "limitPlanetSize20")));
            UI.Add("limitPlanetSize50", gOptions.Add(GSUI.Checkbox("50".Translate(), false, "limitPlanetSize50")));
            UI.Add("limitPlanetSize100", gOptions.Add(GSUI.Checkbox("100".Translate(), false, "limitPlanetSize100")));
            UI.Add("limitPlanetSize200", gOptions.Add(GSUI.Checkbox("200".Translate(), false, "limitPlanetSize200")));
            UI.Add("limitPlanetSize300", gOptions.Add(GSUI.Checkbox("300".Translate(), false, "limitPlanetSize300")));
            UI.Add("limitPlanetSize400", gOptions.Add(GSUI.Checkbox("400".Translate(), false, "limitPlanetSize400")));
            UI.Add("limitPlanetSize500", gOptions.Add(GSUI.Checkbox("500".Translate(), false, "limitPlanetSize500")));
            AddSpacer(gOptions);
            return gOptions;
        }

        private GSOptions CreateStarOverrideOptions()
        {
            var oOptions = new GSOptions();
            AddSpacer(oOptions);
            for (var i = 0; i < 14; i++)
            {
                var tOptions = new GSOptions();
                AddSpacer(tOptions);
                UI.Add($"{typeLetter[i]}planetCount", tOptions.Add(GSUI.RangeSlider("Planet Count".Translate(), 1, 2, 10, 99, 1f, $"{typeLetter[i]}planetCount", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}countBias", tOptions.Add(GSUI.Slider("Count Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}countBias", null, "Prefer Less (lower) or More (higher) Counts".Translate())));
                UI.Add($"{typeLetter[i]}planetSize", tOptions.Add(GSUI.RangeSlider("Telluric Planet Size".Translate(), 5, 50, 500, 510, 1f, $"{typeLetter[i]}planetSize", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}sizeBias", tOptions.Add(GSUI.Slider("Telluric Size Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}sizeBias", null, "Prefer Smaller (lower) or Larger (higher) Sizes".Translate())));
                UI.Add($"{typeLetter[i]}hzOverride", tOptions.Add(GSUI.Checkbox("Override Habitable Zone".Translate(), false, $"{typeLetter[i]}hzOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}hz", tOptions.Add(GSUI.RangeSlider("Habitable Zone".Translate(), 0, preferences.GetFloatFloat($"{typeLetter[i]}hz", new FloatPair(0, 1)).low, preferences.GetFloatFloat($"{typeLetter[i]}hz", new FloatPair(0, 3)).low, 100, 0.01f, $"{typeLetter[i]}hz", null, null, null, "Force habitable zone between these distances".Translate())));
                UI.Add($"{typeLetter[i]}chanceGas", tOptions.Add(GSUI.Slider("Chance for Gas giant".Translate(), 0, 20, 99, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon", tOptions.Add(GSUI.Slider("Chance for Moon".Translate(), 0, 20, 99, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}orbitOverride", tOptions.Add(GSUI.Checkbox("Override Orbits".Translate(), false, $"{typeLetter[i]}orbitOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}orbits", tOptions.Add(GSUI.RangeSlider("Orbit Range".Translate(), 0.02f, 0.1f, 30, 99, 0.01f, $"{typeLetter[i]}orbits", null, null, null, "Force the distances planets can spawn between".Translate())));
                UI.Add($"{typeLetter[i]}inclination", tOptions.Add(GSUI.Slider("Max Inclination".Translate(), -1, -1, 180, 1f, $"{typeLetter[i]}inclination", null, "Maximum angle of orbit".Translate(), "Random".Translate())));
                UI.Add($"{typeLetter[i]}orbitLongitude", tOptions.Add(GSUI.Slider("Max Orbit Longitude".Translate(), -1, -1, 360, 1f, $"{typeLetter[i]}orbitLongitude", null, "Maximum longitude of the ascending node".Translate(), "Random".Translate())));
                UI.Add($"{typeLetter[i]}rareChance", tOptions.Add(GSUI.Slider("Rare Vein Chance % Override".Translate(), -1, -1, 100, 1f, $"{typeLetter[i]}rareChance", null, "Override the chance of planets spawning rare veins".Translate(), "Default".Translate())));
                UI.Add($"{typeLetter[i]}luminosityBoost", tOptions.Add(GSUI.Slider("Luminosity Boost".Translate(), 0.25f, 1, 10, .25f, $"{typeLetter[i]}luminosityBoost", null, "Increase the luminosity of this star type by this amount".Translate(), "Default".Translate())));
                AddSpacer(tOptions);
                oOptions.Add(GSUI.Group($"{typeDesc[i]} Overrides".Translate(), tOptions, $"Change Settings for Type {typeDesc[i]} stars".Translate()));
            }

            AddSpacer(oOptions);
            return oOptions;
        }

        private void AddSpacer(GSOptions options = null)
        {
            var o = options ?? Options;
            o.Add(GSUI.Spacer());
        }

        private GSOptions CreateFreqOptions()
        {
            var freqOptions = new GSOptions();
            AddSpacer(freqOptions);
            UI.Add("freqK", freqOptions.Add(GSUI.Slider("Freq. Type K".Translate(), 0, 40, 100, "freqK")));
            UI.Add("KminStars", freqOptions.Add(GSUI.Slider("Minimum Type K".Translate(), 0, 0, 100, "KminStars")));
            UI.Add("freqM", freqOptions.Add(GSUI.Slider("Freq. Type M".Translate(), 0, 50, 100, "freqM")));
            UI.Add("MminStars", freqOptions.Add(GSUI.Slider("Minimum Type M".Translate(), 0, 0, 100, "MminStars")));
            UI.Add("freqG", freqOptions.Add(GSUI.Slider("Freq. Type G".Translate(), 0, 30, 100, "freqG")));
            UI.Add("GminStars", freqOptions.Add(GSUI.Slider("Minimum Type G".Translate(), 0, 0, 100, "GminStars")));
            UI.Add("freqF", freqOptions.Add(GSUI.Slider("Freq. Type F".Translate(), 0, 25, 100, "freqF")));
            UI.Add("FminStars", freqOptions.Add(GSUI.Slider("Minimum Type F".Translate(), 0, 0, 100, "FminStars")));
            UI.Add("freqA", freqOptions.Add(GSUI.Slider("Freq. Type A".Translate(), 0, 10, 100, "freqA")));
            UI.Add("AminStars", freqOptions.Add(GSUI.Slider("Minimum Type A".Translate(), 0, 0, 100, "AminStars")));
            UI.Add("freqB", freqOptions.Add(GSUI.Slider("Freq. Type B".Translate(), 0, 4, 100, "freqB")));
            UI.Add("BminStars", freqOptions.Add(GSUI.Slider("Minimum Type B".Translate(), 0, 0, 100, "BminStars")));
            UI.Add("freqO", freqOptions.Add(GSUI.Slider("Freq. Type O".Translate(), 0, 2, 100, "freqO")));
            UI.Add("OminStars", freqOptions.Add(GSUI.Slider("Minimum Type O".Translate(), 0, 0, 100, "OminStars")));
            UI.Add("freqBH", freqOptions.Add(GSUI.Slider("Freq. BlackHole".Translate(), 0, 1, 100, "freqBH")));
            UI.Add("BHminStars", freqOptions.Add(GSUI.Slider("Minimum BlackHole".Translate(), 0, 0, 100, "BHminStars")));
            UI.Add("freqNS", freqOptions.Add(GSUI.Slider("Freq. Neutron".Translate(), 0, 1, 100, "freqNS")));
            UI.Add("NSminStars", freqOptions.Add(GSUI.Slider("Minimum Neutron".Translate(), 0, 0, 100, "NSminStars")));
            UI.Add("freqWD", freqOptions.Add(GSUI.Slider("Freq. WhiteDwarf".Translate(), 0, 2, 100, "freqWD")));
            UI.Add("WDminStars", freqOptions.Add(GSUI.Slider("Minimum WhiteDwarf".Translate(), 0, 0, 100, "WDminStars")));
            UI.Add("freqRG", freqOptions.Add(GSUI.Slider("Freq. Red Giant".Translate(), 0, 1, 100, "freqRG")));
            UI.Add("RGminStars", freqOptions.Add(GSUI.Slider("Minimum Red Giant".Translate(), 0, 0, 100, "RGminStars")));
            UI.Add("freqYG", freqOptions.Add(GSUI.Slider("Freq. Yellow Giant".Translate(), 0, 1, 100, "freqYG")));
            UI.Add("YGminStars", freqOptions.Add(GSUI.Slider("Minimum Yellow Giant".Translate(), 0, 0, 100, "YGminStars")));
            UI.Add("freqWG", freqOptions.Add(GSUI.Slider("Freq. White Giant".Translate(), 0, 1, 100, "freqWG")));
            UI.Add("WGminStars", freqOptions.Add(GSUI.Slider("Minimum White Giant".Translate(), 0, 0, 100, "WGminStars")));
            UI.Add("freqBG", freqOptions.Add(GSUI.Slider("Freq. Blue Giant".Translate(), 0, 1, 100, "freqBG")));
            UI.Add("BGminStars", freqOptions.Add(GSUI.Slider("Minimum Blue Giant".Translate(), 0, 0, 100, "BGminStars")));
            AddSpacer(freqOptions);
            return freqOptions;
        }

        private void BinaryCallback(Val o)
        {
            for (var i = 0; i < 14; i++)
                if (preferences.GetBool($"{typeLetter[i]}binaryEnabled"))
                    return;
            UI["KbinaryEnabled"].Set(true);
        }

        private void LuminosityBoostCallback(Val o)
        {
            SetAllStarTypeOptions("luminosityBoost", o);
        }

        private void OrbitLowCallback(Val o)
        {
            SetAllStarTypeRangeSliderMin("orbits", o);
        }

        private void OrbitHighCallback(Val o)
        {
            SetAllStarTypeRangeSliderMax("orbits", o);
        }

        private void HzLowCallback(Val o)
        {
            SetAllStarTypeRangeSliderMin("hz", o);
        }

        private void HzHighCallback(Val o)
        {
            SetAllStarTypeRangeSliderMax("hz", o);
        }

        private void PlanetCountLow(Val o)
        {
            SetAllStarTypeRangeSliderMin("planetCount", o);
        }

        private void PlanetCountHigh(Val o)
        {
            SetAllStarTypeRangeSliderMax("planetCount", o);
        }

        private void PlanetSizeLow(Val o)
        {
            SetAllStarTypeRangeSliderMin("planetSize", Utils.ParsePlanetSize(o));
        }

        private void PlanetSizeHigh(Val o)
        {
            SetAllStarTypeRangeSliderMax("planetSize", Utils.ParsePlanetSize(o));
        }

        private void SizeBiasCallback(Val o)
        {
            SetAllStarTypeOptions("sizeBias", o);
        }

        private void HzOverrideCallback(Val o)
        {
            SetAllStarTypeOptions("hzOverride", o);
        }

        private void OrbitOverrideCallback(Val o)
        {
            SetAllStarTypeOptions("orbitOverride", o);
        }

        private void InclinationCallback(Val o)
        {
            SetAllStarTypeOptions("inclination", o);
        }

        private void LongitudeCallback(Val o)
        {
            SetAllStarTypeOptions("orbitLongitude", o);
        }

        private void RareChanceCallback(Val o)
        {
            SetAllStarTypeOptions("rareChance", o);
        }

        private void CountBiasCallback(Val o)
        {
            SetAllStarTypeOptions("countBias", o);
        }

        private void MoonChanceCallback(Val o)
        {
            SetAllStarTypeOptions("chanceMoon", o);
        }

        private void GasChanceCallback(Val o)
        {
            SetAllStarTypeOptions("chanceGas", o);
        }

        private void SetAllStarTypeOptions(string key, Val value)
        {
            for (var i = 0; i < 14; i++) UI[$"{typeLetter[i]}{key}"].Set(value);
        }

        private void SetAllStarTypeRangeSliderMin(string key, Val value)
        {
            for (var i = 0; i < 14; i++)
            {
                var high = preferences.GetFloatFloat($"{typeLetter[i]}{key}", new FloatPair(1, 10)).high;
                UI[$"{typeLetter[i]}{key}"].Set(new FloatPair(value, high));
                preferences.Set($"{typeLetter[i]}{key}", new FloatPair(value, high));
            }
        }

        private void SetAllStarTypeRangeSliderMax(string key, Val value)
        {
            for (var i = 0; i < 14; i++)
            {
                var low = preferences.GetFloatFloat($"{typeLetter[i]}{key}", new FloatPair(1, 10)).low;
                UI[$"{typeLetter[i]}{key}"].Set(new FloatPair(low, value));
                preferences.Set($"{typeLetter[i]}{key}", new FloatPair(low, value));
            }
        }

        private void CalculateFrequencies()
        {
            var starFreqTupleArray = new (string type, double chance)[14];
            var fK = preferences.GetDouble("freqK", 40);
            var fM = preferences.GetDouble("freqM", 50);
            var fG = preferences.GetDouble("freqG", 30);
            var fF = preferences.GetDouble("freqF", 25);
            var fA = preferences.GetDouble("freqA", 10);
            var fB = preferences.GetDouble("freqB", 4);
            var fO = preferences.GetDouble("freqO", 2);
            var fBh = preferences.GetDouble("freqBH", 1);
            var fN = preferences.GetDouble("freqNS", 1);
            var fW = preferences.GetDouble("freqWD", 2);
            var fRg = preferences.GetDouble("freqRG", 1);

            var fYg = preferences.GetDouble("freqYG", 1);
            var fWg = preferences.GetDouble("freqWG", 1);
            var fBg = preferences.GetDouble("freqBG", 1);
            var total = fK + fM + fG + fF + fA + fB + fO + fBh + fN + fW + fRg + fYg + fWg + fBg;

            starFreqTupleArray[0] = ("K", fK / total);
            starFreqTupleArray[1] = ("M", fM / total);
            starFreqTupleArray[2] = ("G", fG / total);
            starFreqTupleArray[3] = ("F", fF / total);
            starFreqTupleArray[4] = ("A", fA / total);
            starFreqTupleArray[5] = ("B", fB / total);
            starFreqTupleArray[6] = ("O", fO / total);
            starFreqTupleArray[7] = ("BH", fBh / total);
            starFreqTupleArray[8] = ("NS", fN / total);
            starFreqTupleArray[9] = ("WD", fW / total);
            starFreqTupleArray[10] = ("RG", fRg / total);
            starFreqTupleArray[11] = ("YG", fYg / total);
            starFreqTupleArray[12] = ("WG", fWg / total);
            starFreqTupleArray[13] = ("BG", fBg / total);

            starFreq = new Dictionary<string, double>
            {
                { "K", fK / total }
            };
            for (var i = 1; i < starFreqTupleArray.Length; i++)
            {
                var (type, chance) = starFreqTupleArray[i];
                var (_, prevChance) = starFreqTupleArray[i - 1];
                starFreq.Add(type, chance + prevChance);
                starFreqTupleArray[i].chance += prevChance;
            }
        }

        private void InitForcedStars()
        {
            _forcedStars = new List<string>();
            
            GS2.Warn(preferences.GetInt("birthStar", 14).ToString());
            for (var i = 0; i < 14; i++)
            {
                var count = preferences.GetInt($"{typeLetter[i]}minStars", 0);
                for (var j = 0; j < count; j++) _forcedStars.Add(typeLetter[i]);
            }
            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt >= 14) return;
            if (!_forcedStars.Contains(typeLetter[bsInt])) _forcedStars.Add(typeLetter[bsInt]);
        }

        private (EStarType type, ESpectrType spectr) ChooseStarType(bool birth = false)
        {
            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt < 14 && birth) return ((EStar)bsInt).Convert();
            var starType = "";
            if (_forcedStars.Count > 0)
            {
                var choice = random.ItemWithIndex(_forcedStars);
                starType = choice.Item2;
                _forcedStars.RemoveAt(choice.Item1);
            }
            else
            {
                var choice = random.NextDouble();

                for (var i = 0; i < starFreq.Count; i++)
                    if (choice < starFreq.ElementAt(i).Value)
                    {
                        starType = starFreq.ElementAt(i).Key;
                        //GS2.Warn($"Picked Startype {starType} with choice {choice} and value {starFreq.ElementAt(i).Value}");
                        break;
                    }
            }

            return GetStarTypeSpectrFromLetter(starType);
        }

        private static (EStarType type, ESpectrType spectr) GetStarTypeSpectrFromLetter(string starType)
        {
            switch (starType)
            {
                case "K": return (EStarType.MainSeqStar, ESpectrType.K);
                case "M": return (EStarType.MainSeqStar, ESpectrType.M);
                case "G": return (EStarType.MainSeqStar, ESpectrType.G);
                case "F": return (EStarType.MainSeqStar, ESpectrType.F);
                case "A": return (EStarType.MainSeqStar, ESpectrType.A);
                case "B": return (EStarType.MainSeqStar, ESpectrType.B);
                case "O": return (EStarType.MainSeqStar, ESpectrType.O);
                case "BH": return (EStarType.BlackHole, ESpectrType.X);
                case "NS": return (EStarType.NeutronStar, ESpectrType.X);
                case "WD": return (EStarType.WhiteDwarf, ESpectrType.X);
                case "RG": return (EStarType.GiantStar, ESpectrType.M);
                case "YG": return (EStarType.GiantStar, ESpectrType.G);
                case "WG": return (EStarType.GiantStar, ESpectrType.A);
                default: return (EStarType.GiantStar, ESpectrType.B);
            }
        }

        private int GetMaxPlanetCountForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetCount", new FloatPair(1, 10));
            return (int)t.high;
        }

        private int GetMinPlanetCountForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetCount", new FloatPair(1, 10));
            return (int)t.low;
        }

        private int GetMaxPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetSize", new FloatPair(30, 500));
            return (int)t.high;
        }

        private float GetLuminosityBoostForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloat($"{sl}luminosityBoost");
            return t;
        }

        private int GetMinPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetSize", new FloatPair(30, 500));
            return (int)t.low;
        }

        private int GetSizeBiasForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}sizeBias", 50);
        }

        private int GetCountBiasForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}countBias", 50);
        }

        private double GetMoonChanceForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}chanceMoon", 20) / 100.0;
        }

        private double GetGasChanceForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}chanceGas", 20) / 100.0;
        }
    }
}