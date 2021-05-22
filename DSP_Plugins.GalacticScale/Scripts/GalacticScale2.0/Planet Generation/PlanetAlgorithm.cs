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
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Begin");
            //GS2.Log("GS2PlanetAlgorithm|Constructor|CREATING CUSTOM PLANET ALGORITHM FOR " + gsPlanet.Name);
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Selecting Theme " + gsPlanet.Theme);
            GSTheme gSTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Selected Theme");
            this.gsPlanet = gsPlanet;
            this.planet = gsPlanet.planetData;
            this.seed = gsPlanet.Seed;
            //GS2.Log("GS2PlanetAlgorithm|Constructor|planetRawData exists?=" + (planet.data != null));
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Getting Base Algo");
            this.baseAlgorithm = GetBaseAlgo(gsTheme.Algo);
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Resetting Base Algo|" + (gsPlanet.planetData == null));
            baseAlgorithm.Reset(gsPlanet.Seed, gsPlanet.planetData);
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo: " + gsTheme.TerrainSettings.Algorithm + " Vein Algo: " + gsTheme.VeinSettings.Algorithm + " Vege Algo: " + gsTheme.VegeSettings.Algorithm);

            if (gsTheme.TerrainSettings.Algorithm == "Vanilla")
            {
                //GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to Vanilla");
                this.terrainAlgo = (GSPlanet p, double modX, double modY) =>
                {
                    //GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Terrain Algo Running");
                    baseAlgorithm.GenerateTerrain(modX, modY);
                };
            }
            else
            {
                //GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to " + gsTheme.TerrainSettings.Algorithm);
                this.terrainAlgo = GS2.TerrainAlgorithmLibrary.Find(gsTheme.TerrainSettings.Algorithm);
            }
            //this.terrainAlgo = (gsTheme.TerrainSettings.Algorithm == "Vanilla") ? (GSPlanet p, double modX, double modY) => { baseAlgorithm.GenerateTerrain(modX, modY); } : 

            if (gsTheme.VeinSettings.Algorithm == "Vanilla")
            {
                this.veinAlgo = (GSPlanet p, bool sketchOnly) =>
                {
                    //GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                    baseAlgorithm.GenerateVeins(sketchOnly);
                };
            }
            else
            {
                this.veinAlgo = GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            }
            //this.veinAlgo = (gsTheme.VeinSettings.Algorithm == "Vanilla") ? (GSPlanet p, bool sketchOnly) => { baseAlgorithm.GenerateVeins(sketchOnly); } : GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            if (gsTheme.VegeSettings.Algorithm == "Vanilla")
            {
                this.vegeAlgo = (GSPlanet p) =>
                {
                    //GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vege Algo Running");
                    baseAlgorithm.GenerateVegetables();
                };
            }
            else
            {
                this.vegeAlgo = GS2.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);
            }

            //this.vegeAlgo = (gsTheme.VegeSettings.Algorithm == "Vanilla") ? (GSPlanet p) => { 
            //    GS2.Log("GS2.VeinAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);")
            //    baseAlgorithm.GenerateVegetables(); 
            //} : GS2.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);


            //GS2.Log("GS2PlanetAlgorithm|Constructor|End");
        }
        public override void GenerateTerrain(double modX, double modY)
        {
            //GS2.Log("PlanetAlgorithm|GenerateTerrain|" + gsPlanet.Name);
            terrainAlgo(gsPlanet, modX, modY);
            //GS2.Log("PlanetAlgorithm|GenerateTerrain|End");
        }

        public override void GenerateVegetables()
        {
            //GS2.Log("PlanetAlgorithm|GenerateVegetables()");
            vegeAlgo(gsPlanet);
        }

        public override void GenerateVeins(bool sketchOnly)
        {
            //GS2.Log("PlanetAlgorithm|GenerateVeins()");
            veinAlgo(gsPlanet, sketchOnly);
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