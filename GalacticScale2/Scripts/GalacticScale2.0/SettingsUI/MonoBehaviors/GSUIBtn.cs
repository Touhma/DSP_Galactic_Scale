using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIBtn : MonoBehaviour
    {
        public Button _button;
        public GSOptionCallback OnClick;
        public GSUITemplates templates;
        public Text _labelText;
        public Text _hintText;
        public Text _buttonText;
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

        public string Caption
        {
            get => _buttonText.text;
            set => _buttonText.text = value;
        }
        public void OnButtonClick()
        {
            if (OnClick != null) OnClick.Invoke(null);
        }
    }

}