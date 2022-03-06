namespace GalacticScale
{
    //public partial class PatchOnGameData
    //{
    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(GameData), "GetOrCreateFactory")]
    //    public static bool GetOrCreateFactory(PlanetData planet, ref GameData __instance, ref PlanetFactory __result,
    //        ref int ___factoryCount, ref PlanetFactory[] ___factories, ref GameStatData ___statistics)
    //    {
    //        if (planet.factory != null)
    //        {
    //            __result = planet.factory;
    //            return false;
    //        }

    //        var planetFactory = new PlanetFactory();
    //        planetFactory.Init(__instance, planet, ___factoryCount);
    //        ___factories[___factoryCount] = planetFactory;
    //        planet.factory = planetFactory;
    //        planet.factoryIndex = ___factoryCount;
    //        ___statistics.production.CreateFactoryStat(___factoryCount);
    //        ++___factoryCount;
    //        __result = planetFactory;
    //        return false;
    //    }
    //}
}