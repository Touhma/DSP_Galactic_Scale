namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public class PlanetReady {

        public int index;
        public int orbitAround;
        public int orbitIndex;
        public int number;
        public bool gasGiant;
        public int infoSeed;
        public int genSeed;

        public PlanetReady(
            int index,
            int orbitAround,
            int orbitIndex,
            int number,
            bool gasGiant,
            int infoSeed,
            int genSeed) {
            
            this.index = index;
            this.orbitAround = orbitAround;
            this.orbitIndex = orbitIndex;
            this.number = number;
            this.gasGiant = gasGiant;
            this.infoSeed = infoSeed;
            this.genSeed = genSeed;
        }

        public string ToString() {
            return "index : " + this.index + "\n" +
                   "orbitAround : " + this.orbitAround + "\n" +
                   "orbitIndex : " + this.orbitIndex + "\n" +
                   "number : " + this.number + "\n" +
                   "gasGiant : " + this.gasGiant + "\n";


        }
    }
}