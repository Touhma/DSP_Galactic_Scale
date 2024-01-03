using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public static partial class PatchOnEnemyDFGroundSystem
    {
        const float HOVER_HEIGHT = 2f;
        private static bool ready;
        private static float radius;
        private static double radius2;
        private static EnemyData[] pool;

        [HarmonyPostfix, HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.GameTickLogic))]
        public static void GameTick(ref EnemyDFGroundSystem __instance)
        {
            if (__instance.planet == null || __instance.planet != GameMain.localPlanet)
            {
                ready = false;
                return;
            }

            if (ready)
            {
                for (var i = 0; i < __instance.factory.enemyCursor; i++)
                {
                    var e = pool[i];
                    if (e.id == 0)
                    {
                        continue;
                    }

                    // count++;
                    var pos = e.pos;
                    var mag = pos.sqrMagnitude;
                    if (mag == 0 || !(mag < radius2))
                    {
                        continue;
                    }

                    pool[i].pos = pos.normalized * (radius);

                    // GS2.Log($"Caught one at {mag}, now {pool[i].pos.magnitude} Unit id:{e.unitId} Id:{e.id} {__instance.units.cursor}/{pool.Length}:{i}");
                }

                return;
            }

            radius = __instance.planet.realRadius;
            radius += HOVER_HEIGHT;
            radius2 = radius * radius;
            if (__instance.factory?.enemySystem == null) return;
            if (__instance.units == null) return;
            if (__instance.factory.enemyPool == null) return;
            pool = __instance.factory.enemyPool;
            ready = true;
        }
    }
}