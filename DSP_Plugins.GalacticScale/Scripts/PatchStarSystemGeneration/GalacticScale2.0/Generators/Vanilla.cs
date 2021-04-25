namespace GalacticScale.Generators
{
    public class Vanilla : iGenerator
    {
        string iGenerator.Name => "Vanilla";

        string iGenerator.Author => "innominata";

        string iGenerator.Description => "An attempt to mimic the standard generation of dyson sphere program";

        string iGenerator.Version => "0.0";

        string iGenerator.GUID => "space.customizing.generators.vanilla";


        void iGenerator.Generate(int starCount, ref GSSettings settings)
        {
            GS2.Log("Wow, this worked");
        }
    }
}