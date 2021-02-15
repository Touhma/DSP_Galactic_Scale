namespace GalacticScale.Scripts.PatchStarSystemGeneration.Utils {
    public static class GalaxyInfoUtils {
        public static int NumberOfPlanetInGalaxy(StarData[] stars) {
            
            int result = 0;
            foreach (var starData in stars) {
                result += starData.planetCount;
            }

            return result;
        }
    }
}