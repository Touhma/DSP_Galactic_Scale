using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnGuideMissionStandardMode
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuideMissionStandardMode), nameof(GuideMissionStandardMode.Skip))]
        public static bool GuideMissionStandardMode_Skip_Prefix(GameData _gameData, ref GuideMissionStandardMode __instance)
        {
            if (GS3.IsMenuDemo) return true;
            if (NebulaCompat.IsClient) return false; // skip SpacePod if it is client joining
            if (GS3.Failed) return false;

            GS3.Log("Checking gameData... " + (_gameData == null ? "Null" : "Exists"));
            if (_gameData == null) return GS3.AbortGameStart("An error occured during game creation, resulting in no game data being created");

            __instance.gameData = _gameData;
            GS3.Log("Checking mainPlayer... " + (__instance.gameData.mainPlayer == null ? "Null" : "Exists"));
            if (__instance.gameData.mainPlayer == null) return GS3.AbortGameStart("An error occured during game creation, resulting in no player character being created");

            __instance.player = __instance.gameData.mainPlayer;

            GS3.Log("Checking controller... " + (__instance.player.controller == null ? "Null" : "Exists"));
            __instance.controller = __instance.player.controller;
            if (__instance.player.controller == null) return GS3.AbortGameStart("Player controller failed to initialize. Probably an issue with galaxy generation.");

            GS3.Log("Checking localPlanet... " + (__instance.gameData.localPlanet == null ? "Null" : "Exists"));
            if (__instance.gameData.localPlanet == null)
            {
                if (GameMain.localPlanet != null)
                {
                    __instance.gameData.localPlanet = GameMain.localPlanet;
                }
                else
                {
                    __instance.gameData.ArrivePlanet(__instance.gameData.galaxy.PlanetById(__instance.gameData.galaxy.birthPlanetId));
                    if (__instance.gameData.localPlanet == null) return GS3.AbortGameStart("Unable to find a habitable starting planet. If loading from a custom JSON, please check it for errors with an online tool.");
                }
            }

            __instance.localPlanet = __instance.gameData.localPlanet;
            GS3.Log("Checking birthPoint... " + (__instance.localPlanet.birthPoint == null ? "Null" : "Exists"));
            __instance.targetPos = __instance.localPlanet.birthPoint;
            __instance.targetUPos = __instance.localPlanet.uPosition + (VectorLF3)(__instance.localPlanet.runtimeRotation * __instance.targetPos);
            __instance.targetRot = Maths.SphericalRotation(__instance.localPlanet.birthPoint, 0.0f);
            __instance.targetURot = __instance.localPlanet.runtimeRotation * __instance.targetRot;
            if (__instance.localPlanet.factory != null)
            {
                __instance.localPlanet.factory.FlattenTerrain(__instance.targetPos, __instance.targetRot, new Bounds(Vector3.zero, new Vector3(10f, 5f, 10f)), removeVein: true, lift: true);
                GS3.Log("Waking in SpacePod");
                __instance.CreateSpaceCapsuleVegetable();
                GS3.Log("Searching for landing place");
                __instance.gameData.InitLandingPlace();
            }
            Utils.LogDFInfo(GameMain.localStar);
            __instance.player.controller.memCameraTargetRot = __instance.targetRot;
            __instance.player.cameraTarget.rotation = __instance.targetRot;
            // if (GS3.Config.CheatMode && !GS3.ResearchUnlocked)
            // {
            //     GS3.Warn("Cheatmode Enabled. Unlocking Research");
            //     GS3.UnlockTech(null);
            // }

            return false;
        }
    }
}