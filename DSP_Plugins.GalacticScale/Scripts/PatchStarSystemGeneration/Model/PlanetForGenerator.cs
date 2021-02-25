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
        public int genSeed;
        public int infoSeed;
        public string name = "";

        public List<PlanetForGenerator> moons;

        public PlanetForGenerator hostPlanet;

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
            return "index : " + this.planetIndex + "\n" +
                   "orbitAround : " + this.orbitAround + "\n" +
                   "orbitIndex : " + this.orbitIndex + "\n" +
                   "number : " + this.number + "\n" +
                   "isGasGiant : " + this.isGasGiant + "\n";
        }

        public void AddMoonInOrbit(int index, int orbitIndex,int genSeed,int infoSeed) {
            moons.Add(new PlanetForGenerator(index, this.planetIndex + 1 , orbitIndex, this.moons.Count + 1, false, genSeed,infoSeed,this));
        }

        public void GenerateThePlanet(ref GalaxyData galaxy, ref StarData star, ref GameDesc gameDesc) {
            PlanetGen.CreatePlanet( galaxy, star,  gameDesc, this.planetIndex, this.orbitAround, this.orbitIndex, this.number, this.isGasGiant, this.infoSeed, this.genSeed);
        }
    }
}