using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticScale
{
    public class GS2MainSettings : iConfigurableGenerator
    
    {
        public string Name => "Main Options";

        public string Author => "innominata";

        public string Description => "Main Options";

        public string Version => "1";

        public string GUID => "main.options";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();
        public GSGenPreferences Preferences = new GSGenPreferences();

            


        public GSGenPreferences Export()
        {
            GS2.Warn("!");
            return Preferences;
        }

        public void Generate(int starCount)
        {
            
        }

        public void Import(GSGenPreferences preferences)
        {
            GS2.Warn("!");
            Preferences = preferences;
            if (Preferences.GetInt("Generator", -1) == -1)
            {
                Preferences.Set("Generator", 0);
            }
        }

        public void Init()
        {
            GS2.Warn("!");
            Options.Add(GSUI.Combobox("Generator", new List<string>() { "test", "test2" }, 0, "Generator"));
        }
    }
}
