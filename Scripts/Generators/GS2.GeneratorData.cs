using System.Collections.Generic;
using GalacticScale.Generators;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static iGenerator ActiveGenerator
        {
            get
            {
                return _activeGenerator;
            }
            set
            {
                // Log($"ActiveGenerator: {value.Name} set by {GetCaller()}");
                _activeGenerator = value;
            }
        }

        private static iGenerator _activeGenerator = new Vanilla();
        public static List<iGenerator> Generators = new()
        {
            new GS2Generator3(),
            // new Vanilla(),
            //new GS2Generator(),
            new GS2Generator2(),
            //new Generators.SizeTest(),
            //new Generators.TintTest(),
            new Sol()
            // new JsonImport(),
            // new Generators.ThemeViewer(),
            //new Generators.Spiral(),
            //new Debug()
            //new Generators.StarTest()
        };

        public static iGenerator GetGeneratorByID(string guid)
        {
            foreach (var g in Generators)
                if (g.GUID == guid)
                    return g;

            return GetGeneratorByID("space.customizing.generators.gs2dev");
        }

        public static int GetCurrentGeneratorIndex()
        {
            // Log("GetCurrentGenIndex");
            // LogJson(Generators);
            // Log("ActiveGenerator: " + ActiveGenerator.Name);
            for (var i = 0; i < Generators.Count; i++)
                if (Generators[i] == ActiveGenerator)
                    return i;

            return -1;
        }
    }
}