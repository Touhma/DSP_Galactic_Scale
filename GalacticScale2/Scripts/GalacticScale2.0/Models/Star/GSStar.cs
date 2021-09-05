using System;
using UnityEngine;

namespace GalacticScale
{
    public class GSStar
    {
        [NonSerialized] public readonly ValStore genData = new ValStore();

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

        public GSPlanets Planets = new GSPlanets();
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
        //public float SystemRadius => Planets[Planets.Count -1].SystemRadius;
    }
}