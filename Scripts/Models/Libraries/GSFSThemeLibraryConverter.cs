using System;
using System.Collections.Generic;
using GSSerializer;

namespace GalacticScale
{
    public class GSFSThemeLibraryConverter : fsDirectConverter<ThemeLibrary>
    {
        public override object CreateInstance(fsData data, Type storageType)
        {
            //GS3.Log("Start");
            var t = new ThemeLibrary();
            return t;
        }

        protected override fsResult DoSerialize(ThemeLibrary model, Dictionary<string, fsData> serialized)
        {
            //GS3.Log("Start");
            foreach (var kvp in model)
                if (!kvp.Value.Base || !GS3.Config.MinifyJson)
                    SerializeMember(serialized, null, kvp.Key, kvp.Value);
            //GS3.Log("End");
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ThemeLibrary model)
        {
            //GS3.Log("Start");
            model = ThemeLibrary.Vanilla();
            var result = fsResult.Success;
            foreach (var kvp in data)
            {
                GSTheme theme;
                DeserializeMember(data, null, kvp.Key, out theme);
                if (model.ContainsKey(kvp.Key))
                    model[kvp.Key] = theme;
                else
                    model.Add(kvp.Key, theme);
            }

            //GS3.Log("End");
            return result;
        }
    }
}