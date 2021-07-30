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

        internal List<string> StringList(string key, List<string> Default)
        {
            if (!ContainsKey(key)) return Default;
            var fsSerializer = new fsSerializer();
            List<string> parsedResult = new List<string>();
            fsResult result = fsJsonParser.Parse(this[key], out fsData data);
            if (result.Failed)
            {
                GS2.Warn("Failed to parse StringList " + key);
                return Default;
            }
            var deserializedResult = fsSerializer.TryDeserialize(data, ref parsedResult);
            if (deserializedResult.Failed)
            {
                GS2.Warn("Failed to deserialize StringList " + key);
                return Default;
            }
            return parsedResult;
            
        }

        public bool GetBool(string key, bool Default = false)
        {
            bool parsedResult;
            return ContainsKey(key) ? bool.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public void Set(string key, object value)
        {
            if (value.GetType() == typeof(List<string>))
            {
                fsSerializer fs = new fsSerializer();
                var result = fs.TrySerialize(value, out fsData data);
                if (result.Failed)
                {
                    GS2.Warn("Failed to Serialize " + key);
                    return;
                }
                var stringResult = fsJsonPrinter.CompressedJson(data);
                this[key] = stringResult;
            }
            else this[key] = value.ToString();
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