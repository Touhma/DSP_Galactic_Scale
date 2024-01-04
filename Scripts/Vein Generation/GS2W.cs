namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsGS3W(GSPlanet gsPlanet) 
        {
            random = new GS3.Random(gsPlanet.Seed);
            
            InitializeFromVeinSettings(gsPlanet);
            if (GSSettings.BirthPlanet == gsPlanet) GenBirthPoints(gsPlanet);
            
            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            
            if (GSSettings.BirthPlanet == gsPlanet) InitBirthVeinVectors(gsPlanet);
            AddVeinsToPlanetGS3(gsPlanet, CalculateVectorsGS3(gsPlanet, false, false));
        }
    }
}