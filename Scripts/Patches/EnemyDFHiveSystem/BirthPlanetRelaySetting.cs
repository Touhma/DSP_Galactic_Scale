using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnEnemyDFHiveSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFHiveSystem), nameof(EnemyDFHiveSystem.CheckRelayCoLandingCondition), new[] { typeof(int), typeof(double), typeof(double), typeof(double) })]
        public static bool CheckRelayCoLandingConditionForBirthPlanet(
            EnemyDFHiveSystem __instance,
            int _astroId,
            ref bool __result)
        {
            if (GS2.AllowDarkFogOnBirthPlanet || __instance?.galaxy == null ||
                _astroId != __instance.galaxy.birthPlanetId)
                return true;

            __result = true;
            return false;
        }
    }
}
