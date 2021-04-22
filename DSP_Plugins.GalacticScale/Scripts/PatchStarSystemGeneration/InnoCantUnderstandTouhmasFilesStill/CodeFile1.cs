using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    public static class PatchGuideMissionStandardMode
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GuideMissionStandardMode), "Skip")]
        public static bool Skip(GameData _gameData, ref GuideMissionStandardMode __instance)
        {
            Patch.Debug("Skip");
            __instance.gameData = _gameData;
            Patch.Debug("Skip1");
            __instance.player = __instance.gameData.mainPlayer; Patch.Debug("Skip2");
            __instance.controller = __instance.player.controller; Patch.Debug("Skip3");
            __instance.localPlanet = __instance.gameData.localPlanet; Patch.Debug("Skip4");
            Patch.Debug("__instance.gameData.localPlanet" + __instance.gameData.localPlanet == null);Patch.Debug("heh");
            Patch.Debug("hoh" + __instance.localPlanet.birthPoint == null);
            __instance.targetPos = __instance.localPlanet.birthPoint; Patch.Debug("Skip5");
            __instance.targetUPos = __instance.localPlanet.uPosition + (VectorLF3)(__instance.localPlanet.runtimeRotation * __instance.targetPos); Patch.Debug("Skip6");
            __instance.targetRot = Maths.SphericalRotation(__instance.localPlanet.birthPoint, 0.0f); Patch.Debug("Skip7");
            __instance.targetURot = __instance.localPlanet.runtimeRotation * __instance.targetRot; Patch.Debug("Skip8");
            __instance.localPlanet.factory.FlattenTerrain(__instance.targetPos, __instance.targetRot, new Bounds(Vector3.zero, new Vector3(10f, 5f, 10f)), removeVein: true, lift: true); Patch.Debug("Skip9");
            __instance.CreateSpaceCapsuleVegetable(); Patch.Debug("Skip10");
            __instance.gameData.InitLandingPlace(); Patch.Debug("Skip11");
            __instance.player.controller.memCameraTargetRot = __instance.targetRot; Patch.Debug("Skip12");
            __instance.player.cameraTarget.rotation = __instance.targetRot; Patch.Debug("Skip13");
            return false;
        }
    }
}