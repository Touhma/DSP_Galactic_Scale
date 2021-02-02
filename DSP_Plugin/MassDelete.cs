using BepInEx;
using HarmonyLib;
using UnityEngine;


namespace DSP_Plugin {
    [BepInPlugin("org.bepinex.plugins.massdelete", "MassDelete Plug-In", "1.0.0.0")]
    public class MassDelete : BaseUnityPlugin {
        void Awake() {
            var harmony = new Harmony("org.bepinex.plugins.massdelete");
            Harmony.CreateAndPatchAll(typeof(Patch));
        }

        [HarmonyPatch(typeof(PlayerAction_Build))]
        private class Patch {
            [HarmonyPostfix]
            [HarmonyPatch("PrepareBuild")]
            public static void Postfix(PlayerAction_Build __instance, ref Vector3[] ___reformPoints,
                ref PlayerController ___controller, ref int ___altitude,
                ref bool ___multiLevelCovering,
                ref NearColliderLogic ___nearcdLogic,
                ref PlanetFactory ___factory,
                ref int[] ___tmp_conn) {
                UnityEngine.Debug.Log(__instance.reformSize/2);
                if (___controller.cmd.mode == 4) {
                    if (VFInput.jump) {
                        int[] buildingIdsToDelete = new int[100];
                        foreach (var reformPoint in ___reformPoints) {
                            
                            ___nearcdLogic.GetBuildingsInAreaNonAlloc( __instance.reformCenterPoint, (float )__instance.reformSize , buildingIdsToDelete);
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