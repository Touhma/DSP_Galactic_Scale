using System;
using System.IO;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameSave
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), nameof(GameSave.SaveCurrentGame))]
        public static void SaveCurrentGame(string saveName)
        {
            var path = GameConfig.gameSaveFolder + saveName + ".gs3";
            var backuppath = path + ".backup";
            var deletepath = backuppath + ".delete";
            if (File.Exists(deletepath))
            {
                GS3.ShowMessage("There has been a problem with saving GS3 data previously. Please backup your saves!", "WARNING");
                File.Move(deletepath, path + ".previousFailure." + DateTime.Now.ToString("dd"));
            }

            if (File.Exists(backuppath)) File.Move(backuppath, deletepath);
            if (File.Exists(path)) File.Move(path, backuppath);
            // if (saveName.Length < 9 || saveName.Substring(1,8) != "autosave" && saveName.Substring(1,8) != "lastexit")
            GS3.SaveSettingsToJson(path);
            if (File.Exists(deletepath)) File.Delete(deletepath);


            // var path = Path.Combine(GS3.DataDir, "GalaxyBackups");
            // if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            // path = Path.Combine(path, saveName);
            // path = path+ "-" + DateTime.Now.ToString("dd") + ".json";
            // if (saveName.Length < 10) return;
        }
    }
}