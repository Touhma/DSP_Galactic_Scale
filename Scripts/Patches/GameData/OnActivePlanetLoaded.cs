using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        private const int PostFastTravelRecoveryTicks = 180;
        private const int SurfaceValidationIntervalTicks = 30;
        private static int postFastTravelRecoveryTicks;
        private static int surfaceValidationTicks;
        private static readonly HashSet<PlanetData> reportedSurfaceIssuePlanets = new();

        public static bool IsNaNRecoveryActive { get; private set; }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "OnActivePlanetLoaded")]
        public static void OnActivePlanetLoaded(PlanetData planet)
        {
            //GS2.Warn($"{planet.name}");
            if (!GS2.Vanilla)
            {
                RestoreLocalPlanetSurface(planet, true);
                TryRecoverInvalidPlayerPosition(planet);

                var segments = (int)(planet.radius / 4f + 0.1f) * 4;
                if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) GS2.SetLuts(segments, planet.radius);
                PatchOnUIBuildingGrid.refreshGridRadius = Mathf.RoundToInt(planet.radius);
            }
        }

        private static void RestoreLocalPlanetSurface(PlanetData planet, bool reportHealthy = false)
        {
            if (planet == null || planet != GameMain.localPlanet || !planet.loaded || planet.type == EPlanetType.Gas) return;

            try
            {
                var planetObject = planet.gameObject;
                var bodyObject = planet.bodyObject;
                if (planetObject == null || bodyObject == null)
                {
                    ReportSurfaceIssue(planet, $"model root/body missing; loaded={planet.loaded}, loading={planet.loading}, rootNull={planetObject == null}, bodyNull={bodyObject == null}");
                    return;
                }

                var repaired = 0;
                var rendererCount = 0;
                var enabledRenderers = 0;
                var activeRenderers = 0;
                var missingRenderers = 0;
                var missingMeshes = 0;
                var missingMaterials = 0;
                var cameraHiddenRenderers = 0;
                if (!planetObject.activeSelf)
                {
                    planetObject.SetActive(true);
                    repaired++;
                }
                if (!bodyObject.activeSelf)
                {
                    bodyObject.SetActive(true);
                    repaired++;
                }

                var meshes = planet.meshes;
                var renderers = planet.meshRenderers;
                var colliders = planet.meshColliders;
                var meshFilters = (MeshFilter[])null;
                var camera = Camera.main;
                var simulator = planetObject.GetComponent<PlanetSimulator>();
                var count = Math.Max(meshes?.Length ?? 0, Math.Max(renderers?.Length ?? 0, colliders?.Length ?? 0));

                for (var i = 0; i < count; i++)
                {
                    var mesh = meshes != null && i < meshes.Length ? meshes[i] : null;
                    var collider = colliders != null && i < colliders.Length ? colliders[i] : null;
                    var renderer = renderers != null && i < renderers.Length ? renderers[i] : null;
                    mesh ??= collider == null ? null : collider.sharedMesh;

                    if (renderer == null && collider != null)
                        renderer = collider.GetComponent<MeshRenderer>();

                    if (renderer == null && mesh != null)
                    {
                        meshFilters ??= bodyObject.GetComponentsInChildren<MeshFilter>(true);
                        foreach (var meshFilter in meshFilters)
                        {
                            if (meshFilter == null || meshFilter.sharedMesh != mesh) continue;
                            renderer = meshFilter.GetComponent<MeshRenderer>();
                            if (renderer == null) renderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
                            break;
                        }
                    }

                    if (renderer == null)
                    {
                        if (mesh != null || collider != null) missingRenderers++;
                        continue;
                    }

                    if (renderers != null && i < renderers.Length && renderers[i] == null)
                    {
                        renderers[i] = renderer;
                        repaired++;
                    }
                    if (!renderer.enabled)
                    {
                        renderer.enabled = true;
                        repaired++;
                    }
                    if (!renderer.gameObject.activeSelf)
                    {
                        renderer.gameObject.SetActive(true);
                        repaired++;
                    }

                    var filter = renderer.GetComponent<MeshFilter>();
                    if (collider == null) collider = renderer.GetComponent<MeshCollider>();
                    mesh ??= collider == null ? null : collider.sharedMesh;
                    mesh ??= filter == null ? null : filter.sharedMesh;
                    if (meshes != null && i < meshes.Length && meshes[i] == null && mesh != null)
                    {
                        meshes[i] = mesh;
                        repaired++;
                    }
                    if (filter == null && mesh != null) filter = renderer.gameObject.AddComponent<MeshFilter>();
                    if (filter != null && filter.sharedMesh != mesh && mesh != null)
                    {
                        filter.sharedMesh = mesh;
                        repaired++;
                    }
                    if (colliders != null && i < colliders.Length && colliders[i] == null && collider != null)
                    {
                        colliders[i] = collider;
                        repaired++;
                    }
                    if (collider != null && collider.sharedMesh != mesh && mesh != null)
                    {
                        collider.sharedMesh = mesh;
                        repaired++;
                    }
                    if (renderer.sharedMaterial == null && planet.terrainMaterial != null)
                    {
                        renderer.sharedMaterial = planet.terrainMaterial;
                        repaired++;
                    }
                    if (filter == null || filter.sharedMesh == null) missingMeshes++;
                    if (renderer.sharedMaterial == null) missingMaterials++;
                    rendererCount++;
                    if (renderer.enabled) enabledRenderers++;
                    if (renderer.gameObject.activeInHierarchy) activeRenderers++;

                    if (simulator != null)
                    {
                        if (simulator.surfaceRenderer != null && i < simulator.surfaceRenderer.Length && simulator.surfaceRenderer[i] == null)
                        {
                            simulator.surfaceRenderer[i] = renderer;
                            repaired++;
                        }
                        if (simulator.surfaceCollider != null && i < simulator.surfaceCollider.Length && simulator.surfaceCollider[i] == null && collider != null)
                        {
                            simulator.surfaceCollider[i] = collider;
                            repaired++;
                        }
                    }

                    if (camera != null && (camera.cullingMask & (1 << renderer.gameObject.layer)) == 0)
                        cameraHiddenRenderers++;
                }

                if (rendererCount == 0 || missingRenderers > 0 || missingMeshes > 0 || missingMaterials > 0 || cameraHiddenRenderers > 0 || !planetObject.activeInHierarchy || !bodyObject.activeInHierarchy)
                {
                    ReportSurfaceIssue(planet, $"render state remains incomplete; repairs={repaired}, renderers={rendererCount}, missingRenderers={missingRenderers}, missingMeshes={missingMeshes}, missingMaterials={missingMaterials}, cameraHiddenRenderers={cameraHiddenRenderers}, meshSlots={count}, rootActive={planetObject.activeInHierarchy}, bodyActive={bodyObject.activeInHierarchy}, factoryLoaded={planet.factoryLoaded}");
                }
                else
                {
                    reportedSurfaceIssuePlanets.Remove(planet);
                    if (repaired > 0)
                        GS2.Warn($"Restored local planet surface render state for {planet.name}: repairs={repaired}, meshSlots={count}, factoryLoaded={planet.factoryLoaded}.");
                    else if (reportHealthy)
                        GS2.Log($"Validated local planet surface for {planet.name}: renderers={rendererCount}, enabled={enabledRenderers}, active={activeRenderers}, meshSlots={count}, cameraMask=0x{camera?.cullingMask:X8}, factoryLoaded={planet.factoryLoaded}.");
                }
            }
            catch (Exception exception)
            {
                ReportSurfaceIssue(planet, $"surface validation/repair threw {exception}");
            }
        }

        private static void ReportSurfaceIssue(PlanetData planet, string details)
        {
            if (planet != null && !reportedSurfaceIssuePlanets.Add(planet)) return;
            GS2.Warn($"Local planet surface issue on {planet?.name}: {details}.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "GameTick")]
        public static void PlayerControllerGameTick()
        {
            var player = GameMain.mainPlayer;
            if (player == null || player.controller?.actionSail == null)
            {
                postFastTravelRecoveryTicks = 0;
                surfaceValidationTicks = 0;
                IsNaNRecoveryActive = false;
                return;
            }

            var localPlanet = GameMain.localPlanet;
            if (localPlanet == null || !localPlanet.loaded || player.movementState == EMovementState.Fly)
            {
                surfaceValidationTicks = 0;
            }
            else if (++surfaceValidationTicks >= SurfaceValidationIntervalTicks)
            {
                surfaceValidationTicks = 0;
                RestoreLocalPlanetSurface(localPlanet);
            }

            var fastTravelling = player.controller.actionSail.fastTravelling;
            if (fastTravelling)
                postFastTravelRecoveryTicks = PostFastTravelRecoveryTicks;
            else if (postFastTravelRecoveryTicks > 0)
                postFastTravelRecoveryTicks--;

            IsNaNRecoveryActive = fastTravelling || postFastTravelRecoveryTicks > 0;
            if (!IsNaNRecoveryActive || GameMain.localPlanet == null) return;

            if (player.movementState != EMovementState.Fly)
                TryRecoverInvalidPlayerPosition(GameMain.localPlanet);
        }

        public static bool TryRecoverInvalidPlayerPosition(PlanetData planet)
        {
            var player = GameMain.mainPlayer;
            if (planet == null || player == null || !IsFinite(planet.uPosition)) return false;

            var planetRadius = planet.realRadius;
            if (float.IsNaN(planetRadius) || float.IsInfinity(planetRadius) || planetRadius <= 0f) return false;

            var localPosition = player.position;
            var localDistanceSquared = localPosition.sqrMagnitude;
            var invalidLocalPosition = !IsFinite(localPosition) || float.IsNaN(localDistanceSquared) || float.IsInfinity(localDistanceSquared);
            var transform = player.transform;
            var invalidTransformPosition = transform == null || !IsFinite(transform.localPosition);
            var invalidWorldTransformPosition = transform == null || !IsFinite(transform.position);
            var invalidTransformRotation = transform == null || !IsUsableRotation(transform.localRotation);
            var invalidControllerVelocity = player.controller != null && !IsFinite(player.controller.velocity);
            var offset = player.uPosition - planet.uPosition;
            var distanceSquared = SqrMagnitude(offset);
            var invalidPosition = !IsFinite(player.uPosition) || double.IsNaN(distanceSquared) || double.IsInfinity(distanceSquared) ||
                                  invalidLocalPosition || invalidTransformPosition || invalidWorldTransformPosition ||
                                  invalidTransformRotation || !IsUsableRotation(player.uRotation) || invalidControllerVelocity;
            var recoveryThreshold = Mathf.Max(5f, planetRadius * 0.001f);
            var safeRadius = Mathf.Max(0f, planetRadius - recoveryThreshold);
            var safeRadiusSquared = (double)safeRadius * safeRadius;
            var insidePlanet = !invalidPosition &&
                               distanceSquared < safeRadiusSquared;

            if (invalidPosition || insidePlanet)
            {
                var localDistance = Mathf.Sqrt(Mathf.Max(0f, localDistanceSquared));
                var distance = Math.Sqrt(Math.Max(0.0, distanceSquared));
                Vector3 localOutward = Vector3.zero;
                if (!invalidLocalPosition && localDistanceSquared > 0.000001f)
                {
                    localOutward = localPosition.normalized;
                }
                else if (!double.IsNaN(distanceSquared) && !double.IsInfinity(distanceSquared) && distanceSquared > 0.000001)
                {
                    localOutward = (Vector3)Maths.QInvRotateLF(planet.runtimeRotation, offset.normalized);
                }
                else if (planet.star != null && IsFinite(planet.star.uPosition))
                {
                    var starRelativeWorldDirection = (planet.uPosition - planet.star.uPosition).normalized;
                    localOutward = (Vector3)Maths.QInvRotateLF(planet.runtimeRotation, starRelativeWorldDirection);
                }

                if (!IsFinite(localOutward) || localOutward.sqrMagnitude < 0.5f) localOutward = Vector3.forward;
                localOutward.Normalize();
                var worldOutward = (VectorLF3)(planet.runtimeRotation * localOutward).normalized;

                var surfaceDistance = planetRadius + 10.0;
                var surfaceOffset = worldOutward * surfaceDistance;
                player.position = localOutward * (float)surfaceDistance;
                player.uPosition = planet.uPosition + surfaceOffset;
                player.uVelocity = VectorLF3.zero;
                player.uRotation = Quaternion.identity;
                player.planetId = planet.id;
                if (player.controller != null)
                {
                    player.controller.velocity = Vector3.zero;
                    if (player.controller.actionFly != null)
                    {
                        player.controller.actionFly.moveVelocity = Vector3.zero;
                        player.controller.actionFly.rtsVelocity = Vector3.zero;
                    }
                    player.controller.ResetTransRotation();
                }
                var recoveryReason = invalidPosition ? "position or transform was invalid" : "inside the planet";
                GS2.Warn($"Recovered invalid player arrival position at {planet.name}; player was {recoveryReason}. " +
                    $"Local position before recovery: {localPosition} (magnitude {localDistance:0.##}); world distance from planet center: {distance:0.##}. " +
                    $"Placed at radius {surfaceDistance:0.##}; transform local position is {player.transform.localPosition}, rotation is {player.transform.localRotation}.");
                return true;
            }
            else if (!IsFinite(player.uVelocity))
            {
                player.uVelocity = VectorLF3.zero;
                GS2.Warn($"Reset non-finite player velocity after arriving at {planet.name}.");
                return true;
            }

            return false;
        }

        private static bool IsFinite(VectorLF3 vector)
        {
            return IsFinite(vector.x) && IsFinite(vector.y) && IsFinite(vector.z);
        }

        private static double SqrMagnitude(VectorLF3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }

        private static bool IsFinite(Vector3 vector)
        {
            return IsFinite(vector.x) && IsFinite(vector.y) && IsFinite(vector.z);
        }

        private static bool IsFinite(Quaternion rotation)
        {
            return IsFinite(rotation.x) && IsFinite(rotation.y) && IsFinite(rotation.z) && IsFinite(rotation.w);
        }

        private static bool IsUsableRotation(Quaternion rotation)
        {
            return IsFinite(rotation) && rotation.x * rotation.x + rotation.y * rotation.y +
                   rotation.z * rotation.z + rotation.w * rotation.w > 0.25f;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
