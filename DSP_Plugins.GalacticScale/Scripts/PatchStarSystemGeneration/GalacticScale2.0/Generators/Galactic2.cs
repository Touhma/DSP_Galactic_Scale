namespace GalacticScale.Generators
{
    public class GalacticScale2 : iGenerator
    {
        public string Name => "GalacticScale2";

        public string Author => "innominata";

        public string Description => "Just like the other generators, but more so";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.galacticscale2";

        public bool DisableStarCountSlider => false;

        public void Init()
        {
            
        }

        public void Generate(int starCount)
        {
            GS2.Log("Wow, this worked. GalacticScale2");
        }
    }
}