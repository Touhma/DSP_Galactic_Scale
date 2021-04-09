using System.Collections.Generic;
using JetBrains.Annotations;

namespace GalacticScale.Scripts {
    public class GeneratorPlanet {
        public int genSeed;

        // Setup If the current body is a moon
        public GeneratorPlanet HostGeneratorPlanet;
        
        //Setup if the current body is in orbit around the star
        public List<GeneratorPlanet> moons;
        
        public int infoSeed;
        public bool isGasGiant;

        public string name = "";
        public int number;
        public int orbitAround;
        public int orbitIndex;
        public int planetIndex;

        public GeneratorPlanet(
            int planetIndex,
            int orbitAround,
            int orbitIndex,
            int number,
            bool isGasGiant,
            int infoSeed,
            int genSeed,
            [CanBeNull] GeneratorPlanet hostGenerator) {
            this.planetIndex = planetIndex;
            this.orbitAround = orbitAround;
            this.orbitIndex = orbitIndex;
            this.number = number;
            this.isGasGiant = isGasGiant;
            this.infoSeed = infoSeed;
            this.genSeed = genSeed;

            moons = new List<GeneratorPlanet>();
            HostGeneratorPlanet = hostGenerator;
        }

        //debug string
        public string ToStringDebug() {
            return "index : " + planetIndex + "\n" +
                   "orbitAround : " + orbitAround + "\n" +
                   "orbitIndex : " + orbitIndex + "\n" +
                   "number : " + number + "\n" +
                   "isGasGiant : " + isGasGiant + "\n";
        }

        public void AddMoonInOrbit(int index, int orbitIndex, int genSeed, int infoSeed) {
            moons.Add(new GeneratorPlanet(index, planetIndex + 1, orbitIndex, moons.Count + 1, false, genSeed, infoSeed, this));
        }
        
        // Helper to call the creation function
        public void GenerateThePlanet(ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc) {
            PlanetGen.CreatePlanet(galaxy, star, gameDesc, planetIndex, orbitAround, orbitIndex, number, isGasGiant, infoSeed, genSeed);
        }

        // Helper to call the Theme Selection function
        public void SelectTheTheme() {
            
        }
    }
}