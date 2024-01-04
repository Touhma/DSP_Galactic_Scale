using System.IO;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public static partial class PatchOnGameDesc
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameDesc), nameof(GameDesc.Import))]
        public static void Import(BinaryReader r, ref GameDesc __instance)
        {
            GS3.Warn("Import");
            if (!GS3.IsMenuDemo)
            {
                var ForceFile = Path.Combine(GS3.DataDir, "ForceImport.json");
                var saveName = DSPGame.LoadFile;
                var GSSave = GameConfig.gameSaveFolder + saveName + ".gs3";
                GS3.Warn("Not Menu Demo. Importing");
                if (VFInput.shift && File.Exists(ForceFile))
                {
                    GS3.Warn("LOADING GALAXY DESC FROM ForceImport.json");
                    GS3.Import(r, ForceFile);
                }
                else if (File.Exists(GSSave))
                {
                    GS3.Log("Loading GS3 Save Data");
                    GS3.Import(r, GSSave);
                }
 
                GS3.Log("Unsetting Cheatmode");
                GS3.Log("Setting option");
                if (GS3.Config?.CheatMode != null) GS3.Config.DisableCheatMode();
                GS3.Log("Returning");
                return;
            }

            GS3.Warn("Menu Demo: " + GS3.IsMenuDemo);
            GSSettings.Instance.imported = false;
        }
    }
}