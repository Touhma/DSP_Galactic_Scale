using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GalacticScale
{
    public class GSUIRangeSlider : MonoBehaviour
    {
        public InputField _inputLow;
        public InputField _inputHigh;
        public Button _button;
        public RangeSlider _slider;
        public Text _labelText;
        public Text _hintText;
        public bool sliderActive = true;
        public Text _lowValueText;
        public Text _highValueText;
        public GSOptionCallback OnChange;
        public GSOptionCallback OnHighChange;
        public GSOptionCallback OnLowChange;

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

        public bool interactable
        {
            get => _slider.interactable;
            set => _slider.interactable = value;
        }

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

        public void Start()
        {
            var lowValue = (int)(_slider.LowValue * 100f) / 100f;
            var highValue = (int)(_slider.HighValue * 100f) / 100f;
            _lowValueText.text = lowValue.ToString();
            _highValueText.text = highValue.ToString();
            _slider.OnValueChanged.AddListener(OnSliderValueChange);
            _inputLow.onValueChanged.AddListener(onLowInputChange);
            _inputHigh.onValueChanged.AddListener(onHighInputChange);
            _button.onClick.AddListener(OnNumClick);
        }

        public void OnNumClick()
        {
            sliderActive = !sliderActive;
            if (sliderActive)
            {
                
                _inputLow.gameObject.SetActive(false);
                _inputHigh.gameObject.SetActive(false);
                _slider.gameObject.SetActive(true);
            }
            else
            {
                
                _inputLow.gameObject.SetActive(true);
                _inputHigh.gameObject.SetActive(true);
                _slider.gameObject.SetActive(false);
                _inputLow.text = _lowValueText.text;
                _inputHigh.text = _highValueText.text;
            }
        }
        public void onLowInputChange(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            _slider.LowValue = result;
            OnChange?.Invoke(new FloatPair(result, _slider.HighValue));
            OnLowChange?.Invoke(result);
        }
        public void onHighInputChange(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            _slider.HighValue = result;
            OnChange?.Invoke(new FloatPair(_slider.LowValue, _slider.HighValue));
            OnHighChange?.Invoke(result);
        }
        public void OnSliderValueChange(float LowValue, float HighValue)
        {
            // GS2.Log($"SliderValChange(Range) { LowValue} {HighValue}");
            var lowValue = (int)(LowValue * 100f) / 100f;
            var highValue = (int)(HighValue * 100f) / 100f;
            _lowValueText.text = lowValue.ToString();
            _highValueText.text = highValue.ToString();

            OnChange?.Invoke(new FloatPair(lowValue, highValue));
            if (OnLowChange != null) OnLowChange?.Invoke(lowValue);
            if (OnHighChange != null) OnHighChange?.Invoke(highValue);
        }

        public void initialize(GSUI options)
        {
            // GS2.Log("Initializing");
            Label = options.Label;
            Hint = options.Hint;
            var sc = (GSRangeSliderConfig)options.Data;
            //_dropdown.AddOptions(options.Data as List<string>);
            // LowValue = sc.defaultLowValue;
            // HighValue = sc.defaultHighValue;
            minValue = sc.minValue;
            maxValue = sc.maxValue;
            OnLowChange = sc.callbackLow;
            OnHighChange = sc.callbackHigh;
            OnChange = options.callback;
            _slider.WholeNumbers = options.increment % 1f == 0;
            // options.postfix?.Invoke();
        }
    }
}