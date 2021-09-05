using System.Collections.Generic;
using System.Linq;

namespace GalacticScale.Generators
{
    public class ThemeViewer : iConfigurableGenerator
    {
        public static List<string> themeNames = new List<string>();

        public static string themename;
        public ThemeLibrary ThemeTestLibrary = new ThemeLibrary();
        public GSUI uiList;

        public string Name => "ThemeViewer";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.theme";

        public GSGeneratorConfig Config => new GSGeneratorConfig(false, false, 1, 512);

        public GSOptions Options { get; } = new GSOptions();

        public void Init()
        {
            Options.Add(GSUI.Button("Capture Themes", "Go", CaptureThemes));
            uiList = Options.Add(GSUI.Combobox("Themes Captured", themeNames, themeselect));
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            var random = new GS2.Random(GSSettings.Seed);
            var p = new GSPlanets();
            // var i = 0;
            // foreach (var kvp in ThemeTestLibrary)
            // {
            p.Add(new GSPlanet(themename, themename, 200, random.NextFloat() * 10 + 1, 0, 10000, random.Next(359), 0, 10000, 0, -1));
            //     i++;
            // }

            var s = StarDefaults.Random(random);
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

        private void themeselect(Val o)
        {
            GS2.Warn(o);
            themename = themeNames[o];
        }

        public void CaptureThemes(Val o)
        {
            GS2.Log("Start");
            if (GSSettings.ThemeLibrary == null || GSSettings.ThemeLibrary.Count == 0)
            {
                GS2.Warn("No Themes Captured");
                return;
            }

            ThemeTestLibrary = GSSettings.ThemeLibrary;
            themeNames = ThemeTestLibrary.Select(x => { return x.Key; }).ToList();
            GS2.Log("Updating" + (uiList == null));
            GS2.Warn((uiList.RectTransform == null).ToString());
            uiList.SetItems(themeNames);
            GS2.WarnJson(themeNames);
            GS2.Log("End");
        }
    }
}