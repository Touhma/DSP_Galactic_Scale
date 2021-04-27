namespace GalacticScale
{
    public interface iGenerator
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        string GUID { get; }
        void Generate(int starCount);
        bool DisableStarCountSlider { get; }
       
        void Init();
    }
    public interface iConfigurableGenerator : iGenerator
    {
        System.Collections.Generic.List<GS2.GSOption> Options { get; }
        void Import(object preferences);
        object Export();
    }
}