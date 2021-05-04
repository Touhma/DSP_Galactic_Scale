using System.Collections.Generic;

namespace GalacticScale
{
    public class GSThemeLibrary : Dictionary<string, GSTheme>
    {
        public GSThemeLibrary()
        {
        }
    }
    public static partial class GS2
    {
        public static GSThemeLibrary ThemeLibrary = new GSThemeLibrary()
        {
            ["Mediterranean"] = Themes.Mediterranean,
            ["GasGiant"] = Themes.Gas,
            ["GasGiant2"] = Themes.Gas2,
            ["IceGiant"] = Themes.IceGiant,
            ["IceGiant2"] = Themes.IceGiant2,
            ["AridDesert"] = Themes.AridDesert,
            ["AshenGelisol"] = Themes.AshenGelisol,
            ["Jungle"] = Themes.OceanicJungle,
            ["OceanicJungle"] = Themes.OceanicJungle,
            ["Lava"] = Themes.Lava,
            ["IceGelisol"] = Themes.IceGelisol,
            ["BarrenDesert"] = Themes.Barren,
            ["Gobi"] = Themes.Gobi,
            ["VolcanicAsh"] = Themes.VolcanicAsh,
            ["RedStone"] = Themes.RedStone,
            ["Prairie"] = Themes.Prairie,
            ["OceanWorld"] = Themes.OceanWorld
        };
    }
}