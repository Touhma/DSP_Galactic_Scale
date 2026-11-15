using HarmonyLib;

namespace GalacticScale
{
    public static class PatchOnSpaceLaserOneShot
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceLaserOneShot), nameof(SpaceLaserOneShot.TickSkillLogic))]
        public static bool TickSkillLogic_Prefix(ref SpaceLaserOneShot __instance, SkillSystem skillSystem)
        {
            if (!SpaceLaserFactoryReadiness.PlanetTargetReady(skillSystem, __instance.caster) ||
                !SpaceLaserFactoryReadiness.PlanetTargetReady(skillSystem, __instance.target))
            {
                __instance.life = 0;
                return false;
            }

            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0 &&
                !SpaceLaserFactoryReadiness.RayTestReady(skillSystem, __instance.atfAstroId, __instance.atfRayId))
            {
                __instance.life = 0;
                return false;
            }

            return true;
        }
    }
}