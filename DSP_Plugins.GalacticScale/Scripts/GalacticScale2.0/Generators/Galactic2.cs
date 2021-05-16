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
        private List<GSUI> options = new List<GSUI>();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        public void Init()
        {
            config.DefaultStarCount = 16;
            options.Add(GSUI.Checkbox("Ludicrous Mode", false, (object o) => { }, () => { }));
            options.Add(GSUI.Slider("Galaxy Density", 1, 5, 9, (object o) => { }, () => { }));
            options.Add(GSUI.Slider("Max Planet Count", 1, 10, 99, (object o) => { }, () => { }));
            options.Add(GSUI.Checkbox("Secondary Satellites", false, (object o) => { }, () => { }));
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
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }
    }
}