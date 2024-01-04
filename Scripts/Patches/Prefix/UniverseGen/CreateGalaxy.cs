using System.Collections.Generic;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UniverseGen), nameof(UniverseGen.CreateGalaxy))]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            GS3.gameDesc = gameDesc;
            if (DSPGame.IsMenuDemo) return true;
            __result = GS3.ProcessGalaxy(gameDesc, true);
            return false;
        }
    }
}