using System.Collections.Generic;

namespace GalacticScale {
    public delegate void GSOptionCallback(object o);
    public delegate void GSOptionPostfix();
    
    public class GSUI
    {
        public GSOptionPostfix nothing = () => { };
        public string label;
        public string type;
        public object data;
        public GSOptionCallback callback;
        public string tip;
        public UnityEngine.RectTransform rectTransform;
        public GSOptionPostfix postfix;
        public GSUI(string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix, string tip = "")
        {
            this.label = label;
            this.type = type;
            this.data = data;
            this.callback = callback;
            if (postfix == null) this.postfix = delegate { };
            else this.postfix = postfix;
            this.tip = tip;
        }
        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback, GSOptionPostfix postfix, bool enableFloat = false)
        {
            return new GSUI(label, "Slider", new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat }, callback, postfix);
        }
        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix, string tip = "")
        {
            return new GSUI(label, "Checkbox", value, callback, postfix, tip);
        }
        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback, GSOptionPostfix postfix, string tip = "")
        {
            return new GSUI(label, "Checkbox", items, callback, postfix, tip);
        }
        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix, string tip = "")
        {
            return new GSUI(label, "Input", value, callback, postfix, tip);
        }
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix, string tip = "")
        {
            return new GSUI(label, "Button", caption, callback, postfix, tip);
        }
    }

}