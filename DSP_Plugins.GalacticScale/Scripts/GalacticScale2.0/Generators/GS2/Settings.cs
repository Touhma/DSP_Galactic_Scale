using System.Collections.Generic;
namespace GalacticScale.Generators {
    public partial class GS2Generator : iConfigurableGenerator {
        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public GSOptions Options => options;
        private readonly GSOptions options = new GSOptions();
        private readonly GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        private readonly Dictionary<string, GSUI> UI = new Dictionary<string, GSUI>();

        public void enableLudicrousMode() {

        }
        public void Init() {
            config.DefaultStarCount = 10;
            AddUIElements();
        }
        private void AddUIElements() {
            UI.Add("ludicrousMode", options.Add(GSUI.Checkbox("Ludicrous Mode", false, "ludicrousMode", o => enableLudicrousMode())));
            UI.Add("galaxyDensity", options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9, "galaxyDensity")));
            UI.Add("defaultStarCount", options.Add(GSUI.Slider("Default StarCount", 1, 64, 1024, "defaultStarCount")));
            UI.Add("birthPlanetSize", options.Add(GSUI.Slider("Starting Planet Size", 20, 50, 510, "birthPlanetSize")));
            UI.Add("birthPlanetUnlock", options.Add(GSUI.Checkbox("Starting Planet Unlock", false, "birthPlanetUnlock")));
            UI.Add("birthPlanetSiTi", options.Add(GSUI.Checkbox("Starting planet Si/Ti", false, "birthPlanetSiTi")));
            UI.Add("moonsAreSmall", options.Add(GSUI.Checkbox("Moons are small", true, "moonsAreSmall")));
            UI.Add("hugeGasGiants", options.Add(GSUI.Checkbox("Huge gas giants", true, "hugeGasGiants")));
            UI.Add("tidalLockInnerPlanets", options.Add(GSUI.Checkbox("Tidal Lock Inner Planets", false, "tidalLockInnerPlanets")));
            UI.Add("secondarySatellites", options.Add(GSUI.Checkbox("Secondary satellites", false, "secondarySatellites")));

            UI.Add("freqK", options.Add(GSUI.Slider("Freq. Type K", 1, 40, 100, "freqK")));
            UI.Add("freqM", options.Add(GSUI.Slider("Freq. Type M", 1, 50, 100, "freqM")));
            UI.Add("freqG", options.Add(GSUI.Slider("Freq. Type G", 1, 30, 100, "freqG")));
            UI.Add("freqF", options.Add(GSUI.Slider("Freq. Type F", 1, 25, 100, "freqF")));
            UI.Add("freqA", options.Add(GSUI.Slider("Freq. Type A", 1, 10, 100, "freqA")));
            UI.Add("freqB", options.Add(GSUI.Slider("Freq. Type B", 1, 4, 100, "freqB")));
            UI.Add("freqO", options.Add(GSUI.Slider("Freq. Type O", 1, 2, 100, "freqO")));
            UI.Add("freqBH", options.Add(GSUI.Slider("Freq. BlackHole", 1, 1, 100, "freqBH")));
            UI.Add("freqN", options.Add(GSUI.Slider("Freq. Neutron", 1, 1, 100, "freqN")));
            UI.Add("freqW", options.Add(GSUI.Slider("Freq. WhiteDwarf", 1, 2, 100, "freqW")));
            UI.Add("freqRG", options.Add(GSUI.Slider("Freq. Red Giant", 1, 1, 100, "freqRG")));
            UI.Add("freqYG", options.Add(GSUI.Slider("Freq. Yellow Giant", 1, 1, 100, "freqYG")));
            UI.Add("freqWG", options.Add(GSUI.Slider("Freq. White Giant", 1, 1, 100, "freqWG")));
            UI.Add("freqBG", options.Add(GSUI.Slider("Freq. Blue Giant", 1, 1, 100, "freqBG")));

            //options.Add(GSUI.Header("Default Settings", "Changing These Will Reset All Star Specific Options Below"));
            UI.Add("minPlanetCount", options.Add(GSUI.Slider("Min Planets/System", 1, 4, 50, "minPlanetCount")));
            UI.Add("maxPlanetCount", options.Add(GSUI.Slider("Max Planets/System", 1, 10, 50, "maxPlanetCount")));
            UI.Add("maxPlanetSize", options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o => {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) {
                    minSize = 5;
                }

                if (minSize > (float)o) {
                    o = minSize;
                }

                preferences.Set("maxPlanetSize", Utils.ParsePlanetSize((float)o));
                UI["maxPlanetSize"].Set(preferences.GetFloat("maxPlanetSize"));
            })));
            UI.Add("minPlanetSize", options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o => {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) {
                    maxSize = 510;
                }

                if (maxSize < (float)o) {
                    o = maxSize;
                }

                preferences.Set("minPlanetSize", Utils.ParsePlanetSize((float)o));
                UI["minPlanetSize"].Set(preferences.GetFloat("minPlanetSize"));
            })));

            UI.Add("sizeBias", options.Add(GSUI.Slider("Planet Size Bias", 0, 50, 100, "sizeBias")));
            UI.Add("chanceGas", options.Add(GSUI.Slider("Chance Gas", 10, 20, 50, "chanceGas")));
            UI.Add("chanceMoon", options.Add(GSUI.Slider("Chance Moon", 10, 20, 80, "chanceMoon")));
            UI.Add("systemDensity", options.Add(GSUI.Slider("System Density", 1, 3, 5, "systemDensity")));



            string[] typeDesc = {"Type K", "Type M", "Type F", "Type G", "Type A", "Type B", "Type O", "White Dwarf", "Red Giant", "Yellow Giant", "White Giant",
                "Blue Giant", "Neutron Star", "Black Hole" };
            string[] typeLetter = { "K", "M", "F", "G", "A", "B", "O", "WD", "RG", "YG", "WG", "BG", "NS", "BH" };

            for (var i = 0; i < 14; i++) {
                //options.Add(GSUI.Header("$Type K Star Override", "Settings for K type stars only"));
                UI.Add($"{typeLetter[i]}minPlanetCount", options.Add(GSUI.Slider($"{typeDesc[i]} Min Planets", 1, 4, 50, $"{typeLetter[i]}minPlanetCount")));
                UI.Add($"{typeLetter[i]}maxPlanetCount", options.Add(GSUI.Slider($"{typeDesc[i]} Max Planets", 1, 10, 50, $"{typeLetter[i]}maxPlanetCount")));
                UI.Add($"{typeLetter[i]}minPlanetSize", options.Add(GSUI.Slider($"{typeDesc[i]} Min Size", 1, 4, 50, $"{typeLetter[i]}minPlanetSize")));
                UI.Add($"{typeLetter[i]}maxPlanetSize", options.Add(GSUI.Slider($"{typeDesc[i]} Max Size", 1, 10, 50, $"{typeLetter[i]}maxPlanetSize")));
                UI.Add($"{typeLetter[i]}sizeBias", options.Add(GSUI.Slider($"{typeDesc[i]} Size Bias", 0, 50, 100, $"{typeLetter[i]}sizeBias")));
                UI.Add($"{typeLetter[i]}chanceGas", options.Add(GSUI.Slider($"{typeDesc[i]} %Gas", 10, 20, 50, $"{typeLetter[i]}chanceGas")));
                UI.Add($"{typeLetter[i]}chanceMoon", options.Add(GSUI.Slider($"{typeDesc[i]} %Moon", 10, 20, 80, $"{typeLetter[i]}chanceMoon")));
                UI.Add($"{typeLetter[i]}systemDensity", options.Add(GSUI.Slider($"{typeDesc[i]} Density", 1, 3, 5, $"{typeLetter[i]}systemDensity")));
            }

        }

        public void Import(GSGenPreferences preferences) => this.preferences = preferences;

        public GSGenPreferences Export() => preferences;
    }
}