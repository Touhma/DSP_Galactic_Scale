namespace GalacticScale.Generators
{
    public class Vanilla : iGenerator
    {
        public string Name => "Vanilla";

        public string Author => "innominata";

        public string Description => "Disable Custom Generation";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.vanilla";

        public GSGeneratorConfig Config => new();

        public void Init()
        {
        }

        public void Generate(int starCount, StarData birthStar = null)
        {
        }
    }
}