namespace GalacticScale.Generators
{
    public class Vanilla : iGenerator
    {
        public string Name => "Vanilla";

        public string Author => "innominata";

        public string Description => "An attempt to mimic the standard generation of dyson sphere program";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.vanilla";

        public bool DisableStarCountSlider => false;

        public void Init()
        {

        }

        public void Generate(int starCount)
        {
            GS2.Log("Wow, this worked");
        }
    }
}