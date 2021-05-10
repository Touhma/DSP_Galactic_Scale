using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class GSStar
    {
        public string Name;
        public ESpectrType Spectr;
        public EStarType Type;
        public List<GSPlanet> Planets = new List<GSPlanet>();
        public int Seed;
        private float _habitableRadius = -1;
        private float _dysonRadius = -1;
        public float orbitScaler = 1;
        private float _temperature = -1;
        private float _lifetime = -1;
        private float _age = -1;
        private float _mass = -1;
        private float _color = -1;
        private float _classfactor = 3;
        private float _luminosity = -1;
        private float _radius = -1;
        private float _acdiscRadius = -1;
        private float _lightBalanceRadius = -1;
        private float _resourceCoef = -1f;
        private float _physicsRadius = -1;
        private VectorLF3 _pos = new VectorLF3();
        [SerializeField]
        public float level = 1;
        public GSStar(int seed, string name, ESpectrType spectr, EStarType type, List<GSPlanet> planets)
        {
            this.Name = name;
            this.Spectr = spectr;
            this.Type = type;
            this.Planets = planets;
            this.Seed = seed;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public int planetCount { get => Planets.Count; }
        public int bodyCount
        {
            get
            {
                int bodyCount = 0;
                foreach (GSPlanet p in Planets)
                {
                    bodyCount++;
                    bodyCount += p.MoonCount;
                }
                return bodyCount;
            }
        }
        [NonSerialized]
        public int counter = 0;
        [SerializeField]
        public float age { get => _age < 0 ? getAge() : _age; set => _age = value; }
        [SerializeField]
        public float mass { get => _mass < 0 ? getMass() : _mass; set => _mass = value; }
        [SerializeField]
        public float temperature { get => _temperature < 0 ? getTemperature() : _temperature; set => _temperature = value; }
        [SerializeField]
        public float lifetime { get => _lifetime < 0 ? getLifetime() : _lifetime; set => _lifetime = value; }
        [SerializeField]
        public float color { get => _color < 0 ? getColor() : _color; set => _color = value; }
        [SerializeField]
        public float luminosity { get => _luminosity < 0 ? getLuminosity() : _luminosity; set => _luminosity = value; }
        [SerializeField]
        public float radius { get => _radius < 0 ? getRadius() : _radius; set => _radius = value; }
        [SerializeField]
        public float habitableRadius { get => _habitableRadius < 0 ? getHabitableRadius() : _habitableRadius; set => _habitableRadius = value; }
        [SerializeField]
        public float dysonRadius { get => _dysonRadius < 0 ? getDysonRadius() : _dysonRadius; set => _dysonRadius = value; }
        [SerializeField]
        public float lightBalanceRadius { get => _lightBalanceRadius < 0 ? getLightBalanceRadius() : _lightBalanceRadius; set => _lightBalanceRadius = value; }
        [SerializeField]
        public float physicsRadius { get => _physicsRadius < 0 ? getPhysicsRadius() : _physicsRadius; set => _physicsRadius = value; }
        [SerializeField]
        public float classFactor { get => _classfactor > 2 ? getClassFactor() : _classfactor; set => _classfactor = value; }
        [SerializeField]
        public float resourceCoef { get => _resourceCoef < 0 ? getResourceCoef() : _resourceCoef; set => _resourceCoef = value; }
        [SerializeField]
        public float acDiscRadius { get => _acdiscRadius < 0 ? getAcDiscRadius() : _acdiscRadius; set => _acdiscRadius = value; }
        [SerializeField]
        public VectorLF3 position { get => (_pos == new VectorLF3() && assignedIndex != 0) ? getPos() : _pos; set => _pos = value; }

        public double magnitude { get => position.magnitude; }
        [NonSerialized]
        public int assignedIndex = 0;

        float getClassFactor()
        {
            _classfactor = StarDefaults.ClassFactor(this);
            return _classfactor;
        }
        float getPhysicsRadius()
        {
            return radius * 1200f;
        }
        float getAcDiscRadius()
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

        float getDysonRadius()
        {
            _dysonRadius = StarDefaults.DysonRadius(this);
            return _dysonRadius;
        }
        float getHabitableRadius()
        {
            _habitableRadius = StarDefaults.HabitableRadius(this);
            return _habitableRadius;
        }
        float getLightBalanceRadius()
        {
            _lightBalanceRadius = StarDefaults.LightBalanceRadius(this);
            return _lightBalanceRadius;
        }
        float getRadius()
        {
            _radius = StarDefaults.Radius(this);
            return _radius;
        }
        float getAge()
        {
            _age = StarDefaults.Age(this);
            return _age;

        }
        float getMass()
        {
            _mass = StarDefaults.Mass(this);
            return _mass;
        }
        float getLuminosity()
        {
            _luminosity = StarDefaults.Luminosity(this);
            return _luminosity;
        }

        float getTemperature()
        {
            _temperature = StarDefaults.Temperature(this);
            return _temperature;
        }
        float getLifetime()
        {
            _lifetime = StarDefaults.Lifetime(this);
            return _lifetime;
        }
        float getColor()
        {

            _color = StarDefaults.Color(this);
            return _color;
        }
        float getResourceCoef()
        {
            float num1 = (float)position.magnitude / 32f;
            if ((double)num1 > 1.0)
                num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
            _resourceCoef = Mathf.Pow(7f, num1) * 0.6f;
            return resourceCoef;
        }
        VectorLF3 getPos()
        {
            _pos = GS2.tmp_poses[assignedIndex];
            return _pos;
        }
    }

}