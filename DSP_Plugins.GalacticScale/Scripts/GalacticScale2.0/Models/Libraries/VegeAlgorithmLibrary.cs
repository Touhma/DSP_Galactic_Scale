using System.Collections.Generic;

namespace GalacticScale
{
    public delegate void GSVegeAlgorithm(GSPlanet gsPlanet);

    public class VegeAlgorithmLibrary : Dictionary<string, GSVegeAlgorithm>
    {
        public static VegeAlgorithmLibrary Init()
        {
            return new VegeAlgorithmLibrary
            {
                ["GS2"] = VegeAlgorithms.GenerateVeges1
            };
        }

        public GSVegeAlgorithm Find(string name)
        {
            if (!ContainsKey(name))
            {
                GS2.Warn("VegeAlgorithmLibrary|Find|Algorithm '" + name + "' Not Found. Using Default");
                return VegeAlgorithms.GenerateVeges1;
            }

            return this[name];
        }
    }
}