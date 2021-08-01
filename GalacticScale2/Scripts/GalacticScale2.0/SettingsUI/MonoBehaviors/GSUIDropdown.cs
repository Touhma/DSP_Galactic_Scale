using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIDropdown : MonoBehaviour
    {
        public Dropdown _dropdown;

        public GSOptionCallback OnChange;
        public Text _labelText;
        public Text _hintText;
        public List<string> Items
        {
            get=> _dropdown.options.Select((option)=>option.text).ToList();
            set => _dropdown.options = value.Select((s) => new Dropdown.OptionData() {text = s}).ToList();
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
        public string Value
        {
            get
            {
                if (Items.Count < 1 || Items.Count >= _dropdown.value)
                {
                    GS2.Warn($"Index out of bounds: {Label} {_dropdown.value}");
                    return null;
                }
                return Items[_dropdown.value];
            }
            set => _dropdown.value = Items.IndexOf(value);
        }

        public void OnValueChange(int value)
        {
            if (Items.Count < 1 || Items.Count >= value)
            {
                GS2.Warn($"Index out of bounds: {Label} {value}");
                return;
            }
            Value = Items[value];
            OnChange?.Invoke(Items[value]);
        }
        
    }

}