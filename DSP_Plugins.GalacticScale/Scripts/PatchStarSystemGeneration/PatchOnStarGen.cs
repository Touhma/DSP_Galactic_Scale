using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(StarGen))]
    public class PatchOnStarGen {
        [HarmonyPrefix]
        [HarmonyPatch("CreateStarPlanets")]
        public static bool CreateStarPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc) {

            // InnerCount for the System
            //ReworkStarGen.CreateStarPlanetsRework(galaxy, star, gameDesc, new GeneratorPlanetSettings());

            //Patch.Debug("CreateStarPlanets");

            return true;
        }
    }
}