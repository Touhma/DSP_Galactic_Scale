using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorSystem {

        public GeneratorStar SystemStar;
        public List<GeneratorPlanet> SystemPlanets;

        
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

        public GeneratorSystem() {
            PlanetsInOrbit = new List<GeneratorPlanet>();
        }
        
        //Helper to add something in orbit around the star
        public void AddBodyInOrbit(GeneratorPlanet planet) {
            PlanetsInOrbit.Add(planet);
        }
    }
}