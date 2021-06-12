﻿using GSFullSerializer;
using System;
using System.Collections.Generic;

namespace GalacticScale {
    public class GSFSSettingsConverter : fsDirectConverter<GSSettings> {
        public override object CreateInstance(fsData data, Type storageType) {
            //GS2.Log("Start");
            GSSettings.Reset(0);
            return GSSettings.Instance;
        }
        protected override fsResult DoSerialize(GSSettings model, Dictionary<string, fsData> serialized) {
            //GS2.Log("Start" + GS2.GetCaller());
            SerializeMember(serialized, null, "Seed", GSSettings.Seed);
            SerializeMember(serialized, null, "GalaxyParams", GSSettings.GalaxyParams);
            SerializeMember(serialized, null, "Stars", GSSettings.Stars);
            SerializeMember(serialized, null, "ThemeLibrary", GSSettings.ThemeLibrary);
            if (GSSettings.BirthPlanet != null) {
                SerializeMember(serialized, null, "BirthPlanet", GSSettings.BirthPlanet.Name);
            }
            //GS2.Log("End");
            return fsResult.Success;
        }
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GSSettings model) {
            GS2.Log("Start");
            //GS2.Log(model.stars.Count.ToString()+" Stars Already in Model");
            model.stars.Clear();
            var result = fsResult.Success;
            int seed = 0;
            //GS2.Log("Deserializing Seed");
            if (data.ContainsKey("seed")) {
                DeserializeMember(data, null, "seed", out seed);
            }

            if (data.ContainsKey("Seed")) {
                DeserializeMember(data, null, "Seed", out seed);
            }

            model.seed = seed;
            //GS2.Log("seed");
            GS2.Log("Deserializing GalaxyParams");
            GSGalaxyParams galaxyParams = new GSGalaxyParams();
            if (data.ContainsKey("galaxyParams")) {
                DeserializeMember(data, null, "galaxyParams", out galaxyParams);
            }

            if (data.ContainsKey("GalaxyParams")) {
                DeserializeMember(data, null, "GalaxyParams", out galaxyParams);
            }

            GS2.Log("Deserializing Stars");
            GSStars stars = null;
            if (data.ContainsKey("stars")) {
                DeserializeMember(data, null, "stars", out stars);
            }

            if (data.ContainsKey("Stars")) {
                DeserializeMember(data, null, "Stars", out stars);
            }

            model.stars = stars;
            GS2.Log("Deserializing ThemeLibrary");
            ThemeLibrary tl = null;
            if (data.ContainsKey("themeLibrary")) {
                DeserializeMember(data, null, "themeLibrary", out tl);
            }

            if (data.ContainsKey("ThemeLibrary")) {
                DeserializeMember(data, null, "ThemeLibrary", out tl);
            }

            model.themeLibrary = tl;
            string birthPlanetName = null;
            if (data.ContainsKey("BirthPlanet")) {
                DeserializeMember(data, null, "BirthPlanet", out birthPlanetName);
            }

            if (!string.IsNullOrEmpty(birthPlanetName)) {
                GSSettings.BirthPlanetName = birthPlanetName;
            }

            return result;
        }
    }
}