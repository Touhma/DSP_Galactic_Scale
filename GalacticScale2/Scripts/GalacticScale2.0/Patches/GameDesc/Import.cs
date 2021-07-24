using System.IO;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameDesc
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameDesc), "Import")]
        public static void Import(BinaryReader r, ref GameDesc __instance)
        {
            if (!GS2.IsMenuDemo)
            {
                GS2.Log("Not Menu Demo. Importing");
                GS2.Import(r);
                GS2.Log("Unsetting Cheatmode");
                GS2.Log("Setting option");
                if (GS2.Config?.CheatMode != null) GS2.Config.DisableCheatMode();
                GS2.Log("Returning");
                return;
            }

            GS2.Log("Menu Demo: " + GS2.IsMenuDemo);
            GSSettings.Instance.imported = false;
        }
    }
}