using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSVeinAlgorithm(GSPlanet gsPlanet, bool sketchOnly);

    public class VeinAlgorithmLibrary : Dictionary<string, GSVeinAlgorithm>
    {
        public static VeinAlgorithmLibrary Init()
        {
            return new VeinAlgorithmLibrary
            {
                ["Vanilla"] = VeinAlgorithms.GenerateVeinsVanilla,
                ["GS2"] = VeinAlgorithms.GenerateVeinsGS2,
                ["GS2R"] = VeinAlgorithms.GenerateVeinsGS2,
                ["GS2W"] = VeinAlgorithms.GenerateVeinsGS2W
            };
        }

        public GSVeinAlgorithm Find(string name)
        {
            if (!ContainsKey(name))
                // GS2.Warn("VeinAlgorithmLibrary|Find|Algorithm '" + name + "' Not Found. Using Default");
                return VeinAlgorithms.GenerateVeinsGS2;

            return this[name];
        }
    }
}