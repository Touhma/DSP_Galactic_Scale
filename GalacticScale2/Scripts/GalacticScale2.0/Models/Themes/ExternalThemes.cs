namespace GalacticScale
{
    public static partial class GS2
    {
        public static ThemeLibrary externalThemes = new ThemeLibrary();

        public static class ExternalThemeProcessor
        {
            public static void LoadEnabledThemes()
            {
                LogJson(Config.ExternalThemeNames);
                externalThemes = new ThemeLibrary();
                Warn(GSSettings.ThemeLibrary.Count.ToString());
                foreach (var name in Config.ExternalThemeNames)
                {
                    Log($"Loading {name}");
                    var fragments = name.Split('|');
                    var group = fragments[0];
                    var item = fragments[1];
                    if (availableExternalThemes.ContainsKey(group) && availableExternalThemes[group].ContainsKey(item))
                    {
                        externalThemes.Add(item, availableExternalThemes[group][item]);
                        Log($"Added {name}");
                        // GS2.WarnJson(externalThemes.Select(o=>o.Key).ToList());
                    }
                    else
                    {
                        Warn($"Missing Theme {group} - {item}");
                    }
                    // if (group == "Root")
                    // {
                    //     if (!GS2.availableExternalThemes.ContainsKey("Root"))
                    //     {
                    //         GS2.Log("No loose themes loaded!");
                    //         continue;
                    //     }
                    //     ThemeLibrary tl = GS2.availableExternalThemes["Root"];
                    //     if (tl.ContainsKey(item)) GS2.externalThemes.Add(item, tl[item]);
                    // }
                    // else if (GS2.availableExternalThemes.ContainsKey(group))
                    // {
                    //     GS2.externalThemes.AddRange(GS2.availableExternalThemes[group]);
                    // }
                }
                // GS2.Warn("External Themes:");
                // GS2.LogJson(GS2.externalThemes.Keys.ToList());
                // GS2.Warn("End External Themes");
            }
        }
    }
}