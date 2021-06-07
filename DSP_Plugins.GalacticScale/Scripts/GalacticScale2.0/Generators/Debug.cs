using GSFullSerializer;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public GSOptions Options => options;

        private GSOptions options = new GSOptions();
        private GSStars stars = new GSStars();
        public void Init()
        {
            List<string> genList = new List<string>();
            foreach (iGenerator g in GS2.generators) genList.Add(g.Name);
            options.Add(new GSUI("Dryrun Generator", "Combobox", genList, OnDryRunChange, () => { }));
            options.Add(new GSUI("Output Settings", "Button", "Output", OnOutputSettingsClick, () => { }));
            options.Add(new GSUI("Output StarData", "Button", "Output", OnOutputStarDataClick, () => { }));
            options.Add(new GSUI("Output LDB Data", "Button", "Output", OnDumpLDBDataClick, () => { }));
            options.Add(new GSUI("Output Theme Library", "Button", "Output", OnDumpThemesDataClick, () => { })); 
            //options.Add(new GSUI("Import Positions", "Button", "Import", OnImportPositionsClick, () => { }));
            options.Add(new GSUI("Export LocalPlanet Veinsettings", "Button", "Export", OnExportLocalPlanetClick, () => { }));
            //options.Add(GSUI.Button("Unlock All Tech", "Go", UnlockAll, null));
            //OnImportPositionsClick(null);
        }
        public class starStuff
        {
            public string Name;
            public float x;
            public float y;
            public float z;
            public float mass;
            public string spect;
            public float radius;
            public float luminance;
            public float temp;
        }
        public ESpectrType getSpectrType(starStuff s)
        {
            switch (s.spect[0])
            {
                case 'O': return ESpectrType.O;
                case 'F': return ESpectrType.F;
                case 'G': return ESpectrType.G;
                case 'B': return ESpectrType.B;
                case 'M': return ESpectrType.M;
                case 'A': return ESpectrType.A;
                case 'K': return ESpectrType.K;
                default: break;
            }
            return ESpectrType.X;
        }
        public EStarType getStarType(starStuff s)
        {
            switch (s.spect[0])
            {
                case 'O':
                case 'F':
                case 'G': return EStarType.MainSeqStar;
                case 'B': return EStarType.MainSeqStar;
                case 'M': return EStarType.MainSeqStar;
                case 'A': return EStarType.MainSeqStar;
                case 'K': return EStarType.MainSeqStar;
                default: break;
            }
            return EStarType.WhiteDwarf;
        }

        //private void OnImportPositionsClick(object o)
        //{
        //    stars.Clear();
        //    string path = Path.Combine(GS2.DataDir, "undefined.json");
        //    GS2.Log(path);
        //    fsSerializer serializer = new fsSerializer();
        //    string json = File.ReadAllText(path);
        //    GS2.Log(json);
        //    fsData data2 = fsJsonParser.Parse(json);
        //    List<starStuff> ss = new List<starStuff>();
        //    serializer.TryDeserialize<List<starStuff>>(data2, ref ss);

        //    for (var i = 0; i < ss.Count; i++)
        //    {
        //        stars.Add(new GSStar(1, ss[i].Name,ESpectrType.G,EStarType.MainSeqStar,new GSPlanets()));
        //        stars[stars.Count - 1].position = new VectorLF3(ss[i].x, ss[i].y, ss[i].z);
        //        stars[stars.Count - 1].mass = ss[i].mass;
        //        stars[stars.Count - 1].radius = (ss[i].radius);
        //        stars[stars.Count - 1].Type = getStarType(ss[i]);
        //        stars[stars.Count - 1].Spectr = getSpectrType(ss[i]);
        //        stars[stars.Count - 1].luminosity = ss[i].luminance;
        //        stars[stars.Count - 1].temperature = ss[i].temp;
        //    }

        //}
        private void OnDumpLDBDataClick(object o)
        {
            //string outputDir = Path.Combine(GS2.DataDir, "output");
            //if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            //string path = Path.Combine(outputDir, "LDBThemes.json");
            //GS2.DumpObjectToJson(path, LDB.themes);
            //path = Path.Combine(outputDir, "LDBThemes.json");
            //GS2.DumpObjectToJson(path, LDB.themes);
            string DataDir = GS2.DataDir;
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_themes.json"), LDB._themes);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_items.json"), LDB._items);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_techs.json"), LDB._techs);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_models.json"), LDB._models);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_strings.json"), LDB._strings);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_veges.json"), LDB._veges);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_veins.json"), LDB._veins);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_tutorial.json"), LDB._tutorial);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_recipes.json"), LDB._recipes);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_audios.json"), LDB._audios);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_effectEmitters.json"), LDB._effectEmitters);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_advisorTips.json"), LDB._advisorTips);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_players.json"), LDB._players);
            GS2.DumpObjectToJson(Path.Combine(DataDir, "LDB_prompts.json"), LDB._prompts);
        }
        private void OnExportLocalPlanetClick(object o)
        {
            //string outputDir = Path.Combine(GS2.DataDir, "output");
            //if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            //string path = Path.Combine(outputDir, "LocalPlanet-"+GameMain.localPlanet.name+".json");
            GS2.LogJson(GS2.GetGSPlanet(GameMain.localPlanet).veinSettings);
            //path = Path.Combine(outputDir, "LDBThemes.json");
            //GS2.DumpObjectToJson(path, LDB.themes);
        }
        private void OnDumpThemesDataClick(object o)
        {
            string outputDir = Path.Combine(GS2.DataDir, "output");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            string path = Path.Combine(outputDir, "ThemeLibrary.json");
            GS2.DumpObjectToJson(path, GS2.ThemeLibrary);
            foreach (var x in LDB._themes.dataArray)
            {
                GS2.DumpObjectToJson(System.IO.Path.Combine(GS2.DataDir, x.displayName), x.ambientDesc);
            } 
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
            foreach (GSStar a in stars)
            {
                GS2.Log(a.Name);
            }
            GSPlanets p = new GSPlanets();
            p.Add(new GSPlanet("Test", "OceanWorld" , 100, 2f, -1,  -1, 2f * 1, -1, -1, -1, 1f, null));
            GS2.Log("Wow, this worked. GalacticScale2");
            if (starCount > stars.Count) starCount = stars.Count;
            for (var i = 0; i < starCount; i++)
            {
                //int t = i % 7;
                //ESpectrType e = (ESpectrType)t;
                //GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new GSPlanets()));
                
                GSStar s = stars[i];
                if (1 < 4) s.Planets = p;
                GSSettings.Stars.Add(s);

                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                //GSSettings.Stars[i].Spectr = e;
                //GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
            }
        }

        public void Import(GSGenPreferences preferences)
        {
            
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }
        //public void UnlockAll(object o)
        //{
        //    foreach (TechProto tech in LDB.techs.dataArray.Where(x => x.Published))
        //    {
        //        if (!GameMain.history.TechUnlocked(tech.ID))
        //            UnlockTechRecursive(tech.ID, GameMain.history);
        //    }
        //}
        //private static void UnlockTechRecursive(int techId, GameHistoryData history)
        //{
        //    TechState state = history.TechState(techId);
        //    TechProto proto = LDB.techs.Select(techId);

        //    foreach (var techReq in proto.PreTechs)
        //    {
        //        if (!history.TechState(techReq).unlocked)
        //            UnlockTechRecursive(techReq, history);
        //    }
        //    foreach (var techReq in proto.PreTechsImplicit)
        //    {
        //        if (!history.TechState(techReq).unlocked)
        //            UnlockTechRecursive(techReq, history);
        //    }
        //    foreach (var itemReq in proto.itemArray)
        //    {
        //        if (itemReq.preTech != null && !history.TechState(itemReq.preTech.ID).unlocked)
        //            UnlockTechRecursive(itemReq.preTech.ID, history);
        //    }

        //    int current = state.curLevel;
        //    for (; current < state.maxLevel; current++)
        //        for (int j = 0; j < proto.UnlockFunctions.Length; j++)
        //            history.UnlockTechFunction(proto.UnlockFunctions[j], proto.UnlockValues[j], current);

        //    history.UnlockTech(techId);
        //}
    }
}