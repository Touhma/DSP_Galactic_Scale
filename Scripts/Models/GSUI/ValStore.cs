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
                // GS2.Warn($"Trying to get missing value {key}");
                return def;

            return this[key];
        }
    }
}