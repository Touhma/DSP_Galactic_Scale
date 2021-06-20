using System;
using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class Spiral : iGenerator
    {
        public string Name => "Spiral";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.Spiral";
        public GSGeneratorConfig Config => config;

        public bool DisableStarCountSlider => false;
        private readonly GSGeneratorConfig config = new GSGeneratorConfig();
        public void Init()
        {
            //GS2.Log("Spiral:Initializing");
            config.DisableSeedInput = true;
            config.DisableStarCountSlider = false;
            config.MaxStarCount = 1048;
            config.MinStarCount = 1;
            //GSTheme beach = new GSTheme("OceanWorld");
            //beach.name = "Beach";
            ////beach.oceanTint = UnityEngine.Color.green;
            ////beach.lowTint = UnityEngine.Color.green;
            ////beach.terrainTint = new UnityEngine.Color(0.0f, 0.5f, 0.2f);
            //beach.algo = 1;
            //beach.Process();

            //GSTheme test2 = new GSTheme("Gas");
            //test2.name = "GreenGas";
            //test2.terrainTint = Color.green;
            //test2.Process();

            //GSTheme test3 = new GSTheme("Gas");
            //test3.name = "MagentaGas";
            //test3.terrainTint = Color.magenta;
            //test3.Process(); 

            //GSTheme test4 = new GSTheme("Gas");
            //test4.name = "YellowGas";
            //test4.terrainTint = Color.yellow;
            //test4.Process();

            for (var i = 0; i < 8; i++)
            {
                //GS2.Log("Creating Theme for Algo" + i);
                GSTheme temp = new GSTheme("Algo" + i, "Algo" + i, "Mediterranean");
                temp.Algo = i;
                temp.Process();
            }

        }


        public void Generate(int starCount) => generate(starCount);
        ////////////////////////////////////////////////////////////////////



        public void generate(int starCount)
        {
            //GS2.Log("Spiral:Creating New Settings");
            //foreach (KeyValuePair<string, GSTheme> g in GS2.ThemeLibrary) GS2.Log("Theme " + g.Key + " is in library with name " + g.Value.Name);
            List<VectorLF3> positions = new List<VectorLF3>();
            for (var i = 0; i < starCount; i++)
            {
                double x = i * Math.Cos(6 * i) / 3;
                double y = i * Math.Sin(6 * i) / 3;
                double z = i / 4;
                positions.Add(new VectorLF3(y, z, x));
            }
            GS2.Random random = new GS2.Random(GSSettings.Seed);
            //beach.InitTheme(Themes.OceanWorld);
            GSPlanets p = new GSPlanets();
            //{
            //new GSPlanet("Beach", "Beach", 180, 3f, -1, -1, -1, 5f, -1, -1, -1, 1f, null),
            //new GSPlanet("Mediterranian", "Mediterranian", 80, 3f, -1, -1, -1, 1, -1, -1, -1, -1, null),
            //new GSPlanet("Gas", "Gas", 80, 3f, -1, -1, -1, 15, -1, -1, -1, -1, null),
            //new GSPlanet("Gas2", "Gas2", 80, 3f, -1, -1, -1, 30, -1, -1, -1, -1, null),
            //new GSPlanet("IceGiant", "IceGiant", 80, 3f, -1, -1, -1, 45, -1, -1, -1, -1, null),
            //new GSPlanet("IceGiant2", "IceGiant2", 80, 3f, -1, -1, -1, 60, -1, -1, -1, -1, null),
            //new GSPlanet("Arid", "Arid", 80, 3f, -1, -1, -1, 75, -1, -1, -1, -1, null),
            //new GSPlanet("AshenGelisol", "AshenGelisol", 80, 3f, -1, -1, -1, 90, -1, -1, -1, -1, null),
            //new GSPlanet("Jungle", "Jungle", 80, 3f, -1, -1, -1, 105, -1, -1, -1, -1, null),
            //new GSPlanet("Lava", "Lava", 80, 3f, -1, -1, -1, 120, -1, -1, -1, -1, null),
            //new GSPlanet("YellowGas", "YellowGas", 180, 3f, -1, -1, -1, 3f, -1, -1, -1, -1, null),
            //new GSPlanet("GreenGas", "GreenGas", 180, 3f, -1, -1, -1, 6f, -1, -1, -1, -1, null),
            //new GSPlanet("MagentaGas", "MagentaGas", 180, 3f, -1, -1, -1, 9f, -1, -1, -1, -1, null),
            //new GSPlanet("Ice", "Ice", 80, 3f, -1, -1, -1, 150, -1, -1, -1, -1, null),
            //new GSPlanet("Barren", "Barren", 80, 3f, -1, -1, -1, 165, -1, -1, -1, -1, null),
            //new GSPlanet("Gobi", "Gobi", 80, 3f, -1, -1, -1, 180, -1, -1, -1, -1, null),
            //new GSPlanet("VolcanicAsh", "VolcanicAsh", 80, 3f, -1, -1, -1, 195, -1, -1, -1, -1, null),
            //new GSPlanet("RedStone", "RedStone", 80, 3f, -1, -1, -1, 210, -1, -1, -1, -1, null),
            //new GSPlanet("Prarie", "Prarie", 80, 3f, -1, -1, -1, 225, -1, -1, -1, -1, null),
            //new GSPlanet("Ocean", "Ocean", 80, 3f, -1, -1, -1, 240, -1, -1, -1, -1, null),
            //};
            for (var i = 0; i < 8; i++)
            {
                p.Add(new GSPlanet("Algo" + i, "Algo" + i, 100, 2f, -1, -1, 2f * i, -1, -1, -1, 1f, null));
            }
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.G, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                //int t = i % 7;
                //ESpectrType e = (ESpectrType)t;
                //GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new GSPlanets()));
                GSStar s = StarDefaults.Random(random, i);
                GSSettings.Stars.Add(s);
                GSSettings.Stars[i].position = positions[i];
                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                //GSSettings.Stars[i].Spectr = e;
                //GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
            }

        }


    }
}