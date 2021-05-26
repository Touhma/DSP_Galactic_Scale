using System.Collections.Generic;

namespace GalacticScale {
    public delegate void GSOptionCallback(object o);
    public delegate void GSOptionPostfix();
    
    public class GSUI
    {
        public string label;
        public string type;
        public object data;
        public string tip; // Not fully implemented in all ui items yet
        public UnityEngine.RectTransform rectTransform;
        public GSOptionCallback callback;
        public GSOptionPostfix postfix;

        public GSUI(string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            this.label = label;
            this.type = type;
            this.data = data;
            this.callback = callback;
            if (postfix == null) this.postfix = delegate { };
            else this.postfix = postfix;
            this.tip = tip;
        }
        public bool Set(object o)
        {
            if (rectTransform == null) return false;
            switch (type)
            {
                case "Slider": rectTransform.GetComponentInChildren<UnityEngine.UI.Slider>().value = (float)o; return true;
                case "Input": rectTransform.GetComponentInChildren<UnityEngine.UI.InputField>().text = (string)o; return true;
                case "Checkbox": rectTransform.GetComponentInChildren<UnityEngine.UI.Toggle>().isOn = (bool)o; return true;
                case "Combobox": rectTransform.GetComponentInChildren<UIComboBox>().itemIndex = (int)o; return true;
            }
            return false;
        }
        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback, bool enableFloat = false, GSOptionPostfix postfix = null)
        {
            return new GSUI(label, "Slider", new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat }, callback, postfix);
        }
        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Checkbox", value, callback, postfix, tip);
        }
        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Checkbox", items, callback, postfix, tip);
        }
        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Input", value, callback, postfix, tip);
        }
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Button", caption, callback, postfix, tip);
        }
    }

}