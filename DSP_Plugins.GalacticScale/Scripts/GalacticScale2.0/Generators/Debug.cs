using System.Collections.Generic;
using System.IO;

namespace GalacticScale.Generators
{
    public class Debug : iConfigurableGenerator
    {
        public string Name => "Debug";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.debug";

        public GSGeneratorConfig Config => new GSGeneratorConfig();

        public List<GSOption> Options => options;

        private List<GSOption> options = new List<GSOption>();
        public void Init()
        {
            List<string> genList = new List<string>();
            foreach (iGenerator g in GS2.generators) genList.Add(g.Name);
            options.Add(new GSOption("Dryrun Generator", "ComboBox", genList, OnDryRunChange, () => { }));
            options.Add(new GSOption("Output Settings", "Button", "Output", OnOutputSettingsClick, () => { }));
            options.Add(new GSOption("Output StarData", "Button", "Output", OnOutputStarDataClick, () => { }));
            options.Add(new GSOption("Output LDBThemes", "Button", "Output", OnDumpPlanetDataClick, () => { }));
            options.Add(new GSOption("Output Theme Library", "Button", "Output", OnDumpThemesDataClick, () => { }));
        }
        private void OnDumpPlanetDataClick(object o)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            string path = Path.Combine(outputDir, "LDBThemes.json");
            GS2.DumpObjectToJson(path, LDB.themes);
            //path = Path.Combine(outputDir, "LDBThemes.json");
            //GS2.DumpObjectToJson(path, LDB.themes);
        }
        private void OnDumpThemesDataClick(object o)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            string path = Path.Combine(outputDir, "ThemeLibrary.json");
            GS2.DumpObjectToJson(path, GS2.ThemeLibrary);
            //path = Path.Combine(outputDir, "LDBThemes.json");
            //GS2.DumpObjectToJson(path, LDB.themes);
        }
        private void OnDryRunChange(object o)
        {
            BCE.console.WriteLine("DryRun Change" + o, System.ConsoleColor.Yellow);
            int i = (int)o;
            GS2.generator = GS2.generators[i];
            GS2.generator.Generate(64);
        }
        private void OnOutputSettingsClick(object o)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            string path = Path.Combine(outputDir, "settings.json");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            //string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
            //foreach (StarData s in GameMain.galaxy.stars)
            //{
            //    starlist+=s.classFactor+","+s.type + "," + s.spectr + "," + s.age + "," + s.mass +"," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler+"," + s.lightBalanceRadius+"\n";
            //}
            GS2.DumpObjectToJson(path, GSSettings.Instance);
        }
        private void OnOutputStarDataClick(object o)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            string path = Path.Combine(outputDir, "starData.json");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
            foreach (StarData s in GameMain.galaxy.stars)
            {
                starlist += s.classFactor + "," + s.type + "," + s.spectr + "," + s.age + "," + s.mass + "," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler + "," + s.lightBalanceRadius + "\n";
            }
            GS2.DumpObjectToJson(path, starlist);
        }
        public void Generate(int starCount)
        {
            GS2.Log("Wow, this worked. GalacticScale2");
            for (var i = 0; i < starCount; i++)
            {
                GSStar s = StarDefaults.Random();
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