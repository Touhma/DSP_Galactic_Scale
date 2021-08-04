using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GalacticScale
{
    public class GSUIRangeSlider : MonoBehaviour
    {
        public float minValue
        {
            get => _slider.MinValue;
            set => _slider.MinValue = value;
        }
        public float maxValue
        {
            get => _slider.MaxValue;
            set => _slider.MaxValue = value;
        }
        public GSOptionCallback OnLowChange;
        public GSOptionCallback OnHighChange;
        public RangeSlider _slider;
        public Text _labelText;
        public Text _hintText;
        public string Hint
        {
            get => _hintText.text;
            set => _hintText.text = value;
        }
        public string Label
        {
            get => _labelText.text;
            set => _labelText.text = value;
        }
        public float LowValue
        {
            get => _slider.LowValue;
            set => _slider.LowValue = value;
        }
        public float HighValue
        {
            get => _slider.HighValue;
            set => _slider.HighValue = value;
        }
        public bool WholeNumbers
        {
            get => _slider.WholeNumbers;
            set => _slider.WholeNumbers = value;
        }

        public Text _lowValueText;
        public Text _highValueText;
        public void OnSliderValueChange(float LowValue, float HighValue)
        {
            GS2.Log("SliderValChange(Range)");
            float lowValue = (int)(LowValue * 100) / 100f;
            float highValue = (int)(HighValue * 100) / 100f;
            _lowValueText.text = lowValue.ToString();
            _highValueText.text = highValue.ToString();
            if (OnLowChange != null) OnLowChange.Invoke(lowValue);
            if (OnHighChange != null) OnHighChange.Invoke(highValue);
        }
        public void Start()
        {
            float lowValue = (int)(_slider.LowValue * 100) / 100f;
            float highValue = (int)(_slider.HighValue * 100) / 100f;
            _lowValueText.text = lowValue.ToString();
            _highValueText.text = highValue.ToString();
            _slider.OnValueChanged.AddListener(OnSliderValueChange);
        }
        public void initialize(GSUI options)
        {
            GS2.Log("Initializing");

            GSSliderConfig sc = (GSSliderConfig)options.Data;
            //_dropdown.AddOptions(options.Data as List<string>);
            //Value = sc.defaultValue;
            //minValue = sc.minValue;
            //maxValue = sc.maxValue;
            //OnChange = options.callback;
            //options.postfix?.Invoke();

        }

    }

}