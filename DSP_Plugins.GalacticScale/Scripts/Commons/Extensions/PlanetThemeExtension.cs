using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public static class PlanetThemeExtensions {
        public static Dictionary<PlanetTheme, float> GetInternalThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Internal];
        }
        public static Dictionary<PlanetTheme, float> GetHotThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Hot];
        }
        public static Dictionary<PlanetTheme, float> GetColdThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Cold];
        }
        public static Dictionary<PlanetTheme, float> GetDesertHotThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.DesertHot];
        }
        public static Dictionary<PlanetTheme, float> GetDesertColdThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Cold];
        }
        public static Dictionary<PlanetTheme, float> GetBarrenThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Barren];
        }
        public static Dictionary<PlanetTheme, float> GetHabitableThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Habitable];
        }
        public static Dictionary<PlanetTheme, float> GetGasGiantThemes(this PlanetTheme theme) {
            return GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.Gas];
        }
        public static Dictionary<PlanetTheme, float> GetAllDesertsThemes(this PlanetTheme theme) {
            Dictionary<PlanetTheme, float> returnValue = new Dictionary<PlanetTheme, float>();
            foreach (var keyValuePair in GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.DesertHot]) {
                returnValue.Add(keyValuePair.Key, keyValuePair.Value);
            }
            foreach (var keyValuePair in GeneratorPlanetThemeSettings.AllThemeArray[PlanetType.DesertCold]) {
                returnValue.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return returnValue;
        }
    }
}