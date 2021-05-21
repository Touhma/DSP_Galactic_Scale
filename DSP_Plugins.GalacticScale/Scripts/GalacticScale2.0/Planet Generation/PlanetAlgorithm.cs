namespace GalacticScale
{
    public class GS2PlanetAlgorithm : PlanetAlgorithm
    {
        public GSPlanet gsPlanet;
        public GSTerrainAlgorithm terrainAlgo;
        public GSVeinAlgorithm veinAlgo;
        public GSVegeAlgorithm vegeAlgo;
        public PlanetAlgorithm baseAlgorithm;
        public GSTheme gsTheme { get => GS2.ThemeLibrary[gsPlanet.Theme]; }
        public GS2PlanetAlgorithm(GSPlanet gsPlanet)
        {
            this.gsPlanet = gsPlanet;
            this.baseAlgorithm = GetBaseAlgo(GS2.ThemeLibrary[gsPlanet.Theme].Algo);
            this.terrainAlgo = GS2.TerrainAlgorithmLibrary[gsTheme.TerrainSettings.Algorithm];
            this.veinAlgo = GS2.VeinAlgorithmLibrary[gsTheme.VeinSettings.Algorithm];
            this.vegeAlgo = GS2.VegeAlgorithmLibrary[gsTheme.VegeSettings.Algorithm];
        }
        public override void GenerateTerrain(double modX, double modY) => terrainAlgo(gsPlanet, modX, modY);

        public override void GenerateVegetables()
        {

        }
        public override void GenerateVeins(bool sketchOnly)
        {
            base.GenerateVeins(sketchOnly);
        }
        public static PlanetAlgorithm GetBaseAlgo(int algoId)
        {
            switch (algoId)
            {
                case 1: return new PlanetAlgorithm1();
                case 2: return new PlanetAlgorithm2();
                case 3: return new PlanetAlgorithm3();
                case 4: return new PlanetAlgorithm4();
                case 5: return new PlanetAlgorithm5();
                case 6: return new PlanetAlgorithm6();
                case 7: return new PlanetAlgorithm7();
            }
            return new PlanetAlgorithm0();
        }
    }
}