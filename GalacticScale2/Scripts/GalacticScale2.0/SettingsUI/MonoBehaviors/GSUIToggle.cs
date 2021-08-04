using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIToggle : MonoBehaviour
    {

        public Toggle _Toggle;
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
        public bool Value
        {
            get => _Toggle.isOn;
            set { GS2.Warn($"Setting Value for {Label} to {value}"); _Toggle.isOn = value; }
        }

        public GSOptionCallback OnChange;

        public void _OnToggleChange(bool value)
        {
            GS2.Log(value.ToString());
            Value = value;
            OnChange?.Invoke(value);
        }
        public void initialize(GSUI options)
        {
            GS2.Log($"Initializing {Label} {options.Data} {options.DefaultValue} {(options.postfix == null)}");
            //Value = (bool) options.Data;
            Label = options.Label;
            OnChange = options.callback;
            options.postfix?.Invoke();
        }
    }

}