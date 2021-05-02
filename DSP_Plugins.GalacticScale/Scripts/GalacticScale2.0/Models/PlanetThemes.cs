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
            },
            ["Gas2"] = new GSTheme()
            {
                name = "Gas2",
                type = EPlanetType.Gas,
                LDBThemeId = 3,
                algo = 1
            },
            ["IceGiant"] = new GSTheme()
            {
                name = "IceGiant",
                type = EPlanetType.Gas,
                LDBThemeId = 4,
                algo = 1
            },
            ["IceGiant2"] = new GSTheme()
            {
                name = "IceGiant2",
                type = EPlanetType.Gas,
                LDBThemeId = 5,
                algo = 1
            },
            ["Arid"] = new GSTheme()
            {
                name ="Arid",
                type = EPlanetType.Desert,
                LDBThemeId = 6,
                algo = 2,
            },
            ["AshenGelisol"] = new GSTheme()
            {
                name = "AshenGelisol",
                type = EPlanetType.Desert,
                LDBThemeId = 7,
                algo = 1,
            },
            ["Jungle"] = new GSTheme()
            {
                name = "Jungle",
                type = EPlanetType.Ocean,
                LDBThemeId = 8,
                algo = 1,
            },
            ["Lava"] = new GSTheme()
            {
                name = "Lava",
                type = EPlanetType.Vocano,
                LDBThemeId = 9,
                algo = 5,
            },
            ["Ice"] = new GSTheme()
            {
                name = "Ice",
                type = EPlanetType.Desert,
                LDBThemeId = 10,
                algo = 3,
            },
            ["Barren"] = new GSTheme()
            {
                name = "Barren",
                type = EPlanetType.Desert,
                LDBThemeId = 11,
                algo = 4,
            },
            ["Gobi"] = new GSTheme()
            {
                name = "Gobi",
                type = EPlanetType.Desert,
                LDBThemeId = 12,
                algo = 3,
            },
            ["VolcanicAsh"] = new GSTheme()
            {
                name = "VolcanicAsh",
                type = EPlanetType.Vocano,
                LDBThemeId = 13,
                algo = 3,
            },
            ["RedStone"] = new GSTheme()
            {
                name = "RedStone",
                type = EPlanetType.Ocean,
                LDBThemeId = 14,
                algo = 1,
            },
            ["Prarie"] = new GSTheme()
            {
                name = "Prarie",
                type = EPlanetType.Ocean,
                LDBThemeId = 15,
                algo = 6,
            },
            ["Ocean"] = new GSTheme()
            {
                name = "Ocean",
                type = EPlanetType.Ocean,
                LDBThemeId = 16,
                algo = 7,
            },
            ["Test"] = new GSTheme()
            {
                name = "Test",
                type = EPlanetType.Ocean,
                LDBThemeId = 9,
                algo = 7,
            },
        };
    }
}