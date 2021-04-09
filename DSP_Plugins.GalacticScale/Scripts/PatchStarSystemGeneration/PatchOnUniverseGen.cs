using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, GalaxyData __result) {
           
                // InnerCount for the System
                __result = ReworkUniverseGen.ReworkCreateGalaxy(gameDesc);
  


            return true;
        }
    }
}