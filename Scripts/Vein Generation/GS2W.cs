namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsGS2W(GSPlanet gsPlanet) 
        {
            random = new GS2.Random(gsPlanet.Seed);
            
            InitializeFromVeinSettings(gsPlanet);
            if (GSSettings.BirthPlanet == gsPlanet) GenBirthPoints(gsPlanet);
            
            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            
            if (GSSettings.BirthPlanet == gsPlanet) InitBirthVeinVectors(gsPlanet);
            AddVeinsToPlanetGS2(gsPlanet, CalculateVectorsGS2(gsPlanet, false, false));
        }
    }
}