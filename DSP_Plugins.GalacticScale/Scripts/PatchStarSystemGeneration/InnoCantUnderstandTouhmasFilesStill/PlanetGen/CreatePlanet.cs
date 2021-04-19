using System;
using System.Collections.Generic;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static PlanetData CreatePlanet(StarData star, int index)
        {
            System.Random ran = new System.Random(index);
            float r = (float) ran.NextDouble();
            PlanetData planetData = new PlanetData();
            planet planet = settings.Stars[star.index].Planets[index];
            planetData.index = index;
            planetData.galaxy = galaxy;
            planetData.star = star;
            planetData.seed = ran.Next();
            planetData.orbitAround = 0; // moon use id of planet
            planetData.orbitIndex =  index;
            planetData.number = index;
            planetData.id = star.id * 100 + index + 1;
            string roman = GalacticScale.Scripts.RomanNumbers.roman[index];
            planetData.name = (planet.Name == "") ? planet.Name : star.name + " " + roman;
            planetData.orbitRadius = 0.25f;//planet.orbitRadius;
            planetData.orbitInclination = 0;//planet.orbitInclination;
            planetData.orbitLongitude = 1f;// 1+(index * (360/8));// planet.orbitLongitude;
            planetData.orbitalPeriod = 1100f;//planet.orbitalPeriod;
            planetData.orbitPhase = 1+(index * (360/star.planetCount));//planet.orbitPhase;

            Patch.Debug(planetData.orbitPhase + " <- phase");
            planetData.obliquity = 0;//planet.obliquity;
            //planetData.singularity |= planet.singularity.Layside;
            planetData.rotationPeriod = r * 1100f * r;//planet.rotationPeriod;
            planetData.rotationPhase = (float)(r * 360.0);
            planetData.sunDistance = planetData.orbitRadius; //moon use planet.orbitAroundPlanet.orbitalRadius;
            planetData.scale = 1f;
            planetData.radius = 200;//planet.radius;
            planetData.segment = 5;
            //planetData.singularity |= planet.singularity.TidalLocked;
            //planetData.singularity |= planet.singularity.TidalLocked2;
            //planetData.singularity |= planet.singularity.TidalLocked4;
            //planetData.singularity |= planet.singularity.ClockwiseRotate;
            //planetData.singularity |= planet.singularity.TidalLocked;
            planetData.runtimeOrbitRotation = Quaternion.AngleAxis(planetData.orbitLongitude, Vector3.up) * Quaternion.AngleAxis(planetData.orbitInclination, Vector3.forward); // moon planet.runtimeOrbitRotation = planet.orbitAroundPlanet.runtimeOrbitRotation * planet.runtimeOrbitRotation;
            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation * Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);
            planetData.type = EPlanetType.Ocean;//planet.type;
            if (planetData.type == EPlanetType.Gas) planetData.scale = 10f;
            planetData.precision = (int)planetData.radius;
            //planetData.temperatureBias = planet.temperatureBias;
            planetData.luminosity = 10;////planet.luminosity;
            PlanetGen.SetPlanetTheme(planetData, star, gameDesc, 0, 0, ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.NextDouble(), ran.Next());
            star.galaxy.astroPoses[planetData.id].uRadius = planetData.realRadius;
            return planetData;
        }
    }
}