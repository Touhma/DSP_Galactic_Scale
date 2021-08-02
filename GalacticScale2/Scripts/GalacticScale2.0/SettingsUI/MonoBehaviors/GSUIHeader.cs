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
    }

}