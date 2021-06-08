using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSPlanet
    {
        public GS2.Random random;

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
        private List<GSPlanet> _moons = new List<GSPlanet>();
        private int seed = -1;

        public Dictionary<string, GSVein> veins = new Dictionary<string, GSVein>();
        public bool randomizeVeinAmounts = true;
        public bool randomizeVeinCounts = true;

        [SerializeField]
        public int Seed
        {
            get
            {
                return seed;
            }
            set => seed = value;
        }
        [SerializeField]
        public List<GSPlanet> Moons { get => _moons; set => _moons = value; }
        [SerializeField]
        public string Name { get => _name; set => _name = value; }
        [SerializeField]
        public string Theme { get => _theme == null ? InitTheme() : _theme; set => _theme = value; }
        public GSTheme gsTheme { get => string.IsNullOrEmpty(Theme) ? null : GSSettings.ThemeLibrary[Theme]; }
        [SerializeField]
        public int Radius { get => _radius < 0 ? InitRadius() : _radius; set => _radius = value; }
        [SerializeField]
        public float OrbitRadius { get => _orbitRadius < 0 ? InitOrbitRadius() : _orbitRadius; set => _orbitRadius = value; }
        [SerializeField]
        public float OrbitInclination { get => _orbitInclination < 0 ? InitOrbitInclination() : _orbitInclination; set => _orbitInclination = value; }
        // [SerializeField]
        // public float OrbitLongitude { get => _orbitLongitude < 0 ? InitOrbitLongitude():_orbitLongitude ; set => _orbitLongitude = value; }
        [SerializeField]
        public float OrbitalPeriod { get => _orbitalPeriod < 0 ? InitOrbitalPeriod() : _orbitalPeriod; set => _orbitalPeriod = value; }
        [SerializeField]
        public float OrbitPhase { get => _orbitPhase < 0 ? InitOrbitPhase() : _orbitPhase; set => _orbitPhase = value; }
        [SerializeField]
        public float Obliquity { get => _obliquity < 0 ? InitObliquity() : _obliquity; set => _obliquity = value; }
        [SerializeField]
        public float RotationPeriod { get => _rotationPeriod < 0 ? InitRotationPeriod() : _rotationPeriod; set => _rotationPeriod = value; }
        [SerializeField]
        public float RotationPhase { get => _rotationPhase < 0 ? InitRotationPhase() : _rotationPhase; set => _rotationPhase = value; }
        [SerializeField]
        public float Luminosity { get => _luminosity < 0 ? InitLuminosity() : _luminosity; set => _luminosity = value; }
        private float _scale = -1;
        public float Scale { get => _scale < 0 ? InitScale() : _scale; set => _scale = value; }
        [NonSerialized]
        public PlanetData planetData;
        [NonSerialized]
        public GSVeinSettings veinSettings;
        [NonSerialized]
        public GSPlanetVeins veinData = new GSPlanetVeins();
        public bool isHabitable
        {
            get
            {
                if (gsTheme.PlanetType != EPlanetType.Ocean) return false;
                if (Radius < 50) return false;
                if (gsTheme.WaterItemId != 1000) return false;
                if (!gsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Oil, false)) return false;
                if (!gsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Iron, false)) return false;
                if (!gsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Copper, false)) return false;
                if (!gsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Stone, false)) return false;
                if (!gsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Coal, false)) return false;

                return true;
            }
        }
        public GSPlanet()
        {

        }
        public GSPlanet(string name)
        {
            Name = name;
        }

        public GSPlanet(string name,
            string theme,
            int radius,
            float orbitRadius,
            float orbitInclination,
            float orbitalPeriod,
            float orbitPhase,
            float obliquity,
            float rotationPeriod,
            float rotationPhase,
            float luminosity,
            GSPlanets moons = null)
        {
            Name = name;
            Theme = theme;
            Radius = radius;
            OrbitRadius = orbitRadius;
            OrbitInclination = orbitInclination;
            OrbitalPeriod = orbitalPeriod;
            OrbitPhase = orbitPhase;
            Obliquity = obliquity;
            RotationPeriod = rotationPeriod;
            RotationPhase = rotationPhase;
            Luminosity = luminosity;
            Moons = (moons == null) ? new GSPlanets() : moons;
        }
        public float InitScale()
        {
            if (gsTheme == null)
            {
                GS2.Warn("Trying to read theme before setting it: " + Name);
                return -1;
            }
            return (gsTheme.PlanetType == EPlanetType.Gas) ? 10f : 1f;
        }
        public int MoonCount
        {
            get
            {
                if (Moons == null) return 0;
                int count = 0;
                foreach (GSPlanet moon in Moons)
                {
                    count++;
                    count += moon.MoonCount;
                }
                return count;
            }
        }
        public List<GSPlanet> Bodies
        {
            get
            {
                List<GSPlanet> b = new List<GSPlanet>() { this };
                if (Moons == null) return b;
                foreach (GSPlanet moon in Moons)
                {
                    b.AddRange(moon.Bodies);
                }
                return b;
            }
        }


        //private int InitSeed()
        //{
        //    seed = random.Next();
        //    return seed;
        //}

        public override string ToString()
        {
            return this.Name;
        }
        private string InitTheme()
        {
            _theme = "Mediterranean";
            return _theme;
        }
        private float InitLuminosity()
        {
            if (planetData == null) return -1f;
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
        private int InitRadius()
        {
            _radius = 200;
            return _radius;
        }
        private float InitOrbitRadius()
        {
            _orbitRadius = GS2.random.Next(10);
            return _orbitRadius;
        }
        private float InitOrbitInclination()
        {
            _orbitInclination = 0;
            return _orbitInclination;
        }
        private float InitOrbitLongitude()
        {
            _orbitLongitude = 0;
            return _orbitLongitude;
        }
        private float InitOrbitalPeriod()
        {
            _orbitalPeriod = 1000;
            return _orbitalPeriod;
        }
        private float InitOrbitPhase()
        {
            _orbitPhase = 0;
            return _orbitPhase;
        }
        private float InitObliquity()
        {
            _obliquity = 0;
            return _obliquity;
        }
        private float InitRotationPeriod()
        {
            _rotationPeriod = 1000;
            return _rotationPeriod;
        }
        private float InitRotationPhase()
        {
            _rotationPhase = 0;
            return _rotationPhase;
        }
        public GSPlanet MostDistantSatellite
        {
            get
            {
                if (Moons == null) GS2.Error("MOONS NULL!?");
                if (Moons.Count == 0) return this;
                return Moons[Moons.Count - 1].MostDistantSatellite;
            }
        }
        public float RadiusAU { get => Radius * 0.00005f * Scale; }
        public float SystemRadius
        {
            get
            {
                GSPlanet mds = MostDistantSatellite;
                if (mds == this) return RadiusAU;
                return MostDistantSatellite.OrbitRadius + MostDistantSatellite.RadiusAU;
            }
        }
    }
}