using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSTerrainAlgorithm(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0);
    public class TerrainAlgorithmLibrary : Dictionary<string, GSTerrainAlgorithm> {
        public static TerrainAlgorithmLibrary Init() {
            return new TerrainAlgorithmLibrary()
            {
                ["GSTA1"] = VeinAlgorithms.GenerateTerrain1,
                ["GSTA3"] = VeinAlgorithms.GenerateTerrain3,
                ["GSTA5"] = VeinAlgorithms.GenerateTerrain5,
                ["GSTA6"] = VeinAlgorithms.GenerateTerrain6
            };
        }
        public GSTerrainAlgorithm Find(string name)
        {
            //GS2.Log("TerrainAlgorithmLibrary|Find(" + name + ")");
            if (!ContainsKey(name)) { GS2.Warn("TerrainAlgorithmLibrary|Find|Algorithm '" + name + "' Not Found. Using Default"); return VeinAlgorithms.GenerateTerrain1; }
            return this[name];
        }
    }
}
