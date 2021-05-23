using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{

    public class GSFSSettingsConverter : fsDirectConverter<GSSettings>
    {
        public override object CreateInstance(fsData data, Type storageType)
        {
            GS2.Log("Start");
            GSSettings.Reset(0);
            return GSSettings.Instance;
        }

        protected override fsResult DoSerialize(GSSettings model, Dictionary<string, fsData> serialized)
        {
            GS2.Log("Start");

            SerializeMember(serialized, null, "Seed", GSSettings.Seed);


            GS2.Log("End");
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GSSettings model)
        {
            GS2.Log("Start");
            GS2.Log(model.stars.Count.ToString());
            var result = fsResult.Success;
            int seed = 0;
            GS2.Log("Deserializing Seed");
            if (data.ContainsKey("seed")) DeserializeMember(data, null, "seed", out seed);
            GSSettings.Seed = seed; 
            GS2.Log("Deserializing GalaxyParams");
            GSGalaxyParams galaxyParams = new GSGalaxyParams();
            if (data.ContainsKey("galaxyParams")) DeserializeMember(data, null, "galaxyParams", out galaxyParams);
            GSSettings.GalaxyParams = galaxyParams;
            GS2.Log("Deserializing Stars");
            List<GSStar> stars = null;
            if (data.ContainsKey("stars")) DeserializeMember(data, null, "stars", out stars);
            model.stars = stars;
            GS2.Log("Deserializing ThemeLibrary");
            ThemeLibrary tl = null;
            if (data.ContainsKey("themeLibrary")) DeserializeMember(data, null, "themeLibrary", out tl);
            GS2.ThemeLibrary = tl;
            model.themeLibrary = tl;

            //GS2.Log("Instance");
            //GS2.LogJson(GSSettings.Instance);
            //GS2.Log("model");
            //GS2.LogJson(model);
            //GS2.Log("Data =");
            //GS2.LogJson(data);
            //GS2.Log("End");
            return result;
        }
    }
}