using FullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSSettingsConverter))]
    public class GSSettings
    {
        private static GSSettings instance = new GSSettings(0);
        [SerializeField]
        public int seed = 1;
        [SerializeField]
        public  List<GSStar> stars = new List<GSStar>();
        [SerializeField]
        public GSGalaxyParams galaxyParams = new GSGalaxyParams();
        public string version = "2.0";
        [NonSerialized]
        public bool imported = false;
        public static int PlanetCount { get => instance.getPlanetCount(); }
        public static int Seed { get { if (instance != null) return instance.seed; return 0; } set => instance.seed = value; }
        public static List<GSStar> Stars { get => instance.stars; set => instance.stars = value; }
        public static int starCount { get => Stars.Count; }
        public static GSStar BirthStar { get => birthStarId>=0?Stars[birthStarId]:null; }
        public static GSPlanet BirthPlanet { get => BirthStar.Planets[birthPlanetId]; }
        public static int birthStarId = -1;
        public static int birthPlanetId = -1;
        public static GSGalaxyParams GalaxyParams { get => instance.galaxyParams; set => instance.galaxyParams = value; }
        public static ThemeLibrary ThemeLibrary { get => instance.themeLibrary; set => instance.themeLibrary = value; }
        [SerializeField]
        public ThemeLibrary themeLibrary = GS2.ThemeLibrary;// ThemeLibrary.Vanilla();// new ThemeLibrary(true);//GS2.ThemeLibrary; 
        public static GSSettings Instance { get { return instance; } set { instance = value; } }
      

        public GSSettings(int seed)
        {
            GS2.Log("Start");
            this.seed = seed;
            GS2.Log("End");
        }
        public static void Reset(int seed)
        {
            GS2.Log("Start");
            instance = new GSSettings(seed);
            GalaxyParams = new GSGalaxyParams();
            Stars.Clear();
            GS2.Log("End");
        }
        public int getPlanetCount()
        {
            int count = 0;
            foreach (GSStar star in stars) count+=star.bodyCount;
            return count;
        }
    }
    public class GSGalaxyParams
    {
        public int iterations = 4;
        public double minDistance = 2;
        public double minStepLength = 2.3;
        public double maxStepLength = 3.5;
        public double flatten = 0.18;
        public bool ignoreSpecials = true; // allow special ores around regular stars
    }
}