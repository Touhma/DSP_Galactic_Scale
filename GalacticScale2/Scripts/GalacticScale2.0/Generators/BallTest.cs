namespace GalacticScale.Generators
{
    public class BallTest : iConfigurableGenerator
    {
        public string Name => "BallTest";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.ball";

        public GSGeneratorConfig Config => new GSGeneratorConfig(false, false, 1, 512);

        public GSOptions Options { get; } = new GSOptions();

        //private GSStars stars = new GSStars();
        public void Init()
        {
            //List<string> genList = new List<string>();
            //foreach (iGenerator g in GS2.generators) genList.Add(g.Name);
            //options.Add(new GSOption("Dryrun Generator", "ComboBox", genList, OnDryRunChange, () => { }));
            //options.Add(new GSOption("Output Settings", "Button", "Output", OnOutputSettingsClick, () => { }));
            //options.Add(new GSOption("Output StarData", "Button", "Output", OnOutputStarDataClick, () => { }));
            //options.Add(new GSOption("Output LDBThemes", "Button", "Output", OnDumpPlanetDataClick, () => { }));
            //options.Add(new GSOption("Output Theme Library", "Button", "Output", OnDumpThemesDataClick, () => { })); 
            //options.Add(new GSOption("Import Positions", "Button", "Import", OnImportPositionsClick, () => { }));
            //OnImportPositionsClick(null);
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
            //starCount = 2048;
            var random = new GS2.Random(GSSettings.Seed);
            //List<GSPlanet> p = new List<GSPlanet>();
            //for (var j = 0; j < 20; j++)
            //{
            //GSTheme modified = new GSTheme("modified" + j, "modified" + j, "RedStone");
            //    modified.Algo = 5;
            //    //modified.TerrainSettings.heightMulti = (j*modifier)-(5*modifier);
            //    //modified.TerrainSettings.baseHeight = (j * modifier * -100);
            //    modified.TerrainSettings.brightnessFix = false;
            //modified.TerrainSettings.terrainAlgorithm = "GS2";
            //    modified.Process();
            //}
            var lmodified = new GSTheme("modifiedl", "modifiedl", "RedStone");
            //lmodified.Algo = 5;
            //lmodified.TerrainSettings.brightnessFix = false;
            //modified.TerrainSettings.heightMulti = (j * modifier) - (5 * modifier);
            //modified.TerrainSettings.baseHeight = (j * modifier * -100);
            //lmodified.TerrainSettings.Algorithm = "GSTA6";
            //lmodified.VeinSettings.Algorithm = "GS2";
            //lmodified.CustomGeneration = true;
            lmodified.Process();
            //GS2.Warn("CUBEMAP: " + lmodified.ambientDesc.reflectionMap.name);
            for (var i = 0; i < starCount; i++)
            {
                var s = StarDefaults.Random(random);
                s.Name = "Star-" + i;
                if (i < 10)
                {
                    s.Planets.Add(new GSPlanet("redstone", "RedStone", 50, 0.5f, -1, -1, 1, -1, -1, -1, 1f));
                    s.Planets.Add(new GSPlanet("redstone2", "RedStone", 50, 0.5f, -1, -1, 10, -1, -1, -1, 1f));
                    s.Planets.Add(new GSPlanet("redstone3", "modifiedl", 50, 0.5f, -1, -1, 20, -1, -1, -1, 1f));
                    s.Planets.Add(new GSPlanet("redstone4", "modifiedl", 50, 0.5f, -1, -1, 30, -1, -1, -1, 1f));
                    s.Planets.Add(new GSPlanet("redstone5", "modifiedl", 50, 0.5f, -1, -1, 40, -1, -1, -1, 1f));
                    //s.Planets.Add(new GSPlanet("ashenGelisol", "AshenGelisol", 50, 0.5f, -1, -1, -1, 21, -1, -1, -1, 1f, null));
                    //s.Planets.Add(new GSPlanet("barren", "Barren", 50, 0.5f, -1, -1, -1, 31, -1, -1, -1, 1f, null));
                    //s.Planets.Add(new GSPlanet("lava", "Lava", 50, 0.5f, -1, -1, -1, 41, -1, -1, -1, 1f, null));
                    //s.Planets.Add(new GSPlanet("lavam", "modifiedl", 50, 0.5f, -1, -1, -1, 45, -1, -1, -1, 1f, null));
                    //s.Planets.Add(new GSPlanet("ocean", "OceanWorld", 50, 0.5f, -1, -1, -1, 51, -1, -1, -1, 1f, null));
                    //for (var j = 0; j < 20; j++)
                    //{
                    //    s.Planets.Add(new GSPlanet("gs2["+ (10 + (j * 10)) + "-" + ( (j * modifier) - (5*modifier)    ) + "]", "modified" + j, 10+(j*10), 1, -1, -1, -1, 4 + j * (360 / 50), -1, -1, -1, 1f, null));             
                }

                //}
                s.position = random.PointOnSphere(30);
                GSSettings.Stars.Add(s);
                //GS2.EndGame();
                //GS2.LogJson(GSSettings.Stars);
            }
        }

        public void Import(GSGenPreferences preferences)
        {
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }
    }
}