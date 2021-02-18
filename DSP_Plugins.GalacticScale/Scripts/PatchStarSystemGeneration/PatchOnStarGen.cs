using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(StarGen))]
    public class PatchOnStarGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateStarPlanets")]
        public static bool CreateStarPlanets(ref GalaxyData galaxy,ref StarData star,ref GameDesc gameDesc) {
            if (Patch.EnableCustomStarAlgorithm) {
                ReworkStarGen.CreateStarPlanetsRework(ref galaxy, ref star, ref gameDesc);
                return false;
            }

            return true;
        }
    }
}