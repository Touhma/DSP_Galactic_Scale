using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public class star
    {
        public string Name;
        public ESpectrType Spectr;
        public EStarType Type;
        public List<planet> Planets;
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
        private float _resourceCoef = 0.6f;
        private float _physicsRadius = -1;
        [SerializeField] 
        public float level = 1;
        public star(int seed, string name, ESpectrType spectr, EStarType type, List<planet> planets)
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
        public float luminosity { get => _luminosity; set => _luminosity = value; }
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
        public float classFactor { get => _classfactor; }
        [SerializeField] 
        public float resourceCoef { get => _resourceCoef < 0?getResourceCoef(): _resourceCoef; set => _resourceCoef = value; }
        [SerializeField]
        public float acDiscRadius { get => _acdiscRadius < 0 ? getAcDiscRadius() : _acdiscRadius; set => _acdiscRadius = value; }
        public VectorLF3 pos;
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
            _dysonRadius = orbitScaler * 0.28f;
            if ((double)_dysonRadius * 40000.0 < (double)physicsRadius * 1.5)
                _dysonRadius = (float)((double)physicsRadius * 1.5 / 40000.0);
            return _dysonRadius;
        }
        float getHabitableRadius()
        {
            if (_classfactor > 2) getColor();
            float p2 = _classfactor + 2f;
            _habitableRadius = Mathf.Pow(1.7f, p2) + 0.25f * Mathf.Min(1f, orbitScaler);
            return _habitableRadius;
        }
        float getLightBalanceRadius()
        {
            if (_classfactor > 2) getColor();
            float p2 = _classfactor + 2f;
            _lightBalanceRadius = Mathf.Pow(1.7f, p2);
            return _lightBalanceRadius;
        }
        float getRadius()
        {
            System.Random random = new System.Random(Seed);
            double num3 = Math.Pow(2.0, random.NextDouble() * 0.4 - 0.2);
            _radius = (float)(Math.Pow((double)mass, 0.4) * num3);
            return _radius;
        }
        float getAge()
        {
            System.Random random = new System.Random(Seed);
            double r = random.NextDouble();
            switch(Type)
            {
                case EStarType.GiantStar:
                    _age = (float)(r * 0.0399999991059303 + 0.959999978542328); break;
                case EStarType.BlackHole:
                case EStarType.NeutronStar:
                case EStarType.WhiteDwarf:
                    _age = (float)r * 0.4f + 1; break;
                default:
                _age = (double)mass >= 0.5 ? ((double)mass >= 0.8 ? (float)(r * 0.699999988079071 + 0.200000002980232) : (float)(r * 0.400000005960464 + 0.100000001490116)) : (float)(r * 0.119999997317791 + 0.0199999995529652);
                    break;
            }
            return _age;

        }
        float getMass()
        {
            System.Random random = new System.Random(Seed);
            System.Random random2 = new System.Random(Seed);
            double num3 = (random2.NextDouble() - 0.5) * 0.2;
            double y = random2.NextDouble() * 0.4 - 0.2;
            float num6 = Mathf.Lerp(-0.98f, 0.88f, level);
            float averageValue = (double)num6 >= 0.0 ? num6 + 0.65f : num6 - 0.65f;
            float standardDeviation = 0.33f;
            if (Type == EStarType.GiantStar)
            {
                averageValue = y <= -0.08 ? 1.6f : -1.5f;
                standardDeviation = 0.3f;
            }
            double r = random.NextDouble();
            double r2 = random.NextDouble();
            float num7 = GS2.RandNormal(averageValue, standardDeviation, r, r2);
            switch (Spectr)
            {
                case ESpectrType.M:
                    num7 = -3f;
                    break;
                case ESpectrType.O:
                    num7 = 3f;
                    break;
            }
            float p1 = (float)((double)Mathf.Clamp((double)num7 <= 0.0 ? num7 * 1f : num7 * 2f, -2.4f, 4.65f) + num3 + 1.0);
             
            switch (Type)
            {
                case EStarType.WhiteDwarf:
                    _mass = (float)(1.0 + r2 * 5.0); break;
                case EStarType.NeutronStar:
                    _mass = (float)(7.0 + r * 11.0); break;
                case EStarType.BlackHole:
                    _mass = (float)(18.0 + r * r2 * 30.0); break;
                default:
                    _mass = Mathf.Pow(2f, p1); break;
            }
            return _mass;
        }
        float getTemperature()
        {
            float f = (float)(1.0 - (double)Mathf.Pow(Mathf.Clamp01(age), 20f) * 0.5) * mass;
            _luminosity = Mathf.Pow(f, 0.7f);
            _temperature = (float)(Math.Pow((double)f, 0.56 + 0.14 / (Math.Log10((double)f + 4.0) / Math.Log10(5.0))) * 4450.0 + 1300.0);
            return _temperature;
        }
        float getLifetime()
        {
            System.Random random = new System.Random(Seed);
            double r = random.NextDouble();
            double d = 5.0;
            if ((double)mass < 2.0)
                d = 2.0 + 0.4 * (1.0 - (double)mass);
            _lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)mass * 0.5) / Math.Log10(d) + 1.0) * r);
            switch (Type)
            {
                case EStarType.GiantStar:
                    _lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)mass * 0.58) / Math.Log10(d) + 1.0) * r); break;
                case EStarType.WhiteDwarf:
                    _lifetime += 10000f; break;
                case EStarType.NeutronStar:
                    _lifetime += 1000f; break;
            }
            float num8 = _lifetime * age;
            if ((double)num8 > 5000.0)
                num8 = (float)(((double)Mathf.Log(num8 / 5000f) + 1.0) * 5000.0);
            if ((double)num8 > 8000.0)
                num8 = (Mathf.Log(Mathf.Log(Mathf.Log(num8 / 8000f) + 1f) + 1f) + 1f) * 8000f;
            _lifetime = num8 / age;
            return _lifetime;
        }
        float getColor()
        {
            double num9 = Math.Log10(((double)temperature - 1300.0) / 4500.0) / Math.Log10(2.6) - 0.5;
            if (num9 < 0.0)
                num9 *= 4.0;
            if (num9 > 2.0)
                num9 = 2.0;
            else if (num9 < -4.0)
                num9 = -4.0;
            _color = Mathf.Clamp01((float)((num9 + 3.5) * 0.200000002980232));
            _classfactor = (float)num9;
            return _color;
        }
        float getResourceCoef ()
        {
            float num1 = (float)pos.magnitude / 32f;
            if ((double)num1 > 1.0)
                num1 = Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(Mathf.Log(num1) + 1f) + 1f) + 1f) + 1f) + 1f;
            _resourceCoef = Mathf.Pow(7f, num1) * 0.6f;
            return resourceCoef;
        }
    }
    
}