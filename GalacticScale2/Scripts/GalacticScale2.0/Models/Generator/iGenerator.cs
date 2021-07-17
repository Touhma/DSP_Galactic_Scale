namespace GalacticScale
{
    public interface iGenerator
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        string GUID { get; }
        GSGeneratorConfig Config { get; }
        void Generate(int starCount);
        void Init();
    }
}