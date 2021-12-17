using HarmonyLib;
using UnityEngine; // using NebulaAPI;

namespace GalacticScale
{
    public partial class PatchOnGuideMissionStandardMode
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuideMissionStandardMode), "Skip")]
        public static bool GS2_GuideMissionStandardMode_Skip_Prefix(GameData _gameData, ref GuideMissionStandardMode __instance)
        {
            //if (GS2.Vanilla) return true;
            if (GS2.IsMenuDemo) return true;
            //GS2.Warn(NebulaCompatibility.IsMasterClient.ToString());
            if (GS2.NebulaClient) return false;
            if (GS2.Failed) return false;

            GS2.Log("Checking gameData... " + (_gameData == null ? "Null" : "Exists"));
            if (_gameData == null) return GS2.AbortGameStart("An error occured during game creation, resulting in no game data being created");

            __instance.gameData = _gameData;
            GS2.Log("Checking mainPlayer... " + (__instance.gameData.mainPlayer == null ? "Null" : "Exists"));
            if (__instance.gameData.mainPlayer == null) return GS2.AbortGameStart("An error occured during game creation, resulting in no player character being created");

            __instance.player = __instance.gameData.mainPlayer;

            GS2.Log("Checking controller... " + (__instance.player.controller == null ? "Null" : "Exists"));
            __instance.controller = __instance.player.controller;
            if (__instance.player.controller == null) return GS2.AbortGameStart("Player controller failed to initialize. Probably an issue with galaxy generation.");

            GS2.Log("Checking localPlanet... " + (__instance.gameData.localPlanet == null ? "Null" : "Exists"));
            if (__instance.gameData.localPlanet == null)
            {
                if (GameMain.localPlanet != null)
                    __instance.gameData.localPlanet = GameMain.localPlanet;
                else
                {
                    __instance.gameData.ArrivePlanet(__instance.gameData.galaxy.PlanetById(__instance.gameData.galaxy.birthPlanetId));
                    if(__instance.gameData.localPlanet == null)
                    {
                        return GS2.AbortGameStart("Unable to find a habitable starting planet. If loading from a custon JSON, please check it for errors with an online tool.");
                    }
                }
            }

            __instance.localPlanet = __instance.gameData.localPlanet;
            GS2.Log("Checking birthPoint... " + (__instance.localPlanet.birthPoint == null ? "Null" : "Exists"));
            __instance.targetPos = __instance.localPlanet.birthPoint;
            __instance.targetUPos = __instance.localPlanet.uPosition + (VectorLF3)(__instance.localPlanet.runtimeRotation * __instance.targetPos);
            __instance.targetRot = Maths.SphericalRotation(__instance.localPlanet.birthPoint, 0.0f);
            __instance.targetURot = __instance.localPlanet.runtimeRotation * __instance.targetRot;
            __instance.localPlanet.factory.FlattenTerrain(__instance.targetPos, __instance.targetRot, new Bounds(Vector3.zero, new Vector3(10f, 5f, 10f)), removeVein: true, lift: true);
            GS2.Log("Waking in SpacePod");
            __instance.CreateSpaceCapsuleVegetable();
            GS2.Log("Searching for landing place");
            __instance.gameData.InitLandingPlace();
            __instance.player.controller.memCameraTargetRot = __instance.targetRot;
            __instance.player.cameraTarget.rotation = __instance.targetRot;
            // if (GS2.Config.CheatMode && !GS2.ResearchUnlocked)
            // {
            //     GS2.Warn("Cheatmode Enabled. Unlocking Research");
            //     GS2.UnlockTech(null);
            // }

            return false;
        }
    }
}