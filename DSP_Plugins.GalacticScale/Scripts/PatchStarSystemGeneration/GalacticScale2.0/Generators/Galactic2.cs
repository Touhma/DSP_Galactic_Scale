namespace GalacticScale.Generators
{
    public class GalacticScale2 : iGenerator
    {
        string iGenerator.Name => "GalacticScale2";

        string iGenerator.Author => "innominata";

        string iGenerator.Description => "Just like the other generators, but more so";

        string iGenerator.Version => "0.0";

        string iGenerator.GUID => "space.customizing.generators.galacticscale2";


        void iGenerator.Generate(int starCount, ref GSSettings settings)
        {
            GS2.Log("Wow, this worked. GalacticScale2");
        }
    }
}