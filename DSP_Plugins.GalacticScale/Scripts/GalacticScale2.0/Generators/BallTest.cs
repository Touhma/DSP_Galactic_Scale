using FullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class BallTest : iConfigurableGenerator
    {
        public string Name => "BallTest";

        public string Author => "innominata";

        public string Description => "Functions for debugging";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.ball";

        public GSGeneratorConfig Config => new GSGeneratorConfig(false, false, 3, 52, 3);

        public List<GSOption> Options => options;

        private List<GSOption> options = new List<GSOption>();
        private List<GSStar> stars = new List<GSStar>();
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
       
        public void Generate(int starCount)
        {
            GS2.Random random = new GS2.Random();
            List<GSPlanet> p = new List<GSPlanet>();
            for (var i = 0; i < 10; i++)
            {
                GSTheme beach = new GSTheme("Beach"+i, "Beach"+i, "AshenGelisol");
                beach.Algo = 1;
                beach.VeinSettings.VeinPadding = 0.5f;
                beach.VeinSettings.VeinAlgorithm = "GS2";
                //beach.TerrainSettings.num8 = 1*i - 3;
                beach.TerrainSettings.heightMulti = i*10;
                //beach.TerrainSettings.baseHeight = -2f;
                beach.Process();
                GS2.Log("Theme " + "Beach" + i + " created");
            }
            //beach.oceanTint = UnityEngine.Color.green;
            //beach.lowTint = UnityEngine.Color.green;
            //beach.terrainTint = new UnityEngine.Color(0.0f, 0.5f, 0.2f);


            for (var i = 0; i < starCount; i++)
            {

                GSStar s = StarDefaults.Random();
                s.Name = "Star-" + i;
                //s.Planets = new List<GSPlanet>() { new GSPlanet("Test","Lava", 100, 1, -1, -1, -1, 2, -1, -1, -1, 1f, null), 
                //    new GSPlanet("Test", "Lava", 200, 10, -1, -1, 10000, 1, -1, -1, -1, 1f, null), 
                //    new GSPlanet("Test", "Lava", 300, 6, -1, -1, 10000, 0, -1, -1, -1, 1.38f, null) ,
                //new GSPlanet("Test", "Lava", 400, 2.5f, -1, -1, 10000, 0, -1, -1, -1, 1.38f, null) };
                //double z = randomR(20.0);
                if (i == 0)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        GS2.Log("Creating Planet with Theme " + "Beach" + j);
                        //GS2.LogJson(GS2.ThemeLibrary.Habitable);
                        //s.Planets.Add(new GSPlanet("Habitable", GS2.ThemeLibrary.Random(GS2.ThemeLibrary.Habitable).Name, 50, 2, -1, -1, -1, -1, -1, -1, -1, -1, null));
                        //s.Planets.Add(new GSPlanet("Habitable", "AshenGelisol", 30, 2, -1, -1, -1, -1, -1, -1, -1, -1, null));
                        //s.Planets.Add(new GSPlanet("num8-Test[" +(0.5 * j - 0.5)+"]", "Beach"+j, 30, 1.1f, -1, -1, -1, j * (360 / 50), -1, -1, -1, 1f, null));
                        //s.Planets.Add(new GSPlanet("num8-Test[" + (0.5 * j - 0.5) + "]", "Beach" + j, 100, 1, -1, -1, -1, j * (360 / 50), -1, -1, -1, 1f, null));
                        s.Planets.Add(new GSPlanet("r50-Test[" + j * 2f + "]", "Beach" + j, 50, 1, -1, -1, -1, 4 + j * (360 / 50), -1, -1, -1, 1f, null));
                        s.Planets.Add(new GSPlanet("r200-Test[" + j * 2f + "]", "Beach" + j, 200, 1, -1, -1, -1, 3 + j * (360 / 50), -1, -1, -1, 1f, null));
                        s.Planets.Add(new GSPlanet("earth[" + j*2f + "]", "Mediterranean", 50, 1, -1, -1, -1,  j * (360 / 50), -1, -1, -1, 1f, null));
                    }
                }
                //double phi = randomRadian();

                //double x = Mathf.Sqrt((float)(20.0 * 20.0 - (z * z))) * Mathf.Cos((float)phi);

                //double y = Mathf.Sqrt((float)(20.0 * 20.0 - (z * z))) * Mathf.Sin((float)phi);


                //s.position = new VectorLF3(x,y,z);
                s.position = random.PointOnSphere(10);
                GSSettings.Stars.Add(s);
                //GS2.EndGame();
            }
        }
        //private System.Random rand;
        //public double randomRadian()
        //{
        //    if (rand == null) rand = new System.Random(GSSettings.Seed);
        //    return rand.NextDouble() * Mathf.PI*2;
        //}
        //public double randomR(double r)
        //{
        //    if (rand == null) rand = new System.Random(GSSettings.Seed);
        //    return (rand.NextDouble() * 2 * r)-r;
        //}
        public void Import(GSGenPreferences preferences)
        {
            
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }
    }
}