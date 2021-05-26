using BepInEx;
using FullSerializer;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace GalacticScale.Generators
{
    public class GalacticScale2 : iConfigurableGenerator
    {
        public string Name => "GalacticScale2";

        public string Author => "innominata";

        public string Description => "Just like the other generators, but more so";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.galacticscale2";

        public bool DisableStarCountSlider => false;

        public GSGeneratorConfig Config => config;

        public GSOptions Options => options;
        private GSOptions options = new GSOptions();
        private GSGeneratorConfig config = new GSGeneratorConfig();
        private GSGenPreferences preferences = new GSGenPreferences();
        public List<GSStar> stars = new List<GSStar>();
        private GSUI UI_ludicrousMode;
        private GSUI UI_birthPlanetSize;
        private GSUI UI_minPlanetSize;
        private GSUI UI_maxPlanetSize;
        private GSUI UI_galaxyDensity;
        private GSUI UI_maxPlanetCount;
        private GSUI UI_secondarySatellites;
        private GSUI UI_birthPlanetSiTi;
        private GSUI UI_tidalLockInnerPlanets;
        private GSUI UI_moonsAreSmall; 
        private GSUI UI_hugeGasGiants; 
        private GSUI UI_regularBirthTheme; 
        private GSUI UI_systemDensity;
        public void Init()
        {
            config.DefaultStarCount = 16;
            UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            UI_maxPlanetCount = options.Add(GSUI.Slider("Max planets per system", 1, 10, 99, o => preferences.Set("maxPlanetCount", o)));
            UI_minPlanetSize = options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o =>
            {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float)o) o = maxSize;
                preferences.Set("minPlanetSize", GS2.Utils.ParsePlanetSize((float)o));
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize"));
            }));
            UI_maxPlanetSize = options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o =>
            {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) minSize = 5;
                //GS2.Log("min = " + minSize + " max = " + o.ToString());
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", GS2.Utils.ParsePlanetSize((float)o));
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }));
            UI_secondarySatellites = options.Add(GSUI.Checkbox("Secondary satellites", false, o => preferences.Set("secondarySatellites", o)));
            UI_birthPlanetSize = options.Add(GSUI.Slider("Birth planet size", 20, 50, 510, o => { 
                preferences.Set("birthPlanetSize", GS2.Utils.ParsePlanetSize((float)o)); 
                UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize")); 
            }));
            UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            UI_birthPlanetSiTi = options.Add(GSUI.Checkbox("Birth planet Si/Ti", false, o => preferences.Set("birthPlanetSiTi", o)));
            UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            UI_moonsAreSmall = options.Add(GSUI.Checkbox("Moons are small", true, o => preferences.Set("moonsAreSmall", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            UI_hugeGasGiants = options.Add(GSUI.Checkbox("Huge gas giants", true, o => preferences.Set("hugeGasGiants", o)));
            ReadStarData();
        }
        
        public class externalStarData
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
        public ESpectrType getSpectrType(externalStarData s)
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
        public EStarType getStarType(externalStarData s)
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
        public void ReadStarData()
        {
            
        stars.Clear();
                string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "plugins"), "GalacticScale"), "data"), "galaxy.json");
                fsSerializer serializer = new fsSerializer();
                string json = File.ReadAllText(path);
                fsData data2 = fsJsonParser.Parse(json);
                List<externalStarData> localStars = new List<externalStarData>();
                serializer.TryDeserialize(data2, ref localStars);

                for (var i = 0; i < localStars.Count; i++)
                {
                    stars.Add(new GSStar(1, localStars[i].Name, ESpectrType.G, EStarType.MainSeqStar, new GSPlanets()));
                    stars[stars.Count - 1].position = new VectorLF3(localStars[i].x, localStars[i].y, localStars[i].z);
                    stars[stars.Count - 1].mass = localStars[i].mass;
                    stars[stars.Count - 1].radius = (localStars[i].radius);
                    stars[stars.Count - 1].Type = getStarType(localStars[i]);
                    stars[stars.Count - 1].Spectr = getSpectrType(localStars[i]);
                    stars[stars.Count - 1].luminosity = localStars[i].luminance;
                    stars[stars.Count - 1].temperature = localStars[i].temp;
                }
            
        }

        public void Generate(int starCount)
        {
            GS2.Warn("Start " + GS2.GetCaller() );
            GSSettings.Reset(GSSettings.Seed);
            if (starCount > stars.Count) starCount = stars.Count;
            for (var i = 0; i < starCount; i++)
            {
                GSStar s = stars[i].Clone();
                GSSettings.Stars.Add(s);
            }
            GenerateSol(GSSettings.Stars[0]);
           
        }

        public void GenerateSol(GSStar sol)
        {
            GS2.Log("Start" + sol.bodyCount);
            GSTheme oiler = new GSTheme("OilGiant", "SpaceWhale Excrement", "IceGiant");
            oiler.terrainTint = new UnityEngine.Color(0.388f, 0.239f, 0.113f, .9f);
            oiler.atmosphereTint = new UnityEngine.Color(0f, 0f, 0f, 1);
            oiler.thumbTint = new UnityEngine.Color(0.01f, 0.005f, 0f, 0.001f);
            oiler.PlanetType = EPlanetType.Gas;
            oiler.TerrainSettings.Algorithm = "GSTA1";
            oiler.TerrainSettings.HeightMulti = 1.4;
            oiler.CustomGeneration = true;
            //oiler.GasItems = new int[3];
            oiler.GasItems[0] = 1114;
            oiler.GasItems[1] = 1120;

            oiler.Process();
            GSTheme redIce = new GSTheme("RedIce", "RedIce", "IceGelisol");
            redIce.terrainTint = new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1);
            redIce.atmosphereTint = new UnityEngine.Color(0.5f, 0.3f, 0.3f, 1);
            redIce.oceanTint = new UnityEngine.Color(0.0f, 0f, 0f, 1);
            //redIce.oceanTint = UnityEngine.Color.yellow;
            //redIce.terrainMat.SetFloat("_AmbientInc", 0f);
            //redIce.terrainMat.SetFloat("_Multiplier", 0f);
            //redIce.terrainMat.SetFloat("_SpecularHighlights", 0f);
            redIce.TerrainSettings.Algorithm = "GSTA3";
            redIce.TerrainSettings.BiomeHeightMulti = -10f;
            redIce.CustomGeneration = true;
            redIce.Process();
            //redIce.ambientDesc.ambientColor0 = Color.yellow;
            //redIce.ambientDesc.ambientColor1 = Color.yellow;
            //redIce.ambientDesc.ambientColor2 = Color.red;
            //redIce.ambientDesc.biomoColor0 = Color.red;
            //redIce.ambientDesc.biomoColor1 = Color.green;
            //redIce.ambientDesc.biomoColor2 = Color.green;
            redIce.ambientDesc.biomoDustColor0 = Color.gray;
            redIce.ambientDesc.biomoDustColor1 = Color.gray;
            redIce.ambientDesc.biomoDustColor2 = Color.white;
            //redIce.ambientDesc.waterAmbientColor0 = Color.red;
            //redIce.ambientDesc.waterAmbientColor1 = Color.red;
            //redIce.ambientDesc.waterAmbientColor2 = Color.red;
            redIce.ambientDesc.lutContribution = 0;
            ref AssetBundle bundle = ref Scripts.PatchUI.PatchForUI.bundle;
            if (bundle == null) bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location), "galacticbundle"));
            var names = bundle.GetAllAssetNames();
            GS2.LogJson(names);
            Cubemap x = bundle.LoadAsset<Cubemap>("cube2");
            GS2.Warn(x.ToString());
            redIce.ambientDesc.reflectionMap = x;
            //var x = redIce.ambientDesc.reflectionMap.GetPixels(CubemapFace.NegativeX);

            //redIce.terrainMat.SetColor("_AmbientColor1", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_AmbientColor0", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_AmbientColor1", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_AmbientColor2", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_Color", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_EmissionColor", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_LightColorScreen", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_SpeclColor", UnityEngine.Color.green);

            //redIce.terrainMat.SetColor("_SunDir", UnityEngine.Color.green);
            //redIce.terrainMat.SetTexture("_BioShift", null);
            //redIce.terrainMat.SetTexture("_BioTex0A", null); //this one makes everything green...
            //var a = redIce.terrainMat.GetTexture("_BioTex0A");
            //var b = redIce.terrainMat.GetTexture("_BioTex1A");
            //redIce.terrainMat.SetTexture("_BioTex0A", b);
            //GS2.LogJson(redIce.terrainMat.GetTexturePropertyNames());
            //redIce.terrainMat.SetTexture("_BioTex0N", null);///
            //redIce.terrainMat.SetTexture("_BioTex1A", null);
            //redIce.terrainMat.SetTexture("_BioTex1N", null);
            //redIce.terrainMat.SetTexture("_BioTex2N", null);
            //redIce.terrainMat.SetTexture("_BioTex2A", null);

            // _BioTex0A("Bio 0 Albedo", 2D) = "white" { }
            //_BioTex0N("Bio 0 Normal", 2D) = "bump" { }
            //_BioTex1A("Bio 1 Albedo", 2D) = "white" { }
            //_BioTex1N("Bio 1 Normal", 2D) = "bump" { }
            //_BioTex2A("Bio 2 Albedo", 2D) = "white" { }
            //_BioTex2N("Bio 2 Normal", 2D) = "bump" { }
            //_BioShift("Bio Shift", 2D) = "black" { }
            //_NormalStrength("Normal Strength", Float) = 1




            //_GIStrengthDay("全局光照（白天）", Range(0, 1)) = 1

            //_GIStrengthNight("全局光照（夜晚）", Range(0, 1)) = 0.2

            //_GISaturate("全局光照饱和度", Range(0, 2)) = 1

            //Shader s = redIce.terrainMat.shader;
            //GS2.Error("SHADER : " + s.name);
            //GS2.Error(s.GetType().ToString());
            ref GSPlanets planets = ref sol.Planets;
            planets.Add(new GSPlanet("Mercury", "Lava", 150, 0.39f, 7f, 252f, 10556f, 0, 0.034f, 7038, 0, 9f, null));
            planets.Add(new GSPlanet("Venus", "VolcanicAsh", 320, 0.72f, 3.39f, 182f, 26964f, 0, 177f, 1000, 0, 2.6f, null));
            planets.Add(new GSPlanet("Earth", "Mediterranean", 400, 1.0f, 0.0005f, 100f, 43830, 0, 23.44f, 119.67f, 0f, 1.36f, null));
            planets.Add(new GSPlanet("Jumpiter", "GasGiant", 80, 5.0f, 0.0005f, 100f, 43830, 0, 23.44f, 119.67f, 0f, 1.36f, null));
            GSPlanet oily = planets.Add(new GSPlanet(" ", "OilGiant", 5, 0.39f, 7f, 252f, 10556f, 355, 0.034f, 7038, 0, 9f, null));
            planets.Add(new GSPlanet("redIce", "RedIce", 200, 0.72f, 3.39f, 182f, 26964f, 180, 177f, 1000, 0, 2.6f, null));
            oily.scale = 1f;



        }

        public void Import(GSGenPreferences preferences)
        {
            this.preferences = preferences;
            UI_ludicrousMode?.Set(preferences.GetBool("ludicrousMode"));
            UI_birthPlanetSize?.Set(preferences.GetFloat("birthPlanetSize"));
            UI_minPlanetSize?.Set(preferences.GetFloat("minPlanetSize"));
            UI_maxPlanetSize?.Set(preferences.GetFloat("maxPlanetSize"));
            UI_galaxyDensity?.Set(preferences.GetFloat("galaxyDensity"));
            UI_maxPlanetCount?.Set(preferences.GetFloat("maxPlanetCount"));
            UI_secondarySatellites?.Set(preferences.GetBool("secondarySatellites"));
            UI_birthPlanetSiTi?.Set(preferences.GetBool("birthPlanetSiTi"));
            UI_tidalLockInnerPlanets?.Set(preferences.GetBool("tidalLockInnerPlanets"));
            UI_moonsAreSmall?.Set(preferences.GetBool("moonsAreSmall"));
            UI_hugeGasGiants?.Set(preferences.GetBool("hugeGasGiants"));
            UI_regularBirthTheme?.Set(preferences.GetBool("regularBirthTheme"));
            UI_systemDensity?.Set(preferences.GetFloat("systemDensity"));
        }

        public GSGenPreferences Export()
        {
            return preferences;
        }
    }
}