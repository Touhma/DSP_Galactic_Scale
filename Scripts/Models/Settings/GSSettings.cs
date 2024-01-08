﻿using System;
using GSSerializer;
using UnityEngine;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSSettingsConverter))]
    public class GSSettings
    {
        [NonSerialized] public bool GenerationComplete = false;
        public static GSVein BirthIron = new(6, 0.1f);
        
        public static GSVein BirthCopper = new(6, 0.1f);

        // public static GSStar BirthStar { get => birthStarId>=0?Stars[birthStarId-1]:null; }
        private static GSPlanet birthPlanet;
        private static GSStar birthStar;

        private static int birthPlanetId = -1; // this is a vanilla id, not a GS Index!
        private static string birthPlanetName;

        public static bool lobbyReceivedUpdateValues = false;

        [SerializeField] public GSGalaxyParams galaxyParams = new();
        public string generatorGUID = null;

        [NonSerialized] public bool imported;

        [SerializeField] public int seed = 1;

        [SerializeField] public GSStars stars = new();

        [SerializeField] public ThemeLibrary themeLibrary = ThemeLibrary.Vanilla();

        public string version = "2.0";

        public GSSettings(int seed)
        {
            //GS3.Log("Start");
            this.seed = seed;
            //GS3.Log("End");
        }

        public static GSSettings Instance { get; set; } = new(0);

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

        public static GSPlanet BirthPlanet
        {
            get
            {
                if (birthPlanet != null) return birthPlanet;
                GS3.Warn($"BirthPlanet Requested by {GS3.GetCaller(1)} {GS3.GetCaller(2)} {GS3.GetCaller(3)}");
                if (birthPlanetId > 100)
                {
                    GS3.Warn($"Trying to find GSPlanet for id {birthPlanetId} on behalf of {GS3.GetCaller()}");
                    var p = FindPlanet(birthPlanetId);
                    if (p != null)
                    {
                        GS3.Log($"Found birth planet by ID. {p.Name}");
                        birthPlanet = p;
                        return p;
                    }
                }
                else
                {
                    GS3.Warn("BirthPlanetID < 100");

                    if (BirthPlanetName != null && BirthPlanetName != string.Empty)
                    {
                        GS3.Warn($"Trying to get birthPlanet by name of '{BirthPlanetName}'");
                        var p = FindPlanet(BirthPlanetName);
                        if (p == null)
                        {
                            GS3.Error($"BirthPlanet '{BirthPlanetName}' returned null");
                        }
                        else
                        {
                            GS3.Log($"Found birth planet by name: {birthPlanet}");
                            birthPlanet = p;
                            return p;
                        }
                    }
                }

                GS3.Log("Could Not Get Birth Planet From ID or Name. Using Random Habitable Planet.");
                GS3.Log($"BirthPlanetName:{birthPlanetName}");

                if (Stars.HabitablePlanets.Count > 0)
                {
                    GS3.Log($"Picking one of {Stars.HabitablePlanets.Count} at random");
                    var randomPlanet = Stars.HabitablePlanets[new GS3.Random(Seed).Next(Stars.HabitablePlanets.Count)];
                    birthPlanet = randomPlanet;
                    GS3.Log($"Selected {birthPlanet.Name}");
                    return randomPlanet;
                }

                GS3.Log("Could not find any habitable planets. Trying to use first planet.");
                if (StarCount > 0 && PlanetCount > 0) return Stars[0].Planets[0];

                GS3.Error("Could not find birthplanet as there are no stars or planets.");
                GS3.AbortGameStart("Could not find birthplanet as there are no stars or planets.".Translate());
                return null;
            }
        }

        public static GSStar BirthStar
        {
            get
            {
                if (birthStar == null)
                {
                    if (BirthPlanet != null)
                    {
                        foreach (var star in Stars)
                        foreach (var planet in star.Bodies)
                        {
                            if (planet == BirthPlanet)
                            {
                                return star;
                            }
                        }
                    }
                }
                else return birthStar;
                GS3.Error($"Could not find BirthStar when requested by {GS3.GetCaller()}");
                return null;
            }
        }
        public static int BirthStarId => BirthPlanet != null ? BirthPlanet.planetData.star.id : -1;
        public static int BirthPlanetId
        {
            get => BirthPlanet != null ? BirthPlanet.planetData.id : -1;
            set
            {
                GS3.Log($"BirthPlanetID set to {value} by {GS3.GetCaller()}");
                birthPlanet = FindPlanet(value);
                birthPlanetId = value;
            }
        }

        public static string BirthPlanetName
        {
            get => birthPlanetName;
            set
            {
                GS3.Log($"BirthPlanetName set to {value} by {GS3.GetCaller()}");
                birthPlanet = FindPlanet(value);
                birthPlanetName = value;
            }
        }

        public static GSPlanet FindPlanet(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var i = 0;
            foreach (var star in Stars)
            foreach (var planet in star.Bodies)
            {
                i++;
                if (planet.Name == name)
                {
                    birthStar = star;
                    return planet;
                }
            }

            GS3.Error($"FindPlanet Failed to Find {name}. Searched {i} bodies");
            foreach (var star in Stars) star.DebugStarData();
            return null;
        }

        public static GSPlanet FindPlanet(int id)
        {
            var i = 0;
            foreach (var star in Stars)
            foreach (var planet in star.Bodies)
            {
                i++;
                if (planet.planetData.id == id) return planet;
            }

            GS3.Error($"FindPlanet Failed to Find {id}. Searched {i} bodies");
            return null;
        }

        public static int PrimaryStarCount()
        {
            var count = 0;
            // var companions = new List<
            for (var i = 0; i < Stars?.Count; i++)
                // var companion = Stars[i].BinaryCompanion;
            {
                if (Stars[i] == null)
                    continue;
                if (Stars[i].genData == null) continue;
                if (!Stars[i].genData.ContainsKey("binary") || !Stars[i].genData["binary"])
                    count++;
            }
            // if (companion == string.Empty || companion == null) count++;

            return count;
        }

        public static string Serialize()
        {
            var data = Utils.Serialize(Instance, false);
            //GS3.DumpObjectToJson(Path.Combine(GS3.DataDir, "data.json"), data);
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
            GS3.Warn("Loading Data From External String");
            var serializer = new fsSerializer();
            var result = Instance;
            var data2 = fsJsonParser.Parse(json);
            var success = serializer.TryDeserialize(data2, ref result).Succeeded;
            if (result.version != Instance.version)
                GS3.Warn("Version mismatch: " + Instance.version + " trying to load " + result.version + " savedata");
            Instance = result;
            Instance.imported = true;
            GS3.Warn("Loaded Data From External String");
            return success;
        }

        public static void Reset(int seed)
        {
            GS3.Log("Start");
            Instance = new GSSettings(seed);
            GalaxyParams = new GSGalaxyParams();
            Stars.Clear();
            birthPlanetId = -1;
            BirthPlanetName = null;
            birthPlanet = null;
            //birthStarId = -1;
            GS3.gsPlanets.Clear();
            GS3.gsStars.Clear();
            ThemeLibrary = ThemeLibrary.Vanilla();
            // ThemeLibrary = GS3.ThemeLibrary;
            //GS3.Log("End");
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
    public class GSDarkFogParams
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