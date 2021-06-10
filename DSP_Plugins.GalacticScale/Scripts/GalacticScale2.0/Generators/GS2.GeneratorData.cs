using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static iGenerator generator = new Generators.Vanilla();
        public static List<iGenerator> generators = new List<iGenerator>() {
            new Generators.BallTest(),
            new Generators.Vanilla(),
            new Generators.GS2Generator(),
            //new Generators.PhysicsTest(),
            new Generators.TintTest(),
            new Generators.Sol(), 
            new Generators.JsonImport(),
            new Generators.ThemeViewer(),
            //new Generators.Spiral(),
            new Generators.Debug(),
            new Generators.StarTest()
        };
        public static iGenerator GetGeneratorByID(string guid)
        {
            foreach (iGenerator g in generators) if (g.GUID == guid) return g;
            return new Generators.Vanilla();
        }
        public static int GetCurrentGeneratorIndex()
        {
            for (var i = 0; i < generators.Count; i++) if (generators[i] == generator) return i;
            return -1;
        }
    }
}