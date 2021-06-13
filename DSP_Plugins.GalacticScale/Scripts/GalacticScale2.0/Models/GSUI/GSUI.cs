﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale {
    public delegate void GSOptionCallback(object o);
    public delegate void GSOptionPostfix();

    public class GSUI {
        //public static Dictionary<string, GSUI> LoadedUIElements = new Dictionary<string, GSUI>();
        private string label;
        private string key;
        private iConfigurableGenerator generator;
        private iConfigurableGenerator Generator {
            get {
                if (generator != null) return generator;
                GS2.Error($"GSUI ${label}Tried accessing Generator instance when Generator = null.");
                return null;
            }
        }
        public string Label { get => label; }
        private string type;
        public string Type { get => type; }
        private object data;
        public object Data { get => data; }
        public object DefaultValue {
            get {
                switch (Type) {
                    case "Slider":
                    GSSliderConfig cfg = (Data is GSSliderConfig)?(GSSliderConfig)data:new GSSliderConfig(-1,-1,-1);
                    return cfg.defaultValue;
                    case "Checkbox":
                        var bresult = GetBool(Data);
                        if (bresult.succeeded) return bresult.value;
                        GS2.Warn($"No default value found for Checkbox {label}");
                        return false;
                    case "Button": 
                        GS2.Error("Trying to get default value for button {label}");
                        return null;
                    case "Input":
                        return data.ToString();
                    case "Combobox":
                        var cbresult = GetInt(Data);
                        if (cbresult.succeeded) return cbresult.value;
                        GS2.Warn($"No default value found for Combobox {label}");
                        return false;
                }
                GS2.Error($"Failed to return default value for {Type} {label}");
                return null;
            }
        }
        private bool disabled = false;
        public bool Disabled { get => disabled; }
        private string tip; // Not fully implemented in all ui items yet
        public string Tip { get => tip; }
        public RectTransform RectTransform;
        public GSOptionCallback callback;
        private GSOptionPostfix postfix;
        public GSOptionPostfix Postfix { get => postfix; }
        private static Dictionary<int, Color> colors = new Dictionary<int, Color>();
        public GSUI(string label, string key, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType();
            this.generator = Utils.GetConfigurableGeneratorInstance(t);
            this.label = label;
            this.type = type;
            this.data = data;
            this.callback = callback;
            if (postfix == null) {
                this.postfix = delegate { };
            } else {
                this.postfix = postfix;
            }
            this.tip = tip;
        }
        public GSUI(iConfigurableGenerator generator, string key, string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            this.generator = generator;
            this.key = key;
            this.label = label;
            this.type = type;
            this.data = data;
            this.callback = callback;
            if (postfix == null) {
                this.postfix = delegate { };
            } else {
                this.postfix = postfix;
            }
            this.tip = tip;
        }
        (bool succeeded,float value) GetFloat(object o) {
            if (o is float) return (true, (float)o);
            //float result;
            bool success = float.TryParse(o.ToString(), out float result);
            return (success, result);
        }
        (bool succeeded, int value) GetInt(object o) {
            if (o is int) return (true, (int)o);
            bool success = int.TryParse(o.ToString(), out int result);
            return (success,result);
        }
        (bool succeeded, bool value) GetBool(object o) {
            if (o is bool) return (true, (bool)o);
            bool success = bool.TryParse(o.ToString(), out bool result);
            return (success, result);
        }
        public void Reset() => Set(DefaultValue);
        public bool Disable() {
            GS2.Warn("Disabling element" + label);
            if (disabled) {
                GS2.Warn("Trying to disable UI Element that is already disabled");
                return false;
            }
            
            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (Image i in c) {
                int id = i.GetInstanceID();
                if (colors.ContainsKey(id)) colors[id] = i.color;
                else colors.Add(id, i.color);
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a/2);
            }
            switch (type) {
                case "Checkbox": RectTransform.GetComponentInChildren<Toggle>().interactable = false;       disabled = true; return true;
                case "Combobox": RectTransform.GetComponentInChildren<Button>().interactable = false;       disabled = true; return true;
                case "Button":   RectTransform.GetComponentInChildren<Button>().interactable =  false;      disabled = true; return true;
                case "Input":    RectTransform.GetComponentInChildren<InputField>().interactable = false;   disabled = true; return true;
                case "Slider":   RectTransform.GetComponentInChildren<Slider>().interactable = false;       disabled = true; return true;
            }
            return false;
        }
        public bool Enable() {
            GS2.Warn("Enabling element" + label);
            if (!disabled) {
                GS2.Warn("Trying to disable UI Element that is already enabled");
                return false;
            }
            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (Image i in c) {
                int id  = i.GetInstanceID();
                i.color = colors[id];
            }
            switch (type) {
                case "Checkbox": RectTransform.GetComponentInChildren<Toggle>().interactable = true;        disabled = false; return true;
                case "Combobox": RectTransform.GetComponentInChildren<Button>().interactable = true;        disabled = false; return true;
                case "Button":   RectTransform.GetComponentInChildren<Button>().interactable = true;        disabled = false; return true;
                case "Input":    RectTransform.GetComponentInChildren<InputField>().interactable = true;    disabled = false; return true;
                case "Slider":   RectTransform.GetComponentInChildren<Slider>().interactable = true;        disabled = false; return true;
            }
            return false;
        }
        public bool Set(GSSliderConfig cfg) {
            GS2.Warn("Setting Slider? : "+label);
            if (RectTransform == null) {
                return false;
            }
            Slider slider = RectTransform.GetComponentInChildren<Slider>();
            if (slider == null) return false;
            GS2.Warn("Slider Setting...");
            slider.minValue = cfg.minValue     >= 0 ? cfg.minValue     : slider.minValue;
            slider.maxValue = cfg.maxValue     >= 0 ? cfg.maxValue     : slider.maxValue;
            slider.value    = cfg.defaultValue >= 0 ? cfg.defaultValue : slider.value;
            GS2.Warn("Slider Set."); 
            return true;
        }
        public bool Set(object o) {

            if (RectTransform == null) {
                return false;
            }
            switch (type) {
                case "Slider":
                    GS2.Warn("Setting Slider " + label);
                    GS2.Warn(o.ToString());
                    var sliderResult = GetFloat(o);
                    if (sliderResult.succeeded) {
                        GS2.Warn($"Parsed as float:{sliderResult.value}");
                    } else {
                        GS2.Error($"Failed to parse slider Set method input of {o} for slider '{label}'");
                        return false;
                    }
                    RectTransform.GetComponentInChildren<Slider>().value = sliderResult.value;
                    return true;
                case "Input":
                    if (o == null) {
                        GS2.Error($"Failed to set input {label} as value was null");
                        return false;
                    }
                    RectTransform.GetComponentInChildren<InputField>().text = (string)o; 
                    return true;
                case "Checkbox":
                    bool checkboxValue;
                    if ((o is string || o is bool) && bool.TryParse(o.ToString(), out checkboxValue)) {
                        GS2.Log($"Parsed as bool {checkboxValue}");
                    } else {
                        GS2.Error($"Failed to parse checkbox Set method input of {o} for checkbox '{label}'");
                        return false;
                    }
                    RectTransform.GetComponentInChildren<Toggle>().isOn = checkboxValue;
                    return true;
                case "Combobox":
                    int comboboxValue;
                    if ((o is int || o is float || o is double || o is string) && int.TryParse(o as string, out comboboxValue))
                        GS2.Log($"Parsed as int {comboboxValue}");
                    else {
                        GS2.Error($"Failed to parse combobox Set method input of {o} for combobox '{label}'");
                        return false;
                    }
                    if ((int)o < 0) {
                        GS2.Error($"Failed to set {o} for combobox '{label}': Value < 0");
                        return false;
                    }
                    UIComboBox cb = RectTransform.GetComponentInChildren<UIComboBox>();
                    if ((int)o > cb.Items.Count - 1) {
                        GS2.Error($"Failed to set {o} for combobox '{label}': Value > Item Count");
                        return false;
                    }
                    cb.itemIndex = (int)o; 
                    return true;
            }
            return false;
        }
        public bool SetItems(List<string> items) {
            if (type != "Combobox") {
                GS2.Warn("Trying to Set Items on non Combobox UI Element");
                return false;
            }
            RectTransform.GetComponentInChildren<UIComboBox>().Items = items;
            return true;
        }

        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback, bool enableFloat = false, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider", new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat }, callback, postfix, tip);
            return instance;
        }
        public static GSUI Slider(string label, float min, float val, float max, string key, GSOptionCallback callback = null, bool enableFloat = false, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t),key,label,"Slider", new GSSliderConfig() { minValue = min, maxValue = max, defaultValue = val, wholeNumbers = !enableFloat },null,null,tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }
        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType(); 
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Checkbox", value, callback, postfix, tip);
            return instance;
        }
        public static GSUI Checkbox(string label, bool defaultValue, string key, GSOptionCallback callback = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Checkbox", defaultValue, null,null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Combobox", items, callback, postfix, tip);
            return instance;
        }
        public static GSUI Combobox(string label, List<string> items, int defaultValue, string key, GSOptionCallback callback = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Combobox", items, null,null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix  = instance.CreateDefaultPostfix();
            return instance;
        }
        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Input", value, callback, postfix, tip);
            return instance;
        }
        public static GSUI Input(string label, string defaultValue, string key, GSOptionCallback callback = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Input", defaultValue, null,null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix  = instance.CreateDefaultPostfix();
            return instance;
        }
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "") {
            Type t = Utils.GetCallingType();
            GSUI instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Button", caption, callback, postfix, tip);
            return instance; 
        }

        private GSOptionCallback CreateDefaultCallback(GSOptionCallback callback = null) {
            return (o) => {
                if (Generator is null) {
                    GS2.Error($"{label} Trying to create Default Callback when Generator = null");
                    return;
                }
                GSGenPreferences p = Generator.Export();
                p.Set(key, o);
                Generator.Import(p);
                if (callback is GSOptionCallback) {
                    callback(o);
                }
            };
        }
        private GSOptionPostfix CreateDefaultPostfix() {
            return () => {
                if (Generator is null) {
                    GS2.Error($"{label} Trying to create Default Postfix when Generator = null");
                    return;
                }
                object value = Generator.Export().Get(key);
                if (value == null) {
                    value = DefaultValue;
                }
                Set(value);
            };
        }
        private void SetPreference(object value) {
            if (Generator is null) { 
                GS2.Error($"{label} Trying to set preference '{key}' when Generator = null"); 
                return; 
            }
            GSGenPreferences p = Generator.Export();
            p.Set(key, value);
            Generator.Import(p);
        }
        
    }

}