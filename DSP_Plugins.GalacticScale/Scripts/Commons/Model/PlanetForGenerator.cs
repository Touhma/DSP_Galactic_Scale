using System.Collections.Generic;
using JetBrains.Annotations;

namespace GalacticScale.Scripts {
    public class PlanetForGenerator {
        public int genSeed;

        public PlanetForGenerator hostPlanet;
        public int infoSeed;
        public bool isGasGiant;

        public List<PlanetForGenerator> moons;
        public string name = "";
        public int number;
        public int orbitAround;
        public int orbitIndex;
        public int planetIndex;

        public PlanetForGenerator(
            int planetIndex,
            int orbitAround,
            int orbitIndex,
            int number,
            bool isGasGiant,
            int infoSeed,
            int genSeed,
            [CanBeNull] PlanetForGenerator host) {
            this.planetIndex = planetIndex;
            this.orbitAround = orbitAround;
            this.orbitIndex = orbitIndex;
            this.number = number;
            this.isGasGiant = isGasGiant;
            this.infoSeed = infoSeed;
            this.genSeed = genSeed;

            moons = new List<PlanetForGenerator>();
            hostPlanet = host;
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
            moons.Add(new PlanetForGenerator(index, planetIndex + 1, orbitIndex, moons.Count + 1, false, genSeed, infoSeed, this));
        }

        public void GenerateThePlanet(ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc) {
            PlanetGen.CreatePlanet(galaxy, star, gameDesc, planetIndex, orbitAround, orbitIndex, number, isGasGiant, infoSeed, genSeed);
        }
    }
}