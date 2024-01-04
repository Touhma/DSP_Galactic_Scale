namespace GalacticScale
{
    public class GS3PlanetAlgorithm : PlanetAlgorithm
    {
        public PlanetAlgorithm baseAlgorithm;
        public GSPlanet gsPlanet;
        public GSTerrainAlgorithm terrainAlgo;
        public GSVegeAlgorithm vegeAlgo;
        public GSVeinAlgorithm veinAlgo;

        public GS3PlanetAlgorithm(GSPlanet gsPlanet)
        {
            //GS3.Log("GS3PlanetAlgorithm|Constructor|Begin");
            // GS3.Log("GS3PlanetAlgorithm|Constructor|CREATING CUSTOM PLANET ALGORITHM FOR " + gsPlanet.Name);
            // GS3.Log("GS3PlanetAlgorithm|Constructor|Selecting Theme " + gsPlanet.Theme);
            var gSTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            // GS3.Log("GS3PlanetAlgorithm|Constructor|Selected Theme");
            this.gsPlanet = gsPlanet;
            planet = gsPlanet.planetData;
            seed = gsPlanet.Seed;
            //GS3.Log("GS3PlanetAlgorithm|Constructor|planetRawData exists?=" + (planet.data != null));
            // GS3.Log("GS3PlanetAlgorithm|Constructor|Getting Base Algo");
            baseAlgorithm = GetBaseAlgo(gsTheme.Algo);
            // GS3.Log("GS3PlanetAlgorithm|Constructor|Resetting Base Algo|" + (gsPlanet.planetData == null));
            baseAlgorithm.Reset(gsPlanet.Seed, gsPlanet.planetData);
            // GS3.Log($"GS3PlanetAlgorithm|Constructor|Custom Generation:{gsPlanet.GsTheme.CustomGeneration} Terrain Algo: " + gsTheme.TerrainSettings.Algorithm + " Vein Algo: " + gsTheme.VeinSettings.Algorithm + " Vege Algo: " + gsTheme.VegeSettings.Algorithm);

            if (gsTheme.TerrainSettings.Algorithm == "Vanilla")
                // GS3.Log("GS3PlanetAlgorithm|Constructor|Terrain Algo Being Set to Vanilla");
                terrainAlgo = (p, modX, modY) =>
                {
                    // GS3.Log("GS3PlanetAlgorithm|Constructor|Vanilla Terrain Algo Running");
                    if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) baseAlgorithm.GenerateTerrain(modX, modY);
                };
            else
                // GS3.Log("GS3PlanetAlgorithm|Constructor|Terrain Algo Being Set to " + gsTheme.TerrainSettings.Algorithm);
                terrainAlgo = GS3.TerrainAlgorithmLibrary.Find(gsTheme.TerrainSettings.Algorithm);
            //this.}terrainAlgo = (gsTheme.TerrainSettings.Algorithm == "Vanilla") ? (GSPlanet p, double modX, double modY) => { baseAlgorithm.GenerateTerrain(modX, modY); } : 

            if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings())
            {
                // if (gsTheme.VeinSettings.Algorithm == "Vanilla")
                //     veinAlgo = p => //, sketchOnly) =>
                //     {
                //         GS3.Log("GS3PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                //         if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) baseAlgorithm.GenerateVeins();
                //     };
                // else
                // GS3.Log($"Selecting vein algo {gsTheme.VeinSettings.Algorithm} for {gsPlanet.Name}");
                    veinAlgo = GS3.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            }
            else
            {
                // if (gsPlanet.veinSettings.Algorithm == "Vanilla")
                //     veinAlgo = p => //, sketchOnly) =>
                //     {
                //         // GS3.WarnJson(gsPlanet);
                //         // GS3.WarnJson(gsPlanet.GsTheme);
                //         GS3.Log("GS3PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                //         if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) baseAlgorithm.GenerateVeins();
                //     };
                // else
                // GS3.Log($"Selecting vein algo {gsTheme.VeinSettings.Algorithm} for {gsPlanet.Name}");
                    veinAlgo = GS3.VeinAlgorithmLibrary.Find(gsPlanet.veinSettings.Algorithm);
            }

            //this.veinAlgo = (gsTheme.VeinSettings.Algorithm == "Vanilla") ? (GSPlanet p, bool sketchOnly) => { baseAlgorithm.GenerateVeins(sketchOnly); } : GS3.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            if (gsTheme.VegeSettings.Algorithm == "Vanilla")
                vegeAlgo = p =>
                {
                    // GS3.Log("GS3PlanetAlgorithm|Constructor|Vanilla Vege Algo Running");
                    if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.vegeIds != null) baseAlgorithm.GenerateVegetables();
                };
            else
                // GS3.Log("GS3PlanetAlgorithm|Constructor|GS Vege Algo Running");

                vegeAlgo = GS3.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);
            // GS3.Warn("NonstandardVegealgo");

            //this.vegeAlgo = (gsTheme.VegeSettings.Algorithm == "Vanilla") ? (GSPlanet p) => { 
            //    GS3.Log("GS3.VeinAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);")
            //    baseAlgorithm.GenerateVegetables(); 
            //} : GS3.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);


            //GS3.Log("GS3PlanetAlgorithm|Constructor|End");
        }

        public GSTheme gsTheme => GSSettings.ThemeLibrary.Find(gsPlanet.Theme);

        public override void GenerateTerrain(double modX, double modY)
        {
            //GS3.Log("PlanetAlgorithm|GenerateTerrain|" + gsPlanet.Name);
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    terrainAlgo(gsPlanet, modX, modY); //GS3.Log("PlanetAlgorithm|GenerateTerrain|End");
        }

        public override void GenerateVegetables()
        {
            //GS3.Log("PlanetAlgorithm|GenerateVegetables()");
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    vegeAlgo(gsPlanet);
        }

        public override void GenerateVeins()
        {
            // GS3.Log($"PlanetAlgorithm|GenerateVeins() for {gsPlanet.Name} {gsPlanet.Theme}");
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    veinAlgo(gsPlanet); //, false);
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
                case 9 when GS3.Config.ScarletRevert: return new PlanetAlgorithm0();
                case 9 when !GS3.Config.ScarletRevert: return new PlanetAlgorithm9();
                case 10: return new PlanetAlgorithm10();
                case 11: return new PlanetAlgorithm11();
                case 12: return new PlanetAlgorithm12();
                case 13: return new PlanetAlgorithm13();
                case 14: return new PlanetAlgorithm14();
            }

            return new PlanetAlgorithm0();
        }
    }
}