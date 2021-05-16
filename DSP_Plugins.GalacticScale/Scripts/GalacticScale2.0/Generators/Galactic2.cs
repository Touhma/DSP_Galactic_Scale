using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class GalacticScale2 : iConfigurableGenerator
    {
        public string Name => "GalacticScale2";

        public string Author => "innominata";

        public string Description => "Just like the other generators, but more so";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.galacticscale2";

        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public List<GSUI> Options => options;
        private GSOptions options = new GSOptions();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();

        private GSUI UI_ludicrousMode;
        private GSUI UI_birthPlanetSize;
        private GSUI UI_minPlanetSize;
        private GSUI UI_maxPlanetSize;
        private GSUI UI_galaxyDensity;
        private GSUI UI_maxPlanetCount;
        private GSUI UI_secondarySatellites;
        private GSUI UI_birthPlanetSiTi;
        private GSUI UI_tidalLockInnerPlanets;
        private GSUI UI_moonsAreSmall; 
        private GSUI UI_hugeGasGiants; 
        private GSUI UI_regularBirthTheme; 
        private GSUI UI_systemDensity;
        public void Init()
        {
            config.DefaultStarCount = 16;
            UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            UI_maxPlanetCount = options.Add(GSUI.Slider("Max planets per system", 1, 10, 99, o => preferences.Set("maxPlanetCount", o)));
            UI_minPlanetSize = options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o =>
            {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float)o) o = maxSize;
                preferences.Set("minPlanetSize", GS2.Utils.ParsePlanetSize((float)o));
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize"));
            }));
            UI_maxPlanetSize = options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o =>
            {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) minSize = 5;
                GS2.Log("min = " + minSize + " max = " + o.ToString());
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", GS2.Utils.ParsePlanetSize((float)o));
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }));
            UI_secondarySatellites = options.Add(GSUI.Checkbox("Secondary satellites", false, o => preferences.Set("secondarySatellites", o)));
            UI_birthPlanetSize = options.Add(GSUI.Slider("Birth planet size", 20, 50, 510, o => { 
                preferences.Set("birthPlanetSize", GS2.Utils.ParsePlanetSize((float)o)); 
                UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize")); 
            }));
            UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            UI_birthPlanetSiTi = options.Add(GSUI.Checkbox("Birth planet Si/Ti", false, o => preferences.Set("birthPlanetSiTi", o)));
            UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            UI_moonsAreSmall = options.Add(GSUI.Checkbox("Moons are small", true, o => preferences.Set("moonsAreSmall", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
        }

        public void Generate(int starCount)
        {
            for (var i = 0; i < starCount; i++)
            {
                GSStar s = StarDefaults.Random();
            }
        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
            UI_ludicrousMode?.Set(preferences.GetBool("ludicrousMode"));
            UI_birthPlanetSize?.Set(preferences.GetFloat("birthPlanetSize"));
            UI_minPlanetSize?.Set(preferences.GetFloat("minPlanetSize"));
            UI_maxPlanetSize?.Set(preferences.GetFloat("maxPlanetSize"));
            UI_galaxyDensity?.Set(preferences.GetFloat("galaxyDensity"));
            UI_maxPlanetCount?.Set(preferences.GetFloat("maxPlanetCount"));
            UI_secondarySatellites?.Set(preferences.GetBool("secondarySatellites"));
            UI_birthPlanetSiTi?.Set(preferences.GetBool("birthPlanetSiTi"));
            UI_tidalLockInnerPlanets?.Set(preferences.GetBool("tidalLockInnerPlanets"));
            UI_moonsAreSmall?.Set(preferences.GetBool("moonsAreSmall"));
            UI_hugeGasGiants?.Set(preferences.GetBool("hugeGasGiants"));
            UI_regularBirthTheme?.Set(preferences.GetBool("regularBirthTheme"));
            UI_systemDensity?.Set(preferences.GetFloat("systemDensity"));
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }
    }
}