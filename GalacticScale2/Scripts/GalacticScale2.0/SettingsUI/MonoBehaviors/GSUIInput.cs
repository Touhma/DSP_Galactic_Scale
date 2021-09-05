using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIInput : MonoBehaviour
    {
        public InputField _input;
        public Text _textText;
        public Text _labelText;
        public Text _hintText;
        public Input input;

        public GSOptionCallback OnChange;
        public GSOptionCallback OnEndEdit;

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

        public string Value
        {
            get => _textText.text;
            set => _textText.text = value;
        }

        public void OnInputChange(string value)
        {
            // GS2.Log(value);
            Value = value;
            OnChange?.Invoke(value);
        }

        public void OnInputEndEdit(string value)
        {
            // GS2.Log(value);
            Value = value;
            OnEndEdit?.Invoke(value);
        }

        public void initialize(GSUI options)
        {
            //GS2.Log("Initializing");
            Label = options.Label;
            Hint = options.Hint;
            // Value = (string)options.Data;
            OnChange = options.callback;
            //options.postfix?.Invoke();
        }
    }
}