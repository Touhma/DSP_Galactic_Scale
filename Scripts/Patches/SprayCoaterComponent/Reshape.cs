using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnSprayCoaterComponent
    {
        private const float BeltEndDistance = 2.4f;
        private const float BendUnitsPerMeter = 200f;
        private const float BendPackingStride = 400f;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.GetReshapeData))]
        public static bool GetReshapeData(Vector3 _pos, Quaternion _rot, ref Vector3 __result)
        {
            var planet = GameMain.localPlanet;
            if (planet == null) return true;

            __result = GetGridReshapeData(_pos.normalized, _rot.Forward(), planet);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), nameof(SpraycoaterComponent.Reshape))]
        public static bool Reshape(SpraycoaterComponent __instance, PlanetFactory factory, AnimData[] _animPool)
        {
            var entity = factory.entityPool[__instance.entityId];
            var normal = entity.pos.normalized;
            if (IsAtPole(normal)) return false;

            var prefab = LDB.models.Select(entity.modelIndex)?.prefabDesc;
            if (prefab == null) return false;

            var radius = factory.planet.realRadius;
            var position = normal * radius;
            var forward = entity.rot.Forward();
            var bend = GetGridReshapeData(normal, forward, factory.planet);
            var poses = prefab.addonAreaColPoses;

            var incBelt = GetBeltEntityId(factory, __instance.incBeltId);
            if (incBelt != 0)
            {
                var pose = poses[1];
                var center = (position + entity.rot * pose.position).normalized * radius;
                var reach = pose.forward * (prefab.addonAreaSize[1].z * 2.5f);
                var start = (position + entity.rot * (pose.position + reach)).normalized * radius;
                var end = (position + entity.rot * (pose.position - reach)).normalized * radius;
                var beltPosition = factory.entityPool[incBelt].pos.normalized * radius;
                var side = Mathf.Sign(Vector3.Dot((beltPosition - center).normalized, forward));
                bend.z = -side * Maths.DistancePointLine(beltPosition, start, end)
                    * Mathf.Abs(GetBeltAlignment(normal, forward));
            }

            var cargoBelt = GetBeltEntityId(factory, __instance.cargoBeltId);
            if (cargoBelt != 0)
            {
                var pose = poses[0];
                var center = (position + entity.rot * pose.position).normalized * radius;
                var front = (position + entity.rot * (pose.position + pose.forward * BeltEndDistance)).normalized * radius;
                var back = (position + entity.rot * (pose.position - pose.forward * (BeltEndDistance + bend.z))).normalized * radius;
                var right = Vector3.Cross(forward, normal);

                for (var slot = 0; slot < 2; slot++)
                {
                    factory.ReadObjectConn(cargoBelt, slot, out _, out var neighbor, out _);
                    if (!TryGetObjectPosition(factory, neighbor, out var neighborPosition)) continue;

                    var direction = (neighborPosition.normalized * radius - center).normalized;
                    var isFront = Vector3.Dot(direction, forward) >= 0f;
                    factory.ReadObjectConn(neighbor, slot, out _, out var next, out _);
                    if (TryGetObjectPosition(factory, next, out var nextPosition))
                        direction = (nextPosition.normalized * radius - center).normalized;

                    var side = -Mathf.Sign(Vector3.Dot(direction, right));
                    var offset = direction * 5f;
                    var distance = Maths.DistancePointLine(isFront ? front : back, center + offset, center - offset) * side;
                    if (isFront) bend.x = distance;
                    else bend.y = distance;
                }
            }

            ref var animation = ref _animPool[__instance.entityId];
            animation.working_length = -1.265f - bend.z;
            animation.prepare_length = Mathf.Round(bend.y * BendUnitsPerMeter)
                + Mathf.Round(bend.x * BendUnitsPerMeter) * BendPackingStride;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.Import))]
        public static void AfterFactoryImport(PlanetFactory __instance)
        {
            RepairExistingCoaters(__instance);
        }

        public static void RepairExistingCoaters(PlanetFactory factory)
        {
            var planet = factory?.planet;
            var traffic = factory?.cargoTraffic;
            if (planet == null || traffic?.spraycoaterPool == null) return;
            if (planet.realRadius == 200f && GetGridSegmentCount(planet) == 200) return;

            var coaters = traffic.spraycoaterPool;
            for (var id = 1; id < traffic.spraycoaterCursor; id++)
            {
                ref var coater = ref coaters[id];
                if (coater.id != id || coater.entityId <= 0 || coater.entityId >= factory.entityCursor) continue;
                ref var entity = ref factory.entityPool[coater.entityId];
                if (entity.id != coater.entityId || entity.spraycoaterId != id) continue;

                coater.Reshape(factory, factory.entityAnimPool);
            }
        }

        private static int GetBeltEntityId(PlanetFactory factory, int beltId)
        {
            var belts = factory.cargoTraffic.beltPool;
            if (beltId <= 0 || beltId >= belts.Length || belts[beltId].id != beltId) return 0;

            var entityId = belts[beltId].entityId;
            return entityId > 0 && TryGetObjectPosition(factory, entityId, out _) ? entityId : 0;
        }

        private static bool TryGetObjectPosition(PlanetFactory factory, int objectId, out Vector3 position)
        {
            if (objectId > 0 && objectId < factory.entityPool.Length && factory.entityPool[objectId].id == objectId)
            {
                position = factory.entityPool[objectId].pos;
                return true;
            }

            var prebuildId = -objectId;
            if (objectId < 0 && prebuildId > 0 && prebuildId < factory.prebuildPool.Length &&
                factory.prebuildPool[prebuildId].id == prebuildId)
            {
                position = factory.prebuildPool[prebuildId].pos;
                return true;
            }

            position = default;
            return false;
        }

        private static Vector3 GetGridReshapeData(Vector3 normal, Vector3 forward, PlanetData planet)
        {
            if (IsAtPole(normal)) return Vector3.zero;

            var segmentCount = BlueprintUtils.GetLongitudeSegmentCount(normal, GetGridSegmentCount(planet)) * 5;
            if (segmentCount <= 0) return Vector3.zero;

            var alignment = GetBeltAlignment(normal, forward);
            var circleRadius = Mathf.Sqrt(normal.x * normal.x + normal.z * normal.z) * planet.realRadius;
            var gridWidth = circleRadius * (2f * Mathf.PI) / segmentCount;
            var gridWidthDelta = (gridWidth - 2f * Mathf.PI * 200f / 1000f) * Mathf.Abs(alignment);
            var bendPerMeter = -Mathf.Sin(2f * Mathf.PI / segmentCount * Mathf.PI * 0.25f) * alignment;
            return new Vector3(bendPerMeter * BeltEndDistance, bendPerMeter * (BeltEndDistance + gridWidthDelta), gridWidthDelta);
        }

        private static int GetGridSegmentCount(PlanetData planet)
        {
            return planet.aux?.mainGrid?.segment ?? (int)(planet.radius / 4f + 0.1f) * 4;
        }

        private static bool IsAtPole(Vector3 normal)
        {
            return normal.x * normal.x + normal.z * normal.z <= 1E-06f;
        }

        private static float GetBeltAlignment(Vector3 normal, Vector3 forward)
        {
            var pole = normal.y > 0f ? Vector3.up : Vector3.down;
            return Vector3.Dot(Vector3.Cross(normal, (pole - normal).normalized), forward);
        }
    }
}
