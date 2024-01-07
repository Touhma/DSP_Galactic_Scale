using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSStar
    {
        [NonSerialized] public readonly ValStore genData = new();

        private float _acdiscRadius = -1;
        private float _age = -1;
        private float _classfactor = 3;
        private float _color = -1;
        private float _dysonRadius = -1;
        private float _habitableRadius = -1;
        private float _lifetime = -1;
        private float _lightBalanceRadius = -1;
        private float _luminosity = -1;
        private float _mass = -1;
        private int _initialHiveCount = -1;
        private int _maxHiveCount = -1;
        private int _hivePatternLevel = -1;
        private float _safetyFactor = -1;
        private bool _birthStar = false;

        [NonSerialized] private float _physicsRadius = -1;
        private VectorLF3 _pos;
        private float _radius = -1;
        private float _resourceCoef = -1f;
        private float _temperature = -1;
        [NonSerialized] public int assignedIndex = 0;
        [SerializeField] public string BinaryCompanion = null;

        [NonSerialized] public int counter = 0;

        [SerializeField] public bool Decorative = false;

        [SerializeField] public float level = 1;

        [SerializeField] public float MaxOrbit = 100f;

        public string Name;

        [SerializeField] public float orbitScaler = 1;

        public GSPlanets Planets = new();
        public AstroOrbitData[] _hiveOrbits = new AstroOrbitData[8];
        public int Seed;
        public ESpectrType Spectr;
        public EStarType Type;

        public GSStar(int seed, string name, ESpectrType spectr, EStarType type, GSPlanets planets)
        {
            Name = name;
            Spectr = spectr;
            Type = type;
            Planets = planets;
            Seed = seed;
        }

        public int PlanetCount
        {
            get
            {
                if (Planets == null) return 0;
                return Planets.Count;
            }
        }

        public int bodyCount
        {
            get
            {
                var bodyCount = 0;
                foreach (var p in Planets)
                {
                    bodyCount++;
                    bodyCount += p.MoonsCount;
                }

                return bodyCount;
            }
        }

        public GSPlanets Bodies
        {
            get
            {
                var b = new GSPlanets();
                foreach (var p in Planets) b.AddRange(p.Bodies);
                return b;
            }
        }

        public void DebugStarData()
        {
            Debug.Log(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|", this.Name, this.Type, this.Spectr, this.radius));
            foreach (var b in this.Bodies)
            {
                Debug.Log(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|",
                    b.genData.Get("birthPlanet", false) ? "X" : "-", b.Name, b.Radius, b.Theme));
            }
        }

        public GSPlanets TelluricBodies
        {
            get
            {
                var b = new GSPlanets();
                foreach (var p in Bodies)
                    if (p.GsTheme.PlanetType != EPlanetType.Gas)
                        b.Add(p);
                return b;
            }
        }

        public int TelluricBodyCount => TelluricBodies.Count;

        [SerializeField]
        public bool birthStar
        {
            get
            {
                if (GSSettings.Instance.GenerationComplete)
                {
                    if (GSSettings.BirthStar == this)
                    {
                        _birthStar = true;
                    }
                    else
                    {
                        _birthStar = false;
                    }

                    return _birthStar;
                }

                GS3.Error($"birthStar field accessed before generation complete by {GS3.GetCaller()}");
                return false;
            }
           
        }
    
        [SerializeField]
        public AstroOrbitData[] hiveAstroOrbits
        {
            get => _hiveOrbits == null ? InitHiveAstroOrbits() : _hiveOrbits;
            set => _hiveOrbits = value;
        }

        private AstroOrbitData[] InitHiveAstroOrbits()
        {
            var data = new AstroOrbitData[8];
            var random = new GS3.Random(Seed);
            var possibleOrbits = GS3.GeneratePossibleHiveOrbits(this, 10, random);
            for (var i = 0; i < 8; i++)
            {
                data[i] = new AstroOrbitData();
                var orbit = random.ItemAndRemove(possibleOrbits);
                data[i].orbitRadius = orbit;
                GS3.Warn($"Created Hive Orbit at {Name} {Utils.Round2DP(hiveAstroOrbits[i].orbitRadius)}");
                data[i].orbitInclination = random.NextFloat();
                data[i].orbitLongitude = random.NextFloat();
                data[i].orbitPhase = random.NextFloat();
                data[i].orbitalPeriod = Utils.CalculateOrbitPeriod(hiveAstroOrbits[i].orbitRadius);
                data[i].orbitRotation = Quaternion.AngleAxis(hiveAstroOrbits[i].orbitLongitude, Vector3.up) *
                                        Quaternion.AngleAxis(hiveAstroOrbits[i].orbitInclination,
                                            Vector3.forward);
                data[i].orbitNormal =
                    Maths.QRotateLF(hiveAstroOrbits[i].orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
            }

            return data;
        }


        [SerializeField]
        public int initialHiveCount
        {
            get => _initialHiveCount < 0 ? InitInitialHiveCount() : _initialHiveCount;
            set => _initialHiveCount = value < 0 ? InitInitialHiveCount() : (int)value;
        }

        private int InitInitialHiveCount()
        {
            if (!GSSettings.Instance.GenerationComplete)
            {
                GS3.Error($"initialHiveCount accessed before generation complete by {GS3.GetCaller()}");
                return 0;
            }
            if (GS3.gameDesc.combatSettings.initialColonize < 0.015f || Decorative || PlanetCount == 0)
            {
                _initialHiveCount = 0;
            }
            else
            {
                _initialHiveCount = CalculateInitialHiveCount();
            }
            return _initialHiveCount;
        }

        private int CalculateInitialHiveCount()
        {
            if (birthStar) return CalcInitialHiveCountBirthStar();
            float initialColonize = GS3.gameDesc.combatSettings.initialColonize;
            int count;
            var r = new GS3.Random(Seed);

            
                
                float difficultyFactor = Mathf.Pow(Mathf.Clamp01(safetyFactor - 0.2f), 0.86f);
                float typeFactor = Mathf.Clamp01(1f - difficultyFactor - (maxHiveCount - 1) * 0.05f) * (1.1f - maxHiveCount * 0.1f);
                if (initialColonize <= 1f)
                {
                    typeFactor *= initialColonize;
                }
                else
                {
                    typeFactor = Mathf.Lerp(typeFactor, 1f + (initialColonize - 1f) * 0.2f, (initialColonize - 1f) * 0.5f);
                }

                switch (Type)
                {
                    case EStarType.GiantStar:
                        typeFactor *= 1.2f;
                        break;
                    case EStarType.WhiteDwarf:
                        typeFactor *= 1.4f;
                        break;
                    case EStarType.NeutronStar:
                        typeFactor *= 1.6f;
                        break;
                    case EStarType.BlackHole:
                        typeFactor *= 1.8f;
                        break;
                    default:
                    {
                        if (Spectr == ESpectrType.O)
                        {
                            typeFactor *= 1.1f;
                        }

                        break;
                    }
                }

                var proposed = typeFactor * maxHiveCount;
                if (proposed > maxHiveCount + 0.75f)
                {
                    proposed = maxHiveCount + 0.75f;
                }

                var standardDeviation = 0.5f;
                if (proposed <= 0.01)
                {
                    standardDeviation = 0f;
                }
                else
                    standardDeviation = proposed switch
                    {
                        < 1f => Mathf.Sqrt(proposed) * 0.29f + 0.21f,
                        > 1f => 0.3f + 0.2f * proposed,
                        _ => standardDeviation
                    };

                int iterations = 64;
                do
                {
                    count = (int)(StarGen.RandNormal(proposed, standardDeviation, r.NextDouble(), r.NextDouble()) + 0.5);
                } while (iterations-- > 0 && (count < 0 || count > maxHiveCount));

                if (count < 0)
                {
                    count = 0;
                }
                else if (count > maxHiveCount)
                {
                    count = maxHiveCount;
                }
            

            if (Type == EStarType.BlackHole)
            {
                int epicHiveCount = (int)(GS3.gameDesc.combatSettings.maxDensity * 1000f + r.NextFloat(1000f) + 0.5f) / 1000;
                if (count < epicHiveCount)
                {
                    count = epicHiveCount;
                }

                if (count < 1)
                {
                    count = 1;
                }
            }

            return count;
        }
    

        private int CalcInitialHiveCountBirthStar()
        {
            var count = 0;
            var r = new GS3.Random(Seed);
            var initialColonize = GS3.gameDesc.combatSettings.initialColonize;
            int num18 = ((initialColonize * (float)maxHiveCount < 0.7f) ? 0 : 1);
            float proposed = 0.6f * initialColonize * (float)maxHiveCount;
            float standardDeviation = 0.5f;
            if (proposed < 1f)
            {
                standardDeviation = Mathf.Sqrt(proposed) * 0.29f + 0.21f;
            }
            else if (proposed > (float)maxHiveCount)
            {
                proposed = (float)maxHiveCount;
            }
            int iterations = 16;
            do
            {
                count = (int)((double)StarGen.RandNormal(proposed, standardDeviation,  r.NextDouble(),  r.NextDouble()) + 0.5);
            }
            while (iterations-- > 0 && (count < 0 || count > maxHiveCount));
                
            if (count < num18)
            {
                count = num18;
            }
            else if (count > maxHiveCount)
            {
                count = maxHiveCount;
            }

            return count;
        }
        [SerializeField]
        public int maxHiveCount
        {
            get => _maxHiveCount < 0 ? InitMaxHiveCount() : _maxHiveCount;
            set => _maxHiveCount = value < 0 ? InitMaxHiveCount() : (int)value;
        }

        private int InitMaxHiveCount()
        {
            if (!GSSettings.Instance.GenerationComplete)
            {
                GS3.Error($"maxHiveCount accessed before generation complete by {GS3.GetCaller()}");
                return 0;
            }

            if (Decorative || PlanetCount == 0)
            {
                _maxHiveCount = 0;
            }
            else
            {
                var r = new GS3.Random(Seed);
                _maxHiveCount = (int)(GS3.gameDesc.combatSettings.maxDensity * 1000f + (float)r.Next(1000) + 0.5f) /
                                1000;
                _maxHiveCount = Mathf.Clamp(_maxHiveCount, 1, 8);
            }
            return _maxHiveCount;
        }

        [SerializeField]
        public float safetyFactor
        {
            get => _safetyFactor < 0 ? InitSafetyFactor() : _safetyFactor;
            set => _safetyFactor = value < 0 ? InitSafetyFactor() : value;
        }

        private float InitSafetyFactor()
        {
            if (!GSSettings.Instance.GenerationComplete)
            {
                GS3.Error($"Safety Factor accessed before generation complete by {GS3.GetCaller()}");
                return 0;
            }
            var r = new GS3.Random(Seed);
            if (birthStar) _safetyFactor = 0.847f + r.NextFloat() * 0.026f;
            else
            {
                var distanceFactor = Mathf.Clamp((float)(distanceFromBirthStar - 2.0) / 20f, 0.0f, 2.5f);
                var colorFactor = Mathf.Pow(color, 1.3f);
                switch (Type)
                {
                    case EStarType.BlackHole:
                        colorFactor = 5f;
                        break;
                    case EStarType.NeutronStar:
                        colorFactor = 1.7f;
                        break;
                    case EStarType.WhiteDwarf:
                        colorFactor = 1.2f;
                        break;
                    case EStarType.GiantStar:
                        colorFactor = Mathf.Max(0.6f, colorFactor);
                        break;
                }

                if (Spectr == ESpectrType.O) _safetyFactor += 0.05f;
                colorFactor *= 0.9f;
                colorFactor += 0.07f;
                _safetyFactor = Mathf.Clamp01(1f - Mathf.Pow(colorFactor, 0.73f) * Mathf.Pow(distanceFactor, 0.27f) + (float)r.NextFloat() * 0.08f - 0.04f);
            }
            return _safetyFactor;            
        }
        [NonSerialized]
        private double _distanceFromBirthStar = -1;

        public double distanceFromBirthStar
        {
            get
            {
                if (_distanceFromBirthStar < 0)
                {
                    if (!GSSettings.Instance.GenerationComplete)
                    {
                        GS3.Error($"distanceFromBirthStar accessed before generation complete by {GS3.GetCaller()}");
                        return 0;
                    }

                    if (birthStar)
                    {
                        _distanceFromBirthStar = 0;
                    }
                    else _distanceFromBirthStar = (position - GSSettings.BirthStar.position).magnitude;
                }
                return _distanceFromBirthStar;
            }
        }
        
        [SerializeField]
        public int hivePatternLevel
        {
            get => _hivePatternLevel < 0 ? InitHivePatternLevel() : _hivePatternLevel;
            set => _hivePatternLevel = value < 0 ? InitHivePatternLevel() : (int)value;
        }
        private int InitHivePatternLevel()
        {
            if (safetyFactor >= 0.7f)
            {
                _hivePatternLevel = 0;
            }
            else if (safetyFactor >= 0.3f)
            {
                _hivePatternLevel = 1;
            }
            else
            {
                _hivePatternLevel = 2;
            }

            return _hivePatternLevel;
        }
        
        [SerializeField]
        public float age
        {
            get => _age < 0 ? InitAge() : _age;
            set => _age = value;
        }

        [SerializeField]
        public float mass
        {
            get => _mass < 0 ? InitMass() : _mass;
            set => _mass = value;
        }

        [SerializeField]
        public float temperature
        {
            get => _temperature < 0 ? InitTemperature() : _temperature;
            set => _temperature = value;
        }

        [SerializeField]
        public float lifetime
        {
            get => _lifetime < 0 ? InitLifetime() : _lifetime;
            set => _lifetime = value;
        }

        [SerializeField]
        public float color
        {
            get => _color < 0 ? InitColor() : _color;
            set => _color = value;
        }

        [SerializeField]
        public float luminosity
        {
            get => _luminosity < 0 ? InitLuminosity() : _luminosity;
            set => _luminosity = value;
        }

        [SerializeField]
        public float radius
        {
            get => _radius < 0 ? InitRadius() : _radius;
            set => _radius = value;
        }

        [SerializeField]
        public float habitableRadius
        {
            get => _habitableRadius < 0 ? InitHabitableRadius() : _habitableRadius;
            set => _habitableRadius = value;
        }

        [SerializeField]
        public float dysonRadius
        {
            get => _dysonRadius < 0 ? InitDysonRadius() : _dysonRadius;
            set => _dysonRadius = value;
        }

        [SerializeField]
        public float lightBalanceRadius
        {
            get => _lightBalanceRadius < 0 ? InitLightBalanceRadius() : _lightBalanceRadius;
            set => _lightBalanceRadius = value;
        }

        [SerializeField]
        public float physicsRadius
        {
            get => _physicsRadius < 0 ? InitPhysicsRadius() : _physicsRadius;
            set => _physicsRadius = value;
        }

        [SerializeField]
        public float classFactor
        {
            get => _classfactor > 2 ? InitClassFactor() : _classfactor;
            set => _classfactor = value;
        }

        [SerializeField]
        public float resourceCoef
        {
            get => _resourceCoef < 0 ? InitResourceCoef() : _resourceCoef;
            set => _resourceCoef = value;
        }

        [SerializeField]
        public float acDiscRadius
        {
            get => _acdiscRadius < 0 ? InitAcDiscRadius() : _acdiscRadius;
            set => _acdiscRadius = value;
        }

        [SerializeField]
        public VectorLF3 position
        {
            get
            {
                if (_pos == new VectorLF3() && assignedIndex != 0) return InitPos();
                return _pos;
            }
            set => _pos = value;
        }

        public double magnitude => position.magnitude;
        public float RadiusM => radius * 800f;
        public float RadiusAU => RadiusM / 40000f;
        public float RadiusLY => RadiusAU / 60f;

        public string displayType
        {
            get
            {
                if (Type == EStarType.BlackHole) return "Black Hole";
                if (Type == EStarType.WhiteDwarf) return "White Dwarf";
                if (Type == EStarType.NeutronStar) return "Neutron Star";
                if (Type == EStarType.GiantStar)
                {
                    if (Spectr == ESpectrType.F || Spectr == ESpectrType.G) return "Yellow Giant";
                    if (Spectr == ESpectrType.A) return "White Giant";
                    if (Spectr == ESpectrType.B || Spectr == ESpectrType.O) return "Blue Giant";
                    return "Red Giant";
                }

                if (Spectr == ESpectrType.A) return "Type A Star";
                if (Spectr == ESpectrType.B) return "Type B Star";
                if (Spectr == ESpectrType.F) return "Type F Star";
                if (Spectr == ESpectrType.G) return "Type G Star";
                if (Spectr == ESpectrType.M) return "Type M Star";
                if (Spectr == ESpectrType.K) return "Type K Star";
                if (Spectr == ESpectrType.O) return "Type O Star";
                return "Unknown Star";
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private float InitClassFactor()
        {
            _classfactor = StarDefaults.ClassFactor(this);
            return _classfactor;
        }

        private float InitPhysicsRadius()
        {
            return radius * 1200f;
        }

        private float InitAcDiscRadius()
        {
            switch (Type)
            {
                case EStarType.BlackHole:
                    _acdiscRadius = radius * 5f;
                    break;
                case EStarType.NeutronStar:
                    _acdiscRadius = radius * 9f;
                    break;
                default:
                    _acdiscRadius = 0.0f;
                    break;
            }

            return _acdiscRadius;
        }

        private float InitDysonRadius()
        {
            _dysonRadius = StarDefaults.DysonRadius(this);
            return _dysonRadius;
        }

        private float InitHabitableRadius()
        {
            _habitableRadius = StarDefaults.HabitableRadius(this);
            return _habitableRadius;
        }

        private float InitLightBalanceRadius()
        {
            _lightBalanceRadius = StarDefaults.LightBalanceRadius(this);
            return _lightBalanceRadius;
        }

        private float InitRadius()
        {
            _radius = StarDefaults.Radius(this);
            return _radius;
        }

        private float InitAge()
        {
            _age = StarDefaults.Age(this);
            return _age;
        }

        private float InitMass()
        {
            _mass = StarDefaults.Mass(this);
            return _mass;
        }

        private float InitLuminosity()
        {
            _luminosity = StarDefaults.Luminosity(this);
            return _luminosity;
        }

        private float InitTemperature()
        {
            _temperature = StarDefaults.Temperature(this);
            return _temperature;
        }

        private float InitLifetime()
        {
            _lifetime = StarDefaults.Lifetime(this);
            return _lifetime;
        }

        private float InitColor()
        {
            _color = StarDefaults.Color(this);
            return _color;
        }

        private float InitResourceCoef()
        {
            var num1 = (float)position.magnitude / 32f;
            if (num1 > 1.0) num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;

            _resourceCoef = Mathf.Pow(7f, num1) * 0.6f;
            return resourceCoef;
        }

        private VectorLF3 InitPos()
        {
            _pos = StarPositions.tmp_poses[assignedIndex];
            return _pos;
        }

        public GSStar Clone()
        {
            GSStar clone;
            clone = (GSStar)MemberwiseClone();
            clone.Planets = new GSPlanets(Planets);
            return clone;
        }

        public float SystemRadius
        {
            get
            {
                if (PlanetCount == 0) return 0;
                return Planets[Planets.Count - 1].OrbitOutermostSystemRadiusAU + 1;
            }
        }
    }
}