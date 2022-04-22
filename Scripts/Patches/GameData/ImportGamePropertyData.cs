using System.IO;
using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameData), "ImportGamePropertyData")]
        public static bool ImportGamePropertyData(BinaryReader r, ref ClusterPropertyData __result)
        {
            GS2.Warn("Importing GamePropertyData");
            int num = r.ReadInt32();
            if (num >= 4)
            {
                r.ReadInt32();
            }
            AccountData accountData = AccountData.NULL;
            if (num >= 3)
            {
                accountData.Import(r);
            }
            if (accountData.isNull)
            {
                accountData = AccountData.me;
            }
            r.ReadString();
            new GameDesc().Import(r);
            GS2.Import(r);
            GS2.Warn("Imported GS2 Data");
            r.ReadInt64();
            if (num >= 7)
            {
                GS2.Warn($"Loading AchievData { num}");
                GameAchievementData gameAchievementData = new GameAchievementData();
                gameAchievementData.Init();
                gameAchievementData.Import(r);
            }
            if (num >= 1)
            {
                GS2.Warn("LoadingPrefsData");
                new GamePrefsData().Import(r);
            }
            GameHistoryData gameHistoryData = new GameHistoryData();
            gameHistoryData.Init(new GameData());
            GS2.Warn("Loading GameHistoryData");
            gameHistoryData.Import(r);
            __result = gameHistoryData.propertyData;
            return false;
        }
    }
}