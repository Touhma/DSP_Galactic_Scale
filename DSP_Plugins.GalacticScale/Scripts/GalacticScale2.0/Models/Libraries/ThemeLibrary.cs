using System.Collections.Generic;
using System.Linq;
using GSSerializer;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSThemeLibraryConverter))]
    public class ThemeLibrary : Dictionary<string, GSTheme>
    {
        private readonly GS2.Random random = new GS2.Random(GSSettings.Seed);

        //public List<string> Hot {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (
        //                kv.Value.Temperature >= 3 &&
        //                kv.Value.PlanetType != EPlanetType.Gas &&
        //                !kv.Value.Private
        //                ) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Warm {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.Temperature >= 0 && kv.Value.Temperature < 3 && kv.Value.PlanetType != EPlanetType.Gas && !kv.Value.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Temperate {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.Temperature == 0 && kv.Value.PlanetType != EPlanetType.Gas && !(kv.Value.ThemeType == EThemeType.Private)) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Cold {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.Temperature < 0 && kv.Value.Temperature > -3 && kv.Value.PlanetType != EPlanetType.Gas && kv.Value.ThemeType == EThemeType.Telluric) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Frozen {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.Temperature <= -3 && kv.Value.PlanetType != EPlanetType.Gas && kv.Value.ThemeType != EThemeType.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        public List<string> Habitable
        {
            get
            {
                var list = new List<string>();
                foreach (var kv in this)
                    if (kv.Value.Habitable && kv.Value.ThemeType != EThemeType.Private)
                        list.Add(kv.Key);

                return list;
            }
        }

        public new GSTheme Add(string name, GSTheme theme)
        {
            base.Add(name, theme);
            return theme;
        }

        public static ThemeLibrary Vanilla()
        {
            var t = new ThemeLibrary
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
            var clone = new ThemeLibrary();
            foreach (var kvp in this) clone.Add(kvp.Key, kvp.Value);
            return clone;
        }

        public GSTheme Find(string name)
        {
            if (!ContainsKey(name))
            {
                var s = GS2.GetCaller();
                GS2.Error("Failed to find theme " + name + " in Theme Library. Using Default. > " + s);
                return Themes.Mediterranean;
            }

            return this[name];
        }

        public GSTheme QueryRandom()
        {
            return this["Mediterranean"];
        }

        public string Query(GS2.Random random, EThemeType type, EThemeHeat heat, int radius,
            EThemeDistribute distribute = EThemeDistribute.Default)
        {
            var themes = Query(type, heat, radius, distribute);
            return random.Item(themes);
        }

        public List<string> Query(EThemeType type, EThemeHeat heat, int radius,
            EThemeDistribute distribute = EThemeDistribute.Default)
        {
            //List<GSTheme> list = new List<GSTheme>();
            var types = new List<EThemeType>();
            var distributes = new List<EThemeDistribute>();
            if (type == EThemeType.Gas)
            {
                types.Add(EThemeType.Gas);
            }
            else if (type == EThemeType.Telluric)
            {
                types.Add(EThemeType.Telluric);
            }
            else if (type == EThemeType.Planet)
            {
                types.Add(EThemeType.Telluric);
                types.Add(EThemeType.Planet);
            }
            else if (type == EThemeType.Moon)
            {
                types.Add(EThemeType.Moon);
                types.Add(EThemeType.Telluric);
            }

            if (distribute == EThemeDistribute.Default)
            {
                distributes.Add(EThemeDistribute.Default);
                distributes.Add(EThemeDistribute.Birth);
                distributes.Add(EThemeDistribute.Interstellar);
                distributes.Add(EThemeDistribute.Rare);
            }
            else
            {
                distributes.Add(distribute);
            }

            (float min, float max) temp = (3, 6);

            if (heat == EThemeHeat.Warm) temp = (1, 3);

            if (heat == EThemeHeat.Temperate) temp = (0, 1);

            if (heat == EThemeHeat.Cold) temp = (-3, 0);

            if (heat == EThemeHeat.Frozen) temp = (-6, -3);

            var q = from theme in this
                where types.Contains(theme.Value.ThemeType)
                where theme.Value.Temperature < temp.max
                where theme.Value.Temperature >= temp.min
                where theme.Value.MaxRadius >= (radius > 0 ? radius : 0)
                where theme.Value.MinRadius <= (radius > 0 ? radius : 510)
                select theme.Value.Name;
            var results = q.ToList();
            if (results.Count == 0)
            {
                GS2.Error(
                    $"Could not find theme EThemeType {type} EThemeHeat {heat} Radius {radius} EThemeDistribute {distribute} Checking against temp.min:Value>={temp.min} temp.max:Value<{temp.max}");
                foreach (var k in this)
                    GS2.Warn(
                        $"{k.Key} Temp:{k.Value.Temperature} Radius:{k.Value.MinRadius}-{k.Value.MaxRadius} Type:{k.Value.ThemeType} Distribute:{k.Value.Distribute}");
                results.Add("Mediterranean");
            }

            //GS2.Warn($"Selected Themes for EThemeType {type} EThemeHeat {heat} Radius {radius} EThemeDistribute {distribute} Checking against temp.min:Value>={temp.min} temp.max:Value<{temp.max}");
            //GS2.LogJson(results);
            return results;
        }
        //public List<string> Desert {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.PlanetType == EPlanetType.Desert && kv.Value.ThemeType != EThemeType.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Volcanic {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.PlanetType == EPlanetType.Vocano && kv.Value.ThemeType != EThemeType.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> Ice {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.PlanetType == EPlanetType.Ice && kv.Value.ThemeType != EThemeType.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> GasGiant {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (kv.Value.PlanetType == EPlanetType.Gas && kv.Value.Temperature >= 0 && kv.Value.Temperature < 4 && kv.Value.ThemeType != EThemeType.Private) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> HotGasGiant {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (
        //                kv.Value.PlanetType == EPlanetType.Gas
        //                && kv.Value.Temperature >= 4
        //                && kv.Value.ThemeType != EThemeType.Private
        //            ) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}
        //public List<string> IceGiant {
        //    get {
        //        List<string> list = new List<string>();
        //        foreach (KeyValuePair<string, GSTheme> kv in this) {
        //            if (
        //                kv.Value.PlanetType == EPlanetType.Gas
        //                && kv.Value.Temperature < 0
        //                && kv.Value.ThemeType != EThemeType.Private
        //            ) {
        //                list.Add(kv.Key);
        //            }
        //        }

        //        return list;
        //    }
        //}

        public GSTheme Random()
        {
            var choice = random.Next(0, Count);
            return this.ElementAt(choice).Value;
        }

        public GSTheme Random(List<string> themes)
        {
            var choice = random.Next(0, themes.Count);
            return this[themes[choice]];
        }
    }
}