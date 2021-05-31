﻿using GSFullSerializer;
using System.Collections.Generic;
using System.Linq;
namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSThemeLibraryConverter))]
    public class ThemeLibrary : Dictionary<string, GSTheme>
    {
        private GS2.Random random = GS2.random;
        public ThemeLibrary()
        {
        }
        public static ThemeLibrary Vanilla()
        {
            ThemeLibrary t = new ThemeLibrary()
            {
                ["Mediterranean"] = Themes.Mediterranean,
                ["GasGiant"] = Themes.Gas,
                ["GasGiant2"] = Themes.Gas2,
                ["IceGiant"] = Themes.IceGiant,
                ["IceGiant2"] = Themes.IceGiant2,
                ["AridDesert"] = Themes.AridDesert,
                ["AshenGelisol"] = Themes.AshenGelisol,
                ["OceanicJungle"] = Themes.OceanicJungle,
                ["Lava"] = Themes.Lava,
                ["IceGelisol"] = Themes.IceGelisol,
                ["Barren"] = Themes.Barren,
                ["Gobi"] = Themes.Gobi,
                ["VolcanicAsh"] = Themes.VolcanicAsh,
                ["RedStone"] = Themes.RedStone,
                ["Prairie"] = Themes.Prairie,
                ["OceanWorld"] = Themes.OceanWorld,
                ["IceLake"] = Themes.IceLake,
                ["Hurricane"] = Themes.Hurricane,
                ["SaltLake"] = Themes.SaltLake,
                ["Sakura"] = Themes.Sakura
            };
            return t;
        }
        public ThemeLibrary Clone()
        {
            ThemeLibrary clone = new ThemeLibrary();
            foreach (var kvp in this)
            {
                clone.Add(kvp.Key, kvp.Value);
            }
            return clone;
        }
        public GSTheme Find(string name)
        {
            if (!ContainsKey(name))
            {
                string s = GS2.GetCaller();
                GS2.Error("Failed to find theme " + name + " in Theme Library. Using Default. > " + s);
                return Themes.Mediterranean;
            }
            return this[name];
        }
        public List<string> Hot
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.Temperature >= 3 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Warm
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.Temperature >= 0 && kv.Value.Temperature < 3 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Temperate
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.Temperature == 0 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Cold
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.Temperature < 0 && kv.Value.Temperature > -3 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Frozen
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.Temperature <= -3 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Habitable
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.PlanetType == EPlanetType.Ocean && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Desert
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.PlanetType == EPlanetType.Desert && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Volcanic
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.PlanetType == EPlanetType.Vocano && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> Ice
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.PlanetType == EPlanetType.Ice && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> GasGiant
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this) if (kv.Value.PlanetType == EPlanetType.Gas && kv.Value.Temperature >= 0 && kv.Value.Temperature < 4 && !kv.Value.Private) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> HotGasGiant
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this)
                    if (
                        kv.Value.PlanetType == EPlanetType.Gas
                        && kv.Value.Temperature >= 4
                        && !kv.Value.Private
                    ) list.Add(kv.Key);
                return list;
            }
        }
        public List<string> IceGiant
        {
            get
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, GSTheme> kv in this)
                    if (
                        kv.Value.PlanetType == EPlanetType.Gas
                        && kv.Value.Temperature < 0
                        && !kv.Value.Private
                    ) list.Add(kv.Key);
                return list;
            }
        }

        public GSTheme Random()
        {
            int choice = random.Next(0, this.Count);
            return this.ElementAt(choice).Value;
        }
        public GSTheme Random(List<string> themes)
        {
            int choice = random.Next(0, themes.Count);
            return this[themes[choice]];
        }

    }

}