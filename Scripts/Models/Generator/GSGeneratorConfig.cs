using UnityEngine;

namespace GalacticScale
{
    public class GSGeneratorConfig
    {
        private int _defaultStarCount = 1;
        private int _maxStarCount = 64;
        private int _minStarCount = 1;
        public bool DisableSeedInput;
        public bool DisableStarCountSlider;
        public bool enableStarSelector;

        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false, int minStarCount = 1, int maxStarCount = 1024, int defaultStarCount = 1, bool enableSelector = false)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
            MinStarCount = minStarCount;
            MaxStarCount = maxStarCount;
            DefaultStarCount = defaultStarCount;
            enableStarSelector = enableSelector;
        }

        [SerializeField]
        public int MinStarCount
        {
            get => _minStarCount;
            set => _minStarCount = (int)Maths.Clamp(value, 1.0, _maxStarCount);
        }

        [SerializeField]
        public int DefaultStarCount
        {
            get => _defaultStarCount;
            set => _defaultStarCount = (int)Maths.Clamp(value, _minStarCount, _maxStarCount);
        }

        [SerializeField]
        public int MaxStarCount
        {
            get => _maxStarCount;
            set => _maxStarCount = (int)Maths.Clamp(value, _minStarCount, 8092);
        }
    }
}