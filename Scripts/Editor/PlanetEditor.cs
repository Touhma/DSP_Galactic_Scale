using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Editor
{
    public class PlanetEditor : MonoBehaviour
    {
        public string planetName;
        public int radius;
        public float scale;
        public string theme;
        public float orbitRadius;
        public float orbitPeriod;
        public float orbitPhase;
        public float orbitInclination;
        public float luminosity;
        public float rotationPeriod;
        public float rotationPhase;
        public int seed;

        public List<PlanetEditor> moons;

        public InputField planetNameInput;
        public Slider radiusSlider;
        public InputField orbitRadiusInput;
        public InputField orbitPeriodInput;
        public Slider orbitPhaseSlider;
        public Slider orbitInclinationSlider;
        public InputField luminosityInput;
        public InputField rotationPeriodInput;
        public Slider rotationPhaseSlider;
        public InputField seedInput;
        public RectTransform moonsRect;
        public GSPlanet planet;


        public void save()
        {
            planet.Name = planetName;
            planet.Radius = radius;
            planet.Scale = scale;
            planet.OrbitRadius = orbitRadius;
            planet.OrbitalPeriod = orbitPeriod;
            planet.OrbitPhase = orbitPhase;
            planet.OrbitInclination = orbitInclination;
            planet.Luminosity = luminosity;
            planet.RotationPeriod = rotationPeriod;
            planet.RotationPhase = rotationPhase;
            planet.Seed = seed;
            planet.Theme = theme;

            foreach (var moon in moons) moon.save();
        }

        public void reset()
        {
            planetName = planet.Name;
            radius = planet.Radius;
            scale = planet.Scale;
            theme = planet.Theme;
            seed = planet.Seed;
            orbitRadius = planet.OrbitRadius;
            orbitPhase = planet.OrbitPhase;
            orbitPeriod = planet.OrbitalPeriod;
            orbitInclination = planet.OrbitInclination;
            luminosity = planet.Luminosity;
            rotationPeriod = planet.RotationPeriod;
            rotationPhase = planet.RotationPhase;
        }
    }
}