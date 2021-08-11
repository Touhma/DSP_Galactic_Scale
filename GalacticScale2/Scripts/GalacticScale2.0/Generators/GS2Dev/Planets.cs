using System;
using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;
using static GalacticScale.RomanNumbers;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void GeneratePlanets()
        {
            // var highStopwatch = new HighStopwatch();
            foreach (var star in GSSettings.Stars) GeneratePlanetsForStar(star);
            // GS2.Log($"Planet Creation Finished in {highStopwatch.duration}");

        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            // var highStopwatch = new HighStopwatch();
            star.Planets = new GSPlanets();
            var starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            var moonChance = GetMoonChanceForStar(star);
            var moonMoonChance = preferences.GetFloat("chanceMoonMoon", 5f);
            var moonCount = 0;
            var secondaryMoonCount = 0;
            var protos = new List<ProtoPlanet>();
            var moons = new List<ProtoPlanet>();
            if (star == birthStar)
            {
                protos.Add(new ProtoPlanet() { gas = false, radius = preferences.GetInt("birthPlanetSize", 200), birth = true });
            }
            else protos.Add(new ProtoPlanet {gas = CalculateIsGas(star), radius = GetStarPlanetSize(star)});
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
                if (preferences.GetBool("secondarySatellites") && random.NextPick(moonMoonChance) && i != 0)
                {
                    secondaryMoonCount++; // i != 0 Make sure we have at least one actual satellite
                }
                else
                {
                    ProtoPlanet randomProto;
                    List<ProtoPlanet> gasProtos = new List<ProtoPlanet>();
                    List<ProtoPlanet> telProtos = new List<ProtoPlanet>();
                    foreach (var pp in protos)
                    {
                        if (pp.gas) gasProtos.Add(pp);
                        else telProtos.Add(pp);
                    }
                    if (gasProtos.Count > 0 && random.NextPick(preferences.GetInt("moonBias", 50)/100f))
                    {
                        //Gas
                        randomProto = random.Item(gasProtos);

                    } else if (telProtos.Count > 0)
                    {
                        randomProto = random.Item(telProtos);
                    } else randomProto = random.Item(protos);
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
                        Warn("GAS AND NOT >= 50");
                var planet = new GSPlanet(star.Name + "-Planet", null, proto.radius, -1, -1, -1, -1, -1, -1, -1, -1);
                if (proto.birth) birthPlanet = planet;
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

            CreateMoonOrbits(star);
            AssignPlanetOrbits(star);
            SelectPlanetThemes(star);
            FudgeNumbersForPlanets(star); // Probably want to revisit this :)
            // GS2.Log($"Planet Creation for {star} Finished in {highStopwatch.duration}");
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                if (IsPlanetOfStar(star, body))
                {

                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                }
                if (!IsPlanetOfStar(star, body))
                {
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 50f;
                }
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                var starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination", -1);
                // GS2.Log($"StarInc {starInc}");
                if (IsPlanetOfStar(star, body) && starInc > -1)
                {
                    // GS2.Warn($"SETTING starInc Orbit Inclination of {star.Name} to {starInc}");
                    body.OrbitInclination = random.NextFloat(starInc);
                }
                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f)*random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                if (IsPlanetOfStar(star, body) && body.OrbitRadius < preferences.GetFloat("innerPlanetDistance", 1f) && (random.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets", false)) )
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f && random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f && random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (starInc == -1 && random.NextDouble() < 0.05) // Crazy Inclination
                {
                    // GS2.Warn("Setting Crazy Inclination for " + star.Name);
                    body.OrbitInclination = random.NextFloat(20f, 85f);
                }

                // Force inclinations for testing
                // body.OrbitInclination = 0f;
                // body.OrbitPhase = 0f;
                // body.OrbitalPeriod = 10000000f;
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int moonIndex)
        {
            var moons = planet.Moons;
            if (moonIndex == 0) return planet.RadiusAU + moons[moonIndex].SystemRadius;
            return moons[moonIndex - 1].SystemRadius + moons[moonIndex - 1].OrbitRadius + moons[moonIndex].SystemRadius;
        }

        private void CreateMoonOrbits(GSStar star)
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

                // var pOrbit = SelectOrbit(star, planet);
                // if (planetIndex == 0) pOrbit = star.RadiusAU * 1.5f + planet.SystemRadius;
                // else
                //     pOrbit = star.Planets[planetIndex - 1].SystemRadius + GetOrbitGap(star) +
                //              star.Planets[planetIndex - 1].OrbitRadius + planet.SystemRadius;
                //
                // planet.OrbitRadius = pOrbit.radius;
                //Warn($"{planet.Name} orbitRadius:{planet.OrbitRadius} systemRadius:{planet.SystemRadius} Planet Radius(AU):{planet.Radius}({planet.RadiusAU}) Planet Scale:{planet.Scale}");
                //if (planetIndex != 0) Warn($"pOrbit = {star.Planets[planetIndex - 1].SystemRadius} + {GetOrbitGap(star)} + {star.Planets[planetIndex - 1].OrbitRadius} + {planet.SystemRadius};");
                // planet.OrbitalPeriod = Utils.CalculateOrbitPeriod(planet.OrbitRadius);
            }
            //Orbits should be set.
        }



        private float GetMoonOrbit()
        {
            return 0.01f + random.NextFloat(0f, 0.05f);
        }

// public float GetOrbitGap(GSStar star)
//         {
//             (float min, float max) hz = Utils.CalculateHabitableZone(star.luminosity);
//             float pCount = star.Planets.Count;
//             var maxOrbitByRadius = Mathf.Sqrt(star.radius);
//             var maxOrbitByHabitableZone = 30f * hz.min;
//             var maxOrbitByPlanetCount = 50f * pCount / 99f;
//             var maxOrbit = Mathf.Max(maxOrbitByPlanetCount, maxOrbitByRadius, maxOrbitByHabitableZone);
//             var averageOrbit = maxOrbit / pCount;
//             var result = ClampedNormal(0.1f, maxOrbit / pCount, GetSystemDensityBiasForStar(star));
//             Warn(
//                 $"Getting Orbit Gap for Star {star.Name} with {pCount} planets. Avg:{averageOrbit} MaxbyRadius:{maxOrbitByRadius} MaxbyPCount:{maxOrbitByPlanetCount} MaxByHZ:{maxOrbitByHabitableZone} Max:{maxOrbit} HabitableZone:{hz.Item1 * 10f}:{hz.Item2 * 10f} = {result}");
//             return result; // random.NextFloat(0.1f, 2f * averageOrbit);
//         }
//
         public void SelectPlanetThemes(GSStar star)
         {
             foreach (var planet in star.Planets)
             {
                 if (planet == birthPlanet)
                 {
                     var habitableTheme  = GSSettings.ThemeLibrary.Query(random, EThemeType.Telluric, EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200));
                     planet.Theme = habitableTheme;
                     planet.Scale = 1f;
                     continue;
                 }
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

        public bool CalculateIsGas(GSStar star)
        {
            var gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            // (float min, float max) hz = Utils.CalculateHabitableZone(star.luminosity);
            // GS2.Warn($"Habitable zone for {star.Name} is {hz.min} - {hz.max}");
            //Warn($"HZ for {star.Name} {hz.min}-{hz.max}");
            (float min, float max) hz = (star.genData.Get("minHZ"), star.genData.Get("maxHZ"));
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < hz.max * 2) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }

public class ProtoPlanet
        {
            public readonly List<ProtoPlanet> moons = new List<ProtoPlanet>();
            public bool gas;
            public int radius;
            public bool birth = false;
        }
    }
}