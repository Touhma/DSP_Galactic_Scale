﻿using System.Collections.Generic;
using System.Linq;
using static GalacticScale.GS2;
namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private readonly GSGenPreferences preferences = new GSGenPreferences();
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

        public void Import(GSGenPreferences preferences)
        {
            // Warn("Importing");
            for (var i = 0; i < preferences.Count; i++)
            {
                var key = preferences.Keys.ElementAt(i);
                //GS2.Log($"pref set {key} {preferences[key]}");
                this.preferences.Set(key, preferences[key]);
            }

            Config.DefaultStarCount = preferences.GetInt("defaultStarCount");
            if (preferences.GetBool("ludicrousMode")) Config.MaxStarCount = 4096;
            else Config.MaxStarCount = 1024;
        }

        public GSGenPreferences Export()
        {
            // Warn("Exporting");
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
            Log("Enabling LudicrousMode");
            Config.MaxStarCount = 4096;
            UI["safeMode"].Set(false);
            preferences.Set("safeMode", false);
            UI["starSizeMulti"].Set(new GSSliderConfig(0.1f, preferences.GetFloat("starSizeMulti"), 100f));
            UI["minPlanetSize"].Set(new GSSliderConfig(5, preferences.GetInt("minPlanetSize"), 500));
            UI["maxPlanetSize"].Set(new GSSliderConfig(50, preferences.GetInt("maxPlanetSize"), 510));
            UI["defaultStarCount"].Set(new GSSliderConfig(1, preferences.GetInt("defaultStarCount"), 4096));
            UI["minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt("minPlanetCount"), 99));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt("maxPlanetCount"), 99));
            for (var i = 0; i < 14; i++)
            {
                UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt($"{typeLetter[i]}minPlanetCount"), 99));
                UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt($"{typeLetter[i]}maxPlanetCount"), 99));
                UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(5, preferences.GetInt($"{typeLetter[i]}minPlanetSize"), 500));
                UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(50, preferences.GetInt($"{typeLetter[i]}maxPlanetSize"), 510));
            }
        }

        public void DisableLudicrousMode()
        {
            Log("Disabling LudicrousMode");
            Config.MaxStarCount = 1024;
            UI["starSizeMulti"].Set(new GSSliderConfig(1f, preferences.GetFloat("starSizeMulti"), 20f));
            UI["minPlanetSize"].Set(new GSSliderConfig(30, preferences.GetInt("minPlanetSize"), 200));
            UI["maxPlanetSize"].Set(new GSSliderConfig(200, preferences.GetInt("maxPlanetSize"), 500));
            UI["defaultStarCount"].Set(new GSSliderConfig(1, preferences.GetInt("defaultStarCount"), 1024));
            UI["minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt("minPlanetCount"), 25));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt("maxPlanetCount"), 25));
            for (var i = 0; i < 14; i++)
            {
                UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, preferences.GetInt($"{typeLetter[i]}minPlanetCount"), 25));
                UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, preferences.GetInt($"{typeLetter[i]}maxPlanetCount"), 25));
                UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(30, preferences.GetInt($"{typeLetter[i]}minPlanetSize"), 200));
                UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(200, preferences.GetInt($"{typeLetter[i]}maxPlanetSize"), 500));
            }
        }

        public void EnableSafeMode()
        {
            GS2.Warn("Enabling SafeMode");
            LockUI("ludicrousMode", false);
            DisableLudicrousMode();
            UI["minPlanetSize"].Set(new GSSliderConfig(100, 100, 200));
            UI["maxPlanetSize"].Set(new GSSliderConfig(200, 300, 400));
            LockUI("birthPlanetSize", 200);
            LockUI("birthPlanetSiTi", false);
            LockUI("birthPlanetUnlock", false);
            LockUI("hugeGasGiants", false);
            LockUI("galaxyDensity", 5);
            UI["defaultStarCount"].Set(new GSSliderConfig(8, 32, 64));
            LockUI("moonsAreSmall", false);
            LockUI("secondarySatellites", false);
            UI["minPlanetCount"].Set(new GSSliderConfig(1, 1, 5));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, 6, 6));
            for (var i = 0; i < 14; i++)
            {
                LockUI($"{typeLetter[i]}minPlanetCount", 1);
                LockUI($"{typeLetter[i]}maxPlanetCount", 6);
                LockUI($"{typeLetter[i]}minPlanetSize", 100);
                LockUI($"{typeLetter[i]}maxPlanetSize", 400);
                LockUI($"{typeLetter[i]}sizeBias", 50);
                LockUI($"{typeLetter[i]}chanceGas", 50);
                LockUI($"{typeLetter[i]}chanceMoon", 50);
                LockUI($"{typeLetter[i]}systemDensity", 3);
            }
        }

        private void LockUI(string key, Val value)
        {
            // GS2.Warn("LockUI " + key + " " + value.ToString());
            UI[key].Set(value);
            UI[key].Disable();
        }

        private void UnlockUI(string key)
        {
            UI[key].Enable();
        }

        public void DisableSafeMode()
        {
            Log("Disabling SafeMode");
            UI["ludicrousMode"].Enable();
            UI["minPlanetSize"].Set(new GSSliderConfig(30, 50, 200));
            UI["maxPlanetSize"].Set(new GSSliderConfig(50, 500, 500));
            UnlockUI("birthPlanetSize");
            UnlockUI("birthPlanetSiTi");
            UnlockUI("birthPlanetUnlock");
            UnlockUI("hugeGasGiants");
            UnlockUI("galaxyDensity");
            UI["defaultStarCount"].Set(new GSSliderConfig(1, 64, 1024));
            UnlockUI("moonsAreSmall");
            UnlockUI("secondarySatellites");
            UI["minPlanetCount"].Set(new GSSliderConfig(0, 1, 25));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, 10, 25));
            for (var i = 0; i < 14; i++)
            {
                UnlockUI($"{typeLetter[i]}minPlanetCount");
                UnlockUI($"{typeLetter[i]}maxPlanetCount");
                UnlockUI($"{typeLetter[i]}minPlanetSize");
                UnlockUI($"{typeLetter[i]}maxPlanetSize");
                UnlockUI($"{typeLetter[i]}sizeBias");
                UnlockUI($"{typeLetter[i]}chanceGas");
                UnlockUI($"{typeLetter[i]}chanceMoon");
                UnlockUI($"{typeLetter[i]}systemDensity");
            }
        }

        private void DefaultStarCountCallback(Val o)
        {
            Config.DefaultStarCount = preferences.GetInt("defaultStarCount", 64);
        }

        private void InitPreferences()
        {
            GS2.Log("InitPreferences");
            preferences.Set("safeMode", false);
            preferences.Set("ludicrousMode", false);
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
            preferences.Set("minPlanetCount", 1);
            preferences.Set("maxPlanetCount", 10);
            preferences.Set("minPlanetSize", 30);
            preferences.Set("maxPlanetSize", 500);
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
            preferences.Set("freqN", 1);
            preferences.Set("freqW", 2);
            preferences.Set("freqRG", 1);
            preferences.Set("freqYG", 1);
            preferences.Set("freqWG", 1);
            preferences.Set("freqBG", 1);
            for (var i = 0; i < 14; i++)
            {
                preferences.Set($"{typeLetter[i]}minPlanetCount", 1);
                preferences.Set($"{typeLetter[i]}maxPlanetCount", 10);
                preferences.Set($"{typeLetter[i]}maxPlanetSize", 500);
                preferences.Set($"{typeLetter[i]}minPlanetSize", 50);
                preferences.Set($"{typeLetter[i]}sizeBias", 50);
                preferences.Set($"{typeLetter[i]}countBias", 50);
                preferences.Set($"{typeLetter[i]}chanceGas", 20);
                preferences.Set($"{typeLetter[i]}chanceMoon", 20);
                preferences.Set($"{typeLetter[i]}systemDensity", 3);
            }
        }

        private void AddUIElements()
        {
            UI.Add("safeMode", Options.Add(GSUI.Checkbox("Safe Mode", false, "safeMode", o =>
            {
                if ((bool) o) EnableSafeMode();
                else DisableSafeMode();
            })));
            UI.Add("ludicrousMode", Options.Add(GSUI.Checkbox("Ludicrous Mode", false, "ludicrousMode", o =>
            {
                if ((bool) o) EnableLudicrousMode();
                else DisableLudicrousMode();
            })));
            UI.Add("galaxyDensity", Options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9, "galaxyDensity")));
            UI.Add("defaultStarCount",
                Options.Add(GSUI.Slider("Default StarCount", 1, 64, 1024, "defaultStarCount",
                    DefaultStarCountCallback)));
            UI.Add("starSizeMulti",
                Options.Add(GSUI.Slider("Star Size Multiplier", 1f, 10f, 20f, 0.1f, "starSizeMulti")));
            UI.Add("birthPlanetSize",
                Options.Add(GSUI.PlanetSizeSlider("Starting Planet Size", 20, 200, 510, "birthPlanetSize")));
            UI.Add("birthPlanetUnlock",
                Options.Add(GSUI.Checkbox("Starting Planet Unlock", false, "birthPlanetUnlock")));
            UI.Add("birthPlanetSiTi", Options.Add(GSUI.Checkbox("Starting planet Si/Ti", false, "birthPlanetSiTi")));
            UI.Add("moonsAreSmall", Options.Add(GSUI.Checkbox("Moons Are Small", true, "moonsAreSmall")));
            UI.Add("moonBias", Options.Add(GSUI.Slider("Gas Giants Moon Bias", 0, 50, 100, "moonBias")));
            UI.Add("hugeGasGiants", Options.Add(GSUI.Checkbox("Huge Gas Giants", true, "hugeGasGiants")));
            UI.Add("tidalLockInnerPlanets",
                Options.Add(GSUI.Checkbox("Tidal Lock Inner Planets", false, "tidalLockInnerPlanets")));
            UI.Add("secondarySatellites",
                Options.Add(GSUI.Checkbox("Secondary satellites", false, "secondarySatellites")));

            UI.Add("freqK", Options.Add(GSUI.Slider("Freq. Type K", 0, 40, 100, "freqK")));
            UI.Add("freqM", Options.Add(GSUI.Slider("Freq. Type M", 0, 50, 100, "freqM")));
            UI.Add("freqG", Options.Add(GSUI.Slider("Freq. Type G", 0, 30, 100, "freqG")));
            UI.Add("freqF", Options.Add(GSUI.Slider("Freq. Type F", 0, 25, 100, "freqF")));
            UI.Add("freqA", Options.Add(GSUI.Slider("Freq. Type A", 0, 10, 100, "freqA")));
            UI.Add("freqB", Options.Add(GSUI.Slider("Freq. Type B", 0, 4, 100, "freqB")));
            UI.Add("freqO", Options.Add(GSUI.Slider("Freq. Type O", 0, 2, 100, "freqO")));
            UI.Add("freqBH", Options.Add(GSUI.Slider("Freq. BlackHole", 0, 1, 100, "freqBH")));
            UI.Add("freqN", Options.Add(GSUI.Slider("Freq. Neutron", 0, 1, 100, "freqN")));
            UI.Add("freqW", Options.Add(GSUI.Slider("Freq. WhiteDwarf", 0, 2, 100, "freqW")));
            UI.Add("freqRG", Options.Add(GSUI.Slider("Freq. Red Giant", 0, 1, 100, "freqRG")));
            UI.Add("freqYG", Options.Add(GSUI.Slider("Freq. Yellow Giant", 0, 1, 100, "freqYG")));
            UI.Add("freqWG", Options.Add(GSUI.Slider("Freq. White Giant", 0, 1, 100, "freqWG")));
            UI.Add("freqBG", Options.Add(GSUI.Slider("Freq. Blue Giant", 0, 1, 100, "freqBG")));

            //options.Add(GSUI.Header("Default Settings", "Changing These Will Reset All Star Specific Options Below"));
            UI.Add("minPlanetCount",
                Options.Add(GSUI.Slider("Min Planets/System", 1, 4, 25, "minPlanetCount", MinPlanetCountCallback)));
            UI.Add("maxPlanetCount",
                Options.Add(GSUI.Slider("Max Planets/System", 1, 10, 25, "maxPlanetCount", MaxPlanetCountCallback)));
            UI.Add("countBias",
                Options.Add(GSUI.Slider("Planet Count Bias", 0, 50, 100, "sizeBias", CountBiasCallback)));
            UI.Add("minPlanetSize",
                Options.Add(GSUI.PlanetSizeSlider("Min planet size", 30, 50, 200, "minPlanetSize",
                    MinPlanetSizeCallback)));
            UI.Add("maxPlanetSize",
                Options.Add(GSUI.PlanetSizeSlider("Max planet size", 200, 500, 500, "maxPlanetSize",
                    MaxPlanetSizeCallback)));
            UI.Add("sizeBias", Options.Add(GSUI.Slider("Planet Size Bias", 0, 50, 100, "sizeBias", SizeBiasCallback)));

            UI.Add("chanceGas", Options.Add(GSUI.Slider("Chance Gas", 10, 20, 50, "chanceGas", GasChanceCallback)));
            UI.Add("chanceMoon", Options.Add(GSUI.Slider("Chance Moon", 10, 20, 80, "chanceMoon", MoonChanceCallback)));
            UI.Add("systemDensity",
                Options.Add(GSUI.Slider("System Density", 1, 3, 5, "systemDensity", SystemDensityCallback)));

            for (var i = 0; i < 14; i++)
            {
                //options.Add(GSUI.Header("$Type K Star Override", "Settings for K type stars only"));
                typeCallbacks.Add($"{typeLetter[i]}minPlanetSize", CreateTypeMinPlanetSizeCallback(typeLetter[i]));
                typeCallbacks.Add($"{typeLetter[i]}maxPlanetSize", CreateTypeMaxPlanetSizeCallback(typeLetter[i]));
                UI.Add($"{typeLetter[i]}minPlanetCount",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} Min Planets", 1, 1, 25, $"{typeLetter[i]}minPlanetCount")));
                UI.Add($"{typeLetter[i]}maxPlanetCount",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} Max Planets", 1, 10, 25,
                        $"{typeLetter[i]}maxPlanetCount")));
                UI.Add($"{typeLetter[i]}countBias",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} Count Bias", 0, 50, 100, $"{typeLetter[i]}countBias")));
                UI.Add($"{typeLetter[i]}minPlanetSize",
                    Options.Add(GSUI.PlanetSizeSlider($"{typeDesc[i]} Min Size", 30, 50, 200,
                        $"{typeLetter[i]}minPlanetSize", typeCallbacks[$"{typeLetter[i]}minPlanetSize"])));
                UI.Add($"{typeLetter[i]}maxPlanetSize",
                    Options.Add(GSUI.PlanetSizeSlider($"{typeDesc[i]} Max Size", 200, 500, 500,
                        $"{typeLetter[i]}maxPlanetSize", typeCallbacks[$"{typeLetter[i]}maxPlanetSize"])));
                UI.Add($"{typeLetter[i]}sizeBias",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} Size Bias", 0, 50, 100, $"{typeLetter[i]}sizeBias")));
                UI.Add($"{typeLetter[i]}chanceGas",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} %Gas", 10, 20, 50, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} %Moon", 10, 20, 80, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}systemDensity",
                    Options.Add(GSUI.Slider($"{typeDesc[i]} Density", 1, 3, 5, $"{typeLetter[i]}systemDensity")));
            }
        }

        private GSOptionCallback CreateTypeMinPlanetSizeCallback(string type)
        {
            return o =>
            {
                var maxSize = preferences.GetFloat($"{type}maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float) o) o = maxSize;
                if (preferences.GetBool("safeMode")) preferences.Set($"{type}minPlanetSize", SafePlanetSize((float) o));
                else preferences.Set($"{type}minPlanetSize", Utils.ParsePlanetSize((float) o));
                UI[$"{type}minPlanetSize"].Set(preferences.GetFloat($"{type}minPlanetSize"));
            };
        }

        private GSOptionCallback CreateTypeMaxPlanetSizeCallback(string type)
        {
            return o =>
            {
                var minSize = preferences.GetFloat($"{type}minPlanetSize");
                if (minSize == -1f) minSize = 5;
                if (minSize > (float) o) o = minSize;
                if (preferences.GetBool("safeMode")) preferences.Set($"{type}maxPlanetSize", SafePlanetSize((float) o));
                else preferences.Set($"{type}maxPlanetSize", Utils.ParsePlanetSize((float) o));
                UI[$"{type}maxPlanetSize"].Set(preferences.GetFloat($"{type}maxPlanetSize"));
            };
        }

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
            SetAllStarTypeOptions("chanceGas", o);
        }

        private void SystemDensityCallback(Val o)
        {
            SetAllStarTypeOptions("systemDensity", o);
        }

        private void MinPlanetCountCallback(Val o)
        {
            // Log("MinPlanetCountCallback " + o.String());
            var maxCount = preferences.GetInt("maxPlanetCount");
            if (maxCount == -1f) maxCount = 25;
            if (maxCount < o)
            {
                //GS2.Warn("<");
                o = maxCount;
                preferences.Set("minPlanetCount", maxCount);
                UI["minPlanetCount"].Set(o);
            }

            SetAllStarTypeOptions("minPlanetCount", o);
        }

        private void MaxPlanetCountCallback(Val o)
        {
            var minCount = preferences.GetInt("minPlanetCount");
            if (minCount == -1f) minCount = 1;
            if (minCount > o)
            {
                //GS2.Warn(">");
                o = minCount;
                preferences.Set("maxPlanetCount", minCount);
                UI["maxPlanetCount"].Set(o);
            }

            SetAllStarTypeOptions("maxPlanetCount", o);
        }

        private void SetAllStarTypeOptions(string key, Val value)
        {
            for (var i = 0; i < 14; i++) UI[$"{typeLetter[i]}{key}"].Set(value);
        }

        private void SetAllStarTypeMinSize(Val value)
        {
            for (var i = 0; i < 14; i++) typeCallbacks[$"{typeLetter[i]}minPlanetSize"](value);
        }

        private void SetAllStarTypeMaxSize(Val value)
        {
            for (var i = 0; i < 14; i++) typeCallbacks[$"{typeLetter[i]}maxPlanetSize"](value);
        }

        private void MinPlanetSizeCallback(Val o)
        {
            var maxSize = preferences.GetFloat("maxPlanetSize");
            if (maxSize == -1f) maxSize = 510;
            if (maxSize < o) o = maxSize;
            if (preferences.GetBool("safeMode")) preferences.Set("minPlanetSize", SafePlanetSize(o));
            else preferences.Set("minPlanetSize", Utils.ParsePlanetSize(o));
            UI["minPlanetSize"].Set(preferences.GetFloat("minPlanetSize"));
            SetAllStarTypeMinSize(o);
        }

        private void MaxPlanetSizeCallback(Val o)
        {
            var t = o;
            var minSize = preferences.GetFloat("minPlanetSize");
            if (minSize == -1f) minSize = 5;
            if (minSize > o) o = minSize;
            if (preferences.GetBool("safeMode")) preferences.Set("maxPlanetSize", SafePlanetSize(o));
            else preferences.Set("maxPlanetSize", Utils.ParsePlanetSize(o));
            UI["maxPlanetSize"].Set(preferences.GetFloat("maxPlanetSize"));
            SetAllStarTypeMaxSize(o);
        }

        private float SafePlanetSize(float size)
        {
            if (size > 350) return 400;
            if (size > 250) return 300;
            if (size > 150) return 200;
            return 100;
        }

        private Dictionary<string, double> CalculateFrequencies()
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
            var fN = preferences.GetDouble("freqN", 1);
            var fW = preferences.GetDouble("freqW", 2);
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
            StarFreqTupleArray[8] = ("N", fN / total);
            StarFreqTupleArray[9] = ("W", fW / total);
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

            return starFreq;
        }

        private (EStarType type, ESpectrType spectr) ChooseStarType()
        {
            var choice = random.NextDouble();
            var starType = "";
            for (var i = 0; i < starFreq.Count; i++)
                if (choice < starFreq.ElementAt(i).Value)
                {
                    starType = starFreq.ElementAt(i).Key;
                    //GS2.Warn($"Picked Startype {starType} with choice {choice} and value {starFreq.ElementAt(i).Value}");
                    break;
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
                case "N": return (EStarType.NeutronStar, ESpectrType.X);
                case "W": return (EStarType.WhiteDwarf, ESpectrType.X);
                case "RG": return (EStarType.GiantStar, ESpectrType.M);
                case "YG": return (EStarType.GiantStar, ESpectrType.G);
                case "WG": return (EStarType.GiantStar, ESpectrType.A);
                default: return (EStarType.GiantStar, ESpectrType.B);
            }
        }

        private int GetMaxPlanetCountForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}maxPlanetCount", preferences.GetInt("maxPlanetCount"));
        }

        private int GetMinPlanetCountForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}minPlanetCount", preferences.GetInt("minPlanetCount"));
        }

        private int GetMaxPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}maxPlanetSize", preferences.GetInt("maxPlanetSize"));
        }

        private int GetMinPlanetSizeForStar(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            return preferences.GetInt($"{sl}minPlanetSize", preferences.GetInt("minPlanetSize"));
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
                case 5: return 90;
                case 4: return 70;
                case 2: return 30;
                case 1: return 10;
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