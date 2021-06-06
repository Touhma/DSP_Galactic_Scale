using System;
using UnityEngine;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static PlanetData CreatePlanet(ref StarData star, GSPlanet gsPlanet, PlanetData host = null)
        {
            if (GSSettings.Stars[star.index].counter > 99)
            {
                Error($"Create Planet failed: Star '{star.name}' already has 99 bodies");
                return null;
            }
            //Log("CreatePlanet|" + gsPlanet.Name);
            bool isMoon = (host != null);
            if (GSSettings.Stars[star.index] == null) Error($"Star Index {star.index} does not exist in GSSettings.Stars");
            int index = GSSettings.Stars[star.index].counter;
            //GS2.Log("Creating PlanetData");
            PlanetData planet = new PlanetData();
            int counter = GSSettings.Stars[star.index].counter;
            planet.index = index;
            planet.galaxy = galaxy;
            planet.star = star;
            planet.seed = gsPlanet.Seed;
            if (isMoon)
            {
                planet.orbitAround = host.number;
                planet.orbitAroundPlanet = host;

            }
            else planet.orbitAround = 0;
            planet.number = index + 1;
            planet.id = star.id * 100 + index + 1;
            gsPlanets.Add(planet.id, gsPlanet);
            //Log("Setting Roman");
            string roman = "";
            
            if (isMoon) {
                if (RomanNumbers.roman.Length <= host.number + 1) Error($"Roman Number Conversion Error for {host.number + 1}");
                roman = RomanNumbers.roman[host.number + 1] + " - ";
            }
            if (RomanNumbers.roman.Length <= index + 1) Error($"Roman Number Conversion Error for {index + 1}");
            roman += RomanNumbers.roman[index + 1];
            planet.name = (gsPlanet.Name != "") ? gsPlanet.Name : star.name + " " + roman;         
            planet.orbitRadius = gsPlanet.OrbitRadius;        
            planet.orbitInclination = gsPlanet.OrbitInclination;
            //planet.orbitLongitude = gsPlanet.OrbitLongitude;// 1+(index * (360/8));//
            planet.orbitalPeriod = gsPlanet.OrbitalPeriod;
            planet.orbitPhase = gsPlanet.OrbitPhase;//1+(index * (360/star.planetCount));
            planet.obliquity = gsPlanet.Obliquity;
            //planetData.singularity |= gsPlanet.singularity.Layside;
            planet.rotationPeriod = gsPlanet.RotationPeriod;
            planet.rotationPhase = gsPlanet.RotationPhase;
            if (isMoon)
            {
                if (star.planets.Length <= host.index) Error($"star.planets does not contain index {host.index}");
                planet.sunDistance = star.planets[host.index].orbitRadius;
            }
            else planet.sunDistance = planet.orbitRadius;
            planet.radius = gsPlanet.Radius;
            planet.segment = 5;
            
            //planetData.singularity |= gsPlanet.singularity.TidalLocked;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked2;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked4;
            //planetData.singularity |= gsPlanet.singularity.ClockwiseRotate;
            //planetData.singularity |= gsPlanet.singularity.TidalLocked;
            planet.runtimeOrbitRotation = Quaternion.AngleAxis(planet.orbitLongitude, Vector3.up) * Quaternion.AngleAxis(planet.orbitInclination, Vector3.forward); // moon gsPlanet.runtimeOrbitRotation = gsPlanet.orbitAroundPlanet.runtimeOrbitRotation * gsPlanet.runtimeOrbitRotation;
            planet.runtimeSystemRotation = planet.runtimeOrbitRotation * Quaternion.AngleAxis(planet.obliquity, Vector3.forward);
            //GS2.Log("Trying to apply theme " + gsPlanet.Theme);
            planet.type = GSSettings.ThemeLibrary.Find(gsPlanet.Theme).PlanetType;
            //GS2.Log("Applied");
            //Patch.Debug("Type set to " + planetData.type);
            planet.scale = 1f;
            if (planet.type == EPlanetType.Gas) planet.scale = 10f;
            if (gsPlanet.Scale > 0) planet.scale = gsPlanet.Scale;
            planet.precision = (int)gsPlanet.Radius;
            gsPlanet.planetData = planet;
            //GS2.Log("Getting luminosity for " + gsPlanet.Name + " planetData == null?" + (planetData == null));
            planet.luminosity = gsPlanet.Luminosity;
            //Patch.Debug("Setting Theme " + gsPlanet.Theme + " " + gsPlanet.Theme.theme);
            //GS2.DumpObjectToJson(GS2.DataDir + "\\Planet" + planetData.id + ".json", gsPlanet);
            //Log("Setting Theme|"+gsPlanet.Name);
            SetPlanetTheme(planet, gsPlanet);
            //PlanetGen.SetPlanetTheme(planetData, star, gameDesc, 1, 0, ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.Next());
            if (star.galaxy.astroPoses == null) Error("Astroposes array does not exist");
            if (star.galaxy.astroPoses.Length <= planet.id) Error($"Astroposes does not contain index {planet.id} when trying to set planet uRadius");
            star.galaxy.astroPoses[planet.id].uRadius = planet.realRadius;
            if (star.planets.Length <= counter) Error($"star.planets length of {star.planets.Length} <= counter {counter}");
            star.planets[counter] = planet;
            //DebugPlanet(planetData);
            if (GSSettings.Stars.Count <= star.index) Error($"GSSettings.Stars[{star.index}] does not exist");
            GSSettings.Stars[star.index].counter++;
            if (gsPlanet.MoonCount > 0) CreateMoons(ref planet, gsPlanet);
            //Log("PLANET RADIUS "+planetData.radius);
            //Log("End|" + gsPlanet.Name);
            if (planet.orbitalPeriod == planet.rotationPeriod) planet.singularity |= EPlanetSingularity.TidalLocked;
            if (planet.obliquity > 75 || planet.obliquity < -75) planet.singularity |= EPlanetSingularity.LaySide;
            if (planet.rotationPeriod < 0) planet.singularity |= EPlanetSingularity.ClockwiseRotate;
            return planet;
        }

        public static void CreateMoons(ref PlanetData planetData, GSPlanet planet)
        {
            for (var i = 0; i < planet.Moons.Count; i++)
            {
                if (GSSettings.Stars[planetData.star.index].counter > 99)
                {
                    Error($"Create Planet failed: Star '{planetData.star.name}' already has 99 bodies");
                    return;
                }
                PlanetData moon = CreatePlanet(ref planetData.star, planet.Moons[i], planetData);
                if (moon == null)
                {
                    Error($"Creating moons for planet '{planet.Name}' failed. No moon returned");
                    return;
                }
                
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