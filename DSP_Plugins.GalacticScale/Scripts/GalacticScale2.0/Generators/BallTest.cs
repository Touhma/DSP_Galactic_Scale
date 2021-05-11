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

        public GSGeneratorConfig Config => new GSGeneratorConfig(false, false, 1, 1024, 2);

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
            //for (var i = 1f;i < 50f; i++)
            p.Add(new GSPlanet("Test", "Mediterranean" , 100, 1, -1, -1, -1, 0, -1, -1, -1, 1f, null));

            for (var i = 0; i < starCount-1; i++)
            {

                GSStar s = StarDefaults.Random();
                s.Planets = new List<GSPlanet>() { new GSPlanet("Test", "Mediterranean", 100, 1, -1, -1, -1, 0, -1, -1, -1, 1f, null) };
                //double z = randomR(20.0);

                //double phi = randomRadian();

                //double x = Mathf.Sqrt((float)(20.0 * 20.0 - (z * z))) * Mathf.Cos((float)phi);

                //double y = Mathf.Sqrt((float)(20.0 * 20.0 - (z * z))) * Mathf.Sin((float)phi);


                //s.position = new VectorLF3(x,y,z);
                s.position = random.PointOnSphere(20);
                GSSettings.Stars.Add(s);           
                
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