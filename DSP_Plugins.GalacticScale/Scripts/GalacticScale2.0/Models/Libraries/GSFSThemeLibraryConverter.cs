using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{

    public class GSFSThemeLibraryConverter : fsDirectConverter<ThemeLibrary>
    {
        public override object CreateInstance(fsData data, Type storageType) 
        {
            //GS2.Log("GSFSThemeLibraryConverter|CreateInstance");
            //GS2.LogJson(GS2.ThemeLibrary);
            ThemeLibrary t = new ThemeLibrary();//ThemeLibrary.Vanilla();
            //t.Add("Pants", null);
            return t;
           // return GS2.ThemeLibrary;
        }

        protected override fsResult DoSerialize(ThemeLibrary model, Dictionary<string, fsData> serialized)
        {
            foreach (KeyValuePair<string, GSTheme> kvp in model)
            {
                if (!kvp.Value.Base) SerializeMember(serialized, null, kvp.Key, kvp.Value);
            }
           
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ThemeLibrary model)
        {
            //GS2.Log("GSFSThemeLibraryConverter|DoDeserialize");
            model = ThemeLibrary.Vanilla();
            //GS2.LogJson(model);
            //GS2.LogJson(GS2.ThemeLibrary);
            var result = fsResult.Success;
            foreach (var kvp in data)
            {
                GSTheme theme;
                DeserializeMember(data, null, kvp.Key, out theme);
                if (model.ContainsKey(kvp.Key)) model[kvp.Key] = theme; else model.Add(kvp.Key, theme);
            }
            //GS2.LogJson(model);
            return result;
        }
    }
}