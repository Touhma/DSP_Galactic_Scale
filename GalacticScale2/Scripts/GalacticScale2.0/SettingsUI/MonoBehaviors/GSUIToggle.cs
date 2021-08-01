﻿using System;
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
            set => _Toggle.isOn = value;
        }

        public GSOptionCallback OnChange;

        public void _OnToggleChange(bool value)
        {
            Value = value;
            OnChange?.Invoke(value);
        }
    }

}