using System;

namespace GalacticScale.Generators
{
    public class TintTest : iConfigurableGenerator
    {
        ////////////////////////////////////////////////////////////////////
        public GSOptions options = new GSOptions();
        public string Name => "CrashTest";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.tinttest";
        public GSGeneratorConfig Config { get; } = new GSGeneratorConfig();

        //public bool DisableStarCountSlider => false;

        public GSOptions Options => options;

        //public GSPlanets planets = new GSPlanets();
        //public Material oceanMat;
        //private float r = 1f;
        //private float g = 0.0f;
        //private float b = 0.0f;
        //private float a = 0.5f;
        //public void changeValueR(object o)
        //{
        //    float f = 1f;
        //    float.TryParse((string)o, out f);
        //    //GS2.Log(f.ToString());
        //    r = f;
        //}
        //public void changeValueG(object o)
        //{
        //    float f = 1f;
        //    float.TryParse((string)o, out f);
        //    //GS2.Log(f.ToString());
        //    g = f;
        //}
        //public void changeValueB(object o)
        //{
        //    float f = 1f;
        //    float.TryParse((string)o, out f);
        //    //GS2.Log(f.ToString());
        //    b = f;
        //}
        //public void changeValueA(object o)
        //{
        //    float f = 1f;
        //    float.TryParse((string)o, out f);
        //    //GS2.Log(f.ToString());
        //    a = f;
        //}
        //public void changeValuePar(object o)
        //{
        //    string f = o as string;
        //    par = f;
        //}
        //public string par = "_DepthFactor";
        //private void updateOceanMat(object o)
        //{
        //    GSSettings.ThemeLibrary["TintCustom"].oceanMaterial.Tint = new Color(r, g, b, a);
        //}
        public void Init()
        {
            //options.Add(new GSUI("R", "Input", "1", changeValueR, () => { }));
            //options.Add(new GSUI("G", "Input", "0", changeValueG, () => { }));
            //options.Add(new GSUI("B", "Input", "0", changeValueB, () => { }));
            //options.Add(new GSUI("A", "Input", ".75", changeValueA, () => { }));
            ////options.Add(new GSOption("field", "Input", "_DepthFactor", changeValuePar, () => { }));
            //options.Add(new GSUI("Go", "Button", "Go", updateOceanMat, () => { }));
            //GS2.Log("TT:Initializing");
            //config.DisableSeedInput = true;
            //config.DisableStarCountSlider = false;
            //config.MaxStarCount = 1;
            //config.MinStarCount = 1;
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
            //Dictionary<string, Color> colors = new Dictionary<string, Color>();
            //colors.Add("Custom", new Color(r, g, b, a)); 

            //int i = 1;
            //foreach (KeyValuePair<string, Color> c in colors)
            //{
            //    //GS2.Log("Creating Theme for Tint" + c.Key);

            //    GSTheme temp = new GSTheme("Tint" + c.Key, "Tint" + c.Key, "Mediterranean");
            //    //temp.atmosphereTint = c.Value;
            ////temp.terrainTint = c.Value;
            //temp.oceanMaterial.Tint = c.Value;
            ////ColorUtility.TryParseHtmlString("#fce303", out cc);
            //temp.Process();

            //GSTheme temp2 = new GSTheme("TintLava" + c.Key, "TintLava" + c.Key, "Lava");
            ////temp2.atmosphereTint = c.Value;
            //temp2.terrainMaterial.Tint = c.Value;
            ////temp2.oceanTint = c.Value;
            ////ColorUtility.TryParseHtmlString("#fce303", out cc);
            //temp2.Process();

            //GSTheme temp3 = new GSTheme("TintIce" + c.Key, "TintIce" + c.Key, "IceGelisol");
            //temp3.atmosphereMaterial.Tint = c.Value;
            ////temp3.terrainTint = c.Value;
            ////temp3.oceanTint = c.Value;
            //Color cc = Color.white;
            ////ColorUtility.TryParseHtmlString("#fce303", out cc);
            //temp3.Process();

            //GSTheme tempg = new GSTheme("TintGiant" + c.Key, "TintGiant" + c.Key, "GasGiant");
            //tempg.atmosphereMaterial.Tint = c.Value;
            //Material tempMat = Resources.Load<Material>("Universe/Materials/Stars/" + "star-mass-a");
            //if (tempMat != null) tempg.terrainMat = UnityEngine.Object.Instantiate(tempMat);
            ////tempg.terrainTint = c.Value;
            //tempg.Process(); //tempg.Monkey(c.Value);
            //GSTheme tempig = new GSTheme("TintIceGiant" + c.Key, "TintIceGiant" + c.Key, "IceGiant");
            //tempig.atmosphereMaterial.Tint = c.Value;
            ////tempig.terrainTint = c.Value;
            //tempig.Process(); 

            //planets.Add(new GSPlanet("Tint" + c.Key, "Tint" + c.Key, 100, 2f - (i*0.005f), -1, -1, 10000f, (float)i*15, -1, -1, -1, -1, null));
            //planets[planets.Count - 1].Moons.Add(new GSPlanet("TintLava" + c.Key, "TintLava" + c.Key, 100, 0.02f, -1, -1, 100f, 0, -1, -1, -1, -1, null));
            //planets[planets.Count - 1].Moons.Add(new GSPlanet("TintIce" + c.Key, "TintIce" + c.Key, 100, 0.02f, -1, -1, 100f, 180, -1, -1, -1, -1, null));

            //planets[planets.Count - 1].Moons.Add(new GSPlanet("TintGiant" + c.Key, "TintGiant" + c.Key, 100, .1f, -1, -1, 10000f, 0, -1, -1, -1, -1, null));
            //planets[planets.Count - 1].Moons.Add(new GSPlanet("TintIceGiant" + c.Key, "TintIceGiant" + c.Key, 100, .1f, -1, -1, 10000f, 180, -1, -1, -1, -1, null));
            //i += 1;
            //}
        }


        public void Generate(int starCount, StarData birthStar = null)
        {
            throw new Exception("Doh"); //generate(starCount);
        }

        public void Import(GSGenPreferences preferences)
        {
        }

        public GSGenPreferences Export()
        {
            return new GSGenPreferences();
        }


        public void generate(int starCount, StarData birthStar = null)
        {
            //GS2.Log("TT:Creating New Settings");


            //beach.InitTheme(Themes.OceanWorld);
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

            //GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.X, EStarType.BlackHole, planets));
            //string outputDir = Path.Combine(GS2.DataDir, "output");
            //string path = Path.Combine(outputDir, "settings.json");
            //if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
            ////string starlist = "ClassFactor,Type,Spectr,Age,Mass,Color,Luminosity,Lifetime,Radius,Dyson Radius,Temperature,Orbit Scaler,LightbalRadius\n";
            ////foreach (StarData s in GameMain.galaxy.stars)
            ////{
            ////    starlist+=s.classFactor+","+s.type + "," + s.spectr + "," + s.age + "," + s.mass +"," + s.color + "," + s.luminosity + "," + s.lifetime + "," + s.radius + "," + s.dysonRadius + "," + s.temperature + "," + s.orbitScaler+"," + s.lightBalanceRadius+"\n";
            ////}
            //GS2.DumpObjectToJson(path, GSSettings.Instance);
        }
    }
}