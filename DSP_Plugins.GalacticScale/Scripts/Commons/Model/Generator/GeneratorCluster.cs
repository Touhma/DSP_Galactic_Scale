using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorCluster {
        public int seed;
        
        //Infos on the Cluster
        public Dictionary<EStarType, int> NbOfStarByType = new Dictionary<EStarType, int>();
        public Dictionary<ESpectrType, int> NbOfStarBySpectre = new Dictionary<ESpectrType, int>();
        
        public List<GeneratorStar> ClusterStars = new List<GeneratorStar>();
    }
}