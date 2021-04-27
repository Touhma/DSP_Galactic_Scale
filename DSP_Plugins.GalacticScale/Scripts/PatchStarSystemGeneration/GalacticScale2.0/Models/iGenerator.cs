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
        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
        }
    }
}