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
            var parsedString = ContainsKey(key) ? this[key] is string ? this[key] : forceToString ? this[key] : Default : Default;
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

        public List<string> GetStringList(string key, List<string> Default)
        {
            // GS2.Warn("Getting StringList " + key);
            if (!ContainsKey(key)) return Default;
            // GS2.Warn("Didnt return default");
            var fsSerializer = new fsSerializer();
            var parsedResult = new List<string>();
            var result = fsJsonParser.Parse(this[key], out var data);
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

            // GS2.Warn("Returning:");
            // GS2.WarnJson(parsedResult);
            return parsedResult;
        }

        public bool GetBool(string key, bool Default = false)
        {
            bool parsedResult;
            return ContainsKey(key) ? bool.TryParse(this[key], out parsedResult) ? parsedResult : Default : Default;
        }

        public FloatPair GetFloatFloat(string key, FloatPair Default = new FloatPair())
        {
            if (!ContainsKey(key)) return Default;
            Val o = this[key];
            return o;
        }

        public void Set(string key, object value)
        {
            if (value.GetType() == typeof(List<string>))
            {
                var fs = new fsSerializer();
                var result = fs.TrySerialize(value, out var data);
                if (result.Failed)
                {
                    GS2.Warn("Failed to Serialize " + key);
                    return;
                }

                var stringResult = fsJsonPrinter.CompressedJson(data);
                //if (value is FloatPair) GS2.Warn(stringResult);
                this[key] = stringResult;
            }
            else
            {
                // GS2.Log($"Setting {key} to {value}");
                this[key] = value.ToString();
            }
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