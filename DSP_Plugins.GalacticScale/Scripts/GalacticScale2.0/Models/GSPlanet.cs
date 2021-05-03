using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    //public class GSTheme
    //{
    //    public string name;
    //    public EPlanetType type = EPlanetType.Ocean;
    //    public int LDBThemeId = 1;
    //    public int algo = 0;
    //}

    public class GSPlanet
    {
        private string _name;
        private string _theme;
        private int _radius = -1;
        private float _orbitRadius = -1;
        private float _orbitInclination = -1;
        private float _orbitLongitude = -1;
        private float _orbitalPeriod = -1;
        private float _orbitPhase = -1;
        private float _obliquity = -1;
        private float _rotationPeriod = -1;
        private float _rotationPhase = -1;
        private float _luminosity = -1;
        [NonSerialized]
        public PlanetData planetData;
        private List<GSPlanet> _moons = new List<GSPlanet>();
        [SerializeField]
        public string Name { get => _name; set => _name = value; }
        [SerializeField]
        public string Theme { get => _theme != null ? _theme : GetTheme(); set => _theme = value; }
        [SerializeField]
        public int Radius { get => _radius < 0 ? GetRadius():_radius; set => _radius = value; }
        [SerializeField]
        public float OrbitRadius { get => _orbitRadius >= 0 ? _orbitRadius : GetOrbitRadius(); set => _orbitRadius = value; }
        [SerializeField]
        public float OrbitInclination { get => _orbitInclination >= 0 ? _orbitInclination : GetOrbitInclination(); set => _orbitInclination = value; }
        [SerializeField]
        public float OrbitLongitude { get => _orbitLongitude >= 0 ? _orbitLongitude : GetOrbitLongitude(); set => _orbitLongitude = value; }
        [SerializeField]
        public float OrbitalPeriod { get => _orbitalPeriod >= 0 ? _orbitalPeriod : GetOrbitalPeriod(); set => _orbitalPeriod = value; }
        [SerializeField]
        public float OrbitPhase { get => _orbitPhase >= 0 ? _orbitPhase : GetOrbitPhase(); set => _orbitPhase = value; }
        [SerializeField]
        public float Obliquity { get => _obliquity >= 0 ? _obliquity : GetObliquity(); set => _obliquity = value; }
        [SerializeField]
        public float RotationPeriod { get => _rotationPeriod >= 0 ? _rotationPeriod : GetRotationPeriod(); set => _rotationPeriod = value; }
        [SerializeField]
        public float RotationPhase { get => _rotationPhase >= 0 ? _rotationPhase : GetRotationPhase(); set => _rotationPhase = value; }
        [SerializeField]
        public float Luminosity { get => _luminosity >= 0 ? _luminosity : GetLuminosity(); set => _luminosity = value; }
        [SerializeField]
        public List<GSPlanet> Moons { get => _moons; set => _moons = value; }
        public int MoonCount { get
            {
                int count = 0;
                if (Moons == null) return 0;
                foreach (GSPlanet moon in Moons)
                {
                    count++;
                    count += moon.MoonCount;
                }
                return count;
            }
        }
        public int Seed = -1;
        
        public GSPlanet()
        {

        }

        public GSPlanet(string name) { 
            Name = name; 
        }
        public GSPlanet(string name, 
            string theme, 
            int radius, 
            float orbitRadius, 
            float orbitInclination, 
            float orbitLongitude,
            float orbitalPeriod,
            float orbitPhase,
            float obliquity,
            float rotationPeriod,
            float rotationPhase,
            float luminosity,
            List<GSPlanet> moons)
        {
            Name = name;
            Theme = theme;
            Radius = radius;
            OrbitRadius = orbitRadius;
            OrbitInclination = orbitInclination;
            OrbitLongitude = orbitLongitude;
            OrbitalPeriod = orbitalPeriod;
            OrbitPhase = orbitPhase;
            Obliquity = obliquity;
            RotationPeriod = rotationPeriod;
            RotationPhase = rotationPhase;
            Luminosity = luminosity;

        }
        public override string ToString()
        {
          return this.Name;
        }
        private string GetTheme()
        {
            _theme = "Mediterranian";
            return _theme;
        }
        private float GetLuminosity()
        {
            if (planetData == null) return 1f;
            float sunDistance = ((planetData.orbitAround != 0) ? planetData.orbitAroundPlanet.orbitRadius : planetData.orbitRadius);
            float luminosity = Mathf.Pow(planetData.star.lightBalanceRadius / (sunDistance + 0.01f), 0.6f);
            if (luminosity > 1f)
            {
                luminosity = Mathf.Log(luminosity) + 1f;
                luminosity = Mathf.Log(luminosity) + 1f;
                luminosity = Mathf.Log(luminosity) + 1f;
            }
            luminosity = Mathf.Round(luminosity * 100f) / 100f;
            return luminosity;
        }
        private int GetRadius()
        {
            _radius = 200;
            return _radius;
        }
        private float GetOrbitRadius ()
        {
            _orbitRadius = GS2.random.Next(10);
            return _orbitRadius;
        }
        private float GetOrbitInclination()
        {
            _orbitInclination = 0;
            return _orbitInclination;
        }
        private float GetOrbitLongitude()
        {
            _orbitLongitude = 0;
            return _orbitLongitude;
        }
        private float GetOrbitalPeriod()
        {
            _orbitalPeriod = 1000;
            return _orbitalPeriod;
        }
        private float GetOrbitPhase()
        {
            _orbitPhase = 0;
            return _orbitPhase;
        }
        private float GetObliquity()
        {
            _obliquity = 0;
            return _obliquity;
        }
        private float GetRotationPeriod()
        {
            _rotationPeriod = 1000;
            return _rotationPeriod;
        }
        private float GetRotationPhase()
        {
            _rotationPhase = 0;
            return _rotationPhase;
        }
      
    }
}