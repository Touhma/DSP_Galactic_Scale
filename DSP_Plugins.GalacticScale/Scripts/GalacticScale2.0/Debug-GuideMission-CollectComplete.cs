//using HarmonyLib;
//using System.Collections.Generic;
//using UnityEngine;
//using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;
//namespace GalacticScale.Scripts.PatchStarSystemGeneration
//{
//    public static class PatchGuideMissionStandardMode
//    {
//        [HarmonyPrefix, HarmonyPatch(typeof(GuideMissionStandardMode), "Skip")]
//        public static bool Skip(GameData _gameData, ref GuideMissionStandardMode __instance)
//        {
//            Patch.Debug("Skip");
//            __instance.gameData = _gameData;
//            Patch.Debug("Skip1");
//            __instance.player = __instance.gameData.mainPlayer; Patch.Debug("Skip2");
//            __instance.controller = __instance.player.controller; Patch.Debug("Skip3");
//            __instance.localPlanet = __instance.gameData.localPlanet; Patch.Debug("Skip4");
//            Patch.Debug("__instance.gameData.localPlanet" + __instance.gameData.localPlanet == null);Patch.Debug("heh");
//            Patch.Debug("hoh" + __instance.localPlanet.birthPoint == null);
//            __instance.targetPos = __instance.localPlanet.birthPoint; Patch.Debug("Skip5");
//            __instance.targetUPos = __instance.localPlanet.uPosition + (VectorLF3)(__instance.localPlanet.runtimeRotation * __instance.targetPos); Patch.Debug("Skip6");
//            __instance.targetRot = Maths.SphericalRotation(__instance.localPlanet.birthPoint, 0.0f); Patch.Debug("Skip7");
//            __instance.targetURot = __instance.localPlanet.runtimeRotation * __instance.targetRot; Patch.Debug("Skip8");
//            __instance.localPlanet.factory.FlattenTerrain(__instance.targetPos, __instance.targetRot, new Bounds(Vector3.zero, new Vector3(10f, 5f, 10f)), removeVein: true, lift: true); Patch.Debug("Skip9");
//            __instance.CreateSpaceCapsuleVegetable(); Patch.Debug("Skip10");
//            __instance.gameData.InitLandingPlace(); Patch.Debug("Skip11");
//            __instance.player.controller.memCameraTargetRot = __instance.targetRot; Patch.Debug("Skip12");
//            __instance.player.cameraTarget.rotation = __instance.targetRot; Patch.Debug("Skip13");
//            return false;
//        }
//        [HarmonyPrefix, HarmonyPatch(typeof(GamePrefsData), "CollectComplete")]
//        public static bool CollectComplete(ref GamePrefsData __instance)
//        {
//            if (GameMain.instance.isMenuDemo)
//                return false;
//            GS2.Log("a");
//            __instance.Collect();
//            GS2.Log("b");
//            UIGame uiGame = UIRoot.instance.uiGame;
//            GS2.Log("c");
//            Dictionary<int, int> multipliers = uiGame.replicator.multipliers;
//            GS2.Log("d");
//            if (multipliers != null)
//            {
//                GS2.Log("e");
//                __instance.replicatorMultipliers.Clear();
//                GS2.Log("f");
//                foreach (KeyValuePair<int, int> keyValuePair in multipliers)
//                    __instance.replicatorMultipliers[keyValuePair.Key] = keyValuePair.Value;
//            }
//            GS2.Log("g");
//            __instance.tutorialShowing.Clear();
//            GS2.Log("h");
//            foreach (UITutorialTipEntry tutorialTipEntry in uiGame.tutorialTip.entryShowed)
//            {
//                GS2.Log("i");
//                if (tutorialTipEntry.tutorialId == tutorialTipEntry.functionId)
//                    __instance.tutorialShowing.Add(tutorialTipEntry.tutorialId);
//            }
//            GS2.Log("j");
//            __instance.astroNameOverride.Clear();
//            GS2.Log("k");
//            foreach (StarData star in __instance.gameData.galaxy.stars)
//            {
//                GS2.Log("l");
//                if (!string.IsNullOrEmpty(star.overrideName))
//                    __instance.astroNameOverride[star.id * 100] = star.overrideName;
//                GS2.Log(star.planets.Length.ToString());
//                foreach (PlanetData planet in star.planets)
//                {
//                    GS2.Log("m");
//                    if (!string.IsNullOrEmpty(planet.overrideName))
//                    {
//                        GS2.Log("Planet is " + planet.id);
//                        GS2.Log("Astronameoverride length is " + __instance.astroNameOverride.Count);
//                        __instance.astroNameOverride[planet.id] = planet.overrideName;
//                    }
//                }
//            }
//            GS2.Log("n");
//            __instance.detailPower = PowerSystemRenderer.powerGraphOn;
//            GS2.Log("o");
//            __instance.detailVein = uiGame.dfVeinOn;
//            GS2.Log("p");
//            __instance.detailSpaceGuide = uiGame.dfSpaceGuideOn;
//            GS2.Log("q");
//            __instance.detailSign = EntitySignRenderer.showSign;
//            GS2.Log("r");
//            __instance.detailIcon = EntitySignRenderer.showIcon;
//            GS2.Log("s");
//            return false;
//        }
//    }
//}