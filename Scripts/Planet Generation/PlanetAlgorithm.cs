namespace GalacticScale
{
    using UnityEngine;

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
            // GS2.Log("GS2PlanetAlgorithm|Constructor|CREATING CUSTOM PLANET ALGORITHM FOR " + gsPlanet.Name);
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Selecting Theme " + gsPlanet.Theme);
            var gSTheme = GSSettings.ThemeLibrary.Find(gsPlanet.Theme);
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Selected Theme");
            this.gsPlanet = gsPlanet;
            planet = gsPlanet.planetData;
            seed = gsPlanet.Seed;
            //GS2.Log("GS2PlanetAlgorithm|Constructor|planetRawData exists?=" + (planet.data != null));
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Getting Base Algo");
            baseAlgorithm = GetBaseAlgo(gsTheme.Algo);
            // GS2.Log("GS2PlanetAlgorithm|Constructor|Resetting Base Algo|" + (gsPlanet.planetData == null));
            baseAlgorithm.Reset(gsPlanet.Seed, gsPlanet.planetData);
            // GS2.Log($"GS2PlanetAlgorithm|Constructor|Custom Generation:{gsPlanet.GsTheme.CustomGeneration} Terrain Algo: " + gsTheme.TerrainSettings.Algorithm + " Vein Algo: " + gsTheme.VeinSettings.Algorithm + " Vege Algo: " + gsTheme.VegeSettings.Algorithm);

            // FIXED: Force ALL large planets to use custom terrain generation
            if (gsPlanet.Radius > 200)
            {
                GS2.Log($"Large planet {gsPlanet.Name} (radius {gsPlanet.Radius}) - forcing GenerateTerrainLarge regardless of theme algorithm");
                terrainAlgo = (p, modX, modY) =>
                {
                    if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) 
                    {
                        TerrainAlgorithms.GenerateTerrainLarge(gsPlanet, modX, modY);
                    }
                };
            }
            else if (gsTheme.TerrainSettings.Algorithm == "Vanilla")
                // GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to Vanilla");
                terrainAlgo = (p, modX, modY) =>
                {
                    // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Terrain Algo Running");
                    if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) 
                    {
                        baseAlgorithm.GenerateTerrain(modX, modY);
                    }
                };
            else
                // GS2.Log("GS2PlanetAlgorithm|Constructor|Terrain Algo Being Set to " + gsTheme.TerrainSettings.Algorithm);
                terrainAlgo = GS2.TerrainAlgorithmLibrary.Find(gsTheme.TerrainSettings.Algorithm);
            //this.}terrainAlgo = (gsTheme.TerrainSettings.Algorithm == "Vanilla") ? (GSPlanet p, double modX, double modY) => { baseAlgorithm.GenerateTerrain(modX, modY); } : 

            if (gsPlanet.veinSettings == null || gsPlanet.veinSettings == new GSVeinSettings())
            {
                // if (gsTheme.VeinSettings.Algorithm == "Vanilla")
                //     veinAlgo = p => //, sketchOnly) =>
                //     {
                //         GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                //         if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) baseAlgorithm.GenerateVeins();
                //     };
                // else
                // GS2.Log($"Selecting vein algo {gsTheme.VeinSettings.Algorithm} for {gsPlanet.Name}");
                    veinAlgo = GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            }
            else
            {
                // if (gsPlanet.veinSettings.Algorithm == "Vanilla")
                //     veinAlgo = p => //, sketchOnly) =>
                //     {
                //         // GS2.WarnJson(gsPlanet);
                //         // GS2.WarnJson(gsPlanet.GsTheme);
                //         GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vein Algo Running");
                //         if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.heightData != null) baseAlgorithm.GenerateVeins();
                //     };
                // else
                // GS2.Log($"Selecting vein algo {gsTheme.VeinSettings.Algorithm} for {gsPlanet.Name}");
                    veinAlgo = GS2.VeinAlgorithmLibrary.Find(gsPlanet.veinSettings.Algorithm);
            }

            //this.veinAlgo = (gsTheme.VeinSettings.Algorithm == "Vanilla") ? (GSPlanet p, bool sketchOnly) => { baseAlgorithm.GenerateVeins(sketchOnly); } : GS2.VeinAlgorithmLibrary.Find(gsTheme.VeinSettings.Algorithm);
            if (gsTheme.VegeSettings.Algorithm == "Vanilla")
                vegeAlgo = p =>
                {
                    // GS2.Log("GS2PlanetAlgorithm|Constructor|Vanilla Vege Algo Running");
                    if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null && gsPlanet.planetData.data.vegeIds != null) baseAlgorithm.GenerateVegetables();
                };
            else
                // GS2.Log("GS2PlanetAlgorithm|Constructor|GS Vege Algo Running");

                vegeAlgo = GS2.VegeAlgorithmLibrary.Find(gsTheme.VegeSettings.Algorithm);
            // GS2.Warn("NonstandardVegealgo");

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
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                {
                    // DIAGNOSTIC LOGGING: Identify which terrain algorithm is being used
                    var algo = gsPlanet.GsTheme?.TerrainSettings?.Algorithm ?? "unknown";
                    GS2.Log($"GenerateTerrain START: planet={gsPlanet.Name}, radius={gsPlanet.Radius}, " +
                           $"precision={gsPlanet.planetData.precision}, scale={gsPlanet.planetData.scale:F2}, " +
                           $"segment={gsPlanet.planetData.segment}, algo={algo}");
                    
                    // Ensure full precision heightData is allocated before terrain generation
                    gsPlanet.planetData.data.AddFactoredRadius(gsPlanet.planetData);
                    terrainAlgo(gsPlanet, modX, modY); //GS2.Log("PlanetAlgorithm|GenerateTerrain|End");
                    
                    GS2.Log($"GenerateTerrain END: planet={gsPlanet.Name}");
                }
        }

        public override void GenerateVegetables()
        {
            //GS2.Log("PlanetAlgorithm|GenerateVegetables()");
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    vegeAlgo(gsPlanet);
        }

        public override void GenerateVeins()
        {
            // GS2.Log($"PlanetAlgorithm|GenerateVeins() for {gsPlanet.Name} {gsPlanet.Theme}");
            if (gsPlanet != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    veinAlgo(gsPlanet); //, false);
        }

        public override void CalcWaterPercent()
        {
            // Simply delegate to the base algorithm's CalcWaterPercent method
            if (gsPlanet != null && baseAlgorithm != null)
                if (!UIRoot.instance.backToMainMenu && gsPlanet.planetData != null && gsPlanet.planetData.data != null)
                    baseAlgorithm.CalcWaterPercent();
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
                case 9 when GS2.Config.ScarletRevert: return new PlanetAlgorithm0();
                case 9 when !GS2.Config.ScarletRevert: return new PlanetAlgorithm9();
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