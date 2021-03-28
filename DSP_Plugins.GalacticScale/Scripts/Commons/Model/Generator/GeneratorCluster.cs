using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorCluster {
        public int seed =0;
        public int algo =0;
        public int starCount =0;
        
        public List<VectorLF3> tmp_poses;
        public List<VectorLF3> astro_poses;
        
        //Infos on the Cluster
        public Dictionary<EStarType, int> NbOfStarByType = new Dictionary<EStarType, int>();
        public Dictionary<ESpectrType, int> NbOfStarBySpectre = new Dictionary<ESpectrType, int>();
        
        public List<GeneratorSystem> ClusterSystems = new List<GeneratorSystem>();
    }
}