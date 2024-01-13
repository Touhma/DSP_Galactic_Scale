using System.Collections.Generic;

namespace GalacticScale
{
    public class ValStore : Dictionary<string, Val>
    {
        public Val Set(string key, Val value)
        {
            if (ContainsKey(key)) this[key] = value;
            else Add(key, value);
            return value;
        }

        public Val Get(string key, Val def = null)
        {
            if (!ContainsKey(key))
            {
                GS2.DevLog($"Trying to get missing value {key}");
                return def;
            }

            return this[key];
        }
        public Val GetBool(string key)
        {
            return Get(key, false);
        }
        public Val GetString(string key)
        {
            return Get(key, "Key Not Found");
        }
        public Val GetInt(string key)
        {
            return Get(key, -1);
        }
        public Val GetFloat(string key)
        {
            return Get(key, -1f);
        }
        public Val GetDouble(string key)
        {
            return Get(key, -1.0);
        }
        
    }
}