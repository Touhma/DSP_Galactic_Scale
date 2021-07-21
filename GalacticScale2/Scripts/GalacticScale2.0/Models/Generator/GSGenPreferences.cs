using System.Collections.Generic;
using GSSerializer;

namespace GalacticScale
{
    public class GSGenPreferences : Dictionary<string, string>
    {
        public string Get(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }

        public string GetString(string key, string Default = "", bool forceToString = false)
        {
            var parsedString = ContainsKey(key)
                ? this[key] is string ? this[key] :
                forceToString ? this[key] : Default
                : Default;
            return parsedString;
        }

        public int GetInt(string key, int Default = -1)
        {
            int parsedResult;
            return ContainsKey(key) ? int.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public float GetFloat(string key, float Default = -1f)
        {
            float parsedResult;
            return ContainsKey(key) ? float.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public double GetDouble(string key, double Default = -1.0)
        {
            double parsedResult;
            return ContainsKey(key) ? double.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public bool GetBool(string key, bool Default = false)
        {
            bool parsedResult;
            return ContainsKey(key) ? bool.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public void Set(string key, object value)
        {
            if (key == "minPlanetCount") GS2.Warn($"Setting minPlanetCount in preferences to {value}");
            this[key] = value.ToString();
        }

        public string SerializeAndSet(string key, object value)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(value, out var data);
            var json = fsJsonPrinter.CompressedJson(data);
            this[key] = json;
            return json;
        }

        public string Serialize(object value, bool pretty = true)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(value, out var data);
            if (!pretty) return fsJsonPrinter.CompressedJson(data);

            return fsJsonPrinter.PrettyJson(data);
        }
    }
}