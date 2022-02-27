namespace GalacticScale
{
    public static partial class VeinAlgorithms
    {
        public static void GenerateVeinsGS2W(GSPlanet gsPlanet, bool sketchOnly)
        {
            // GS2.Warn($"Using GS2W for {gsPlanet.Name}");
            random = new GS2.Random(gsPlanet.Seed);
            InitializeFromVeinSettings(gsPlanet);
            if (GSSettings.BirthPlanet == gsPlanet && !sketchOnly)
                GenBirthPoints(gsPlanet);
            AddSpecialVeins(gsPlanet);
            gsPlanet.veinData.Clear();
            if (sketchOnly) return;
            if (GSSettings.BirthPlanet == gsPlanet) InitBirthVeinVectors(gsPlanet);
            AddVeinsToPlanetGS2(gsPlanet, CalculateVectorsGS2(gsPlanet, false, false));
        }
    }
}