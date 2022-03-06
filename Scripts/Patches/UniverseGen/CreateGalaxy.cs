using System.Collections.Generic;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UniverseGen), "CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            GS2.gameDesc = gameDesc;
            // GS2.LogJson(gameDesc);
            if (DSPGame.IsMenuDemo) return true;

            if (GS2.Vanilla) return true;

            // GS2.Warn("Create Galaxy");
            // GS2.ResearchUnlocked = false;
            __result = GS2.ProcessGalaxy(gameDesc, true);
            return false;
        }
    }
}