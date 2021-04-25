namespace GalacticScale.Generators
{
    public class JsonImport : iGenerator
    {
        string iGenerator.Name => "Custom Json";

        string iGenerator.Author => "innominata";

        string iGenerator.Description => "Nothing left to chance. This allows external generators to create a universe description.";

        string iGenerator.Version => "0.0";

        string iGenerator.GUID => "space.customizing.generators.customjson";


        GSSettings iGenerator.Generate(int starCount)
        {
            GS2.Log("Wow, this worked. Json");
            string path = System.IO.Path.Combine(GS2.DataDir, "GSData.json");
            GS2.LoadSettingsFromJson(path);
            return GSSettings.Instance;
        }
    }
}