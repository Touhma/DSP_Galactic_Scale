using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(StarGen))]
    public class PatchOnStarGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateStarPlanets")]
        public static bool CreateStarPlanets(ref GalaxyData galaxy,ref StarData star,ref GameDesc gameDesc) {
            if (Patch.EnableCustomStarAlgorithm.Value) {
                // InnerCount for the System
                
                
                ReworkStarGen.CreateStarPlanetsRework(ref galaxy, ref star, ref gameDesc, new PlanetGeneratorSettings());
                return false;
            }

            return true;
        }
    }
}