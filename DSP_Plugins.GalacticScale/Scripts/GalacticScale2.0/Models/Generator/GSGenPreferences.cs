using System.Collections.Generic;
using GSFullSerializer;

namespace GalacticScale
{
    public class GSGenPreferences : Dictionary<string, string>
    {
        public object Get(string key)
        {
            return ContainsKey(key)?this[key]:null;
        }
        public string GetString(string key, string Default = "", bool forceToString = false)
        {
            string parsedString = ContainsKey(key)?this[key] is string?this[key]:forceToString?this[key].ToString():Default:Default;
            return parsedString;
        }
        public int GetInt(string key, int Default = -1)
        {
            int parsedResult;
            return ContainsKey(key) ? (int.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }
        public float GetFloat(string key, float Default = -1f)
        {
            float parsedResult;
            return ContainsKey(key) ? (float.TryParse(this[key], out parsedResult)) ? parsedResult : Default : Default;
        }
        public double GetDouble(string key, double Default = -1.0)
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
        public string SerializeAndSet(string key, object value)
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