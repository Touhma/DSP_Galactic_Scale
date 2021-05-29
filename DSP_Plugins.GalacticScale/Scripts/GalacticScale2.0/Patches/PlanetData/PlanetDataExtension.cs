namespace GalacticScale {
    public static class PlanetDataExtension {
        public static float GetScaleFactored(this PlanetData planet) {
            if (planet.type == EPlanetType.Gas) return planet.radius / 80;
            return planet.radius / 200;
        }
    }
}