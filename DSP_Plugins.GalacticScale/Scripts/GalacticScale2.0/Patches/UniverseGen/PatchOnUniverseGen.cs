using HarmonyLib;
using System.Collections.Generic;


namespace GalacticScale
{
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            GS2.gameDesc = gameDesc;
            if (DSPGame.IsMenuDemo) return true;
            if (GS2.Vanilla) return true;
            __result = GS2.CreateGalaxy(gameDesc);
            return false;
        }
    }
}
