using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static PlanetData CreatePlanet(ref StarData star, GSPlanet gsPlanet, Random random, PlanetData host = null)
        {
            if (GSSettings.Stars[star.index].counter > 99)
            {
                Error($"Create Planet failed: Star '{star.name}' already has 99 bodies");
                return null;
            }
            // var highStopwatch = new HighStopwatch();

            // highStopwatch.Begin();

            // Log("CreatePlanet|" + gsPlanet.Name);
            var isMoon = host != null;
            if (GSSettings.Stars[star.index] == null)
                Error($"Star Index {star.index} does not exist in GSSettings.Stars");

            var index = GSSettings.Stars[star.index].counter;
            // GS2.Log("Creating PlanetData");
            var planet = new PlanetData();
            var counter = GSSettings.Stars[star.index].counter;
            planet.index = index;
            planet.galaxy = galaxy;
            planet.star = star;
            //if (gsPlanet.Seed < 0) gsPlanet.Seed = random.Next();
            planet.seed = gsPlanet.Seed = gsPlanet.Seed < 0 ? random.Next() : gsPlanet.Seed;
            if (isMoon)
            {
                planet.orbitAround = host.number;
                planet.orbitAroundPlanet = host;
            }
            else
            {
                planet.orbitAround = 0;
            }

            planet.number = index + 1;
            planet.id = star.id * 100 + index + 1;
            if (!gsPlanets.ContainsKey(planet.id)) gsPlanets.Add(planet.id, gsPlanet);
            else gsPlanets[planet.id] = gsPlanet;
            // Log("Setting Roman");
            var roman = "";

            if (isMoon)
            {
                if (RomanNumbers.roman.Length <= host.number + 1)
                    Error($"Roman Number Conversion Error for {host.number + 1}");

                roman = RomanNumbers.roman[host.number + 1] + " - ";
            }

            // Log($"Start1 of {planet.name} creation took {highStopwatch.duration:F5} s\r\n");
            // highStopwatch.Begin();
            if (RomanNumbers.roman.Length <= index + 1) Error($"Roman Number Conversion Error for {index + 1}");

            roman += RomanNumbers.roman[index + 1];
            planet.name = gsPlanet.Name != "" ? gsPlanet.Name : star.name + " " + roman;
            // GS2.Log($"Creating Planet {planet.name} with seed:{planet.seed}");
            planet.orbitRadius = gsPlanet.OrbitRadius;
            if (planet.orbitRadius < 0)
            {
                Warn($"Planet {planet.name} orbit broken. {star.type} {star.spectr}");
                planet.orbitRadius = random.NextFloat(1, 50);
            }

            planet.orbitInclination = gsPlanet.OrbitInclination;
            planet.orbitLongitude = gsPlanet.OrbitLongitude; // 1+(index * (360/8));//
            planet.orbitalPeriod = gsPlanet.OrbitalPeriod;
            planet.orbitPhase = gsPlanet.OrbitPhase; //1+(index * (360/star.planetCount));
            planet.obliquity = gsPlanet.Obliquity;
            //planetData.singularity |= gsPlanet.singularity.Layside;
            planet.rotationPeriod = gsPlanet.RotationPeriod;
            planet.rotationPhase = gsPlanet.RotationPhase;
            planet.sunDistance = GetSunDistance(GSSettings.Stars[star.index], gsPlanet, host);

            planet.radius = gsPlanet.Radius;
            planet.segment = 5;
            var segments = (int)(planet.radius / 4f + 0.1f) * 4;
            if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) SetLuts(segments, planet.radius);
            PatchOnUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            // Log($"Start2 of {planet.name} creation took {highStopwatch.duration:F5} s\r\n");
            // highStopwatch.Begin();
            planet.runtimeOrbitRotation = Quaternion.AngleAxis(planet.orbitLongitude, Vector3.up) * Quaternion.AngleAxis(planet.orbitInclination, Vector3.forward); // moon gsPlanet.runtimeOrbitRotation = gsPlanet.orbitAroundPlanet.runtimeOrbitRotation * gsPlanet.runtimeOrbitRotation;
            planet.runtimeSystemRotation = planet.runtimeOrbitRotation * Quaternion.AngleAxis(planet.obliquity, Vector3.forward);
            //GS2.Log("Trying to apply theme " + gsPlanet.Theme);
            // Log($"OrbitRotation for {planet.name} took {highStopwatch.duration:F5} s\r\n");
            // highStopwatch.Begin();

            planet.type = GSSettings.ThemeLibrary.Find(gsPlanet.Theme).PlanetType;
            //GS2.Log("Applied");
            //Patch.Debug("Type set to " + planetData.type);
            planet.scale = 1f;
            if (planet.type == EPlanetType.Gas) planet.scale = 10f;

            if (gsPlanet.Scale > 0) planet.scale = gsPlanet.Scale;

            planet.precision = gsPlanet.Radius;
            gsPlanet.planetData = planet;
            //GS2.Log("Getting luminosity for " + gsPlanet.Name + " planetData == null?" + (planetData == null));
            planet.luminosity = gsPlanet.Luminosity;
            //Patch.Debug("Setting Theme " + gsPlanet.Theme + " " + gsPlanet.Theme.theme);
            //GS2.DumpObjectToJson(GS2.DataDir + "\\Planet" + planetData.id + ".json", gsPlanet);
            //Log("Setting Theme|"+gsPlanet.Name);
            // Log($"ThemeLibrary? for {planet.name} took {highStopwatch.duration:F5} s\r\n");
            // highStopwatch.Begin();
            SetPlanetTheme(planet, gsPlanet);
            // Log($"Theme Set for {planet.name} took {highStopwatch.duration:F5} s\r\n");
            // highStopwatch.Begin();
            //PlanetGen.SetPlanetTheme(planetData, star, gameDesc, 1, 0, ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.Next());
            if (star.galaxy.astroPoses == null) Error("Astroposes array does not exist");

            if (star.galaxy.astroPoses.Length <= planet.id)
                Error($"Astroposes does not contain index {planet.id} when trying to set planet uRadius");
            //GS2.Warn($"Setting astropose for {planet.name}");
            star.galaxy.astroPoses[planet.id].uRadius = planet.realRadius;
            if (star.planets.Length <= counter)
                Error($"star.planets length of {star.planets.Length} <= counter {counter}");

            star.planets[counter] = planet;
            // DebugPlanet(planet);
            if (GSSettings.Stars.Count <= star.index) Error($"GSSettings.Stars[{star.index}] does not exist");

            GSSettings.Stars[star.index].counter++;
            if (gsPlanet.MoonsCount > 0) CreateMoons(ref planet, gsPlanet, random);
            //Log("PLANET RADIUS "+planetData.radius);
            //Log("End|" + gsPlanet.Name);
            //GS2.Log("Setting Singularities");
            //GS2.Log($"Added Planet {planet.name} to galaxy with id:{planet.id} and index:{planet.index} star:{planet.star.name} with id:{planet.star.id} rotation:{planet.rotationPeriod} orbit:{planet.orbitalPeriod} obliq:{planet.obliquity} bodies:{gsPlanet.Bodies.Count}");
            if (Math.Abs(planet.orbitalPeriod - planet.rotationPeriod) < 1f) //GS2.Log("Setting TidalLock"); 
                planet.singularity |= EPlanetSingularity.TidalLocked;
            if (Math.Abs(planet.orbitalPeriod - planet.rotationPeriod * 2) < 1f) //GS2.Log("Setting TidalLock2"); 
                planet.singularity |= EPlanetSingularity.TidalLocked2;
            if (Math.Abs(planet.orbitalPeriod - planet.rotationPeriod * 4) < 1f) //GS2.Log("Setting TidalLock4");
                planet.singularity |= EPlanetSingularity.TidalLocked4;
            if (gsPlanet.Bodies.Count > 2) //GS2.Log("Setting Multisatellite");
                planet.singularity |= EPlanetSingularity.MultipleSatellites;
            if (planet.obliquity > 75 || planet.obliquity < -75) //GS2.Log("Setting LaySide"); 
                planet.singularity |= EPlanetSingularity.LaySide;

            if (planet.rotationPeriod < 0)
                //GS2.Log("Setting ReverseRotation");
                planet.singularity |= EPlanetSingularity.ClockwiseRotate;

            // GS2.Warn(planet.singularityString + " " + planet.singularity);
            // Log($"{planet.name} created in {highStopwatch.duration:F5} s\r\n");
            return planet;
        }

        public static float GetSunDistance(GSStar star, GSPlanet planet, PlanetData host)
        {
            if (host == null) return planet.OrbitRadius;

            if (IsPlanetOfStar(star, GetGSPlanet(host)))
                return host.orbitRadius;
            foreach (var p in star.Planets)
            foreach (var b in p.Bodies)
                if (planet == b)
                    return p.OrbitRadius;
            Error($"Cannot Get Sun Distance for planet {planet.Name}");
            return 100f;
        }

        public static void CreateMoons(ref PlanetData planetData, GSPlanet planet, Random random)
        {
            for (var i = 0; i < planet.Moons.Count; i++)
            {
                if (GSSettings.Stars[planetData.star.index].counter > 99)
                {
                    Error($"Create Planet failed: Star '{planetData.star.name}' already has 99 bodies");
                    return;
                }

                var moon = CreatePlanet(ref planetData.star, planet.Moons[i], random, planetData);
                if (moon == null)
                {
                    Error($"Creating moons for planet '{planet.Name}' failed. No moon returned");
                    return;
                }

                moon.orbitAroundPlanet = planetData;
            }
        }

        public static void DebugPlanet(PlanetData planet)
        {
            BCE.Console.WriteLine("Creating Planet " + planet.id, ConsoleColor.Red);
            BCE.Console.WriteLine("Index " + planet.index, ConsoleColor.Green);
            BCE.Console.WriteLine("OrbitAround " + planet.orbitAround, ConsoleColor.Green);
            BCE.Console.WriteLine("Seed " + planet.seed, ConsoleColor.Green);
            BCE.Console.WriteLine("Period " + planet.orbitalPeriod, ConsoleColor.Green);
            BCE.Console.WriteLine("Inclination " + planet.orbitInclination, ConsoleColor.Green);
            BCE.Console.WriteLine("Orbit Index " + planet.orbitIndex, ConsoleColor.Green);
            BCE.Console.WriteLine("Orbit Longitude " + planet.orbitLongitude, ConsoleColor.Green);
            BCE.Console.WriteLine("Orbit Phase " + planet.orbitPhase, ConsoleColor.Green);
            BCE.Console.WriteLine("Orbit Radius " + planet.orbitRadius, ConsoleColor.Green);
            BCE.Console.WriteLine("Precision " + planet.precision, ConsoleColor.Green);
            BCE.Console.WriteLine("Radius " + planet.radius, ConsoleColor.Green);
            BCE.Console.WriteLine("Rotation Period " + planet.rotationPeriod, ConsoleColor.Green);
            BCE.Console.WriteLine("Rotation Phase " + planet.rotationPhase, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeLocalSunDirection " + planet.runtimeLocalSunDirection, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeOrbitPhase " + planet.runtimeOrbitPhase, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeOrbitRotation " + planet.runtimeOrbitRotation, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimePosition " + planet.runtimePosition, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimePositionNext " + planet.runtimePositionNext, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeRotation " + planet.runtimeRotation, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeRotationNext " + planet.runtimeRotationNext, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeRotationPhase " + planet.runtimeRotationPhase, ConsoleColor.Green);
            BCE.Console.WriteLine("RuntimeSystemRotation " + planet.runtimeSystemRotation, ConsoleColor.Green);
            BCE.Console.WriteLine("SingularityString " + planet.singularityString, ConsoleColor.Green);
            BCE.Console.WriteLine("Sundistance " + planet.sunDistance, ConsoleColor.Green);
            BCE.Console.WriteLine("TemperatureBias " + planet.temperatureBias, ConsoleColor.Green);
            BCE.Console.WriteLine("Theme " + planet.theme, ConsoleColor.Green);
            BCE.Console.WriteLine("Type " + planet.type, ConsoleColor.Green);
            BCE.Console.WriteLine("uPosition " + planet.uPosition, ConsoleColor.Green);
            BCE.Console.WriteLine("uPositionNext " + planet.uPositionNext, ConsoleColor.Green);
            BCE.Console.WriteLine("Wanted " + planet.wanted, ConsoleColor.Green);
            BCE.Console.WriteLine("WaterHeight " + planet.waterHeight, ConsoleColor.Green);
            BCE.Console.WriteLine("WaterItemID " + planet.waterItemId, ConsoleColor.Green);
            BCE.Console.WriteLine("WindStrength " + planet.windStrength, ConsoleColor.Green);
            BCE.Console.WriteLine("Number " + planet.number, ConsoleColor.Green);
            BCE.Console.WriteLine("Obliquity " + planet.obliquity, ConsoleColor.Green);
            BCE.Console.WriteLine("Name " + planet.name, ConsoleColor.Green);
            BCE.Console.WriteLine("mod_y " + planet.mod_y, ConsoleColor.Green);
            BCE.Console.WriteLine("mod_x " + planet.mod_x, ConsoleColor.Green);
            BCE.Console.WriteLine("Luminosity " + planet.luminosity, ConsoleColor.Green);
            BCE.Console.WriteLine("Levelized " + planet.levelized, ConsoleColor.Green);
            BCE.Console.WriteLine("Land% " + planet.landPercent, ConsoleColor.Green);
            BCE.Console.WriteLine("Ion Height " + planet.ionHeight, ConsoleColor.Green);
            BCE.Console.WriteLine("HabitableBias " + planet.habitableBias, ConsoleColor.Green);
            BCE.Console.WriteLine("GasTotalHeat " + planet.gasTotalHeat, ConsoleColor.Green);
            BCE.Console.WriteLine("BirthResourcePoint1 " + planet.birthResourcePoint1, ConsoleColor.Green);
            BCE.Console.WriteLine("BirthResourcePoint0 " + planet.birthResourcePoint0, ConsoleColor.Green);
            BCE.Console.WriteLine("BirthPoint " + planet.birthPoint, ConsoleColor.Green);
            BCE.Console.WriteLine("Algo ID " + planet.algoId, ConsoleColor.Green);
            BCE.Console.WriteLine("---------------------", ConsoleColor.Red);
        }
    }
}