using HarmonyLib;
using System.Collections.Concurrent;

namespace GalacticScale
{
    public static class SpaceLaserFactoryReadiness
    {
        public static PlanetFactory GetFactory(SkillSystem skillSystem, int astroId)
        {
            var factories = skillSystem?.astroFactories;
            return factories != null && astroId >= 0 && astroId < factories.Length
                ? factories[astroId]
                : null;
        }

        public static bool FactoryReady(SkillSystem skillSystem, int astroId)
        {
            var factory = GetFactory(skillSystem, astroId);
            return factory?.planet?.factoryLoaded == true &&
                   factory.enemyPool != null && factory.craftPool != null &&
                   factory.entityPool != null && factory.vegePool != null &&
                   factory.veinPool != null;
        }

        public static bool PlanetTargetReady(SkillSystem skillSystem, SkillTarget target)
        {
            if (target.type == ETargetType.Enemy && target.astroId >= 1000000)
                return SpaceEnemyTargetReady(skillSystem, target);

            if (!SpaceSector.IsPlanetAstro(target.astroId))
                return true;

            var factory = GetFactory(skillSystem, target.astroId);
            switch (target.type)
            {
                case ETargetType.Enemy:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.enemyPool, target.id);
                case ETargetType.Craft:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.craftPool, target.id);
                case ETargetType.None:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.entityPool, target.id);
                case ETargetType.Vegetable:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.vegePool, target.id);
                case ETargetType.Vein:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.veinPool, target.id);
                case ETargetType.Prebuild:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.prebuildPool, target.id);
                case ETargetType.Ruin:
                    return FactoryReady(skillSystem, target.astroId) && HasEntry(factory?.ruinPool, target.id);
                default:
                    return true;
            }
        }

        private static bool SpaceEnemyTargetReady(SkillSystem skillSystem, SkillTarget target)
        {
            var sector = skillSystem?.sector;
            var hiveIndex = target.astroId - 1000000;
            var hives = sector?.dfHivesByAstro;
            var astros = sector?.astros;
            var enemies = sector?.enemyPool;

            return sector != null &&
                   hives != null && hiveIndex >= 0 && hiveIndex < hives.Length && hives[hiveIndex] != null &&
                   astros != null && hiveIndex < astros.Length && astros[hiveIndex].id > 0 &&
                   enemies != null && target.id > 0 && target.id < enemies.Length &&
                   enemies[target.id].id == target.id;
        }

        public static bool RayTestReady(SkillSystem skillSystem, int astroId, int rayId)
        {
            var factory = GetFactory(skillSystem, astroId);
            var rayTests = factory?.planetATField?.rayTests;
            return FactoryReady(skillSystem, astroId) && rayTests != null &&
                   rayId > 0 && rayId < rayTests.Length;
        }

        private static bool HasEntry<T>(T[] pool, int id)
        {
            return pool != null && id >= 0 && id < pool.Length;
        }
    }

    public static class PatchOnSpaceLaserSweep
    {
        private static readonly ConcurrentDictionary<int, byte> OwnerFactoryWarnings = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceLaserSweep), nameof(SpaceLaserSweep.TickSkillLogic))]
        public static bool TickSkillLogic_Prefix(ref SpaceLaserSweep __instance, SkillSystem skillSystem)
        {
            if (SpaceSector.IsPlanetAstro(__instance.astroId) &&
                !SpaceLaserFactoryReadiness.FactoryReady(skillSystem, __instance.astroId))
            {
                if (OwnerFactoryWarnings.TryAdd(__instance.astroId, 0))
                    GS2.Warn($"Retiring space laser sweeps while owner planet factory is unavailable " +
                             $"(astro {__instance.astroId})");
                return Retire(ref __instance);
            }

            if (!SpaceLaserFactoryReadiness.PlanetTargetReady(skillSystem, __instance.caster))
                return Retire(ref __instance);

            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0 &&
                !SpaceLaserFactoryReadiness.RayTestReady(skillSystem, __instance.atfAstroId, __instance.atfRayId))
            {
                return Retire(ref __instance);
            }

            return true;
        }

        private static bool Retire(ref SpaceLaserSweep sweep)
        {
            sweep.life = 0;
            return false;
        }
    }
}
