using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public static class PatchOnLocalGeneralProjectile
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(LocalGeneralProjectile), nameof(LocalGeneralProjectile.TickSkillLogic))] //225f 212f
        public static IEnumerable<CodeInstruction> Fix39000(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions)
                .MatchForward(
                    true,
                    new CodeMatch(i =>
                    {
                        return (i.opcode == Ldc_R4) &&
                               (
                                   Math.Abs(Convert.ToDouble(i.operand ?? 0.0) - 39006.25) < 1
                               );
                    })
                );
            if (matcher.IsInvalid)
            {
                GS2.Warn("Transpiler LocalGeneralProjectile.TickSkillLogic fail!\nCan't find constant 39006.25");
                return instructions;
            }
            matcher.Repeat(matcher =>
            {
                matcher.InsertAndAdvance(new CodeInstruction(Ldarg_0));
                matcher.InsertAndAdvance(new CodeInstruction(Utils.LoadField(typeof(LocalGeneralProjectile),
                    nameof(LocalGeneralProjectile.astroId))));
                matcher.SetInstruction(new CodeInstruction(Call, matcher.GetSquareRadiusFromAstroFactoryId()));
            });

            return matcher.InstructionEnumeration();
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), nameof(SkillSystem.GetObjectUPositionAndVelocity),
            new[] { typeof(SkillTarget), typeof(VectorLF3), typeof(Vector3) },
            new[] { ArgumentType.Ref, ArgumentType.Out, ArgumentType.Out })]
        public static bool GetObjectUPositionAndVelocity_Prefix(
            SkillSystem __instance,
            ref SkillTarget obj,
            out VectorLF3 upos,
            out Vector3 uvel,
            ref bool __result)
        {
            upos = default;
            uvel = default;
            __result = false;

            if (!SpaceSector.IsPlanetAstro(obj.astroId) || !UsesPlanetFactory(obj.type))
                return true;

            var factories = __instance?.astroFactories;
            var astroId = obj.astroId;
            if (factories == null || astroId < 0 || astroId >= factories.Length)
                return false;

            var factory = factories[astroId];
            if (factory == null)
                return false;

            var sector = __instance.sector;
            var galaxyAstros = sector?.galaxyAstros;
            if (galaxyAstros == null || astroId >= galaxyAstros.Length)
                return false;

            var astro = galaxyAstros[astroId];
            switch (obj.type)
            {
                case ETargetType.Enemy:
                {
                    var pool = factory.enemyPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref EnemyData enemy = ref pool[obj.id];
                    astro.PositionU(ref enemy.pos, out upos);
                    astro.VelocityL2U(ref enemy.pos, ref enemy.vel, out uvel);
                    break;
                }
                case ETargetType.Craft:
                {
                    var pool = factory.craftPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref CraftData craft = ref pool[obj.id];
                    astro.PositionU(ref craft.pos, out upos);
                    astro.VelocityL2U(ref craft.pos, ref craft.vel, out uvel);
                    break;
                }
                case ETargetType.None:
                {
                    var pool = factory.entityPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref EntityData entity = ref pool[obj.id];
                    astro.PositionU(ref entity.pos, out upos);
                    astro.VelocityU(ref entity.pos, out uvel);
                    break;
                }
                case ETargetType.Vegetable:
                {
                    var pool = factory.vegePool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref VegeData vegetable = ref pool[obj.id];
                    astro.PositionU(ref vegetable.pos, out upos);
                    astro.VelocityU(ref vegetable.pos, out uvel);
                    break;
                }
                case ETargetType.Vein:
                {
                    var pool = factory.veinPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref VeinData vein = ref pool[obj.id];
                    astro.PositionU(ref vein.pos, out upos);
                    astro.VelocityU(ref vein.pos, out uvel);
                    break;
                }
                case ETargetType.Prebuild:
                {
                    var pool = factory.prebuildPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref PrebuildData prebuild = ref pool[obj.id];
                    astro.PositionU(ref prebuild.pos, out upos);
                    astro.VelocityU(ref prebuild.pos, out uvel);
                    break;
                }
                case ETargetType.Ruin:
                {
                    var pool = factory.ruinPool;
                    if (pool == null || obj.id < 0 || obj.id >= pool.Length || pool[obj.id].id == 0)
                        return false;
                    ref RuinData ruin = ref pool[obj.id];
                    astro.PositionU(ref ruin.pos, out upos);
                    astro.VelocityU(ref ruin.pos, out uvel);
                    break;
                }
                default:
                    return true;
            }

            __result = true;
            return false;
        }

        private static bool UsesPlanetFactory(ETargetType type)
        {
            return type == ETargetType.Enemy || type == ETargetType.Craft ||
                   type == ETargetType.None || type == ETargetType.Vegetable ||
                   type == ETargetType.Vein || type == ETargetType.Prebuild ||
                   type == ETargetType.Ruin;
        }
    }
}
