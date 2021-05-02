using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static Dictionary<string, GSTheme> planetThemes = new Dictionary<string, GSTheme>()
        {
            ["Mediterranian"] = new GSTheme()
            {
                name = "Mediterranian",
                type = EPlanetType.Ocean,
                LDBThemeId = 1,
                algo = 0,
            },
            ["Gas"] = new GSTheme()
            {
                name = "Gas",
                type = EPlanetType.Gas,
                LDBThemeId = 2,
                algo = 1
            }
        };
    }
}