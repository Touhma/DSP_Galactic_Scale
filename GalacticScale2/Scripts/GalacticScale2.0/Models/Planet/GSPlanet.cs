using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSPlanet
    {
        [NonSerialized] public readonly ValStore genData = new ValStore();

        [NonSerialized] public Dictionary<string, string> fields = new Dictionary<string, string>(); // Temporary string store for generator use, not saved

        private float luminosity = -1;

        private float obliquity = -1;
        private float orbitalPeriod = -1;
        private float orbitInclination = -1;
        private float orbitLongitude = -1;
        private float orbitPhase = -1;
        private float orbitRadius = -1;

        [NonSerialized] public PlanetData planetData;

        private int radius = -1;
        [NonSerialized] public GS2.Random random;
        public bool randomizeVeinAmounts = true;
        public bool randomizeVeinCounts = true;
        public float rareChance = -1f;
        private float rotationPeriod = -1;
        private float rotationPhase = -1;

        [SerializeField] private float scale = -1;

        private string theme;

        [NonSerialized] public GSPlanetVeins veinData = new GSPlanetVeins();

        //[NonSerialized] public Dictionary<string, GSVein> veins = new Dictionary<string, GSVein>();

        public GSVeinSettings veinSettings = null;

        public GSPlanet()
        {
        }

        public GSPlanet(string name)
        {
            Name = name;
        }

        public GSPlanet(string name, string theme, int radius, float orbitRadius, float orbitInclination, float orbitalPeriod, float orbitPhase, float obliquity, float rotationPeriod, float rotationPhase, float luminosity, GSPlanets moons = null)
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
            Moons = moons == null ? new GSPlanets() : moons;
        }

        public string _type
        {
            get
            {
                if (scale == 10f) return "Gas";
                return "Telluric";
            }
        }

        public string details => $"Name:{Name} Theme:{Theme} Radius:{Radius} Type:{_type}";

        [SerializeField] public int Seed { get; set; } = -1;

        [SerializeField] public GSPlanets Moons { get; set; } = new GSPlanets();

        [SerializeField] public string Name { get; set; }

        [SerializeField]
        public string Theme
        {
            get => theme == null ? InitTheme() : theme;
            set => theme = value;
        }

        public GSTheme GsTheme => string.IsNullOrEmpty(Theme) ? null : GSSettings.ThemeLibrary.Find(Theme);

        [SerializeField]
        public int Radius
        {
            get => radius < 0 ? InitRadius() : radius;
            set => radius = Utils.ParsePlanetSize(value);
        }

        [SerializeField]
        public float OrbitRadius
        {
            get => orbitRadius < 0 ? InitOrbitRadius() : orbitRadius;
            set => orbitRadius = value;
        }

        [SerializeField]
        public float OrbitInclination
        {
            get => orbitInclination < 0 ? InitOrbitInclination() : orbitInclination;
            set => orbitInclination = value;
        }

        [SerializeField]
        public float OrbitalPeriod
        {
            get => orbitalPeriod == -1 ? InitOrbitalPeriod() : orbitalPeriod;
            set => orbitalPeriod = value;
        }

        [SerializeField]
        public float OrbitPhase
        {
            get => orbitPhase < 0 ? InitOrbitPhase() : orbitPhase;
            set => orbitPhase = value;
        }

        [SerializeField]
        public float OrbitLongitude
        {
            get => orbitLongitude < 0 ? InitOrbitLongitude() : orbitLongitude;
            set => orbitLongitude = value;
        }

        [SerializeField]
        public float Obliquity
        {
            get => obliquity < 0 ? InitObliquity() : obliquity;
            set => obliquity = value;
        }

        [SerializeField]
        public float RotationPeriod
        {
            get => rotationPeriod == -1 ? InitRotationPeriod() : rotationPeriod;
            set => rotationPeriod = value;
        }

        [SerializeField]
        public float RotationPhase
        {
            get => rotationPhase < 0 ? InitRotationPhase() : rotationPhase;
            set => rotationPhase = value;
        }

        [SerializeField]
        public float Luminosity
        {
            get => luminosity < 0 ? InitLuminosity() : luminosity;
            set => luminosity = value;
        }

        public float Scale
        {
            get => scale < 0 ? InitScale() : scale;
            set => scale = value;
        }

        public bool IsHabitable
        {
            get
            {
                //GS2.Warn($"Checking Habitable {Name} {GsTheme.Name} {GsTheme.PlanetType} {GsTheme.WaterItemId} {Radius}");
                //GS2.Warn($"{(!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Oil))} {(!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Iron))} {(!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Copper))} {(!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Stone))}");
                if (GsTheme.PlanetType != EPlanetType.Ocean) return false;

                if (Radius < 50) return false;

                if (GsTheme.WaterItemId != 1000) return false;

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Oil)) return false;

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Iron)) return false;

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Copper)) return false;

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Stone)) return false;

                if (!GsTheme.VeinSettings.VeinTypes.ContainsVein(EVeinType.Coal)) return false;

                return true;
            }
        }

        public int MoonsCount
        {
            get
            {
                if (Moons == null) return 0;

                var count = 0;
                foreach (var moon in Moons)
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

        public GSPlanets Bodies
        {
            get
            {
                var b = new GSPlanets { this };
                if (Moons == null) return b;

                foreach (var moon in Moons) b.AddRange(moon.Bodies);
                return b;
            }
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

        public float RadiusAU => Radius * 0.000025f * Scale;

        public float SystemRadius
        {
            get
            {
                var mds = MostDistantSatellite;
                if (mds == this)
                    //GS2.Log($"{Name} Most Distant is itself");
                    return RadiusAU;
                var p = this;
                var c = 0;
                float rad = 0;
                while (p != mds || c < 99)
                {
                    //GS2.Log($"p:{p.Name} while. MDS:{mds.Name} MoonCount : {p.MoonCount} {p.MoonsCount} {p.Moons.Count}");
                    c++;
                    var moon = p.Moons[p.MoonCount - 1];
                    rad += moon.OrbitRadius;
                    p = moon;
                    if (p == mds) break;
                }

                //GS2.Log($"{Name} most distant:{mds.Name} its RadiusAU:{mds.RadiusAU} its OrbitRadius:{mds.OrbitRadius}");
                return rad + mds.RadiusAU;
            }
        }

        public float OrbitOutermostSystemRadiusAU => orbitRadius + SystemRadius;
        public float OrbitInnermostSystemRadiusAU => orbitRadius - SystemRadius;
        public float OrbitOutermostSurfaceRadiusAU => orbitRadius + RadiusAU;
        public float OrbitInnermostSurfaceRadiusAU => orbitRadius - RadiusAU;

        public float InitScale()
        {
            if (GsTheme == null) return -1;
            return GsTheme.PlanetType == EPlanetType.Gas ? 10f : 1f;
        }

        public override string ToString()
        {
            return details;
        }

        private string InitTheme()
        {
            theme = "Mediterranean";
            return theme;
        }

        private float InitLuminosity()
        {
            if (planetData == null) return -1f;

            var sunDistance = planetData.orbitAround != 0 ? planetData.orbitAroundPlanet.orbitRadius : planetData.orbitRadius;
            var luminosity = Mathf.Pow(planetData.star.lightBalanceRadius / (sunDistance + 0.01f), 0.6f);
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
            if (random == null) random = new GS2.Random(GSSettings.Seed);

            orbitRadius = -1; // random.Next(10);
            return orbitRadius;
        }

        private float InitOrbitInclination()
        {
            orbitInclination = 0;
            return orbitInclination;
        }

        private float InitOrbitLongitude()
        {
            orbitLongitude = 0;
            return orbitLongitude;
        }

        private float InitOrbitalPeriod()
        {
            orbitalPeriod = Utils.CalculateOrbitPeriod(OrbitRadius);
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
    }
}