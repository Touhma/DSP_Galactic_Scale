using System;
using System.IO;
using GSSerializer;

namespace GalacticScale
{
    public static partial class GS3
    {
        public static bool Import(string LoadPath = "") // Load Settings from Save Game
        {
            // return true;
            Warn("Import");

            Log("Importing from Save.");
            if (!SaveOrLoadWindowOpen) GSSettings.Reset(0);
            if (SaveOrLoadWindowOpen) return true;
            if (LoadPath != "")
            {
                Warn($"*** Loading Settings From {LoadPath}");
                LoadSettingsFromJson(LoadPath);
                Warn($"StarCount : {GSSettings.StarCount}");
            }
            ActiveGenerator = GetGeneratorByID("space.customizing.generators.gs3");
            Warn($"Failed to find GS3 File in SaveDir");
            GSSettings.Instance.imported = true;
            return true;
        }
    }
}