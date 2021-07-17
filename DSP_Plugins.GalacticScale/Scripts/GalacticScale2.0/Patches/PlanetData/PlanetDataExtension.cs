namespace GalacticScale
{
    public static class PlanetDataExtension
    {
        public static float GetScaleFactored(this PlanetData planet)
        {
            if (planet == null)
            {
                GS2.Error("Trying to get factored scale while planet is null");
                return 1f;
            }

            if (planet.type == EPlanetType.Gas) return planet.radius / 80;

            return planet.radius / 200;
        }
    }
}