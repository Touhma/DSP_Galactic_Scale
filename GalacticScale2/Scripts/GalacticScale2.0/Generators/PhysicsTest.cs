namespace GalacticScale.Generators
{
    public class PhysicsTest : iConfigurableGenerator
    {
        private readonly GSStars stars = new GSStars();
        public string Name => "PhysicsTest";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.phys";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public GSOptions Options { get; } = new GSOptions();

        public void Import(GSGenPreferences preferences)
        {
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }

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
            var p = new GSPlanets();
            for (var i = 1f; i < 50f; i++) p.Add(new GSPlanet("Test", "OceanWorld", 100, i, -1, -1, 0, -1, -1, -1, 1f));

            for (var i = 0; i < 100; i++)
            {
                var s = new GSStar(1, "PhysRadius" + i, ESpectrType.B, EStarType.MainSeqStar, p);
                s.position = new VectorLF3(i * (1 + i % 2), i, i * (-1 + 1 % 2));
                s.physicsRadius = (i + 0.000001f) * 1000;
                //GS2.Log("added star " + i + " with physics radius " + s.physicsRadius);
                GSSettings.Stars.Add(s);
            }
        }
    }
}