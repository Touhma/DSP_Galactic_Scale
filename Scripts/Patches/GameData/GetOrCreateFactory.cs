using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "GetOrCreateFactory")]
        public static bool GetOrCreateFactory(PlanetData planet, ref GameData __instance, ref PlanetFactory __result,
            ref int ___factoryCount, ref PlanetFactory[] ___factories, ref GameStatData ___statistics)
        {
            if (planet.factory != null)
            {
                __result = planet.factory;
                return false;
            }

            var planetFactory = new PlanetFactory();
            GS2.Log($"Creating Factory on {planet.name} with index {___factoryCount} ___factories.Length = {___factories.Length} GSSettings PlanetCount:{GSSettings.PlanetCount}");
            
            if (___factoryCount >= ___factories.Length)
            {
                var newFactories = new PlanetFactory[___factories.Length * 2];
                for (var i = 0; i < ___factories.Length; i++) newFactories[i] = ___factories[i];
                ___factories = newFactories;
            }
            planetFactory.Init(__instance, planet, ___factoryCount);
            ___factories[___factoryCount] = planetFactory;
            planet.factory = planetFactory;
            planet.factoryIndex = ___factoryCount;
            ___statistics.production.CreateFactoryStat(___factoryCount);
            ++___factoryCount;
            __result = planetFactory;
            return false;
        }
    }
}