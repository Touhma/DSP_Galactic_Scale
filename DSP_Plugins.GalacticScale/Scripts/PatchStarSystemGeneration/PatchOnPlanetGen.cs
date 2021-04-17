﻿using System.Collections.Generic;
using HarmonyLib;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    [HarmonyPatch(typeof(PlanetGen))]
    public class PatchOnPlanetGen {
        /*
                 * PlanetGen.CreatePlanet(galaxy, star, gameDesc, i, 0, 3, 1, false, info_seed, gen_seed)
                 * galaxy , star, gameDesc : internal data
                 * index : index of the planet
                 * orbit around : if it's a moon then decide around what index it's orbiting around if it's a planet : put 0 --> 0 is the index of the star
                 * orbit index : the index of the orbit chosen, here it's the index in the _orbitRadiusArrayPlanetList
                 * number : index on the local system. if it's a moon with number 1 : it's the first moon around the planet on orbitIndex  
                 * gasGiant : is it a gas Giant ?
                 * info_seed, gen_seed : internal random numbers
                 */

        [HarmonyPrefix]
        [HarmonyPatch("CreatePlanet")]
        public static bool CreatePlanet(
            GalaxyData galaxy,
            StarData star,
            GameDesc gameDesc,
            int index,
            int orbitAround,
            int orbitIndex,
            int number,
            bool gasGiant,
            int info_seed,
            int gen_seed,
            ref PlanetData __result) {
            //Debug.Log("CreatePlanet -----");
            if (!DSPGame.IsMenuDemo)
            {
                __result = ReworkPlanetGen.ReworkCreatePlanet(ref galaxy, ref star, ref gameDesc, index, orbitAround, orbitIndex, number, gasGiant, info_seed, gen_seed);
                //Debug.Log("__result.index" + __result.id);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetPlanetTheme")]
        public static bool SetPlanetTheme(
            ref List<int> ___tmp_theme,
            PlanetData planet,
            StarData star,
            GameDesc game_desc,
            int set_theme,
            int set_algo,
            double rand1,
            double rand2,
            double rand3,
            double rand4,
            int theme_seed) {
            if (planet.type == EPlanetType.Gas) {
                ReworkSetPlanetTheme.SetPlanetTheme(ref planet,ref star, theme_seed);
                return false;
            }
            return true;

        }
    }
}