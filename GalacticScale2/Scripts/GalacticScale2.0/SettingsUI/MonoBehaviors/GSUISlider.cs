using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUISlider : MonoBehaviour
    {
        public Slider _slider;
        public Text _labelText;
        public Text _hintText;
        public Text _valueText;
        public string negativeLabel = "";
        public GSOptionCallback OnChange;

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
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

        public float minValue
        {
            get => _slider.minValue;
            set => _slider.minValue = value;
        }

        public float maxValue
        {
            get => _slider.maxValue;
            set => _slider.maxValue = value;
        }

        public void OnSliderValueChange(Slider slider)
        {
            // GS2.Warn("*");
            var value = (int)(slider.value * 100) / 100f;
            _valueText.text = value.ToString();
            if (negativeLabel != "" && value < 0) _valueText.text = negativeLabel;
            OnChange?.Invoke(value);
        }

        public void initialize(GSUI options)
        {
            // GS2.Log("Initializing");

            var sc = (GSSliderConfig)options.Data;
            //_dropdown.AddOptions(options.Data as List<string>);
            //Value = sc.defaultValue;
            Label = options.Label;
            Hint = options.Hint;
            var wholenumbers = options.increment % 1f == 0;
            _slider.wholeNumbers = wholenumbers;
            negativeLabel = sc.negativeLabel;
            minValue = sc.minValue;
            maxValue = sc.maxValue;
            OnChange = options.callback;
            //options.postfix?.Invoke();
        }
    }
}