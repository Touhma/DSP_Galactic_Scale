using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static ThemeLibrary externalThemes = new ThemeLibrary();
        public static class ExternalThemeProcessor
        {
            public static void LoadEnabledThemes()
            {
                GS2.LogJson(Config.ExternalThemeNames);
                externalThemes = new ThemeLibrary();
                foreach (string name in GS2.Config.ExternalThemeNames)
                {
                    GS2.Log($"Loading {name}");
                    var fragments = name.Split('|');
                    var group = fragments[0];
                    var item = fragments[1];
                    if (availableExternalThemes.ContainsKey(group) && availableExternalThemes[group].ContainsKey(item))
                    {
                        GS2.externalThemes.Add(item, GS2.availableExternalThemes[group][item]);
                        GS2.Log($"Added {name}");
                        // GS2.WarnJson(externalThemes.Select(o=>o.Key).ToList());
                    }
                    else
                    {
                        GS2.Warn($"Missing Theme {group} - {item}");
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