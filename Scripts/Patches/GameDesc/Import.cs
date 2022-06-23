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
            GS2.Warn("Import");
            if (!GS2.IsMenuDemo)
            {
                var ForceFile = Path.Combine(GS2.DataDir, "ForceImport.json");
                var saveName = DSPGame.LoadFile;
                var GSSave = GameConfig.gameSaveFolder + saveName + ".gs2";
                GS2.Log("Not Menu Demo. Importing");
                if (VFInput.shift && File.Exists(ForceFile))
                {
                    GS2.Warn("LOADING GALAXY DESC FROM ForceImport.json");
                    GS2.Import(r, ForceFile);
                }
                else if (File.Exists(GSSave))
                {
                    GS2.Log("Loading GS2.5+ Save Data");
                    GS2.Import(r, GSSave);
                }
                else
                {
                    GS2.Warn("Loading save from GS version < 2.5");
                    GS2.Import(r);
                }

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