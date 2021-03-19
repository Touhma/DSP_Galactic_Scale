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



        public GeneratorStar(int seed, ESpectrType spectr, EStarType type) {
            Seed = seed;
            Spectr = spectr;
            Type = type;

        }

    

        // TODO : Create the method that actually generate the system
        public void GenerateSystem() { }
    }
}