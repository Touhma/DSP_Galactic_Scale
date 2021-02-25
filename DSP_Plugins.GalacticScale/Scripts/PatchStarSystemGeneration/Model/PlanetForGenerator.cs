using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public class PlanetForGenerator {
        public int planetIndex;
        public int orbitAround;
        public int orbitIndex;
        public int number;
        public bool isGasGiant;

        public List<PlanetForGenerator> moons;

        public PlanetForGenerator hostPlanet;

        public PlanetForGenerator(
            int planetIndex,
            int orbitAround,
            int orbitIndex,
            int number,
            bool isGasGiant, 
            [CanBeNull] PlanetForGenerator host) {
            this.planetIndex = planetIndex;
            this.orbitAround = orbitAround;
            this.orbitIndex = orbitIndex;
            this.number = number;
            this.isGasGiant = isGasGiant;

            moons = new List<PlanetForGenerator>();
            hostPlanet = host;
        }

        //debug string
        public string ToString() {
            return "index : " + this.planetIndex + "\n" +
                   "orbitAround : " + this.orbitAround + "\n" +
                   "orbitIndex : " + this.orbitIndex + "\n" +
                   "number : " + this.number + "\n" +
                   "isGasGiant : " + this.isGasGiant + "\n";
        }

        public void AddMoonInOrbit(int index, int orbitIndex) {
            moons.Add(new PlanetForGenerator(index, this.planetIndex + 1, orbitIndex, this.moons.Count + 1, false, this));
        }

        public void GenerateThePlanet(ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc, int infoSeed, int genSeed) {
            PlanetGen.CreatePlanet( galaxy, star,  gameDesc, this.planetIndex, this.orbitAround, this.orbitIndex, this.number, this.isGasGiant, infoSeed, genSeed);
        }
    }
}