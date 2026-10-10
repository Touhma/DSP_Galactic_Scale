using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.InternalTickRemote))]
    public static class RemoteShipStuckRecovery
    {
        private const int StuckTimeoutTicks = 1800;
        private const int EscapeDurationTicks = 300; 
        private const int RecoveryCooldownTicks = 1800;
        private const double ProgressDistance = 250.0;
        private const float EscapeSpeedMultiplier = 2.0f;

        private sealed class ShipProgress
        {
            public double ClosestDistance;
            public int LastProgressTick;
            public int ObservedTicks;
            public int EscapeUntilTick;
            public int CooldownUntilTick;
        }

        private static readonly Dictionary<long, ShipProgress> ProgressByShip = new Dictionary<long, ShipProgress>();
        [HarmonyPostfix]
        public static void RecoverStuckReturningShips(StationComponent __instance, AstroData[] astroPoses, float shipSailSpeed)
        {
            if (__instance.workShipDatas == null || __instance.shipDiskPos == null || __instance.planetId <= 0 ||
                __instance.planetId >= astroPoses.Length)
            {
                return;
            }

            AstroData home = astroPoses[__instance.planetId];
            for (int i = 0; i < __instance.workShipCount; i++)
            {
                ref ShipData ship = ref __instance.workShipDatas[i];
                if (ship.direction >= 0 || ship.stage != 0 || ship.shipIndex < 0 || ship.shipIndex >= __instance.shipDiskPos.Length)
                {
                    if (ship.shipIndex >= 0)
                    {
                        long staleKey = ((long)__instance.gid << 32) | (uint)ship.shipIndex;
                        ProgressByShip.Remove(staleKey);
                    }
                    continue;
                }

                long key = ((long)__instance.gid << 32) | (uint)ship.shipIndex;
                Vector3 dockLocal = __instance.shipDiskPos[ship.shipIndex];
                float dockLength = dockLocal.magnitude;
                if (dockLength > 0f)
                    dockLocal *= 1f + 25f / dockLength;
                StationComponent.lpos2upos_out(ref home.uPos, ref home.uRot, ref dockLocal, out VectorLF3 dockPosition);

                VectorLF3 offset = dockPosition - ship.uPos;
                double distance = offset.magnitude;
                if (distance < 1.0)
                {
                    ProgressByShip.Remove(key);
                    continue;
                }

                if (!ProgressByShip.TryGetValue(key, out ShipProgress progress))
                {
                    progress = new ShipProgress { ClosestDistance = distance, LastProgressTick = 0 };
                    ProgressByShip.Add(key, progress);
                }
                progress.ObservedTicks++;
                if (distance < progress.ClosestDistance - ProgressDistance)
                {
                    progress.ClosestDistance = distance;
                    progress.LastProgressTick = progress.ObservedTicks;
                }

                if (progress.ObservedTicks < progress.CooldownUntilTick)
                    continue;

                bool stuck = progress.ObservedTicks - progress.LastProgressTick >= StuckTimeoutTicks;
                if (stuck)
                {
                    progress.EscapeUntilTick = progress.ObservedTicks + EscapeDurationTicks;
                    progress.CooldownUntilTick = progress.ObservedTicks + RecoveryCooldownTicks;
                    progress.LastProgressTick = progress.ObservedTicks;
                    progress.ClosestDistance = distance;
                    GS2.Warn($"ILS vessel {ship.shipIndex} at station {__instance.gid} was stuck returning. Applying a temporary direct-to-station recovery heading.");
                }

                if (progress.ObservedTicks <= progress.EscapeUntilTick)
                {
                    VectorLF3 direction = offset / distance;
                    Vector3 forward = new Vector3((float)direction.x, (float)direction.y, (float)direction.z).normalized;
                    if (forward.sqrMagnitude > 0.0001f)
                    {
                        ship.uVel = forward;
                        ship.uRot = Quaternion.LookRotation(forward, Vector3.up);
                        ship.uSpeed = Mathf.Max(ship.uSpeed, shipSailSpeed * EscapeSpeedMultiplier);
                    }
                }
            }
        }
    }
}
