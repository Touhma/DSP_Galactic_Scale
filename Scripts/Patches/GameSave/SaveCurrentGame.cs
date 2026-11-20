using System;
using System.IO;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameSave
    {
        private static readonly object SaveGate = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), "SaveCurrentGame")]
        public static void SaveCurrentGame(string saveName)
        {
            lock (SaveGate)
            {
                var path = GameConfig.gameSaveFolder + saveName + ".gs2";
                var backupPath = path + ".backup";
                var deletePath = backupPath + ".delete";
                string pendingPath = null;

                try
                {
                    if (!GS2.Vanilla)
                    {
                        pendingPath = UniqueSidecarPath(path + ".pending");
                        GS2.SaveSettingsToJson(pendingPath);
                    }

                    if (File.Exists(deletePath))
                    {
                        GS2.ShowMessage("There has been a problem with saving GS2 data previously. Please backup your saves!", "WARNING");
                        MoveSidecar(deletePath, UniqueSidecarPath(path + ".previousFailure"));
                    }

                    if (File.Exists(backupPath)) MoveSidecar(backupPath, deletePath);
                    if (File.Exists(path)) MoveSidecar(path, backupPath);
                    if (pendingPath != null) MoveSidecar(pendingPath, path);
                    if (File.Exists(deletePath)) File.Delete(deletePath);
                }
                catch (Exception exception)
                {
                    GS2.Warn($"Could not rotate Galactic Scale metadata for save '{saveName}' at '{path}': {exception}");
                }
            }
        }

        private static string UniqueSidecarPath(string basePath)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var candidate = basePath + "." + timestamp;
            var suffix = 1;
            while (File.Exists(candidate)) candidate = basePath + "." + timestamp + "." + suffix++;
            return candidate;
        }

        private static void MoveSidecar(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
                throw new IOException($"Refusing to overwrite existing Galactic Scale sidecar '{destinationPath}' while moving '{sourcePath}'.");

            File.Move(sourcePath, destinationPath);
        }
    }
}
