using HarmonyLib;
using System;
using System.Reflection;
using NebulaCompatibility;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnGuideMissionStandardMode
    {
        private static readonly MethodInfo FlattenTerrainMethod = AccessTools.Method(typeof(PlanetFactory), "FlattenTerrain");

        private static void FlattenBirthTerrain(PlanetFactory factory, Vector3 pos, Quaternion rot)
        {
            if (FlattenTerrainMethod == null)
                throw new MissingMethodException(nameof(PlanetFactory), "FlattenTerrain");

            var parameters = FlattenTerrainMethod.GetParameters();
            var bound = new Bounds(Vector3.zero, new Vector3(10f, 5f, 10f));
            var args = new object[] { pos, rot, bound, 6f, 1f, true, true, true, true, new Bounds() };

            // DSP 0.10.35 added a SkillTarget caster before removeVegeBound.
            if (parameters.Length == 11 && parameters[9].ParameterType.Name == "SkillTarget")
            {
                args = new object[] { pos, rot, bound, 6f, 1f, true, true, true, true,
                    Activator.CreateInstance(parameters[9].ParameterType), new Bounds() };
            }
            else if (parameters.Length != 10)
            {
                throw new MissingMethodException(nameof(PlanetFactory), "FlattenTerrain with a supported signature");
            }

            FlattenTerrainMethod.Invoke(factory, args);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuideMissionStandardMode), "Skip")]
        public static bool GS2_GuideMissionStandardMode_Skip_Prefix(GameData _gameData, ref GuideMissionStandardMode __instance)
        {
            //if (GS2.Vanilla) return true;
            if (GS2.IsMenuDemo) return true;
            if (NebulaCompat.IsClient) return false; // skip SpacePod if it is client joining
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
                {
                    __instance.gameData.localPlanet = GameMain.localPlanet;
                }
                else
                {
                    __instance.gameData.ArrivePlanet(__instance.gameData.galaxy.PlanetById(__instance.gameData.galaxy.birthPlanetId));
                    if (__instance.gameData.localPlanet == null) return GS2.AbortGameStart("Unable to find a habitable starting planet. If loading from a custom JSON, please check it for errors with an online tool.");
                }
            }

            __instance.localPlanet = __instance.gameData.localPlanet;
            GS2.Log("Checking birthPoint... " + (__instance.localPlanet.birthPoint == null ? "Null" : "Exists"));
            __instance.targetPos = __instance.localPlanet.birthPoint;
            __instance.targetUPos = __instance.localPlanet.uPosition + (VectorLF3)(__instance.localPlanet.runtimeRotation * __instance.targetPos);
            __instance.targetRot = Maths.SphericalRotation(__instance.localPlanet.birthPoint, 0.0f);
            __instance.targetURot = __instance.localPlanet.runtimeRotation * __instance.targetRot;
            if (__instance.localPlanet.factory != null)
            {
                FlattenBirthTerrain(__instance.localPlanet.factory, __instance.targetPos, __instance.targetRot);
                GS2.Log("Waking in SpacePod");
                __instance.CreateSpaceCapsuleVegetable();
                GS2.Log("Searching for landing place");
                __instance.gameData.InitLandingPlace();
            }
            Utils.LogDFInfo(GameMain.localStar);
            __instance.player.controller.memCameraTargetRot = __instance.targetRot;
            __instance.player.cameraTarget.rotation = __instance.targetRot;
            // if (GS2.Config.CheatMode && !GS2.ResearchUnlocked)
            // {
            //     GS2.Warn("Cheatmode Enabled. Unlocking Research");
            //     GS2.UnlockTech(null);
            // }
            if (GS2.Config.DevMode) GS2.debugtool = DebugTool.Init();
            return false;
        }
    }
}
