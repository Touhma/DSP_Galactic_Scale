namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref  StarData star, GameDesc gameDesc)
        {
            Log("Start|" + star.name);
            GSSettings.Stars[star.index].counter = 0;
            star.planets = new PlanetData[GSSettings.Stars[star.index].bodyCount];
            for (var i = 0; i < GSSettings.Stars[star.index].planetCount; i++) CreatePlanet(ref star, GSSettings.Stars[star.index].Planets[i], null);
            Log("End|" + star.name);
        }
    }
}