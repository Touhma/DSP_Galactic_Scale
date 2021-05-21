namespace GalacticScale
{
	public static class PatchOnPlanetModellingManager
	{
		public static bool Algorithm(PlanetData planet, PlanetAlgorithm __result)
		{
            GSPlanet gsPlanet = GS2.GetGSPlanet(planet);
			if (!GS2.ThemeLibrary[gsPlanet.Theme].CustomGeneration) return true;
            __result = new GS2PlanetAlgorithm(gsPlanet);
			return false;
		}
	}
    
}