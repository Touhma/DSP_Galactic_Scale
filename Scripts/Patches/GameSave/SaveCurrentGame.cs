using HarmonyLib;
using System;
using System.IO;

namespace GalacticScale
{
    public partial class PatchOnGameSave
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), "SaveCurrentGame")]
        public static void SaveCurrentGame(string saveName)
{
            string path = GameConfig.gameSaveFolder + saveName + ".gs2";
            string backuppath = path + ".backup";
            string deletepath = backuppath + ".delete";
            if (File.Exists(deletepath))
            {
                GS2.ShowMessage("There has been a problem with saving GS2 data previously. Please backup your saves!","WARNING");
                File.Move(deletepath, path +".previousFailure." + DateTime.Now.ToString("dd"));
            }
            if (File.Exists(backuppath)) File.Move(backuppath, deletepath);
            if (File.Exists(path)) File.Move(path, backuppath);
            // if (saveName.Length < 9 || saveName.Substring(1,8) != "autosave" && saveName.Substring(1,8) != "lastexit")
            if (!GS2.Vanilla) GS2.SaveSettingsToJson(path);
            if (File.Exists(deletepath)) File.Delete(deletepath);
            
            
            // var path = Path.Combine(GS2.DataDir, "GalaxyBackups");
            // if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            // path = Path.Combine(path, saveName);
            // path = path+ "-" + DateTime.Now.ToString("dd") + ".json";
            // if (saveName.Length < 10) return;
        }
    }

}