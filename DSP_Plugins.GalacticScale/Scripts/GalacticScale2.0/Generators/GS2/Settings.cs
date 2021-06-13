﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace GalacticScale.Generators {
    public partial class GS2Generator : iConfigurableGenerator {
        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        public  Dictionary<string, GSUI> UI = new Dictionary<string, GSUI>();
        private Dictionary<string, double> starFreq = new Dictionary<string, double>();
        string[] typeDesc = {"Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O", "White Dwarf", "Red Giant", "Yellow Giant", "White Giant",
                "Blue Giant", "Neutron Star", "Black Hole" };
        string[] typeLetter = { "K", "M", "F", "G", "A", "B", "O", "WD", "RG", "YG", "WG", "BG", "NS", "BH" };
        public void EnableLudicrousMode() {
            UI["safeMode"].Set(false);
            preferences.Set("safeMode", false);
            UI["minPlanetSize"].Set(new GSSliderConfig(5, 30, 500));
            UI["maxPlanetSize"].Set(new GSSliderConfig(50, 500, 510));
            UI["defaultStarCount"].Set(new GSSliderConfig(1, 64, 4096));
            UI["minPlanetCount"].Set(new GSSliderConfig(0, 1, 99));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, 10, 99));
            for (var i = 0; i < 14; i++) {
                UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, 1, 99));
                UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, 10, 99));
                UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(5, 30, 500));
                UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(50, 500, 510));
            }
        }
        public void DisableLudicrousMode() {
            UI["minPlanetSize"].Set(new GSSliderConfig(30, 50, 200));
            UI["maxPlanetSize"].Set(new GSSliderConfig(200, 500, 500));
            UI["defaultStarCount"].Set(new GSSliderConfig(1, 64, 1024));
            UI["minPlanetCount"].Set(new GSSliderConfig(0, 1, 25));
            UI["maxPlanetCount"].Set(new GSSliderConfig(1, 10, 25));
            for (var i = 0; i < 14; i++) {
                UI[$"{typeLetter[i]}minPlanetCount"].Set(new GSSliderConfig(0, 1, 25));
                UI[$"{typeLetter[i]}maxPlanetCount"].Set(new GSSliderConfig(1, 10, 25));
                UI[$"{typeLetter[i]}minPlanetSize"].Set(new GSSliderConfig(30, 50, 200));
                UI[$"{typeLetter[i]}maxPlanetSize"].Set(new GSSliderConfig(200, 500, 500));
            }
        }
        public void EnableSafeMode() {
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
            for (var i = 0; i < 14; i++) {
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
        private void LockUI(string key, object value) {
            GS2.Warn("LockUI " + key +" " + value.ToString());
            UI[key].Set(value);
            UI[key].Disable();
        }
        private void UnlockUI(string key) => UI[key].Enable();
        public void DisableSafeMode() {
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
            for (var i = 0; i < 14; i++) {
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
        public void Init() {
            config.DefaultStarCount = 10;
            AddUIElements();
        }
        private void AddUIElements() {
            UI.Add("safeMode", options.Add(GSUI.Checkbox("Safe Mode", false, "safeMode", o => { if ((bool)o) EnableSafeMode(); else DisableSafeMode(); })));
            UI.Add("ludicrousMode", options.Add(GSUI.Checkbox("Ludicrous Mode", false, "ludicrousMode", o => { if ((bool)o) EnableLudicrousMode(); else DisableLudicrousMode(); })));
            UI.Add("galaxyDensity", options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9, "galaxyDensity")));
            UI.Add("defaultStarCount", options.Add(GSUI.Slider("Default StarCount", 1, 64, 1024, "defaultStarCount")));
            UI.Add("birthPlanetSize", options.Add(GSUI.Slider("Starting Planet Size", 20, 50, 510, "birthPlanetSize")));
            UI.Add("birthPlanetUnlock", options.Add(GSUI.Checkbox("Starting Planet Unlock", false, "birthPlanetUnlock")));
            UI.Add("birthPlanetSiTi", options.Add(GSUI.Checkbox("Starting planet Si/Ti", false, "birthPlanetSiTi")));
            UI.Add("moonsAreSmall", options.Add(GSUI.Checkbox("Moons are small", true, "moonsAreSmall")));
            UI.Add("hugeGasGiants", options.Add(GSUI.Checkbox("Huge gas giants", true, "hugeGasGiants")));
            UI.Add("tidalLockInnerPlanets", options.Add(GSUI.Checkbox("Tidal Lock Inner Planets", false, "tidalLockInnerPlanets")));
            UI.Add("secondarySatellites", options.Add(GSUI.Checkbox("Secondary satellites", false, "secondarySatellites")));

            UI.Add("freqK", options.Add(GSUI.Slider("Freq. Type K", 0, 40, 100, "freqK")));
            UI.Add("freqM", options.Add(GSUI.Slider("Freq. Type M", 0, 50, 100, "freqM")));
            UI.Add("freqG", options.Add(GSUI.Slider("Freq. Type G", 0, 30, 100, "freqG")));
            UI.Add("freqF", options.Add(GSUI.Slider("Freq. Type F", 0, 25, 100, "freqF")));
            UI.Add("freqA", options.Add(GSUI.Slider("Freq. Type A", 0, 10, 100, "freqA")));
            UI.Add("freqB", options.Add(GSUI.Slider("Freq. Type B", 0, 4, 100, "freqB")));
            UI.Add("freqO", options.Add(GSUI.Slider("Freq. Type O", 0, 2, 100, "freqO")));
            UI.Add("freqBH", options.Add(GSUI.Slider("Freq. BlackHole", 0, 1, 100, "freqBH")));
            UI.Add("freqN", options.Add(GSUI.Slider("Freq. Neutron", 0, 1, 100, "freqN")));
            UI.Add("freqW", options.Add(GSUI.Slider("Freq. WhiteDwarf", 0, 2, 100, "freqW")));
            UI.Add("freqRG", options.Add(GSUI.Slider("Freq. Red Giant", 0, 1, 100, "freqRG")));
            UI.Add("freqYG", options.Add(GSUI.Slider("Freq. Yellow Giant", 0, 1, 100, "freqYG")));
            UI.Add("freqWG", options.Add(GSUI.Slider("Freq. White Giant", 0, 1, 100, "freqWG")));
            UI.Add("freqBG", options.Add(GSUI.Slider("Freq. Blue Giant", 0, 1, 100, "freqBG")));

            //options.Add(GSUI.Header("Default Settings", "Changing These Will Reset All Star Specific Options Below"));
            UI.Add("minPlanetCount", options.Add(GSUI.Slider("Min Planets/System", 1, 4, 50, "minPlanetCount")));
            UI.Add("maxPlanetCount", options.Add(GSUI.Slider("Max Planets/System", 1, 10, 50, "maxPlanetCount")));
            UI.Add("maxPlanetSize", options.Add(GSUI.Slider("Max planet size", 50, 30, 510, MaxPlanetSizeCallback)));
            UI.Add("minPlanetSize", options.Add(GSUI.Slider("Min planet size", 5, 30, 510, MinPlanetSizeCallback)));
            UI.Add("sizeBias", options.Add(GSUI.Slider("Planet Size Bias", 0, 50, 100, "sizeBias", SizeBiasCallback)));
            UI.Add("chanceGas", options.Add(GSUI.Slider("Chance Gas", 10, 20, 50, "chanceGas")));
            UI.Add("chanceMoon", options.Add(GSUI.Slider("Chance Moon", 10, 20, 80, "chanceMoon")));
            UI.Add("systemDensity", options.Add(GSUI.Slider("System Density", 1, 3, 5, "systemDensity")));

            for (var i = 0; i < 14; i++) {
                //options.Add(GSUI.Header("$Type K Star Override", "Settings for K type stars only"));
                UI.Add($"{typeLetter[i]}minPlanetCount", options.Add(GSUI.Slider($"{typeDesc[i]} Min Planets", 1, 1, 25, $"{typeLetter[i]}minPlanetCount")));
                UI.Add($"{typeLetter[i]}maxPlanetCount", options.Add(GSUI.Slider($"{typeDesc[i]} Max Planets", 1, 10, 25, $"{typeLetter[i]}maxPlanetCount")));
                UI.Add($"{typeLetter[i]}minPlanetSize", options.Add(GSUI.Slider($"{typeDesc[i]} Min Size", 30, 50, 200, $"{typeLetter[i]}minPlanetSize")));
                UI.Add($"{typeLetter[i]}maxPlanetSize", options.Add(GSUI.Slider($"{typeDesc[i]} Max Size", 200, 500, 500, $"{typeLetter[i]}maxPlanetSize")));
                UI.Add($"{typeLetter[i]}sizeBias", options.Add(GSUI.Slider($"{typeDesc[i]} Size Bias", 0, 50, 100, $"{typeLetter[i]}sizeBias")));
                UI.Add($"{typeLetter[i]}chanceGas", options.Add(GSUI.Slider($"{typeDesc[i]} %Gas", 10, 20, 50, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon", options.Add(GSUI.Slider($"{typeDesc[i]} %Moon", 10, 20, 80, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}systemDensity", options.Add(GSUI.Slider($"{typeDesc[i]} Density", 1, 3, 5, $"{typeLetter[i]}systemDensity")));
            }
        }
        private void SizeBiasCallback(object o) {
            SetAllStarTypeOptions("sizeBias", o);
        }
        private void MoonChanceCallback(object o) {

        }
        private void SetAllStarTypeOptions(string key, object value) {
            for (var i = 0; i < 14; i++) {
                UI[$"{typeLetter[i]}{key}"].Set(value);
            }
        }
        private void MinPlanetSizeCallback(object o) {
            float maxSize = preferences.GetFloat("maxPlanetSize");
            if (maxSize == -1f) maxSize = 510;
            if (maxSize < (float)o) o = maxSize;
            if (preferences.GetBool("safeMode", false)) preferences.Set("minPlanetSize", SafePlanetSize((float)o));
            else preferences.Set("minPlanetSize", Utils.ParsePlanetSize((float)o));
            UI["minPlanetSize"].Set(preferences.GetFloat("minPlanetSize"));
        }
        private void MaxPlanetSizeCallback(object o) {
            float minSize = preferences.GetFloat("minPlanetSize");
            if (minSize == -1f) minSize = 5;
            if (minSize > (float)o) o = minSize;
            if (preferences.GetBool("safeMode", false)) preferences.Set("maxPlanetSize", SafePlanetSize((float)o));
            else preferences.Set("maxPlanetSize", Utils.ParsePlanetSize((float)o));
            UI["maxPlanetSize"].Set(preferences.GetFloat("maxPlanetSize"));
        }
        private float SafePlanetSize(float size) {
            if (size > 350) return 400;
            if (size > 250) return 300;
            if (size > 150) return 200;
            return 100;
        }
        private Dictionary<string, double> CalculateFrequencies() {
            (string type, double chance)[] StarFreqTupleArray = new (string type, double chance)[14];
            double fK  = preferences.GetDouble("freqK", 40 );
            double fM  = preferences.GetDouble("freqM" ,50);
            double fG  = preferences.GetDouble("freqG" ,30);
            double fF  = preferences.GetDouble("freqF" ,25);
            double fA  = preferences.GetDouble("freqA" ,10);
            double fB  = preferences.GetDouble("freqB" ,4);
            double fO  = preferences.GetDouble("freqO" ,2);
            double fBH = preferences.GetDouble("freqBH",1);
            double fN  = preferences.GetDouble("freqN" ,1);
            double fW  = preferences.GetDouble("freqW" ,2);
            double fRG = preferences.GetDouble("freqRG",1);
            double fYG = preferences.GetDouble("freqYG",1);
            double fWG = preferences.GetDouble("freqWG",1);
            double fBG = preferences.GetDouble("freqBG",1);
            double total = fK + fM + fG + fF + fA + fB + fO + fBH + fN + fW + fRG +fYG + fWG +fBG;
            GS2.Warn($"{total} = {fK} + {fM} + {fG} + {fF} + {fA} + {fB} + {fO} + {fBH} + {fN} + {fW} + {fRG} + {fYG} + {fWG} + {fBG}");
            
            StarFreqTupleArray[0] = ("K", fK /total);
            StarFreqTupleArray[1] = ("M", fM /total);
            StarFreqTupleArray[2] = ("G", fG /total);
            StarFreqTupleArray[3] = ("F", fF /total);
            StarFreqTupleArray[4] = ("A", fA /total);
            StarFreqTupleArray[5] = ("B", fB /total);
            StarFreqTupleArray[6] = ("O", fO /total);
            StarFreqTupleArray[7] = ("BH",fBH/total);
            StarFreqTupleArray[8] = ("N", fN /total);
            StarFreqTupleArray[9] = ("W", fW /total);
            StarFreqTupleArray[10] = ("RG",fRG/total);
            StarFreqTupleArray[11] = ("YG",fYG/total);
            StarFreqTupleArray[12] = ("WG",fWG/total);
            StarFreqTupleArray[13] = ("BG",fBG/total);
            //string[] keys = StarFreq.Keys.ToArray();
            GS2.LogJson(StarFreqTupleArray, true);
            starFreq = new Dictionary<string, double>();
            for (var i=1;i<StarFreqTupleArray.Length;i++) {
                var element = StarFreqTupleArray[i];
                var previousElement = StarFreqTupleArray[i-1];
                starFreq.Add(element.type, (element.chance + previousElement.chance));
                StarFreqTupleArray[i].chance += previousElement.chance;
            }
            GS2.LogJson(starFreq, true);

            return starFreq;
        }
        private (EStarType type, ESpectrType spectr) ChooseStarType() {
            double choice = random.NextDouble();
            string starType = "";
            for (var i=0;i<starFreq.Count;i++) {
                if (choice < starFreq.ElementAt(i).Value) {
                    starType = starFreq.ElementAt(i).Key;
                    GS2.Warn($"Picked Startype {starType} with choice {choice} and value {starFreq.ElementAt(i).Value}");
                    break;
                }
            }
            switch (starType) {
                case "K":  return (EStarType.MainSeqStar, ESpectrType.K);
                case "M":  return (EStarType.MainSeqStar, ESpectrType.M);
                case "G":  return (EStarType.MainSeqStar, ESpectrType.G);
                case "F":  return (EStarType.MainSeqStar, ESpectrType.F);
                case "A":  return (EStarType.MainSeqStar, ESpectrType.A);
                case "B":  return (EStarType.MainSeqStar, ESpectrType.B);
                case "O":  return (EStarType.MainSeqStar, ESpectrType.O);
                case "BH": return (EStarType.BlackHole,   ESpectrType.X);
                case "N":  return (EStarType.NeutronStar, ESpectrType.X);
                case "W":  return (EStarType.WhiteDwarf,  ESpectrType.X);
                case "RG": return (EStarType.GiantStar,   ESpectrType.M);
                case "YG": return (EStarType.GiantStar,   ESpectrType.G);
                case "WG": return (EStarType.GiantStar,   ESpectrType.A);
                default:   return (EStarType.GiantStar,   ESpectrType.B);
            }
        }

        public void Import(GSGenPreferences preferences) => this.preferences = preferences;

        public GSGenPreferences Export() => preferences;
    }
}