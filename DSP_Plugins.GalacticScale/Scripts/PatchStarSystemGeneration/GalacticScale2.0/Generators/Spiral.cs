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
        private GSGeneratorConfig config = new GSGeneratorConfig();
        public void Init()
        {
            GS2.Log("Spiral:Initializing");
            config.DisableSeedInput = true;
            config.DisableStarCountSlider = false;
            config.MaxStarCount = 1048;
            config.MinStarCount = 1;

        }

      
        public void Generate(int starCount)
        {
            generate(starCount);
        }
        ////////////////////////////////////////////////////////////////////



        public void generate(int starCount)
        {
            GS2.Log("Spiral:Creating New Settings");
            List<VectorLF3> positions = new List<VectorLF3>();
            for (var i = 0; i < starCount; i++) {
                double x = i * Math.Cos(6 * i) / 3;
                double y = i * Math.Sin(6 * i) / 3;
                double z = i / 4;
                positions.Add(new VectorLF3(y, z, x));
            }
            List<GSplanet> p = new List<GSplanet>
            {
                new GSplanet("Urf")
            };
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.O, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                int t = i % 7;
                ESpectrType e = (ESpectrType)t;
                GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new List<GSplanet>()));
                GSSettings.Stars[i].position = positions[i];
                //GSSettings.Stars[i].classFactor = (float)(new Random(i).NextDouble() * 6.0)-4f;
                GSSettings.Stars[i].Spectr = e;
                GSSettings.Stars[i].Name = "CF" + GSSettings.Stars[i].classFactor + "-" + e.ToString();
            }

        }


    }
}