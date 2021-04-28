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
        private int _minStarCount = 4;
        public int MinStarCount{ get => _minStarCount; set => _minStarCount = (int)Maths.Clamp(value, 1.0, _maxStarCount); }
        private int _maxStarCount = 64;
        public int MaxStarCount { get => _maxStarCount; set => _maxStarCount = (int)Maths.Clamp((double)value, _minStarCount, 1024); }
        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false, int minStarCount = 1, int maxStarCount = 1024)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
            MinStarCount = minStarCount;
            MaxStarCount = maxStarCount;
        }
    }
}