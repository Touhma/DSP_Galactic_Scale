using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSVeinAlgorithm(GSPlanet gsPlanet, bool sketchOnly);
    public class VeinAlgorithmLibrary : Dictionary<string, GSVeinAlgorithm> {
        public static VeinAlgorithmLibrary Init() {
            return new VeinAlgorithmLibrary()
            {
                ["Vanilla"] = VeinAlgorithms.GenerateVeinsVanilla,
                ["GS2"] = VeinAlgorithms.GenerateVeinsGS2
            };
        }
    }
}
