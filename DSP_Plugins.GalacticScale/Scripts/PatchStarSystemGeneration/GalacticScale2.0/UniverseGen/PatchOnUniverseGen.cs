using HarmonyLib;
using System.Collections.Generic;


namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            if (DSPGame.IsMenuDemo) return true;
            __result = GS2.CreateGalaxy(gameDesc);
            return false;
        }
    }
}
