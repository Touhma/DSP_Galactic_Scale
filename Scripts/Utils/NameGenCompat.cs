using System;
using System.Globalization;
using System.Reflection;
using HarmonyLib;

namespace GalacticScale
{
    internal static class NameGenCompat
    {
        private static readonly MethodInfo RandomStarNameMethod = AccessTools.Method(typeof(NameGen), "_RandomStarName");

        internal static string RandomStarName(int seed, StarData starData)
        {
            if (RandomStarNameMethod == null)
                throw new MissingMethodException(nameof(NameGen), "_RandomStarName");

            var parameterCount = RandomStarNameMethod.GetParameters().Length;
            if (parameterCount == 2)
                return (string)RandomStarNameMethod.Invoke(null, new object[] { seed, starData });

            // DSP 0.10.35 added the selected language's LCID to star-name generation.
            if (parameterCount == 3)
            {
                var lcid = (int?)AccessTools.Property(typeof(Localization), "CurrentLanguageLCID")?.GetValue(null, null)
                           ?? CultureInfo.CurrentUICulture.LCID;
                return (string)RandomStarNameMethod.Invoke(null, new object[] { seed, lcid, starData });
            }

            throw new MissingMethodException(nameof(NameGen), "_RandomStarName with a supported signature");
        }
    }
}
