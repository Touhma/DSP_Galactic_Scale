using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSVeinAlgorithm(GSPlanet gsPlanet); //, bool sketchOnly);

    public class VeinAlgorithmLibrary : Dictionary<string, GSVeinAlgorithm>
    {
        public static VeinAlgorithmLibrary Init()
        {
            return new VeinAlgorithmLibrary
            {
                ["Vanilla"] = VeinAlgorithms.GenerateVeinsVanilla,
                ["GS3"] = VeinAlgorithms.GenerateVeinsGS3,
                ["GS3R"] = VeinAlgorithms.GenerateVeinsGS3,
                ["GS3W"] = VeinAlgorithms.GenerateVeinsGS3W
            };
        }

        public GSVeinAlgorithm Find(string name)
        {
            if (!ContainsKey(name))
                // GS3.Warn("VeinAlgorithmLibrary|Find|Algorithm '" + name + "' Not Found. Using Default");
                return VeinAlgorithms.GenerateVeinsGS3;

            return this[name];
        }
    }
}