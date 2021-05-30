using BepInEx;
using GSFullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace GalacticScale.Generators
{
    public partial class Sol : iConfigurableGenerator
    {
        public string Name => "Sol";

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
            config.DefaultStarCount = 1;
            UI_ludicrousMode = options.Add(GSUI.Checkbox("Ludicrous mode", false, o => preferences.Set("ludicrousMode", o)));
            UI_galaxyDensity = options.Add(GSUI.Slider("Galaxy density", 1, 5, 9, o => preferences.Set("galaxyDensity", o)));
            UI_systemDensity = options.Add(GSUI.Slider("System density", 1, 5, 9, o => preferences.Set("systemDensity", o)));
            UI_maxPlanetCount = options.Add(GSUI.Slider("Max planets per system", 1, 10, 99, o => preferences.Set("maxPlanetCount", o)));
            UI_minPlanetSize = options.Add(GSUI.Slider("Min planet size", 5, 30, 510, o =>
            {
                float maxSize = preferences.GetFloat("maxPlanetSize");
                if (maxSize == -1f) maxSize = 510;
                if (maxSize < (float)o) o = maxSize;
                preferences.Set("minPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_minPlanetSize.Set(preferences.GetFloat("minPlanetSize"));
            }));
            UI_maxPlanetSize = options.Add(GSUI.Slider("Max planet size", 50, 30, 510, o =>
            {
                float minSize = preferences.GetFloat("minPlanetSize");
                if (minSize == -1f) minSize = 5;
                //GS2.Log("min = " + minSize + " max = " + o.ToString());
                if (minSize > (float)o) o = minSize;
                preferences.Set("maxPlanetSize", Utils.ParsePlanetSize((float)o));
                UI_maxPlanetSize.Set(preferences.GetFloat("maxPlanetSize"));
            }));
            UI_secondarySatellites = options.Add(GSUI.Checkbox("Secondary satellites", false, o => preferences.Set("secondarySatellites", o)));
            UI_birthPlanetSize = options.Add(GSUI.Slider("Birth planet size", 20, 50, 510, o => { 
                preferences.Set("birthPlanetSize", Utils.ParsePlanetSize((float)o)); 
                UI_birthPlanetSize.Set(preferences.GetFloat("birthPlanetSize")); 
            }));
            UI_regularBirthTheme = options.Add(GSUI.Checkbox("Regular birth theme", true, o => preferences.Set("regularBirthTheme", o)));
            UI_birthPlanetSiTi = options.Add(GSUI.Checkbox("Birth planet Si/Ti", false, o => preferences.Set("birthPlanetSiTi", o)));
            UI_tidalLockInnerPlanets = options.Add(GSUI.Checkbox("Tidal lock inner planets", false, o => preferences.Set("tidalLockInnerPlanets", o)));
            UI_moonsAreSmall = options.Add(GSUI.Checkbox("Moons are small", true, o => preferences.Set("moonsAreSmall", o)));
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
            InitThemes();
            ref GSPlanets planets = ref sol.Planets;
            planets.Add(new GSPlanet("Mercury", "Lava", 150, 0.39f, 7.005f, 252.25f, 10556.28f, 0, 0.034f, 7038f, 0, 9.0827f));
            planets.Add(new GSPlanet("Earth", "Mediterranean", 140, 1.0f, 0.0005f, 100f, 43830, 0, 23.44f, 119.67f, 0f, 1.36f, new GSPlanets() { 
            new GSPlanet("The Moon", "BarrenSatellite", 110, 0.015f,5.145f,0.0f,3278f, 0,6.68f,3278f, 0, 1.36f)}));
            GSPlanet oily = planets.Add(new GSPlanet(" ", "OilGiant", 5, 0.39f, 7f, 252f, 10556f, 355, 0.034f, 7038, 0, 9f));
            planets.Add(new GSPlanet("Venus", "AcidGreenhouse", 320, 0.72f, 3.39f, 182f, 26964f, 0, 177.4f, -1000, 0, 2.6f));
            planets.Add(new GSPlanet("Obsidian", "Obsidian", 100, 0.72f, 3.39f, 182f, 26964f, 180, 177f, 1000, 0, 2.6f));
            planets.Add(new GSPlanet("IceMalusol", "IceMalusol", 100, 0.72f, 3.39f, 182f, 26964f, 10, 177f, 1000, 0, 2.6f));
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