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
        GSGeneratorConfig Config { get; }
       
        void Init();
    }
    public interface iConfigurableGenerator : iGenerator
    {
        System.Collections.Generic.List<GS2.GSOption> Options { get; }
        void Import(object preferences);
        object Export();
    }
    public class GSGeneratorConfig
    {
        public bool DisableStarCountSlider = false;
        public bool DisableSeedInput = false;
        public int MinStarCount = 8;
        public int MaxStarCount = 1024;
        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false, int minStarCount = 8, int maxStarCount = 1024)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
            MinStarCount = minStarCount;
            MaxStarCount = maxStarCount;
        }
    }
}