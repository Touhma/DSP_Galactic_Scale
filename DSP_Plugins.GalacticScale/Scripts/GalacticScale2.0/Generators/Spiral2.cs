using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale.Generators
{
    public class Spiral2 : iGenerator
    {
        public string Name => "Spiral2";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.Spiral2";
        public GSGeneratorConfig Config => config;

        public bool DisableStarCountSlider => false;
        private GSGeneratorConfig config = new GSGeneratorConfig();
        public void Init()
        {
            //GS2.Log("Spiral2:Initializing");
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
        
            for (var i=0;i<8;i++)
            {
                //GS2.Log("Creating Theme for Algo" + i);
                GSTheme temp = new GSTheme("Algo" + i, "Algo" + i, "Mediterranean");
                temp.Algo = i;
                temp.Process();
            }
            
        }


        public void Generate(int starCount)
        {
            generate(starCount);
        }
        ////////////////////////////////////////////////////////////////////

        public double getZ(Vector2 xy)
        {
            float dist = Vector2.SqrMagnitude(xy);
            return (1 + 5 * (float)Math.Exp(-1 * ((dist * dist) / 500f)));
        }

        public void generate(int starCount)
        {
            //GS2.Log("Spiral2:Creating New Settings");
            
            List<VectorLF3> positions = new List<VectorLF3>();


            System.Random random = new System.Random(GSSettings.Seed);
            List<VectorLF3> points = new List<VectorLF3>();

            //float p = 0.05f;
            //float a = 10f; //higher = less twist
            //float b = 0.5f; //higher = less twist
            //float angle = 0;
            //for (var i = 0; i < 1024; i+=4)
            //{
            //    //GS2.Log(":"+i.ToString());
            //    angle += p;
            //    float e = (float)Math.Exp(angle * b);
            //    double x = a * Math.Cos(angle) * e / 8;
            //    double y = a * Math.Sin(angle) * e / 8;
            //    double w = a * Math.Sin(-angle) * e / 8;
            //    double z = a * Math.Cos(-angle) * e / 8;
            //    float x1 = (float)x / 10; float x2 = (float)-x / 10;
            //    float y1 = (float)y / 10; float y2 = (float)-y / 10;
            //    float x3 = (float)w / 10; float x4 = (float)-w / 10;
            //    float y3 = (float)z / 10; float y4 = (float)-z / 10;
            //    var rv = Math.Max((p * 500) - (angle), 0);
            //    float rand1 = rv * (float)random.NextDouble();
            //    float rand2 = rv * (float)random.NextDouble();
            //    float rand3 = rv * (float)random.NextDouble();
            //    float rand4 = rv * (float)random.NextDouble();
            //    float rand5 = rv * (float)random.NextDouble();
            //    float rand6 = rv * (float)random.NextDouble();
            //    float rand7 = rv * (float)random.NextDouble();
            //    float rand8 = rv * (float)random.NextDouble();

            //    //float zHeight = (1 + 5 * (float)Math.Exp(-1 * ((i * i) / 50f)));

            //    points.Add(new VectorLF3(x1 + rand1, getZ(new Vector2(x1,y1)), y1 + rand2));
            //    points.Add(new VectorLF3(x2 + rand3, getZ(new Vector2(x2, y2)), y2 + rand4));
            //    points.Add(new VectorLF3(x3 + rand5, getZ(new Vector2(x3, y3)), y3 + rand6));
            //    points.Add(new VectorLF3(x4 + rand7, getZ(new Vector2(x4, y4)), y4 + rand8));
            //    GS2.Log("Zheight for " + i + " is " + getZ(new Vector2(x1, y1)) + " " + x1+" "+y1);
            List<Vector2> vectors = GenerateGalaxy(1020, 2, 3f, 0.6d, 2);
            //GS2.Log("heh" + vectors.Count);
            foreach (Vector2 v in vectors) positions.Add(new VectorLF3(v.x*20, 0, v.y*20));
            //GS2.Log(positions.Count.ToString());

            //int remaining = starCount;

            //GS2.Log("Starting While");
            //while (remaining > 0)
            //{
            //    //GS2.Log("Remaining: " + remaining);
            //    remaining--;
            //    int i = Mathf.FloorToInt((Mathf.FloorToInt(points.Count / 4)) + (float)random.NextDouble() * points.Count / 2);
            //    //GS2.Log(i.ToString());
            //    positions.Add(points[i]);
            //    points.RemoveAt(i);

            //}
            //positions.Sort(VLF3Sort);





            //beach.InitTheme(Themes.OceanWorld);
            List<GSPlanet> planets = new List<GSPlanet>();
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
            for (var i = 0; i < 1; i++)
            {
                planets.Add(new GSPlanet("Algo" + i, "Algo" + i, 100, 2f, -1, -1, -1, 2f * (float)i, -1, -1, -1, 1f, null));
            }
            //GS2.Log("**");
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.G, EStarType.MainSeqStar, planets));
            for (var i = 1; i < positions.Count; i++)
            {
                //int t = i % 7;
                //ESpectrType e = (ESpectrType)t;
                //GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new List<GSPlanet>()));
                GSStar s = StarDefaults.Random(i);
                s.Planets = new List<GSPlanet>();
                GSSettings.Stars.Add(s);
                //GS2.Log("LastFor " + i.ToString() + " of " + GSSettings.Stars.Count);
                GSSettings.Stars[i].position = positions[i];
                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                //GSSettings.Stars[i].Spectr = e;
                //GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
                //GS2.Log("Done" + i);
            }

        }

        public List<Vector2> GenerateGalaxy(int numOfStars, int numOfArms, float spin, double armSpread, double starsAtCenterRatio)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < numOfArms; i++)
            {
                result.AddRange(GenerateArm(numOfStars / numOfArms, (float)i / (float)numOfArms, spin, armSpread, starsAtCenterRatio));
            }
            return result;
        }

        public List<Vector2> GenerateArm(int numOfStars, float rotation, float spin, double armSpread, double starsAtCenterRatio)
        {
            List<Vector2> result = new List<Vector2>();
            System.Random r = new System.Random();

            for (int i = 0; i < numOfStars; i++)
            {
                //GS2.Log(i + " / " + numOfStars);
                double part = (double)i / (double)numOfStars;
                part = Math.Pow(part, starsAtCenterRatio);

                float distanceFromCenter = (float)part;
                double position = (part * spin + rotation) * Math.PI * 2;

                double xFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;
                double yFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;

                float resultX = (float)Math.Cos(position) * distanceFromCenter / 2 + 0.5f + (float)xFluctuation;
                float resultY = (float)Math.Sin(position) * distanceFromCenter / 2 + 0.5f + (float)yFluctuation;

                result.Add( new Vector2(resultX, resultY));
            }

            return result;
        }

        public static double Pow3Constrained(double x)
        {
            double value = Math.Pow(x - 0.5, 3) * 4 + 0.5d;
            return Math.Max(Math.Min(1, value), 0);
        }

        public static int VLF3Sort(VectorLF3 a,VectorLF3 b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            if (a.magnitude == b.magnitude) return 0;
            if (a.magnitude > b.magnitude) return 1;
            return -1;
        }

    }
}