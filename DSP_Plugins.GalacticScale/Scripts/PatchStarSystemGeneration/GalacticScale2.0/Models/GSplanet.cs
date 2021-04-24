using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSTheme
    {
        public string name;
        public EPlanetType type;
        public ThemeProto theme;
        public int algo = 0;
    }

    public class GSplanet
    {
        private string _name;
        private GSTheme _theme;
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
        private List<GSplanet> _moons;
        [SerializeField]
        public string Name { get => _name; set => _name = value; }
        public GSTheme Theme { get => _theme != null ? _theme : GetTheme(); set => _theme = value; }
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
        public List<GSplanet> Moons { get => _moons; set => _moons = value; }
        public int MoonCount { get
            {
                int count = 0;
                if (Moons == null) return 0;
                foreach (GSplanet moon in Moons)
                {
                    count++;
                    count += moon.MoonCount;
                }
                return count;

            }
        }
        public GSplanet()
        {

        }
        public GSplanet(string name) { }
        public GSplanet(string name, 
            GSTheme theme, 
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
            List<GSplanet> moons)
        {
            this.Name = name;
            
            this.Theme = theme;
            
            

        }
        public override string ToString()
        {
          return this.Name;
        }
        private GSTheme GetTheme()
        {
            _theme = GS2.planetThemes["Mediterranian"];
            return _theme;
        }
        private float GetLuminosity()
        {
            return 1;
        }
        private int GetRadius()
        {
            return 200;
        }
        private float GetOrbitRadius ()
        {
            return 1;
        }
        private float GetOrbitInclination()
        {
            return 1;
        }
        private float GetOrbitLongitude()
        {
            return 1;
        }
        private float GetOrbitalPeriod()
        {
            return 1000;
        }
        private float GetOrbitPhase()
        {
            return 10;
        }
        private float GetObliquity()
        {
            return 1;
        }
        private float GetRotationPeriod()
        {
            return 20;
        }
        private float GetRotationPhase()
        {
            return 1;
        }
      
    }
}