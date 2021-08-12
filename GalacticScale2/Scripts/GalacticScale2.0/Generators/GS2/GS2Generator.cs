using System;
using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;
using static GalacticScale.RomanNumbers;

namespace GalacticScale.Generators
{
    public partial class GS2Generator : iConfigurableGenerator
    {
        private GSPlanet birthPlanet;
        private GSPlanet birthPlanetHost;
        private int birthPlanetIndex = -1;
        private bool birthPlanetIsMoon;
        private GSStar birthStar;
        private float maxStepLength = 3.5f;
        private float minDistance = 2f;


        private float minStepLength = 2.3f;

        private GS2.Random random;
        public string Name => "GalacticScale";

        public string Author => "innominata";

        public string Description => "The Galactic Scale 2 Generator";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.gs2";

        public void Generate(int starCount, StarData birthStar = null)
        {
            Log($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
            SetupBaseThemes();
            InitThemes();
            GSSettings.GalaxyParams.graphDistance = 32;
            GSSettings.GalaxyParams.graphMaxStars = 512;
            //starCount = preferences.GetInt("defaultStarCount", 64);
            SetGalaxyDensity(preferences.GetInt("galaxyDensity", 5));
            random = new GS2.Random(GSSettings.Seed);
            CalculateFrequencies();
            Log("Generating Stars");
            for (var i = 0; i < starCount; i++)
            {
                var starType = ChooseStarType();
                var star = new GSStar(random.Next(), SystemNames.GetName(i), starType.spectr, starType.type,
                    new GSPlanets());
                if (star.Type != EStarType.BlackHole) star.radius *= preferences.GetFloat("starSizeMulti", 10f);
                if (star.Type == EStarType.BlackHole && preferences.GetFloat("starSizeMulti", 10f) < 2.1f)
                    star.radius *= preferences.GetFloat("starSizeMulti", 10f);
                //Warn($"Habitable zone for {star.Name} {Utils.CalculateHabitableZone(star.luminosity)}");
                GSSettings.Stars.Add(star);
                GeneratePlanetsForStar(star);
            }

            Log("Picking BirthPlanet");
            PickNewBirthPlanet();
            Log("Birthplanet Picked");
            if (!preferences.GetBool("birthPlanetUnlock", true)) birthPlanet.Theme = "Mediterranean";
            Log((birthPlanet != null).ToString());
            GSSettings.BirthPlanetName = birthPlanet.Name;
            Log("BirthPlanet Set");
            if (preferences.GetBool("birthPlanetSiTi")) AddSiTiToBirthPlanet();

            if (preferences.GetInt("birthPlanetSize", 400) != birthPlanet.Radius)
            {
                Log("Forcing BirthPlanet Size");
                //int oldRadius = birthPlanet.Radius;
                var newRadius = preferences.GetInt("birthPlanetSize", 400);

                if (birthPlanet.Radius < newRadius) //We have a problem with orbits!
                {
                    Log("Fixing Orbits...");
                    FixOrbitsForBirthPlanet(newRadius);
                }

                birthPlanet.Radius = newRadius;
                birthPlanet.Scale = 1f;
            }
            //Log("Logging BirthPlanet Json");
            //LogJson(birthPlanet, true);

            Log("End");
            foreach (var star in GSSettings.Stars)
            {
                //GS2.Warn($"DysonRadius for star {star.Name} is {star.dysonRadius}");
                star.dysonRadius =
                    star.dysonRadius * Mathf.Clamp(preferences.GetFloat("starSizeMulti", 10f), 0.5f, 100f);
                foreach (var body in star.Bodies)
                foreach (var m in body.Moons)
                    if (m.Radius > body.Radius && body.Scale != 10f)
                        Warn(
                            $"RADIUS ERROR {m.Name} radius {m.Radius} greater than {body.Name} radius of {body.Radius} Theme:{body.Theme}");
            }

            EnsureBirthSystemHasTi();
            GSSettings.BirthPlanetName = birthPlanet.Name;
        }


        private void EnsureBirthSystemHasTi()
        {
            if (!BirthSystemHasTi())
            {
                if (birthStar.TelluricBodyCount < 2)
                {
                    if (!GSSettings.ThemeLibrary.ContainsKey("AshenGelisol")) Themes.AshenGelisol.Process();
                    var tiPlanet = birthStar.Planets.Add(new GSPlanet("Black Swan", "AshenGelisol",
                        GetStarPlanetSize(birthStar), GetOrbitGap(birthStar) * birthStar.PlanetCount, 0f, 100000f, 0f,
                        0f, 360f, 0f, -1f));
                    tiPlanet.OrbitalPeriod =
                        Utils.CalculateOrbitPeriodFromStarMass(tiPlanet.OrbitRadius, birthStar.mass);
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

        private void FixOrbitsForBirthPlanet(int newRadius)
        {
            var radiusDifference = newRadius - birthPlanet.Radius;
            var newRadiusAU = newRadius * 0.000025f;
            var auRadiusDifference = radiusDifference * 0.000025f;
            if (birthPlanet.MoonCount > 0)
                for (var i = 0; i < birthPlanet.MoonCount; i++)
                    if (birthPlanet.Moons[i].OrbitRadius + birthPlanet.Moons[i].SystemRadius > newRadiusAU)
                    {
                        birthPlanet.Moons.RemoveRange(0, i + 1);
                        Log($"Fixed birthplanet orbits by removing {i + 1} moons");
                        return;
                    }

            //Is the birthPlanet a moon?
            if (birthPlanetIsMoon)
            {
                //Can we solve this by removing sub moons?
                if (birthPlanet.MoonCount > 0)
                    for (var i = 0; i < birthPlanet.MoonCount; i++)
                        if (birthPlanet.Moons[i].OrbitRadius + birthPlanet.Moons[i].SystemRadius > newRadiusAU)
                        {
                            birthPlanet.Moons.RemoveRange(0, i + 1);
                            Log($"Fixed birthplanet orbits by removing {i + 1} sub moons");
                            return;
                        }

                //Can we solve this by removing host moons?
                if (birthPlanetHost.MoonCount > 1)
                {
                    var cumulativeSystemRadii = 0.0;
                    for (var i = birthPlanetIndex - 1; i > 0; i--)
                    {
                        // check in towards the host
                        cumulativeSystemRadii += birthPlanetHost.Moons[i].SystemRadius;
                        if (cumulativeSystemRadii > auRadiusDifference)
                        {
                            birthPlanetHost.Moons.RemoveRange(i, birthPlanetIndex - i);
                            birthPlanet.OrbitRadius -= auRadiusDifference;
                            Log($"Fixed birthplanet orbits by removing {birthPlanetIndex - i} host moons on inside");
                        }
                    }

                    cumulativeSystemRadii = 0.0;
                    for (var i = birthPlanetIndex + 1; i < birthPlanetHost.MoonCount; i++)
                    {
                        // check out away from the host
                        cumulativeSystemRadii += birthPlanetHost.Moons[i].SystemRadius;
                        if (cumulativeSystemRadii > auRadiusDifference)
                        {
                            birthPlanetHost.Moons.RemoveRange(birthPlanetIndex + 1, i - birthPlanetIndex);
                            birthPlanet.OrbitRadius -= auRadiusDifference;
                            Log($"Fixed birthplanet orbits by removing {i - birthPlanetIndex} host moons on outside");
                        }
                    }
                }

                //Can we solve this by making the host smaller?
                if (birthPlanetHost.Scale == 1f && birthPlanetHost.RadiusAU > auRadiusDifference)
                {
                    birthPlanetHost.Radius -= radiusDifference;
                    Log("Fixed birthplanet orbits by making host planet smaller");
                    return;
                }

                if (birthPlanetHost.Scale == 10f && birthPlanetHost.RadiusAU > auRadiusDifference)
                {
                    var reduction = Mathf.Max(Utils.ParsePlanetSize(radiusDifference / 10), 10);
                    birthPlanetHost.Radius -= reduction;
                    Warn("Fixed birthplanet orbits by making host planet smaller");
                    return;
                }
            }

            //Is the birthPlanet a planet?
            if (!birthPlanetIsMoon)
            {
                //Fix by moving all orbits out
                for (var i = birthPlanetIndex; i < birthStar.PlanetCount; i++)
                {
                    birthStar.Planets[i].OrbitRadius += 2 * auRadiusDifference;
                    birthPlanet.OrbitRadius -= auRadiusDifference;
                }

                Log(
                    $"Fixed birthplanet orbits by adding size difference to orbit radius for all planets at or above index {birthPlanetIndex}");
                return;
            }

            Error("Failed to adjust orbits for birthPlanet Increased Size");
        }

        public void SetGalaxyDensity(int density)
        {
            switch (density)
            {
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
                    minStepLength = 2f;
                    maxStepLength = 3.5f;
                    minDistance = 2.3f;
                    break;
            }

            GSSettings.GalaxyParams.minDistance = minDistance;
            GSSettings.GalaxyParams.minStepLength = minStepLength;
            GSSettings.GalaxyParams.maxStepLength = maxStepLength;
        }

        private int GetStarPlanetCount(GSStar star)
        {
            var min = GetMinPlanetCountForStar(star);
            var max = GetMaxPlanetCountForStar(star);
            //int result = random.NextInclusive(min, max);
            var result = ClampedNormal(min, max, GetCountBiasForStar(star));
            //Log($"{star.Name} count :{result} min:{min} max:{max}");
            return result;
        }

        private int GetStarPlanetSize(GSStar star)
        {
            var min = GetMinPlanetSizeForStar(star);
            var max = GetMaxPlanetSizeForStar(star);
            var bias = GetSizeBiasForStar(star);
            //float average = ((max - (float)min) / 2) + min;
            //int range = max - min;
            //float sd = (float)range / 6;
            //return Mathf.Clamp(Utils.ParsePlanetSize(random.Normal(average, sd)), min, max);
            return ClampedNormalSize(min, max, bias);
        }

        private int ClampedNormal(int min, int max, int bias)
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

        private float ClampedNormal(float min, float max, int bias)
        {
            var range = max - min;
            var average = bias / 100f * range + min;
            var sdHigh = (max - average) / 3;
            var sdLow = (average - min) / 3;
            var sd = Math.Max(sdLow, sdHigh);
            var rResult = random.Normal(average, sd);
            var result = Mathf.Clamp(rResult, min, max);
            //Warn($"ClampedNormal min:{min} max:{max} bias:{bias} range:{range} average:{average} sdHigh:{sdHigh} sdLow:{sdLow} sd:{sd} fResult:{fResult} result:{result}");
            return result;
        }

        private int ClampedNormalSize(int min, int max, int bias)
        {
            var range = max - min;
            var average = bias / 100f * range + min;
            var sdHigh = (max - average) / 3;
            var sdLow = (average - min) / 3;
            var sd = Math.Max(sdLow, sdHigh);
            var fResult = random.Normal(average, sd);
            var result = Mathf.Clamp(Utils.ParsePlanetSize(fResult), min, max);
            //Warn($"ClampedNormal min:{min} max:{max} bias:{bias} range:{range} average:{average} sdHigh:{sdHigh} sdLow:{sdLow} sd:{sd} fResult:{fResult} result:{result}");
            return result;
        }

        private int GetStarMoonSize(GSStar star, int hostRadius, bool hostGas)
        {
            if (hostGas) hostRadius *= 10;
            var min = Utils.ParsePlanetSize(GetMinPlanetSizeForStar(star));
            int max;
            if (preferences.GetBool("moonsAreSmall", true))
            {
                float divider = 2;
                if (hostGas) divider = 4;
                max = Utils.ParsePlanetSize(Mathf.RoundToInt(hostRadius / divider));
            }
            else
            {
                max = Utils.ParsePlanetSize(hostRadius - 10);
            }

            if (max <= min) return min;
            float average = (max - min) / 2 + min;
            var range = max - min;
            var sd = (float) range / 4;
            //int size = Utils.ParsePlanetSize(random.Next(min, max));
            var size = ClampedNormalSize(min, max, GetSizeBiasForStar(star));
            //if (size > hostRadius)
            //{
            //Warn($"MoonSize {size} selected for {star.Name} moon with host size {hostRadius} avg:{average} sd:{sd} max:{max} min:{min} range:{range} hostGas:{hostGas}");
            //    size = Utils.ParsePlanetSize(hostRadius - 10);
            //}
            return size;
        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            star.Planets = new GSPlanets();
            var starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            var moonChance = GetMoonChanceForStar(star);
            var moonCount = 0;
            var secondaryMoonCount = 0;
            var protos = new List<ProtoPlanet>();
            var moons = new List<ProtoPlanet>();
            protos.Add(new ProtoPlanet {gas = CalculateIsGas(star), radius = GetStarPlanetSize(star)});
            if (protos[0].radius < 50) protos[0].radius = 50;
            for (var i = 1; i < starBodyCount; i++)
                if (random.NextPick(moonChance))
                {
                    moonCount++;
                }
                else
                {
                    var p = new ProtoPlanet {gas = CalculateIsGas(star), radius = GetStarPlanetSize(star)};

                    if (p.gas)
                    {
                        if (!preferences.GetBool("hugeGasGiants", true)) p.radius = 80;
                        if (p.radius < 50)
                            //Warn("Setting radius to 50 for gas");
                            p.radius = 50;
                    }

                    protos.Add(p);
                }

            for (var i = 0; i < Math.Min(2, protos.Count); i++)
                if (protos[i].gas && protos[i].radius > 80) //GS2.Warn("RADIUS 80"); 
                    protos[i].radius = 80;
            //if (protos[i].radius > 300)
            //{
            //    protos[i].radius = Mathf.Clamp(300, GetMinPlanetSizeForStar(star), GetMaxPlanetSizeForStar(star)); 
            //    //GS2.Warn("Clamping Radius");
            //}
            for (var i = 0; i < moonCount; i++)
                if (preferences.GetBool("secondarySatellites") && random.NextPick(moonChance) && i != 0)
                {
                    secondaryMoonCount++; // i != 0 Make sure we have at least one actual satellite
                }
                else
                {
                    var randomProto = random.Item(protos);
                    var moon = new ProtoPlanet
                        {gas = false, radius = GetStarMoonSize(star, randomProto.radius, randomProto.gas)};
                    randomProto.moons.Add(moon);
                    moons.Add(moon);
                }

            for (var i = 0; i < secondaryMoonCount; i++)
            {
                var randomMoon = random.Item(moons);
                randomMoon.moons.Add(new ProtoPlanet {radius = GetStarMoonSize(star, randomMoon.radius, false)});
            }

            foreach (var proto in protos)
            {
                if (proto.gas)
                    if (proto.radius < 50)
                        Warn("GAS AND NOT 50");
                var planet = new GSPlanet(star.Name + "-Planet", null, proto.radius, -1, -1, -1, -1, -1, -1, -1, -1);
                //planet.fields.Add("gas", proto.gas.ToString());
                if (proto.gas) planet.Scale = 10f;
                else planet.Scale = 1f;
                //Warn($"PlanetScale {planet.Name} {planet.Scale} {planet.Radius}");
                if (proto.moons.Count > 0) planet.Moons = new GSPlanets();
                foreach (var moon in proto.moons)
                {
                    var planetMoon = new GSPlanet(star.Name + "-Moon", null, moon.radius, -1, -1, -1, -1, -1, -1, -1,
                        -1);
                    planetMoon.Scale = 1f;
                    if (moon.moons.Count > 0) planetMoon.Moons = new GSPlanets();
                    foreach (var moonmoon in moon.moons)
                    {
                        var moonMoon = new GSPlanet(star.Name + "-MoonMoon", null, moonmoon.radius, -1, -1, -1, -1, -1,
                            -1, -1, -1);
                        moonMoon.Scale = 1f;
                        planetMoon.Moons.Add(moonMoon);
                    }

                    planet.Moons.Add(planetMoon);
                }

                star.Planets.Add(planet);
            }

            CreatePlanetOrbits(star);
            SelectPlanetThemes(star);
            FudgeNumbersForPlanets(star); // Probably want to revisit this :)
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                if (!IsPlanetOfStar(star, body))
                    body.OrbitInclination = (random.NextBool() ? 1 : -1) * (10f + random.NextFloat() * 50f);
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                body.RotationPeriod = random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                if (body.OrbitRadius < 1f && random.NextFloat() < 0.5f)
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (body.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (body.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (random.NextDouble() < 0.05) // Crazy Inclination
                    body.OrbitInclination = random.NextFloat(20f, 85f);

                //// Force inclinations for testing
                //body.OrbitInclination = 0f;
                //body.OrbitPhase = 0f;
                //body.OrbitalPeriod = 10000000f;
            }
        }

        private void SelectPlanetThemes(GSStar star)
        {
            foreach (var planet in star.Planets)
            {
                var heat = CalculateThemeHeat(star, planet.OrbitRadius);
                var type = EThemeType.Planet;
                if (planet.Scale == 10f) type = EThemeType.Gas;
                planet.Theme = GSSettings.ThemeLibrary.Query(random, type, heat, planet.Radius);
                //Warn($"Planet Theme Selected. {planet.Name}:{planet.Theme} Radius:{planet.Radius * planet.Scale} {((planet.Scale == 10f) ? EThemeType.Gas : EThemeType.Planet)}");
                foreach (var body in planet.Bodies)
                    if (body != planet)
                        body.Theme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon, heat, body.Radius);
                //Warn($"Set Theme for {body.Name} to {body.Theme}");
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int MoonIndex)
        {
            var moons = planet.Moons;
            if (MoonIndex == 0) return planet.RadiusAU + moons[MoonIndex].SystemRadius;
            return moons[MoonIndex - 1].SystemRadius + moons[MoonIndex - 1].OrbitRadius + moons[MoonIndex].SystemRadius;
        }

        private void CreatePlanetOrbits(GSStar star)
        {
            // Now Work Backwards from secondary Satellites to Planets, creating orbits.
            for (var planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                var planet = star.Planets[planetIndex];
                planet.Name = $"{star.Name} - {roman[planetIndex + 1]}";
                //For each Planet
                for (var moonIndex = 0; moonIndex < planet.MoonCount; moonIndex++)
                {
                    var moon = planet.Moons[moonIndex];
                    moon.Name = $"{star.Name} - {roman[planetIndex + 1]} - {roman[moonIndex + 1]}";
                    //For Each Moon
                    for (var moon2Index = 0; moon2Index < moon.MoonCount; moon2Index++)
                    {
                        //for each subsatellite
                        //float m2orbit;

                        var moon2 = moon.Moons[moon2Index];
                        moon2.Name =
                            $"{star.Name} - {roman[planetIndex + 1]} - {roman[moonIndex + 1]} - {roman[moon2Index + 1]}";
                        moon2.OrbitRadius = GetMoonOrbit() / 2f + GetNextAvailableOrbit(moon, moon2Index);
                        //Warn($"{moon2.Name} OrbitRadius:{moon2.OrbitRadius} Moon.SystemRadius:{moon.SystemRadius} Moon2.RadiusAU:{moon2.RadiusAU}  ");
                        moon2.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon2.OrbitRadius);
                    }

                    moon.OrbitRadius = GetMoonOrbit() + GetNextAvailableOrbit(planet, moonIndex);
                    //Warn($"{moon.Name} OrbitRadius:{moon.OrbitRadius} Planet.SystemRadius:{planet.SystemRadius} Moon.RadiusAU:{moon.RadiusAU} Planet Radius(AU):{planet.Radius}({planet.RadiusAU}) Planet Scale:{planet.Scale} Theme:{planet.Theme} ");
                    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
                }

                float pOrbit;
                if (planetIndex == 0) pOrbit = star.RadiusAU * 1.5f + planet.SystemRadius;
                else
                    pOrbit = star.Planets[planetIndex - 1].SystemRadius + GetOrbitGap(star) +
                             star.Planets[planetIndex - 1].OrbitRadius + planet.SystemRadius;
                planet.OrbitRadius = pOrbit;
                //Warn($"{planet.Name} orbitRadius:{planet.OrbitRadius} systemRadius:{planet.SystemRadius} Planet Radius(AU):{planet.Radius}({planet.RadiusAU}) Planet Scale:{planet.Scale}");
                //if (planetIndex != 0) Warn($"pOrbit = {star.Planets[planetIndex - 1].SystemRadius} + {GetOrbitGap(star)} + {star.Planets[planetIndex - 1].OrbitRadius} + {planet.SystemRadius};");
                planet.OrbitalPeriod = Utils.CalculateOrbitPeriod(planet.OrbitRadius);
            }
            //Orbits should be set.
        }

        private float GetMoonOrbit()
        {
            return 0.02f + random.NextFloat(0f, 0.2f);
        }

        private float GetOrbitGap(GSStar star)
        {
            (float min, float max) hz = Utils.CalculateHabitableZone(star.luminosity);
            float pCount = star.Planets.Count;
            var maxOrbitByRadius = Mathf.Sqrt(star.radius);
            var maxOrbitByHabitableZone = 30f * hz.min;
            var maxOrbitByPlanetCount = 50f * pCount / 99f;
            var maxOrbit = Mathf.Max(maxOrbitByPlanetCount, maxOrbitByRadius, maxOrbitByHabitableZone);
            var averageOrbit = maxOrbit / pCount;
            var result = ClampedNormal(0.1f, maxOrbit / pCount, GetSystemDensityBiasForStar(star));
            //Warn($"Getting Orbit Gap for Star {star.Name} with {pCount} planets. Avg:{averageOrbit} MaxbyRadius:{maxOrbitByRadius} MaxbyPCount:{maxOrbitByPlanetCount} MaxByHZ:{maxOrbitByHabitableZone} Max:{maxOrbit} HabitableZone:{hz.Item1*10f}:{hz.Item2*10f} = {result}");
            return result; // random.NextFloat(0.1f, 2f * averageOrbit);
        }

        private void AddSiTiToBirthPlanet()
        {
            Warn("Setting SI/TI");
            birthPlanet.GsTheme.VeinSettings.Algorithm = "GS2";
            birthPlanet.GsTheme.CustomGeneration = true;
            birthPlanet.GsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(
                EVeinType.Silicium,
                1, 10, 0.6f, 0.6f, 5, 10, false));
            birthPlanet.GsTheme.VeinSettings.VeinTypes.Add(GSVeinType.Generate(
                EVeinType.Titanium,
                1, 10, 0.6f, 0.6f, 5, 10, false));
        }

        private void PickNewBirthPlanet()
        {
            if (GSSettings.StarCount == 0) Error("Cannot pick birth planet as there are 0 generated stars");
            //LogJson(GSSettings.Stars.HabitablePlanets, true);

            var HabitablePlanets = GSSettings.Stars.HabitablePlanets;
            if (HabitablePlanets.Count == 1)
            {
                birthPlanet = HabitablePlanets[0];
                birthStar = GetGSStar(birthPlanet);
                if (IsPlanetOfStar(birthStar, birthPlanet))
                {
                    birthPlanetHost = null;
                    Log($"Selected only habitable planet {birthPlanet.Name} as planet of {birthStar.Name}");
                    return;
                }

                foreach (var planet in birthStar.Planets)
                foreach (var moon in planet.Moons)
                {
                    if (moon == birthPlanet)
                    {
                        birthPlanetHost = planet;
                        Log($"Selected only habitable planet {birthPlanet.Name} as moon of {birthStar.Name}");
                        return;
                    }

                    if (IsMoonOfPlanet(moon, birthPlanet))
                    {
                        birthPlanetHost = moon;
                        Log($"Selected only habitable planet {birthPlanet.Name} as submoon of {birthStar.Name}");
                        return;
                    }
                }
            }

            if (HabitablePlanets.Count == 0)
            {
                Log("Generating new habitable planet by overwriting an existing one");
                var star = GSSettings.Stars.RandomStar;
                var index = 0;
                //Warn("Getting index");
                if (star.PlanetCount > 1) index = Mathf.RoundToInt((star.PlanetCount - 1) / 2);

                var planet = star.Planets[index];
                //LogJson(planet, true);
                //Warn("Getting themeNames");
                var themeNames = GSSettings.ThemeLibrary.Habitable;
                //Warn($"Count = {themeNames.Count}");
                var themeName = themeNames[random.Next(themeNames.Count)];
                Log($"Setting Planet Theme to {themeName}");
                planet.Theme = themeName;
                //Warn("Setting birthPlanet");
                birthPlanet = planet;
                birthPlanetIndex = index;
                birthStar = star;
                Log($"Selected {birthPlanet.Name}");
                //LogJson(planet, true);
            }
            else if (HabitablePlanets.Count > 1)
            {
                Log("Selecting random habitable planet");
                birthPlanet = HabitablePlanets[random.Next(1, HabitablePlanets.Count - 1)];
                birthStar = GetGSStar(birthPlanet);
                for (var i = 0; i < birthStar.PlanetCount; i++)
                {
                    if (birthStar.Planets[i] == birthPlanet)
                    {
                        birthPlanetIsMoon = false;
                        birthPlanetIndex = i;
                        Log($"Selected {birthPlanet.Name} as birthPlanet (planet) index {i} of star {birthStar.Name}");
                        return;
                    }

                    for (var j = 0; j < birthStar.Planets[i].Moons.Count; j++)
                    {
                        if (birthStar.Planets[i].Moons[j] == birthPlanet)
                        {
                            birthPlanetIsMoon = true;
                            birthPlanetHost = birthStar.Planets[i];
                            birthPlanetIndex = j;
                            Log(
                                $"Selected {birthPlanet.Name} as birthPlanet (moon) index {j} of planet {birthPlanetHost.Name} ");
                            return;
                        }

                        for (var k = 0; k < birthStar.Planets[i].Moons[j].Moons.Count; k++)
                            if (birthStar.Planets[i].Moons[j].Moons[k] == birthPlanet)
                            {
                                birthPlanetIsMoon = true;
                                birthPlanetHost = birthStar.Planets[i].Moons[j];
                                birthPlanetIndex = k;
                                Log(
                                    $"Selected {birthPlanet.Name} as birthPlanet (sub moon) index {k} of moon {birthPlanetHost.Name} ");
                                return;
                            }
                    }
                }


                Error($"Selected {birthPlanet.Name} but failed to find a birthStar or host!");
            }
        }

        private bool CalculateIsGas(GSStar star)
        {
            var gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = Utils.CalculateHabitableZone(star.luminosity);
            hz.max *= 5f;
            hz.min *= 10f;
            //Warn($"HZ for {star.Name} {hz.min}-{hz.max}");
            if (OrbitRadius < hz.max / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.max) return EThemeHeat.Warm;
            if (OrbitRadius < hz.min) return EThemeHeat.Temperate;
            if (OrbitRadius < hz.min * 2) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }

        private class ProtoPlanet
        {
            public bool gas;
            public readonly List<ProtoPlanet> moons = new List<ProtoPlanet>();
            public int radius;
        }
    }
}