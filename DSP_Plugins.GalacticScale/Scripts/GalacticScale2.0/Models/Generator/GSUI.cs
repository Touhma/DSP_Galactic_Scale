using System;
using System.Collections.Generic;

namespace GalacticScale {
    public delegate void GSOptionCallback(object o);
    public delegate void GSOptionPostfix();
    
    public class GSUI
    {
        public static Dictionary<string, GSUI> LoadedUIElements = new Dictionary<string, GSUI>();
        public string label;
        public string type;
        public object data;
        public string tip; // Not fully implemented in all ui items yet
        public UnityEngine.RectTransform rectTransform;
        public GSOptionCallback callback;
        public GSOptionPostfix postfix;
        public string preferencesKey;
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
            //GS2.Warn($"Trying to Set({o}) with type {o.GetType()}");
            switch (type)
            {
                case "Slider":
                    if (o is string) if (float.TryParse(o as string, out float parsedResult)) o = parsedResult;
                        else GS2.Error($"Failed to parse slider Set method input of {o} for slider '{label}'");
                    rectTransform.GetComponentInChildren<UnityEngine.UI.Slider>().value = (float)o; 
                    return true;
                case "Input": rectTransform.GetComponentInChildren<UnityEngine.UI.InputField>().text = (string)o; return true;
                case "Checkbox":
                    if (o is string) if (bool.TryParse(o as string, out bool parsedResult)) o = parsedResult;
                        else GS2.Error($"Failed to parse checkbox Set method input of {o} for checkbox '{label}'");
                    if (!(o is bool)) return false;
                    rectTransform.GetComponentInChildren<UnityEngine.UI.Toggle>().isOn = (bool)o; return true;
                case "Combobox":
                    if (o is string) if (int.TryParse(o as string, out int parsedResult)) o = parsedResult;
                        else GS2.Error($"Failed to parse combobox Set method input of {o} for combobox '{label}'");
                    rectTransform.GetComponentInChildren<UIComboBox>().itemIndex = (int)o; return true;
            }
            return false;
        }
        public bool SetItems(List<string> items)
        {
            if (type != "Combobox")
            {
                GS2.Warn("Trying to Set Items on non Combobox UI Element");
                return false;
            }
            rectTransform.GetComponentInChildren<UIComboBox>().Items = items;
            return true;
        }
        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback, bool enableFloat = false, GSOptionPostfix postfix = null)
        {
            return new GSUI(label, "Slider", new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat }, callback, postfix);
        }
        public static GSUI Slider(string label, float min, float val, float max, string key, GSOptionCallback callback = null, bool enableFloat = false, string tip = "")
        {
            Type t = Utils.GetCallingType();
            string id = $"{Utils.GetConfigurableGeneratorInstance(t).GUID}|{key}";
            if (LoadedUIElements.ContainsKey(id)) { GS2.Warn("Error adding GSUI element Slider with key {id}. Element already exists."); return LoadedUIElements[id]; }
            LoadedUIElements.Add(id, new GSUI(
                label, 
                "Slider", 
                new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat }, 
                CreateDefaultCallback(id, callback), 
                CreateDefaultPostfix(id, val), 
                tip)
                );
            return LoadedUIElements[id];
        }
        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Checkbox", value, callback, postfix, tip);
        }
        public static GSUI Checkbox(string label, bool defaultValue, string key, GSOptionCallback callback = null, string tip = "")
        {
            Type t = Utils.GetCallingType();
            string id = $"{Utils.GetConfigurableGeneratorInstance(t).GUID}|{key}";
            if (LoadedUIElements.ContainsKey(id)) { GS2.Warn("Error adding GSUI element Checkbox with key {id}. Element already exists."); return LoadedUIElements[id]; }
            LoadedUIElements.Add(id, new GSUI(label, "Checkbox", defaultValue, CreateDefaultCallback(id, callback), CreateDefaultPostfix(id, defaultValue), tip));
            return LoadedUIElements[id];
        }

        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Combobox", items, callback, postfix, tip);
        }
        public static GSUI Combobox(string label, List<string> items, int defaultValue, string key, GSOptionCallback callback = null, string tip = "")
        {
            Type t = Utils.GetCallingType();
            string id = $"{Utils.GetConfigurableGeneratorInstance(t).GUID}|{key}";
            if (LoadedUIElements.ContainsKey(id)) { GS2.Warn("Error adding GSUI element Combobox with key {id}. Element already exists."); return LoadedUIElements[id]; }
            LoadedUIElements.Add(id, new GSUI(label, "Combobox", items, CreateDefaultCallback(id, callback), CreateDefaultPostfix(id, defaultValue), tip));
            return LoadedUIElements[id];
        }
        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Input", value, callback, postfix, tip);
        }
        public static GSUI Input(string label, string defaultValue, string key, GSOptionCallback callback = null, string tip = "")
        {
            Type t = Utils.GetCallingType();
            string id = $"{Utils.GetConfigurableGeneratorInstance(t).GUID}|{key}";
            if (LoadedUIElements.ContainsKey(id)) { GS2.Warn("Error adding GSUI element Input with key {id}. Element already exists."); return LoadedUIElements[id]; }
            LoadedUIElements.Add(id, new GSUI(label, "Input", defaultValue, CreateDefaultCallback(id, callback), CreateDefaultPostfix(id, defaultValue), tip));
            return LoadedUIElements[id];
        }
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            return new GSUI(label, "Button", caption, callback, postfix, tip);
        }

        private static GSOptionCallback CreateDefaultCallback(string id, GSOptionCallback callback = null) => (o) =>
        {
            string key = id.Split('|')[1];
            iConfigurableGenerator gen = GS2.GetGeneratorByID(id.Split('|')[0]) as iConfigurableGenerator;
            GSGenPreferences p = gen.Export();
            p.Set(key, o);
            gen.Import(p);
            if (callback is GSOptionCallback) callback(o);
        };
        private static GSOptionPostfix CreateDefaultPostfix(string id, object defaultValue) => () =>
        {
            string key = id.Split('|')[1];
            iConfigurableGenerator gen = GS2.GetGeneratorByID(id.Split('|')[0]) as iConfigurableGenerator;
            object value = gen.Export().Get(key);
            if (value == null) value = defaultValue;
            LoadedUIElements[id]?.Set(value);
        };
    }

}