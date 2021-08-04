using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public delegate void GSOptionCallback(Val o);

    public delegate void GSOptionPostfix();

    public class GSUI
    {
        private static readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();
        public GSOptionCallback callback;
        private readonly iConfigurableGenerator generator;
        public float increment = 1f;
        private readonly string key;
        public RectTransform RectTransform;

        private GSUI()
        {
        }

        public GSUI(string label, string key, string type, object data, GSOptionCallback callback,
            GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            generator = Utils.GetConfigurableGeneratorInstance(t);
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Tip = tip;
            //GS2.Warn("Created GSUI " + label);
        }

        public GSUI(iConfigurableGenerator generator, string key, string label, string type, object data,
            GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            this.generator = generator;
            this.key = key;
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Tip = tip;
        }

        private iConfigurableGenerator Generator
        {
            get
            {
                if (generator != null) return generator;
                GS2.Error($"GSUI ${Label}Tried accessing Generator instance when Generator = null.");
                return null;
            }
        }

        private Slider slider => RectTransform.GetComponentInChildren<Slider>();
        public string Label { get; }

        public string Type { get; }

        public object Data { get; private set; }

        public Val DefaultValue
        {
            get
            {
                switch (Type)
                {
                    case "Slider":
                        var cfg = Data is GSSliderConfig ? (GSSliderConfig) Data : new GSSliderConfig(-1, -1, -1);
                        return cfg.defaultValue;
                    case "Checkbox":
                        var bresult = GetBool(Data);
                        if (bresult.succeeded) return bresult.value;
                        GS2.Warn($"No default value found for Checkbox {Label}");
                        return false;
                    case "Button":
                        GS2.Error("Trying to get default value for button {label}");
                        return null;
                    case "Input":
                        return Data.ToString();
                    case "Combobox":
                        var cbresult = GetInt(Data);
                        if (cbresult.succeeded) return cbresult.value;
                        GS2.Warn($"No default value found for Combobox {Label}");
                        return false;
                }

                GS2.Error($"Failed to return default value for {Type} {Label}");
                return null;
            }
        }

        public bool Disabled { get; private set; }

        public string Tip { get; }

        public GSOptionPostfix postfix { get; private set; }

        private (bool succeeded, float value) GetFloat(object o)
        {
            //GS2.Warn(label);
            if (o is float) return (true, (float) o);
            //float result;
            var success = float.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, int value) GetInt(object o)
        {
            // GS2.Warn(Label);
            if (o is int) return (true, (int) o);
            var success = int.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, bool value) GetBool(object o)
        {
            if (o is bool) return (true, (bool) o);
            var success = bool.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        public void Reset()
        {
            Set(DefaultValue);
        }

        public bool Disable()
        {
            //GS2.Warn("Disabling element" + label);
            if (Disabled)
            {
                GS2.Warn("Trying to disable UI Element that is already disabled");
                return false;
            }

            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (var i in c)
            {
                var id = i.GetInstanceID();
                if (colors.ContainsKey(id)) colors[id] = i.color;
                else colors.Add(id, i.color);
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a / 2);
            }
            GS2.Warn("Set Image Done");
            switch (Type)
            {
                case "Checkbox":
                    GS2.Warn("Disabling Checkbox");

                    RectTransform.GetComponent<Toggle>().interactable = false;
                    Disabled = true;
                    return true;
                case "Combobox":
                    GS2.Warn("Disabling Combobox");
                    RectTransform.GetComponentInChildren<Dropdown>().interactable = false;
                    Disabled = true;
                    return true;
                case "Button":
                    GS2.Warn("Disabling Button");
                    RectTransform.GetComponentInChildren<Button>().interactable = false;
                    Disabled = true;
                    return true;
                case "Input":
                    GS2.Warn("Disabling Input");
                    RectTransform.GetComponentInChildren<InputField>().interactable = false;
                    Disabled = true;
                    return true;
                case "Slider":
                    GS2.Warn("Disabling Slider");
                    RectTransform.GetComponentInChildren<Slider>().interactable = false;
                    Disabled = true;
                    return true;
            }

            return false;
        }

        public bool Enable()
        {
            //GS2.Warn("Enabling element" + label);
            if (!Disabled)
            {
                GS2.Warn("Trying to disable UI Element that is already enabled");
                return false;
            }

            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (var i in c)
            {
                var id = i.GetInstanceID();
                i.color = colors[id];
            }

            switch (Type)
            {
                case "Checkbox":
                    RectTransform.GetComponent<Toggle>().interactable = true;
                    Disabled = false;
                    return true;
                case "Combobox":
                    RectTransform.GetComponentInChildren<Dropdown>().interactable = true;
                    Disabled = false;
                    return true;
                case "Button":
                    RectTransform.GetComponentInChildren<Button>().interactable = true;
                    Disabled = false;
                    return true;
                case "Input":
                    RectTransform.GetComponentInChildren<InputField>().interactable = true;
                    Disabled = false;
                    return true;
                case "Slider":
                    RectTransform.GetComponentInChildren<Slider>().interactable = true;
                    Disabled = false;
                    return true;
            }

            return false;
        }

        public bool Set(GSSliderConfig cfg)
        {
            //GS2.Warn("Setting Slider? : " + label);
            if (RectTransform == null) return false;
            var slider = RectTransform.GetComponentInChildren<Slider>();
            if (slider == null) return false;
            // GS2.Warn($"{Label} Slider Setting...({slider.value}) {DefaultValue} -> {cfg.defaultValue} {cfg.minValue}:{cfg.maxValue}");
            slider.minValue = cfg.minValue >= 0 ? cfg.minValue : slider.minValue;
            slider.maxValue = cfg.maxValue >= 0 ? cfg.maxValue : slider.maxValue;
            slider.value = cfg.defaultValue >= 0 ? cfg.defaultValue : slider.value;
            //GS2.Warn("Slider Set.");
            return true;
        }

        public bool Set(Val o)
        {
            GS2.Log($"Set called by {GS2.GetCaller()} to set {o} for {Label}");

            if (RectTransform == null)
             {   GS2.Warn($"RectTransform for {Label} null");
            
                return false;
            }
            switch (Type)
            {
                case "Slider":
                    //GS2.Warn("Setting Slider " + label);
                    //GS2.Warn(o.ToString());
                    //var sliderResult = GetFloat(o);
                    //if (sliderResult.succeeded)
                    //{
                    //    //GS2.Warn($"Parsed as float:{sliderResult.value}");
                    //}
                    //else
                    //{
                    //    GS2.Error($"Failed to parse slider Set method input of {o} for slider '{label}'");
                    //    return false;
                    //}
                    RectTransform.GetComponentInChildren<Slider>().value = o; //sliderResult.value;
                    return true;
                case "Input":
                    if (o == null)
                    {
                        GS2.Error($"Failed to set input {Label} as value was null");
                        return false;
                    }

                    RectTransform.GetComponentInChildren<InputField>().text = o;
                    return true;
                case "Checkbox":
                    //var checkboxResult = GetBool(o);
                    //if (!checkboxResult.succeeded)
                    //{
                    //    GS2.Error($"Failed to parse checkbox Set method input of {o} for checkbox '{label}'");
                    //    return false;
                    //}
                    var toggle = RectTransform.GetComponent<GSUIToggle>();
                    if (toggle is null)
                    {
                        GS2.Error($"Failed to find Toggle for {Label}");
                        return false;
                    }
                    GS2.Log($"Found toggle for {Label} setting isOn:{o}");
                    toggle.Value = o; // checkboxResult.value;
                    return true;
                case "Combobox":
                    //var comboResult = GetInt(o);
                    //if (!comboResult.succeeded)
                    //{
                    //    GS2.Error($"Failed to parse combobox Set method input of {o} for combobox '{label}'");
                    //    return false;
                    //} //else GS2.Log($"Parsed as int {comboboxValue}");
                    //if (comboResult.value < 0)
                    //{
                    //    GS2.Error($"Failed to set {comboResult.value} for combobox '{label}': Value < 0");
                    //    return false;
                    //}
                    var cb = RectTransform.GetComponentInChildren<Dropdown>();
                    if (cb is null)
                    {
                        GS2.Error($"Failed to find UICombobox for {Label}");
                        return false;
                    }

                    if (o > cb.options.Count - 1)
                    {
                        GS2.Error($"Failed to set {o} for combobox '{Label}': Value > Item Count");
                        return false;
                    }

                    cb.value = o;
                    return true;
            }

            return false;
        }

        public bool SetItems(List<string> items)
        {
            if (Type != "Combobox")
            {
                GS2.Warn("Trying to Set Items on non Combobox UI Element");
                return false;
            }

            Data = items;
            if (RectTransform != null) RectTransform.GetComponentInChildren<UIComboBox>().Items = items;
            return true;
        }

        // Slider (Integer, no key)
        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback,
            GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, null, postfix, tip);
            instance.callback = callback;
            instance.postfix = postfix;
            return instance;
        }

        // Slider for Planet Sizes with key
        public static GSUI PlanetSizeSlider(string label, float min, float val, float max, string key,
            GSOptionCallback callback = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, null, null, tip);
            var defaultCallback = instance.CreateDefaultCallback(callback);
            instance.callback = CreatePlanetSizeCallback(instance, defaultCallback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        // Slider for Planet Sizes without key
        public static GSUI PlanetSizeSlider(string label, float min, float val, float max,
            GSOptionCallback callback = null, GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, callback, postfix, tip);
            instance.callback = CreatePlanetSizeCallback(instance, callback);
            instance.postfix = postfix;
            return instance;
        }

        //Slider with preferences Key
        public static GSUI Slider(string label, float min, float val, float max, string key,
            GSOptionCallback callback = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            // GS2.Warn($"{label} Slider Creation...({val})");
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, null, null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Slider with increment and preferences Key
        public static GSUI Slider(string label, float min, float val, float max, float increment, string key,
            GSOptionCallback callback = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, null, null, tip);
            var defaultCallback = instance.CreateDefaultCallback(callback);
            var CB = defaultCallback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, defaultCallback);
            //if (planetSizes) CB = CreatePlanetSizeCallback(instance, defaultCallback);
            instance.callback = CB;
            instance.postfix = instance.CreateDefaultPostfix();
            instance.increment = increment;
            return instance;
        }

        //Slider with increment and no Key
        public static GSUI Slider(string label, float min, float val, float max, float increment,
            GSOptionCallback callback = null, GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider",
                new GSSliderConfig {minValue = min, maxValue = max, defaultValue = val}, null, postfix, tip);
            var CB = callback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, callback);
            instance.callback = CB;
            instance.increment = increment;
            return instance;
        }

        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix = null,
            string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Checkbox", value, callback,
                postfix, tip);
            return instance;
        }

        public static GSUI Checkbox(string label, bool defaultValue, string key, GSOptionCallback callback = null,
            string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Checkbox", defaultValue,
                null, null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback,
            GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Combobox", items, callback,
                postfix, tip);
            return instance;
        }

        public static GSUI Combobox(string label, List<string> items, int defaultValue, string key,
            GSOptionCallback callback = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Combobox", items, null,
                null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix = null,
            string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Input", value, callback,
                postfix, tip);
            return instance;
        }

        public static GSUI Input(string label, string defaultValue, string key, GSOptionCallback callback = null,
            string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), key, label, "Input", defaultValue, null,
                null, tip);
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        public static GSUI Button(string label, string caption, GSOptionCallback callback,
            GSOptionPostfix postfix = null, string tip = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Button", caption, callback,
                null, tip);
            return instance;
        }

        private static GSOptionCallback CreateIncrementCallback(float increment, GSUI instance,
            GSOptionCallback existingCallback)
        {
            return o =>
            {
                var value = 0.1f;
                if (!float.TryParse(o.ToString(), out value))
                {
                    GS2.Error($"Failed to parse increment {o} for slider {instance.Label}");
                }
                else
                {
                    var cfg = (GSSliderConfig) instance.Data;
                    if (value >= cfg.maxValue - increment / 2)
                    {
                        //GS2.Warn($"Max hit on {label}");

                        var iMax = cfg.maxValue;
                        instance.Set(iMax);
                        existingCallback(iMax);
                    }
                    else
                    {
                        //GS2.Warn($"Executing increment test of {increment} on {label}");
                        existingCallback(value - value % increment);
                        if (value - value % increment != instance.slider.value) instance.Set(value - value % increment);
                    }
                }
            };
        }

        private static GSOptionCallback CreatePlanetSizeCallback(GSUI instance, GSOptionCallback existingCallback)
        {
            return o =>
            {
                var value = 200;
                if (!int.TryParse(o.ToString(), out value))
                {
                    GS2.Error($"Failed to parse planet size {o} for slider {instance.Label}");
                }
                else
                {
                    float parsedSize = Utils.ParsePlanetSize(value);
                    if (parsedSize != instance.slider.value) instance.Set(parsedSize);
                    existingCallback(parsedSize);
                }
            };
        }

        private GSOptionCallback CreateDefaultCallback(GSOptionCallback callback = null)
        {

            return o =>
            {
                if (Generator is null)
                {
                    GS2.Error($"{Label} Trying to create Default Callback when Generator = null");
                    return;
                }

                var p = Generator.Export();
                p.Set(key, o);
                Generator.Import(p);
                //if (Label == "Min Planets/System") GS2.Error(Label);
                if (callback is GSOptionCallback) callback(o);
            };
        }

        private GSOptionPostfix CreateDefaultPostfix()
        {
            GS2.Warn("Creating DefaultPostfix for {Label}");
            return () =>
            {
                GS2.Warn($"Executing DefaultPostfix for {Label}");

                if (Generator is null)
                {
                    GS2.Error($"{Label} Trying to create Default Postfix when Generator = null");
                    return;
                }


                var value = Generator.Export().Get(key);
                GS2.Log($"{key} Value:{value} is null?:{value == null}");
                if (value == null)
                {
                    GS2.Warn($"Setting value which was null for {key} to {DefaultValue}");
                    value = DefaultValue;
                }
                if (value != null)
                {
                    GS2.Warn($"Setting non null value for {key} to {value}");
                    Set(value);
                }
                else GS2.Log($"Caution: Preference value for {Label} not found.");
            };
        }

        private void SetPreference(object value)
        {
            if (Generator is null)
            {
                GS2.Error($"{Label} Trying to set preference '{key}' when Generator = null");
                return;
            }

            var p = Generator.Export();
            p.Set(key, value);
            Generator.Import(p);
        }
    }
}