namespace GalacticScale.Generators
{
    public class JsonImport : iGenerator
    {
        string iGenerator.Name => "Custom Json";

        string iGenerator.Author => "innominata";

        string iGenerator.Description => "Nothing left to chance. This allows external generators to create a universe description.";

        string iGenerator.Version => "0.0";

        string iGenerator.GUID => "space.customizing.generators.customjson";


        void iGenerator.Generate(int starCount, ref GSSettings settings)
        {
            GS2.Log("Wow, this worked. Json");
        }
    }
}