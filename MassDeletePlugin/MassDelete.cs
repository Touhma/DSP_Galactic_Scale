using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;


namespace DspPluginTest {
    [BepInPlugin("org.bepinex.plugins.masdelete", "MassDelete Plug-In", "1.0.0.0")]
    public class MassDelete : BaseUnityPlugin {
        void Awake() {
            var harmony = new Harmony("org.bepinex.plugins.masdelete");
            Harmony.CreateAndPatchAll(typeof(Patch));
        }

        [HarmonyPatch(typeof(PlayerAction_Build))]
        private class Patch {
            [HarmonyPostfix]
            [HarmonyPatch("PrepareBuild")]
            public static void Postfix(PlayerAction_Build __instance, ref Vector3[] ___reformPoints,
                ref PlayerController ___controller,
                ref bool ___multiLevelCovering,
                ref NearColliderLogic ___nearcdLogic,
                ref PlanetFactory ___factory, ref Vector3 ___reformChangedPoint,
                ref int[] ___tmp_conn) {
                if (___controller.cmd.mode == 4) {
                    if (VFInput.jump) {
                        int[] buildingIdsToDelete = new int[100];
                        foreach (var reformPoint in ___reformPoints) {
                            ___nearcdLogic.GetBuildingsInAreaNonAlloc(reformPoint, 1f, buildingIdsToDelete);
                            foreach (var i in buildingIdsToDelete) {
                                __instance.DoDestructObject(i);
                            }
                        }
                    }
                }
            }
        }
    }
}