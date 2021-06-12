namespace GalacticScale {
    public interface iGenerator {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        string GUID { get; }
        void Generate(int starCount);
        GSGeneratorConfig Config { get; }
        void Init();
    }
}