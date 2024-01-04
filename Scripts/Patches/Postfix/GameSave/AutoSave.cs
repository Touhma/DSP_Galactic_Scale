using System;
using System.IO;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameSave
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), nameof(GameSave.AutoSave))]
        public static void AutoSave()
        {
            var text = GameConfig.gameSaveFolder + GameSave.AutoSaveTmp + ".gs3";
            var text2 = GameConfig.gameSaveFolder + GameSave.AutoSave0 + ".gs3";
            var text3 = GameConfig.gameSaveFolder + GameSave.AutoSave1 + ".gs3";
            var text4 = GameConfig.gameSaveFolder + GameSave.AutoSave2 + ".gs3";
            var text5 = GameConfig.gameSaveFolder + GameSave.AutoSave3 + ".gs3";
            try
            {
                if (File.Exists(text))
                {
                    if (File.Exists(text5)) File.Delete(text5);
                    if (File.Exists(text4)) File.Move(text4, text5);
                    if (File.Exists(text3)) File.Move(text3, text4);
                    if (File.Exists(text2)) File.Move(text2, text3);
                    File.Move(text, text2);
                }
            }
            catch(Exception e)
            {
                GS3.Warn($"The write operation could not be performed because the file is locked. {e.GetType().Name}");
            }
        }
    }
}