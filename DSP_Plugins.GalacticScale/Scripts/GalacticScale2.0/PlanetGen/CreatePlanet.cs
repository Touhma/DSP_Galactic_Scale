using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;
using System.Diagnostics;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static PlanetData CreatePlanet(ref StarData star, GSPlanet gsPlanet, PlanetData host = null)
        {
            bool isMoon = (host != null);
            int index = GSSettings.Stars[star.index].counter;
            PlanetData planetData = new PlanetData();
            int counter = GSSettings.Stars[star.index].counter;
            planetData.index = index;
            planetData.galaxy = galaxy;
            planetData.star = star;
            planetData.seed = 1;
            if (isMoon)
            {
                planetData.orbitAround = host.number;
                planetData.orbitAroundPlanet = host;

            }
            else planetData.orbitAround = 0;
            planetData.number = index + 1;
            planetData.id = star.id * 100 + index + 1;
            gsPlanets.Add(planetData.id, gsPlanet);
            string roman = "";
            if (isMoon) roman = Scripts.RomanNumbers.roman[host.number + 1] + " - ";
            roman += Scripts.RomanNumbers.roman[index + 1];
            planetData.name = (gsPlanet.Name != "") ? gsPlanet.Name : star.name + " " + roman;         
            planetData.orbitRadius = gsPlanet.OrbitRadius;        
            planetData.orbitInclination = gsPlanet.OrbitInclination;
            planetData.orbitLongitude = gsPlanet.OrbitLongitude;// 1+(index * (360/8));//
            planetData.orbitalPeriod = gsPlanet.OrbitalPeriod;
            planetData.orbitPhase = gsPlanet.OrbitPhase;//1+(index * (360/star.planetCount));
            planetData.obliquity = gsPlanet.Obliquity;
            //planetData.singularity |= gsPlanet.singularity.Layside;
            planetData.rotationPeriod = gsPlanet.RotationPeriod;
            planetData.rotationPhase = gsPlanet.RotationPhase;
            if (isMoon) planetData.sunDistance = star.planets[host.index].orbitRadius;
            else planetData.sunDistance = planetData.orbitRadius;
            planetData.radius = gsPlanet.Radius;
            planetData.segment = 5;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked2;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked4;
            //planetData.singularity |= gsPlanet.singularity.ClockwiseRotate;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked;
            planetData.runtimeOrbitRotation = Quaternion.AngleAxis(planetData.orbitLongitude, Vector3.up) * Quaternion.AngleAxis(planetData.orbitInclination, Vector3.forward); // moon gsPlanet.runtimeOrbitRotation = gsPlanet.orbitAroundPlanet.runtimeOrbitRotation * gsPlanet.runtimeOrbitRotation;
            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation * Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);
            //GS2.Log("Trying to apply theme " + gsPlanet.Theme);
            planetData.type = GSSettings.ThemeLibrary[gsPlanet.Theme].PlanetType;
            //GS2.Log("Applied");
            //Patch.Debug("Type set to " + planetData.type);
            planetData.scale = 1f;
            if (planetData.type == EPlanetType.Gas) planetData.scale = 10f;
            planetData.precision = (int)gsPlanet.Radius;
            gsPlanet.planetData = planetData;
            //GS2.Log("Getting luminosity for " + gsPlanet.Name + " planetData == null?" + (planetData == null));
            planetData.luminosity = gsPlanet.Luminosity;
            //Patch.Debug("Setting Theme " + gsPlanet.Theme + " " + gsPlanet.Theme.theme);
            //GS2.DumpObjectToJson(GS2.DataDir + "\\Planet" + planetData.id + ".json", gsPlanet);
            
            SetPlanetTheme(planetData, gsPlanet);
            //PlanetGen.SetPlanetTheme(planetData, star, gameDesc, 1, 0, ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.Next());
            star.galaxy.astroPoses[planetData.id].uRadius = planetData.realRadius;
            star.planets[counter] = planetData;
            //DebugPlanet(planetData);
            GSSettings.Stars[star.index].counter++;
            if (gsPlanet.MoonCount > 0) CreateMoons(ref planetData, gsPlanet);
            //Log("PLANET RADIUS "+planetData.radius);

            return planetData;
        }

        public static void CreateMoons(ref PlanetData planetData, GSPlanet planet)
        {
            for (var i = 0; i < planet.Moons.Count; i++)
            {
                PlanetData moon = CreatePlanet(ref planetData.star, planet.Moons[i], planetData);
                moon.orbitAroundPlanet = planetData;
                if (i > 1) planetData.singularity |= EPlanetSingularity.MultipleSatellites;
            }
        }
        public static void DebugPlanet(PlanetData planet)
        {
            BCE.console.WriteLine("Creating Planet " + planet.id, ConsoleColor.Red);
            BCE.console.WriteLine("Index " + planet.index, ConsoleColor.Green);
            BCE.console.WriteLine("OrbitAround " + planet.orbitAround, ConsoleColor.Green);
            BCE.console.WriteLine("Seed " + planet.seed, ConsoleColor.Green);
            BCE.console.WriteLine("Period " + planet.orbitalPeriod, ConsoleColor.Green);
            BCE.console.WriteLine("Inclination " + planet.orbitInclination, ConsoleColor.Green);
            BCE.console.WriteLine("Orbit Index " + planet.orbitIndex, ConsoleColor.Green);
            BCE.console.WriteLine("Orbit Longitude " + planet.orbitLongitude, ConsoleColor.Green);
            BCE.console.WriteLine("Orbit Phase " + planet.orbitPhase, ConsoleColor.Green);
            BCE.console.WriteLine("Orbit Radius " + planet.orbitRadius, ConsoleColor.Green);
            BCE.console.WriteLine("Precision " + planet.precision, ConsoleColor.Green);
            BCE.console.WriteLine("Radius " + planet.radius, ConsoleColor.Green);
            BCE.console.WriteLine("Rotation Period " + planet.rotationPeriod, ConsoleColor.Green);
            BCE.console.WriteLine("Rotation Phase " + planet.rotationPhase, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeLocalSunDirection " + planet.runtimeLocalSunDirection, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeOrbitPhase " + planet.runtimeOrbitPhase, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeOrbitRotation " + planet.runtimeOrbitRotation, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimePosition " + planet.runtimePosition, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimePositionNext " + planet.runtimePositionNext, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeRotation " + planet.runtimeRotation, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeRotationNext " + planet.runtimeRotationNext, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeRotationPhase " + planet.runtimeRotationPhase, ConsoleColor.Green);
            BCE.console.WriteLine("RuntimeSystemRotation " + planet.runtimeSystemRotation, ConsoleColor.Green);
            BCE.console.WriteLine("SingularityString " + planet.singularityString, ConsoleColor.Green);
            BCE.console.WriteLine("Sundistance " + planet.sunDistance, ConsoleColor.Green);
            BCE.console.WriteLine("TemperatureBias " + planet.temperatureBias, ConsoleColor.Green);
            BCE.console.WriteLine("Theme " + planet.theme, ConsoleColor.Green);
            BCE.console.WriteLine("Type " + planet.type, ConsoleColor.Green);
            BCE.console.WriteLine("uPosition " + planet.uPosition, ConsoleColor.Green);
            BCE.console.WriteLine("uPositionNext " + planet.uPositionNext, ConsoleColor.Green);
            BCE.console.WriteLine("Wanted " + planet.wanted, ConsoleColor.Green);
            BCE.console.WriteLine("WaterHeight " + planet.waterHeight, ConsoleColor.Green);
            BCE.console.WriteLine("WaterItemID " + planet.waterItemId, ConsoleColor.Green);
            BCE.console.WriteLine("WindStrength " + planet.windStrength, ConsoleColor.Green);
            BCE.console.WriteLine("Number " + planet.number, ConsoleColor.Green);
            BCE.console.WriteLine("Obliquity " + planet.obliquity, ConsoleColor.Green);
            BCE.console.WriteLine("Name " + planet.name, ConsoleColor.Green);
            BCE.console.WriteLine("mod_y " + planet.mod_y, ConsoleColor.Green);
            BCE.console.WriteLine("mod_x " + planet.mod_x, ConsoleColor.Green);
            BCE.console.WriteLine("Luminosity " + planet.luminosity, ConsoleColor.Green);
            BCE.console.WriteLine("Levelized " + planet.levelized, ConsoleColor.Green);
            BCE.console.WriteLine("Land% " + planet.landPercent, ConsoleColor.Green);
            BCE.console.WriteLine("Ion Height " + planet.ionHeight, ConsoleColor.Green);
            BCE.console.WriteLine("HabitableBias " + planet.habitableBias, ConsoleColor.Green);
            BCE.console.WriteLine("GasTotalHeat " + planet.gasTotalHeat, ConsoleColor.Green);
            BCE.console.WriteLine("BirthResourcePoint1 " + planet.birthResourcePoint1, ConsoleColor.Green);
            BCE.console.WriteLine("BirthResourcePoint0 " + planet.birthResourcePoint0, ConsoleColor.Green);
            BCE.console.WriteLine("BirthPoint " + planet.birthPoint, ConsoleColor.Green);
            BCE.console.WriteLine("Algo ID " + planet.algoId, ConsoleColor.Green);
            BCE.console.WriteLine("---------------------", ConsoleColor.Red);
        }
    }
}