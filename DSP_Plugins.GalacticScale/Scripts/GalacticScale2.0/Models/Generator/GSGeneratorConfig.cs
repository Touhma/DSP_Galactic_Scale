using UnityEngine;

namespace GalacticScale
{
    public class GSGeneratorConfig
    {
        public bool DisableStarCountSlider = false;
        public bool DisableSeedInput = false;
        private int _minStarCount = 1;
        private int _defaultStarCount = 1;
        [SerializeField]
        public int MinStarCount{ get => _minStarCount; set => _minStarCount = (int)Maths.Clamp(value, 1.0, _maxStarCount); }
        public int DefaultStarCount { get => _defaultStarCount; set => _defaultStarCount = (int)Maths.Clamp(value,_minStarCount, _maxStarCount); }
        private int _maxStarCount = 64;
        [SerializeField]
        public int MaxStarCount { get => _maxStarCount; set => _maxStarCount = (int)Maths.Clamp((double)value, _minStarCount, 1024); }
        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false, int minStarCount = 1, int maxStarCount = 1024, int defaultStarCount = 1)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
            MinStarCount = minStarCount;
            MaxStarCount = maxStarCount;
            DefaultStarCount = defaultStarCount;
        }
    }
}