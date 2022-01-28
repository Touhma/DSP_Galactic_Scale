namespace GalacticScale
{
    public class GS2PlanetAlgorithm : PlanetAlgorithm
    {
        public PlanetAlgorithm baseAlgorithm;
        public GSPlanet gsPlanet;
        public GSTerrainAlgorithm terrainAlgo;
        public GSVegeAlgorithm vegeAlgo;
        public GSVeinAlgorithm veinAlgo;

        public GS2PlanetAlgorithm(GSPlanet gsPlanet)
        {
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Begin");
            GS2.Log("GS2PlanetAlgorithm|Constructor|CREATING CUSTOM PLANET ALGORITHM FOR " + gsPlanet.Name);
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Selecting Theme " + gsPlanet.Theme);
            var gSTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Selected Theme");
            this.gsPlanet = gsPlanet;
            planet = gsPlanet.planetData;
            seed = gsPlanet.Seed;
            //GS2.Log("GS2PlanetAlgorithm|Constructor|planetRawData exists?=" + (planet.data != null));
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Getting Base Algo");
            baseAlgorithm = GetBaseAlgo(gsTheme.Algo);
            //GS2.Log("GS2PlanetAlgorithm|Constructor|Resetting Base Algo|" + (gsPlanet.planetData == null));
            baseAlgorithm.Reset(gsPlanet.Seed, gsPlanet.planetData);
            // GS2.Log($"GS2PlanetAlgorithm|Constructor|Custom Generation:{gsPlanet.GsTheme.CustomGeneration} Terrain Algo: " + gsTheme.TerrainSettings.Algorithm + " Vein Algo: " + gsTheme.VeinSettings.Algorithm + " Vege Algo: " + gsTheme.VegeSettings.Algorithm);

            if (gsTheme.TerrainSettings.Algorithm == "Vanilla")
            {
                // GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to Vanilla");
                terrainAlgo = (p, modX, modY) =>
                {
                    // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Terrain Algo Running");
                    baseAlgorithm.GenerateTerrain(modX, modY);
                };
            }
            else
            {
                // GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to " + gsTheme.TerrainSettings.Algorithm);
                terrainAlgo = GS2.TerrainAlgorithmLibrary.Find(gsTheme.TerrainSettings.Algorithm);
                //this.}terrainAlgo = (gsTheme.TerrainSettings.Algorithm == "Vanilla") ? (GSPlanet p, double modX, double modY) => { baseAlgorithm.GenerateTerrain(modX, modY); } : 
            }

            if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings())
            {
                if (gsTheme.VeinSettings.Algorithm == "Vanilla")
                    veinAlgo = (p, sketchOnly) =>
                    {
                        // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                        baseAlgorithm.GenerateVeins(sketchOnly);
                    };
                else
                    veinAlgo = GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            }
            else
            {
                if (gsPlanet.veinSettings.Algorithm == "Vanilla")
                    veinAlgo = (p, sketchOnly) =>
                    {
                        // GS2.WarnJson(gsPlanet);
                        // GS2.WarnJson(gsPlanet.GsTheme);
                        // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                        baseAlgorithm.GenerateVeins(sketchOnly);
                    };
                else
                    veinAlgo = GS2.VeinAlgorithmLibrary.Find(gsPlanet.veinSettings.Algorithm);
            }

            //this.veinAlgo = (gsTheme.VeinSettings.Algorithm == "Vanilla") ? (GSPlanet p, bool sketchOnly) => { baseAlgorithm.GenerateVeins(sketchOnly); } : GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            if (gsTheme.VegeSettings.Algorithm == "Vanilla")
            {
                vegeAlgo = p =>
                {
                    // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vege Algo Running");
                    baseAlgorithm.GenerateVegetables();
                };
            }
            else
            {
                // GS2.Log("GS2PlanetAlgorithm|Constructor|GS Vege Algo Running");

                vegeAlgo = GS2.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);
                // GS2.Warn("NonstandardVegealgo");
            }

            //this.vegeAlgo = (gsTheme.VegeSettings.Algorithm == "Vanilla") ? (GSPlanet p) => { 
            //    GS2.Log("GS2.VeinAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);")
            //    baseAlgorithm.GenerateVegetables(); 
            //} : GS2.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);


            //GS2.Log("GS2PlanetAlgorithm|Constructor|End");
        }

        public GSTheme gsTheme => GSSettings.ThemeLibrary.Find(gsPlanet.Theme);

        public override void GenerateTerrain(double modX, double modY)
        {
            //GS2.Log("PlanetAlgorithm|GenerateTerrain|" + gsPlanet.Name);
            terrainAlgo(gsPlanet, modX, modY); //GS2.Log("PlanetAlgorithm|GenerateTerrain|End");
        }

        public override void GenerateVegetables()
        {
            //GS2.Log("PlanetAlgorithm|GenerateVegetables()");
            vegeAlgo(gsPlanet);
        }

        public override void GenerateVeins(bool sketchOnly)
        {
            // GS2.Log($"PlanetAlgorithm|GenerateVeins() for {gsPlanet.Name} {gsPlanet.Theme}");
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
                case 8: return new PlanetAlgorithm8();
                case 9: if (GS2.Config.ScarletRevert) return new PlanetAlgorithm0(); 
                        return new PlanetAlgorithm9();
            }

            return new PlanetAlgorithm0();
        }
    }
}