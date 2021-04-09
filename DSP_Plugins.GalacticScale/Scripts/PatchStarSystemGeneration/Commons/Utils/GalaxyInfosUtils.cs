namespace GalacticScale.Scripts {
    public static class GalaxyInfoUtils {
        public static int NumberOfPlanetInGalaxy(StarData[] stars) {
            var result = 0;
            foreach (var starData in stars) result += starData.planetCount;

            return result;
        }
    }
}