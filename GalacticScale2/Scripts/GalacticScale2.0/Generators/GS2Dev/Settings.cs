using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GalacticScale.GS2;
namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private GSGenPreferences preferences = new GSGenPreferences();
        private Dictionary<string, double> starFreq = new Dictionary<string, double>();

        private readonly Dictionary<string, GSOptionCallback>
            typeCallbacks = new Dictionary<string, GSOptionCallback>();

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
            AddUIElements();
            InitPreferences();
        }

        private static bool loaded = false;
        public void Import(GSGenPreferences preferences)
        {
            
            if (!this.preferences.GetBool("ludicrousMode", false) && preferences.GetBool("ludicrousMode", false))
            {
               
                EnableLudicrousMode();
                Config.MaxStarCount = 4096;
            }

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

        public void EnableLudicrousMode()
        {
            LockUI("ludicrousMode", false);LockUI("safeMode", false);
            return;
            // Log("Enabling LudicrousMode");
            // Config.MaxStarCount = 4096;
            // UI["safeMode"].Set(false);
            // preferences.Set("safeMode", false);
            //
            // UI["starSizeMulti"].Set(new GSSliderConfig(0.1f, preferences.GetFloat("starSizeMulti"), 100f));
            // UI["minPlanetSize"].Set(new GSSliderConfig(5, preferences.GetInt("minPlanetSize"), 500));
            // UI["maxPlanetSize"].Set(new GSSliderConfig(50, preferences.GetInt("maxPlanetSize"), 510));
            // UI["defaultStarCount"].Set(new GSSliderConfig(1, preferences.GetInt("defaultStarCount"), 4096));
            // UI["minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt("minPlanetCount"), 99));
            // UI["maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt("maxPlanetCount"), 99));
            // UI["chanceGas"].Set(new GSSliderConfig(0, preferences.GetInt("chanceGas"), 99));
            // UI["chanceMoon"].Set(new GSSliderConfig(0, preferences.GetInt("chanceMoon"), 99));
            // for (var i = 0; i < 14; i++)
            // {
            //     UI[$"{typeLetter[i]}chanceGas"].Set(new GSSliderConfig(0,preferences.GetInt($"{typeLetter[i]}chanceGas", 99), 99));
            //     UI[$"{typeLetter[i]}chanceMoon"].Set(new GSSliderConfig(0,preferences.GetInt($"{typeLetter[i]}chanceMoon"), 99));
            //     UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt($"{typeLetter[i]}minPlanetCount"), 99));
            //     UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt($"{typeLetter[i]}maxPlanetCount"), 99));
            //     UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(5, preferences.GetInt($"{typeLetter[i]}minPlanetSize"), 500));
            //     UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(50, preferences.GetInt($"{typeLetter[i]}maxPlanetSize"), 510));
            //
            // }
        }

        public void DisableLudicrousMode()
        {
            Log("Disabling LudicrousMode");
            // Config.MaxStarCount = 1024;
            //
            // UI["starSizeMulti"].Set(new GSSliderConfig(1f, preferences.GetFloat("starSizeMulti"), 20f));
            // UI["minPlanetSize"].Set(new GSSliderConfig(30, preferences.GetInt("minPlanetSize"), 200));
            // UI["maxPlanetSize"].Set(new GSSliderConfig(200, preferences.GetInt("maxPlanetSize"), 500));
            // UI["defaultStarCount"].Set(new GSSliderConfig(1, preferences.GetInt("defaultStarCount"), 1024));
            // UI["minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt("minPlanetCount"), 25));
            // UI["maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt("maxPlanetCount"), 25));
            // UI["chanceGas"].Set(new GSSliderConfig(10, Mathf.Clamp(preferences.GetInt("chanceGas"), 10, 50), 50));
            // UI["chanceMoon"].Set(new GSSliderConfig(10, Mathf.Clamp(preferences.GetInt("chanceMoon"), 10, 80), 80));
            // for (var i = 0; i < 14; i++)
            // {
            //     UI[$"{typeLetter[i]}chanceGas"].Set(new GSSliderConfig(0,Mathf.Clamp(preferences.GetInt($"{typeLetter[i]}chanceGas"), 10, 50), 50));
            //     UI[$"{typeLetter[i]}chanceMoon"].Set(new GSSliderConfig(0,Mathf.Clamp(preferences.GetInt($"{typeLetter[i]}chanceMoon"), 10, 80), 80));
            //     UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt($"{typeLetter[i]}minPlanetCount"), 25));
            //     UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt($"{typeLetter[i]}maxPlanetCount"), 25));
            //     UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(30, preferences.GetInt($"{typeLetter[i]}minPlanetSize"), 200));
            //     UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(200, preferences.GetInt($"{typeLetter[i]}maxPlanetSize"), 500));
            //
            // }
        }

        public void EnableSafeMode()
        {
            LockUI("ludicrousMode", false);
            LockUI("safeMode", false);
            return;
            // GS2.Warn("Enabling SafeMode");
            // LockUI("ludicrousMode", false);
            // DisableLudicrousMode();
            // UI["minPlanetSize"].Set(new GSSliderConfig(100, 100, 200));
            // UI["maxPlanetSize"].Set(new GSSliderConfig(200, 300, 400));
            // LockUI("birthPlanetSize", 200);
            // LockUI("birthPlanetSiTi", false);
            // LockUI("birthPlanetUnlock", false);
            // LockUI("hugeGasGiants", false);
            // LockUI("galaxyDensity", 5);
            // UI["defaultStarCount"].Set(new GSSliderConfig(8, 32, 64));
            // LockUI("moonsAreSmall", false);
            // LockUI("secondarySatellites", false);
            // UI["minPlanetCount"].Set(new GSSliderConfig(1, 1, 5));
            // UI["maxPlanetCount"].Set(new GSSliderConfig(1, 6, 6));
            // for (var i = 0; i < 14; i++)
            // {
            //     LockUI($"{typeLetter[i]}minPlanetCount", 1);
            //     LockUI($"{typeLetter[i]}maxPlanetCount", 6);
            //     LockUI($"{typeLetter[i]}minPlanetSize", 100);
            //     LockUI($"{typeLetter[i]}maxPlanetSize", 400);
            //     LockUI($"{typeLetter[i]}sizeBias", 50);
            //     LockUI($"{typeLetter[i]}chanceGas", 50);
            //     LockUI($"{typeLetter[i]}chanceMoon", 50);
            //     LockUI($"{typeLetter[i]}systemDensity", 3);
            // }
        }

        private void LockUI(string key, Val value)
        {
            GS2.Warn($"Start {key}");
            if (UI.ContainsKey(key)) UI[key].Set(value);
            GS2.Warn("#");
            if (UI.ContainsKey(key)) UI[key].Disable();
            GS2.Warn("##");
        }

        private void UnlockUI(string key)
        {
            UI[key].Enable();
        }

        public void DisableSafeMode()
        {
            Log("Disabling SafeMode");
            // UI["ludicrousMode"].Enable();
            // UI["minPlanetSize"].Set(new GSSliderConfig(30, 50, 200));
            // UI["maxPlanetSize"].Set(new GSSliderConfig(50, 500, 500));
            // UnlockUI("birthPlanetSize");
            // UnlockUI("birthPlanetSiTi");
            // UnlockUI("birthPlanetUnlock");
            // UnlockUI("hugeGasGiants");
            // UnlockUI("galaxyDensity");
            // UI["defaultStarCount"].Set(new GSSliderConfig(1, 64, 1024));
            // UnlockUI("moonsAreSmall");
            // UnlockUI("secondarySatellites");
            // UI["minPlanetCount"].Set(new GSSliderConfig(0, 1, 25));
            // UI["maxPlanetCount"].Set(new GSSliderConfig(1, 10, 25));
            // for (var i = 0; i < 14; i++)
            // {
            //     UnlockUI($"{typeLetter[i]}minPlanetCount");
            //     UnlockUI($"{typeLetter[i]}maxPlanetCount");
            //     UnlockUI($"{typeLetter[i]}minPlanetSize");
            //     UnlockUI($"{typeLetter[i]}maxPlanetSize");
            //     UnlockUI($"{typeLetter[i]}sizeBias");
            //     UnlockUI($"{typeLetter[i]}chanceGas");
            //     UnlockUI($"{typeLetter[i]}chanceMoon");
            //     UnlockUI($"{typeLetter[i]}systemDensity");
            // }
        }

        private void DefaultStarCountCallback(Val o)
        {
            Config.DefaultStarCount = preferences.GetInt("defaultStarCount", 64);
        }

        public void Reset(Val o)
        {
            InitPreferences();
            foreach (var ui in UI)
            {
                ui.Value.Set(preferences.Get(ui.Key));
            }
        }

        public Dictionary<string, (float, float)> hzDefs = new Dictionary<string, (float, float)>();
        public void getHZ()
        {
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
            preferences.Set("rotationMulti", 1f);
            preferences.Set("innerPlanetDistance", 1f);
            preferences.Set("safeMode", false);
            preferences.Set("ludicrousMode", false);
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
                preferences.Set($"{typeLetter[i]}orbits", (0.1,10));
                preferences.Set($"{typeLetter[i]}orbitOverride", false);
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
            Val l = preferences.GetBool("ludicrousMode", false);
            GS2.Warn(l);
            UI.Add("safeMode", Options.Add(GSUI.Checkbox("Safe Mode".Translate(), false, "safeMode", o =>
            {
                if ((bool) o) EnableSafeMode();
                else DisableSafeMode();
            }, "Disabled for now".Translate())));
            
            
            UI.Add("ludicrousMode", Options.Add(GSUI.Checkbox("Ludicrous Mode".Translate(), false, "ludicrousMode", o =>
            {
                if ((bool) o) EnableLudicrousMode();
                else DisableLudicrousMode();
            }, "Disabled for now".Translate())));
            
            UI.Add("galaxyDensity", Options.Add(GSUI.Slider("Galaxy Density".Translate(), 1, 5, 9, "galaxyDensity", null, "Higher = Stars are closer to each other".Translate())));
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
            UI.Add("moonBias", Options.Add(GSUI.Slider("Gas Giants Moon Bias".Translate(), 0, 50, 100, "moonBias", null, "Lower prefers telluric plants, higher gas giants".Translate())));
            UI.Add("hugeGasGiants", Options.Add(GSUI.Checkbox("Huge Gas Giants".Translate(), true, "hugeGasGiants", null,"Allow gas giants larger than 800 radius".Translate())));
            UI.Add("tidalLockInnerPlanets",
                Options.Add(GSUI.Checkbox("Tidal Lock Inner Planets".Translate(), false, "tidalLockInnerPlanets")));
            UI.Add("innerPlanetDistance", Options.Add(GSUI.Slider("Inner Planet Distance (AU)".Translate(), 0, 1, 100, 0.1f, "innerPlanetDistance", null, "Distance forced tidal locking stops acting".Translate())));
            UI.Add("allowResonances", Options.Add(GSUI.Checkbox("Allow Orbital Harmonics".Translate(), true, "allowResonances", null, "Allow Orbital Resonance 1:2 and 1:4".Translate())));
            UI.Add("rotationMulti", Options.Add(GSUI.Slider("Rotation Multiplier".Translate(), 0.5f, 1, 100, 0.5f, "rotationMulti", null, "Increase the duration of night/day".Translate())));
            UI.Add("secondarySatellites",
                Options.Add(GSUI.Checkbox("Secondary satellites".Translate(), false, "secondarySatellites", null, "Allow moons to have moons".Translate())));

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
            UI.Add("planetCount", Options.Add(GSUI.RangeSlider("Planet Count".Translate(), 1, 2, 10, 99, 1f, "planetCount", null, planetCountLow, planetCountHigh, "Size of Starting Planet. 200 is normal".Translate())));
            UI.Add("countBias", Options.Add(GSUI.Slider("Planet Count Bias".Translate(), 0, 50, 100, "sizeBias", CountBiasCallback)));
            UI.Add("planetSize",Options.Add(GSUI.PlanetSizeRangeSlider("Telluric Planet Size".Translate(), 5, 50, 400, 510, "planetSize", planetSize, planetSizeLow, planetSizeHigh, "Min/Max Size of Rocky Planets".Translate())));
            UI.Add("sizeBias", Options.Add(GSUI.Slider("Planet Size Bias".Translate(), 0, 50, 100, "sizeBias", SizeBiasCallback)));
            UI.Add("chanceGas", Options.Add(GSUI.Slider("Chance Gas".Translate(), 10, 20, l?99:50, "chanceGas", GasChanceCallback)));
            UI.Add("chanceMoon", Options.Add(GSUI.Slider("Chance Moon".Translate(), 10, 20, 80, "chanceMoon", MoonChanceCallback)));
            // UI.Add("systemDensity", Options.Add(GSUI.Slider("System Density".Translate(), 1, 3, 5, "systemDensity", SystemDensityCallback)));
            Options.Add(GSUI.Separator());
            for (var i = 0; i < 14; i++)
            {
                var tOptions = new GSOptions();
                UI.Add($"{typeLetter[i]}planetCount", tOptions.Add(GSUI.RangeSlider($"{typeDesc[i]} Planet Count".Translate(), 1, 2, 10, 99, 1f, $"{typeLetter[i]}planetCount", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}countBias", tOptions.Add(GSUI.Slider($"{typeDesc[i]} Count Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}countBias", null, "Prefer Less (lower) or More (higher) Counts".Translate())));
                UI.Add($"{typeLetter[i]}planetSize", tOptions.Add(GSUI.RangeSlider($"{typeDesc[i]} Telluric Planet Size".Translate(), 5, 50, 500, 510, 1f, $"{typeLetter[i]}planetSize", null, null, null, "Will be selected randomly from this range".Translate())));
                UI.Add($"{typeLetter[i]}sizeBias", tOptions.Add(GSUI.Slider($"{typeDesc[i]} Telluric Size Bias".Translate(), 0, 50, 100, $"{typeLetter[i]}sizeBias", null, "Prefer Smaller (lower) or Larger (higher) Sizes".Translate())));
                UI.Add($"{typeLetter[i]}hzOverride", tOptions.Add(GSUI.Checkbox("Override Habitable Zone".Translate(), false, $"{typeLetter[i]}hzOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}hz", tOptions.Add(GSUI.RangeSlider("Habitable Zone".Translate(), 0, preferences.GetFloatFloat($"{typeLetter[i]}hz", (0,1)).Item1,preferences.GetFloatFloat($"{typeLetter[i]}hz", (0,3)).Item2, 100, 0.01f, $"{typeLetter[i]}hz", null, null, null, "Habitable Zone override".Translate())));
                UI.Add($"{typeLetter[i]}chanceGas", tOptions.Add(GSUI.Slider($"{typeDesc[i]} %Gas".Translate(), l?0:10, 20, l?99:50, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon", tOptions.Add(GSUI.Slider($"{typeDesc[i]} %Moon".Translate(), l?0:10, 20, l?99:80, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}orbitOverride", tOptions.Add(GSUI.Checkbox("Override Orbits".Translate(), false, $"{typeLetter[i]}orbitOverride", null, "Enable the slider below".Translate())));
                UI.Add($"{typeLetter[i]}orbits", tOptions.Add(GSUI.RangeSlider($"{typeDesc[i]} Orbit Range".Translate(), 0.02f, 0.1f, 30, 99, 1f, $"{typeLetter[i]}orbits", null, null, null, "Distances planets can spawn between".Translate())));

          
                // UI.Add($"{typeLetter[i]}systemDensity", tOptions.Add(GSUI.Slider($"{typeDesc[i]} Density".Translate(), 1, 3, 5, $"{typeLetter[i]}systemDensity", null, "Lower is less dense".Translate())));
                Options.Add(GSUI.Group($"{typeDesc[i]} Overrides".Translate(), tOptions, $"Change Settings for Type {typeDesc[i]} stars".Translate()));
                Options.Add(GSUI.Separator());
            }
            Options.Add(GSUI.Button("Reset".Translate(), "Now".Translate(), Reset));
            loaded = true;
        }

        private void planetCountLow(Val o)
        {
            Warn(o);
            
            SetAllStarTypeRSMin("planetCount", o);
        }    
        private void planetCountHigh(Val o)
        {
            GS2.Warn(o);
            SetAllStarTypeRSMax("planetCount", o);
        }        
        private void planetSizeLow(Val o)
        {
            Warn(o);
            
            SetAllStarTypeRSMin("planetSize", Utils.ParsePlanetSize(o));
        }    
        private void planetSizeHigh(Val o)
        {
            GS2.Warn(o);
            SetAllStarTypeRSMax("planetSize", Utils.ParsePlanetSize(o));
        }

        private void planetSize(Val o)
        {
            Warn(o);
        }
        // private GSOptionCallback CreateTypeMinPlanetSizeCallback(string type)
        // {
        //     return o =>
        //     {
        //         var maxSize = preferences.GetFloat($"{type}maxPlanetSize");
        //         if (maxSize == -1f) maxSize = 510;
        //         if (maxSize < (float) o) o = maxSize;
        //         if (preferences.GetBool("safeMode")) preferences.Set($"{type}minPlanetSize", SafePlanetSize((float) o));
        //         else preferences.Set($"{type}minPlanetSize", Utils.ParsePlanetSize((float) o));
        //         UI[$"{type}minPlanetSize"].Set(preferences.GetFloat($"{type}minPlanetSize"));
        //     };
        // }
        //
        // private GSOptionCallback CreateTypeMaxPlanetSizeCallback(string type)
        // {
        //     return o =>
        //     {
        //         var minSize = preferences.GetFloat($"{type}minPlanetSize");
        //         if (minSize == -1f) minSize = 5;
        //         if (minSize > (float) o) o = minSize;
        //         if (preferences.GetBool("safeMode")) preferences.Set($"{type}maxPlanetSize", SafePlanetSize((float) o));
        //         else preferences.Set($"{type}maxPlanetSize", Utils.ParsePlanetSize((float) o));
        //         UI[$"{type}maxPlanetSize"].Set(preferences.GetFloat($"{type}maxPlanetSize"));
        //     };
        // }

        private void SizeBiasCallback(Val o)
        {
            SetAllStarTypeOptions("sizeBias", o);
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
            GS2.Warn("Setting Gas Chance"+ o.String());
            SetAllStarTypeOptions("chanceGas", o);
        }

        private void SystemDensityCallback(Val o)
        {
            SetAllStarTypeOptions("systemDensity", o);
        }

        // private void MinPlanetCountCallback(Val o)
        // {
        //     var maxCount = preferences.GetInt("maxPlanetCount");
        //     if (maxCount == -1f) maxCount = 25;
        //     if (maxCount < o)
        //     {
        //         //GS2.Warn("<");
        //         o = maxCount;
        //         preferences.Set("minPlanetCount", maxCount);
        //         UI["minPlanetCount"].Set(o);
        //     }
        //
        //     SetAllStarTypeOptions("minPlanetCount", o);
        // }
        //
        // private void MaxPlanetCountCallback(Val o)
        // {
        //     var minCount = preferences.GetInt("minPlanetCount");
        //     if (minCount == -1f) minCount = 1;
        //     if (minCount > o)
        //     {
        //         //GS2.Warn(">");
        //         o = minCount;
        //         preferences.Set("maxPlanetCount", minCount);
        //         UI["maxPlanetCount"].Set(o);
        //     }
        //
        //     SetAllStarTypeOptions("maxPlanetCount", o);
        // }

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
        // private void SetAllStarTypeMinSize(Val value)
        // {
        //     for (var i = 0; i < 14; i++) typeCallbacks[$"{typeLetter[i]}minPlanetSize"](value);
        // }
        //
        // private void SetAllStarTypeMaxSize(Val value)
        // {
        //     for (var i = 0; i < 14; i++) typeCallbacks[$"{typeLetter[i]}maxPlanetSize"](value);
        // }

        // private void MinPlanetSizeCallback(Val o)
        // {
        //     var maxSize = preferences.GetFloat("maxPlanetSize");
        //     if (maxSize == -1f) maxSize = 510;
        //     if (maxSize < o) o = maxSize;
        //     if (preferences.GetBool("safeMode")) preferences.Set("minPlanetSize", SafePlanetSize(o));
        //     else preferences.Set("minPlanetSize", Utils.ParsePlanetSize(o));
        //     UI["minPlanetSize"].Set(preferences.GetFloat("minPlanetSize"));
        //     SetAllStarTypeMinSize(o);
        // }
        //
        // private void MaxPlanetSizeCallback(Val o)
        // {
        //     var t = o;
        //     var minSize = preferences.GetFloat("minPlanetSize");
        //     if (minSize == -1f) minSize = 5;
        //     if (minSize > o) o = minSize;
        //     if (preferences.GetBool("safeMode")) preferences.Set("maxPlanetSize", SafePlanetSize(o));
        //     else preferences.Set("maxPlanetSize", Utils.ParsePlanetSize(o));
        //     UI["maxPlanetSize"].Set(preferences.GetFloat("maxPlanetSize"));
        //     SetAllStarTypeMaxSize(o);
        // }

        // private float SafePlanetSize(float size)
        // {
        //     if (size > 350) return 400;
        //     if (size > 250) return 300;
        //     if (size > 150) return 200;
        //     return 100;
        // }

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
            //GS2.LogJson(starFreq, true);
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
            GS2.Warn("Forced Stars:");
            GS2.WarnJson(ForcedStars);
        }
        private (EStarType type, ESpectrType spectr) ChooseStarType()
        {
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