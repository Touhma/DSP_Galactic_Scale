using GSFullSerializer;
using System;
using UnityEngine;

namespace GalacticScale {
    [fsObject(Converter = typeof(GSFSSettingsConverter))]
    public class GSSettings {
        public static GSSettings Instance { get => instance; set => instance = value; }
        public static ThemeLibrary ThemeLibrary { get => instance.themeLibrary; set => instance.themeLibrary = value; }
        public static GSGalaxyParams GalaxyParams { get => instance.galaxyParams; set => instance.galaxyParams = value; }
        public static int PlanetCount => instance.getPlanetCount();
        public static int Seed { get { if (instance != null) { return instance.seed; } return 0; } set => instance.seed = value; }
        public static GSStars Stars { get => instance.stars; set => instance.stars = value; }
        public static int StarCount => Stars.Count;
        // public static GSStar BirthStar { get => birthStarId>=0?Stars[birthStarId-1]:null; }
        private static GSPlanet birthPlanet = null;
        public static GSPlanet BirthPlanet {
            get {
                if (birthPlanet != null) {
                    return birthPlanet;
                }
                //GS2.Warn($"BirthPlanet Requested by {GS2.GetCaller(1)} {GS2.GetCaller(2)} {GS2.GetCaller(3)}");
                if (birthPlanetId > 100) {
                    //GS2.Warn($"Trying to find GSPlanet for id {birthPlanetId} on behalf of {GS2.GetCaller()}");
                    GSPlanet p = GS2.GetGSPlanet(birthPlanetId);
                    if (p != null) {
                        //GS2.Log($"Found birth planet by ID. {p.Name}");
                        birthPlanet = p;
                        return p;
                    } else {
                        //GS2.Warn($"Planet ID {birthPlanetId} returned null");
                    }
                } else {
                    //GS2.Warn("BirthPlanetID < 100");

                    if (BirthPlanetName != null && BirthPlanetName != string.Empty) {
                        //GS2.Warn($"Trying to get birthPlanet by name of '{BirthPlanetName}'");
                        GSPlanet p = GS2.GetGSPlanet(BirthPlanetName);
                        if (p == null) {
                            GS2.Error($"BirthPlanet '{BirthPlanetName}' returned null");
                        } else {
                            //GS2.Log($"Found birth planet by name: {birthPlanet}");
                            birthPlanet = p;
                            return p;
                        }
                    }
                }

                GS2.Log("Could Not Get Birth Planet From ID or Name. Using Random Habitable Planet.");
                if (Stars.HabitablePlanets.Count > 0) {
                    GS2.Log($"Picking one of {Stars.HabitablePlanets.Count} at random");
                    GSPlanet randomPlanet = Stars.HabitablePlanets[GS2.random.Next(Stars.HabitablePlanets.Count)];
                    birthPlanet = randomPlanet;
                    GS2.Log($"Selected {birthPlanet.Name}");
                    return randomPlanet;
                }
                GS2.Log("Could not find any habitable planets. Trying to use first planet.");
                if (StarCount > 0 && PlanetCount > 0) {
                    return Stars[0].Planets[0];
                }

                GS2.Error("Could not find birthplanet as there are no stars or planets.");
                return null;
            }
        }
        public static int BirthStarId => (BirthPlanet != null) ? BirthPlanet.planetData.star.id : -1;
        public static int BirthPlanetId { get => (BirthPlanet != null) ? BirthPlanet.planetData.id : -1; set { GS2.Log($"BirthPlanetID set to {value} by {GS2.GetCaller()}"); birthPlanetId = value; } }
        public static string BirthPlanetName { get => birthPlanetName; set { GS2.Log($"BirthPlanetName set to {value} by {GS2.GetCaller()}"); birthPlanetName = value; } }


        private static int birthPlanetId = -1;// this is a vanilla id, not a GS Index!
        private static string birthPlanetName = null;



        private static GSSettings instance = new GSSettings(0);

        public string version = "2.0";

        [SerializeField]
        public int seed = 1;
        [SerializeField]
        public GSStars stars = new GSStars();
        [SerializeField]
        public GSGalaxyParams galaxyParams = new GSGalaxyParams();
        [SerializeField]
        public ThemeLibrary themeLibrary = GS2.ThemeLibrary;

        [NonSerialized]
        public bool imported = false;

        public GSSettings(int seed) {
            //GS2.Log("Start");
            this.seed = seed;
            //GS2.Log("End");
        }
        public static void Reset(int seed) {
            GS2.Log("Start");
            instance = new GSSettings(seed);
            GalaxyParams = new GSGalaxyParams();
            Stars.Clear();
            birthPlanetId = -1;
            BirthPlanetName = null;
            birthPlanet = null;
            //birthStarId = -1;
            GS2.gsPlanets.Clear();
            GS2.gsStars.Clear();
            //GS2.Log("End");
        }
        private int getPlanetCount() {
            int count = 0;
            foreach (GSStar star in stars) {
                count += star.bodyCount;
            }

            return count;
        }
    }
    public class GSGalaxyParams {
        public int iterations = 4;
        public double minDistance = 2;
        public double minStepLength = 2.3;
        public double maxStepLength = 3.5;
        public double flatten = 0.18;
        public int graphDistance = 64;
        public int graphMaxStars = 64;
        public bool ignoreSpecials = false; // allow special ores around regular stars
    }
}