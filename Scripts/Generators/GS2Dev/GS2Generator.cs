using System;
using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator3 : iConfigurableGenerator
    {
        private readonly Dictionary<GSStar, List<Orbit>> starOrbits = new();

        private GSPlanet birthPlanet;

        private GSStar birthStar;
        private string forcedBirthStar;
        private float maxStepLength = 3.5f;
        private float minDistance = 2f;


        private float minStepLength = 2.3f;

        private GS2.Random random;


        public string Name => "Galactic Scale 2 Dev";

        public string Author => "innominata";

        public string Description => "The Galactic Scale 2 Development Generator";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.gs2devActual";

        public void OnUpdate(string key, Val val)
        {
            preferences.Set(key, val);
        }

        public void Generate(int starCount, StarData forcedBirthStar = null)
        {
            if (forcedBirthStar != null)
                this.forcedBirthStar = forcedBirthStar.name;
            // GS2.Warn("Forcing BirthStar to "+this.forcedBirthStar);
            var highStopwatch = new HighStopwatch();
            highStopwatch.Begin();


            // Log($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
            random = new GS2.Random(GSSettings.Seed);
            highStopwatch.Begin();
            InitForcedStars();
            SetupBaseThemes();
            // Log($"Base Themes Setup: {highStopwatch.duration:F5}");
            highStopwatch.Begin();
            InitThemes();
            // Log($"Themes Setup: {highStopwatch.duration:F5}");
            GSSettings.GalaxyParams.graphDistance = 32;
            GSSettings.GalaxyParams.graphMaxStars = 512;
            //starCount = preferences.GetInt("defaultStarCount", 64);
            SetGalaxyDensity(preferences.GetInt("galaxyDensity", 5));

            highStopwatch.Begin();
            CalculateFrequencies();
            // Log($"Frequencies Calculated: {highStopwatch.duration:F5}");
            highStopwatch.Begin();
            GenerateStars(starCount);
            // Log($"Stars Generated: {highStopwatch.duration:F5}");
            // GenerateOrbits();
            highStopwatch.Begin();
            GeneratePlanets();


            // Log($"Planets Generated: {highStopwatch.duration:F5}");
            // AssignOrbits();
            highStopwatch.Begin();
            SetPlanetOrbitPhase();
            // Log($"Orbits Phased: {highStopwatch.duration:F5}");
            highStopwatch.Begin();
            // SelectBirthPlanet();
            if (birthPlanet.veinSettings == null) birthPlanet.veinSettings = birthPlanet.GsTheme.VeinSettings.Clone();
            if (birthPlanet.veinSettings.Algorithm == "Vanilla")
                birthPlanet.veinSettings.Algorithm = "GS2";
            birthPlanet.GsTheme.CustomGeneration = true;
            if (!preferences.GetBool("noRaresStartingSystem")) RemoveRaresFromStartingSystem();
            if (preferences.GetBool("birthPlanetSiTi")) AddSiTiToBirthPlanet();
            Log($"BirthPlanet Selected: {birthPlanet.Name}{highStopwatch.duration:F5}");
            highStopwatch.Begin();
            SanityCheck();
            // Log($"Sanity Checked: {highStopwatch.duration:F5}");
            highStopwatch.Begin();
            EnsureBirthSystemHasTi();
         
            Log($"Setting Birthplanet Name to {birthPlanet.Name}");
            GSSettings.BirthPlanetName = birthPlanet.Name;
            if (preferences.GetBool("birthRareDisable", true)) birthPlanet.rareChance = 0f;
            foreach (var star in GSSettings.Stars)
                if (!star.Decorative && preferences.GetBool("cometsEnabled") && random.NextPick(preferences.GetFloat("cometChance", 0) / 100f) && star.PlanetCount < 100)
                {
                    // GS2.Warn($"{preferences.GetFloat("cometChance", 0) / 100f} {random.NextPick(preferences.GetFloat("cometChance", 0) / 100f)}");
                    CreateComet(star);
                }

            // AssignStarLevels();
            Log($"Finished in : {highStopwatch.duration:F5}");
        }


        private void SanityCheck()
        {
            foreach (var star in GSSettings.Stars)
                //GS2.Warn($"DysonRadius for star {star.Name} is {star.dysonRadius}");
            {
                if (star.Decorative) continue;

                foreach (var body in star.Bodies)
                foreach (var m in body.Moons)
                    if (m.Radius > body.Radius && body.Scale != 10f)
                        Warn($"RADIUS ERROR {m.Name} radius {m.Radius} greater than {body.Name} radius of {body.Radius} Theme:{body.Theme}");
            }
        }

        private void EnsureBirthSystemHasTi()
        {
            if (!BirthSystemHasTi())
            {
                Warn("Birth System Has No Ti!");
                if (birthStar.TelluricBodyCount < 2)
                {
                    Themes.AshenGelisol.Process();
                    if (!GSSettings.ThemeLibrary.ContainsKey("AshenGelisol"))
                    {
                        GSSettings.ThemeLibrary.Add("AshenGelisol", Themes.AshenGelisol);
                        Themes.AshenGelisol.Process();
                    }
                    birthPlanet.Moons.Add(new GSPlanet("Titania McGrath", "AshenGelisol", GetStarMoonSize(birthStar, birthPlanet.Radius, false), 0.02f, 69f, 42069f, 0f, 69f, 420f, 0f, -1f));
                    return;
                }

                var p = birthPlanet;
                while (p == birthPlanet) p = random.Item(birthStar.TelluricBodies);
                p.Theme = "AshenGelisol";
            }
        }

        private bool BirthSystemHasTi()
        {
            foreach (var p in birthStar.Bodies)
                if (p.GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Titanium))
                    return true;
            return false;
        }

     

        public (float,float,float) SetGalaxyDensity(int density)
        {
            switch (density)
            {
                case 0:
                    var stepLength = preferences.GetFloatFloat("stepLength");
                    minStepLength =stepLength.low;
                    maxStepLength = stepLength.high;
                    minDistance = preferences.GetFloat("minDistance");
                    break;
                case 1:
                    minStepLength = 1.2f;
                    maxStepLength = 1.5f;
                    minDistance = 1.2f;
                    break;
                case 2:
                    minStepLength = 1.4f;
                    maxStepLength = 2f;
                    minDistance = 1.5f;
                    break;
                case 3:
                    minStepLength = 1.6f;
                    maxStepLength = 2.5f;
                    minDistance = 1.7f;
                    break;
                case 4:
                    minStepLength = 1.8f;
                    maxStepLength = 3f;
                    minDistance = 2f;
                    break;
                case 5:
                    minStepLength = 2f;
                    maxStepLength = 3.5f;
                    minDistance = 2.3f;
                    break;
                case 6:
                    minStepLength = 2.2f;
                    maxStepLength = 4.2f;
                    minDistance = 2.4f;
                    break;
                case 7:
                    minStepLength = 2.5f;
                    maxStepLength = 5.0f;
                    minDistance = 2.6f;
                    break;
                case 8:
                    minStepLength = 2.7f;
                    maxStepLength = 6.0f;
                    minDistance = 2.8f;
                    break;
                case 9:
                    minStepLength = 3.0f;
                    maxStepLength = 7.0f;
                    minDistance = 3.0f;
                    break;
                default:
                    var multi = density - 9;
                    minStepLength = 3f + 0.3f * multi;
                    maxStepLength = 7f + multi;
                    minDistance = 3f + multi * 0.2f;
                    break;
            }

            GSSettings.GalaxyParams.minDistance = minDistance;
            GSSettings.GalaxyParams.minStepLength = minStepLength;
            GSSettings.GalaxyParams.maxStepLength = maxStepLength;
            return (minDistance, minStepLength, maxStepLength);
        }


        private int ClampedNormal(GS2.Random random, int min, int max, int bias)
        {
            var range = max - min;
            var average = bias / 100f * range + min;
            var sdHigh = (max - average) / 3;
            var sdLow = (average - min) / 3;
            var sd = Math.Max(sdLow, sdHigh);
            var rResult = Mathf.RoundToInt(random.Normal(average, sd));
            var result = Mathf.Clamp(rResult, min, max);
            //Warn($"ClampedNormal min:{min} max:{max} bias:{bias} range:{range} average:{average} sdHigh:{sdHigh} sdLow:{sdLow} sd:{sd} fResult:{fResult} result:{result}");
            return result;
        }

       



        private void AddSiTiToBirthPlanet()
        {
            // Warn($"Adding SI/TI to birthPlanet {birthPlanet.Name}");

            //Warn("2");
            var s = GSVeinType.Generate(EVeinType.Silicium, 1, 10, 0.6f, 0.6f, 5, 10, false);
            var t = GSVeinType.Generate(EVeinType.Titanium, 1, 10, 0.6f, 0.6f, 5, 10, false);
            var vts = new List<EVeinType>();
            foreach (var vt in birthPlanet.veinSettings.VeinTypes) vts.Add(vt.type);

            if (!vts.Contains(EVeinType.Silicium)) birthPlanet.veinSettings.VeinTypes.Add(s);
            if (!vts.Contains(EVeinType.Titanium)) birthPlanet.veinSettings.VeinTypes.Add(t);
            foreach (var vt in birthPlanet.veinSettings.VeinTypes)
                if (vt.type == EVeinType.Silicium || vt.type == EVeinType.Titanium)
                    vt.rare = false;

            // WarnJson(birthPlanet.veinSettings);
        }

        // private void PickNewBirthPlanet()
        // {
        //     if (GSSettings.StarCount == 0) Error("Cannot pick birth planet as there are 0 generated stars");
        //     //LogJson(GSSettings.Stars.HabitablePlanets, true);
        //
        //     var HabitablePlanets = GSSettings.Stars.HabitablePlanets;
        //     if (HabitablePlanets.Count == 1)
        //     {
        //         birthPlanet = HabitablePlanets[0];
        //         birthStar = GetGSStar(birthPlanet);
        //         if (IsPlanetOfStar(birthStar, birthPlanet))
        //         {
        //             birthPlanetHost = null;
        //             Log($"Selected only habitable planet {birthPlanet.Name} as planet of {birthStar.Name}");
        //             return;
        //         }
        //
        //         foreach (var planet in birthStar.Planets)
        //         foreach (var moon in planet.Moons)
        //         {
        //             if (moon == birthPlanet)
        //             {
        //                 birthPlanetHost = planet;
        //                 Log($"Selected only habitable planet {birthPlanet.Name} as moon of {birthStar.Name}");
        //                 return;
        //             }
        //
        //             if (IsMoonOfPlanet(moon, birthPlanet))
        //             {
        //                 birthPlanetHost = moon;
        //                 Log($"Selected only habitable planet {birthPlanet.Name} as submoon of {birthStar.Name}");
        //                 return;
        //             }
        //         }
        //     }
        //
        //     if (HabitablePlanets.Count == 0)
        //     {
        //         Log("Generating new habitable planet by overwriting an existing one");
        //         var star = GSSettings.Stars.RandomStar;
        //         var index = 0;
        //         //Warn("Getting index");
        //         if (star.PlanetCount > 1) index = Mathf.RoundToInt((star.PlanetCount - 1) / 2);
        //
        //         var planet = star.Planets[index];
        //         //LogJson(planet, true);
        //         //Warn("Getting themeNames");
        //         var themeNames = GSSettings.ThemeLibrary.Habitable;
        //         //Warn($"Count = {themeNames.Count}");
        //         var themeName = themeNames[random.Next(themeNames.Count)];
        //         Log($"Setting Planet Theme to {themeName}");
        //         planet.Theme = themeName;
        //         //Warn("Setting birthPlanet");
        //         birthPlanet = planet;
        //         birthPlanetIndex = index;
        //         birthStar = star;
        //         Log($"Selected {birthPlanet.Name}");
        //         //LogJson(planet, true);
        //     }
        //     else if (HabitablePlanets.Count > 1)
        //     {
        //         Log("Selecting random habitable planet");
        //         birthPlanet = HabitablePlanets[random.Next(1, HabitablePlanets.Count - 1)];
        //         birthStar = GetGSStar(birthPlanet);
        //         for (var i = 0; i < birthStar.PlanetCount; i++)
        //         {
        //             if (birthStar.Planets[i] == birthPlanet)
        //             {
        //                 //birthPlanetIsMoon = false;
        //                 birthPlanetIndex = i;
        //                 Log($"Selected {birthPlanet.Name} as birthPlanet (planet) index {i} of star {birthStar.Name}");
        //                 return;
        //             }
        //
        //             for (var j = 0; j < birthStar.Planets[i].Moons.Count; j++)
        //             {
        //                 if (birthStar.Planets[i].Moons[j] == birthPlanet)
        //                 {
        //                     //birthPlanetIsMoon = true;
        //                     birthPlanetHost = birthStar.Planets[i];
        //                     birthPlanetIndex = j;
        //                     Log($"Selected {birthPlanet.Name} as birthPlanet (moon) index {j} of planet {birthPlanetHost.Name} ");
        //                     return;
        //                 }
        //
        //                 for (var k = 0; k < birthStar.Planets[i].Moons[j].Moons.Count; k++)
        //                     if (birthStar.Planets[i].Moons[j].Moons[k] == birthPlanet)
        //                     {
        //                         //birthPlanetIsMoon = true;
        //                         birthPlanetHost = birthStar.Planets[i].Moons[j];
        //                         birthPlanetIndex = k;
        //                         Log($"Selected {birthPlanet.Name} as birthPlanet (sub moon) index {k} of moon {birthPlanetHost.Name} ");
        //                         return;
        //                     }
        //             }
        //         }
        //
        //
        //         Error($"Selected {birthPlanet.Name} but failed to find a birthStar or host!");
        //     }
        // }
    }
}