namespace GalacticScale
{
    public static partial class GS3
    {
        public static ThemeLibrary externalThemes = new();

        public static class ExternalThemeProcessor
        {
            public static void LoadEnabledThemes()
            {
                // LogJson(Config.ExternalThemeNames);
                externalThemes = new ThemeLibrary();
                // Warn(GSSettings.ThemeLibrary.Count.ToString());
                foreach (var name in Config.ExternalThemeNames)
                {
                    // Log($"Loading {name}");
                    var fragments = name.Split('|');
                    var group = fragments[0];
                    var item = fragments[1];
                    if (availableExternalThemes.ContainsKey(group) && availableExternalThemes[group].ContainsKey(item))
                    {
                        externalThemes.Add(item, availableExternalThemes[group][item]);
                        Log($"Added {name}");
                        // GS3.WarnJson(externalThemes.Select(o=>o.Key).ToList());
                    }
                    else
                    {
                        Warn($"Missing Theme {group} - {item}");
                    }
                    // if (group == "Root")
                    // {
                    //     if (!GS3.availableExternalThemes.ContainsKey("Root"))
                    //     {
                    //         GS3.Log("No loose themes loaded!");
                    //         continue;
                    //     }
                    //     ThemeLibrary tl = GS3.availableExternalThemes["Root"];
                    //     if (tl.ContainsKey(item)) GS3.externalThemes.Add(item, tl[item]);
                    // }
                    // else if (GS3.availableExternalThemes.ContainsKey(group))
                    // {
                    //     GS3.externalThemes.AddRange(GS3.availableExternalThemes[group]);
                    // }
                }
                // GS3.Warn("External Themes:");
                // GS3.LogJson(GS3.externalThemes.Keys.ToList());
                // GS3.Warn("End External Themes");
            }
        }
    }
}