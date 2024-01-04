using System.IO;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.ImportGamePropertyData))]
        public static bool ImportGamePropertyData(BinaryReader r, ref ClusterPropertyData __result)
        {
            GS3.Warn("Importing GamePropertyData");
            var num = r.ReadInt32();
            if (num >= 4) r.ReadInt32();
            var accountData = AccountData.NULL;
            if (num >= 3) accountData.Import(r);
            if (accountData.isNull) accountData = AccountData.me;
            r.ReadString();
            new GameDesc().Import(r);
            GS3.Import(r);
            GS3.Warn("Imported GS3 Data");
            r.ReadInt64();
            if (num >= 7)
            {
                GS3.Warn($"Loading AchievData {num}");
                var gameAchievementData = new GameAchievementData();
                gameAchievementData.Init();
                gameAchievementData.Import(r);
            }

            if (num >= 1)
            {
                GS3.Warn("LoadingPrefsData");
                new GamePrefsData().Import(r);
            }

            GS3.Warn("Loaded");
            var gameHistoryData = new GameHistoryData();
            gameHistoryData.Init(new GameData());
            GS3.Warn("Loading GameHistoryData");
            gameHistoryData.Import(r);
            __result = gameHistoryData.propertyData;
            return false;
        }
    }
}