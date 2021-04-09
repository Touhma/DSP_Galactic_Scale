using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorPlanetThemeSettings {
        public static Dictionary<PlanetType, Dictionary<PlanetTheme, float>> AllThemeArray;
        public Dictionary<PlanetType, Dictionary<PlanetTheme, float>> AllThemeChances =
            new Dictionary<PlanetType, Dictionary<PlanetTheme, float>> {
                {
                    PlanetType.Internal, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.Birth, 1f }
                    }
                }, {
                    PlanetType.Hot, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.Lava, 0f },
                        { PlanetTheme.VolcanicAsh, 0f }
                    }
                }, {
                    PlanetType.DesertHot, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.AridDesert, 0f }
                    }
                }, {
                    PlanetType.Habitable, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.OceanicJungle, 0f },
                        { PlanetTheme.RedStone, 0f },
                        { PlanetTheme.Prairie, 0f },
                        { PlanetTheme.Ocean, 0f }
                    }
                }, {
                    PlanetType.DesertCold, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.Gelisol, 0f },
                        { PlanetTheme.IceFieldGelisol, 0f }
                    }
                }, {
                    PlanetType.Cold, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.Gobi, 0f },
                    }
                }, {
                    PlanetType.Barren, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.BarrenDesert, 0f }
                    }
                }, {
                    PlanetType.Gas, new Dictionary<PlanetTheme, float>() {
                        { PlanetTheme.RedGasGiant, 0f },
                        { PlanetTheme.YellowGasGiant, 0f },
                        { PlanetTheme.GreenIceGiant, 0f },
                        { PlanetTheme.BlueIceGiant, 0f }
                    }
                }
            };

        public GeneratorPlanetThemeSettings() {
            AllThemeArray = AllThemeChances;
        }
    }
}