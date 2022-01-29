using System;
using System.Linq;
using GSSerializer;
using UnityEngine;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSSettingsConverter))]
    public class GSSettings
    {
        public static GSVein BirthIron = new GSVein(6, 0.1f);

        public static GSVein BirthCopper = new GSVein(6, 0.1f);

        // public static GSStar BirthStar { get => birthStarId>=0?Stars[birthStarId-1]:null; }
        private static GSPlanet birthPlanet;


        private static int birthPlanetId = -1; // this is a vanilla id, not a GS Index!
        private static string birthPlanetName;

        [SerializeField] public GSGalaxyParams galaxyParams = new GSGalaxyParams();
        public string generatorGUID = null;

        public static bool lobbyReceivedUpdateValues = false;

        [NonSerialized] public bool imported;

        [SerializeField] public int seed = 1;

        [SerializeField] public GSStars stars = new GSStars();

        [SerializeField] public ThemeLibrary themeLibrary = ThemeLibrary.Vanilla();

        public string version = "2.0";

        public GSSettings(int seed)
        {
            //GS2.Log("Start");
            this.seed = seed;
            //GS2.Log("End");
        }

        public static GSSettings Instance { get; set; } = new GSSettings(0);

        public static ThemeLibrary ThemeLibrary
        {
            get => Instance.themeLibrary;
            set => Instance.themeLibrary = value;
        }

        public static GSGalaxyParams GalaxyParams
        {
            get => Instance.galaxyParams;
            set => Instance.galaxyParams = value;
        }

        public static int PlanetCount => Instance.getPlanetCount();

        public static int Seed
        {
            get
            {
                if (Instance != null) return Instance.seed;
                return 0;
            }
            set => Instance.seed = value;
        }

        public static GSStars Stars
        {
            get => Instance.stars;
            set => Instance.stars = value;
        }

        public static int StarCount => Stars.Count;

        public static int PrimaryStarCount()
        {
            int count = 0;
            // var companions = new List<
            for (int i = 0; i < Stars.Count; i++)
            {
                // var companion = Stars[i].BinaryCompanion;
                if (!Stars[i].genData.ContainsKey("binary") || !Stars[i].genData["binary"]) count++;
                // if (companion == string.Empty || companion == null) count++;
            }

            return count;
        }
        
        public static GSPlanet BirthPlanet
        {
            get
            {
                if (birthPlanet != null) return birthPlanet;
                if (GS2.Vanilla)
                {
                    GS2.Log("Getting BirthPlanet For Vanilla");
                    birthPlanet = GS2.GetGSPlanet(GameMain.galaxy.birthPlanetId);
                    return birthPlanet;
                }

                //GS2.Warn($"BirthPlanet Requested by {GS2.GetCaller(1)} {GS2.GetCaller(2)} {GS2.GetCaller(3)}");
                if (birthPlanetId > 100)
                {
                    //GS2.Warn($"Trying to find GSPlanet for id {birthPlanetId} on behalf of {GS2.GetCaller()}");
                    var p = GS2.GetGSPlanet(birthPlanetId);
                    if (p != null)
                    {
                        //GS2.Log($"Found birth planet by ID. {p.Name}");
                        birthPlanet = p;
                        return p;
                    }
                }
                else
                {
                    //GS2.Warn("BirthPlanetID < 100");

                    if (BirthPlanetName != null && BirthPlanetName != string.Empty)
                    {
                        //GS2.Warn($"Trying to get birthPlanet by name of '{BirthPlanetName}'");
                        var p = GS2.GetGSPlanet(BirthPlanetName);
                        if (p == null)
                        {
                            GS2.Error($"BirthPlanet '{BirthPlanetName}' returned null");
                        }
                        else
                        {
                            GS2.Log($"Found birth planet by name: {birthPlanet}");
                            birthPlanet = p;
                            return p;
                        }
                    }
                }

                GS2.Log("Could Not Get Birth Planet From ID or Name. Using Random Habitable Planet.");
                GS2.Log($"BirthPlanetName:{birthPlanetName}");

                if (Stars.HabitablePlanets.Count > 0)
                {
                    GS2.Log($"Picking one of {Stars.HabitablePlanets.Count} at random");
                    var randomPlanet = Stars.HabitablePlanets[new GS2.Random(Seed).Next(Stars.HabitablePlanets.Count)];
                    birthPlanet = randomPlanet;
                    GS2.Log($"Selected {birthPlanet.Name}");
                    return randomPlanet;
                }

                GS2.Log("Could not find any habitable planets. Trying to use first planet.");
                if (StarCount > 0 && PlanetCount > 0) return Stars[0].Planets[0];

                GS2.Error("Could not find birthplanet as there are no stars or planets.");
                GS2.AbortGameStart("Could not find birthplanet as there are no stars or planets.".Translate());
                return null;
            }
        }

        public static int BirthStarId => BirthPlanet != null ? BirthPlanet.planetData.star.id : -1;

        public static int BirthPlanetId
        {
            get => BirthPlanet != null ? BirthPlanet.planetData.id : -1;
            set
            {
                GS2.Log($"BirthPlanetID set to {value} by {GS2.GetCaller()}");
                birthPlanetId = value;
            }
        }

        public static string BirthPlanetName
        {
            get => birthPlanetName;
            set
            {
                GS2.Log($"BirthPlanetName set to {value} by {GS2.GetCaller()}");
                birthPlanetName = value;
            }
        }

        public static string Serialize()
        {
            var data = Utils.Serialize(Instance, false);
            //GS2.DumpObjectToJson(Path.Combine(GS2.DataDir, "data.json"), data);
            return data;
        }

        public new static string ToString()
        {
            return Utils.Serialize(Instance);
        }

        public static bool DeSerialize(string json)
        {
            return FromString(json);
        }

        public static bool FromString(string json)
        {
            Reset(0);
            GS2.Warn("Loading Data From External String");
            var serializer = new fsSerializer();
            var result = Instance;
            var data2 = fsJsonParser.Parse(json);
            var success = serializer.TryDeserialize(data2, ref result).Succeeded;
            if (result.version != Instance.version)
                GS2.Warn("Version mismatch: " + Instance.version + " trying to load " + result.version + " savedata");
            Instance = result;
            Instance.imported = true;
            GS2.Warn("Loaded Data From External String");
            return success;
        }

        public static void Reset(int seed)
        {
            GS2.Log("Start");
            Instance = new GSSettings(seed);
            GalaxyParams = new GSGalaxyParams();
            Stars.Clear();
            birthPlanetId = -1;
            BirthPlanetName = null;
            birthPlanet = null;
            //birthStarId = -1;
            GS2.gsPlanets.Clear();
            GS2.gsStars.Clear();
            ThemeLibrary = ThemeLibrary.Vanilla();
            // ThemeLibrary = GS2.ThemeLibrary;
            //GS2.Log("End");
        }

        private int getPlanetCount()
        {
            var count = 0;
            foreach (var star in stars) count += star.bodyCount;

            return count;
        }
    }

    public class GSGalaxyParams
    {
        public double flatten = 0.18;
        public bool forceSpecials = false; // allow special ores around regular stars
        public int graphDistance = 64;
        public int graphMaxStars = 64;
        public int iterations = 4;
        public double maxStepLength = 3.5;
        public double minDistance = 2;
        public double minStepLength = 2.3;
        public float resourceMulti = 1.1f;
    }
}