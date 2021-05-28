using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnGuideMissionStandardMode
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GuideMissionStandardMode), "Skip")]
        public static bool GS2_GuideMissionStandardMode_Skip_Prefix(GameData _gameData, ref GuideMissionStandardMode __instance)
        {
            if (GS2.Failed)return false;
            GS2.Log("Checking gameData... " + ((_gameData == null)?"Null":"Exists"));
            __instance.gameData = _gameData;
            GS2.Log("Checking mainPlayer... " + ((__instance.gameData.mainPlayer == null) ? "Null" : "Exists"));
            __instance.player = __instance.gameData.mainPlayer;
            GS2.Log("Checking controller... " + ((__instance.player.controller == null) ? "Null" : "Exists"));
            __instance.controller = __instance.player.controller;
            GS2.Log("Checking localPlanet... " + ((__instance.gameData.localPlanet == null) ? "Null" : "Exists"));
            if (__instance.gameData.localPlanet == null)
            {
                GS2.Log("Aborting");
                GS2.Failed = true;
                UIRoot.instance.CloseLoadingUI();
                UIRoot.instance.CloseGameUI();
                UIRoot.instance.launchSplash.Restart();
                DSPGame.StartDemoGame(0);
                UIMessageBox.Show("Somewhat Fatal Error", "No habitable starting planet found", "Rats!", 3, new UIMessageBox.Response(() => {                    
                    UIRoot.instance.OpenMainMenuUI();
                    UIRoot.ClearFatalError();
                }));
                UIRoot.ClearFatalError();
                return false;
            } 
            __instance.localPlanet = __instance.gameData.localPlanet;
            GS2.Log("Checking birthPoint... " + ((__instance.localPlanet.birthPoint == null) ? "Null" : "Exists"));
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
            return false;
        }
    }
}
