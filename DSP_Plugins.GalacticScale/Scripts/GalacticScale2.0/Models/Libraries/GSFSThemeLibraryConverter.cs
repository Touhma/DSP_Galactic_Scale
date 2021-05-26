﻿using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{

    public class GSFSThemeLibraryConverter : fsDirectConverter<ThemeLibrary>
    {
        public override object CreateInstance(fsData data, Type storageType) 
        {
            GS2.Log("Start");
            ThemeLibrary t = new ThemeLibrary();
            return t;
        }

        protected override fsResult DoSerialize(ThemeLibrary model, Dictionary<string, fsData> serialized)
        {
            GS2.Log("Start");
            foreach (KeyValuePair<string, GSTheme> kvp in model)
            {
                if (!kvp.Value.Base) SerializeMember(serialized, null, kvp.Key, kvp.Value);
            }
            GS2.Log("End");
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ThemeLibrary model)
        {
            GS2.Log("Start");
            model = ThemeLibrary.Vanilla();
            var result = fsResult.Success;
            foreach (var kvp in data)
            {
                GSTheme theme;
                DeserializeMember(data, null, kvp.Key, out theme);
                if (model.ContainsKey(kvp.Key)) model[kvp.Key] = theme; else model.Add(kvp.Key, theme);
            }
            GS2.Log("End");
            return result;
        }
    }
}