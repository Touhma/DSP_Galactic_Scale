using System.Collections.Generic;

namespace GalacticScale.Scripts {
   public static class PlanetThemeExtensions {
        public static Dictionary<PlanetType, Dictionary<PlanetTheme, int>> AllTheme =
            new Dictionary<PlanetType, Dictionary<PlanetTheme, int>> {
                {
                    PlanetType.Internal, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.Birth, 1 }
                    }
                }, {
                    PlanetType.Hot, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.Lava, 9 },
                        { PlanetTheme.VolcanicAsh, 13 }
                    }
                }, {
                    PlanetType.DesertHot, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.AridDesert, 6 }
                    }
                }, {
                    PlanetType.Habitable, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.OceanicJungle, 8 },
                        { PlanetTheme.RedStone, 14 },
                        { PlanetTheme.Prairie, 15 },
                        { PlanetTheme.Ocean, 16 }
                    }
                }, {
                    PlanetType.DesertCold, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.Gelisol, 7 },
                        { PlanetTheme.IceFieldGelisol, 10 }
                    }
                }, {
                    PlanetType.Cold, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.Gobi, 12 },
                    }
                }, {
                    PlanetType.Barren, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.BarrenDesert, 11 }
                    }
                }, {
                    PlanetType.Gas, new Dictionary<PlanetTheme, int>() {
                        { PlanetTheme.RedGasGiant, 8 },
                        { PlanetTheme.YellowGasGiant, 14 },
                        { PlanetTheme.GreenIceGiant, 15 },
                        { PlanetTheme.BlueIceGiant, 16 }
                    }
                }
            };

        public static Dictionary<PlanetTheme, int> GetInternalThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Internal];
        }
        public static Dictionary<PlanetTheme, int> GetHotThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Hot];
        }
        public static Dictionary<PlanetTheme, int> GetColdThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Cold];
        }
        public static Dictionary<PlanetTheme, int> GetDesertHotThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.DesertHot];
        }
        public static Dictionary<PlanetTheme, int> GetDesertColdThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Cold];
        }
        public static Dictionary<PlanetTheme, int> GetBarrenThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Barren];
        }
        public static Dictionary<PlanetTheme, int> GetHabitableThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Habitable];
        }
        public static Dictionary<PlanetTheme, int> GetGasGiantThemes(this PlanetTheme theme) {
            return AllTheme[PlanetType.Gas];
        }
        public static Dictionary<PlanetTheme, int> GetAllDesertsThemes(this PlanetTheme theme) {
            Dictionary<PlanetTheme, int> returnValue = new Dictionary<PlanetTheme, int>();
            foreach (var keyValuePair in AllTheme[PlanetType.DesertHot]) {
                returnValue.Add(keyValuePair.Key,keyValuePair.Value);
            }
            foreach (var keyValuePair in AllTheme[PlanetType.DesertCold]) {
                returnValue.Add(keyValuePair.Key,keyValuePair.Value);
            }
            return returnValue;
        }
    }
}