using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public delegate void GSOptionCallback(Val o);

    public delegate void GSOptionPostfix();

    public partial class GSUI
    {
        private static readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();

        public static List<string> Settables = new List<string>
        {
            "Slider",
            "RangeSlider",
            "Checkbox",
            "Combobox",
            "Inputfield"
        };

        private readonly iConfigurableGenerator generator;
        private readonly string key;
        private readonly iConfigurablePlugin plugin;
        public GSOptionCallback callback;
        private int comboDefault = -1;
        public float increment = 1f;
        public RectTransform RectTransform;

        private GSUI()
        {
        }

        public GSUI(string label, string key, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string tip = "")
        {
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface)
                    generator = Utils.GetConfigurableGeneratorInstance(tt);
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface)
                    plugin = Utils.GetConfigurablePluginInstance(tt);
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Hint = tip;
            //GS2.Warn("Created GSUI " + label);
        }

        public GSUI(iConfigurableGenerator generator, string key, string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
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
            Hint = hint;
        }

        public GSUI(iConfigurablePlugin plugin, string key, string label, string type, object data, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            this.plugin = plugin;
            this.key = key;
            Label = label;
            Type = type;
            Data = data;
            this.callback = callback;
            if (postfix == null)
                this.postfix = delegate { };
            else
                this.postfix = postfix;
            Hint = hint;
        }

        public string Label { get; }
        public string Hint { get; private set; }
        public string Type { get; }
        public object Data { get; private set; }

        private iConfigurableGenerator Generator
        {
            get
            {
                if (generator != null) return generator;
                GS2.Error($"GSUI ${Label} Tried accessing Generator instance when Generator = null." + GS2.GetCaller() + GS2.GetCaller(1));
                return null;
            }
        }

        private iConfigurablePlugin Plugin
        {
            get
            {
                if (plugin != null) return plugin;
                GS2.Error($"GSUI ${Label} Tried accessing Plugin instance when Generator = null." + GS2.GetCaller() + GS2.GetCaller(1));
                return null;
            }
        }

        private Slider slider => RectTransform.GetComponentInChildren<Slider>();


        public Val DefaultValue
        {
            get
            {
                switch (Type)
                {
                    case "Group":
                        var gcfg = Data is GSUIGroupConfig ? (GSUIGroupConfig)Data : new GSUIGroupConfig(null, true, true);
                        return gcfg.defaultValue;
                    case "Slider":
                        var cfg = Data is GSSliderConfig ? (GSSliderConfig)Data : new GSSliderConfig(-1, -1, -1);
                        return cfg.defaultValue;
                    case "RangeSlider":
                        var rcfg = Data is GSRangeSliderConfig ? (GSRangeSliderConfig)Data : new GSRangeSliderConfig(-1, -1, -1, -1);
                        return rcfg.defaultValue;
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
                        // GS2.Warn($"Combo {Label} {Data}");
                        // var cbresult = GetInt(Data);
                        // if (cbresult.succeeded) return cbresult.value;
                        if (comboDefault >= 0) return comboDefault;
                        GS2.Warn($"No default value found for Combobox {Label}");
                        return false;
                    case "Selector":
                        var list = Data as List<string>;
                        if (comboDefault >= 0)
                            if (list.Count > comboDefault)
                                return list[comboDefault];
                        GS2.Warn($"No default value found for Combobox {Label}");
                        return false;
                }

                GS2.Error($"Failed to return default value for {Type} {Label}");
                return null;
            }
        }

        public bool Disabled { get; private set; }

        public GSOptionPostfix postfix { get; private set; }

        private (bool succeeded, float value) GetFloat(object o)
        {
            //GS2.Warn(label);
            if (o is float) return (true, (float)o);
            //float result;
            var success = float.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, int value) GetInt(object o)
        {
            // GS2.Warn(Label);
            if (o is int) return (true, (int)o);
            var success = int.TryParse(o.ToString(), out var result);
            return (success, result);
        }

        private (bool succeeded, bool value) GetBool(object o)
        {
            if (o is bool) return (true, (bool)o);
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
                case "List":
                    RectTransform.GetComponent<GSUIList>().interactable = false;
                    Disabled = true;
                    return true;
                case "Checkbox":
                    GS2.Warn("Disabling Checkbox");

                    RectTransform.GetComponent<Toggle>().interactable = false;
                    //GS2.Warn("!");
                    Disabled = true;
                    //GS2.Warn("!!");
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
                case "RangeSlider":
                    GS2.Warn("Disabling RangeSlider");
                    RectTransform.GetComponentInChildren<GSUIRangeSlider>().interactable = false;
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
                case "List":
                    RectTransform.GetComponent<GSUIList>().interactable = true;
                    Disabled = false;
                    return true;
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
                case "RangeSlider":
                    RectTransform.GetComponentInChildren<GSUIRangeSlider>().interactable = true;
                    Disabled = false;
                    return true;
            }

            return false;
        }

        public void SetHint(string hint)
        {
            Hint = hint;
        }


        private static GSOptionCallback CreateIncrementCallback(float increment, GSUI instance, GSOptionCallback existingCallback)
        {
            return o =>
            {
                // GS2.Warn("*");

                if (o.ToString().Split(':').Length > 1)
                {
                    FloatPair val = o;
                    val.low = val.low - val.low % increment;
                    val.high = val.high - val.high % increment;
                    if (val.high > ((GSRangeSliderConfig)instance.Data).maxValue) val.high = ((GSRangeSliderConfig)instance.Data).maxValue;
                    instance.Set(val);
                    existingCallback(val);
                }
                else
                {
                    var value = 0.1f;
                    if (!float.TryParse(o.ToString(), out value))
                    {
                        GS2.Error($"Failed to parse increment {o} for slider {instance.Label}");
                    }
                    else
                    {
                        var cfg = (GSSliderConfig)instance.Data;
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
                            if (value - value % increment != instance.slider.value)
                                instance.Set(value - value % increment);
                        }
                    }
                }
            };
        }

        private static GSOptionCallback CreatePlanetSizeCallback(GSUI instance, GSOptionCallback existingCallback)
        {
            return o =>
            {
                // GS2.Warn("*");

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

        private static GSOptionCallback CreatePlanetSizeRangeCallback(GSUI instance, GSOptionCallback existingCallback)
        {
            return o =>
            {
                // GS2.Warn("*");
                var value = o.FloatFloat();
                float parsedLow = Utils.ParsePlanetSize(value.low);
                float parsedHigh = Utils.ParsePlanetSize(value.high);
                instance.Set(new FloatPair(parsedLow, parsedHigh));
                existingCallback(new FloatPair(parsedLow, parsedHigh));
            };
        }

        private GSOptionCallback CreateDefaultCallback(GSOptionCallback callback = null)
        {
            // GS2.Warn("Creating default callback for "+Label);
            return o =>
            {
                if (generator is null && plugin == null)
                {
                    GS2.Error($"{Label} Trying to create Default Callback when Generator = null");
                    return;
                }

                if (generator != null)
                {
                    var p = generator.Export();

                    p.Set(key, o);
                    generator.Import(p);
                    // GS2.Warn($"Test setting {key} to {o}");
                    if (callback is GSOptionCallback) callback(o);
                }
                else
                {
                    var p = plugin.Export();

                    p.Set(key, o);
                    // GS2.Log($"Callback for {Label} exporting etc");
                    plugin.Import(p);
                    // GS2.Warn($"Test setting {key} to {o}");
                    if (callback is GSOptionCallback) callback(o);
                }
            };
        }

        private GSOptionPostfix CreateDefaultPostfix()
        {
            // GS2.Warn("Creating DefaultPostfix for {Label}");
            return () =>
            {
                // GS2.Warn($"Executing DefaultPostfix for {Label}");

                if (generator is null && plugin is null)
                {
                    GS2.Error($"{Label} Trying to create Default Postfix when Generator = null");
                    return;
                }

                if (generator != null)
                {
                    var value = Generator.Export().Get(key);
                    // GS2.Log($"{key} Value:{value} is null?:{value == null}");
                    if (value == null)
                        // GS2.Warn($"Setting value which was null for {key} to {DefaultValue}");
                        value = DefaultValue;

                    if (value != null)
                        // GS2.Warn($"Setting non null value for {key} to {value}");
                        Set(value);
                    else GS2.Log($"Caution: Preference value for {Label} not found.");
                }

                if (plugin != null)
                {
                    var value = plugin.Export().Get(key);
                    // GS2.Log($"{key} Value:{value} is null?:{value == null}");
                    if (value == null)
                        // GS2.Warn($"Setting value which was null for {key} to {DefaultValue}");
                        value = DefaultValue;

                    if (value != null)
                        // GS2.Warn($"Setting non null value for {key} to {value}");
                        Set(value);
                    else GS2.Log($"Caution: Preference value for {Label} not found.");
                }
            };
        }

        private void SetPreference(object value)
        {
            if (generator is null)
            {
                GS2.Error($"{Label} Trying to set preference '{key}' when Generator = null");
                return;
            }

            var p = Generator.Export();
            try
            {
                p.Set(key, value);
            }
            catch (Exception e)
            {
                GS2.Warn($"Error:{Label}");
                GS2.Warn(e.Message);
            }

            Generator.Import(p);
        }

        public static GSUI Separator()
        {
            return new GSUI((iConfigurableGenerator)null, null, null, "Separator", null, null, null, null);
        }
    }

    public class GSUIGroupConfig
    {
        public GSOptionCallback callback = null;
        public bool collapsible = true;
        public bool defaultValue;
        public bool header = true;
        public List<GSUI> options = new List<GSUI>();

        public GSUIGroupConfig(List<GSUI> options, bool header, bool collapsible, bool defaultValue = false)
        {
            this.options = options;
            this.header = header;
            this.collapsible = collapsible;
            this.defaultValue = defaultValue;
        }

        public GSUIGroupConfig()
        {
        }
    }

    public struct GSSliderConfig
    {
        public float minValue;
        public float maxValue;
        public float defaultValue;
        public string negativeLabel;

        public GSSliderConfig(float minValue, float value, float maxValue, string negativeLabel = "")
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            defaultValue = value;
            this.negativeLabel = negativeLabel;
        }
    }

    public struct GSRangeSliderConfig
    {
        public GSOptionCallback callbackLow;
        public GSOptionCallback callbackHigh;
        public float minValue;
        public float maxValue;
        public float defaultLowValue;
        public float defaultHighValue;
        public FloatPair defaultValue => new FloatPair(defaultLowValue, defaultHighValue);

        public GSRangeSliderConfig(float minValue, float lowValue, float highValue, float maxValue, GSOptionCallback callbackLow = null, GSOptionCallback callbackHigh = null)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            defaultLowValue = lowValue;
            defaultHighValue = highValue;
            this.callbackHigh = callbackHigh;
            this.callbackLow = callbackLow;
        }
    }
}