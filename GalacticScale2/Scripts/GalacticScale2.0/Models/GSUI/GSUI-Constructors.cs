using System.Collections.Generic;

namespace GalacticScale
{
    public partial class GSUI
    {
        public static GSUI Header(string label, string hint = "")
        {
            return new GSUI(label, null, "Header", null, null, null, hint);
        }

        public static GSUI Spacer()
        {
            return new GSUI(null, null, "Spacer", null, null, null, null);
        }

        public static GSUI Group(string label, List<GSUI> options, string hint = "", bool header = true, bool collapsible = true, GSOptionCallback callback = null)
        {
            var data = new GSUIGroupConfig(options, header, collapsible);

            var instance = new GSUI(label, null, "Group", data, null);
            instance.callback = callback;
            return instance;
        }

        public static GSUI Selector(string label, List<string> items, string defaultValue, string key, GSOptionCallback callback = null, string hint = null)
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Selector", items, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Selector", items, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            instance.comboDefault = items.IndexOf(defaultValue);
            return instance;
        }

        // Group with Checkbox and Key
        public static GSUI Group(string label, List<GSUI> options, string key, bool defaultValue, string hint = "", bool collapsible = true, GSOptionCallback callback = null)
        {
            var data = new GSUIGroupConfig(options, true, collapsible, defaultValue);
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Group", data, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Group", data, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }


        // Slider (Integer, no key)
        public static GSUI Slider(string label, float min, float val, float max, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, postfix, hint);
            instance.callback = callback;
            instance.postfix = postfix;
            return instance;
        }

        // Slider for Planet Sizes with key
        public static GSUI PlanetSizeSlider(string label, float min, float val, float max, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, null, hint);
            if (instance == null) return null;
            var defaultCallback = instance.CreateDefaultCallback(callback);
            instance.callback = CreatePlanetSizeCallback(instance, defaultCallback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        // Slider for Planet Sizes without key
        public static GSUI PlanetSizeSlider(string label, float min, float val, float max, GSOptionCallback callback = null, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, callback, postfix, hint);
            instance.callback = CreatePlanetSizeCallback(instance, callback);
            instance.postfix = postfix;
            return instance;
        }

        //Slider with preferences Key
        public static GSUI Slider(string label, float min, float val, float max, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iConfigurableGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, null, hint);

            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Slider with increment and preferences Key
        public static GSUI Slider(string label, float min, float val, float max, float increment, string key, GSOptionCallback callback = null, string hint = "", string negativeLabel = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val, negativeLabel = negativeLabel }, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val, negativeLabel = negativeLabel }, null, null, hint);
            if (instance == null) return null;
            var defaultCallback = instance.CreateDefaultCallback(callback);
            var CB = defaultCallback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, defaultCallback);
            instance.callback = CB;
            instance.postfix = instance.CreateDefaultPostfix();
            instance.increment = increment;
            return instance;
        }


        //RangeSlider with increment and preferences Key

        public static GSUI RangeSlider(string label, float min, float lowVal, float highVal, float max, float increment, string key, GSOptionCallback callback = null, GSOptionCallback callbackLow = null, GSOptionCallback callbackHigh = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "RangeSlider", new GSRangeSliderConfig { minValue = min, maxValue = max, defaultLowValue = lowVal, defaultHighValue = highVal, callbackLow = callbackLow, callbackHigh = callbackHigh }, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "RangeSlider", new GSRangeSliderConfig { minValue = min, maxValue = max, defaultLowValue = lowVal, defaultHighValue = highVal, callbackLow = callbackLow, callbackHigh = callbackHigh }, null, null, hint);
            if (instance == null) return null;

            var defaultCallback = instance.CreateDefaultCallback(callback);
            var CB = defaultCallback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, defaultCallback);
            //if (planetSizes) CB = CreatePlanetSizeCallback(instance, defaultCallback);
            instance.callback = CB;
            instance.postfix = instance.CreateDefaultPostfix();
            instance.increment = increment;
            return instance;
        }

        // Slider for Planet Sizes with key
        public static GSUI PlanetSizeRangeSlider(string label, float min, float valLow, float valHigh, float max, string key, GSOptionCallback callback = null, GSOptionCallback callbackLow = null, GSOptionCallback callbackHigh = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "RangeSlider", new GSRangeSliderConfig { minValue = min, maxValue = max, defaultLowValue = valLow, defaultHighValue = valHigh, callbackLow = callbackLow, callbackHigh = callbackHigh }, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "RangeSlider", new GSRangeSliderConfig { minValue = min, maxValue = max, defaultLowValue = valLow, defaultHighValue = valHigh, callbackLow = callbackLow, callbackHigh = callbackHigh }, null, null, hint);
            if (instance == null) return null;

            var defaultCallback = instance.CreateDefaultCallback(callback);
            instance.callback = CreatePlanetSizeRangeCallback(instance, defaultCallback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Slider with increment and no Key
        public static GSUI Slider(string label, float min, float val, float max, float increment, GSOptionCallback callback = null, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Slider", new GSSliderConfig { minValue = min, maxValue = max, defaultValue = val }, null, postfix, hint);
            var CB = callback;
            if (increment != 1f) CB = CreateIncrementCallback(increment, instance, callback);
            instance.callback = CB;
            instance.increment = increment;
            return instance;
        }

        //Checkbox with no key
        public static GSUI Checkbox(string label, bool value, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Checkbox", value, callback, postfix, hint);
            return instance;
        }

        //Checkbox with key
        public static GSUI Checkbox(string label, bool defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Checkbox", defaultValue, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Checkbox", defaultValue, null, null, hint);
            if (instance == null) return null;

            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Combobox without key
        public static GSUI Combobox(string label, List<string> items, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Combobox", items, callback, postfix, hint);
            return instance;
        }

        //Combobox with key
        public static GSUI Combobox(string label, List<string> items, int defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Combobox", items, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Combobox", items, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            instance.comboDefault = defaultValue;
            return instance;
        }

        //Input no key
        public static GSUI Input(string label, string value, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Input", value, callback, postfix, hint);
            return instance;
        }

        //Input with Key
        public static GSUI Input(string label, string defaultValue, string key, GSOptionCallback callback = null, string hint = "")
        {
            GSUI instance = null;
            var tt = Utils.GetCallingType();
            foreach (var t in tt.GetInterfaces())
                if (t.Name == "iGenerator" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurableGeneratorInstance(tt), key, label, "Input", defaultValue, null, null, hint);
                else if (t.Name == "iConfigurablePlugin" && !tt.IsAbstract && !tt.IsInterface) instance = new GSUI(Utils.GetConfigurablePluginInstance(tt), key, label, "Input", defaultValue, null, null, hint);
            if (instance == null) return null;
            instance.callback = instance.CreateDefaultCallback(callback);
            instance.postfix = instance.CreateDefaultPostfix();
            return instance;
        }

        //Button
        public static GSUI Button(string label, string caption, GSOptionCallback callback, GSOptionPostfix postfix = null, string hint = "")
        {
            var t = Utils.GetCallingType();
            var instance = new GSUI(Utils.GetConfigurableGeneratorInstance(t), null, label, "Button", caption, callback, null, hint);
            return instance;
        }
    }
}