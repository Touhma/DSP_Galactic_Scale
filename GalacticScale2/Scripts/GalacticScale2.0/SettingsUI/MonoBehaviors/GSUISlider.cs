using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUISlider : MonoBehaviour
    {
        public GSOptionCallback OnChange;
        public Slider _slider;
        public Text _labelText;
        public Text _hintText;
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
        public Text _valueText;

        public void OnSliderValueChange(Slider slider)
        {
            float value = (int)(slider.value * 100) / 100f;
            _valueText.text = value.ToString();
            OnChange?.Invoke(value);
        }
        public void initialize(GSUI options)
        {
            GS2.Log("Initializing");

            GSSliderConfig sc = (GSSliderConfig) options.Data;
            //_dropdown.AddOptions(options.Data as List<string>);
            //Value = sc.defaultValue;
            Label = options.Label;
            minValue = sc.minValue;
            maxValue = sc.maxValue;
            OnChange = options.callback;
            //options.postfix?.Invoke();

        }

    }

}