using System.Collections.Generic;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnEnemyDFGroundSystem
    {
        const float HOVER_HEIGHT = 2f;
        private static Dictionary<EnemyDFGroundSystem, bool> ready = new();
        private static Dictionary<EnemyDFGroundSystem, (float radius, double radius2)> data = new();

        [HarmonyPostfix, HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.GameTickLogic))]
        public static void GameTick(ref EnemyDFGroundSystem __instance)
        {
            if (!ready.ContainsKey(__instance)) ready.Add(__instance, false);
            if (__instance.planet == null || __instance.planet != GameMain.localPlanet)
            {
                // Log($"Turning off EnemyDFGroundSystem patch for planet:{__instance.planet?.name} {__instance.planet == null} {GameMain.localPlanet?.name} {__instance.planet != GameMain.localPlanet}");
                ready[__instance] = false;
                return;
            }

            if (ready[__instance])
            {
                for (var i = 0; i < __instance.factory.enemyCursor; i++)
                {
                    var e = __instance.factory.enemyPool[i];
                    if (e.id == 0)
                    {
                        continue;
                    }

                    // count++;
                    var pos = e.pos;
                    var mag = pos.sqrMagnitude;
                    if (mag == 0 || !(mag < data[__instance].radius2))
                    {
                        continue;
                    }

                    __instance.factory.enemyPool[i].pos = pos.normalized * (data[__instance].radius);

                    // Log($"Caught one at {mag}, now {pool[i].pos.magnitude} Unit id:{e.unitId} Id:{e.id} {__instance.units.cursor}/{pool.Length}:{i}");
                }

                return;
            }
            // Log("Turning ON EnemyDFGroundSystem patch for planet:" + __instance.planet?.name);

            if (!data.ContainsKey(__instance))
            {
                var radius = __instance.planet.realRadius;
                radius += HOVER_HEIGHT;
                data.Add(__instance, (radius, radius * radius));
            }

            if (__instance.factory?.enemySystem == null) return;
            if (__instance.units == null) return;
            if (__instance.factory.enemyPool == null) return;
            ready[__instance] = true;
        }
    }
}