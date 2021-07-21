using System.Collections.Generic;
using GalacticScale.Generators;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static iGenerator generator = new Vanilla();

        public static List<iGenerator> generators = new List<iGenerator>
        {
            //new Generators.BallTest(),
            new Vanilla(),
            new GS2Generator(),
            new GS2Generator2(),
            //new Generators.SizeTest(),
            //new Generators.TintTest(),
            new Sol(),
            new JsonImport()
            //new Generators.ThemeViewer(),
            //new Generators.Spiral(),
            //new Generators.Debug(),
            //new Generators.StarTest()
        };

        public static iGenerator GetGeneratorByID(string guid)
        {
            foreach (var g in generators)
                if (g.GUID == guid)
                    return g;

            return new Vanilla();
        }

        public static int GetCurrentGeneratorIndex()
        {
            for (var i = 0; i < generators.Count; i++)
                if (generators[i] == generator)
                    return i;

            return -1;
        }
    }
}