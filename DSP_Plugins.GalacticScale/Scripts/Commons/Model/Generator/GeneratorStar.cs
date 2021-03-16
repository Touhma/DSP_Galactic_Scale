using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorStar {
        public int Seed;

        // Star information :
        public ESpectrType Spectr;
        public EStarType Type;

        public string Name = string.Empty;

        public float Mass;
        public float Lifetime;
        public float Age;
        public float Temperature;
        public float ClassFactor;
        public float Color;
        public float Luminosity;
        public float Radius;
        public float AccretionDiskRadius;
        public float HabitableRadius;
        public float LightBalanceRadius;
        public float OrbitScaler;

        // Details on the composition of the system

        public List<GeneratorPlanet> PlanetsInOrbit;

        public int NbOfGasGiantPlanets = 0;
        public int NbOfMoons = 0;
        public int NbOfMoonsGasGiant = 0;
        public int NbOfMoonsTelluric = 0;
        public int NbOfPlanets = 0;
        public int NbOfStellarBodies = 0;
        public int NbOfTelluricPlanets = 0;

        // Asteroids belt related stuff
        public int asterBelt1OrbitIndex;
        public int asterBelt2OrbitIndex;
        public float asterBelt1Radius;
        public float asterBelt2Radius;

        public GeneratorStar(int seed, ESpectrType spectr, EStarType type) {
            Seed = seed;
            Spectr = spectr;
            Type = type;
            PlanetsInOrbit = new List<GeneratorPlanet>();
        }

        //Helper to add something in orbit around the star
        public void AddBodyInOrbit(GeneratorPlanet planet) {
            PlanetsInOrbit.Add(planet);
        }

        // TODO : Create the method that actually generate the system
        public void GenerateSystem() { }
    }
}