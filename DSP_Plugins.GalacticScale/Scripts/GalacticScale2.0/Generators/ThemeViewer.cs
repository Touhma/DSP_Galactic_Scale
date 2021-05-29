using GSFullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class ThemeViewer : iConfigurableGenerator
    {
        public string Name => "ThemeViewer";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.theme";

        public GSGeneratorConfig Config => new GSGeneratorConfig(false, false, 1, 512, 1);

        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        public ThemeLibrary ThemeTestLibrary = new ThemeLibrary();
        public GSUI uiList;
        public List<string> themeNames { get
            {
                GS2.Log("Start");
                List<string> list = new List<string>();
                foreach (var kvp in ThemeTestLibrary)
                {
                    GSTheme theme = kvp.Value;
                    list.Add(theme.DisplayName);
                }
                GS2.LogJson(list);
                GS2.Log("End");
                return list;
            } }
        public void Init()
        {
            options.Add(GSUI.Button("Capture Themes", "Go", CaptureThemes));
            uiList = options.Add(GSUI.Combobox("Themes Captured", themeNames, null, null));
        }
        public void CaptureThemes(object o)
        {
            GS2.Log("Start");
            if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary.Count == 0)
            {
                GS2.Warn("No Themes Captured");
                return;
            }
            ThemeTestLibrary = GSSettings.ThemeLibrary.Clone();
            GS2.Log("Updating");
            uiList.SetItems(themeNames);
            GS2.Log("End");
        }
        public void Generate(int starCount)
        {
            GS2.Random random = new GS2.Random(GSSettings.Seed);
            GSPlanets p = new GSPlanets();
            var i = 0;
            foreach(var kvp in ThemeTestLibrary)
            {
                p.Add(new GSPlanet(kvp.Key, kvp.Key, 100, random.NextFloat() * 10 + 1, 0, 0, 10000, random.Next(359), 0, 10000, 0, -1, null));
                i++;
            }
            GSStar s = StarDefaults.Random();
            s.Name = "ThemeTest";
            s.Planets = p;
            GSSettings.Stars.Add(s);

        }
        public void Import(GSGenPreferences preferences)
        {

        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }
    }
}