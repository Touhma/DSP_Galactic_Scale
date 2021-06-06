using GSFullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSSettingsConverter))]
    public class GSSettings
    {
        public static GSSettings Instance { get { return instance; } set { instance = value; } }
        public static ThemeLibrary ThemeLibrary { get => instance.themeLibrary; set => instance.themeLibrary = value; }
        public static GSGalaxyParams GalaxyParams { get => instance.galaxyParams; set => instance.galaxyParams = value; }
        public static int PlanetCount { get => instance.getPlanetCount(); }
        public static int Seed { get { if (instance != null) return instance.seed; return 0; } set => instance.seed = value; }
        public static List<GSStar> Stars { get => instance.stars; set => instance.stars = value; }
        public static int StarCount { get => Stars.Count; }
        public static GSStar BirthStar { get => birthStarId>=0?Stars[birthStarId]:null; }
        public static GSPlanet BirthPlanet { get {
                //GS2.Warn("Trying to find GSPLANET for id " + birthPlanetId);
                GSPlanet p = GS2.GetGSPlanet(birthPlanetId);
                if (p != null)
                {
                    //GS2.Log("Found birth planet. " + p.Name);
                    return p;
                }
                GS2.Warn("Could Not Get Birth Planet From ID. Using first Planet.");
                if (StarCount > 0 && PlanetCount > 0) return Stars[0].Planets[0];
                return null;
            }
        }
        public static string BirthPlanetName { get => instance.birthPlanetName; set => instance.birthPlanetName = value; }
        public static int birthStarId = -1; // this is a vanilla id, not a GS Index!
        public static int birthPlanetId = -1;// this is a vanilla id, not a GS Index!
        public string birthPlanetName = null;
        private static GSSettings instance = new GSSettings(0);
        
        public string version = "2.0";

        [SerializeField]
        public int seed = 1;
        [SerializeField]
        public  List<GSStar> stars = new List<GSStar>();
        [SerializeField]
        public GSGalaxyParams galaxyParams = new GSGalaxyParams();
        [SerializeField]
        public ThemeLibrary themeLibrary = GS2.ThemeLibrary;

        [NonSerialized]
        public bool imported = false;

        public GSSettings(int seed)
        {
            //GS2.Log("Start");
            this.seed = seed;
            //GS2.Log("End");
        }
        public static void Reset(int seed)
        {
            GS2.Log("Start");
            instance = new GSSettings(seed);
            GalaxyParams = new GSGalaxyParams();
            Stars.Clear();
            birthPlanetId = -1;
            birthStarId = -1;
            GS2.gsPlanets.Clear();
            //GS2.Log("End");
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
        public bool ignoreSpecials = false; // allow special ores around regular stars
    }
}