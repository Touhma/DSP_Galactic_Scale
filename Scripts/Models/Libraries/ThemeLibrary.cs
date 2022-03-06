using System.Collections.Generic;
using System.Linq;
using GSSerializer;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSThemeLibraryConverter))]
    public class ThemeLibrary : Dictionary<string, GSTheme>
    {
        private readonly GS2.Random random = new(GSSettings.Seed);

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
            // if (name == "BlueLava")
            // {
            // GS2.Warn($"Adding {name},{theme == null} {GS2.GetCaller()}");
            // GS2.Warn(GS2.GetCaller(1));
            // GS2.Warn(GS2.GetCaller(2));
            // GS2.Warn(GS2.GetCaller(3));
            // GS2.Warn(GS2.GetCaller(4));
            // GS2.Warn(GS2.GetCaller(5));
            // }
            if (theme == null) return null;
            if (string.IsNullOrEmpty(name)) name = theme.Name ?? "Hmm";
            if (ContainsKey(name))
            {
                // GS2.Warn("Theme already exists. Updating.");
                this[name] = theme;
                return theme;
            }

            base.Add(name, theme);
            return theme;
        }

        public ThemeLibrary AddRange(ThemeLibrary values)
        {
            // GS2.Warn("Adding Range " + GS2.GetCaller() + GS2.GetCaller(1) + GS2.GetCaller(2) + GS2.GetCaller(3));
            // GS2.WarnJson(values.Select(p => p.Key).ToList());
            foreach (var theme in values)
                if (ContainsKey(theme.Key)) // GS2.Warn("Adding Duplicate Theme " + theme.Key);
                    this[theme.Key] = theme.Value;
                else Add(theme.Key, theme.Value);
            return this;
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
                ["Sakura"] = Themes.Sakura,
                ["GasGiant3"] = Themes.Gas3,
                ["GasGiant4"] = Themes.Gas4,
                ["IceGiant3"] = Themes.IceGiant3,
                ["IceGiant4"] = Themes.IceGiant4,
                ["GasGiant5"] = Themes.Gas5,
                ["AridDesert2"] = Themes.AridDesert2
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
                GS2.WarnJson(GSSettings.ThemeLibrary.Select(x => x.Key).ToList());
                return Themes.Mediterranean;
            }

            return this[name];
        }

        public GSTheme QueryRandom()
        {
            return this["Mediterranean"];
        }

        public string Query(GS2.Random random, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default, bool habitable = false)
        {
            var themes = Query(type, heat, radius, distribute, habitable);
            return random.Item(themes);
        }

        public string Query(GS2.Random random, EStar starType, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default)
        {
            var themes = Query(starType, type, heat, radius, distribute);
            return random.Item(themes);
        }

        public string Query(GS2.Random random, List<EStar> starTypes, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default)
        {
            var themes = Query(starTypes, type, heat, radius, distribute);
            return random.Item(themes);
        }

        public List<string> Query(EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default, bool habitable = false)
        {
            var allstars = new List<EStar> { EStar.A, EStar.B, EStar.BlackHole, EStar.BlueGiant, EStar.F, EStar.G, EStar.K, EStar.M, EStar.NeutronStar, EStar.O, EStar.RedGiant, EStar.WhiteDwarf, EStar.WhiteGiant, EStar.YellowGiant };
            var themes = QueryThemes(allstars, type, heat, radius, distribute, habitable);
            var results = from theme in themes select theme.Name;
            return results.ToList();
        }

        public List<string> Query(List<EStar> starTypes, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default)
        {
            var themes = QueryThemes(starTypes, type, heat, radius, distribute);
            var results = from theme in themes select theme.Name;
            return results.ToList();
        }

        public List<string> Query(EStar starType, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default)
        {
            var themes = QueryThemes(new List<EStar> { starType }, type, heat, radius, distribute);
            var results = from theme in themes select theme.Name;
            return results.ToList();
        }

        public List<GSTheme> QueryThemes(List<EStar> starTypes, EThemeType type, EThemeHeat heat, int radius, EThemeDistribute distribute = EThemeDistribute.Default, bool habitable = false)
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

            var q = from theme in this where types.Contains(theme.Value.ThemeType) where distributes.Contains(theme.Value.Distribute) where theme.Value.StarTypes.Intersect(starTypes).Count() > 1 where theme.Value.Temperature < temp.max where theme.Value.Temperature >= temp.min where theme.Value.MaxRadius >= (radius > 0 ? radius : 0) where theme.Value.MinRadius <= (radius > 0 ? radius : 510) select theme.Value;
            var results = q.ToList();
            if (habitable)
                results = (from theme in results where theme.Habitable select theme).ToList();
            //GS2.WarnJson(results.Select(o => o.Name).ToList());

            if (results.Count == 0)
            {
                GS2.Error($"Could not find theme EThemeType {type} EThemeHeat {heat} Radius {radius} EThemeDistribute {distribute} Checking against temp.min:Value>={temp.min} temp.max:Value<{temp.max}");
                foreach (var k in this)
                    GS2.Warn($"{k.Key} Temp:{k.Value.Temperature} Radius:{k.Value.MinRadius}-{k.Value.MaxRadius} Type:{k.Value.ThemeType} Distribute:{k.Value.Distribute}");
                if (type == EThemeType.Gas)
                    switch (heat)
                    {
                        case EThemeHeat.Cold:
                            results.Add(Themes.IceGiant);
                            break;
                        case EThemeHeat.Temperate:
                            results.Add(Themes.Gas2);
                            break;
                        case EThemeHeat.Frozen:
                            results.Add(Themes.IceGiant2);
                            break;
                        case EThemeHeat.Warm:
                        case EThemeHeat.Hot:
                            results.Add(Themes.Gas);
                            break;
                    }
                else
                    switch (heat)
                    {
                        case EThemeHeat.Temperate:
                            results.Add(Themes.Mediterranean);
                            break;
                        case EThemeHeat.Cold:
                        case EThemeHeat.Frozen:
                            results.Add(Themes.AshenGelisol);
                            break;
                        case EThemeHeat.Warm:
                            results.Add(Themes.Barren);
                            break;
                        case EThemeHeat.Hot:
                            results.Add(Themes.VolcanicAsh);
                            break;
                    }
            }

            // GS2.Warn($"Selected Themes for EThemeType {type} EThemeHeat {heat} Radius {radius} EThemeDistribute {distribute} Checking against temp.min:Value>={temp.min} temp.max:Value<{temp.max}");
            // GS2.WarnJson((from result in results select result.Name).ToList());
            return results;
        }

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