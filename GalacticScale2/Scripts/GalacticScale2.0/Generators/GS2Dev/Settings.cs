using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GalacticScale.GS2;
namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private GSGenPreferences preferences = new GSGenPreferences();
        private Dictionary<string, double> starFreq = new Dictionary<string, double>();

        private readonly Dictionary<string, GSOptionCallback> typeCallbacks = new Dictionary<string, GSOptionCallback>();

        private readonly string[] typeDesc =
        {
            "Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O", "White Dwarf", "Red Giant",
            "Yellow Giant", "White Giant",
            "Blue Giant", "Neutron Star", "Black Hole"
        };

        private readonly string[] typeLetter =
            {"K", "M", "F", "G", "A", "B", "O", "WD", "RG", "YG", "WG", "BG", "NS", "BH"};

        public Dictionary<string, GSUI> UI = new Dictionary<string, GSUI>();
        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config { get; } = new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();

        public void Init()
        {
            Config.enableStarSelector = true;
            AddUIElements();
            InitPreferences();
        }

        private static bool loaded = false;
        public void Import(GSGenPreferences preferences)
        {
            
            // this.preferences = preferences;
            for (var i = 0; i < preferences.Count; i++)
            {
                var key = preferences.Keys.ElementAt(i);
                //GS2.Log($"pref set {key} {preferences[key]}");
                this.preferences.Set(key, preferences[key]);

                if (loaded && UI.ContainsKey(key))
                {
                    // GS2.Warn($"Setting {key} {this.preferences[key]}");
                    UI[key].Set(preferences[key]);
                }
            }

            if (loaded) loaded = false;
            Config.DefaultStarCount = preferences.GetInt("defaultStarCount");
            if (!preferences.GetBool("ludicrousMode")) Config.MaxStarCount = 1024;
        }

        public GSGenPreferences Export()
        {
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
            }

            GS2.Error($"Failed to get star type letter from star of type {star.Type} spectr {star.Spectr}");
            return "WD";
        }

        private string GetMainSeqStarTypeLetter(ESpectrType spectr)
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

            GS2.Error($"Failed to get star type letter from main sequence star of spectr {spectr}");
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

            GS2.Error($"Failed to get star type letter from giant star of spectr {spectr}");
            return "WD";
        }

      

       

        // private void LockUI(string key, Val value)
        // {
        //     GS2.Warn($"Start {key}");
        //     if (UI.ContainsKey(key)) UI[key].Set(value);
        //     GS2.Warn("#");
        //     if (UI.ContainsKey(key)) UI[key].Disable();
        //     GS2.Warn("##");
        // }

        // private void UnlockUI(string key)
        // {
        //     UI[key].Enable();
        // }

        // public void DisableSafeMode()
        // {
        //     Log("Disabling SafeMode");
        //     // UI["ludicrousMode"].Enable();
        //     // UI["minPlanetSize"].Set(new GSSliderConfig(30, 50, 200));
        //     // UI["maxPlanetSize"].Set(new GSSliderConfig(50, 500, 500));
        //     // UnlockUI("birthPlanetSize");
        //     // UnlockUI("birthPlanetSiTi");
        //     // UnlockUI("birthPlanetUnlock");
        //     // UnlockUI("hugeGasGiants");
        //     // UnlockUI("galaxyDensity");
        //     // UI["defaultStarCount"].Set(new GSSliderConfig(1, 64, 1024));
        //     // UnlockUI("moonsAreSmall");
        //     // UnlockUI("secondarySatellites");
        //     // UI["minPlanetCount"].Set(new GSSliderConfig(0, 1, 25));
        //     // UI["maxPlanetCount"].Set(new GSSliderConfig(1, 10, 25));
        //     // for (var i = 0; i < 14; i++)
        //     // {
        //     //     UnlockUI($"{typeLetter[i]}minPlanetCount");
        //     //     UnlockUI($"{typeLetter[i]}maxPlanetCount");
        //     //     UnlockUI($"{typeLetter[i]}minPlanetSize");
        //     //     UnlockUI($"{typeLetter[i]}maxPlanetSize");
        //     //     UnlockUI($"{typeLetter[i]}sizeBias");
        //     //     UnlockUI($"{typeLetter[i]}chanceGas");
        //     //     UnlockUI($"{typeLetter[i]}chanceMoon");
        //     //     UnlockUI($"{typeLetter[i]}systemDensity");
        //     // }
        // }

        private void DefaultStarCountCallback(Val o)
        {
            Config.DefaultStarCount = preferences.GetInt("defaultStarCount", 64);
        }

        public void Reset(Val o)
        {
            preferences = new GSGenPreferences();
            InitPreferences();
            foreach (var ui in UI)
            {
                GS2.Log($"Resetting {ui.Key}");
                if (GSUI.Settables.Contains(ui.Value.Type)) ui.Value.Set(preferences.Get(ui.Key));
            }
        }

        public Dictionary<string, (float, float)> hzDefs = new Dictionary<string, (float, float)>();
        public void getHZ()
        {
            hzDefs = new Dictionary<string, (float, float)>();
            hzDefs.Add("K", Utils.CalculateHabitableZone(1.2f));
            hzDefs.Add("M", Utils.CalculateHabitableZone(0.6f));
            hzDefs.Add("A", Utils.CalculateHabitableZone(2.1f));
            hzDefs.Add("B", Utils.CalculateHabitableZone(5f));
            hzDefs.Add("F", Utils.CalculateHabitableZone(1.3f));
            hzDefs.Add("G", Utils.CalculateHabitableZone(1f));
            hzDefs.Add("O", Utils.CalculateHabitableZone(12f));
            hzDefs.Add("RG", Utils.CalculateHabitableZone(.85f));
            hzDefs.Add("YG", Utils.CalculateHabitableZone(1f));
            hzDefs.Add("WG", Utils.CalculateHabitableZone(6f));
            hzDefs.Add("BG", Utils.CalculateHabitableZone(10f));
            hzDefs.Add("WD", Utils.CalculateHabitableZone(0.05f));
            hzDefs.Add("NS", Utils.CalculateHabitableZone(0.35f));
            hzDefs.Add("BH", Utils.CalculateHabitableZone(0.006f));
        }
        private void InitPreferences()
        {
            GS2.Log("InitPreferences");
            getHZ();
            foreach (var hz in hzDefs)
            {
                preferences.Set($"{hz.Key}hz", hz.Value);
            }
            preferences.Set("birthPlanetStar", 14);
            preferences.Set("rotationMulti", 1f);
            preferences.Set("innerPlanetDistance", 1f);
            // preferences.Set("safeMode", false);
            // preferences.Set("ludicrousMode", false);
            preferences.Set("allowResonances", true);
            preferences.Set("galaxyDensity", 5);
            preferences.Set("defaultStarCount", 64);
            preferences.Set("starSizeMulti", 10);
            preferences.Set("birthPlanetSize", 200);
            preferences.Set("birthPlanetUnlock", false);
            preferences.Set("birthPlanetSiTi", false);
            preferences.Set("moonsAreSmall", true);
            preferences.Set("moonBias", 50);
            preferences.Set("hugeGasGiants", true);
            preferences.Set("tidalLockInnerPlanets", false);
            preferences.Set("secondarySatellites", false);
            preferences.Set("moonBias", 50);
            // preferences.Set("minPlanetCount", 1);
            // preferences.Set("maxPlanetCount", 10);
            // preferences.Set("minPlanetSize", 30);
            // preferences.Set("maxPlanetSize", 500);
            preferences.Set($"planetCount", (1,10));
            preferences.Set($"planetSize", (50,500));
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
            preferences.Set("hz", (0.9f, 2));
            preferences.Set("orbits", (0.1f, 10));
            preferences.Set("inclination", -1);
            preferences.Set("orbitLongitude", 0);
            // preferences.Set("systemDensity", 3);
            for (var i = 0; i < 14; i++)
            {
                preferences.Set($"{typeLetter[i]}minStars", 0);
                preferences.Set($"{typeLetter[i]}planetCount", (1,10));
                preferences.Set($"{typeLetter[i]}planetSize", (50,500));
                // preferences.Set($"{typeLetter[i]}minPlanetCount", 1);
                // preferences.Set($"{typeLetter[i]}maxPlanetCount", 10);
                // preferences.Set($"{typeLetter[i]}maxPlanetSize", 500);
                // preferences.Set($"{typeLetter[i]}minPlanetSize", 50);
                preferences.Set($"{typeLetter[i]}sizeBias", 50);
                preferences.Set($"{typeLetter[i]}countBias", 50);
                preferences.Set($"{typeLetter[i]}chanceGas", 20);
                preferences.Set($"{typeLetter[i]}chanceMoon", 20);
                // preferences.Set($"{typeLetter[i]}systemDensity", 3);
                preferences.Set($"{typeLetter[i]}orbits", (0.9f, 2));
                preferences.Set($"{typeLetter[i]}orbits", (0.1f,10));
                preferences.Set($"{typeLetter[i]}orbitOverride", false);
                preferences.Set($"{typeLetter[i]}inclination", -1);
                preferences.Set($"{typeLetter[i]}orbitLongitude", 0);
                preferences.Set($"{typeLetter[i]}hzOverride", false);
            }
        }

        private void AddUIElements()
        {
            var starTypes = new List<string>()
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

            // Val l = false;//preferences.GetBool("ludicrousMode", false);
            // GS2.Warn(l);
            // UI.Add("safeMode", Options.Add(GSUI.Checkbox("Safe Mode".Translate(), false, "safeMode", o =>
            // {
            //     if ((bool) o) EnableSafeMode();
            //     else DisableSafeMode();
            // }, "Disabled for now")));
            //
            //
            // UI.Add("ludicrousMode", Options.Add(GSUI.Checkbox("Ludicrous Mode".Translate(), false, "ludicrousMode", o =>
            // {
            //     if ((bool) o) EnableLudicrousMode();
            //     else DisableLudicrousMode();
            // }, "Disabled for now")));

            
            UI.Add("galaxyDensity", Options.Add(GSUI.Slider("Galaxy Spread".Translate(), 1, 5, 9, "galaxyDensity", null, "Lower = Stars are closer to each other".Translate())));
            UI.Add("defaultStarCount",
                Options.Add(GSUI.Slider("Default StarCount".Translate(), 1, 64, 1024, "defaultStarCount",
                    DefaultStarCountCallback, "How many stars should the slider default to".Translate())));
            UI.Add("starSizeMulti",
                Options.Add(GSUI.Slider("Star Size Multiplier".Translate(), 1f, 10f, 20f, 0.1f, "starSizeMulti", null, "GS2 uses 10x as standard. They just look cooler.".Translate())));
            var bOptions = new GSOptions();
            UI.Add("birthPlanetSize",
                bOptions.Add(GSUI.PlanetSizeSlider("Starting Planet Size".Translate(), 20, 200, 510, "birthPlanetSize")));
            UI.Add("birthPlanetUnlock",
                bOptions.Add(GSUI.Checkbox("Starting Planet Unlock".Translate(), false, "birthPlanetUnlock", null, "Allow other habitable themes for birth planet".Translate())));
            UI.Add("birthPlanetSiTi", bOptions.Add(GSUI.Checkbox("Starting planet Si/Ti".Translate(), false, "birthPlanetSiTi", null, "Force Silicon and Titanius on the birth planet".Translate())));
            UI.Add("birthPlanetStar", bOptions.Add(GSUI.Combobox("BirthPlanet Star".Translate(), starTypes, 7, "birthStar",null, "Type of Star to Start at".Translate())));
            Options.Add(GSUI.Group("Birth Planet Settings".Translate(), bOptions, "Settings that only affect the starting planet".Translate()));
            UI.Add("moonsAreSmall", Options.Add(GSUI.Checkbox("Moons Are Small".Translate(), true, "moonsAreSmall", null, "Try to ensure moons are 1/2 their planets size or less".Translate())));
            UI.Add("moonBias", Options.Add(GSUI.Slider("Gas Giants Moon Bias".Translate(), 0, 50, 100, "moonBias", null, "Lower prefers telluric planets, higher gas giants".Translate())));
            UI.Add("hugeGasGiants", Options.Add(GSUI.Checkbox("Huge Gas Giants".Translate(), true, "hugeGasGiants", null,"Allow gas giants larger than 800 radius".Translate())));
            UI.Add("tidalLockInnerPlanets",
                Options.Add(GSUI.Checkbox("Tidal Lock Inner Planets".Translate(), false, "tidalLockInnerPlanets")));
            UI.Add("innerPlanetDistance", Options.Add(GSUI.Slider("Inner Planet Distance (AU)".Translate(), 0, 1, 100, 0.1f, "innerPlanetDistance", null, "Distance forced tidal locking stops acting".Translate())));
            UI.Add("allowResonances", Options.Add(GSUI.Checkbox("Allow Orbital Harmonics".Translate(), true, "allowResonances", null, "Allow Orbital Resonance 1:2 and 1:4".Translate())));
            UI.Add("rotationMulti", Options.Add(GSUI.Slider("Day/Night Multiplier".Translate(), 0.5f, 1, 100, 0.5f, "rotationMulti", null, "Increase the duration of night/day".Translate())));
            UI.Add("secondarySatellites", Options.Add(GSUI.Checkbox("Secondary satellites".Translate(), false, "secondarySatellites", null, "Allow moons to have moons".Translate())));


            UI.Add("chanceMoonMoon", Options.Add(GSUI.Slider("Secondary Satellite Chance".Translate(), 0, 5, 99, "chanceMoonMoon", null, "% Chance for a moon to have a moon".Translate())));

            var FreqOptions = new GSOptions();
            UI.Add("freqK", FreqOptions.Add(GSUI.Slider("Freq. Type K".Translate(), 0, 40, 100, "freqK")));
            UI.Add("KminStars", FreqOptions.Add(GSUI.Slider("Minimum Type K".Translate(), 0, 0, 100, "KminStars")));
            UI.Add("freqM", FreqOptions.Add(GSUI.Slider("Freq. Type M".Translate(), 0, 50, 100, "freqM")));
            UI.Add("MminStars", FreqOptions.Add(GSUI.Slider("Minimum Type M".Translate(), 0, 0, 100, "MminStars")));
            UI.Add("freqG", FreqOptions.Add(GSUI.Slider("Freq. Type G".Translate(), 0, 30, 100, "freqG")));
            UI.Add("GminStars", FreqOptions.Add(GSUI.Slider("Minimum Type G".Translate(), 0, 0, 100, "GminStars")));
            UI.Add("freqF", FreqOptions.Add(GSUI.Slider("Freq. Type F".Translate(), 0, 25, 100, "freqF")));
            UI.Add("FminStars", FreqOptions.Add(GSUI.Slider("Minimum Type F".Translate(), 0, 0, 100, "FminStars")));
            UI.Add("freqA", FreqOptions.Add(GSUI.Slider("Freq. Type A".Translate(), 0, 10, 100, "freqA")));
            UI.Add("AminStars", FreqOptions.Add(GSUI.Slider("Minimum Type A".Translate(), 0, 0, 100, "AminStars")));
            UI.Add("freqB", FreqOptions.Add(GSUI.Slider("Freq. Type B".Translate(), 0, 4, 100, "freqB")));
            UI.Add("BminStars", FreqOptions.Add(GSUI.Slider("Minimum Type B".Translate(), 0, 0, 100, "BminStars")));
            UI.Add("freqO", FreqOptions.Add(GSUI.Slider("Freq. Type O".Translate(), 0, 2, 100, "freqO")));
            UI.Add("OminStars", FreqOptions.Add(GSUI.Slider("Minimum Type O".Translate(), 0, 0, 100, "OminStars")));
            UI.Add("freqBH", FreqOptions.Add(GSUI.Slider("Freq. BlackHole".Translate(), 0, 1, 100, "freqBH")));
            UI.Add("BHminStars", FreqOptions.Add(GSUI.Slider("Minimum BlackHole".Translate(), 0, 0, 100, "BHminStars")));
            UI.Add("freqNS", FreqOptions.Add(GSUI.Slider("Freq. Neutron".Translate(), 0, 1, 100, "freqNS")));
            UI.Add("NSminStars", FreqOptions.Add(GSUI.Slider("Minimum Neutron".Translate(), 0, 0, 100, "NSminStars")));
            UI.Add("freqWD", FreqOptions.Add(GSUI.Slider("Freq. WhiteDwarf".Translate(), 0, 2, 100, "freqWD")));
            UI.Add("WDminStars", FreqOptions.Add(GSUI.Slider("Minimum WhiteDwarf".Translate(), 0, 0, 100, "WDminStars")));
            UI.Add("freqRG", FreqOptions.Add(GSUI.Slider("Freq. Red Giant".Translate(), 0, 1, 100, "freqRG")));
            UI.Add("RGminStars", FreqOptions.Add(GSUI.Slider("Minimum Red Giant".Translate(), 0, 0, 100, "RGminStars")));
            UI.Add("freqYG", FreqOptions.Add(GSUI.Slider("Freq. Yellow Giant".Translate(), 0, 1, 100, "freqYG")));
            UI.Add("YGminStars", FreqOptions.Add(GSUI.Slider("Minimum Yellow Giant".Translate(), 0,0, 100, "YGminStars")));
            UI.Add("freqWG", FreqOptions.Add(GSUI.Slider("Freq. White Giant".Translate(), 0, 1, 100, "freqWG")));
            UI.Add("WGminStars", FreqOptions.Add(GSUI.Slider("Minimum White Giant".Translate(), 0, 0, 100, "WGminStars")));
            UI.Add("freqBG", FreqOptions.Add(GSUI.Slider("Freq. Blue Giant".Translate(), 0, 1, 100, "freqBG")));
            UI.Add("BGminStars", FreqOptions.Add(GSUI.Slider("Minimum Blue Giant".Translate(), 0, 0, 100, "BGminStars")));
            Options.Add(GSUI.Group("Star Relative Frequencies".Translate(), FreqOptions, "How often to select a star type".Translate()));
            Options.Add(GSUI.Spacer());
            Options.Add(GSUI.Header("Default Settings".Translate(), "Changing these will reset all star specific options below".Translate()));
            UI.Add("planetCount", Options.Add(GSUI.RangeSlider("Planet Count".Translate(), 1, 2, 10, 99, 1f, "planetCount", null, planetCountLow, planetCountHigh, "The amount of planets per star".Translate())));
            UI.Add("countBias", Options.Add(GSUI.Slider("Planet Count Bias".Translate(), 0, 50, 100, "sizeBias", CountBiasCallback)));
            UI.Add("planetSize",Options.Add(GSUI.PlanetSizeRangeSlider("Telluric Planet Size".Translate(), 5, 50, 400, 510, "planetSize", null, planetSizeLow, planetSizeHigh, "Min/Max Size of Rocky Planets".Translate())));
            UI.Add("sizeBias", Options.Add(GSUI.Slider("Planet Size Bias".Translate(), 0, 50, 100, "sizeBias", SizeBiasCallback)));
            UI.Add("chanceGas", Options.Add(GSUI.Slider("Chance Gas".Translate(), 0, 20, 99, "chanceGas", GasChanceCallback)));
            UI.Add("chanceMoon", Options.Add(GSUI.Slider("Chance Moon".Translate(), 0, 20, 99, "chanceMoon", MoonChanceCallback)));
            UI.Add($"hzOverride", Options.Add(GSUI.Checkbox("Override Habitable Zone".Translate(), false, $"hzOverride", hzOverrideCallback, "Enable the slider below".Translate())));
            UI.Add($"hz", Options.Add(GSUI.RangeSlider("Habitable Zone".Translate(), 0, preferences.GetFloatFloat($"hz", (0, 1)).Item1, preferences.GetFloatFloat($"hz", (0, 3)).Item2, 100, 0.01f, $"hz", null, hzLowCallback, hzHighCallback, "Force habitable zone between these distances".Translate())));

            UI.Add($"orbitOverride", Options.Add(GSUI.Checkbox("Override Orbits".Translate(), false, $"orbitOverride", orbitOverrideCallback, "Enable the slider below".Translate())));
            UI.Add($"orbits", Options.Add(GSUI.RangeSlider($"Orbit Range".Translate(), 0.02f, 0.1f, 30, 99, 0.01f, $"orbits", null, orbitLowCallback, orbitHighCallback, "Force the distances planets can spawn between".Translate())));
            UI.Add($"inclination", Options.Add(GSUI.Slider("Max Inclination".Translate(), -1, -1, 180, 1f, $"inclination", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/inclinationCallback, "Maximum angle of orbit".Translate(), "Random".Translate())));
            UI.Add($"orbitLongitude", Options.Add(GSUI.Slider("Max Orbit Longitude".Translate(), -1, -1, 360, 1f, $"orbitLongitude", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/longitudeCallback, "Maximum longitude of the ascending node".Translate(), "Random".Translate())));
            UI.Add($"rareChance", Options.Add(GSUI.Slider("Rare Vein Chance % Override".Translate(), -1, -1, 100, 1f, $"rareChance", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/rareChanceCallback, "Override the chance of planets spawning rare veins".Translate(), "Default".Translate())));

            // UI.Add("systemDensity", Options.Add(GSUI.Slider("System Density".Translate(), 1, 3, 5, "systemDensity", SystemDensityCallback)));
            Options.Add(GSUI.Separator());
            for (var i = 0; i < 14; i++)
            {
                //CreateTypeInclinationCallBack($"{typeLetter[i]}inclination");
                var tOptions = new GSOptions();
                UI.Add($"{typeLetter[i]}planetCount", tOptions.Add(GSUI.RangeSlider($"Planet Count".Translate(), 1, 2, 10, 99, 1f, $"{typeLetter[i]}planetCount", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}countBias", tOptions.Add(GSUI.Slider($"Count Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}countBias", null, "Prefer Less (lower) or More (higher) Counts".Translate())));
                UI.Add($"{typeLetter[i]}planetSize", tOptions.Add(GSUI.RangeSlider($"Telluric Planet Size".Translate(), 5, 50, 500, 510, 1f, $"{typeLetter[i]}planetSize", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}sizeBias", tOptions.Add(GSUI.Slider($"Telluric Size Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}sizeBias", null, "Prefer Smaller (lower) or Larger (higher) Sizes".Translate())));
                UI.Add($"{typeLetter[i]}hzOverride", tOptions.Add(GSUI.Checkbox("Override Habitable Zone".Translate(), false, $"{typeLetter[i]}hzOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}hz", tOptions.Add(GSUI.RangeSlider("Habitable Zone".Translate(), 0, preferences.GetFloatFloat($"{typeLetter[i]}hz", (0,1)).Item1,preferences.GetFloatFloat($"{typeLetter[i]}hz", (0,3)).Item2, 100, 0.01f, $"{typeLetter[i]}hz", null, null, null, "Force habitable zone between these distances".Translate())));
                UI.Add($"{typeLetter[i]}chanceGas", tOptions.Add(GSUI.Slider($"Chance for Gas giant".Translate(), 0, 20, 99, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon", tOptions.Add(GSUI.Slider($"Chance for Moon".Translate(), 0, 20, 99, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}orbitOverride", tOptions.Add(GSUI.Checkbox("Override Orbits".Translate(), false, $"{typeLetter[i]}orbitOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}orbits", tOptions.Add(GSUI.RangeSlider($"Orbit Range".Translate(), 0.02f, 0.1f, 30, 99, 0.01f, $"{typeLetter[i]}orbits", null, null, null, "Force the distances planets can spawn between".Translate())));
                UI.Add($"{typeLetter[i]}inclination", tOptions.Add(GSUI.Slider("Max Inclination".Translate(), -1, -1, 180, 1f, $"{typeLetter[i]}inclination", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/null, "Maximum angle of orbit".Translate(), "Random".Translate())));
                UI.Add($"{typeLetter[i]}orbitLongitude", tOptions.Add(GSUI.Slider("Max Orbit Longitude".Translate(), -1, -1, 360, 1f, $"{typeLetter[i]}orbitLongitude", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/null, "Maximum longitude of the ascending node".Translate(), "Random".Translate())));
                UI.Add($"{typeLetter[i]}rareChance", tOptions.Add(GSUI.Slider("Rare Vein Chance % Override".Translate(), -1, -1, 100, 1f, $"{typeLetter[i]}rareChance", /*typeCallbacks[$"{typeLetter[i]}inclination"]*/null, "Override the chance of planets spawning rare veins".Translate(), "Default".Translate())));
                //UI[$"{typeLetter[i]}inclination"].Set(new GSSliderConfig(-1, -1, 90, true));

                // UI.Add($"{typeLetter[i]}systemDensity", tOptions.Add(GSUI.Slider($"{typeDesc[i]} Density".Translate(), 1, 3, 5, $"{typeLetter[i]}systemDensity", null, "Lower is less dense".Translate())));
                Options.Add(GSUI.Group($"{typeDesc[i]} Overrides".Translate(), tOptions, $"Change Settings for Type {typeDesc[i]} stars".Translate()));
                Options.Add(GSUI.Separator());
            }
            Options.Add(GSUI.Button("Reset".Translate(), "Now".Translate(), Reset));
            loaded = true;
        }
        private void orbitLowCallback(Val o)
        {
            // Warn(o);

            SetAllStarTypeRSMin("orbits", o);
        }
        private void orbitHighCallback(Val o)
        {
            // GS2.Warn(o);
            SetAllStarTypeRSMax("orbits", o);
        }
        private void hzLowCallback(Val o)
        {
            // Warn(o);

            SetAllStarTypeRSMin("hz", o);
        }
        private void hzHighCallback(Val o)
        {
            // GS2.Warn(o);
            SetAllStarTypeRSMax("hz", o);
        }
        private void planetCountLow(Val o)
        {
            // Warn(o);

            SetAllStarTypeRSMin("planetCount", o);
        }
        private void planetCountHigh(Val o)
        {
            // GS2.Warn(o);
            SetAllStarTypeRSMax("planetCount", o);
        }
        private void planetSizeLow(Val o)
        {
            // Warn(o);
            
            SetAllStarTypeRSMin("planetSize", Utils.ParsePlanetSize(o));
        }    
        private void planetSizeHigh(Val o)
        {
            // GS2.Warn(o);
            SetAllStarTypeRSMax("planetSize", Utils.ParsePlanetSize(o));
        }

        private void SizeBiasCallback(Val o)
        {
            SetAllStarTypeOptions("sizeBias", o);
        }
        private void hzOverrideCallback(Val o)
        {
            SetAllStarTypeOptions("hzOverride", o);
        }
        private void orbitOverrideCallback(Val o)
        {
            SetAllStarTypeOptions("orbitOverride", o);
        }
        private void inclinationCallback(Val o)
        {
            SetAllStarTypeOptions("inclination", o);
        }
        private void longitudeCallback(Val o)
        {
            SetAllStarTypeOptions("orbitLongitude", o);
        }
        private void rareChanceCallback(Val o)
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
            // GS2.Warn("Setting Gas Chance"+ o.String());
            SetAllStarTypeOptions("chanceGas", o);
        }

        private void SystemDensityCallback(Val o)
        {
            SetAllStarTypeOptions("systemDensity", o);
        }

        private void SetAllStarTypeOptions(string key, Val value)
        {
            for (var i = 0; i < 14; i++) UI[$"{typeLetter[i]}{key}"].Set(value);
        }
        private void SetAllStarTypeRSMin(string key, Val value)
        {
            for (var i = 0; i < 14; i++)
            {
                var high = preferences.GetFloatFloat($"{typeLetter[i]}{key}", (1, 10)).Item2;//UI[$"{typeLetter[i]}{key}"].RectTransform.GetComponent<GSUIRangeSlider>().HighValue;
                UI[$"{typeLetter[i]}{key}"].Set((value, high));
                preferences.Set($"{typeLetter[i]}{key}", (value, high));
            }
        }
        private void SetAllStarTypeRSMax(string key, Val value)
        {
            for (var i = 0; i < 14; i++)
            {
                var low = preferences.GetFloatFloat($"{typeLetter[i]}{key}", (1, 10)).Item1;//UI[$"{typeLetter[i]}{key}"].RectTransform.GetComponent<GSUIRangeSlider>().LowValue;
                UI[$"{typeLetter[i]}{key}"].Set((low,value));
                preferences.Set($"{typeLetter[i]}{key}", (low, value));
            }
        }

        private void CalculateFrequencies()
        {
            var StarFreqTupleArray = new (string type, double chance)[14];
            var fK = preferences.GetDouble("freqK", 40);
            var fM = preferences.GetDouble("freqM", 50);
            var fG = preferences.GetDouble("freqG", 30);
            var fF = preferences.GetDouble("freqF", 25);
            var fA = preferences.GetDouble("freqA", 10);
            var fB = preferences.GetDouble("freqB", 4);
            var fO = preferences.GetDouble("freqO", 2);
            var fBH = preferences.GetDouble("freqBH", 1);
            var fN = preferences.GetDouble("freqNS", 1);
            var fW = preferences.GetDouble("freqWD", 2);
            var fRG = preferences.GetDouble("freqRG", 1);
            var fYG = preferences.GetDouble("freqYG", 1);
            var fWG = preferences.GetDouble("freqWG", 1);
            var fBG = preferences.GetDouble("freqBG", 1);
            var total = fK + fM + fG + fF + fA + fB + fO + fBH + fN + fW + fRG + fYG + fWG + fBG;
            //GS2.Warn($"{total} = {fK} + {fM} + {fG} + {fF} + {fA} + {fB} + {fO} + {fBH} + {fN} + {fW} + {fRG} + {fYG} + {fWG} + {fBG}");

            StarFreqTupleArray[0] = ("K", fK / total);
            StarFreqTupleArray[1] = ("M", fM / total);
            StarFreqTupleArray[2] = ("G", fG / total);
            StarFreqTupleArray[3] = ("F", fF / total);
            StarFreqTupleArray[4] = ("A", fA / total);
            StarFreqTupleArray[5] = ("B", fB / total);
            StarFreqTupleArray[6] = ("O", fO / total);
            StarFreqTupleArray[7] = ("BH", fBH / total);
            StarFreqTupleArray[8] = ("NS", fN / total);
            StarFreqTupleArray[9] = ("WD", fW / total);
            StarFreqTupleArray[10] = ("RG", fRG / total);
            StarFreqTupleArray[11] = ("YG", fYG / total);
            StarFreqTupleArray[12] = ("WG", fWG / total);
            StarFreqTupleArray[13] = ("BG", fBG / total);
            //string[] keys = StarFreq.Keys.ToArray();
            //GS2.LogJson(StarFreqTupleArray, true);
            starFreq = new Dictionary<string, double>();
            starFreq.Add("K", fK / total);
            for (var i = 1; i < StarFreqTupleArray.Length; i++)
            {
                var element = StarFreqTupleArray[i];
                var previousElement = StarFreqTupleArray[i - 1];
                starFreq.Add(element.type, element.chance + previousElement.chance);
                StarFreqTupleArray[i].chance += previousElement.chance;
            }
        }
        private List<string> ForcedStars = new List<string>();
        private void InitForcedStars()
        {
            ForcedStars = new List<string>();
            

            for (var i = 0; i < 14; i++)
            {
                var count = preferences.GetInt($"{typeLetter[i]}minStars", 0);
                for (var j = 0; j < count; j++) ForcedStars.Add(typeLetter[i]);
            }
        }
        private (EStarType type, ESpectrType spectr) ChooseStarType(bool birth = false)
        {
            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt < 14 && birth)
            {               
                return ((EStar)bsInt).Convert();
            }
            var starType = "";
            if (ForcedStars.Count > 0)
            {
                var choice = random.ItemWithIndex(ForcedStars);
                starType = choice.Item2;
                ForcedStars.RemoveAt(choice.Item1);
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
            var t = preferences.GetFloatFloat($"{sl}planetCount", (1, 10));
            return (int)t.Item2;
            // return preferences.GetInt($"{sl}maxPlanetCount", preferences.GetInt("maxPlanetCount"));
        }

        private int GetMinPlanetCountForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetCount", (1, 10));
            return (int)t.Item1;
            // return preferences.GetInt($"{sl}minPlanetCount", preferences.GetInt("minPlanetCount"));
        }

        private int GetMaxPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetSize", (30, 500));
            return (int)t.Item2;
            // return preferences.GetInt($"{sl}maxPlanetSize", preferences.GetInt("maxPlanetSize"));
        }

        private int GetMinPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            var t = preferences.GetFloatFloat($"{sl}planetSize", (30, 500));
            return (int)t.Item1;
            // return preferences.GetInt($"{sl}minPlanetSize", preferences.GetInt("minPlanetSize"));
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

        private int GetSystemDensityForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}systemDensity", 3);
        }

        private int GetSystemDensityBiasForStar(GSStar star)
        {
            switch (GetSystemDensityForStar(star))
            {
                case 1: return 100;
                case 2: return 70;
                case 4: return 45;
                case 5: return 35;
                default: return 50;
            }
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