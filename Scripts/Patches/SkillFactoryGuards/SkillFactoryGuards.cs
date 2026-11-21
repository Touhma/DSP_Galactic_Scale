using HarmonyLib;

namespace GalacticScale
{
    public static class PatchSkillFactoryGuards
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalGeneralProjectile), nameof(LocalGeneralProjectile.TickSkillLogic))]
        public static bool LocalGeneralProjectile_Prefix(ref LocalGeneralProjectile __instance, SkillSystem skillSystem)
        {
            if (SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.astroId)) return true;
            __instance.targetCombatStatId = 0;
            __instance.life = 0;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalCannonade), nameof(LocalCannonade.TickSkillLogic))]
        public static bool LocalCannonade_Prefix(ref LocalCannonade __instance, SkillSystem skillSystem)
        {
            if (SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.astroId)) return true;
            __instance.life = 0;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalDisturbingWave), nameof(LocalDisturbingWave.TickSkillLogic))]
        public static bool LocalDisturbingWave_Prefix(ref LocalDisturbingWave __instance, SkillSystem skillSystem)
        {
            if (SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.astroId)) return true;
            __instance.life = 0;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalLaserContinuous), nameof(LocalLaserContinuous.TickSkillLogic))]
        public static bool LocalLaserContinuous_Prefix(ref LocalLaserContinuous __instance, SkillSystem skillSystem)
        {
            if (SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.astroId)) return true;
            __instance.Stop(skillSystem);
            __instance.fade = 0f;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bomb_Explosive), nameof(Bomb_Explosive.TickSkillLogic))]
        public static bool BombExplosive_Prefix(ref Bomb_Explosive __instance, SkillSystem skillSystem)
        {
            if (__instance.nearPlanetAstroId <= 0 ||
                SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.nearPlanetAstroId)) return true;
            __instance.life = 0;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bomb_EMCapsule), nameof(Bomb_EMCapsule.TickSkillLogic))]
        public static bool BombEMCapsule_Prefix(ref Bomb_EMCapsule __instance, SkillSystem skillSystem)
        {
            if (__instance.nearPlanetAstroId <= 0 ||
                SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.nearPlanetAstroId)) return true;
            __instance.life = 0;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bomb_Liquid), nameof(Bomb_Liquid.TickSkillLogic))]
        public static bool BombLiquid_Prefix(ref Bomb_Liquid __instance, SkillSystem skillSystem)
        {
            if (__instance.nearPlanetAstroId <= 0 ||
                SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.nearPlanetAstroId)) return true;
            __instance.life = 0;
            return false;
        }
    }
}
