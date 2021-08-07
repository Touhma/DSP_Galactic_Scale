using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIHeader : MonoBehaviour
    {
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

        public void initialize(GSUI options)
        {
            // GS2.Log("Initializing");
            //_dropdown.AddOptions(options.Data as List<string>);
            Label = options.Label;
            Hint = options.Hint;
        }
    }
}