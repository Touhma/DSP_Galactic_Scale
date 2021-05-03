using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static Dictionary<string, GSTheme> planetThemes = new Dictionary<string, GSTheme>()
        {
            ["Mediterranean"] = Themes.Mediterranean,
            ["Mediterranian"] = Themes.Mediterranean,
            ["Gas"] = Themes.Gas,
            ["Gas2"] = Themes.Gas2,
            ["IceGiant"] = Themes.IceGiant,
            ["IceGiant2"] = Themes.IceGiant2,
            ["Arid"] = Themes.AridDesert,
            ["AridDesert"] = Themes.AridDesert,
            ["AshenGelisol"] = Themes.AshenGelisol,
            ["Jungle"] = Themes.OceanicJungle,
            ["OceanicJungle"] = Themes.OceanicJungle,
            ["Lava"] = Themes.Lava,
            ["Ice"] = Themes.IceGelisol,
            ["IceGelisol"] = Themes.IceGelisol,
            ["Barren"] = Themes.Barren,
            ["BarrenDesert"] = Themes.Barren,
            ["Gobi"] = Themes.Gobi,
            ["VolcanicAsh"] = Themes.VolcanicAsh,
            ["RedStone"] = Themes.RedStone,
            ["Prarie"] = Themes.Prarie,
            ["Ocean"] = Themes.OceanWorld,
            ["OceanWorld"] = Themes.OceanWorld,
            ["Test"] = new GSTheme("Lava")
            {
                name = "Test",
                type = EPlanetType.Vocano,
                algo = 7,
                terrainTint = UnityEngine.Color.green,
                oceanTint = UnityEngine.Color.magenta
            },
        };
    }
}