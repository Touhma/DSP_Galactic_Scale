using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSTerrainAlgorithm(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0);

    public class TerrainAlgorithmLibrary : Dictionary<string, GSTerrainAlgorithm>
    {
        public static TerrainAlgorithmLibrary Init()
        {
            return new TerrainAlgorithmLibrary
            {
                ["GSTA1"] = TerrainAlgorithms.GenerateTerrain1,
                ["GSTA3"] = TerrainAlgorithms.GenerateTerrain3,
                ["GSTA5"] = TerrainAlgorithms.GenerateTerrain5,
                ["GSTA6"] = TerrainAlgorithms.GenerateTerrain6,
                ["GSTA00"] = TerrainAlgorithms.GenerateTerrain00
            };
        }

        public GSTerrainAlgorithm Find(string name)
        {
            if (!ContainsKey(name))
            {
                GS2.Warn("TerrainAlgorithmLibrary|Find|Algorithm '" + name + "' Not Found. Using Default");
                return TerrainAlgorithms.GenerateTerrain1;
            }

            return this[name];
        }
    }
}