using BepInEx.Bootstrap;

namespace GalacticScale
{
    public static class ShieldCompat
    {
    public static readonly string[] PlanetwideShieldGuids =
    {
        "lltcggie.DSP.plugin.PlanetwidePlanetaryShieldGenerator", 
        "un1eagle.planetwideshield"
    };

    private static bool? _planetwideShield;

    public static bool PlanetwideShieldInstalled => _planetwideShield ??= System.Linq.Enumerable.Any(PlanetwideShieldGuids, Chainloader.PluginInfos.ContainsKey);
    }
}