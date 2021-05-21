using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSVegeAlgorithm(GSPlanet gsPlanet);
    public class VegeAlgorithmLibrary : Dictionary<string, GSVegeAlgorithm> {
        public static VegeAlgorithmLibrary Init() {
            return new VegeAlgorithmLibrary()
            {

            };
        }
    }
}
