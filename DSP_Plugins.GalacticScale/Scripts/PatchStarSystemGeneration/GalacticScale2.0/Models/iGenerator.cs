using UnityEngine.Events;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
    public interface iGenerator
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        string GUID { get; }
        void Generate(int starCount);
        GSGeneratorConfig Config { get; }
       
        void Init();
    }
    public interface iConfigurableGenerator : iGenerator
    {
        List<GSOption> Options { get; }
        void Import(GSGenPreferences preferences);
        GSGenPreferences Export();
    }
    public class GSGeneratorConfig
    {
        public bool DisableStarCountSlider = false;
        public bool DisableSeedInput = false;
        private int _minStarCount = 4;
        public int MinStarCount{ get => _minStarCount; set => _minStarCount = (int)Maths.Clamp(value, 1.0, _maxStarCount); }
        private int _maxStarCount = 64;
        public int MaxStarCount { get => _maxStarCount; set => _maxStarCount = (int)Maths.Clamp((double)value, _minStarCount, 1024); }
        public GSGeneratorConfig(bool disableStarCountSlider = false, bool disableSeedInput = false, int minStarCount = 1, int maxStarCount = 1024)
        {
            DisableStarCountSlider = disableStarCountSlider;
            DisableSeedInput = disableSeedInput;
            MinStarCount = minStarCount;
            MaxStarCount = maxStarCount;
        }
    }
    public delegate void GSOptionCallback(object o);
    public delegate void GSOptionPostfix();
    public class GSOption
    {
        public string label;
        public string type;
        public object data;
        public GSOptionCallback callback;
        public string tip;
        public UnityEngine.RectTransform rectTransform;
        public GSOptionPostfix postfix;
        public GSOption(string _label, string _type, object _data, GSOptionCallback _callback, GSOptionPostfix _postfix, string _tip = "")
        {
            this.label = _label;
            this.type = _type;
            this.data = _data;
            this.callback = _callback;
            if (_postfix == null) postfix = delegate { };
            else postfix = _postfix;
            this.tip = _tip;
        }
    }
    public class GSGenPreferences : Dictionary<string, string>
    {
        public object Get(string key)
        {
            return ContainsKey(key)?this[key]:null;
        }
        public int GetInt(string key, int Default = 0)
        {
            int parsedResult;
            return ContainsKey(key) ? (int.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }
        public float GetFloat(string key, float Default = 0f)
        {
            float parsedResult;
            return ContainsKey(key) ? (float.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }
        public double GetDouble(string key, double Default = 0.0)
        {
            double parsedResult;
            return ContainsKey(key) ? (double.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }
        public bool GetBool(string key, bool Default = false)
        {
            bool parsedResult;
            return ContainsKey(key) ? (bool.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }

        public void Set(string key, object value)
        {
            this[key] = value.ToString();
        }
        public string Serialize(string key, object value)
        {
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(value, out fsData data);
            string json = fsJsonPrinter.CompressedJson(data);
            this[key] = json;
            return json;
        }
        public string Serialize(object value, bool pretty = true)
        {
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(value, out fsData data);
            if (!pretty) return fsJsonPrinter.CompressedJson(data);
            return fsJsonPrinter.PrettyJson(data);
        }

    }
}