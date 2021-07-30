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
                foreach (string name in GS2.Config.ExternalThemeNames)
                {
                    var fragments = name.Split('|');
                    var group = fragments[0];
                    var item = fragments[1];
                    if (group == "Root")
                    {
                        if (!GS2.availableExternalThemes.ContainsKey("Root"))
                        {
                            GS2.Log("No loose themes loaded!");
                            continue;
                        }
                        ThemeLibrary tl = GS2.availableExternalThemes["Root"];
                        if (tl.ContainsKey(item)) GS2.externalThemes.Add(item, tl[item]);
                    }
                    else if (GS2.availableExternalThemes.ContainsKey(group))
                    {
                        GS2.externalThemes.AddRange(GS2.availableExternalThemes[group]);
                    }

                }
                GS2.Warn("External Themes:");
                GS2.LogJson(GS2.externalThemes.Keys.ToList());
                GS2.Warn("End External Themes");
            }
        }
    }
}