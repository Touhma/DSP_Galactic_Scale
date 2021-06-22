using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSPlanet
    {
        public GS2.Random random;

        private string name;
        private string theme;
        private int radius = -1;
        private float orbitRadius = -1;
        private float orbitInclination = -1;
        private float orbitalPeriod = -1;
        private float orbitPhase = -1;
        private float obliquity = -1;
        private float rotationPeriod = -1;
        private float rotationPhase = -1;
        private float luminosity = -1;
        [NonSerialized]
        public Dictionary<string, string> fields = new Dictionary<string, string>(); // Temporary string store for generator use, not saved
        private List<GSPlanet> moons = new List<GSPlanet>();
        private int seed = -1;

        public Dictionary<string, GSVein> veins = new Dictionary<string, GSVein>();
        public bool randomizeVeinAmounts = true;
        public bool randomizeVeinCounts = true;

        [SerializeField]
        public int Seed
        {
            get => seed;
            set => seed = value;
        }
        [SerializeField]
        public List<GSPlanet> Moons { get => moons; set => moons = value; }
        [SerializeField]
        public string Name { get => name; set => name = value; }
        [SerializeField]
        public string Theme { get => theme == null ? InitTheme() : theme; set => theme = value; }
        public GSTheme GsTheme => string.IsNullOrEmpty(Theme) ? null : GSSettings.ThemeLibrary[Theme];
        [SerializeField]
        public int Radius { get => radius < 0 ? InitRadius() : radius; set => radius = Utils.ParsePlanetSize(value); }
        [SerializeField]
        public float OrbitRadius { get => orbitRadius < 0 ? InitOrbitRadius() : orbitRadius; set => orbitRadius = value; }
        [SerializeField]
        public float OrbitInclination { get => orbitInclination < 0 ? InitOrbitInclination() : orbitInclination; set => orbitInclination = value; }
        [SerializeField]
        public float OrbitalPeriod { get => orbitalPeriod < 0 ? InitOrbitalPeriod() : orbitalPeriod; set => orbitalPeriod = value; }
        [SerializeField]
        public float OrbitPhase { get => orbitPhase < 0 ? InitOrbitPhase() : orbitPhase; set => orbitPhase = value; }
        [SerializeField]
        public float Obliquity { get => obliquity < 0 ? InitObliquity() : obliquity; set => obliquity = value; }
        [SerializeField]
        public float RotationPeriod { get => rotationPeriod < 0 ? InitRotationPeriod() : rotationPeriod; set => rotationPeriod = value; }
        [SerializeField]
        public float RotationPhase { get => rotationPhase < 0 ? InitRotationPhase() : rotationPhase; set => rotationPhase = value; }
        [SerializeField]
        public float Luminosity { get => luminosity < 0 ? InitLuminosity() : luminosity; set => luminosity = value; }
        private float scale = -1;
        public float Scale { get => scale < 0 ? InitScale() : scale; set => scale = value; }
        [NonSerialized]
        public PlanetData planetData;
        [NonSerialized]
        public GSVeinSettings veinSettings;
        [NonSerialized]
        public GSPlanetVeins veinData = new GSPlanetVeins();
        public bool IsHabitable
        {
            get
            {
                if (GsTheme.PlanetType != EPlanetType.Ocean)
                {
                    return false;
                }

                if (Radius < 50)
                {
                    return false;
                }

                if (GsTheme.WaterItemId != 1000)
                {
                    return false;
                }

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Oil, false))
                {
                    return false;
                }

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Iron, false))
                {
                    return false;
                }

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Copper, false))
                {
                    return false;
                }

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Stone, false))
                {
                    return false;
                }

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Coal, false))
                {
                    return false;
                }

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
            if (GsTheme == null)
            {
                return -1;
            }
            return (GsTheme.PlanetType == EPlanetType.Gas) ? 10f : 1f;
        }
        public int MoonsCount
        {
            get
            {
                if (Moons == null)
                {
                    return 0;
                }

                int count = 0;
                foreach (GSPlanet moon in Moons)
                {
                    count++;
                    count += moon.MoonsCount;
                }
                return count;
            }
        }
        public int MoonCount
        {
            get
            {
                if (Moons == null) return 0;
                return Moons.Count;
            }
        }
        public List<GSPlanet> Bodies
        {
            get
            {
                List<GSPlanet> b = new List<GSPlanet>() { this };
                if (Moons == null)
                {
                    return b;
                }

                foreach (GSPlanet moon in Moons)
                {
                    b.AddRange(moon.Bodies);
                }
                return b;
            }
        }

        public override string ToString() => this.Name;
        private string InitTheme()
        {
            theme = "Mediterranean";
            return theme;
        }
        private float InitLuminosity()
        {
            if (planetData == null)
            {
                return -1f;
            }

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
            radius = 200;
            return radius;
        }
        private float InitOrbitRadius()
        {
            if (random == null)
            {
                random = new GS2.Random(GSSettings.Seed);
            }

            orbitRadius = random.Next(10);
            return orbitRadius;
        }
        private float InitOrbitInclination()
        {
            orbitInclination = 0;
            return orbitInclination;
        }
        private float InitOrbitalPeriod()
        {
            orbitalPeriod = 1000;
            return orbitalPeriod;
        }
        private float InitOrbitPhase()
        {
            orbitPhase = 0;
            return orbitPhase;
        }
        private float InitObliquity()
        {
            obliquity = 0;
            return obliquity;
        }
        private float InitRotationPeriod()
        {
            rotationPeriod = 1000;
            return rotationPeriod;
        }
        private float InitRotationPhase()
        {
            rotationPhase = 0;
            return rotationPhase;
        }
        public GSPlanet MostDistantSatellite
        {
            get
            {
                if (Moons == null)
                {
                    GS2.Error("MOONS NULL!?");
                }

                if (Moons.Count == 0)
                {
                    return this;
                }

                return Moons[Moons.Count - 1].MostDistantSatellite;
            }
        }
        public float RadiusAU => Radius * 0.00005f * Scale;
        public float SystemRadius
        {
            get
            {
                GSPlanet mds = MostDistantSatellite;
                if (mds == this)
                {
                    return RadiusAU;
                }

                return MostDistantSatellite.OrbitRadius + MostDistantSatellite.RadiusAU;
            }
        }
    }
}