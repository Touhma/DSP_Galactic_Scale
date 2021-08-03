using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIDropdown : MonoBehaviour
    {
        public Dropdown _dropdown;
        public UIComboBox _combobox;
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
            GS2.Log($"ValueChange {value}");
            if (Items.Count < 1 || Items.Count >= value)
            {
                GS2.Warn($"Index out of bounds: {Label} {value}");
                return;
            }
            Value = Items[value];
            OnChange?.Invoke(Items[value]);
        }
        public void initialize(GSUI options)
        {
            GS2.Log("Initializing");
            Items = options.Data as List<string>;
            var ap = _dropdown.GetComponent<RectTransform>().anchoredPosition;
            //Object.DestroyImmediate(_dropdown.gameObject);
            //var axz = new GameObject();
            //axz.transform.SetParent(transform);
            //axz.GetComponent<RectTransform>().anchoredPosition = ap;

            var ct = Instantiate(SettingsUI.comboTemplate, _dropdown.transform, false);
            //ct.anchoredPosition = ap;
            var uicb = ct.GetComponent<UIComboBox>();
            uicb.Items = options.Data as List<string>;
            uicb.itemIndex = options.DefaultValue;
            //_dropdown.AddOptions(options.Data as List<string>);
            //GS2.LogJson(_dropdown.options);
            //GS2.LogJson(options.Data);
            //Value = options.DefaultValue;
            //Label = options.Label;
            //OnChange = options.callback;
            //options.Postfix?.Invoke();
            
        }
        public void Fix(BaseEventData e)
        {
            GS2.Warn("Fix");
            //var c = _dropdown.transform.Find("Dropdown List").GetComponent<Canvas>();
            //c.overrideSorting = false;
            //c.renderMode = RenderMode.ScreenSpaceOverlay; 
            //if (base.transform.parent != null)
            //{
            //    base.transform.parent.SetAsLastSibling();
            //}
            //else
            //{
            //    base.transform.SetAsLastSibling();
            //}
            //float num = (float)Mathf.Clamp(this._dropdown.value, 0, Mathf.Max(0, this.Items.Count - this._dropdown.options.Count));
            //m_DropDownContent.anchoredPosition = new Vector2(m_DropDownContent.anchoredPosition.x, num * 40f);
        }
        
    }

}