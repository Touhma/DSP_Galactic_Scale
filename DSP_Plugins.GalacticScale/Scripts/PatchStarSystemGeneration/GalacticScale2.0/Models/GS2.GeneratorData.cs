using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static iGenerator generator = new Generators.Dummy();
        public static List<iGenerator> generators = new List<iGenerator>() { new Generators.Vanilla(), new Generators.GalacticScale2(), new Generators.JsonImport() , new Generators.Dummy()};
    }
}