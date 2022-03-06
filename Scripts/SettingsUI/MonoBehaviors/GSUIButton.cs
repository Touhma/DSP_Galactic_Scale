using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIButton : MonoBehaviour
    {
        public Button _button;
        public Text _labelText;
        public Text _hintText;
        public Text _buttonText;
        public GSOptionCallback OnClick;

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
            if (OnClick != null) OnClick.Invoke("Click");
        }

        public void initialize(GSUI options)
        {
            // GS2.Log("Initializing");
            //_dropdown.AddOptions(options.Data as List<string>);
            Caption = (string)options.Data;
            Canvas.ForceUpdateCanvases();
            Label = options.Label;
            Hint = options.Hint;
            OnClick = options.callback;
            //options.postfix?.Invoke();
        }
    }
}