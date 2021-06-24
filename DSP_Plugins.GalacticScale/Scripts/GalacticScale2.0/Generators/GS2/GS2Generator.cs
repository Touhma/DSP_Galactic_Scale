using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator : iConfigurableGenerator
    {
        public string Name => "GalacticScale";

        public string Author => "innominata";

        public string Description => "The Galactic Scale 2 Generator";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.gs2";

        private GS2.Random random;
        private GSPlanet birthPlanet;
        private GSStar birthStar;
        private int birthPlanetIndex = -1;
        private bool birthPlanetIsMoon = false;
        private GSPlanet birthPlanetHost = null;

        public void Generate(int starCount)
        {

            Warn($"Start {GSSettings.Seed}");
            GSSettings.Reset(GSSettings.Seed);
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
                GSStar star = new GSStar(random.Next(), SystemNames.GetName(i), starType.spectr, starType.type, new GSPlanets());
                star.radius *= 10f;
                GSSettings.Stars.Add(star);
                GeneratePlanetsForStar(star);

            }
            Log("Picking BirthPlanet");
            PickNewBirthPlanet();
            GSSettings.BirthPlanet.Name = birthPlanet.Name;
            if (preferences.GetBool("birthPlanetSiTi", false))
            {
                AddSiTiToBirthPlanet();
            }

            if (preferences.GetInt("birthPlanetSize", 400) != 400)
            {
                int oldRadius = birthPlanet.Radius;
                int newRadius = preferences.GetInt("birthPlanetSize", 400);
                
                if (birthPlanet.Radius < newRadius) //We have a problem with orbits!
                {
                    FixOrbitsForBirthPlanet(newRadius);
                   
                }
                birthPlanet.Radius = newRadius;
                birthPlanet.Scale = 1f;
            }
            GS2.LogJson(birthPlanet, true);
            Log("End");
        }
        private void FixOrbitsForBirthPlanet(int newRadius)
        {
            int radiusDifference = (newRadius - birthPlanet.Radius);
            float newRadiusAU = newRadius * 0.000025f;
            float auRadiusDifference = radiusDifference * 0.000025f;
            float minOrbitRadius = birthPlanet.OrbitRadius - birthPlanet.SystemRadius;
            float maxOrbitRadius = birthPlanet.OrbitRadius + birthPlanet.SystemRadius;
            float distanceBetweenNeighboringOrbits = birthPlanet.SystemRadius;
            //Can we solve this by removing moons?
            if (birthPlanet.MoonCount > 0)
                for (var i=0;i<birthPlanet.MoonCount;i++)
                    if (birthPlanet.Moons[i].OrbitRadius + birthPlanet.Moons[i].SystemRadius > newRadiusAU)
                    {
                        birthPlanet.Moons.RemoveRange(0, i + 1);
                        Warn($"Fixed birthplanet orbits by removing {i+1} moons");
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
                            Warn($"Fixed birthplanet orbits by removing {i + 1} sub moons");
                            return;
                        }
                //Can we solve this by removing host moons?
                if (birthPlanetHost.MoonCount > 1)
                {
                    double cumulativeSystemRadii = 0.0;
                    for (var i = birthPlanetIndex -1; i > 0; i--)
                    {
                        // check in towards the host
                        cumulativeSystemRadii += birthPlanetHost.Moons[i].SystemRadius;
                        if (cumulativeSystemRadii > auRadiusDifference)
                        {
                            birthPlanetHost.Moons.RemoveRange(i, (birthPlanetIndex - i));
                            birthPlanet.OrbitRadius -= auRadiusDifference;
                            Warn($"Fixed birthplanet orbits by removing {(birthPlanetIndex - i)} host moons on inside");
                        }
                    }
                    cumulativeSystemRadii = 0.0;
                    for (var i = birthPlanetIndex + 1 ; i < birthPlanetHost.MoonCount; i++)
                    {
                        // check out away from the host
                        cumulativeSystemRadii += birthPlanetHost.Moons[i].SystemRadius;
                        if (cumulativeSystemRadii > auRadiusDifference)
                        {
                            birthPlanetHost.Moons.RemoveRange(birthPlanetIndex + 1, i - birthPlanetIndex);
                            birthPlanet.OrbitRadius -= auRadiusDifference;
                            Warn($"Fixed birthplanet orbits by removing {i - birthPlanetIndex} host moons on outside");
                        }
                    }
                }
                //Can we solve this by making the host smaller?
                if (birthPlanetHost.Scale == 1f && birthPlanetHost.RadiusAU > auRadiusDifference)
                {
                    birthPlanetHost.Radius -= radiusDifference;
                    Warn($"Fixed birthplanet orbits by making host planet smaller");
                    return;
                }
                if (birthPlanetHost.Scale == 10f && birthPlanetHost.RadiusAU > auRadiusDifference)
                {
                    int reduction = Mathf.Max(Utils.ParsePlanetSize(radiusDifference / 10), 10);
                    birthPlanetHost.Radius -= reduction;
                    Warn($"Fixed birthplanet orbits by making host planet smaller");
                    return;
                }
            }
            //Is the birthPlanet a planet?
            if (!birthPlanetIsMoon)
            {
                //Fix by moving all orbits out
                for (var i=birthPlanetIndex;i < birthStar.PlanetCount; i++)
                {
                    birthStar.Planets[i].OrbitRadius += auRadiusDifference;
                }
                Warn($"Fixed birthplanet orbits by adding size difference to orbit radius for all planets at or above index {birthPlanetIndex}");
                return;
            }
            Error("Failed to adjust orbits for birthPlanet Increased Size");
            
        }




        public void SetGalaxyDensity(int density)
        {

        }
        private int GetStarPlanetCount(GSStar star)
        {
            int min = GetMinPlanetCountForStar(star);
            int max = GetMaxPlanetCountForStar(star);
            return random.NextInclusive(min, max);
        }
        private int GetStarPlanetSize(GSStar star)
        {
            int min = GetMinPlanetSizeForStar(star);
            int max = GetMaxPlanetSizeForStar(star);
            float average = (((float)max - (float)min) / 2) + (float)min;
            int range = max - min;
            float sd = (float)range / 6;
            return Mathf.Clamp(Utils.ParsePlanetSize(random.Normal(average, sd)), min, max);
        }
        private int GetStarMoonSize(GSStar star, int hostRadius, bool hostGas)
        {
            if (hostGas) hostRadius *= 10;
            int min = Utils.ParsePlanetSize(GetMinPlanetSizeForStar(star));
            int max;
            if (preferences.GetBool("moonsAreSmall", true))
            {
                float divider = 2;
                if (hostGas) divider = 4;
                max = Utils.ParsePlanetSize(Mathf.RoundToInt((float)hostRadius / divider));
            }
            else
            {
                max = Utils.ParsePlanetSize(hostRadius - 10);
            }
            if (max <= min) return min;
            float average = (((float)max - (float)min) / 2) + (float)min;
            int range = max - min;
            float sd = (float)range / 4;
            return Utils.ParsePlanetSize(random.Normal(average, sd));
        }
        private class ProtoPlanet
        {
            public bool gas;
            public List<ProtoPlanet> moons = new List<ProtoPlanet>();
            public int radius;
        }
        private void GeneratePlanetsForStar(GSStar star)
        {
            star.Planets = new GSPlanets();
            int starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            double moonChance = GetMoonChanceForStar(star);
            int moonCount = 0;
            int secondaryMoonCount = 0;
            List<ProtoPlanet> protos = new List<ProtoPlanet>();
            List<ProtoPlanet> moons = new List<ProtoPlanet>();
            protos.Add(new ProtoPlanet() { gas = CalculateIsGas(star) });
            for (var i=1;i<starBodyCount;i++)
            {
                if (random.NextPick(moonChance)) moonCount++;
                else protos.Add(new ProtoPlanet() { gas = CalculateIsGas(star), radius = GetStarPlanetSize(star) }); 
            }
            
            for (var i = 0;i<moonCount;i++)
            {
                if (preferences.GetBool("secondarySatellites", false) && random.NextPick(moonChance) && i != 0) secondaryMoonCount++; // i != 0 Make sure we have at least one actual satellite
                else
                {
                    ProtoPlanet randomProto = random.Item(protos);
                    ProtoPlanet moon = new ProtoPlanet() { gas = false, radius = GetStarMoonSize(star, randomProto.radius, randomProto.gas) };
                    randomProto.moons.Add(moon);
                    moons.Add(moon);
                }
            }
            for (var i = 0; i < secondaryMoonCount; i++)
            {
                ProtoPlanet randomMoon = random.Item(moons);
                randomMoon.moons.Add(new ProtoPlanet() { radius = GetStarMoonSize(star, randomMoon.radius, false) });
            }

            foreach (ProtoPlanet proto in protos) {
                GSPlanet planet = new GSPlanet(star.Name + "-Planet", null, proto.radius, -1, -1, -1, -1, -1, -1, -1, -1);
                //planet.fields.Add("gas", proto.gas.ToString());
                if (proto.gas) planet.Scale = 10f;
                else planet.Scale = 1f;
                if (proto.moons.Count > 0) planet.Moons = new GSPlanets();
                foreach (ProtoPlanet moon in proto.moons)
                {
                    GSPlanet planetMoon = new GSPlanet(star.Name + "-Moon", null, moon.radius, -1, -1, -1, -1, -1, -1, -1, -1);
                    planetMoon.Scale = 1f;
                    if (moon.moons.Count > 0) planetMoon.Moons = new GSPlanets();
                    foreach (ProtoPlanet moonmoon in moon.moons)
                    {
                        GSPlanet moonMoon = new GSPlanet(star.Name + "-MoonMoon", null, moonmoon.radius, -1, -1, -1, -1, -1, -1, -1, -1);
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
            foreach (GSPlanet body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                body.OrbitInclination = (random.NextFloat() * 4 + random.NextFloat() * 5);
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                body.RotationPeriod = random.Next(60, 3600);
                if (random.NextDouble() < 0.02)
                {
                    body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                }
                if (body.OrbitRadius < 1f && random.NextFloat() < 0.5f)
                {
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                }
                else if (body.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                {
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                }
                else if (body.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                {
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                }
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                {
                    body.Obliquity = random.NextFloat(20f, 85f);
                }
                if (random.NextDouble() < 0.05) // Crazy Inclination
                {
                    body.OrbitInclination = random.NextFloat(20f, 85f);
                }

                // Force inclinations for testing
                body.OrbitInclination = 0f;
                body.OrbitPhase = 0f;
                body.OrbitalPeriod = 10000000f;
            }
        }
        private void SelectPlanetThemes(GSStar star)
        {
            foreach (GSPlanet planet in star.Planets)
            {
                EThemeHeat heat = CalculateThemeHeat(star, planet.OrbitRadius);
                EThemeType type = EThemeType.Planet;
                if (planet.Scale == 10f) type = EThemeType.Gas;
                planet.Theme = GSSettings.ThemeLibrary.Query(random,type, heat, planet.Radius, EThemeDistribute.Default);
                Warn($"Planet Theme Selected. {planet.Name}:{planet.Theme} Radius:{planet.Radius * planet.Scale} {((planet.Scale == 10f) ? EThemeType.Gas : EThemeType.Planet)}");
                foreach (GSPlanet body in planet.Bodies)
                {
                    if (body != planet) body.Theme = GSSettings.ThemeLibrary.Query(random,EThemeType.Moon, heat, body.Radius, EThemeDistribute.Default);
                    //Warn($"Set Theme for {body.Name} to {body.Theme}");
                }
            }
        }
        private float GetNextAvailableOrbit(GSPlanet planet, int MoonIndex)
        {
            GSPlanets moons = planet.Moons;
            if (MoonIndex == 0) return planet.RadiusAU + moons[MoonIndex].SystemRadius;
            return moons[MoonIndex - 1].SystemRadius + moons[MoonIndex -1].OrbitRadius + moons[MoonIndex].SystemRadius;
        }
        private void CreatePlanetOrbits(GSStar star)
        {
            // Now Work Backwards from secondary Satellites to Planets, creating orbits.
            float minOrbit = 0.0f; //This will need tweaking.
            for (var planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                GSPlanet planet = star.Planets[planetIndex];
                planet.Name += $"-{planetIndex}";
                //For each Planet
                for (var moonIndex = 0; moonIndex < planet.MoonCount; moonIndex++)
                {
                    GSPlanet moon = planet.Moons[moonIndex];
                    moon.Name += $"-{planetIndex}-{moonIndex}";
                    //For Each Moon
                    for (var moon2Index = 0; moon2Index < moon.MoonCount; moon2Index++)
                    {
                        //for each subsatellite
                        float m2orbit;
                        
                        GSPlanet moon2 = moon.Moons[moon2Index];
                        //if (moon2Index == 0) m2orbit = moon.RadiusAU + minOrbit;
                        m2orbit = moon.SystemRadius + minOrbit;
                        moon2.Name += $"-{planetIndex}-{moonIndex}-{moon2Index}";
                        moon2.OrbitRadius = GetNextAvailableOrbit(moon, moon2Index);
                        Warn($"{moon2.Name} OrbitRadius:{moon2.OrbitRadius} Moon.SystemRadius:{moon.SystemRadius} Moon2.RadiusAU:{moon2.RadiusAU}  ");
                        moon2.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon2.OrbitRadius);
                    }
                    //float m1orbit;
                    //if (moonIndex == 0) m1orbit = planet.RadiusAU + minOrbit;
                    //else m1orbit = planet.SystemRadius + minOrbit;

                    moon.OrbitRadius = GetNextAvailableOrbit(planet, moonIndex);
                    Warn($"{moon.Name} OrbitRadius:{moon.OrbitRadius} Planet.SystemRadius:{planet.SystemRadius} Moon.RadiusAU:{moon.RadiusAU} Planet Radius(AU):{planet.Radius}({planet.RadiusAU}) Planet Scale:{planet.Scale} Theme:{planet.Theme} ");
                    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
                }
                float pOrbit;
                if (planetIndex == 0) pOrbit = (star.RadiusAU * 1.5f) + planet.SystemRadius;
                else pOrbit = star.Planets[planetIndex - 1].SystemRadius + minOrbit + star.Planets[planetIndex - 1].OrbitRadius + planet.SystemRadius;
                planet.OrbitRadius = pOrbit;
                Warn($"{planet.Name} orbitRadius:{planet.OrbitRadius} systemRadius:{planet.SystemRadius} Planet Radius(AU):{planet.Radius}({planet.RadiusAU}) Planet Scale:{planet.Scale}");
                planet.OrbitalPeriod = Utils.CalculateOrbitPeriod(planet.OrbitRadius);
            }
            //Orbits should be set.
        }
        //private float RadiusToAU(float radius)
        //{
        //    return radius / 40000f;
        //}
        private void AddSiTiToBirthPlanet()
        {
            GS2.Warn("Setting SI/TI");
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
            if (GSSettings.StarCount == 0) GS2.Error("Cannot pick birth planet as there are 0 generated stars");
            GS2.LogJson(GSSettings.Stars.HabitablePlanets, true);

            GSPlanets HabitablePlanets = GSSettings.Stars.HabitablePlanets;
            if (HabitablePlanets.Count == 0)
            {
                GS2.Warn("Generating new habitable planet by overwriting an existing one");
                GSStar star = GSSettings.Stars.RandomStar;
                int index = 0;
                GS2.Warn("Getting index");
                if (star.PlanetCount > 1)
                {
                    index = Mathf.RoundToInt((star.PlanetCount - 1) / 2);
                }

                GSPlanet planet = star.Planets[index];
                GS2.LogJson(planet, true);
                GS2.Warn("Getting themeNames");
                List<string> themeNames = GSSettings.ThemeLibrary.Habitable;
                GS2.Warn($"Count = {themeNames.Count}");
                string themeName = themeNames[random.Next(themeNames.Count)];
                GS2.Warn($"Setting Planet Theme to {themeName}");
                planet.Theme = themeName;
                GS2.Warn("Setting birthPlanet");
                birthPlanet = planet;
                birthPlanetIndex = index;
                birthStar = star;
                GS2.Warn($"Selected {birthPlanet.Name}");
                GS2.LogJson(planet, true);
                return;
            }
            else if (HabitablePlanets.Count > 1)
            {
                GS2.Warn("Selecting random habitable planet");
                birthPlanet = HabitablePlanets[random.Next(1, HabitablePlanets.Count - 1)];
                birthStar = GetGSStar(birthPlanet);
                for (var i = 0; i< birthStar.PlanetCount; i++)
                {
                    if (birthStar.Planets[i] == birthPlanet)
                    {
                        birthPlanetIsMoon = false;
                        birthPlanetIndex = i;
                        GS2.Warn($"Selected {birthPlanet.Name} as birthPlanet (planet) index {i} of star {birthStar.Name}");
                    }
                    for (var j=0;j < birthStar.Planets[i].Moons.Count; j++)
                    {
                        if (birthStar.Planets[i].Moons[j] == birthPlanet)
                        {
                            birthPlanetIsMoon = true;
                            birthPlanetHost = birthStar.Planets[i];
                            birthPlanetIndex = j;
                            GS2.Warn($"Selected {birthPlanet.Name} as birthPlanet (moon) index {j} of planet {birthPlanetHost.Name} ");
                            return;
                        }
                        for (var k=0;k<birthStar.Planets[i].Moons[j].Moons.Count;k++)
                        {
                            if (birthStar.Planets[i].Moons[j].Moons[k] == birthPlanet)
                            {
                                birthPlanetIsMoon = true;
                                birthPlanetHost = birthStar.Planets[i].Moons[j];
                                birthPlanetIndex = k;
                                Warn($"Selected {birthPlanet.Name} as birthPlanet (sub moon) index {k} of moon {birthPlanetHost.Name} ");
                                return;
                            }
                        }
                    }
                }


                GS2.Error($"Selected {birthPlanet.Name} but failed to find a birthStar or host!");
                return;
            }
        }

        //private float CalculateNextAvailableOrbit(GSPlanet planet, GSPlanet moon)
        //{
        //    float randomvariance = random.NextFloat(0.005f, 0.01f);
        //    float planetsize = planet.RadiusAU;
        //    float moonsize = moon.RadiusAU;
        //    if (planet.Moons?.Count < 1)
        //    {
        //        return planetsize + moonsize + randomvariance;
        //    }
        //    GSPlanet lastMoon = planet.Moons[planet.Moons.Count - 1];
        //    float lastOrbit = lastMoon.OrbitRadius + lastMoon.SystemRadius;
        //    float thisMoonSystemRadius = moon.SystemRadius;
        //    return lastOrbit + thisMoonSystemRadius + randomvariance;
        //}
        //private float CalculateNextAvailableOrbit(GSStar star, GSPlanet planet)
        //{
        //    float randomvariance;
        //    if (random.NextDouble() < 0.1)
        //    {
        //        randomvariance = random.NextFloat(0.05f, 2f);
        //    }
        //    else
        //    {
        //        randomvariance = random.NextFloat(0.4f, 1f);
        //    }

        //    float planetsize = planet.RadiusAU;
        //    if (star.Planets?.Count < 1)
        //    {
        //        return randomvariance + planetsize;
        //    }

        //    GSPlanet lastPlanet = star.Planets[star.Planets.Count - 1];
        //    float lastPlanetOrbit = lastPlanet.OrbitRadius + lastPlanet.SystemRadius;
        //    float thisPlanetSystemRadius = planet.SystemRadius;
        //    return lastPlanetOrbit + thisPlanetSystemRadius + randomvariance;
        //}


        //public GSPlanet RandomMoon(GSStar star, GSPlanet host, string name, int index, int orbitCount, string heat)
        //{
        //    //GS2.Log($"Creating moon. Heat = {heat} name = {name} index = {index}/{orbitCount}");
        //    string theme;
        //    List<string> themeNames;
        //    //switch (heat) {
        //    //    case "Hot": themeNames = GSSettings.ThemeLibrary.Hot; break;
        //    //    case "Warm": themeNames = GSSettings.ThemeLibrary.Warm; break;
        //    //    case "Temperate": themeNames = GSSettings.ThemeLibrary.Temperate; break;
        //    //    case "Cold": themeNames = GSSettings.ThemeLibrary.Cold; break;
        //    //    default: themeNames = GSSettings.ThemeLibrary.Frozen; break;
        //    //}
        //    //theme = themeNames[random.Next(0, themeNames.Count - 1)];
        //    int radius = random.Next(30, host.Radius - 10);
        //    if (preferences.GetBool("moonsAreSmall", true) && radius > 200)
        //    {
        //        radius = random.Next(30, 190);
        //    }
        //    EThemeHeat ThemeHeat = EThemeHeat.Frozen;
        //    switch (heat)
        //    {
        //        case "Hot": ThemeHeat = EThemeHeat.Hot; break;
        //        case "Warm": ThemeHeat = EThemeHeat.Warm; break;
        //        case "Temperate": ThemeHeat = EThemeHeat.Temperate; break;
        //        case "Cold": ThemeHeat = EThemeHeat.Cold; break;
        //        default: break;
        //    }
        //    themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Moon, ThemeHeat, radius);
        //    theme = themeNames[random.Next(0, themeNames.Count - 1)];
        //    float rotationPeriod = random.Next(60, 3600);

        //    float luminosity = -1;
        //    float orbitInclination = 0f;
        //    float orbitPhase = random.NextFloat(360);
        //    float orbitObliquity = random.NextFloat(0f, 90f);
        //    float rotationPhase = 0f;
        //    GSPlanet moon = new GSPlanet(name, theme, radius, -1, orbitInclination, -1, orbitPhase, orbitObliquity, rotationPeriod, rotationPhase, luminosity);
        //    moon.Scale = 1f;
        //    moon.OrbitRadius = CalculateNextAvailableOrbit(host, moon);
        //    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
        //    if (index / orbitCount < random.NextFloat())
        //    {
        //        moon.RotationPeriod = moon.OrbitalPeriod;
        //    }
        //    //GS2.LogJson(moon);
        //    return moon;
        //}
        //public GSPlanet RandomPlanet(GSStar star, string name, int orbitIndex, int orbitCount, int moonCount, int availMoons)
        //{
            //GS2.Log($"Creating Random Planet for {star.Name}. Named: {name}. orbitIndex:{orbitIndex}/{orbitCount} moons:{moonCount}");

            //float thisOrbitDistance;
            //int radius;
            //string themeName;
            //string heat;
            //int hotOrbitMax = Mathf.RoundToInt(orbitCount / 6.66f);
            //int frozenOrbitMax = orbitCount - hotOrbitMax;
            //int warmOrbitMax = hotOrbitMax * 2;
            //int coldOrbitMax = frozenOrbitMax - hotOrbitMax;
            //int temperateOrbitMax = Mathf.RoundToInt((float)((coldOrbitMax - warmOrbitMax) / 2) + warmOrbitMax);

            //List<string> themeNames;
            //float chanceTiny;
            //float chanceHuge;
            //float chanceGas;
            //if (orbitIndex < hotOrbitMax)
            //{
            //    heat = "Hot";
            //    //themeNames = GSSettings.ThemeLibrary.Hot;
            //    chanceTiny = 0.5f;
            //    chanceGas = 0.1f;
            //    chanceHuge = 0.1f;
            //}
            //else if (orbitIndex < warmOrbitMax)
            //{
            //    heat = "Warm";
            //    //themeNames = GSSettings.ThemeLibrary.Warm;
            //    chanceTiny = 0.3f;
            //    chanceGas = 0.05f;
            //    chanceHuge = 0.25f;
            //}
            //else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax)
            //{
            //    heat = "Temperate";
            //    //themeNames = GSSettings.ThemeLibrary.Temperate;
            //    chanceTiny = 0.2f;
            //    chanceGas = 0.1f;
            //    chanceHuge = 0.3f;
            //}
            //else if (orbitIndex < coldOrbitMax)
            //{
            //    heat = "Cold";
            //    //themeNames = GSSettings.ThemeLibrary.Cold;
            //    chanceTiny = 0.2f;
            //    chanceGas = 0.2f;
            //    chanceHuge = 0.3f;
            //}
            //else
            //{
            //    heat = "Frozen";
            //    //themeNames = GSSettings.ThemeLibrary.Frozen;
            //    chanceTiny = 0.6f;
            //    chanceGas = 0.1f;
            //    chanceHuge = 0.3f;
            //}

            //themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            //float scale;
            //bool tiny = false;
            //bool huge = false;
            //bool gas = false;
            //if (random.NextFloat() < chanceTiny)
            //{
            //    tiny = true;
            //}

            //if (random.NextFloat() < chanceHuge)
            //{
            //    huge = true;
            //}

            //if (random.NextFloat() < chanceGas)
            //{
            //    gas = true;
            //}
            //EThemeHeat ThemeHeat = EThemeHeat.Frozen;
            //switch (heat)
            //{
            //    case "Hot": ThemeHeat = EThemeHeat.Hot; break;
            //    case "Warm": ThemeHeat = EThemeHeat.Warm; break;
            //    case "Temperate": ThemeHeat = EThemeHeat.Temperate; break;
            //    case "Cold": ThemeHeat = EThemeHeat.Cold; break;
            //    default: break;
            //}
            //if (gas)
            //{
            //    scale = 10f;
            //    if (!tiny && !huge)
            //    {
            //        radius = random.Next(100, 200);
            //    }
            //    else if (tiny && !huge)
            //    {
            //        radius = random.Next(60, 200);
            //    }
            //    else if (huge && !tiny)
            //    {
            //        radius = random.Next(200, 510);
            //    }
            //    else
            //    {
            //        radius = random.Next(60, 200);
            //    }
            //    themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Gas, ThemeHeat, radius);
            //    if (themeNames.Count > 0) themeName = themeNames[random.Next(0, themeNames.Count)];
            //    else themeName = "Mediterranean";
            //    ////GS2.Log("Gas. Radius " + radius);
            //    //if (orbitIndex < hotOrbitMax) {
            //    //    themeNames = GSSettings.ThemeLibrary.HotGasGiant;
            //    //} else if (orbitIndex < warmOrbitMax) {
            //    //    themeNames = GSSettings.ThemeLibrary.GasGiant;
            //    //} else if (orbitIndex < temperateOrbitMax && orbitIndex > warmOrbitMax) {
            //    //    themeNames = GSSettings.ThemeLibrary.GasGiant;
            //    //} else if (orbitIndex < coldOrbitMax) {
            //    //    themeNames = GSSettings.ThemeLibrary.IceGiant;
            //    //} else {
            //    //    themeNames = GSSettings.ThemeLibrary.IceGiant;
            //    //}

            //    //themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            //}
            //else
            //{
            //    scale = 1f;
            //    var mn = preferences.GetInt("minPlanetSize");
            //    var mx = preferences.GetInt("maxPlanetSize");
            //    var hugeRange = (min: Mathf.Clamp(350, mn, mx), max: mx);
            //    var normalRange = (min: mn, max: mx);
            //    var tinyRange = (min: mn, max: Mathf.Clamp(140, mn, mx));
            //    if (hugeRange.min > hugeRange.max) hugeRange.min = hugeRange.max;
            //    if (tinyRange.min > tinyRange.max) tinyRange.max = tinyRange.min;

            //    if (!(tiny || huge))
            //    {
            //        radius = random.Next(normalRange.min, normalRange.max);
            //    }
            //    else if (tiny && !huge)
            //    {
            //        radius = random.Next(tinyRange.min, tinyRange.max);
            //    }
            //    else if (huge && !tiny)
            //    {
            //        radius = random.Next(hugeRange.min, hugeRange.max); //needs more limits, but I got bored
            //    }
            //    else
            //    {
            //        radius = random.Next(100, 500);
            //    }
            //    themeNames = GSSettings.ThemeLibrary.Query(EThemeType.Planet, ThemeHeat, radius);
            //    themeName = themeNames[random.Next(0, themeNames.Count - 1)];
            //}
            //float position = orbitIndex / orbitCount;
            //int radius = Utils.ParsePlanetSize(GetStarPlanetSize(star));
            //bool gas = CalculateIsGas(star);
            //float rotationalPeriod = random.Next(60, 3600);

            //GSPlanet g = new GSPlanet(
            //    name,
            //    themeName,
            //    Utils.ParsePlanetSize(radius),                  // Radius
            //    -1,                                             // Orbit Radius
            //    (random.NextFloat() * 4 + random.NextFloat() * 5),  // Orbit Inclination
            //    -1,                                             // Orbit Period
            //    random.Next(359),                               // Phase
            //    random.NextFloat() * 20,                        // Obliquity
            //    rotationalPeriod,                               // Rotation Period
            //    random.Next(359),                               // Rotational Phase
            //     -1                                             // Luminosity
            //  );
            //g.OrbitRadius = CalculateNextAvailableOrbit(star, g);
            //g.OrbitalPeriod = Utils.CalculateOrbitPeriod(g.OrbitRadius);
            
            
            //if (random.NextDouble() < 0.02)
            //{
            //    g.OrbitalPeriod = -1 * g.OrbitalPeriod; // Clockwise Rotation
            //}
            //if (g.OrbitRadius < 1f && random.NextFloat() < 0.5f)
            //{
            //    g.RotationPeriod = g.OrbitalPeriod; // Tidal Lock
            //}
            //else if (g.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
            //{
            //    g.RotationPeriod = g.OrbitalPeriod / 2; // 1:2 Resonance
            //}
            //else if (g.OrbitRadius < 2f && random.NextFloat() < 0.1f)
            //{
            //    g.RotationPeriod = g.OrbitalPeriod / 4; // 1:4 Resonance
            //}

            //if ((moonCount < 6 && gas && availMoons > 10))
            //{
            //    moonCount = random.Next(moonCount, 7);
            //}

            //if (moonCount > 0 && availMoons > moonCount)
            //{
            //    g.Moons = new GSPlanets();
            //    for (var i = 0; i < moonCount; i++)
            //    {
            //        GSPlanet moon = RandomMoon(star, g, name + " - " + RomanNumbers.roman[i + 1], i, moonCount, heat);
            //        g.Moons.Add(moon);
            //    }
            //}
            //if (random.NextDouble() < 0.05) // Crazy Obliquity
            //{
            //    g.Obliquity = random.NextFloat(20f, 85f);
            //}
            //if (random.NextDouble() < 0.05) // Crazy Inclination
            //{
            //    g.OrbitInclination = random.NextFloat(20f, 85f);
            //}

            //g.Luminosity = -1;
            //g.Scale = scale;
            ////GS2.Warn($"Planet {g.Name} scale:{g.Scale}");
            //return g;
        //}

        private bool CalculateIsGas(GSStar star)
        {
            double gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }
        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = Utils.CalculateHabitableZone(star.luminosity);
            hz.min *= 10f; //Huge Stars need some tweaking :)
            hz.max *= 10f;
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < hz.max * 2) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }
    }
}